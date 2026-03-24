using Memory_game.Model.Services;
using Memory_game.ViewModel;
using System.Windows;

namespace Memory_game.View
{
    public partial class CardDeckWindow : Window
    {
        public CardDeckWindow(INavigationService navigationService, ICardDeckService deckService)
        {
            InitializeComponent();
            CardDeckViewModel viewModel = new CardDeckViewModel(navigationService, deckService);
            DataContext = viewModel;
            Owner = Application.Current.MainWindow;
        }
    }
}