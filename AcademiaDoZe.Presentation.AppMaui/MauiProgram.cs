// Nicolas Vaz

using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using AcademiaDoZe.Presentation.AppMaui.Views;
using Microsoft.Extensions.Logging;

namespace AcademiaDoZe.Presentation.AppMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont(
                    "OpenSans-Regular.ttf",
                    "OpenSansRegular");

                fonts.AddFont(
                    "OpenSans-Semibold.ttf",
                    "OpenSansSemibold");
            });

        // Configuração do banco e serviços da aplicação
        AcademiaDoZe.Presentation.AppMaui.Configuration.ConfigurationHelper.ConfigureServices(
        builder.Services);

        // ViewModels
        builder.Services.AddTransient<DashboardListViewModel>();
        builder.Services.AddTransient<LogradouroListViewModel>();
        builder.Services.AddTransient<LogradouroViewModel>();

        // Pages
        builder.Services.AddTransient<DashboardListPage>();
        builder.Services.AddTransient<LogradouroListPage>();
        builder.Services.AddTransient<LogradouroPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}