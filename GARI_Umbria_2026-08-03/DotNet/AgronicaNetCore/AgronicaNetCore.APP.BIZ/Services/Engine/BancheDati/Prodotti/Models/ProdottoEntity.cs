namespace AgronicaNetCore.APP.BIZ.Services.Engine.BancheDati.Prodotti.Models
{
    public class ProdottoEntity
    {
        public int codice { get; set; }
        public string? descrizione { get; set; }
        public string? nomeComune { get; set; }
        public int elemCod { get; set; }
        public int unitaDiMisuraCod { get; set; }
        public int specieCod { get; set; }
        public double N { get; set; }
        public double P2O5 { get; set; }
        public double K20 { get; set; }
        public double Cu { get; set; }
    }
}
