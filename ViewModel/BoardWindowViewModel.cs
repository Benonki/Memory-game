using Memory_game.MVVM;
using System.Collections.ObjectModel;
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

        public BoardWindowViewModel(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;
            _score = 0;
            _firstSelectedCard = null;
            _secondSelectedCard = null;
            CanInteract = true;

            InitializeCards();
        }

        private void InitializeCards()
        {
            int totalCards = Rows * Columns;
            for (int i = 0; i < totalCards / 2; i++)
            {
                int pairId = i + 1;
                string imagePath = $"/Assets/Cards/{pairId}.png";
                Cards.Add(new CardViewModel(pairId, imagePath));
                Cards.Add(new CardViewModel(pairId, imagePath));
            }

            ShuffleCards();
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

        private void FlipCard(CardViewModel card)
        {
            if (!CanInteract)
                return;

            if (card.IsMatched)
                return;

            if (card.IsFaceUp)
                return;

            if (_firstSelectedCard != null && _secondSelectedCard != null)
            {
                ResetSelectedCards();
            }

            if (_firstSelectedCard == null)
            {
                card.IsFaceUp = true;
                _firstSelectedCard = card;
            }
            else if (_secondSelectedCard == null && _firstSelectedCard != card)
            {
                card.IsFaceUp = true;
                _secondSelectedCard = card;
                CheckMatch();
            }
        }

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