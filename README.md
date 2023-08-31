# Mia Game

## Introduction

This Mia game has a tradition in Germany as drinking game. No worries you don't need to drink alcohol. It's highly motivating to compete just for points or money as well. 
The software system is build for team events, where everyone tries to compete with his own bot against others.

## The Game

In the bar, the rules are pretty simple. As you can imagine it just needs a dice cup and two dices. The loser has to drink, the announcement of the correct numbers becomes more complicated later in the night. 

With the game server, small teams develop bots to compete. The people watch how their bots are performing live on a dashboard. It works at it best when a projector shows the scoring data in real time. 

### Game Mode

The option is to run the server every 30 minutes for 2 minutes, or continuous run with live deployments of the bots. As a minimum, 1200 moves per minute* are expected, which makes 100 game results per minute.

### 3 Score Modes

The original game mode is everyone starts with six lives. If you want to see how you lose your money, take the money mode. Or just go with scoring by points, this is perfect for dashboard watching while bots fight.

Start reading the [Game Rules](docs/GameRules.md).

## Bot Starter Kit

All starter bots know the basics like REGISTER, JOIN_ROUND, ROLL, ANNOUNCE. The rest is your job. Basically there is no limit for your creativity, strategies like cheating, data science, practical approaches in terms of knowing the character of your colleague, log for better analysis, hack the server, everything is allowed. Have fun!

- [Python](clients/python-simple-bot/)
- [JavaScript](clients/javascript-simple-bot/)
- [Java](clients/java-simple-bot/)
- [C++](clients/cplusplus-simple-bot/)
- [C#](clients/csharp-simple-bot/)
- [Go](clients/go-simple-bot/)
- [Elixir](clients/elixir-simple-bot/)
 
In case you are familiar with UDP and another programming language feel free to implement your own bot, it's a fairly simple protocol. 

Check the [Server Protocol](docs/ServerProtocol.md).

## How to win the high score?

Once again, everything is allowed. Use bugs to your advantage after you find them.

(*) This was tested locally per game with default timeout configuration for responses. There is room for improvement. The cluster scenario with multiple game instances including WAN brings another complexity to it. 
