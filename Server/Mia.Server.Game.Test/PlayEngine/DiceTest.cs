using System.Collections.Generic;
using Xunit;


namespace Mia.Server.Game.PlayEngine.Test
{
    public class DiceTest
    {
        [Fact]
        public void Should_Always_Have_Higher_Dice_First()
        {
            // Act
            var dice1 = new Dice(1, 6);
            var dice2 = new Dice(6, 1);
            var dice3 = new Dice(2, 2);

            // Arrange

            // Assert
            Assert.True(dice1.DiceOne == 6 && dice1.DiceTwo == 1);
            Assert.True(dice2.DiceOne == 6 && dice2.DiceTwo == 1);
            Assert.True(dice3.DiceOne == 2 && dice3.DiceTwo == 2);
        }

        [Fact]
        public void Detect_Mia()
        {
            // Act
            var dice1 = new Dice(1, 6);
            var dice2 = new Dice(1, 2);
            var dice3 = new Dice(2, 1);

            // Arrange

            // Assert
            Assert.False(dice1.IsMia);
            Assert.True(dice2.IsMia);
            Assert.True(dice3.IsMia);
        }

        [Fact]
        public void Detect_Double()
        {
            // Act
            var dice1 = new Dice(1, 6);
            var dice2 = new Dice(1, 1);

            // Arrange

            // Assert
            Assert.False(dice1.IsDouble);
            Assert.True(dice2.IsDouble);
        }

        /// <summary>
        /// Scores: 21 (Mia), 66, 55, 44, 33, 22, 11, 65, 64, 63, 62, 61, 54, 53, 52, 51, 43, 42, 41, 32, 31.
        /// </summary>
        [Fact]
        public void Detect_Higher_Score()
        {
            // Act
            var scores = new List<Dice>();

            // Arrange
            scores.AddRange( new List<Dice>
            {
                new Dice(3, 1),
                new Dice(3, 2),
                new Dice(4, 1),
                new Dice(4, 2),
                new Dice(4, 3),
                new Dice(5, 1),
                new Dice(5, 2),
                new Dice(5, 3),
                new Dice(5, 4),
                new Dice(6, 1),
                new Dice(6, 2),
                new Dice(6, 3),
                new Dice(6, 4),
                new Dice(6, 5),
                new Dice(1, 1),
                new Dice(2, 2),
                new Dice(3, 3),
                new Dice(4, 4),
                new Dice(5, 5),
                new Dice(6, 6),
                new Dice(2, 1)
            });

            // Assert
            for (int i = 1; i < scores.Count - 1; i++)
                Assert.True(scores[i].IsHigherThan(scores[i - 1]));

            for (int j = scores.Count - 1; j > 0; j--)
                Assert.False(scores[j - 1].IsHigherThan(scores[j]));
            
        }
    }
}
