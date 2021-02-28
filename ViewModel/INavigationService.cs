using GalaSoft.MvvmLight;

namespace ProFer.ViewModel
{
    public interface INavigationService
    {
        string CurrentViewModelName { get; }
        ViewModelBase GetViewModel(string viewModelName);
    }
}
