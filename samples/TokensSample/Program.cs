var builder = WebApplication.CreateBuilder(args);

// see https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-8.0
builder.Services.AddDataProtection();

builder.Services.AddControllers()
                .AddTokens();

var app = builder.Build();

app.MapControllers();

app.Run();
