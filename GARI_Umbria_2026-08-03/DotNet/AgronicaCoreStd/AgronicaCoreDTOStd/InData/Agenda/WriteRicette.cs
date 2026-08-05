using System;
using System.Data;
using InData.Anagrafica;

namespace InData.Agenda
{
    public class WriteRicette
    {
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int Ricetta_Cod { get; set; }
        public string Ricetta_Des { get; set; }
        public string Ricetta_Des_Long { get; set; }
        public enum_TipoRicetta Tipo_Ricetta { get; set; }
        public int? Veg_Cod { get; set; }        
        public string Note { get; set; }
        public int? Programmazione_Cod { get; set; }
        public string Ricetta_Numero { get; set; }
        public int? Imputazione_Cod { get; set; }
        public int? Imputazione_Fase_Cod { get; set; }
        public string Origine { get; set; }

        public int? Inviato { get; set; }
        public DateTime? Data_Invio { get; set; }
        public int? Blocco_Flag { get; set; }
        public DateTime? Blocco_Data { get; set; }
        public string Blocco_Username { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }
        public int? DataLock { get; set; }

        public WriteRicette() { }

        public WriteRicette(string piva, int saCod, int ricettaCod)
        {
            this.Piva = piva;
            this.Sa_Cod = saCod;
            this.Ricetta_Cod = ricettaCod;
        }
    }

    public class RicetteRow : DataRowWrapper
    {
        public RicetteRow(DataRow row) : base(row) { }
    }
}
