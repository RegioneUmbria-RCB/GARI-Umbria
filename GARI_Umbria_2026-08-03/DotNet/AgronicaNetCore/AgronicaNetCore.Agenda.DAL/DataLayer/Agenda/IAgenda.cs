using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.DataExchange.AntaresTrace;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace AgronicaNetCore.Agenda.DAL.DataLayer.Agenda
{
    public interface IAgenda
    {
        //Task<DataTable> LeggiAgendaHubQDC(List<int> Id_Agenda, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiAgendaAsync(string? piva, List<int>? idAgendas, int? sacod, List<int>? lavCods, List<string>? cauMovs, IntervalloTemporale dateInterval, AgronicaCoreParametriServer objParametriServer,
    // NUOVO PARAMETRO
    List<FiltroRicercaSemine>? filtroSemineImpiantil);
        Task<DataTable> LeggiImpiantiAsync(List<int> Id_Agenda, AgronicaCoreParametriServer objParametriServer, bool createTempEsercizi = false, bool leggiFlagEsercizioChiuso = false, bool leggiIndirizzoAppezzamento = false,
                                            bool leggiGIS = false,bool leggiDettagli_Tecnici = false, List<int>? Id_Esercizi = null, List<int>? Lav_cod = null);
        /// <param name="objParametriServer"></param>
        /// <returns></returns>
        Task<DataTable> LeggiProdottiOperazioniSenzaImpiantoAsync(List<int> idAgendas, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiProdottiAsync(List<int> Id_Agenda, bool leggiMateriePrime, bool leggiEsercizi, AgronicaCoreParametriServer objParametriServer,bool estraiSpecieECultivar = true, bool leggiFlagEsercizioChiuso = false, List<int>? Id_Esercizi = null, List<int>? Lav_cod = null);
        Task<DataTable> LeggiProdottiAgendaNoQdCAAsync(List<int>? Id_Agenda, List<int>? lavCods, List<string>? cauMovs, List<int>? elemCods, bool leggiMateriePrime, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiAcquaAsync(List<int> Id_Agenda, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiRilieviAsync(List<int> Id_Agenda, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiOperazioniAsync(string piva, int sacod, int vegcod, IntervalloTemporale dateInterval, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiDatiBaseOperazioniCorrelate(IEnumerable<int> idAgenda, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiDettCodAsync(IReadOnlyList<int> idAgendas, AgronicaCoreParametriServer objParametriServer);
        Task<bool> BulkInsertEserciziAsync(DataTable dtEsercizi, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiOperazionixVerificaConformitaAsync(
           AgronicaCoreParametriServer objParametriServer, bool isOggettoVerifica, bool isOggettoDiVerificaMagazzino,
           string? piva = null, IEnumerable<int>? sacod = null, IEnumerable<int>? vegcod = null,
           IEnumerable<int>? typeOperation = null, IntervalloTemporale? dateInterval = null);
        Task<DataTable> LeggiMovimentiProdottiPerOperazioniAsync(AgronicaCoreParametriServer objParametriServer, List<string> idAgendas, string piva);
        Task<DataTable> LeggiMovimentiCorrelatiAsync(List<MovimentoFilter> movimenti, DateTime dataFine, AgronicaCoreParametriServer objParametriServer);
    }
}
