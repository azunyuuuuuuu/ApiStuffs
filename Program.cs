using Ical.Net;
using Ical.Net.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CalendarDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();

app.MapPost("/calendar", async (string url) =>
{
    var calendarSerialzier = new CalendarSerializer();
    var results = await new HttpClient().GetStringAsync(url);

    var calendar = Calendar.Load(results);

    return Results.Text(calendarSerialzier.SerializeToString(calendar), "text/calendar");
});

app.UseSwaggerUI();
app.Run();
