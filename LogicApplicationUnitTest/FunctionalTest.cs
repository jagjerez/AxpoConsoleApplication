using LogicApplication.Application.UseCases;
using LogicApplication.Domain;
using LogicApplication.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Globalization;
using Xunit.Sdk;

namespace LogicApplicationUnitTest
{
    public class FunctionalTest
    {
        private CreateReportServices service;
        private IConfiguration appSettingsService;
        private readonly TimeZoneInfo GmtTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        private readonly TimeZoneInfo ApplicationTimeZoneInfo;

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
            ApplicationTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(configuration.GetSection("Configuration:LocalTimeZone").Value ?? throw new DomainException("You set LocalTimeZone"));
        }

        [Fact]
        public async Task CreateCsvTest()
        {
            var now = DateTime.Now;
            DateTime executionTimeDayAhead = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc), ApplicationTimeZoneInfo);
            var referenceDate = executionTimeDayAhead.AddDays(1);
            var fileName = string.Format(
                                appSettingsService.GetSection("Configuration:FormatFileName").Value ?? throw new DomainException("You set FormatFileName"),
                                referenceDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                                executionTimeDayAhead.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture));
            var path = appSettingsService.GetSection("Configuration:RouteFile").Value ?? throw new DomainException("You set RouteFile");

            try
            {
                await service.Execute(now, FormatReport.CSV);
                Assert.True(File.Exists(Path.Combine(path, fileName)));
                File.Delete(Path.Combine(path, fileName));
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