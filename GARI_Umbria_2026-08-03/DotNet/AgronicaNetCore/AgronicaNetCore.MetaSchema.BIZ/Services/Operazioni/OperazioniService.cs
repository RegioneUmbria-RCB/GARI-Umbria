using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreDTOStd.OutData.FiltroRicerca;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.BIZ.Resources;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazioni;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioniFiltroMono;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.MetaSchema.BIZ.Services.Operazioni
{
    public class OperazioniService : BaseServiceMetaschemaBIZ, IOperazioniService
    {
        private readonly IOperazioni _operazioni;
        private readonly IUtentiImpostazioniFiltroMono _utentiImpostazioniMono;

        public OperazioniService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _operazioni = _serviceProvider.GetRequiredService<IOperazioni>();
            _utentiImpostazioniMono = _serviceProvider.GetRequiredService<IUtentiImpostazioniFiltroMono>();
        }

        public async Task<DtConVisibilita_OUT> Operazioni_GestioneFiltroUtente_LeggiAsync(LeggiOperazioni_IN leggiOperazioni, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            DtConVisibilita_OUT dtConVisibilita;
            try
            {
                var utentiImpostazioniMonoDt = await _utentiImpostazioniMono.LeggiAsync(TipiEnumerativi.Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_OPERAZIONI, 0, objParametriUtenti, objParametriServer);
                dtConVisibilita = await _operazioni.Operazioni_GestioneFiltroUtente_LeggiAsync(leggiOperazioni, utentiImpostazioniMonoDt, objParametriServer, objParametriUtenti);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return dtConVisibilita;
        }

        public async Task<DataTable> LeggiOperazioniPerTipoAsync(string tipoGruppoOperazione,
            AgronicaCoreParametriServer objParametriServer)
        {
            DataTable dt;
            try
            {
                dt = await _operazioni.LeggiOperazioniPerTipoAsync(tipoGruppoOperazione, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return dt;
        }
    }
}