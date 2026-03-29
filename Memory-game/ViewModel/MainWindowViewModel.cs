using Memory_game.Model.Services;
using Memory_game.Model.Services.Impl;
using Memory_game.MVVM;

namespace Memory_game.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;
        private readonly ILobbyService lobbyService;

        public RelayCommand OpenBoardCommand => new RelayCommand(execute => navigationService.OpenBoardSetup(), canExecute => true);

        public RelayCommand OpenCardDeckWindowCommand => new RelayCommand(execute => navigationService.OpenCardDeckWindow(), canExecute => true);

        public RelayCommand SendMessageToSever => new RelayCommand(async execute => await lobbyService.SendMessageAsync(), canExecute => true);
        public RelayCommand OpenServerList => new RelayCommand(execute => navigationService.OpenServerListWindow(), canExecute => true);

        public MainWindowViewModel(INavigationService navigation, ILobbyService lobby)
        {
            navigationService = navigation;
            lobbyService = new LobbyService();

        }
    }
}