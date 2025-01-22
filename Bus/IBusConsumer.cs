namespace Bus
{
    public interface IBusConsumer
    {
        bool ChannelConstructed { get; }

        Task InitializeConnection();

        Task ListenAsync(CancellationToken ct);
    }
}
