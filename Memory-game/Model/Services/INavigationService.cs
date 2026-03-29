namespace Memory_game.Model.Services
{
    public interface INavigationService
    {
        void OpenBoardSetup();
        void OpenBoard(int rows, int columns, string deckName);
        void OpenMainWindow();
        void OpenCardDeckWindow();
        void OpenServerListWindow();
        string SelectedDeck { get; set; }

    }
}