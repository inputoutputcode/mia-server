using Game.Server.Engine.Mia.Interface;

namespace Game.Server.Engine.Mia
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
