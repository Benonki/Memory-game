using Memory_game_shared.Constants;
using Memory_game_shared.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;

namespace Memory_game.Model.Services.Impl
{
    public class LobbyService : ILobbyService
    {

        HubConnection? connection;
        public event Action<string> OnGameStarted;
        public event Action<GameState> OnGameStartedWithState;

        public async Task ConnectAsync(string serverAddress)
        {
            Debug.WriteLine("Trying to connect");
            try
            {
                connection = new HubConnectionBuilder()
                .WithUrl($"http://{serverAddress}/gamehub")
                .Build();

                HandleServerEvents();

                await connection.StartAsync();

                Debug.WriteLine(connection.State);
            }catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

        }

        private void HandleServerEvents()
        {
            connection.On<string>(HubMethods.GameStarted, (message) =>
            {
                Debug.WriteLine(message);
                OnGameStarted?.Invoke(message);
            });
        }

        public async Task JoinGameAsync()
        {
            if (connection == null)
                return;

            await connection.InvokeAsync(HubMethods.JoinGame);
        }

        public async Task DisconnectAsync()
        {
            if(connection != null)
                await connection.StopAsync();
        }

        public async Task SendMessageAsync()
        {
            if(connection != null )
                await connection.InvokeAsync(HubMethods.SendMessage, "Message from client");
        }

        public async Task CreateNewGame(GameSettings gameSettings)
        {
            if(connection != null)
            await connection.InvokeAsync(HubMethods.CreateNewGame, gameSettings);
        }
    }
}
