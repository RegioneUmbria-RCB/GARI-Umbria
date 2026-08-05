namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Response model for Smart Tractor permission verification.
/// </summary>
public class SmartTractorPermissionResponse
{
    /// <summary>
    /// Whether the user is authorized.
    /// </summary>
    public bool Authorized { get; set; }

    /// <summary>
    /// The permission type.
    /// </summary>
    public string? PermissionType { get; set; }

    /// <summary>
    /// Whether the result came from cache.
    /// </summary>
    public bool CacheHit { get; set; }

    /// <summary>
    /// When the permission was cached (ISO 8601).
    /// </summary>
    public string? CachedAt { get; set; }

    /// <summary>
    /// When the cache expires (ISO 8601).
    /// </summary>
    public string? ExpiresAt { get; set; }

    /// <summary>
    /// The user ID.
    /// </summary>
    public string UserId { get; set; }
}