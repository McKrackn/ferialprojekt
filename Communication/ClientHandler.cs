
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ProFer.Communication
{
    internal class ClientHandler
    {
        private Action<string, Socket> action;
        private byte[] buffer = new byte[512];
        private Thread clientReceiveThread;
        const string endMessage = "@quit";
        public string Name { get; private set; }

        public Socket Clientsocket
        {
            get;
            private set;
        }

        public ClientHandler(Socket socket, Action<string, Socket> action)
        {
            this.Clientsocket = socket;
            this.action = action;
            clientReceiveThread = new Thread(Receive);
            clientReceiveThread.Start();
        }

        private void Receive()
        {
            string message = "";
            while (!message.Contains(endMessage))
            {
                try
                {
                    var length = Clientsocket.Receive(buffer);
                    message = Encoding.UTF8.GetString(buffer, 0, length);
                    //set name property if not already done
                    if (Name == null && message.Contains("np:"))
                    {
                        Name = message.Split("np:")[1];
                    }
                    //inform GUI via delegate
                    action(message, Clientsocket);
                    message = "";
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            Close();
        }

        public void Send(string message)
        {
            Clientsocket.Send(Encoding.UTF8.GetBytes(message));
            message = "";
        }

        public void Close()
        {
            Send(endMessage); //sends endmessage to client 
            Clientsocket.Close(1); //disconnects
            clientReceiveThread.Abort(); //abort client threads
        }
    }
}