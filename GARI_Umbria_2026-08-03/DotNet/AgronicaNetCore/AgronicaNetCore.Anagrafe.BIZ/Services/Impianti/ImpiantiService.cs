using AgronicaCoreModelsSTD.metaschema;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impianti;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Impianti
{
    public class ImpiantiService : BaseServiceAnagrafeBIZ, IImpiantiService
    {
        private readonly IImpianti _impianti;
        private readonly ILoggingService _loggingService;

        public ImpiantiService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _impianti = _serviceProvider.GetRequiredService<IImpianti>();
            _loggingService = provider.GetRequiredService<ILoggingService>();
        }

        public async Task<List<Contribute>?> GetContributiACAAsync(AgronicaCoreParametriServer objParametriServer)
        {
            List<Contribute> contributi = new();

            try
            {
                DataSet ds = await _impianti.GetExistsContributiACAAsync(objParametriServer);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds.Tables[0].Rows[0]["esistenza"].ToString() != "0")
                        {
                            if (ds.Tables[1].Rows.Count > 0)
                            {
                                foreach (DataRow row in ds.Tables[1].Rows)
                                    contributi.Add(new Contribute()
                                    {
                                        type = 1, //Fisso ACA
                                        description = row["ContributoDes"].ToString(),
                                        code = (int)row["ContributoCod"]
                                    });
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex.Message, objParametriServer, ex);
                contributi = null;
            }

            return contributi;
        }

        public async Task<DataTable> LeggiColturexPivaAsync(IEnumerable<string> pivas, AgronicaCoreParametriServer objParametriServer)
            => await _impianti.LeggiColturexPivaAsync(pivas, objParametriServer);
    }
}
