
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Webhook.BIZ.Authentication
{
    public interface IFmisContextFactory
    {
        FmisContextDataBase? CreateFmisContextData(string fmisContextHeader);
    }
}
