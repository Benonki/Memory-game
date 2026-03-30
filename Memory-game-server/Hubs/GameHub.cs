
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

        public async Task CreateNewGame(GameSettings gameSettings)
        {
            _players.Clear();
            _players.Add(Context.ConnectionId);

            _gameState.settings = gameSettings;
        }


        public async Task JoinGame()
        {
            if (!_players.Contains(Context.ConnectionId))
            {
                _players.Add(Context.ConnectionId);
                Debug.WriteLine("New player joined");

            }
           

            if(_players.Count == 2)
            {

                // Creating game and starting it

                GenerateBoard(_gameState);
                await Clients.All.SendAsync(HubMethods.GameStarted, _gameState);
                //await Clients.All.SendAsync(HubMethods.GameStarted, _gameState);
            }

        }

        private void GenerateBoard(GameState gameState)
        {
            int totalCards = gameState.settings.Rows * gameState.settings.Columns;
            int pairOfCards = totalCards / 2;

            List<Card> cardsToShuffle = new List<Card>();

            for(int i = 1; i <= pairOfCards; i++)
            {
                string imagePath = $"Image{i}";

                cardsToShuffle.Add(new Card { pairId = i, imagePath = imagePath });
                cardsToShuffle.Add(new Card { pairId = i, imagePath = imagePath });
            }

            ShuffleCards(cardsToShuffle);

            for(int i = 0; i < cardsToShuffle.Count; i++)
            {
                Card card = cardsToShuffle[i];
                card.id = i;
                card.isFaceUp = false;
                card.isMatched = false;

                _gameState.CardsOnBoard.Add(card);
            }

        }

        private void ShuffleCards(List<Card> cardsToShuffle)
        {
            Random rng = new Random();
            int n = cardsToShuffle.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = cardsToShuffle[k];
                cardsToShuffle[k] = cardsToShuffle[n];
                cardsToShuffle[n] = value;
            }
        }


    }
}
