﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using AForge.Imaging.Filters;
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
        public static void DrawMask(string maskFileName, string backgroungFilename, string resultFileName)
        {
            using (var mask = new Bitmap(maskFileName))
            using (var background = new Bitmap(backgroungFilename))
            using (var result = new Bitmap(background.Width, background.Height))
            {
                for (var row = 0; row < mask.Height; row++)
                {
                    for (var col = 0; col < mask.Width; col++)
                    {
                        result.SetPixel(col, row,
                            mask.GetPixel(col, row).R > 0 
                                ? Color.Red 
                                : background.GetPixel(col, row));
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
        public static void DrawNaturalColor(string redChannelFileName, string greenChannelFileName,
            string blueChannelFileName, CuttedImageInfo imageInfo, string resultFileName)
        {
            var redChannel = new LandsatNormilizedSnapshotReader(redChannelFileName);
            var greenChannel = new LandsatNormilizedSnapshotReader(greenChannelFileName);
            var blueChannel = new LandsatNormilizedSnapshotReader(blueChannelFileName);

            //Коэф. для увеличения яркости
            var factor = 3.0f;

            int x = 0, y = 0;
            using (var bitmap = new Bitmap(imageInfo.Width, imageInfo.Height))
            {
                for (var row = imageInfo.Row; row < imageInfo.Row + imageInfo.Height; row++)
                {
                    var redBuffer = redChannel.ReadScanline(row);
                    var greenBuffer = greenChannel.ReadScanline(row);
                    var blueBuffer = blueChannel.ReadScanline(row);
                    for (var col = imageInfo.Col; col < imageInfo.Col + imageInfo.Width; col++)
                    {
                        var redValue = CalculateValue(redBuffer[col], factor);
                        var greenValue = CalculateValue(greenBuffer[col], factor);
                        var blueValue = CalculateValue(blueBuffer[col], factor);
                        var color = Color.FromArgb(redValue, greenValue, blueValue);

                        bitmap.SetPixel(y, x, color);

                        y++;
                    }

                    y = 0;
                    x++;
                }

                bitmap.Save(resultFileName, ImageFormat.Png);
            }
        }

        /// <summary>
        /// Отобразить контуры для входного изображения
        /// </summary>
        /// <param name="filename">Входной файл</param>
        /// <param name="resultFilename">Результирующий файл, содержащий контуры</param>
        public static void DrawEdges(string filename, string resultFilename)
        {
            var canny = new CannyEdgeDetector();

            var inputBmp = new Bitmap(filename);

            Bitmap grayScaled = ToGrayscale(inputBmp);

            using (Bitmap edgedImage = canny.Apply(grayScaled))
            {
                edgedImage.Save(resultFilename);
            }
        }

        public static void DrawMask(byte[] data, int width, int heigth, string resultFileName)
        {
            using (var bitmap = new Bitmap(width, heigth))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.FillRectangle(Brushes.White, 0, 0, width, heigth);

                for (var row = 0; row < heigth; row++)
                {
                    for (var col = 0; col < width; col++)
                    {
                        if (data[row * width + col] > 0)
                        {
                            bitmap.SetPixel(col, row, Color.Red);
                        }
                    }
                }

                bitmap.Save(resultFileName, ImageFormat.Png);
            }
        }

        private static Bitmap ToGrayscale(Bitmap bmp)
        {
            var result = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format8bppIndexed);

            BitmapData data = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            // Copy the bytes from the image into a byte array
            byte[] bytes = new byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    var c = bmp.GetPixel(x, y);
                    var rgb = (byte)((c.R + c.G + c.B) / 3);

                    bytes[y * data.Stride + x] = rgb;
                }
            }

            // Copy the bytes from the byte array into the image
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);

            result.UnlockBits(data);

            return result;
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