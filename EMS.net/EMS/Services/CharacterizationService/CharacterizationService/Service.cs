using System;
using System.Collections.Generic;
using BusContracts;
using Common.Constants;
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

        private Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>> GetBusConfigurations()
        {
            var busConfig = new Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>>
            {
                {
                    "queue_name", e =>
                    {
                    }
                }
            };
            return busConfig;
        }
    }
}
