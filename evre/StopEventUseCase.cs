using Google.Apis.Calendar.v3.Data;

namespace evre;

public class StopEventUseCase
{
    private readonly EventRepository _eventRepository;
    private readonly OngoingEventRepository _ongoingEventRepository;

    public StopEventUseCase(EventRepository eventRepository, OngoingEventRepository ongoingEventRepository)
    {
        _eventRepository = eventRepository;
        _ongoingEventRepository = ongoingEventRepository;
    }

    public async Task Execute(DateTime endAt)
    {
        var id = _ongoingEventRepository.Find();
        if (string.IsNullOrWhiteSpace(id)) throw new InvalidOperationException();
        var e = await _eventRepository.Find(id);
        e.End = new EventDateTime
        {
            DateTime = endAt
        };
        await _eventRepository.Update(e, id);
        _ongoingEventRepository.Remove();
    }
}
