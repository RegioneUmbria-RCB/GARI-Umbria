using System;
using System.Threading;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Infrastructure
{
    public enum SyncStatus { Accepted, InProgress, Succeeded, Failed }

    public class SyncStatusRecord
    {
        public string CorrelationId { get; set; }
        public SyncStatus Status { get; set; }
        public string ResultJson { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>Astrazione per il tracciamento dello stato dei messaggi di sincronizzazione.</summary>
    public interface IRequestStatusStore
    {
        Task SetAsync(SyncStatusRecord record, CancellationToken ct = default);
        Task<SyncStatusRecord> GetAsync(string correlationId, CancellationToken ct = default);
    }
}
