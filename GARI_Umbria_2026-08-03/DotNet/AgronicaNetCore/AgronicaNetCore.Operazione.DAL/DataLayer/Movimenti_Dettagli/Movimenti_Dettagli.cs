using System.Text;
using System.Data;
using InData.Agenda;
using System.Dynamic;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Constants;
using static AgronicaNetCore.Base.Models.AgronicaCoreParametri;
using System.Transactions;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Operazione.DAL.Resources;
using AgronicaDataProvider6.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti_Dettagli
{
    public class MovimentiDettagli : BaseDALOperazione, IMovimenti_Dettagli
    {
        private readonly IAgro_Sequence _sequenceDal;

    public MovimentiDettagli(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider,localizer, securityBypass)
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

        private WriteMovDettagli Valorizza(WriteMovDettagli dtoMovDettagli)
        {
            DateTime adInizio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
            DateTime adFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);

            dtoMovDettagli.Sa_Cod ??= 0;
            dtoMovDettagli.Elem_Cod ??= 0;
            dtoMovDettagli.Pro_Cod ??= 0;
            dtoMovDettagli.Mat_Cod ??= 0;
            dtoMovDettagli.Mov_Det_Des ??= "";
            dtoMovDettagli.Udm_Cod ??= 0;
            dtoMovDettagli.Qta ??= 0;
            dtoMovDettagli.Cod_Iva ??= 0;
            dtoMovDettagli.Sconto ??= 0;
            dtoMovDettagli.Prezzo_Unitario ??= 0;
            dtoMovDettagli.Cod_Conto ??= 0;
            dtoMovDettagli.Cod_Progetto ??= 0;
            dtoMovDettagli.Fase_Cod ??= 0;
            dtoMovDettagli.Contabilizzato ??= 0;
            dtoMovDettagli.Pendente ??= 0;
            dtoMovDettagli.Cal_Cod ??= 0;
            dtoMovDettagli.Extra_Str ??= "";
            dtoMovDettagli.Extra_Int ??= 0;
            dtoMovDettagli.Extra_Date = SetIfNotInAgroDateRange(dtoMovDettagli.Extra_Date, adFine);
            dtoMovDettagli.Anno ??= 0;
            dtoMovDettagli.Ric_Cod ??= 0;
            dtoMovDettagli.Imponibile ??= 0;
            dtoMovDettagli.Iva ??= 0;
            dtoMovDettagli.Lotto ??= "";
            dtoMovDettagli.Jolly_Int ??= 0;
            dtoMovDettagli.Imponibile_Netto ??= 0;
            dtoMovDettagli.Prezzo_Unitario_Netto ??= 0;
            dtoMovDettagli.UDM_COD_EXTRA ??= 0;
            dtoMovDettagli.QTA_EXTRA ??= 0;
            dtoMovDettagli.Prezzo_Effettivo ??= 0;
            dtoMovDettagli.ChkIva_Manuale ??= 0;
            dtoMovDettagli.Cod_IvaIndetraibile ??= 0;
            dtoMovDettagli.Qta_Extra_Totale ??= 0;
            dtoMovDettagli.Tara ??= 0;
            dtoMovDettagli.ChkLayOut_Hide ??= 0;
            dtoMovDettagli.Variazione ??= 0;
            dtoMovDettagli.Listino_Cod ??= 0;
            dtoMovDettagli.Sconto_Listino ??= 0;
            dtoMovDettagli.Sconto_Modalita ??= 0;
            dtoMovDettagli.Mat_Cod_Alias ??= 0;
            dtoMovDettagli.Mezzo_Det ??= 0;
            dtoMovDettagli.Sconto_Testo ??= "";
            dtoMovDettagli.Ric_Cod_Pat ??= 0;
            dtoMovDettagli.Cod_Conto_Pat ??= 0;
            dtoMovDettagli.TempoCarenza ??= 0;
            dtoMovDettagli.DoseEtichetta ??= "";
            dtoMovDettagli.Turno_Cod ??= 0;
            dtoMovDettagli.ID_Attivita ??= 0;
            dtoMovDettagli.Dettaglio_VegCod ??= 0;
            dtoMovDettagli.Iva_Indetraibile ??= 0;
            dtoMovDettagli.Iva_Indetraibile_Perc ??= 0;
            dtoMovDettagli.PrincipiAttivi ??= "";
            dtoMovDettagli.ClassiTossicologiche ??= "";
            dtoMovDettagli.DoseEtichetta_Value ??= "";
            dtoMovDettagli.Iva_Deto_Cod ??= 0;
            dtoMovDettagli.Qta_Dettaglio1 ??= 0;
            dtoMovDettagli.Qta_Dettaglio2 ??= 0;
            dtoMovDettagli.Dettagli_Blocco_Flag ??= 0;
            dtoMovDettagli.Dettagli_Blocco_Username ??= "";
            dtoMovDettagli.Dettagli_Blocco_Data = SetIfNotInAgroDateRange(dtoMovDettagli.Extra_Date, adInizio);
            dtoMovDettagli.Qualifica_Cod ??= 0;
            dtoMovDettagli.Tariffa_Cod ??= 0;
            dtoMovDettagli.Ordine_Det ??= 0;
            dtoMovDettagli.Deroga_Cod ??= 0;
            dtoMovDettagli.Prezzo_Livello ??= 0;
            dtoMovDettagli.PrincipiAttiviPesi ??= "";
            dtoMovDettagli.Buffer ??= "";
            dtoMovDettagli.Rif_Esterno ??= "";
            dtoMovDettagli.Rif_Esterno_2 ??= "";
            dtoMovDettagli.PrincipiAttiviPercAbb ??= "";
            dtoMovDettagli.Polverulento ??= 0;
            dtoMovDettagli.Dettaglio_IdCod ??= 0;
            dtoMovDettagli.Dettaglio_GenCod ??= 0;
            dtoMovDettagli.Dettaglio_SpeCod ??= 0;
            dtoMovDettagli.Dettaglio_IProCod ??= 0;
            dtoMovDettagli.Id_Mov_Esterno ??= 0;
            dtoMovDettagli.RegSco_Numero ??= "";

            dtoMovDettagli.Inviato ??= 0;

            if (!dtoMovDettagli.Validita_Inizio.IsInRange(adInizio, adFine))
                dtoMovDettagli.Validita_Inizio = adInizio;
            if (!dtoMovDettagli.Validita_Fine.IsInRange(adInizio, adFine))
                dtoMovDettagli.Validita_Fine = adFine;

            return dtoMovDettagli;
        }

        public async Task<DataTable> ReadAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT * FROM Movimenti_dettagli ")
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
            if (Id_Mov_Det != 0)
            {
                sqlParams.TryAdd("@idMovDet", Id_Mov_Det);
                stbQuery.AppendLine("    AND Id_Mov_Det = @idMovDet ");
            }

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            sqlParams.TryAdd("@inizio", objParametriServer.FinestraTemporaleFine);
            sqlParams.TryAdd("@fine", objParametriServer.FinestraTemporaleInizio);
            stbQuery.AppendLine("    AND Validita_Inizio <= @inizio ")
                .AppendLine("    AND Validita_Fine >= @fine ");

            stbQuery.AppendLine("ORDER BY Id_Agenda, Id_Mov, Id_Mov_Det DESC ");

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

        public async Task<bool> ExistAsync(int Id_Mov_Det, AgronicaCoreParametriServer objParametriServer)
        {
            if (Id_Mov_Det == 0)
                throw new Exception("Id_Mov_Det non valorizzato.");

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@idMovDet", Id_Mov_Det);

            stbQuery.AppendLine("SELECT TOP(1) * FROM Movimenti_dettagli ")
                .AppendLine("WHERE Id_Mov_Det = @idMovDet ");

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

        public async Task<bool> CreateAsync(WriteMovDettagli dtoMovDettagli, AgronicaCoreParametriServer objParametriServer)
        {
            dtoMovDettagli = Valorizza(dtoMovDettagli);

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            HashSet<(string col, string pName, object? value)> sqlFields = new();

            sqlFields.Add(("PIVA", "@piva", dtoMovDettagli.Piva));
            sqlFields.Add(("Sa_Cod", "@saCod", dtoMovDettagli.Sa_Cod));
            sqlFields.Add(("Id_Agenda", "@idAgenda", dtoMovDettagli.Id_Agenda));
            sqlFields.Add(("Id_Mov", "@idMov", dtoMovDettagli.Id_Mov));
            sqlFields.Add(("Id_Mov_Det", "@idMovDet", dtoMovDettagli.Id_Mov_Det));
            sqlFields.Add(("Elem_Cod", "@elemCod", dtoMovDettagli.Elem_Cod));
            sqlFields.Add(("Pro_Cod", "@proCod", dtoMovDettagli.Pro_Cod));
            sqlFields.Add(("Mat_Cod", "@matCod", dtoMovDettagli.Mat_Cod));
            sqlFields.Add(("Mov_Det_Des", "@movDetDes", dtoMovDettagli.Mov_Det_Des));
            sqlFields.Add(("Udm_Cod", "@udmCod", dtoMovDettagli.Udm_Cod));
            sqlFields.Add(("Qta", "@qta", dtoMovDettagli.Qta));
            sqlFields.Add(("Cod_Iva", "@codIva", dtoMovDettagli.Cod_Iva));
            sqlFields.Add(("Sconto", "@sconto", dtoMovDettagli.Sconto));
            sqlFields.Add(("Prezzo_Unitario", "@prezzoUnit", dtoMovDettagli.Prezzo_Unitario));
            sqlFields.Add(("Cod_Conto", "@codConto", dtoMovDettagli.Cod_Conto));
            sqlFields.Add(("Cod_Progetto", "@codProg", dtoMovDettagli.Cod_Progetto));
            sqlFields.Add(("Fase_Cod", "@faseCod", dtoMovDettagli.Fase_Cod));
            sqlFields.Add(("Contabilizzato", "@contab", dtoMovDettagli.Contabilizzato));
            sqlFields.Add(("Pendente", "@pendente", dtoMovDettagli.Pendente));
            sqlFields.Add(("Extra_Str", "@extraStr", dtoMovDettagli.Extra_Str));
            sqlFields.Add(("Extra_Int", "@extraInt", dtoMovDettagli.Extra_Int));
            sqlFields.Add(("Extra_Date", "@extraDt", dtoMovDettagli.Extra_Date));
            sqlFields.Add(("Anno", "@anno", dtoMovDettagli.Anno));
            sqlFields.Add(("Ric_Cod", "@ricCod", dtoMovDettagli.Ric_Cod));
            sqlFields.Add(("Imponibile", "@imponib", dtoMovDettagli.Imponibile));
            sqlFields.Add(("Iva", "@iva", dtoMovDettagli.Iva));
            sqlFields.Add(("Lotto", "@lotto", dtoMovDettagli.Lotto));
            sqlFields.Add(("Jolly_Int", "@jollyInt", dtoMovDettagli.Jolly_Int));
            sqlFields.Add(("Imponibile_Netto", "@imponibNetto", dtoMovDettagli.Imponibile_Netto));
            sqlFields.Add(("Prezzo_Unitario_Netto", "@prezzoUnitNetto", dtoMovDettagli.Prezzo_Unitario_Netto));
            sqlFields.Add(("UDM_COD_EXTRA", "@udmCodEx", dtoMovDettagli.UDM_COD_EXTRA));
            sqlFields.Add(("QTA_EXTRA", "@qtaEx", dtoMovDettagli.QTA_EXTRA));
            sqlFields.Add(("Prezzo_Effettivo", "@prezzoEff", dtoMovDettagli.Prezzo_Effettivo));
            sqlFields.Add(("ChkIva_Manuale", "@chkIvaMan", dtoMovDettagli.ChkIva_Manuale));
            sqlFields.Add(("Cod_IvaIndetraibile", "@codIvaInd", dtoMovDettagli.Cod_IvaIndetraibile));
            sqlFields.Add(("Qta_Extra_Totale", "@qtaExTot", dtoMovDettagli.Qta_Extra_Totale));
            sqlFields.Add(("Tara", "@tara", dtoMovDettagli.Tara));
            sqlFields.Add(("ChkLayOut_Hide", "@chkHide", dtoMovDettagli.ChkLayOut_Hide));
            sqlFields.Add(("Variazione", "@variaz", dtoMovDettagli.Variazione));
            sqlFields.Add(("Listino_Cod", "@listinoCod", dtoMovDettagli.Listino_Cod));
            sqlFields.Add(("Sconto_Listino", "@scontoList", dtoMovDettagli.Sconto_Listino));
            sqlFields.Add(("Sconto_Modalita", "@scontoMod", dtoMovDettagli.Sconto_Modalita));
            sqlFields.Add(("Mat_Cod_Alias", "@matCodA", dtoMovDettagli.Mat_Cod_Alias));
            sqlFields.Add(("Mezzo_Det", "@mezzoDet", dtoMovDettagli.Mezzo_Det));
            sqlFields.Add(("Sconto_Testo", "@scontoTes", dtoMovDettagli.Sconto_Testo));
            sqlFields.Add(("Ric_Cod_Pat", "@ricCodPat", dtoMovDettagli.Ric_Cod_Pat));
            sqlFields.Add(("Cod_Conto_Pat", "@codContoPat", dtoMovDettagli.Cod_Conto_Pat));
            sqlFields.Add(("TempoCarenza", "@tempoCar", dtoMovDettagli.TempoCarenza));
            sqlFields.Add(("DoseEtichetta", "@doseEtic", dtoMovDettagli.DoseEtichetta));
            sqlFields.Add(("Turno_Cod", "@turnoCod", dtoMovDettagli.Turno_Cod));
            sqlFields.Add(("ID_Attivita", "@idAtt", dtoMovDettagli.ID_Attivita));
            sqlFields.Add(("Dettaglio_VegCod", "@dettVegCod", dtoMovDettagli.Dettaglio_VegCod));
            sqlFields.Add(("Iva_Indetraibile", "@ivaInd", dtoMovDettagli.Iva_Indetraibile));
            sqlFields.Add(("Iva_Indetraibile_Perc", "@ivaIndPerc", dtoMovDettagli.Iva_Indetraibile_Perc));
            sqlFields.Add(("PrincipiAttivi", "@princAtt", dtoMovDettagli.PrincipiAttivi));
            sqlFields.Add(("ClassiTossicologiche", "@classiToss", dtoMovDettagli.ClassiTossicologiche));
            sqlFields.Add(("DoseEtichetta_Value", "@doseEticV", dtoMovDettagli.DoseEtichetta_Value));
            sqlFields.Add(("Iva_Deto_Cod", "@ivaDetoCod", dtoMovDettagli.Iva_Deto_Cod));
            sqlFields.Add(("Qta_Dettaglio1", "@qtaDett1", dtoMovDettagli.Qta_Dettaglio1));
            sqlFields.Add(("Qta_Dettaglio2", "@qtaDett2", dtoMovDettagli.Qta_Dettaglio2));
            sqlFields.Add(("Dettagli_Blocco_Flag", "@dettBlFl", dtoMovDettagli.Dettagli_Blocco_Flag));
            sqlFields.Add(("Dettagli_Blocco_Username", "@dettBlUs", dtoMovDettagli.Dettagli_Blocco_Username));
            sqlFields.Add(("Dettagli_Blocco_Data", "@dettBlDt", dtoMovDettagli.Dettagli_Blocco_Data));
            sqlFields.Add(("Qualifica_Cod", "@qualCod", dtoMovDettagli.Qualifica_Cod));
            sqlFields.Add(("Tariffa_Cod", "@tarifCod", dtoMovDettagli.Tariffa_Cod));
            sqlFields.Add(("Ordine_Det", "@ordinDet", dtoMovDettagli.Ordine_Det));
            sqlFields.Add(("Deroga_Cod", "@derogaCod", dtoMovDettagli.Deroga_Cod));
            sqlFields.Add(("Prezzo_Livello", "@prezzoLvl", dtoMovDettagli.Prezzo_Livello));
            sqlFields.Add(("PrincipiAttiviPesi", "@prinAttP", dtoMovDettagli.PrincipiAttiviPesi));
            sqlFields.Add(("Buffer", "@buffer", dtoMovDettagli.Buffer));
            sqlFields.Add(("Rif_Esterno", "@rifEst", dtoMovDettagli.Rif_Esterno));
            sqlFields.Add(("Rif_Esterno_2", "@rifEst2", dtoMovDettagli.Rif_Esterno_2));
            sqlFields.Add(("PrincipiAttiviPercAbb", "@prinAttPA", dtoMovDettagli.PrincipiAttiviPercAbb));
            sqlFields.Add(("Polverulento", "@polv", dtoMovDettagli.Polverulento));
            sqlFields.Add(("Dettaglio_IdCod", "@dettIdCod", dtoMovDettagli.Dettaglio_IdCod));
            sqlFields.Add(("Dettaglio_GenCod", "@dettGenCod", dtoMovDettagli.Dettaglio_GenCod));
            sqlFields.Add(("Dettaglio_SpeCod", "@dettSpeCod", dtoMovDettagli.Dettaglio_SpeCod));
            sqlFields.Add(("Dettaglio_IProCod", "@dettIProCod", dtoMovDettagli.Dettaglio_IProCod));
            sqlFields.Add(("Id_Mov_Esterno", "@idMovEst", dtoMovDettagli.Id_Mov_Esterno));
            sqlFields.Add(("RegSco_Numero", "@regScoNum", dtoMovDettagli.RegSco_Numero));
            sqlFields.Add(("inviato", "@inviato", dtoMovDettagli.Inviato));
            sqlFields.Add(("datainvio", "@dtInvio", dtoMovDettagli.Data_Invio));
            sqlFields.Add(("Username_Creazione", "@userOp", objParametriServer.UsernameOperazione));
            sqlFields.Add(("Username_Modifica", "@userOp", objParametriServer.UsernameOperazione));
            sqlFields.Add(("Validita_Inizio", "@inizio", dtoMovDettagli.Validita_Inizio));
            sqlFields.Add(("Validita_Fine", "@fine", dtoMovDettagli.Validita_Fine));
            sqlFields.Add(("Data_Modifica", "@dataModifica", CostantiPersonalizzate.AGRODATAINIZIO_DATE));

            sqlFields = sqlFields.Where(x => x.value != null).ToHashSet();

            stbQuery.AppendLine("INSERT INTO Movimenti_dettagli ( ");
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

            //stbQuery.AppendLine("INSERT INTO Movimenti_dettagli (PIVA, Sa_Cod, Id_Agenda, Id_Mov, Id_Mov_Det, Elem_Cod, Pro_Cod, Mat_Cod, Mov_Det_Des, Udm_Cod, Qta, ")
            //    .AppendLine("    Cod_Iva, Sconto, Prezzo_Unitario, Cod_Conto, Cod_Progetto, Fase_Cod, Contabilizzato, Pendente, Cal_Cod, Extra_Str, Extra_Int, Extra_Date, ")
            //    .AppendLine("    Anno, Ric_Cod, Imponibile, Iva, Lotto, Jolly_Int, Imponibile_Netto, Prezzo_Unitario_Netto, UDM_COD_EXTRA, QTA_EXTRA, Prezzo_Effettivo, ")
            //    .AppendLine("    ChkIva_Manuale, Cod_IvaIndetraibile, Qta_Extra_Totale, Tara, ChkLayOut_Hide, Variazione, Listino_Cod, Sconto_Listino, Sconto_Modalita, ")
            //    .AppendLine("    Mat_Cod_Alias, Mezzo_Det, Sconto_Testo, Ric_Cod_Pat, Cod_Conto_Pat, TempoCarenza, DoseEtichetta, Turno_Cod, ID_Attivita, Dettaglio_VegCod, ")
            //    .AppendLine("    Iva_Indetraibile, Iva_Indetraibile_Perc, PrincipiAttivi, ClassiTossicologiche, DoseEtichetta_Value, Iva_Deto_Cod, Qta_Dettaglio1, Qta_Dettaglio2, ")
            //    .AppendLine("    Dettagli_Blocco_Flag, Dettagli_Blocco_Username, Dettagli_Blocco_Data, Qualifica_Cod, Tariffa_Cod, Ordine_Det, Deroga_Cod, Prezzo_Livello, ")
            //    .AppendLine("    PrincipiAttiviPesi, Buffer, Rif_Esterno, Rif_Esterno_2, PrincipiAttiviPercAbb, Polverulento, Dettaglio_IdCod, Dettaglio_GenCod, Dettaglio_SpeCod, ")
            //    .Append("    Dettaglio_IProCod, Id_Mov_Esterno, RegSco_Numero, ")
            //    .AppendLine("    inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ");
            //stbQuery.AppendLine("VALUES (@piva, @saCod, @idAgenda, @idMov, @idMovDet, @elemCod, @proCod, @matCod, @movDetDes, @udmCod, @qta, ")
            //    .AppendLine("    @codIva, @sconto, @prezzoUnit, @codConto, @codProg, @faseCod, @contab, @pendente, @extraStr, @extraInt, @extraDt, ")
            //    .AppendLine("    @anno, @ricCod, @imponib, @iva, @lotto, @jollyInt, @imponibNetto, @prezzoUnitNetto, @udmCodEx, @qtaEx, @prezzoEff, ")
            //    .AppendLine("    @chkIvaMan, @codIvaInd, @qtaExTot, @tara, @chkHide, @variaz, @listinoCod, @scontoList, @scontoMod, ")
            //    .AppendLine("    @matCodA, @mezzoDet, @scontoTes, @ricCodPat, @codContoPat, @tempoCar, @doseEtic, @turnoCod, @idAtt, @dettVegCod, ")
            //    .AppendLine("    @ivaInd, @ivaIndPerc, @princAtt, @classiToss, @doseEticV, @ivaDetoCod, @qtaDett1, @qtaDett2, ")
            //    .AppendLine("    @dettBlFl, @dettBlUs, @dettBlDt, @qualCod, @tarifCod, @ordiniDet, @derogaCod, @prezzoLvl, ")
            //    .AppendLine("    @prinAttP, @buffer, @rifEst, @rifEst2, prinAttPA, @polv, @dettIdCod, @dettGenCod, @dettSpeCod, @dettIProCod, @idMovEst, @regScoNum, ")
            //    .AppendLine("    @inviato, @dtInvio, GETDATE(), GETDATE(), @userOp, @userOp, @inizio, @fine) ");

            //var expandoObj = new ExpandoObject();
            //expandoObj.TryAdd("@piva", dtoMovDettagli.Piva);
            //expandoObj.TryAdd("@saCod", dtoMovDettagli.Sa_Cod);
            //expandoObj.TryAdd("@idAgenda", dtoMovDettagli.Id_Agenda);
            //expandoObj.TryAdd("@idMov", dtoMovDettagli.Id_Mov);
            //expandoObj.TryAdd("@idMovDet", dtoMovDettagli.Id_Mov_Det);
            //expandoObj.TryAdd("@elemCod", dtoMovDettagli.Elem_Cod);
            //expandoObj.TryAdd("@proCod", dtoMovDettagli.Pro_Cod);
            //expandoObj.TryAdd("@matCod", dtoMovDettagli.Mat_Cod);
            //expandoObj.TryAdd("@movDetDes", dtoMovDettagli.Mov_Det_Des);
            //expandoObj.TryAdd("@udmCod", dtoMovDettagli.Udm_Cod);
            //expandoObj.TryAdd("@qta", dtoMovDettagli.Qta);

            //expandoObj.TryAdd("@codIva", dtoMovDettagli.Cod_Iva);
            //expandoObj.TryAdd("@sconto", dtoMovDettagli.Sconto);
            //expandoObj.TryAdd("@prezzoUnit", dtoMovDettagli.Prezzo_Unitario);
            //expandoObj.TryAdd("@codConto", dtoMovDettagli.Cod_Conto);
            //expandoObj.TryAdd("@codProg", dtoMovDettagli.Cod_Progetto);
            //expandoObj.TryAdd("@faseCod", dtoMovDettagli.Fase_Cod);
            //expandoObj.TryAdd("@contab", dtoMovDettagli.Contabilizzato);
            //expandoObj.TryAdd("@pendente", dtoMovDettagli.Pendente);
            //expandoObj.TryAdd("@extraStr", dtoMovDettagli.Extra_Str);
            //expandoObj.TryAdd("@extraInt", dtoMovDettagli.Extra_Int);
            //expandoObj.TryAdd("@extraDt", dtoMovDettagli.Extra_Date);
            //expandoObj.TryAdd("@anno", dtoMovDettagli.Anno);
            //expandoObj.TryAdd("@ricCod", dtoMovDettagli.Ric_Cod);
            //expandoObj.TryAdd("@imponib", dtoMovDettagli.Imponibile);
            //expandoObj.TryAdd("@iva", dtoMovDettagli.Iva);
            //expandoObj.TryAdd("@lotto", dtoMovDettagli.Lotto);
            //expandoObj.TryAdd("@jollyInt", dtoMovDettagli.Jolly_Int);
            //expandoObj.TryAdd("@imponibNett", dtoMovDettagli.Imponibile_Netto);
            //expandoObj.TryAdd("@prezzoUnitNetto", dtoMovDettagli.Prezzo_Unitario_Netto);
            //expandoObj.TryAdd("@udmCodEx", dtoMovDettagli.UDM_COD_EXTRA);
            //expandoObj.TryAdd("@qtaEx", dtoMovDettagli.QTA_EXTRA);
            //expandoObj.TryAdd("@prezzoEff", dtoMovDettagli.Prezzo_Effettivo);
            //expandoObj.TryAdd("@chkIvaMan", dtoMovDettagli.ChkIva_Manuale);
            //expandoObj.TryAdd("@codIvaInd", dtoMovDettagli.Cod_IvaIndetraibile);
            //expandoObj.TryAdd("@qtaExTot", dtoMovDettagli.Qta_Extra_Totale);
            //expandoObj.TryAdd("@tara", dtoMovDettagli.Tara);
            //expandoObj.TryAdd("@chkHide", dtoMovDettagli.ChkLayOut_Hide);
            //expandoObj.TryAdd("@variaz", dtoMovDettagli.Variazione);
            //expandoObj.TryAdd("@listinoCod", dtoMovDettagli.Listino_Cod);
            //expandoObj.TryAdd("@scontoList", dtoMovDettagli.Sconto_Listino);
            //expandoObj.TryAdd("@scontoMod", dtoMovDettagli.Sconto_Modalita);
            //expandoObj.TryAdd("@matCodA", dtoMovDettagli.Mat_Cod_Alias);
            //expandoObj.TryAdd("@mezzoDet", dtoMovDettagli.Mezzo_Det);
            //expandoObj.TryAdd("@scontoTes", dtoMovDettagli.Sconto_Testo);
            //expandoObj.TryAdd("@ricCodPat", dtoMovDettagli.Ric_Cod_Pat);
            //expandoObj.TryAdd("@codContoPat", dtoMovDettagli.Cod_Conto_Pat);
            //expandoObj.TryAdd("@tempoCar", dtoMovDettagli.TempoCarenza);
            //expandoObj.TryAdd("@doseEtic", dtoMovDettagli.DoseEtichetta);
            //expandoObj.TryAdd("@turnoCod", dtoMovDettagli.Turno_Cod);
            //expandoObj.TryAdd("@idAtt", dtoMovDettagli.ID_Attivita);
            //expandoObj.TryAdd("@dettVegCod", dtoMovDettagli.Dettaglio_VegCod);
            //expandoObj.TryAdd("@ivaInd", dtoMovDettagli.Iva_Indetraibile);
            //expandoObj.TryAdd("@ivaIndPerc", dtoMovDettagli.Iva_Indetraibile_Perc);
            //expandoObj.TryAdd("@princAtt", dtoMovDettagli.PrincipiAttivi);
            //expandoObj.TryAdd("@classiToss", dtoMovDettagli.ClassiTossicologiche);
            //expandoObj.TryAdd("@doseEticV", dtoMovDettagli.DoseEtichetta_Value);
            //expandoObj.TryAdd("@ivaDetoCod", dtoMovDettagli.Iva_Deto_Cod);
            //expandoObj.TryAdd("@qtaDett1", dtoMovDettagli.Qta_Dettaglio1);
            //expandoObj.TryAdd("@qtaDett2", dtoMovDettagli.Qta_Dettaglio2);
            //expandoObj.TryAdd("@dettBlFl", dtoMovDettagli.Dettagli_Blocco_Flag);
            //expandoObj.TryAdd("@dettBlUs", dtoMovDettagli.Dettagli_Blocco_Username);
            //expandoObj.TryAdd("@dettBlDt", dtoMovDettagli.Dettagli_Blocco_Data);
            //expandoObj.TryAdd("@qualCod", dtoMovDettagli.Qualifica_Cod);
            //expandoObj.TryAdd("@tarifCod", dtoMovDettagli.Tariffa_Cod);
            //expandoObj.TryAdd("@ordinDet", dtoMovDettagli.Ordine_Det);
            //expandoObj.TryAdd("@derogaCod", dtoMovDettagli.Deroga_Cod);
            //expandoObj.TryAdd("@prezzoLvl", dtoMovDettagli.Prezzo_Livello);
            //expandoObj.TryAdd("@prinAttP", dtoMovDettagli.PrincipiAttiviPesi);
            //expandoObj.TryAdd("@buffer", dtoMovDettagli.Buffer);
            //expandoObj.TryAdd("@rifEst", dtoMovDettagli.Rif_Esterno);
            //expandoObj.TryAdd("@rifEst2", dtoMovDettagli.Rif_Esterno_2);
            //expandoObj.TryAdd("@prinAttPA", dtoMovDettagli.PrincipiAttiviPercAbb);
            //expandoObj.TryAdd("@polv", dtoMovDettagli.Polverulento);
            //expandoObj.TryAdd("@dettIdCod", dtoMovDettagli.Dettaglio_IdCod);
            //expandoObj.TryAdd("@dettGenCod", dtoMovDettagli.Dettaglio_GenCod);
            //expandoObj.TryAdd("@dettSpeCod", dtoMovDettagli.Dettaglio_SpeCod);
            //expandoObj.TryAdd("@dettIProCod", dtoMovDettagli.Dettaglio_IProCod);
            //expandoObj.TryAdd("@idMovEst", dtoMovDettagli.Id_Mov_Esterno);
            //expandoObj.TryAdd("@regScoNum", dtoMovDettagli.RegSco_Numero);

            //expandoObj.TryAdd("@inviato", dtoMovDettagli.Inviato);
            //expandoObj.TryAdd("@dtInvio", dtoMovDettagli.Data_Invio);
            //expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            //expandoObj.TryAdd("@inizio", dtoMovDettagli.Validita_Inizio);
            //expandoObj.TryAdd("@fine", dtoMovDettagli.Validita_Fine);

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

        public async Task<bool> UpdateAsync(WriteMovDettagli dtoMovDettagli, AgronicaCoreParametriServer objParametriServer)
        {
            if (dtoMovDettagli.Id_Mov_Det == 0)
                throw new Exception("Id_Mov_Det non valorizzato.");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", dtoMovDettagli.Piva);
            expandoObj.TryAdd("@idAgenda", dtoMovDettagli.Id_Agenda);
            expandoObj.TryAdd("@idMov", dtoMovDettagli.Id_Mov);
            expandoObj.TryAdd("@idMovDet", dtoMovDettagli.Id_Mov_Det);
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);

            var excludedProperties = new HashSet<string>
            {
                "Piva",
                "Sa_Cod",
                "Id_Agenda",
                "Id_Mov",
                "Id_Mov_Det",
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
            foreach (var property in typeof(WriteMovDettagli).GetProperties())
            {
                if (excludedProperties.Contains(property.Name)) continue;

                var value = property.GetValue(dtoMovDettagli);
                if (value != null)
                {
                    string paramName = "@" + property.Name;
                    setClauses.Add($"{property.Name} = {paramName}");
                    expandoObj.TryAdd(paramName, value);
                }
            }
            if (setClauses.Count == 0) return false;

            string updateQuery =
                $@"UPDATE Movimenti_dettagli SET
                    {string.Join(", ", setClauses)}
                    , Username_Modifica = @userOp
                    , Data_Modifica = GETDATE()
                WHERE Piva = @piva AND Id_Agenda = @idAgenda AND Id_Mov = @idMov AND Id_Mov_Det = @idMovDet ";

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

        public async Task<bool> DeleteAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, AgronicaCoreParametriServer objParametriServer)
        {
            if (Id_Mov_Det == 0)
                throw new Exception("Id_Mov_Det non valorizzato.");

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", Piva);
            expandoObj.TryAdd("@idAgenda", Id_Agenda);
            expandoObj.TryAdd("@idMov", Id_Mov);
            expandoObj.TryAdd("@idMovDet", Id_Mov_Det);

            if (objParametriServer.FlagCancellazioneLogica == enumCancellazioneLogica.CancellazioneLogica)
            {
                stbQuery.AppendLine("UPDATE Movimenti_dettagli SET ")
                    .AppendLine("    Inviato = -1, ")
                    .AppendLine("    Username_Modifica = @userOp ")
                    .AppendLine("WHERE Inviato >= 0 ");
                expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            }
            else
            {
                stbQuery.AppendLine("DELETE FROM Movimenti_dettagli ")
                    .AppendLine("WHERE 1=1 ");
            }

            stbQuery.AppendLine("    AND Piva = @piva ")
                .AppendLine("    AND Id_Agenda = @idAgenda ")
                .AppendLine("    AND Id_Mov = @idMov ")
                .AppendLine("    AND Id_Mov_Det = @idMovDet ");

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

        public async Task<int> ScriviModificaAsync(WriteMovDettagli dtoMovDettagli, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                bool isNew = false;

                if (dtoMovDettagli.Id_Mov_Det == 0)
                {
                    dtoMovDettagli.Id_Mov_Det = await _sequenceDal.NuovoId_TabellaAsync("movimenti_dettagli", 0, 2000000000, objParametriServer);
                    isNew = true;
                }
                else
                    isNew = !await ExistAsync(dtoMovDettagli.Id_Mov_Det, objParametriServer);

                if (isNew)
                    await CreateAsync(dtoMovDettagli, objParametriServer);
                else
                    await UpdateAsync(dtoMovDettagli, objParametriServer);

                return dtoMovDettagli.Id_Mov_Det;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(WriteMovDettagli dtoMovDettagli, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await DeleteAsync(dtoMovDettagli.Piva, dtoMovDettagli.Id_Agenda, dtoMovDettagli.Id_Mov, dtoMovDettagli.Id_Mov_Det, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(List<WriteMovDettagli> MovDettagli, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(objParametriServer.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var dto in MovDettagli)
                        await DeleteAsync(dto.Piva, dto.Id_Agenda, dto.Id_Mov, dto.Id_Mov_Det, objParametriServer);

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
