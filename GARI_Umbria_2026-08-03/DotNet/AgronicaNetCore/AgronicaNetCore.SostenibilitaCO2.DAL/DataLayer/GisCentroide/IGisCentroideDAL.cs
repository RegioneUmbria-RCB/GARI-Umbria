//using AgronicaNetCore.Base.Models;

//namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.GisCentroide
//{
//    /// <summary>
//    /// Contratto per il recupero del centroide geografico di un appezzamento
//    /// dalle tabelle GIS, nel contesto del calcolo CO2.
//    /// Riferimento spec: DS04-BL AssemblyPayloadM4AppezzamentoImpianto — campo <c>centroide</c>.
//    /// </summary>
//    public interface IGisCentroideDAL
//    {
//        /// <summary>
//        /// Restituisce il centroide WKT dell'appezzamento leggendolo da
//        /// <c>GIS_ElementiGrafici_Clustering</c> tramite join su <c>GIS_ElementiGrafici</c>
//        /// e <c>GIS_Entita</c>.
//        /// </summary>
//        /// <param name="piva">Partita IVA dell'azienda.</param>
//        /// <param name="saCod">Codice azienda satellite.</param>
//        /// <param name="appezza">Codice appezzamento.</param>
//        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
//        /// <returns>
//        /// Stringa WKT del centroide (es. <c>"POINT(12.3456 41.2345)"</c>)
//        /// oppure <c>null</c> se nessun record è presente.
//        /// </returns>
//        Task<string?> GetCentroideWktAsync(
//            string piva,
//            string saCod,
//            string appezza,
//            AgronicaCoreParametriServer objParametriServer);
//    }
//}
