using AgronicaCoreDTOStd.InData;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.BIZ.Resources;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiPermessi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.Utenti.BIZ.Services.UtentiPermessi
{
    public class UtentiPermessiService : BaseServiceUtentiBIZ, IUtentiPermessiService
    {
        private readonly IUtentiPermessi _utentiPermessiDAL;

        public UtentiPermessiService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _utentiPermessiDAL = _serviceProvider.GetRequiredService<IUtentiPermessi>();
        }

        public async Task<bool> ReadAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, int idService, enum_Security_Attivita idActivity, int idOperation, int id = 1)
        {
            bool res = false;
            DataTable? result = null;
            try
            {
                result = await _utentiPermessiDAL.ReadAsync(objParametriUtenti, objParametriServer, idService, ((int)idActivity), idOperation, id);
                res = result != null && result.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }

            return res;
        }

        public async Task<List<Utente_Permesso>> ReadAllAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            var res = new List<Utente_Permesso>();
            DataTable? result = null;
            try
            {
                result = await _utentiPermessiDAL.ReadAllAsync(objParametriUtenti, objParametriServer);

                if (result != null && result.Rows.Count > 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        res.Add(new Utente_Permesso(row.Field<int>("Id_Attivita"), row.Field<int>("Id_Operazione")));
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }

            return res;
        }
    }
}
