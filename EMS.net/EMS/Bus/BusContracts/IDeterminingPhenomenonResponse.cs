using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusContracts
{
    public interface IDeterminingPhenomenonResponse
    {
        /// <summary>
        /// Признак обнаружения явления
        /// </summary>
        bool IsDetermined { get; set; }

        /// <summary>
        /// Guid запроса пользователя
        /// </summary>
        string RequestId { get; set; }
    }
}
