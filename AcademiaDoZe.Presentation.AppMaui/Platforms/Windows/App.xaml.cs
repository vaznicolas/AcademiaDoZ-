using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AcademiaDoZe.Presentation.AppMaui.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        var arquivoErro = Path.Combine(Path.GetTempPath(), "academia_erro.txt");

        this.UnhandledException += (s, e) =>
        {
            try
            {
                File.WriteAllText(arquivoErro,
                    "MENSAGEM: " + e.Message + Environment.NewLine + Environment.NewLine +
                    "EXCECAO: " + e.Exception);
            }
            catch { }
        };

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            try
            {
                File.WriteAllText(arquivoErro,
                    "DOMAIN: " + e.ExceptionObject);
            }
            catch { }
        };

        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

