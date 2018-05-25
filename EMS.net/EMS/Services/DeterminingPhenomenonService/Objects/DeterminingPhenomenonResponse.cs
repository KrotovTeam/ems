using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusContracts;

namespace DeterminingPhenomenonService
{
    public class DeterminingPhenomenonResponse: IDeterminingPhenomenonResponse
    {
        public bool IsDetermined { get; set; }

        public string RequestId { get; set; }
    }
}
