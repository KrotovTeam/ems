using Common.Objects;

namespace CharacterizationService.Objects.DigitalReliefModel
{
    public class SrtmFile
    {
        public string Name { get; set; }
        public bool IsExists { get; set; }
        public Point From { get; set; }
        public Point To { get; set; }
    }
}
