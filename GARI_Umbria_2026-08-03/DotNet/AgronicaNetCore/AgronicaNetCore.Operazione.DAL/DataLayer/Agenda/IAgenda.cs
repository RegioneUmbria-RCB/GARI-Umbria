using InData.Agenda;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Agenda
{
    public interface IAgenda
    {
        public Task<DataTable> ReadAsync(string Piva, int Id_Agenda, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null);
        public Task<bool> ExistAsync(int Id_Agenda, AgronicaCoreParametriServer objParametriServer);

        public Task<bool> CreateAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> UpdateAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> DeleteAsync(string Piva, int Id_Agenda, int Sa_Cod, AgronicaCoreParametriServer objParametriServer);

        public Task<int> ScriviModificaAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer, string jobj = "");
        public Task<bool> BloccaAttivitaAsync(string Piva, int Sa_Cod, int Id_Agenda, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> BloccaAttivitaAsync(string Piva, int Sa_Cod, List<int> Operazioni, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> SbloccaAttivitaAsync(string Piva, int Sa_Cod, int Id_Agenda, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> SbloccaAttivitaAsync(string Piva, int Sa_Cod, List<int> Operazioni, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAssociateAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer);

        public Task<DataTable> ReadUtentiOperazioniAsync(string nomeDbUtenti, string dataInizio, string dataFine, AgronicaCoreParametriServer objParametriServer);
        Task<RisultatoTabelleAgenda> LeggiTutteLeTabelleDiAgenda(List<int> idAgendas, OpzioniLetturaAgenda opzioniLetturaAgenda, AgronicaCoreParametriTriple parametriTriple);
        Task<(DataTable dtAttivita, DataTable dtAttivitaCancellate)> LeggiAttivitaPerAppAsync(string piva, DateTime dataRiferimento, DateTime dataUltimaSincro, AgronicaCoreParametriServer parametriServer);
        Task<(DataTable dtBrogliacci, DataTable dtBrogliacciCancellati)> LeggiBrogliacciPerAppAsync(string piva, DateTime dataRiferimento,
                    DateTime dataUltimaSincro, AgronicaCoreParametriServer parametriServer);
        Task<(DataTable dtAttivita, DataTable dtAttivitaCancellate, DataTable dtBrogliacci, DataTable dtBrogliacciCancellati)> LeggiAttivitaBrogliacciPerAppAsync(string piva,
            DateTime dataRiferimento, DateTime dataUltimaSincro, AgronicaCoreParametriServer parametriServer);
        Task<(DataTable dtOperatori, DataTable dtMacchine)> LeggiOreOperatorePerListaAttivitaAsync(List<int> listaDiRicettaOperazioneCod, AgronicaCoreParametriServer parametriServer);
    }
}
