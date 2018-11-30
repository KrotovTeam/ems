using BusContracts;

namespace DeterminingPhenomenonService.Objects
{
    public class DeterminingPhenomenonResponse: IDeterminingPhenomenonResponse
    {
        public bool IsDetermined { get; set; }

        public string RequestId { get; set; }
    }
}
