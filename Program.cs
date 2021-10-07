using Ical.Net;
using Ical.Net.Serialization;
using Ical.Net.Proxies;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// setup the modules
builder.RegisterModules();

// register additional services
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
app.MapEndpoints();

app.MapGet("/calendar/{*inputurls}", async (
    string inputurls,
    IHttpClientFactory clientFactory
    ) =>
{
    var urls = inputurls.Split(";");
    using var client = clientFactory.CreateClient();

    var temp = new List<string>();
    await Parallel.ForEachAsync(urls, async (url, token) => temp.Add(await client.GetStringAsync(url, token)));

    var tenp2 = string.Join(Environment.NewLine, temp);

    var calendar = CalendarCollection.Load(tenp2);

    var calendarserializer = new CalendarSerializer();
    var serialized = calendarserializer.SerializeToString(calendar);
    return Results.Text(serialized, "text/calendar");
});

app.Run();
