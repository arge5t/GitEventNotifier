using GitEventNotifier.Infrastructure.Interfaces;
using RabbitMQ.Client;

namespace GitEventNotifier.Infrastructure.Implementations;

public class RabbitMqProducer : IRabbitMqProducer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IMessageSerializer _serializer;

    public RabbitMqProducer(
        string hostName, 
        IMessageSerializer serializer)
    {
        _serializer = serializer;
        var factory = new ConnectionFactory() {  HostName = hostName };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    } 
    
    public async Task PublishAsync<T>(string queueName, T message, CancellationToken token = default)
    {
        if (token.IsCancellationRequested) return;

        _channel.QueueDeclare(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        var body = _serializer.Serialize(message);
        _channel.BasicPublish(
            exchange: "", 
            routingKey: queueName, 
            basicProperties: null, 
            body:body);
    }
    
    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}