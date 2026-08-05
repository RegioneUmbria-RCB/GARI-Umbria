using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze
{
    public interface IAgro_Sequence
    {
        public Task<int> NuovoId_TabellaAsync(string NomeTabella, int Base, int Fine,AgronicaCoreParametri objParametri);
        public (int, int) Calcola_BaseCode(int ProgressivoGias, AgronicaCoreParametri objParametri);
        public Task<int> NuovoId_xPivaAsync(string NomeTabella, string NomeCampoContatore, string Piva, int Base, int Fine, AgronicaCoreParametri objParametri);
        public Task<int> NuovoId_xPiva_xSaCodAsync(string NomeTabella,string NomeCampoContatore,string Piva,int Sa_Cod,int Base,int Fine, AgronicaCoreParametri objParametri);
        public Task<int> NuovoId_xPiva_xSaCod_xAppezzaAsync(string NomeTabella, string NomeCampoContatore, string Piva, int Sa_Cod, int Appezza, int Base, int Fine, AgronicaCoreParametri objParametri);
        public Task<bool> CancellaId_xPivaAsync(string NomeTabella, string NomeCampoContatore, string Piva, AgronicaCoreParametri objParametri);
        public Task<bool> CancellaId_xPiva_xSaCodAsync(string NomeTabella, string NomeCampoContatore, string Piva, int Sa_Cod, AgronicaCoreParametri objParametri);
        public Task<bool> CancellaId_xPiva_xSaCod_xAppezzaAsync(string NomeTabella, string NomeCampoContatore, string Piva, int Sa_Cod, int Appezza, AgronicaCoreParametri objParametri);
    }
}
