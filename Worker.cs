namespace Worker_AppConfig;

public class Worker : BackgroundService
{
  private readonly ILogger<Worker> _logger;
  private readonly IConfiguration _config;

  public Worker(ILogger<Worker> logger, IConfiguration config)
  {
    _logger = logger;
    _config = config;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      _logger.LogInformation("Message: {message}", _config["Worker:Settings:Message"] ?? "Hello from Code!");
      await Task.Delay(1000, stoppingToken);
    }
  }
}
