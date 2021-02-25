
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProFer.Model;

namespace ProFer.Communication
{
    public class Com
    {
        
        Socket serverSocket;
        Socket clientSocket;
        int port;
        IPAddress ip;
        byte[] buffer = new byte[512];
        public Action<string> GUIAction;
        Thread acceptingThread;
        List<ClientHandler> clients;
        ObservableCollection<Player> players;
        private bool gameStarted;

        public Com(bool isServer, Action<string> action, ObservableCollection<Player> playerList, bool gameStarted, string ipPort)
        {
            this.GUIAction = action;
            port = int.Parse(ipPort.Split(':')[1]);

            if (isServer)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                serverSocket.Listen(5);
                Task.Factory.StartNew(StartAccepting);
                this.players = playerList;
                this.gameStarted = gameStarted;
            }
            else
            {
                TcpClient client = new TcpClient();
                try
                {
                    client.Connect(new IPEndPoint(IPAddress.Parse(ipPort.Split(':')[0]), port));
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                clientSocket = client.Client;
                Task.Factory.StartNew(Receive);
            }
        }

        private void Receive()
        {
            string message = "";
            while (!message.Contains("@quit"))
            {
                try
                {

                    int length = clientSocket.Receive(buffer);
                    message = Encoding.UTF8.GetString(buffer, 0, length);
                    //inform GUI via delegate
                    GUIAction(message);
                    message = "";
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            clientSocket.Close();
        }

        private void StartAccepting()
        {
            acceptingThread = new Thread(new ThreadStart(Accept));
            acceptingThread.IsBackground = true;
            acceptingThread.Start();
        }
        private void Accept()
        {
            clients = new List<ClientHandler>();
            while (acceptingThread.IsAlive && !gameStarted)
            {
                try
                {
                    clients.Add(new ClientHandler(serverSocket.Accept(), new Action<string, Socket>(NewMessageReceived)));
                    foreach (Player actPlayer in players)
                    {
                        string msg = "";
                        msg = "np:" + actPlayer.Name;
                        clients.Last().Send(msg);
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public void Send(string data)
        {
            if (clients!=null)
            {
                foreach (var item in clients)
                {
                    item.Send(data);
                }
            }
            else
            {
                clientSocket?.Send(Encoding.UTF8.GetBytes(data));
            }
        }
        private void NewMessageReceived(string message, Socket senderSocket)
        {
            GUIAction(message);
            foreach (var item in clients)
            {
                if (item.Clientsocket != senderSocket )
                {
                    item.Send(message);
                }
            }

        }
        public void DisconnectSpecificClient(string name)
        {
            foreach (var item in clients)
            {
                if (item.Name.Equals(name))
                {
                    item.Close();
                    clients.Remove(item); 
                    break;
                }
            }
        }
    }
}

