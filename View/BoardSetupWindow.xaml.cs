using Memory_game.ViewModel;
using Memory_game.Model.Services;
using System.Windows;

namespace Memory_game.View
{
    public partial class BoardSetupWindow : Window
    {
        public BoardSetupWindow(INavigationService navigationService)
        {
            InitializeComponent();
            BoardSetupViewModel viewModel = new BoardSetupViewModel(navigationService);
            DataContext = viewModel;
            Owner = Application.Current.MainWindow;
        }
    }
}