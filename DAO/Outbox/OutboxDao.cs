using Microsoft.EntityFrameworkCore;

namespace DAO.Outbox
{
    public class OutboxDao
    {
        private readonly UnitOfWork _unitOfWork;

        public OutboxDao(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task CreateEntry(OutboxEntry entry)
        {
            _unitOfWork.Add(entry);
            _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            return Task.CompletedTask;
        }

        public Task MarkAsSent(OutboxEntry entry)
        {
            entry.MessageStatus = OutboxEntryStatus.AlreadySent;
            _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            return Task.CompletedTask;
        }

        public Task MarkAsError(OutboxEntry entry)
        {
            entry.MessageStatus = OutboxEntryStatus.Error;
            _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            return Task.CompletedTask;
        }

        public async Task<OutboxEntry?> QueryAsync(string correlationId)
        {
            return await _unitOfWork.OutboxEntries
                .FirstOrDefaultAsync(oe => oe.CorrelationId == correlationId)
                .ConfigureAwait(false);
        }
    }
}
