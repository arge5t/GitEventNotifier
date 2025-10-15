namespace GitEventNotifier.Infrastructure.Interfaces;

public interface IRabbitMqProducer
{
    Task PublishAsync<T>(string queueName,T message, CancellationToken token = default);
}