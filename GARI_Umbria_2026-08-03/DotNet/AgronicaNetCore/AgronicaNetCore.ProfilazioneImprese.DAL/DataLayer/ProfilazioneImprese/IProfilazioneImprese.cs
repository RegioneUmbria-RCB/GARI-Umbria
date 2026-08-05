using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.ProfilazioneImprese.DAL.DataLayer.ProfilazioneImprese;

public interface IProfilazioneImprese
{
    public Task<DataTable> LeggiProfilazioneImpreseAsync(string piva,
        int idProfiloDati,
        string codiceChiave,
        string idGruppo,
        string filtroAggiuntivo,
        int lavCod,
        int vegCod,
        bool leggiSoloNoteConLavorazioneNullSeLavCodZero,
        AgronicaCoreParametriServer objParametriServer
    );

    public Task<bool> ScriviAsync(string piva, int idProfiloDati, string codiceChiave, string quesito, string idGruppo,
        string valoreSalvato, DateTime validitaInizio, DateTime validitaFine, int lavCod, int vegCod,
        AgronicaCoreParametriServer objParametriServer);

    public Task<bool> ModificaAsync(string quesito, string valoreSalvato, string piva, int idProfiloDati, string codiceChiave,
        string idGruppo, DateTime validitaInizio, DateTime validitaFine, int lavCod, int vegCod,
        AgronicaCoreParametriServer objParametriServer);

    public Task<bool> CancellaAsync(string piva, string codiceChiave, string idGruppo, int lavCod, int vegCod,
        AgronicaCoreParametriServer objParametriServer);

    public Task<bool> CancellaTutteLavorazioniAsync(string piva, string idGruppo, int vegCod, AgronicaCoreParametriServer objParametriServer);
}