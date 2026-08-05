using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Indirizzi
{
    /// <summary>
    /// Represents a geographic key used to identify an appezzamento's location.
    /// </summary>
    public record AppezzamentoGeoKey(string Piva, int SaCod, int Appezza);

    /// <summary>
    /// Data-access contract for resolving geographic data (Province, Regione, Nazione)
    /// for a batch of appezzamenti via the address registry.
    /// </summary>
    /// <remarks>
    /// <c>Appezzamento → AppezzamentixIndirizzi → Indirizzi → Lista_Province → Lista_Regioni → Lista_Stati</c>.
    /// </remarks>
    public interface IIndirizzi
    {
        /// <summary>
        /// Returns geographic descriptors for all supplied appezzamento keys in a single batch.
        /// </summary>
        /// <param name="chiavi">Distinct set of (PIVA, SA_COD, APPEZZA) combinations to resolve.</param>
        /// <param name="objParametriServer">Server-level request context.</param>
        /// <returns>
        /// DataTable with columns: <c>PIVA, SA_COD, APPEZZA, Provincia, Regione, Nazione</c>.
        /// </returns>
        Task<DataTable> LeggiIndirizzoxAppezzamentoAsync(
            IEnumerable<AppezzamentoGeoKey> chiavi,
            AgronicaCoreParametriServer objParametriServer
        );

        Task<DataTable> LeggiIndirizziImpresaAsync(
            List<string> chiaviImprese,
            List<int> tipoIndirizzo,
            bool indirizzoCompleto,
            AgronicaCoreParametriServer objParametriServer
        );

        Task<DataTable> LeggiIndirizziCentroAsync(
            List<(string, int)> chiaviCentro,
            List<int> tipoIndirizzo,
            bool indirizzoCompleto,
            AgronicaCoreParametriServer objParametriServer
        );

        Task<DataTable> LeggiIndirizziContattiAsync(
            List<(string, int)> chiaviContatto,
            List<int> tipoIndirizzo,
            bool indirizzoCompleto,
            AgronicaCoreParametriServer objParametriServer
        );

        Task<DataTable> LeggiIndirizziAppezzamentiAsync(
            List<(string, int, int)> chiaviAppezzamento,
            List<int> tipoIndirizzo,
            bool indirizzoCompleto,
            AgronicaCoreParametriServer objParametriServer
        );
    }
}
