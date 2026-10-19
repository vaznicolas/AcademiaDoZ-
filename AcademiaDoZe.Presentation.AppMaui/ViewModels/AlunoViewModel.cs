
// Nicolas Vaz

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

[QueryProperty(nameof(AlunoId), "Id")]
public partial class AlunoViewModel : BaseViewModel
{
    private readonly IAlunoService _alunoService;
    private readonly ILogradouroService _logradouroService;

    private AlunoDto _aluno = CriarAlunoPadrao();

    public AlunoDto Aluno
    {
        get => _aluno;
        set => SetProperty(ref _aluno, value);
    }

    private int _alunoId;
    public int AlunoId
    {
        get => _alunoId;
        set => SetProperty(ref _alunoId, value);
    }

    private bool _isEditMode;
    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    private string _confirmarSenha = string.Empty;
    public string ConfirmarSenha
    {
        get => _confirmarSenha;
        set => SetProperty(ref _confirmarSenha, value);
    }

    public DateTime DataNascimentoDateTime
    {
        get => Aluno.DataNascimento.ToDateTime(TimeOnly.MinValue);
        set
        {
            Aluno.DataNascimento = DateOnly.FromDateTime(value);
            OnPropertyChanged();
        }
    }

    public AlunoViewModel(
        IAlunoService alunoService,
        ILogradouroService logradouroService)
    {
        _alunoService = alunoService;
        _logradouroService = logradouroService;
        Title = "Cadastro de Aluno";
    }

