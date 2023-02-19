namespace evre;

public class RemoveEventCacheUseCase
{
    private readonly OngoingEventRepository _repository;

    public RemoveEventCacheUseCase(OngoingEventRepository repository)
    {
        _repository = repository;
    }

    public void Execute()
    {
        _repository.Remove();
    }
}
