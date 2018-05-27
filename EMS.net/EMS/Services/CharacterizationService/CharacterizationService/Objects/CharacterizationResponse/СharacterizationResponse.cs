﻿using System.Collections.Generic;
using BusContracts;

namespace CharacterizationService.Objects.CharacterizationResponse
{
    public class СharacterizationResponse : IСharacterizationResponse
    {
        public string RequestId { get; set; }
        public List<ICharacteristicResult> Results { get; set; }
    }
}
