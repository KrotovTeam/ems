using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using Common.Objects;
using Common.PointsReaders;
using DeterminingPhenomenonService.Objects;
using OSGeo.GDAL;

namespace DeterminingPhenomenonService.Helpers
{
    public static class ClipImageHelper
    {
        public static CuttedImageInfo GetCuttedImageInfoByPolygon(string filename, GeographicPolygon polygon)
        {
            CuttedImageInfo cuttedImageInfo;

            using (var ds = Gdal.Open(filename, Access.GA_ReadOnly))
            {
                double[] geotransform = new double[6];
                ds.GetGeoTransform(geotransform);

                var utmPolygon = Helper.ConvertGeographicPolygonToUtm(polygon, ds); 

                cuttedImageInfo = GetCuttedImageInfoByPolygonData(utmPolygon, geotransform);
            }

            return cuttedImageInfo;
        }

        public static CuttedImageInfo GetCuttedImageInfoByPolygonData(UtmPolygon utmPolygon, double[] geoTransform)
        {
            //utm-easting upperleft point
            var xinit = geoTransform[0];

            //utm-northing upperleft point
            var yinit = geoTransform[3];

            //размер пикселя 
            var xsize = geoTransform[1];
            var ysize = geoTransform[5];

            var row1 = Convert.ToInt32((utmPolygon.UpperLeft.Northing - yinit) / ysize);
            var col1 = Convert.ToInt32((utmPolygon.UpperLeft.Easting - xinit) / xsize);
            var row2 = Convert.ToInt32((utmPolygon.LowerRight.Northing - yinit) / ysize);
            var col2 = Convert.ToInt32((utmPolygon.LowerRight.Easting - xinit) / xsize);
            var colSize = col2 - col1 + 1;
            var rowSize = row2 - row1 + 1;

            var cuttedImageInfo = new CuttedImageInfo
            {
                Col = col1,
                Row = row1,
                Width = colSize,
                Height = rowSize
            };

            return cuttedImageInfo;
        }

        public static List<GeographicPoint> GetGeographicPointsByPointsIndexes(List<Point> points, string filename, GeographicPolygon polygon)
        {  
            using (var ds = Gdal.Open(filename, Access.GA_ReadOnly))
            {
                var georgrapicPoints = new List<GeographicPoint>();
                double[] geotransform = new double[6];
                ds.GetGeoTransform(geotransform);

                var utmPolygon = Helper.ConvertGeographicPolygonToUtm(polygon, ds);

                var xinit = utmPolygon.UpperLeft.Easting;

                //utm-northing upperleft point
                var yinit = utmPolygon.UpperLeft.Northing;

                //размер пикселя 
                var xsize = geotransform[1];
                var ysize = geotransform[5];
                
                var utmPoints = points.Select(p => new UtmPoint
                    {
                        Easting = xinit + xsize * p.X,
                        Northing = yinit - ysize * p.Y
                    }
                );

                georgrapicPoints = utmPoints.Select(p => Helper.ConvertUtmPointToGeographic(p, ds)).ToList();

                return georgrapicPoints;
            }
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
