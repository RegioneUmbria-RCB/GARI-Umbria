using System.Data;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Note.DAL.DataLayer.NoteInterventoUtilizzo;

public interface INote
{
    public Task<DataTable> LeggiNoteInterventoUtilizzoAsync(int notaUtilizzoCod, AgronicaCoreParametriServer objParametriServer);

    public Task<DataTable> LeggiProfilazioneNoteAsync(int notaUtilizzoCod, int notaGruppoCod, bool soloNoteConUtilizzo,
        Visibilita visibilita, string filtroAggiuntivo, AgronicaCoreParametriServer objParametriServer);

    public Task<string> LeggiNotaDesFromNotaCodAsync(int notaCod, AgronicaCoreParametriServer objParametriServer);
    public Task<string> LeggiNotaUtilizzoDesFromNotaUtilizzoCodAsync(int i, AgronicaCoreParametriServer objParametriServer);

    public Task<DataTable> LeggiGruppoNoteAsync(int notaGruppoCod, string filtroAggiuntivo, string orderBy,
        AgronicaCoreParametriServer objParametriServer);

    public Task<bool> SalvaGruppoNoteAsync(int notaGruppoCod, string notaGruppoDes, DateTime validitaInizio, DateTime validitaFine,
        AgronicaCoreParametriServer objParametriServer);

    public Task<int> NoteInterventoGruppiNewIdAsync(AgronicaCoreParametriServer objParametriServer);

    public Task<bool> AggiornaGruppoNoteAsync(int bodyNotaGruppoCod, string bodyNotaGruppoDes, DateTime finestraTemporaleInizio,
        DateTime finestraTemporaleFine, bool visibile, AgronicaCoreParametriServer objParametriServer);

    public Task<bool> CancellaGruppoNoteAsync(int gruppoNoteCod, AgronicaCoreParametriServer objParametriServer);

    public Task<bool> SalvaNotaAsync(int notaCod, string notaDes, int notaGruppoCod, DateTime validitaInizio,
        DateTime validitaFine, AgronicaCoreParametriServer objParametriServer);

    public Task<int> NoteNewIdAsync(AgronicaCoreParametriServer objParametriServer);

    public Task<DataTable> LeggiNoteInterventoUtilizzoGruppiAsync(int notaGruppoCod, int notaUtilizzoCod,
        AgronicaCoreParametriServer objParametriServer);

    public Task<bool> CancellaGruppoNoteUtilizzoAsync(int notaGruppoCod, int notaUtilizzoCod, AgronicaCoreParametriServer objParametriServer);

    public Task<bool> ScriviNoteUtilizzoAsync(int notaGruppoCod, int utilizzoCod, DateTime dtInizio, DateTime dtFine,
        AgronicaCoreParametriServer objParametriServer);

    public Task<bool> ModificaVisibilitaGruppoAsync(int notaGruppoCod, bool visible, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> ModificaVisibilitaNotaAsync(int notaCod, bool visible, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> CancellaNotaAsync(int notaCod, AgronicaCoreParametriServer objParametriServer);
}