using Memory_game.ViewModel;
using Memory_game.Model.Services;
using System.Windows;
using Memory_game_server.Services.Impl;
using Memory_game.Model.Services.Impl;

namespace Memory_game.View
{
    public partial class BoardSetupWindow : Window
    {
        public BoardSetupWindow(INavigationService navigationService, ICardDeckService deckService)
        {
            InitializeComponent();
            BoardSetupViewModel viewModel = new BoardSetupViewModel(
                navigationService,
                deckService, 
                App.SharedLobbyService,
                new BroadcastServiceImpl(),
                new ServerManagerImpl(),
                new DeckPackageService()
                );

            DataContext = viewModel;
            Owner = Application.Current.MainWindow;
        }
    }
}