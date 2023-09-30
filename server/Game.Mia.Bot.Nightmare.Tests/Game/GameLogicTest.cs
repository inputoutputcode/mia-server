using Game.Mia.Bot.Nightmare.Game;

using Xunit;


namespace Game.Mia.Bot.Nightmare.Tests.Game
{
    public class GameLogicTest
    {
        [Fact]
        public void Do_Not_Announce_Loosing_Dice()
        {
            // Arrange
            var gameLogic = new GameLogic();

            // Action
            string msgAnnounce = $"ANNOUNCED;41;{Guid.NewGuid}";
            gameLogic.ProcessEvent(msgAnnounce);

            string msgRolled = $"ROLLED;41;{Guid.NewGuid}";
            string response = gameLogic.ProcessEvent(msgRolled);

            // Assert
            Assert.StartsWith("ANNOUNCE;42;", response);
        }
    }
}