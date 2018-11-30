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
                Folder = @"C:\Users\User\Downloads\Krim\2016-06-22",
                SatelliteType = SatelliteType.Landsat8
            };

            busManager.Send<IDataNormalizationRequest>(BusQueueConstants.DataNormalizationRequestQueueName, message).Wait();
        }
    }
}
