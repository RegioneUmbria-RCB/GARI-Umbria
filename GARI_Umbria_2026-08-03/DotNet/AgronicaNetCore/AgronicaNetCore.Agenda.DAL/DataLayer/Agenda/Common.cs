using AgronicaNetCore.Base.Constants;

namespace AgronicaNetCore.Agenda.DAL.DataLayer.Agenda
{
    public static class Common
    {
        // Operazioni non sottostanti verifica di conformità ma propedeutiche per la verifica di altre
        public static readonly List<int> OperazioniNonOggettoDiVerifica = new List<int>
        {
            LAV_COD.LAVCOD_DISORIENTAMENTO_SESSUALE,
            LAV_COD.LAVCOD_CONFUSIONE_SESSUALE,
            LAV_COD.LAVCOD_CONCIA_SEME,
            LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA,
        };

        public static readonly List<int> OperazioniConAcqua = new List<int>
        {
            LAV_COD.LAVCOD_CONCIMAZIONE_FOGLIARE,
            LAV_COD.LAVCOD_FERTIRRIGAZIONE,
            LAV_COD.LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
            LAV_COD.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
            LAV_COD.LAVCOD_DISERBO,
            LAV_COD.LAVCOD_DISSECCAMENTO,
            LAV_COD.LAVCOD_GEODISINFESTAZIONE,
            LAV_COD.LAVCOD_CONCIA_SEME,
            LAV_COD.LAVCOD_TRATTAMENTO_FITOREGOLATORE,
            LAV_COD.LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
            LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA
        };

        public static readonly List<int> OperazioniRilievo = new List<int>
        {
            LAV_COD.LAVCOD_RILIEVO_AVVERSITA_CAMPO,
            LAV_COD.LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE,
            LAV_COD.LAVCOD_RILIEVO_INDICI_MATURITA,
            LAV_COD.LAVCOD_DANNI_RACCOLTA,
            LAV_COD.LAVCOD_FASI_FENOLOGICHE,
            LAV_COD.LAVCOD_RILIEVO_ERBE_INFESTANTI,
            LAV_COD.LAVCOD_RILIEVO_PIOGGE,
            LAV_COD.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
        };

        public static readonly List<int> OperazioniTrattamento = new List<int>
        {
            LAV_COD.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
            LAV_COD.LAVCOD_CONCIA_SEME,
            LAV_COD.LAVCOD_GEODISINFESTAZIONE,
            LAV_COD.LAVCOD_DISERBO,
            LAV_COD.LAVCOD_DISSECCAMENTO,
            LAV_COD.LAVCOD_TRATTAMENTO_FITOREGOLATORE,
            LAV_COD.LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
            LAV_COD.LAVCOD_CONFUSIONE_SESSUALE,
            LAV_COD.LAVCOD_DISORIENTAMENTO_SESSUALE,
            LAV_COD.LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA,
            LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA,
        };

        public static readonly List<int> OperazioniTrappoleInstallate = new List<int>
        {
            LAV_COD.LAVCOD_DISTRIBUZIONE_INSETTI,
            LAV_COD.LAVCOD_CATTURE_MASSA,
            LAV_COD.LAVCOD_INSTALLAZIONE_TRAPPOLE,
            LAV_COD.LAVCOD_REINNESCO_TRAPPOLE,
        };

        public static readonly List<int> OperazioniFertilizzazione = new List<int>
        {
            LAV_COD.LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
            LAV_COD.LAVCOD_CONCIMAZIONE_FOGLIARE,
            LAV_COD.LAVCOD_DISTRIBUZIONE_AMMENDANTI,
            LAV_COD.LAVCOD_DISTRIBUZIONE_CONCIME,
            LAV_COD.LAVCOD_FERTIRRIGAZIONE,
            LAV_COD.LAVCOD_SARCHIATURA_CONCIMAZIONE
        };

        public static readonly List<int> OperazioniRaccolta = new List<int>
        {
            LAV_COD.LAVCOD_RACCOLTA
        };
        public static readonly List<int> OperazioniSeminaTrapianto = new List<int>
        {
            LAV_COD.LAVCOD_SEMINA,
            LAV_COD.LAVCOD_TRAPIANTO,
            LAV_COD.LAVCOD_SOVESCIO,
            LAV_COD.LAVCOD_SOD_SEDDING
        };
        public static readonly List<int> OperazioniConCaricoMagazzinoNoConferimento = new List<int>
        {
            LAV_COD.LAVCOD_BOLLA_RICEVUTA,
            LAV_COD.LAVCOD_FATTURA_RICEVUTA,
            LAV_COD.LAVCOD_NOTA_ACCREDITO_RICEVUTA,
            LAV_COD.LAVCOD_CARICO
        };
        public static readonly List<int> OperazioniConferimento = new List<int>
        {
            LAV_COD.LAVCOD_ACCETTAZIONE_DIVERSI, 
            LAV_COD.LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
            LAV_COD.LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
        };
    }
}
