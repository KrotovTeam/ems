using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.PointsReaders.Interfaces;

namespace Common.PointsReaders
{
    public class LandsatIsodataPointsReader : IPointsReader, IDisposable
    {
        public int Width { get; set; }
        public int Heigth { get; set; }

        public long PointsCount { get; }
        public double MaxValue { get; }
        public byte DimensionalityCount { get; }

        private readonly LandsatNormilizedSnapshotReader[] _readers;

        public LandsatIsodataPointsReader(params string[] fileNames)
        {
            _readers = new LandsatNormilizedSnapshotReader[fileNames.Length];
            for (var i = 0; i < fileNames.Length; i++)
            {
                _readers[i] = new LandsatNormilizedSnapshotReader(fileNames[i]);
            }

            Width = _readers[0].Width;
            Heigth = _readers[0].Heigth;
            DimensionalityCount = (byte)fileNames.Length;
            PointsCount = Width * Heigth;
            MaxValue = 1;
        }

        public void Foreach(Action<double[], int, int> action)
        {
            var buffer = new double[DimensionalityCount][];
            for (var x = 0; x < Heigth; x++)
            {
                for (var t = 0; t < DimensionalityCount; t++)
                {
                    buffer[t] = _readers[t].ReadScanline(x);
                }


                Parallel.For(0, Width, y =>
                {
                    var values = new double[DimensionalityCount];
                
                    for (var t = 0; t < DimensionalityCount; t++)
                    {
                        values[t] = buffer[t][y];
                    }

                    //Особенности считывания
                    action(values, y, x);
                });
            }
        }

        ~LandsatIsodataPointsReader()
        {
            for (var i = 0; i < DimensionalityCount; i++)
            {
                _readers[i].Dispose();
            }
        }

        public void Dispose()
        {
            for (var i = 0; i < DimensionalityCount; i++)
            {
                _readers[i].Dispose();
            }
        }
    }
}
