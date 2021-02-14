namespace Mia.Server.Command
{
    public enum FailureCommand
    {
        None,
        DID_NOT_TAKE_TURN,
        INVALID_TURN,
        DID_NOT_ANNOUNCE,
        ANNOUNCED_LOSING_DICE,
        MIA,
        LIED_ABOUT_MIA,
        SEE_BEFORE_FIRST_ROLL,
        CAUGHT_BLUFFING,
        SEE_FAILED
    }
}
