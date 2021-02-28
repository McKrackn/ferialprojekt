using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Views;
using MultiVm.ViewModel;

namespace ProFer.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<INavigationService>(() => new DefaultNavigationService());
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<OverviewVm>();
            SimpleIoc.Default.Register<RankingViewModel>();
        }
        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();
        public OverviewVm InsertSurveyRecord => SimpleIoc.Default.GetInstance<OverviewVm>();
        public RankingViewModel ShowCurrentState => SimpleIoc.Default.GetInstance<RankingViewModel>();
    }
}
