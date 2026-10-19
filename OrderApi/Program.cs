using OrderApi.Dtos;
using OrderApi.Services;
using Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<RabbitMqProducer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/pedidos", async (CriarPedidoRequest request, RabbitMqProducer producer) =>
{
    var pedindoEvento = new PedidoCriadoEvent(
        PedidoId: Guid.NewGuid(),
        ClienteEmail: request.ClienteEmail,
        ClienteNome:request.ClienteNome,
        ValorTotal:request.ValorTotal,
        DataCriacao:DateTime.UtcNow
        );

    await producer.PublicarPedidoAsync(pedindoEvento);
    
    return Results.Accepted($"/pedidos/{pedindoEvento.PedidoId}", new
    {
        Status =  "Processamento Iniciado",
        Pedido = pedindoEvento
    });


});

app.Run();





