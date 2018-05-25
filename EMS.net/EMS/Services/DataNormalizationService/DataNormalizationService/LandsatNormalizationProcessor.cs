using System;
using System.IO;
using System.Threading.Tasks;
using OSGeo.GDAL;

namespace DataNormalizationService
{
    public class LandsatNormalizationProcessor
    {
        #region Private fields

        private readonly string _extension = ".l8n";

        #endregion

        #region Public methods

        /// <summary>
        /// Нормализация снимка Landsat
        /// </summary>
        /// <param name="fileName">Наименнование файла</param>
        /// <param name="folder">Папка</param>
        /// <param name="ml">RADIANCE_MULT_BAND_X</param>
        /// <param name="al">RADIANCE_ADD_BAND_X</param>
        /// <param name="sunElevation">SUN_ELEVATION</param>
        /// <param name="d">EARTH_SUN_DISTANCE</param>
        /// <param name="radianceMax">RADIANCE_MAXIMUM_BAND_X</param>
        /// <param name="reflectanceMax">REFLECTANCE_MAXIMUM_BAND_X</param>
        public void Normalization(string fileName, double ml, double al, double sunElevation, double d, double radianceMax, double reflectanceMax)
        {
            // Если файл уже существует не делать нормализацию
            var normalizedFileName = $"{fileName}{_extension}";
            if (File.Exists(normalizedFileName))
            {
                return;
            }

            var channel = Gdal.Open(fileName, Access.GA_ReadOnly);

            using (var currentBand = channel.GetRasterBand(1))
            {
                var width = currentBand.XSize;
                var heigth = currentBand.YSize;

                //DNmin - значения яркости пикселя 1% тёмного объекта;
                double dnmin = GetDnmin(currentBand, width, heigth);

                //lmin спектральная радиация для 1% тёмного объекта
                double lmin = ml * dnmin + al;

                //tetta - зенитное расстояние для солнца в радианах
                double tetta = (Math.PI * (90 - sunElevation)) / 180.0;

                //esun вычисленный коэффициент солнечного внеатмосферного спектрального излучения
                double esun = (Math.PI * Math.Pow(d, 2) * radianceMax) / reflectanceMax;

                //l1 коэффициент влияния угла падения и отражения солнечных лучей для 1% тёмного объекта
                double l1 = (0.01 * Math.Pow(Math.Cos(tetta), 3) * esun) / (Math.PI * Math.Pow(d, 2));

                //lhazing значение атмосферной дымки (hazing)
                double lhazing = lmin - l1;

                using (var file = new FileStream(normalizedFileName, FileMode.Create))
                using (var writer = new BinaryWriter(file))
                {
                    writer.Write(width);
                    writer.Write(heigth);

                    Foreach(currentBand, width, heigth, value =>
                    {
                            //Lλ — спектральная радиация, пришедшая на сенсор спутника;
                            double l = ml * value + al;

                            //ρλ — атмоферно скорректированные значения отражённой солнечной радиации
                            double pl = (Math.PI * (l - lhazing) * Math.Pow(d, 2)) / (esun * Math.Pow(Math.Cos(tetta), 2));

                            writer.Write(pl);
                    });

                    file.Flush(true);
                }
            }
        }

        #endregion

        #region Private methods

        private double GetDnmin(Band band, int width, int heigth)
        {
            long dataAll = 0;
            Foreach(band, width, heigth, value => { dataAll += value; });
            double data0001 = 0.0001 * dataAll;

            double dnmin = 0;
            var step = 1000;
            var i = true;
            while (i)
            {
                long dnsum = 0;
                Foreach(band, width, heigth, value =>
                {
                    if (value <= dnmin && value > 0)
                    {
                        dnsum += value;
                    }
                });

                if (dnsum > data0001)
                {
                    dnmin -= step;
                    if (step != 1)
                    {
                        step /= 10;
                    }
                    else
                    {
                        i = false;
                    }
                }
                else
                {
                    dnmin += step;
                }
            }

            return dnmin;
        }

        /// <summary>
        /// Цикл по всем точкам
        /// </summary>
        /// <param name="band"></param>
        /// <param name="width"></param>
        /// <param name="heigth"></param>
        /// <param name="action"></param>
        private void Foreach(Band band, int width, int heigth, Action<int> action)
        {
            var buffer = new int[width];

            for (var x = 0; x < heigth; x++)
            {
                band.ReadRaster(0, x, band.XSize, 1, buffer, band.XSize, 1, 0, 0);
                for (var y = 0; y < width; y++)
                {
                    action(buffer[y]);
                }
            }
        }

        #endregion
    }
}
