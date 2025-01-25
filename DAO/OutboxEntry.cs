namespace DAO
{
    public class OutboxEntry
    {
        public int Id { get; set; }

        public string CorrelationId { get; set; }

        public string Destination { get; set; }

        public string MessageBody { get; set; }

        public string MessageContentType { get; set; }

        public OutboxEntryStatus MessageStatus { get; set; }
    }
}
