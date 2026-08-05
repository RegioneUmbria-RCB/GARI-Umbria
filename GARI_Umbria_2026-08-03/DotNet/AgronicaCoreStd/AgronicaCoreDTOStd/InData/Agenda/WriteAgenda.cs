using System;
using System.Data;

namespace InData.Agenda
{
    public class WriteAgenda
    {
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int Id_Agenda { get; set; }
        public int Lav_Cod { get; set; }
        public string Des_Lib { get; set; }

        public int? Sta_Num { get; set; }
        public int? Linea_Cod { get; set; }
        public int? Preparazione_Cod { get; set; }
        public int? Id_Trasformazione { get; set; }
        public int? Tipo_Accettazione { get; set; }
        public int? Audit_Cod { get; set; }
        public int? Stato_Export { get; set; }
        public int? Stato_Export2 { get; set; }
        public int? Tipo_Visibilita { get; set; }
        public int? ChkCoge_Manuale { get; set; }
        public int? Id_Attivita { get; set; }
        public int? Modulo { get; set; }
        public int? Raccoglitore_Cod { get; set; }
        public int? Split { get; set; }
        public int? Pratica_Cod { get; set; }
        public string Origine { get; set; }
        public int? Stato_Cod { get; set; }
        public int? DaRemoto { get; set; }

        public int? Inviato { get; set; }
        public DateTime? Data_Invio { get; set; }
        public int? Blocco_Flag { get; set; }
        public DateTime? Blocco_Data { get; set; }
        public string Blocco_Username { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteAgenda() { }

        public WriteAgenda(string piva, int saCod, int idAgenda)
        {
            this.Piva = piva;
            this.Sa_Cod = saCod;
            this.Id_Agenda = idAgenda;
        }
    }

    public class AgendaRow : DataRowWrapper
    {
        public AgendaRow(DataRow row) : base(row) { }
    }
}
