using Google.Apis.Calendar.v3.Data;

namespace evre;

public class UpdateOngoingEventUseCase
{
    private readonly EventRepository _eventRepository;
    private readonly OngoingEventRepository _ongoingEventRepository;

    public UpdateOngoingEventUseCase(EventRepository eventRepository, OngoingEventRepository ongoingEventRepository)
    {
        _eventRepository = eventRepository;
        _ongoingEventRepository = ongoingEventRepository;
    }

    public async Task Execute(Event e)
    {
        var id = _ongoingEventRepository.Find();
        if (string.IsNullOrWhiteSpace(id)) throw new InvalidOperationException();
        await _eventRepository.Update(e, id);
    }
}
