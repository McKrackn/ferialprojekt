using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MultiVm.ViewModel;

namespace ProFer.ViewModel
{
    public class DefaultNavigationService : INavigationService
    {
        public string CurrentViewModelName { get; private set; }

        public ViewModelBase GetViewModel(string viewModelName)
        {
            CurrentViewModelName = viewModelName;
            if (CurrentViewModelName == "scoreView")
            {
                return SimpleIoc.Default.GetInstance<OverviewVm>();
            }
            return SimpleIoc.Default.GetInstance<RankingViewModel>();
        }
    }
}
