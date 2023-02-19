using Google.Apis.Calendar.v3.Data;

namespace evre;

public class StartEventUseCase
{
    private readonly EventRepository _eventRepository;
    private readonly OngoingEventRepository _ongoingEventRepository;

    public StartEventUseCase(EventRepository eventRepository, OngoingEventRepository ongoingEventRepository)
    {
        _eventRepository = eventRepository;
        _ongoingEventRepository = ongoingEventRepository;
    }

    public async Task Execute(string name, string description, DateTime startAt)
    {
        var e = new Event
        {
            Summary = name,
            Description = description,
            Start = new EventDateTime
            {
                DateTime = startAt
            },
            End = new EventDateTime
            {
                DateTime = startAt.Add(TimeSpan.FromMinutes(10))
            }
        };
        var response = await _eventRepository.Insert(e);
        _ongoingEventRepository.Update(response.Id);
    }
}
