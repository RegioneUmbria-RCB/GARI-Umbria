using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.OperazioniColturali.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.OperazioniColturali.DAL.DataLayer;

/// <summary>
/// Base DAL class for Smart Tractor operations.
/// </summary>
public class BaseDALOperazioniColturali : DAL_Base
{
    protected readonly IStringLocalizer<Messages> _localizer;

    public BaseDALOperazioniColturali(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
    {
        _localizer = localizer;
    }
}