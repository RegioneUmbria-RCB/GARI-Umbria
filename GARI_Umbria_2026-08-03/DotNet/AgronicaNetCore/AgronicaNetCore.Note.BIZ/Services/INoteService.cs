using System.Data;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using InData.Note;

namespace AgronicaNetCore.Note.BIZ.Services;

public interface INoteService
{
    public Task<DataTable> LeggiNoteInterventoUtilizzoAsync(int notaUtilizzoCod, AgronicaCoreParametriServer objParametriServer);

    public Task<DataTable> LeggiProfilazioneNoteAsync(int notaUtilizzoCod, int notaGruppoCod, bool soloNoteConUtilizzo,
        string filtroAggiuntivo, AgronicaCoreParametriServer objParametriServer);

    public Task<DataTable> LeggiProfilazioneNoteAsync(string piva, string codiceChiave, int lavCod, int vegCod,
        bool filtraVegCodAncheSeZero, bool leggiSoloNoteConLavorazioneNullSeLavCodZero,
        AgronicaCoreParametriServer objParametriServer);

    public Task<DataTable> LeggiNoteAsync(string piva, int inputLavCod, int inputVegCod, AgronicaCoreParametriServer objParametriServer);
    public Task<DataTable> LeggiGruppoNoteAsync(AgronicaCoreParametriServer objParametriServer);
    public Task<bool> SalvaGruppoNoteAsync(SalvaGruppoNote_In body, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> AggiornaGruppoNoteAsync(SalvaGruppoNote_In body, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> CancellaGruppoNoteAsync(int gruppoNoteCod, AgronicaCoreParametriServer objParametriServer);

    public Task<DataTable> LeggiNoteXUtilizzoAsync(int notaUtilizzoCod, int notaGruppoCod, AgronicaCoreParametriServer objParametriServer);

    public Task<bool> AggiungiNotaAsync(string notaDes, int notaGruppoCod, AgronicaCoreParametriServer objParametriServer);

    public Task<DataTable> LeggiNoteInterventoUtilizzoGruppiAsync(int notaGruppoCod, int notaUtilizzoCod,
        AgronicaCoreParametriServer objParametriServer);

    public Task<bool> SalvaGruppoNoteCompletoAsync(SalvaGruppoNoteCompleto_In body, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> CancellaNotaAsync(int notaCod, AgronicaCoreParametriServer objParametriServer);
}