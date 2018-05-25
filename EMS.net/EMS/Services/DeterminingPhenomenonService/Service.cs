using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusContracts;
using Topshelf;
using Common.Constants;
using Common.Enums;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace DeterminingPhenomenonService
{
    public class Service: ServiceControl
    {
        private BusManager.BusManager _busManager;

        public bool Start(HostControl hostControl)
        {
            _busManager = new BusManager.BusManager();
            _busManager.StartBus(GetBusConfigurations());

            _busManager.Send<IDeterminingPhenomenonResponse>("zalupa", new { }).Wait();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _busManager.StopBus();

            return true;
        }

        private async Task ProcessRequest(IDeterminingPhenomenonRequest request)
        {
            var processor = new DeterminingPhenomenonProcessor();
            processor.Phenomenon = request.Phenomenon;
            processor.Coordinates = new Dictionary<ImageCorner, double[]>()
            {
                {ImageCorner.UpperLeft, new []{request.LeftUpper.Latitude, request.LeftUpper.Longitude}},
                {ImageCorner.LowerRight, new[]{request.RightLower.Latitude, request.RightLower.Longitude}},
            };
            processor.ResultFolder = request.ResultFolder;
            processor.DataFolders = request.DataFolders;

            var response = new DeterminingPhenomenonResponse();

            response.RequestId = request.RequestId;
            response.IsDetermined = processor.Proccess();

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
