
// Nicolas Vaz

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class AlunoListViewModel : BaseViewModel
{
    private readonly IAlunoService _alunoService;

    public ObservableCollection<AlunoDto> Alunos { get; } = [];

    public ObservableCollection<string> FilterTypes { get; } =
        ["Nome", "CPF", "Id", "Email"];

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

    public AlunoListViewModel(IAlunoService alunoService)
    {
        _alunoService = alunoService;
        Title = "Alunos";
    }

    [RelayCommand]
    private async Task LoadAlunosAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var resultado = await _alunoService.ObterTodosAsync();

            Alunos.Clear();

            foreach (var aluno in resultado)
                Alunos.Add(aluno);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Não foi possível carregar os alunos: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddAlunoAsync()
    {
        await Shell.Current.GoToAsync("aluno");
    }

    [RelayCommand]
    private async Task EditAlunoAsync(AlunoDto aluno)
    {
        if (aluno is null)
            return;

        await Shell.Current.GoToAsync($"aluno?Id={aluno.Id}");
    }

    [RelayCommand]
    private async Task SearchAlunosAsync()
    {
        var termo = SearchText.Trim();

        if (string.IsNullOrWhiteSpace(termo))
        {
            await LoadAlunosAsync();
            return;
        }

        try
        {
            IsBusy = true;

            var todos = await _alunoService.ObterTodosAsync();

            var filtrados = todos.Where(aluno =>
                SelectedFilterType switch
                {
                    "Nome" => aluno.Nome.Contains(
                        termo, StringComparison.OrdinalIgnoreCase),

                    "CPF" => aluno.Cpf.Contains(
                        termo, StringComparison.OrdinalIgnoreCase),

                    "Id" => aluno.Id.ToString().Contains(termo),

                    "Email" => (aluno.Email ?? "").Contains(
                        termo, StringComparison.OrdinalIgnoreCase),

                    _ => true
                });

            Alunos.Clear();

            foreach (var aluno in filtrados)
                Alunos.Add(aluno);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Não foi possível pesquisar alunos: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAlunoAsync(AlunoDto aluno)
    {
        if (aluno is null || IsBusy)
            return;

        bool confirmar = await Shell.Current.DisplayAlertAsync(
            "Confirmar exclusão",
            $"Deseja excluir o aluno {aluno.Nome}?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        try
        {
            IsBusy = true;

            bool removido = await _alunoService.RemoverAsync(aluno.Id);

            if (removido)
            {
                Alunos.Remove(aluno);

                await Shell.Current.DisplayAlertAsync(
                    "Sucesso", "Aluno excluído.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync(
                    "Aviso", "Não foi possível excluir o aluno.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Erro ao excluir aluno: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
