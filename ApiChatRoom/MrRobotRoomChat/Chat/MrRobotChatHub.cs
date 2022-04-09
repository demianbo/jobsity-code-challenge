using Microsoft.AspNetCore.SignalR;

namespace MrRobotRoomChat.Chat
{
    public class MrRobotChatHub : Hub
    {
        public async Task SendMessageAsync(string room, string user, string message)
        {
            await Clients.Group(room).SendAsync("ReceiveMessage", user, message);
        }

        public async Task AddToGroupAsync(string room)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, room);
            await Clients.Group(room).SendAsync("ShowWho", $"New user connected {Context.ConnectionId}");
        }
    }
}
