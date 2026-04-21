using Memory_game.Model.Services;
using Memory_game.MVVM;
using Memory_game_shared.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace Memory_game.ViewModel
{
    public class ServerListWindowViewModel : ViewModelBase
    {
        private IServerListener _serverListener;
        private ILobbyService _lobbyService;
        private INavigationService _navigationService;

        public ObservableCollection<string> AvailableServers { get; } = new();

        public RelayCommand ConnectToSevrer => new RelayCommand(async execute => await JoinGameAsync(), canExecute => true);
        public ServerListWindowViewModel(IServerListener serverListener, ILobbyService lobbyService, INavigationService navigationService)
        {
            _serverListener = serverListener;
            _lobbyService = lobbyService;
            _navigationService = navigationService;

            _serverListener.ServerFound += (serverAddress =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (!AvailableServers.Contains(serverAddress))
                        AvailableServers.Add(serverAddress);
                });

            });

            _serverListener.StartListeningAsync();
            _lobbyService.OnGameStarted += HandleGameStarter;

        }

        private string _selectedServer;
        public string SelectedServer
        {
            get => _selectedServer;
            set
            {
                _selectedServer = value;
                OnPropertyChanged();
            }
        }

        private async Task JoinGameAsync()
        {
            await _lobbyService.ConnectAsync(SelectedServer);
            await _lobbyService.JoinGameAsync();
        }

        private void HandleGameStarter(GameState gameState)
        {
            _lobbyService.OnGameStarted -= HandleGameStarter;
            Application.Current.Dispatcher.Invoke(() =>
            {
                _navigationService.OpenBoard(gameState, gameState.settings.DeckName);
            });
        }

        public void CleanUp()
        {
            _lobbyService.OnGameStarted -= HandleGameStarter;
            _serverListener.StopListening();
            Debug.WriteLine("Closing udp");
        }




    }
}
