using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SmartTractor.DAL.ProviderMappings;

/// <summary>
/// Interface for Data Access Layer operations on the Operazione entity.
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività (FASE 3)
/// </summary>
public interface IProviderMappingPrescription
{
    /// <summary>
    /// Fetches an Operazione row by its UUID.
    /// </summary>
    /// <param name="operazioneId">The operazione UUID.</param>
    /// <param name="serverParams">Database connection parameters.</param>
    /// <returns>
    /// A <see cref="DataTable"/> with a single row when found; empty table otherwise.
    /// </returns>
    Task<DataTable> GetPrescriptionMappingAsync(
        int prescriptionTypeId,
        int providerId,
        AgronicaCoreParametriServer serverParams);
}
