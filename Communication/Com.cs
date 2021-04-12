using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using DicePokerMQ.Model;

namespace DicePokerMQ.Communication
{
    public class Com : ICom
    {
        private static ISession session;
        public Action<string> GUIAction;
        Thread acceptingThread;
        List<IMessageProducer> clients;
        IDestination clientQueue;
        ObservableCollection<Player> players;
        private bool gameStarted;

        public Com(bool isServer, Action<string> action, ObservableCollection<Player> playerList, bool gameStarted, string ipPort)
        {
            IConnectionFactory factory = new ConnectionFactory("tcp://"+ipPort);
            IConnection connection = factory.CreateConnection();
            connection.Start();
            session = connection.CreateSession();
            clientQueue = session.GetQueue("DicePoker.ProducerClientQueue");

            this.players = playerList;
            this.GUIAction = action;

            if (isServer)
            {
                Task.Factory.StartNew(StartAccepting);
                Task.Run(NewMessageReceived);
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
        }

        private void StartAccepting()
        {
            acceptingThread = new Thread(new ThreadStart(Accept));
            acceptingThread.IsBackground = true;
            acceptingThread.Start();
        }

        public void Accept()
        {
            clients = new List<IMessageProducer>();
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
                    clients.Add(session.CreateProducer(session.GetQueue("DicePoker." + mapMessage.Split(':')[1])));
                    foreach (Player actPlayer in players)
                    {
                        clients.Last().Send(clients.Last().CreateObjectMessage("np:" + actPlayer.Name));
                    }
                    GUIAction("np:" + mapMessage.Split(':')[1]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private void NewMessageReceived()
        {
            IMessageConsumer consumer = session.CreateConsumer(clientQueue);
            IMessage message;
            while ((message = consumer.Receive(TimeSpan.FromDays(1))) != null)
            {
                var objectMessage = message as IObjectMessage;
                var mapMessage = objectMessage?.Body as string;
                GUIAction(mapMessage);

                foreach (var producer in clients)
                {
                    producer.Send(objectMessage);
                }
            }
        }
        public void Send(string data)
        {
            if (clients != null)
            {
                foreach (var producer in clients)
                {
                    var objectMessage = producer.CreateObjectMessage(data);
                    producer.Send(objectMessage);
                }
            }
            else
            {
                IMessageProducer producer = session.CreateProducer(clientQueue);
                var objectMessage = producer.CreateObjectMessage(data);
                producer.Send(objectMessage);
            }
        }
    }
}

