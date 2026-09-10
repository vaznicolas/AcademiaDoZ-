// Nicolas Vaz

using AcademiaDoZe.Domain.Exceptions;

namespace AcademiaDoZe.Tests.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void Deve_Criar_DomainException_Com_Mensagem()
    {
        var mensagem = "Ocorreu um erro de domínio.";

        var exception = new DomainException(mensagem);

        Assert.Equal(mensagem, exception.Message);
    }
}