using Azure.Identity;
using Worker_AppConfig;

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
      var settings = config.Build();
      var endpoint = settings["AppConfiguration:Endpoint"];
      config.AddAzureAppConfiguration(options =>
      {
        var credential = new DefaultAzureCredential();
        options.Connect(new Uri(endpoint), credential);
        // options.ConfigureRefresh(refresh =>
        // {
        //   refresh
        //     .Register("refreshAll", true);
        // });
      });
    })
    .ConfigureServices(configureServices)
    .Build();

host.Run();

static void configureServices(HostBuilderContext context, IServiceCollection services)
{
  services.Configure<WorkerConfig>(context.Configuration.GetSection("Worker:Settings"));
  services.AddLogging();
  services.AddHostedService<Worker>();
}