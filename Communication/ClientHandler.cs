using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Apache.NMS;

namespace DicePokerMQ.Communication
{
    internal class ClientHandler
    {
        private Thread clientReceiveThread;
        const string endMessage = "@quit";
        public string Name { get; private set; }
        private ISession session;
        private IMessageProducer producer;

        public ClientHandler(string playerName, ISession session)
        {
            this.Name = playerName;
            this.session = session;

            IDestination personalProducerQueue = session.GetQueue("DicePoker." + this.Name);
            producer = session.CreateProducer(personalProducerQueue);

            //clientReceiveThread = new Thread(Receive);
            //clientReceiveThread.Start();
        }

        /*private void Receive()
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
                    action(message);
                    message = "";
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e);
                    action("=== rm:Player " + Name + " left ===");
                    Clientsocket.Close(1);
                }
            }
            Close();
        }*/

        public void Send(string message)
        {
            var objectMessage = producer.CreateObjectMessage(message);
            producer.Send(objectMessage);
        }

        public void Close()
        {
            Send(endMessage); //sends endmessage to client 
            //Clientsocket.Close(1); //disconnects
            //clientReceiveThread.Abort(); //abort client threads
        }
    }
}