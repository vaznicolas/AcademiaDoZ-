
// Nicolas Vaz

using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class MatriculaPage : ContentPage
{
    private bool _inicializando;

    public MatriculaPage(MatriculaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is not MatriculaViewModel viewModel)
            return;

        _inicializando = true;

        await viewModel.InitializeAsync();

        DataInicioPicker.Date =
            viewModel.Matricula.DataInicio.ToDateTime(TimeOnly.MinValue);

        DataFimPicker.Date =
            viewModel.Matricula.DataFim.ToDateTime(TimeOnly.MinValue);

        PlanoPicker.SelectedItem = viewModel.Matricula.Plano;

        _inicializando = false;
    }

    private void OnDataInicioChanged(
        object? sender,
        DateChangedEventArgs e)
    {
        if (_inicializando ||
            BindingContext is not MatriculaViewModel viewModel)
            return;

        if (e.NewDate is not DateTime novaData)
            return;

        viewModel.Matricula.DataInicio =
            DateOnly.FromDateTime(novaData);

        AtualizarDataFim();
    }

    private void OnPlanoChanged(
        object? sender,
        EventArgs e)
    {
        if (_inicializando ||
            BindingContext is not MatriculaViewModel viewModel ||
            PlanoPicker.SelectedItem == null)
            return;

        viewModel.Matricula.Plano =
            (AppMatriculaPlano)PlanoPicker.SelectedItem;

        AtualizarDataFim();
    }

    private void AtualizarDataFim()
    {
        if (BindingContext is not MatriculaViewModel viewModel)
            return;

        var dataInicio = viewModel.Matricula.DataInicio;
        var plano = viewModel.Matricula.Plano;

        var meses = plano switch
        {
            AppMatriculaPlano.Mensal => 1,
            AppMatriculaPlano.Trimestral => 3,
            AppMatriculaPlano.Semestral => 6,
            AppMatriculaPlano.Anual => 12,
            _ => 12
        };

        var dataFim = dataInicio.AddMonths(meses);

        viewModel.Matricula.DataFim = dataFim;

        DataFimPicker.Date =
            dataFim.ToDateTime(TimeOnly.MinValue);
    }
}
