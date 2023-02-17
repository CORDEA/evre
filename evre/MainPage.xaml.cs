using System.ComponentModel;
using Google;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace evre;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    private readonly Authorizer _authorizer;
    private string _description = "";
    private string _name = "";

    public MainPage(Authorizer authorizer)
    {
        _authorizer = authorizer;
        InitializeComponent();
        BindingContext = this;
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged();
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Name)) return;
        if (_authorizer.Initializer == null)
        {
            try
            {
                await _authorizer.Authorize();
            }
            catch (Exception exception)
            {
                // TODO
                return;
            }
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
}
