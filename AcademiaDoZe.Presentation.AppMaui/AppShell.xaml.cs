// Nicolas Vaz

using AcademiaDoZe.Presentation.AppMaui.Views;

namespace AcademiaDoZe.Presentation.AppMaui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("aluno", typeof(AlunoPage));
        Routing.RegisterRoute("colaborador", typeof(ColaboradorPage));
        RegisterRoutes();
    }

    private static void RegisterRoutes()
    {
        Routing.RegisterRoute(
            "logradouro",
            typeof(LogradouroPage));
    }
}