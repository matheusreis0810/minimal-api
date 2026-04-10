using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using MinimalApi.DTOs;

namespace Test.Mocks;

public class VeiculoServicoMock : IVeiculoServico
{
    private static List<Veiculo> Veiculos = new List<Veiculo>(){
        new Veiculo{
            Id = 1,
            Nome = "RS7",
            Marca = "audi",
            Ano = 2024
        },
        new Veiculo{
            Id = 2,
            Nome = "x8",
            Marca = "bmw",
            Ano = 2026
        }
    };

    public void Apagar(Veiculo veiculo)
    {
        var v = BuscaPorId(veiculo.Id);
        if (v != null) Veiculos.Remove(v);
    }

    public void Atualizar(Veiculo veiculo)
    {
        var index = Veiculos.FindIndex(v => v.Id == veiculo.Id);
        if (index != -1) Veiculos[index] = veiculo;
    }

    public Veiculo? BuscaPorId(int id)
    {
        return Veiculos.FirstOrDefault(v => v.Id == id);
    }

    public void Incluir(Veiculo veiculo)
    {
        veiculo.Id = Veiculos.Any() ? Veiculos.Max(v => v.Id) + 1 : 1;
        Veiculos.Add(veiculo);
    }
    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        return Veiculos;
    }
}