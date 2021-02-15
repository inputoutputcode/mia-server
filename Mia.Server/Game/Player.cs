using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

using Mia.Server.Command;
using Mia.Server.Messaging;


namespace Mia.Server
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
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

        public void ChangeState(PlayerState state)
        {
            currentState = state;
        }

        public void YourTurn(Guid turnToken)
        {
            string command = ServerCommand.YOUR_TURN.ToString() + ";" + turnToken;

            messageServer.SendAndWaitForAnswer(command, turnToken, udpState);
        }

        public string GetDebuggerDisplay()
        {
            return this.Name;
        }

        #endregion Methods
    }
}
