using Serilog;
using Topshelf;

namespace DataNormalizationService
{
    class Program
    {
        public static int Main(string[] args)
        {
            GdalConfiguration.ConfigureGdal();
            GdalConfiguration.ConfigureOgr();

            ILogger configuration = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            return (int)HostFactory.Run(cfg =>
            {
                cfg.Service(x => new NormalizationService());
                cfg.UseSerilog(configuration);
            });
        }
    }
}
