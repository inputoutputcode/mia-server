using System;
using Xunit;


namespace Game.Server.Tests.PlayEngine.Mia
{
    public class PlayerListTest
    {
        [Fact]
        public void Spectators_Should_Not_Be_Active_Players()
        {
            // Arrange
            var playerList = new Engine.Mia.PlayerList();
            var player1 = new Engine.Mia.Player("Player1", true);
            playerList.Register(player1);
            playerList.Join(player1);

            var player2 = new Engine.Mia.Player("Player2", false);
            playerList.Register(player2);
            playerList.Join(player2);

            var player3 = new Engine.Mia.Player("Player3", false);
            playerList.Register(player3);
            playerList.Join(player3);

            // Act

            // Assert
            Assert.True(playerList.ActivePlayers.Count == 2);
        }

        [Fact]
        public void Pick_Previous_Player_After_First_Round_With_2_Active_Players()
        {
            // Arrange
            var playerList = new Engine.Mia.PlayerList();
            var player1 = new Engine.Mia.Player("Player1");
            playerList.Register(player1);
            playerList.Join(player1);

            var player2 = new Engine.Mia.Player("Player2");
            playerList.Register(player2);
            playerList.Join(player2);

            // Act
            playerList.First();
            playerList.Next();

            // Assert
            var previousPlayer = playerList.Previous();
            Assert.True(previousPlayer.Name == player1.Name);
        }

        [Fact]
        public void Pick_The_Next_Player()
        {
            // Arrange
            var playerList = new Engine.Mia.PlayerList();
            var player1 = new Engine.Mia.Player("Player1");
            playerList.Register(player1);
            playerList.Join(player1);

            var player2 = new Engine.Mia.Player("Player2");
            playerList.Register(player2);
            playerList.Join(player2);

            var player3 = new Engine.Mia.Player("Player3");
            playerList.Register(player3);
            playerList.Join(player3);

            // Act
            playerList.First();
            var nextPlayer = playerList.Next();

            // Assert
            Assert.True(nextPlayer.Name == player2.Name);
        }

        [Fact]
        public void Pick_The_Next_Player_Last_Out_Of_Three()
        {
            // Arrange
            var playerList = new Engine.Mia.PlayerList();
            var player1 = new Engine.Mia.Player("Player1");
            playerList.Register(player1);
            playerList.Join(player1);

            var player2 = new Engine.Mia.Player("Player2");
            playerList.Register(player2);
            playerList.Join(player2);

            var player3 = new Engine.Mia.Player("Player3");
            playerList.Register(player3);
            playerList.Join(player3);

            // Act
            playerList.First();
            playerList.Next();
            var nextPlayer = playerList.Next();

            // Assert
            Assert.True(nextPlayer.Name == player3.Name);
        }

        [Fact]
        public void Pick_The_First_Player()
        {
            // Arrange
            var playerList = new Engine.Mia.PlayerList();
            var player1 = new Engine.Mia.Player("Player1");
            playerList.Register(player1);
            playerList.Join(player1);

            var player2 = new Engine.Mia.Player("Player2");
            playerList.Register(player2);
            playerList.Join(player2);

            var player3 = new Engine.Mia.Player("Player3");
            playerList.Register(player3);
            playerList.Join(player3);

            // Act
            var player = playerList.First();

            // Assert
            Assert.True(player.Name == player1.Name);
        }

        [Fact]
        public void Permute_The_List_Of_Player()
        {
            // Arrange
            var playerList = new Engine.Mia.PlayerList();
            var player1 = new Engine.Mia.Player("Player1", false);
            playerList.Register(player1);
            var player2 = new Engine.Mia.Player("Player2", false);
            playerList.Register(player2);
            var player3 = new Engine.Mia.Player("Player3", false);
            playerList.Register(player3);

            // Act
            playerList.Permute();
            var playerListResultOne = playerList.RegisteredPlayers;
            playerList.Permute();
            var playerListResultTwo = playerList.RegisteredPlayers;

            // Assert
            Assert.True(playerListResultOne[0].Name != playerListResultTwo[0].Name);
            Assert.True(playerListResultOne[1].Name != playerListResultTwo[1].Name);
        }

        [Fact]
        public void Add_New_Player()
        {
            // Arrange
            var playerList = new Engine.Mia.PlayerList();
            var player1 = new Engine.Mia.Player("Player1", false);

            // Act
            playerList.Register(player1);

            // Assert
            Assert.True(playerList.RegisteredPlayers.Count == 1);
        }

        [Fact]
        public void Add_Existing_Player()
        {
            // Arrange
            var playerList = new Engine.Mia.PlayerList();
            var player1 = new Engine.Mia.Player("Player1", false);

            // Act
            playerList.Register(player1);
            playerList.Register(player1);

            // Assert
            Assert.True(playerList.RegisteredPlayers.Count == 1);
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
