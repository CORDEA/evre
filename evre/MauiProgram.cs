using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace evre;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<Authorizer>();

        builder.Services.AddSingleton<EventRepository>();
        builder.Services.AddSingleton<OngoingEventRepository>();

        builder.Services.AddSingleton<HasOngoingEventUseCase>();
        builder.Services.AddSingleton<StartEventUseCase>();
        builder.Services.AddSingleton<StopEventUseCase>();
        builder.Services.AddSingleton<RemoveOngoingEventUseCase>();

        builder.Services.AddTransient<MainViewModel>();

        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
