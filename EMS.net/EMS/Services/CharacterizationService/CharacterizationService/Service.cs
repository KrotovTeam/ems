using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusContracts;
using CharacterizationService.Abstraction;
using CharacterizationService.Objects.CharacterizationResponse;
using CharacterizationService.Processors;
using CharacterizationService.Processors.AreaOfDamage;
using CharacterizationService.Processors.NDWI;
using CharacterizationService.Processors.Temperature;
using Common.Constants;
using Common.Enums;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Topshelf;
using Topshelf.Logging;

namespace CharacterizationService
{
    public class Service : ServiceControl
    {
        private BusManager.BusManager _busManager;
        private static readonly LogWriter Logger = HostLogger.Get<Service>();

        private readonly Dictionary<CharacteristicType, AbstractCharacterizationProcessor> _processorsDictionary =
            new Dictionary<CharacteristicType, AbstractCharacterizationProcessor>
            {
                //{CharacteristicType.AreaOfDamage, new AreaOfDamageCharacterizationProcessor(Logger) },
                //{CharacteristicType.DigitalReliefModel, new DigitalReliefModelCharacterizationProcessor(Logger)}
                {CharacteristicType.Temperature,  new TemperatureCharacterizationProcessor(Logger)},
                {CharacteristicType.NDWI,  new NDWICharacterizationProcessor(Logger)}
            };

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

        private async Task Process(ISpectralСharacterizationRequest request)
        {
            Logger.Info($"Получен запрос (RequestId = {request.RequestId})");

            var response = new SpectralСharacterizationResponse
            {
                RequestId = request.RequestId,
                Results = new List<ICharacteristicResult>()
            };

            foreach (var characteristic in request.Characteristics)
            {
                Logger.Info($"{characteristic.CharacteristicType.GetDescription()}...");
                var result = new CharacteristicResult
                {
                    CharacteristicType = characteristic.CharacteristicType,
                    Paths = _processorsDictionary[characteristic.CharacteristicType].Process(request.LeftUpper,
                        request.RigthLower,
                        characteristic.DataFolder, characteristic.ResultFolder)
                };
                response.Results.Add(result);
            }

            await _busManager.Send<ISpectralСharacterizationResponse>(BusQueueConstants.CharacterizationResponseQueueName, response);

            Logger.Info($"Запрос обработан (RequestId = {request.RequestId})");
        }

        private Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>> GetBusConfigurations()
        {
            var busConfig = new Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>>
            {
                {
                    BusQueueConstants.CharacterizationRequestQueueName, e =>
                    {
                        e.Handler<ISpectralСharacterizationRequest>(context => Process(context.Message));
                    }
                }
            };
            return busConfig;
        }
    }
}
