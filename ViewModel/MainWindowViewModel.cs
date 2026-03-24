using Memory_game.Model.Services;
using Memory_game.MVVM;

namespace Memory_game.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;

        public RelayCommand OpenBoardCommand => new RelayCommand(execute => navigationService.OpenBoardSetup(), canExecute => true);

        public RelayCommand OpenCardDeckWindowCommand => new RelayCommand(execute => navigationService.OpenCardDeckWindow(), canExecute => true);

        public MainWindowViewModel(INavigationService navigation)
        {
            navigationService = navigation;
        }
    }
}