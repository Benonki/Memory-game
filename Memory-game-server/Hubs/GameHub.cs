
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
        private static int _currentPlayerIndex = 0;

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
            }

            int maxPlayers = _gameState.settings.MaxPlayers;

            await Clients.All.SendAsync(HubMethods.WaitingForPlayers, _players.Count, maxPlayers);

            if (_players.Count == maxPlayers)
            {
                Random rng = new Random();
                _currentPlayerIndex = rng.Next(_players.Count);
                _currentPlayerTurn = _players[_currentPlayerIndex];

                if (_gameState.settings.DeckZipData != null && _gameState.settings.DeckZipData.Length > 0)
                {
                    foreach (var playerId in _players)
                    {
                        if (playerId != _players[0])
                        {
                            await Clients.Client(playerId).SendAsync(
                                HubMethods.DeckPackage,
                                _gameState.settings.DeckName,
                                _gameState.settings.DeckZipData,
                                _gameState.settings.ImagePaths.Length);
                        }
                    }
                }

                GenerateBoard(_gameState);

                await Clients.All.SendAsync(HubMethods.GameStarted, _gameState);

                int turnTimeSeconds = _gameState.settings.TurnTimeSeconds;
                await Clients.All.SendAsync(HubMethods.ChangeTurn, _currentPlayerTurn, turnTimeSeconds);
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

            if (_currentlyFlippedCards.Count == 2)
            {
                Card firstCard = _gameState.CardsOnBoard.First(card => card.id == _currentlyFlippedCards[0]);
                Card secondCard = _gameState.CardsOnBoard.First(card => card.id == _currentlyFlippedCards[1]);

                if (firstCard.pairId == secondCard.pairId)
                {
                    firstCard.isMatched = true;
                    secondCard.isMatched = true;
                    _gameState.Scores[_currentPlayerTurn] = _gameState.Scores.GetValueOrDefault(_currentPlayerTurn, 0) + 1;

                    await CheckGameOver();
                    await Clients.All.SendAsync(HubMethods.MatchFound, _currentlyFlippedCards, _currentPlayerTurn);
                }
                else
                {
                    await Task.Delay(1000);
                    firstCard.isFaceUp = false;
                    secondCard.isFaceUp = false;

                    _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
                    _currentPlayerTurn = _players[_currentPlayerIndex];

                    await Clients.All.SendAsync(HubMethods.MatchFailed, _currentlyFlippedCards);

                    int turnTimeSeconds = _gameState.settings.TurnTimeSeconds;
                    await Clients.All.SendAsync(HubMethods.ChangeTurn, _currentPlayerTurn, turnTimeSeconds);
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
                int maxScore = _gameState.Scores.Values.Max();
                var winners = _gameState.Scores.Where(s => s.Value == maxScore).Select(s => s.Key).ToList();

                foreach (var playerId in _players)
                {
                    string result;
                    if (winners.Count == 1 && winners[0] == playerId)
                        result = "win";
                    else if (winners.Count > 1 && winners.Contains(playerId))
                        result = "draw";
                    else if (winners.Count == 1)
                        result = "loss";
                    else
                        result = "draw";

                    await Clients.Client(playerId).SendAsync(HubMethods.GameOver, result);
                }
            }
        }

        public async Task TurnTimeout()
        {
            if (Context.ConnectionId != _currentPlayerTurn)
                return;

            if (_currentlyFlippedCards.Count > 0)
            {
                foreach (int cardId in _currentlyFlippedCards)
                {
                    Card card = _gameState.CardsOnBoard.First(c => c.id == cardId);
                    card.isFaceUp = false;
                }

                await Clients.All.SendAsync(HubMethods.MatchFailed, _currentlyFlippedCards);
                _currentlyFlippedCards.Clear();
            }

            bool allMatched = _gameState.CardsOnBoard.All(card => card.isMatched);
            if (allMatched)
            {
                await CheckGameOver();
                return;
            }

            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
            _currentPlayerTurn = _players[_currentPlayerIndex];

            int turnTimeSeconds = _gameState.settings.TurnTimeSeconds;
            await Clients.All.SendAsync(HubMethods.ChangeTurn, _currentPlayerTurn, turnTimeSeconds);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _players.Remove(Context.ConnectionId);

            await Clients.All.SendAsync(HubMethods.PlayerDisconnected);

            await base.OnDisconnectedAsync(exception);
        }


    }
}
