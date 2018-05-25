using Topshelf;

namespace DeterminingPhenomenonService
{
    class Program
    {
        static int Main(string[] args)
        {
            GdalConfiguration.ConfigureGdal();
            GdalConfiguration.ConfigureOgr();

            return (int)HostFactory.Run(cfg => cfg.Service(x => new Service()));
        }
    }
}
