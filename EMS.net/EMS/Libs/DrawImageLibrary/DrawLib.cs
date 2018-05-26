using System;
using System.Drawing;
using System.Drawing.Imaging;
using Common.Objects;
using Common.PointsReaders;

namespace DrawImageLibrary
{
    public static class DrawLib
    {

        /// <summary>
        /// Создать изображение с белым фоном
        /// </summary>
        /// <param name="width"></param>
        /// <param name="heigth"></param>
        /// <param name="fileName"></param>
        public static void CreateImage(int width, int heigth, string fileName)
        {
            using (var bitmap = new Bitmap(width, heigth))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.FillRectangle(Brushes.White, 0, 0, width, heigth);
                bitmap.Save(fileName, ImageFormat.Png);
            }
        }

        /// <summary>
        /// Отрисовка маски
        /// </summary>
        /// <param name="maskFileName"></param>
        /// <param name="resultFileName"></param>
        public static void DrawMask(string maskFileName, string resultFileName)
        {
            using (var mask = new Bitmap(maskFileName))
            using (var result = new Bitmap(mask.Width, mask.Height))
            {
                for (var row = 0; row < mask.Height; row++)
                {
                    for (var col = 0; col < mask.Width; col++)
                    {
                        if (mask.GetPixel(col, row).R > 0)
                        {
                            result.SetPixel(col, row, Color.Red);
                        }
                    }
                }

                result.Save(resultFileName);
            }
        }

        /// <summary>
        /// Отрисовать картинку в видимом диапозоне
        /// </summary>
        /// <param name="redChannelFileName"></param>
        /// <param name="greenChannelFileName"></param>
        /// <param name="blueChannelFileName"></param>
        /// <param name="imageInfo"></param>
        /// <param name="resultFileName"></param>
        public static void DrawNaturalColor(string redChannelFileName, string greenChannelFileName, string blueChannelFileName, CuttedImageInfo imageInfo, string resultFileName)
        {
            var redChannel = new LandsatNormilizedSnapshotReader(redChannelFileName);
            var greenChannel = new LandsatNormilizedSnapshotReader(greenChannelFileName);
            var blueChannel = new LandsatNormilizedSnapshotReader(blueChannelFileName);

            //Коэф. для увеличения яркости
            var factor = 3.0f;

            using (var bitmap = new Bitmap(imageInfo.Width, imageInfo.Height))
            {
                for (var row = imageInfo.Row; row < imageInfo.Height; row++)
                {
                    var redBuffer = redChannel.ReadScanline(row);
                    var greenBuffer = greenChannel.ReadScanline(row);
                    var blueBuffer = blueChannel.ReadScanline(row);
                    for (var col = imageInfo.Col; col < imageInfo.Width; col++)
                    {
                        var redValue = CalculateValue(redBuffer[col], factor);
                        var greenValue = CalculateValue(greenBuffer[col], factor);
                        var blueValue = CalculateValue(blueBuffer[col], factor);
                        var color = Color.FromArgb(redValue, greenValue, blueValue);

                        bitmap.SetPixel(col, row, color);
                    }
                }

                bitmap.Save(resultFileName, ImageFormat.Png);
            }
        }

        /// <summary>
        /// Рассчитать значение цвета для знанечения отражения
        /// </summary>
        /// <param name="val">Значение отражения</param>
        /// <param name="factor">Параметр увеличения яркости</param>
        /// <returns></returns>
        private static byte CalculateValue(double val, float factor)
        {
            var result = Math.Abs(val) * 255 * factor;
            if (result > 255)
            {
                result = 255;
            }
            return (byte)result;
        }
    }
}
