using System.Data;
using AgronicaNetCore.Base.Models;
using InData.DefaultPianiColturali;
using InData.SpecieVegetali;
using OutData.DefaultPianiColturali;

namespace AgronicaNetCore.DefaultPianiColturali.BIZ.Services;

public interface IDefaultPianiColturaliService
{
    public Task<DefaultDistintaProduzione_Out> LeggiDefaultDistintaDiProduzioneAsync(string piva, int vegCod, AgronicaCoreParametriServer objParametriServer);
    public Task<DataTable> LeggiDefaultGeneraleSpecieAsync(string piva, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> ScriviSpecieVegetaleAsync(SpecieVegetali_In body, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> CancellaSpecieVegetaleAsync(string piva, int vegCod, AgronicaCoreParametriServer objParametriServer);

    public Task<bool> ScriviDefaultGeneraliInizialiAsync(DefaultGeneraliColtura_In body, AgronicaCoreParametriServer objParametriServer);

    public Task<bool> ScriviDefaultSpecieInizialiAsync(DefaultGeneraliColtura_In body, AgronicaCoreParametriServer objParametriServer);
    public Task<DataTable> LeggiDefaultGeneraliAsync(string piva, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> ScriviDefaultGeneraliAsync(DefaultGenerali_In body, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> CancellaDefaultGeneraleAsync(string piva, int vegCod, int culCod, AgronicaCoreParametriServer objParametriServer);
    public Task<bool> SalvaDefaultDistintaDiProduzioneAsync(DistintaProduzione_In body, AgronicaCoreParametriServer objParametriServer);
}