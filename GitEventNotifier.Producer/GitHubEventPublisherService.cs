using System.Collections.Concurrent;
using System.Diagnostics;
using GitEventNotifier.Common;
using GitEventNotifier.Infrastructure.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Octokit;

namespace GitEventNotifier.Producer;

public class GitHubEventPublisherService : BackgroundService
{
    private readonly IRabbitMqProducer _producer;
    private readonly ILogger<GitHubEventPublisherService> _logger;
    private readonly string _githubUserName;
    private readonly GitHubClient _githubClient;
    private readonly ConcurrentDictionary<string, bool> _processedEvents = new();

    public GitHubEventPublisherService(
        IRabbitMqProducer producer,
        ILogger<GitHubEventPublisherService>? logger,
        IConfiguration configuration)
    {
        _producer = producer;
        _logger = logger;
        _githubUserName = configuration["GitHub:Username"] 
                          ?? throw new InvalidOperationException("GitHub:Username is required");
        _githubClient = new GitHubClient(new ProductHeaderValue("GitEventNotifier"));
        
        var token = configuration["GitHub:Token"];
        if(!string.IsNullOrWhiteSpace(token))
            _githubClient.Credentials = new Credentials(token);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"GitHub event publisher for:{_githubUserName}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var events = await _githubClient.Activity.Events.GetAllUserPerformedPublic(_githubUserName);
                foreach (var ev in events)
                {
                    if (_processedEvents.ContainsKey(ev.Id.ToString())) continue;
                    if (ev.Type == "CreateEvent" && ev.Payload is CreateEventPayload payload)
                    {
                        var a = ev.Repo?.Name ?? "unknown";
                        var gitEvent = new GitEvent()
                        {
                            EventId = ev.Id,
                            UserName = ev.Actor.Login,
                            Repository = ev.Repo?.Name ?? "unknown",
                            Branch = payload.Ref?.Replace("refs/heads/", "") ?? "unknown",
                            AuthorEmail = "*@gmail.com",
                            Timestamp = ev.CreatedAt.LocalDateTime,
                            Type = GitEventType.Push
                        };
                        
                        await _producer.PublishAsync("git-event", gitEvent, stoppingToken);
                        _processedEvents.TryAdd(ev.Id.ToString(), true);
                        _logger.LogInformation($"Published: {gitEvent.Repository}@{gitEvent.Branch}");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Publisher error");
            }
            
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}