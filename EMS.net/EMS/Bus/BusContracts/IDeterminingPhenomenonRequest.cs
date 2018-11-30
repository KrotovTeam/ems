using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Common.Objects;

namespace BusContracts
{
    public interface IDeterminingPhenomenonRequest
    {  
        string ResultFolder { get; set; }

        IGeographicPoint LeftUpper { get; set; }

        IGeographicPoint RightLower { get; set; }

        PhenomenonType Phenomenon { get; set; }

        string[] DataFolders { get; set; }

        string RequestId { get; set; }
    }
}
