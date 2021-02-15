namespace Mia.Server.Command
{
    public enum ServerCommandFailureReason
    {
        None,
        NO_PLAYERS,
        ONLY_ONE_PLAYER,
        INVALID_NAME,
        NAME_ALREADY_TAKEN
    }
}
