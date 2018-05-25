namespace BusContracts
{
    public interface IReliefCharacteristicResponse
    {
        /// <summary>
        /// Характеристики
        /// </summary>
        IReliefCharacteristicProduct[] Products { get; set; }
    }
}
