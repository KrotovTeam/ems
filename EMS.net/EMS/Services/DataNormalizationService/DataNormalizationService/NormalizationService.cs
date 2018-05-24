using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BusContracts;
using BusContracts.Implementation;
using Common.Constants;
using Common.Enums;
using Common.Objects;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Newtonsoft.Json;
using Topshelf;

namespace DataNormalizationService
{
    public class NormalizationService : ServiceControl
    {
        private BusManager.BusManager _busManager;

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
            if (request.SatelliteType != SatelliteType.Landsat8)
            {
                throw new ArgumentException("Неверно задан тип спутника");
            }

            if (Directory.Exists(request.Folder))
            {
                var fileNames = Directory.GetFiles(request.Folder);

                var metadataFileName = fileNames.Single(name => name.EndsWith("mlt.json", StringComparison.CurrentCultureIgnoreCase));
                LandsatMetadata metadata = null;
                using (var metadataFileStream = new FileStream(metadataFileName, FileMode.Open))
                using (var reader = new StreamReader(metadataFileStream))
                {
                    metadata = JsonConvert.DeserializeObject<LandsatMetadata>(reader.ReadToEnd());
                }

                LandsatNormalizationProcessor processor = new LandsatNormalizationProcessor();

                var channel1 = fileNames.Single(name => name.EndsWith("B1.TIF", StringComparison.CurrentCultureIgnoreCase));
                processor.Normalization(channel1, request.Folder,
                    metadata.RadiometricRescaling.RadianceMultBand1, metadata.RadiometricRescaling.RadianceAddBand1,
                    metadata.ImageAttributes.SunElevation, metadata.ImageAttributes.EarthSunDistance,
                    metadata.MinMaxRadiance.RadianceMaximumBand1, metadata.MinMaxReflectance.ReflectanceMaximumBand1);

                var channel2 = fileNames.Single(name => name.EndsWith("B2.TIF", StringComparison.CurrentCultureIgnoreCase));
                processor.Normalization(channel2, request.Folder,
                    metadata.RadiometricRescaling.RadianceMultBand2, metadata.RadiometricRescaling.RadianceAddBand2,
                    metadata.ImageAttributes.SunElevation, metadata.ImageAttributes.EarthSunDistance,
                    metadata.MinMaxRadiance.RadianceMaximumBand2, metadata.MinMaxReflectance.ReflectanceMaximumBand2);

                var channel3 = fileNames.Single(name => name.EndsWith("B3.TIF", StringComparison.CurrentCultureIgnoreCase));
                processor.Normalization(channel3, request.Folder,
                    metadata.RadiometricRescaling.RadianceMultBand3, metadata.RadiometricRescaling.RadianceAddBand3,
                    metadata.ImageAttributes.SunElevation, metadata.ImageAttributes.EarthSunDistance,
                    metadata.MinMaxRadiance.RadianceMaximumBand3, metadata.MinMaxReflectance.ReflectanceMaximumBand3);

                var channel4 = fileNames.Single(name => name.EndsWith("B4.TIF", StringComparison.CurrentCultureIgnoreCase));
                processor.Normalization(channel4, request.Folder,
                    metadata.RadiometricRescaling.RadianceMultBand4, metadata.RadiometricRescaling.RadianceAddBand4,
                    metadata.ImageAttributes.SunElevation, metadata.ImageAttributes.EarthSunDistance,
                    metadata.MinMaxRadiance.RadianceMaximumBand4, metadata.MinMaxReflectance.ReflectanceMaximumBand4);

                var channel5 = fileNames.Single(name => name.EndsWith("B5.TIF", StringComparison.CurrentCultureIgnoreCase));
                processor.Normalization(channel5, request.Folder,
                    metadata.RadiometricRescaling.RadianceMultBand5, metadata.RadiometricRescaling.RadianceAddBand5,
                    metadata.ImageAttributes.SunElevation, metadata.ImageAttributes.EarthSunDistance,
                    metadata.MinMaxRadiance.RadianceMaximumBand5, metadata.MinMaxReflectance.ReflectanceMaximumBand5);

                var channel6 = fileNames.Single(name => name.EndsWith("B6.TIF", StringComparison.CurrentCultureIgnoreCase));
                processor.Normalization(channel6, request.Folder,
                    metadata.RadiometricRescaling.RadianceMultBand6, metadata.RadiometricRescaling.RadianceAddBand6,
                    metadata.ImageAttributes.SunElevation, metadata.ImageAttributes.EarthSunDistance,
                    metadata.MinMaxRadiance.RadianceMaximumBand6, metadata.MinMaxReflectance.ReflectanceMaximumBand6);

                var channel7 = fileNames.Single(name => name.EndsWith("B7.TIF", StringComparison.CurrentCultureIgnoreCase));
                processor.Normalization(channel7, request.Folder,
                    metadata.RadiometricRescaling.RadianceMultBand7, metadata.RadiometricRescaling.RadianceAddBand7,
                    metadata.ImageAttributes.SunElevation, metadata.ImageAttributes.EarthSunDistance,
                    metadata.MinMaxRadiance.RadianceMaximumBand7, metadata.MinMaxReflectance.ReflectanceMaximumBand7);

                var channel8 = fileNames.Single(name => name.EndsWith("B8.TIF", StringComparison.CurrentCultureIgnoreCase));
                processor.Normalization(channel8, request.Folder,
                    metadata.RadiometricRescaling.RadianceMultBand8, metadata.RadiometricRescaling.RadianceAddBand8,
                    metadata.ImageAttributes.SunElevation, metadata.ImageAttributes.EarthSunDistance,
                    metadata.MinMaxRadiance.RadianceMaximumBand8, metadata.MinMaxReflectance.ReflectanceMaximumBand8);

                var channel9 = fileNames.Single(name => name.EndsWith("B9.TIF", StringComparison.CurrentCultureIgnoreCase));
                processor.Normalization(channel9, request.Folder,
                    metadata.RadiometricRescaling.RadianceMultBand9, metadata.RadiometricRescaling.RadianceAddBand9,
                    metadata.ImageAttributes.SunElevation, metadata.ImageAttributes.EarthSunDistance,
                    metadata.MinMaxRadiance.RadianceMaximumBand9, metadata.MinMaxReflectance.ReflectanceMaximumBand9);

                await _busManager.Send<IDataNormalizationResponse>(BusQueueConstants.DataNormalizationResponsesQueueName, new DataNormalizationResponse
                {
                    Folder = request.Folder
                });
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
