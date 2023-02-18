using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace evre;

public class EventRepository
{
    private const string CalendarId = "primary";

    private readonly Authorizer _authorizer;

    public EventRepository(Authorizer authorizer)
    {
        _authorizer = authorizer;
    }

    private CalendarService Service => new(_authorizer.Initializer);

    public Task<Event> Insert(Event e)
    {
        return Service.Events.Insert(e, CalendarId).ExecuteAsync();
    }

    public async Task Update(Event e, string eventId)
    {
        await Service.Events.Update(e, CalendarId, eventId).ExecuteAsync();
    }
}
