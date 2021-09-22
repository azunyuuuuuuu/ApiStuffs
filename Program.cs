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
app.UseSwaggerUI();

app.MapPost("/calendar", async (string url) =>
{
    var calendarSerialzier = new CalendarSerializer();
    var results = await new HttpClient().GetStringAsync(url);

    var calendar = Calendar.Load(results);

    return Results.Text(calendarSerialzier.SerializeToString(calendar), "text/calendar");
});

// create bucket via magic string and return bucket object

// add reference to bucket and return reference guid

// remove from bucket via reference guid

// remove bucket via bucket guid

// get ical from bucket

// refresh references via bucket id

// refresh references via reference id


app.Run();
