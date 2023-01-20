using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography.X509Certificates;

namespace ChatAppASP
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string clientId, string message)
        {
            Console.WriteLine(message);
            await Clients.All.SendAsync("MessageReceived", clientId + ": " + message);
        }
        public async Task SendRoomMessage(string message, string roomName)
        {
            Console.WriteLine(message);
            await Clients.Group(roomName).SendAsync("RoomMessageReceived", message);
        }
        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

            await Clients.Group(roomName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {roomName}.");
        }
        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

            await Clients.Group(roomName).SendAsync("Send", $"{Context.ConnectionId} has left the group {roomName}.");
        }

        public async Task BroadcastToConnection(string data, string connectionId)
        {
            await Clients.Client(connectionId).SendAsync("broadcasttoclient", data);
        }

        public async Task BroadcastToUser(string data, string userId)
        {
            await Clients.User(userId).SendAsync("broadcasttouser", data);
        }

        public string GetConnectionId() => Context.ConnectionId;
    }
}