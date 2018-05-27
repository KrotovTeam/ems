using System.Collections.Generic;
using Common.Enums;

namespace BusContracts
{
    public interface IСharacterizationRequest
    {
        /// <summary>
        /// Идентификатор запроса
        /// </summary>
        string RequestId { get; set; }

        /// <summary>
        /// Тип явления
        /// </summary>
        PhenomenonType PhenomenonType { get; set; }

        /// <summary>
        /// Характеристики и условия распространения 
        /// </summary>
        List<ICharacteristicDescription> Characteristics { get; set; }

        /// <summary>
        /// Верхний левый угол полигона
        /// </summary>
        IGeographicPoint LeftUpper { get; set; }

        /// <summary>
        /// Нижний правый угол полигона
        /// </summary>
        IGeographicPoint RigthLower { get; set; }
    }
}
