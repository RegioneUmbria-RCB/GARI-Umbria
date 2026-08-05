using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgronicaCoreDTOStd.InData.ActivityImport
{
    /// <summary>
    /// Classe base per l'import di operazioni agenda.
    /// Il modello base viene usato da Orogel.
    /// </summary>
    /// <remarks>
    /// La classe ricalca la struttura di AgronicaCoreDTOStd.InData.Demetra.Attivita
    /// usata nei CoreWS per l'import di attività di Demetra. Non tutti i campi
    /// di AgronicaCoreDTOStd.InData.Demetra.Attivita sono presenti in questo modello.
    /// </remarks>
    public class ActivityImportData : IActivityImportData
    {
        /// <summary>
        /// Il codice indentificativo dell'operazione del sistema esterno
        /// </summary>
        public string codice { get; set; }
        /// <summary>
        /// Codice identificativo dell'operazione all'interno del sistema GIAS.
        /// Per far riferimento a un'operazione ancora non presente nel sistema
        /// (da creare) è necessario valorizzare i campo con una string vuota.
        /// </summary>
        public string codice_esterno { get; set; }
        /// <summary>
        /// Se impostato a <tt>True</tt> indica che l'operazione è da cancellare.
        /// </summary>
        public bool flag_cancellazione { get; set; } = false;
        /// <summary>
        /// Se impostato a <tt>True</tt> indica che gli impianti utilizzati
        /// nell'operazione sono da bloccare a seguito del salvataggio della stessa.
        /// </summary>
        public bool flag_blocco_impianti { get; set; } = false;
        /// <summary>
        /// Data in cui è stata effettuata l'operazione.
        /// </summary>
        public DateTime data { get; set; }
        /// <summary>
        /// Codice di riferimento per il tipo di operazione effettuata (lav_cod).
        /// </summary>
        public int tipo_operazione { get; set; }
        /// <summary>
        /// Note relative all'operazione.
        /// </summary>
        public string note { get; set; }
        public IEnumerable<IActivityImportPlant> impianti { get; set; } = new List<ActivityImportPlant>();
    }

    public class ActivityImportPlant: IActivityImportPlant
    {
        /// <summary>
        /// Chiave dell'impianto.
        /// </summary>
        public string plot_id { get; set; }
        /// <summary>
        /// Superficie trattata in ettari.
        /// </summary>
        public decimal superficie_trattata { get; set; }
    }
}
