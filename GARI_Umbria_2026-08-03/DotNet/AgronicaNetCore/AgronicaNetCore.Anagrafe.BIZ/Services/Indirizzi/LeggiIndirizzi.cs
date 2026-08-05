using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Base.Constants;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Indirizzi
{
    public class LeggiIndirizzi
    {
        public EntitaAlberoImprese ElementoAnagrafico { get; set; }
        public List<int> TipoIndirizzo { get; set; } = new List<int>();
        public bool IndirizzoCompleto { get; set; }

        public List<Impresa.PK> ChiaviImprese { get; set; } = new List<Impresa.PK>();
        public List<CentroAziendale.PK> ChiaviCentriAziendali { get; set; } =
            new List<CentroAziendale.PK>();
        public List<Appezzamento.PK> ChiaviAppezzamenti { get; set; } = new List<Appezzamento.PK>();
        public List<Contatto.PK> ChiaviContatti { get; set; } = new List<Contatto.PK>();
    }
}
