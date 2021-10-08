using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.Serialization;
using LiteDB.Async;

public class CalendarModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<ILiteDatabaseAsync, LiteDatabaseAsync>(_ => new LiteDatabaseAsync("Filename=data/calendar.db;Connection=shared;"));
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/calendar/{id}.ics", async (HttpContext context, ILiteDatabaseAsync db, IHttpClientFactory clientFactory, string id) =>
        {
            using var client = clientFactory.CreateClient();

            var guid = id.ToGuidFromShortString();
            var collection = db.GetCollection<CalendarReference>();
            var entity = await collection.FindOneAsync(x => x.Id == guid);

            var results = new Calendar();

            foreach (var uri in entity.Uris)
            {
                var temp = await client.GetStringAsync(uri);
                var calendar = Calendar.Load(temp);
                var filteredEvents = calendar.Events.Where(x => x.Start.AsDateTimeOffset > DateTimeOffset.Now.AddMonths(-3) && x.Start.AsDateTimeOffset < DateTimeOffset.Now.AddMonths(3));
                foreach (var entry in filteredEvents)
                {
                    results.AddChild<CalendarEvent>(entry);
                }
            }

            var calendarserializer = new CalendarSerializer();
            var serialized = calendarserializer.SerializeToString(results);
            return Results.Text(serialized, "text/calendar");
        });

        endpoints.MapPost("/calendar", async (HttpContext context, ILiteDatabaseAsync db, CalendarPostDto dto) =>
        {
            // check input
            if (Uri.TryCreate(dto.Url, UriKind.Absolute, out var uri) == false
                && (uri.Scheme != Uri.UriSchemeHttp || uri.Scheme != Uri.UriSchemeHttps))
                throw new ArgumentException("Url is not valid", nameof(CalendarPostDto.Url));

            // add to database
            var entity = new CalendarReference { Id = Guid.NewGuid(), Uris = new List<Uri> { uri } };

            var collection = db.GetCollection<CalendarReference>();
            await collection.InsertAsync(entity);

            await context.Response.WriteAsJsonAsync(new { id = entity.Id.ToShortString() });
        });

        endpoints.MapPut("/calendar", async (HttpContext context, ILiteDatabaseAsync db, CalendarPutDto dto) =>
        {
            if (Uri.TryCreate(dto.Url, UriKind.Absolute, out var uri) == false
                && (uri.Scheme != Uri.UriSchemeHttp || uri.Scheme != Uri.UriSchemeHttps))
                throw new ArgumentException("Url is not valid", nameof(CalendarPutDto.Url));

            var guid = dto.Id.ToGuidFromShortString();
            var collection = db.GetCollection<CalendarReference>();

            var entity = await collection.FindOneAsync(x => x.Id == guid);

            entity.Uris.Add(uri);
            await collection.UpdateAsync(entity);

            await context.Response.WriteAsJsonAsync(new
            {
                id = entity.Id.ToShortString()
            });
        });

        return endpoints;
    }

    private record CalendarPostDto(string Url);
    private record CalendarPutDto(string Id, string Url);
    private record CalendarReference
    {
        public Guid Id { get; init; }
        public List<Uri> Uris { get; init; }
    };
}
