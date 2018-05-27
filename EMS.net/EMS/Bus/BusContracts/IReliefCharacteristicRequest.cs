using Common.Enums;
using Common.Objects;

namespace BusContracts
{
    public interface IReliefCharacteristicRequest
    {
        /// <summary>
        /// Верхняя левая точка
        /// </summary>
        IGeographicPoint LeftUpper { get; set; }

        /// <summary>
        /// Нижняя правая точка
        /// </summary>
        IGeographicPoint RightLower { get; set; }

        /// <summary>
        /// Папка с данными (абсолютный путь)
        /// </summary>
        string DataFolder { get; set; }

        /// <summary>
        /// Папка для сохранения результата (абсолютный путь)
        /// </summary>
        string ResultFolder { get; set; }

        /// <summary>
        /// Типы необходимых характеристик
        /// </summary>
        ReliefCharacteristicType[] CharacteristicTypes { get; set; }
    }
}
