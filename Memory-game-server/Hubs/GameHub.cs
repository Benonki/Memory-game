
using Memory_game_shared.Constants;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace Memory_game_server.Hubs
{
    public class GameHub : Hub
    {

        public async Task SendMessage(string message)
        {
            Debug.WriteLine($"Na serwerze {message} ");
            await Clients.All.SendAsync(HubMethods.ReceiveMessage, "Message from server");
        }

    }
}
