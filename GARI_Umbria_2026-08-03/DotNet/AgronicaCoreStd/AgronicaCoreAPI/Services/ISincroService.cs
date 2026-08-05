using AgronicaCoreAPI.Messaging.Contracts;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreVarieBizSTD;
using DocumentoPerScarico = AgronicaCoreModelloSTD.DocumentoPerScarico;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Services
{
    /// <summary>
    /// Servizio centralizzato per la scrittura su CoreWS.
    /// Condivide la logica tra il path sincrono (controller REST) e il path asincrono (RabbitMQ handler).
    /// </summary>
    public interface ISincroService
    {
        Task<SyncResult> ScriviAttivitaAsync(Attivita attivita, AuthInfo auth, CancellationToken ct = default);
        Task<SyncResult> ScriviAttivitaMisteAsync(List<Attivita> attivita, AuthInfo auth, CancellationToken ct = default);
        Task<SyncResult> ScriviRilievoAsync(Attivita rilievo, AuthInfo auth, CancellationToken ct = default);
        Task<SyncResult> ScriviVisiteAsync(Attivita attivita, AuthInfo auth, CancellationToken ct = default);
        Task<SyncResult> ScriviDocumentiAsync(DocumentoPerScarico documento, AuthInfo auth, CancellationToken ct = default);
        Task<SyncResult> ScriviMovimentiAsync(List<MovimentoDiMagazzino> movimenti, AuthInfo auth, CancellationToken ct = default);
        Task<SyncResult> ScriviAcquistoAsync(Acquisto acquisto, AuthInfo auth, CancellationToken ct = default);
        Task<SyncResult> ScriviManutenzioniAsync(List<Manutenzione> manutenzioni, AuthInfo auth, CancellationToken ct = default);
    }
}
