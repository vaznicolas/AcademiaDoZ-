// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Tests.Entities;

public class EntityTests
{
    private class EntityTeste : Entity
    {
        public EntityTeste(int id)
            : base(id)
        {
        }
    }

    [Fact]
    public void Deve_Criar_Entity_Com_Id_Valido()
    {
        var entity = new EntityTeste(10);

        Assert.Equal(10, entity.Id);
    }

    [Fact]
    public void Deve_Permitir_Id_Igual_A_Zero()
    {
        var entity = new EntityTeste(0);

        Assert.Equal(0, entity.Id);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_Id_For_Negativo()
    {
        var excecao = Assert.Throws<Exception>(
            () => new EntityTeste(-1));

        Assert.Equal("ID_NEGATIVO", excecao.Message);
    }
}