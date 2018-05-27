namespace Common.Constants
{
    public class BusQueueConstants
    {
        public const string DataNormalizationRequestQueueName = "EMS.DataNormalizationService.Requests";
        public const string DataNormalizationResponsesQueueName = "EMS.DataNormalizationService.Responses";

        public const string DeterminingPhenomenonRequestsQueueName = "EMS.DeterminingPhenomenonService.Requests";
        public const string DeterminingPhenomenonResponsesQueueName = "EMS.DeterminingPhenomenonService.Responses";

        public const string CharacterizationRequestQueueName = "EMS.CharacterizationService.Request";
        public const string CharacterizationResponseQueueName = "EMS.CharacterizationService.Responses";
    }
}
