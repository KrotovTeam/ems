using System.Collections.Generic;
using Common.Enums;
using Common.Objects;

namespace BusContracts
{
    public interface ISpectralСharacterizationRequest
    {
        /// <summary>
        /// Идентификатор запроса
        /// </summary>
        string RequestId { get; set; }

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
