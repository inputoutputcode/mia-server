using Game.Mia.Bot.Nightmare.Game;

using Xunit;


namespace Game.Mia.Bot.Nightmare.Tests.Game
{
    public class DicerTest
    {
        [Fact]
        public void DiceOne_Always_Higher_Than_DiceTwo()
        {
            // Arrange
            Dicer diceOne;
            Dicer diceTwo;

            // Action
            diceOne = new Dicer(4, 1);
            diceTwo = new Dicer(4, 2);
            Assert.True(diceOne.IsHigherThan(diceTwo) == -1);

            diceOne = new Dicer(3, 1);
            diceTwo = new Dicer(3, 1);
            Assert.True(diceOne.IsHigherThan(diceTwo) == 0);

            diceOne = new Dicer(4, 4);
            diceTwo = new Dicer(4, 4);
            Assert.True(diceOne.IsHigherThan(diceTwo) == 0);

            diceOne = new Dicer(4, 4);
            diceTwo = new Dicer(5, 1);
            Assert.True(diceOne.IsHigherThan(diceTwo) == 1);

            diceOne = new Dicer(5, 1);
            diceTwo = new Dicer(4, 4);
            Assert.True(diceOne.IsHigherThan(diceTwo) == -1);

            diceOne = new Dicer(2, 1);
            diceTwo = new Dicer(4, 2);
            Assert.True(diceOne.IsHigherThan(diceTwo) == 1);

            diceOne = new Dicer(4, 2);
            diceTwo = new Dicer(2, 1);
            Assert.True(diceOne.IsHigherThan(diceTwo) == -1);

            diceOne = new Dicer(5, 1);
            diceTwo = new Dicer(4, 3);
            Assert.True(diceOne.IsHigherThan(diceTwo) == 1);

            diceOne = new Dicer(4, 3);
            diceTwo = new Dicer(5, 1);
            Assert.True(diceOne.IsHigherThan(diceTwo) == -1);

            // Assert
        }
    }
}
