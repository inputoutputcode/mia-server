using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game.Server.Engine.Mia;
using Game.Server.Engine.Mia.Interface;
using Game.Server.Engine.Mia.Move;
using Game.Server.Network.Interface;
using Game.Server.Scoring;
using Game.Server.Register.Interface;
using Game.Server.Network;
using Game.Server.Network.Event.Interface;
using Game.Server.Network.Event;
using Game.Server.Logging;

using LiteNetLib;


namespace Game.Server.Register
{
    public class GameManager : IGameManager
    {
        private IServer serverInstance;
        private List<IGameInstance> activeGameInstances;
        private List<IGame> activeGames;
        private List<IClient> clients;
        private int serverPort;

        private IGameInstance currentGameInstance;
        private IGame currentGame;

        private static readonly object lockObject = new object();

        public List<IGameInstance> ActiveGamesInstances
        {
            // TODO: return a clone
            get { return activeGameInstances; }
        }

        public int ServerPort
        {
            set { serverPort = value; }
        }

        public GameManager()
        {
            this.serverInstance = new Network.Server(this);
            Initialize();
        }

        public GameManager(IServer serverInstance)
        {
            this.serverInstance = serverInstance;
            Initialize();
        }

        private void Initialize()
        {
            activeGameInstances = new List<IGameInstance>();
            activeGames = new List<IGame>();
            clients = new List<IClient>();
        }

        public void Start()
        {
            serverInstance.CreateServer(serverPort);

            // Wait to let clients register to game server
            Task.Delay(Config.Config.Settings.InitializeDelayTime);

            var scoreMode = ScoreMode.Points;
            Enum.TryParse(Config.Config.Settings.ScoreMode, out scoreMode);

            StartGame(scoreMode);
        }

        public void AddGame(IGame game)
        {
            activeGames.Add(game);
        }

        public void ReceiveEventMessage(string eventMessage, NetPeer peer)
        {
            var clientEvent = new ClientEvent(eventMessage, peer);
            ReceiveEvent(clientEvent);
        }

        public void SendEventMessage(string eventMessage, NetPeer peer)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(eventMessage);
            peer.Send(messageBytes, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Default mode start only one game. Advanced mode is to start thousands of games. Both modes start a join session with limited time.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        private async void StartGame(ScoreMode scoreMode)
        {
            int currentRoundNumber = 0;
            var gameScorer = GameScoreFactory.Create(scoreMode);

            while (currentRoundNumber < Config.Config.Settings.RoundsPerGame)
            {
                Log.Write($"Start game: {currentRoundNumber}");

                currentGame = new Engine.Mia.Game(currentRoundNumber, scoreMode, this, null, gameScorer);
                currentGameInstance = new GameInstance(currentGame.GameNumber.ToString(), currentGame.Token);

                for(int i = 0; i < clients.Count; i++)
                {
                    // BUG?: isSpectator=false 
                    currentGame.Register(new Player(clients[i].Name, false, clients[i].Peer.EndPoint.Address.ToString()));
                }

                await currentGame.StartAsync();
                currentRoundNumber += 1;
            }
        }

        public void ReceiveEvent(IClientEvent clientEvent)
        {
            if (string.IsNullOrWhiteSpace(clientEvent.Message) || clientEvent.Peer == null)
                return;

            lock (lockObject)
            {
                var client = FindClient(clientEvent.Peer);
                string[] eventParts = clientEvent.Message.Split(';');

                if (client == null && eventParts.Length > 0)
                {
                    string firstPart = eventParts[0];

                    if (firstPart == "REGISTER" && eventParts.Length > 1)
                    {
                        string playerName = eventParts[1];
                        IClient newClient = new Client(playerName, clientEvent.Peer);
                        RegisterClient(newClient);
                    }
                }
                else if (client != null && eventParts.Length > 1)
                {
                    string firstEventPart = eventParts[0];

                    if (firstEventPart == "FIND_GAME" && eventParts.Length > 1)
                    {
                        var gameInstance = FindGameInstance(eventParts[1].ToString());
                        // TODO: only for cluster version
                    }
                    else if (firstEventPart == "JOIN_ROUND" && eventParts.Length > 1)
                    {
                        Guid gameToken;
                        bool isParsed = Guid.TryParse(eventParts[1], out gameToken);
                        if (isParsed)
                        {
                            var gameInstance = FindGameInstance(gameToken);
                            if (gameInstance != null)
                            {
                                Log.Write($"{client.Name}: {ClientMoveCode.JOIN_ROUND};{gameToken}");
                                JoinGame(gameInstance, client, false);
                            }
                        }
                    }
                    else if (firstEventPart == "JOIN_SPECTATOR" && eventParts.Length > 1)
                    {
                        Guid gameToken;
                        bool isParsed = Guid.TryParse(eventParts[1], out gameToken);
                        if (isParsed)
                        {
                            var gameInstance = FindGameInstance(gameToken);
                            if (gameInstance != null)
                            {
                                Log.Write($"{client.Name}: {ClientEventCode.JOIN_SPECTATOR};{gameToken}");
                                JoinGame(gameInstance, client, true);
                            }
                        }
                    }
                    else if (eventParts.Length > 1)
                    {
                        Guid gameToken;
                        string eventMessage = null;
                        string eventValue = string.Empty;
                        string gameTokenValue = string.Empty;

                        if (eventParts.Length == 2)
                        {
                            eventMessage = eventParts[0];
                            gameTokenValue = eventParts[1];
                        }
                        else if (eventParts.Length == 3)
                        {
                            eventMessage = eventParts[0];
                            eventValue = eventParts[1];
                            gameTokenValue = eventParts[2];
                        }

                        if (Guid.TryParse(gameTokenValue, out gameToken))
                        {
                            var game = FindGame(gameToken);
                            if (game != null)
                            {
                                var player = game.Players.Find(p => p.Name == client.Name);
                                if (player != null)
                                {
                                    Log.Write($"{player.Name}: {eventMessage};{eventValue};{gameToken}");

                                    game.ReceiveClientEvent(eventMessage, eventValue, player, gameToken);
                                }
                            }
                            else
                            {
                                Log.Write($"Game not found: {client.Name}: {eventMessage};{eventValue};{gameToken}");
                            }
                        }
                    }
                }
            }
        }

        public void SendEvent(string eventMessage, IPlayer[] players)
        {
            Log.Write($"{eventMessage} - {string.Join(",", players.Select(p => p.Name))} ");

            for (int i = 0; i < players.Length; i++)
            {
                var client = clients.FirstOrDefault(x => x.Name == players[i].Name);
                if (client != null)
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(eventMessage);
                    client.Peer.Send(messageBytes, DeliveryMethod.ReliableOrdered);
                }
            }
        }

