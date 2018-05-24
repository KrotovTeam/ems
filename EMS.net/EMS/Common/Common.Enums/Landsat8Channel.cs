namespace Common.Enums
{
    /// <summary>
    /// Каналы спутника Landsat-8 
    /// </summary>
    public enum Landsat8Channel : byte
    {
        /// <summary>
        /// Неизвестный
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// побережья и аэрозоли (Coastal / Aerosol, New Deep Blue)
        /// </summary>
        Channel1 = 1,

        /// <summary>
        /// синий (Blue)
        /// </summary>
        Channel2 = 2,

        /// <summary>
        /// зелёный (Green)
        /// </summary>
        Channel3 = 3,

        /// <summary>
        /// красный (Red)
        /// </summary>
        Channel4 = 4,

        /// <summary>
        /// ближний ИК (Near Infrared, NIR)
        /// </summary>
        Channel5 = 5,

        /// <summary>
        /// ближний ИК (Short Wavelength Infrared, SWIR 2)
        /// </summary>
        Channel6 = 6,

        /// <summary>
        /// ближний ИК (Short Wavelength Infrared, SWIR 3)
        /// </summary>
        Channel7 = 7,

        /// <summary>
        /// панхроматический (Panchromatic, PAN)
        /// </summary>
        Channel8 = 8,

        /// <summary>
        /// перистые облака (Cirrus, SWIR)
        /// </summary>
        Channel9 = 9,

        /// <summary>
        /// дальний ИК (Long Wavelength Infrared, TIR1)
        /// </summary>
        Channel10 = 10,

        /// <summary>
        /// дальний ИК (Long Wavelength Infrared, TIR2)
        /// </summary>
        Channel11 = 11
    }
}
