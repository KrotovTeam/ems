using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusContracts;
using Common.Constants;
using Common.Enums;
using Common.Helpers;
using Common.Objects.Landsat;
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

            var folderDescription = new LandsatDataDescription(request.Folder);

            LandsatMetadata metadataFile = JsonHelper.Deserialize<LandsatMetadata>(folderDescription.MetadataMtlJson);
            LandsatNormalizationProcessor processor = new LandsatNormalizationProcessor(Logger);
            RadiometricRescaling radiometricRescaling = metadataFile.L1MetadataFile.RadiometricRescaling;
            ImageAttributes imageAttributes = metadataFile.L1MetadataFile.ImageAttributes;
            MinMaxRadiance minMaxRadiance = metadataFile.L1MetadataFile.MinMaxRadiance;
            MinMaxReflectance minMaxReflectance = metadataFile.L1MetadataFile.MinMaxReflectance;

            #region Channel normalization

            var normalizationDataFolder = $@"{request.Folder}{FilenamesConstants.PathToNormalizedDataFolder}";

            processor.Normalization(folderDescription.Channel1.Raw, normalizationDataFolder,
                radiometricRescaling.RadianceMultBand1, radiometricRescaling.RadianceAddBand1,
                imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                minMaxRadiance.RadianceMaximumBand1, minMaxReflectance.ReflectanceMaximumBand1);

            processor.Normalization(folderDescription.Channel2.Raw, normalizationDataFolder,
                radiometricRescaling.RadianceMultBand2, radiometricRescaling.RadianceAddBand2,
                imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                minMaxRadiance.RadianceMaximumBand2, minMaxReflectance.ReflectanceMaximumBand2);

            processor.Normalization(folderDescription.Channel3.Raw, normalizationDataFolder,
                radiometricRescaling.RadianceMultBand3, radiometricRescaling.RadianceAddBand3,
                imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                minMaxRadiance.RadianceMaximumBand3, minMaxReflectance.ReflectanceMaximumBand3);

            processor.Normalization(folderDescription.Channel4.Raw, normalizationDataFolder,
                radiometricRescaling.RadianceMultBand4, radiometricRescaling.RadianceAddBand4,
                imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                minMaxRadiance.RadianceMaximumBand4, minMaxReflectance.ReflectanceMaximumBand4);

            processor.Normalization(folderDescription.Channel5.Raw, normalizationDataFolder,
                radiometricRescaling.RadianceMultBand5, radiometricRescaling.RadianceAddBand5,
                imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                minMaxRadiance.RadianceMaximumBand5, minMaxReflectance.ReflectanceMaximumBand5);

            processor.Normalization(folderDescription.Channel6.Raw, normalizationDataFolder,
                radiometricRescaling.RadianceMultBand6, radiometricRescaling.RadianceAddBand6,
                imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                minMaxRadiance.RadianceMaximumBand6, minMaxReflectance.ReflectanceMaximumBand6);

            processor.Normalization(folderDescription.Channel7.Raw, normalizationDataFolder,
                radiometricRescaling.RadianceMultBand7, radiometricRescaling.RadianceAddBand7,
                imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                minMaxRadiance.RadianceMaximumBand7, minMaxReflectance.ReflectanceMaximumBand7);

            processor.Normalization(folderDescription.Channel8.Raw, normalizationDataFolder,
                radiometricRescaling.RadianceMultBand8, radiometricRescaling.RadianceAddBand8,
                imageAttributes.SunElevation, imageAttributes.EarthSunDistance,
                minMaxRadiance.RadianceMaximumBand8, minMaxReflectance.ReflectanceMaximumBand8);

            processor.Normalization(folderDescription.Channel9.Raw, normalizationDataFolder,
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
