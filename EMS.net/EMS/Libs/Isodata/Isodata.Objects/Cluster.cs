using System.Collections.Generic;

namespace Isodata.Objects
{
    public class Cluster
    {
        /// <summary>
        /// Значения центра кластера
        /// </summary>
        public double[] CenterCluster { get; set; }

        /// <summary>
        /// Значения точек
        /// </summary>
        public List<double[]> PointValues { get; set; }

        /// <summary>
        /// Признак объединенности кластеров
        /// </summary>
        public bool IsJoined { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Cluster()
        {
            PointValues = new List<double[]>();
            IsJoined = false;
        }
    }
}
