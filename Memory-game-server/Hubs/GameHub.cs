
using Memory_game_shared.Constants;
using Memory_game_shared.Models;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace Memory_game_server.Hubs
{
    public class GameHub : Hub
    {

        private static GameState _gameState = new GameState();
        private static List<string> _players = new List<string>();

        public async Task SendMessage(string message)
        {
            Debug.WriteLine($"Na serwerze {message} ");
            await Clients.All.SendAsync(HubMethods.ReceiveMessage, "Message from server");
        }


        public async Task JoinGame()
        {
            _players.Add(Context.ConnectionId);

            if(_players.Count == 2)
            {

                // Creating game and starting it

                await Clients.All.SendAsync(HubMethods.GameStarted, "JOINED GAME STARTING");
                //await Clients.All.SendAsync(HubMethods.GameStarted, _gameState);
            }

        }


    }
}
