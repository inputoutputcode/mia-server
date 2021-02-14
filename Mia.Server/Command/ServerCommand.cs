namespace Mia.Server.Command
{
    public enum ServerCommand
    {
        None,
        REGISTERED,
        REJECTED,
        ROUND_STARTING,
        ROUND_STARTED,
        ROUND_CANCELED,
        YOUR_TURN,
        PLAYER_ROLLS,
        ROLLED,
        ANNOUNCED,
        PLAYER_LOST,
        PLAYER_WANTS_TO_SEE,
        ACTUAL_DICE,
        SCORE,
        INVALID_TURN
    }
}
