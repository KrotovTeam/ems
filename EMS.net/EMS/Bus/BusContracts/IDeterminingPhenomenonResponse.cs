using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusContracts
{
    public interface IDeterminingPhenomenonResponse
    {
        bool IsDetermined { get; set; }

        string RequestId { get; set; }
    }
}
