using ProFer.Communication;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
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

        public ObservableCollection<Player> PlayerList { get; set; } = new ObservableCollection<Player>();
        #endregion
        
        #region gui commands/params

        public int TurnNumber { get; private set; } = 1;
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; ActAsClientBtnCommand.RaiseCanExecuteChanged(); ActAsServerBtnCommand.RaiseCanExecuteChanged();}
        }
        private Player _self;
        public Player Self
        {
            get => _self;
            set => _self = value;
        }
        private Player _selectedUser;
        public Player SelectedUser
        {
            get => _selectedUser;
            set { Set(ref _selectedUser, value); DropClientBtnCommand.RaiseCanExecuteChanged();}
        }
        private string _gameControlVisibility = "Visible";
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
        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>() { "---   Meskalero PokerDice   ---" };

        private Roll[] _actRoll = new Roll[5];
        public Roll[] ActRoll
        {
            get => _actRoll;
            set => Set(ref _actRoll, value);
        }
        private ObservableCollection<Roll> _selectedRoll = new ObservableCollection<Roll>();
        public ObservableCollection<Roll> SelectableRoll
        {
            get => new ObservableCollection<Roll>(ActRoll.ToList());
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
            //initialize dices with aces
            for (int i = 0; i < ActRoll.Length; i++)
            {
                ActRoll[i] = new Roll(6);
                ActRoll[i].DiceImage.Freeze();
            }

            ActAsServerBtnCommand = new RelayCommand(
                () =>
                {
                    com = new Com(true, GUIAction);
                    this.isConnected = true;
                    this.isServer = true;

                    Self = new Player(Name);
                    PlayerList.Add(Self);

                    Player TestPlayer = new Player("TestPlayer");
                    PlayerList.Add(TestPlayer);

                    ActAsClientBtnCommand.RaiseCanExecuteChanged();
                    StartGameCommand.RaiseCanExecuteChanged();
                    ActAsServerBtnCommand.RaiseCanExecuteChanged();
                    DropClientBtnCommand.RaiseCanExecuteChanged();
                    NameVisibility = "false";

                    Messages.Insert(0,"Welcome Player " + Name + " (Server). You can start the game whenever you wish.");
                    Task.Factory.StartNew(RotateDice);

                }, () => !this.isConnected && !string.IsNullOrWhiteSpace(_name));

            ActAsClientBtnCommand = new RelayCommand(
                () =>
                {
                    try
                    {
                        com = new Com(false, GUIAction);
                        this.isConnected = true;

                        Self = new Player(Name);
                        PlayerList.Add(Self);

                        ActAsClientBtnCommand.RaiseCanExecuteChanged();
                        ActAsServerBtnCommand.RaiseCanExecuteChanged();
                        NameVisibility = "false";
                        Messages.Insert(0,"Welcome Player " + Name + ". Please wait until the host starts the game..");
                        Task.Factory.StartNew(RotateDice);
                    }
                    catch (SocketException e)
                    {
                        Messages.Add("The computer at " + e.Message.Substring(e.Message.LastIndexOf('[')) + " said no");
                    }
                }, () => !this.isConnected && !string.IsNullOrWhiteSpace(_name));

            StartGameCommand = new RelayCommand(
                () =>
                {
                    gameStarted = true;
                    StartGameCommand.RaiseCanExecuteChanged();
                    Messages.Insert(0,"The game has started, rien ne vas plus!");
                    string message = "";
                    com.Send(message);
                    GUIAction(message);

                },
                () => isServer && !gameStarted);

            DropClientBtnCommand = new RelayCommand(() =>
                {
                    com.DisconnectSpecificClient(SelectedUser.Name);
                    PlayerList.Remove(SelectedUser); 
                },
                () => { return (SelectedUser != null && SelectedUser.Name != Self.Name && isServer); });

            RollCommand = new RelayCommand(() =>
                {
                    Messages.Insert(0,Self.Name + " is rolling the dice...");
                    //Task.Factory.StartNew(Shuffle);
                    Messages.Insert(0,"make your turn, " + Self.Name);
                    for (int i = 0; i < ActRoll.Length; i++)
                    {
                        ActRoll[i].Cleanup();
                        ActRoll[i] = new Roll();
                        ActRoll[i].DiceImage.Freeze();
                        ActRoll[i].RaisePropertyChanged();

                    }
                    RaisePropertyChanged("");
                    RaisePropertyChanged("SelectableRoll");
                },
                () => { return (GameControlVisibility == "Visible"); });
        }

        private void RotateDice()
        {
            while (!gameStarted)
            {
                for (int i = 0; i < ActRoll.Length; i++)
                {
                    ActRoll[i] = new Roll();
                    ActRoll[i].DiceImage.Freeze();
                }
                RaisePropertyChanged("ActRoll");
                Thread.Sleep(1500);
            }
        }
        private void Shuffle()
        {
            for (int i = 0; i < 30; i++)
            {
                int randNo = new Random().Next(0, 5);
                ActRoll[randNo] = new Roll();
                ActRoll[randNo].DiceImage.Freeze();
                RaisePropertyChanged("ActRoll");
            }
            Thread.Sleep(50);
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
