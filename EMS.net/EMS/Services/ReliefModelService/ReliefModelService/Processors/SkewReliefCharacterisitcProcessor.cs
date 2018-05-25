using System;
using System.Drawing;
using System.Drawing.Imaging;
using BusContracts;
using Common.Enums;
using ReliefModelService.Abstraction;
using ReliefModelService.Objects;

namespace ReliefModelService.Processors
{
    public class SkewReliefCharacterisitcProcessor : AbstractReliefCharacteristicProcessor
    {
        public override IReliefCharacteristicProduct Process(SrtmDataset dataset, string folder)
        {
            var legend = new Bitmap(@"..\..\Content\skew.png");
            var filePath = $@"{folder}skew.tif";
            using (var result = CreateImage(dataset, legend))
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
                            var skew = GetSkew(a.Value, b.Value, c.Value, d.Value, f.Value, g.Value, h.Value, i.Value);
                            color = GetColor(skew);
                        }
                        else
                        {
                            color = Color.Black;
                        }

                        result.SetPixel(x, y, color);
                    }
                }

                SetLegend(result, legend);
                legend.Dispose();
                result.Save(filePath, ImageFormat.Tiff);
            }

            return new ReliefCharacteristicProduct
            {
                FilePath = filePath,
                Type = ReliefCharacteristicType.Height
            };
        }

        private Color GetColor(double skew)
        {
            if (skew >= 0 && skew < 5)
            {
                return Color.FromArgb(230, 235, 151);
            }

            if (skew >= 5 && skew < 8)
            {
                return Color.FromArgb(210, 222, 138);
            }

            if (skew >= 8 && skew < 10)
            {
                return Color.FromArgb(173, 187, 136);
            }

            if (skew >= 10 && skew < 15)
            {
                return Color.FromArgb(172, 183, 114);
            }

            if (skew >= 15 && skew < 20)
            {
                return Color.FromArgb(125, 133, 182);
            }

            if (skew >= 20 && skew < 25)
            {
                return Color.FromArgb(127, 106, 162);
            }

            if (skew >= 25 && skew < 30)
            {
                return Color.FromArgb(3, 132, 187);
            }

            if (skew >= 30 && skew < 35)
            {
                return Color.FromArgb(253, 195, 33);
            }

            if (skew >= 35 && skew < 40)
            {
                return Color.FromArgb(205, 168, 100);
            }

            if (skew >= 40 && skew < 45)
            {
                return Color.FromArgb(196, 137, 95);
            }

            if (skew >= 45)
            {
                return Color.FromArgb(247, 131, 74);
            }

            return Color.White;
        }

        private double GetSkew(double a, double b, double c, double d, double f, double g, double h, double i)
        {
            var dzdx = ((a + 2 * d + g) - (c + 2 * f + i)) / 8;
            var dzdy = ((a + 2 * b + c) - (g + 2 * h + i)) / 8;

            return Math.Atan(Math.Sqrt(Math.Pow(dzdx, 2) + Math.Pow(dzdy, 2))) * (180 / Math.PI);
        }
    }
}
