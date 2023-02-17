using System.ComponentModel;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

namespace evre;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    private CalendarService _calendarService;
    private string _description = "";
    private string _name = "";

    public MainPage()
    {
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
        _calendarService ??= await Authorize();
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
        await _calendarService.Events.Insert(ev, "primary").ExecuteAsync();
    }

    private async Task<CalendarService> Authorize()
    {
        var platform = DeviceInfo.Current.Platform;
        var name = platform == DevicePlatform.Android
            ? "client_secret.android.txt"
            : "client_secret.ios.txt";
        await using var stream = await FileSystem.OpenAppPackageFileAsync(name);
        using var reader = new StreamReader(stream);
        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets
            {
                ClientId = await reader.ReadLineAsync()
            },
            new[] { CalendarService.Scope.CalendarEvents },
            "user",
            CancellationToken.None,
            codeReceiver: new CodeReceiver()
        );
        return new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        });
    }

    private class CodeReceiver : ICodeReceiver
    {
        public string RedirectUri => new LaunchUriBuilder(LaunchType.OAuth2Redirect).Build().AbsoluteUri;

        public async Task<AuthorizationCodeResponseUrl> ReceiveCodeAsync(AuthorizationCodeRequestUrl url,
            CancellationToken taskCancellationToken)
        {
            await Launcher.Default.OpenAsync(url.Build().AbsoluteUri);
            var result = await LaunchUriHandler.LaunchResult;
            if (result.Type != LaunchType.OAuth2Redirect) throw new OperationCanceledException();
            return new AuthorizationCodeResponseUrl
            {
                Code = result.Query.Get("code"),
                State = result.Query.Get("state"),
                Error = result.Query.Get("error"),
                ErrorDescription = result.Query.Get("error_description"),
                ErrorUri = result.Query.Get("error_uri")
            };
        }
    }
}
