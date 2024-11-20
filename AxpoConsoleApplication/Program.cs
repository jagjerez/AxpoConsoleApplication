using Cronos;
using LogicApplication.Application.UseCases;
using LogicApplication.Domain;
using LogicApplication.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

var serviceCollection = new ServiceCollection();

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)  // Asegura que se lea desde el directorio correcto
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

serviceCollection.AddSingleton<IConfiguration>(configuration);

ConfigureServices(serviceCollection);

var serviceProvider = serviceCollection.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

bool init = true;

using var cancellationTokenSource = new CancellationTokenSource();

Console.WriteLine("Set type of file (CSV): ");
string csvType = Console.ReadLine();  // Captura la ruta del archivo

if (string.IsNullOrEmpty(csvType))
{
    Console.WriteLine("Ruta no válida.");
    return;
}

Console.CancelKeyPress += (sender, eventArgs) =>
{
    Console.WriteLine("Cancellation requested...");
    cancellationTokenSource.Cancel();
    eventArgs.Cancel = true; // Prevent immediate termination
};

try
{
    Console.WriteLine("Press Ctrl+C to cancel.");
    await RunAsync(cancellationTokenSource.Token);
}
catch (OperationCanceledException)
{
    logger.LogError("Operation was canceled.");
}
finally
{
    logger.LogInformation("Application shutting down...");
}

async Task RunAsync(CancellationToken cancellationToken)
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var service = serviceProvider.GetRequiredService<CreateReportServices>();
    var cronExpression = configuration.GetSection("Configuration:Cron").Value;
    var cron = CronExpression.Parse(cronExpression);
    while (!cancellationToken.IsCancellationRequested)
    {
        var utc = DateTime.UtcNow;
        var now = DateTime.Now;
        var next = cron.GetNextOccurrence(utc);
        try
        {
            if (next.HasValue)
            {
                var nextOccurrence = next.Value.ToLocalTime();  // Convierte a la hora local si es necesario

                // Calcula el tiempo restante hasta la próxima ocurrencia
                var timeToWait = nextOccurrence - DateTime.Now;

                if (!init && timeToWait > TimeSpan.Zero)
                {
                    // Espera hasta la siguiente ejecución
                    await Task.Delay(timeToWait, cancellationToken);
                }
                await service.Execute(now, (FormatReport)Enum.Parse(typeof(FormatReport), csvType));
                init = false;
            }
        }
        catch (ApplicationException ex)
        {
            logger.LogError(ex,ex.Message);
        }
    }
}

void ConfigureServices(IServiceCollection services)
{
    services.AddLogging(configure => configure.AddConsole());
    InfrastructureConfiguration.Configuration(services);
}
