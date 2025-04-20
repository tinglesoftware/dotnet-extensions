##USAGE

## Application Insights Instrumentation Key

Get the key for your ApplicationInsights resource on Azure.

Either replace the `00000000-0000-0000-0000-000000000000` in `appsettings.json` with the key,
or configure it using the user-secret command, e.g.:

dotnet user-secrets set AppSettings:YouTubeApiKey <app-server-key>

## Running the sample

1. Run the application (F5),
2. Make a HTTP GET request to https://{your-endpoint}/api/values e.g. `https://localhost:50073/api/values`
3. Make a HTTP POST request to https://{your-endpoint}/api/values e.g. `https://localhost:50073/api/values`
4. Wait for about 2 minutes and check the insights for both requests
5. The failed request should have more data e.g. `Client`, `IpAddress`, `problem.title`, 'problem.detail', `headers` e.t.c.
