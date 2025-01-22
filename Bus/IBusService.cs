namespace Bus
{
    public interface IBusService
    {
        Task SendMessageAsync(object messageObj, string destination, CancellationToken ct = default);

        Task SendMessageAsync(string message, string destination, CancellationToken ct = default);
    }
}
