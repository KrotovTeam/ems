using CharacterizationService.Objects.DigitalReliefModel;

namespace CharacterizationService.Abstraction
{
    public abstract class AbstractReliefCharacteristicProcessor
    {
        /// <summary>
        /// Обработка характеристики
        /// </summary>
        /// <param name="dataset">Данные</param>
        /// <param name="folder"></param>
        public abstract string Process(SrtmDataset dataset, string folder);
    }
}
