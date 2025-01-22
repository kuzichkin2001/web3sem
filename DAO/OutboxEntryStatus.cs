namespace DAO
{
    public enum OutboxEntryStatus
    {
        Error = 0,
        InProcess = 1,
        AlreadySent = 2,
    }
}
