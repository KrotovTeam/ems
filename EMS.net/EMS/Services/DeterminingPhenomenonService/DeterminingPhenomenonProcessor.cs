using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusContracts;
using Common.Constants;
using Common.Enums;
using Common.Helpers;
using Common.Objects;
using Common.Objects.Landsat;
using Common.PointsReaders;
using DeterminingPhenomenonService.Helpers;
using DeterminingPhenomenonService.Objects;
using DrawImageLibrary;
using Isodata;
using Isodata.Abstraction;
using Isodata.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OSGeo.GDAL;
using Topshelf.Logging;
using IsodataMathHelper = Isodata.Helpers.MathHelper;

namespace DeterminingPhenomenonService
{
    public class DeterminingPhenomenonProcessor
    {
        //public Dictionary<ImageCorner, double[]> Coordinates;
        private readonly GeographicPolygon _polygon;
        private readonly string[] _dataFolders;
        private readonly string _resultFolder;
        private readonly PhenomenonType _phenomenon;

        private readonly string _pathToCloudMaskTiffFile;
        private readonly string _pathToCloudMaskPngFile;
        private readonly string _pathToDynamicFile;
        private readonly string _pathToEdgedDynamicFile;
        private readonly string _pathToVisibleImage;
        private readonly string _pathToVisibleDynamicFile;
        private string _pathToClustersFolder;
        private readonly string _pathToDynamicPointsJson;

        private readonly LogWriter _logger;

        public DeterminingPhenomenonProcessor(LogWriter logger, string[] dataFolders, string resultFolder, GeographicPolygon polygon,
            PhenomenonType phenomenon)
        {
            _logger = logger;
            _dataFolders = dataFolders;
            _resultFolder = resultFolder;
            _phenomenon = phenomenon;
            _polygon = polygon;

            _pathToCloudMaskTiffFile = $@"{_resultFolder}{FilenamesConstants.PathToCloudMaskTiffFile}";
            _pathToCloudMaskPngFile = $@"{_resultFolder}{FilenamesConstants.PathToCloudMaskPngFile}";
            _pathToDynamicFile = $@"{_resultFolder}{FilenamesConstants.PathToDynamicFile}";
            _pathToEdgedDynamicFile = $@"{_resultFolder}{FilenamesConstants.PathToEdgedDynamicFile}";
            _pathToVisibleImage = $@"{_resultFolder}{FilenamesConstants.PathToVisibleImage}";
            _pathToVisibleDynamicFile = $@"{_resultFolder}{FilenamesConstants.PathToVisibleDynamicFile}";
            _pathToDynamicPointsJson = $@"{_resultFolder}{FilenamesConstants.PathToDynamicGeoPointsJson}";
        }

        public bool Proccess()
        {
            //если невалидно, то говорим пользователю о том, что невозможно обнаружить явление и подсчитать его характеристики.
            _logger.Info($"Сервис обнаружения явления. Статус: валидация облачности.");
            var isValidCloudy = ValidationHelper.CloudValidation(_dataFolders, _polygon, _pathToCloudMaskTiffFile,
                _pathToCloudMaskPngFile);
            if (!isValidCloudy)
                _logger.Info($"Сервис обнаружения явления. Статус: валидация на облачность не пройдена. Явление не обнаружено.");

            var clusteringManager = new ClusteringManager();

            List<TemporaryData> temporaryDatas = new List<TemporaryData>();

            foreach (var folder in _dataFolders)
            {

                List<string> necessaryDataFiles = new List<string>();

                var landsatData = new LandsatDataDescription(folder);

                if (_phenomenon == PhenomenonType.ForestPlantationsDeseases)
                {
                    necessaryDataFiles.AddRange(
                        new[] {landsatData.Channel4.Normalized, landsatData.Channel5.Normalized});
                }

                using (var isodataPointsReader = new LandsatIsodataPointsReader(necessaryDataFiles.ToArray()))
                {
                    List<Cluster> clusters = new List<Cluster>();
                    var jsonClustersFilename = string.Empty;

                    if (_phenomenon == PhenomenonType.ForestPlantationsDeseases)
                    {
                        jsonClustersFilename = FilenamesConstants.B4B5Clusters;
                    }

                    if (landsatData.ClustersJson.Any())
                    {
                        var fullPathToClustersJson =
                            landsatData.ClustersJson.FirstOrDefault(f => f.Contains(jsonClustersFilename));

                        if (!string.IsNullOrEmpty(fullPathToClustersJson))
                        {
                            clusters = JsonHelper.Deserialize<List<Cluster>>(fullPathToClustersJson);
                        }
                    }
                    else
                    {
                        _pathToClustersFolder = $@"{folder}{FilenamesConstants.PathToClustersFolder}";
                        _logger.Info($"Сервис обнаружения явления. Текущее действие: кластеризация данных {string.Join(",", necessaryDataFiles)}");
                        clusters = clusteringManager.Process(isodataPointsReader, new NdviIsodataProfile());
                        JsonHelper.Serialize($"{_pathToClustersFolder}{jsonClustersFilename}", clusters);
                        _logger.Info($"Сервис обнаружения явления. Текущее действие: данные прошли кластеризацию. Кол-во кластеров: {clusters.Count}");
                    }

                    var temporaryData = new TemporaryData
                    {
                        Clusters = clusters,
                        DataDescription = landsatData
                    };
                    temporaryDatas.Add(temporaryData);
                }
            }

            var cloudMask = Helper.GetCloudMaskFromFile(_pathToCloudMaskTiffFile);

            return CalculateDynamic(temporaryDatas, cloudMask, _phenomenon);
        }

