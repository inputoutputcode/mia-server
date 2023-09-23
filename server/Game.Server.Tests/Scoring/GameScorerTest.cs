using System.Collections.Generic;

using Xunit;

using Game.Server.Scoring;
using Game.Server.Engine.Mia;
using Game.Server.Engine.Mia.Interface;


namespace Game.Server.Tests.Scoring
{
    public class GameServerTest
    {
        [Fact]
        public void Should_Have_Points_For_One_Round()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Should_Have_Points_For_Two_Rounds()
        {
            // Arrange
            var pointScorer = GameScoreFactory.Create(ScoreMode.Points);

            var player1 = new Player("Player1", false, "10.0.0.1");
            var player2 = new Player("Player2", false, "10.0.0.2");
            var player3 = new Player("Player3", false, "10.0.0.3");
            var playerList = new List<IPlayer>() { player1, player2, player3 };

            pointScorer.SetActivePlayers(playerList);

            int scoreDecrement = Config.Config.Settings.ScoreDecrement;
            int scoreIncrement = Config.Config.Settings.ScoreIncrement;

            // Act
            pointScorer.Lost(player1);
            pointScorer.Winner(player2);

            pointScorer.Lost(player1);
            pointScorer.Winner(player2);


            // Assert
            Assert.True(player1.Score == -(scoreDecrement * 2));
            Assert.True(player2.Score == (scoreIncrement * 2));
        }

        [Fact]
        public void Should_Be_Game_Over_After_Maximum_Points_Reached()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Should_Reduce_Lifes_For_One_Round()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Should_Reduce_Lifes_For_Two_Rounds()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Should_Have_Game_Over_With_Zero_Lifes()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Should_Add_Money_After_One_Round()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Should_Add_Money_After_Two_Round()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Should_Be_Game_Over_When_One_Player_Is_Without_Money()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Existing_Player_With_Same_IP_Address_But_Different_Name_Creates_New_Score()
        {
            // Arrange
            var pointScorer = GameScoreFactory.Create(ScoreMode.Points);

            var player1 = new Player("Player1", false, "10.0.0.1");
            var player2 = new Player("Player2", false, "10.0.0.2");
            var player3 = new Player("Player3", false, "10.0.0.3");
            var player4 = new Player("Player4", false, "10.0.0.3");

            var playerList = new List<IPlayer>() { player1, player2, player3 };
            pointScorer.SetActivePlayers(playerList);

            int scoreDecrement = Config.Config.Settings.ScoreDecrement;
            int scoreIncrement = Config.Config.Settings.ScoreIncrement;

            // Act
            pointScorer.Lost(player1);
            pointScorer.Winner(player3);

            var secondPlayerList = new List<IPlayer>() { player1, player2, player4 };
            pointScorer.SetActivePlayers(secondPlayerList);

            pointScorer.Lost(player1);
            pointScorer.Winner(player3);

            pointScorer.Lost(player1);
            pointScorer.Winner(player4);

            var scoredPlayerList = pointScorer.GetScores();

            // Assert
            Assert.True(scoredPlayerList.Find(p => p.Name == player1.Name).Score == -(scoreDecrement * 3));
            Assert.True(scoredPlayerList.Find(p => p.Name == player3.Name).Score == scoreIncrement);
            Assert.True(scoredPlayerList.Find(p => p.Name == player4.Name).Score == scoreIncrement);
        }
    }
}
