using BusContracts;

namespace ReliefModelService.Objects
{
    public class ReliefCharacteristicResponse : IReliefCharacteristicResponse
    {
        public IReliefCharacteristicProduct[] Products { get; set; }
    }
}
