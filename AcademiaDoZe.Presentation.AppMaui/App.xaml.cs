// Nicolas Vaz

namespace AcademiaDoZe.Presentation.AppMaui;

public partial class App : global::Microsoft.Maui.Controls.Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}