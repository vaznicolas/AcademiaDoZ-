// Nicolas Vaz

using System.Collections.ObjectModel;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class LogradouroListViewModel : BaseViewModel
{
    private readonly ILogradouroService _logradouroService;

    public ObservableCollection<LogradouroDto> Logradouros { get; } = new();

    public List<string> FilterTypes { get; } =
[
    "Todos",
    "CEP",
    "Cidade"
];

    private string _selectedFilterType = "Todos";
    public string SelectedFilterType
    {
        get => _selectedFilterType;
        set => SetProperty(ref _selectedFilterType, value);
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public LogradouroListViewModel(ILogradouroService logradouroService)
    {
        _logradouroService = logradouroService;
        Title = "Logradouros";
    }

    [RelayCommand]
    private async Task LoadLogradourosAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
            TimeSpan.FromMinutes(2));

            var logradouros =
                await _logradouroService.ObterTodosAsync(cts.Token);

            Logradouros.Clear();

            foreach (var logradouro in logradouros)
            {
                Logradouros.Add(logradouro);
            }
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Tempo Esgotado",
                "O carregamento dos logradouros expirou.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Erro ao carregar logradouros: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;

        try
        {
            await LoadLogradourosAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task SearchLogradourosAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText) ||
            SelectedFilterType == "Todos")
        {
            await LoadLogradourosAsync();
            return;
        }

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(5));

            IEnumerable<LogradouroDto> resultado = SelectedFilterType switch
            {
                "CEP" =>
                    await _logradouroService.ObterPorCepAsync(
                        SearchText,
                        cts.Token) is { } porCep
                        ? [porCep]
                        : [],

                "Cidade" =>
                    await _logradouroService.ObterPorCidadeAsync(
                        SearchText,
                        cts.Token),

                _ =>
                    await _logradouroService.ObterTodosAsync(
                        cts.Token)
            };

            Logradouros.Clear();

            foreach (var logradouro in resultado)
            {
                Logradouros.Add(logradouro);
            }
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Tempo Esgotado",
                "A busca expirou.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Erro ao buscar logradouros: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddLogradouroAsync()
    {
        await Shell.Current.GoToAsync("logradouro");
    }

    [RelayCommand]
    private async Task EditLogradouroAsync(LogradouroDto logradouro)
    {
        if (logradouro == null)
            return;

        await Shell.Current.GoToAsync(
            $"logradouro?Id={logradouro.Id}");
    }

    [RelayCommand]
    private async Task DeleteLogradouroAsync(LogradouroDto logradouro)
    {
        if (logradouro == null)
            return;

        bool confirmar = await Shell.Current.DisplayAlertAsync(
            "Confirmar exclusão",
            $"Deseja excluir o logradouro '{logradouro.Nome}'?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(5));

            await _logradouroService.RemoverAsync(
                logradouro.Id,
                cts.Token);

            Logradouros.Remove(logradouro);

            await Shell.Current.DisplayAlertAsync(
                "Sucesso",
                "Logradouro excluído com sucesso!",
                "OK");
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Tempo Esgotado",
                "A exclusão expirou.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Erro ao excluir logradouro: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}