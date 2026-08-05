using System;
using System.Data;

namespace InData.Zoo
{
    public class WriteRicetteZooAgenda
    {
        public int IdRicetta { get; set; }
        public int IdAgenda { get; set; }
        public string Numero { get; set; }
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }

        public int? Lav_Cod { get; set; }        
        public string Des_Lib { get; set; }
        public string Descrizione { get; set; }
        public DateTime? DataInizioTrattamento { get; set; }
        public DateTime? DataFineTrattamento { get; set; }
        public int? DurataTrattamento { get; set; }        
        public string Note { get; set; }
        public int? Intervallo_Somm { get; set; }
        public int? Numero_Somm { get; set; }

        public int? Inviato { get; set; }
        public DateTime? DataInvio { get; set; }
        public int? Blocco_Flag { get; set; }
        public DateTime? Blocco_Data { get; set; }
        public string Blocco_Username { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteRicetteZooAgenda() { }

        public WriteRicetteZooAgenda(string piva, int saCod, int idRicetta, int idAgenda)
        {
            this.Piva = piva;
            this.Sa_Cod = saCod;
            this.IdRicetta = idRicetta;
            this.IdAgenda = idAgenda;
        }

        public WriteRicetteZooAgenda(string piva, int saCod, int staNum, int idRicetta, int idAgenda, string numero)
        {
            this.Piva = piva;
            this.Sa_Cod = saCod;
            this.IdRicetta = idRicetta;
            this.IdAgenda = idAgenda;
            this.Numero = numero;
        }
    }

    public class RicetteZooAgendaRow : DataRowWrapper
    {
        public RicetteZooAgendaRow(DataRow row) : base(row) { }
    }
}
