using PatternsApp.Shared;

namespace PatternsApp.Server.Services;

public class ChatsStorage
{
    public List<UserInfo> Users { get; set; }
    public List<ChatRoom> ChatRooms { get; set; }

    public UserInfo Register(string userId, string login)
    {
        var user = Users.Find(u => u.UserId == userId);
        if (user is not null) return user;

        UserInfo newUser = new()
        {
            Color = "#fff",
            Nick = login,
            UserId = userId
        };
        Users.Add(newUser);
        return newUser;
    }

    public bool SetUserColor(string userId, string color)
    {
        var user = Users.Find(u => u.UserId == userId);
        if (user is null) return false;

        user.Color = color;
        return true;
    }

    public IEnumerable<ChatRoomInfo> GetUserChats(string userId)
    {
        var user = Users.Find(u => u.UserId == userId);
        if (user is null) yield break;

        var userChats = ChatRooms.Where(c => user.ChatRooms.Contains(c.Id));
        foreach (var userChat in userChats)
            yield return userChat;
    }

    public void AddMessage(UserMessage message)
    {
        var room = ChatRooms.Find(r => r.Id == message.ChatRoomId);
        room?.Messages.Add(message);
    }

    public bool CreateRoom(string userId, string roomName)
    {
        var user = Users.Find(u => u.UserId == userId);
        if (user is null) return false;
        ChatRoom room = new()
        {
            Id = Guid.NewGuid(),
            Name = roomName
        };
        user.ChatRooms.Add(room.Id);
        ChatRooms.Add(room);
        return true;
    }
}