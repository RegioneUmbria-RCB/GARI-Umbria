using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Base.Base
{
    public interface ILoggingService
    {
        void LogTrace(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars);
        void LogDebug(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars);
        void LogInformation(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars);
        void LogWarning(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars);
        void LogError(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars);
        void LogCritical(string msg, AgronicaCoreParametri? objParametri = null, Exception? ex = null, params object?[] pars);
        string SanitizeLogMessage(string msg);
    }
}
