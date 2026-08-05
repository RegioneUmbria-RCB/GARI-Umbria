using System.Threading;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Infrastructure
{
    /// <summary>
    /// Implementazione no-op di IRequestStatusStore.
    /// Usato quando SyncStore:Type = "None" (default produzione).
    /// Nessuna persistenza — fire-and-forget.
    /// </summary>
    public class NoOpRequestStatusStore : IRequestStatusStore
    {
        public Task SetAsync(SyncStatusRecord record, CancellationToken ct = default) => Task.CompletedTask;

        public Task<SyncStatusRecord> GetAsync(string correlationId, CancellationToken ct = default) =>
            Task.FromResult<SyncStatusRecord>(null);
    }
}
