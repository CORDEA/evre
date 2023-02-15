using System.Collections.Specialized;
using System.Web;

namespace evre;

public static class LaunchUriHandler
{
    private static readonly TaskCompletionSource<LaunchResult> OnReturned = new();

    public static Task<LaunchResult> LaunchResult => OnReturned.Task;

    public static bool TryHandle(Uri uri)
    {
        if (uri.Scheme != AppInfo.PackageName) return false;
        var type = uri.AbsolutePath == LaunchUriBuilder.RedirectPath ? LaunchType.OAuth2Redirect : LaunchType.Unknown;
        var query = HttpUtility.ParseQueryString(uri.Query);
        OnReturned.SetResult(new LaunchResult(type, query));
        return true;
    }
}

public class LaunchResult
{
    public LaunchResult(LaunchType type, NameValueCollection query)
    {
        Type = type;
        Query = query;
    }

    public LaunchType Type { get; }
    public NameValueCollection Query { get; }
}
