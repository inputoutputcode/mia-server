using Game.Server.Network.Interface;
using Game.Server.Register;

using Moq;
using Xunit;


namespace Game.Server.Game.Register.Test
{
    public class GameManagerTest
    {
        [Fact]
        public void Validate_Player_Name()
        {
            // Arrange
            var commandServer = new Mock<IServer>();
            var gameManager = new GameManager(commandServer.Object);

            string playerName1 = "ThisNameIsOk12";
            string playerName2 = "ThisNameIsNotOk!";
            string playerName3 = "ThisNameIsObviouslyFarToooooooLong";

            // Act

            // Assert
            Assert.True(gameManager.ValidateName(playerName1));
            Assert.False(gameManager.ValidateName(playerName2));
            Assert.False(gameManager.ValidateName(playerName3));
        }
    }
}
