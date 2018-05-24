using System;
using System.Collections.Generic;
using System.Linq;
using Isodata.Abstraction;
using Isodata.Helpers;
using Isodata.Objects;

namespace Isodata
{
    public class ClusteringManager
    {
        #region Fields

        /// <summary>
        /// Кластеры
        /// </summary>
        private List<Cluster> _z;

        /// <summary>
        /// Среднее расстояние между объектами входящих в кластер
        /// </summary>
        private readonly List<double> _dj = new List<double>();

        /// <summary>
        /// Обобщенное среднее расстояние между объектами, находящимися в отдельных кластерах, и соответствующими
        /// центрами кластеров
        /// </summary>
        private double _d;

        /// <summary>
        /// Вектор среднеквадратичного отколнения
        /// </summary>
        private readonly List<double[]> _sigmaj = new List<double[]>();

        /// <summary>
        /// Максимальная компонента в векторе среднеквадратичного отклонения
        /// </summary>
        private readonly List<Tuple<int, double>> _sigmajMax = new List<Tuple<int, double>>();

        /// <summary>
        /// Расстояния между всеми парами кластеров
        /// </summary>
        private readonly List<Tuple<Cluster, Cluster, double>> _dij = new List<Tuple<Cluster, Cluster, double>>();

        /// <summary>
        /// Данные для кластеризации
        /// </summary>
        private IIsodataPointsReader _isodataPointsReader;

        /// <summary>
        /// Профайл алгоритма
        /// </summary>
        private IIsodataProfile _profile;

        #endregion

        #region Clustering

        /// <summary>
        /// Кластеризация данных методом Isodata
        /// </summary>
        /// <param name="isodataPointsReader">Данные для кластеризации</param>
        /// <param name="profile">Профайл с параметрами кластеризации</param>
        /// <returns></returns>
        public List<Cluster> Process(IIsodataPointsReader isodataPointsReader, IIsodataProfile profile)
        {
            _isodataPointsReader = isodataPointsReader;
            _profile = profile;

            //Шаг 1 алгоритма - Выбор начальных центров кластеров
            _z = Init();

            for (var i = 1; i <= _profile.I; i++)
            {
                //Обнуляем точки кластеров
                foreach (var cluster in _z)
                {
                    cluster.PointValues = new List<double[]>();
                    GC.Collect();
                }

                //Шаг 2 алгоритма - Определение для каждой точки ближайшего кластера
                _isodataPointsReader.Foreach((values, x, y) =>
                {
                    var perfectCluster = _z.OrderBy(cluster => MathHelper.EuclideanDistance(values, cluster.CenterCluster)).First();
                    lock (perfectCluster)
                    {
                        perfectCluster.PointValues.Add(values);
                    }
                });

                //Шаг 3 алгоритма - Удаление кластеров с кол-вом точек меньшим _tettaN
                var clustersToDelete = _z.Where(p => p.PointValues.Count < _profile.TettaN).ToList();
                foreach (var cluster in clustersToDelete)
                {
                    _z.Remove(cluster);
                }

                //Шаг 4 алгоритма - Обновление центров кластеров
                foreach (var cluster in _z)
                {
                    cluster.CenterCluster = new double[_isodataPointsReader.DimensionalityCount];
                    for (var s = 0; s < _isodataPointsReader.DimensionalityCount; s++)
                    {
                        double sum = 0;
                        foreach (var item in cluster.PointValues)
                        {
                            sum += item[s];
                        }
                        cluster.CenterCluster[s] = sum / cluster.PointValues.Count;
                    }
                }

                //Шаг 5 алгоритма - Рассчитывается среднее расстояния _dj от каждого элемента кластера до центра кластера
                _dj.Clear();
                foreach (var cluster in _z)
                {
                    double sum = 0;
                    foreach (var item in cluster.PointValues)
                    {
                        sum += MathHelper.EuclideanDistance(item, cluster.CenterCluster);
                    }
                    var value = sum / cluster.PointValues.Count;
                    _dj.Add(value);
                }

                //Шаг 6 алгоритма - Вычисляется общее среднее расстояние от элементов до центра соответствующего кластера
                _d = 0;
                for (var k = 0; k < _z.Count; k++)
                {
                    _d += _z[k].PointValues.Count * _dj[k];
                }
                _d /= _isodataPointsReader.PointsCount;

                //Шаг 7 алгоритма
                if (i == _profile.I)
                {
                    _profile.TettaC = 0;
                    Step11();
                }
                else if (_z.Count > 2 * profile.ClustersCount || i % 2 == 0)
                {
                    Step11();
                }
                else
                {
                    if (Step8())
                    {
                        continue;
                    }
                    Step11();
                }
            }

            _z.ForEach(c => c.PointValues.Clear());
            GC.Collect();

            return _z;
        }

