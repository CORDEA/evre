using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Google;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace evre;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly Authorizer _authorizer;
    private string _description = "";
    private string _name = "";

    public MainViewModel(Authorizer authorizer)
    {
        _authorizer = authorizer;
    }

    public ICommand AddEventCommand => new Command(RegisterEvent);

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set => SetField(ref _description, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;


    private async void RegisterEvent()
    {
        if (string.IsNullOrWhiteSpace(Name)) return;
        if (_authorizer.Initializer == null)
            try
            {
                await _authorizer.Authorize();
            }
            catch (Exception exception)
            {
                // TODO
                return;
            }

        var now = DateTime.Now;
        var ev = new Event
        {
            Summary = Name,
            Description = Description,
            Start = new EventDateTime
            {
                DateTime = now
            },
            End = new EventDateTime
            {
                DateTime = now.Add(TimeSpan.FromMinutes(10))
            }
        };
        var service = new CalendarService(_authorizer.Initializer);
        Event response;
        try
        {
            response = await service.Events.Insert(ev, "primary").ExecuteAsync();
        }
        catch (GoogleApiException exception)
        {
            // TODO
        }
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
