// Nicolas Vaz

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Logradouro : Entity, IAggregateRoot
{
    public Cep Cep { get; protected set; }
    public string Pais { get; protected set; }
    public string Estado { get; protected set; }
    public string Cidade { get; protected set; }
    public string Bairro { get; protected set; }
    public string Nome { get; protected set; }

    protected Logradouro(
        int id,
        Cep cep,
        string pais,
        string estado,
        string cidade,
        string bairro,
        string nome) : base(id)
    {
        Cep = cep;
        Pais = pais;
        Estado = estado;
        Cidade = cidade;
        Bairro = bairro;
        Nome = nome;
    }

    public static Result<Logradouro> Criar(
        int id,
        string cep,
        string nome,
        string bairro,
        string cidade,
        string estado,
        string pais)
    {
        var notifications = new List<Notification>();

        var cepResult = Cep.Criar(cep);

        if (cepResult.IsFailure)
            notifications.AddRange(cepResult.Notifications);

        if (NormalizacaoService.TextoVazioOuNulo(nome))
            notifications.Add(new Notification("Nome", "NOME_OBRIGATORIO"));
        else
            nome = NormalizacaoService.LimparEspacos(nome);

        if (NormalizacaoService.TextoVazioOuNulo(bairro))
            notifications.Add(new Notification("Bairro", "BAIRRO_OBRIGATORIO"));
        else
            bairro = NormalizacaoService.LimparEspacos(bairro);

        if (NormalizacaoService.TextoVazioOuNulo(cidade))
            notifications.Add(new Notification("Cidade", "CIDADE_OBRIGATORIO"));
        else
            cidade = NormalizacaoService.LimparEspacos(cidade);

        if (NormalizacaoService.TextoVazioOuNulo(estado))
            notifications.Add(new Notification("Estado", "ESTADO_OBRIGATORIO"));
        else
            estado = NormalizacaoService.LimparEspacos(estado);

        if (NormalizacaoService.TextoVazioOuNulo(pais))
            notifications.Add(new Notification("Pais", "PAIS_OBRIGATORIO"));
        else
            pais = NormalizacaoService.LimparEspacos(pais);

        if (notifications.Count != 0)
            return Result<Logradouro>.Failure(notifications);

        return Result<Logradouro>.Success(
            new Logradouro(
                id,
                cepResult.Value!,
                pais,
                estado,
                cidade,
                bairro,
                nome));
    }
}