using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Common.Objects;

namespace BusContracts
{
    public interface IDeterminingPhenomenonRequest
    {
        /// <summary>
        /// Путь папке, в которой будут храниться результаты процесса обнаружения явления
        /// </summary>
        string ResultFolder { get; set; }

        /// <summary>
        /// Верхняя левая точка
        /// </summary>
        IGeographicPoint LeftUpper { get; set; }

        /// <summary>
        /// Нижняя правая точка
        /// </summary>
        IGeographicPoint RightLower { get; set; }

        /// <summary>
        /// Тип явления, которое необходимо обнаружить
        /// </summary>
        PhenomenonType Phenomenon { get; set; }

        /// <summary>
        /// Пути к папкам, в которых находятся откалиброванные данные, необходимые для обнаружения явления
        /// </summary>
        string[] DataFolders { get; set; }

        /// <summary>
        /// Guid запроса пользователя
        /// </summary>
        string RequestId { get; set; }
    }
}
