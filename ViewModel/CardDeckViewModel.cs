using Memory_game.MVVM;
using Memory_game.Model.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using Memory_game.View;
using System.Windows;

namespace Memory_game.ViewModel
{
    public class CardDeckViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ICardDeckService _deckService;
        private string _newDeckName = string.Empty;
        private string _selectedDeck = string.Empty;
        private string _errorMessage = string.Empty;
        private ObservableCollection<string> _availableDecks;
        private INavigationService navigationService;

        public RelayCommand CreateDeckCommand => new RelayCommand(execute => CreateDeck(), canExecute => true);
        public RelayCommand DeleteDeckCommand => new RelayCommand(execute => DeleteDeck(), canExecute => !string.IsNullOrEmpty(SelectedDeck));
        public RelayCommand AddCardsCommand => new RelayCommand(execute => AddCards(), canExecute => !string.IsNullOrEmpty(SelectedDeck));
        public RelayCommand CancelCommand => new RelayCommand(execute => Cancel(), canExecute => true);

        public CardDeckViewModel(INavigationService navigationService, ICardDeckService deckService)
        {
            _navigationService = navigationService;
            _deckService = deckService;
            _availableDecks = new ObservableCollection<string>(_deckService.GetAllDecks());
            _selectedDeck = _navigationService.SelectedDeck;
        }

        public CardDeckViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public ObservableCollection<string> AvailableDecks
        {
            get => _availableDecks;
            set
            {
                _availableDecks = value;
                OnPropertyChanged();
            }
        }

        public string SelectedDeck
        {
            get => _selectedDeck;
            set
            {
                _selectedDeck = value;
                _navigationService.SelectedDeck = value;
                OnPropertyChanged();
            }
        }

        public string NewDeckName
        {
            get => _newDeckName;
            set
            {
                _newDeckName = value;
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

        private void CreateDeck()
        {
            if (string.IsNullOrWhiteSpace(NewDeckName))
            {
                ErrorMessage = "Podaj nazwę talii.";
                return;
            }

            if (AvailableDecks.Contains(NewDeckName))
            {
                ErrorMessage = "Talia o tej nazwie już istnieje.";
                return;
            }

            _deckService.CreateDeck(NewDeckName, Array.Empty<string>());
            AvailableDecks.Add(NewDeckName);
            NewDeckName = string.Empty;
            ErrorMessage = string.Empty;
        }

        private void DeleteDeck()
        {
            if (string.IsNullOrEmpty(SelectedDeck))
            {
                ErrorMessage = "Wybierz talię do usunięcia.";
                return;
            }

            if(string.Equals(SelectedDeck, "Default")){
                ErrorMessage = "Nie można usunąć domyślnej talii.";
                return;
            }

            _deckService.DeleteDeck(SelectedDeck);
            AvailableDecks.Remove(SelectedDeck);
            SelectedDeck = string.Empty;
        }

        private void AddCards()
        {
            if (string.IsNullOrEmpty(SelectedDeck))
                return;

            var dialog = new OpenFileDialog
            {
                Filter = "Obrazy (*.png)|*.png",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                _deckService.AddCardsToDeck(SelectedDeck, dialog.FileNames);
                ErrorMessage = string.Empty;
            }
        }

        private void Cancel()
        {
            Application.Current.Windows.OfType<CardDeckWindow>().FirstOrDefault()?.Close();
        }
    }
}