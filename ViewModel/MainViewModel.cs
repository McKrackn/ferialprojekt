using ProFer.Communication;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ProFer.Model;

namespace ProFer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region globals
        private Com com;
        private bool isConnected = false;
        private bool isServer = false;
        private bool gameStarted = false;

        public ObservableCollection<int> ActRoll { get; set; } = new ObservableCollection<int>(){6,6,6,6,6};
        public ObservableCollection<Player> PlayerList { get; set; } = new ObservableCollection<Player>();
        #endregion
        
        #region dice images

        private BitmapImage _diceImage1;
        public BitmapImage DiceImage1
        {
            get
            {return _diceImage1;
            }
            set
            {
                _diceImage1 = value; RaisePropertyChanged();
            }
        }
        private BitmapImage _diceImage2;
        public BitmapImage DiceImage2
        {
            get
            {
                return _diceImage2;
            }
            set
            {
                _diceImage2 = value; RaisePropertyChanged();
            }
        }
        private BitmapImage _diceImage3;
        public BitmapImage DiceImage3
        {
            get
            {
                return _diceImage3;
            }
            set
            {
                _diceImage3 = value; RaisePropertyChanged();
            }
        }
        private BitmapImage _diceImage4;
        public BitmapImage DiceImage4
        {
            get
            {
                return _diceImage4;
            }
            set
            {
                _diceImage4 = value; RaisePropertyChanged();
            }
        }
        private BitmapImage _diceImage5;
        public BitmapImage DiceImage5
        {
            get
            {
                return _diceImage5;
            }
            set
            {
                _diceImage5 = value; RaisePropertyChanged();
            }
        }
        #endregion

        #region gui commands/params
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; ActAsClientBtnCommand.RaiseCanExecuteChanged(); ActAsServerBtnCommand.RaiseCanExecuteChanged();}
        }
        public string SelectedUser { get; set; }
        private string _gameControlVisibility = "Hidden";
        public string GameControlVisibility
        {
            get => _gameControlVisibility;
            set { _gameControlVisibility = value; RaisePropertyChanged(); }
        }
        private string _nameVisibility = "True";
        public string NameVisibility
        {
            get => _nameVisibility;
            set { _nameVisibility = value; RaisePropertyChanged(); }
        }
        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>() { "---   Game started   ---" };

        private int[,] _scores;
        public int[,] Scores
        {
            get => _scores;
            set { _scores = value; }
        }
        #endregion

        #region RelayCommands
        public RelayCommand ActAsClientBtnCommand { get; set; }
        public RelayCommand StartGameCommand { get; set; }
        public RelayCommand RollCommand { get; set; }
        public RelayCommand ActAsServerBtnCommand { get; set; }
        public RelayCommand DropClientBtnCommand { get; set; }
        #endregion

        public MainViewModel()
        {
            ActAsServerBtnCommand = new RelayCommand(
                () =>
                {
                    this.isConnected = true;
                    this.isServer = true;
                    com = new Com(true, GUIAction);

                    ActAsClientBtnCommand.RaiseCanExecuteChanged();
                    StartGameCommand.RaiseCanExecuteChanged();
                    ActAsServerBtnCommand.RaiseCanExecuteChanged();
                    DropClientBtnCommand.RaiseCanExecuteChanged();
                    NameVisibility = "false";

                    Player self = new Player(Name);
                    PlayerList.Add(self);
                    Messages.Insert(0,"Welcome Player " + Name + " (Server). You can start the game whenever you wish.");
                    Task.Factory.StartNew(RotateDice);

                }, () => !this.isConnected && !string.IsNullOrWhiteSpace(_name));

            ActAsClientBtnCommand = new RelayCommand(
                () =>
                {
                    this.isConnected = true;
                    com = new Com(false, GUIAction); 

                    ActAsClientBtnCommand.RaiseCanExecuteChanged();
                    ActAsServerBtnCommand.RaiseCanExecuteChanged();
                    NameVisibility = "false";

                    Messages.Insert(0,"Welcome Player " + Name + ". Please wait until the host starts the game..");
                    Task.Factory.StartNew(RotateDice);

                }, () => !this.isConnected && !string.IsNullOrWhiteSpace(_name));

            StartGameCommand = new RelayCommand(
                () =>
                {
                    gameStarted = true;
                    StartGameCommand.RaiseCanExecuteChanged();
                    Scores = new int[10, 1];
                    Messages.Insert(0,"The game has started, rien ne vas plus!");
                    string message = "";
                    com.Send(message);
                    GUIAction(message);

                },
                () => isServer && !gameStarted);

            DropClientBtnCommand = new RelayCommand(() =>
                {
                    com.DisconnectSpecificClient(SelectedUser);
                    //PlayerList.Remove(SelectedUser); 
                },
                () => { return (SelectedUser != null && SelectedUser != Name && isServer); });

            RollCommand = new RelayCommand(() =>
                {
                    Messages.Insert(0,"...rolling...");
                    //PlayerList.Remove(SelectedUser); 
                },
                () => { return (GameControlVisibility == "visible"); });
        }

        private void RotateDice()
        {
            Thread.Sleep(1500);
            while (true)
            {
                ActRoll.RemoveAt(4);
                ActRoll.Insert(0, new Random().Next(1, 7));
                DiceImage1=new BitmapImage(new Uri($"..\\..\\..\\img\\{ActRoll[0]}.png", UriKind.Relative));
                DiceImage1.Freeze();
                DiceImage2 = new BitmapImage(new Uri($"..\\..\\..\\img\\{ActRoll[1]}.png", UriKind.Relative));
                DiceImage2.Freeze();
                DiceImage3 = new BitmapImage(new Uri($"..\\..\\..\\img\\{ActRoll[2]}.png", UriKind.Relative));
                DiceImage3.Freeze();
                DiceImage4 = new BitmapImage(new Uri($"..\\..\\..\\img\\{ActRoll[3]}.png", UriKind.Relative));
                DiceImage4.Freeze();
                DiceImage5 = new BitmapImage(new Uri($"..\\..\\..\\img\\{ActRoll[4]}.png", UriKind.Relative));
                DiceImage5.Freeze();
                Thread.Sleep(1500);
            }
        }

        private void GUIAction(string message)
        {
            //
            // This method handles the encoding of the recieved message 
            // into an instance of the "Player" class
            // and adds it into the ObservableCollection being displayed 
            // on the GUI´s DataGrid
            //

            App.Current.Dispatcher.Invoke(
                () =>
                {
                    //split the recieved message into 4 different strings
                    // example of string: "A1-123|Dispenser|20.21|Enigne"
                    string[] splitted = message.Split("|");

                });
        }
    }
}
