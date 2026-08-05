using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.UtilityDB.BIZ.Resources;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Utenti_CodiciGiasPro;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.UtilityDB.BIZ.Services.Agro_Sequenze
{
    public class Agro_SequenceService : BaseServiceUtilityDBBiz, IAgro_SequenceService
    {
        private readonly IAgro_Sequence _sequenceDal;
        private readonly IUtenti_CodiciGiasPro _progGiasDal;

        public Agro_SequenceService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _sequenceDal = _serviceProvider.GetRequiredService<IAgro_Sequence>();
            _progGiasDal = _serviceProvider.GetRequiredService<IUtenti_CodiciGiasPro>();
        }


        /// <summary>
        /// Metodo per il recupero del base e top code dei contatori a partire dal progressivoGIAS associato al superuser
        /// </summary>
        /// <param name="objParametriServer"></param>
        /// <param name="objParametriUtenti"></param>
        /// <returns></returns>
        public async Task<(int, int)> Calcola_BaseCodeAsync(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            (int, int) ret = (0,0);
            try
            {
                var serialGias = await _progGiasDal.LeggiProgressivoGIASAsync(objParametriUtenti, objParametriServer);
                if (serialGias <= 0) throw new Exception("No Gias serial.");
                ret = _sequenceDal.Calcola_BaseCode(serialGias, objParametriServer);
            }
            catch (Exception)
            {
                ret = (0, 0);
                throw;
            }
            return ret;
        }

        #region "Cancellazione Id/Sequenze"
        public async Task<bool> CancellaId_AppezzamentoAsync(string piva, int sa_cod, AgronicaCoreParametri objParametri)
        {
            var chk = false;
            try
            {
                chk = await _sequenceDal.CancellaId_xPiva_xSaCodAsync("SeqAppezzamento", "Appezza", piva, sa_cod, objParametri);
            }
            catch (Exception)
            {
                chk = false;
                throw;
            }
            return chk;
        }

        public async Task<bool> CancellaId_CampiAsync(string piva, int sa_cod, AgronicaCoreParametri objParametri)
        {
            var chk = false;
            try
            {
                chk = await _sequenceDal.CancellaId_xPiva_xSaCodAsync("SeqCampi", "Campo_Cod", piva, sa_cod, objParametri);
            }
            catch (Exception)
            {
                chk = false;
                throw;
            }
            return chk;
        }

        public async Task<bool> CancellaId_CentriAziendaliAsync(string piva, AgronicaCoreParametri objParametri)
        {
            var chk = false;
            try
            {
                chk = await _sequenceDal.CancellaId_xPivaAsync("SeqCentri_Aziendali", "Sa_cod", piva, objParametri);
            }
            catch (Exception)
            {
                chk = false;
                throw;
            }
            return chk;
        }

        public async Task<bool> CancellaId_MagazzinoAsync(string piva, int sa_cod, AgronicaCoreParametri objParametri)
        {
            var chk = false;
            try
            {
                chk = await _sequenceDal.CancellaId_xPiva_xSaCodAsync("SeqMagazzino", "Mag_Cod", piva, sa_cod, objParametri);
            }
            catch (Exception)
            {
                chk = false;
                throw;
            }
            return chk;
        }

        public async Task<bool> CancellaId_Reg_ImpiantiAsync(string piva, int sa_cod, int appezza, AgronicaCoreParametri objParametri)
        {
            var chk = false;
            try
            {
                chk = await _sequenceDal.CancellaId_xPiva_xSaCod_xAppezzaAsync("SeqReg_Impianti", "Id_Reg", piva, sa_cod, appezza, objParametri);
            }
            catch (Exception)
            {
                chk = false;
                throw;
            }
            return chk;
        }

        public async Task<bool> CancellaId_Seq_xPivaAsync(string nomeTabella, string nomeCampoContatore, string piva, AgronicaCoreParametri objParametri)
        {
            var chk = false;
            try
            {
                chk = await _sequenceDal.CancellaId_xPivaAsync(nomeTabella, nomeCampoContatore, piva, objParametri);
            }
            catch (Exception)
            {
                chk = false;
                throw;
            }
            return chk;
        }

        public async Task<bool> CancellaId_Seq_xPiva_xSaCodAsync(string nomeTabella, string nomeCampoContatore, string piva, int sa_cod, AgronicaCoreParametri objParametri)
        {
            var chk = false;
            try
            {
                chk = await _sequenceDal.CancellaId_xPiva_xSaCodAsync(nomeTabella, nomeCampoContatore, piva,sa_cod, objParametri);
            }
            catch (Exception)
            {
                chk = false;
                throw;
            }
            return chk;
        }
        #endregion

        #region "Nuovi Id/Sequenze"

        /// <summary>
        /// Metodo per il recupero di un nuovo Id Appezzamento. (Il range sarà compreso da baseCode e topCode)
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="sa_cod"></param>
        /// <param name="objParametri"></param>
        /// <param name="baseCode"></param>
        /// <param name="topCode"></param>
        /// <returns></returns>
        public async Task<int> NuovoId_AppezzamentoAsync(string piva, int sa_cod, AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode)
        {
            var id = 0;
            try
            {
                CheckBaseTopCode(baseCode,topCode);
                id = await _sequenceDal.NuovoId_xPiva_xSaCodAsync("SeqAppezzamento","Appezza",piva,sa_cod,baseCode,topCode,objParametri);
            }
            catch (Exception)
            {
                id = 0;
                throw;
            }
            return id;
        }


        /// <summary>
        /// Metodo per il recupero di un nuovo Id Campo. (Il range sarà compreso da baseCode e topCode)
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="sa_cod"></param>
        /// <param name="objParametri"></param>
        /// <param name="baseCode"></param>
        /// <param name="topCode"></param>
        /// <returns></returns>
        public async Task<int> NuovoId_CampiAsync(string piva, int sa_cod, AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode)
        {
            var id = 0;
            try
            {
                CheckBaseTopCode(baseCode, topCode);
                id = await _sequenceDal.NuovoId_xPiva_xSaCodAsync("SeqCampi", "Campo_Cod", piva, sa_cod, baseCode, topCode, objParametri);
            }
            catch (Exception)
            {
                id = 0;
                throw;
            }
            return id;
        }

        /// <summary>
        /// Metodo per il recupero di un nuovo Id Centro Aziendale. (Il range sarà compreso da baseCode e topCode)
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="objParametri"></param>
        /// <param name="baseCode"></param>
        /// <param name="topCode"></param>
        /// <returns></returns>
        public async Task<int> NuovoId_CentriAziendaliAsync(string piva,  AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode)
        {
            var id = 0;
            try
            {
                CheckBaseTopCode(baseCode, topCode);
                id = await _sequenceDal.NuovoId_xPivaAsync("SeqCentri_Aziendali", "Sa_Cod", piva, baseCode, topCode, objParametri);
            }
            catch (Exception)
            {
                id = 0;
                throw;
            }
            return id;
        }

        /// <summary>
        /// Metodo per il recupero di un nuovo Id Magazzino. (Il range sarà compreso da baseCode e topCode)
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="sa_cod"></param>
        /// <param name="objParametri"></param>
        /// <param name="baseCode"></param>
        /// <param name="topCode"></param>
        /// <returns></returns>
        public async Task<int> NuovoId_MagazzinoAsync(string piva, int sa_cod, AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode)
        {
            var id = 0;
            try
            {
                CheckBaseTopCode(baseCode, topCode);
                id = await _sequenceDal.NuovoId_xPiva_xSaCodAsync("SeqMagazzino", "Mag_Cod", piva, sa_cod, baseCode, topCode, objParametri);
            }
            catch (Exception)
            {
                id = 0;
                throw;
            }
            return id;
        }

        /// <summary>
        /// Metodo per il recupero di un nuovo Id Reg_Impianti. (Il range sarà compreso da baseCode e topCode)
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="sa_cod"></param>
        /// <param name="appezza"></param>
        /// <param name="objParametri"></param>
        /// <param name="baseCode"></param>
        /// <param name="topCode"></param>
        /// <returns></returns>
        public async Task<int> NuovoId_Reg_ImpiantiAsync(string piva, int sa_cod, int appezza, AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode)
        {
            var id = 0;
            try
            {
                CheckBaseTopCode(baseCode, topCode);
                id = await _sequenceDal.NuovoId_xPiva_xSaCod_xAppezzaAsync("SeqReg_Impianti", "Id_Reg", piva, sa_cod,appezza, baseCode, topCode, objParametri);
            }
            catch (Exception)
            {
                id = 0;
                throw;
            }
            return id;
        }

        /// <summary>
        /// Metodo per il recupero di un nuovo Id per Tabella e Counter personalizzato organizzato per PartitaIva. (Il range sarà compreso da baseCode e topCode)
        /// Richiede che la tabella sia pre esistente
        /// </summary>
        /// <param name="nomeTabella"></param>
        /// <param name="nomeCampoContatore"></param>
        /// <param name="piva"></param>
        /// <param name="objParametri"></param>
        /// <param name="baseCode"></param>
        /// <param name="topCode"></param>
        /// <returns></returns>
        public async Task<int> NuovoId_Seq_xPivaAsync(string nomeTabella, string nomeCampoContatore, string piva, AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode)
        {
            var id = 0;
            try
            {
                id = await _sequenceDal.NuovoId_xPivaAsync(nomeTabella, nomeCampoContatore, piva, baseCode, topCode, objParametri);
            }
            catch (Exception)
            {
                id = 0;
                throw;
            }
            return id;
        }

        /// <summary>
        /// Metodo per il recupero di un nuovo Id per Tabella e Counter personalizzato organizzato per PartitaIva e Centro Aziendale. (Il range sarà compreso da baseCode e topCode)
        /// Richiede che la tabella sia pre esistente
        /// </summary>
        /// <param name="nomeTabella"></param>
        /// <param name="nomeCampoContatore"></param>
        /// <param name="piva"></param>
        /// <param name="sa_cod"></param>
        /// <param name="objParametri"></param>
        /// <param name="baseCode"></param>
        /// <param name="topCode"></param>
        /// <returns></returns>
        public async Task<int> NuovoId_Seq_xPiva_xSaCodAsync(string nomeTabella, string nomeCampoContatore, string piva, int sa_cod, AgronicaCoreParametri objParametri, int baseCode = CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode)
        {
            var id = 0;
            try
            {
                id = await _sequenceDal.NuovoId_xPiva_xSaCodAsync(nomeTabella, nomeCampoContatore, piva, sa_cod, baseCode, topCode, objParametri);
            }
            catch (Exception)
            {
                id = 0;
                throw;
            }
            return id;
        }

        /// <summary>
        /// Metodo per il recupero di un nuovo Id per Counter personalizzato.
        /// </summary>
        /// <param name="nomeTabella"></param>
        /// <param name="objParametri"></param>
        /// <param name="baseCode"></param>
        /// <param name="topCode"></param>
        /// <returns></returns>
        public async Task<int> NuovoId_TabellaAsync(string nomeTabella, AgronicaCoreParametri objParametri, int baseCode =CounterBaseTopCode.BaseCode, int topCode = CounterBaseTopCode.TopCode)
        {
            var id = 0;
            try
            {
                id = await _sequenceDal.NuovoId_TabellaAsync(nomeTabella,baseCode,topCode, objParametri); 
            }
            catch (Exception)
            {
                id = 0;
                throw;
            }
            return id;
        }
        #endregion

        #region "Private methods"
        private void CheckBaseTopCode(int baseCode,int topCode)
        {
            if (baseCode == CounterBaseTopCode.BaseCode)
                throw new Exception("base code is missing");
            if (topCode == CounterBaseTopCode.TopCode)
                throw new Exception("top code is missing");
        }
        #endregion
    }
}
