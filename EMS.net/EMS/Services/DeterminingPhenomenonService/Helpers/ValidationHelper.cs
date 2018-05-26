using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Enums;
using Common.Objects;
using DrawImageLibrary;
using OSGeo.GDAL;
using OSGeo.OSR;

namespace DeterminingPhenomenonService.Helpers
{
    public static class ValidationHelper
    {
        public static bool ValidateBuffers(Dictionary<string, List<double[,]>> temporaryDataBuffers)
        {
            var imagesSizes = new List<CuttedImageInfo>();
            foreach (var temporaryDataBuffer in temporaryDataBuffers)
            {
                var cuttedImageInfos = temporaryDataBuffer.Value.Select(imageData => new CuttedImageInfo { Height = imageData.GetLength(0), Width = imageData.GetLength(1) });
                imagesSizes.AddRange(cuttedImageInfos);
            }

            var currentImageInfo = imagesSizes.FirstOrDefault();
            foreach (var imageInfo in imagesSizes.Skip(1))
            {
                if (currentImageInfo.Width == imageInfo.Width && currentImageInfo.Height == imageInfo.Height)
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        public static bool ValidatePointByCloudMask(byte[] cloudMask, int row, int col, int width)
        {
            if (cloudMask[row * width + col] == 0)
            {
                return true;
            }

            return false;
        }

        public static bool CloudValidation(string[] folders, Dictionary<ImageCorner, double[]> coordinates, string resultCloudMaskFilename)
        {
            Dictionary<ImageCorner, int[]> cornersIndexes = new Dictionary<ImageCorner, int[]>();
            byte[] cloudMask = null;
            int width = 0;
            SpatialReference tifProjection = null;
            int height = 0;
            foreach (var folder in folders)
            {
                var qaFile = Directory.EnumerateFiles(folder).First(f => f.Contains("BQA"));
                if (string.IsNullOrEmpty(qaFile))
                {
                    return false;
                }

                using (var ds = Gdal.Open(qaFile, Access.GA_ReadOnly))
                {
                    double[] geotransform = new double[6];
                    ds.GetGeoTransform(geotransform);
                    var xinit = geotransform[0];
                    var yinit = geotransform[3];

                    var xsize = geotransform[1];
                    var ysize = geotransform[5];
                    var utmCoordinates = Helper.ConvertLatLonToUtm(coordinates, ds);
                    using (var band = ds.GetRasterBand(1))
                    {
                        var projectionRef = ds.GetProjectionRef();
                        tifProjection = new SpatialReference(projectionRef);

                        var row1 = Convert.ToInt32((utmCoordinates[ImageCorner.UpperLeft][1] - yinit) / ysize);
                        var col1 = Convert.ToInt32((utmCoordinates[ImageCorner.UpperLeft][0] - xinit) / xsize);
                        var row2 = Convert.ToInt32((utmCoordinates[ImageCorner.LowerRight][1] - yinit) / ysize);
                        var col2 = Convert.ToInt32((utmCoordinates[ImageCorner.LowerRight][0] - xinit) / xsize);
                        var colSize = col2 - col1 + 1;
                        var rowSize = row2 - row1 + 1;

                        if (!cornersIndexes.ContainsKey(ImageCorner.UpperLeft))
                        {
                            cornersIndexes.Add(ImageCorner.UpperLeft, new[]
                            {
                                col1,
                                row1
                            });
                        }
                        cloudMask = cloudMask ?? new byte[colSize * rowSize];
                        cloudMask = GetCloudMaskByBandAndCoordinates(band, cornersIndexes, colSize, rowSize, cloudMask);
                        width = colSize;
                        height = rowSize;
                    }
                }
            }

            if (cloudMask == null)
                return false;

            double cloudedPointCount = cloudMask.Count(x => x != 0);

            var percentOfCloudedPoints = cloudedPointCount / cloudMask.Count();

            bool isValidCloudy = percentOfCloudedPoints < 0.14;

            tifProjection.ExportToWkt(out var inputShapeSrs);
            double[] argin = { coordinates[ImageCorner.UpperLeft][0], 30, 0, coordinates[ImageCorner.LowerRight][1], 0, -30 };

            Helper.SaveDataInFile(resultCloudMaskFilename, cloudMask, width, height, DataType.GDT_Byte, argin, inputShapeSrs);
            //DrawLib.DrawMask(cloudMask, width, height, resultCloudMaskFilename);

            return isValidCloudy;
        }

        private static byte[] GetCloudMaskByBandAndCoordinates(Band qaBand, Dictionary<ImageCorner, int[]> cornersIndexes, int width, int height, byte[] cloudMask)
        {
            var buffer = new short[width * height];

            qaBand.ReadRaster(cornersIndexes[ImageCorner.UpperLeft][0], cornersIndexes[ImageCorner.UpperLeft][1], width, height, buffer, width, height, 0, 0);
            byte[] currentCloudMask = buffer.Select(x => (byte)(IsCloud(x) ? 1 : 0)).ToArray();
            for (int i = 0; i < cloudMask.Length; i++)
            {
                cloudMask[i] += currentCloudMask[i];
            }

            return cloudMask;
        }

        private static bool IsCloud(this int value)
        {
            return СloudesConstants.Contains(value);
        }

        private static readonly List<int> СloudesConstants = new List<int>()
        {
            //Cloud Confidence - High
            2800, 2804, 2808, 2812, 3008, 2752, 6896 ,6900, 6904, 6908,
            //Cloud Shadow - High
            2976, 2980, 2984, 2988, 3008, 3012, 3016, 3020, 7072, 7076, 7080, 7084, 7104, 7108, 7112, 7116,
            //Cloud
            2800, 2804, 2808, 2812, 6896, 6900, 6904, 6908,
            //Cirrus Confidence - High
            6816, 6820, 6824, 6828, 6848, 6852, 6856, 6860, 6896, 6900, 6904,
            6908, 7072, 7076, 7080, 7084, 7104, 7108, 7112, 7116, 7840, 7844,
            7848, 7852, 7872, 7876, 7880, 7884
        };
    }
}
