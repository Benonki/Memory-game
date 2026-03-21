using Memory_game.Model.Services;
using Memory_game.MVVM;
using Memory_game.View;

namespace Memory_game.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        private readonly INavigationService navigationService;

        public RelayCommand OpenBoardCommand => new RelayCommand(execute => navigationService.OpenBoard(), canExecute => true);

        public MainWindowViewModel(INavigationService navigation)
        {
            navigationService = navigation;
        }

    }
}
