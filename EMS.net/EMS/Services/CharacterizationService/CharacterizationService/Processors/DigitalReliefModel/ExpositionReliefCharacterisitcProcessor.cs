using System;
using System.Drawing;
using System.Drawing.Imaging;
using CharacterizationService.Abstraction;
using CharacterizationService.Objects.DigitalReliefModel;
using DrawImageLibrary;

namespace CharacterizationService.Processors.DigitalReliefModel
{
    public class ExpositionReliefCharacterisitcProcessor : AbstractReliefCharacteristicProcessor
    {
        public override string Process(SrtmDataset dataset, string folder)
        {
            var filePath = $@"{folder}\exposition.png";
            using (var result = DrawLib.CreateImageWithLegend(dataset.Width, dataset.Heigth, @"..\..\Content\exposition.png"))
            {
                for (var x = 1; x < dataset.Width - 1; x++)
                {
                    for (var y = 1; y < dataset.Heigth - 1; y++)
                    {
                        var a = (double?)dataset.Values[x - 1, y - 1];
                        var b = (double?)dataset.Values[x, y - 1];
                        var c = (double?)dataset.Values[x + 1, y - 1];
                        var d = (double?)dataset.Values[x - 1, y];
                        var f = (double?)dataset.Values[x + 1, y];
                        var g = (double?)dataset.Values[x - 1, y + 1];
                        var h = (double?)dataset.Values[x, y + 1];
                        var i = (double?)dataset.Values[x + 1, y + 1];

                        Color color;

                        if (a.HasValue && b.HasValue && c.HasValue && d.HasValue && f.HasValue && g.HasValue && h.HasValue && i.HasValue)
                        {
                            var aspect = GetAspect(a.Value, b.Value, c.Value, d.Value, f.Value, g.Value, h.Value, i.Value);
                            color = GetColor(aspect);
                        }
                        else
                        {
                            color = Color.Black;
                        }

                        result.SetPixel(x, y, color);
                    }
                }

                result.Save(filePath, ImageFormat.Png);
            }

            return filePath;
        }

        private Color GetColor(double aspect)
        {
            if (aspect == -1)
            {
                return Color.Gray;
            }
            if (aspect >= 337.5 || aspect < 22.5)
            {
                return Color.Red;
            }
            if (aspect >= 22.5 && aspect < 67.5)
            {
                return Color.Orange;
            }
            if (aspect >= 67.5 && aspect < 112.5)
            {
                return Color.Yellow;
            }
            if (aspect >= 112.5 && aspect < 157.5)
            {
                return Color.Green;
            }
            if (aspect >= 157.5 && aspect < 202.5)
            {
                return Color.Aqua;
            }
            if (aspect >= 202.5 && aspect < 247.5)
            {
                return Color.CornflowerBlue;
            }
            if (aspect >= 247.5 && aspect < 292.5)
            {
                return Color.Blue;
            }
            if (aspect >= 292.5 && aspect < 337.5)
            {
                return Color.Violet;
            }

            return Color.Gray;
        }

        /// <summary>
        /// Получить направление склона
        /// </summary>
        /// <returns></returns>
        private double GetAspect(double a, double b, double c, double d, double f, double g, double h, double i)
        {
            var dzdx = ((c + 2 * f + i) - (a + 2 * d + g)) / 8;
            var dzdy = ((g + 2 * h + i) - (a + 2 * b + c)) / 8;

            if (dzdy == 0 && dzdx == 0)
            {
                return -1;
            }

            var aspect = (Math.Atan2(dzdy, -dzdx) * 180) / Math.PI;

            double angle;
            if (aspect < 0)
            {
                angle = 90.0 - aspect;
            }
            else if (aspect > 90.0)
            {
                angle = 360.0 - aspect + 90.0;
            }
            else
            {
                angle = 90.0 - aspect;
            }
            return angle;
        }
    }
}
