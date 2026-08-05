using System;
using System.Data;

namespace InData.Agenda
{
    public class WriteRicettexAgenda
    {
        public int Ricetta_Cod { get; set; }
        public int? Ricetta_Operazione_Cod { get; set; }
        public int Id_Agenda { get; set; }

        public int? Inviato { get; set; }
        public DateTime? Data_Invio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }
        public int? DataLock { get; set; }

        public WriteRicettexAgenda() { }
    }

    public class RicettexAgendaRow : DataRowWrapper
    {
        public RicettexAgendaRow(DataRow row) : base(row) { }
    }
}
