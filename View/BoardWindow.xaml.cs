using Memory_game.Model.Services;
using Memory_game.ViewModel;
using System.Windows;

namespace Memory_game.View
{
    public partial class BoardWindow : Window
    {
        public BoardWindow(int rows, int columns, string deckName, ICardDeckService deckService)
        {
            InitializeComponent();
            BoardWindowViewModel viewModel = new BoardWindowViewModel(rows, columns, deckName, deckService);
            DataContext = viewModel;
        }
    }
}