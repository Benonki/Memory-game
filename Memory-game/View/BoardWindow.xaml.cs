using Memory_game.Model.Services;
using Memory_game.ViewModel;
using Memory_game_shared.Models;
using System.Windows;

namespace Memory_game.View
{
    public partial class BoardWindow : Window
    {
        public BoardWindow(GameState gameState, string deckName, ICardDeckService deckService)
        {
            InitializeComponent();
            BoardWindowViewModel viewModel = new BoardWindowViewModel(gameState, deckName, deckService);
            DataContext = viewModel;
        }
    }
}