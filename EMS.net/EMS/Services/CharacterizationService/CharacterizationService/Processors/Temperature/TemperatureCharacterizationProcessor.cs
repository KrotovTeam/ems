using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CharacterizationService.Abstraction;
using Common.Helpers;
using Common.Objects;
using Common.Objects.Landsat;
using DeterminingPhenomenonService.Helpers;
using OSGeo.GDAL;
using Topshelf.Logging;

namespace CharacterizationService.Processors.Temperature
{
    public class TemperatureCharacterizationProcessor : AbstractCharacterizationProcessor
    {
        public TemperatureCharacterizationProcessor(LogWriter logger) : base(logger)
        {
        }

        public override string[] Process(IGeographicPoint leftUpper, IGeographicPoint rigthLower, string dataFolder,
            string resultFolder)
        {
            var folderDescription = new LandsatDataDescription(@"C:\Users\User\Downloads\Карпаты2\185026_20160826\");

            var path = @"C:\Users\User\Downloads\Карпаты2\185026_20160826\";
            LandsatMetadata metadataFile = JsonHelper.Deserialize<LandsatMetadata>(folderDescription.MetadataMtlJson);
            TirsThermalConstants thermalConstants = metadataFile.L1MetadataFile.TirsThermalConstants;

            using (var ds = Gdal.Open(path + "2016_Temperature.TIF", Access.GA_ReadOnly))
            {
                CalculateTemperature(ds.GetRasterBand(1),
                    metadataFile.L1MetadataFile.RadiometricRescaling.RadianceMultBand11,
                    metadataFile.L1MetadataFile.RadiometricRescaling.RadianceAddBand11,
                    thermalConstants.K1ConstantBand11,
                    thermalConstants.K2ConstantBand11
                );
            }

            return new[] {"temperature_2016.png"};
        }

        public static void CalculateTemperature(Band band, double ml, double al, double K1, double K2)
        {
            var resultFilename = "karpati_2016.png";
            var temperatureRanges = new List<Legend.Range>
            {
                //new Legend.Range(-30, -25, Color.FromArgb(60, 0, 255)),
                //new Legend.Range(-25, -20, Color.FromArgb(0, 0, 255)),
                //new Legend.Range(-20, -15, Color.FromArgb(0, 80, 255)),
                //new Legend.Range(-15, -10, Color.FromArgb(0, 150, 255)),
                //new Legend.Range(-10, -5, Color.FromArgb(150, 255, 255)),
                //new Legend.Range(-5, 0, Color.FromArgb(255, 255, 255)),
                //new Legend.Range(0, 5, Color.FromArgb(255, 255, 150)),
                //new Legend.Range(5, 10, Color.FromArgb(255, 250, 50)),
                //new Legend.Range(10, 15, Color.FromArgb(255, 180, 50)),
                //new Legend.Range(15, 20, Color.FromArgb(255, 100, 30)),
                //new Legend.Range(20, 25, Color.FromArgb(255, 80, 0)),
                //new Legend.Range(25, 30, Color.FromArgb(255, 55, 0)),
                //new Legend.Range(30, 35, Color.FromArgb(255, 45, 0)),
                //new Legend.Range(35, 40, Color.FromArgb(255, 35, 0)),
                //new Legend.Range(40, 45, Color.FromArgb(255, 0, 0))
                //new Legend.Range(20, 25, Color.FromArgb(255, 80, 0)),
                //new Legend.Range(25, 30, Color.FromArgb(255, 55, 0)),
                //new Legend.Range(30, 35, Color.FromArgb(255, 45, 0)),
                //new Legend.Range(35, 40, Color.FromArgb(255, 35, 0)),
                //new Legend.Range(40, 45, Color.FromArgb(255, 0, 0))
            };
            using (band)
            {
                var width = band.XSize;
                var heigth = band.YSize;
                var legend = new Legend(5, 45, 5, Color.Yellow, Color.Red);
                double max = -100000;
                double min = 100000;
                legend.GetLegend().Save("legend.png");
                using (var bmp = new Bitmap(width, heigth))
                {
                    for (var row = 0; row < heigth; row++)
                    {
                        var buffer = new int[width];

                        band.ReadRaster(0, row, band.XSize, 1, buffer, band.XSize, 1, 0, 0);
                        for (var col = 0; col < width; col++)
                        {
                            if (buffer[col] != 0)
                            {
                                var rad = buffer[col] * ml + al;
                                
                                var temp = (K2 / Math.Log((K1 / rad) + 1)) - 273.15;

                                max = temp > max ? temp : max;
                                min = temp < min ? temp : min;

                                bmp.SetPixel(col, row, legend.GetColor(temp));
                            }
                            else
                            {
                                bmp.SetPixel(col, row, Color.Black);
                            }
                        }
                    }

                    var kek = 0;
                    bmp.Save(resultFilename);
                    legend.GetLegend().Save("legent_2016_karpati.png");
                }
            }
        }
    }
}