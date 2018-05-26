using System;
using System.Collections.Generic;
using Common.Enums;
using Common.Objects;
using Common.PointsReaders;
using OSGeo.GDAL;

namespace DeterminingPhenomenonService.Helpers
{
    public static class ClipImageHelper
    {
        public static CuttedImageInfo GetCuttedImageInfoByCoordinates(string filename, Dictionary<ImageCorner, double[]> coordinates)
        {
            CuttedImageInfo cuttedImageInfo = new CuttedImageInfo();

            using (var ds = Gdal.Open(filename, Access.GA_ReadOnly))
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
                    var row1 = Convert.ToInt32((utmCoordinates[ImageCorner.UpperLeft][1] - yinit) / ysize);
                    var col1 = Convert.ToInt32((utmCoordinates[ImageCorner.UpperLeft][0] - xinit) / xsize);
                    var row2 = Convert.ToInt32((utmCoordinates[ImageCorner.LowerRight][1] - yinit) / ysize);
                    var col2 = Convert.ToInt32((utmCoordinates[ImageCorner.LowerRight][0] - xinit) / xsize);
                    var colSize = col2 - col1 + 1;
                    var rowSize = row2 - row1 + 1;

                    cuttedImageInfo.Row = row1;
                    cuttedImageInfo.Col = col1;
                    cuttedImageInfo.Width = colSize;
                    cuttedImageInfo.Height = rowSize;
                }
            }

            return cuttedImageInfo;
        }

        public static double[,] ReadBufferByIndexes(CuttedImageInfo cuttedImageInfo, LandsatNormilizedSnapshotReader reader)
        {
            var buffer = new double[cuttedImageInfo.Height, cuttedImageInfo.Width];
            int i = 0, j = 0;

            for (var x = cuttedImageInfo.Row; x < cuttedImageInfo.Row + cuttedImageInfo.Height; x++)
            {
                foreach (var data in reader.ReadCuttedline(x, cuttedImageInfo.Col, cuttedImageInfo.Width))
                {
                    buffer[i, j] = data;
                    j++;
                }

                j = 0;
                i++;
            }
            reader.Dispose();
            return buffer;
        }
    }
}
