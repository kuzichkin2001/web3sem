using Bus.Shared;

namespace Bus
{
    public interface IBusService
    {
        Task SendMessageAsync(object messageObj, string destination, string next = "", CancellationToken ct = default);

        Task SendMessageAsync(BusMessage busMessage, CancellationToken ct = default);
    }
}
