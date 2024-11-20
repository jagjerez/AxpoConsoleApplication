using LogicApplication.Application.UseCases;
using LogicApplication.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace LogicApplication.Application
{
    public class ApplicationConfiguration
    {
        public static void Configuration(IServiceCollection serviceCollector) 
        {
            serviceCollector.AddTransient<CreateReportServices>();
            serviceCollector.AddTransient<IReportServices, ReportServices>();
        }
    }
}
