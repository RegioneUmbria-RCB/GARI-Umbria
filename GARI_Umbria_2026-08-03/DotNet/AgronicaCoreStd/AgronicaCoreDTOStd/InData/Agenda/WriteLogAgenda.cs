using System;
using System.Data;

namespace InData.Agenda
{
    public class WriteLogAgenda
    {
        public int ID { get; set; }
        public string SuperUser { get; set; }
        public string Utente { get; set; }
        public DateTime? Data_Ora_Lavorazione { get; set; }
        public int? Tipo_Operazione { get; set; }
        public string Des_Lib { get; set; }
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        public int? Id_Agenda { get; set; }
        public int? Lav_Cod { get; set; }
        public DateTime? Data_Ora_RegistrazioneLog { get; set; }
        public int? Id_Servizio { get; set; }
        public string Object_Data { get; set; }
        public int? Origine { get; set; }
        public int? Raccoglitore_Cod { get; set; }

        public WriteLogAgenda() { }
    }

    public class LogAgendaRow : DataRowWrapper
    {
        public LogAgendaRow(DataRow row) : base(row) { }
    }
}
