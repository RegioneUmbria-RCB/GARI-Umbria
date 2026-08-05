using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impresa;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Data;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Imprese
{
    public class ImpresaService : BaseServiceAnagrafeBIZ, IImpresaService
    {
        private readonly ILogger<ImpresaService> _logger;
        private readonly IUtentiVisibilitaAppoggio _utentiVisibilitaAppoggio;

        public ImpresaService(ILogger<ImpresaService> logger, IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { 
            _logger = logger;
            _utentiVisibilitaAppoggio = _serviceProvider.GetRequiredService<IUtentiVisibilitaAppoggio>();
        }

        public async Task<DataTable> TestClausolaINAsync(List<string> elencoPiva, List<int> elencoVegCod, int varieta, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {

                var impresaDal = _serviceProvider.GetRequiredService<IImpresa>();

                var dt = await impresaDal.LeggiClausolaInAsync(elencoPiva, elencoVegCod,varieta, objParametriServer);
                using (LogContext.PushProperty("LogPath", Path.Combine(objParametriServer.LogDirectory, Path.GetFileNameWithoutExtension(objParametriServer.LogFileName))))
                {
                    _logger.LogInformation("questo è un log di prova");
                }
                using (LogContext.PushProperty("General", true))
                {
                    _logger.LogInformation("questo è un log di prova - 2");
                }
                return dt!;

            }
            catch (Exception ex)
            {
                using (LogContext.PushProperty("LogPath", Path.Combine(objParametriServer.LogDirectory, Path.GetFileNameWithoutExtension(objParametriServer.LogFileName))))
                {
                    _logger.LogError(ex, ex.Message);
                }
                //_logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<DataTable> Leggi2Async(AgronicaCoreParametriServer objParametriServer, string partiIva)
        {
            try
            {

                var impresaDal = _serviceProvider.GetRequiredService<IImpresa>();

                var dt = await impresaDal.LeggiAsync(partiIva, objParametriServer);
                using (LogContext.PushProperty("LogPath", Path.Combine(objParametriServer.LogDirectory, Path.GetFileNameWithoutExtension(objParametriServer.LogFileName))))
                {
                    _logger.LogInformation("questo è un log di prova");
                }
                using (LogContext.PushProperty("General",true))
                {
                    _logger.LogInformation("questo è un log di prova - 2");
                }
                return dt!;

            }catch(Exception ex)
            {
                using (LogContext.PushProperty("LogPath", Path.Combine(objParametriServer.LogDirectory, Path.GetFileNameWithoutExtension(objParametriServer.LogFileName))))
                {
                    _logger.LogError(ex, ex.Message);
                }
                //_logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<AgronicaCoreModelsSTD.anagrafiche.Impresa?> LeggiImpresaAsync(AgronicaCoreParametriServer objParametriServer, string piva)
        {
            try
            {

                var impresaDal = _serviceProvider.GetRequiredService<IImpresa>();

                var dt = await impresaDal.LeggiAsync(piva, objParametriServer);

                using (LogContext.PushProperty("LogPath", Path.Combine(objParametriServer.LogDirectory, Path.GetFileNameWithoutExtension(objParametriServer.LogFileName))))
                {
                    _logger.LogInformation("Questo è una log di prova GGGGG");
                }

                if (dt != null) { 

                    return new AgronicaCoreModelsSTD.anagrafiche.Impresa()
                    {
                        partitaIva = dt.Rows[0]["PIVA"].ToString(),
                        ragioneSociale = dt.Rows[0]["rag_soc"].ToString(),
                        validita = new AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale()
                        {
                            inizio = DateTime.Parse(dt.Rows[0]["Validita_Inizio"].ToString()!),
                            fine = DateTime.Parse(dt.Rows[0]["Validita_Inizio"].ToString()!)
                        }
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                using (LogContext.PushProperty("LogPath", Path.Combine(objParametriServer.LogDirectory, Path.GetFileNameWithoutExtension(objParametriServer.LogFileName))))
                {
                    _logger.LogError(ex, ex.Message);
                }
                //_logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message,ex);
            }

        }

        public async Task<DataTable> LeggiImpreseAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable result;
            try
            {
                var dtImpreseVisibili = await _utentiVisibilitaAppoggio.ReadAsync((int)Enum_TipoEntita.Impresa, objParametriServer);
                var impresaDal = _serviceProvider.GetRequiredService<IImpresa>();
                result = await impresaDal.LeggiImpreseAsync(piva, dtImpreseVisibili, objParametriServer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            return result;
        }

        public async Task<DataTable> LeggiPadriAsync(AgronicaCoreParametriServer objParametriServer)
        {
            DataTable dt;

            try
            {
                var dtImpreseVisibili = await _utentiVisibilitaAppoggio.ReadAsync((int)Enum_TipoEntita.Impresa, objParametriServer);
                var impresaDal = _serviceProvider.GetRequiredService<IImpresa>();
                dt = await impresaDal.LeggiPadriAsync(dtImpreseVisibili, objParametriServer);
            }
            catch (Exception ex)
            {
                using (LogContext.PushProperty("LogPath", Path.Combine(objParametriServer.LogDirectory, Path.GetFileNameWithoutExtension(objParametriServer.LogFileName))))
                {
                    _logger.LogError(ex, ex.Message);
                }
                throw new Exception(ex.Message, ex);
            }
            return dt;
        }
    }
}
