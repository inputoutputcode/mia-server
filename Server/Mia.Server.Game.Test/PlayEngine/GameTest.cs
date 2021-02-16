using Moq;
using Xunit;
using Mia.Server.Game.Scoring;
using Mia.Server.Game.PlayEngine.Move;
using Mia.Server.Game.Register.Interface;
using Mia.Server.Game.PlayEngine.Move.Interface;
using Mia.Server.Game.Interface;


namespace Mia.Server.Game.PlayEngine.Test
{
    public class GameTest
    {


        [Fact]
        public void Game_Round_Will_Be_Cancelled_Without_Players()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            gameManager.Setup(m => m.ProcessMove(It.Is<IServerMove>()));
            

            int rounds = 1;
            var game = new Game("Game1", rounds, ScoreMode.Points, gameManager.Object);

            var player1 = new Player("Player1", true);
            game.JoinGame(player1);
            var player2 = new Player("Player2", true);
            game.JoinGame(player2);

            var joinGamePlayer1 = new PlayerMove(PlayerMoveCode.JOIN_ROUND, player1.Name, player1, game.Token);
            var joinGamePlayer2 = new PlayerMove(PlayerMoveCode.JOIN_ROUND, player2.Name, player2, game.Token);

            // Act

            // Assert
            gameManager.Verify(m => m.ProcessMove(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_CANCELLED)));
        }

        [Fact]
        public void Game_Round_Will_Be_Cancelled_With_One_Player()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Should_Send_The_First_Server_Move()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            gameManager.Setup(m => m.ProcessMove(It.IsAny<IServerMove>()));

            int rounds = 1;
            IGame game = new Game("Game1", rounds, ScoreMode.Points, gameManager.Object);

            var player1 = new Player("Player1", false);
            game.JoinGame(player1);
            var player2 = new Player("Player2", false);
            game.JoinGame(player2);

            var joinGamePlayer1 = new PlayerMove(PlayerMoveCode.JOIN_ROUND, player1.Name, player1, game.Token);
            var joinGamePlayer2 = new PlayerMove(PlayerMoveCode.JOIN_ROUND, player2.Name, player2, game.Token);

            // Act
            game.StartRound();

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
