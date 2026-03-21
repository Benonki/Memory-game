using Memory_game.ViewModel;
using System.Windows;

namespace Memory_game.View
{
    /// <summary>
    /// Interaction logic for BoardWindow.xaml
    /// </summary>
    public partial class BoardWindow : Window
    {
        public BoardWindow()
        {
            InitializeComponent();
            BoardWindowViewModel viewModel = new BoardWindowViewModel();
            DataContext = viewModel;
        }
    }
}
