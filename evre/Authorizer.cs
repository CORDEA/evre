using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

namespace evre;

public class Authorizer
{
    public BaseClientService.Initializer Initializer { get; private set; }

    public async Task Authorize()
    {
        if (Initializer != null) return;
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
        Initializer = new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        };
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
