namespace DicePokerMQ.Communication
{
    public interface ICom
    {
        //send messages to server if client, or to all clients if server
        void Send(string data);

        //accept new players until the game has started
        void Accept();

    }
}