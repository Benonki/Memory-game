using Memory_game.Model.Services;
using Memory_game.MVVM;
using System.Collections.ObjectModel;

namespace Memory_game.ViewModel
{
    public class ServerListWindowViewModel : ViewModelBase
    {
        private IServerListener _serverListener;
        private ILobbyService _lobbyService;

        public ObservableCollection<string> AvailableServers { get; } = new();

        public RelayCommand ConnectToSevrer => new RelayCommand(async execute => await _lobbyService.JoinGame(SelectedServer), canExecute => true);
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




    }
}
