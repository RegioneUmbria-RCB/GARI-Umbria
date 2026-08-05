namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Response returned by <see cref="Services.Engine.IEngineService.SendPrescriptionAsync"/>
/// after a successful HTTP POST to the Smart Tractor provider API.
/// </summary>
public class SendPrescriptionResponse
{
    /// <summary>
    /// Gets or sets the provider-assigned activity identifier returned in the
    /// response body (<c>activityId</c> field).
    /// </summary>
    public long ActivityId { get; set; }
    public string status { get; set; } = SmartTractorRequestStatus.Pending;
    public string error { get; set; } = string.Empty;
}
