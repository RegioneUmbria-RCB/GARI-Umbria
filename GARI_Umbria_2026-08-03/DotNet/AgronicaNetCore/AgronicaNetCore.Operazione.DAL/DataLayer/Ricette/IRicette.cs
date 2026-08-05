using System.Data;
using InData.Agenda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Ricette
{
    public interface IRicette
    {
        Task<DataTable> ReadAsync(string Piva, int Sa_Cod, int Ricetta_Cod, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null);
        public Task<bool> ExistAsync(int Ricetta_Cod, AgronicaCoreParametriServer objParametriServer);

        public Task<bool> CreateAsync(WriteRicette dtoRicetta, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> UpdateAsync(WriteRicette dtoRicetta, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> DeleteAsync(int Ricetta_Cod, AgronicaCoreParametriServer objParametriServer);

        public Task<int> ScriviModificaAsync(WriteRicette dtoRicetta, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> BloccaAttivitaAsync(string Piva, int Sa_Cod, int Ricetta_Cod, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> BloccaAttivitaAsync(string Piva, int Sa_Cod, List<int> Ricette, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> SbloccaAttivitaAsync(string Piva, int Sa_Cod, int Ricetta_Cod, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> SbloccaAttivitaAsync(string Piva, int Sa_Cod, List<int> Ricette, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(WriteRicette dtoRicetta, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAssociateAsync(WriteRicette dtoRicetta, AgronicaCoreParametriServer objParametriServer);
        Task<RisultatoTabelleRicette> LeggiTutteLeTabellePerLeRicetteAsync(List<int> listaDiRicettaCod, List<int> listaDiRicettaOperazioneCod,
            AgronicaCoreParametriServer parametriServer);
    }
}
