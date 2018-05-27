using System.Drawing;
using System.Drawing.Imaging;
using CharacterizationService.Abstraction;
using CharacterizationService.Objects.DigitalReliefModel;
using DrawImageLibrary;

namespace CharacterizationService.Processors.DigitalReliefModel
{
    public class HeigthReliefCharacterisitcProcessor : AbstractReliefCharacteristicProcessor
    {
        public override string Process(SrtmDataset dataset, string folder)
        {
            var filePath = $@"{folder}\height.png";
            using (var result = DrawLib.CreateImageWithLegend(dataset.Width, dataset.Heigth, @"..\..\Content\heigth.png"))
            {
                for (var x = 1; x < dataset.Width - 1; x++)
                {
                    for (var y = 1; y < dataset.Heigth - 1; y++)
                    {
                        var value = dataset.Values[x, y];
                        result.SetPixel(x, y, GetColor(value));
                    }
                }

                result.Save(filePath, ImageFormat.Png);
            }

            return filePath;
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
