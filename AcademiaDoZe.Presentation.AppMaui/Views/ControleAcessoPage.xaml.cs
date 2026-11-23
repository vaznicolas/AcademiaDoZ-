
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ControleAcessoPage : ContentPage
{
    private readonly IAcessoAlunoService _acessoAlunoService;
    private readonly IAcessoColaboradorService _acessoColaboradorService;

    private readonly IAlunoRepository _alunoRepository;
    private readonly IColaboradorRepository _colaboradorRepository;

    private readonly IAcessoAlunoRepository _acessoAlunoRepository;
    private readonly IAcessoColaboradorRepository _acessoColaboradorRepository;

    private bool _carregandoHistorico;
    private bool _registrandoAcesso;

    public ControleAcessoPage(
        IAcessoAlunoService acessoAlunoService,
        IAcessoColaboradorService acessoColaboradorService,
        Func<IAlunoRepository> alunoRepositoryFactory,
        Func<IColaboradorRepository> colaboradorRepositoryFactory,
        Func<IAcessoAlunoRepository> acessoAlunoRepositoryFactory,
        Func<IAcessoColaboradorRepository> acessoColaboradorRepositoryFactory)
    {
        InitializeComponent();

        _acessoAlunoService = acessoAlunoService;
        _acessoColaboradorService = acessoColaboradorService;

        _alunoRepository = alunoRepositoryFactory();
        _colaboradorRepository = colaboradorRepositoryFactory();

        _acessoAlunoRepository = acessoAlunoRepositoryFactory();

        _acessoColaboradorRepository =
            acessoColaboradorRepositoryFactory();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CarregarAcessosHoje();
    }

    private async void TipoPessoaPicker_SelectedIndexChanged(
        object sender,
        EventArgs e)
    {
        PessoaPicker.ItemsSource = null;
        PessoaPicker.SelectedItem = null;
        MensagemLabel.Text = string.Empty;

        try
        {
            if (TipoPessoaPicker.SelectedIndex == 0)
            {
                var alunos = await _alunoRepository.ObterTodos();

                PessoaPicker.ItemsSource = alunos.ToList();
            }
            else if (TipoPessoaPicker.SelectedIndex == 1)
            {
                var colaboradores =
                    await _colaboradorRepository.ObterTodos();

                PessoaPicker.ItemsSource = colaboradores.ToList();
            }
        }
        catch (Exception ex)
        {
            MensagemLabel.Text =
                $"Erro ao carregar pessoas: {ex.Message}";

            MensagemLabel.TextColor = Colors.Red;
        }
    }

    private async void RegistrarEntrada_Clicked(
        object sender,
        EventArgs e)
    {
        await RegistrarAcesso(true);
    }

    private async void RegistrarSaida_Clicked(
        object sender,
        EventArgs e)
    {
        await RegistrarAcesso(false);
    }

    private async Task RegistrarAcesso(bool entrada)
    {
        if (_registrandoAcesso)
            return;

        if (PessoaPicker.SelectedItem == null)
        {
            MensagemLabel.Text = "Selecione uma pessoa.";
            MensagemLabel.TextColor = Colors.Red;
            return;
        }

        _registrandoAcesso = true;

        try
        {
            (bool sucesso, string mensagem) resultado;

            if (TipoPessoaPicker.SelectedIndex == 0 &&
                PessoaPicker.SelectedItem is Aluno aluno)
            {
                resultado = entrada
                    ? await _acessoAlunoService.RegistrarEntrada(
                        aluno.Id)
                    : await _acessoAlunoService.RegistrarSaida(
                        aluno.Id);
            }
            else if (TipoPessoaPicker.SelectedIndex == 1 &&
                     PessoaPicker.SelectedItem is Colaborador colaborador)
            {
                resultado = entrada
                    ? await _acessoColaboradorService.RegistrarEntrada(
                        colaborador.Id)
                    : await _acessoColaboradorService.RegistrarSaida(
                        colaborador.Id);
            }
            else
            {
                MensagemLabel.Text = "Seleção inválida.";
                MensagemLabel.TextColor = Colors.Red;
                return;
            }

            MensagemLabel.Text = resultado.mensagem;

            MensagemLabel.TextColor = resultado.sucesso
                ? Colors.Green
                : Colors.Red;

            if (resultado.sucesso)
            {
                await CarregarAcessosHoje();
            }
        }
        catch (Exception ex)
        {
            MensagemLabel.Text = $"Erro: {ex.Message}";
            MensagemLabel.TextColor = Colors.Red;
        }
        finally
        {
            _registrandoAcesso = false;
        }
    }

    private async void AtualizarAcessos_Clicked(
        object sender,
        EventArgs e)
    {
        await CarregarAcessosHoje();
    }

    private async Task CarregarAcessosHoje()
    {
        if (_carregandoHistorico)
            return;

        _carregandoHistorico = true;

        CarregandoAcessos.IsVisible = true;
        CarregandoAcessos.IsRunning = true;

        HistoricoMensagemLabel.Text = "Carregando acessos...";
        HistoricoMensagemLabel.TextColor = Colors.Gray;

        try
        {
            var inicio = DateTime.Today;
            var fim = inicio.AddDays(1);

            var acessosAlunos =
                await _acessoAlunoRepository.ObterPorPeriodo(
                    inicio, fim);

            var acessosColaboradores =
                await _acessoColaboradorRepository.ObterPorPeriodo(
                    inicio, fim);

            var registros = new List<RegistroAcessoHoje>();

            foreach (var acesso in acessosAlunos)
            {
                if (acesso.DataHoraEntrada >= fim)
                    continue;

                registros.Add(new RegistroAcessoHoje
                {
                    Nome = acesso.Aluno.Nome,
                    Tipo = "Aluno",
                    Entrada = acesso.DataHoraEntrada,
                    Saida = acesso.DataHoraSaida
                });
            }

            foreach (var acesso in acessosColaboradores)
            {
                if (acesso.DataHoraEntrada >= fim)
                    continue;

                registros.Add(new RegistroAcessoHoje
                {
                    Nome = acesso.Colaborador.Nome,
                    Tipo = "Colaborador",
                    Entrada = acesso.DataHoraEntrada,
                    Saida = acesso.DataHoraSaida
                });
            }

            registros = registros
                .OrderByDescending(x => x.Entrada)
                .ToList();

            ListaAcessosLayout.Children.Clear();

            if (registros.Count == 0)
            {
                HistoricoMensagemLabel.Text =
                    "Nenhum acesso registrado hoje.";

                return;
            }

            HistoricoMensagemLabel.Text =
                $"{registros.Count} acesso(s) registrado(s) hoje.";

            foreach (var registro in registros)
            {
                ListaAcessosLayout.Children.Add(
                    CriarItemHistorico(registro));
            }
        }
        catch (Exception ex)
        {
            HistoricoMensagemLabel.Text =
                $"Erro ao carregar histórico: {ex.Message}";

            HistoricoMensagemLabel.TextColor = Colors.Red;
        }
        finally
        {
            CarregandoAcessos.IsRunning = false;
            CarregandoAcessos.IsVisible = false;

            _carregandoHistorico = false;
        }
    }


    private static View CriarItemHistorico(
        RegistroAcessoHoje registro)
    {
        bool possuiSaida = registro.Saida.HasValue;

        Color corSituacao = possuiSaida
            ? Color.FromArgb("#2E7D32")
            : Color.FromArgb("#C77700");

        string textoSituacao = possuiSaida
            ? "SAÍDA REGISTRADA"
            : "AINDA NA ACADEMIA";

        // NOME DA PESSOA

        var nomeLabel = new Label
        {
            Text = registro.Nome,
            FontFamily = "Oswald",
            FontSize = 19,
            TextColor = Color.FromArgb("#0B2A6F"),
            LineBreakMode = LineBreakMode.WordWrap
        };

        // TIPO DE PESSOA

        var tipoLabel = new Label
        {
            Text = registro.Tipo.ToUpperInvariant(),
            FontFamily = "Roboto",
            FontSize = 12,
            TextColor = Color.FromArgb("#5A6478")
        };

        // SITUACAO

        var situacaoLabel = new Label
        {
            Text = textoSituacao,
            FontFamily = "Oswald",
            FontSize = 13,
            TextColor = corSituacao,
            HorizontalTextAlignment = TextAlignment.End
        };

        // HORARIO DE ENTRADA

        var entradaLabel = new Label
        {
            Text = $"ENTRADA: {registro.Entrada:HH:mm:ss}",
            FontFamily = "Roboto",
            FontSize = 14,
            TextColor = Color.FromArgb("#071A47")
        };

        // HORARIO DE SAIDA E PERMANENCIA

        string textoSaida;

        if (registro.Saida.HasValue)
        {
            var permanencia =
                registro.Saida.Value - registro.Entrada;

            textoSaida =
                $"SAÍDA: {registro.Saida.Value:HH:mm:ss}  |  " +
                $"PERMANÊNCIA: {(int)permanencia.TotalHours}h " +
                $"{permanencia.Minutes:D2}min";
        }
        else
        {
            textoSaida = "SAÍDA: AINDA NÃO REGISTRADA";
        }

        var saidaLabel = new Label
        {
            Text = textoSaida,
            FontFamily = "Roboto",
            FontSize = 13,
            TextColor = corSituacao,
            LineBreakMode = LineBreakMode.WordWrap
        };

        // CABECALHO DO CARTAO

        var cabecalho = new Grid
        {
            ColumnDefinitions =
        {
            new ColumnDefinition
            {
                Width = GridLength.Star
            },
            new ColumnDefinition
            {
                Width = GridLength.Auto
            }
        },
            ColumnSpacing = 12
        };

        var identificacao = new VerticalStackLayout
        {
            Spacing = 3,
            Children =
        {
            nomeLabel,
            tipoLabel
        }
        };

        cabecalho.Add(identificacao);
        cabecalho.Add(situacaoLabel);

        Grid.SetColumn(situacaoLabel, 1);

        // LINHA DIVISORIA

        var divisor = new BoxView
        {
            HeightRequest = 1,
            Color = Color.FromArgb("#D8DCE5")
        };

        // CONTEUDO DO CARTAO

        var conteudo = new VerticalStackLayout
        {
            Spacing = 10,
            Children =
        {
            cabecalho,
            divisor,
            entradaLabel,
            saidaLabel
        }
        };

        // CARTAO FINAL

        return new Border
        {
            Padding = new Thickness(16),
            BackgroundColor = Colors.White,
            Stroke = Color.FromArgb("#0B2A6F"),
            StrokeThickness = 1,
            StrokeShape =
                new Microsoft.Maui.Controls.Shapes.RoundRectangle
                {
                    CornerRadius = new CornerRadius(8)
                },
            Content = conteudo
        };
    }


    private sealed class RegistroAcessoHoje
    {
        public string Nome { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public DateTime Entrada { get; set; }

        public DateTime? Saida { get; set; }
    }
}
