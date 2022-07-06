using Microsoft.AspNetCore.SignalR;
using PatternsApp.Server.Services;
using PatternsApp.Shared;

namespace PatternsApp.Server.Hubs;

public class ChatHub : Hub
{
    private readonly ChatsStorage _chatsStorage;

    public ChatHub(ChatsStorage chatsStorage)
    {
        _chatsStorage = chatsStorage;
    }

    public UserInfo GetUserInfo(string login)
    {
        return _chatsStorage.Register(Context.ConnectionId, login);
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async IAsyncEnumerable<ChatRoomInfo> GetChatRooms()
    {
        foreach (var chat in _chatsStorage.GetUserChats(Context.ConnectionId))
            yield return chat;
    }
}