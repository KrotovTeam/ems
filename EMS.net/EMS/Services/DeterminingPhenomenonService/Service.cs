using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusContracts;
using Topshelf;
using Common.Constants;
using DeterminingPhenomenonService.Objects;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Topshelf.Logging;

namespace DeterminingPhenomenonService
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

        private async Task ProcessRequest(IDeterminingPhenomenonRequest request)
        {
            var polygon = new GeographicPolygon()
            {
                UpperLeft = request.LeftUpper,
                LowerRight = request.RightLower
            };

            var processor = new DeterminingPhenomenonProcessor(request.DataFolders, request.ResultFolder, polygon, request.Phenomenon);

            var response = new DeterminingPhenomenonResponse
            {
                RequestId = request.RequestId,
                IsDetermined = processor.Proccess()
            };


            await _busManager.Send<IDeterminingPhenomenonResponse>(BusQueueConstants.DeterminingPhenomenonResponsesQueueName, response);
        }

        private Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>> GetBusConfigurations()
        {
            var busConfig = new Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>>
            {
                {
                    BusQueueConstants.DeterminingPhenomenonRequestsQueueName, e =>
                    {
                        e.Handler<IDeterminingPhenomenonRequest>(context => ProcessRequest(context.Message));
                    }
                }
            };
            return busConfig;
        }
    }
}
