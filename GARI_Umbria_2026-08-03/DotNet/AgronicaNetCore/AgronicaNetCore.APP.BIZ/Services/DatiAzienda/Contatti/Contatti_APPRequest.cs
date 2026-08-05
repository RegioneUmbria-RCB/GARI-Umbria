namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Contatti
{
    public class Contatti_APPRequest
    {
        public string? Piva { get; set; }

        public bool IncludeContatti { get; set; }

        public bool IncludeFornitoriMeteo { get; set; }

        public bool IncludeFornitori { get; set; }

        public bool FlagIndirizzi { get; set; }

        public bool FlagRubrica { get; set; }

        public bool ModalitaDemetra { get; set; }
    }
}