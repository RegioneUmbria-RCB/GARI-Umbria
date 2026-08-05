using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Base.Constants;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.CodiciAnagrafe
{
    public class LeggiCodiciAnagrafe
    {
        public EntitaAlberoImprese ElementoAnagrafico { get; set; }
        public bool EscludiCodiciCliente { get; set; }

        public List<Impresa.PK> ChiaviImprese { get; set; } = new List<Impresa.PK>();
        public List<CentroAziendale.PK> ChiaviCentriAziendali { get; set; } = new List<CentroAziendale.PK>();
        public List<Campo.PK> ChiaviCampi { get; set; } = new List<Campo.PK>();
        public List<Appezzamento.PK> ChiaviAppezzamenti { get; set; } = new List<Appezzamento.PK>();
        public List<Impianto.PK> ChiaviImpianti { get; set; } = new List<Impianto.PK>();
        public List<Esercizio.PK> ChiaviEsercizi { get; set; } = new List<Esercizio.PK>();
        public List<Fabbricato.PK> ChiaviFabbricati { get; set; } = new List<Fabbricato.PK>();
        public List<AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine.PK> ChiaviParcoMacchine { get; set; } = new List<AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine.PK>();
        public List<Contatto.PK> ChiaviContatti { get; set; } = new List<Contatto.PK>();
    }
}
