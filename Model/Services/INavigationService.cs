namespace Memory_game.Model.Services
{
    public interface INavigationService
    {
        void OpenBoardSetup();
        void OpenBoard(int rows, int columns);
        void OpenMainWindow();
    }
}