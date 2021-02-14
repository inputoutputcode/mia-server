using System;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

using Mia.Server.Logging;
using Mia.Server.Command;
using Mia.Server.Messaging;


namespace Mia.Server
{
    /// <summary>
    /// This class handle all the game rules without messaging and game rounds.
    /// </summary>
    public class MiaGameServer
    {
        #region Members

        private MessageServer messageServer;
        private List<Player> registeredPlayers;
        private MiaGame currentGame;
        private ScoreList playerScores;

        #endregion Members


        #region Properties

        public List<Player> RegisteredPlayers
        {
            get { return registeredPlayers; }
        }

        #endregion Properties


        #region Constructor

        public MiaGameServer(MessageServer messageServer)
        {
            messageServer.MiaServerInstance = this;
            this.messageServer = messageServer;
            this.registeredPlayers = new List<Player>();

            Logger.Log("The server is online.");

            // start registration phase
            messageServer.WaitForRegistration();

            // init score list
            var playerList = new PlayerList(registeredPlayers);
            playerScores = new ScoreList(playerList);

            // loop for max game round numbers
            while (playerScores.GameCount < Config.RoundsPerRun)
            {
                currentGame = new MiaGame(messageServer, playerScores);
                currentGame.Start();
            }
        }

        #endregion Constructor


        #region Methods

        private Player FindPlayer(UdpState udpState)
        {
            Player player = null;

            if (currentGame != null && currentGame.Players.RealPlayers.Count > 0)
            {
                player = currentGame.Players.RealPlayers.Find(p => p.UdpState.EndPoint.Address.Equals(udpState.EndPoint.Address));
            }

            return player;
        }

        private Player FindPlayer(string playerName)
        {
            Player player = null;

            if (registeredPlayers.Count > 0)
            {
                player = registeredPlayers.Find(p => p.Name == playerName);
            }

            return player;
        }

        /// <summary>
        /// Check the name of player for duplicates and other rules.
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="endPoint"></param>
        /// <param name="isSpectator"></param>
        public void HandleRegistration(string playerName, UdpState udpState, bool isSpectator)
        {
            if (IsPlayerNameNotValid(playerName))
            {
                string commandText = ServerCommand.REJECTED + ";" + ServerCommandArguments.INVALID_NAME;
                messageServer.SendInfo(commandText, udpState);
                Logger.Log(commandText);
            }
            else if (IsPlayerNameTakenByAnotherPlayer(playerName))
            {
                messageServer.SendInfo(ServerCommand.REJECTED + ";" + ServerCommandArguments.NAME_ALREADY_TAKEN, udpState);
            }
            else
            {
                registeredPlayers.Add(new Player(playerName, udpState, isSpectator, messageServer));
                messageServer.SendInfo(ServerCommand.REGISTERED.ToString(), udpState);
            }
        }

        /// <summary>
        /// Check if the name is used by an other registered player.
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        private bool IsPlayerNameTakenByAnotherPlayer(string playerName)
        {
            return registeredPlayers.Any(p => p.Name == playerName);
        }

        /// <summary>
        /// Check the player name for whitespaces, maximum of 20 sign, letters which are not alphanumeric or underscore.
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        private bool IsPlayerNameNotValid(string playerName)
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

        public void HandleReceivedRawMessage(string message, UdpState udpState)
        {
            Logger.Log(string.Format("Received Message: \"{0}\" from {1}:{2}", message, udpState.EndPoint.Address, udpState.EndPoint.Port));

            string[] messageParts = message.Split(';');
            string command = messageParts[0];
            string[] args = messageParts.Skip(1).ToArray();

            ClientCommand clientCommand;
            if (Enum.TryParse(command, out clientCommand))
            {
                HandleMessage(clientCommand, args, udpState);
            }
            else
            {
                var player = FindPlayer(udpState);
                currentGame.InvalidTurn(player);
            }
        }

        public void HandleMessage(ClientCommand command, string[] args, UdpState udpState)
        {
            Guid senderToken;
            var player = FindPlayer(udpState);

            switch (command)
            {
                case ClientCommand.REGISTER:
                case ClientCommand.REGISTER_SPECTATOR:

                    string playerName = args[0];
                    bool isSpectator = command == ClientCommand.REGISTER_SPECTATOR;
                    HandleRegistration(playerName, udpState, isSpectator);
                    break;

                case ClientCommand.JOIN:

                    if (Guid.TryParse(args[0], out senderToken))
                    {
                        if (senderToken == currentGame.Token)
                        {
                            currentGame.Players.Add(player);
                        }
                    }
                    else
                    {
                        currentGame.InvalidTurn(player);
                    }
                    
                    break;

                case ClientCommand.ROLL:
                case ClientCommand.SEE:

                    if (Guid.TryParse(args[0], out senderToken))
                    {
                        if (currentGame.Turn.Token == senderToken)
                        {
                            switch (command)
                            {
                                case ClientCommand.ROLL:
                                    currentGame.Roll(player);
                                    break;

                                case ClientCommand.SEE:
                                    currentGame.See(player);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        currentGame.InvalidTurn(player);
                    }

                    break;

                case ClientCommand.ANNOUNCE:

                    bool isValidCommand = false;
                    if (Guid.TryParse(args[1], out senderToken))
                    {
                        if (currentGame.Turn.Token == senderToken && currentGame.Turn.RollCount > 0)
                        {
                            string diceValue = args[0];
                            int diceOne, diceTwo;
                            if (int.TryParse(diceValue[0].ToString(), out diceOne) && int.TryParse(diceValue[1].ToString(), out diceTwo))
                            {
                                var announcedDice = new Dice(diceOne, diceTwo);
                                currentGame.Announce(player, announcedDice);

                                isValidCommand = true;
                            }
                        }
                    }

                    if (!isValidCommand)
                    {
                        currentGame.InvalidTurn(player);
                    }
                    break;

                default:
                    currentGame.InvalidTurn(player);
                    break;
            }
        }

        #endregion Methods
    }
}
