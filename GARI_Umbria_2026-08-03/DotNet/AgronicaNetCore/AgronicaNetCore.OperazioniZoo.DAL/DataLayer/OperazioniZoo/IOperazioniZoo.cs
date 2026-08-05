using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.OperazioniZoo
{
    public interface IOperazioniZoo
    {
        public Task<DataTable> LeggiCentriZooAsync(string piva, AgronicaCoreParametriServer objParametriServer, bool visibilita_totale);

        public Task<DataTable> LeggiStalleZooAsync(string piva, int centro, AgronicaCoreParametriServer objParametriServer, bool visibilita_totale);

        public Task<DataTable> LeggiStalleRaggruppamentiZooAsync(string piva, int centro, int stanum, AgronicaCoreParametriServer objParametriServer);

        public Task<DataTable> LeggiCategorieFarmaciSemplificateAsync(AgronicaCoreParametriServer objParametriServer);

        public Task<DataTable> CaricaAgendaZooAsync(string piva, int sa_Cod, int sta_Num, DateTime validita_Inizio, DateTime validita_Fine, DateTime validita_FineGiacenze, List<int> filtroCentri, int livelloCompatibilita, AgronicaCoreParametriServer objParametriServer);

        public Task<DataTable> CaricaAgendaZooNewAsync(string piva, int sa_Cod, int sta_Num, DateTime validita_Inizio, DateTime validita_Fine, DateTime validita_FineGiacenze, List<int> filtroCentri, int livelloCompatibilita, AgronicaCoreParametriServer objParametriServer);
        
        public Task<DataTable> CaricaAgendaZooAsyncCarichiScarichiAsync(string piva, int sa_Cod, int sta_Num, DateTime validita_Inizio, DateTime validita_Fine, DateTime validita_FineGiacenze, List<string> filtroCentri, int livelloCompatibilita, AgronicaCoreParametriServer objParametriServer);

        public Task<DataTable> CaricaAgendaZooAsyncSpostamentiAsync(string piva, int sa_Cod, int sta_Num, DateTime validita_Inizio, DateTime validita_Fine, DateTime validita_FineGiacenze, List<string> filtroCentri, int livelloCompatibilita, AgronicaCoreParametriServer objParametriServer);

        public Task<DataTable> CaricaAgendaZooAsyncAltreAsync(string piva, int sa_Cod, int sta_Num, DateTime validita_Inizio, DateTime validita_Fine, DateTime validita_FineGiacenze, List<string> filtroCentri, int livelloCompatibilita, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> Leggi_GiacenzeAsync(
           string Piva,
           int Sa_Cod,
           int STA_NUM,
           int Raggruppamento_Cod,
           int Cod_Animale,
           DateTime Data,
           AgronicaCoreParametriServer objParametriServer,
           bool bAll = false,
           bool Filtro_Visibilita_Utente = false,
           List<int> listCod_Animali = null,
           bool filtraGiacenze1 = true,
           bool filtraFornitori = false,
           bool mostraPesate = false,
           bool mostraAnomalie = false,
           //string xFiltroAggiuntivo = "",
           string Matricola = "",
           bool MostraGGPrimoCaricamento = false,
           string CFproprietario = "",
           List<string> listMatricola_Animali = null, 
           bool leggiUltimaPesata = false);

        public Task<DataTable> LeggiTrattamentiAsync(
            string piva,
            int saCod,
            int staNum,
            int raggruppamentoCod,
            int codAnimale,
            string matricola,
            DateTime dataInizio,
            DateTime dataFine,
            bool filtroVisibilitaUtente,
            List<int>? listCodAnimali,
            int[]? farmCatList,
            int[]? farmCatSemplList,
            AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiCapiSenzaTrattamentiAsync
            (
                string piva,
                int saCod,
                int staNum,
                int raggruppamentoCod,
                int codAnimale,
                DateTime data,
                int giorniSenzaTrattamenti,
                bool filtroVisibilitaUtente,
                List<int>? listCodAnimali,
                int[]? farmCatList,
                int[]? farmCatSemplList,
                AgronicaCoreParametriServer objParametriServer,
                int compatibilityLevel,
                bool mostraGGPrimoCaricamento = false,
                bool mostraAnomalie = false
            );

        Task<DataTable> LeggiStazionamentoZooAsync
           (
            string piva,
            int saCod,
            int staNum,
            int raggruppamentoCod,
            int codAnimale,
            DateTime data,
            int giorniStazionamento,
            bool filtroVisibilitaUtente,
            List<int>? listCod_Animali,
            AgronicaCoreParametriServer objParametriServer,
            int compatibilityLevel
           );

        Task<DataTable> LeggiTrattamentiCorrenti(string piva, int saCod, int staNum, DateTime data, AgronicaCoreParametri objP);

        Task<DataTable> LeggiCaricoCapiAsync(
            string piva,
            int saCod,
            int staNum,
            int raggruppamentoCod,
            int codAnimale,
            List<int>? listCodAnimali,
            string matricola,
            DateTime dataInizio,
            DateTime dataFine,
            int lavCod,
            bool flagFornitore,
            bool flagAziendaUscita,
            bool mostraPesate,
            bool filtroVisibilitaUtente,
            AgronicaCoreParametriServer objParametriServer, 
            int compatibilityLevel);

        Task<DataTable> LeggiScaricoCapiAsync(
            string piva,
            int saCod,
            int staNum,
            int raggruppamentoCod,
            int codAnimale,
            List<int>? listCodAnimali,
            string matricola,
            DateTime dataInizio,
            DateTime dataFine,
            int lavCod,
            bool flagFornitore,
            bool flagAziendaUscita,
            bool mostraPesate,
            bool filtroVisibilitaUtente,
            AgronicaCoreParametriServer objParametriServer,
            int compatibilityLevel);
    }
}
