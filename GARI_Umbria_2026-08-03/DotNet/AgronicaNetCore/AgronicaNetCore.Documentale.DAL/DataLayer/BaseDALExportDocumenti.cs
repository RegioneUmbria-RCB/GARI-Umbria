using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Documentale.DAL.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Documentale.DAL.DataLayer
{
    public class BaseDALExportDocumenti : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALExportDocumenti(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
