using MinimalApi.Infraestrutura.Db;
using MinimalApi.DTOs;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;

#region builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options => {options.UseMySql(builder.Configuration.GetConnectionString("mysql"),
                                          ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
                                          });

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
//LOGIN DE ADM
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    if(administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso");
    else
        return Results.Unauthorized();
}).WithTags("Administradores");

//CRIAR ADMs
app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensagens.Add("Email não pode ser vazio");
    if (string.IsNullOrEmpty(administradorDTO.Senha))
        validacao.Mensagens.Add("Senha não pode ser vazia");
    if ((administradorDTO.Perfil == null))
        validacao.Mensagens.Add("Perfil não pode ser vazio");

    if(validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);
    
    var adm = new Administrador{
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil?.ToString() ?? Perfil.Editor.ToString()
    };
    administradorServico.Incluir(adm);
    
    return Results.Created($"/administrador/{adm.Id}", new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
}).WithTags("Administradores");

//LISTAR ADMs
app.MapGet("/administradores", ([FromQuery]int? pagina, IAdministradorServico administradorServico) =>
{   
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);
    foreach(var adm in administradores)
    {
        adms.Add(new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
    return Results.Ok(adms);
}).WithTags("Administradores");

//BUSCAR ADM POR 
app.MapGet("/administradores/{id}", ([FromRoute]int id, IAdministradorServico administradorServico) =>
{
    var adm = administradorServico.BuscaPorID(id);
    if(adm == null) return Results.NotFound();
    return Results.Ok(new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
}).WithTags("Administradores");
#endregion


#region Veiculos
ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensagens.Add("O nome nao pode ser vazio");

    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensagens.Add("A marca nao pode ficar em branco");

    if (veiculoDTO.Ano < 1950)
        validacao.Mensagens.Add("Veiculo muito antigo, aceito somente anos superiores a 1950");

    return validacao;
}
//INCLUI VEICULOS
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var veiculo = new Veiculo{
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);
    
    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

//LISTA OS VEICULOS POR PAGINA
app.MapGet("/veiculos", ([FromQuery]int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);
    
    return Results.Ok(veiculos);
}).WithTags("Veiculos");

//BUSCA O VEICULO POR ID
app.MapGet("/veiculos/{id}", ([FromRoute]int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();
    return Results.Ok(veiculo);
}).WithTags("Veiculos");

//ALTERA INFORMAÇOES DO VEICULO
app.MapPut("/veiculos/{id}", ([FromRoute]int id, VeiculoDTO veiculoDTO ,IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();

    var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    veiculo.Nome=veiculoDTO.Nome;
    veiculo.Marca=veiculoDTO.Marca;
    veiculo.Ano=veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).WithTags("Veiculos");

//DELETAR VEICULO
app.MapDelete("/veiculos/{id}", ([FromRoute]int id,IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
}).WithTags("Veiculos");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion

