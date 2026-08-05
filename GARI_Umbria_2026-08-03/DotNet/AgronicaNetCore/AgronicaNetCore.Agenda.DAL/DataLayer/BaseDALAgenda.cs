
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Agenda.DAL.Resources;

namespace AgronicaNetCore.Agenda.DAL.DataLayer
{
    public class BaseDALAgenda : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALAgenda(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
