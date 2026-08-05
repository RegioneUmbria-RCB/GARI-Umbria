using System;
using System.Data;

namespace InData.Zoo
{
    public class WriteIntervento
    {
        public int Id_Terapia { get; set; }
        public int Id_Intervento { get; set; }
        public string Intervento_Des { get; set; }
        public int? Ordine { get; set; }

        public int? inviato { get; set; }
        public DateTime? DataInvio { get; set; }
        public DateTime? Data_Creazione { get; set; }
        public DateTime? Data_Modifica { get; set; }
        public string Utente_Creazione { get; set; }
        public string Utente_Modifica { get; set; }
        public DateTime? Validita_Inizio { get; set; }
        public DateTime? Validita_Fine { get; set; }

        public WriteIntervento() { }

        public WriteIntervento(int idTerapia, int idIntervento, string des, int ordine = 0)
        {
            this.Id_Terapia = idTerapia;
            this.Id_Intervento = idIntervento;
            this.Intervento_Des = des;
            this.Ordine = ordine;
        }
    }

    public class InterventoRow : DataRowWrapper
    {
        public InterventoRow(DataRow row) : base(row) { }
    }
}
