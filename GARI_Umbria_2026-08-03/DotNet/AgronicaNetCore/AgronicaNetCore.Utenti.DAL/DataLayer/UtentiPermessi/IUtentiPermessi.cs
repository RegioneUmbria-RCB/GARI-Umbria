using AgronicaNetCore.Base.Models;
using System.Data;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.UtentiPermessi
{
    public interface IUtentiPermessi
    {
        Task<bool> ControllaPermessiUtenteAsync(
            string username,
            Enum_Id_Servizio idServizio,
            Enum_Security_Attivita idAttivita,
            Enum_Security_Operazione idOperazione,
            DateTime dataOraControllo,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer);


        /// <summary>
        /// Fully-batch variant: checks multiple activities and operations in a single DB round-trip.
        /// Returns a dictionary keyed by activity, where each value is the set of granted operations.
        /// </summary>
        Task<IReadOnlyDictionary<Enum_Security_Attivita, IReadOnlySet<Enum_Security_Operazione>>> ControllaPermessiUtenteAsync(
            string username,
            Enum_Id_Servizio idServizio,
            IList<Enum_Security_Attivita> attivita,
            IList<Enum_Security_Operazione> operazioni,
            DateTime dataOraControllo,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer);

        Task<DataTable?> ReadAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, int idService, int idActivity, int idOperation, int id = 1);
        Task<DataTable?> ReadAllAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);

    }
}
