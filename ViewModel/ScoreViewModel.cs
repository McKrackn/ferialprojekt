using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using ProFer.Model;

namespace MultiVm.ViewModel
{
    public class OverviewVm : ViewModelBase
    {
        private Messenger messenger = SimpleIoc.Default.GetInstance<Messenger>();

        public ObservableCollection<Roll> Items { get; set; }
     
        private RelayCommand<Roll> buyBtnClickedCmd;

        public RelayCommand<Roll> BuyBtnClickedCmd
        {
            get
            {
                return buyBtnClickedCmd;
            }

            set
            {
                buyBtnClickedCmd = value; RaisePropertyChanged();
            }
        }

        public OverviewVm()
        {
            BuyBtnClickedCmd = new RelayCommand<Roll>((p) =>
            {
                messenger.Send<PropertyChangedMessage<Roll>>(new PropertyChangedMessage<Roll>(null, p, "AddNew"), "Write");
                
                //send info to message bar
                //messenger.Send<PropertyChangedMessage<Message>>(new PropertyChangedMessage<Message>(null, new Message("New Entry Added", MessageState.Info), ""), "@Message");
                
            }, (p) => { return true; });
            Items = new ObservableCollection<Roll>();
            GenerateDemoData();

        }



        private void GenerateDemoData()
        {
            /*
            Items.Add(new Roll("Lego", new BitmapImage(new Uri("../Images/lego1.jpg", UriKind.Relative)), "-"));
            Items.Add(new Roll("Playmobil", new BitmapImage(new Uri("../Images/playmobil1.jpg", UriKind.Relative)), "-"));
            Items[Items.Count - 1].AddItem(
                new Roll("Playmobil 2", new BitmapImage(new Uri("../Images/playmobil2.jpg", UriKind.Relative)), "5+"));
            Items[Items.Count - 1].AddItem(
                new Roll("Playmobil 3", new BitmapImage(new Uri("../Images/playmobil3.jpg", UriKind.Relative)), "10+"));
            Items[Items.Count - 1].AddItem(
               new Roll("Playmobil 2", new BitmapImage(new Uri("../Images/playmobil2.jpg", UriKind.Relative)), "5+"));
            Items[Items.Count - 1].AddItem(
                new Roll("Playmobil 3", new BitmapImage(new Uri("../Images/playmobil3.jpg", UriKind.Relative)), "10+"));
            Items[Items.Count - 1].AddItem(
               new Roll("Playmobil 2", new BitmapImage(new Uri("../Images/playmobil2.jpg", UriKind.Relative)), "5+"));
            Items[Items.Count - 1].AddItem(
                new Roll("Playmobil 3", new BitmapImage(new Uri("../Images/playmobil3.jpg", UriKind.Relative)), "10+"));
            Items[Items.Count - 1].AddItem(
               new Roll("Playmobil 2", new BitmapImage(new Uri("../Images/playmobil2.jpg", UriKind.Relative)), "5+"));
            Items[Items.Count - 1].AddItem(
                new Roll("Playmobil 3", new BitmapImage(new Uri("../Images/playmobil3.jpg", UriKind.Relative)), "10+"));
            Items[Items.Count - 1].AddItem(
               new Roll("Playmobil 2", new BitmapImage(new Uri("../Images/playmobil2.jpg", UriKind.Relative)), "5+"));
            Items[Items.Count - 1].AddItem(
                new Roll("Playmobil 3", new BitmapImage(new Uri("../Images/playmobil3.jpg", UriKind.Relative)), "10+"));
            */
        }

    }
}
