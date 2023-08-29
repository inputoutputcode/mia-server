# Design considerations

## Start with a plan
1. Rule set
1. Base architecture
    1. Scaling
    1. Routing
    1. Service compute breakdown
1. Unit tests with ServiceFabric.Mocks, XUnit
1. C# simple starter bot
1. C# advanced bot, all moves plus greater variance

## General requirements
1. Games must run fast, high density, fast decision making by bots
1. Multi-game option
1. UDP protocol (challenge)
1. IP address + Player name builds the actor ID
1. Only one game session for one player at the same time
1. Durability=Silver/Reliability=Silver with D2v3 on Windows, .NET Core, no container
1. Endless capacity design
    1. Scaling all services based on metrics or demand count
        1. Actor - only offers partitioning concept, workaround with dynamic created instances
        1. Stateless - single service addition, instance count based on nodes, named instance, auto-scaling
        1. Stateful - named instance, partitioning, auto-scaling
    1. Scaling nodes based on custom metrics 
    1. Auto scaling for instances and partitions
    1. Hashing by FNV-1 or Murmur3

## Capacity threshold for the start
1. Gateway - InstanceCount = -1
1. GameRegister - Instance = 1
1. GameManager - InstanceCount = 2
1. Game - InstanceCount = 1, capacity 5,000 games
1. Client - InstanceCount = 1, capacity 10,000 clients

## Load patterns
1. Rapid increase during load test for all out
1. Slowly increasing as well decreasing
1. Maximum with default capacity
1. Long time test

## Traffic assumptions
1. 5000 requests per sec on five nodes
1. 10 rounds per sec
1. 50 ms for one request/response trip, max hops in backend 3, default 2

## Config defaults
1. MaxPlayersPerGame = 10
1. MaxSpectatorsPerGame = 20
1. MaxGamesPerGameRegister = 5000 
1. RefreshGameRegisterInSecs = 10
1. GameReservedInstances = 20
1. GameDollarMinimum = 10
1. GameCompeteModeDefault = PlayForPoints
1. GameRunModeDefault = Continuous
1. GameDeletionAfterIddleInMins = 5
1. ResponseTimeoutInMs = 250
1. MaxGameRoundRuns = 200000
1. MinimumMoneyAmount = 10

## Minimal design
1. Gateway - Stateless (InstanceCount=-1) Try InstanceCount=3 with dynamic scaling through GameRegister
    1. Routing requests
        1. FindGameByName -> GameRegister
        1. CreateGame -> GameRegister
        1. GameTotal -> GameRegister
        1. PlayerTotal -> GameRegister
        1. RegisterPlayer -> GameManager (Name, IPAddress) 
        1. CreateGame -> GameRegister -> GameId
        1. JoinGame -> Game (by GameId, Token, RunMode, CompeteMode)
        1. AnyTurn -> Game (by GameId, Token)
        1. LeaveGame -> Game (by GameId, Token)
1. GameRegister - Stateful with auto-scaling
    1. Interface
        1. int TotalGames
        1. int TotalPlayers
        1. List<GameId, Name> GameList
    1. Logic
        1. Scaling for Gateway
        1. Scaling for Nodes
        1. Scaling for GameManager
1. GameManager - Stateful with auto-scaling
    1. Manage a reserved pool of free Game instances
        1. Store the numbers of games, clients, pre-configured round runs.
    1. Automatic scaling for instances and nodes
        1. Named services by manager
    1. Validates clients
    1. Interface
        1. Token RegisterNewClient(Name) 
        1. ReEnterExistingClient(Name, Token)
        1. StartNewGame([MinimumMoneyAmount, GameCompeteMode, GameRunModeDefault, MaxGameRoundRuns, Won, Lost, Others])
    1. Commands 
        1. REGISTERED
        1. REJECTED
    1. Unit tests
    1. Logic 
        1. Maintain a pool of free Game instances
        1. Register a new game in GameRegister
        1. Deregister a game in GameRegister
        1. Register a new client, return the login token
1. Game - Stateful with named instances
    1. Manage clients
    1. Manage game play
    1. GameCompeteMode
        1. Money
            1. Lost -5
            1. Won 3
            1. Others 1
        1. Points
            1. Lost 0
            1. Won 5
            1. Others 2
    1. GameRunMode
        1. Continuous
        1. RunFor5After30Mins
    1. Reset game
        1. Only if all players confirm 
    1. Interfaces
        1. RegisterPlayer(GameId, Name, Token) 
        1. JoinGame(GameId)
        1. LeaveGame
        1. JoinSpectator(GameId)
        1. LeaveSpectator
    1. Commands
        1. ROUND_STARTING
        1. ROUND_STARTED
        1. ROUND_CANCELED;NO_PLAYERS
        1. ROUND_CANCELED;ONLY_ONE_PLAYER
        1. YOUR_TURN;token
        1. ROLLED;dice;token
        1. ANNOUNCED;name;dice
        1. PLAYER_LOST;names;reason
        1. PLAYER_WANTS_TO_SEE;
        1. ACTUAL_DICE;dice 
        1. PLAYER_LOST;name;reason
        1. SCORE;playerpoints
    1. Reason codes
        1. SEE_BEFORE_FIRST_ROLL
        1. LIED_ABOUT_MIA
        1. ANNOUNCED_LOSING_DICE
        1. DID_NOT_ANNOUNCE
        1. DID_NOT_TAKE_TURN
        1. INVALID_TURN
        1. SEE_FAILED
        1. CAUGHT_BLUFFING
        1. MIA
    1. Unit tests
    1. Logic
        1. Report player on join or leave event to ClientManager
1. Client - Actor or Stateful with scaling policy
    1. Commands
        1. REGISTER;name
        1. REGISTER_SPECTATOR;name
        2. JOIN;token
        1. ANNOUNCE;dice;token
    1. Unit tests

## Advancing the scenario
1. Load test with jMeter 
1. Load testing reports to describe bottlenecks and thresholds
1. EventHub for statistic data
1. PowerBi live dashboard
1. TCP version with web UI for client and server, for better demo plus easier entry level
1. Azure Monitoring 
1. Endpoint Monitoring?
1. FabricObserver
1. Service based resource usage with FabricObserver pushed to Log Analytics dashboard
1. Manual OS image upgrade
1. Manual runtime upgrades
1. Game data for AI model
1. Implement custom metrics to get full hardware usage
    1. GameCount
    1. GameRuns
    1. Spectator
    1. TBD
1. Scaling considerations
    1. named partitions
        1. AddRemoveIncrementalNamedPartitionScalingMechanism
    1. stateless on metrics
    1. physical metrics
        1. servicefabric:/_CpuCores
        1. servicefabric:/_MemoryInMB
        1. VMSS auto-scaling
1. Testing hardware saturation on different SKUs including spot VM, low priority VM, availibility zone, managed clusters, 