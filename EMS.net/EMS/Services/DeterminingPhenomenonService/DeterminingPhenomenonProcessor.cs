using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusContracts;
using Common.Enums;
using Common.Helpers;
using Common.PointsReaders;
using DeterminingPhenomenonService.Helpers;
using Isodata;
using Isodata.Abstraction;
using Isodata.Objects;
using Newtonsoft.Json;
using OSGeo.GDAL;
using IsodataMathHelper = Isodata.Helpers.MathHelper;

namespace DeterminingPhenomenonService
{
    public class DeterminingPhenomenonProcessor
    {
        public Dictionary<ImageCorner, double[]> Coordinates;
        public string[] DataFolders;
        public string ResultFolder;
        public PhenomenonType Phenomenon;

        private string _pathToCloudMaskFile = @"/cloudMask.tif";
        private string _pathToDynamicFile = @"/dynamic.tif";

        public bool Proccess()
        {
            //если невалидно, то говорим пользователю о том, что невозможно обнаружить явление и подсчитать его характеристики.
            var isValidCloudy = ValidationHelper.CloudValidation(DataFolders, Coordinates, ResultFolder + _pathToCloudMaskFile);
            if (!isValidCloudy)
                return false;

            var clusteringManager = new ClusteringManager();

            var temporaryClusters = new Dictionary<string, List<Cluster>>();
            foreach (var folder in DataFolders)
            {
                
                var files = Directory.EnumerateFiles(folder).ToList();
                var channel4 = files.SingleOrDefault(f => f.Contains("B4") && !f.Contains("cutted") && !f.EndsWith(".l8n"));
                var channel5 = files.SingleOrDefault(f => f.Contains("B5") && !f.Contains("cutted") && !f.EndsWith(".l8n"));

                using (var isodataPointsReader = new LandsatIsodataPointsReader(channel4 + ".l8n", channel5 + ".l8n"))
                {
                    List<Cluster> clusters;
                    var jsonClustersFilename = folder + @"/B4_B5_clusters.json";
                   
                    if (string.IsNullOrEmpty(files.FirstOrDefault(f => f.Contains("clusters.json") && !f.Contains("cutted"))))
                    {
                        clusters = clusteringManager.Process(isodataPointsReader, new NdviIsodataProfile());
                        JsonHelper.Serialize(jsonClustersFilename, clusters);
                    }
                    else
                    {
                        clusters = JsonHelper.Deserialize<List<Cluster>>(jsonClustersFilename);
                    }

                    temporaryClusters.Add(folder, clusters);
                }
            }

            var cloudMask = Helper.GetCloudMaskFromFile(ResultFolder + _pathToCloudMaskFile);
           
            return CalculateDynamic(temporaryClusters, Coordinates, cloudMask, Phenomenon, ResultFolder + _pathToDynamicFile);
        }

        private bool CalculateDynamic(Dictionary<string, List<Cluster>> temporaryClustersDatas, Dictionary<ImageCorner, double[]> coordinates, 
            byte[] cloudMask, PhenomenonType phenomenon, string dynamicResultFilename)
        {
            Dictionary<string, List<double[,]>> temporaryBuffers = GetTemporaryBuffers(temporaryClustersDatas, coordinates);

            if (!ValidationHelper.ValidateBuffers(temporaryBuffers))
            {
                return false;
            }

            //получаем любой снимок за последнюю временную область, чтобы определить результирующие ширину и высоту маски динамики 
            var currentTemporaryImagesData = temporaryBuffers.LastOrDefault().Value.First();

            var width = currentTemporaryImagesData.GetLength(1);
            var heigth = currentTemporaryImagesData.GetLength(0);

            var dymanicMask = new byte[width * heigth];

            //получаем данные за последнюю временную область
            KeyValuePair<string, List<double[,]>> currentTemporaryBuffers = temporaryBuffers.LastOrDefault();

            Dictionary<string, List<double[,]>> pastTemporaryBuffers = temporaryBuffers.Take(temporaryBuffers.Count - 1).ToDictionary(x => x.Key, x => x.Value);

            for (int row = 0; row < heigth; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (!ValidationHelper.ValidatePointByCloudMask(cloudMask, row, col, width))
                    {
                        continue;
                    }

                    double pastNdvi = Helper.CalculateAverageNdviForPastTemporaryPoint(pastTemporaryBuffers, temporaryClustersDatas, row, col);

                    double currentNdvi = Helper.CalculateNdviForCurrentTemporaryPoint(currentTemporaryBuffers, temporaryClustersDatas, row, col);

                    var dynamic = Math.Abs(currentNdvi - pastNdvi) / currentNdvi;

                    dymanicMask[row * width + col] = (byte)(dynamic >= 0.3 ? 1 : 0);
                }
            }

            bool isValidDynamic = true;//DynaValidateDynamic();
           

            var path = Helper.SaveDataInFile(dynamicResultFilename, dymanicMask, width, heigth, DataType.GDT_Byte);

            return true;
            //DrawManager.DrawDynamicMask(path, @"Karpati/dynamicMask.bmp");
        }

        private Dictionary<string, List<double[,]>> GetTemporaryBuffers(Dictionary<string, List<Cluster>> temporaryClustersDatas, 
            Dictionary<ImageCorner, double[]> coordinates)
        {
            Dictionary<string, List<double[,]>> temporaryBuffers = new Dictionary<string, List<double[,]>>();

            foreach (var temporaryClustersData in temporaryClustersDatas)
            {
                var temporaryNormilizedDataFiles = Directory.EnumerateFiles(temporaryClustersData.Key)
                    .Where(f => f.Contains(".l8n") && !f.Contains("cutted"));
                var pastTemporaryGeotiffDataFile =
                    Directory.EnumerateFiles(temporaryClustersData.Key).First(f => f.Contains("B4"));

                var cuttedImageInfo = ClipImageHelper.GetCuttedImageInfoByCoordinates(pastTemporaryGeotiffDataFile, coordinates);

                var temporaryDataBuffers = temporaryNormilizedDataFiles.Select(
                    pastTemporaryNormilizedDataFile => ClipImageHelper.ReadBufferByIndexes(
                        cuttedImageInfo,
                        new LandsatNormilizedSnapshotReader(pastTemporaryNormilizedDataFile)
                    )
                ).ToList();

                temporaryBuffers.Add(temporaryClustersData.Key, temporaryDataBuffers);
            }

            return temporaryBuffers;
        }
    }
}
