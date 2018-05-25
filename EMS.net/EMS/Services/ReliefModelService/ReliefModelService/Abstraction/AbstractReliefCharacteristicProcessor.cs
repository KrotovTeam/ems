using System.Drawing;
using BusContracts;
using ReliefModelService.Objects;

namespace ReliefModelService.Abstraction
{
    public abstract class AbstractReliefCharacteristicProcessor
    {
        /// <summary>
        /// Обработка характеристики
        /// </summary>
        /// <param name="dataset">Данные</param>
        /// <param name="folder"></param>
        public abstract IReliefCharacteristicProduct Process(SrtmDataset dataset, string folder);

        /// <summary>
        /// Создать рисунок
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="legend"></param>
        /// <returns></returns>
        protected Bitmap CreateImage(SrtmDataset dataset, Image legend)
        {
            var width = dataset.Width + legend.Width;
            var heigth = dataset.Heigth;
            if (heigth < legend.Height)
            {
                heigth = legend.Height;
            }

            var bitmap = new Bitmap(width, heigth);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.FillRectangle(Brushes.White, 0, 0, width, heigth);
            }

            return bitmap;
        }

        /// <summary>
        /// Добавить легенду
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="legend"></param>
        protected void SetLegend(Bitmap bitmap, Image legend)
        {
            var x = bitmap.Width - legend.Width;
            var y = bitmap.Height / 2 - legend.Height / 2;

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawImage(legend, x, y, legend.Width, legend.Height);
            }
        }
    }
}
