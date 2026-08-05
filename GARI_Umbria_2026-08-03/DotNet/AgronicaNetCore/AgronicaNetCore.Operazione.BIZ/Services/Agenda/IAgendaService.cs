using InData.Agenda;
using AgronicaNetCore.Base.Models;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreDTOStd.InData.Agenda.OperazioneAgenda_Temp;
using AgronicaNetCore.Operazione.DAL.DataLayer.Agenda;

namespace AgronicaNetCore.Operazione.BIZ.Services.Agenda
{
    public interface IAgendaService
    {
        public Task<int> ScriviModificaAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer, string jobj = "");
        public Task<bool> BloccaAttivitaAsync(string Piva, int Sa_Cod, int Id_Agenda, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> BloccaAttivitaAsync(string Piva, int Sa_Cod, List<int> Operazioni, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> SbloccaAttivitaAsync(string Piva, int Sa_Cod, int Id_Agenda, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> SbloccaAttivitaAsync(string Piva, int Sa_Cod, List<int> Operazioni, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer);
        //public Task<bool> EliminaAssociateAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer);
        Task ScriviListaAttivitaAgendaAsync(List<(Attivita attivita, OperazioneAgenda agenda)> agendaActivityList, AgronicaCoreParametriServer objParametriServer);
        Task<int> ScriviAttivitaAgendaAsync((Attivita attivita, OperazioneAgenda agenda) agendaActivityList, AgronicaCoreParametriServer objParametriServer, bool useTransaction = true);
        Task ScriviListaAttivitaAsync(List<Attivita> activityList, AgronicaCoreParametriServer objParametriServer);
        public Task<List<Attivita>> LeggiListaAttivitaAsync(List<int> idAgendas, OpzioniLetturaAgenda opzioniLetturaAgenda, AgronicaCoreParametriTriple parametriTriple, bool manageConnectionLifetime = true);
        public Task<Attivita> LeggiAttivitaAsync(int idAgenda, OpzioniLetturaAgenda opzioniLetturaAgenda, AgronicaCoreParametriTriple parametriTriple, bool manageConnectionLifetime = true);
        Task ValorizzaArchivioAttivitaPerAppAsync(ArchivioAttivitaCampagna archivioAttivitaCampagna, string piva, DateTime dataUltimaSincro, DateTime dataRiferimento, AgronicaCoreParametriTriple objParametriTriple);
        Task<List<Attivita>> LeggiListaBrogliacciAsync(List<int> listaDiRicettaCod, List<int> listaDiRicettaOperazioneCod,
             AgronicaCoreParametriTriple parametriTriple, bool manageConnectionLifetime = true);
        Task<Attivita> LeggiBrogliaccioAsync(int ricettaCod, int ricettaOperazioneCod, AgronicaCoreParametriTriple parametriTriple, bool manageConnectionLifetime = true);
    }
}
