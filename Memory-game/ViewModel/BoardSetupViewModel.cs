using Memory_game.MVVM;
using Memory_game.Model.Services;
using System.Windows;
using Memory_game_shared.Models;
using System.Diagnostics;
using Memory_game_server.Services;

namespace Memory_game.ViewModel
{
    public class BoardSetupViewModel : ViewModelBase
    {
        private string _rows = "4";
        private string _columns = "4";
        private string _errorMessage = string.Empty;
        private string _selectedDeck = "DefaultDeck1";
        private bool _isServerStarting;

        private readonly ILobbyService _lobbyService;
        private readonly ICardDeckService _deckService;
        private readonly INavigationService _navigationService;
        private readonly IBroadcastService _broadcastService;
        private readonly IServerManager _serverManager;

        public BoardSetupViewModel(INavigationService navigationService,
            ICardDeckService deckService,
            ILobbyService lobbyService,
            IBroadcastService broadcastService,
            IServerManager serverManager)
        {

            _navigationService = navigationService;
            _deckService = deckService;
            _lobbyService = lobbyService;
            _broadcastService = broadcastService;
            _serverManager = serverManager;

            _selectedDeck = _navigationService.SelectedDeck;

            _lobbyService.OnGameStarted += HandleGameStarted;

            CanInteract = true;

        }

        public string Rows
        {
            get => _rows;
            set
            {
                _rows = value;
                OnPropertyChanged();
            }
        }

        public string Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public string SelectedDeck
        {
            get => _selectedDeck;
            set
            {
                _selectedDeck = value;
                OnPropertyChanged();
            }
        }

        public bool CanInteract
        {
            get => !_isServerStarting;
            set
            {
                _isServerStarting = !value;
                OnPropertyChanged();
            }
        }


        public RelayCommand StartCommand => new RelayCommand(async execute => await Start(), canExecute => true);

        public RelayCommand CancelCommand => new RelayCommand(execute => Cancel(), canExecute => true);

        private async Task Start()
        {
            if (int.TryParse(Rows, out int rows) && int.TryParse(Columns, out int columns))
            {
                int totalCards = rows * columns;
                if (totalCards % 2 != 0)
                {
                    ErrorMessage = $"Liczba kart ({rows} x {columns} = {totalCards}) musi być parzysta. Wybierz inne wymiary.";
                    return;
                }

                if (rows < 2 || rows > 6 || columns < 2 || columns > 6)
                {
                    ErrorMessage = "Wiersze i kolumny muszą być w zakresie 2-6";
                    return;
                }

                int availableCards = _deckService.GetCardCount(SelectedDeck);
                if (totalCards / 2 > availableCards)
                {
                    ErrorMessage = $"Wybrany zestaw kart '{SelectedDeck}' ma tylko {availableCards} kart, a potrzebujesz {totalCards / 2}. Wybierz inny zestaw lub zmniejsz wymiary planszy.";
                    return;
                }

                ErrorMessage = "Łączenie z serwerem";

                GameSettings gameSettings = new GameSettings
                {
                    Rows = rows,
                    Columns = columns,
                    ImagePaths = _deckService.GetCardsFromDeck(SelectedDeck),
                    DeckName = SelectedDeck
                };

                try
                {
                    ErrorMessage = "Uruchamianie serwera";
                    CanInteract = false;

                    await _serverManager.StartServerAsync(5000);

                    _ = _broadcastService.StartBroadcastingAsync(5000);

                    ErrorMessage = "Łączenie z serwerem";

                    await _lobbyService.ConnectAsync("localhost:5000");
                    await _lobbyService.CreateNewGame(gameSettings);

                    ErrorMessage = "Czekanie na drugiego gracza";
                }catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    ErrorMessage = "Nie udało się uruchomić serwera";

                    await _serverManager.StopServerAsync();
                    _broadcastService.StopBroadcasting();

                    CanInteract = true;
                }

                
            }
            else
            {
                ErrorMessage = "Wprowadź poprawne liczby całkowite";
            }
        }

        private async Task Cancel()
        {

            await _serverManager.StopServerAsync();
            _broadcastService.StopBroadcasting();

            await _lobbyService.DisconnectAsync();

            CanInteract = true;

            _navigationService.OpenMainWindow();
        }

        private void HandleGameStarted(GameState gameState)
        {
            _lobbyService.OnGameStarted -= HandleGameStarted;

            _broadcastService.StopBroadcasting();

            Application.Current.Dispatcher.Invoke(() =>
            {
                _navigationService.OpenBoard(gameState, SelectedDeck, _serverManager);
            });
        }
    }
}