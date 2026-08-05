using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.SmartTractor.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SmartTractor.DAL.DataLayer;

/// <summary>
/// Base DAL class for Smart Tractor operations.
/// </summary>
public class BaseDALSmartTractor : DAL_Base
{
    protected readonly IStringLocalizer<Messages> _localizer;

    public BaseDALSmartTractor(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
    {
        _localizer = localizer;
    }
}