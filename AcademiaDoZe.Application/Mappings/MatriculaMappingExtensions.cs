
// Nicolas Vaz

using System.Text;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

public static class MatriculaMappingExtensions
{
    private const string LaudoNaoInformado = "laudo_nao_informado";

    public static MatriculaDto ToDto(
        this Matricula matricula,
        AlunoDto alunoDto)
    {
        ArgumentNullException.ThrowIfNull(matricula);
        ArgumentNullException.ThrowIfNull(alunoDto);

        return new MatriculaDto
        {
            Id = matricula.Id,
            AlunoMatricula = alunoDto,
            Plano = matricula.Plano.ToApplication(),
            DataInicio = matricula.DataInicio,
            DataFim = matricula.DataFinal,
            Objetivo = matricula.Objetivo,
            RestricoesMedicas = matricula.Restricoes.ToApplication(),
            ObservacoesRestricoes = matricula.ObservacoesRestricoes,

            LaudoMedico =
                matricula.LaudoMedico != null &&
                matricula.LaudoMedico.Valor != LaudoNaoInformado
                    ? new ArquivoDto
                    {
                        Conteudo = ObterBytesDoLaudo(
                            matricula.LaudoMedico.Valor)
                    }
                    : null
        };
    }

    public static Matricula ToEntity(
        this MatriculaDto matriculaDto,
        Aluno aluno)
    {
        ArgumentNullException.ThrowIfNull(matriculaDto);
        ArgumentNullException.ThrowIfNull(aluno);

        Arquivo laudo = CriarLaudo(matriculaDto.LaudoMedico);

        return Matricula.Criar(
            matriculaDto.Id,
            aluno,
            matriculaDto.Plano.ToDomain(),
            matriculaDto.DataInicio,
            matriculaDto.DataFim,
            matriculaDto.Objetivo,
            matriculaDto.RestricoesMedicas.ToDomain(),
            matriculaDto.ObservacoesRestricoes ?? string.Empty,
            laudo);
    }

    public static Matricula UpdateFromDto(
        this Matricula matricula,
        MatriculaDto matriculaDto,
        Aluno aluno)
    {
        ArgumentNullException.ThrowIfNull(matricula);
        ArgumentNullException.ThrowIfNull(matriculaDto);
        ArgumentNullException.ThrowIfNull(aluno);

        // Mantém o laudo atual caso nenhuma alteração seja enviada.
        Arquivo laudo = matricula.LaudoMedico;

        if (matriculaDto.LaudoMedico != null)
        {
            // Conteúdo vazio representa a remoção do laudo.
            laudo = CriarLaudo(matriculaDto.LaudoMedico);
        }

        return Matricula.Criar(
            matricula.Id,
            aluno,
            matriculaDto.Plano != default
                ? matriculaDto.Plano.ToDomain()
                : matricula.Plano,
            matriculaDto.DataInicio != default
                ? matriculaDto.DataInicio
                : matricula.DataInicio,
            matriculaDto.DataFim != default
                ? matriculaDto.DataFim
                : matricula.DataFinal,
            matriculaDto.Objetivo ?? matricula.Objetivo,
            matriculaDto.RestricoesMedicas != default
                ? matriculaDto.RestricoesMedicas.ToDomain()
                : matricula.Restricoes,
            matriculaDto.ObservacoesRestricoes
                ?? matricula.ObservacoesRestricoes,
            laudo);
    }

    private static Arquivo CriarLaudo(ArquivoDto? laudoDto)
    {
        string conteudo = laudoDto?.Conteudo is { Length: > 0 }
            ? Convert.ToBase64String(laudoDto.Conteudo)
            : LaudoNaoInformado;

        var resultado = Arquivo.Criar(conteudo);

        if (resultado.IsFailure)
        {
            throw new InvalidOperationException(
                "Não foi possível converter o laudo médico.");
        }

        return resultado.Value!;
    }

    private static byte[] ObterBytesDoLaudo(string valor)
    {
        try
        {
            return Convert.FromBase64String(valor);
        }
        catch (FormatException)
        {
            // Compatibilidade com laudos gravados no formato antigo.
            return Encoding.UTF8.GetBytes(valor);
        }
    }
}
