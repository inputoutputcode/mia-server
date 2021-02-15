using System;


namespace Mia.Server
{
    public class Turn
    {
        private Player player;
        private int rollCount;
        private bool hasAnnounced;

        public int RollCount
        {
            get { return rollCount; }
        }

        public Player Player
        {
            get { return player; }
        }

        public bool HasAnnounced
        {
            get { return hasAnnounced; }
            set { hasAnnounced = value; }
        }

        public Turn(Player player)
        {
            this.player = player;
        }

        public void AddRollCount()
        {
            rollCount++;
        }
    }
}
