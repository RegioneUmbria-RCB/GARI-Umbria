using System.Text;
using InData.Agenda;
using System.Dynamic;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Constants;
using static AgronicaNetCore.Base.Models.AgronicaCoreParametri;
using System.Data;
using System.Transactions;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Operazione.DAL.Resources;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti_Zoo
{
    public class Movimenti_Zoo : BaseDALOperazione, IMovimenti_Zoo
    {
        public Movimenti_Zoo(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider,localizer, securityBypass)
        {
        }

        private WriteMovimentiZoo Valorizza(WriteMovimentiZoo dtoMovZoo)
        {
            DateTime adInizio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
            DateTime adFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);

            dtoMovZoo.Sa_Cod ??= 0;
            dtoMovZoo.Raz_Cod ??= 0;
            dtoMovZoo.Nazione_Cod ??= "";
            dtoMovZoo.Progetto ??= "";
            dtoMovZoo.Certificato ??= "";
            dtoMovZoo.Data_Documento_Ingresso ??= adInizio;
            dtoMovZoo.Fornitore_Provenienza ??= "";
            dtoMovZoo.Fornitore_Fatturazione ??= "";
            dtoMovZoo.Lotto_Fornitore ??= "";
            dtoMovZoo.N_Bolla_Fornitore ??= "";
            dtoMovZoo.Data_DDT_Ingresso ??= adInizio;
            dtoMovZoo.Modello4_Ingresso ??= "";
            dtoMovZoo.Modello4_Ingresso_Numero ??= "";
            dtoMovZoo.Kg_Pagati ??= 0;
            dtoMovZoo.Kg_Arrivo ??= 0;
            dtoMovZoo.Kg_Pagati_Medio ??= 0;
            dtoMovZoo.Kg_Arrivo_Medio ??= 0;
            dtoMovZoo.Calo_Tot ??= 0;
            dtoMovZoo.Calo_Medio ??= 0;
            dtoMovZoo.Calo_Perc ??= 0;
            dtoMovZoo.Incremento_Teorico ??= 0;
            dtoMovZoo.Costo_Totale ??= 0;
            dtoMovZoo.Costo_Unitario ??= 0;
            dtoMovZoo.Pres_Numero ??= "";
            dtoMovZoo.PresRiga_Numero ??= "";
            dtoMovZoo.Tipo_Calcolo_Peso ??= 0;
            dtoMovZoo.Kg_Partenza ??= 0;
            dtoMovZoo.Coeff_Calo_Peso ??= 0;
            dtoMovZoo.Stato_Trattamento ??= 0;
            dtoMovZoo.Tipo_Trattamento ??= 0;
            dtoMovZoo.Kg_Aggiuntivi ??= 0;
            dtoMovZoo.Note ??= "";

            dtoMovZoo.Inviato ??= 0;

            if (dtoMovZoo.Validita_Inizio < adInizio)
                dtoMovZoo.Validita_Inizio = adInizio;
            if (dtoMovZoo.Validita_Fine > adFine)
                dtoMovZoo.Validita_Fine = adFine;

            return dtoMovZoo;
        }

        public async Task<DataTable> ReadAsync(string Piva, int Id_Agenda, int Id_Mov, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT * FROM Movimenti_Zoo ")
                .AppendLine("WHERE 1=1 ");

            if (!string.IsNullOrEmpty(Piva))
            {
                sqlParams.TryAdd("@piva", Piva);
                stbQuery.AppendLine("    AND PIVA = @piva ");
            }
            if (Id_Agenda != 0)
            {
                sqlParams.TryAdd("@idAgenda", Id_Agenda);
                stbQuery.AppendLine("    AND Id_Agenda = @idAgenda ");
            }
            if (Id_Mov != 0)
            {
                sqlParams.TryAdd("@idMov", Id_Mov);
                stbQuery.AppendLine("    AND Id_Mov = @idMov ");
            }
            sqlParams.TryAdd("@inizio", objParametriServer.FinestraTemporaleFine);
            sqlParams.TryAdd("@fine", objParametriServer.FinestraTemporaleInizio);
            stbQuery.AppendLine("    AND Validita_Inizio <= @inizio ")
                .AppendLine("    AND Validita_Fine >= @fine ");

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            stbQuery.AppendLine("ORDER BY Id_Agenda, Id_Mov DESC ");

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

        public async Task<bool> ExistAsync(string Piva, int Id_Agenda, int Id_Mov, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrEmpty(Piva))
                throw new Exception("Piva non valorizzata.");
            if (Id_Agenda == 0)
                throw new Exception("Id_Agenda non valorizzato.");
            if (Id_Mov == 0)
                throw new Exception("Id_Mov non valorizzato.");

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@piva", Piva);
            sqlParams.TryAdd("@idAgenda", Id_Agenda);
            sqlParams.TryAdd("@idMov", Id_Mov);

            stbQuery.AppendLine("SELECT TOP(1) * FROM Movimenti_Zoo ")
                .AppendLine("WHERE 1=1 ")
                .AppendLine("    AND Piva = @piva ")
                .AppendLine("    AND Id_Agenda = @idAgenda ")
                .AppendLine("    AND Id_Mov = @idMov ");

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

        public async Task<bool> CreateAsync(WriteMovimentiZoo dtoMovZoo, AgronicaCoreParametriServer objParametriServer)
        {
            dtoMovZoo = Valorizza(dtoMovZoo);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Movimenti_Zoo (Piva, Sa_Cod, Id_Agenda, Id_Mov, ")
                .AppendLine("    Raz_Cod, Nazione_Cod, Progetto, Certificato, Data_Documento_Ingresso, Fornitore_Provenienza, Fornitore_Fatturazione, ")
                .AppendLine("    Lotto_Fornitore, N_Bolla_Fornitore, Data_DDT_Ingresso, Modello4_Ingresso, Modello4_Ingresso_Numero, Kg_Pagati, Kg_Arrivo, ")
                .AppendLine("    Kg_Pagati_Medio, Kg_Arrivo_Medio, Calo_Tot, Calo_Medio, Calo_Perc, Incremento_Teorico, Costo_Totale, Costo_Unitario, ")
                .AppendLine("    Pres_Numero, PresRiga_Numero, Tipo_Calcolo_Peso, Kg_Partenza, Coeff_Calo_Peso, Stato_Trattamento, Tipo_Trattamento, Kg_Aggiuntivi, Note, ")
                .AppendLine("    inviato, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ");
            stbQuery.AppendLine("VALUES (@piva, @saCod, @idAgenda, @idMov, ")
                .AppendLine("    @razCod, @nazCod, @prog, @certif, @dataDocIn, @fornProv, @fornFatt, ")
                .AppendLine("    @lottoForn, @nBollaForn, @dataDdtIn, @mod4In, @mod4InN, @kgPag, @kgArr, ")
                .AppendLine("    @kgPagM, @kgArrM, @caloTot, @caloMed, @caloPerc, @incTeo, @costoTot, @costoUnit, ")
                .AppendLine("    @presNum, @presRNum, @tipoCalcPeso, @kgPart, @coeffCaloP, @statoTratt, @tipoTratt, @kgAgg, @note, ")
                .AppendLine("    @inviato, GETDATE(), GETDATE(), @userOp, @userOp, @inizio, @fine) ");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", dtoMovZoo.Piva);
            expandoObj.TryAdd("@saCod", dtoMovZoo.Sa_Cod);
            expandoObj.TryAdd("@idAgenda", dtoMovZoo.Id_Agenda);
            expandoObj.TryAdd("@idMov", dtoMovZoo.Id_Mov);

            expandoObj.TryAdd("@razCod", dtoMovZoo.Raz_Cod);
            expandoObj.TryAdd("@nazCod", dtoMovZoo.Nazione_Cod);
            expandoObj.TryAdd("@prog", dtoMovZoo.Progetto);
            expandoObj.TryAdd("@certif", dtoMovZoo.Certificato);
            expandoObj.TryAdd("@dataDocIn", dtoMovZoo.Data_Documento_Ingresso);
            expandoObj.TryAdd("@fornProv", dtoMovZoo.Fornitore_Provenienza);
            expandoObj.TryAdd("@fornFatt", dtoMovZoo.Fornitore_Fatturazione);
            expandoObj.TryAdd("@lottoForn", dtoMovZoo.Lotto_Fornitore);
            expandoObj.TryAdd("@nBollaForn", dtoMovZoo.N_Bolla_Fornitore);
            expandoObj.TryAdd("@dataDdtIn", dtoMovZoo.Data_DDT_Ingresso);
            expandoObj.TryAdd("@mod4In", dtoMovZoo.Modello4_Ingresso);
            expandoObj.TryAdd("@mod4InN", dtoMovZoo.Modello4_Ingresso_Numero);
            expandoObj.TryAdd("@kgPag", dtoMovZoo.Kg_Pagati);
            expandoObj.TryAdd("@kgArr", dtoMovZoo.Kg_Arrivo);
            expandoObj.TryAdd("@kgPagM", dtoMovZoo.Kg_Pagati_Medio);
            expandoObj.TryAdd("@kgArrM", dtoMovZoo.Kg_Arrivo_Medio);
            expandoObj.TryAdd("@caloTot", dtoMovZoo.Costo_Totale);
            expandoObj.TryAdd("@caloMed", dtoMovZoo.Calo_Medio);
            expandoObj.TryAdd("@caloPerc", dtoMovZoo.Calo_Perc);
            expandoObj.TryAdd("@incTeo", dtoMovZoo.Incremento_Teorico);
            expandoObj.TryAdd("@costoTot", dtoMovZoo.Costo_Totale);
            expandoObj.TryAdd("@costoUnit", dtoMovZoo.Costo_Unitario);
            expandoObj.TryAdd("@presNum", dtoMovZoo.Pres_Numero);
            expandoObj.TryAdd("@presRNum", dtoMovZoo.PresRiga_Numero);
            expandoObj.TryAdd("@tipoCalcPeso", dtoMovZoo.Tipo_Calcolo_Peso);
            expandoObj.TryAdd("@kgPart", dtoMovZoo.Kg_Partenza);
            expandoObj.TryAdd("@coeffCaloP", dtoMovZoo.Coeff_Calo_Peso);
            expandoObj.TryAdd("@statoTratt", dtoMovZoo.Stato_Trattamento);
            expandoObj.TryAdd("@tipoTratt", dtoMovZoo.Tipo_Trattamento);
            expandoObj.TryAdd("@kgAgg", dtoMovZoo.Kg_Aggiuntivi);
            expandoObj.TryAdd("@note", dtoMovZoo.Note);

            expandoObj.TryAdd("@inviato", dtoMovZoo.Inviato);
            //expandoObj.TryAdd("@dtInvio", dtoMovZoo.Data_Invio);
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@inizio", dtoMovZoo.Validita_Inizio);
            expandoObj.TryAdd("@fine", dtoMovZoo.Validita_Fine);

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

        public async Task<bool> UpdateAsync(WriteMovimentiZoo dtoMovZoo, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrEmpty(dtoMovZoo.Piva))
                throw new Exception("Piva non valorizzato.");
            if (dtoMovZoo.Id_Agenda == 0)
                throw new Exception("Id_Agenda non valorizzato.");
            if (dtoMovZoo.Id_Mov == 0)
                throw new Exception("Id_Mov non valorizzato.");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", dtoMovZoo.Piva);
            expandoObj.TryAdd("@idAgenda", dtoMovZoo.Id_Agenda);
            expandoObj.TryAdd("@idMov", dtoMovZoo.Id_Mov);
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);

            var excludedProperties = new HashSet<string>
            {
                "Piva",
                "Sa_Cod",
                "Id_Agenda",
                "Id_Mov",
                "inviato",
                "DataInvio",
                "Validita_Inizio",
                "Validita_Fine",
                "Username_Creazione",
                "Username_Modifica",
                "Data_Creazione",
                "Data_Modifica"
            };

            var setClauses = new List<string>();
            foreach (var property in typeof(WriteMovimentiZoo).GetProperties())
            {
                if (excludedProperties.Contains(property.Name)) continue;

                var value = property.GetValue(dtoMovZoo);
                if (value != null)
                {
                    string paramName = "@" + property.Name;
                    setClauses.Add($"{property.Name} = {paramName}");
                    expandoObj.TryAdd(paramName, value);
                }
            }
            if (setClauses.Count == 0) return false;

            string updateQuery =
                $@"UPDATE Movimenti_Zoo SET
                    {string.Join(", ", setClauses)}
                    , Username_Modifica = @userOp
                    , Data_Modifica = GETDATE()
                WHERE Piva = @piva AND Id_Agenda = @idAgenda AND Id_Mov = @idMov ";

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

        public async Task<bool> DeleteAsync(string Piva, int Id_Agenda, int Id_Mov, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrEmpty(Piva))
                throw new Exception("Piva non valorizzato.");
            if (Id_Agenda == 0)
                throw new Exception("Id_Agenda non valorizzato.");
            if (Id_Mov == 0)
                throw new Exception("Id_Mov non valorizzato.");

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", Piva);
            expandoObj.TryAdd("@idAgenda", Id_Agenda);
            expandoObj.TryAdd("@idMov", Id_Mov);

            if (objParametriServer.FlagCancellazioneLogica == enumCancellazioneLogica.CancellazioneLogica)
            {
                stbQuery.AppendLine("UPDATE Movimenti_Zoo SET ")
                    .AppendLine("    Inviato = -1, ")
                    .AppendLine("    Username_Modifica = @userOp ")
                    .AppendLine("WHERE Inviato >= 0 ");
                expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            }
            else
            {
                stbQuery.AppendLine("DELETE FROM Movimenti_Zoo ")
                    .AppendLine("WHERE 1=1 ");
            }

            stbQuery.AppendLine("    AND Piva = @piva ")
                .AppendLine("    AND Id_Agenda = @idAgenda ")
                .AppendLine("    AND Id_Mov = @idMov ");

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

        public async Task<bool> ScriviModificaAsync(WriteMovimentiZoo dtoMovZoo, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                bool isNew = !await ExistAsync(dtoMovZoo.Piva, dtoMovZoo.Id_Agenda, dtoMovZoo.Id_Mov, objParametriServer);

                if (isNew)
                    await CreateAsync(dtoMovZoo, objParametriServer);
                else
                    await UpdateAsync(dtoMovZoo, objParametriServer);

                return true;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(WriteMovimentiZoo dtoMovZoo, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await DeleteAsync(dtoMovZoo.Piva, dtoMovZoo.Id_Agenda, dtoMovZoo.Id_Mov, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(List<WriteMovimentiZoo> MovZoo, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(objParametriServer.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var dto in MovZoo)
                        await DeleteAsync(dto.Piva, dto.Id_Agenda, dto.Id_Mov, objParametriServer);
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
    }
}
