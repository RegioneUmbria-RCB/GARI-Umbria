using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Note.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Note.DAL.DataLayer
{
    public class BaseDALNote : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALNote(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
