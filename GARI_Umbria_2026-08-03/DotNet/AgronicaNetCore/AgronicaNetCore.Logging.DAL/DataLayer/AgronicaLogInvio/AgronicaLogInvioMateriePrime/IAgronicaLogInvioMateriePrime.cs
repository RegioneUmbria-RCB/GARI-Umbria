using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AgronicaNetCore.Base.Models;
using System.Threading.Tasks;
using System.Data;

namespace AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogInvio.AgronicaLogInvioMateriePrime
{
    /// <summary>
    /// Interfaccia per il Data Access Layer (DAL) dedicato alla logica di sincronizzazione
    /// delle anagrafiche Materie Prime con sistemi esterni come Antares.
    /// </summary>
    public interface IAgronicaLogInvioMateriePrime
    {
        /// <summary>
        /// Identifica le anagrafiche che necessitano di essere sincronizzate.
        /// È una funzione generica che replica la logica di 'LeggiLogAnagrafeJoinInvio'.
        /// </summary>
        /// <param name="tipoAnagrafica">Il nome dell'anagrafica da processare (es. "Materie_Prime").</param>
        /// <param name="tipoEsportazione">L'ID del flusso di esportazione.</param>
        /// <param name="saCodFilter">Il valore di Sa_Cod da usare nel filtro. Passare null per non applicare il filtro.</param>
        /// IEnumerable<int> elemCodFilter,
        /// <param name="objParametriServer">Parametri di contesto del server.</param>
        /// <returns>Un DataTable con l'elenco delle anagrafiche da processare.</returns>
        Task<DataTable> GetAnagraficheDaSincronizzareAsync(
            string tipoAnagrafica,
            int tipoEsportazione,
            int? saCodFilter,
            IEnumerable<int> elemCodFilter,
            AgronicaCoreParametriServer objParametriServer);
    }
}
