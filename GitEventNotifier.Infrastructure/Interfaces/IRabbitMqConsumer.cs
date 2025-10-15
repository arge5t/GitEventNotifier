namespace GitEventNotifier.Infrastructure.Interfaces;

public interface IRabbitMqConsumer
{
    Task StartConsumingAsync<T>(
        string queueName,
        Func<T,Task> onMessageReceived,
        CancellationToken token = default);
}