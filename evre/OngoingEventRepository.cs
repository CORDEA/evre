namespace evre;

public class OngoingEventRepository
{
    private const string OngoingEventKey = "OngoingEventKey";

    public string Find()
    {
        return Preferences.Default.Get(OngoingEventKey, "");
    }

    public void Update(string id)
    {
        Preferences.Default.Set(OngoingEventKey, id);
    }
}
