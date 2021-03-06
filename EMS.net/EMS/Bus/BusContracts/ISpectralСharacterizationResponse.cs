﻿using System.Collections.Generic;

namespace BusContracts
{
    public interface ISpectralСharacterizationResponse
    {
        /// <summary>
        /// Идентификатор запроса
        /// </summary>
        string RequestId { get; set; }

        /// <summary>
        /// Результаты определения характеристики и условий распространения
        /// </summary>
        List<ICharacteristicResult> Results { get; set; }
    }
}
