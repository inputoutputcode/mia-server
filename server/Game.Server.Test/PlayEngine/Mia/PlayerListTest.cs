using System;
using Xunit;


namespace Game.Server.Test.PlayEngine.Mia
{
    public class PlayerListTest
    {
        [Fact]
        public void Spectators_Should_Not_Be_Active_Players()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Pick_The_First_Player()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Pick_The_Next_Player()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Pick_The_Last_Player()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Permute_The_List_Of_Player()
        {
            // Arrange
            var playerList = new Engine.Mia.PlayerList();
            var player1 = new Engine.Mia.Player("Player1", false);
            playerList.Join(player1);
            var player2 = new Engine.Mia.Player("Player2", false);
            playerList.Join(player2);

            // Act
            playerList.Permute();

            // Assert
            Assert.True(playerList.ActivePlayers[0].Name == player1.Name);
            Assert.True(playerList.ActivePlayers[1].Name == player2.Name);
        }

        [Fact]
        public void Add_New_Player()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Add_Existing_Player()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Do_Not_Manipulate_Internal_Player_List()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }
    }
}
