using Memory_game.View;
using Memory_game.ViewModel;
using Memory_game_shared.Models;
using System.Windows;

namespace Memory_game.Model.Services.Impl
{
    public class NavigationServiceImpl : INavigationService
    {
        private readonly ICardDeckService _deckService = new CardDeckServiceImpl();
        private readonly IDeckPackageService _deckPackageService = new DeckPackageService();
        public string SelectedDeck { get; set; } = "DefaultDeck1";
        public void OpenBoardSetup()
        {
            BoardSetupWindow setupWindow = new BoardSetupWindow(this, _deckService);
            setupWindow.ShowDialog();
        }

        public void OpenBoard(GameState gameState, string deckName, IServerManager? serverManager = null)
        {
            BoardWindow boardWindow = new BoardWindow(gameState, gameState.settings.DeckName, _deckService, serverManager);
            boardWindow.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is BoardSetupWindow || window is ServerListWindow)
                {
                    window.Close();
                    break;
                }
            }
        }

        public void OpenMainWindow()
        {
            Window mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            if (mainWindow != null)
            {
                mainWindow.Show();
                if (mainWindow.WindowState == WindowState.Minimized)
                {
                    mainWindow.WindowState = WindowState.Normal;
                }
                mainWindow.Activate();
            }
            else
            {
                mainWindow = new MainWindow();
                mainWindow.Show();
            }

            foreach (Window window in Application.Current.Windows)
            {
                if (window is BoardSetupWindow)
                {
                    window.Close();
                    break;
                }
            }
        }

        public void OpenCardDeckWindow()
        {
            CardDeckWindow cardDeckWindow = new CardDeckWindow(this, _deckService);
            cardDeckWindow.ShowDialog();
        }

        public void OpenServerListWindow()
        {
            ServerListWindow serverListWindow = new ServerListWindow();
            serverListWindow.ShowDialog();
        }
    }
}