# Tingle.Extensions.Logging.LogAnalytics

Azure Log Analytics is the primary tool in the Azure portal for writing log queries and interactively
analyzing their results. It can be accessed independently or through other Azure products such as Azure Security Center. 

Log analytics software collects logs from events such as application installation, security breaches,
system setup/startup operational information.
A log entry includes such information as date and time the event occurred, the device the event occurred on,
an identification of the user, category of the event, and the program that initiated the event. 

This library comprises of extensions for logging in Azure Log Analytics. 

## Creating logs in Startup class

```cs
public void ConfigureServices(IServiceCollection services)
{
     services.AddLogging(builder =>
     {
         builder.AddLogAnalytics(workspaceId: Configuration["LogAnalytics:WorkspaceId"],
                                 workspaceKey: Configuration["LogAnalytics:WorkspaceKey"]);
     });
}
```

## Using IHostBuilder with known values

```cs
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging((context, loggingBuilder) =>
            {
                var workspaceId = context.Configuration["LogAnalytics:WorkspaceId"];
                var workspaceKey = context.Configuration["LogAnalytics:WorkspaceKey"];
                loggingBuilder.AddLogAnalytics(workspaceId, workspaceKey);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
```

## Using IHostBuilder with IConfiguration

```cs
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging((context, loggingBuilder) =>
            {
                loggingBuilder.AddLogAnalytics(context.Configuration.GetSection("LogAnalytics"));
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
```

You can modify the minimum log level that is sent to log analytics via configuring the `LogLevel` above.

## Configuration

Logging configuration is commonly provided by the Logging section of app settings files.
The following example shows the required contents of a typical `appsettings.json` file:

```json
{
  "LogAnalytics": {
    "WorkspaceId": "00000000-0000-0000-0000-000000000000", // replace with appropriate
    "WorkspaceKey": "AAAAAAAAAAA=" // replace with appropriate
  }
}
```

The values for `WorkspaceId` and `WorkspaceKey` are sensitive information.
They should be hidden and never commited in source code.
During development you can add them to your secrets using:

```console
dotnet user-secrets set "LogAnalytics:WorkspaceId" "00000000-0000-0000-0000-000000000000"
dotnet user-secrets set "LogAnalytics:WorkspaceKey" "AAAAAAAAAAA="
```

Ensure you get the corret values from your workspace on Azure portal.

## Configuring LogLevel

There are times you want to configure the logging level to LogAnalytics hence avoid too many logs being sent.
This can be done by creating a `LogAnalytics` section `Logging` section of your configuration in `appsettings.json`.
Below is an example that allows upto `Information` for `System*` logs and upto `Warning` for `Microsoft*` logs.

```json
{
  "Logging": {
    "LogAnalytics": {
      "LogLevel": {
        "System": "Information",
        "Microsoft": "Warning"
      }
    },
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```
When the `LogAnalytics` section under the `Logging` section is not provided, the defaults are taken from the `LogLevel` section.