using Ical.Net;
using Ical.Net.Serialization;

var builder = WebApplication.CreateBuilder(args);

// setup the services
builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// build the app
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// explore the api
app.UseSwagger();
app.UseSwaggerUI();

// map the endpoints
app.MapGet("/calendar", async (IHttpClientFactory clientFactory, string url) =>
{
    using var client = clientFactory.CreateClient();

    var calendarSerialzier = new CalendarSerializer();
    var results = await client.GetStringAsync(url);

    var calendar = Calendar.Load(results);

    return Results.Text(calendarSerialzier.SerializeToString(calendar), "text/calendar");
});

app.Run();
