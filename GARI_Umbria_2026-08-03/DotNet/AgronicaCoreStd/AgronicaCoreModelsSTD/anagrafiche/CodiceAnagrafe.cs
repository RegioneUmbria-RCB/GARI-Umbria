using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class CodiceAnagrafe : CodiceAnagrafeBase
    {
        public int lunghezza { get; set; }
        public string picture { get; set; }
        public string tipo { get; set; }
        public string gruppo { get; set; }
        public int genitore { get; set; }
        public string creatore { get; set; }

        public IntervalloTemporale validita { get; set; }
        public bool flag_cancellazione { get; set; }

        public CodiceAnagrafe(int codice) : base(codice, "")
        {
            this.flag_cancellazione = false;
        }
        public CodiceAnagrafe(int codice, string descrizione) : base(codice, descrizione)
        {
            this.flag_cancellazione = false;
        }

        public CodiceAnagrafe() : base()
        {

        }
    }

    public class CodiceAnagrafeBase : BaseCodeDescr
    {
        /// <summary>
        /// Vedi enum Enum_TipoControllo in AgronicaNetCore.Base 
        /// </summary>
        public int TipoControllo_Cod { get; set; }

        public CodiceAnagrafeBase(int codice, string descr) : base(codice, descr) { }

        public CodiceAnagrafeBase() : base() { }
    }
}
