
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

        // Aluno
        builder.Services.AddTransient<AlunoListViewModel>();
        builder.Services.AddTransient<AlunoViewModel>();
        builder.Services.AddTransient<AlunoListPage>();
        builder.Services.AddTransient<AlunoPage>();

        // Dashboard e Logradouro
        builder.Services.AddTransient<DashboardListViewModel>();
        builder.Services.AddTransient<LogradouroListViewModel>();
        builder.Services.AddTransient<LogradouroViewModel>();

        builder.Services.AddTransient<ConfigPage>();
        builder.Services.AddTransient<DashboardListPage>();
        builder.Services.AddTransient<LogradouroListPage>();
        builder.Services.AddTransient<LogradouroPage>();

        // Colaborador
        builder.Services.AddTransient<ColaboradorListViewModel>();
        builder.Services.AddTransient<ColaboradorViewModel>();
        builder.Services.AddTransient<ColaboradorListPage>();
        builder.Services.AddTransient<ColaboradorPage>();

        // Matrícula
        builder.Services.AddTransient<MatriculaListViewModel>();
        builder.Services.AddTransient<MatriculaViewModel>();
        builder.Services.AddTransient<MatriculaListPage>();
        builder.Services.AddTransient<MatriculaPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
