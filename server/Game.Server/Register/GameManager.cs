using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game.Server.Engine.Mia;
using Game.Server.Engine.Mia.Interface;
using Game.Server.Engine.Mia.Move;
using Game.Server.Engine.Mia.Move.Interface;
using Game.Server.Network.Interface;
using Game.Server.Scoring;
using Game.Server.Register.Interface;
using Game.Server.Network;
using Game.Server.Network.Command.Interface;
using Game.Server.Network.Command;
using Game.Server.Logging;

using LiteNetLib;


namespace Game.Server.Register
{
    public class GameManager : IGameManager
    {
        private IServer server;

        private List<IGameInstance> activeGameInstances;

        private List<IGame> activeGames;

        private List<IClient> clients;

        private int serverPort;

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
            this.server = new Network.Server(this);
        }

        public GameManager(IServer commandServer)
        {
            this.server = commandServer;
        }

        public void Initialize()
        {
            activeGameInstances = new List<IGameInstance>();
            activeGames = new List<IGame>();
            clients = new List<IClient>();

            server.CreateServer(serverPort);

            // Wait to let clients register to game server
            Task.Delay(100);

            var scoreMode = ScoreMode.Points;
            Enum.TryParse(Config.Config.Settings.ScoreMode, out scoreMode);

            StartGame(scoreMode);
        }

        public void ProcessEvent(string eventMessage, NetPeer peer)
        {
            var clientCommand = new ClientCommand(eventMessage, peer);
            ProcessCommand(clientCommand);
        }

        public void SendEvent(IServerCommand serverCommand)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(serverCommand.CommandText);
            serverCommand.Peer.Send(messageBytes, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Default mode start only one game. Advanced mode is to start thousands of games. Both modes start a join session with limited time.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        private async void StartGame(ScoreMode scoreMode)
        {
            int currentRoundNumber = 0;

            while (currentRoundNumber < Config.Config.Settings.RoundsPerGame)
            {
                Log.Write($"Start game: {currentRoundNumber}");

                var game = new Engine.Mia.Game(currentRoundNumber, scoreMode, this);
                var gameInstance = new GameInstance(game.GameNumber.ToString(), game.Token);

                activeGameInstances.Add(gameInstance);
                activeGames.Add(game);

                for(int i = 0; i < clients.Count; i++)
                {
                    game.Register(new Player(clients[i].Name, false));
                }

                await game.StartAsync();
                //game.GetScore();

                currentRoundNumber += 1;
            }
        }

        public void ProcessCommand(IClientCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.CommandText) || command.Peer == null)
                return;

            var client = FindClient(command.Peer);
            string[] commandParts = command.CommandText.Split(';');

            if (client == null && commandParts.Length > 0)
            {
                string firstPart = commandParts[0];

                if (firstPart == "REGISTER" && commandParts.Length > 1)
                {
                    string playerName = commandParts[1];
                    IClient newClient = new Client(playerName, command.Peer);
                    RegisterClient(newClient);
                }
            }
            else if (client != null && commandParts.Length > 1)
            {
                string firstCommandPart = commandParts[0];

                if (firstCommandPart == "FIND_GAME" && commandParts.Length > 1)
                {
                    var gameInstance = FindGameInstance(commandParts[1].ToString());
                    // TODO: only for cluster version
                }
                else if (firstCommandPart == "JOIN_ROUND" && commandParts.Length > 1)
                {
                    Guid gameToken;
                    bool isParsed = Guid.TryParse(commandParts[1], out gameToken);
                    if (isParsed)
                    {
                        var gameInstance = FindGameInstance(gameToken);
                        if (gameInstance != null)
                            JoinGame(gameInstance, client, false);
                    }
                }
                else if (firstCommandPart == "JOIN_SPECTATOR" && commandParts.Length > 1)
                {
                    Guid gameToken;
                    bool isParsed = Guid.TryParse(commandParts[1], out gameToken);
                    if (isParsed)
                    {
                        var gameInstance = FindGameInstance(gameToken);
                        JoinGame(gameInstance, client, true);
                    }
                }
                else if (commandParts.Length > 1)
                {
                    Guid gameToken;
                    string commandValue = null;
                    string gameValue = string.Empty;
                    string gameTokenValue = string.Empty;

                    if (commandParts.Length == 2)
                    {
                        commandValue = commandParts[0];
                        gameTokenValue = commandParts[1];
                    }
                    else if (commandParts.Length == 3)
                    {
                        commandValue = commandParts[0];
                        gameValue = commandParts[1];
                        gameTokenValue = commandParts[2];
                    }

                    if (Guid.TryParse(gameTokenValue, out gameToken))
                    {
                        var game = FindGame(gameToken);

                        // TODO: Move codes should be part of Game domain only (SoC)
                        PlayerMoveCode moveCode;
                        if (game != null && Enum.TryParse(firstCommandPart, out moveCode))
                        {
                            var player = game.Players.Find(p => p.Name == client.Name);
                            if (player != null)
                            {
                                var playerMove = new PlayerMove(moveCode, gameValue, player, gameToken);
                                game.Move(playerMove);
                            }
                        }
                    }
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
            IServerCommand command;

            if (isRejected)
            {
                command = new ServerCommand(ServerCommandCode.REJECTED.ToString(), client.Peer);
                Log.Write($"{client.Name} {ServerCommandCode.REJECTED}");
            }
            else
            {
                clients.Add(client);
                command = new ServerCommand(ServerCommandCode.REGISTERED.ToString(), client.Peer);
                Log.Write($"{client.Name} {ServerCommandCode.REGISTERED}");
            }

            SendEvent(command);
        }

        public IGameInstance FindGameInstance(Guid gameToken)
        {
            return activeGameInstances.FirstOrDefault(x => x.GameToken == gameToken);
        }

        public IGameInstance FindGameInstance(string name)
        {
            return activeGameInstances.FirstOrDefault(x => x.Name == name);
        }

        public IGame FindGame(Guid gameToken)
        {
            return activeGames.FirstOrDefault(x => x.Token == gameToken);
        }

        private void JoinGame(IGameInstance gameInstance, IClient client, bool isSpectator = false)
        {
            var game = activeGames.FirstOrDefault(x => x.Token == gameInstance.GameToken);
            if (game != null)
            {
                var player = new Player(client.Name, isSpectator);
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

        public void ProcessMove(IServerMove serverMove)
        {
            for (int i = 0; i < serverMove.Players.Length; i++)
            {
                var client = clients.FirstOrDefault(x => x.Name == serverMove.Players[i].Name);
                if (client != null)
                {
                    // TODO: Domain knowledge of the game (SoC)
                    string flexibleValuePart = string.Empty;
                    
                    if (!string.IsNullOrEmpty(serverMove.Value))
                        flexibleValuePart += $"{serverMove.Value};";

                    if (serverMove.FailureReasonCode != ServerFailureReasonCode.None)
                        flexibleValuePart += $"{serverMove.FailureReasonCode};";

                    string commandMessage = $"{serverMove.Code};{flexibleValuePart}{serverMove.Token}";

                    byte[] messageBytes = Encoding.UTF8.GetBytes(commandMessage);
                    client.Peer.Send(messageBytes, DeliveryMethod.ReliableOrdered);
                }
            }            
        }
    }
}
