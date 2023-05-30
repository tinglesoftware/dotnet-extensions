var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/", async (context) =>
{
    await context.Response.WriteAsync("Hello World!");
});

app.Run();
