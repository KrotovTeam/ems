namespace BusContracts
{
    public interface IGeographicPoint
    {
        /// <summary>
        /// Широта
        /// </summary>
        double Latitude { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        double Longitude { get; set; }
    }
}
