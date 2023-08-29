namespace Game.Server.Engine.Mia.Move
{
    public enum ServerMoveCode
    {
        None,
        REGISTERED,
        REJECTED,
        ROUND_STARTING,
        ROUND_STARTED,
        ROUND_CANCELLED,
        YOUR_TURN,
        PLAYER_ROLLS,
        ROLLED,
        ANNOUNCED,
        PLAYER_LOST,
        PLAYER_WANTS_TO_SEE,
        ACTUAL_DICE,
        SCORE
    }
}
