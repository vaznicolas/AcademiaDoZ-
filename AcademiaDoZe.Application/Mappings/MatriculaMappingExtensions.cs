// Nicolas Vaz

using System.Text;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

public static class MatriculaMappingExtensions
{
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
            LaudoMedico = matricula.LaudoMedico != null
                ? new ArquivoDto
                {
                    Conteudo = Encoding.UTF8.GetBytes(
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

        Arquivo? laudo = null;

        if (matriculaDto.LaudoMedico?.Conteudo != null)
        {
            var laudoResult = Arquivo.Criar(
                Encoding.UTF8.GetString(
                    matriculaDto.LaudoMedico.Conteudo));

            if (laudoResult.IsSuccess)
                laudo = laudoResult.Value;
        }

        return Matricula.Criar(
            matriculaDto.Id,
            aluno,
            matriculaDto.Plano.ToDomain(),
            matriculaDto.DataInicio,
            matriculaDto.DataFim,
            matriculaDto.Objetivo,
            matriculaDto.RestricoesMedicas.ToDomain(),
            matriculaDto.ObservacoesRestricoes ?? string.Empty,
            laudo!);
    }

    public static Matricula UpdateFromDto(
        this Matricula matricula,
        MatriculaDto matriculaDto,
        Aluno aluno)
    {
        ArgumentNullException.ThrowIfNull(matricula);
        ArgumentNullException.ThrowIfNull(matriculaDto);
        ArgumentNullException.ThrowIfNull(aluno);

        Arquivo? laudo = matricula.LaudoMedico;

        if (matriculaDto.LaudoMedico?.Conteudo != null)
        {
            var laudoResult = Arquivo.Criar(
                Encoding.UTF8.GetString(
                    matriculaDto.LaudoMedico.Conteudo));

            if (laudoResult.IsSuccess)
                laudo = laudoResult.Value;
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
            laudo!);
    }
}