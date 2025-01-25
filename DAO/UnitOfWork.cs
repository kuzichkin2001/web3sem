using Configuration.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DAO
{
    public class UnitOfWork : DbContext
    {
        public DbSet<OutboxEntry> OutboxEntries { get; set; }

        private readonly string _connectionString;

        public UnitOfWork(IOptions<DataAccessOptions> connectionOptions)
        {
            _connectionString = connectionOptions.Value.ConnectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseNpgsql(_connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OutboxEntry>(eb =>
            {
                eb.ToTable("outbox_entry", "inout_box");
                eb.HasKey(oe => oe.Id).HasName("PK_outbox_entry_id");
                eb.Property(oe => oe.Id).HasColumnName("outbox_entry_id").IsRequired().HasIdentityOptions(1, 1);
                eb.Property(oe => oe.CorrelationId).HasColumnName("outbox_entry_uid").IsRequired();
                eb.Property(oe => oe.Destination).HasColumnName("outbox_destination_name").HasDefaultValue(string.Empty);
                eb.Property(oe => oe.MessageBody).HasColumnName("outbox_message_body").IsRequired();
                eb.Property(oe => oe.MessageContentType).HasColumnName("outbox_message_content_type").IsRequired();
                eb.Property(oe => oe.MessageStatus).HasColumnName("outbox_message_status").IsRequired();
            });
        }
    }
}
