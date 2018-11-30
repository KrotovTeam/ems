using System;
using System.Collections.Generic;
using BusContracts;
using Common.Constants;
using Common.Enums;

namespace CharacterizationServiceStub
{
    class Program
    {
        static void Main(string[] args)
        {
            var busManager = new BusManager.BusManager();

            var message = new
            {
                RequestId = Guid.NewGuid().ToString("N"),
                PhenomenonType = PhenomenonType.ForestPlantationsDeseases,
                Characteristics = new List<object>
                {
                    new
                    {
                        SatelliteType = SatelliteType.Landsat8,
                        DataFolder = @"C:\Users\User\Downloads\Карпаты2\185026_20160826",
                        ResultFolder = @"C:\Users\User\Downloads\Карпаты2\resultKarpati2015-2016\Characteristics",
                        CharacteristicType = CharacteristicType.NDWI
                    }
                },
                LeftUpper = new
                {
                    Latitude = 49.2215,
                    Longitude = 24.0042
                },
                RigthLower = new
                {
                    Latitude = 48.7891,
                    Longitude = 24.6305
                }
            };

            busManager.Send<ISpectralСharacterizationRequest>(BusQueueConstants.CharacterizationRequestQueueName, message).Wait();
        }
    }
}
