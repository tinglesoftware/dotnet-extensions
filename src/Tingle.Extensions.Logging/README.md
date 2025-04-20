# Tingle.Extensions.Logging

This library provides convenience extensions and logging in CLI applications.

The default console logger uses the `Simple` formatter by default but this includes the category and event id whereas in CLI applications we may not want them. This library contains a `CliConsoleFormatter` that make them optional.

For example, you can configure the `Simple` formatter as:

```json
{
  "Logging": {
    "Console": {
      "FormatterName": "Simple",
      "FormatterOptions": {
        "SingleLine": true,
        "IncludeScopes": false,
        "TimestampFormat": "yyyy-MM-dd HH:mm:ss "
      }
    }
  }
}
```

The output would be:

```txt
2025-04-20 13:19:11 info: System.Net.Http.HttpClient.IpifyClient.LogicalHandler[100] Start processing HTTP request GET https://api64.ipify.org/?*
2025-04-20 13:19:11 info: System.Net.Http.HttpClient.IpifyClient.ClientHandler[100] Sending HTTP request GET https://api64.ipify.org/?*
2025-04-20 13:19:12 info: System.Net.Http.HttpClient.IpifyClient.ClientHandler[101] Received HTTP response headers after 553.3962ms - 200
2025-04-20 13:19:12 info: System.Net.Http.HttpClient.IpifyClient.LogicalHandler[101] End processing HTTP request after 568.9045ms - 200
```

Now you can configure the `cli` formatter as:

```json
{
  "Logging": {
    "Console": {
      "FormatterName": "cli",
      "FormatterOptions": {
        "SingleLine": true,
        "IncludeScopes": false,
        "IncludeEventId": false,
        "TimestampFormat": "yyyy-MM-dd HH:mm:ss "
      }
    }
  }
}
```

```cs
var builder = Host.CreateApplicationBuilder();
builder.Logging.AddCliConsole();
```

The output would be:

```txt
2025-04-20 13:19:11 info:  Start processing HTTP request GET https://api64.ipify.org/?*
2025-04-20 13:19:11 info: Sending HTTP request GET https://api64.ipify.org/?*
2025-04-20 13:19:12 info: Received HTTP response headers after 553.3962ms - 200
2025-04-20 13:19:12 info: End processing HTTP request after 568.9045ms - 200
```
