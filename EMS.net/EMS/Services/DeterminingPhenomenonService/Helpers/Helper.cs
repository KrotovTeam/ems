using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
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

        public static Dictionary<ImageCorner, double[]> ConvertLatLonToUtm(Dictionary<ImageCorner, double[]> coordinates, Dataset ds)
        {
            var utmPoints = new Dictionary<ImageCorner, double[]>();
            SpatialReference monUtm = new SpatialReference(ds.GetProjectionRef());


            SpatialReference monGeo = new SpatialReference(ds.GetProjectionRef());
            monGeo.ImportFromEPSG(4326);
            foreach (var coordinate in coordinates)
            {
                double[] res = new double[3];

                CoordinateTransformation coordTrans = new CoordinateTransformation(monGeo, monUtm);
                coordTrans.TransformPoint(res, coordinate.Value[1], coordinate.Value[0], 0);
                utmPoints.Add(coordinate.Key, new double[] { res[0], res[1] });
            }

            return utmPoints;
        }

        public static double CalculateAverageNdviForPastTemporaryPoint(Dictionary<string, List<double[,]>> pastTemporartyBuffers, 
            Dictionary<string, List<Cluster>> temporaryClustersDatas, int row, int col)
        {
            double ndvi = 0;

            var buffersCount = pastTemporartyBuffers.Count;
            var sum = 0.0;
            foreach (var temporaryBuffer in pastTemporartyBuffers)
            {
                var values = new[] { temporaryBuffer.Value.First()[row, col], temporaryBuffer.Value.Last()[row, col] };
                var perfectCluster = temporaryClustersDatas[temporaryBuffer.Key].OrderBy(z => MathHelper.EuclideanDistance(values, z.CenterCluster)).First();
                sum += CalculateNdvi(perfectCluster.CenterCluster[0], perfectCluster.CenterCluster[1]);
            }

            ndvi = sum / buffersCount;

            return ndvi;
        }

        public static double CalculateNdviForCurrentTemporaryPoint(KeyValuePair<string, List<double[,]>> currentTemporaryBuffers,
            Dictionary<string, List<Cluster>> temporaryClustersDatas, int row, int col)
        {
            var currentValues = new[]
            {
                currentTemporaryBuffers.Value.First()[row, col],
                currentTemporaryBuffers.Value.Last()[row, col]
            };

            var perfectCluster = temporaryClustersDatas[currentTemporaryBuffers.Key]
                .OrderBy(z => MathHelper.EuclideanDistance(currentValues, z.CenterCluster)).First();

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
            Dataset outputDataset = outputDriver.Create(filename, width, height, 1, dataType, null);
            if (argin != null)
            {
                outputDataset.SetGeoTransform(argin);
            }

            Band outputband = outputDataset.GetRasterBand(1);
            switch (dataType)
            {
                case DataType.GDT_UInt16:
                    outputband.WriteRaster(0, 0, width, height, (short[])data, width, height, 0, 0);
                    break;
                case DataType.GDT_Byte:
                    outputband.WriteRaster(0, 0, width, height, (byte[])data, width, height, 0, 0);
                    break;
            }

            if (!string.IsNullOrEmpty(shapeSrs))
            {
                outputDataset.SetProjection(shapeSrs);
            }

            outputDataset.FlushCache();
            outputband.FlushCache();

            return filename;
        }

    }
}
