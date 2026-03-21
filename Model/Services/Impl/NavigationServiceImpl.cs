using Memory_game.View;
using Memory_game.ViewModel;
using System.Windows;

namespace Memory_game.Model.Services.Impl
{
    public class NavigationServiceImpl : INavigationService
    {
        public void OpenBoardSetup()
        {
            BoardSetupWindow setupWindow = new BoardSetupWindow(this);
            setupWindow.ShowDialog();
        }

        public void OpenBoard(int rows, int columns)
        {
            BoardWindow boardWindow = new BoardWindow(rows, columns);
            boardWindow.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is BoardSetupWindow)
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
    }
}