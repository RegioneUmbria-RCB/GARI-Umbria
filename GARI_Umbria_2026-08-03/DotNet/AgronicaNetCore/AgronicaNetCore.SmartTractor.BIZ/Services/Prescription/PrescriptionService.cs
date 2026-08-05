using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.BIZ.Exceptions;
using AgronicaNetCore.SmartTractor.BIZ.Models;
using AgronicaNetCore.SmartTractor.BIZ.Resources;
using AgronicaNetCore.SmartTractor.DAL.Attivita;
using AgronicaNetCore.SmartTractor.DAL.ProviderMappings;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.Prescription;

/// <summary>
/// Reads activity data from the legacy RICETTE* tables and pre-validates provider
/// mappings, returning a fully populated <see cref="PrescriptionPayloadDTO"/> ready for
/// Smart Tractor payload composition.
///
/// Replicates the behaviour of the VB.NET function
/// <c>LeggiListaAttivitaDaRicettaOperazionePerRicetta()</c>.
///
/// Referenced in Design Specification: DS03-BLb - Lettura Dati AttivitÃ 
/// </summary>
public class PrescriptionService : BaseServiceSmartTractorBIZ, IPrescriptionService
{
    private readonly IPrescription _attivitaRepository;
    private readonly IProviderMappingPlant _providerMappingPlant;
    private readonly IProviderMappingProduct _providerMappingProduct;

    public PrescriptionService(
        IServiceProvider provider,
        IStringLocalizer<Messages> localizer,
        IPrescription attivitaRepository,
        IProviderMappingPlant providerMappingPlant,
        IProviderMappingProduct providerMappingProduct)
        : base(provider, localizer)
    {
        _attivitaRepository = attivitaRepository ?? throw new ArgumentNullException(nameof(attivitaRepository));
        _providerMappingPlant = providerMappingPlant ?? throw new ArgumentNullException(nameof(providerMappingPlant));
        _providerMappingProduct = providerMappingProduct ?? throw new ArgumentNullException(nameof(providerMappingProduct));
    }

    /// <inheritdoc/>
    public async Task<PrescriptionDataDTO> ReadPrescriptionDataAsync(
        int ricettaOperazioneId,
        AgronicaCoreParametriServer serverParams)
    {
        // FASE 1 — Input validation
        if (ricettaOperazioneId == 0)
            throw new AttivitaValidationException("ricettaOperazioneCod non fornito");

        // FASE 2 — Fetch RICETTE_OPERAZIONI row to obtain the activity type (Lav_Cod)
        var operazioneTable = await _attivitaRepository.GetPrescriptionTypeAsync(ricettaOperazioneId, serverParams);
        if (operazioneTable.Rows.Count == 0)
            throw new DataConsistencyException($"Ricetta_Operazione non trovata per Cod={ricettaOperazioneId}");

        var lavCod = Convert.ToInt32(operazioneTable.Rows[0]["Lav_Cod"]);
        var startdate = Convert.ToDateTime(operazioneTable.Rows[0]["Validita_Inizio"]);
        var endDate = Convert.ToDateTime(operazioneTable.Rows[0]["Validita_Fine"]);

        // FASE 3 — Fetch RICETTE_DETTAGLI (all rows) and RICETTE_DESTINAZIONI in parallel
        var dettagliTask     = _attivitaRepository.GetPrescriptionDetailsAsync(ricettaOperazioneId, null, serverParams);
        var destinazioniTask = _attivitaRepository.GetPrescriptionDestinationsAsync(ricettaOperazioneId, null, serverParams);

        await Task.WhenAll(dettagliTask, destinazioniTask);

        var dettagliTable     = dettagliTask.Result;
        var destinazioniTable = destinazioniTask.Result;

        // FASE 4 — Partition RICETTE_DETTAGLI rows into machineries and products
        //   • Machineries : Elem_Cod = 1             → code is always in Mat_Cod
        //   • Products    : Elem_Cod IN {0,3,10,191,210} → code is in Pro_Cod when not null,
        //                                                   otherwise in Mat_Cod
        var machineryIds         = new List<int>();
        var prodotti             = new List<ProductDetails>();
        var productElemCodValues = new HashSet<int> { 0, 3, 10, 191, 210 };

        foreach (DataRow row in dettagliTable.Rows)
        {
            if (row["Elem_Cod"] == DBNull.Value) continue;

            var elemCod = Convert.ToInt32(row["Elem_Cod"]);

            if (elemCod == 1)
            {
                if (row["Mat_Cod"] != DBNull.Value)
                    machineryIds.Add(Convert.ToInt32(row["Mat_Cod"]));
            }
            else if (productElemCodValues.Contains(elemCod))
            {
                // Prefer Pro_Cod; fall back to Mat_Cod
                int productCode = row["Pro_Cod"] != DBNull.Value
                    ? Convert.ToInt32(row["Pro_Cod"])
                    : row["Mat_Cod"] != DBNull.Value
                        ? Convert.ToInt32(row["Mat_Cod"])
                        : 0;

                if (productCode != 0)
                {
                    prodotti.Add(new ProductDetails
                    {
                        Id       = productCode,
                        ElemCod = elemCod,
                        Qty = row["Qta"] != DBNull.Value ? Convert.ToSingle(row["Qta"]) : 0f
                    });
                }
            }
        }

        // FASE 5 — Map RICETTE_DESTINAZIONI rows into PlantInfo objects
        var destinazioni = new List<PlantId>(destinazioniTable.Rows.Count);
        foreach (DataRow row in destinazioniTable.Rows)
        {
            var piva = row.Field<string?>("Piva") ?? string.Empty;
            var saCod = Convert.ToInt32(row["Sa_Cod"]);
            var appezza = Convert.ToInt32(row["Appezza"]);
            var idReg = Convert.ToInt32(row["Id_Reg"]);

            if (!String.IsNullOrEmpty(piva.Trim()) && saCod != 0 && appezza != 0 && idReg != 0)
            {
                destinazioni.Add(new PlantId
                {
                    Piva = piva,
                    SaCod = saCod,
                    Appezza = appezza,
                    IdReg = idReg
                });
            }
        }

        return new PrescriptionDataDTO
        {
            RicettaOperazioneCod = ricettaOperazioneId,
            LavCod               = lavCod,
            MachineryIds         = machineryIds,
            Prodotti             = prodotti,
            Destinazioni         = destinazioni,
            StartDate            = startdate,
            EndDate              = endDate
        };
    }
}
