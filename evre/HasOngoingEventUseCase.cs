namespace evre;

public class HasOngoingEventUseCase
{
    private readonly OngoingEventRepository _repository;

    public HasOngoingEventUseCase(OngoingEventRepository repository)
    {
        _repository = repository;
    }

    public bool Execute()
    {
        return !string.IsNullOrWhiteSpace(_repository.Find());
    }
}
