using Axpo;
using LogicApplication.Application;
using LogicApplication.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace LogicApplication.Infrastructure
{
    public class InfrastructureConfiguration
    {
        public static void Configuration(IServiceCollection serviceCollector)
        {
            serviceCollector.AddTransient<IExportDataToCsvFormatServices, ExportDataToCsvFormatServices>();
            serviceCollector.AddTransient<ICalculationServices, CalculationServicesAxpoLibrary>();
            serviceCollector.AddTransient<IPowerService, PowerService>();
            ApplicationConfiguration.Configuration(serviceCollector);
        }
    }
}
