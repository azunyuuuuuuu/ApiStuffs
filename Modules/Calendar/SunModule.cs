using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Innovative.SolarCalculator;
using Microsoft.AspNetCore.Mvc;

public class SunModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services)
    {
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/sun/", (
            HttpContext context,
            [FromQuery(Name = "lat")] double latitude,
            [FromQuery(Name = "lon")] double longitude,
            [FromQuery(Name = "sunrise")] bool? includeSunrise,
            [FromQuery(Name = "sunset")] bool? includeSunset,
            [FromQuery(Name = "daylight")] bool? includeDayLight,
            [FromQuery(Name = "night")] bool? includeNightTime
            ) =>
        {
            var output = new Calendar();

            var res = Enumerable.Range(0, 365)
                .Select(x => new DateTime(DateTimeOffset.Now.Year, 1, 1) + TimeSpan.FromDays(x))
                .Where(x => x.Year == DateTimeOffset.Now.Year)
                .Select(x => new
                {
                    PreviousDay = new SolarTimes(new DateTimeOffset(x.AddDays(-1)), latitude, longitude),
                    CurrentDay = new SolarTimes(new DateTimeOffset(x), latitude, longitude),
                    NextDay = new SolarTimes(new DateTimeOffset(x.AddDays(1)), latitude, longitude),
                }
                )
                .Select(x => new
                {
                    Date = x.CurrentDay.ForDate,
                    SunsetEndPreviousDay = new CalDateTime(x.PreviousDay.DawnCivil - TimeSpan.FromHours((double)x.PreviousDay.TimeZoneOffset), "UTC"),
                    SunriseBegin = new CalDateTime(x.CurrentDay.DawnCivil - TimeSpan.FromHours((double)x.CurrentDay.TimeZoneOffset), "UTC"),
                    SunriseEnd = new CalDateTime(x.CurrentDay.Sunrise - TimeSpan.FromHours((double)x.CurrentDay.TimeZoneOffset), "UTC"),
                    SunsetBegin = new CalDateTime(x.CurrentDay.Sunset - TimeSpan.FromHours((double)x.CurrentDay.TimeZoneOffset), "UTC"),
                    SunsetEnd = new CalDateTime(x.CurrentDay.DuskCivil - TimeSpan.FromHours((double)x.CurrentDay.TimeZoneOffset), "UTC"),
                    SunriseBeginNextDay = new CalDateTime(x.NextDay.DawnCivil - TimeSpan.FromHours((double)x.NextDay.TimeZoneOffset), "UTC"),
                    Latitude = latitude,
                    Longitude = longitude
                })
                .Select(x => new
                {
                    SunriseEvent = new CalendarEvent
                    {
                        Summary = "🌅 Sunrise",
                        Start = x.SunriseBegin,
                        End = x.SunriseEnd
                    },
                    SunsetEvent = new CalendarEvent
                    {
                        Summary = "🌇 Sunset",
                        Start = x.SunsetBegin,
                        End = x.SunsetEnd
                    },
                    DayLight = new CalendarEvent
                    {
                        Summary = "☀️ Daylight",
                        Start = x.SunriseEnd,
                        End = x.SunsetBegin
                    },
                    NightTime = new CalendarEvent
                    {
                        Summary = "🌕 Night",
                        Start = x.SunsetEnd,
                        End = x.SunriseBeginNextDay
                    }
                });

            foreach (var item in res)
            {
                if (includeSunrise == true)
                    output.AddChild<CalendarEvent>(item.SunriseEvent);
                if (includeSunset == true)
                    output.AddChild<CalendarEvent>(item.SunsetEvent);
                if (includeNightTime == true)
                    output.AddChild<CalendarEvent>(item.NightTime);
                if (includeDayLight == true)
                    output.AddChild<CalendarEvent>(item.DayLight);
            }

            var calendarserializer = new CalendarSerializer();
            var serialized = calendarserializer.SerializeToString(output);
            return Results.Text(serialized, "text/calendar");
        });


        return endpoints;
    }
}
