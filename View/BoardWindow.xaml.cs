using Memory_game.ViewModel;
using System.Windows;

namespace Memory_game.View
{
    public partial class BoardWindow : Window
    {
        public BoardWindow(int rows, int columns)
        {
            InitializeComponent();
            BoardWindowViewModel viewModel = new BoardWindowViewModel(rows, columns);
            DataContext = viewModel;
        }
    }
}