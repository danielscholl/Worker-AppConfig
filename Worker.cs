using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Options;

namespace Worker_AppConfig;

public class Worker : BackgroundService
{
  private readonly ILogger<Worker> _logger;
  private readonly IOptionsMonitor<WorkerConfig> _settings;
  private readonly IConfigurationRefresher _refresher;


  public Worker(ILogger<Worker> logger, IOptionsMonitor<WorkerConfig> settings, IConfigurationRefresher refresher)
  {
    this._logger = logger;
    this._settings = settings;
    this._refresher = refresher;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      _logger.LogInformation("Message: {message}", _settings.CurrentValue.Message ?? "Hello from Code!"); // This one is not updating
      await this._refresher.TryRefreshAsync();
      await Task.Delay(1000, stoppingToken);
    }
  }
}
