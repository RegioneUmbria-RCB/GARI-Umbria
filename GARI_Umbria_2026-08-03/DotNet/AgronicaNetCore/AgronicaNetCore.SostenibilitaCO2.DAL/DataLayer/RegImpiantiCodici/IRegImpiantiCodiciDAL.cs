using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.RegImpiantiCodici
{
    /// <summary>
    /// Contratto per l'accesso alla tabella <c>Reg_Impianti_Codici</c>
    /// nel contesto del calcolo CO2.
    /// Riferimento spec: DS04-BL AssemblyPayloadM4AppezzamentoImpianto — Persistenze, Regola 5 (Anno Impianto).
    /// </summary>
    public interface IRegImpiantiCodiciDAL
    {
        /// <summary>
        /// Recupera l'anno di impianto (<c>val_cod</c>) da <c>Reg_Impianti_Codici</c>
        /// per la combinazione piva / sa_cod / appezza / Id_Reg con <c>id_cod = 1362</c>.
        /// </summary>
        /// <param name="piva">Partita IVA dell'azienda.</param>
        /// <param name="saCod">Codice azienda satellite.</param>
        /// <param name="appezza">Codice appezzamento.</param>
        /// <param name="idReg">Identificativo del registro impianto.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// Anno di impianto come intero se trovato; <c>null</c> se nessuna corrispondenza (campo opzionale).
        /// </returns>
        Task<int?> GetAnnoImpiantoAsync(string piva, int saCod, int appezza, int idReg, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Recupera l'identificativo di destinazione d'uso da <c>Reg_Impianti_Codici.id_cod</c>
        /// nel range [3000, 4000] per la combinazione piva / sa_cod / appezza / Id_Reg.
        /// </summary>
        /// <param name="piva">Partita IVA dell'azienda.</param>
        /// <param name="saCod">Codice azienda satellite.</param>
        /// <param name="appezza">Codice appezzamento.</param>
        /// <param name="idReg">Identificativo del registro impianto.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// Identificativo destinazione d'uso come intero se trovato; <c>null</c> se nessuna corrispondenza.
        /// </returns>
        Task<int?> GetIdDestinazioneUsoAsync(string piva, int saCod, int appezza, int idReg, AgronicaCoreParametriServer objParametriServer);
    }
}
