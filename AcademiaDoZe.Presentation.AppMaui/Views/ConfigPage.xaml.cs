
// Nicolas Vaz

using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ConfigPage : ContentPage
{
    public ConfigPage()
    {
        InitializeComponent();

        TemaPicker.SelectedIndex = Preferences.Get("Tema", "system") switch
        {
            "light" => 1,
            "dark" => 2,
            _ => 0
        };

        ServidorEntry.Text = Preferences.Get("SqlServer_Servidor", "localhost,1433");
        BancoEntry.Text = Preferences.Get("SqlServer_Banco", "db_academia_do_ze");
        UsuarioEntry.Text = Preferences.Get("SqlServer_Usuario", "academia");
        SenhaEntry.Text = Preferences.Get("SqlServer_Senha", "");
    }

    private void OnSalvarTemaClicked(object sender, EventArgs e)
    {
        string tema = TemaPicker.SelectedIndex switch
        {
            1 => "light",
            2 => "dark",
            _ => "system"
        };

        Preferences.Set("Tema", tema);
        WeakReferenceMessenger.Default.Send(
            new TemaPreferencesUpdatedMessage("TemaAlterado"));

        DisplayAlert("Configurações", "Tema salvo.", "OK");
    }

    private void OnSalvarBancoClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ServidorEntry.Text) ||
            string.IsNullOrWhiteSpace(BancoEntry.Text) ||
            string.IsNullOrWhiteSpace(UsuarioEntry.Text) ||
            string.IsNullOrWhiteSpace(SenhaEntry.Text))
        {
            DisplayAlert(
                "Atenção",
                "Preencha todos os campos do banco de dados.",
                "OK");
            return;
        }

        Preferences.Set("SqlServer_Servidor", ServidorEntry.Text.Trim());
        Preferences.Set("SqlServer_Banco", BancoEntry.Text.Trim());
        Preferences.Set("SqlServer_Usuario", UsuarioEntry.Text.Trim());
        Preferences.Set("SqlServer_Senha", SenhaEntry.Text);

        WeakReferenceMessenger.Default.Send(
            new BancoPreferencesUpdatedMessage("BancoAlterado"));

        DisplayAlert(
            "Configurações",
            "Credenciais salvas.",
            "OK");
    }
}
