using AgronicaNetCore.Base.Models;
using OutData.Zoo.DataMars;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.PrevenzioneDuplicati;

/// <summary>
/// Verifica che un'operazione di pesatura raggruppata per data non esista già in agenda.
/// <para>Riferimento spec: DS06-BL PrevenzioneDuplicatiOperazioniAgenda; DS02-BL Step 2c.</para>
/// </summary>
public interface IPrevenzioneDuplicatiOperazioniAgendaService
{
    /// <summary>
    /// Esegue il controllo duplicati per un gruppo di pesate della stessa data:
    /// verifica se esiste già un'operazione di tipo PESATURA per la stessa data con almeno
    /// uno degli stessi animali (match su Cod_Progetto IN lista o LidMatricola IN lista).
    /// <para>Riferimento spec: DS02-BL Step 2c — Prevenzione Duplicati Per Data.</para>
    /// </summary>
    Task<DuplicatoCheckResult> CheckDuplicatoPerDataAsync(
        string piva,
        int saCod,
        int staNum,
        IReadOnlyList<int> codProgettiAnimali,
        DateTime dataPesata,
        AgronicaCoreParametriServer objParametriServer);
}
