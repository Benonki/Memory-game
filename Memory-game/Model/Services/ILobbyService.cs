using Memory_game_shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_game.Model.Services
{
    public interface ILobbyService
    {
        public event Action<GameState> OnGameStarted;
        public event Action<int> OnCardFlipped;
        public Task SendFlipCardAsync(int cardId);
        public Task CreateNewGame(GameSettings gameSettings);
        public Task ConnectAsync(string serverAddress);
        public Task JoinGameAsync();
        public Task SendMessageAsync();
        public Task DisconnectAsync();

    }
}
