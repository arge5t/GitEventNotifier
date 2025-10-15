using GitEventNotifier.Common;
using GitEventNotifier.Infrastructure.Interfaces;

namespace GitEventNotifier.Consumer;

public class GitEventHandlingService : BackgroundService
{
    private readonly IRabbitMqConsumer _consumer;
    private readonly ILogger<GitEventHandlingService> _logger;

    public GitEventHandlingService(IRabbitMqConsumer consumer, ILogger<GitEventHandlingService> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting GitEvent consumer...");

        await _consumer.StartConsumingAsync<GitEvent>("git-event", async ev =>
        {
            var msg = $"[{ev.Type}] {ev.UserName}/{ev.Repository}@{ev.Branch} by {ev.AuthorEmail} ({ev.CommitHash})";
            _logger.LogInformation(msg);
            await File.AppendAllTextAsync("git-events.log", $"{DateTime.UtcNow:o}: {msg}{Environment.NewLine}");
        }, stoppingToken);
    }
}