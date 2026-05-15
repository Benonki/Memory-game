using Memory_game.MVVM;

namespace Memory_game.ViewModel
{
    public class PlayerScoreViewModel : ViewModelBase
    {
        private int _score;

        public PlayerScoreViewModel(string displayName, bool isCurrentPlayer, int score)
        {
            PlayerName = displayName;
            IsCurrentPlayer = isCurrentPlayer;
            _score = score;
        }

        public string PlayerName { get; }

        public bool IsCurrentPlayer { get; }

        public string DisplayName => IsCurrentPlayer ? $"{PlayerName} (Ty)" : PlayerName;

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                OnPropertyChanged();
            }
        }
    }
}