        private bool CalculateDynamic(List<TemporaryData> temporaryDatas,
            byte[] cloudMask, PhenomenonType phenomenon)
        {
            _logger.Info($"Сервис обнаружения явления. Статус: определение динамики изменений за {temporaryDatas.Count} лет");
            SetTemporaryCuttedBuffers(temporaryDatas);

            if (!ValidationHelper.ValidateBuffers(temporaryDatas.Select(datas => datas.Buffers)))
            {
                return false;
            }

            //получаем данные за последнюю временную область
            var currentTemporaryBuffers = temporaryDatas.LastOrDefault();

            if (currentTemporaryBuffers == null)
            {
                return false;
            }

            //получаем любой снимок за последнюю временную область, чтобы определить результирующие ширину и высоту маски динамики
            var currentImageTemporaryDataImage = currentTemporaryBuffers.Buffers.Channels[Landsat8Channel.Channel4];

            var width = currentImageTemporaryDataImage.GetLength(1);
            var heigth = currentImageTemporaryDataImage.GetLength(0);

            var dymanicMask = new byte[width * heigth];

            var pastTemporaryDatasBuffers = temporaryDatas.Take(temporaryDatas.Count - 1).ToList();

            for (int row = 0; row < heigth; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (!ValidationHelper.ValidatePointByCloudMask(cloudMask, row, col, width))
                    {
                        continue;
                    }

                    double pastNdvi = Helper.CalculateAverageNdviForPastTemporaryPoint(pastTemporaryDatasBuffers, row, col);

                    double currentNdvi = Helper.CalculateNdviForCurrentTemporaryPoint(currentTemporaryBuffers, row, col);

                    double dynamic = -1.0;

                    if (phenomenon == PhenomenonType.ForestPlantationsDeseases)
                    {
                        dynamic = pastNdvi >= 0.2 && pastNdvi < 1
                            ? (pastNdvi > currentNdvi
                                ? Math.Abs(currentNdvi - pastNdvi) / currentNdvi
                                : 0)
                            : 0;
                    }

                    dymanicMask[row * width + col] = (byte)(dynamic >= 0.3 ? 1 : 0);
                }
            }

            int amountDynamicPoints = dymanicMask.Count(p => p > 0);

            if (amountDynamicPoints < 30)
            {
                _logger.Info($"Сервис обнаружения явления. Статус: динамика измнений обнаружена, но имеет очень слабое воздействие на выбранную область.");
                return false;
            }

            _logger.Info($"Сервис обнаружения явления. Статус: отрисовка результатов динамики.");
            DrawDynamicResult(dymanicMask, width, heigth);
            SaveDynamicGeoPoints();

            return true;
        }

        private void SaveDynamicGeoPoints()
        {
            var dynamicPoints = DrawLib.GetMaskIndexes(_pathToEdgedDynamicFile);
            var landsatDescription = new LandsatDataDescription(_dataFolders.Last());
            var currentTemporaryImage = landsatDescription.Channel4.Raw;

            var geographicPoints = ClipImageHelper.GetGeographicPointsByPointsIndexes(dynamicPoints, currentTemporaryImage, _polygon);

            JsonHelper.Serialize(_pathToDynamicPointsJson, geographicPoints);
        }

        private void SetTemporaryCuttedBuffers(List<TemporaryData> temporaryDatas)
        {
            foreach (var temporaryData in temporaryDatas)
            {
                temporaryData.Buffers = new LandsatCuttedBuffers();
                var temporaryDataBuffers = new LandsatCuttedBuffers();

                if (_phenomenon == PhenomenonType.ForestPlantationsDeseases)
                {
                    var temporaryGeotiffDataFile = temporaryData.DataDescription.Channel4.Raw;

                    var cuttedImageInfo =
                        ClipImageHelper.GetCuttedImageInfoByPolygon(temporaryGeotiffDataFile, _polygon);

                    foreach (var channelBuffer in temporaryDataBuffers.Channels)
                    {
                        var buffer = ClipImageHelper.ReadBufferByIndexes(cuttedImageInfo,
                            new LandsatNormilizedSnapshotReader(
                                temporaryData.DataDescription.GetLandsatSnapshotDescriptionByChannel(channelBuffer.Key)
                                    .Normalized));

                        temporaryData.Buffers.Channels[channelBuffer.Key] =  buffer;
                    }
                }
            }
        }

        private void DrawDynamicResult(byte[] bytes, int width, int height)
        {
            var currentFolder = _dataFolders.Last();
            var landsatDescription = new LandsatDataDescription(currentFolder);

            var currentImageInfo = ClipImageHelper.GetCuttedImageInfoByPolygon(landsatDescription.Channel4.Raw, _polygon);

            DrawLib.DrawMask(bytes, width, height, _pathToDynamicFile);

            DrawLib.DrawEdges(_pathToDynamicFile, _pathToEdgedDynamicFile);

            DrawLib.DrawNaturalColor(landsatDescription.Channel4.Normalized, landsatDescription.Channel3.Normalized, landsatDescription.Channel2.Normalized,
                currentImageInfo, _pathToVisibleImage);

            DrawLib.DrawMask(_pathToEdgedDynamicFile, _pathToVisibleImage, _pathToVisibleDynamicFile);
        }
    }
}
