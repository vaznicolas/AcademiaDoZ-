
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.Entities;

public class AcessoAluno : Entity, IAggregateRoot
{
    public Aluno Aluno { get; private set; }

    public DateTime DataHoraEntrada { get; private set; }

    public DateTime? DataHoraSaida { get; private set; }

    private AcessoAluno(
        int id,
        Aluno aluno,
        DateTime dataHoraEntrada,
        DateTime? dataHoraSaida)
        : base(id)
    {
        Aluno = aluno;
        DataHoraEntrada = dataHoraEntrada;
        DataHoraSaida = dataHoraSaida;
    }

    // Calcula quanto tempo o aluno permaneceu na academia.
    public TimeSpan? TempoPermanencia =>
        DataHoraSaida.HasValue
            ? DataHoraSaida.Value - DataHoraEntrada
            : null;

    // Registra a saída do aluno.
    public void RegistrarSaida(DateTime horario)
    {
        if (DataHoraSaida.HasValue)
        {
            throw new InvalidOperationException(
                "A saída já foi registrada.");
        }

        if (horario < DataHoraEntrada)
        {
            throw new ArgumentException(
                "A saída não pode ser anterior à entrada.",
                nameof(horario));
        }

        DataHoraSaida = horario;
    }

    public static AcessoAluno Criar(
        int id,
        Aluno aluno,
        DateTime dataHoraEntrada,
        DateTime? dataHoraSaida)
    {
        return new AcessoAluno(
            id,
            aluno,
            dataHoraEntrada,
            dataHoraSaida);
    }
}
