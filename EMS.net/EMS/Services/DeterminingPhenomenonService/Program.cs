using Serilog;
using Topshelf;

namespace DeterminingPhenomenonService
{
    class Program
    {
        static int Main(string[] args)
        {
            GdalConfiguration.ConfigureGdal();
            GdalConfiguration.ConfigureOgr();

            ILogger configuration = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            return (int)HostFactory.Run(cfg =>
            {
                cfg.Service(x => new Service());
                cfg.UseSerilog(configuration);
            });
        }
    }
}
