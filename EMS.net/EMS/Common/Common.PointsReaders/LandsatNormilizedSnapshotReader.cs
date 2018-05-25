using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.PointsReaders
{
    public sealed class LandsatNormilizedSnapshotReader : IDisposable
    {

        #region Public fields

        public int Width { get; set; }
        public int Heigth { get; set; }

        #endregion

        #region Private fields

        private readonly string _extenstion = ".l8n";
        private readonly byte _endOfHeaderPosition = 8;
        private readonly byte _doubleBytesCount = 8;

        private readonly FileStream _fileStream;
        private readonly BinaryReader _binaryReader;
        private readonly int _scanlineSize;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename">File name</param>
        public LandsatNormilizedSnapshotReader(string filename)
        {
            var extension = Path.GetExtension(filename);
            if (string.IsNullOrEmpty(extension) || !extension.Equals(_extenstion, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException("Extension should be .l8n");
            }

            _fileStream = new FileStream(filename, FileMode.Open);
            _binaryReader = new BinaryReader(_fileStream);

            Width = _binaryReader.ReadInt32();
            Heigth = _binaryReader.ReadInt32();
            _scanlineSize = Width * _doubleBytesCount;
        }

        ~LandsatNormilizedSnapshotReader()
        {
            Dispose(false);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Read scanline
        /// </summary>
        /// <param name="x">line</param>
        /// <returns></returns>
        public double[] ReadScanline(int x)
        {
            if (x > Heigth - 1)
            {
                throw new ArgumentException("X more than the height");
            }

            _fileStream.Position = _endOfHeaderPosition + x * _scanlineSize;
            var bytesBuffer = _binaryReader.ReadBytes(_scanlineSize);
            var doubleBuffer = new double[Width];
            Buffer.BlockCopy(bytesBuffer, 0, doubleBuffer, 0, bytesBuffer.Length);

            return doubleBuffer;
        }

        /// <summary>
        /// Read scanline by row, col and width
        /// </summary>
        /// <param name="row"></param>
        /// <param name="width"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public double[] ReadCuttedline(int row, int col, int width)
        {
            return ReadScanline(row).Skip(col).Take(width).ToArray();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private methods

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fileStream?.Dispose();
                _binaryReader?.Dispose();
            }
        }

        #endregion
    }
}
