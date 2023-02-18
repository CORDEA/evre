using Microsoft.Extensions.Logging;

namespace evre;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddSingleton<Authorizer>();
        builder.Services.AddSingleton<EventRepository>();
        builder.Services.AddSingleton<OngoingEventRepository>();
        return builder.Build();
    }
}
