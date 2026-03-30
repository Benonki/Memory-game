using Memory_game.Model.Services;
using Memory_game.MVVM;
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
        private int _score;
        private CardViewModel? _firstSelectedCard;
        private CardViewModel? _secondSelectedCard;
        private bool _isProcessingMove;
        private DispatcherTimer? _delayTimer;
        private readonly ICardDeckService _deckService;
        private readonly ILobbyService _lobbyService;

        public ObservableCollection<CardViewModel> Cards { get; set; } = new ObservableCollection<CardViewModel>();

        public RelayCommand FlipCardCommand => new RelayCommand(execute => FlipCard((CardViewModel)execute), canExecute => true);

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

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
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
            _score = 0;
            _firstSelectedCard = null;
            _secondSelectedCard = null;
            CanInteract = true;
            _deckService = deckService;
            _lobbyService = lobbyService;

            _lobbyService.OnCardFlipped += HandleCardFlipped;

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

        private void ShuffleCards()
        {
            Random rng = new Random();
            int n = Cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = Cards[k];
                Cards[k] = Cards[n];
                Cards[n] = value;
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

        private async Task FlipCard(CardViewModel card)
        {
            if (card != null && !card.IsFaceUp && !card.IsMatched)
            {
                await _lobbyService.SendFlipCardAsync(card.Id);
            }
        }

        //private void FlipCard(CardViewModel card)
        //{
        //    if (!CanInteract)
        //        return;

        //    if (card.IsMatched)
        //        return;

        //    if (card.IsFaceUp)
        //        return;

        //    if (_firstSelectedCard != null && _secondSelectedCard != null)
        //    {
        //        ResetSelectedCards();
        //    }

        //    if (_firstSelectedCard == null)
        //    {
        //        card.IsFaceUp = true;
        //        _firstSelectedCard = card;
        //    }
        //    else if (_secondSelectedCard == null && _firstSelectedCard != card)
        //    {
        //        card.IsFaceUp = true;
        //        _secondSelectedCard = card;
        //        CheckMatch();
        //    }
        //}

        private void CheckMatch()
        {
            if (_firstSelectedCard == null || _secondSelectedCard == null)
                return;

            if (_firstSelectedCard.PairId == _secondSelectedCard.PairId)
            {
                Score++;
                _firstSelectedCard.IsMatched = true;
                _secondSelectedCard.IsMatched = true;
                ResetSelectedCards();
            }
            else
            {
                CanInteract = false;

                _delayTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };

                _delayTimer.Tick += (s, e) =>
                {
                    if (_firstSelectedCard != null && !_firstSelectedCard.IsMatched)
                        _firstSelectedCard.IsFaceUp = false;
                    if (_secondSelectedCard != null && !_secondSelectedCard.IsMatched)
                        _secondSelectedCard.IsFaceUp = false;

                    ResetSelectedCards();
                    CanInteract = true;

                    if (_delayTimer != null)
                    {
                        _delayTimer.Stop();
                        _delayTimer = null;
                    }
                };

                _delayTimer.Start();
            }
        }

        private void ResetSelectedCards()
        {
            _firstSelectedCard = null;
            _secondSelectedCard = null;
        }
    }
}