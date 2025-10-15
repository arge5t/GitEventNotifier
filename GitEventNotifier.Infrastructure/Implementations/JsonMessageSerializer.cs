using System.Text.Json;
using GitEventNotifier.Infrastructure.Interfaces;

namespace GitEventNotifier.Infrastructure.Implementations;

public class JsonMessageSerializer : IMessageSerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    
    public byte[] Serialize<T>(T message) => 
        JsonSerializer.SerializeToUtf8Bytes(message, _options);  
    
    public T Deserialize<T>(byte[] bytes) =>
        JsonSerializer.Deserialize<T>(bytes, _options) ?? throw new Exception("Deserialization failed");
}