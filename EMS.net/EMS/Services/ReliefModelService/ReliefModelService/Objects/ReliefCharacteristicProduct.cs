using BusContracts;
using Common.Enums;

namespace ReliefModelService.Objects
{
    public class ReliefCharacteristicProduct : IReliefCharacteristicProduct
    {
        public string FilePath { get; set; }
        public ReliefCharacteristicType Type { get; set; }
    }
}
