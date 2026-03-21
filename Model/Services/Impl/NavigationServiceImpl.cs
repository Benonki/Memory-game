using Memory_game.View;

namespace Memory_game.Model.Services.Impl
{
    public class NavigationServiceImpl : INavigationService
    {
        public void OpenBoard()
        {
            BoardWindow boardWindow = new BoardWindow();
            boardWindow.Show();
        }
    }
}
