namespace Common.Enums
{
    public enum SatelliteType : byte
    {
        Unknown = 0,

        /// <summary>
        /// Ландсат-8
        /// </summary>
        Landsat8 = 1,

        /// <summary>
        /// Сентинель-2
        /// </summary>
        Sentinel2 = 2,

        /// <summary>
        /// Модис
        /// </summary>
        Modis = 3,

        /// <summary>
        /// СРТМ
        /// </summary>
        Srtm = 4
    }
}
