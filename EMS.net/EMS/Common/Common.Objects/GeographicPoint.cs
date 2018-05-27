using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;

namespace Common.Objects
{
    public class GeographicPoint:IGeographicPoint
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
