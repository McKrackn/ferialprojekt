using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
// using System.Net;
// using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using DicePokerMQ.Model;

namespace DicePokerMQ.Communication
{
    public class Com : ICom
    {
        private ISession session;
        public Action<string> GUIAction;
        Thread acceptingThread;
        List<ClientHandler> clients;
        ObservableCollection<Player> players;
        private bool gameStarted;

        public Com(bool isServer, Action<string> action, ObservableCollection<Player> playerList, bool gameStarted, string ipPort)
        {
            IConnectionFactory factory = new ConnectionFactory("tcp://"+ipPort);
            IConnection connection = factory.CreateConnection();
            connection.Start();
            session = connection.CreateSession();

            this.GUIAction = action;

            if (isServer)
            {
                IDestination dest = session.GetQueue("DicePoker.ServerQueue");
                IMessageProducer producer = session.CreateProducer(dest);
                IDestination clientQueue = session.GetQueue("DicePoker.ProducerClientQueue");
                IMessageConsumer clientMessages = session.CreateConsumer(clientQueue);

                Task.Factory.StartNew(StartAccepting);
                Task.Run(NewMessageReceived);
                this.players = playerList;
                this.gameStarted = gameStarted;
            }
            else
            {
                try
                {
                    IDestination dest = session.GetQueue("DicePoker.NewPlayerQueue");
                    IMessageProducer producer = session.CreateProducer(dest);
                    var objectMessage = producer.CreateObjectMessage("np:"+players[0].Name);
                    producer.Send(objectMessage);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                Task.Factory.StartNew(Receive);
            }
        }

        private void Receive()
        {
            IDestination personalConsumerQueue = session.GetQueue("DicePoker."+players[0].Name);
            IMessageConsumer consumer = session.CreateConsumer(personalConsumerQueue);
            IMessage message;

            while ((message = consumer.Receive(TimeSpan.FromDays(1))) != null)
            {
                var objectMessage = message as IObjectMessage;
                var mapMessage = objectMessage?.Body as string;
                GUIAction(mapMessage);
            }
            //string message = "";
            //while (!message.Contains("@quit"))
            //{
            //    try
            //    {

            //        int length = clientSocket.Receive(buffer);
            //        message = Encoding.UTF8.GetString(buffer, 0, length);
            //        //inform GUI via delegate
            //        GUIAction(message);
            //        message = "";
            //    }
            //    catch (SocketException e)
            //    {
            //        Console.WriteLine(e);
            //        throw;
            //    }
            //}
            //clientSocket.Close();
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
            IDestination newPlayerQueue = session.GetQueue("DicePoker.NewPlayerQueue");
            IMessageConsumer getNewPlayers = session.CreateConsumer(newPlayerQueue);
            IMessage message;
            while (acceptingThread.IsAlive && !gameStarted && (message = getNewPlayers.Receive(TimeSpan.FromDays(1))) != null)
            {
                var objectMessage = message as IObjectMessage;
                var mapMessage = objectMessage?.Body as string;
                try
                {
                    this.Send("np:" + mapMessage.Split(':')[1]);
                    clients.Add(new ClientHandler(mapMessage.Split(':')[1], session));
                    foreach (Player actPlayer in players)
                    {
                        string msg = "";
                        msg = "np:" + actPlayer.Name;
                        clients.Last().Send(msg);
                    }
                    GUIAction("np:" + mapMessage.Split(':')[1]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            //while (acceptingThread.IsAlive && !gameStarted)
            {
                //try
                //{
                //    clients.Add(new ClientHandler(serverSocket.Accept(), new Action<string, Socket>(NewMessageReceived)));
                //    foreach (Player actPlayer in players)
                //    {
                //        string msg = "";
                //        msg = "np:" + actPlayer.Name;
                //        clients.Last().Send(msg);
                //    }
                //}
                //catch (SocketException e)
                //{
                //    Console.WriteLine(e);
                //    throw;
                //}
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
                IDestination dest = session.GetQueue("DicePoker.ProducerClientQueue");
                IMessageProducer producer = session.CreateProducer(dest);
                var objectMessage = producer.CreateObjectMessage(data);
                producer.Send(objectMessage);
                //clientSocket?.Send(Encoding.UTF8.GetBytes(data));
            }
        }
        private void NewMessageReceived()
        {
            IDestination dest = session.GetQueue("DicePoker.ProducerClientQueue");
            IMessageConsumer consumer = session.CreateConsumer(dest);
            IMessage queueMessage;
            while ((queueMessage = consumer.Receive(TimeSpan.FromDays(1))) != null)
            {
                var objectMessage = queueMessage as IObjectMessage;
                var mapMessage = objectMessage?.Body as string;
                GUIAction(mapMessage);

                foreach (var item in clients)
                {
                    if (item.Name != "senderName")
                    {
                        item.Send(mapMessage);
                    }
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

