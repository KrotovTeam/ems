using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusContracts;
using Common.Constants;
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

        private Task Process(IСharacterizationRequest request)
        {
            Logger.Info($"Получен запрос (RequestId = {request.RequestId})");
            return Task.FromResult(true);
        }

        private Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>> GetBusConfigurations()
        {
            var busConfig = new Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>>
            {
                {
                    BusQueueConstants.CharacterizationRequestQueueName, e =>
                    {
                        e.Handler<IСharacterizationRequest>(context => Process(context.Message));
                    }
                }
            };
            return busConfig;
        }
    }
}
