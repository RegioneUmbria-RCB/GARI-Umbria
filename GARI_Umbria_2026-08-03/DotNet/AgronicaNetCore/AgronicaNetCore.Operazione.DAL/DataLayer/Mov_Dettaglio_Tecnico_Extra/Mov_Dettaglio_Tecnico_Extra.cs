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
using Microsoft.Extensions.DependencyInjection;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Dettaglio_Tecnico_Extra
{
    public class Mov_Dettaglio_Tecnico_Extra : BaseDALOperazione, IMov_Dettaglio_Tecnico_Extra
    {
        private readonly IAgro_Sequence _sequenceDal;

    public Mov_Dettaglio_Tecnico_Extra(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider,localizer, securityBypass)
        {
            _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
        }

        private WriteMovDettTecnicoExtra Valorizza(WriteMovDettTecnicoExtra dtoMovDettTecnEx)
        {
            DateTime adInizio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
            DateTime adFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);

            dtoMovDettTecnEx.Sa_Cod ??= 0;
            dtoMovDettTecnEx.Regione ??= "";
            dtoMovDettTecnEx.Asl ??= "";
            dtoMovDettTecnEx.Serie ??= "";
            dtoMovDettTecnEx.Numero ??= "";
            dtoMovDettTecnEx.Mac_Cod ??= 0;
            dtoMovDettTecnEx.Cod_RisUm ??= 0;
            dtoMovDettTecnEx.Trasportatore ??= "";
            dtoMovDettTecnEx.Mezzo_Trasporto ??= "";
            dtoMovDettTecnEx.Targa ??= "";
            dtoMovDettTecnEx.N_Immatricolazione ??= "";
            dtoMovDettTecnEx.N_Immatricolazione_Rimorchio ??= "";
            dtoMovDettTecnEx.N_Autorizzazione_Trasporto ??= "";
            dtoMovDettTecnEx.Data_Rilascio_Autorizzazione ??= adInizio;
            dtoMovDettTecnEx.Peso ??= 0;
            dtoMovDettTecnEx.Codice_Prodotto ??= 0;
            dtoMovDettTecnEx.Colore ??= 0;
            dtoMovDettTecnEx.Zona_Viticola ??= "";
            dtoMovDettTecnEx.Manipolazioni ??= 0;
            dtoMovDettTecnEx.Precisazioni ??= "";
            dtoMovDettTecnEx.Annotazioni ??= "";
            dtoMovDettTecnEx.Num_Contenitori ??= 0;
            dtoMovDettTecnEx.Marche_Contenitori ??= "";
            dtoMovDettTecnEx.Des_Contenitori ??= "";
            dtoMovDettTecnEx.Tipo_Documento ??= "";
            dtoMovDettTecnEx.Id_Cod_Autorita ??= 0;
            dtoMovDettTecnEx.Luogo_Partenza ??= "";
            dtoMovDettTecnEx.Luogo_Consegna ??= "";
            dtoMovDettTecnEx.Data_Spedizione ??= adInizio;
            dtoMovDettTecnEx.Indicazioni_Complementari ??= "";
            dtoMovDettTecnEx.Titolo_Alcol ??= 0;
            dtoMovDettTecnEx.Codice_NC ??= "";
            dtoMovDettTecnEx.Num_Riferimento ??= "";
            dtoMovDettTecnEx.Data_Dichiarazione ??= adInizio;
            dtoMovDettTecnEx.Garanzia ??= "";
            dtoMovDettTecnEx.Certificati ??= "";
            dtoMovDettTecnEx.Durata_Viaggio ??= "";
            dtoMovDettTecnEx.Peso_Lordo ??= 0;
            dtoMovDettTecnEx.Num_Colli ??= 0;
            dtoMovDettTecnEx.Contenitore_Cod ??= 0;
            dtoMovDettTecnEx.Imballaggio_Cod ??= 0;
            dtoMovDettTecnEx.Agente_Cod ??= 0;
            dtoMovDettTecnEx.Provvigione ??= 0;
            dtoMovDettTecnEx.Tipo_Trasporto ??= 0;
            dtoMovDettTecnEx.Unita_Trasporto ??= 0;
            dtoMovDettTecnEx.Codice_Alternativo ??= "";
            dtoMovDettTecnEx.Id_Gestione_Vettore ??= 0;
            dtoMovDettTecnEx.Ritenuta_Acconto_Cod ??= 0;
            dtoMovDettTecnEx.Ritenuta_Acconto ??= 0;
            dtoMovDettTecnEx.Enasarco_Cod ??= 0;
            dtoMovDettTecnEx.Enasarco ??= 0;
            dtoMovDettTecnEx.ACCDAA_Cod_RisUm_Destinatario ??= 0;
            dtoMovDettTecnEx.ACCDAA_Cod_RisUm_Destinazione ??= 0;
            dtoMovDettTecnEx.ACCDAA_Cod_IndirizzoRisUm_Destinatario ??= 0;
            dtoMovDettTecnEx.ACCDAA_Cod_IndirizzoRisUm_Destinazione ??= 0;
            dtoMovDettTecnEx.CapoArea_Cod ??= 0;
            dtoMovDettTecnEx.Provvigione_CapoArea ??= 0;
            dtoMovDettTecnEx.Provvigione_Pagata_Agente ??= 0;
            dtoMovDettTecnEx.Provvigione_Pagata_CapoArea ??= 0;
            dtoMovDettTecnEx.N_Doc_Cliente ??= "";
            dtoMovDettTecnEx.Data_Doc_Cliente ??= adInizio;
            dtoMovDettTecnEx.N_Nota_Fattura ??= "";
            dtoMovDettTecnEx.Data_Nota_Fattura ??= adInizio;
            dtoMovDettTecnEx.N_Nota_DDT ??= "";
            dtoMovDettTecnEx.N_Nota_Riga_DDT ??= "";
            dtoMovDettTecnEx.Data_Nota_DDT ??= adInizio;
            dtoMovDettTecnEx.Causale_Fattura ??= 0;
            dtoMovDettTecnEx.N_Doc_Ente ??= "";
            dtoMovDettTecnEx.Anno_Doc_Ente ??= 0;
            dtoMovDettTecnEx.Num_Conf_Riscontrate ??= 0;
            dtoMovDettTecnEx.Num_Colli_Riscontrati ??= 0;
            dtoMovDettTecnEx.Num_Imballi_Riscontrati ??= 0;
            dtoMovDettTecnEx.Peso_Netto_Riscontrato ??= 0;
            dtoMovDettTecnEx.Peso_Lordo_Riscontrato ??= 0;
            dtoMovDettTecnEx.Tara_Unit_Conf_Riscontrata ??= 0;
            dtoMovDettTecnEx.Tara_Unit_Collo_Riscontrata ??= 0;
            dtoMovDettTecnEx.Tara_Unit_Imballo_Riscontrata ??= 0;
            dtoMovDettTecnEx.Distanza_Trasporto_Udm ??= 0;
            dtoMovDettTecnEx.Distanza_Trasporto ??= 0;

            dtoMovDettTecnEx.Inviato ??= 0;

            if (dtoMovDettTecnEx.Validita_Inizio < adInizio)
                dtoMovDettTecnEx.Validita_Inizio = adInizio;
            if (dtoMovDettTecnEx.Validita_Fine > adFine)
                dtoMovDettTecnEx.Validita_Fine = adFine;

            return dtoMovDettTecnEx;
        }

        public async Task<DataTable> ReadAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Reg_Det, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT * FROM Mov_Dettaglio_Tecnico_Extra ")
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
            if (Id_Reg_Det != 0)
            {
                sqlParams.TryAdd("@idRegDet", Id_Reg_Det);
                stbQuery.AppendLine("    AND Id_Reg_Dettaglio = @idRegDet ");
            }
            sqlParams.TryAdd("@inizio", objParametriServer.FinestraTemporaleFine);
            sqlParams.TryAdd("@fine", objParametriServer.FinestraTemporaleInizio);
            stbQuery.AppendLine("    AND Validita_Inizio <= @inizio ")
                .AppendLine("    AND Validita_Fine >= @fine ");

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            stbQuery.AppendLine("ORDER BY Id_Agenda, Id_Mov, Id_Mov_Det, Id_Reg_Dettaglio DESC ");

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

        public async Task<bool> ExistAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Reg_Det, AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrEmpty(Piva))
                throw new Exception("Piva non valorizzata.");
            if (Id_Agenda == 0)
                throw new Exception("Id_Agenda non valorizzato.");
            if (Id_Mov == 0)
                throw new Exception("Id_Mov non valorizzato.");
            if (Id_Mov_Det == 0)
                throw new Exception("Id_Mov_Det non valorizzato.");
            if (Id_Reg_Det == 0)
                throw new Exception("Id_Reg_Dettaglio non valorizzato.");

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@piva", Piva);
            sqlParams.TryAdd("@idAgenda", Id_Agenda);
            sqlParams.TryAdd("@idMov", Id_Mov);
            sqlParams.TryAdd("@idMovDet", Id_Mov_Det);
            sqlParams.TryAdd("@idRegDet", Id_Reg_Det);

            stbQuery.AppendLine("SELECT TOP(1) * FROM Mov_Dettaglio_Tecnico_Extra ")
                .AppendLine("WHERE 1=1 ")
                .AppendLine("    AND Piva = @piva ")
                .AppendLine("    AND Id_Agenda = @idAgenda ")
                .AppendLine("    AND Id_Mov = @idMov ")
                .AppendLine("    AND Id_Mov_Det = @idMovDet ")
                .AppendLine("    AND Id_Reg_Dettaglio = @idRegDet ");

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

        public async Task<bool> CreateAsync(WriteMovDettTecnicoExtra dtoMovDettTecnEx, AgronicaCoreParametriServer objParametriServer)
        {
            dtoMovDettTecnEx = Valorizza(dtoMovDettTecnEx);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Mov_Dettaglio_Tecnico_Extra (Piva, Sa_Cod, Id_Agenda, Id_Mov, Id_Mov_Det, Id_Reg_Dettaglio, Regione, ASL, Serie, Numero, Mac_Cod, ")
                .AppendLine("    Cod_RisUm, Trasportatore, Mezzo_Trasporto, Targa, N_Immatricolazione, N_Immatricolazione_Rimorchio, N_Autorizzazione_Trasporto, Data_Rilascio_Autorizzazione, ")
                .AppendLine("    Peso, Codice_Prodotto, Colore, Zona_Viticola, Manipolazioni, Precisazioni, Annotazioni, Num_Contenitori, Marche_Contenitori, Des_Contenitori, ")
                .AppendLine("    Tipo_Documento, Id_Cod_Autorita, Luogo_Partenza, Luogo_Consegna, Data_Spedizione, Indicazioni_Complementari, Titolo_Alcol, Codice_NC, Num_Riferimento, ")
                .AppendLine("    Num_Riferimento, Data_Dichiarazione, Garanzia, Certificati, Durata_Viaggio, Peso_Lordo, Num_Colli, Contenitore_Cod, Imballaggio_Cod, Agente_Cod, ")
                .AppendLine("    Provvigione, Tipo_Trasporto, Unita_Trasporto, Codice_Alternativo, Id_Gestione_Vettore, Ritenuta_Acconto_Cod, Ritenuta_Acconto, Enasarco_Cod, Enasarco, ")
                .AppendLine("    ACCDAA_Cod_Risum_Destinatario, ACCDAA_Cod_Risum_Destinazione, ACCDAA_Cod_IndirizzoRisum_Destinatario, ACCDAA_Cod_IndirizzoRisum_Destinazione, ")
                .AppendLine("    CapoArea_Cod, Provvigione_CapoArea, Provvigione_Pagata_Agente, Provvigione_Pagata_CapoArea, N_Doc_Cliente, Data_Doc_Cliente, N_Nota_Fattura, Data_Nota_Fattura, ")
                .AppendLine("    N_Nota_DDT, N_Nota_Riga_DDT, Data_Nota_DDT, Causale_Fattura, N_Doc_Ente, Anno_Doc_Ente, Num_Conf_Riscontrate, Num_Colli_Riscontrati, Num_Imballi_Riscontrati, ")
                .AppendLine("    Peso_Netto_Riscontrato, Peso_Lordo_Riscontrato, Tara_Unit_Conf_Riscontrata, Tra_Unit_Collo_Riscontrata, Tara_Unit_Imballo_Riscontrata, Distanza_Trasporto_Udm, Distanza_Trasporto, ")
                .AppendLine("    inviato, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ");
            stbQuery.AppendLine("VALUES (@piva, @saCod, @idAgenda, @idMov, @idMovDet, @idRegDet, @regione, @asl, @serie, @numero, @macCod, ")
                .AppendLine("    @codRisUm, @trasp, @mezzoTrasp, @targa, @nImmat, @nImmatRim, @nAutoTrasp, @dataRilAuto, ")
                .AppendLine("    @peso, @codProd, @colore, @zonaVit, @manipol, @precis, @annot, @numCont, @marcheCont, @desCont, ")
                .AppendLine("    @tipoDoc, @idCodAut, @luogoPart, @luogoCons, @dataSped, @indicCompl, @titoloAlc, @codNC, ")
                .AppendLine("    @numRif, @dataDich, @garanzia, @certif, @durataV, @pesoLordo, @numColli, @contenCod, @imballCod, @agenteCod, ")
                .AppendLine("    @provv, @tipoTrasp, @unitTrasp, @codAlt, @idGestVet, @ritenAccCod, @ritenAcc, @enasCod, @enas, ")
                .AppendLine("    @codRUDestio, @codRUDestone, @codIndRUDestio, @codIndRUDestone, ")
                .AppendLine("    @capoACod, @provvCapoA, @provvPagAg, @provvPagCapoA, @nDocCl, @dataDocCl, @nNotaFatt, @dataNotaFatt, ")
                .AppendLine("    @nNotaDdt, @nNotaRDdt, @dataNotaDdt, @causFatt, @nDocEnte, @annoDocEnte, @numConfRis, @numColliRis, @numImbRis, ")
                .AppendLine("    @pesoNettoRis, @pesoLordoRis, @taraConfRis, @taraCollRis, @taraImbRis, @distTrUdm, @distTr, ")
                .AppendLine("    @inviato, GETDATE(), GETDATE(), @userOp, @userOp, @inizio, @fine) ");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", dtoMovDettTecnEx.Piva);
            expandoObj.TryAdd("@saCod", dtoMovDettTecnEx.Sa_Cod);
            expandoObj.TryAdd("@idAgenda", dtoMovDettTecnEx.Id_Agenda);
            expandoObj.TryAdd("@idMov", dtoMovDettTecnEx.Id_Mov);
            expandoObj.TryAdd("@idMovDet", dtoMovDettTecnEx.Id_Mov_Det);
            expandoObj.TryAdd("@idRegDet", dtoMovDettTecnEx.Id_Reg_Det);

            expandoObj.TryAdd("@regione", dtoMovDettTecnEx.Regione);
            expandoObj.TryAdd("@asl", dtoMovDettTecnEx.Asl);
            expandoObj.TryAdd("@serie", dtoMovDettTecnEx.Serie);
            expandoObj.TryAdd("@numero", dtoMovDettTecnEx.Numero);
            expandoObj.TryAdd("@macCod", dtoMovDettTecnEx.Mac_Cod);
            expandoObj.TryAdd("@codRisUm", dtoMovDettTecnEx.Cod_RisUm);
            expandoObj.TryAdd("@trasp", dtoMovDettTecnEx.Trasportatore);
            expandoObj.TryAdd("@mezzoTrasp", dtoMovDettTecnEx.Mezzo_Trasporto);
            expandoObj.TryAdd("@targa", dtoMovDettTecnEx.Targa);
            expandoObj.TryAdd("@nImmat", dtoMovDettTecnEx.N_Immatricolazione);
            expandoObj.TryAdd("@nImmatRim", dtoMovDettTecnEx.N_Immatricolazione_Rimorchio);
            expandoObj.TryAdd("@nAutoTrasp", dtoMovDettTecnEx.N_Autorizzazione_Trasporto);
            expandoObj.TryAdd("@dataRilAuto", dtoMovDettTecnEx.Data_Rilascio_Autorizzazione);
            expandoObj.TryAdd("@peso", dtoMovDettTecnEx.Peso);
            expandoObj.TryAdd("@codProd", dtoMovDettTecnEx.Codice_Prodotto);
            expandoObj.TryAdd("@colore", dtoMovDettTecnEx.Colore);
            expandoObj.TryAdd("@zonaVit", dtoMovDettTecnEx.Zona_Viticola);
            expandoObj.TryAdd("@manipol", dtoMovDettTecnEx.Manipolazioni);
            expandoObj.TryAdd("@precis", dtoMovDettTecnEx.Precisazioni);
            expandoObj.TryAdd("@annot", dtoMovDettTecnEx.Annotazioni);
            expandoObj.TryAdd("@numCont", dtoMovDettTecnEx.Num_Contenitori);
            expandoObj.TryAdd("@marcheCont", dtoMovDettTecnEx.Marche_Contenitori);
            expandoObj.TryAdd("@desCont", dtoMovDettTecnEx.Des_Contenitori);
            expandoObj.TryAdd("@tipoDoc", dtoMovDettTecnEx.Tipo_Documento);
            expandoObj.TryAdd("@idCodAut", dtoMovDettTecnEx.Id_Cod_Autorita);
            expandoObj.TryAdd("@luogoPart", dtoMovDettTecnEx.Luogo_Partenza);
            expandoObj.TryAdd("@luogoCons", dtoMovDettTecnEx.Luogo_Consegna);
            expandoObj.TryAdd("@dataSped", dtoMovDettTecnEx.Data_Spedizione);
            expandoObj.TryAdd("@indicCompl", dtoMovDettTecnEx.Indicazioni_Complementari);
            expandoObj.TryAdd("@titoloAlc", dtoMovDettTecnEx.Titolo_Alcol);
            expandoObj.TryAdd("@codNC", dtoMovDettTecnEx.Codice_NC);
            expandoObj.TryAdd("@numRif", dtoMovDettTecnEx.Num_Riferimento);
            expandoObj.TryAdd("@dataDich", dtoMovDettTecnEx.Data_Dichiarazione);
            expandoObj.TryAdd("@garanzia", dtoMovDettTecnEx.Garanzia);
            expandoObj.TryAdd("@certif", dtoMovDettTecnEx.Certificati);
            expandoObj.TryAdd("@durataV", dtoMovDettTecnEx.Durata_Viaggio);
            expandoObj.TryAdd("@pesoLordo", dtoMovDettTecnEx.Peso_Lordo);
            expandoObj.TryAdd("@numColli", dtoMovDettTecnEx.Num_Colli);
            expandoObj.TryAdd("@contenCod", dtoMovDettTecnEx.Contenitore_Cod);
            expandoObj.TryAdd("@imballCod", dtoMovDettTecnEx.Imballaggio_Cod);
            expandoObj.TryAdd("@agenteCod", dtoMovDettTecnEx.Agente_Cod);
            expandoObj.TryAdd("@provv", dtoMovDettTecnEx.Provvigione);
            expandoObj.TryAdd("@tipoTrasp", dtoMovDettTecnEx.Tipo_Trasporto);
            expandoObj.TryAdd("@unitTrasp", dtoMovDettTecnEx.Unita_Trasporto);
            expandoObj.TryAdd("@codAlt", dtoMovDettTecnEx.Codice_Alternativo);
            expandoObj.TryAdd("@idGestVet", dtoMovDettTecnEx.Id_Gestione_Vettore);
            expandoObj.TryAdd("@ritenAccCod", dtoMovDettTecnEx.Ritenuta_Acconto_Cod);
            expandoObj.TryAdd("@ritenAcc", dtoMovDettTecnEx.Ritenuta_Acconto);
            expandoObj.TryAdd("@enasCod", dtoMovDettTecnEx.Enasarco_Cod);
            expandoObj.TryAdd("@enas", dtoMovDettTecnEx.Enasarco);
            expandoObj.TryAdd("@codRUDestio", dtoMovDettTecnEx.ACCDAA_Cod_RisUm_Destinatario);
            expandoObj.TryAdd("@codRUDestone", dtoMovDettTecnEx.ACCDAA_Cod_RisUm_Destinazione);
            expandoObj.TryAdd("@codIndRUDestio", dtoMovDettTecnEx.ACCDAA_Cod_IndirizzoRisUm_Destinatario);
            expandoObj.TryAdd("@codIndRUDestone", dtoMovDettTecnEx.ACCDAA_Cod_IndirizzoRisUm_Destinazione);
            expandoObj.TryAdd("@capoACod", dtoMovDettTecnEx.CapoArea_Cod);
            expandoObj.TryAdd("@provvCapoA", dtoMovDettTecnEx.Provvigione_CapoArea);
            expandoObj.TryAdd("@provvPagAg", dtoMovDettTecnEx.Provvigione_Pagata_Agente);
            expandoObj.TryAdd("@provvPagCapoA", dtoMovDettTecnEx.Provvigione_Pagata_CapoArea);
            expandoObj.TryAdd("@nDocCl", dtoMovDettTecnEx.N_Doc_Cliente);
            expandoObj.TryAdd("@dataDocCl", dtoMovDettTecnEx.Data_Doc_Cliente);
            expandoObj.TryAdd("@nNotaFatt", dtoMovDettTecnEx.N_Nota_Fattura);
            expandoObj.TryAdd("@dataNotaFatt", dtoMovDettTecnEx.Data_Nota_Fattura);
            expandoObj.TryAdd("@nNotaDdt", dtoMovDettTecnEx.N_Nota_DDT);
            expandoObj.TryAdd("@nNotaRDdt", dtoMovDettTecnEx.N_Nota_Riga_DDT);
            expandoObj.TryAdd("@dataNotaDdt", dtoMovDettTecnEx.Data_Nota_DDT);
            expandoObj.TryAdd("@causFatt", dtoMovDettTecnEx.Causale_Fattura);
            expandoObj.TryAdd("@nDocEnte", dtoMovDettTecnEx.N_Doc_Ente);
            expandoObj.TryAdd("@annoDocEnte", dtoMovDettTecnEx.Anno_Doc_Ente);
            expandoObj.TryAdd("@numConfRis", dtoMovDettTecnEx.Num_Conf_Riscontrate);
            expandoObj.TryAdd("@numColliRis", dtoMovDettTecnEx.Num_Colli_Riscontrati);
            expandoObj.TryAdd("@numImbRis", dtoMovDettTecnEx.Num_Imballi_Riscontrati);
            expandoObj.TryAdd("@pesoNettoRis", dtoMovDettTecnEx.Peso_Netto_Riscontrato);
            expandoObj.TryAdd("@pesoLordoRis", dtoMovDettTecnEx.Peso_Lordo_Riscontrato);
            expandoObj.TryAdd("@taraConfRis", dtoMovDettTecnEx.Tara_Unit_Conf_Riscontrata);
            expandoObj.TryAdd("@taraCollRis", dtoMovDettTecnEx.Tara_Unit_Collo_Riscontrata);
            expandoObj.TryAdd("@taraImbRis", dtoMovDettTecnEx.Tara_Unit_Imballo_Riscontrata);
            expandoObj.TryAdd("@distTrUdm", dtoMovDettTecnEx.Distanza_Trasporto_Udm);
            expandoObj.TryAdd("@distTr", dtoMovDettTecnEx.Distanza_Trasporto);

            expandoObj.TryAdd("@inviato", dtoMovDettTecnEx.Inviato);
            //expandoObj.TryAdd("@dtInvio", dtoMovDettTecnEx.Data_Invio);
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@inizio", dtoMovDettTecnEx.Validita_Inizio);
            expandoObj.TryAdd("@fine", dtoMovDettTecnEx.Validita_Fine);

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

        public async Task<bool> UpdateAsync(WriteMovDettTecnicoExtra dtoMovDettTecnEx, AgronicaCoreParametriServer objParametriServer)
        {
            if (dtoMovDettTecnEx.Id_Reg_Det == 0)
                throw new Exception("Id_Reg_Dettaglio non valorizzato.");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", dtoMovDettTecnEx.Piva);
            expandoObj.TryAdd("@idAgenda", dtoMovDettTecnEx.Id_Agenda);
            expandoObj.TryAdd("@idMov", dtoMovDettTecnEx.Id_Mov);
            expandoObj.TryAdd("@idMovDet", dtoMovDettTecnEx.Id_Mov_Det);
            expandoObj.TryAdd("@idRegDet", dtoMovDettTecnEx.Id_Reg_Det);
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);

            var excludedProperties = new HashSet<string>
            {
                "Piva",
                "Sa_Cod",
                "Id_Agenda",
                "Id_Mov",
                "Id_Mov_Det",
                "Id_Reg_Dettaglio",
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
            foreach (var property in typeof(WriteMovDettTecnicoExtra).GetProperties())
            {
                if (excludedProperties.Contains(property.Name)) continue;

                var value = property.GetValue(dtoMovDettTecnEx);
                if (value != null)
                {
                    string paramName = "@" + property.Name;
                    setClauses.Add($"{property.Name} = {paramName}");
                    expandoObj.TryAdd(paramName, value);
                }
            }
            if (setClauses.Count == 0) return false;

            string updateQuery =
                $@"UPDATE Mov_Dettaglio_Tecnico_Extra SET
                    {string.Join(", ", setClauses)}
                    , Username_Modifica = @userOp
                    , Data_Modifica = GETDATE()
                WHERE Piva = @piva AND Id_Agenda = @idAgenda AND Id_Mov = @idMov AND Id_Mov_Det = @idMovDet AND Id_Reg_Dettaglio = @idRegDet ";

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

        public async Task<bool> DeleteAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Reg_Det, AgronicaCoreParametriServer objParametriServer)
        {
            if (Id_Reg_Det == 0)
                throw new Exception("Id_Reg_Dettaglio non valorizzato.");

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", Piva);
            expandoObj.TryAdd("@idAgenda", Id_Agenda);
            expandoObj.TryAdd("@idMov", Id_Mov);
            expandoObj.TryAdd("@idMovDet", Id_Mov_Det);
            expandoObj.TryAdd("@idRegDet", Id_Reg_Det);

            if (objParametriServer.FlagCancellazioneLogica == enumCancellazioneLogica.CancellazioneLogica)
            {
                stbQuery.AppendLine("UPDATE Mov_Dettaglio_Tecnico_Extra SET ")
                    .AppendLine("    Inviato = -1, ")
                    .AppendLine("    Username_Modifica = @userOp ")
                    .AppendLine("WHERE Inviato >= 0 ");
                expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            }
            else
            {
                stbQuery.AppendLine("DELETE FROM Mov_Dettaglio_Tecnico_Extra ")
                    .AppendLine("WHERE 1=1 ");
            }

            stbQuery.AppendLine("    AND Piva = @piva ")
                .AppendLine("    AND Id_Agenda = @idAgenda ")
                .AppendLine("    AND Id_Mov = @idMov ")
                .AppendLine("    AND Id_Mov_Det = @idMovDet ")
                .AppendLine("    AND Id_Reg_Dettaglio = @idRegDet ");

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

        public async Task<int> ScriviModificaAsync(WriteMovDettTecnicoExtra dtoMovDettTecExtra, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                bool isNew = false;

                if (dtoMovDettTecExtra.Id_Reg_Det == 0)
                {
                    dtoMovDettTecExtra.Id_Reg_Det = await _sequenceDal.NuovoId_TabellaAsync("movimenti_dettagli_tecnici_extra", 0, 2000000000, objParametriServer);
                    isNew = true;
                }
                else
                    isNew = !await ExistAsync(dtoMovDettTecExtra.Piva, dtoMovDettTecExtra.Id_Agenda, dtoMovDettTecExtra.Id_Mov, dtoMovDettTecExtra.Id_Mov_Det, dtoMovDettTecExtra.Id_Reg_Det, objParametriServer);

                if (isNew)
                    await CreateAsync(dtoMovDettTecExtra, objParametriServer);
                else
                    await UpdateAsync(dtoMovDettTecExtra, objParametriServer);

                return dtoMovDettTecExtra.Id_Reg_Det;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(WriteMovDettTecnicoExtra dtoMovDettTecExtra, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await DeleteAsync(dtoMovDettTecExtra.Piva, dtoMovDettTecExtra.Id_Agenda, dtoMovDettTecExtra.Id_Mov, dtoMovDettTecExtra.Id_Mov_Det, dtoMovDettTecExtra.Id_Reg_Det, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAsync(List<WriteMovDettTecnicoExtra> MovDettTecExtra, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(objParametriServer.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var dto in MovDettTecExtra)
                        await DeleteAsync(dto.Piva, dto.Id_Agenda, dto.Id_Mov, dto.Id_Mov_Det, dto.Id_Reg_Det, objParametriServer);

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
