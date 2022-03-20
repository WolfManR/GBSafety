using System.Diagnostics.CodeAnalysis;

namespace PatternsApp.Shared;

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