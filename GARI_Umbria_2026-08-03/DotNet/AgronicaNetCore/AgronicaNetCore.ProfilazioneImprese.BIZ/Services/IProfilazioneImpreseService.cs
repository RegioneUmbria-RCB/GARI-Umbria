using System.Data;
using AgronicaNetCore.Base.Models;
using InData.Note;
using InData.ProfilazioneImprese;

namespace AgronicaNetCore.ProfilazioneImprese.BIZ.Services;

public interface IProfilazioneImpreseService
{
    public Task<DataTable> LeggiAsync(string piva, int idProfiloDati, string codiceChiave, string idGruppo, int lavCod,
        int vegCod, bool leggiSoloNoteConLavorazioneNullSeLavCodZero, AgronicaCoreParametriServer objParametriServer);

    public Task<bool> SalvaProfilazioneAsync(SalvaProfilazione_In body, AgronicaCoreParametriServer objParametriServer);
    
    public Task<bool> SalvaNoteAsync(SalvaNote_In body, AgronicaCoreParametriServer objParametriServer);

    public Task<bool> ScriviInserisceOAggiornaAsync(string piva, string codiceChiave, string idGruppo, string quesito,
        string valoreSalvato, DateTime validitaInizio, DateTime validitaFine, int lavCod, int vegCod,
        AgronicaCoreParametriServer objParametriServer);

    public Task<DataTable> LeggiProfilazioneMacchineXContattiAsync(string piva, int vegCod, AgronicaCoreParametriServer objParametriServer);

    public Task<bool> CancellaAsync(string piva, int notaUtilizzoCod, string idGruppo, int lavCod, int vegCod,
        AgronicaCoreParametriServer objParametriServer);

    public Task<bool> PropagaSuTutteLeOperazioniAsync(string piva, int lavCod, int vegCod, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> AggiornaOreMinutiAsync(AggiornaOreMinuti_In body, AgronicaCoreParametriServer objParametriServer);
}