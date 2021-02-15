using System;
using System.Collections.Generic;
using System.Net;
using Mia.Server.Game.Communication.Interface;
using Mia.Server.Game.Interface;
using Mia.Server.Game.Scoring;
using Mia.Server.Game.Register.Interface;
using Mia.Server.Game.Communication;
using Mia.Server.Game.Communication.Command.Interface;
using Mia.Server.Game.Communication.Command;


namespace Mia.Server.Game.Register
{
    public class GameManager : IGameManager
    {
        private ICommandServer commandServer;

        private List<IGame> activeGames;

        public List<IClient> clients;

        public ICommandServer CommandServer
        { 
            get 
            { 
                return commandServer; 
            } 
        }

        public List<IGame> ActiveGames
        {
            // TODO: return a clone
            get { return activeGames;  }
        }

        public GameManager()
        {
            this.commandServer = new CommandServer();
            StartGame("Party Game", 1, ScoreMode.Points);

            while (true)
            {
                var command = commandServer.GetClientCommand();
                ProcessCommand(command);
            }
        }

        public GameManager(ICommandServer commandServer)
        {
            this.commandServer = commandServer;
        }

        /// <summary>
        /// Default mode start only one game. Advanced mode is to start thousands of games. Both modes start a join session with limited time.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        public void StartGame(string name, int rounds, ScoreMode mode)
        {
            activeGames.Add(new PlayEngine.Game(name, rounds, mode));
        }

        public void ProcessCommand(ClientCommand command)
        {
            var client = FindClient(command.EndPoint);
            if (client == null)
            {

            }
            else 
            {


                switch (command.)
            }

            
        }

        public void JoinGame(IClient client, bool singleGameMode = true)
        {
            clients.Add(client);
        }

        public void JoinRound(string name, IClient client)
        { 
        
        }

        public IClientCommand ConvertCommand(string message, IPEndPoint endPoint)
        {
            var clientCommand = new ClientCommand();
            string[] messageParts = message.Split(';');

            if (messageParts.Length > 0)
            {
                string commandPart = messageParts[0];

                ClientCommandCode commandCode;
                bool isCommandParsed = Enum.TryParse(commandPart, out commandCode);

                if (isCommandParsed && messageParts.Length > 1)
                {
                    string commandValue = string.Empty;
                    var gameToken = Guid.Empty;
                    string valuePart = messageParts[1];
                    string gameTokenPart = messageParts[2];

                    switch (commandCode)
                    {
                        case ClientCommandCode.REGISTER:
                        case ClientCommandCode.REGISTER_SPECTATOR:
                            commandValue = valuePart;
                            break;

                        case ClientCommandCode.SEE:
                        case ClientCommandCode.ROLL:
                            Guid.TryParse(gameTokenPart, out gameToken);
                            break;

                        case ClientCommandCode.JOIN_GAME:
                        case ClientCommandCode.ANNOUNCE:
                            commandValue = valuePart;
                            Guid.TryParse(gameTokenPart, out gameToken);
                            break;
                    }

                    clientCommand = new ClientCommand(commandCode, commandValue, endPoint, gameToken);
                }
            }

            return clientCommand;
        }



        public event EventHandler<CustomEventArgs> SomethingHappened = delegate { };

        protected virtual void OnSomethingHappened(CustomEventArgs e)
        {
            SomethingHappened(this, e);
        }

    }

    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
