
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


    public ControleAcessoPage(
        IAcessoAlunoService acessoAlunoService,
        IAcessoColaboradorService acessoColaboradorService,
        Func<IAlunoRepository> alunoRepositoryFactory,
        Func<IColaboradorRepository> colaboradorRepositoryFactory)
    {
        InitializeComponent();

        _acessoAlunoService = acessoAlunoService;
        _acessoColaboradorService = acessoColaboradorService;

        _alunoRepository = alunoRepositoryFactory();
        _colaboradorRepository = colaboradorRepositoryFactory();
    }


    private async void TipoPessoaPicker_SelectedIndexChanged(
        object sender, EventArgs e)
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
        }
    }

    private async void RegistrarEntrada_Clicked(
        object sender, EventArgs e)
    {
        await RegistrarAcesso(true);
    }

    private async void RegistrarSaida_Clicked(
        object sender, EventArgs e)
    {
        await RegistrarAcesso(false);
    }

    private async Task RegistrarAcesso(bool entrada)
    {
        if (PessoaPicker.SelectedItem == null)
        {
            MensagemLabel.Text = "Selecione uma pessoa.";
            return;
        }

        try
        {
            (bool sucesso, string mensagem) resultado;

            if (TipoPessoaPicker.SelectedIndex == 0 &&
                PessoaPicker.SelectedItem is Aluno aluno)
            {
                resultado = entrada
                    ? await _acessoAlunoService.RegistrarEntrada(aluno.Id)
                    : await _acessoAlunoService.RegistrarSaida(aluno.Id);
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
                return;
            }

            MensagemLabel.Text = resultado.mensagem;
            MensagemLabel.TextColor = resultado.sucesso
                ? Colors.Green
                : Colors.Red;
        }
        catch (Exception ex)
        {
            MensagemLabel.Text = $"Erro: {ex.Message}";
            MensagemLabel.TextColor = Colors.Red;
        }
    }
}
