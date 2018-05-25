using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusContracts;
using Common.Constants;
using Common.Enums;
using MassTransit;
using MassTransit.RabbitMqTransport;
using ReliefModelService.Abstraction;
using ReliefModelService.Objects;
using ReliefModelService.Processors;
using Topshelf;

namespace ReliefModelService
{
    public class Service : ServiceControl
    {
        private readonly Dictionary<ReliefCharacteristicType, AbstractReliefCharacteristicProcessor> _processorsDictionary =
            new Dictionary<ReliefCharacteristicType, AbstractReliefCharacteristicProcessor>
            {
                { ReliefCharacteristicType.Exposition, new ExpositionReliefCharacterisitcProcessor() },
                { ReliefCharacteristicType.Skew, new SkewReliefCharacterisitcProcessor() },
                { ReliefCharacteristicType.Height, new HeigthReliefCharacterisitcProcessor() }
            };

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

        private async Task ProcessRequest(IReliefCharacteristicRequest request)
        {
            var dataset = new SrtmDataset(request.LeftUpper, request.RightLower, request.DataFolder);
            var response = new ReliefCharacteristicResponse
            {
                Products = new IReliefCharacteristicProduct[request.CharacteristicTypes.Length]
            };

            for (var i = 0; i < request.CharacteristicTypes.Length; i++)
            {
                response.Products[i] = _processorsDictionary[request.CharacteristicTypes[i]].Process(dataset, request.ResultFolder);
            }

            await _busManager.Send<IReliefCharacteristicResponse>(BusQueueConstants.ReliefModelResponsesQueueName, response);
        }

        private Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>> GetBusConfigurations()
        {
            var busConfig = new Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>>
            {
                {
                    BusQueueConstants.ReliefModelRequestQueueName, e =>
                    {
                        e.Handler<IReliefCharacteristicRequest>(context => ProcessRequest(context.Message));
                    }
                }
            };
            return busConfig;
        }
    }
}
