using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DicePokerMQ.Communication;
using DicePokerMQ.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DicePokerMQ.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private Com com;
        private bool isConnected;
        private bool isServer;
        private bool gameStarted;
        public bool GameStarted
        {
            get => gameStarted;
            set => gameStarted = value;
        }
        public ObservableCollection<Player> PlayerList { get; set; } = new ObservableCollection<Player>();

        #region roll values
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

        public int Strasse
        {
            get => _strasse;
            set { _strasse = value;TakeStreetBtnCommand.RaiseCanExecuteChanged(); }
        }

        public int Full
        {
            get => _full;
            set { _full = value; TakeFullBtnCommand.RaiseCanExecuteChanged(); }
        }

        public int Poker
        {
            get => _poker;
            set { _poker = value; TakePokerBtnCommand.RaiseCanExecuteChanged(); }
        }

        public int Grande
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
        private int _strasse;
        private int _full;
        private int _poker;
        private int _grande;
        #endregion

        #region gui commands/params
        private int _rollNumber = 1;
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
        private string _ipport = "localhost:61616";
        public string IPPort
        {
            get { return _ipport; }
            set { _ipport = value; ActAsClientBtnCommand.RaiseCanExecuteChanged(); ActAsServerBtnCommand.RaiseCanExecuteChanged(); }
        }
        private string _buttonColor = "SteelBlue";
        public string ButtonColor
        {
            get { return _buttonColor; }
            set { _buttonColor = value; RaisePropertyChanged(); TakeNinesBtnCommand.RaiseCanExecuteChanged();}
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
        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string> { "---   Meskalero PokerDice   ---" };

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
        public Roll RollSelectedItem
        { get { return rollSelectedItem; }
            set {
                var selectedItems = SelectableRoll.Count(x => x.IsSelected);
                RaisePropertyChanged();
            }
        }
        private string _actPlayer;
        public string ActPlayer
        {
            get => _actPlayer;
            set { _actPlayer = value; RollCommand.RaiseCanExecuteChanged(); }
        }

        public bool RollFinished { get; set; }
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

            #region TakeButtonCommands
            TakeNinesBtnCommand = new RelayCommand(
                () =>
                {
                    ButtonColor = "SteelBlue";
                    RollNumber = 5;
                    com.Send("nt:" + Self.Name + ":" + _neuner);
                    Messages.Insert(0,"you took " + _neuner + " nines, making " + _neuner*1 + " points");
                    PlayerList[0].Neuner = _neuner * 1;
                    RefreshTakeButtons();
                    RollFinished = true;
                },
                () => (RollNumber > 1 && Neuner > 0 && PlayerList[0].Neuner == 0 && RollNumber <= 4) || (ButtonColor=="Red" && PlayerList[0].Neuner == 0));
            TakeTensBtnCommand = new RelayCommand(
                () =>
                {
                    ButtonColor = "SteelBlue";
                    RollNumber = 5;
                    com.Send("tt:" + Self.Name + ":" + _zehner);
                    Messages.Insert(0, "you took " + _zehner + " tens, making " + _zehner * 2 + " points");
                    PlayerList[0].Zehner = _zehner * 2;         
                    RefreshTakeButtons();
                    RollFinished = true;
                },
                () => (RollNumber > 1 && Zehner > 0 && PlayerList[0].Zehner == 0 && RollNumber <= 4) || (ButtonColor == "Red" && PlayerList[0].Zehner == 0));
            TakeJacksBtnCommand = new RelayCommand(
                () =>
                {
                    ButtonColor = "SteelBlue";
                    RollNumber = 5;
                    com.Send("jt:" + Self.Name + ":" + _buben);
                    Messages.Insert(0, "you took " + _buben + " jacks, making " + _buben * 3 + " points");
                    PlayerList[0].Buben = _buben * 3; 
                    RefreshTakeButtons();
                    RollFinished = true;
                },
                () => (RollNumber > 1 && Buben > 0 && PlayerList[0].Buben== 0 && RollNumber <= 4) || (ButtonColor == "Red" && PlayerList[0].Buben == 0));
            TakeQueensBtnCommand = new RelayCommand(
                () =>
                {
                    ButtonColor = "SteelBlue";
                    RollNumber = 5;
                    com.Send("qt:" + Self.Name + ":" + _damen);
                    Messages.Insert(0, "you took " + _damen + " queens, making " + _damen * 4 + " points");
                    PlayerList[0].Damen = _damen * 4;
                    RefreshTakeButtons();
                    RollFinished = true;
                },
                () => (RollNumber > 1 && Damen > 0 && PlayerList[0].Damen== 0 && RollNumber <= 4) || (ButtonColor == "Red" && PlayerList[0].Damen == 0));
            TakeKingsBtnCommand = new RelayCommand(
                () =>
                {
                    ButtonColor = "SteelBlue";
                    RollNumber = 5;
                    com.Send("kt:" + Self.Name + ":" + _koenige);
                    Messages.Insert(0, "you took " + _koenige + " kings, making " + _koenige * 5 + " points");
                    PlayerList[0].Koenige = _koenige * 5;
                    RefreshTakeButtons();
                    RollFinished = true;
                },
                () => (RollNumber > 1 && Koenige > 0 && PlayerList[0].Koenige== 0 && RollNumber <= 4) || (ButtonColor == "Red" && PlayerList[0].Koenige == 0));
            TakeAcesBtnCommand = new RelayCommand(
                () =>
                {
                    ButtonColor = "SteelBlue";
                    RollNumber = 5;
                    com.Send("at:" + Self.Name + ":" + _asse);
                    Messages.Insert(0, "you took " + _asse + " nines, making " + _asse * 6 + " points");
                    PlayerList[0].Asse = _asse * 6;
                    RefreshTakeButtons();
                    RollFinished = true;
                },
                () => (RollNumber > 1 && Asse > 0 && PlayerList[0].Asse== 0 && RollNumber <= 4) || (ButtonColor == "Red" && PlayerList[0].Asse == 0));
            TakeStreetBtnCommand = new RelayCommand(
                () =>
                {
                    ButtonColor = "SteelBlue";
                    RollNumber = 5;
                    com.Send("st:" + Self.Name + ":" + _strasse);
                    Messages.Insert(0, "you took a street, making " + _strasse + " points");
                    PlayerList[0].Strasse = _strasse;
                    RefreshTakeButtons();
                    RollFinished = true;
                },
                () => (RollNumber > 1 && Strasse > 0 && PlayerList[0].Strasse == 0 && RollNumber <= 4) || (ButtonColor == "Red" && PlayerList[0].Strasse == 0));
            TakeFullBtnCommand = new RelayCommand(
                () =>
                {
                    ButtonColor = "SteelBlue";
                    RollNumber = 5;
                    com.Send("ft:" + Self.Name + ":" + _full);
                    Messages.Insert(0, "you took a full, making " + _full + " points");
                    PlayerList[0].Full = _full;
                    RefreshTakeButtons();
                    RollFinished = true;
                },
                () => (RollNumber > 1 && Full > 0 && PlayerList[0].Full == 0 && RollNumber <= 4) || (ButtonColor == "Red" && PlayerList[0].Full == 0));
            TakePokerBtnCommand = new RelayCommand(
                () =>
                {
                    ButtonColor = "SteelBlue";
                    RollNumber = 5;
                    com.Send("pt:" + Self.Name + ":" + _poker);
                    Messages.Insert(0, "you took a poker, making " + _poker + " points");
                    PlayerList[0].Poker = _poker * 1;
                    RefreshTakeButtons();
                    RollFinished = true;
                },
                () => (RollNumber > 1 && Poker > 0 && PlayerList[0].Poker == 0 && RollNumber <= 4) || (ButtonColor == "Red" && PlayerList[0].Poker == 0));
            TakeGrandBtnCommand = new RelayCommand(
                () =>
                {
                    ButtonColor = "SteelBlue";
                    RollNumber = 5;
                    com.Send("gt:" + Self.Name + ":" + _grande);
                    Messages.Insert(0, "xox you made a grand, what a fine roll! " + _grande + " points for you xox");
                    PlayerList[0].Grande = _grande * 1;
                    RefreshTakeButtons();
                    RollFinished = true;
                },
                () => (RollNumber > 1 && Grande > 0 && PlayerList[0].Grande == 0 && RollNumber <= 4) || (ButtonColor == "Red" && PlayerList[0].Grande == 0));
            #endregion

            ActAsServerBtnCommand = new RelayCommand(
                () =>
                {
                    Self = new Player(Name);
                    PlayerList.Add(Self);

                    com = new Com(true, GUIAction, PlayerList, GameStarted, IPPort);
                    isConnected = true;
                    isServer = true;

                    ActAsClientBtnCommand.RaiseCanExecuteChanged();
                    StartGameCommand.RaiseCanExecuteChanged();
                    ActAsServerBtnCommand.RaiseCanExecuteChanged();
                    NameVisibility = "false";

                    Messages.Insert(0,"Welcome Player " + Name + " (Server). You can start the game whenever you wish.");
                    Task.Factory.StartNew(RotateDice);

                }, () => !isConnected && !string.IsNullOrWhiteSpace(_name) && !string.IsNullOrWhiteSpace(_ipport));

            ActAsClientBtnCommand = new RelayCommand(
                () =>
                {
                    try
                    {
                        Self = new Player(Name);
                        PlayerList.Add(Self);

                        com = new Com(false, GUIAction, PlayerList, GameStarted, IPPort);
                        isConnected = true;

                        ActAsClientBtnCommand.RaiseCanExecuteChanged();
                        ActAsServerBtnCommand.RaiseCanExecuteChanged();
                        NameVisibility = "false";
                        Messages.Insert(0,"Welcome Player " + Name + ". Please wait until the host starts the game..");
                        //com.Send("np:" + Name);
                        Task.Factory.StartNew(RotateDice);
                    }
                    catch (SocketException e)
                    {
                        Messages.Insert(0,"The computer at " + e.Message.Substring(e.Message.LastIndexOf('[')) + " said no");
                    }
                }, () => !isConnected && !string.IsNullOrWhiteSpace(_name) && !string.IsNullOrWhiteSpace(_ipport));

            StartGameCommand = new RelayCommand(
                () =>
                {
                    string message = "gs:" + PlayerList.Count;
                    com.Send(message);
                    GUIAction(message);
                    Task.Factory.StartNew(StartGame);
                },
                () => isServer && !GameStarted);

            RollCommand = new RelayCommand(() =>
                {
                    com.Send("nr:"+Self.Name+":"+RollNumber);
                    Messages.Insert(0,"you are rolling the dice for the " + RollNumber + ". time...");
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
                    com.Send("rm:" + sendRoll + sendSelected);
                    CalculateRoll();
                    RollCommand.RaiseCanExecuteChanged();
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
            _strasse = 0;
            if (_zehner == 1 && _buben == 1 && _damen == 1 && _koenige == 1)
            {
                _strasse = RollNumber == 2 ? 25 : 20;
            }
            TakeStreetBtnCommand.RaiseCanExecuteChanged();
            _full = 0;
            if ((_neuner == 3 || _zehner == 3 || _buben == 3 || _damen == 3 || _koenige == 3 || _asse == 3) && (_neuner == 2 || _zehner == 2 || _buben == 2 || _damen == 2 || _koenige == 2 || _asse == 2))
            {
                _full = RollNumber == 2 ? 35 : 30;
            }
            TakeFullBtnCommand.RaiseCanExecuteChanged();
            _poker = 0;
            if (_neuner == 4 || _zehner == 4 || _buben == 4 || _damen == 4 || _koenige == 4 || _asse == 4)
            {
                _poker = RollNumber == 2 ? 45 : 40;
            }
            TakePokerBtnCommand.RaiseCanExecuteChanged();
            _grande = 0;
            if (_neuner == 5 || _zehner == 5 || _buben == 5 || _damen == 5 || _koenige == 5 || _asse == 5)
            {
                _grande = RollNumber == 2 ? 100 : 50;
            }
            TakeGrandBtnCommand.RaiseCanExecuteChanged();
            if (!TakeNinesBtnCommand.CanExecute(null) && !TakeTensBtnCommand.CanExecute(null) &&
                !TakeJacksBtnCommand.CanExecute(null) && !TakeQueensBtnCommand.CanExecute(null) &&
                !TakeKingsBtnCommand.CanExecute(null) && !TakeAcesBtnCommand.CanExecute(null) &&
                !TakeStreetBtnCommand.CanExecute(null) && !TakeFullBtnCommand.CanExecute(null) &&
                !TakePokerBtnCommand.CanExecute(null) && !TakeGrandBtnCommand.CanExecute(null) && RollNumber==4)
            {
                ButtonColor = "Red";
                Messages.Insert(0, "xxx bad luck, you have to strike out xxx");
                com.Send("xx:"+Name);
                _neuner = -1;
                _zehner = -1;
                _buben = -1;
                _damen = -1;
                _koenige = -1;
                _asse = -1;
                _strasse = -1;
                _full = -1;
                _poker = -1;
                _grande = -1;
                RefreshTakeButtons();
            }
        }

        private void RefreshTakeButtons()
        {
            TakeNinesBtnCommand.RaiseCanExecuteChanged();
            TakeTensBtnCommand.RaiseCanExecuteChanged();
            TakeJacksBtnCommand.RaiseCanExecuteChanged();
            TakeQueensBtnCommand.RaiseCanExecuteChanged();
            TakeKingsBtnCommand.RaiseCanExecuteChanged();
            TakeAcesBtnCommand.RaiseCanExecuteChanged();
            TakeStreetBtnCommand.RaiseCanExecuteChanged();
            TakeFullBtnCommand.RaiseCanExecuteChanged();
            TakePokerBtnCommand.RaiseCanExecuteChanged();
            TakeGrandBtnCommand.RaiseCanExecuteChanged();
            RollCommand.RaiseCanExecuteChanged();
        }

        private void StartGame()
        {
            for (int i = 1; i < 11; i++)
            {
                com.Send("gm:--- turn " + i + " has started ---");
                GUIAction("gm:--- turn " + i + " has started ---");
                foreach (Player actPlayer in PlayerList)
                {
                    RollFinished = false;
                    if (actPlayer == PlayerList.First())
                    {
                        com.Send("gm:++ player " + actPlayer + " starts ++");
                        GUIAction("gm:++ player " + actPlayer + " starts ++");
                    } else if (actPlayer == PlayerList.Last())
                    {
                        com.Send("gm:++ final player in this turn is " + actPlayer + " ++");
                        GUIAction("gm:++ final player in this turn is " + actPlayer + " ++");
                    } else
                    {
                        com.Send("gm:++ next is " + actPlayer + " ++");
                        GUIAction("gm:++ next is " + actPlayer + " ++");
                    }
                    Thread.Sleep(100);
                    com.Send("ap:"+actPlayer.Name);
                    GUIAction("ap:"+actPlayer.Name);
                    while (!RollFinished)
                    {

                    }
                }
            }
            com.Send("gm:+-+-++-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-");
            Thread.Sleep(100);
            com.Send("gm:+-+-+ the game is over, please get lost now -+-+-");
            Thread.Sleep(100);
            com.Send("gm:+-+-++-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-");
            GUIAction("gm:+-+-++-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-");
            GUIAction("gm:+-+-+ the game is over, please get lost now -+-+-");
            GUIAction("gm:+-+-++-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-");
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
                        if (int.Parse(message.Split(':')[2]) == -1)
                        {
                            Messages.Insert(0, message.Split(':')[1] + " stroke the nines");
                        } else
                        {
                            Messages.Insert(0,
                                message.Split(':')[1] + " took " + message.Split(':')[2] + " nines, making " +
                                int.Parse(message.Split(':')[2]) * 1 + " points");
                        }                        
                        
                        foreach (var actPlayer in PlayerList) 
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Neuner = int.Parse(message.Split(':')[2]) * 1;
                        RaisePropertyChanged("PlayerList");
                        RollFinished = true;
                    }
                    else if (message.StartsWith("tt:"))
                    {
                        if (int.Parse(message.Split(':')[2]) == -1)
                        {
                            Messages.Insert(0, message.Split(':')[1] + " stroke the tens");
                        }
                        else
                        {
                            Messages.Insert(0,
                                message.Split(':')[1] + " took " + message.Split(':')[2] + " tens, making " +
                                int.Parse(message.Split(':')[2]) * 2 + " points");
                        }

                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Zehner = int.Parse(message.Split(':')[2]) * 2;
                        RaisePropertyChanged("PlayerList");
                        RollFinished = true;
                    }
                    else if (message.StartsWith("jt:"))
                    {
                        if (int.Parse(message.Split(':')[2]) == -1)
                        {
                            Messages.Insert(0, message.Split(':')[1] + " stroke the jacks");
                        }
                        else
                        {
                            Messages.Insert(0,
                                message.Split(':')[1] + " took " + message.Split(':')[2] + " jacks, making " +
                                int.Parse(message.Split(':')[2]) * 3 + " points");
                        }

                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Buben = int.Parse(message.Split(':')[2]) * 3;
                        RaisePropertyChanged("PlayerList");
                        RollFinished = true;
                    }
                    else if (message.StartsWith("qt:"))
                    {

                        if (int.Parse(message.Split(':')[2]) == -1)
                        {
                            Messages.Insert(0, message.Split(':')[1] + " stroke the queens");
                        }
                        else
                        {
                            Messages.Insert(0,
                                message.Split(':')[1] + " took " + message.Split(':')[2] + " queens, making " +
                                int.Parse(message.Split(':')[2]) * 4 + " points");
                        }

                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Damen = int.Parse(message.Split(':')[2]) * 4;
                        RaisePropertyChanged("PlayerList");
                        RollFinished = true;
                    }
                    else if (message.StartsWith("kt:"))
                    {
                        if (int.Parse(message.Split(':')[2]) == -1)
                        {
                            Messages.Insert(0, message.Split(':')[1] + " stroke the kings");
                        }
                        else
                        {
                            Messages.Insert(0,
                                message.Split(':')[1] + " took " + message.Split(':')[2] + " kings, making " +
                                int.Parse(message.Split(':')[2]) * 5 + " points");
                        }

                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Koenige = int.Parse(message.Split(':')[2]) * 5;
                        RaisePropertyChanged("PlayerList");
                        RollFinished = true;
                    }
                    else if (message.StartsWith("at:"))
                    {
                        if (int.Parse(message.Split(':')[2]) == -1)
                        {
                            Messages.Insert(0, message.Split(':')[1] + " stroke the aces");
                        }
                        else
                        {
                            Messages.Insert(0,
                                message.Split(':')[1] + " took " + message.Split(':')[2] + " aces, making " +
                                int.Parse(message.Split(':')[2]) * 6 + " points");
                        }

                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Asse = int.Parse(message.Split(':')[2]) * 6;
                        RaisePropertyChanged("PlayerList");
                        RollFinished = true;
                    }
                    else if (message.StartsWith("st:"))
                    {

                        if (int.Parse(message.Split(':')[2]) == -1)
                        {
                            Messages.Insert(0, message.Split(':')[1] + " stroke the street");
                        }
                        else
                        {
                            Messages.Insert(0,
                                message.Split(':')[1] + " took a street, making " + int.Parse(message.Split(':')[2]) +
                                " points");
                        }

                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Strasse = int.Parse(message.Split(':')[2]);
                        RaisePropertyChanged("PlayerList");
                        RollFinished = true;
                    }
                    else if (message.StartsWith("ft:"))
                    {
                        if (int.Parse(message.Split(':')[2]) == -1)
                        {
                            Messages.Insert(0, message.Split(':')[1] + " stroke the full");
                        }
                        else
                        {
                            Messages.Insert(0,
                                message.Split(':')[1] + " took a full, making " + int.Parse(message.Split(':')[2]) +
                                " points");
                        }

                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Full = int.Parse(message.Split(':')[2]);
                        RaisePropertyChanged("PlayerList");
                        RollFinished = true;
                    }
                    else if (message.StartsWith("pt:"))
                    {
                        if (int.Parse(message.Split(':')[2]) == -1)
                        {
                            Messages.Insert(0, message.Split(':')[1] + " stroke the poker");
                        }
                        else
                        {
                            Messages.Insert(0,
                                message.Split(':')[1] + " took a poker, making " + int.Parse(message.Split(':')[2]) +
                                " points");
                        }

                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Poker = int.Parse(message.Split(':')[2]);
                        RaisePropertyChanged("PlayerList");
                        RollFinished = true;
                    }
                    else if (message.StartsWith("gt:"))
                    {

                        if (int.Parse(message.Split(':')[2]) == -1)
                        {
                            Messages.Insert(0, message.Split(':')[1] + " stroke the grand");
                        }
                        else
                        {
                            Messages.Insert(0,
                                message.Split(':')[1] + " took the grand, watch out! making " +
                                int.Parse(message.Split(':')[2]) + " points");
                        }

                        foreach (var actPlayer in PlayerList)
                            if (actPlayer.Name == message.Split(':')[1]) actPlayer.Grande = int.Parse(message.Split(':')[2]);
                        RaisePropertyChanged("PlayerList");
                        RollFinished = true;
                    }
                    else if (message.StartsWith("ap:"))
                    {
                        ActPlayer = message.Split(':')[1];
                        RollCommand.RaiseCanExecuteChanged();
                    }
                    else if (message.StartsWith("xx:"))
                    {
                        Messages.Insert(0, "xxx " + message.Split(':')[1] + " got bad luck and has to strike out something xxx");
                    }
                    else if (message.StartsWith("gm:"))
                    {
                        Messages.Insert(0, message.Split(':')[1]);
                        RollNumber = 1;
                        RollCommand.RaiseCanExecuteChanged();
                    }
                    else if (message.StartsWith("rm:"))
                    {
                        Messages.Insert(0, message.Split(':')[1]);
                    }
                });
        }
    }
}
