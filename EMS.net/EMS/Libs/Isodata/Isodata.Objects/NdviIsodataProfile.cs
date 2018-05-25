using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isodata.Abstraction;
namespace Isodata.Objects
{
    public class NdviIsodataProfile : IIsodataProfile
    {
        public int ClustersCount => 20;
        public int TettaN => 1000;
        public double TettaS => 0.01;
        public double TettaC { get => 0.01; set { } }
        public int L => 5;
        public int I => 10;
        public double Coefficient => 1;
    }
}
