using System.ComponentModel;

namespace Common.Enums
{
    public enum CharacteristicType
    {
        [Description("")]
        Unknown = 0,

        [Description("Площадь повреждений")]
        AreaOfDamage = 1,

        [Description("Цифровая модель рельефа")]
        DigitalReliefModel = 2
    }
}
