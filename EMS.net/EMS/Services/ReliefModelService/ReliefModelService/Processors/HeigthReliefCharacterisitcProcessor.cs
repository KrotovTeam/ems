using System.Drawing;
using System.Drawing.Imaging;
using BusContracts;
using Common.Enums;
using ReliefModelService.Abstraction;
using ReliefModelService.Objects;

namespace ReliefModelService.Processors
{
    public class HeigthReliefCharacterisitcProcessor : AbstractReliefCharacteristicProcessor
    {
        public override IReliefCharacteristicProduct Process(SrtmDataset dataset, string folder)
        {
            var legend = new Bitmap(@"..\..\Content\heigth.png");
            var filePath = $@"{folder}height.tif";
            using (var result = CreateImage(dataset, legend))
            {
                for (var x = 1; x < dataset.Width - 1; x++)
                {
                    for (var y = 1; y < dataset.Heigth - 1; y++)
                    {
                        var value = dataset.Values[x, y];
                        result.SetPixel(x, y, GetColor(value));
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

        private Color GetColor(short? value)
        {
            if (value.HasValue)
            {
                if (value.Value <= 0)
                {
                    return Color.FromArgb(3, 132, 187);
                }

                if (value.Value > 0 && value.Value < 50)
                {
                    return Color.FromArgb(7, 127, 7);
                }

                if (value.Value >= 50 && value.Value < 120)
                {
                    return Color.FromArgb(7, 175, 7);
                }

                if (value.Value >= 120 && value.Value < 260)
                {
                    return Color.FromArgb(143, 215, 63);
                }

                if (value.Value >= 260 && value.Value < 570)
                {
                    return Color.FromArgb(255, 255, 63);
                }

                if (value.Value >= 570 && value.Value < 1250)
                {
                    return Color.FromArgb(255, 171, 63);
                }

                if (value.Value >= 1250 && value.Value < 2670)
                {
                    return Color.FromArgb(255, 123, 63);
                }

                if (value.Value >= 2670)
                {
                    return Color.FromArgb(223, 63, 63);
                }
            }
            return Color.Black;
        }
    }
}
