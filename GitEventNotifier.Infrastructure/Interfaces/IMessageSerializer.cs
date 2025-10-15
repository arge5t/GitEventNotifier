namespace GitEventNotifier.Infrastructure.Interfaces;

public interface IMessageSerializer
{
    byte[] Serialize<T>(T message);
    T Deserialize<T>(byte[] bytes);
}