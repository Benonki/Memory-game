
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
        private static string _currentPlayerTurn = "";
        private static List<int> _currentlyFlippedCards = new List<int>();

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

                _currentPlayerTurn = _players[0];

                GenerateBoard(_gameState);
                
                await Clients.All.SendAsync(HubMethods.GameStarted, _gameState);

                await Clients.All.SendAsync(HubMethods.ChangeTurn, _currentPlayerTurn);
            }

        }

        public async Task FlipCard(int cardId)
        {

            if (Context.ConnectionId != _currentPlayerTurn)
                return;


            Card cardToFlip = _gameState.CardsOnBoard.FirstOrDefault(card => card.id == cardId);

            if (cardToFlip == null || cardToFlip.isFaceUp)
                return;


            cardToFlip.isFaceUp = true;
            _currentlyFlippedCards.Add(cardId);
            await Clients.All.SendAsync(HubMethods.FlipCard, cardId);

            if(_currentlyFlippedCards.Count == 2)
            {
                Card firstCard = _gameState.CardsOnBoard.First(card => card.id == _currentlyFlippedCards[0]);
                Card secondCard = _gameState.CardsOnBoard.First(card => card.id == _currentlyFlippedCards[1]);

                if(firstCard.pairId == secondCard.pairId)
                {
                    firstCard.isMatched = true;
                    secondCard.isMatched = true;
                    _gameState.Scores[_currentPlayerTurn] = _gameState.Scores.GetValueOrDefault(_currentPlayerTurn, 0) + 1;
                    await CheckGameOver();

                    await Clients.All.SendAsync(HubMethods.MatchFound, _currentlyFlippedCards, _currentPlayerTurn);
                }else
                {
                    await Task.Delay(1000);
                    firstCard.isFaceUp = false;
                    secondCard.isFaceUp = false;

                    _currentPlayerTurn = _players.First(player => player != _currentPlayerTurn);

                    await Clients.All.SendAsync(HubMethods.MatchFailed, _currentlyFlippedCards);

                    await Clients.All.SendAsync(HubMethods.ChangeTurn, _currentPlayerTurn);
                }
                _currentlyFlippedCards.Clear();
            }
            
        }

        private void GenerateBoard(GameState gameState)
        {
            int totalCards = gameState.settings.Rows * gameState.settings.Columns;
            int pairOfCards = totalCards / 2;

            List<Card> cardsToShuffle = new List<Card>();

            for(int i = 0; i < pairOfCards; i++)
            {
                string imagePath = gameState.settings.ImagePaths[i];

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

        private async Task CheckGameOver()
        {
            bool allMatched = _gameState.CardsOnBoard.All(card => card.isMatched);

            if (allMatched)
            {
                int player1Score = _gameState.Scores.GetValueOrDefault(_players[0], 0);
                int player2Score = _gameState.Scores.GetValueOrDefault(_players[1], 0);

                string result;
                if (player1Score > player2Score)
                    result = "win";
                else if (player2Score > player1Score)
                    result = "loss";
                else
                    result = "draw";

                await Clients.Client(_players[0]).SendAsync(HubMethods.GameOver,
                    player1Score > player2Score ? "win" : (player1Score == player2Score ? "draw" : "loss"));
                await Clients.Client(_players[1]).SendAsync(HubMethods.GameOver,
                    player2Score > player1Score ? "win" : (player1Score == player2Score ? "draw" : "loss"));
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _players.Remove(Context.ConnectionId);

            await Clients.All.SendAsync(HubMethods.PlayerDisconnected);

            await base.OnDisconnectedAsync(exception);
        }


    }
}
