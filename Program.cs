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

// create calendar via magic string and return bucket object
app.MapPost("/cal", () => "henlo :3");

// add reference to calendar and return reference object
app.MapPut("/cal/{bucketid:guid}", (Guid bucketid) => "henlo :3");

// remove reference via reference guid
app.MapDelete("/ref/{bucketid:guid}/{referenceid:guid}", (Guid bucketid, Guid referenceid) => "henlo :3");

// remove calendar via bucket guid
app.MapDelete("/cal/{bucketid:guid}", (Guid bucketid) => "henlo :3");

// get ical from calendar with optional refresh
app.MapGet("/cal/{bucketid:guid}", (Guid bucketid) => "henlo :3");

// get single reference with optional refresh
app.MapGet("/ref/{referenceid:guid}", (Guid referenceid) => "henlo :3");

app.Run();
