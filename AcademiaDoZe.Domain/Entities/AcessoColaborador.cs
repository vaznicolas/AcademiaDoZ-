// Nicolas Vaz

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