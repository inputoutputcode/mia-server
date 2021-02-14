using System;


namespace Mia.Server
{
    public class Turn
    {
        private Guid token;
        private int rollCount;
        private Player player;
        private bool hasAnnounced;

        public Guid Token
        {
            get { return token; }
        }

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
        }

        public Turn(Player player)
        {
            this.player = player;
            token = Guid.NewGuid();
        }

        public void AddRollCount()
        {
            rollCount++;
        }

        public bool IsValid(Player player, Guid token)
        {
            return (this.player == player && this.token == token);
        }

        public void Announce()
        {
            hasAnnounced = true;
        }
    }
}
