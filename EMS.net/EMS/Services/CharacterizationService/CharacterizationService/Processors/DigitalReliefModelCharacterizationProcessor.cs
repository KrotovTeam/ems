using System.Collections.Generic;
using CharacterizationService.Abstraction;
using CharacterizationService.Objects.DigitalReliefModel;
using CharacterizationService.Processors.DigitalReliefModel;
using Common.Objects;
using Topshelf.Logging;

namespace CharacterizationService.Processors
{
    public class DigitalReliefModelCharacterizationProcessor : AbstractCharacterizationProcessor
    {
        private readonly List<AbstractReliefCharacteristicProcessor> _processors =
            new List<AbstractReliefCharacteristicProcessor>
            {
                new ExpositionReliefCharacterisitcProcessor(),
                new SkewReliefCharacterisitcProcessor(),
                new HeigthReliefCharacterisitcProcessor()
            };

        public DigitalReliefModelCharacterizationProcessor(LogWriter logger) : base(logger)
        {
        }

        public override string[] Process(IGeographicPoint leftUpper, IGeographicPoint rigthLower, string dataFolder, string resultFolder)
        {
            var dataset = new SrtmDataset(leftUpper, rigthLower, dataFolder);
            var filePaths = new List<string>();

            foreach (var processor in _processors)
            {
                filePaths.Add(processor.Process(dataset, resultFolder));
            }

            return filePaths.ToArray();
        }
    }
}
