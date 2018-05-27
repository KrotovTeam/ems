using CharacterizationService.Abstraction;
using CharacterizationService.Objects.AreaOfDamage;
using Common.Constants;
using Common.Helpers;
using Common.Objects;
using DrawImageLibrary;
using Topshelf.Logging;

namespace CharacterizationService.Processors.AreaOfDamage
{
    public class AreaOfDamageCharacterizationProcessor:AbstractCharacterizationProcessor
    {
        private const int LandsatPixelSize = 30;
        
        public AreaOfDamageCharacterizationProcessor(LogWriter logger) : base(logger)
        {
            
        }

        public override string[] Process(IGeographicPoint leftUpper, IGeographicPoint rigthLower, string dataFolder, string resultFolder)
        {
            var pathToEdgedDynamic = $@"{dataFolder}{FilenamesConstants.PathToEdgedDynamicFile}";
            var pathToAreOfDamageResult = $@"{resultFolder}{FilenamesConstants.PathToDamagedAreaResult}";

            var amountOfDynamicPoints = DrawLib.GetAmountOfDynamicPoints(pathToEdgedDynamic);

            double areaOfDamage = amountOfDynamicPoints * LandsatPixelSize;

            var resultPath = JsonHelper.Serialize(pathToAreOfDamageResult, new AreaOfDamageResult
            {
                AreaOfDamage = areaOfDamage,
                DamagedPointsCount = amountOfDynamicPoints
            });

            return new[] { resultPath };
        }
    }
}
