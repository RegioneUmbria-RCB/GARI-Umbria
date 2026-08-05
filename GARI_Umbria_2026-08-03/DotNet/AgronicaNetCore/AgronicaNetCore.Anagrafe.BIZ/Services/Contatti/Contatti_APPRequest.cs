namespace AgronicaNetCore.Anagrafe.BIZ.Services.Contatti
{
    public class Contatti_APPRequest
    {
        public string? Piva { get; set; }

        public bool IncludeContatti { get; set; }

        public bool IncludeFornitoriMeteo { get; set; }

        public bool IncludeFornitoriNormali { get; set; }
    }
}