namespace Bus.Shared
{
    public class BusMessage
    {
        public string Source { get; set; }

        public string Destination { get; set; }

        public string? Next { get; set; }

        public string Type { get; set; }

        public object? Body { get; set; }
    }
}
