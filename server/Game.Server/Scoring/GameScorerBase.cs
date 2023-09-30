using System.Collections.Generic;
using System.Linq;

using Game.Server.Engine.Mia;
using Game.Server.Engine.Mia.Interface;
using Game.Server.Scoring.Interface;


namespace Game.Server.Scoring
{
    public abstract class GameScorerBase : IGameScorer
    {
        private List<IPlayer> scoredPlayers = new List<IPlayer>();

        internal IPlayer FindPlayer(string name, string ipAddress)
        {
            return scoredPlayers.FirstOrDefault(p => p.Name == name && p.IPAddress == ipAddress);
        }

        public void SetActivePlayers(List<IPlayer> playerList)
        {
            foreach (var player in playerList)
            {
                if (player.CurrentState != PlayerState.Spectator)
                {
                    IPlayer existingPlayer = null;
                    bool uniqueIp = Config.Config.Settings.UniqueIpAddressPerClientRequired;

                    if (uniqueIp)
                        existingPlayer = scoredPlayers.FirstOrDefault(p => p.Name == player.Name || p.IPAddress == player.IPAddress);
                    else
                        existingPlayer = scoredPlayers.FirstOrDefault(p => p.Name == player.Name);

                    // new player
                    if (existingPlayer == null)
                    {
                        scoredPlayers.Add(new Player(player.Name, false, player.IPAddress) { CurrentState = PlayerState.Active });
                    }
                    // existing IP address, but new player name
                    else if (existingPlayer.Name != player.Name && existingPlayer.IPAddress == player.IPAddress)
                    {
                        var sameAddress = scoredPlayers.FirstOrDefault(p => p.IPAddress == player.IPAddress);
                        if (sameAddress != null)
                            sameAddress.CurrentState = PlayerState.Inactive;

                        scoredPlayers.Add(new Player(player.Name, false, player.IPAddress) { CurrentState = PlayerState.Active });
                    }
                    // existing player name, but different IP address
                    else if (existingPlayer.Name == player.Name && existingPlayer.IPAddress != player.IPAddress)
                    {
                        var sameAddress = scoredPlayers.FirstOrDefault(p => p.Name == player.Name);
                        if (sameAddress != null)
                            sameAddress.CurrentState = PlayerState.Inactive;

                        scoredPlayers.Add(new Player(player.Name, false, player.IPAddress) { CurrentState = PlayerState.Active });
                    }
                    // existing player name with same IP address comes back
                    else
                    {
                        existingPlayer.CurrentState = PlayerState.Active;
                    }
                }
            }
        }

        public abstract void Lost(IPlayer player);

        public abstract void Neutral(IPlayer player);

        public abstract void Winner(IPlayer player);

        public List<IPlayer> GetScores()
        {
            return new List<IPlayer>(scoredPlayers);
        }

        public string GetScoreValues()
        {
            string scores = string.Empty;

            foreach (IPlayer player in scoredPlayers.OrderByDescending(p => p.Score))
            {
                scores += player.Name + ":" + player.Score + ",";
            }

            scores = scores.Remove(scores.Length - 1);

            return scores;
        }
    }
}
