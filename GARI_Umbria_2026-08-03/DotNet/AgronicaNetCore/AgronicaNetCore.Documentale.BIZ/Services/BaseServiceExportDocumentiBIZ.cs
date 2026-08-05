using AgronicaNetCore.Documentale.BIZ.Resources;
using AgronicaNetCore.Base.Base;
using Microsoft.Extensions.Localization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Documentale.BIZ.Services
{
    public class BaseServiceExportDocumentiBIZ  :BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceExportDocumentiBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }

    
}
