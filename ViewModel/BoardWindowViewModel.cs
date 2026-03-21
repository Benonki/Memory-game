using Memory_game.MVVM;
using System.Collections.ObjectModel;

namespace Memory_game.ViewModel
{
    public class BoardWindowViewModel : ViewModelBase
    {
        private int _rows;
        private int _columns;

        public ObservableCollection<CardViewModel> Cards { get; set; } = new ObservableCollection<CardViewModel>();

        public RelayCommand FlipCardCommand => new RelayCommand(execute => FlipCard((CardViewModel)execute), canExecute => true);

        public BoardWindowViewModel(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;

            InitializeCards();
        }

        private void InitializeCards()
        {
            int totalCards = Rows * Columns;
            for (int i = 0; i < totalCards / 2; i++)
            {
                string content = (i + 1).ToString();
                Cards.Add(new CardViewModel(content));
                Cards.Add(new CardViewModel(content));
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

        private void FlipCard(CardViewModel card)
        {
            if (card.IsFaceUp)
                card.IsFaceUp = false;
            else
                card.IsFaceUp = true;
        }
    }
}