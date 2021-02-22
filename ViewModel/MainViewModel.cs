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
        public bool GameStarted
        {
            get => gameStarted;
            set => gameStarted = value;
        }

        public ObservableCollection<Player> PlayerList { get; set; } = new ObservableCollection<Player>();
        public int Neuner
        {
            get => _neuner;
            set { _neuner = value; TakeNinesBtnCommand.RaiseCanExecuteChanged(); }
        }
        public int Zehner
        {
            get => _zehner;
            set { _zehner = value; TakeTensBtnCommand.RaiseCanExecuteChanged(); }
        }

        public int Buben
        {
            get => _buben;
            set { _buben = value; TakeJacksBtnCommand.RaiseCanExecuteChanged(); }
        }

        public int Damen
        {
            get => _damen;
            set { _damen = value; TakeQueensBtnCommand.RaiseCanExecuteChanged(); }
        }

        public int Koenige
        {
            get => _koenige;
            set { _koenige = value; TakeKingsBtnCommand.RaiseCanExecuteChanged(); }
        }

        public int Asse
        {
            get => _asse;
            set { _asse = value; TakeStreetBtnCommand.RaiseCanExecuteChanged(); }
        }

        public bool Strasse
        {
            get => _strasse;
            set { _strasse = value;TakeStreetBtnCommand.RaiseCanExecuteChanged(); }
        }

        public bool Full
        {
            get => _full;
            set { _full = value; TakeFullBtnCommand.RaiseCanExecuteChanged(); }
        }

        public bool Poker
        {
            get => _poker;
            set { _poker = value; TakePokerBtnCommand.RaiseCanExecuteChanged(); }
        }

        public bool Grande
        {
            get => _grande;
            set { _grande = value; TakeGrandBtnCommand.RaiseCanExecuteChanged(); }
        }

        private int _neuner;
        private int _zehner;
        private int _buben;
        private int _damen;
        private int _koenige;
        private int _asse;
        private bool _strasse;
        private bool _full;
        private bool _poker;
        private bool _grande;
        #endregion

        #region gui commands/params

        public int RollNumber
        {
            get => _rollNumber;
            private set { _rollNumber = value; }//RollCommand.RaiseCanExecuteChanged(); }
        }

        private string _name = "";
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
        private Roll rollSelectedItem;
        private int _rollNumber = 1;
        private string _actPlayer;

        public Roll RollSelectedItem
        {
            get
            {
                return rollSelectedItem;
            }
            set
            {
                var selectedItems = SelectableRoll.Count(x => x.IsSelected);
                this.RaisePropertyChanged("RollSelectedItem");
            }
        }

        public string ActPlayer
        {
            get => _actPlayer;
            set { _actPlayer = value; RollCommand.RaiseCanExecuteChanged(); }
        }

        public bool RollFinished { get; set; } = false;
        #endregion

        #region RelayCommands
        public RelayCommand ActAsClientBtnCommand { get; set; }
        public RelayCommand StartGameCommand { get; set; }
        public RelayCommand RollCommand { get; set; }
        public RelayCommand ActAsServerBtnCommand { get; set; }
        public RelayCommand DropClientBtnCommand { get; set; }

        public RelayCommand TakeNinesBtnCommand { get; set; }
        public RelayCommand TakeTensBtnCommand { get; set; }
        public RelayCommand TakeJacksBtnCommand { get; set; }
        public RelayCommand TakeQueensBtnCommand { get; set; }
        public RelayCommand TakeKingsBtnCommand { get; set; }
        public RelayCommand TakeAcesBtnCommand { get; set; }
        public RelayCommand TakeStreetBtnCommand { get; set; }
        public RelayCommand TakeFullBtnCommand { get; set; }
        public RelayCommand TakePokerBtnCommand { get; set; }
        public RelayCommand TakeGrandBtnCommand { get; set; }

        #endregion

        public MainViewModel()
        {
            //initialize dices with aces
            for (int i = 0; i < ActRoll.Length; i++)
            {
                ActRoll[i] = new Roll(6);
                ActRoll[i].DiceImage.Freeze();
                ActRoll[i].IsSelected = false;
            }

            TakeNinesBtnCommand = new RelayCommand(
                () =>
                {
                    RollNumber = 4;
                    com.Send("nt:" + Self.Name + ":" + _neuner);
                    RollFinished = true;
                    Messages.Insert(0,"you took " + _neuner + " nines, making " + _neuner*1 + " points");
                    PlayerList[0].Neuner = _neuner * 1;
                    RollCommand.RaiseCanExecuteChanged();
                    TakeNinesBtnCommand.RaiseCanExecuteChanged();
                },
                () => RollNumber>1 && Neuner>0 && PlayerList[0].Neuner==0);
            TakeTensBtnCommand = new RelayCommand(
                () =>
                {
                    RollNumber = 3;
                    com.Send("tt:" + Self.Name + ":" + _zehner);
                    Messages.Insert(0, "you took " + _zehner + " tens, making " + _zehner * 2 + " points");
                    PlayerList[0].Zehner = _zehner * 2;
                    RollCommand.RaiseCanExecuteChanged();
                    TakeTensBtnCommand.RaiseCanExecuteChanged();
                },
                () => RollNumber > 1 && Zehner > 0 && PlayerList[0].Zehner==0);
            TakeJacksBtnCommand = new RelayCommand(
                () =>
                {
                    RollNumber = 3;
                    com.Send("jt:" + Self.Name + ":" + _buben);
                    Messages.Insert(0, "you took " + _buben + " jacks, making " + _buben * 3 + " points");
                    PlayerList[0].Buben = _buben * 3;
                    RollCommand.RaiseCanExecuteChanged();
                    TakeJacksBtnCommand.RaiseCanExecuteChanged();
                },
                () => RollNumber > 1 && Buben > 0 && PlayerList[0].Buben==0);
            TakeQueensBtnCommand = new RelayCommand(
                () =>
                {
                    RollNumber = 3;
                    com.Send("qt:" + Self.Name + ":" + _damen);
                    Messages.Insert(0, "you took " + _damen + " queens, making " + _damen * 4 + " points");
                    PlayerList[0].Damen = _damen * 4;
                    RollCommand.RaiseCanExecuteChanged();
                    TakeQueensBtnCommand.RaiseCanExecuteChanged();
                },
                () => RollNumber > 1 && Damen > 0 && PlayerList[0].Damen==0);
            TakeKingsBtnCommand = new RelayCommand(
                () =>
                {
                    RollNumber = 3;
                    com.Send("kt:" + Self.Name + ":" + _koenige);
                    Messages.Insert(0, "you took " + _koenige + " kings, making " + _koenige * 5 + " points");
                    PlayerList[0].Koenige = _koenige * 5;
                    RollCommand.RaiseCanExecuteChanged();
                    TakeKingsBtnCommand.RaiseCanExecuteChanged();
                },
                () => RollNumber > 1 && Koenige > 0 && PlayerList[0].Koenige==0);
            TakeAcesBtnCommand = new RelayCommand(
                () =>
                {
                    RollNumber = 3;
                    com.Send("at:" + Self.Name + ":" + _asse);
                    Messages.Insert(0, "you took " + _asse + " nines, making " + _asse * 6 + " points");
                    PlayerList[0].Asse = _asse * 6;
                    RollCommand.RaiseCanExecuteChanged();
                    TakeAcesBtnCommand.RaiseCanExecuteChanged();
                },
                () => RollNumber > 1 && Asse > 0 && PlayerList[0].Asse==0);
            TakeStreetBtnCommand = new RelayCommand(
                () =>
                {
                    RollNumber = 3;
                    com.Send("nt:" + Self.Name + ":" + _neuner);
                    Messages.Insert(0, "you took " + _neuner + " nines, making " + _neuner * 1 + " points");
                    PlayerList[0].Neuner = _neuner * 1;
                    RollCommand.RaiseCanExecuteChanged();
                },
                () => RollNumber > 1 && Neuner > 0);
            TakeFullBtnCommand = new RelayCommand(
                () =>
                {
                    RollNumber = 3;
                    com.Send("nt:" + Self.Name + ":" + _neuner);
                    Messages.Insert(0, "you took " + _neuner + " nines, making " + _neuner * 1 + " points");
                    PlayerList[0].Neuner = _neuner * 1;
                    RollCommand.RaiseCanExecuteChanged();
                },
                () => RollNumber > 1 && Neuner > 0);
            TakePokerBtnCommand = new RelayCommand(
                () =>
                {
                    RollNumber = 3;
                    com.Send("nt:" + Self.Name + ":" + _neuner);
                    Messages.Insert(0, "you took " + _neuner + " nines, making " + _neuner * 1 + " points");
                    PlayerList[0].Neuner = _neuner * 1;
                    RollCommand.RaiseCanExecuteChanged();
                },
                () => RollNumber > 1 && Neuner > 0);
            TakeGrandBtnCommand = new RelayCommand(
                () =>
                {
                    RollNumber = 3;
                    com.Send("nt:" + Self.Name + ":" + _neuner);
                    Messages.Insert(0, "you took " + _neuner + " nines, making " + _neuner * 1 + " points");
                    PlayerList[0].Neuner = _neuner * 1;
                    RollCommand.RaiseCanExecuteChanged();
                },
                () => RollNumber > 1 && Neuner > 0);

            ActAsServerBtnCommand = new RelayCommand(
                () =>
                {
                    com = new Com(true, GUIAction, PlayerList);
                    this.isConnected = true;
                    this.isServer = true;

                    Self = new Player(Name);
                    PlayerList.Add(Self);

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
                        com = new Com(false, GUIAction, null);
                        this.isConnected = true;

                        Self = new Player(Name);
                        PlayerList.Add(Self);

                        ActAsClientBtnCommand.RaiseCanExecuteChanged();
                        ActAsServerBtnCommand.RaiseCanExecuteChanged();
                        NameVisibility = "false";
                        Messages.Insert(0,"Welcome Player " + Name + ". Please wait until the host starts the game..");
                        com.Send("np:" + Name);
                        Task.Factory.StartNew(RotateDice);
                    }
                    catch (SocketException e)
                    {
                        Messages.Insert(0,"The computer at " + e.Message.Substring(e.Message.LastIndexOf('[')) + " said no");
                    }
                }, () => !this.isConnected && !string.IsNullOrWhiteSpace(_name));

            StartGameCommand = new RelayCommand(
                () =>
                {
                    string message = "gs:" + PlayerList.Count;
                    com.Send(message);
                    GUIAction(message);
                    Task.Factory.StartNew(StartGame);
                },
                () => isServer && !GameStarted);

            DropClientBtnCommand = new RelayCommand(() =>
                {
                    com.DisconnectSpecificClient(SelectedUser.Name);
                    PlayerList.Remove(SelectedUser); 
                },
                () => { return (SelectedUser != null && SelectedUser.Name != Self.Name && isServer); });

            RollCommand = new RelayCommand(() =>
                {
                    com.Send("nr:"+Self.Name+":"+RollNumber);
                    Messages.Insert(0,"you are rolling the dice for the " + RollNumber + ". time...");
                    //Task.Factory.StartNew(Shuffle);
                    /*for (int i = 0; i < 30; i++)
                    {
                        int randNo = new Random().Next(0, 5);
                        if (!ActRoll[randNo].IsSelected)
                        {
                            ActRoll[randNo].Cleanup();
                            ActRoll[randNo] = new Roll();
                            ActRoll[randNo].DiceImage.Freeze();
                        }
                        Thread.Sleep(50);
                        RaisePropertyChanged("SelectableRoll");

                    }*/
                    for (int i = 0; i < ActRoll.Length; i++)
                    {
                        if (RollNumber!=1)
                        {
                            if (!ActRoll[i].IsSelected)
                            {
                                ActRoll[i] = new Roll();
                                ActRoll[i].DiceImage.Freeze();
                            }
                        }
                        else
                        {
                            ActRoll[i] = new Roll();
                            ActRoll[i].DiceImage.Freeze();
                        }
                    }
                    string sendSelected = "with ";
                    foreach (var actRoll in ActRoll)
                    {
                        if (actRoll.Value == 6 && actRoll.IsSelected)
                        {
                            sendSelected = sendSelected + "an " + actRoll.Description + ", ";
                        }
                        else if (actRoll.IsSelected)
                        {
                            sendSelected = sendSelected + "a " + actRoll.Description + ", ";
                        }
                    }
                    sendSelected = sendSelected + " selected";
                    RollNumber++;
                    RaisePropertyChanged("SelectableRoll");

                    string sendRoll = Name + " rolled ";
                    foreach (var actRoll in SelectableRoll)
                    {
                        if (actRoll.Value == 6)
                        {
                            sendRoll = sendRoll + "an " + actRoll.Description + ", ";
                        }
                        sendRoll = sendRoll + "a " + actRoll.Description + ", ";
                    }
                    com.Send(sendRoll);
                    com.Send(sendSelected);
                    if (RollNumber==3) RollCommand.RaiseCanExecuteChanged();
                    CalculateRoll();
                },
                () => { return (GameControlVisibility == "Visible" && RollNumber <= 3 && ActPlayer==Name); });
        }

        private void CalculateRoll()
        {
            _neuner = 0;
            foreach (var nineRoll in ActRoll)
            {
                if (nineRoll.Value == 1)
                {
                    _neuner++;
                }
            }
            TakeNinesBtnCommand.RaiseCanExecuteChanged();
            _zehner = 0;
            foreach (var tenRoll in ActRoll)
            {
                if (tenRoll.Value == 2)
                {
                    _zehner++;
                }
            }
            TakeTensBtnCommand.RaiseCanExecuteChanged();
            _buben = 0;
            foreach (var jackRoll in ActRoll)
            {
                if (jackRoll.Value == 3)
                {
                    _buben++;
                }
            }
            TakeJacksBtnCommand.RaiseCanExecuteChanged();
            _damen = 0;
            foreach (var queenRoll in ActRoll)
            {
                if (queenRoll.Value == 4)
                {
                    _damen++;
                }
            }
            TakeQueensBtnCommand.RaiseCanExecuteChanged();
            _koenige = 0;
            foreach (var kingRoll in ActRoll)
            {
                if (kingRoll.Value == 5)
                {
                    _koenige++;
                }
            }
            TakeKingsBtnCommand.RaiseCanExecuteChanged();
            _asse = 0;
            foreach (var aceRoll in ActRoll)
            {
                if (aceRoll.Value == 6)
                {
                    _asse++;
                }
            }
            TakeAcesBtnCommand.RaiseCanExecuteChanged();
            _neuner = 0;
            foreach (var nineRoll in ActRoll)
            {
                if (nineRoll.Value == 1)
                {
                    _neuner++;
                }
            }
            TakeNinesBtnCommand.RaiseCanExecuteChanged();
            _neuner = 0;
            foreach (var nineRoll in ActRoll)
            {
                if (nineRoll.Value == 1)
                {
                    _neuner++;
                }
            }
            TakeNinesBtnCommand.RaiseCanExecuteChanged();
            _neuner = 0;
            foreach (var nineRoll in ActRoll)
            {
                if (nineRoll.Value == 1)
                {
                    _neuner++;
                }
            }
            TakeNinesBtnCommand.RaiseCanExecuteChanged();
            _neuner = 0;
            foreach (var nineRoll in ActRoll)
            {
                if (nineRoll.Value == 1)
                {
                    _neuner++;
                }
            }
            TakeNinesBtnCommand.RaiseCanExecuteChanged();
        }

        private void StartGame()
        {
            for (int i = 1; i < 11; i++)
            {
                //Messages.Insert(0,"--- turn" + i + " has started ---");
                foreach (Player actPlayer in PlayerList)
                {
                    RollNumber = 1;
                    RollFinished = false;
                    if (actPlayer == PlayerList.First())
                    {
                        //Messages.Insert(0,"++ player " + actPlayer + " starts ++");
                    } else if (actPlayer == PlayerList.Last())
                    {
                        //Messages.Insert(0,"++ final player in this turn is " + actPlayer + " ++");
                    } else {
                        //Messages.Insert(0,"++ next is player " + actPlayer + " ++ ");
                    }
                    com.Send("ap:"+actPlayer.Name);
                    GUIAction("ap:"+actPlayer.Name);
                    while (!RollFinished)
                    {

                    }
                }
            }
        }

        private void RotateDice()
        {
            while (!GameStarted)
            {
                for (int i = 0; i < ActRoll.Length; i++)
                {
                    ActRoll[i] = new Roll();
                    ActRoll[i].DiceImage.Freeze();
                }
                RaisePropertyChanged("SelectableRoll");
                Thread.Sleep(1500);
            }
        }
        private void Shuffle()
        {
            
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
                    if (message.StartsWith("gs:"))
                    {
                        GameStarted = true;
                        StartGameCommand.RaiseCanExecuteChanged();
                        RollCommand.RaiseCanExecuteChanged();
                        Messages.Insert(0, "the game has started with " + message.Split(':')[1] + " players, rien ne vas plus!");
                    } else if (message.StartsWith("np:")) {
                        string[] splitted = message.Split("np:");
                        {
                            for (var i = 1; i < splitted.Length; i++)
                            {
                                PlayerList.Add(new Player(splitted[i]));
                            }
                        }
                    } else if (message.StartsWith("nt:"))
                    {
                        Messages.Insert(0,message.Split(':')[1] + " took " + message.Split(':')[2] + " nines, making " + int.Parse(message.Split(':')[2]) * 1 + " points");
                        foreach (var actPlayer in PlayerList) 
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Neuner = int.Parse(message.Split(':')[2]) * 1;
                        RaisePropertyChanged("PlayerList");
                        if (isServer)
                        {
                            RollFinished = true;
                        }
                    }
                    else if (message.StartsWith("tt:"))
                    {
                        Messages.Insert(0, message.Split(':')[1] + " took " + message.Split(':')[2] + " tens, making " + int.Parse(message.Split(':')[2]) * 2 + " points");
                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Zehner = int.Parse(message.Split(':')[2]) * 2;
                        RaisePropertyChanged("PlayerList");
                    }
                    else if (message.StartsWith("jt:"))
                    {
                        Messages.Insert(0, message.Split(':')[1] + " took " + message.Split(':')[2] + " jacks, making " + int.Parse(message.Split(':')[2]) * 3 + " points");
                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Buben = int.Parse(message.Split(':')[2]) * 3;
                        RaisePropertyChanged("PlayerList");
                    }
                    else if (message.StartsWith("qt:"))
                    {
                        Messages.Insert(0, message.Split(':')[1] + " took " + message.Split(':')[2] + " queens, making " + int.Parse(message.Split(':')[2]) * 4 + " points");
                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Damen = int.Parse(message.Split(':')[2]) * 4;
                        RaisePropertyChanged("PlayerList");
                    }
                    else if (message.StartsWith("kt:"))
                    {
                        Messages.Insert(0, message.Split(':')[1] + " took " + message.Split(':')[2] + " kings, making " + int.Parse(message.Split(':')[2]) * 5 + " points");
                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Koenige = int.Parse(message.Split(':')[2]) * 5;
                        RaisePropertyChanged("PlayerList");
                    }
                    else if (message.StartsWith("at:"))
                    {
                        Messages.Insert(0, message.Split(':')[1] + " took " + message.Split(':')[2] + " aces, making " + int.Parse(message.Split(':')[2]) * 6 + " points");
                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Asse = int.Parse(message.Split(':')[2]) * 6;
                        RaisePropertyChanged("PlayerList");
                    }
                    else if (message.StartsWith("st:"))
                    {
                        Messages.Insert(0, message.Split(':')[1] + " took " + message.Split(':')[2] + " nines, making " + int.Parse(message.Split(':')[2]) * 1 + " points");
                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Neuner = int.Parse(message.Split(':')[2]) * 1;
                        RaisePropertyChanged("PlayerList");
                    }
                    else if (message.StartsWith("ft:"))
                    {
                        Messages.Insert(0, message.Split(':')[1] + " took " + message.Split(':')[2] + " nines, making " + int.Parse(message.Split(':')[2]) * 1 + " points");
                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Neuner = int.Parse(message.Split(':')[2]) * 1;
                        RaisePropertyChanged("PlayerList");
                    }
                    else if (message.StartsWith("pt:"))
                    {
                        Messages.Insert(0, message.Split(':')[1] + " took " + message.Split(':')[2] + " nines, making " + int.Parse(message.Split(':')[2]) * 1 + " points");
                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Neuner = int.Parse(message.Split(':')[2]) * 1;
                        RaisePropertyChanged("PlayerList");
                    }
                    else if (message.StartsWith("gt:"))
                    {
                        Messages.Insert(0, message.Split(':')[1] + " took " + message.Split(':')[2] + " nines, making " + int.Parse(message.Split(':')[2]) * 1 + " points");
                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Neuner = int.Parse(message.Split(':')[2]) * 1;
                        RaisePropertyChanged("PlayerList");
                    }
                    else if (message.StartsWith("ap:"))
                    {
                        ActPlayer = message.Split(':')[1];
                        RollCommand.RaiseCanExecuteChanged();
                        Messages.Insert(0,ActPlayer);
                    }
                    Messages.Insert(0,message);
                });
        }
    }
}
