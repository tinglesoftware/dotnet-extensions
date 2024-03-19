var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry()
                .AddApplicationInsightsTelemetryHeaders()
                .AddApplicationInsightsTelemetryExtras();

builder.Services.AddControllers()
                .AddControllersAsServices();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapControllers();

app.Run();
