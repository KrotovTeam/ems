using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeterminingPhenomenonService.Objects
{
    public class UtmPolygon
    {
        public UtmPoint UpperLeft;

        public UtmPoint LowerRight;

        public UtmPolygon()
        {
            UpperLeft = new UtmPoint();
            LowerRight = new UtmPoint();
        }
    }
}
