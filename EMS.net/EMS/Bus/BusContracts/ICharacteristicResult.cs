using Common.Enums;

namespace BusContracts
{
    public interface ICharacteristicResult
    {
        /// <summary>
        /// Тип характеристики/условия распространения явления
        /// </summary>
        CharacteristicType CharacteristicType { get; set; }

        /// <summary>
        /// Абсолютные путь к файлам результатам
        /// </summary>
        string[] Paths { get; set; }
    }
}
