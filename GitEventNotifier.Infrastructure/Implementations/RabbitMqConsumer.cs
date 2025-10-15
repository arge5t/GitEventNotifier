using GitEventNotifier.Infrastructure.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GitEventNotifier.Infrastructure.Implementations;

public class RabbitMqConsumer : IRabbitMqConsumer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IMessageSerializer _serializer;

    public RabbitMqConsumer(string hostName, IMessageSerializer serializer)
    {
        _serializer = serializer;
        var factory = new ConnectionFactory() { HostName = hostName };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }
    
    public async Task StartConsumingAsync<T>(
        string queueName, 
        Func<T, Task> onMessageReceived, 
        CancellationToken token = default)
    {
        _channel.QueueDeclare(
            queue: queueName, 
            durable: false,
            exclusive: false, 
            autoDelete: false,
            arguments: null);
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (sender, eventArgs) =>
        {
            try
            {
                var message = _serializer.Deserialize<T>(eventArgs.Body.ToArray());
                await onMessageReceived(message);
                _channel.BasicAck(eventArgs.DeliveryTag, false);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                _channel.BasicNack(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false);
            }
        };
        
        _channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer);
        
        await Task.Delay(Timeout.Infinite, token);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection.Dispose();
    }
}