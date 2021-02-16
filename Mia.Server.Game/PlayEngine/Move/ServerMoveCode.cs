namespace Mia.Server.Game.PlayEngine.Move
{
    public enum ServerMoveCode
    {
        None,
        REGISTERED,
        REJECTED,
        GAME_STARTING,
        GAME_STARTED,
        GAME_CANCELLED,
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
