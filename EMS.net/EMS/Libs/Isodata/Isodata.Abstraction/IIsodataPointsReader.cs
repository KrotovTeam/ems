using System;

namespace Isodata.Abstraction
{
    public interface IIsodataPointsReader
    {
        /// <summary>
        /// Высота снимка
        /// </summary>
        int Heigth { get; }

        /// <summary>
        /// Ширина
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Общее количество точек
        /// </summary>
        long PointsCount { get; }

        /// <summary>
        /// Максимальное значение для точки
        /// </summary>
        double MaxValue { get; }

        /// <summary>
        /// Количество размерностей
        /// </summary>
        byte DimensionalityCount { get; }

        /// <summary>
        /// Цикл по всем точкам
        /// </summary>
        /// <param name="action">Лямбда</param>
        void Foreach(Action<double[], int, int> action);
    }
}