    private static AlunoDto CriarAlunoPadrao() => new()
    {
        Nome = string.Empty,
        Cpf = string.Empty,
        DataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-18)),
        Telefone = string.Empty,
        Email = string.Empty,
        Senha = string.Empty,

        Endereco = new LogradouroDto
        {
            Cep = string.Empty,
            Nome = string.Empty,
            Bairro = string.Empty,
            Cidade = string.Empty,
            Estado = string.Empty,
            Pais = "Brasil"
        },

        Numero = string.Empty,
        Complemento = string.Empty
    };

    public async Task InitializeAsync()
    {
        ConfirmarSenha = string.Empty;

        if (AlunoId > 0)
        {
            IsEditMode = true;
            Title = "Editar Aluno";
            await LoadAlunoAsync();
        }
        else
        {
            IsEditMode = false;
            Title = "Novo Aluno";
            Aluno = CriarAlunoPadrao();
            OnPropertyChanged(nameof(DataNascimentoDateTime));
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task LoadAlunoAsync()
    {
        if (AlunoId <= 0)
            return;

        try
        {
            IsBusy = true;

            using var cts =
                new CancellationTokenSource(TimeSpan.FromSeconds(5));

            var alunoData = await _alunoService.ObterPorIdAsync(
                AlunoId, cts.Token);

            if (alunoData is null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Aviso", "Aluno não encontrado.", "OK");
                return;
            }

            alunoData.Endereco ??= new LogradouroDto
            {
                Cep = string.Empty,
                Nome = string.Empty,
                Bairro = string.Empty,
                Cidade = string.Empty,
                Estado = string.Empty,
                Pais = "Brasil"
            };

            alunoData.Senha = string.Empty;
            Aluno = alunoData;
            ConfirmarSenha = string.Empty;

            OnPropertyChanged(nameof(DataNascimentoDateTime));
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Tempo esgotado",
                "O carregamento demorou demais. Verifique a conexão.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Erro ao carregar aluno: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SaveAlunoAsync()
    {
        if (IsBusy)
            return;

        var erros = ValidarAluno();

        if (erros.Count > 0)
        {
            await Shell.Current.DisplayAlertAsync(
                "Verifique os dados",
                string.Join(Environment.NewLine, erros),
                "OK");
            return;
        }

        try
        {
            IsBusy = true;

            using var cts =
                new CancellationTokenSource(TimeSpan.FromSeconds(5));

            if (Aluno.Endereco != null &&
                Aluno.Endereco.Id <= 0 &&
                !string.IsNullOrWhiteSpace(Aluno.Endereco.Cep))
            {
                var cep = ApenasDigitos(Aluno.Endereco.Cep);

                var logradouro = await _logradouroService.ObterPorCepAsync(
                    cep, cts.Token);

                if (logradouro is null)
                {
                    await Shell.Current.DisplayAlertAsync(
                        "Endereço",
                        "O CEP precisa estar cadastrado em Logradouros.",
                        "OK");
                    return;
                }

                Aluno.Endereco = logradouro;
            }

            Aluno.Nome = Aluno.Nome.Trim();
            Aluno.Cpf = ApenasDigitos(Aluno.Cpf);
            Aluno.Telefone = ApenasDigitos(Aluno.Telefone);
            Aluno.Numero = Aluno.Numero.Trim();

            Aluno.Email = string.IsNullOrWhiteSpace(Aluno.Email)
                ? null
                : Aluno.Email.Trim();

            Aluno.Complemento = string.IsNullOrWhiteSpace(Aluno.Complemento)
                ? null
                : Aluno.Complemento.Trim();

            Aluno.Senha = string.IsNullOrWhiteSpace(Aluno.Senha)
                ? null
                : Aluno.Senha.Trim();

            if (IsEditMode)
            {
                await _alunoService.AtualizarAsync(Aluno, cts.Token);

                await Shell.Current.DisplayAlertAsync(
                    "Sucesso", "Aluno atualizado!", "OK");
            }
            else
            {
                await _alunoService.AdicionarAsync(Aluno, cts.Token);

                await Shell.Current.DisplayAlertAsync(
                    "Sucesso", "Aluno cadastrado!", "OK");
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (ArgumentException ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Dados inválidos", ex.Message, "OK");
        }
        catch (InvalidOperationException ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Não foi possível salvar", ex.Message, "OK");
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync(
                "Tempo esgotado",
                "A operação demorou demais. Verifique a conexão.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Erro",
                $"Não foi possível salvar o aluno: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private List<string> ValidarAluno()
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(Aluno.Nome))
            erros.Add("O nome é obrigatório.");

        var cpf = ApenasDigitos(Aluno.Cpf);

        if (cpf.Length != 11)
        {
            erros.Add("O CPF deve conter 11 dígitos.");
        }
        else
        {
            var resultadoCpf =
                AcademiaDoZe.Domain.ValueObjects.Cpf.Criar(cpf);

            if (resultadoCpf.IsFailure)
                erros.Add("O CPF informado é inválido. Confira os números.");
        }

        if (ApenasDigitos(Aluno.Telefone).Length != 11)
            erros.Add("O telefone deve conter 11 dígitos, incluindo o DDD.");

        if (Aluno.DataNascimento >=
            DateOnly.FromDateTime(DateTime.Today.AddYears(-12)))
        {
            erros.Add("O aluno deve ter pelo menos 12 anos.");
        }

        if (!IsEditMode && string.IsNullOrWhiteSpace(Aluno.Senha))
            erros.Add("A senha é obrigatória no cadastro.");

        if (!string.IsNullOrWhiteSpace(Aluno.Senha))
        {
            if (Aluno.Senha.Length < 8)
                erros.Add("A senha deve ter pelo menos 8 caracteres.");

            if (!Aluno.Senha.Any(char.IsUpper))
                erros.Add("A senha deve conter uma letra maiúscula.");

            if (!Aluno.Senha.Any(char.IsLower))
                erros.Add("A senha deve conter uma letra minúscula.");

            if (!Aluno.Senha.Any(char.IsDigit))
                erros.Add("A senha deve conter um número.");

            if (Aluno.Senha != ConfirmarSenha)
                erros.Add("As senhas não coincidem.");
        }

        if (Aluno.Endereco is null ||
            (Aluno.Endereco.Id <= 0 &&
             string.IsNullOrWhiteSpace(Aluno.Endereco.Cep)))
        {
            erros.Add("Informe um endereço com CEP cadastrado.");
        }

        return erros;
    }

    private static string ApenasDigitos(string? valor) =>
        new((valor ?? string.Empty).Where(char.IsDigit).ToArray());
}
