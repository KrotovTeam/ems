using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Enums;
using Common.Objects;
using Common.Objects.Landsat;
using DeterminingPhenomenonService.Objects;
using DrawImageLibrary;
using OSGeo.GDAL;
using OSGeo.OSR;

namespace DeterminingPhenomenonService.Helpers
{
    public static class ValidationHelper
    {
        public static bool ValidateBuffers(IEnumerable<LandsatCuttedBuffers> temporaryBuffers)
        {
            var imagesSizes = new List<CuttedImageInfo>();
            foreach (var temporaryDataBuffer in temporaryBuffers.SelectMany(b => b.Channels.Select( ch => ch.Value)))
            {
                var cuttedImageInfos = new CuttedImageInfo
                {
                    Height = temporaryDataBuffer.GetLength(0),
                    Width = temporaryDataBuffer.GetLength(1)
                };
                imagesSizes.Add(cuttedImageInfos);
            }

            var currentImageInfo = imagesSizes.First();
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

        public static bool CloudValidation(string[] folders, GeographicPolygon polygon, string resultCloudMaskTifFilename, string resultCloudMaskPngFilename)
        {
            byte[] cloudMask = null;
            int width = 0;
            SpatialReference tifProjection = null;
            int height = 0;
            var utmPolygon = new UtmPolygon();
            foreach (var folder in folders)
            {
                var landsatData = new LandsatDataDescription(folder);
                var qaFile = landsatData.ChannelBqa;

                using (var ds = Gdal.Open(qaFile, Access.GA_ReadOnly))
                {
                    double[] geotransform = new double[6];
                    ds.GetGeoTransform(geotransform);

                    utmPolygon = Helper.ConvertGeographicPolygonToUtm(polygon, ds);

                    using (var band = ds.GetRasterBand(1))
                    {
                        var projectionRef = ds.GetProjectionRef();
                        tifProjection = new SpatialReference(projectionRef);
                        
                        var cuttedImageInfo = ClipImageHelper.GetCuttedImageInfoByPolygonData(utmPolygon, geotransform);

                        cloudMask = cloudMask ?? new byte[cuttedImageInfo.Width * cuttedImageInfo.Height];
                        cloudMask = GetCloudMaskByBandAndCoordinates(band, cuttedImageInfo, cloudMask);

                        width = cuttedImageInfo.Width;
                        height = cuttedImageInfo.Height;
                    }
                }
            }

            if (cloudMask == null)
                return false;

            double cloudedPointCount = cloudMask.Count(x => x != 0);

            var percentOfCloudedPoints = cloudedPointCount / cloudMask.Count();

            bool isValidCloudy = percentOfCloudedPoints < 0.14;

            tifProjection.ExportToWkt(out var inputShapeSrs);
            double[] argin = { polygon.UpperLeft.Latitude, 30, 0, polygon.UpperLeft.Longitude, 0, -30 };

            Helper.SaveDataInFile(resultCloudMaskTifFilename, cloudMask, width, height, DataType.GDT_Byte, argin, inputShapeSrs);
            DrawLib.DrawMask(cloudMask, width, height, resultCloudMaskPngFilename);

            return isValidCloudy;
        }

        private static byte[] GetCloudMaskByBandAndCoordinates(Band qaBand, CuttedImageInfo imageInfo, byte[] cloudMask)
        {
            var buffer = new short[imageInfo.Width * imageInfo.Height];

            qaBand.ReadRaster(imageInfo.Col, imageInfo.Row, imageInfo.Width, imageInfo.Height, buffer, imageInfo.Width, imageInfo.Height, 0, 0);
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
            ////Cirrus Confidence - Low
            //2720, 2722, 2724, 2728, 2732, 2752, 2756, 2760, 2764, 2800, 2804, 2804, 2808, 2812, 2976,
            //2980, 2984, 2988, 3008, 3012, 3016, 3020, 3744, 3748, 3752, 3756, 3780, 3784, 3788,
            ////Cloud Confidence-Low
            //2720, 2722, 2724, 2728, 2732, 2976, 2980, 2984, 2988, 3744, 3748, 3752, 3756, 6816, 6820, 6824,
            //6828, 7072, 7076, 7080, 7084, 7840, 7844, 7848, 7852,
            //Snow-ice high
            3744, 3748, 3752, 3756, 3776, 3780, 3784, 3788, 7840, 7844, 7848, 7852, 7872, 7876, 7880, 7884,
            //Cloud Confidence - Medium
            2752, 2756, 2760, 2764, 3008, 3012, 3016, 3020, 3776, 3780, 3784, 3788,
            6848, 6852, 6856, 6860, 7104, 7108, 7112, 7116, 7872, 7876, 7880, 7884,
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
