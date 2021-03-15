using Mia.Server.Game.Interface;


namespace Mia.Server.Game.PlayEngine
{
    public class Turn : ITurn
    {
        private IPlayer player;
        private int rollCount;

        public IPlayer Player
        {
            get { return player; }
        }
        public int RollCount
        {
            get { return rollCount; }
        }

        public Turn(IPlayer player)
        {
            this.player = player;
        }

        public void AddRollCount()
        {
            rollCount++;
        }
    }
}
