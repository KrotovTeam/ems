using Common.Enums;

namespace BusContracts
{
    public interface IReliefCharacteristicProduct
    {
        /// <summary>
        /// Путь к файлу
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// Тип характеристики
        /// </summary>
        ReliefCharacteristicType Type { get; set; }
    }
}
