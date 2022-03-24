namespace PatternsApp.Shared;

public class UserInfo : User
{
    public HashSet<Guid> ChatRooms { get; set; } = new();
}