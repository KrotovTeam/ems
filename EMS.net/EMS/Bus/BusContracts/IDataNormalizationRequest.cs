﻿using Common.Enums;

namespace BusContracts
{
    public interface IDataNormalizationRequest
    {
        /// <summary>
        /// Папка где находятся сырые данные
        /// Именно изображения
        /// </summary>
        string Folder { get; set; }

        /// <summary>
        /// Тип спутника
        /// </summary>
        SatelliteType SatelliteType { get; set; }
    }
}
