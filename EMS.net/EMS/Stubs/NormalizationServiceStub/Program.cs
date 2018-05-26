using System;
using BusContracts;
using Common.Constants;
using Common.Enums;

namespace NormalizationServiceStub
{
    class Program
    {
        static void Main(string[] args)
        {
            var busManager = new BusManager.BusManager();

            var message = new 
            {
                RequestId = Guid.NewGuid().ToString("N"),
                Folder = @"F:\КАРПАТЫV2\185026_20150824",
                SatelliteType = SatelliteType.Landsat8
            };

            busManager.Send<IDataNormalizationRequest>(BusQueueConstants.DataNormalizationRequestQueueName, message).Wait();
        }
    }
}
