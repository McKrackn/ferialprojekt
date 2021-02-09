using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace ProFer.Model
{
    public class Player : ViewModelBase
    {
        private string _name;
        private ObservableCollection<int> _rolls;

        public int Neuner;
        public int Zehner;
        public int Buben;
        public int Damen;
        public int Koenige;
        public int Asse;
        public int Strasse;
        public int Full;
        public int Poker;
        public int Grande;

        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(); }
        }
        public ObservableCollection<int> Rolls
        {
            get { return _rolls; }
            set { _rolls = value; RaisePropertyChanged(); }
        }

        public Player(string Name)
        {
            this.Name = Name;
            Rolls=new ObservableCollection<int>();
        }

        public override string ToString()
        {
            return Name + "     " + (Neuner+Zehner+Buben+Damen+Koenige+Asse);
        }
    }
}
