namespace PatternsApp.Shared;

public class UserMessage
{
    public string UserId { get; set; }
    public Guid ChatRoomId { get; set; }
    public Guid MessageId { get; set; }
    public string Text { get; set; }
}