using Mia.Server.Game.Interface;


namespace Mia.Server.Game.PlayEngine
{
    public class Turn : ITurn
    {
        private IPlayer player;
        private int rollCount;
        private bool hasAnnounced;
        private bool hasFinished;

        public IPlayer Player
        {
            get { return player; }
        }

        public bool HasAnnounced
        {
            get { return hasAnnounced; }
            set { hasAnnounced = value; }
        }

        public bool HasFinished
        {
            get { return hasFinished; }
            set { hasFinished = value; }
        }

        public Turn(IPlayer player)
        {
            this.player = player;
        }

        public int RollCount
        {
            get { return rollCount; }
        }

        public void AddRollCount()
        {
            rollCount++;
        }
    }
}
