
// Nicolas Vaz

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

[QueryProperty(nameof(ColaboradorId), "Id")]
public partial class ColaboradorViewModel : BaseViewModel
{
    private readonly IColaboradorService _colaboradorService;
    private readonly ILogradouroService _logradouroService;

    public IEnumerable<AppColaboradorTipo> ColaboradorTipos { get; } =
        Enum.GetValues<AppColaboradorTipo>();

    public IEnumerable<AppColaboradorVinculo> ColaboradorVinculos { get; } =
        Enum.GetValues<AppColaboradorVinculo>();

    private ColaboradorDto _colaborador = CriarColaboradorPadrao();

    public ColaboradorDto Colaborador
    {
        get => _colaborador;
        set => SetProperty(ref _colaborador, value);
    }

    private int _colaboradorId;

    public int ColaboradorId
    {
        get => _colaboradorId;
        set => SetProperty(ref _colaboradorId, value);
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

    // Propriedades utilizadas pelos DatePickers da tela.
    public DateTime DataNascimentoDateTime
    {
        get => Colaborador.DataNascimento.ToDateTime(TimeOnly.MinValue);
        set
        {
            Colaborador.DataNascimento = DateOnly.FromDateTime(value);
            OnPropertyChanged();
        }
    }

    public DateTime DataAdmissaoDateTime
    {
        get => Colaborador.DataAdmissao.ToDateTime(TimeOnly.MinValue);
        set
        {
            Colaborador.DataAdmissao = DateOnly.FromDateTime(value);
            OnPropertyChanged();
        }
    }

    public bool HasEnderecoVinculado =>
        Colaborador.Endereco != null && Colaborador.Endereco.Id > 0;

    public bool HasNoEnderecoVinculado => !HasEnderecoVinculado;

    public ColaboradorViewModel(
        IColaboradorService colaboradorService,
        ILogradouroService logradouroService)
    {
        _colaboradorService = colaboradorService;
        _logradouroService = logradouroService;

        Title = "Detalhes do Colaborador";
    }

    public async Task InitializeAsync()
    {
        ConfirmarSenha = string.Empty;

        if (ColaboradorId > 0)
        {
            IsEditMode = true;
            Title = "Editar Colaborador";
            await LoadColaboradorAsync();
        }
        else
        {
            IsEditMode = false;
            Title = "Novo Colaborador";
            Colaborador = CriarColaboradorPadrao();
            NotificarStatusEndereco();
            OnPropertyChanged(nameof(DataNascimentoDateTime));
            OnPropertyChanged(nameof(DataAdmissaoDateTime));
        }
    }

    private void NotificarStatusEndereco()
    {
        OnPropertyChanged(nameof(HasEnderecoVinculado));
        OnPropertyChanged(nameof(HasNoEnderecoVinculado));
    }

    private static ColaboradorDto CriarColaboradorPadrao() => new()
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
        Complemento = string.Empty,
        DataAdmissao = DateOnly.FromDateTime(DateTime.Today),
        Tipo = AppColaboradorTipo.Atendente,
        Vinculo = AppColaboradorVinculo.CLT
    };

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task LoadColaboradorAsync()
    {
        if (ColaboradorId <= 0)
            return;

        try
        {
            IsBusy = true;

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(5));

            var colaboradorData =
                await _colaboradorService.ObterPorIdAsync(
                    ColaboradorId, cts.Token);

            if (colaboradorData is null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Aviso", "Colaborador não encontrado.", "OK");
                return;
            }

            colaboradorData.Endereco ??= new LogradouroDto
            {
                Cep = string.Empty,
                Nome = string.Empty,
                Bairro = string.Empty,
                Cidade = string.Empty,
                Estado = string.Empty,
                Pais = "Brasil"
            };

            // A senha não deve ser carregada para a tela.
            colaboradorData.Senha = string.Empty;
            Colaborador = colaboradorData;
            ConfirmarSenha = string.Empty;

            OnPropertyChanged(nameof(DataNascimentoDateTime));
            OnPropertyChanged(nameof(DataAdmissaoDateTime));
            NotificarStatusEndereco();
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
                $"Erro ao carregar colaborador: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SaveColaboradorAsync()
    {
        if (IsBusy)
            return;

        var erros = ValidarColaborador();

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

            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(5));

            // Normaliza os campos antes de enviar ao serviço.
            Colaborador.Nome = Colaborador.Nome.Trim();
            Colaborador.Cpf = ApenasDigitos(Colaborador.Cpf);
            Colaborador.Telefone = ApenasDigitos(Colaborador.Telefone);
            Colaborador.Numero = Colaborador.Numero.Trim();

            Colaborador.Email = string.IsNullOrWhiteSpace(Colaborador.Email)
                ? null
                : Colaborador.Email.Trim();

            Colaborador.Complemento =
                string.IsNullOrWhiteSpace(Colaborador.Complemento)
                    ? null
                    : Colaborador.Complemento.Trim();

            Colaborador.Senha =
                string.IsNullOrWhiteSpace(Colaborador.Senha)
                    ? null
                    : Colaborador.Senha.Trim();

            if (Colaborador.Endereco != null &&
                Colaborador.Endereco.Id <= 0 &&
                !string.IsNullOrWhiteSpace(Colaborador.Endereco.Cep))
            {
                var cep = ApenasDigitos(Colaborador.Endereco.Cep);

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

                Colaborador.Endereco = logradouro;
            }


            if (IsEditMode)
            {
                await _colaboradorService.AtualizarAsync(Colaborador, cts.Token);

                await Shell.Current.DisplayAlertAsync(
                    "Sucesso",
                    "Colaborador atualizado!",
                    "OK");
            }
            else
            {
                await _colaboradorService.AdicionarAsync(Colaborador, cts.Token);

                await Shell.Current.DisplayAlertAsync(
                    "Sucesso",
                    "Colaborador cadastrado!",
                    "OK");
            }


            await Shell.Current.GoToAsync("..");
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
                $"Não foi possível salvar o colaborador: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private List<string> ValidarColaborador()
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(Colaborador.Nome))
            erros.Add("O nome é obrigatório.");

