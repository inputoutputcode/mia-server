using Game.Server.Network.Interface;
using Game.Server.Register;
using Game.Server.Register.Interface;
using Game.Server.Scoring;
using Game.Server.Scoring.Interface;
using Moq;
using System;
using Xunit;


namespace Game.Server.Tests.Register
{
    public class GameManagerTest
    {
        [Fact]
        public void Validate_Player_Name()
        {
            // Arrange
            var server = new Mock<IServer>();
            var gameManager = new GameManager(server.Object);

            string playerName1 = "ThisNameIsOk12";
            string playerName2 = "ThisNameIsNotOk!";
            string playerName3 = "ThisNameIsObviouslyFarToooooooLong";

            // Act

            // Assert
            Assert.True(gameManager.ValidateName(playerName1));
            Assert.False(gameManager.ValidateName(playerName2));
            Assert.False(gameManager.ValidateName(playerName3));
        }

        [Fact]
        public void Lookup_Old_Game_Token()
        {
            // Arrange
            var server = new Mock<IServer>();
            var gameManager = new GameManager(server.Object);
            var gameScorer = new Mock<IGameScorer>(MockBehavior.Strict);

            var game1 = new Engine.Mia.Game(0, ScoreMode.Points, gameManager, null, gameScorer.Object);
            var game2 = new Engine.Mia.Game(0, ScoreMode.Points, gameManager, null, gameScorer.Object);

            gameManager.AddGame(game1);
            gameManager.AddGame(game2);

            // Act
            var fakeGameToken = Guid.NewGuid();
            var game = gameManager.FindGame(fakeGameToken);

            // Assert
            Assert.True(game == null);
        }
    }
}
