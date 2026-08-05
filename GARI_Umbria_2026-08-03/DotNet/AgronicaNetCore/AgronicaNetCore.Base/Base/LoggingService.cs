using AgronicaNetCore.Base.DataLayer.ConfigurazioneElasticSearch;
using AgronicaNetCore.Base.DataLayer.LastCUAA;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace AgronicaNetCore.Base.Base
{
    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;
        protected readonly IServiceProvider _serviceProvider;
        private ILastCUAA _lastCuaaDal;
        private IConfigurazioneElasticSearchDAL _configurazioneElasticSearchDal;
        private readonly IConfiguration _configuration;


        public LoggingService(IServiceProvider provider)
        {
            _serviceProvider = provider;
            _logger = provider.GetRequiredService<ILogger<LoggingService>>();
            _configuration = provider.GetRequiredService<IConfiguration>();
        }

        public void LogTrace(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars)
        {
            using (PushPropertiesInLogContextAndFormatMessage(objParametri, LogLevel.Trace, ex, ref msg))
            {
                WriteLog(LogLevel.Trace, ex, msg, pars);
            }

        }
        public void LogDebug(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars)
        {
            using (PushPropertiesInLogContextAndFormatMessage(objParametri, LogLevel.Debug, ex, ref msg))
            {
                WriteLog(LogLevel.Debug, ex, msg, pars);
            }
        }
        public void LogInformation(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars)
        {
            using (PushPropertiesInLogContextAndFormatMessage(objParametri, LogLevel.Information, ex, ref msg))
            {
                WriteLog(LogLevel.Information, ex, msg, pars);
            }
        }
        public void LogWarning(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars)
        {
            using (PushPropertiesInLogContextAndFormatMessage(objParametri, LogLevel.Warning, ex, ref msg))
            {
                WriteLog(LogLevel.Warning, ex, msg, pars);
            }
        }
        public void LogError(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars)
        {
            using (PushPropertiesInLogContextAndFormatMessage(objParametri, LogLevel.Error, ex, ref msg))
            {
                WriteLog(LogLevel.Error, ex, msg, pars);
            }
        }
        public void LogCritical(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars)
        {
            using (PushPropertiesInLogContextAndFormatMessage(objParametri, LogLevel.Critical, ex, ref msg))
            {
                WriteLog(LogLevel.Critical, ex, msg, pars);
            }
        }

        public string SanitizeLogMessage(string msg)
        {
            return msg.Replace("\n", " ").Replace("\r", " ");
        }

        private CompositeDisposable PushPropertiesInLogContextAndFormatMessage(AgronicaCoreParametri? objParametri, LogLevel logLevel, Exception? ex, ref string message)
        {

            string applicationName = _configuration.GetValue<string>("ApplicationName") ?? "NetCore6";

            var keyContext = new KeyValuePair<string, object>();

            var logFilePath = string.Empty;

            if (objParametri == null)
                logFilePath = _configuration["Serilog:WriteTo:1:Args:configureLogger:WriteTo:0:Args:path"] ?? "";
            else
                logFilePath = Path.Combine(objParametri.LogDirectory, applicationName, Path.GetFileNameWithoutExtension(objParametri.LogFileName));

            keyContext = new KeyValuePair<string, object>("LogPath", logFilePath);

            var directory = Path.GetDirectoryName(logFilePath);
            Directory.CreateDirectory(directory);

            var disposables = new List<IDisposable>();

            if (objParametri is not null)
            {
                if (objParametri is AgronicaCoreParametriServer objParametriServer)
                {
                    string cuaa = string.Empty;
                    ConfigurazioneElasticSearch configurazioneElasticSearch = null;
                    try
                    {
                        if (_lastCuaaDal is null)
                        {
                            _lastCuaaDal = _serviceProvider.GetRequiredService<ILastCUAA>();
                        }
                        cuaa = _lastCuaaDal.ReadCuaaForLog(objParametriServer);

                        if (_configurazioneElasticSearchDal is null)
                        {
                            _configurazioneElasticSearchDal = _serviceProvider.GetRequiredService<IConfigurazioneElasticSearchDAL>();
                        }
                        configurazioneElasticSearch = _configurazioneElasticSearchDal.LeggiConfigurazione(objParametriServer);
                    }
                    catch (Exception)
                    {
                        // non faccio nulla con l'eventuale errore, mi serve l'informazione sull'exception principale
                    }

                    var cuaaAndDescription = string.IsNullOrWhiteSpace(cuaa) ? "" : $"CUAA {cuaa}";
                    message = $"{cuaaAndDescription} : {message}";

                    disposables.Add(LogContext.PushProperty("LogWithElasticSearch", true));
                    disposables.Add(LogContext.PushProperty("CUAA", cuaa));
                    disposables.Add(LogContext.PushProperty("message", message));
                    disposables.Add(LogContext.PushProperty("dateTime", DateTime.Now));

                    string logSeverity = string.Empty;
                    if (logLevel == LogLevel.Information)
                        logSeverity = "Informazione";
                    else if (logLevel == LogLevel.Warning)
                        logSeverity = "Warning";
                    else if (logLevel == LogLevel.Error)
                        logSeverity = "Errore";
                    else if (logLevel == LogLevel.Critical)
                        logSeverity = "Critical";

                    disposables.Add(LogContext.PushProperty("logSeverity", logSeverity));

                    if (configurazioneElasticSearch is not null)
                    {
                        disposables.Add(LogContext.PushProperty("ElasticSearchUrl", configurazioneElasticSearch.ElasticSearchUrl));
                        disposables.Add(LogContext.PushProperty("ElasticSearchAmbiente", configurazioneElasticSearch.Parametri.Ambiente));
                    }

                    if (ex is not null)
                    {
                        disposables.Add(LogContext.PushProperty("ExceptionStackTrace", ex.StackTrace));
                    }
                }

                if (!string.IsNullOrWhiteSpace(objParametri?.UtenteUsername))
                {
                    message = $"{{{objParametri.UtenteUsername}}} {message}";
                }
            }

            disposables.Add(LogContext.PushProperty(keyContext.Key, keyContext.Value));
            disposables.Add(LogContext.PushProperty("Username", "General"));

            return new CompositeDisposable(disposables);
        }

        private void WriteLog(LogLevel level, Exception? ex, string message, params object[] pars)
        {
            if (ex == null)
                _logger.Log(level, message, pars);
            else
                _logger.Log(level, ex, message, pars);
        }
    }
}
