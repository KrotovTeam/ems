using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BusContracts;
using Common.Constants;
using Common.Enums;
using Common.Helpers;
using Common.Objects;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Topshelf;
using Topshelf.Logging;

namespace DataNormalizationService
{
    public class NormalizationService : ServiceControl
    {
        private BusManager.BusManager _busManager;
        private static readonly LogWriter Logger = HostLogger.Get<NormalizationService>();

        public bool Start(HostControl hostControl)
        {
            _busManager = new BusManager.BusManager();
            _busManager.StartBus(GetBusConfigurations());

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _busManager.StopBus();

            return true;
        }

        private async Task ProcessRequest(IDataNormalizationRequest request)
        {
            Logger.Info($"Получен запрос (RequestId = {request.RequestId}) на нормализацию данных в папке {request.Folder}");
            if (request.SatelliteType != SatelliteType.Landsat8)
            {
                throw new ArgumentException("Неверно задан тип спутника");
            }

            if (Directory.Exists(request.Folder))
            {
                var fileNames = Directory.GetFiles(request.Folder);

                var normalizationDataFolder = $@"{request.Folder}\NormalizationData";
                if (!Directory.Exists(normalizationDataFolder))
                {
                    Directory.CreateDirectory(normalizationDataFolder);
                }

                var metadataFileName = fileNames.Single(name => name.EndsWith("mtl.json", StringComparison.OrdinalIgnoreCase));
                LandsatMetadata metadataFile = JsonHelper.Deserialize<LandsatMetadata>(metadataFileName);
                LandsatNormalizationProcessor processor = new LandsatNormalizationProcessor(Logger);
                RadiometricRescaling radiometricRescaling = metadataFile.L1MetadataFile.RadiometricRescaling;
                ImageAttributes imageAttributes = metadataFile.L1MetadataFile.ImageAttributes;
                MinMaxRadiance minMaxRadiance = metadataFile.L1MetadataFile.MinMaxRadiance;
                MinMaxReflectance minMaxReflectance = metadataFile.L1MetadataFile.MinMaxReflectance;

                #region Channel normalization

                var channel1FilePath = fileNames.Single(name => name.EndsWith("B1.TIF", StringComparison.OrdinalIgnoreCase));
                processor.Normalization(channel1FilePath, normalizationDataFolder,
                    radiometricRescaling.RadianceMultBand1, radiometricRescaling.RadianceAddBand1,
                    imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                    minMaxRadiance.RadianceMaximumBand1, minMaxReflectance.ReflectanceMaximumBand1);

                var channel2FilePath = fileNames.Single(name => name.EndsWith("B2.TIF", StringComparison.OrdinalIgnoreCase));
                processor.Normalization(channel2FilePath, normalizationDataFolder,
                    radiometricRescaling.RadianceMultBand2, radiometricRescaling.RadianceAddBand2,
                    imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                    minMaxRadiance.RadianceMaximumBand2, minMaxReflectance.ReflectanceMaximumBand2);

                var channel3FilePath = fileNames.Single(name => name.EndsWith("B3.TIF", StringComparison.OrdinalIgnoreCase));
                processor.Normalization(channel3FilePath, normalizationDataFolder,
                    radiometricRescaling.RadianceMultBand3, radiometricRescaling.RadianceAddBand3,
                    imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                    minMaxRadiance.RadianceMaximumBand3, minMaxReflectance.ReflectanceMaximumBand3);

                var channel4FilePath = fileNames.Single(name => name.EndsWith("B4.TIF", StringComparison.OrdinalIgnoreCase));
                processor.Normalization(channel4FilePath, normalizationDataFolder,
                    radiometricRescaling.RadianceMultBand4, radiometricRescaling.RadianceAddBand4,
                    imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                    minMaxRadiance.RadianceMaximumBand4, minMaxReflectance.ReflectanceMaximumBand4);

                var channel5FilePath = fileNames.Single(name => name.EndsWith("B5.TIF", StringComparison.OrdinalIgnoreCase));
                processor.Normalization(channel5FilePath, normalizationDataFolder,
                    radiometricRescaling.RadianceMultBand5, radiometricRescaling.RadianceAddBand5,
                    imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                    minMaxRadiance.RadianceMaximumBand5, minMaxReflectance.ReflectanceMaximumBand5);

                var channel6FilePath = fileNames.Single(name => name.EndsWith("B6.TIF", StringComparison.OrdinalIgnoreCase));
                processor.Normalization(channel6FilePath, normalizationDataFolder,
                    radiometricRescaling.RadianceMultBand6, radiometricRescaling.RadianceAddBand6,
                    imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                    minMaxRadiance.RadianceMaximumBand6, minMaxReflectance.ReflectanceMaximumBand6);

                var channel7FilePath = fileNames.Single(name => name.EndsWith("B7.TIF", StringComparison.OrdinalIgnoreCase));
                processor.Normalization(channel7FilePath, normalizationDataFolder,
                    radiometricRescaling.RadianceMultBand7, radiometricRescaling.RadianceAddBand7,
                    imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                    minMaxRadiance.RadianceMaximumBand7, minMaxReflectance.ReflectanceMaximumBand7);

                var channel8FilePath = fileNames.Single(name => name.EndsWith("B8.TIF", StringComparison.OrdinalIgnoreCase));
                processor.Normalization(channel8FilePath, normalizationDataFolder,
                    radiometricRescaling.RadianceMultBand8, radiometricRescaling.RadianceAddBand8,
                    imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                    minMaxRadiance.RadianceMaximumBand8, minMaxReflectance.ReflectanceMaximumBand8);

                var channel9FilePath = fileNames.Single(name => name.EndsWith("B9.TIF", StringComparison.OrdinalIgnoreCase));
                processor.Normalization(channel9FilePath, normalizationDataFolder,
                    radiometricRescaling.RadianceMultBand9, radiometricRescaling.RadianceAddBand9,
                    imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                    minMaxRadiance.RadianceMaximumBand9, minMaxReflectance.ReflectanceMaximumBand9);

                #endregion

                await _busManager.Send<IDataNormalizationResponse>(BusQueueConstants.DataNormalizationResponsesQueueName, new
                {
                    RequestId = request.RequestId
                });

                Logger.Info($"Запрос обработан (RequestId = {request.RequestId})");
            }
            else
            {
                throw new ArgumentException($"Папка {request.Folder} не найдена");
            }
        }

        private Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>> GetBusConfigurations()
        {
            var busConfig = new Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>>
            {
                {
                    BusQueueConstants.DataNormalizationRequestQueueName, e =>
                    {
                        e.Handler<IDataNormalizationRequest>(context => ProcessRequest(context.Message));
                    }
                }
            };
            return busConfig;
        }
    }
}
