using System;
using System.Data;

namespace InData.Zoo
{
    public class WriteRicetteZooDettagli
    {
        public int IdRicetta { get; set; }
        public int IdAgenda { get; set; }
        public int IdMov { get; set; }
        public int IdDettaglio { get; set; }
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        
        public int? Elem_Cod { get; set; }
        public int? Pro_Cod { get; set; }
        public int? Mat_Cod { get; set; }
        public int? Udm_Cod { get; set; }
        public string RegScoNumero { get; set; }
        public int? FlAntimicrobico { get; set; }
        public int? FlOrmonale { get; set; }
        public string FlTipoMedicinale { get; set; }
        public int? FlVaccino { get; set; }
        public string MangimeComposizione { get; set; }
        public string MangimeDenominazione { get; set; }
        public string Posologia { get; set; }
        public string ProdottoAic { get; set; }
        public float? Quantitativo { get; set; }
        public float? Qta_Dose { get; set; }
        public int? Udm_Dose { get; set; }
        public int? Arrotondamento_Peso { get; set; }
        public int? Massivo { get; set; }

        public int? Inviato { get; set; }
        public DateTime? DataInvio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteRicetteZooDettagli() { }

        public WriteRicetteZooDettagli(string piva, int saCod, int idRicetta, int idAgenda, int idMov, int idDettaglio)
        {
            this.Piva = piva;
            this.Sa_Cod = saCod;
            this.IdRicetta = idRicetta;
            this.IdAgenda = idAgenda;
            this.IdMov = idMov;
            this.IdDettaglio = idDettaglio;
        }
    }

    public class RicetteZooDettagliRow : DataRowWrapper
    {
        public RicetteZooDettagliRow(DataRow row) : base(row) { }
    }
}
