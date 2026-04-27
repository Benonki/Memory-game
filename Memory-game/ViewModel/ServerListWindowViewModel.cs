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
        private Dictionary<string, string> _lobbyAddresses = new();
        private bool _isConnecting = false;

        private string _waitingMessage = string.Empty;
        public string WaitingMessage
        {
            get => _waitingMessage;
            set
            {
                _waitingMessage = value;
                OnPropertyChanged();
            }
        }

        private bool _isWaiting;
        public bool IsWaiting
        {
            get => _isWaiting;
            set
            {
                _isWaiting = value;
                OnPropertyChanged();
            }
        }

        private bool _canConnect = true;
        public bool CanConnect
        {
            get => _canConnect;
            set
            {
                _canConnect = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ConnectToSevrer => new RelayCommand(async execute => await JoinGameAsync(), canExecute => true);
        public ServerListWindowViewModel(IServerListener serverListener, ILobbyService lobbyService, INavigationService navigationService)
        {
            _serverListener = serverListener;
            _lobbyService = lobbyService;
            _navigationService = navigationService;

            _serverListener.ServerFound += (lobbyName, address) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (!AvailableServers.Contains(lobbyName))
                    {
                        AvailableServers.Add(lobbyName);
                        _lobbyAddresses[lobbyName] = address;
                    }
                });
            };

            _serverListener.StartListeningAsync();
            _lobbyService.OnGameStarted += HandleGameStarter;
            _lobbyService.OnWaitingForPlayers += HandleWaitingForPlayers;

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
            if (_isConnecting) return;
            _isConnecting = true;

            try
            {
                if (!string.IsNullOrEmpty(SelectedServer) && _lobbyAddresses.ContainsKey(SelectedServer))
                {
                    string address = _lobbyAddresses[SelectedServer];
                    await _lobbyService.ConnectAsync(address);
                    await _lobbyService.JoinGameAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd łączenia: {ex.Message}");
                MessageBox.Show($"Nie udało się połączyć z serwerem: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isConnecting = false;
            }
        }

        private void HandleWaitingForPlayers(int currentCount, int maxCount)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsWaiting = true;
                CanConnect = false;
                WaitingMessage = $"Oczekiwanie na graczy... ({currentCount}/{maxCount})";
            });
        }

        private void HandleGameStarter(GameState gameState)
        {
            _lobbyService.OnGameStarted -= HandleGameStarter;
            _lobbyService.OnWaitingForPlayers -= HandleWaitingForPlayers;

            Application.Current.Dispatcher.Invoke(() =>
            {
                IsWaiting = false;
                _navigationService.OpenBoard(gameState, gameState.settings.DeckName);
            });
        }

        public void CleanUp()
        {
            _lobbyService.OnGameStarted -= HandleGameStarter;
            _lobbyService.OnWaitingForPlayers -= HandleWaitingForPlayers;
            _serverListener.StopListening();
            Debug.WriteLine("Closing udp");
        }




    }
}
