namespace DAO
{
    public class OutboxEntry
    {
        public int Id { get; set; }

        public string Destination { get; set; }

        public int MessageBody { get; set; }

        public int MessageContentType { get; set; }

        public OutboxEntryStatus MessageStatus { get; set; }
    }
}
