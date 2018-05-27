using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using Common.Objects;
using DeterminingPhenomenonService.Objects;
using Isodata.Helpers;
using Isodata.Objects;
using OSGeo.GDAL;
using OSGeo.OSR;

namespace DeterminingPhenomenonService.Helpers
{
    public static class Helper
    {
        public static byte[] GetCloudMaskFromFile(string cloudMaskFilename)
        {
            using (var cloudDs = Gdal.Open(cloudMaskFilename, Access.GA_ReadOnly))
            {
                using (var cloudBand = cloudDs.GetRasterBand(1))
                {
                    var buffer = new byte[cloudBand.XSize * cloudBand.YSize];

                    cloudBand.ReadRaster(0, 0, cloudBand.XSize, cloudBand.YSize, buffer, cloudBand.XSize, cloudBand.YSize, 0, 0);

                    return buffer;
                }
            }
        }

        public static UtmPolygon ConvertGeographicPolygonToUtm(GeographicPolygon polygon, Dataset ds)
        {
            SpatialReference monUtm = new SpatialReference(ds.GetProjectionRef());

            var utmPolygon = new UtmPolygon();

            SpatialReference monGeo = new SpatialReference(ds.GetProjectionRef());
            monGeo.ImportFromEPSG(4326);
            double[] res = new double[3];

            CoordinateTransformation coordTrans = new CoordinateTransformation(monGeo, monUtm);

            coordTrans.TransformPoint(res, polygon.UpperLeft.Longitude, polygon.UpperLeft.Latitude, 0);
            utmPolygon.UpperLeft.Easting = res[0];
            utmPolygon.UpperLeft.Northing = res[1];

            coordTrans.TransformPoint(res, polygon.LowerRight.Longitude, polygon.LowerRight.Latitude, 0);
            utmPolygon.LowerRight.Easting = res[0];
            utmPolygon.LowerRight.Northing = res[1];

            return utmPolygon;
        }

        public static GeographicPoint ConvertUtmPointToGeographic(UtmPoint utmPoint, Dataset ds)
        {
            SpatialReference monUtm = new SpatialReference(ds.GetProjectionRef());

            var geoPoint = new GeographicPoint();

            SpatialReference monGeo = new SpatialReference(ds.GetProjectionRef());
            monGeo.ImportFromEPSG(4326);
            double[] res = new double[3];

            CoordinateTransformation coordTrans = new CoordinateTransformation(monUtm, monGeo);

            coordTrans.TransformPoint(res, utmPoint.Easting, utmPoint.Northing, 0);
            geoPoint.Longitude = res[0];
            geoPoint.Latitude = res[1];

            return geoPoint;
        }

        public static GeographicPolygon ConvertUtmPolygonToGeographic(UtmPolygon utmPolygon, Dataset ds)
        {
            SpatialReference monUtm = new SpatialReference(ds.GetProjectionRef());

            var geoPolygon = new GeographicPolygon();

            SpatialReference monGeo = new SpatialReference(ds.GetProjectionRef());
            monGeo.ImportFromEPSG(4326);
            double[] res = new double[3];

            CoordinateTransformation coordTrans = new CoordinateTransformation(monUtm, monGeo);

            coordTrans.TransformPoint(res, utmPolygon.UpperLeft.Easting, utmPolygon.UpperLeft.Northing, 0);
            geoPolygon.UpperLeft.Longitude = res[0];
            geoPolygon.UpperLeft.Latitude = res[1];

            coordTrans.TransformPoint(res, utmPolygon.LowerRight.Easting, utmPolygon.LowerRight.Northing, 0);
            geoPolygon.LowerRight.Longitude = res[0];
            geoPolygon.LowerRight.Latitude = res[1];

            return geoPolygon;
        }

        public static double CalculateAverageNdviForPastTemporaryPoint(List<TemporaryData> pastTemporaryDatas, int row, int col)
        {
            double ndvi = 0;

            var buffersCount = pastTemporaryDatas.Count;
            var sum = 0.0;
            foreach (var pastTemporaryData in pastTemporaryDatas)
            {
                var perfectCluster = CalculateNdviInnerForTemporaryData(pastTemporaryData, row, col);

                sum += CalculateNdvi(perfectCluster.CenterCluster[0], perfectCluster.CenterCluster[1]);
            }

            ndvi = sum / buffersCount;

            return ndvi;
        }

        public static double CalculateNdviForCurrentTemporaryPoint(TemporaryData currentTemporaryData, int row, int col)
        {
            var perfectCluster = CalculateNdviInnerForTemporaryData(currentTemporaryData, row, col);

            return CalculateNdvi(perfectCluster.CenterCluster[0], perfectCluster.CenterCluster[1]);
        }

        public static double CalculateNdvi(double red, double nir)
        {
            var operand1 = nir - red;
            var operand2 = nir + red;

            var ndvi = operand1 / operand2;

            return ndvi;
        }

        public static string SaveDataInFile(string filename, IEnumerable data, int width, int height, DataType dataType, double[] argin = null, string shapeSrs = null)
        {

            OSGeo.GDAL.Driver outputDriver = Gdal.GetDriverByName("GTiff");
            using (Dataset outputDataset = outputDriver.Create(filename, width, height, 1, dataType, null))
            {
                if (argin != null)
                {
                    outputDataset.SetGeoTransform(argin);
                }

                Band outputband = outputDataset.GetRasterBand(1);
                switch (dataType)
                {
                    case DataType.GDT_UInt16:
                        outputband.WriteRaster(0, 0, width, height, (short[]) data, width, height, 0, 0);
                        break;
                    case DataType.GDT_Byte:
                        outputband.WriteRaster(0, 0, width, height, (byte[]) data, width, height, 0, 0);
                        break;
                }

                if (!string.IsNullOrEmpty(shapeSrs))
                {
                    outputDataset.SetProjection(shapeSrs);
                }

                outputDataset.FlushCache();
                outputband.FlushCache();
            }

            return filename;
        }

        private static Cluster CalculateNdviInnerForTemporaryData(TemporaryData currentTemporaryData, int row, int col)
        {
            var currentValues = new[]
            {
                currentTemporaryData.Buffers.Channels[Landsat8Channel.Channel4][row, col],
                currentTemporaryData.Buffers.Channels[Landsat8Channel.Channel5][row, col]
            };

            var perfectCluster = currentTemporaryData.Clusters
                .OrderBy(z => MathHelper.EuclideanDistance(currentValues, z.CenterCluster))
                .First();

            return perfectCluster;
        }

    }
}
