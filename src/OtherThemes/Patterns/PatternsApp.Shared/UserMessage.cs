namespace PatternsApp.Shared;

public class UserMessage
{
    public Guid UserId { get; set; }
    public Guid MessageId { get; set; }
    public string Text { get; set; }
}