using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class InfomodificaOperazioneSingola
    {
        /// <summary>
        /// Definisce il tipo di operazione da eseguire.
        /// 
        /// In Angular equivale all'enumerativo:
        /// <code>
        ///     enum Enum_DBTypeOperation {
        ///         Read = 0,
        ///         Write = 1,
        ///         Update = 2,
        ///         Delete = 3
        ///     }
        /// </code>
        /// </summary>
        public int tipo { get; set; }

        /// <summary>
        /// La data dell'attività selezionata
        /// </summary>
        public string dataOp { get; set; }

        public string id_agenda { get; set; }

        public string lav_cod { get; set; }

        public string blocco_flag { get; set; }

        public int veg_cod { get; set; }

        public string piva { get; set; }

        public VariabiliInSessione_NG variabiliInSessione_NG { get; set; }
    }
}
