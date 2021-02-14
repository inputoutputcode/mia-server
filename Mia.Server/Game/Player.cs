using System;
using System.Net;
using System.Net.Sockets;

using Mia.Server.Command;
using Mia.Server.Messaging;


namespace Mia.Server
{
    public class Player
    {
        #region Members

        private PlayerState currentState;
        private string name;
        private bool isSpectator;
        private UdpState udpState;
        private MessageServer messageServer;

        #endregion Members


        #region Properties

        public bool IsSpectator 
        {
            get { return isSpectator; } 
        }

        public PlayerState CurrentState 
        {
            get { return currentState; } 
        }

        public string Name
        {
            get { return name; }
        }

        public UdpState UdpState
        {
            get { return udpState; }
        }

        public int Score { get; set; }

        #endregion Properties


        #region Constructor

        public Player(string name, UdpState udpState, bool isSpectator, MessageServer messageServer)
        {
            this.name = name;
            this.udpState = udpState;
            this.isSpectator = isSpectator;
            this.messageServer = messageServer;

            currentState = new PlayerState();
        }

        #endregion Constructor


        #region Methods

        public void HandleMessage(ClientCommand command, string[] args)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(ServerCommand command, string arguments)
        {
            string message = command.ToString() + ";#" + arguments;
            
            throw new NotImplementedException();
        }

        public void Registered()
        {
            SendMessage(ServerCommand.REGISTERED, string.Empty);
        }

        public void RegistrationRejected(ServerCommandArguments commandArguments)
        {
            SendMessage(ServerCommand.REJECTED, commandArguments.ToString());
        }

        public void ChangeState(PlayerState state)
        {
            currentState = state;
        }

        public void WillJoinRound()
        {
            Guid tokenId = Guid.NewGuid();

            UdpClient udpClient = new UdpClient();
            //SendMessage(ServerCommand.ROUND_STARTED, 

            //SendMessage(ServerCommand.ROUND_STARTING, tokenId.ToString());
        }

        public void RoundCanceled(ServerCommandArguments commandArguments)
        {
            throw new NotImplementedException();
        }

        public void YourTurn(Guid turnToken)
        {
            string command = ServerCommand.YOUR_TURN.ToString() + ";" + turnToken;

            messageServer.SendAndWaitForAnswer(command, turnToken, udpState);
        }

        public void PlayerLost(Player[] losingPlayers, Command.FailureCommand failureCommand)
        {
            throw new NotImplementedException();
        }

        internal void CurrentScore(string scores)
        {
            throw new NotImplementedException();
        }

        #endregion Methods


    }
}
