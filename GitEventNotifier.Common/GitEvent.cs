namespace GitEventNotifier.Common;

public class GitEvent
{
    public string?  EventId { get; set; }
    public string? UserName { get; set; }
    public string? Repository { get; set; }
    public string? Branch { get; set; }
    public string? CommitHash { get; set; }
    public string? AuthorEmail { get; set; }
    public DateTime? Timestamp { get; set; }
    public GitEventType? Type { get; set; }
}

public enum GitEventType
{
    Push,
    Rollbach,
    Pull
}