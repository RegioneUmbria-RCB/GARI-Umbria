using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.BIZ.Models;
using AgronicaNetCore.SmartTractor.BIZ.Resources;
using AgronicaNetCore.Utenti.BIZ.Services.UtentiPermessi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.Permission;

/// <summary>
/// Service for verifying Smart Tractor permissions with caching.
/// Implements DS01-BL: Verifica Permessi Utente Smart Tractor con Cache.
/// </summary>
public class SmartTractorPermissionService : BaseServiceSmartTractorBIZ, ISmartTractorPermissionService
{
    private readonly ILogger<SmartTractorPermissionService> _logger;
    private readonly IUtentiPermessiService _utentiPermessiSevice;

    // Throttling: track last request time per user
    private readonly Dictionary<string, DateTime> _lastRequestTimes = new();
    private readonly TimeSpan _throttleInterval = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// Initializes a new instance of the <see cref="SmartTractorPermissionService"/> class.
    /// </summary>
    /// <param name="userAuthService">The GIAS permission service.</param>
    /// <param name="cache">The permission cache.</param>
    public SmartTractorPermissionService(
        ILogger<SmartTractorPermissionService> logger,
        IServiceProvider provider,
        IStringLocalizer<Messages> localizer) : base(provider, localizer)
    {
        _logger = logger;
        _utentiPermessiSevice = _serviceProvider.GetRequiredService<IUtentiPermessiService>();
    }

    /// <summary>
    /// Verifies if a user has permission for a Smart Tractor operation.
    /// </summary>
    /// <param name="request">The permission request.</param>
    /// <returns>The permission response.</returns>
    public async Task<SmartTractorPermissionResponse> VerifySendPermissionAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, bool forceRefresh)
    {
        if (objParametriUtenti == null) throw new ArgumentNullException(nameof(objParametriUtenti));
        if (objParametriServer == null) throw new ArgumentNullException(nameof(objParametriServer));
        if (string.IsNullOrWhiteSpace(objParametriUtenti.UtenteUsername)) throw new InvalidUserException("User ID cannot be null or empty.");

        // Update last request time
        _lastRequestTimes[objParametriUtenti.UtenteUsername] = DateTime.UtcNow;

        try
        {
            var hasPermission = await _utentiPermessiSevice.ReadAsync(objParametriUtenti, objParametriServer, 5, enum_Security_Attivita.SmartTractors_InvioRicette, 2);
            var permissionType = hasPermission ? "SMARTTRACTOR_SEND_PERMISSION" : null;

            var cacheData = new SmartTractorPermissionCacheData
            {
                HasPermission = hasPermission,
                CachedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            return CreateResponse(objParametriUtenti.UtenteUsername, cacheData, false);
        }
        catch (GiasConnectionException ex)
        {
            throw;
        }
    }

    private SmartTractorPermissionResponse CreateResponse(string userId, SmartTractorPermissionCacheData data, bool cacheHit)
    {
        return new SmartTractorPermissionResponse
        {
            Authorized = data.HasPermission,
            PermissionType = data.PermissionType,
            CacheHit = cacheHit,
            CachedAt = data.CachedAt.ToString("O"), // ISO 8601
            ExpiresAt = data.ExpiresAt.ToString("O"),
            UserId = userId
        };
    }
}

public class SmartTractorPermissionCacheData
{
    public bool HasPermission { get; set; }
    public string PermissionType { get; set; }
    public DateTime CachedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Exception thrown when GIAS connection fails.
/// </summary>
public class GiasConnectionException : Exception
{
    public GiasConnectionException(string message) : base(message) { }
    public GiasConnectionException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Exception thrown when cache is expired and GIAS is unavailable.
/// </summary>
public class PermissionCacheExpiredException : Exception
{
    public PermissionCacheExpiredException(string message) : base(message) { }
    public PermissionCacheExpiredException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Exception thrown for invalid user.
/// </summary>
public class InvalidUserException : Exception
{
    public InvalidUserException(string message) : base(message) { }
}