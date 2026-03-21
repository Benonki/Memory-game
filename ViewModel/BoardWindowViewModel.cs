using Memory_game.MVVM;
using System.Collections.ObjectModel;

namespace Memory_game.ViewModel
{
    public class BoardWindowViewModel : ViewModelBase
    {
        int row = 3;
        int column = 3;

        public ObservableCollection<CardViewModel> Cards { get; set; } = new ObservableCollection<CardViewModel>();

        public RelayCommand FlipCardCommand => new RelayCommand(execute => FlipCard((CardViewModel)execute), canExecute => true);

        public BoardWindowViewModel()
        {
            for (int i = 0; i < Rows * Columns; i++)
            {
                Cards.Add(new CardViewModel(i.ToString()));
            }
        }

        public int Rows
        {
            get => row;
            set
            {
                row = value;
                OnPropertyChanged();
            }
        }

        public int Columns
        {
            get => column;
            set
            {
                column = value;
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
