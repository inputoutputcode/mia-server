using Moq;
using Xunit;
using Mia.Server.Game.Scoring;
using Mia.Server.Game.PlayEngine.Move;
using Mia.Server.Game.Register.Interface;


namespace Mia.Server.Game.PlayEngine.Test
{
    public class GameScorer
    {
        [Fact]
        public void Game_Round_Will_Be_Cancelled_Without_Players()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>().Object;
            int rounds = 1;
            var game = new Game("Game1", rounds, ScoreMode.Points, gameManager);

            var player1 = new Player("Player1", false);
            var player2 = new Player("Player2", false);

            var joinGamePlayer1 = new PlayerMove(PlayerMoveCode.JOIN_GAME, player1.Name, player1, game.Token);
            var joinGamePlayer2 = new PlayerMove(PlayerMoveCode.JOIN_GAME, player2.Name, player2, game.Token);

            // Act

            // Assert

        }

        [Fact]
        public void Game_Round_Will_Be_Cancelled_With_One_Player()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public void See_Should_Raise_Lost_For_Current_Player_If_Dice_Higher_Or_Equal()
        {
            // Arrange

            // Act

            // Assert

        }

        [Fact]
        public void See_Should_Raise_Win_For_Current_Player_If_Dice_Lower()
        {
            // Arrange

            // Act

            // Assert
        }


        [Fact]
        public void See_Should_Raise_Mia_For_Current_Player_If_Dice_Is_Mia()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public void Lie_About_Mia()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public void Player_Commanded_Invalid_Turn_Third_Roll()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public void Player_Commanded_During_Inactive_Turn()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public void Player_Commanded_Announce_After_Second_Roll()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public void Invalid_Turn_Player_Wants_To_See_During_First_Turn()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public void Invalid_Turn_Player_Announced_Lower_Dice()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public void Template()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}
