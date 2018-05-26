namespace Common.Objects.Landsat
{
    /// <summary>
    /// Описание файла снимка ландсата
    /// </summary>
    public class LandsatSnapshotDescription
    {
        /// <summary>
        /// Абсолютный путь к сырому файлу
        /// </summary>
        public string Raw { get; set; }

        /// <summary>
        /// Абсолютный путь к нормализованному файлу
        /// </summary>
        public string Normalized { get; set; }
    }
}
