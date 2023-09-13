using Moq;
using Xunit;

using Game.Server.Engine.Mia;
using Game.Server.Engine.Mia.Interface;
using Game.Server.Engine.Mia.Move;
using Game.Server.Engine.Mia.Move.Interface;
using Game.Server.Network.Event.Interface;
using Game.Server.Scoring;
using Game.Server.Register.Interface;


namespace Game.Server.Test.PlayEngine.Mia
{
    public class GameTest
    {
        [Fact]
        public async void Game_Round_Will_Be_Cancelled_Without_Players()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>(MockBehavior.Strict);
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };
            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()));

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_STARTING)), Times.Never);
        }

        [Fact]
        public async void Game_Round_Will_Be_Cancelled_Without_ActivePlayers()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>(MockBehavior.Strict);
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };
            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()));

            var player1 = new Player("Player1", true);
            game.Object.Register(player1);
            var player2 = new Player("Player2", true);
            game.Object.Register(player2);

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_CANCELLED)), Times.Once);
        }

        [Fact]
        public async void Game_Round_Will_Be_Cancelled_With_One_Player()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>(MockBehavior.Strict);
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };
            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()));

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", true);
            game.Object.Register(player2);

            // Act
            await game.Object.StartAsync();
            game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);

            // Assert
            //TODO: No server code because round is not starting
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.ROUND_CANCELLED)), Times.Once);
        }

        [Fact]
        public async void Should_Send_The_First_Server_Move()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            var dice = new Mock<Dice>() { CallBase = true };

            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };
            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()));

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", false);
            game.Object.Register(player2);

            // Act
            await game.Object.StartAsync();
            game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
            game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.YOUR_TURN)), Times.Once);
        }

        [Fact]
        public async void See_Should_Raise_Lost_For_Current_Player_If_Dice_Higher_Or_Equal()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };
            dice.SetupProperty(d => d.DiceOne);
            dice.SetupProperty(d => d.DiceTwo);
            dice.Setup(d => d.Shake()).Callback(() =>
            {
                switch (game.Object.TurnCount)
                {
                    case 1:
                        if (game.Object.CurrentTurn.RollCount == 0)
                        {
                            dice.Object.DiceOne = 3;
                            dice.Object.DiceTwo = 1;
                        }
                        break;
                }
            });

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", false);
            game.Object.Register(player2);

            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var serverMove = (ServerMove)invocation.Arguments[0];
                    if (ServerMoveCode.ROUND_STARTING == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code && game.Object.Players[0].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ROLL.ToString(), string.Empty, game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                    else if (ServerMoveCode.ROLLED == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ANNOUNCE.ToString(), "31", game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code && game.Object.Players[1].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.SEE.ToString(), string.Empty, game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                }
            ));

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.PLAYER_LOST)));
        }

        [Fact]
        public async void See_Should_Raise_Win_For_Current_Player_If_Dice_Lower_On_See()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };
            dice.SetupProperty(d => d.DiceOne);
            dice.SetupProperty(d => d.DiceTwo);
            dice.Setup(d => d.Shake()).Callback(() =>
            {           
                switch (game.Object.TurnCount)
                {
                    case 1:
                        if (game.Object.CurrentTurn.RollCount == 0)
                        {
                            dice.Object.DiceOne = 3;
                            dice.Object.DiceTwo = 1;
                        }
                        break;
                }
            });

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", false);
            game.Object.Register(player2);

            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var serverMove = (ServerMove)invocation.Arguments[0];
                    if (ServerMoveCode.ROUND_STARTING == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code && game.Object.Players[0].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ROLL.ToString(), string.Empty, game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                    else if (ServerMoveCode.ROLLED == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ANNOUNCE.ToString(), "41", game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code && game.Object.Players[1].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.SEE.ToString(), string.Empty, game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                }
            ));

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.FailureReasonCode == ServerFailureReasonCode.CAUGHT_BLUFFING)));
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.PLAYER_LOST)));
        }

        [Fact]
        public async void Server_Should_Raise_Mia_For_Current_Player_If_Dice_Is_Mia_From_First_Roll()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };
            dice.SetupProperty(d => d.DiceOne);
            dice.SetupProperty(d => d.DiceTwo);
            dice.Setup(d => d.Shake()).Callback(() =>
            {
                dice.Object.DiceOne = 2;
                dice.Object.DiceTwo = 1;
            });

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", false);
            game.Object.Register(player2);

            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var serverMove = (ServerMove)invocation.Arguments[0];
                    if (ServerMoveCode.ROUND_STARTING == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ROLL.ToString(), string.Empty, game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                }
            ));

            // Act
            await game.Object.StartAsync();

            // Assert
            // TODO: verify whole response object with one assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.FailureReasonCode == ServerFailureReasonCode.MIA)));
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.PLAYER_LOST)));
        }

        [Fact]
        public async void Lie_About_Mia_Results_In_Player_Lost()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };
            dice.SetupProperty(d => d.DiceOne);
            dice.SetupProperty(d => d.DiceTwo);
            dice.Setup(d => d.Shake()).Callback(() =>
            {
                switch (game.Object.TurnCount)
                {
                    case 1:
                        if (game.Object.CurrentTurn.RollCount == 0)
                        {
                            dice.Object.DiceOne = 3;
                            dice.Object.DiceTwo = 1;
                        }
                        break;
                }
            });

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", false);
            game.Object.Register(player2);

            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var serverMove = (ServerMove)invocation.Arguments[0];
                    if (ServerMoveCode.ROUND_STARTING == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code && game.Object.Players[0].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ROLL.ToString(), string.Empty, game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                    else if (ServerMoveCode.ROLLED == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ANNOUNCE.ToString(), "21", game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                }
            ));

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.FailureReasonCode == ServerFailureReasonCode.LIED_ABOUT_MIA)));
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.PLAYER_LOST)));
        }

        [Fact]
        public async void Spectator_Sends_Command_Will_Be_Ignored()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };

            var player1 = new Player("Player1");
            game.Object.Register(player1);
            var player2 = new Player("Player2");
            game.Object.Register(player2);
            var spectator1 = new Player("Spectator1", true);
            game.Object.Register(spectator1);

            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var serverMove = (ServerMove)invocation.Arguments[0];
                    if (ServerMoveCode.ROUND_STARTING == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, spectator1, game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code && game.Object.Players[0].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ANNOUNCE.ToString(), "21", spectator1, game.Object.Token);
                    }
                }
            ));

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.FailureReasonCode == ServerFailureReasonCode.DID_NOT_TAKE_TURN)));
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.PLAYER_LOST)));
        }

        [Fact]
        public async void Invalid_Turn_Player_Sends_Dice_In_Incorrect_Order()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };
            dice.SetupProperty(d => d.DiceOne);
            dice.SetupProperty(d => d.DiceTwo);
            dice.Setup(d => d.Shake()).Callback(() =>
            {
                switch (game.Object.TurnCount)
                {
                    case 1:
                        if (game.Object.CurrentTurn.RollCount == 0)
                        {
                            dice.Object.DiceOne = 3;
                            dice.Object.DiceTwo = 1;
                        }
                        break;
                }
            });

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", false);
            game.Object.Register(player2);

            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var serverMove = (ServerMove)invocation.Arguments[0];
                    if (ServerMoveCode.ROUND_STARTING == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code && game.Object.Players[0].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ROLL.ToString(), string.Empty, game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                    else if (ServerMoveCode.ROLLED == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ANNOUNCE.ToString(), "13", game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                }
            ));

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.FailureReasonCode == ServerFailureReasonCode.INVALID_TURN)));
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.PLAYER_LOST)));
        }

        [Fact]
        public async void Invalid_Turn_Player_Sends_Empty_Dice()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };
            dice.SetupProperty(d => d.DiceOne);
            dice.SetupProperty(d => d.DiceTwo);
            dice.Setup(d => d.Shake()).Callback(() =>
            {
                switch (game.Object.TurnCount)
                {
                    case 1:
                        if (game.Object.CurrentTurn.RollCount == 0)
                        {
                            dice.Object.DiceOne = 3;
                            dice.Object.DiceTwo = 1;
                        }
                        break;
                }
            });

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", false);
            game.Object.Register(player2);

            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var serverMove = (ServerMove)invocation.Arguments[0];
                    if (ServerMoveCode.ROUND_STARTING == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code && game.Object.Players[0].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ROLL.ToString(), string.Empty, game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                    else if (ServerMoveCode.ROLLED == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ANNOUNCE.ToString(), "", game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                }
            ));

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.FailureReasonCode == ServerFailureReasonCode.INVALID_TURN)));
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.PLAYER_LOST)));
        }

        [Fact]
        public async void Invalid_Turn_Player_Sends_Third_Roll()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };
            dice.SetupProperty(d => d.DiceOne);
            dice.SetupProperty(d => d.DiceTwo);
            dice.Setup(d => d.Shake()).Callback(() =>
            {
                switch (game.Object.TurnCount)
                {
                    case 1:
                        if (game.Object.CurrentTurn.RollCount == 0)
                        {
                            dice.Object.DiceOne = 3;
                            dice.Object.DiceTwo = 1;
                        }
                        break;
                }
            });

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", false);
            game.Object.Register(player2);

            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var serverMove = (ServerMove)invocation.Arguments[0];
                    if (ServerMoveCode.ROUND_STARTING == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code && game.Object.Players[0].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ROLL.ToString(), string.Empty, game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                    else if (ServerMoveCode.ROLLED == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ROLL.ToString(), "32", game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                }
            ));

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.FailureReasonCode == ServerFailureReasonCode.INVALID_TURN)));
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.PLAYER_LOST)));
        }

        [Fact]
        public async void Invalid_Turn_Player_Sends_Command_As_Non_Current_Player()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };
            dice.SetupProperty(d => d.DiceOne);
            dice.SetupProperty(d => d.DiceTwo);
            dice.Setup(d => d.Shake()).Callback(() =>
            {
                switch (game.Object.TurnCount)
                {
                    case 1:
                        if (game.Object.CurrentTurn.RollCount == 0)
                        {
                            dice.Object.DiceOne = 3;
                            dice.Object.DiceTwo = 1;
                        }
                        break;
                }
            });

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", false);
            game.Object.Register(player2);

            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var serverMove = (ServerMove)invocation.Arguments[0];
                    if (ServerMoveCode.ROUND_STARTING == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code && game.Object.Players[0].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ROLL.ToString(), string.Empty, game.Object.Players[1], game.Object.Token);
                    }
                }
            ));

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.FailureReasonCode == ServerFailureReasonCode.INVALID_TURN)));
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.PLAYER_LOST)));
            // TODO: Should check all commands like ROLL, ANNOUNCE, SEE
        }

        [Fact]
        public async void Invalid_Turn_Player_Announce_After_Second_Roll()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };
            dice.SetupProperty(d => d.DiceOne);
            dice.SetupProperty(d => d.DiceTwo);
            dice.Setup(d => d.Shake()).Callback(() =>
            {
                switch (game.Object.TurnCount)
                {
                    case 1:
                        if (game.Object.CurrentTurn.RollCount == 0)
                        {
                            dice.Object.DiceOne = 3;
                            dice.Object.DiceTwo = 1;
                        }
                        break;
                }
            });

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", false);
            game.Object.Register(player2);

            string lastPlayerName = string.Empty;
            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var serverMove = (ServerMove)invocation.Arguments[0];
                    if (ServerMoveCode.ROUND_STARTING == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code && game.Object.Players[0].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ROLL.ToString(), string.Empty, game.Object.Players[0], game.Object.Token);
                    }
                    else if (ServerMoveCode.ROLLED == serverMove.Code)
                    {
                        if (game.Object.CurrentTurn.RollCount == 1)
                        {
                            game.Object.ReceiveClientEvent(ClientMoveCode.ROLL.ToString(), "32", game.Object.Players[0], game.Object.Token);
                        }
                        else if (game.Object.CurrentTurn.RollCount == 2)
                        {
                            lastPlayerName = game.Object.Players[0].Name;
                            game.Object.ReceiveClientEvent(ClientMoveCode.ANNOUNCE.ToString(), "32", game.Object.Players[0], game.Object.Token);
                        }
                    }
                }
            ));

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => 
                x.FailureReasonCode == ServerFailureReasonCode.INVALID_TURN &&
                x.Code == ServerMoveCode.PLAYER_LOST &&
                x.Value == lastPlayerName
            )));
        }

        [Fact]
        public async void Invalid_Turn_Player_Wants_To_See_During_First_Turn()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", false);
            game.Object.Register(player2);

            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var serverMove = (ServerMove)invocation.Arguments[0];
                    if (ServerMoveCode.ROUND_STARTING == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code && game.Object.Players[0].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.SEE.ToString(), string.Empty, game.Object.CurrentTurn.Player, game.Object.Token);
                    }
                }
            ));

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.FailureReasonCode == ServerFailureReasonCode.SEE_BEFORE_FIRST_ROLL)));
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x => x.Code == ServerMoveCode.PLAYER_LOST)));
        }

        [Fact]
        public async void Invalid_Turn_Player_Announced_Lower_Dice()
        {
            // Arrange
            var gameManager = new Mock<IGameManager>();
            var dice = new Mock<Dice>() { CallBase = true };
            var game = new Mock<Engine.Mia.Game>(1, ScoreMode.Points, gameManager.Object, dice.Object) { CallBase = true };
            dice.SetupProperty(d => d.DiceOne);
            dice.SetupProperty(d => d.DiceTwo);
            dice.Setup(d => d.Shake()).Callback(() =>
            {
                switch (game.Object.TurnCount)
                {
                    case 1:
                        if (game.Object.CurrentTurn.RollCount == 0)
                        {
                            dice.Object.DiceOne = 3;
                            dice.Object.DiceTwo = 1;
                        }
                        break;
                }
            });

            var player1 = new Player("Player1", false);
            game.Object.Register(player1);
            var player2 = new Player("Player2", false);
            game.Object.Register(player2);

            string lastPlayerName = string.Empty;
            game.Setup(m => m.SendServerMessage(It.IsAny<IServerMove>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var serverMove = (ServerMove)invocation.Arguments[0];
                    if (ServerMoveCode.ROUND_STARTING == serverMove.Code)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player1, game.Object.Token);
                        game.Object.ReceiveClientEvent(ClientMoveCode.JOIN_ROUND.ToString(), string.Empty, player2, game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code && game.Object.Players[0].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ROLL.ToString(), string.Empty, game.Object.Players[0], game.Object.Token);
                    }
                    else if (ServerMoveCode.ROLLED == serverMove.Code && game.Object.Players[0].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ANNOUNCE.ToString(), "32", game.Object.Players[0], game.Object.Token);
                    }
                    else if (ServerMoveCode.YOUR_TURN == serverMove.Code && game.Object.Players[1].Name == serverMove.Players[0].Name)
                    {
                        game.Object.ReceiveClientEvent(ClientMoveCode.ROLL.ToString(), string.Empty, game.Object.Players[1], game.Object.Token);
                    }
                    else if (ServerMoveCode.ROLLED == serverMove.Code && game.Object.Players[1].Name == serverMove.Players[0].Name)
                    {
                        lastPlayerName = game.Object.Players[1].Name;
                        game.Object.ReceiveClientEvent(ClientMoveCode.ANNOUNCE.ToString(), "31", game.Object.Players[1], game.Object.Token);
                    }
                }
            ));

            // Act
            await game.Object.StartAsync();

            // Assert
            game.Verify(m => m.SendServerMessage(It.Is<IServerMove>(x =>
                x.FailureReasonCode == ServerFailureReasonCode.ANNOUNCED_LOSING_DICE &&
                x.Code == ServerMoveCode.PLAYER_LOST &&
                x.Value == lastPlayerName
            )));
        }

        [Fact]
        public void Only_One_Turn_After_Kicking_One_Player_Out_Of_Three()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Passing_Value_Without_Roll_Keeps_Last_Rolled_Dice()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void See_For_Announced_Mia_Cost_Twice_As_No_See()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void Unregistered_Player_Sends_Command_Will_Be_Ignored()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(false);
        }

        // TODO: Check all failure codes
    }
}
