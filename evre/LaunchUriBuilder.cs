namespace evre;

public class LaunchUriBuilder
{
    public const string RedirectPath = "oauth2/redirect";

    private readonly LaunchType _type;

    public LaunchUriBuilder(LaunchType type)
    {
        _type = type;
    }

    public Uri Build()
    {
        switch (_type)
        {
            case LaunchType.OAuth2Redirect:
                return new UriBuilder(AppInfo.PackageName, "")
                {
                    Path = RedirectPath
                }.Uri;
            default:
                throw new NotSupportedException();
        }
    }
}
