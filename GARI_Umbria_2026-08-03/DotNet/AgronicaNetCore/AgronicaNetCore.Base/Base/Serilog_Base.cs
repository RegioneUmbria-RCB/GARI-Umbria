using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.Base.Base
{
    public class Serilog_Base
    {
        protected readonly IServiceProvider _serviceProvider;
        private ILoggingService _loggingService;

        public Serilog_Base(IServiceProvider provider)
        {
            _serviceProvider = provider;
            _loggingService = provider.GetRequiredService<ILoggingService>();
        }

        protected void LogTrace(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars)
        {
            _loggingService.LogTrace(msg, objParametri, ex, pars);
        }
        protected void LogDebug(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars)
        {
            _loggingService.LogDebug(msg, objParametri, ex, pars);
        }
        protected void LogInformation(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars)
        {
            _loggingService.LogInformation(msg, objParametri, ex, pars);
        }
        protected void LogWarning(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars)
        {
            _loggingService.LogWarning(msg, objParametri, ex, pars);
        }
        protected void LogError(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars)
        {
            _loggingService.LogError(msg, objParametri, ex, pars);
        }
        protected void LogCritical(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars)
        {
            _loggingService.LogCritical(msg, objParametri, ex, pars);
        }
        protected string SanitizeLogMessage(string msg)
        {
            return _loggingService.SanitizeLogMessage(msg);
        }
    }
}
