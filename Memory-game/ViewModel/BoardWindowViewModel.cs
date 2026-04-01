using Memory_game.Model.Services;
using Memory_game.MVVM;
using Memory_game.View;
using Memory_game_shared.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Memory_game.ViewModel
{
    public class BoardWindowViewModel : ViewModelBase
    {
        private int _rows;
        private int _columns;
        private int _myScore;
        private int _opponentScore;
        private string _currentTurnText;
        private string _gameResultMessage = string.Empty;
        private CardViewModel? _firstSelectedCard;
        private CardViewModel? _secondSelectedCard;
        private bool _isProcessingMove;
        private DispatcherTimer? _delayTimer;
        private readonly ICardDeckService _deckService;
        private readonly ILobbyService _lobbyService;

        public ObservableCollection<CardViewModel> Cards { get; set; } = new ObservableCollection<CardViewModel>();

        public RelayCommand FlipCardCommand => new RelayCommand(async execute => await FlipCard((CardViewModel)execute), canExecute => true);

        public int Rows
        {
            get => _rows;
            set
            {
                _rows = value;
                OnPropertyChanged();
            }
        }

        public int Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                OnPropertyChanged();
            }
        }

        public int MyScore
        {
            get => _myScore;
            set
            {
                _myScore = value;
                OnPropertyChanged();
            }
        }

        public int OpponentScore
        {
            get => _opponentScore;
            set
            {
                _opponentScore = value;
                OnPropertyChanged();
            }
        }

        public string CurrentTurnText
        {
            get => _currentTurnText;
            set
            {
                _currentTurnText = value;
                OnPropertyChanged();
            }
        }



        public bool CanInteract
        {
            get => !_isProcessingMove;
            set
            {
                _isProcessingMove = !value;
                OnPropertyChanged();
            }
        }

        public BoardWindowViewModel(GameState gameState, string deckName, ICardDeckService deckService, ILobbyService lobbyService)
        {
            _rows = gameState.settings.Rows;
            _columns = gameState.settings.Columns;
            _myScore = 0;
            _opponentScore = 0;
            _firstSelectedCard = null;
            _secondSelectedCard = null;
            CanInteract = true;
            _deckService = deckService;
            _lobbyService = lobbyService;

            _lobbyService.OnCardFlipped += HandleCardFlipped;
            _lobbyService.OnMatchFound += HandleCardsMatchFound;
            _lobbyService.OnMatchFailed += HandleCardsMatchFailed;
            _lobbyService.OnTurnChanged += HandleTurnChange;
            _lobbyService.OnGameOver += HandleGameOver;

            InitializeCards(gameState.CardsOnBoard ,deckName);
        }

        private void InitializeCards(List<Card> cardsFromServer,string deckName)
        {
            int totalCards = Rows * Columns;
            string[] imageFiles = _deckService.GetCardsFromDeck(deckName);

            foreach(Card card in cardsFromServer)
            {
                Cards.Add(new CardViewModel(card.id, card.pairId, card.imagePath));
            }
        }

        private void HandleCardFlipped(int cardId)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var cardToFlip = Cards.FirstOrDefault(card => card.Id == cardId);
                if (cardToFlip != null)
                {
                    cardToFlip.IsFaceUp = true;
                }
            });
        }

        private void HandleCardsMatchFound(List<int> cardIds, string currentPlayerId)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (int cardId in cardIds)
                {
                    CardViewModel? card = Cards.FirstOrDefault(card => card.Id == cardId);
                    if(card != null)
                        card.IsMatched = true;
                }
                
                if(currentPlayerId == _lobbyService.MyConnectionId)
                {
                    MyScore++;
                }
                else
                {
                    OpponentScore++;
                }
            });
        }

        private void HandleCardsMatchFailed(List<int> cardIds)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (int cardId in cardIds)
                {
                    CardViewModel? card = Cards.FirstOrDefault(card => card.Id == cardId);
                    if (card != null)
                        card.IsFaceUp = false;
                }
            });
        }

        private void HandleTurnChange(string currentPlayerId)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if(currentPlayerId == _lobbyService.MyConnectionId)
                {
                    CurrentTurnText = "Twoja tura";
                    CanInteract = true;
                }
                else
                {
                    CurrentTurnText = "Tura przeciwnika";
                    CanInteract = false;
                }
            });
        }

        private void HandleGameOver(string result)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                string message;
                switch (result)
                {
                    case "win":
                        message = "Gratulacje! Wygrałeś!";
                        break;
                    case "loss":
                        message = "Niestety, przegrałeś. Spróbuj ponownie!";
                        break;
                    case "draw":
                        message = "Remis!";
                        break;
                    default:
                        message = "Koniec gry!";
                        break;
                }

                MessageBoxResult mbResult = MessageBox.Show(
                    message + "\n\nKliknij OK, aby zamknąć grę.",
                    "Koniec gry",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                if (mbResult == MessageBoxResult.OK)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window is BoardWindow)
                            {
                                window.Close();
                                break;
                            }
                        }
                    });
                }
            });
        }

        private async Task FlipCard(CardViewModel card)
        {
            if (card != null && !card.IsFaceUp && !card.IsMatched)
            {
                await _lobbyService.SendFlipCardAsync(card.Id);
            }
        }

    }
}