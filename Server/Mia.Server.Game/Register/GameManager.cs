using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Mia.Server.Game.Communication.Interface;
using Mia.Server.Game.Interface;
using Mia.Server.Game.Scoring;
using Mia.Server.Game.Register.Interface;
using Mia.Server.Game.Communication;
using Mia.Server.Game.Communication.Command.Interface;
using Mia.Server.Game.Communication.Command;
using Mia.Server.Game.PlayEngine;
using Mia.Server.Game.PlayEngine.Move;
using Mia.Server.Game.PlayEngine.Move.Interface;
using Mia.Server.Game.Monitoring;
using Mia.Server.Game.Configuration;


namespace Mia.Server.Game.Register
{
    public class GameManager : IGameManager
    {
        private ICommandServer commandServer;

        private List<IGameInstance> activeGameInstances;

        private List<IGame> activeGames;

        private List<IClient> clients;

        public List<IGameInstance> ActiveGames
        {
            // TODO: return a clone
            get { return activeGameInstances; }
        }

        public GameManager(int listenPort)
        {
            this.commandServer = new CommandServer(listenPort);

            Initialize();
        }

        public GameManager(ICommandServer commandServer)
        {
            Initialize();

            this.commandServer = commandServer;
        }

        private async void Initialize()
        {
            activeGameInstances = new List<IGameInstance>();
            activeGames = new List<IGame>();
            clients = new List<IClient>();

            var scoreMode = ScoreMode.Points;
            Enum.TryParse(Config.Settings.ScoreMode, out scoreMode);

            var gameTask = StartGameAsync(scoreMode);
            var clientCommandsTask = ReceiveClientCommands();

            await Task.WhenAny(clientCommandsTask, gameTask);
        }

        private async Task ReceiveClientCommands()
        {
            while (true)
            {
                var command = commandServer.GetClientCommand();
                if (command != null)
                    ProcessCommand(command);
            }
        }

        /// <summary>
        /// Default mode start only one game. Advanced mode is to start thousands of games. Both modes start a join session with limited time.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        private async Task StartGameAsync(ScoreMode scoreMode)
        {
            int currentRoundNumber = 1;

            while (currentRoundNumber <= Config.Settings.RoundsPerGame)
            {
                var game = new PlayEngine.Game(currentRoundNumber, scoreMode, this);
                var gameInstance = new GameInstance(game.GameNumber.ToString(), game.Token);

                activeGameInstances.Add(gameInstance);
                activeGames.Add(game);

                await game.StartAsync();
                //game.GetScore();

                currentRoundNumber += 1;
            }
        }

        public void ProcessCommand(IClientCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.CommandText) || command.EndPoint == null)
                return;

            var client = FindClient(command.EndPoint);
            string[] commandParts = command.CommandText.Split(';');

            if (client == null && commandParts.Length > 0)
            {
                string firstPart = commandParts[0];

                if (firstPart == "REGISTER" && commandParts.Length > 1)
                {
                    string playerName = commandParts[1];
                    IClient newClient = new Client(playerName, command.EndPoint);
                    RegisterClient(newClient);
                }
            }
            else if (client == null && commandParts.Length > 1)
            {
                string firstCommandPart = commandParts[0];

                if (firstCommandPart == "FIND_GAME" && commandParts.Length > 1)
                {
                    var gameInstance = FindGameInstance(commandParts[1].ToString());
                    // TODO: only for cluster version
                }
                else if (firstCommandPart == "JOIN_GAME" && commandParts.Length > 1)
                {
                    Guid gameToken;
                    bool isParsed = Guid.TryParse(commandParts[1], out gameToken);
                    if (isParsed)
                    {
                        var gameInstance = FindGameInstance(gameToken);
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
                    string gameTokenValue;

                    if (commandParts.Length > 2)
                    {
                        commandValue = commandParts[1];
                        gameTokenValue = commandParts[2];
                    }
                    else
                    {
                        gameTokenValue = commandParts[1];
                    }

                    if (Guid.TryParse(gameTokenValue, out gameToken))
                    {
                        var game = FindGame(gameToken);

                        // TODO: Move codes should be part of Game domain only (SoC)
                        PlayerMoveCode moveCode;
                        if (Enum.TryParse(firstCommandPart, out moveCode))
                        {
                            var player = game.Players.Find(p => p.Name == client.Name);
                            if (player != null)
                            {
                                var playerMove = new PlayerMove(moveCode, commandValue, player, gameToken);
                                game.Move(playerMove);
                            }
                        }
                    }
                }
            }
        }

        public IClient FindClient(IPEndPoint endPoint)
        {
            // TODO: Performance?
            return clients.FirstOrDefault(x => x.EndPoint == endPoint);
        }

        public IClient FindClient(string playerName)
        {
            // TODO: Performance?
            return clients.FirstOrDefault(x => x.Name == playerName);
        }

        public void RegisterClient(IClient client)
        {
            var isRejected = clients.Exists(x => x.Name == client.Name || x.EndPoint.Address == client.EndPoint.Address);
            IServerCommand command;

            if (isRejected)
            {
                command = new ServerCommand(ServerCommandCode.REJECTED.ToString(), client.EndPoint);
                Log.Message($"{client.Name} {ServerCommandCode.REJECTED}");
            }
            else
            {
                clients.Add(client);
                command = new ServerCommand(ServerCommandCode.REGISTERED.ToString(), client.EndPoint);
                Log.Message($"{client.Name} {ServerCommandCode.REGISTERED}");
            }

            commandServer.SendCommand(command);
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
            else if (playerName.All(n => Char.IsLetterOrDigit(n) || n == '_'))
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
                    
                    if (string.IsNullOrEmpty(serverMove.Value))
                        flexibleValuePart += $"{serverMove.Value};";

                    if (serverMove.FailureReasonCode != ServerFailureReasonCode.None)
                        flexibleValuePart += $"{serverMove.FailureReasonCode};";

                    string commandValue = $"{serverMove.Code};{flexibleValuePart}{serverMove.Token}";
                    commandServer.SendCommand(new ServerCommand(commandValue, client.EndPoint));
                }
            }            
        }
    }
}
