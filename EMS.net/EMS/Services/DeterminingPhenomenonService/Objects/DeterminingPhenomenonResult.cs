using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Objects;

namespace DeterminingPhenomenonService.Objects
{
    public class DeterminingPhenomenonResult
    {
        public CuttedImageInfo ProcessingImageInfo { get; set; }

        public bool IsDetermined { get; set; }
    }
}
