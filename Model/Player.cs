using GalaSoft.MvvmLight;

namespace DicePokerMQ.Model
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

        public int SumRolls
        {
            //_grande = RollNumber == 2 ? 100 : 50;

            get { return (Neuner == -1 ? 0 : Neuner) + (Zehner == -2 ? 0 : Zehner) + (Buben == -3 ? 0 : Buben) + (Damen == -4 ? 0 : Damen) + (Koenige == -5 ? 0 : Koenige) + (Asse == -6 ? 0 : Asse) + (Strasse == -1 ? 0 : Strasse) + (Full == -1 ? 0 : Full) + (Poker == -1 ? 0 : Poker) + (Grande == -1 ? 0 : Grande); }
        }

        public int Neuner
        {
            get { return _neuner; }
            set { _neuner = value; RaisePropertyChanged("SumRolls"); RaisePropertyChanged();}
        }

        public int Zehner
        {
            get { return _zehner; }
            set { _zehner = value; RaisePropertyChanged("SumRolls"); RaisePropertyChanged(); }
        }

        public int Buben
        {
            get { return _buben; }
            set { _buben = value; RaisePropertyChanged("SumRolls"); RaisePropertyChanged(); }
        }

        public int Damen
        {
            get { return _damen; }
            set { _damen = value; RaisePropertyChanged("SumRolls"); RaisePropertyChanged(); }
        }

        public int Koenige
        {
            get { return _koenige; }
            set { _koenige = value; RaisePropertyChanged("SumRolls"); RaisePropertyChanged(); }
        }

        public int Asse
        {
            get { return _asse; }
            set { _asse = value; RaisePropertyChanged("SumRolls"); RaisePropertyChanged(); }
        }

        public int Strasse
        {
            get { return _strasse; }
            set { _strasse = value; RaisePropertyChanged("SumRolls"); RaisePropertyChanged(); }
        }

        public int Full
        {
            get { return _full; }
            set { _full = value; RaisePropertyChanged("SumRolls"); RaisePropertyChanged(); }
        }

        public int Poker
        {
            get { return _poker; }
            set { _poker = value; RaisePropertyChanged("SumRolls"); RaisePropertyChanged(); }
        }

        public int Grande
        {
            get { return _grande; }
            set { _grande = value; RaisePropertyChanged("SumRolls"); RaisePropertyChanged(); }
        }

        public Player(string Name)
        {
            this.Name = Name;
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
}
