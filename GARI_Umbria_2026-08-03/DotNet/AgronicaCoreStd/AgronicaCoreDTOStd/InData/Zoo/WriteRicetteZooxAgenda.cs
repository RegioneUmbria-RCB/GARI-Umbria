using System;
using System.Data;

namespace InData.Zoo
{
    public class WriteRicetteZooxAgenda
    {
        public int Id_Ricetta { get; set; }
        public int Id_RigaRicetta { get; set; }
        public int Id_Agenda { get; set; }
        public string SuperUser_Ricetta { get; set; }

        public int? Inviato { get; set; }
        public DateTime? DataInvio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteRicetteZooxAgenda() { }

        public WriteRicetteZooxAgenda(int idRicetta, int idRiga, int idAgenda)
        {
            this.Id_Ricetta = idRicetta;
            this.Id_RigaRicetta = idRiga;
            this.Id_Agenda = idAgenda;
        }
    }

    public class RicetteZooxAgendaRow : DataRowWrapper
    {
        public RicetteZooxAgendaRow(DataRow row) : base(row) { }
    }
}
