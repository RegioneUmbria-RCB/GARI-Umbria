using AgronicaCoreDTOStd.InData.Zoo;
using AgronicaCoreModelsSTD.attivita;
using AgronicaNetCore.Base.Models;
using InData.Zoo;
using System.Data;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.OperazioniZoo
{
    public interface IOperazioniZooService
    {
        Task<DataTable> LeggiCentriAziendaliZooAsync(string piva, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti);

        Task<DataTable> LeggiStalleZooAsync(string piva, int centro, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti);

        Task<DataTable> LeggiStalleRaggruppamentiZooAsync(string piva, int centro, int stanum, AgronicaCoreParametriServer objParametriServer);

        Task<List<Zootecnia>> LeggiOperazioniZooAsync(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti);

        Task<List<Zootecnia>> LeggiOperazioniZooPreferiteAsync(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti);

        Task<DataTable> LeggiOperazioniAgendaZooAsync(string piva, int sa_Cod, int sta_Num, DateTime validita_Inizio, DateTime validita_Fine, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiOperazioniAgendaZooNewAsync(string piva, int sa_Cod, int sta_Num, DateTime validita_Inizio, DateTime validita_Fine, AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiGiacenzeZooAsync(
            LeggiGiacenzeZooDto paramsLeggiGiacenze,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti,
            string? superUserUsername = null);

        Task<DataTable> LeggiTrattamentiZooAsync(
             GetTrattamentiZooDto filter,
             AgronicaCoreParametriUtenti objParametriUtenti,
             AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiCapiSenzaTrattamentiAsync(
            GetSenzaTrattamentiZooDto filter,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiStazionamentoZooAsync(
            GetStazionamentoZooDto filter,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer);

        Task<DataTable> LeggiGiacenzeZooDaAAsync(
            string piva,
            int codCentro,
            int codStalla,
            int codRaggruppamento,
            int codAnimale,
            DateTime periodoInizio,
            DateTime periodoFine,
            bool mostraPesate,
            bool mostraGgInizioTot,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer);

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
            bool mostraPesate,
            bool mostraGgInizioTot,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Legge le giacenze dei capi animali per la prima somministrazione (necessaria per controllo su .
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="objP_Server"></param>
        /// <param name="objP_Utenti"></param>
        /// <returns></returns>
        Task<DataTable> LeggiGiacenzeZooFirstSommAsync(LeggiGiacenzeZooFirstSommDto dto, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti);
    }
}
