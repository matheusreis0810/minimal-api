using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public class VeiculoTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        //Arrange - todas as variaveis q vai ser criadas
        var veiculo = new Veiculo();

        //Act - acoes q seram executadas
        veiculo.Id = 1;
        veiculo.Nome = "RS6";
        veiculo.Marca = "Audi";
        veiculo.Ano = 2020;

        //Assert - validacao dos dados
        Assert.AreEqual(1, veiculo.Id);
        Assert.AreEqual("RS6", veiculo.Nome);
        Assert.AreEqual("Audi", veiculo.Marca);
        Assert.AreEqual(2020, veiculo.Ano);
    }
}