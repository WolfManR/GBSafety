using System.Diagnostics.CodeAnalysis;

namespace PatternsApp.Models;

public class UserMessage
{
    public Guid UserId { get; set; }
    public Guid MessageId { get; set; }
    public string Text { get; set; }
}

public class User
{
    public Guid UserId { get; set; }
    public string Color { get; set; }
    public string Nick { get; set; }
}

public class Chat
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public User[] Users { get; set; }
    public List<UserMessage> Messages { get; set; }

    public bool TryFindUser(Guid id,[NotNullWhen(true)] out User? user)
    {
        user = null;
        if (Users.FirstOrDefault(u => u.UserId == id) is not { } found) return false;
        user = found;
        return true;
    }
}

public class ChatsInfo
{
    public List<ChatInfo> Chats { get; set; }

    public List<User> Contacts { get; set; }
}

public class ChatInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}