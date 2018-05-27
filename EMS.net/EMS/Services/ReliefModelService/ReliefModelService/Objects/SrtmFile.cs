using Common.Objects;

namespace ReliefModelService.Objects
{
    public class SrtmFile
    {
        public string Name { get; set; }
        public bool IsExists { get; set; }
        public Point From { get; set; }
        public Point To { get; set; }
    }
}
