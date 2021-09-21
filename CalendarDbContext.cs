using Microsoft.EntityFrameworkCore;

public class CalendarDbContext : DbContext
{
    public DbSet<Collection> Collections { get; set; }
}

public class Collection
{
    public Guid CollectionId { get; set; } = Guid.NewGuid();
    public List<Calendar> Calendars { get; set; } = new List<Calendar>();
}

public class Calendar
{
    public Guid CalendarId { get; set; } = Guid.NewGuid();

    public Uri SourceUri { get; set; } =

    public List<CalendarEntry> CalendarEntries { get; set; } = new List<CalendarEntry>();
}

public record CalendarEntry
{
    public Guid Id { get; init; }
    public string Title { get; set; }
    public string Subject { get; init; }
    public string Location { get; init; }
    public string Body { get; init; }
    public EventStatus EventStatus { get; init; }
    public DateTimeOffset EventStart { get; init; }
    public DateTimeOffset EventEnd { get; init; }
    public bool IsAllDay { get; init; }
}

public enum EventStatus
{
    Free,
    Tentative,
    Busy,
    OutOfOffice
}