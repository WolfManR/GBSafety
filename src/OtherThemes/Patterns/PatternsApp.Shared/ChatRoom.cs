namespace PatternsApp.Shared;

public class ChatRoom : ChatRoomInfo
{
    public List<UserMessage> Messages { get; set; } = new();
}