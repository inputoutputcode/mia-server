using Moq;
using Xunit;

using Game.Server.Engine.Mia;
using Game.Server.Engine.Mia.Interface;
using Game.Server.Engine.Mia.Move;
using Game.Server.Engine.Mia.Move.Interface;
using Game.Server.Network.Event.Interface;
using Game.Server.Scoring;
using Game.Server.Register.Interface;


namespace Game.Server.Test.PlayEngine.Mia
{
    public class GameTest
    {
        [Fact]
        public async void Game_Round_Will_Be_Cancelled_Without_Players()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>(MockBehavior.Strict);
            gameManager.Setup(m => m.SendEvent(It.IsAny<string>(), It.IsAny<IPlayer[]>()));


            int rounds = 1;
            var game = new Mock<Engine.Mia.Game>(rounds, ScoreMode.Points, gameManager.Object, true) { CallBase = true };
            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()));

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_STARTING)), Times.Never);
        }

        [Fact]
        public async void Game_Round_Will_Be_Cancelled_Without_ActivePlayers()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>(MockBehavior.Strict);
            gameManager.Setup(m => m.ReceiveEvent(It.IsAny<IClientEvent>()));
            gameManager.Setup(m => m.SendEvent(It.IsAny<string>(), It.IsAny<IPlayer[]>()));

            int rounds = 1;
            var game = new Mock<Engine.Mia.Game>(rounds, ScoreMode.Points, gameManager.Object, true) { CallBase = true };
            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()));

            var player1 = new Player("Player1", true);
            game.Object.Register(player1);
            var player2 = new Player("Player2", true);
            game.Object.Register(player2);

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_CANCELLED)), Times.Once);
        }

        [Fact]
        public async void Game_Round_Will_Be_Cancelled_With_One_Player()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>(MockBehavior.Strict);
            gameManager.Setup(m => m.ReceiveEvent(It.IsAny<IClientEvent>()));
            gameManager.Setup(m => m.SendEvent(It.IsAny<string>(), It.IsAny<IPlayer[]>()));

            int rounds = 1;
            var game = new Mock<Engine.Mia.Game>(rounds, ScoreMode.Points, gameManager.Object, true) { CallBase = true };
            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()));

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", true);
            game.Object.Register(player2);

            // Act
            await game.Object.StartAsync();
            game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);

            // Assert
            //TODO: No server code because round is not starting
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_CANCELLED)), Times.Once);
        }

        [Fact]
        public async void Should_Send_The_First_Server_Move()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            gameManager.Setup(m => m.SendEvent(It.IsAny<string>(), It.IsAny<IPlayer[]>()));
            gameManager.Setup(m => m.ReceiveEvent(It.IsAny<IClientEvent>()));

            int rounds = 1;
            var game = new Mock<Engine.Mia.Game>(rounds, ScoreMode.Points, gameManager.Object, true) { CallBase = true };
            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()));

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", false);
            game.Object.Register(player2);

            // Act
            await game.Object.StartAsync();
            game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
            game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.YOUR_TURN)), Times.Once);
        }

        [Fact]
        public async void See_Should_Raise_Lost_For_Current_Player_If_Dice_Higher_Or_Equal()
        {
            
        }

        [Fact]
        public async void See_Should_Raise_Win_For_Current_Player_If_Dice_Lower_On_See()
        {
            
        }


        [Fact]
        public async void Server_Should_Raise_Mia_For_Current_Player_If_Dice_Is_Mia_From_First_Roll()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            gameManager.Setup(m => m.SendEvent(It.IsAny<string>(), It.IsAny<IPlayer[]>()));
            gameManager.Setup(m => m.ReceiveEvent(It.IsAny<IClientEvent>()));

            var dice = new Mock<Dice>() { CallBase = true };
            //dice.Setup(d => d.Shake()).Callback(() => dice.Object.SetDices(1, 2));

            int rounds = 1;
            var game = new Mock<Engine.Mia.Game>(rounds, ScoreMode.Points, gameManager.Object, dice.Object, true) { CallBase = true };
            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()));

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", false);
            game.Object.Register(player2);

            // Act
            await game.Object.StartAsync();
            game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
            game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);

            game.Object.ReceiveClientEvent(ClientMoveCode.ROLL.ToString(), string.Empty, player1, game.Object.Token);

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.PLAYER_LOST)), Times.Once);
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.FailureReasonCode == ServerFailureReasonCode.MIA)), Times.Once);
        }

        [Fact]
        public void Lie_About_Mia()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Player_Commanded_Invalid_Turn_Third_Roll()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Player_Commanded_During_Inactive_Turn()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Player_Commanded_Announce_After_Second_Roll()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Invalid_Turn_Player_Wants_To_See_During_First_Turn()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Invalid_Turn_Player_Announced_Lower_Dice()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }
    }
}