        public IClient FindClient(NetPeer peer)
        {
            // TODO: Performance?
            return clients.FirstOrDefault(x => x.Peer == peer);
        }

        public IClient FindClient(string playerName)
        {
            // TODO: Performance?
            return clients.FirstOrDefault(x => x.Name == playerName);
        }

        public void RegisterClient(IClient client)
        {
            var isRejected = clients.Exists(x => x.Name == client.Name || x.Peer.EndPoint.Address == client.Peer.EndPoint.Address);
            IServerEvent serverEvent;

            if (isRejected)
            {
                Log.Write($"{client.Name} {ServerEventCode.REJECTED}");
                serverEvent = new ServerEvent(ServerEventCode.REJECTED.ToString(), client.Peer);
            }
            else
            {
                Log.Write($"{client.Name} {ServerEventCode.REGISTERED}");
                clients.Add(client);
                serverEvent = new ServerEvent(ServerEventCode.REGISTERED.ToString(), client.Peer);
            }

            SendEventMessage(serverEvent.Message, serverEvent.Peer);
        }

        public IGameInstance FindGameInstance(Guid gameToken)
        {
            if (currentGameInstance.GameToken == gameToken)
                return currentGameInstance;

            return null;
            //return activeGameInstances.FirstOrDefault(x => x.GameToken == gameToken);
        }

        public IGameInstance FindGameInstance(string name)
        {
            if (currentGameInstance.Name == name)
                return currentGameInstance;

            return null;
            //return activeGameInstances.FirstOrDefault(x => x.Name == name);
        }

        public IGame FindGame(Guid gameToken)
        {
            if (currentGame.Token == gameToken)
                return currentGame;

            return null;

            // BUG: Exception when game added but still bot sends message to old game
            //return activeGames.FirstOrDefault(x => x.Token == gameToken);
        }

        private void JoinGame(IGameInstance gameInstance, IClient client, bool isSpectator = false)
        {
            var game = activeGames.FirstOrDefault(x => x.Token == gameInstance.GameToken);
            if (game != null)
            {
                var player = new Player(client.Name, isSpectator, client.Peer.EndPoint.Address.ToString());
                game.Register(player);
            }
        }

        public bool ValidateName(string playerName)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(playerName))
            {
                isValid = false;
            }
            else if (playerName.Length > 20)
            {
                isValid = false;
            }
            else if (!playerName.All(n => (Char.IsLetterOrDigit(n) || n == '_')))
            {
                isValid = false;
            }

            return isValid;
        }
    }
}
