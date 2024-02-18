using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RinhaBackendApi;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(builder.Configuration.GetConnectionString("pgsql"));

builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("pgsql"));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<TransactionDb>();
builder.Services.AddSingleton<ExtratoDb>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var transactionDb = app.Services.GetRequiredService<TransactionDb>();
var extratoDb = app.Services.GetRequiredService<ExtratoDb>();

app.MapPost("/clients/{id}/transacoes", async Task<Results<Ok<TransacaoResp>, NotFound, BadRequest ,UnprocessableEntity>> (int id, [FromBody] TransacaoReq transacao) =>
{
    var tipoValido = transacao.Tipo == "d" || transacao.Tipo == "c";

    if (!tipoValido)
        return TypedResults.BadRequest();

    var result = await transactionDb.Add(transacao, id);

    if (result.Status == ResultStatus.Success)
        return TypedResults.Ok(result.Data);
    if (result.Status == ResultStatus.EntityNotFound)
        return TypedResults.NotFound();
    if (result.Status == ResultStatus.NotProcessed)
        return TypedResults.UnprocessableEntity();

    throw new InvalidOperationException();

})
.WithName("AddTransacao")
.WithOpenApi();

app.MapGet("/clients/{id}/extrato", async Task<Results<Ok<Extrato>, NotFound>> ([FromRoute]int id) =>
{
    var result = await extratoDb.Get(id);

    if (result.Status == ResultStatus.EntityNotFound)
        return TypedResults.NotFound();
    if (result.Status == ResultStatus.Success)
        return TypedResults.Ok(result.Data);

    throw new InvalidOperationException();
})
.WithName("GetExtrato")
.WithOpenApi();

app.Run();

