using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AForge.Imaging.Filters;
using Common.Objects;
using Common.PointsReaders;
using Point = Common.Objects.Point;

namespace DrawImageLibrary
{
    public static class DrawLib
    {
        /// <summary>
        /// Создать изображение с легендой
        /// </summary>
        /// <param name="width">Ширина</param>
        /// <param name="heigth">Высота</param>
        /// <param name="legendFileName">Абсолютный путь к файлу легенды</param>
        /// <returns></returns>
        public static Bitmap CreateImageWithLegend(int width, int heigth, string legendFileName)
        {
            using (var legend = new Bitmap(legendFileName))
            {
                var bitmapWidth = width + legend.Width;
                var bitmapHeigth = heigth;
                if (heigth < legend.Height)
                {
                    bitmapHeigth = legend.Height;
                }

                var bitmap = new Bitmap(bitmapWidth, bitmapHeigth);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                   graphics.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
                }

                var x = bitmap.Width - legend.Width;
                var y = bitmap.Height / 2 - legend.Height / 2;

                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.DrawImage(legend, x, y, legend.Width, legend.Height);
                }

                return bitmap;
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
            var canny = new CannyEdgeDetector((byte)80, (byte)180);

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

        /// <summary>
        /// Возвращает список точек, которые являются помечеными в маске(например, динамика)
        /// </summary>
        /// <param name="maskFilename"></param>
        /// <returns></returns>
        public static List<Point> GetMaskIndexes(string maskFilename)
        {
            List<Point> pointsIndexes= new List<Point>();
            using (var dynamic = new Bitmap(maskFilename))
            {
                for (int i = 0; i < dynamic.Height; i++)
                {
                    for (int j = 0; j < dynamic.Width; j++)
                    {
                        if (dynamic.GetPixel(j, i).R > 0)
                        {
                            pointsIndexes.Add(new Point { X = j, Y = i });
                        }
                    }
                }
            }

            return pointsIndexes;
        }

        /// <summary>
        /// Возвращает кол-во точек, в которых была обнаружена динамика
        /// </summary>
        /// <param name="edgedDynamicFileName">Файл маски динамики</param>
        /// <returns></returns>
        public static int GetAmountOfDynamicPoints(string edgedDynamicFileName)
        {
            var amountOfDynamicPoints = 0;
            var filters = new FiltersSequence(new HomogenityEdgeDetector(), new Closing(), new GaussianBlur(5, 17));

            using (var edgedDynamic = new Bitmap(edgedDynamicFileName))
            using (var transformed = filters.Apply(edgedDynamic))
            {
                for (var i = 0; i < transformed.Height; i++)
                {
                    for (var j = 0; j < transformed.Width; j++)
                    {
                        var pixel = transformed.GetPixel(j, i);
                        if (pixel.R > 0)
                        {
                            amountOfDynamicPoints++;
                        }
                    }
                }
            }

            return amountOfDynamicPoints;
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
