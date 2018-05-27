using BusContracts;
using Common.Enums;

namespace CharacterizationService.Objects.CharacterizationResponse
{
    public class CharacteristicResult : ICharacteristicResult
    {
        public CharacteristicType CharacteristicType { get; set; }
        public string[] Paths { get; set; }
    }
}
