namespace BusContracts
{
    public interface IDataNormalizationResponse
    {
        /// <summary>
        /// Папка с нормализованными данными
        /// </summary>
        string Folder { get; set; }
    }
}
