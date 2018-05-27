using System;
using System.Collections.Generic;
using System.IO;
using BusContracts;
using Common.Helpers;
using Common.Objects;
using OSGeo.GDAL;

namespace CharacterizationService.Objects.DigitalReliefModel
{
    public class SrtmDataset
    {
        #region Private fields

        private readonly IGeographicPoint _leftUpper;
        private readonly IGeographicPoint _rigthLower;
        private readonly string _folder;
        private SrtmFile[,] _grid;
        private readonly int _tileSize = 1201;
        private readonly short _noData = -32768;

        #endregion

        #region Public fields

        public int Width { get; private set; }
        public int Heigth { get; private set; }
        public short?[,] Values { get; private set; }

        #endregion

        #region Constructor

        public SrtmDataset(IGeographicPoint leftUpper, IGeographicPoint rigthLower, string folder)
        {
            _leftUpper = leftUpper;
            _rigthLower = rigthLower;
            _folder = folder;

            FormSrtmGrid();
            FillData();
        }

        #endregion

        #region FormSrtmGrid

        private void FormSrtmGrid()
        {
            var latitudes = new List<short>();
            var longtitudes = new List<short>();

            for (var i = (short)Math.Truncate(_leftUpper.Latitude); i >= (short)Math.Truncate(_rigthLower.Latitude); i--)
            {
                latitudes.Add(i);
            }

            for (var i = (short)Math.Truncate(_leftUpper.Longitude); i <= (short)Math.Truncate(_rigthLower.Longitude); i++)
            {
                longtitudes.Add(i);
            }

            _grid = new SrtmFile[latitudes.Count, longtitudes.Count];

            for (var y = 0; y < latitudes.Count; y++)
            {
                var latitudeLetter = latitudes[y] > 0
                    ? "N"
                    : "S";
                for (var x = 0; x < longtitudes.Count; x++)
                {
                    var longitudeLetter = longtitudes[x] > 0
                        ? "E"
                        : "W";
                    var srtmFile = new SrtmFile
                    {
                        Name = $"{latitudeLetter}{Math.Abs(latitudes[y]):00}{longitudeLetter}{Math.Abs(longtitudes[x]):000}.hgt"
                    };
                    srtmFile.IsExists = File.Exists($@"{_folder}\{srtmFile.Name}");
                    _grid[y, x] = srtmFile;
                }
            }

            var step = 1 / (double)_tileSize;

            if (latitudes.Count == 1 && longtitudes.Count == 1)
            {
                _grid[0, 0].From = new Point
                {
                    X = GetCoordinate(_leftUpper.Longitude, step),
                    Y = _tileSize - GetCoordinate(_leftUpper.Latitude, step)
                };
                _grid[0, 0].To = new Point
                {
                    X = GetCoordinate(_rigthLower.Longitude, step),
                    Y = _tileSize - GetCoordinate(_rigthLower.Latitude, step)
                };

                return;
            }

            if (latitudes.Count == 1)
            {
                for (var x = 0; x < longtitudes.Count; x++)
                {
                    var from = new Point();
                    var to = new Point();

                    if (x == 0)
                    {
                        from.X = GetCoordinate(_leftUpper.Longitude, step);
                        to.X = _tileSize;
                    }
                    else if (x == longtitudes.Count - 1)
                    {
                        from.X = 0;
                        to.X = GetCoordinate(_rigthLower.Longitude, step);
                    }
                    else
                    {
                        from.X = 0;
                        to.X = _tileSize;
                    }

                    from.Y = _tileSize - GetCoordinate(_leftUpper.Latitude, step);
                    to.Y = _tileSize - GetCoordinate(_rigthLower.Latitude, step);

                    _grid[0, x].From = from;
                    _grid[0, x].To = to;
                }

                return;
            }

            for (var y = 0; y < latitudes.Count; y++)
            {
                for (var x = 0; x < longtitudes.Count; x++)
                {
                    var from = new Point();
                    var to = new Point();

                    if (y == 0)
                    {
                        if (x == 0)
                        {
                            from.X = GetCoordinate(_leftUpper.Longitude, step);
                            to.X = _tileSize;
                        }
                        else if (x == longtitudes.Count - 1)
                        {
                            from.X = 0;
                            to.X = GetCoordinate(_rigthLower.Longitude, step);
                        }
                        else
                        {
                            from.X = 0;
                            to.X = _tileSize;
                        }

                        from.Y = _tileSize - GetCoordinate(_leftUpper.Latitude, step);
                        to.Y = _tileSize;
                    }
                    else if (y == latitudes.Count - 1)
                    {
                        if (x == 0)
                        {
                            from.X = GetCoordinate(_leftUpper.Longitude, step);
                            to.X = _tileSize;
                        }
                        else if (x == longtitudes.Count - 1)
                        {
                            from.X = 0;
                            to.X = GetCoordinate(_rigthLower.Longitude, step);
                        }
                        else
                        {
                            from.X = 0;
                            to.X = _tileSize;
                        }

                        from.Y = 0;
                        to.Y = _tileSize - GetCoordinate(_rigthLower.Latitude, step);
                    }
                    else
                    {
                        if (x == 0)
                        {
                            from.X = GetCoordinate(_leftUpper.Longitude, step);
                            to.X = _tileSize;
                        }
                        else if (x == longtitudes.Count - 1)
                        {
                            from.X = 0;
                            to.X = GetCoordinate(_rigthLower.Longitude, step);
                        }
                        else
                        {
                            from.X = 0;
                            to.X = _tileSize;
                        }

                        from.Y = 0;
                        to.Y = _tileSize;
                    }

                    _grid[y, x].From = from;
                    _grid[y, x].To = to;
                }
            }
        }

