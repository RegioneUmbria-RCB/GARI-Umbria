using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.UtilityDB;
using AgronicaNetCore.UtilityDB.DAL.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Data;
using System.Dynamic;

namespace AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze
{
    public class Agro_Sequence : BaseDALUtilityDB, IAgro_Sequence
    {
        private readonly IUtilityDB _utilityDBDAL;
        private readonly TableSequences _seqTables;
        private readonly SqlSettings _sqlSettings;

        public Agro_Sequence(IServiceProvider provider,IOptions<SqlSettings> sqlOpt, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _utilityDBDAL = _serviceProvider.GetRequiredService<IUtilityDB>();
            _seqTables = new TableSequences();
            _sqlSettings = sqlOpt.Value;
        }
        #region "Public Methods"
        public async Task<bool> CancellaId_xPivaAsync(string NomeTabella, string NomeCampoContatore, string Piva, AgronicaCoreParametri objParametri)
        {
            return await Internal_CancellaId_xPivaAsync(NomeTabella,Piva,objParametri);
        }
        public async Task<bool> CancellaId_xPiva_xSaCodAsync(string NomeTabella, string NomeCampoContatore, string Piva, int Sa_Cod, AgronicaCoreParametri objParametri)
        {
            return await Internal_CancellaId_xPiva_xSaCodAsync(NomeTabella, Piva,Sa_Cod, objParametri);
        }
        public async Task<bool> CancellaId_xPiva_xSaCod_xAppezzaAsync(string NomeTabella, string NomeCampoContatore, string Piva, int Sa_Cod, int Appezza, AgronicaCoreParametri objParametri)
        {
            return await Internal_CancellaId_xPiva_xSaCod_xAppezzaAsync(NomeTabella, Piva, Sa_Cod,Appezza, objParametri);
        }
        public async Task<int> NuovoId_TabellaAsync(string NomeTabella, int Base, int Fine, AgronicaCoreParametri objParametri)
        {
            if (await CheckSequenceXTabellaAsync(NomeTabella, objParametri))
                return await Internal_NuovoId_Tabella_DaSequenzaAsync(NomeTabella, Base, Fine, objParametri);
            else
                return await Internal_NuovoId_TabellaAsync(NomeTabella, Base, Fine, objParametri);
        }
        public async Task<int> NuovoId_xPivaAsync(string NomeTabella, string NomeCampoContatore, string Piva, int Base, int Fine, AgronicaCoreParametri objParametri)
        {
            return await Internal_NuovoId_xPivaAsync(NomeTabella, NomeCampoContatore, Piva, Base, Fine, objParametri);
        }
        public async Task<int> NuovoId_xPiva_xSaCodAsync(string NomeTabella, string NomeCampoContatore, string Piva, int Sa_Cod, int Base, int Fine, AgronicaCoreParametri objParametri)
        {
            return await Internal_NuovoId_xPiva_xSaCodAsync(NomeTabella,NomeCampoContatore,Piva,Sa_Cod, Base, Fine, objParametri);
        }
        public async Task<int> NuovoId_xPiva_xSaCod_xAppezzaAsync(string NomeTabella, string NomeCampoContatore, string Piva, int Sa_Cod,int Appezza, int Base, int Fine, AgronicaCoreParametri objParametri)
        {
            return await Internal_NuovoId_xPiva_xSaCod_xAppezzaAsync(NomeTabella,NomeCampoContatore,Piva,Sa_Cod,Appezza,Base, Fine, objParametri);
        }
        public (int,int) Calcola_BaseCode(int ProgressivoGias, AgronicaCoreParametri objParametri)
        {
            var BaseCode = 0;
            var TopCode = 0;
            try
            {
                var k = 2 ^ 17;
                BaseCode = k * ProgressivoGias;
                TopCode = BaseCode + (k - 1);

            }catch(Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                BaseCode = 0;
                TopCode = 0;
            }
            return (BaseCode, TopCode);
        }
        #endregion

