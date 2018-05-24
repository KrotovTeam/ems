namespace Isodata.Abstraction
{
    public interface IIsodataProfile
    {
        /// <summary>
        /// Необходимое число кластеров
        /// </summary>
        int ClustersCount { get; }

        /// <summary>
        /// Параметр, с которым сравнивается количество выборочных образов, вошедших в кластер
        /// </summary>
        int TettaN { get; }

        /// <summary>
        /// Параметр, характеризующий среднеквадратическое отклонение
        /// </summary>
        double TettaS { get; }

        /// <summary>
        /// Параметр, характеризующий компактность
        /// </summary>
        double TettaC { get; set; }

        /// <summary>
        /// Максимальное количество пар центров кластеров, которые можно объединить
        /// </summary>
        int L { get; }

        /// <summary>
        /// Допустимое число циклов итерации
        /// </summary>
        int I { get; }

        /// <summary>
        /// Коэффициент при высчитывании gammaj
        /// </summary>
        double Coefficient { get; }
    }
}
