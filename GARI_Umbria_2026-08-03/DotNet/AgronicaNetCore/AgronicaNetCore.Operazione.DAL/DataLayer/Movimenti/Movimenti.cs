using System.Text;
using InData.Agenda;
using System.Dynamic;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Constants;
using static AgronicaNetCore.Base.Models.AgronicaCoreParametri;
using System.Data;
using System.Linq;
using System.Transactions;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Operazione.DAL.Resources;
using AgronicaDataProvider6.Extensions;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti
{
    public class Movimenti : BaseDALOperazione, IMovimenti
    {
        private readonly IAgro_Sequence _sequenceDal;

    public Movimenti(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider,localizer, securityBypass)
        {
            _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
        }

        private DateTime SetIfNotInAgroDateRange(DateTime? toCheck, DateTime elseValue)
        {
            return toCheck != null && ((DateTime)toCheck).IsInRange(
                    CostantiPersonalizzate.AGRODATAINIZIO_DATE, CostantiPersonalizzate.AGRODATAFINE_DATE) 
                ? (DateTime)toCheck
                : elseValue;
        }

        private WriteMovimenti Valorizza(WriteMovimenti dtoMovimenti)
        {
            DateTime adInizio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
            DateTime adFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);

            dtoMovimenti.Sa_Cod ??= 0;
            dtoMovimenti.Mov_Desc ??= "";
            dtoMovimenti.Cod_RisUm ??= 0;
            dtoMovimenti.Scadenza = SetIfNotInAgroDateRange(dtoMovimenti.Scadenza, adFine);
            dtoMovimenti.Doc_Numero ??= 0;
            dtoMovimenti.Num_Protocollo ??= 0;
            dtoMovimenti.Cod_IndirizzoRisUm ??= 0;
            dtoMovimenti.Cod_Destinazione ??= 0;
            dtoMovimenti.Cod_IndirizzoDestinazione ??= 0;
            dtoMovimenti.Mezzo ??= 0;
            dtoMovimenti.Cod_Vettore ??= 0;
            dtoMovimenti.Cod_IndirizzoVettore ??= 0;
            dtoMovimenti.Causale_Trasporto ??= 0;
            dtoMovimenti.Causale_Trasporto_Cod ??= 0;
            dtoMovimenti.Aspetto ??= "";
            dtoMovimenti.Peso ??= 0;
            dtoMovimenti.Ora = SetIfNotInAgroDateRange(dtoMovimenti.Ora, adInizio);
            dtoMovimenti.Colli ??= 0;
            dtoMovimenti.Extra_Str ??= "";
            dtoMovimenti.Extra_Int ??= 0;
            dtoMovimenti.Extra_Date = SetIfNotInAgroDateRange(dtoMovimenti.Extra_Date, adInizio);
            dtoMovimenti.Tipo_Sconto ??= 0;
            dtoMovimenti.Doc_Numero_Des ??= "";
            dtoMovimenti.Natura_Beni ??= "";
            dtoMovimenti.Tara_Veicolo ??= 0;
            dtoMovimenti.Tara_Imballi ??= 0;
            dtoMovimenti.Tipo_Peso ??= 0;
            dtoMovimenti.Modalita ??= 0;
            dtoMovimenti.Username_Note ??= "";
            dtoMovimenti.Scadenza_Extra = SetIfNotInAgroDateRange(dtoMovimenti.Scadenza_Extra, adInizio);
            dtoMovimenti.Doc_Numero_Sin ??= "";
            dtoMovimenti.Progr_Protocollo ??= 0;
            dtoMovimenti.Progr_Registrazione ??= 0;
            dtoMovimenti.Data_Registrazione = SetIfNotInAgroDateRange(dtoMovimenti.Data_Registrazione, adInizio);
            dtoMovimenti.ChkLayOut_Bypass_Fatturato ??= 0;
            dtoMovimenti.ChkLayOut_Join_Prodotti ??= 0;
            dtoMovimenti.Cod_RisUm_Altro ??= 0;
            dtoMovimenti.ChkLayOut_Peso ??= 0;
            dtoMovimenti.ChkLayOut_Prezzo ??= 0;
            dtoMovimenti.ChkFiltro_Varietale ??= 0;
            dtoMovimenti.Disciplinare_PubblicoPrivato ??= 0;
            dtoMovimenti.Sezionale_Cod ??= 0;
            dtoMovimenti.ChkLayOut_Litri ??= 0;
            dtoMovimenti.Cod_RisUm_Aggiuntivo ??= 0;
            dtoMovimenti.Cod_Indirizzo_Aggiuntivo ??= 0;
            dtoMovimenti.ChkLayOut_Riscontrato ??= 0;
            dtoMovimenti.Doc_Numero_Visualizzato ??= "";
            dtoMovimenti.TipoDocumento ??= 0;
            dtoMovimenti.Cod_Macchina_Lav ??= "";
            dtoMovimenti.OraFine = SetIfNotInAgroDateRange(dtoMovimenti.OraFine, adFine); ;
            dtoMovimenti.Modalita_Applicazione ??= 0;

            dtoMovimenti.Inviato ??= 0;

            if (!dtoMovimenti.Validita_Inizio.IsInRange(adInizio, adFine))
                dtoMovimenti.Validita_Inizio = adInizio;
            if (!dtoMovimenti.Validita_Fine.IsInRange(adInizio, adFine))
                dtoMovimenti.Validita_Fine = adFine;

            return dtoMovimenti;
        }

        public async Task<DataTable> ReadAsync(string Piva, int Id_Agenda, int Id_Mov, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            stbQuery.AppendLine("SELECT * FROM Movimenti ")
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

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            sqlParams.TryAdd("@inizio", objParametriServer.FinestraTemporaleFine);
            sqlParams.TryAdd("@fine", objParametriServer.FinestraTemporaleInizio);
            stbQuery.AppendLine("    AND Validita_Inizio <= @inizio ")
                .AppendLine("    AND Validita_Fine >= @fine ");

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

        public async Task<bool> ExistAsync(int Id_Mov, AgronicaCoreParametriServer objParametriServer)
        {
            if (Id_Mov == 0)
                throw new Exception("Id_Mov non valorizzato.");

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@idMov", Id_Mov);

            stbQuery.AppendLine("SELECT TOP(1) * FROM Movimenti ")
                .AppendLine("WHERE Id_Mov = @idMov ");

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

        public async Task<bool> CreateAsync(WriteMovimenti dtoMovimenti, AgronicaCoreParametriServer objParametriServer)
        {
            dtoMovimenti = Valorizza(dtoMovimenti);

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            HashSet<(string col, string pName, object? value)> sqlFields = new();

            sqlFields.Add(("PIVA", "@piva", dtoMovimenti.Piva));
            sqlFields.Add(("Sa_Cod", "@saCod", dtoMovimenti.Sa_Cod));
            sqlFields.Add(("Id_Agenda", "@idAgenda", dtoMovimenti.Id_Agenda));
            sqlFields.Add(("Id_Mov", "@idMov", dtoMovimenti.Id_Mov));
            sqlFields.Add(("Cod_RisUm", "@codRisUm", dtoMovimenti.Cod_RisUm));
            sqlFields.Add(("Cau_Mov", "@cauMov", dtoMovimenti.Cau_Mov));
            sqlFields.Add(("Mov_Desc", "@movDesc", dtoMovimenti.Mov_Desc));
            sqlFields.Add(("Data_Movimento", "@dtMov", dtoMovimenti.Data_Movimento));
            sqlFields.Add(("Scadenza", "@scadenza", dtoMovimenti.Scadenza));
            sqlFields.Add(("Doc_Numero", "@docNum", dtoMovimenti.Doc_Numero));
            sqlFields.Add(("Num_Protocollo", "@numProt", dtoMovimenti.Num_Protocollo));
            sqlFields.Add(("Cod_IndirizzoRisUm", "@codIndRisUm", dtoMovimenti.Cod_IndirizzoRisUm));
            sqlFields.Add(("Cod_Destinazione", "@codDest", dtoMovimenti.Cod_Destinazione));
            sqlFields.Add(("Cod_IndirizzoDestinazione", "@codIndDest", dtoMovimenti.Cod_IndirizzoDestinazione));
            sqlFields.Add(("Mezzo", "@mezzo", dtoMovimenti.Mezzo));
            sqlFields.Add(("Cod_Vettore", "@codVett", dtoMovimenti.Cod_Vettore));
            sqlFields.Add(("Cod_IndirizzoVettore", "@codIndVett", dtoMovimenti.Cod_IndirizzoVettore));
            sqlFields.Add(("Causale_Trasporto", "@cauTrasp", dtoMovimenti.Causale_Trasporto));
            sqlFields.Add(("Aspetto", "@aspetto", dtoMovimenti.Aspetto));
            sqlFields.Add(("Peso", "@peso", dtoMovimenti.Peso));
            sqlFields.Add(("Ora", "@ora", dtoMovimenti.Ora));
            sqlFields.Add(("Colli", "@colli", dtoMovimenti.Colli));
            sqlFields.Add(("Tipo_Sconto", "@tipoSconto", dtoMovimenti.Tipo_Sconto));
            sqlFields.Add(("Extra_Str", "@extraStr", dtoMovimenti.Extra_Str));
            sqlFields.Add(("Extra_Int", "@extraInt", dtoMovimenti.Extra_Int));
            sqlFields.Add(("Extra_Date", "@extraDate", dtoMovimenti.Extra_Date));
            sqlFields.Add(("Doc_Numero_Des", "@docNumDes", dtoMovimenti.Doc_Numero_Des));
            sqlFields.Add(("Natura_Beni", "@naturaBeni", dtoMovimenti.Natura_Beni));
            sqlFields.Add(("Tara_Veicolo", "@taraV", dtoMovimenti.Tara_Veicolo));
            sqlFields.Add(("Tara_Imballi", "@taraI", dtoMovimenti.Tara_Imballi));
            sqlFields.Add(("Tipo_Peso", "@tipoPeso", dtoMovimenti.Tipo_Peso));
            sqlFields.Add(("Modalita", "@modalita", dtoMovimenti.Modalita));
            sqlFields.Add(("Username_Note", "@userNote", dtoMovimenti.Username_Note));
            sqlFields.Add(("Scadenza_Extra", "@scadExtra", dtoMovimenti.Scadenza_Extra));
            sqlFields.Add(("Doc_Numero_Sin", "@docNumSin", dtoMovimenti.Doc_Numero_Sin));
            sqlFields.Add(("Progr_Protocollo", "@progrProt", dtoMovimenti.Progr_Protocollo));
            sqlFields.Add(("Progr_Registrazione", "@progrReg", dtoMovimenti.Progr_Registrazione));
            sqlFields.Add(("Data_Registrazione", "@dtRegist", dtoMovimenti.Data_Registrazione));
            sqlFields.Add(("ChkLayOut_Bypass_Fatturato", "@chkBypassFatt", dtoMovimenti.ChkLayOut_Bypass_Fatturato));
            sqlFields.Add(("ChkLayOut_Join_Prodotti", "@chkJoinProd", dtoMovimenti.ChkLayOut_Join_Prodotti));
            sqlFields.Add(("Cod_RisUm_Altro", "@codRisUmAltro", dtoMovimenti.Cod_RisUm_Altro));
            sqlFields.Add(("ChkLayOut_Peso", "@chkPeso", dtoMovimenti.ChkLayOut_Peso));
            sqlFields.Add(("ChkLayOut_Prezzo", "@chkPrezzo", dtoMovimenti.ChkLayOut_Prezzo));
            sqlFields.Add(("ChkFiltro_Varietale", "@chkVarietale", dtoMovimenti.ChkFiltro_Varietale));
            sqlFields.Add(("Disciplinare_PubblicoPrivato", "@discPP", dtoMovimenti.Disciplinare_PubblicoPrivato));
            sqlFields.Add(("Sezionale_Cod", "@sezCod", dtoMovimenti.Sezionale_Cod));
            sqlFields.Add(("Causale_Trasporto_Cod", "@cauTraspCod", dtoMovimenti.Causale_Trasporto_Cod));
            sqlFields.Add(("ChkLayOut_Litri", "@chkLitri", dtoMovimenti.ChkLayOut_Litri));
            sqlFields.Add(("Cod_RisUm_Aggiuntivo", "@codRisUmAgg", dtoMovimenti.Cod_RisUm_Aggiuntivo));
            sqlFields.Add(("Cod_Indirizzo_Aggiuntivo", "@codIndAgg", dtoMovimenti.Cod_Indirizzo_Aggiuntivo));
            sqlFields.Add(("ChkLayOut_Riscontrato", "@chkRisc", dtoMovimenti.ChkLayOut_Riscontrato));
            sqlFields.Add(("Doc_Numero_Visualizzato", "@docNumVis", dtoMovimenti.Doc_Numero_Visualizzato));
            sqlFields.Add(("Cod_Macchina_Lav", "@codMaccLav", dtoMovimenti.Cod_Macchina_Lav));
            sqlFields.Add(("TipoDocumento", "@tipoDoc", dtoMovimenti.TipoDocumento));
            sqlFields.Add(("OraFine", "@oraFine", dtoMovimenti.OraFine));
            sqlFields.Add(("Modalita_Applicazione", "@modApp", dtoMovimenti.Modalita_Applicazione));
            sqlFields.Add(("inviato", "@inviato", dtoMovimenti.Inviato));
            sqlFields.Add(("datainvio", "@datainvio", dtoMovimenti.Data_Invio));
            sqlFields.Add(("inviato", "@inviato", dtoMovimenti.Inviato));
            sqlFields.Add(("Validita_Inizio", "@inizio", dtoMovimenti.Validita_Inizio));
            sqlFields.Add(("Validita_Fine", "@fine", dtoMovimenti.Validita_Fine));
            sqlFields.Add(("Data_Modifica", "@dtModifica", CostantiPersonalizzate.AGRODATAINIZIO_DATE));
            sqlFields.Add(("Username_Creazione", "@userOp", objParametriServer.UsernameOperazione));
            sqlFields.Add(("Username_Modifica", "@userOp", objParametriServer.UsernameOperazione));

            sqlFields = sqlFields.Where(x => x.value != null).ToHashSet();

            stbQuery.AppendLine("INSERT INTO Movimenti ( ");
            sqlFields.Select((x, i) => i == sqlFields.Count - 1 ? x.col : x.col + ",")
                .Chunk(5)
                .Select(cols => cols.Aggregate((a, b) => a + " " + b))
                .ToList().ForEach(cols => stbQuery.AppendLine(cols));
            stbQuery.AppendLine(" , Data_Creazione ");
            stbQuery.AppendLine(") ");
            stbQuery.AppendLine("VALUES ( ");
            sqlFields.Select((x, i) => i == sqlFields.Count - 1 ? x.pName : x.pName + ",")
                .Chunk(5)
                .Select(cols => cols.Aggregate((a, b) => a + " " + b))
                .ToList().ForEach(cols => stbQuery.AppendLine(cols));
            stbQuery.AppendLine(" , GETDATE() ");
            stbQuery.AppendLine(") ");

            sqlFields.ToList().ForEach(x => expandoObj.TryAdd(x.pName, x.value));

            //stbQuery.AppendLine("INSERT INTO Movimenti (PIVA, Sa_Cod, Id_Agenda, Id_Mov, Cod_RisUm, Cau_Mov, Mov_Desc, Data_Movimento, Scadenza, ")
            //    .AppendLine("    Doc_Numero, Num_Protocollo, Cod_IndirizzoRisUm, Cod_Destinazione, Cod_IndirizzoDestinazione, Mezzo, Cod_Vettore, Cod_IndirizzoVettore, ")
            //    .AppendLine("    Causale_Trasporto, Aspetto, Peso, Ora, Colli, Extra_Str, Extra_Int, Extra_Date, Tipo_Sconto, Doc_Numero_Des, Natura_Beni, ")
            //    .AppendLine("    Tara_Veicolo, Tara_Imballi, Tipo_Peso, Modalita, Username_Note, Scadenza_Extra, Doc_Numero_Sin, Progr_Protocollo, Progr_Registrazione, ")
            //    .AppendLine("    Data_Registrazione, ChkLayOut_Bypass_Fatturato, ChkLayOut_Join_Prodotti, Cod_RisUm_Altro, ChkLayOut_Peso, ChkLayOut_Prezzo, ")
            //    .AppendLine("    ChkFiltro_Varietale, Disciplinare_PubblicoPrivato, Sezionale_Cod, Causale_Trasporto_Cod, ChkLayOut_Litri, Cod_RisUm_Aggiuntivo,  ")
            //    .AppendLine("    Cod_Indirizzo_Aggiuntivo, ChkLayOut_Riscontrato, Doc_Numero_Visualizzato, TipoDocumento, Cod_Macchina_Lav, OraFine, Modalita_Applicazione, ")
            //    .AppendLine("    inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ");
            //stbQuery.AppendLine("VALUES (@piva, @saCod, @idAgenda, @idMov, @codRisUm, @cauMov, @movDesc, @dtMov, @scadenza, ")
            //    .AppendLine("    @docNum, @numProt, @codIndRisUm, @codDest, @codIndDest, @mezzo, @codVett, @codIndVett, ")
            //    .AppendLine("    @taraV, @taraI, @tipoPeso, @modalita, @userNote, @scadExtra, @docNumSin, @progrProt, @progrReg, ")
            //    .AppendLine("    @dtRegist, @chkBypassFatt, @chkJoinProd, @codRisUmAltro, @chkPeso, @chkPrezzo, ")
            //    .AppendLine("    @chkVarietale, @discPP, @sezCod, @cauTraspCod, @chkLitri, @codRisUmAgg, ")
            //    .AppendLine("    @codIndAgg, @chkRisc, @docNumVis, @tipoDoc, @codMaccLav, @oraFine, @modApp, ")
            //    .AppendLine("    @inviato, @dtInvio, GETDATE(), GETDATE(), @userOp, @userOp, @inizio, @fine) ");

            //expandoObj.TryAdd("@piva", dtoMovimenti.Piva);
            //expandoObj.TryAdd("@saCod", dtoMovimenti.Sa_Cod);
            //expandoObj.TryAdd("@idAgenda", dtoMovimenti.Id_Agenda);
            //expandoObj.TryAdd("@idMov", dtoMovimenti.Id_Mov);
            //expandoObj.TryAdd("@codRisUm", dtoMovimenti.Cod_RisUm);
            //expandoObj.TryAdd("@cauMov", dtoMovimenti.Cau_Mov);
            //expandoObj.TryAdd("@movDesc", dtoMovimenti.Mov_Desc);
            //expandoObj.TryAdd("@dtMov", dtoMovimenti.Data_Movimento);
            //expandoObj.TryAdd("@scadenza", dtoMovimenti.Scadenza);

            //expandoObj.TryAdd("@docNum", dtoMovimenti.Doc_Numero);
            //expandoObj.TryAdd("@numProt", dtoMovimenti.Num_Protocollo);
            //expandoObj.TryAdd("@codIndRisUm", dtoMovimenti.Cod_IndirizzoRisUm);
            //expandoObj.TryAdd("@codDest", dtoMovimenti.Cod_Destinazione);
            //expandoObj.TryAdd("@codIndDest", dtoMovimenti.Cod_IndirizzoDestinazione);
            //expandoObj.TryAdd("@mezzo", dtoMovimenti.Mezzo);
            //expandoObj.TryAdd("@codVett", dtoMovimenti.Cod_Vettore);
            //expandoObj.TryAdd("@codIndVett", dtoMovimenti.Cod_IndirizzoVettore);
            //expandoObj.TryAdd("@taraV", dtoMovimenti.Tara_Veicolo);
            //expandoObj.TryAdd("@taraI", dtoMovimenti.Tara_Imballi);
            //expandoObj.TryAdd("@tipoPeso", dtoMovimenti.Tipo_Peso);
            //expandoObj.TryAdd("@modalita", dtoMovimenti.Modalita);
            //expandoObj.TryAdd("@userNote", dtoMovimenti.Username_Note);
            //expandoObj.TryAdd("@scadExtra", dtoMovimenti.Scadenza_Extra);
            //expandoObj.TryAdd("@docNumSin", dtoMovimenti.Doc_Numero_Sin);
            //expandoObj.TryAdd("@progrProt", dtoMovimenti.Progr_Protocollo);
            //expandoObj.TryAdd("@progrReg", dtoMovimenti.Progr_Registrazione);
            //expandoObj.TryAdd("@dtRegist", dtoMovimenti.Data_Registrazione);
            //expandoObj.TryAdd("@chkBypassFatt", dtoMovimenti.ChkLayOut_Bypass_Fatturato);
            //expandoObj.TryAdd("@chkJoinProd", dtoMovimenti.ChkLayOut_Join_Prodotti);
            //expandoObj.TryAdd("@codRisUmAltro", dtoMovimenti.Cod_RisUm_Altro);
            //expandoObj.TryAdd("@chkPeso", dtoMovimenti.ChkLayOut_Peso);
            //expandoObj.TryAdd("@chkPrezzo", dtoMovimenti.ChkLayOut_Prezzo);
            //expandoObj.TryAdd("@chkVarietale", dtoMovimenti.ChkFiltro_Varietale);
            //expandoObj.TryAdd("@discPP", dtoMovimenti.Disciplinare_PubblicoPrivato);
            //expandoObj.TryAdd("@sezCod", dtoMovimenti.Sezionale_Cod);
            //expandoObj.TryAdd("@cauTraspCod", dtoMovimenti.Causale_Trasporto_Cod);
            //expandoObj.TryAdd("@chkLitri", dtoMovimenti.ChkLayOut_Litri);
            //expandoObj.TryAdd("@codRisUmAgg", dtoMovimenti.Cod_RisUm_Aggiuntivo);
            //expandoObj.TryAdd("@codIndAgg", dtoMovimenti.Cod_Indirizzo_Aggiuntivo);
            //expandoObj.TryAdd("@chkRisc", dtoMovimenti.ChkLayOut_Riscontrato);
            //expandoObj.TryAdd("@docNumVis", dtoMovimenti.Doc_Numero_Visualizzato);
            //expandoObj.TryAdd("@tipoDoc", dtoMovimenti.TipoDocumento);
            //expandoObj.TryAdd("@codMaccLav", dtoMovimenti.Cod_Macchina_Lav);
            //expandoObj.TryAdd("@oraFine", dtoMovimenti.OraFine);
            //expandoObj.TryAdd("@modApp", dtoMovimenti.Modalita_Applicazione);

            //expandoObj.TryAdd("@inviato", dtoMovimenti.Inviato);
            //expandoObj.TryAdd("@dtInvio", dtoMovimenti.Data_Invio);
            //expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            //expandoObj.TryAdd("@inizio", dtoMovimenti.Validita_Inizio);
            //expandoObj.TryAdd("@fine", dtoMovimenti.Validita_Fine);

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

        public async Task<bool> UpdateAsync(WriteMovimenti dtoMovimenti, AgronicaCoreParametriServer objParametriServer)
        {
            if (dtoMovimenti.Id_Mov == 0)
                throw new Exception("Id_Mov non valorizzato.");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", dtoMovimenti.Piva);
            expandoObj.TryAdd("@idAgenda", dtoMovimenti.Id_Agenda);
            expandoObj.TryAdd("@idMov", dtoMovimenti.Id_Mov);
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);

            var excludedProperties = new HashSet<string>
            {
                "Piva",
                "Sa_Cod",
                "Id_Agenda",
                "Id_Mov",
                "inviato",
                "DataInvio",
                "Username_Creazione",
                "Username_Modifica",
                "Data_Creazione",
                "Data_Modifica"
            };

            var setClauses = new List<string>();
            foreach (var property in typeof(WriteMovimenti).GetProperties())
            {
                if (excludedProperties.Contains(property.Name)) continue;

                var value = property.GetValue(dtoMovimenti);
                if (value != null)
                {
                    string paramName = "@" + property.Name;
                    setClauses.Add($"{property.Name} = {paramName}");
                    expandoObj.TryAdd(paramName, value);
                }
            }
            if (setClauses.Count == 0) return false;

            string updateQuery =
                $@"UPDATE Movimenti SET
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
            if (Id_Mov == 0)
                throw new Exception("Id_Mov non valorizzato.");

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", Piva);
            expandoObj.TryAdd("@idAgenda", Id_Agenda);
            expandoObj.TryAdd("@idMov", Id_Mov);

            if (objParametriServer.FlagCancellazioneLogica == enumCancellazioneLogica.CancellazioneLogica)
            {
                stbQuery.AppendLine("UPDATE Movimenti SET ")
                    .AppendLine("    Inviato = -1, ")
                    .AppendLine("    Username_Modifica = @userOp ")
                    .AppendLine("WHERE Inviato >= 0 ");
                expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            }
            else
            {
                stbQuery.AppendLine("DELETE FROM Movimenti ")
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

        public async Task<int> ScriviModificaAsync(WriteMovimenti dtoMovimenti, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                bool isNew = false;

                if (dtoMovimenti.Id_Mov == 0)
                {
                    dtoMovimenti.Id_Mov = await _sequenceDal.NuovoId_TabellaAsync("movimenti", 0, 2000000000, objParametriServer);
                    isNew = true;
                }
                else
                    isNew = !await ExistAsync(dtoMovimenti.Id_Mov, objParametriServer);

                if (isNew)
                    await CreateAsync(dtoMovimenti, objParametriServer);
                else
                    await UpdateAsync(dtoMovimenti, objParametriServer);

                return dtoMovimenti.Id_Mov;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(WriteMovimenti dtoMovimenti, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await DeleteAsync(dtoMovimenti.Piva, dtoMovimenti.Id_Agenda, dtoMovimenti.Id_Mov, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(List<WriteMovimenti> Movimenti, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(objParametriServer.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var dto in Movimenti)
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
