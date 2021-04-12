using Apache.NMS;

//depreciated (hopefully)
namespace DicePokerMQ.Communication
{
    internal class ClientHandler
    {
        const string endMessage = "@quit";
        public string Name { get; private set; }
        private IMessageProducer producer;

        public ClientHandler(string playerName, IMessageProducer producer)
        {
            this.Name = playerName;
            this.producer = producer;
        }

        public void Send(string message)
        {
            var objectMessage = producer.CreateObjectMessage(message);
            producer.Send(objectMessage);
        }

        public void Close()
        {
            Send(endMessage);
        }
    }
}