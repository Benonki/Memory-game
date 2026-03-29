using Memory_game.Model.Services;
using Memory_game.Model.Services.Impl;
using Memory_game.ViewModel;
using System.Windows;

namespace Memory_game.View
{

    public partial class ServerListWindow : Window
    {
        public ServerListWindow()
        {
            InitializeComponent();
            ServerListWindowViewModel viewModel = new ServerListWindowViewModel(new ServerListener(), new LobbyService());
            DataContext = viewModel;
            Owner = Application.Current.MainWindow;
        }
    }
}