        #region "Private Methods"
        private async Task<bool> Esiste_SequenzaAsync(string NomeTabella, AgronicaCoreParametri objParametri)
        {
            var ret = false;
            var strSql = "";
            var parSql = new Dictionary<string, object>();
            var nomeSequenza = "Sequence_" + NomeTabella;
            DataTable? result = null;

            try
            {
                strSql = @$"
                    Select 
                        1
                    from
                        sys.sequences 
                    where
                        [name]=@p1
                    ";
                parSql.Add("@p1", nomeSequenza);

                result = await GetDataProvider(objParametri).ExecuteReadAsync(strSql, parSql);
                if (result != null && result.Rows.Count > 0)
                    ret = true;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                ret = false;
            }
            return ret;
        }
        private async Task<bool> Crea_SequenzaAsync(string NomeTabella, int StartValue, AgronicaCoreParametri objParametri, int Increment = 1)
        {
            var ret = false;
            var strSql = "";
            var nomeSequenza = "Sequence_" + NomeTabella;

            try
            {
                if (StartValue == 0) {
                    switch (NomeTabella.ToLower())
                    {
                        case "contatti":
                            StartValue = -2000000000;
                            break;
                        case "impresa":
                            StartValue = -2000000000;
                            break;
                    }
                } 
                StartValue += 1;

                strSql = @$"
                    create sequence {nomeSequenza}
                    start with {StartValue}
                    increment by {Increment}
                    ";

                ret = await GetDataProvider(objParametri).Execute_WriteAsync(strSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                ret = false;
            }
            return ret;
        }
        private async Task<int> ProssimoValore_SequenzaAsync(string NomeTabella, AgronicaCoreParametri objParametri)
        {
            var nextValue = 0;
            var strSql = "";
            var nomeSequenza = "Sequence_" + NomeTabella;
            DataTable? result = null;

            try
            {
                strSql = @$"
                    Select next value for {nomeSequenza}
                    ";

                result = await GetDataProvider(objParametri).ExecuteReadAsync(strSql);
                if (result != null && result.Rows.Count > 0)
                {
                    try
                    {
                        nextValue = Convert.ToInt32(result.Rows[0][0]);
                    }
                    catch(OverflowException)
                    {
                        nextValue = Int32.MaxValue;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                nextValue = 0;
                throw;
            }
            return nextValue;
        }
        private async Task<int> LastValue_SequenzaTabelleAsync(string NomeTabella, AgronicaCoreParametri objParametri)
        {
            var lastValue = 0;
            var strSql = "";
            var parSql = new Dictionary<string, object>();
            DataTable? result = null;
            try
            {
                strSql = @$"
                    Select Ultimo_Valore 
                    from Sequenza_Tabelle where 
                    Nome_Tabella=@p1
                    ";

                parSql.Add("@p1", NomeTabella);

                result = await GetDataProvider(objParametri).ExecuteReadAsync(strSql, parSql);
                if (result != null && result.Rows.Count > 0)
                    lastValue = (int)result.Rows[0]["Ultimo_Valore"];
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                lastValue = 0;
                throw;
            }
            return lastValue;
        }
        private async Task<bool> CheckSequenceXTabellaAsync(string NomeTabella, AgronicaCoreParametri objParametri)
        {
            if (!_sqlSettings.UseSequence)
            {
                return false;
            }

            var sqlMajor = await _utilityDBDAL.ReadSQLVersionMajorAsync(objParametri);
            if (sqlMajor <= 10) //major 10 corrisponde a sql server 2008 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            {
                return false;
            }
            if (_seqTables.SequenceTableExceptions.Select((x) => x.ToLower()).Contains(NomeTabella.ToLower()))
            {
                return false;
            }

            if (NomeTabella.ToLower().Contains("pdc_campioni_"))
            {
                return false;
            }

            return true;
        }
        
        private async Task<int> Internal_NuovoId_Tabella_DaSequenzaAsync(string NomeTabella, int Base, int Fine, AgronicaCoreParametri objParametri)
        {
            var newValue = 1;
            //var objDp = GetDataProvider(objParametri);
            //var (objConn, objTransact) = objDp.OpenConnection(false);
            //objParametri.objConnessione = objConn;
            try
            {
                if (!await Esiste_SequenzaAsync(NomeTabella, objParametri))
                {
                    var lVal = await LastValue_SequenzaTabelleAsync(NomeTabella, objParametri);
                    await Crea_SequenzaAsync(NomeTabella, lVal, objParametri);
                }
                newValue = await ProssimoValore_SequenzaAsync(NomeTabella, objParametri);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                throw;
            }
            finally
            {
                //objDp.CloseConnection(ref objConn);
                //objParametri.objConnessione = objConn;
            }
            return newValue;

        }
        private async Task<int> Internal_NuovoId_TabellaAsync(string NomeTabella, int Base, int Fine, AgronicaCoreParametri objParametri)
        {
            var newValue = 1;
            //var objDp = GetDataProvider(objParametri);
            //var LocalConnection = (objParametri.objConnessione==null ? true: false);
            //var LocalTransaction = (objParametri.objTransazione == null ? true : false);

            //if (LocalConnection)
            //{
            //    var (objConn, objTransact) = objDp.OpenConnection(LocalTransaction);
            //    objParametri.objConnessione = objConn;
            //    objParametri.objTransazione = objTransact;
            //}
            var strSql = "";
            var parSql = new Dictionary<string, object>();
            DataTable? result = null;

            try
            {
                strSql = @$"
                    if exists (select 1 from Sequenza_Tabelle where Nome_Tabella = @p1)
                        update Sequenza_Tabelle with (XLOCK, ROWLOCK) set Ultimo_Valore=Ultimo_Valore+1 
                        output inserted.Ultimo_Valore
                        where Nome_Tabella=@p1
                    else
                        insert into Sequenza_Tabelle output inserted.Ultimo_Valore
                        values (
                            @p1,
                            @p2,
                            @p3,    
                            @p4,
                            0,
                            null,
                            @p5,
                            @p5,
                            @p6,
                            @p6)
                    ";

                parSql.Add("@p1", NomeTabella);
                parSql.Add("@p2", newValue);
                parSql.Add("@p3", Base);
                parSql.Add("@p4", Fine);
                parSql.Add("@p5", DateTime.Now);
                parSql.Add("@p6", objParametri.UsernameOperazione);

                result = await GetDataProvider(objParametri).ExecuteReadAsync(strSql, parSql);
                if (result != null && result.Rows.Count > 0)
                    newValue = (int)result.Rows[0]["Ultimo_Valore"];

            }
            catch (Exception ex)
            {
                //if (LocalTransaction)
                //{
                //    var objTransact = objParametri.objTransazione;
                //    objDp.CloseTransaction(ref objTransact, true);
                //    objParametri.objTransazione = objTransact;
                //}
                LogError(ex.Message, objParametri, ex);
                result = null;
                throw;
            }
            finally
            {
                //if (LocalConnection)
                //{
                //    var objConn = objParametri.objConnessione;
                //    objDp.CloseConnection(ref objConn);
                //    objParametri.objConnessione = objConn;
                //}
            }
            return newValue;
        }
        private async Task<int> Internal_NuovoId_xPivaAsync(string NomeTabella, string NomeCampoContatore, string Piva, int Base, int Fine, AgronicaCoreParametri objParametri)
        {
            var newValue = 1;
            var strSql = "";
            var parSql = new Dictionary<string, object>();
            DataTable? result = null;

            try
            {
                strSql = @$"
                    if exist (select top 1 * from {NomeTabella} where piva = @p1)
                        update {NomeTabella} with (XLOCK, ROWLOCK) set {NomeCampoContatore}={NomeCampoContatore}+1 
                        output inserted.{NomeCampoContatore}
                        where piva = @p1
                    else
                        insert into {NomeTabella}
                        ( {NomeCampoContatore},
                          Piva,
                          Base,
                          [END],
                          inviato,
                          Data_Creazione,
                          Data_Modifica,
                          Username_Creazione,
                          Username_Modifica
                        )
                        output inserted.{NomeCampoContatore}
                        values (
                            @p2,
                            @p1,
                            @p3,    
                            @p4,
                            0,
                            @p5,
                            @p6)
                    ";

                parSql.Add("@p1", Piva);
                parSql.Add("@p2", newValue);
                parSql.Add("@p3", Base);
                parSql.Add("@p4", Fine);
                parSql.Add("@p5", DateTime.Now);
                parSql.Add("@p6", objParametri.UsernameOperazione);

                result = await GetDataProvider(objParametri).ExecuteReadAsync(strSql, parSql);
                if (result != null && result.Rows.Count > 0)
                    newValue = (int)result.Rows[0][NomeCampoContatore];

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                result = null;
                newValue = 0;
            }
            return newValue;
        }
        private async Task<int> Internal_NuovoId_xPiva_xSaCodAsync(string NomeTabella, string NomeCampoContatore,string Piva,int Sa_cod, int Base, int Fine, AgronicaCoreParametri objParametri)
        {
            var newValue = 1;
            var strSql = "";
            var parSql = new Dictionary<string, object>();
            DataTable? result = null;

            try
            {
                strSql = @$"
                    if exist (select top 1 * from {NomeTabella} where piva = @p1 and sa_Cod=@p2)
                        update {NomeTabella} with (XLOCK, ROWLOCK) set {NomeCampoContatore}={NomeCampoContatore}+1 
                        output inserted.{NomeCampoContatore}
                        where piva = @p1 and sa_Cod=@p2
                    else
                        insert into {NomeTabella}
                        ( {NomeCampoContatore},
                          Piva,
                          Sa_Cod,
                          Base,
                          [END],
                          inviato,
                          Data_Creazione,
                          Data_Modifica,
                          Username_Creazione,
                          Username_Modifica
                        )
                        output inserted.{NomeCampoContatore}
                        values (
                            @p3,
                            @p1,
                            @p2,    
                            @p4,
                            @p5,
                            0,
                            @p6,
                            @p7)
                    ";

                parSql.Add("@p1", Piva);
                parSql.Add("@p2", Sa_cod);
                parSql.Add("@p3", newValue);
                parSql.Add("@p4", Base);
                parSql.Add("@p5", Fine);
                parSql.Add("@p6", DateTime.Now);
                parSql.Add("@p7", objParametri.UsernameOperazione);

                result = await GetDataProvider(objParametri).ExecuteReadAsync(strSql, parSql);
                if (result != null && result.Rows.Count > 0)
                    newValue = (int)result.Rows[0][NomeCampoContatore];

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                result = null;
                newValue = 0;
            }
            return newValue;
        }
        private async Task<int> Internal_NuovoId_xPiva_xSaCod_xAppezzaAsync(string NomeTabella, string NomeCampoContatore, string Piva, int Sa_cod, int Appezza, int Base, int Fine, AgronicaCoreParametri objParametri)
        {
            var newValue = 1;
            var strSql = "";
            var parSql = new Dictionary<string, object>();
            DataTable? result = null;

            try
            {
                strSql = @$"
                    if exist (select top 1 * from {NomeTabella} where piva = @p1 and sa_Cod=@p2 and Appezza=@p3)
                        update {NomeTabella} with (XLOCK, ROWLOCK) set {NomeCampoContatore}={NomeCampoContatore}+1 
                        output inserted.{NomeCampoContatore}
                        where piva = @p1 and sa_Cod=@p2 and appezza=@p3
                    else
                        insert into {NomeTabella}
                        ( {NomeCampoContatore},
                          Piva,
                          Sa_Cod,
                          Appezza,
                          Base,
                          [END],
                          inviato,
                          Data_Creazione,
                          Data_Modifica,
                          Username_Creazione,
                          Username_Modifica
                        )
                        output inserted.{NomeCampoContatore}
                        values (
                            @p4,
                            @p1,
                            @p2,    
                            @p3,
                            @p5,
                            @p6,
                            0,
                            @p7,
                            @p8)
                    ";

                parSql.Add("@p1", Piva);
                parSql.Add("@p2", Sa_cod);
                parSql.Add("@p3", Appezza);
                parSql.Add("@p4", newValue);
                parSql.Add("@p5", Base);
                parSql.Add("@p6", Fine);
                parSql.Add("@p7", DateTime.Now);
                parSql.Add("@p8", objParametri.UsernameOperazione);

                result = await GetDataProvider(objParametri).ExecuteReadAsync(strSql, parSql);
                if (result != null && result.Rows.Count > 0)
                    newValue = (int)result.Rows[0][NomeCampoContatore];

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                result = null;
                newValue = 0;
            }
            return newValue;
        }
        private async Task<bool> Internal_CancellaId_xPivaAsync(string NomeTabella, string Piva, AgronicaCoreParametri objParametri)
        {
            var ret = false;
            var strSql = "";
            var parSql = new ExpandoObject();

            try
            {
                if (objParametri.FlagCancellazioneLogica == AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica)
                {
                    strSql = @$"
                        update {NomeTabella} with (XLOCK, ROWLOCK)
                        set
                            Username_Modifica = @p1,
                            Data_Modifica = @p2,
                            inviato=1
                        where
                            inviato >=0
                        and piva = @p3
                    ";

                    parSql.TryAdd("@p1", objParametri.UsernameOperazione);
                    parSql.TryAdd("@p2", DateTime.Now);
                    parSql.TryAdd("@p3", Piva);
                }
                else
                {
                    strSql = @$"
                        delete from {NomeTabella}
                        where
                            piva = @p1
                        ";

                    parSql.TryAdd("@p1", Piva);
                }

                ret = await GetDataProvider(objParametri).Execute_WriteAsync(strSql, parSql);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                ret = false;
            }
            return ret;
        }
        private async Task<bool> Internal_CancellaId_xPiva_xSaCodAsync(string NomeTabella, string Piva, int Sa_cod,AgronicaCoreParametri objParametri)
        {
            var ret = false;
            var strSql = "";
            var parSql = new ExpandoObject();

            try
            {
                if (objParametri.FlagCancellazioneLogica == AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica)
                {
                    strSql = @$"
                        update {NomeTabella} with (XLOCK, ROWLOCK)
                        set
                            Username_Modifica = @p1,
                            Data_Modifica = @p2,
                            inviato=1
                        where
                            inviato >=0
                        and piva = @p3
                        and sa_cod=@p4
                    ";

                    parSql.TryAdd("@p1", objParametri.UsernameOperazione);
                    parSql.TryAdd("@p2", DateTime.Now);
                    parSql.TryAdd("@p3", Piva);
                    parSql.TryAdd("@p4", Sa_cod);
                }
                else
                {
                    strSql = @$"
                        delete from {NomeTabella}
                        where
                            piva = @p1
                        and sa_cod=@p2
                        ";

                    parSql.TryAdd("@p1", Piva);
                    parSql.TryAdd("@p2", Sa_cod);
                }

                ret = await GetDataProvider(objParametri).Execute_WriteAsync(strSql, parSql);

            }
            catch(Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                ret = false;
            }
            return ret;
        }
        private async Task<bool> Internal_CancellaId_xPiva_xSaCod_xAppezzaAsync(string NomeTabella, string Piva, int Sa_cod, int Appezza, AgronicaCoreParametri objParametri)
        {
            var ret = false;
            var strSql = "";
            var parSql = new ExpandoObject();

            try
            {
                if (objParametri.FlagCancellazioneLogica == AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica)
                {
                    strSql = @$"
                        update {NomeTabella} with (XLOCK, ROWLOCK)
                        set
                            Username_Modifica = @p1,
                            Data_Modifica = @p2,
                            inviato=1
                        where
                            inviato >=0
                        and piva = @p3
                        and sa_cod=@p4
                        and appezza=@p5
                    ";

                    parSql.TryAdd("@p1", objParametri.UsernameOperazione);
                    parSql.TryAdd("@p2", DateTime.Now);
                    parSql.TryAdd("@p3", Piva);
                    parSql.TryAdd("@p4", Sa_cod);
                    parSql.TryAdd("@p5", Appezza);
                }
                else
                {
                    strSql = @$"
                        delete from {NomeTabella}
                        where
                            piva = @p1
                        and sa_cod=@p2
                        and appezza=@p3
                        ";

                    parSql.TryAdd("@p1", Piva);
                    parSql.TryAdd("@p2", Sa_cod);
                    parSql.TryAdd("@p3", Appezza);
                }

                ret = await GetDataProvider(objParametri).Execute_WriteAsync(strSql, parSql);

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametri, ex);
                ret = false;
            }
            return ret;
        }
        #endregion
    }
}
