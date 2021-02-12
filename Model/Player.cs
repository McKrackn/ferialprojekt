using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace ProFer.Model
{
    public class Player : ViewModelBase
    {
        private string _name;

        private int _neuner;
        private int _zehner;
        private int _buben;
        private int _damen;
        private int _koenige;
        private int _asse;
        private int _strasse;
        private int _full;
        private int _poker;
        private int _grande;

        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(); }
        }

        public int Neuner
        {
            get { return _neuner; }
            set { _neuner = value; RaisePropertyChanged(); }
        }

        public int Zehner
        {
            get { return _zehner; }
            set { _zehner = value; RaisePropertyChanged(); }
        }

        public int Buben
        {
            get { return _buben; }
            set { _buben = value; RaisePropertyChanged(); }
        }

        public int Damen
        {
            get { return _damen; }
            set { _damen = value; RaisePropertyChanged(); }
        }

        public int Koenige
        {
            get { return _koenige; }
            set { _koenige = value; RaisePropertyChanged(); }
        }

        public int Asse
        {
            get { return _asse; }
            set { _asse = value; RaisePropertyChanged(); }
        }

        public int Strasse
        {
            get { return _strasse; }
            set { _strasse = value; RaisePropertyChanged(); }
        }

        public int Full
        {
            get { return _full; }
            set { _full = value; RaisePropertyChanged(); }
        }

        public int Poker
        {
            get { return _poker; }
            set { _poker = value; RaisePropertyChanged(); }
        }

        public int Grande
        {
            get { return _grande; }
            set { _grande = value; RaisePropertyChanged(); }
        }

        public Player(string Name)
        {
            this.Name = Name;
        }
        public int GetSum()
        {
            return _neuner + _zehner + _buben + _damen + _koenige + _asse + _strasse + _full + _poker + _grande;

        }
        public override string ToString()
        {
            return Name + "     " + GetSum();
        }
    }
}
