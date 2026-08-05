using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.UtilityDB.BIZ.Services.Agro_Sequenze
{
    public interface IAgro_SequenceService
    {
        public Task<int> NuovoId_CentriAziendaliAsync(string piva, AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode);
        public Task<bool> CancellaId_CentriAziendaliAsync(string piva, AgronicaCoreParametri objParametri);
        public Task<int> NuovoId_CampiAsync(string piva,int sa_cod,  AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode);
        public Task<bool> CancellaId_CampiAsync(string piva, int sa_cod, AgronicaCoreParametri objParametri);
        public Task<int> NuovoId_AppezzamentoAsync(string piva, int sa_cod,  AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode);
        public Task<bool> CancellaId_AppezzamentoAsync(string piva, int sa_cod, AgronicaCoreParametri objParametri);
        public Task<int> NuovoId_Reg_ImpiantiAsync(string piva, int sa_cod,int appezza,  AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode);
        public Task<bool> CancellaId_Reg_ImpiantiAsync(string piva, int sa_cod,int appezza, AgronicaCoreParametri objParametri);
        public Task<int> NuovoId_MagazzinoAsync(string piva, int sa_cod,  AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode);
        public Task<bool> CancellaId_MagazzinoAsync(string piva, int sa_cod, AgronicaCoreParametri objParametri);
        public Task<int> NuovoId_TabellaAsync(string nomeTabella,  AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode);
        public Task<int> NuovoId_Seq_xPivaAsync(string nomeTabella, string nomeCampoContatore,string piva,  AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode);
        public Task<bool> CancellaId_Seq_xPivaAsync(string nomeTabella, string nomeCampoContatore, string piva, AgronicaCoreParametri objParametri);
        public Task<int> NuovoId_Seq_xPiva_xSaCodAsync(string nomeTabella, string nomeCampoContatore, string piva, int sa_cod,  AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode);
        public Task<bool> CancellaId_Seq_xPiva_xSaCodAsync(string nomeTabella, string nomeCampoContatore, string piva, int sa_cod, AgronicaCoreParametri objParametri);
        public Task<(int,int)> Calcola_BaseCodeAsync(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
