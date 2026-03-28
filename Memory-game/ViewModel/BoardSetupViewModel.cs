using Memory_game.MVVM;
using Memory_game.Model.Services;
using System.Windows;

namespace Memory_game.ViewModel
{
    public class BoardSetupViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private string _rows = "4";
        private string _columns = "4";
        private string _errorMessage = string.Empty;
        private readonly ICardDeckService _deckService;
        private string _selectedDeck = "Default";

        public BoardSetupViewModel(INavigationService navigationService, ICardDeckService deckService)
        {
            _navigationService = navigationService;
            _deckService = deckService;
            _selectedDeck = _navigationService.SelectedDeck;
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

        public RelayCommand StartCommand => new RelayCommand(execute => Start(), canExecute => true);

        public RelayCommand CancelCommand => new RelayCommand(execute => Cancel(), canExecute => true);

        private void Start()
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

                ErrorMessage = string.Empty;

                _navigationService.SelectedDeck = SelectedDeck;
                _navigationService.OpenBoard(rows, columns, SelectedDeck);
            }
            else
            {
                ErrorMessage = "Wprowadź poprawne liczby całkowite";
            }
        }

        private void Cancel()
        {
            _navigationService.OpenMainWindow();
        }
    }
}