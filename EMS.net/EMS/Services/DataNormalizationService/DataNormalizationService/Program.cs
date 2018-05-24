using Topshelf;

namespace DataNormalizationService
{
    class Program
    {
        public static int Main(string[] args)
        {
            GdalConfiguration.ConfigureGdal();
            GdalConfiguration.ConfigureOgr();

            return (int)HostFactory.Run(cfg => cfg.Service(x => new NormalizationService()));
        }
    }
}
