using Game.Mia.Bot.Nightmare.Game;

using Xunit;


namespace Game.Mia.Bot.Nightmare.Tests.Game
{
    public class GameLogicTest
    {
        [Fact]
        public void Do_Not_Announce_Loosing_Dice_For_Rolled_Equal()
        {
            // Arrange
            var gameLogic = new GameLogic();

            // Action
            string msgAnnounce = $"ANNOUNCED;Name;53;{Guid.NewGuid}";
            gameLogic.ProcessEvent(msgAnnounce);

            string msgRolled = $"ROLLED;51;{Guid.NewGuid}";
            string response = gameLogic.ProcessEvent(msgRolled);

            // Assert
            Assert.StartsWith("ANNOUNCE;54;", response);
        }

        [Fact]
        public void Do_Not_Announce_Loosing_Dice_For_Rolled_Lower()
        {
            // Arrange
            var gameLogic = new GameLogic();

            // Action
            string msgAnnounce = $"ANNOUNCED;Name;43;{Guid.NewGuid()}";
            gameLogic.ProcessEvent(msgAnnounce);

            string msgRolled = $"ROLLED;31;{Guid.NewGuid}";
            string response = gameLogic.ProcessEvent(msgRolled);

            // Assert
            Assert.StartsWith("ANNOUNCE;51;", response);
        }
    }
}