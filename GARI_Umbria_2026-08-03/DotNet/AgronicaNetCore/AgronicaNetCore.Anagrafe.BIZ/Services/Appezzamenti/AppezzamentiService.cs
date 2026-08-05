using AgronicaCoreModelsSTD.metaschema;
using AgronicaDataProvider6.Utils;
using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Apezzamenti;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impianti;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Data;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Appezzamenti
{
    public class AppezzamentiService : BaseServiceAnagrafeBIZ, IAppezzamentiService
    {
        private readonly IAppezzamenti _appezzamenti;
        private readonly TempChiaviMassivo _tempChiaviMassivo;
        private readonly ILoggingService _loggingService;

        public AppezzamentiService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _appezzamenti = _serviceProvider.GetRequiredService<IAppezzamenti>();
            _tempChiaviMassivo = _serviceProvider.GetRequiredService<TempChiaviMassivo>();
            _loggingService = provider.GetRequiredService<ILoggingService>();
        }

        public async Task<List<(string, List<Comune>)>> GetAppezzamentixParticelleAsync(
            List<(string, int, int)> chiaviAppezzamento,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            List<(string, List<Comune>)> result = new();

            try
            {
                // await OpenConnectionAsync(objParametriServer, true, IsolationLevel.ReadUncommitted);

                await _tempChiaviMassivo.CreaTabellaTemp_FiltroAppezzamenti(
                    chiaviAppezzamento,
                    objParametriServer
                );

                DataTable dt = await _appezzamenti.GetAppezzamentixParticelleAsync(
                    true,
                    objParametriServer,
                    soloChiavi: true
                );

                if (dt == null || dt.Rows.Count == 0)
                {
                    await _tempChiaviMassivo.EliminaTabellaTemp_FiltroAppezzamenti(
                        objParametriServer
                    );
                    //    CloseTransaction(objParametriServer);
                    return new();
                }

                List<(string, int, int)> listAppxParticelle =
                    dt.AsEnumerable()
                        .Select(r =>
                            (
                                r.Field<string>("piva") ?? "",
                                r.Field<int>("sa_Cod"),
                                r.Field<int>("appezza")
                            )
                        )
                        .Distinct()
                        .ToList()
                    ?? new();

                foreach (var app in listAppxParticelle)
                {
                    _loggingService.LogInformation(
                        $"Recupero i dati per l'app {app.Item1}-{app.Item2}-{app.Item3}",objParametriServer
                    );

                    var chiave = $"{app.Item1}_{app.Item2}_{app.Item3}";
                    var comuni =
                        dt.AsEnumerable()
                            .Where(r =>
                                (r.Field<string>("piva") ?? "") == app.Item1
                                && r.Field<int>("sa_Cod") == app.Item2
                                && r.Field<int>("appezza") == app.Item3
                            )
                            .Select(r => new Comune
                            {
                                codice = r.Field<string>("com"),
                                provincia = new() { codice = r.Field<string>("prov") },
                            })
                            .Distinct()
                            .ToList()
                        ?? new();
                    result.Add((chiave, comuni));
                }

                await _tempChiaviMassivo.EliminaTabellaTemp_FiltroAppezzamenti(objParametriServer);

                //      CloseTransaction(objParametriServer);
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex.Message, objParametriServer, ex);
                await _tempChiaviMassivo.EliminaTabellaTemp_FiltroAppezzamenti(objParametriServer);
                //            CloseTransaction(objParametriServer, Rollback: true);
            }
            finally
            {
                //         CloseConnection(objParametriServer);
            }

            return result;
        }
    }
}
