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
