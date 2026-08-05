using System;
using System.Data;

namespace InData.Zoo
{
    public class WriteRicetteZooDettaglioTecnico
    {
        public int Id_Ricetta { get; set; }
        public int Id_Agenda { get; set; }
        public int Id_Mov { get; set; }
        public int Id_Mov_Det { get; set; }
        public int Id_Reg_Dettaglio { get; set; }
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        
        public int? Dett_Cod { get; set; }
        public string Lotto { get; set; }
        public string Extra_Str { get; set; }
        public int? Extra_Int { get; set; }
        public DateTime? Extra_Date { get; set; }

        public int? Inviato { get; set; }
        public DateTime? DataInvio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteRicetteZooDettaglioTecnico() { }

        public WriteRicetteZooDettaglioTecnico(string piva, int saCod, int idRicetta, int idAgenda, int idMov, int idMovDet, int idRegDet)
        {
            this.Piva = piva;
            this.Sa_Cod = saCod;
            this.Id_Ricetta = idRicetta;
            this.Id_Agenda = idAgenda;
            this.Id_Mov = idMov;
            this.Id_Mov_Det = idMovDet;
            this.Id_Reg_Dettaglio = idRegDet;
        }
    }

    public class RicetteZooDettTecnicoRow : DataRowWrapper
    {
        public RicetteZooDettTecnicoRow(DataRow row) : base(row) { }
    }
}
