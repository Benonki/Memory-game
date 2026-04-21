using Memory_game.Model.Services;
using Memory_game.Model.Services.Impl;
using Memory_game.ViewModel;
using System.Windows;

namespace Memory_game
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel viewModel = new MainWindowViewModel(new NavigationServiceImpl(), App.SharedLobbyService);
            DataContext = viewModel;
        }
    }
}