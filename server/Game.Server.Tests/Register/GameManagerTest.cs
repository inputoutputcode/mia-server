using Game.Server.Network.Interface;
using Game.Server.Register;

using Moq;
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
    }
}
