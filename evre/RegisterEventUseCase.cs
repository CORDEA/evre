using Google.Apis.Calendar.v3.Data;

namespace evre;

public class RegisterEventUseCase
{
    private readonly EventRepository _eventRepository;
    private readonly OngoingEventRepository _ongoingEventRepository;

    public RegisterEventUseCase(EventRepository eventRepository, OngoingEventRepository ongoingEventRepository)
    {
        _eventRepository = eventRepository;
        _ongoingEventRepository = ongoingEventRepository;
    }

    public async Task Execute(Event e)
    {
        var response = await _eventRepository.Insert(e);
        _ongoingEventRepository.Update(response.Id);
    }
}
