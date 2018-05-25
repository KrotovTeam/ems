using Topshelf;

namespace ReliefModelService
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
