using Memory_game.Model;
using Memory_game.MVVM;

namespace Memory_game.ViewModel
{
    public class CardViewModel : ViewModelBase
    {
        private Card card;

        public CardViewModel(string pattern)
        {
            card = new Card { content = pattern };
        }

        public string Content
        { 
            get => card.content; 
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
    }
}
