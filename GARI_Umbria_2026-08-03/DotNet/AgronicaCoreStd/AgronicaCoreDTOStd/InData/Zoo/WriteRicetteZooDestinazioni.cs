using System;
using System.Data;

namespace InData.Zoo
{
    public class WriteRicetteZooDestinazioni
    {
        public int IdRicetta { get; set; }
        public int IdAgenda { get; set; }
        public int IdMov { get; set; }
        public int IdDettaglio { get; set; }
        public int IdDestinazione { get; set; }
        public string Piva { get; set; }
        public int? Sa_Cod { get; set; }
        
        public int? CodAnimale { get; set; }
        public string Matricola { get; set; }
        public string CodificaCodice { get; set; }
        public string CodificaDescrizione { get; set; }
        public string DiagnosiCodice { get; set; }
        public string DiagnosiDescrizione { get; set; }
        public DateTime? DataNascita { get; set; }
        public string FlStatoAnomalia { get; set; }
        public string Identificativo { get; set; }
        public string Numero { get; set; }
        public string Note { get; set; }
        public int? NumeroAnimali { get; set; }
        public string Sesso { get; set; }
        public string SomministrazioneCodice { get; set; }
        public string SottocategoriaCodice { get; set; }
        public string SpecieCodice { get; set; }
        public int? Cardinalita { get; set; }
        public int? TempiSospensione { get; set; }
        public string RegSco_Numero { get; set; }

        public int? Inviato { get; set; }
        public DateTime? DataInvio { get; set; }

        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public WriteRicetteZooDestinazioni() { }

        public WriteRicetteZooDestinazioni(string piva, int saCod, int idRicetta, int idAgenda, int idMov, int idDettaglio)
        {
            this.Piva = piva;
            this.Sa_Cod = saCod;
            this.IdRicetta = idRicetta;
            this.IdAgenda = idAgenda;
            this.IdMov = idMov;
            this.IdDettaglio = idDettaglio;
        }
    }
    
    public class RicetteZooDestinazioniRow : DataRowWrapper
    {
        public RicetteZooDestinazioniRow(DataRow row) : base(row) { }
    }
}
