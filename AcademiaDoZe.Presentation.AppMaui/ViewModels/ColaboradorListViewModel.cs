
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class ColaboradorListViewModel : BaseViewModel
{
    private readonly IColaboradorService _colaboradorService;

    public ObservableCollection<ColaboradorDto> Colaboradores { get; } = [];

    public ObservableCollection<string> FilterTypes { get; } =
        ["Nome", "CPF", "Id", "Email", "Tipo", "Vínculo"];

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private string _selectedFilterType = "Nome";
    public string SelectedFilterType
    {
        get => _selectedFilterType;
        set => SetProperty(ref _selectedFilterType, value);
    }

    public ColaboradorListViewModel(IColaboradorService colaboradorService)
    {
        _colaboradorService = colaboradorService;
        Title = "Colaboradores";
    }

    [RelayCommand]
    private async Task LoadColaboradoresAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            var resultado = await _colaboradorService.ObterTodosAsync();

            Colaboradores.Clear();

            foreach (var colaborador in resultado)
                Colaboradores.Add(colaborador);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro", $"Não foi possível carregar os colaboradores: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddColaboradorAsync()
    {
        await Shell.Current.GoToAsync("colaborador");
    }

    [RelayCommand]
    private async Task EditColaboradorAsync(ColaboradorDto colaborador)
    {
        if (colaborador is null)
            return;

        await Shell.Current.GoToAsync($"colaborador?Id={colaborador.Id}");
    }

    [RelayCommand]
    private async Task SearchColaboradoresAsync()
    {
        var termo = SearchText.Trim();

        if (string.IsNullOrWhiteSpace(termo))
        {
            await LoadColaboradoresAsync();
            return;
        }

        var todos = await _colaboradorService.ObterTodosAsync();

        var filtrados = todos.Where(c =>
            SelectedFilterType switch
    {
        "Nome" => c.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase),
        "CPF" => c.Cpf.Contains(termo, StringComparison.OrdinalIgnoreCase),
        "Id" => c.Id.ToString().Contains(termo),
        "Email" => (c.Email ?? "").Contains(termo, StringComparison.OrdinalIgnoreCase),
        "Tipo" => c.Tipo.ToString().Contains(termo, StringComparison.OrdinalIgnoreCase),
        "Vínculo" => c.Vinculo.ToString().Contains(termo, StringComparison.OrdinalIgnoreCase),
        _ => true
    });

        Colaboradores.Clear();

        foreach (var colaborador in filtrados)
            Colaboradores.Add(colaborador);
    }

    [RelayCommand]
    private async Task DeleteColaboradorAsync(ColaboradorDto colaborador)
    {
        if (colaborador is null)
            return;

        bool confirmar = await Shell.Current.DisplayAlertAsync(
            "Confirmar exclusão",
            $"Deseja excluir o colaborador {colaborador.Nome}?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        try
        {
            IsBusy = true;

            bool removido = await _colaboradorService.RemoverAsync(colaborador.Id);

            if (removido)
            {
                Colaboradores.Remove(colaborador);

                await Shell.Current.DisplayAlertAsync(
                    "Sucesso",
                    "Colaborador excluído.",
                    "OK");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync(
                    "Aviso",
                    "Não foi possível excluir o colaborador.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Erro ao excluir colaborador: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
