using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.ActivityImport
{
    public interface IActivityImportData
    {
        /// <summary>
        /// Il codice indentificativo dell'operazione del sistema esterno
        /// </summary>
        string codice { get; set; }
        /// <summary>
        /// Codice identificativo dell'operazione all'interno del sistema GIAS.
        /// Per far riferimento a un'operazione ancora non presente nel sistema
        /// (da creare) è necessario valorizzare i campo con una string vuota.
        /// </summary>
        string codice_esterno { get; set; }
        /// <summary>
        /// Se impostato a <tt>True</tt> indica che l'operazione è da cancellare.
        /// </summary>
        bool flag_cancellazione { get; set; }
        /// <summary>
        /// Se impostato a <tt>True</tt> indica che gli impianti utilizzati
        /// nell'operazione sono da bloccare a seguito del salvataggio della stessa.
        /// </summary>
        bool flag_blocco_impianti { get; set; }
        /// <summary>
        /// Data in cui è stata effettuata l'operazione.
        /// </summary>
        DateTime data { get; set; }
        /// <summary>
        /// Codice di riferimento per il tipo di operazione effettuata (lav_cod).
        /// </summary>
        int tipo_operazione { get; set; }
        /// <summary>
        /// Note relative all'operazione.
        /// </summary>
        string note { get; set; }
        IEnumerable<IActivityImportPlant> impianti { get; set; }
    }

    public interface IActivityImportPlant
    {
        /// <summary>
        /// Chiave dell'impianto.
        /// </summary>
        string plot_id { get; set; }
        /// <summary>
        /// Superficie trattata in ettari.
        /// </summary>
        decimal superficie_trattata { get; set; }
    }
}
