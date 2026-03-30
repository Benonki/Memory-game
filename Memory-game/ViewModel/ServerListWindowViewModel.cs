using Memory_game.Model.Services;
using Memory_game.MVVM;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Memory_game.ViewModel
{
    public class ServerListWindowViewModel : ViewModelBase
    {
        private IServerListener _serverListener;
        private ILobbyService _lobbyService;

        public ObservableCollection<string> AvailableServers { get; } = new();

        public RelayCommand ConnectToSevrer => new RelayCommand(async execute => await JoinGameAsync(), canExecute => true);
        public ServerListWindowViewModel(IServerListener serverListener, ILobbyService lobbyService)
        {
            _serverListener = serverListener;
            _lobbyService = lobbyService;

            _serverListener.ServerFound += (serverAddress =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (!AvailableServers.Contains(serverAddress))
                        AvailableServers.Add(serverAddress);
                });

            });

            _serverListener.StartListeningAsync();
            _lobbyService = lobbyService;
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

        public void CleanUp()
        {
            _serverListener.StopListening();
            Debug.WriteLine("Closing udp");
        }




    }
}
