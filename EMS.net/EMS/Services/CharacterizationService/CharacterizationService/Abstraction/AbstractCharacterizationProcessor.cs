using BusContracts;
using Common.Objects;
using Topshelf.Logging;

namespace CharacterizationService.Abstraction
{
    public abstract class AbstractCharacterizationProcessor
    {
        protected LogWriter Logger { get; set; }

        protected AbstractCharacterizationProcessor(LogWriter logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Получение характеристики/условия распространения
        /// </summary>
        /// <param name="leftUpper"></param>
        /// <param name="rigthLower"></param>
        /// <param name="dataFolder"></param>
        /// <param name="resultFolder"></param>
        /// <returns></returns>
        public abstract string[] Process(IGeographicPoint leftUpper, IGeographicPoint rigthLower, string dataFolder, string resultFolder);
    }
}
