using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Infrastructure
{
    /// <summary>
    /// Implementazione Redis di IRequestStatusStore tramite IDistributedCache.
    /// TTL 7 giorni. Chiave: "syncstatus:{correlationId}".
    /// Attivare con SyncStore:Type = "Redis" e configurando StackExchange.Redis.
    /// </summary>
    public class RedisRequestStatusStore : IRequestStatusStore
    {
        private const string KeyPrefix = "syncstatus:";
        private static readonly TimeSpan Ttl = TimeSpan.FromDays(7);

        private readonly IDistributedCache _cache;

        public RedisRequestStatusStore(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetAsync(SyncStatusRecord record, CancellationToken ct = default)
        {
            record.UpdatedAt = DateTime.UtcNow;
            var json = JsonConvert.SerializeObject(record);
            await _cache.SetStringAsync(
                KeyPrefix + record.CorrelationId,
                json,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = Ttl },
                ct);
        }

        public async Task<SyncStatusRecord> GetAsync(string correlationId, CancellationToken ct = default)
        {
            var json = await _cache.GetStringAsync(KeyPrefix + correlationId, ct);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonConvert.DeserializeObject<SyncStatusRecord>(json);
        }
    }
}
