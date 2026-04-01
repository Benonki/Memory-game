using Memory_game_shared.Constants;
using Memory_game_shared.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;

namespace Memory_game.Model.Services.Impl
{
    public class LobbyService : ILobbyService
    {

        HubConnection? connection;
        public event Action<GameState> OnGameStarted;
        public event Action<int> OnCardFlipped;
        public event Action<List<int>, string> OnMatchFound;
        public event Action<List<int>> OnMatchFailed;
        public event Action<string> OnTurnChanged;
        public event Action<string> OnGameOver;

        public string MyConnectionId => connection?.ConnectionId ?? "";

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
            connection.On<GameState>(HubMethods.GameStarted, (gameState) =>
            {
                Debug.WriteLine(gameState);
                OnGameStarted?.Invoke(gameState);
            });

            connection.On<int>(HubMethods.FlipCard, (cardId) =>
            {
                OnCardFlipped?.Invoke(cardId);
            });

            connection.On<List<int>, string>(HubMethods.MatchFound, (cardIds, currentPlayerId) =>
            {
                OnMatchFound?.Invoke(cardIds, currentPlayerId);
            });

            connection.On<List<int>>(HubMethods.MatchFailed, (cardIds) =>
            {
                OnMatchFailed?.Invoke(cardIds);
            });

            connection.On<string>(HubMethods.ChangeTurn, (currentPlayerId) =>
            {
                OnTurnChanged?.Invoke(currentPlayerId);
            });

            connection.On<string>(HubMethods.GameOver, (result) =>
            {
                OnGameOver?.Invoke(result);
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

        public async Task SendFlipCardAsync(int cardId)
        {
            if(connection != null)
                await connection.InvokeAsync(HubMethods.FlipCard, cardId);
        }
    }
}
