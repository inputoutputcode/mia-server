﻿using System;
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
using System.Net.NetworkInformation;


namespace Game.Server.Register
{
    public class GameManager : IGameManager
    {
        private IServer serverInstance;

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
            this.serverInstance = new Network.Server(this);
        }

        public GameManager(IServer serverInstance)
        {
            this.serverInstance = serverInstance;
        }

        public void Initialize()
        {
            activeGameInstances = new List<IGameInstance>();
            activeGames = new List<IGame>();
            clients = new List<IClient>();

            serverInstance.CreateServer(serverPort);

            // Wait to let clients register to game server
            Task.Delay(100);

            var scoreMode = ScoreMode.Points;
            Enum.TryParse(Config.Config.Settings.ScoreMode, out scoreMode);

            StartGame(scoreMode);
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

        public void ReceiveEvent(IClientEvent clientEvent)
        {
            if (string.IsNullOrWhiteSpace(clientEvent.Message) || clientEvent.Peer == null)
                return;

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
                            JoinGame(gameInstance, client, false);
                    }
                }
                else if (firstEventPart == "JOIN_SPECTATOR" && eventParts.Length > 1)
                {
                    Guid gameToken;
                    bool isParsed = Guid.TryParse(eventParts[1], out gameToken);
                    if (isParsed)
                    {
                        var gameInstance = FindGameInstance(gameToken);
                        JoinGame(gameInstance, client, true);
                    }
                }
                else if (eventParts.Length > 1)
                {
                    Guid gameToken;
                    string eventMessage = null;
                    string gameValue = string.Empty;
                    string gameTokenValue = string.Empty;

                    if (eventParts.Length == 2)
                    {
                        eventMessage = eventParts[0];
                        gameTokenValue = eventParts[1];
                    }
                    else if (eventParts.Length == 3)
                    {
                        eventMessage = eventParts[0];
                        gameValue = eventParts[1];
                        gameTokenValue = eventParts[2];
                    }

                    if (Guid.TryParse(gameTokenValue, out gameToken))
                    {
                        var game = FindGame(gameToken);

                        // TODO: Move codes should be part of Game domain only (SoC)
                        PlayerMoveCode moveCode;
                        if (game != null && Enum.TryParse(firstEventPart, out moveCode))
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

        public void SendEvent(IServerMove serverMove)
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

                    string eventMessage = $"{serverMove.Code};{flexibleValuePart}{serverMove.Token}";

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
                serverEvent = new ServerEvent(ServerEventCode.REJECTED.ToString(), client.Peer);
                Log.Write($"{client.Name} {ServerEventCode.REJECTED}");
            }
            else
            {
                clients.Add(client);
                serverEvent = new ServerEvent(ServerEventCode.REGISTERED.ToString(), client.Peer);
                Log.Write($"{client.Name} {ServerEventCode.REGISTERED}");
            }

            SendEventMessage(serverEvent.Message, serverEvent.Peer);
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
    }
}