        /// <summary>
        /// 8-й шаг алгоритма
        /// </summary>
        private bool Step8()
        {
            _sigmaj.Clear();
            _sigmajMax.Clear();

            //Шаг 8 - Находится вектор стандартного отклонения для каждого кластера
            foreach (var cluster in _z)
            {
                var values = new double[_isodataPointsReader.DimensionalityCount];
                for (var k = 0; k < _isodataPointsReader.DimensionalityCount; k++)
                {
                    double sum = 0;
                    foreach (var point in cluster.PointValues)
                    {
                        sum += Math.Pow(point[k] - cluster.CenterCluster[k], 2);
                    }
                    values[k] = Math.Sqrt(sum / cluster.PointValues.Count);
                }
                _sigmaj.Add(values);
            }

            //Шаг 9 - Нахождение максимального _sigmaj
            for (var i = 0; i < _z.Count; i++)
            {
                var max = double.MinValue;
                var index = 0;
                for (var k = 0; k < _isodataPointsReader.DimensionalityCount; k++)
                {
                    if (_sigmaj[i][k] > max)
                    {
                        max = _sigmaj[i][k];
                        index = k;
                    }
                }
                _sigmajMax.Add(new Tuple<int, double>(index, max));
            }

            //Шаг 10 - Расщепление кластера
            for (var i = 0; i < _z.Count; i++)
            {
                if (_sigmajMax[i].Item2 > _profile.TettaS && (_dj[i] > _d && _z[i].PointValues.Count > 2 * (_profile.TettaN + 1) || _z.Count <= _profile.ClustersCount / 2))
                {
                    var gammaj = _profile.Coefficient * _sigmajMax[i].Item2;

                    var newCluster = new Cluster
                    {
                        CenterCluster = new double[_isodataPointsReader.DimensionalityCount]
                    };
                    for (var k = 0; k < _isodataPointsReader.DimensionalityCount; k++)
                    {
                        newCluster.CenterCluster[k] = _z[i].CenterCluster[k];
                    }

                    //zj+
                    _z[i].CenterCluster[_sigmajMax[i].Item1] = _z[i].CenterCluster[_sigmajMax[i].Item1] + gammaj;

                    //zj-
                    newCluster.CenterCluster[_sigmajMax[i].Item1] = newCluster.CenterCluster[_sigmajMax[i].Item1] - gammaj;
                    _z.Add(newCluster);

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 11-й шаг алгоритма
        /// </summary>
        private void Step11()
        {
            //Шаг 11 - Рассчет эвклидова расстояния между всеми кластерами
            for (var i = 0; i < _z.Count - 1; i++)
            {
                for (var j = i + 1; j < _z.Count - 1; j++)
                {
                    var tuple = new Tuple<Cluster, Cluster, double>(_z[i], _z[j], MathHelper.EuclideanDistance(_z[i].CenterCluster, _z[j].CenterCluster));
                    _dij.Add(tuple);
                }
                _z[i].IsJoined = false;
            }

            //Шаг 12 - Определение пар кластеров для объединения
            var clustersForJoin = _dij.Where(p => p.Item3 < _profile.TettaC).OrderBy(p => p.Item3).Take(_profile.L);

            //Шаг 13 - Объединение кластеров
            foreach (var cluster in clustersForJoin)
            {
                if (!cluster.Item1.IsJoined && !cluster.Item2.IsJoined)
                {
                    cluster.Item1.IsJoined = cluster.Item2.IsJoined = true;

                    var newCluster = new Cluster
                    {
                        CenterCluster = new double[_isodataPointsReader.DimensionalityCount]
                    };
                    for (var k = 0; k < _isodataPointsReader.DimensionalityCount; k++)
                    {
                        var value1 = cluster.Item1.CenterCluster[k] * cluster.Item1.PointValues.Count;
                        var value2 = cluster.Item2.CenterCluster[k] * cluster.Item2.PointValues.Count;
                        newCluster.CenterCluster[k] = (value1 + value2) / (cluster.Item1.PointValues.Count + cluster.Item2.PointValues.Count);
                    }

                    _z.Add(newCluster);
                    _z.Remove(cluster.Item1);
                    _z.Remove(cluster.Item2);
                }
            }
        }

        /// <summary>
        /// Реализация первого шага алгоритма Isodata.
        /// Определение начальных центров кластеров.
        /// </summary>
        /// <returns></returns>
        private List<Cluster> Init()
        {
            var step = _isodataPointsReader.MaxValue / _profile.ClustersCount;
            var clusters = new List<Cluster>();
            var i = 0.0;
            while (i < _isodataPointsReader.MaxValue)
            {
                var centerCluster = new double[_isodataPointsReader.DimensionalityCount];
                for (var j = 0; j < _isodataPointsReader.DimensionalityCount; j++)
                {
                    centerCluster[j] = i;
                }
                clusters.Add(new Cluster
                {
                    CenterCluster = centerCluster
                });
                i += step;
            }
            return clusters;
        }

        #endregion
    }
}
