using LogicApplication.Application.UseCases;
using LogicApplication.Domain;
using LogicApplication.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Xunit.Sdk;

namespace LogicApplicationFunctionalTest
{
    public class FunctionalTest
    {
        private CreateReportServices service;
        private IConfiguration appSettingsService;
        private readonly TimeZoneInfo ApplicationTimeZoneInfo;
        private readonly TimeZoneInfo GmtTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        public FunctionalTest()
        {
            var serviceCollection = new ServiceCollection();

            // Configura la lectura de la configuración desde appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)  // Asegura que se lea desde el directorio correcto
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Registra la configuración en el contenedor de dependencias
            serviceCollection.AddSingleton<IConfiguration>(configuration);

            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            service = serviceProvider.GetRequiredService<CreateReportServices>();
            appSettingsService = serviceProvider.GetRequiredService<IConfiguration>();
            ApplicationTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(configuration.GetSection("Configuration:LocalTimeZone").Value ?? throw new DomainException("Not set LocalTimeZone"));
        }

        [Fact]
        public async Task CreateCsvTest()
        {
    
            DateTime utcDateTime = DateTime.UtcNow;
            DateTime applicationDatetime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, ApplicationTimeZoneInfo);
            var referenceDate = applicationDatetime.AddDays(1);
            var formatFile = appSettingsService.GetSection("Configuration:FormatFileName").Value ?? throw new DomainException("Not set FormatFileName");

            var fileName = string.Format(
                                formatFile,
                                referenceDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                                utcDateTime.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture));
            var path = appSettingsService.GetSection("Configuration:RouteFile").Value ?? throw new DomainException("Not set RouteFile");

            try
            {
                await service.Execute(applicationDatetime, FormatReport.CSV);
                Assert.True(File.Exists(Path.Combine(path, fileName)));
            } catch(ApplicationException ex)
            {
                Assert.Equivalent(typeof(ApplicationException), ex.GetType());
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole());
            InfrastructureConfiguration.Configuration(services);
        }
    }
}