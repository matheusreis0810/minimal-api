using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.DTOs;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class VeiculoRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    private async Task LogarComoAdmin()
    {
        var loginDTO = new LoginDTO { Email = "adm@teste.com", Senha = "123456" };
        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");
        
        var response = await Setup.client.PostAsync("/administradores/login", content);
        
        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro no Login do Teste: Status {response.StatusCode} - Mensagem: {erro}");
        }

        var result = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Setup.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", admLogado?.Token);
    }
    
    [TestMethod]
    public async Task TestarGetSetPropriedades()
    {
        await LogarComoAdmin();

        var veiculoDTO = new VeiculoDTO {
            Nome = "x8",
            Marca = "bmw",
            Ano = 2026
        };
        var content = new StringContent(JsonSerializer.Serialize(veiculoDTO), Encoding.UTF8, "application/json");

        var response = await Setup.client.PostAsync("/veiculos", content);

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("x8"));
    }
}
