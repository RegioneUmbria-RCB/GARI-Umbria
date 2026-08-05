using AgronicaNetCore.Base.Interfaces;
using Newtonsoft.Json;
using System.Data;
using System.Data.Common;

namespace AgronicaNetCore.Base.Models
{
    public class AgronicaCoreParametri : IDeepCloneObject<AgronicaCoreParametri> //, IDBConnection,IDBTransaction
    {
        public string PivaSuperUser { get; set; }
        public string UsernameOperazione { get; set; }
        public string UtenteUsername { get; set; }
        public string UtenteCodFiscale { get; set; }
        public string SuperUserUsername { get; set; }
        public DateTime FinestraTemporaleInizio { get; set; }
        public DateTime FinestraTemporaleFine { get; set; }
        public enumVisibilita FlagVisibilita { get; set; }
        public enumCancellazioneLogica FlagCancellazioneLogica { get; set; }
        public DbConnection? objConnessione { get; set; }
        public DbTransaction? objTransazione { get; set; }
        public string StringaConnessione { get; set; }
        public string LogDirectory { get; set; }
        public string LogFileName { get; set; }
        public string LogDescrizioneUtente { get; set; }
        public int Lingua_Cod { get; set; } = 1;
        public int TimeoutQuery { get; set; } = 1200;
        public agronicacoreparametri_tipologia Tipologia { get; set; }
        public DateTime AppoggioFinestraTemporaleInizio { get; set; }
        public DateTime AppoggioFinestraTemporaleFine { get; set; }
        public string AppoggioUsernameOperazione { get; set; }
        public string LinkGiasBase { get; set; } = string.Empty;
        [JsonIgnore] public string ConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [JsonIgnore] public int ConnectionTimeout => throw new NotImplementedException();

        [JsonIgnore] public string Database => throw new NotImplementedException();

        [JsonIgnore] public ConnectionState State => throw new NotImplementedException();

        [JsonIgnore] public IDbConnection? Connection => throw new NotImplementedException();

        [JsonIgnore] public IsolationLevel IsolationLevel => throw new NotImplementedException();

        public enum enumVisibilita
        {
            visibilita_SoloNonInviati = 1,
            Visibilita_SoloNonCancellati = 1,
            Visibilita_SoloCancellati = 2,
            Visibilita_Tutti = 3
        }

        public enum enumCancellazioneLogica
        {
            CancellazioneFisica = 0,
            CancellazioneLogica = 1
        }

        public enum agronicacoreparametri_tipologia
        {
            Standard = 0,
            WineMatch = 1,
            Vinificazione = 2,
            Gestionale = 3
        }

        public AgronicaCoreParametri()
        {
            PivaSuperUser = string.Empty;
            UsernameOperazione = string.Empty;
            FinestraTemporaleInizio = new DateTime(1900, 1, 1);
            FinestraTemporaleFine = new DateTime(2100, 12, 31);
            FlagVisibilita = enumVisibilita.Visibilita_Tutti;
            FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneFisica;
            objConnessione = null;
            objTransazione = null;
            StringaConnessione = string.Empty;
            LogDirectory = string.Empty;
            LogFileName = string.Empty;
            LogDescrizioneUtente = string.Empty;
            Tipologia = agronicacoreparametri_tipologia.Standard;
        }

        public AgronicaCoreParametri(string pivaSuperUser, string usernameOperazione, string utenteUsername, string utenteCodFiscale, string superUserUsername, DateTime finestraTemporaleInizio, DateTime finestraTemporaleFine, enumVisibilita flagVisibilita, enumCancellazioneLogica flagCancellazioneLogica, string stringaConnessione, string logDirectory, string logFileName, string logDescrizioneUtente)
        {
            PivaSuperUser = pivaSuperUser;
            UsernameOperazione = usernameOperazione;
            UtenteUsername = utenteUsername;
            UtenteCodFiscale = utenteCodFiscale;
            SuperUserUsername = superUserUsername;
            FinestraTemporaleInizio = finestraTemporaleInizio;
            FinestraTemporaleFine = finestraTemporaleFine;
            FlagVisibilita = flagVisibilita;
            FlagCancellazioneLogica = flagCancellazioneLogica;
            StringaConnessione = stringaConnessione;
            LogDirectory = logDirectory;
            LogFileName = logFileName;
            LogDescrizioneUtente = logDescrizioneUtente;
            objConnessione = null;
            objTransazione = null;
        }

        public AgronicaCoreParametri CreateDeepCopy<AgronicaCoreParametri>(AgronicaCoreParametri obj)
        {
            var cpy = JsonConvert.SerializeObject(obj);
            return (JsonConvert.DeserializeObject<AgronicaCoreParametri>(cpy))!;
        }

        //public void OpenConnection(AgronicaCoreParametri objParametri)
        //{
        //    if (objConnessione == null) {
        //        objConnessione=DataProvider6Factory.Instance.
        //}

        //public void CloseConnection(AgronicaCoreParametri objParametri)
        //{
        //    throw new NotImplementedException();
        //}

        //public void OpenTransaction(AgronicaCoreParametri objParametri)
        //{
        //    throw new NotImplementedException();
        //}

        //public void CloseTransaction(AgronicaCoreParametri objParametri, bool Rollback = false)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
