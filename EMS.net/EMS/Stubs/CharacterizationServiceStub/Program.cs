﻿using System;
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
                        DataFolder = @"C:\Users\User\Downloads\Карпаты2\resultKarpati2015-2016",
                        ResultFolder = @"C:\Users\User\Downloads\Карпаты2\resultKarpati2015-2016\Characteristics",
                        CharacteristicType = CharacteristicType.AreaOfDamage
                    }
                },
                LeftUpper = new
                {
                    Latitude = 45.5427,
                    Longitude = 32.4476
                },
                RigthLower = new
                {
                    Latitude = 44.5022,
                    Longitude = 34.6833
                }
            };

            busManager.Send<IСharacterizationRequest>(BusQueueConstants.CharacterizationRequestQueueName, message).Wait();
        }
    }
}
