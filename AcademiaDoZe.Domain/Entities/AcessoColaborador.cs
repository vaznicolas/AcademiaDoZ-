
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.Entities;

public class AcessoColaborador : Entity, IAggregateRoot
{
    public Colaborador Colaborador { get; private set; }

    public DateTime DataHoraEntrada { get; private set; }

    public DateTime? DataHoraSaida { get; private set; }

    private AcessoColaborador(
        int id,
        Colaborador colaborador,
        DateTime dataHoraEntrada,
        DateTime? dataHoraSaida)
        : base(id)
    {
        Colaborador = colaborador;
        DataHoraEntrada = dataHoraEntrada;
        DataHoraSaida = dataHoraSaida;
    }

    // Calcula a duração deste acesso.
    public TimeSpan? TempoPermanencia =>
        DataHoraSaida.HasValue
            ? DataHoraSaida.Value - DataHoraEntrada
            : null;

    // Registra a saída do colaborador.
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

    public static AcessoColaborador Criar(
        int id,
        Colaborador colaborador,
        DateTime dataHoraEntrada,
        DateTime? dataHoraSaida)
    {
        return new AcessoColaborador(
            id,
            colaborador,
            dataHoraEntrada,
            dataHoraSaida);
    }
}
