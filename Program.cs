using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Worker_AppConfig;


class Program
{
  private static IConfigurationRefresher _refresher = null;

  static async Task Main(string[] args)
  {

    IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, config) =>
        {
          // Option 1: Get the connection string from appsettings.json (Not good because it has a key)
          // config.AddAzureAppConfiguration(config.Build().GetConnectionString("AppConfigurationConnectionString"));


          // Option 2: Get the connection string from an environment variable (Good because it doesn't have a key)
          // PowerShell - $Env:ConnectionString = "connection-string-of-your-app-configuration-store"
          // Bash -- export ConnectionString='connection-string-of-your-app-configuration-store'
          //config.AddAzureAppConfiguration(Environment.GetEnvironmentVariable("ConnectionString"));


          // Option 3: Get the connection string from a user secret (Good because it doesn't have a key)
          // dotnet user-secrets init
          // dotnet user-secrets set "ConnectionStrings:SecretAppConfigurationConnectionString" "connection-string-of-your-app-configuration-store"
          //config.AddAzureAppConfiguration(config.Build().GetConnectionString("SecretAppConfigurationConnectionString"));


          // Option 4: Use Managed Identity (Good because it doesn't have a key)
          // Ensure that the User has the "App Configuration Data Reader" role assigned
          var settings = config.Build();

          // PowerShell - $Env:APP_CONFIG_ENDPOINT = "https://<your_endpoint>.azconfig.io"
          // Bash -- export APP_CONFIG_ENDPOINT='https://<your_endpoint>.azconfig.io'
          var endpoint = Environment.GetEnvironmentVariable("APP_CONFIG_ENDPOINT") ?? String.Empty;  // From environment variable
          // var endpoint = settings["AppConfiguration:Endpoint"];  // From appsettings.json

          config.AddAzureAppConfiguration(options =>
          {
            var credential = new DefaultAzureCredential();
            options.Connect(new Uri(endpoint), credential);
            options.Select("refresh", LabelFilter.Null);
            options.Select("Worker:*", LabelFilter.Null);

            options.ConfigureRefresh(refresh =>
            {
              refresh
                .Register("refresh", true)
                .SetCacheExpiration(TimeSpan.FromSeconds(5));
            });

            _refresher = options.GetRefresher();
          });
        })
        .ConfigureServices(configureServices)
        .Build();

    host.Run();
  }

  static void configureServices(HostBuilderContext context, IServiceCollection services)
  {
    services.AddOptions<WorkerConfig>().Bind (context.Configuration.GetSection("Worker:Settings"));
    //services.Configure<WorkerConfig>(context.Configuration.GetSection("Worker:Settings"));
    //services.AddSingleton(context.Configuration.GetSection("Worker:Settings").Get<WorkerConfig>());
    services.AddLogging();
    services.AddSingleton(_refresher);
    services.AddHostedService<Worker>();
  }
}
