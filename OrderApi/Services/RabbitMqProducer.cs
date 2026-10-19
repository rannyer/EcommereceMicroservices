using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Shared;

namespace OrderApi.Services;

public class RabbitMqProducer
{
    private readonly string _uri;

    public RabbitMqProducer(IConfiguration configuration)
    {
        _uri = configuration["RabbitMq:Uri"] 
               ?? throw new ArgumentNullException("RabbitMq:Uri nao configurada");
    }

    public async Task PublicarPedidoAsync(PedidoCriadoEvent evento)
    {
        var factory = new ConnectionFactory { Uri = new Uri(_uri) };
        using var connection = await factory.CreateConnectionAsync();
        using var chanel = await connection.CreateChannelAsync();

        await chanel.QueueDeclareAsync(
            queue: "pedidos-queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var payload = JsonSerializer.Serialize(evento);
        var body = Encoding.UTF8.GetBytes(payload);

        await chanel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "pedidos-queue",
            body: body
        );
        

    }
}