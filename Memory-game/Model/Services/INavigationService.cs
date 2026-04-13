using Memory_game_shared.Models;

namespace Memory_game.Model.Services
{
    public interface INavigationService
    {
        void OpenBoardSetup();
        void OpenBoard(GameState gameState, string deckName, IServerManager? serverManager = null);
        void OpenMainWindow();
        void OpenCardDeckWindow();
        void OpenServerListWindow();
        string SelectedDeck { get; set; }

    }
}