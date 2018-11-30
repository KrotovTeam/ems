using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public class Legend
    {
        #region Class Range

        public class Range
        {
            private readonly double _start;
            private readonly double _end;
            public readonly Color Color;

            public Range(double start, double end, Color color)
            {
                _start = start;
                _end = end;
                Color = color;
            }

            public Color? StepIn(double value)
            {
                if (value > _start && value <= _end)
                {
                    return Color;
                }

                return null;
            }
        }

        #endregion

        #region Private fields

        private readonly Range[] _ranges;

        #endregion

        #region Constructor

        public Legend(double minValue, double maxValue, double step, Color startColor, Color endColor)
        {
            var stepsCount = GetStepsCount(minValue, maxValue, step);
            _ranges = new Range[stepsCount];
            double red = startColor.R;
            double green = startColor.G;
            double blue = startColor.B;
            var border = minValue;

            var stepRed = (endColor.R - startColor.R) / (double) stepsCount;
            var stepGreen = (endColor.G - startColor.G) / (double) stepsCount;
            var stepBlue = (endColor.B - startColor.B) / (double) stepsCount;

            for (var i = 0; i < stepsCount; i++)
            {
                _ranges[i] = new Range(border, border + step, Color.FromArgb((byte) red, (byte) green, (byte) blue));

                red += stepRed;
                green += stepGreen;
                blue += stepBlue;
                border += step;
            }
        }

        public Legend(params Range[] ranges)
        {
            _ranges = ranges;
        }

        #endregion

        #region Public methods

        public Color GetColor(double value)
        {
            foreach (var range in _ranges)
            {
                var color = range.StepIn(value);
                if (color.HasValue)
                {
                    return color.Value;
                }
            }

           return Color.Black;
        }

        public Bitmap GetLegend()
        {
            var pixelsForRange = 25;
            var heigth = 50;
            var bitmap = new Bitmap(pixelsForRange * _ranges.Length, heigth);

            for (var i = 0; i < _ranges.Length; i++)
            {
                for (var x = 0; x < heigth; x++)
                {
                    for (var y = pixelsForRange * i; y < (i + 1) * pixelsForRange; y++)
                    {
                        bitmap.SetPixel(y, x, _ranges[i].Color);
                    }
                }
            }

            return bitmap;
        }

        #endregion

        #region Private methods

        private int GetStepsCount(double start, double end, double step)
        {
            var stepsCount = 0;
            for (var i = start; i <= end; i += step)
            {
                stepsCount++;
            }

            return stepsCount;
        }

        #endregion
    }
}