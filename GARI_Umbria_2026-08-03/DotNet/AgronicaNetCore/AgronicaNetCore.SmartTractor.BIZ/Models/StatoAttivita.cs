namespace AgronicaNetCore.SmartTractor.BIZ.Models;

/// <summary>
/// Represents the lifecycle state of an activity.
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività
/// </summary>
public enum StatoAttivita
{
    /// <summary>Activity is scheduled and waiting to start.</summary>
    Pending,

    /// <summary>Activity is currently in progress.</summary>
    InProgress,

    /// <summary>Activity has been completed successfully.</summary>
    Completed,

    /// <summary>Activity has been cancelled.</summary>
    Cancelled
}
