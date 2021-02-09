
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProFer.Communication
{
    public class Com
    {
        
        Socket serverSocket;
        Socket clientSocket;
        const int port = 9090;
        byte[] buffer = new byte[512];
        public Action<string> GUIAction;
        Thread acceptingThread;
        List<ClientHandler> clients;


        public Com(bool isServer, Action<string> action)
        {
            this.GUIAction = action;

            if (isServer)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Loopback, port));
                serverSocket.Listen(5);
                Task.Factory.StartNew(StartAccepting);
            }
            else
            {
                TcpClient client = new TcpClient();
                client.Connect(new IPEndPoint(IPAddress.Loopback, port));
                clientSocket = client.Client;
                Task.Factory.StartNew(Receive);
            }
        }

        private void Receive()
        {
            while (true)
            {
                var length = clientSocket.Receive(buffer);
                GUIAction(Encoding.UTF8.GetString(buffer, 0, length));
            }
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
            while (acceptingThread.IsAlive)
            {
                clients.Add(new ClientHandler(serverSocket.Accept(), new Action<string, Socket>(NewMessageReceived)));
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
                if (item.Clientsocket != senderSocket)
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