        private int GetCoordinate(double value, double step)
        {
            return (int)Math.Round(MathHelper.Fraction(value) / step);
        }

        #endregion

        #region FillData

        private void FillData()
        {
            var xSize = _grid.GetLength(0);
            var ySize = _grid.Length / xSize;

            var width = 0;
            var heigth = 0;
            for (var x = 0; x < xSize; x++)
            {
                heigth += _grid[x, 0].To.Y - _grid[x, 0].From.Y;
            }

            for (var y = 0; y < ySize; y++)
            {
                width += _grid[0, y].To.X - _grid[0, y].From.X;
            }

            Width = width;
            Heigth = heigth;
            Values = new short?[width, heigth];

            var heigthBorder = 0;
            for (var x = 0; x < xSize; x++)
            {
                var blockHeigth = _grid[x, 0].To.Y - _grid[x, 0].From.Y;
                var widthBorder = 0;
                for (var y = 0; y < ySize; y++)
                {
                    var bufferSize = _tileSize * _tileSize;
                    var buffer = new short[bufferSize];

                    if (_grid[x, y].IsExists)
                    {
                        using (var data = Gdal.Open($@"{_folder}\{_grid[x, y].Name}", Access.GA_ReadOnly))
                        using (var band = data.GetRasterBand(1))
                        {
                            band.ReadRaster(0, 0, band.XSize, band.YSize, buffer, band.XSize, band.YSize, 0, 0);
                        }
                    }
                    else
                    {
                        //Если файл отсутствует заполняем его значением отсутствия данных
                        for (var i = 0; i < bufferSize; i++)
                        {
                            buffer[i] = _noData;
                        }
                    }

                    int xPosition = widthBorder;
                    int yPosition = heigthBorder;
                    var blockWidth = _grid[x, y].To.X - _grid[x, y].From.X;

                    for (var x2 = _grid[x, y].From.X; x2 < _grid[x, y].To.X; x2++)
                    {
                        for (var y2 = _grid[x, y].From.Y; y2 < _grid[x, y].To.Y; y2++)
                        {
                            var value = buffer[(y2 * _tileSize) + x2];
                            if (value == _noData)
                            {
                                Values[xPosition, yPosition] = null;
                            }
                            else
                            {
                                Values[xPosition, yPosition] = value;
                            }
                            yPosition++;
                        }

                        xPosition++;
                        yPosition = heigthBorder;
                    }

                    widthBorder += blockWidth;
                }

                heigthBorder += blockHeigth;
            }
        }

        #endregion
    }
}