        var cpf = ApenasDigitos(Colaborador.Cpf);

        if (cpf.Length != 11)
        {
            erros.Add("O CPF deve conter 11 dígitos.");
        }
        else
        {
            var resultadoCpf = AcademiaDoZe.Domain.ValueObjects.Cpf.Criar(cpf);

            if (resultadoCpf.IsFailure)
            {
                erros.Add(
                    "O CPF informado é inválido. Confira os números digitados.");
            }
        }

        if (ApenasDigitos(Colaborador.Telefone).Length != 11)
            erros.Add("O telefone deve conter 11 dígitos, incluindo o DDD.");

        if (Colaborador.DataNascimento >= DateOnly.FromDateTime(
                DateTime.Today.AddYears(-12)))
            erros.Add("O colaborador deve ter pelo menos 12 anos.");

        if (!IsEditMode && string.IsNullOrWhiteSpace(Colaborador.Senha))
            erros.Add("A senha é obrigatória no cadastro.");

        if (!string.IsNullOrWhiteSpace(Colaborador.Senha))
        {
            if (Colaborador.Senha.Length < 8)
                erros.Add("A senha deve ter pelo menos 8 caracteres.");

            if (!Colaborador.Senha.Any(char.IsUpper))
                erros.Add("A senha deve conter uma letra maiúscula.");

            if (!Colaborador.Senha.Any(char.IsLower))
                erros.Add("A senha deve conter uma letra minúscula.");

            if (!Colaborador.Senha.Any(char.IsDigit))
                erros.Add("A senha deve conter um número.");
        }

        if (!string.IsNullOrWhiteSpace(Colaborador.Senha) &&
            Colaborador.Senha != ConfirmarSenha)
        {
            erros.Add("As senhas não coincidem.");
        }

        if (Colaborador.Endereco is null ||
            Colaborador.Endereco.Id <= 0 &&
            string.IsNullOrWhiteSpace(Colaborador.Endereco.Cep))
        {
            erros.Add("Informe um endereço com CEP cadastrado.");
        }

        return erros;
    }

    private static string ApenasDigitos(string? valor) =>
        new((valor ?? string.Empty).Where(char.IsDigit).ToArray());
}
