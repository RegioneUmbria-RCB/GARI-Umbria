using System;
using System.Data;

namespace InData.Zoo
{
    public class WriteRicetteZooMovimenti
    {
        public int IdRicetta { get; set; }
        public int IdAgenda { get; set; }
        public int IdMov { get; set; }
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        
        public string Cau_Mov { get; set; }
        public string Mov_Desc { get; set; }
        public int? Cod_RisUm { get; set; }
        public DateTime? Data_Movimento { get; set; }
        public DateTime? Scadenza { get; set; }
        public float? Doc_Numero { get; set; }
        public float? Num_Protocollo { get; set; }

        public int? Inviato { get; set; }
        public DateTime? DataInvio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteRicetteZooMovimenti() { }

        public WriteRicetteZooMovimenti(string piva, int saCod, int idRicetta, int idAgenda, int idMov)
        {
            this.Piva = piva;
            this.Sa_Cod = saCod;
            this.IdRicetta = idRicetta;
            this.IdAgenda = idAgenda;
            this.IdMov = idMov;
        }
    }

    public class RicetteZooMovimentiRow : DataRowWrapper
    {
        public RicetteZooMovimentiRow(DataRow row) : base(row) { }
    }
}
