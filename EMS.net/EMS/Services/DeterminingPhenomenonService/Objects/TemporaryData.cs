using System.Collections.Generic;
using Common.Objects.Landsat;
using Isodata.Objects;

namespace DeterminingPhenomenonService.Objects
{
    public class TemporaryData
    {
        public LandsatDataDescription DataDescription { get; set; }

        public List<Cluster> Clusters;

        public LandsatCuttedBuffers Buffers { get; set; }
    }
}
