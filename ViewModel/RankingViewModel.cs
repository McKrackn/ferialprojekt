using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

using ProFer.Model;

namespace ProFer.ViewModel
{
    public class RankingViewModel : ViewModelBase
    {
        private Messenger messenger = SimpleIoc.Default.GetInstance<Messenger>();


           
        public RankingViewModel()
        {
            //Toys = new ObservableCollection<Roll>();
            //messenger.Register<PropertyChangedMessage<Roll>>(this, "Write", update);
        }

        private void update(PropertyChangedMessage<Roll> obj)
        
        {
            //Add(obj.NewValue);
        }

    }
}
