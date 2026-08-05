namespace AgronicaNetCore.Base.Models
{
    public class AgronicaCoreParametriServer : AgronicaCoreParametri
    {
        public AgronicaCoreParametriServer() : base() { }

        public AgronicaCoreParametriServer(string pivaSuperUser, string usernameOperazione, string utenteUsername, string utenteCodFiscale, string superUserUsername, 
            DateTime finestraTemporaleInizio, DateTime finestraTemporaleFine, enumVisibilita flagVisibilita, enumCancellazioneLogica flagCancellazioneLogica, 
            string stringaConnessione, string logDirectory, string logFileName, string logDescrizioneUtente) 
            : base(pivaSuperUser, usernameOperazione, utenteUsername, utenteCodFiscale, superUserUsername, 
                  finestraTemporaleInizio, finestraTemporaleFine, flagVisibilita,flagCancellazioneLogica,
                  stringaConnessione, logDirectory, logFileName, logDescrizioneUtente)
        
        { }
    }
}
