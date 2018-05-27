using Common.Enums;

namespace BusContracts
{
    public interface ICharacteristicDescription
    {
        /// <summary>
        /// Тип спутника
        /// </summary>
        SatelliteType SatelliteType { get; set; }

        /// <summary>
        /// Папка с данными
        /// </summary>
        string DataFolder { get; set; }

        /// <summary>
        /// Папка для результатов
        /// </summary>
        string ResultFolder { get; set; }

        /// <summary>
        /// Тип характеристики
        /// </summary>
        CharacteristicType CharacteristicType { get; set; }
    }
}
