using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;

namespace evre;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var name = DeviceInfo.Current.Platform == DevicePlatform.Android
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
    }

    private class CodeReceiver : ICodeReceiver
    {
        // https://developers.google.com/identity/protocols/oauth2/native-app?hl=en
        public string RedirectUri => "jp.cordea.evre:oauth2/redirect";

        public async Task<AuthorizationCodeResponseUrl> ReceiveCodeAsync(AuthorizationCodeRequestUrl url,
            CancellationToken taskCancellationToken)
        {
            await Browser.OpenAsync(url.Build().AbsoluteUri, BrowserLaunchMode.External);
            throw new NotImplementedException();
        }
    }
}
