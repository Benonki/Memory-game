using Memory_game.Model;
using Memory_game.MVVM;
using Memory_game_shared.Models;

namespace Memory_game.ViewModel
{
    public class CardViewModel : ViewModelBase
    {
        private Card card;

        public CardViewModel(int pairId, string imagePath)
        {
            card = new Card{pairId = pairId, imagePath = imagePath};
        }

        public int PairId
        {
            get => card.pairId;
        }

        public string ImagePath
        {
            get => card.imagePath;
        }

        public bool IsFaceUp
        {
            get => card.isFaceUp;
            set
            {
                card.isFaceUp = value;
                OnPropertyChanged();
            }
        }

        public bool IsMatched
        {
            get => card.isMatched;
            set
            {
                card.isMatched = value;
                OnPropertyChanged();
                if (value)
                {
                    IsFaceUp = true;
                }
            }
        }
    }
}