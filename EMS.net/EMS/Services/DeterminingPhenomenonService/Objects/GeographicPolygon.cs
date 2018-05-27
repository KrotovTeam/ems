using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Common.Objects;

namespace DeterminingPhenomenonService.Objects
{
    public class GeographicPolygon
    {
        public IGeographicPoint UpperLeft;
               
        public IGeographicPoint UpperRight;
               
        public IGeographicPoint LowerLeft;
               
        public IGeographicPoint LowerRight;

        public GeographicPolygon()
        {
            UpperLeft = new GeographicPoint();
            UpperRight = new GeographicPoint();
            LowerRight = new GeographicPoint();
            LowerLeft = new GeographicPoint();
        }

    }
}
