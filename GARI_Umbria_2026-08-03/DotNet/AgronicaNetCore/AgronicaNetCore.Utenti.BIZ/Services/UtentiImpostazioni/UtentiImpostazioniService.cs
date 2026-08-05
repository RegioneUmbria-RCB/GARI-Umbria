using AgronicaCoreDTOStd.OutData;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.BIZ.Resources;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.Utenti.BIZ.Services.UtentiImpostazioni
{
    public class UtentiImpostazioniService : BaseServiceUtentiBIZ, IUtentiImpostazioniService
    {
        private readonly IUtentiImpostazioni _utentiImpostazioniDAL;

        public UtentiImpostazioniService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _utentiImpostazioniDAL = _serviceProvider.GetRequiredService<IUtentiImpostazioni>();
        }

        public async Task<DataTable?> Read_User_Then_SuperUserAsync(int cod, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable? res = null;

            try
            {
                res = await _utentiImpostazioniDAL.Read_User_Then_SuperUserAsync(cod, objParametriUtenti, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }

            return res;
        }

        public async Task<DataTable?> ReadAsync(int cod, int User1_SuperUser2, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable? res = null;

            try
            {
                res = await _utentiImpostazioniDAL.ReadAsync(cod, User1_SuperUser2, objParametriUtenti, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }

            return res;
        }

        public async Task<DataInizioEFine> LeggiAnnataAgrariaAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            var dataInizioEFine = new DataInizioEFine();
            try
            {
                dataInizioEFine = await _utentiImpostazioniDAL.CropYearAsync(DateTime.Now, objParametriUtenti, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }

            return dataInizioEFine;
        }

    }
}
