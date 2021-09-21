var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CalendarDbContext>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
