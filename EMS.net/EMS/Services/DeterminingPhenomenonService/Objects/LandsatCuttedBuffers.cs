using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;

namespace DeterminingPhenomenonService.Objects
{
    public class LandsatCuttedBuffers
    {
        public Dictionary<Landsat8Channel, double[,]> Channels { get; set; }

        public LandsatCuttedBuffers()
        {
            Channels = new Dictionary<Landsat8Channel, double[,]>()
            {
                {Landsat8Channel.Channel4, null },
                {Landsat8Channel.Channel5, null }
            };
        }
     }
}
