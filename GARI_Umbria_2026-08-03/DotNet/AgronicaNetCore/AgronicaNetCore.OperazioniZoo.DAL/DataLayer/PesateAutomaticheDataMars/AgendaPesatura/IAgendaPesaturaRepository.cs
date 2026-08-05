using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer.PesateAutomaticheDataMars.AgendaPesatura;

/// <summary>
/// Accesso ai dati per le operazioni di pesatura su Agenda/Movimenti/Movimenti_Dettagli/Mov_Destinazioni.
/// <para>Riferimento spec: DS02-BL TrasformazionePesateStagingAgenda — Step 3 Prevenzione Duplicati
/// e Step 4 Creazione Operazione Agenda; DS06-BL PrevenzioneDuplicatiOperazioniAgenda.</para>
/// </summary>
public interface IAgendaPesaturaRepository
{
    /// <summary>
    /// Verifica quante operazioni di pesatura esistono già per la stessa data con almeno uno
    /// degli animali indicati (match su Cod_Progetto IN lista e CAST(Data_Movimento AS DATE)).
    /// <para>Riferimento spec: DS02-BL Step 2c — Prevenzione Duplicati Per Data; DS06-BL.</para>
    /// </summary>
    Task<int> CheckDuplicatoPesaturaPerDataAsync(
        string piva,
        int saCod,
        int staNum,
        IReadOnlyList<int> codProgettiAnimali,
        DateTime dataPesata,
        AgronicaCoreParametriServer objParametriServer);
}
