using System;
using System.Net;
using Xunit;
using Mia.Server.Game.PlayEngine.Move;


namespace Mia.Server.Game.Communication.Test
{
    /// <summary>
    /// Client commands:
    /// 
    /// REGISTER;name
    /// [FIND_GAME;name]
    /// [CREATE_GAME;name]
    /// JOIN_SPECTATOR;name;token
    /// JOIN_GAME;name;token
    /// 
    /// Player moves:
    /// 
    /// JOIN_ROUND;name;token
    /// SEE;token
    /// ROLL;token
    /// ANNOUNCE;dice;token
    /// 
    /// Server commands:
    /// 
    /// REGISTERED
    /// REJECTED
    /// [FOUND_GAME]
    /// [GAME_CREATED_JOINED;]
    /// [GAME_JOINED]
    /// [NOT_FOUND]
    /// 
    /// Game moves:
    /// 
    /// GAME_STARTING;token
    /// GAME_STARTED;players;token
    /// GAME_CANCELLED;NO_PLAYERS
    /// GAME_CANCELLED;ONLY_ONE_PLAYER
    /// YOUR_TURN;token
    /// ROLLED;dice;token
    /// ANNOUNCED;name;dice;token 
    /// PLAYER_WANTS_TO_SEE;name;token
    /// ACTUAL_DICE;dice;token
    /// PLAYER_ROLLS;name;token 
    /// PLAYER_LOST;name;reason;token
    /// PLAYER_LOST;names;reason;token
    /// PLAYER_LOST;name;reason;token
    /// SCORE;playerpoints;token
    /// 
    /// Server reason code:
    /// 
    /// SEE_BEFORE_FIRST_ROLL
    /// LIED_ABOUT_MIA
    /// ANNOUNCED_LOSING_DICE
    /// DID_NOT_ANNOUNCE
    /// DID_NOT_TAKE_TURN
    /// INVALID_TURN
    /// SEE_FAILED
    /// CAUGHT_BLUFFING
    /// MIA
    /// 
    /// [x] reserved for online version
    /// </summary>
    public class CommandServerTest
    {



        [Fact]
        public void Client_Sends_Register()
        {
            // Arrange
            var commandServer = new CommandServer();

            var udpState = new UdpState();
            udpState.EndPoint = new IPEndPoint(IPAddress.Parse("10.10.4.6"), 1001);
            udpState.UdpClient = null;

            string playerName = "Player1";
            string message = $"{PlayerMoveCode.REGISTER};{playerName}";

            // Act
            var clientCommand = commandServer.ConvertMessage(message, udpState);

            // Assert
            Assert.True(clientCommand.Code == PlayerMoveCode.REGISTER);
            Assert.True(clientCommand.Value == playerName);
            Assert.True(clientCommand.Client == );
        }

        [Fact]
        public void Client_Sends_Join_Game()
        {
            // Arrange
            var commandServer = new CommandServer();

            var udpState = new UdpState();
            udpState.EndPoint = new IPEndPoint(IPAddress.Parse("10.10.4.6"), 1001);
            udpState.UdpClient = null;

            string playerName = "Player1";
            Guid gameToken = Guid.NewGuid();
            string message = $"{PlayerMoveCode.JOIN_GAME};{playerName};{gameToken}";

            // Act
            var clientCommand = commandServer.ConvertMessage(message, udpState);

            // Assert
            Assert.True(clientCommand.Code == PlayerMoveCode.JOIN_GAME);
            Assert.True(clientCommand.Value == playerName);
            Assert.True(clientCommand.Client == );
            Assert.True(clientCommand.GameToken == gameToken);
        }

        [Fact]
        public void Scoring_For_Money()
        {
            // Arrange

            // Act

            // Assert
        }

    }
}
