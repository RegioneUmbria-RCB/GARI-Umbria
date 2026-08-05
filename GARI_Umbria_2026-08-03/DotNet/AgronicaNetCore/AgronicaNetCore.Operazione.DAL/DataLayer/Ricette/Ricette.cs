using System.Data;
using System.Text;
using System.Dynamic;
using InData.Agenda;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Anagrafe.DAL.Base;
using static AgronicaNetCore.Base.Models.AgronicaCoreParametri;
using AgronicaDataProvider6.Extensions;
using System.Transactions;
using Microsoft.Extensions.DependencyInjection;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using AgronicaNetCore.Operazione.DAL.DataLayer.RicettexAgenda;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Ricette
{
    public class Ricette : DAL_Base, IRicette
    {
        private readonly IAgro_Sequence _sequenceDal;
        private readonly IRicettexAgenda _ricettexAgDal;

        public Ricette(IServiceProvider provider, bool securityBypass = false) : base(provider, securityBypass)
        {
            _ricettexAgDal = provider.GetRequiredService<IRicettexAgenda>();

            _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
        }

        private WriteRicette Valorizza(WriteRicette dtoRicette)
        {
            DateTime adInizio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
            DateTime adFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);

            dtoRicette.Sa_Cod ??= 0;
            dtoRicette.Ricetta_Des ??= "";
            dtoRicette.Ricetta_Des_Long ??= "";
            dtoRicette.Veg_Cod ??= -1;
            dtoRicette.Note ??= "";
            dtoRicette.Programmazione_Cod ??= 0;
            dtoRicette.Ricetta_Numero ??= "";
            dtoRicette.Imputazione_Cod ??= 0;
            dtoRicette.Imputazione_Fase_Cod ??= 0;
            dtoRicette.Origine ??= "";
            dtoRicette.DataLock ??= 0;

            dtoRicette.Blocco_Flag ??= 0;
            dtoRicette.Blocco_Data ??= adInizio;
            dtoRicette.Blocco_Username ??= "";
            dtoRicette.Inviato ??= 0;

            if (dtoRicette.Validita_Inizio < adInizio)
                dtoRicette.Validita_Inizio = adInizio;
            if (dtoRicette.Validita_Fine > adFine)
                dtoRicette.Validita_Fine = adFine;

            return dtoRicette;
        }

        public async Task<DataTable> ReadAsync(string Piva, int Sa_Cod, int Ricetta_Cod, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            stbQuery.AppendLine("SELECT * FROM Ricette ")
                .AppendLine("WHERE 1=1 ");

            sqlParams.TryAdd("@superUser", objParametriServer.PivaSuperUser);
            stbQuery.AppendLine("    AND SuperUser_Ricetta = @superUser ");

            if (!string.IsNullOrEmpty(Piva))
            {
                sqlParams.TryAdd("@piva", Piva);
                stbQuery.AppendLine("    AND PIVA = @piva ");
            }
            if (Sa_Cod != 0)
            {
                sqlParams.TryAdd("@saCod", Sa_Cod);
                stbQuery.AppendLine("    AND Sa_Cod = @saCod ");
            }
            if (Ricetta_Cod != 0)
            {
                sqlParams.TryAdd("@ricettaCod", Ricetta_Cod);
                stbQuery.AppendLine("    AND Ricetta_Cod = @ricettaCod ");
            }
            sqlParams.TryAdd("@inizio", objParametriServer.FinestraTemporaleFine);
            sqlParams.TryAdd("@fine", objParametriServer.FinestraTemporaleInizio);
            stbQuery.AppendLine("    AND Validita_Inizio <= @inizio ")
                .AppendLine("    AND Validita_Fine >= @fine ");

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            stbQuery.AppendLine("ORDER BY Piva, Sa_Cod, Ricetta_Cod ");

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> ExistAsync(int Ricetta_Cod, AgronicaCoreParametriServer objParametriServer)
        {
            if (Ricetta_Cod == 0)
                throw new Exception("Ricetta_Cod non valorizzato.");

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@ricettaCod", Ricetta_Cod);
            sqlParams.TryAdd("@superUser", objParametriServer.PivaSuperUser);

            stbQuery.AppendLine("SELECT TOP(1) * FROM Ricette ")
                .AppendLine("WHERE Ricetta_SuperUser = @superUser AND Ricetta_Cod = @ricettaCod ");

            try
            {
                var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
                if (dt.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> CreateAsync(WriteRicette dtoRicetta, AgronicaCoreParametriServer objParametriServer)
        {
            dtoRicetta = Valorizza(dtoRicetta);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Ricette (Piva, Sa_Cod, Ricetta_Cod, Ricetta_SuperUser, Tipo_Ricetta, Ricetta_Des, Ricetta_Des_Long, ")
                .AppendLine("    Veg_Cod, Note, DataLock, Programmazione_Cod, Ricetta_Numero, Imputazione_Cod, Imputazione_Fase_Cod, Origine, ")
                .AppendLine("    Blocco_Flag, Blocco_Data, Blocco_Username, inviato, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ");
            stbQuery.AppendLine("VALUES (@piva, @saCod, @ricettaCod, @superUser, @tipoRicetta, @ricettaDes, @ricettaDesLong, ")
                .AppendLine("    @vegCod, @note, @dataLock, @progrCod, @ricNum, @impuCod, @impuFCod, @orig, ")
                .AppendLine("    @bloccoFl, @bloccoDt, @bloccoUsr, @inviato, GETDATE(), GETDATE(), @userOp, @userOp, @inizio, @fine) ");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", dtoRicetta.Piva);
            expandoObj.TryAdd("@saCod", dtoRicetta.Sa_Cod);
            expandoObj.TryAdd("@ricettaCod", dtoRicetta.Ricetta_Cod);
            expandoObj.TryAdd("@superUser", objParametriServer.PivaSuperUser);
            expandoObj.TryAdd("@tipoRicetta", (int)dtoRicetta.Tipo_Ricetta);
            expandoObj.TryAdd("@ricettaDes", dtoRicetta.Ricetta_Des);
            expandoObj.TryAdd("@ricettaDesLong", dtoRicetta.Ricetta_Des_Long);
            
            expandoObj.TryAdd("@vegCod", dtoRicetta.Veg_Cod);
            expandoObj.TryAdd("@note", dtoRicetta.Note);
            expandoObj.TryAdd("@dataLock", dtoRicetta.DataLock);
            expandoObj.TryAdd("@progrCod", dtoRicetta.Programmazione_Cod);
            expandoObj.TryAdd("@ricNum", dtoRicetta.Ricetta_Numero);
            expandoObj.TryAdd("@impuCod", dtoRicetta.Imputazione_Cod);
            expandoObj.TryAdd("@impuFCod", dtoRicetta.Imputazione_Fase_Cod);
            expandoObj.TryAdd("@orig", dtoRicetta.Origine);

            expandoObj.TryAdd("@bloccoFl", dtoRicetta.Blocco_Flag);
            expandoObj.TryAdd("@bloccoDt", dtoRicetta.Blocco_Data);
            expandoObj.TryAdd("@bloccoUsr", dtoRicetta.Blocco_Username);
            expandoObj.TryAdd("@inviato", dtoRicetta.Inviato);
            //expandoObj.TryAdd("@dtInvio", dtoRicetta.Data_Invio);
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@inizio", dtoRicetta.Validita_Inizio);
            expandoObj.TryAdd("@fine", dtoRicetta.Validita_Fine);

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(WriteRicette dtoRicetta, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrEmpty(dtoRicetta.Piva))
                throw new Exception("Piva non valorizzato.");
            if (dtoRicetta.Sa_Cod == 0)
                throw new Exception("Sa_Cod non valorizzato.");
            if (dtoRicetta.Ricetta_Cod == 0)
                throw new Exception("Ricetta_Cod non valorizzato.");
            
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@superUser", objParametriServer.PivaSuperUser);
            expandoObj.TryAdd("@piva", dtoRicetta.Piva);
            expandoObj.TryAdd("@saCod", dtoRicetta.Sa_Cod);
            expandoObj.TryAdd("@ricettaCod", dtoRicetta.Ricetta_Cod);
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);

            var excludedProperties = new HashSet<string>
            {
                "Ricetta_SuperUser",
                "Piva",
                "Sa_Cod",
                "inviato",
                "DataInvio",
                "Validita_Inizio",
                "Validita_Fine",
                "Blocco_Flag",
                "Blocco_Data",
                "Blocco_Username",
                "Username_Creazione",
                "Username_Modifica",
                "Data_Creazione",
                "Data_Modifica"
            };

            var setClauses = new List<string>();
            foreach (var property in typeof(WriteRicette).GetProperties())
            {
                if (excludedProperties.Contains(property.Name)) continue;

                var value = property.GetValue(dtoRicetta);
                if (value != null)
                {
                    string paramName = "@" + property.Name;
                    setClauses.Add($"{property.Name} = {paramName}");
                    expandoObj.TryAdd(paramName, value);
                }
            }
            if (setClauses.Count == 0) return false;

            string updateQuery =
                $@"UPDATE Ricette SET
                    {string.Join(", ", setClauses)}
                    , Username_Modifica = @userOp
                    , Data_Modifica = GETDATE()
                WHERE Ricetta_SuperUser = @superUser AND Piva = @piva AND Sa_Cod = @saCod AND Ricetta_Cod = @ricettaCod ";

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(updateQuery, expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int Ricetta_Cod, AgronicaCoreParametriServer objParametriServer)
        {
            if (Ricetta_Cod == 0)
                throw new Exception("Ricetta_Cod non valorizzato.");

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@superUser", objParametriServer.PivaSuperUser);
            expandoObj.TryAdd("@ricettaCod", Ricetta_Cod);

            if (objParametriServer.FlagCancellazioneLogica == enumCancellazioneLogica.CancellazioneLogica)
            {
                stbQuery.AppendLine("UPDATE Ricette SET ")
                    .AppendLine("    Inviato = -1, ")
                    .AppendLine("    Username_Modifica = @userOp ")
                    .AppendLine("WHERE Inviato >= 0 ");
                expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            }
            else
            {
                stbQuery.AppendLine("DELETE FROM Ricette ")
                    .AppendLine("WHERE 1=1 ");
            }

            stbQuery.AppendLine("    AND SuperUser_Ricetta = @superUser ")
                .AppendLine("    AND Ricetta_Cod = @ricettaCod ");

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<int> ScriviModificaAsync(WriteRicette dtoRicetta, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                bool isNew = false;

                if (dtoRicetta.Ricetta_Cod == 0)
                {
                    dtoRicetta.Ricetta_Cod = await _sequenceDal.NuovoId_TabellaAsync("ricette", 0, 2000000000, objParametriServer);
                    isNew = true;
                }
                else
                    isNew = !await ExistAsync(dtoRicetta.Ricetta_Cod, objParametriServer);

                if (isNew)
                    await CreateAsync(dtoRicetta, objParametriServer);
                else
                    await UpdateAsync(dtoRicetta, objParametriServer);

                return dtoRicetta.Ricetta_Cod;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> BloccaAttivitaAsync(string Piva, int Sa_Cod, int Ricetta_Cod, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                WriteRicette dtoRicetta = new(Piva, Sa_Cod, Ricetta_Cod)
                {
                    Blocco_Flag = 1,
                    Blocco_Data = DateTime.Now,
                    Blocco_Username = objParametriServer.UsernameOperazione
                };
                return await UpdateAsync(dtoRicetta, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> SbloccaAttivitaAsync(string Piva, int Sa_Cod, int Ricetta_Cod, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                WriteRicette dtoRicetta = new(Piva, Sa_Cod, Ricetta_Cod)
                {
                    Blocco_Flag = 0,
                    Blocco_Data = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                    Blocco_Username = objParametriServer.UsernameOperazione
                };
                return await UpdateAsync(dtoRicetta, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> BloccaAttivitaAsync(string Piva, int Sa_Cod, List<int> Ricette, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(objParametriServer.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (int Ricetta_Cod in Ricette)
                    {
                        WriteRicette dtoRicetta = new(Piva, Sa_Cod, Ricetta_Cod)
                        {
                            Blocco_Flag = 1,
                            Blocco_Data = DateTime.Now,
                            Blocco_Username = objParametriServer.UsernameOperazione
                        };
                        await UpdateAsync(dtoRicetta, objParametriServer);
                    }

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    LogError(ex.Message, objParametriServer, ex);
                    throw;
                }
                finally
                {
                    if (objParametriServer.objTransazione == null)
                        ts.Dispose();
                }
            }

            return true;
        }

        public async Task<bool> SbloccaAttivitaAsync(string Piva, int Sa_Cod, List<int> Ricette, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(objParametriServer.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (int Ricetta_Cod in Ricette)
                    {
                        WriteRicette dtoRicetta = new(Piva, Sa_Cod, Ricetta_Cod)
                        {
                            Blocco_Flag = 0,
                            Blocco_Data = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                            Blocco_Username = objParametriServer.UsernameOperazione
                        };
                        await UpdateAsync(dtoRicetta, objParametriServer);
                    }

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    LogError(ex.Message, objParametriServer, ex);
                    throw;
                }
                finally
                {
                    if (objParametriServer.objTransazione == null)
                        ts.Dispose();
                }
            }

            return true;
        }

        public async Task<bool> EliminaAsync(WriteRicette dtoRicetta, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await DeleteAsync(dtoRicetta.Ricetta_Cod, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAssociateAsync(WriteRicette dtoRicetta, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(objParametriServer.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    /*-- RICETTExAGENDA --*/
                    DataTable dtRxA = await _ricettexAgDal.ReadAsync(dtoRicetta.Ricetta_Cod, 0, objParametriServer);
                    if (dtRxA.Rows.Count > 0)
                    {
                        var dtosRxA = dtRxA.ToDictionaryList().Select(row => new WriteRicettexAgenda()
                        {
                            Ricetta_Cod = (int)row["Ricetta_Cod"],
                            Id_Agenda = (int)row["Id_Agenda"]
                        }).ToList();
                        await _ricettexAgDal.EliminaAsync(dtosRxA, objParametriServer);
                    }

                    /*-- RICETTE --*/
                    await DeleteAsync(dtoRicetta.Ricetta_Cod, objParametriServer);

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    LogError(ex.Message, objParametriServer, ex);
                    throw;
                }
                finally
                {
                    if (objParametriServer.objTransazione == null)
                        ts.Dispose();
                }
            }

            return true;
        }

        public async Task<RisultatoTabelleRicette> LeggiTutteLeTabellePerLeRicetteAsync(List<int> listaDiRicettaCod, List<int> listaDiRicettaOperazioneCod, 
            AgronicaCoreParametriServer parametriServer)
        {
            var sb = new StringBuilder();
            var parametersSql = new Dictionary<string, object>();

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Ricetta_Cod, Tipo_Ricetta, Origine, Ricetta_Des, Ricetta_Des_Long, Ricetta_Numero, Note, Validita_Inizio, Validita_Fine, ");
            sb.AppendLine("     Programmazione_Cod, Piva, Sa_Cod ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     Ricette ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     Ricetta_Cod IN (@listaDiRicettaCod) ");

            sb.AppendLine(" ;");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Ricetta_Cod, Ricetta_SuperUser, Ricetta_Operazione_Cod, Ricetta_Operazione_Des, Lav_Cod, Extra_Int, ");
            sb.AppendLine("     Validita_Inizio, Ora, Validita_Fine, Note, W_Anagrafica_Stati_Cod, Raccoglitore_Cod, Invia_App, APP_Ricetta_Operazione_ID, ");
            sb.AppendLine("     Num_Protocollo, Id_Rcdpi, Mezzo");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     Ricette_Operazioni ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     Ricetta_Operazione_Cod IN (@listaDiRicettaOperazioneCod) ");

            sb.AppendLine(" ;");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Ricetta_Cod, Ricetta_SuperUser, Ricetta_Operazione_Cod, Ricetta_Dettaglio_Cod, Elem_Cod, ");
            sb.AppendLine("     Pro_Cod, Mat_Cod, Lotto, Cau_Mov, ID_Attivita, Qta, Qta_Extra, ");
            sb.AppendLine("     Qta_Extra_Totale, Mezzo_Det, Udm_Cod, Udm_Cod_Extra, Extra_Int, DoseEtichetta,");
            sb.AppendLine("     DoseEtichetta_Value, PrincipiAttivi, PrincipiAttiviPesi, PrincipiAttiviPercAbb, Buffer,");
            sb.AppendLine("     Extra_Str, TempoCarenza, Polverulento, Validita_Inizio ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     Ricette_Dettagli ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     Ricetta_Operazione_Cod IN (@listaDiRicettaOperazioneCod) ");

            sb.AppendLine(" ;");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Ricetta_Operazione_Cod, Ricetta_Dettaglio_Cod, Ricetta_Tecnico_Cod,    ");
            sb.AppendLine("     Qta_Ril, Dose, Parziale, Efficienza, Nitrati,");
            sb.AppendLine("     Inn1_Data, Inn2_Data, Freatimetro, Dett_Cod, Ditta_Cod, Av_Cod, Av_Gru,");
            sb.AppendLine("     Soglia_Cod, Soglia_Quantita, N, P, K, Cu, Mg ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     Ricette_Dettaglio_Tecnico ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     Ricetta_Operazione_Cod IN (@listaDiRicettaOperazioneCod) ");

            sb.AppendLine(" ;");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Ricetta_Cod, Ricetta_SuperUser, Ricetta_Operazione_Cod, Ricetta_Dettaglio_Cod, Ricetta_Destinazione_Cod, ");
            sb.AppendLine("     Tipo_Destinazione, Qta, Piva, Sa_Cod, Appezza, ");
            sb.AppendLine("     Id_Reg, Qta2, Sup_Riduzione_BufferZone, Perc_Riduzione_Deriva, ");
            sb.AppendLine("     MagazzinoEsterno_Cod, MagazzinoEsterno_Des, MagazzinoEsterno_Dettagli ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     Ricette_Destinazioni ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     Ricetta_Operazione_Cod IN (@listaDiRicettaOperazioneCod) ");

            sb.AppendLine(" ;");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Id_Agenda, Ricetta_Operazione_Cod ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     RicettexAgenda ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     Ricetta_Operazione_Cod IN (@listaDiRicettaOperazioneCod) ");

            sb.AppendLine(" ;");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Nota_Cod, Ricetta_Operazione_Cod ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     RicettexNote ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     Ricetta_Operazione_Cod IN (@listaDiRicettaOperazioneCod) ");

            sb.AppendLine(" ;");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("   ni.[Nota_Cod], ");
            sb.AppendLine("   ni.[NotaGruppo_Cod] ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("   Note_Intervento ni ");
            sb.AppendLine(" JOIN ");
            sb.AppendLine("   Note_Intervento_Gruppi nig ");
            sb.AppendLine("   ON ni.PivaSuperUser = nig.PivaSuperUser  ");
            sb.AppendLine("   And ni.NotaGruppo_Cod = nig.NotaGruppo_Cod ");
            sb.AppendLine(" JOIN ");
            sb.AppendLine("   Note_Intervento_UtilizzoxGruppi niu ");
            sb.AppendLine("   ON nig.PivaSuperUser = niu.PivaSuperUser  ");
            sb.AppendLine("   AND nig.NotaGruppo_Cod = niu.NotaGruppo_Cod ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("   ni.PivaSuperUser = @pivaSuperUser ");
            sb.AppendLine("   AND NotaUtilizzo_Cod = @notaUtilizzoCod");

            sb.AppendLine(" ;");

            parametersSql.Add("@pivaSuperUser", parametriServer.PivaSuperUser);
            parametersSql.Add("@notaUtilizzoCod", (int)Enum_Note_Intervento_Utilizzo.Ricetta);

            Dictionary<string, Dictionary<Type, List<object>>> parametersIn = new()
            {
                { "@listaDiRicettaCod", FormatClauseIn(listaDiRicettaCod) },
                { "@listaDiRicettaOperazioneCod", FormatClauseIn(listaDiRicettaOperazioneCod) },
            };

            DataSet risultatoLettura;
            try
            {
                risultatoLettura = await GetDataProvider(parametriServer).ExecuteMultipleReadAsync(sb.ToString(), "resultTabelleRicette", parametersSql, parametersIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriServer, ex);
                throw;
            }

            DataTable dtRicette = risultatoLettura.Tables[0];
            DataTable dtRicetteOperazioni = risultatoLettura.Tables[1];
            DataTable dtRicetteDettagli = risultatoLettura.Tables[2];
            DataTable dtRicetteDettaglioTecnico = risultatoLettura.Tables[3];
            DataTable dtRicetteDestinazioni = risultatoLettura.Tables[4];
            DataTable dtRicettexAgenda = risultatoLettura.Tables[5];
            DataTable dtNote = risultatoLettura.Tables[6];
            DataTable dtNoteIntervento = risultatoLettura.Tables[7];

            return new RisultatoTabelleRicette(dtRicette.AsEnumerable().ToList(),
                dtRicetteOperazioni.AsEnumerable().ToList(),
                dtRicetteDettagli.AsEnumerable().ToList(),
                dtRicetteDettaglioTecnico.AsEnumerable().ToList(),
                dtRicetteDestinazioni.AsEnumerable().ToList(),
                dtRicettexAgenda.AsEnumerable().ToList(),
                dtNote.AsEnumerable().ToList(),
                dtNoteIntervento.AsEnumerable().ToList());
        }
    }

    public class RisultatoTabelleRicette
    {
        public List<DataRow> RowsRicette { get; set; }
        public List<DataRow> RowsRicetteOperazioni { get; set; }
        public List<DataRow> RowsRicetteDettagli { get; set; }
        public List<DataRow> RowsRicetteDettaglioTecnico { get; set; }
        public List<DataRow> RowsRicetteDestinazioni { get; set; }
        public List<DataRow> RowsRicetteXAgenda { get; set; }
        public List<DataRow> RowsNote { get; set; }
        public List<DataRow> RowsNoteIntervento { get; set; }
        public RisultatoTabelleRicette(List<DataRow> rowsRicette, List<DataRow> rowsRicetteOperazioni, List<DataRow> rowsRicetteDettagli,
            List<DataRow> rowsRicetteDettaglioTecnico, List<DataRow> rowsRicetteDestinazioni, List<DataRow> rowsRicetteXAgenda,
            List<DataRow> rowsNote, List<DataRow> rowsNoteIntervento)
        {
            RowsRicette = rowsRicette;
            RowsRicetteOperazioni = rowsRicetteOperazioni;
            RowsRicetteDettagli = rowsRicetteDettagli;
            RowsRicetteDettaglioTecnico = rowsRicetteDettaglioTecnico;
            RowsRicetteDestinazioni = rowsRicetteDestinazioni;
            RowsRicetteXAgenda = rowsRicetteXAgenda;
            RowsNote = rowsNote;
            RowsNoteIntervento = rowsNoteIntervento;
        }
    }

}
