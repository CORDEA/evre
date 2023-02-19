namespace evre;

public class RemoveOngoingEventUseCase
{
    private readonly EventRepository _eventRepository;
    private readonly OngoingEventRepository _ongoingEventRepository;

    public RemoveOngoingEventUseCase(EventRepository eventRepository, OngoingEventRepository ongoingEventRepository)
    {
        _eventRepository = eventRepository;
        _ongoingEventRepository = ongoingEventRepository;
    }

    public async Task Execute()
    {
        var id = _ongoingEventRepository.Find();
        if (string.IsNullOrWhiteSpace(id)) throw new InvalidOperationException();
        await _eventRepository.Delete(id);
        _ongoingEventRepository.Remove();
    }
}
