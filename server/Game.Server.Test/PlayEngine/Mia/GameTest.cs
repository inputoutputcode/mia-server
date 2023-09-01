using Moq;
using Xunit;

using Game.Server.Engine.Mia.Move;
using Game.Server.Engine.Mia.Move.Interface;
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
            gameManager.Setup(m => m.ProcessMove(It.IsAny<IServerMove>()));


            int rounds = 1;
            var game = new Engine.Mia.Game(rounds, ScoreMode.Points, gameManager.Object);

            // Act
            await game.StartAsync();

            // Assert
            gameManager.Verify(m => m.ProcessMove(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_STARTING)), Times.AtMost(2));
            gameManager.Verify(m => m.ProcessMove(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_STARTED)), Times.AtMost(2));
            gameManager.Verify(m => m.ProcessMove(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_CANCELLED)), Times.AtMost(1));
        }

        [Fact]
        public async void Game_Round_Will_Be_Cancelled_Without_ActivePlayers()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>(MockBehavior.Strict);
            gameManager.Setup(m => m.ProcessMove(It.IsAny<IServerMove>()));


            int rounds = 1;
            var game = new Engine.Mia.Game(rounds, ScoreMode.Points, gameManager.Object);

            var player1 = new Engine.Mia.Player("Player1", true);
            game.Register(player1);
            var player2 = new Engine.Mia.Player("Player2", true);
            game.Register(player2);

            // Act
            await game.StartAsync();

            // Assert
            gameManager.Verify(m => m.ProcessMove(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_STARTING)), Times.AtMost(2));
            gameManager.Verify(m => m.ProcessMove(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_STARTED)), Times.AtMost(2));
            gameManager.Verify(m => m.ProcessMove(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_CANCELLED)), Times.AtMost(1));
        }

        [Fact]
        public async void Game_Round_Will_Be_Cancelled_With_One_Player()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>(MockBehavior.Strict);
            gameManager.Setup(m => m.ProcessMove(It.IsAny<IServerMove>())).Verifiable();


            int rounds = 1;
            var game = new Engine.Mia.Game(rounds, ScoreMode.Points, gameManager.Object);

            var player1 = new Engine.Mia.Player("Player1", true);
            game.Register(player1);
            var player2 = new Engine.Mia.Player("Player2", true);
            game.Register(player2);

            var joinGamePlayer1 = new PlayerMove(PlayerMoveCode.JOIN_ROUND, player1.Name, player1, game.Token);
            var joinGamePlayer2 = new PlayerMove(PlayerMoveCode.JOIN_ROUND, player2.Name, player2, game.Token);

            // Act
            await game.StartAsync();
            game.Move(joinGamePlayer1);

            // Assert
            gameManager.Verify(m => m.ProcessMove(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_CANCELLED)));
        }

        [Fact]
        public async void Should_Send_The_First_Server_Move()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            gameManager.Setup(m => m.ProcessMove(It.IsAny<IServerMove>()));

            int rounds = 1;
            var game = new Engine.Mia.Game(rounds, ScoreMode.Points, gameManager.Object);

            var player1 = new Engine.Mia.Player("Player1", false);
            game.Register(player1);
            var player2 = new Engine.Mia.Player("Player2", false);
            game.Register(player2);

            var joinGamePlayer1 = new PlayerMove(PlayerMoveCode.JOIN_ROUND, player1.Name, player1, game.Token);
            var joinGamePlayer2 = new PlayerMove(PlayerMoveCode.JOIN_ROUND, player2.Name, player2, game.Token);

            // Act
            await game.StartAsync();

            // Assert
            gameManager.Verify(m => m.ProcessMove(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_STARTING)));
            gameManager.Verify(m => m.ProcessMove(It.Is<IServerMove>(x => x.Token != game.Token)));
            gameManager.Verify(m => m.ProcessMove(It.Is<IServerMove>(x => x.FailureReasonCode == ServerFailureReasonCode.None)));
            gameManager.Verify(m => m.ProcessMove(It.Is<IServerMove>(x => x.Value == string.Empty)));
            gameManager.Verify(m => m.ProcessMove(It.Is<IServerMove>(x => x.Players.Length == 2)));
        }

        [Fact]
        public void See_Should_Raise_Lost_For_Current_Player_If_Dice_Higher_Or_Equal()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void See_Should_Raise_Win_For_Current_Player_If_Dice_Lower()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }


        [Fact]
        public void See_Should_Raise_Mia_For_Current_Player_If_Dice_Is_Mia()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
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

        [Fact]
        public void Template()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }
    }
}
