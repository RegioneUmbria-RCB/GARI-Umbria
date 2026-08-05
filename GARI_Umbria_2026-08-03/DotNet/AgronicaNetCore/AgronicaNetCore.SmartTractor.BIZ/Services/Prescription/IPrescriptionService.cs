using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.BIZ.Models;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.Prescription;

/// <summary>
/// Service interface for reading activity data from the legacy RICETTE* tables and
/// pre-validating provider mappings, producing an <see cref="PrescriptionPayloadDTO"/>
/// ready for Smart Tractor payload composition.
/// Referenced in Design Specification: DS03-BLb - Lettura Dati Attività
/// </summary>
public interface IPrescriptionService
{
    /// <summary>
    /// Reads all raw prescription data for the given <paramref name="ricettaOperazioneCod"/>
    /// from the legacy RICETTE* tables, without performing any provider-mapping resolution.
    ///
    /// <list type="number">
    ///   <item><description>FASE 1 – Validates input parameters.</description></item>
    ///   <item><description>FASE 2 – Fetches RICETTE_OPERAZIONI row → LavCod (activity type).</description></item>
    ///   <item><description>FASE 3 – Fetches RICETTE_DETTAGLI and RICETTE_DESTINAZIONI in parallel.</description></item>
    ///   <item><description>FASE 4 – Partitions detail rows into machineries (Elem_Cod=1) and products (Elem_Cod IN {0,3,10,191,210}).</description></item>
    ///   <item><description>FASE 5 – Maps destination rows into <see cref="PlantInfo"/> objects.</description></item>
    /// </list>
    /// </summary>
    /// <param name="ricettaOperazioneCod">
    /// GUID primary key from RICETTE_OPERAZIONI. Must not be <see cref="Guid.Empty"/>.
    /// </param>
    /// <param name="serverParams">Database connection context (includes PivaSuperUser for row filtering).</param>
    /// <returns>
    /// A <see cref="PrescriptionDataDTO"/> populated with the activity type, machinery IDs,
    /// products, and destination plots.
    /// </returns>
    /// <exception cref="Exceptions.AttivitaValidationException">
    /// When <paramref name="ricettaOperazioneCod"/> is <see cref="Guid.Empty"/>.
    /// </exception>
    /// <exception cref="Exceptions.DataConsistencyException">
    /// When no RICETTE_OPERAZIONI row is found for the given GUID.
    /// </exception>
    Task<PrescriptionDataDTO> ReadPrescriptionDataAsync(int ricettaOperazioneCod, AgronicaCoreParametriServer serverParams);
}
