namespace Game.Server.Engine.Mia.Move
{
    public enum ServerFailureReasonCode
    {
        None,
        NO_PLAYERS,
        ONLY_ONE_PLAYER,
        ANNOUNCED_LOSING_DICE,
        INVALID_TURN,
        DID_NOT_TAKE_TURN,
        DID_NOT_ANNOUNCE,
        MIA,
        LIED_ABOUT_MIA,
        SEE_BEFORE_FIRST_ROLL,
        CAUGHT_BLUFFING,
        SEE_FAILED
    }
}
