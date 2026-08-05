using System.Data;
using AgronicaCoreDTOStd.InData.FiltroRicerca;
using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreModelsSTD.Models.ConfiguratoreFiltroRicerca;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility.ObjectExtension;
using AgronicaNetCore.FiltroRicerca.DAL.DataLayer.FiltroRicerca;
using static AgronicaNetCore.FiltroRicerca.DAL.Utils.Utils.Utils;
using AgronicaCoreDTOStd.OutData.FiltroRicerca;
using OutData.Kendo;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;
using Microsoft.Extensions.DependencyInjection;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.UtilityDB;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazioni;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.UnitaMisuraConversione;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioniFiltroMono;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.SpecieVegetali;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.FiltroRicerca.BIZ.Resources;

namespace AgronicaNetCore.FiltroRicerca.BIZ.Services
{
    public class FiltroRicercaService : BaseServiceFiltroRicercaBIZ, IFiltroRicercaService
    {
        private readonly IUtentiVisibilitaAppoggio _utentiVisibilitaAppoggio;
        private readonly IFiltroRicerca _filtroRicerca;
        private readonly IUtilityDB _utilityDB;
        private readonly IUtentiImpostazioni _utentiImpostazioni;
        private readonly IOperazioni _operazioni;
        private readonly IUnitaMisuraConversione _unitaMisuraConversione;
        private readonly IUtentiImpostazioniFiltroMono _utentiImpostazioniFiltroMono;
        private readonly ISpecieVegetali _specieVegetali;
        private readonly ISecurityLayerDAL _securityLayerDal;

        public FiltroRicercaService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _utentiVisibilitaAppoggio = _serviceProvider.GetRequiredService<IUtentiVisibilitaAppoggio>();
            _filtroRicerca = _serviceProvider.GetRequiredService<IFiltroRicerca>();
            _utilityDB = _serviceProvider.GetRequiredService<IUtilityDB>();
            _utentiImpostazioni = _serviceProvider.GetRequiredService<IUtentiImpostazioni>();
            _operazioni = _serviceProvider.GetRequiredService<IOperazioni>();
            _unitaMisuraConversione = _serviceProvider.GetRequiredService<IUnitaMisuraConversione>();
            _utentiImpostazioniFiltroMono = _serviceProvider.GetRequiredService<IUtentiImpostazioniFiltroMono>();
            _specieVegetali = _serviceProvider.GetRequiredService<ISpecieVegetali>();
            _securityLayerDal = _serviceProvider.GetRequiredService<ISecurityLayerDAL>();
        }

        public async Task<CriteriRicerca_OUT> GetResultAsync(CriteriRicerca_IN criteriRicerca_IN,
                                                             AgronicaCoreParametriServer objParametriServer,
                                                             AgronicaCoreParametriUtenti objParametriUtenti,
                                                             AgronicaCoreParametriSuperServer objParametriSuperServer,
                                                             int? capSelectTopRows = null)
        {

            CriteriRicerca_OUT criteriRicerca_OUT;

            try
            {
                ConfiguratoreFiltroRicerca configuratore = new((Enum_TipoMostra_FiltroRicerca)criteriRicerca_IN.TipoMostra, criteriRicerca_IN.CaricaDati);

                CriteriRicercaExtended criteriRicerca = new();
                ObjectExtension.PropertyCopier(criteriRicerca_IN, criteriRicerca);

                // All six pre-flight checks are independent DB reads — fan them out in parallel
                // so we pay only the cost of the slowest call instead of the sum of all six.
                if (!criteriRicerca_IN.OnLoad)
                {
                    var visibilitaAziendeTask  = _utentiVisibilitaAppoggio.ReadRowExistsForUsernameAsync((int)Enum_TipoEntita.Impresa, objParametriServer.UtenteUsername, objParametriServer);
                    var visibilitaCentriTask   = _utentiVisibilitaAppoggio.ReadRowExistsForUsernameAsync((int)Enum_TipoEntita.Centro, objParametriServer.UtenteUsername, objParametriServer);
                    var sqlCompatibilityTask   = _utilityDB.Read_SQL_Compatibility_LevelAsync(objParametriServer);
                    var sqlVersionTask         = _utilityDB.ReadSQLVersionMajorAsync(objParametriServer);
                    var fattoreTask            = GetFattoreConversioneAsync(objParametriServer, objParametriUtenti);
                    var topRowsTask            = GetNumeroMassimoRigheEstraibiliFiltroRicerca(objParametriServer, objParametriSuperServer);

                    await Task.WhenAll(visibilitaAziendeTask, visibilitaCentriTask, sqlCompatibilityTask, sqlVersionTask, fattoreTask, topRowsTask);

                    criteriRicerca.CheckVisibilitaAziende              = Convert.ToBoolean(visibilitaAziendeTask.Result!.Rows[0]["RowExists"]);
                    criteriRicerca.CheckVisibilitaCentriAziendali      = Convert.ToBoolean(visibilitaCentriTask.Result!.Rows[0]["RowExists"]);
                    criteriRicerca.SQLCompatibility                    = sqlCompatibilityTask.Result >= 130;
                    criteriRicerca.USE_FORCE_LEGACY_CARDINALITY_ESTIMATION = sqlVersionTask.Result >= 12; //major 12 corrisponde a sql server 2014  https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-And-install-latest-updates
                    criteriRicerca.FattoreConversione = fattoreTask.Result;

                    // Apply cap: use capSelectTopRows when it is tighter than the configured limit
                    // (or when the configured limit is 0, meaning unlimited).
                    int configuredTopRows = topRowsTask.Result;
                    if (capSelectTopRows.HasValue && capSelectTopRows.Value > 0 &&
                        (configuredTopRows == 0 || capSelectTopRows.Value < configuredTopRows))
                        criteriRicerca.selectTopRows = capSelectTopRows.Value;
                    else
                        criteriRicerca.selectTopRows = configuredTopRows;
                }

                if (!criteriRicerca_IN.OnLoad)
                {
                    await VerificaApplicaVisibilitaSpecieAsync(criteriRicerca_IN.TipoMostra, criteriRicerca, objParametriUtenti, objParametriServer);
                    await VerificaApplicaVisibilitaOperazioniAsync(criteriRicerca_IN.TipoMostra, criteriRicerca, objParametriServer, objParametriUtenti);
                }

                string utenti_DB_name = _utilityDB.GetDBName(objParametriUtenti);
                criteriRicerca_OUT = await _filtroRicerca.GetResultAsync(criteriRicerca, configuratore, utenti_DB_name, objParametriServer, objParametriUtenti);

                OperazioniPostProcess(criteriRicerca, configuratore, ref criteriRicerca_OUT);

                criteriRicerca_OUT.overflowSelectTopRows = (criteriRicerca.selectTopRows > 0 && criteriRicerca_OUT.result.Rows.Count == criteriRicerca.selectTopRows + 1);
            }

            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw new Exception(ex.Message);
            }

            return criteriRicerca_OUT;
        }

        public async Task<string> GetPivaRealeAsync(string piva, AgronicaCoreParametriServer objParametriServer)
        {
            string pivaReale = piva;
            try
            {
                pivaReale = await _utilityDB.GetPivaRealeAsync(piva, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw new Exception(ex.Message);
            }

            return pivaReale;
        }

        private async Task VerificaApplicaVisibilitaSpecieAsync(int TipoMostra, CriteriRicercaExtended criteriRicerca, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            bool VerificaApplicaVisibilitaSpecie = false;

            //Verifico se ho applicato anche un solo filtro per il quale applicare filtro sulla specie                    
            FiltriPianoColturale FiltriPianoColturale = criteriRicerca.FiltriPianoColturale;

            //L'utente vuole solo le Destinazioni d'Uso (no specie)
            if (FiltriPianoColturale != null && FiltriPianoColturale.FiltroDestinazioneUso == (int)Enum_FiltroDestinazioneUso_FiltroRicerca.SoloDestinazioniUso)
                return;

            switch (TipoMostra)
            {
                case (int)Enum_TipoMostra_FiltroRicerca.Esercizi:
                case (int)Enum_TipoMostra_FiltroRicerca.Impianti:
                case (int)Enum_TipoMostra_FiltroRicerca.Appezzamenti:
                case (int)Enum_TipoMostra_FiltroRicerca.PianoColturale:
                case (int)Enum_TipoMostra_FiltroRicerca.PianoColturaleBudget:
                case (int)Enum_TipoMostra_FiltroRicerca.Campi:
                case (int)Enum_TipoMostra_FiltroRicerca.Movimenti:
                    VerificaApplicaVisibilitaSpecie = true;
                    break;

                default:
                    //Verifico se ho applicato anche un solo filtro per il quale applicare filtro sulla specie                    
                    FiltriTemporali FiltriTemporali = criteriRicerca.FiltriTemporali;
                    FiltriServizi FiltriServizi = criteriRicerca.FiltriServizi;
                    FiltriGIS FiltriGIS = criteriRicerca.FiltriGIS;
                    FiltriCatasto FiltriCatasto = criteriRicerca.FiltriCatasto;

                    if (FiltriPianoColturale != null)
                    {

                        // METODO PRODUZIONE                 
                        if (FiltriPianoColturale.UtilizzoTerreno != null && FiltriPianoColturale.UtilizzoTerreno.Count > 0)
                            VerificaApplicaVisibilitaSpecie = true;

                        if (FiltriPianoColturale.FiltroDestinazioneUso > 0)
                            VerificaApplicaVisibilitaSpecie = true;

                        // LOTTO
                        if (FiltriPianoColturale.Lotto != null && FiltriPianoColturale.Lotto.Trim() != "")
                            VerificaApplicaVisibilitaSpecie = true;

                        // PROGETTO
                        if (FiltriPianoColturale.Progetto != null && FiltriPianoColturale.Progetto.Trim() != "")
                            VerificaApplicaVisibilitaSpecie = true;
                    }

                    if (FiltriTemporali != null)
                        if (FiltriTemporali.FiltriData != null && FiltriTemporali.FiltriData.Count > 0)
                            VerificaApplicaVisibilitaSpecie = true;

                    if (FiltriGIS != null)
                    {
                        if (FiltriGIS.FiltroPoligoni > 0)
                            VerificaApplicaVisibilitaSpecie = true;

                        if (FiltriGIS.Anomalie != null && FiltriGIS.Anomalie.Count > 0)
                            VerificaApplicaVisibilitaSpecie = true;
                    }

                    if (FiltriCatasto != null)
                        if (FiltriCatasto.FiltroRipartoCatastale > 0)
                            VerificaApplicaVisibilitaSpecie = true;

                    break;
            }

            //L'utente non ha filtrato direttamente Specie e/o Gruppi Vegetali, ma altro correlato all'impianto
            if (VerificaApplicaVisibilitaSpecie)
            {
                DataTable utentiImpostazioniMonoDt_GruppiVegetali = await _utentiImpostazioniFiltroMono.LeggiAsync(TipiEnumerativi.Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI, 0, objParametriUtenti, objParametriServer);
                bool CheckVisibilitaGruppiVegetali = utentiImpostazioniMonoDt_GruppiVegetali.Rows.Count > 0;

                if (CheckVisibilitaGruppiVegetali)
                {
                    DataTable VisibilitaGruppiVegetaliDT = await _specieVegetali.GruppoVegetale_GestioneFiltroUtente_LeggiAsync(0, objParametriUtenti, objParametriServer);

                    criteriRicerca.FiltriPianoColturale ??= new();
                    //Applico il filtro nel Gruppo Vegetale, come se lo avesse selezionato l'utente
                    criteriRicerca.FiltriPianoColturale.GruppoVegetale = VisibilitaGruppiVegetaliDT.AsEnumerable().Select(r => r.Field<int>("Gru_Cod")).Distinct().ToList();

                    criteriRicerca.FiltriCampi ??= new();
                    //Applico il filtro nel Gruppo Vegetale del Campo (non esiste in interfaccia)
                    criteriRicerca.FiltriCampi.GruppoVegetale = VisibilitaGruppiVegetaliDT.AsEnumerable().Select(r => r.Field<int>("Gru_Cod")).Distinct().ToList();

                    DataTable utentiImpostazioniMonoDt_SpecieVegetali = await _utentiImpostazioniFiltroMono.LeggiAsync(TipiEnumerativi.Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI, 0, objParametriUtenti, objParametriServer);
                    bool CheckVisibilitaSpecieVegetali = utentiImpostazioniMonoDt_SpecieVegetali.Rows.Count > 0;

                    if (CheckVisibilitaSpecieVegetali)
                    {
                        DataTable VisibilitaSpecieDT = (await _specieVegetali.SpecieVegetali_GestioneFiltroUtente_LeggiAsync(new LeggiSpecieVegetali_IN(), utentiImpostazioniMonoDt_GruppiVegetali, objParametriUtenti, objParametriServer)).DataTable;

                        //Posso applicare filtri all'interno dei criteri di rcierca solo se non si tratta di Campi
                        if (VisibilitaSpecieDT.Rows.Count > 0)
                        {
                            //Applico il filtro nella Specie, come se lo avesse selezionato l'utente
                            criteriRicerca.FiltriPianoColturale.Specie = VisibilitaSpecieDT.AsEnumerable().Select(r => r.Field<int>("Veg_Cod")).ToList();

                            //Applico il filtro nella Specie del Campo (non esiste in interfaccia)
                            criteriRicerca.FiltriCampi.Specie = VisibilitaSpecieDT.AsEnumerable().Select(r => r.Field<int>("Veg_Cod")).ToList();

                            DataTable utentiImpostazioniMonoDt_Varieta = await _utentiImpostazioniFiltroMono.LeggiAsync(TipiEnumerativi.Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_VARIETA, 0, objParametriUtenti, objParametriServer);
                            bool CheckVisibilitaVarieta = utentiImpostazioniMonoDt_Varieta.Rows.Count > 0;

                            if (CheckVisibilitaVarieta)
                            {
                                DataTable VisibilitaVarietaDT = await _specieVegetali.Cultivar_GestioneFiltroUtente_LeggiAsync(new Cultivar_GestioneFiltroUtente_Leggi_IN(), utentiImpostazioniMonoDt_GruppiVegetali, objParametriUtenti, objParametriServer);
                                if (VisibilitaVarietaDT.Rows.Count > 0)
                                {
                                    criteriRicerca.FiltriPianoColturale ??= new();
                                    //Applico il filtro nel Gruppo Vegetale, come se lo avesse selezionato l'utente
                                    criteriRicerca.FiltriPianoColturale.Varieta = VisibilitaVarietaDT.AsEnumerable().Select(r => r.Field<int>("Cul_Cod")).Distinct().ToList();
                                }
                            }
                        }
                    }
                }
            }
        }

        private async Task VerificaApplicaVisibilitaOperazioniAsync(int TipoMostra, CriteriRicercaExtended criteriRicerca, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {

            //Verifico se ho applicato anche un solo filtro per il quale applicare filtro sulla specie                    
            FiltriMovimenti FiltriMovimenti = criteriRicerca.FiltriMovimenti;
            //L'utente non sta filtrando sui movimenti
            if (FiltriMovimenti != null && FiltriMovimenti.FiltroOperazioni == (int)Enum_FiltroOperazioniAgenda_FiltroRicerca.Tutto)
                return;

            //se è stato impostato un valore diverso da Enum_FiltroOperazioniAgenda_FiltroRicerca.Tutto), allora devo verificare la visibilità sulle operazioni
            DataTable utentiImpostazioniDt_GruppoOperazioni = await _utentiImpostazioni.Read_JoinWithFiltroMonoAsync(TipiEnumerativi.Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_OPERAZIONI, 1, objParametriUtenti, objParametriServer);
            bool CheckVisibilitaGruppoOperazioni = utentiImpostazioniDt_GruppoOperazioni.Rows.Count > 0;

            if (CheckVisibilitaGruppoOperazioni)
            {
                criteriRicerca.FiltriMovimenti ??= new();

                List<int> gruppiGestitiDaImpostazioniUtente = utentiImpostazioniDt_GruppoOperazioni.AsEnumerable().Select(r => r.Field<int>("ID_0")).Where(r => GruppoOperazioniAmmissibili.Contains(r)).Distinct().ToList();

                if (gruppiGestitiDaImpostazioniUtente.Count > 0)
                {
                    //Applico il filtro nel Gruppo Operazioni, come se lo avesse selezionato l'utente (dalle impostazioni utente prendo solo i gruppi ammissibili) 
                    criteriRicerca.FiltriMovimenti.GruppoOperazioni = utentiImpostazioniDt_GruppoOperazioni.AsEnumerable().Select(r => r.Field<int>("ID_0")).Where(r => GruppoOperazioniAmmissibili.Contains(r)).Distinct().ToList();

                    DataTable utentiImpostazioniDt_Operazioni = await _utentiImpostazioni.Read_JoinWithFiltroMonoAsync(TipiEnumerativi.Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI, 1, objParametriUtenti, objParametriServer);
                    bool CheckVisibilitaOperazioni = utentiImpostazioniDt_Operazioni.Rows.Count > 0;

                    //Nelle impostazioni utente i due filtri NON sono a cascata, quindi devo controllare i gruppi operazioni e le operazioni separatamente
                    if (CheckVisibilitaOperazioni)
                    {
                        DataTable VisibilitaOperazioniDT = (await _operazioni.Operazioni_GestioneFiltroUtente_LeggiAsync(new LeggiOperazioni_IN { GruppoOperazioni = GruppoOperazioniAmmissibili }, utentiImpostazioniDt_GruppoOperazioni, objParametriServer, objParametriUtenti)).DataTable;

                        if (VisibilitaOperazioniDT.Rows.Count > 0)
                        {
                            //Applico il filtro nelle Operazioni, come se lo avesse selezionato l'utente
                            criteriRicerca.FiltriMovimenti.Operazioni = VisibilitaOperazioniDT.AsEnumerable().Select(r => r.Field<int>("Lav_Cod")).ToList();
                        }
                    }
                }
                else
                {
                    //Nessuno dei gruppi utilizzabili nel filtro di ricerca è visibile per l'utente
                    criteriRicerca.FiltriMovimenti.GruppoOperazioni.Add(-999);
                }
            }
        }

        private async Task<decimal> GetFattoreConversioneAsync(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            int udm_cod = 0;
            decimal fattoreConversione = 1;

            DataTable DtUtentiImp = (await _utentiImpostazioni.ReadAsync((int)Enum_Impostazioni_Utenti.Visualizza_UdmAggiuntiva_xSup, 2, objParametriUtenti, objParametriServer))!;

            if (DtUtentiImp!.Rows.Count > 0)
                udm_cod = (DtUtentiImp.Rows[0]["Impostazione_Valore_1"] == null) ? 0 : Convert.ToInt32(DtUtentiImp.Rows[0]["Impostazione_Valore_1"]);

            if (udm_cod != 0)
            {
                DataTable dtfattoreConversione = await _unitaMisuraConversione.ReadAsync(new UnitaMisuraConversione_IN { Udm_Cod_Da = (int)Enum_UnitaMisura.Ettaro, Udm_Cod_A = udm_cod }, objParametriServer);

                if (dtfattoreConversione.Rows.Count > 0)
                    fattoreConversione = dtfattoreConversione.Rows[0]["fattoreconversione"] == null ? 0 : Convert.ToDecimal(dtfattoreConversione.Rows[0]["fattoreconversione"]);
            }

            return fattoreConversione;
        }

        private async Task<int> GetNumeroMassimoRigheEstraibiliFiltroRicerca(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            string valore = (await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync("NumeroMassimoRigheEstraibiliFiltroRicerca", objParametriServer, objParametriSuperServer))!;

            if (int.TryParse(valore, out int selectTopRows))
                selectTopRows = Convert.ToInt32(valore);
            else
                selectTopRows = 0;

            return selectTopRows;
        }

        #region "Operazioni Post"
        public static void OperazioniPostProcess(CriteriRicercaExtended criteriRicerca, ConfiguratoreFiltroRicerca configuratore, ref CriteriRicerca_OUT criteriRicerca_OUT)
        {
            if (criteriRicerca.TipoMostra == (int)Enum_TipoMostra_FiltroRicerca.Impianti)
                RimuoviImpiantiDoppi(ref criteriRicerca_OUT);

            switch (criteriRicerca.TipoMostra)
            {
                case (int)Enum_TipoMostra_FiltroRicerca.Esercizi:
                case (int)Enum_TipoMostra_FiltroRicerca.PianoColturale:
                case (int)Enum_TipoMostra_FiltroRicerca.PianoColturaleBudget:
                    PredisponiDatiMancantiEsercizio(criteriRicerca, ref criteriRicerca_OUT);
                    break;
            }

            //Quando c'è di mezzo il catasto, le chiavi della particella devono essere inserite nella pk del Datatable,
            //Non è detto che questi valori siano != DbNull, devo correggere
            if (configuratore.CaricaDatiCatastaliCentroAziendale)
                ReplaceDBNull(ref criteriRicerca_OUT);

            if (criteriRicerca.TipoMostra == (int)Enum_TipoMostra_FiltroRicerca.PianoColturaleBudget)
            {
                AggiungiColonnaBudget(criteriRicerca.Id_Budget, ref criteriRicerca_OUT);
            }

            AggiungiColonnaChiave(criteriRicerca.TipoMostra, ref criteriRicerca_OUT);

            AggiungiPK(configuratore, ref criteriRicerca_OUT);

            PulisciKendoColumns(ref criteriRicerca_OUT);
        }

        private static void ReplaceDBNull(ref CriteriRicerca_OUT criteriRicerca_OUT)
        {
            foreach (DataColumn col in criteriRicerca_OUT.result.Columns)
            {
                foreach (DataRow row in criteriRicerca_OUT.result.Rows)
                {
                    if (col.DataType.Name.ToLowerInvariant() == "string" && row[col].GetType().Name.ToLowerInvariant() == "dbnull" && row[col] == DBNull.Value)
                    {
                        row[col] = "";
                    }
                }
            }
        }

        private static void PulisciKendoColumns(ref CriteriRicerca_OUT criteriRicerca_OUT)
        {
            List<DataColumn> columnsToRemove = new();
            List<KendoColumn> kendoColumnsToRemove = new();
            foreach (DataColumn col in criteriRicerca_OUT.result.Columns)
            {
                //Rimuovo tutte le colonne 'key' che non servono al client
                if (col.ColumnName.Contains("key"))
                {
                    columnsToRemove.Add(col);
                }
            }

            foreach (DataColumn col in columnsToRemove)
            {
                criteriRicerca_OUT.result.Columns.Remove(col);
            }
            foreach (KendoColumn kCol in kendoColumnsToRemove)
            {
                criteriRicerca_OUT.kendoColumns.ToList().Remove(kCol);
            }

            //Ordino le colonne default
            criteriRicerca_OUT.kendoColumns = criteriRicerca_OUT.kendoColumns.OrderBy(k => k.ColumnOrder).ToArray();
        }

        private static void AggiungiPK(ConfiguratoreFiltroRicerca configuratore, ref CriteriRicerca_OUT criteriRicerca_OUT)
        {
            var pk = new List<DataColumn>();
            foreach (DataColumn col in criteriRicerca_OUT.result.Columns)
            {
                switch (col.ColumnName.ToLowerInvariant())
                {
                    case "piva":
                        pk.Add(col);
                        break;
                    case "sa_cod":
                        if (configuratore.isMostraCentriAziendali || configuratore.isMostraCampi || configuratore.isMostraAppezzamenti || configuratore.isMostraImpianti || configuratore.isMostraEsercizi || configuratore.isMostraFabbricati || configuratore.isMostraMovimenti)
                        {
                            pk.Add(col);
                            criteriRicerca_OUT.kendoColumns = criteriRicerca_OUT.kendoColumns.Append(new KendoColumn { Field = "Sa_Cod", Title = "Sa_Cod", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false }).ToArray();
                        }
                        break;
                    case "campo_cod":
                        if (configuratore.isMostraCampi)
                        {
                            pk.Add(col);
                            criteriRicerca_OUT.kendoColumns = criteriRicerca_OUT.kendoColumns.Append(new KendoColumn { Field = "Campo_Cod", Title = "Campo_Cod", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false }).ToArray();
                        }
                        break;
                    case "appezza":
                        if (configuratore.isMostraAppezzamenti || configuratore.isMostraImpianti || configuratore.isMostraEsercizi || configuratore.isMostraMovimenti)
                        {
                            pk.Add(col);
                            criteriRicerca_OUT.kendoColumns = criteriRicerca_OUT.kendoColumns.Append(new KendoColumn { Field = "Appezza", Title = "Appezza", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false }).ToArray();
                        }
                        break;
                    case "id_reg":
                        if (configuratore.isMostraImpianti || configuratore.isMostraEsercizi || configuratore.isMostraMovimenti)
                        {
                            pk.Add(col);
                            criteriRicerca_OUT.kendoColumns = criteriRicerca_OUT.kendoColumns.Append(new KendoColumn { Field = "Id_Reg", Title = "Id_Reg", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false }).ToArray();
                        }
                        break;
                    case "progetto_cod":
                        if (configuratore.isMostraEsercizi || configuratore.isMostraImpianti || configuratore.isMostraMovimenti)
                        {
                            //Il progetto_cod non è direttamente chiave dei movimenti
                            //In data 22/05 sono stati trovati su un BAK di produzione dati sporchi sugli esercizi (due esercizi identici, con la stessa validità, ma progetto_cod diverso)
                            //Per evitare che il sistema si rompa con l'eccezione di righe non univoche, aggiungiamo questo livello di chiave
                            pk.Add(col);
                            criteriRicerca_OUT.kendoColumns = criteriRicerca_OUT.kendoColumns.Append(new KendoColumn { Field = "Progetto_Cod", Title = "Progetto_Cod", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false }).ToArray();
                        }
                        break;
                    case "fabbricato_cod":
                        if (configuratore.isMostraFabbricati)
                        {
                            pk.Add(col);
                            criteriRicerca_OUT.kendoColumns = criteriRicerca_OUT.kendoColumns.Append(new KendoColumn { Field = "Fabbricato_Cod", Title = "Fabbricato_Cod", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false }).ToArray();
                        }
                        break;
                    case "veg_cod":
                        if (configuratore.isMostraImpianti || configuratore.isMostraEsercizi || configuratore.isMostraMovimenti)
                        {
                            pk.Add(col);
                            criteriRicerca_OUT.kendoColumns = criteriRicerca_OUT.kendoColumns.Append(new KendoColumn { Field = "Veg_Cod", Title = "Veg_Cod", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false }).ToArray();
                        }
                        break;
                    case "id_agenda":
                        if (configuratore.isMostraMovimenti)
                        {
                            pk.Add(col);
                            criteriRicerca_OUT.kendoColumns = criteriRicerca_OUT.kendoColumns.Append(new KendoColumn { Field = "Id_Agenda", Title = "Id_Agenda", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false }).ToArray();
                        }
                        break;
                    case "id_particella":
                        if (configuratore.isMostraCentriAziendali && configuratore.CaricaDatiCatastaliCentroAziendale)
                            pk.Add(col);
                        break;
                    case "pratica_cod":
                        if (configuratore.isMostraAziende && configuratore.CaricaDatiServizi)
                            pk.Add(col);
                        break;
                    case "id_budget":
                        if (configuratore.joinBudget)
                        {
                            pk.Add(col);
                            criteriRicerca_OUT.kendoColumns = criteriRicerca_OUT.kendoColumns.Append(new KendoColumn { Field = "Id_Budget", Title = "Id_Budgetv", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false }).ToArray();
                        }
                        break;
                }
            }

            criteriRicerca_OUT.result.PrimaryKey = pk.ToArray();

        }
        private static void AggiungiColonnaChiave(int TipoMostra, ref CriteriRicerca_OUT criteriRicerca_OUT)
        {

            //Per i chiamanti del filtrone, non visibile
            criteriRicerca_OUT.result.Columns.Add("chiave", typeof(string));
            criteriRicerca_OUT.kendoColumns = criteriRicerca_OUT.kendoColumns.Append(new KendoColumn { Field = "chiave", Title = "chiave", TranslateTitle = false, DataType = "string", ColumnOrder = -1, Display = false, Hidden = true }).ToArray();

            //Colonna 0 delle varei griglie, helptool per gli sviluppatori
            criteriRicerca_OUT.result.Columns.Add("chiave_GIAS", typeof(string));
            criteriRicerca_OUT.kendoColumns = criteriRicerca_OUT.kendoColumns.Append(new KendoColumn { Field = "chiave_GIAS", Title = "Chiave GIAS", TranslateTitle = false, DataType = "string", ColumnOrder = -1, Display = true, Hidden = true }).ToArray();

            switch (TipoMostra)
            {
                case (int)Enum_TipoMostra_FiltroRicerca.Aziende:
                    foreach (DataRow row in criteriRicerca_OUT.result.Rows)
                    {
                        row["chiave"] = row["Piva"].ToString();
                        row["chiave_GIAS"] = row["Piva"].ToString();
                    }
                    break;
                case (int)Enum_TipoMostra_FiltroRicerca.CentriAziendali:
                    foreach (DataRow row in criteriRicerca_OUT.result.Rows)
                    {
                        row["chiave"] = row["Piva"].ToString() + '_' + row["Sa_Cod"].ToString();

                        row["chiave_GIAS"] = row["Piva"].ToString() + '-' + row["Sa_Cod"].ToString();
                    }
                    break;
                case (int)Enum_TipoMostra_FiltroRicerca.Campi:
                    foreach (DataRow row in criteriRicerca_OUT.result.Rows)
                    {
                        row["chiave"] = row["Piva"].ToString() + '_' + row["Sa_Cod"].ToString() + '_' + row["Campo_Cod"].ToString();

                        row["chiave_GIAS"] = row["Piva"].ToString() + '-' + row["Sa_Cod"].ToString() + '-' + row["Campo_Cod"].ToString();
                    }
                    break;
                case (int)Enum_TipoMostra_FiltroRicerca.Appezzamenti:
                    foreach (DataRow row in criteriRicerca_OUT.result.Rows)
                    {
                        row["chiave"] = row["Piva"].ToString() + '_' + row["Sa_Cod"].ToString() + '_' + row["Appezza"].ToString();

                        row["chiave_GIAS"] = row["Piva"].ToString() + '-' + row["Sa_Cod"].ToString() + '-' + row["Appezza"].ToString();
                    }
                    break;
                case (int)Enum_TipoMostra_FiltroRicerca.Impianti:
                    foreach (DataRow row in criteriRicerca_OUT.result.Rows)
                    {
                        string Veg_Cod = (row["Veg_Cod"] != DBNull.Value ? row["Veg_Cod"].ToString() : "0")!;
                        string Progetto_Cod = (row["Progetto_Cod"] != DBNull.Value ? row["Progetto_Cod"].ToString() : "0")!;
                        row["chiave"] = row["Piva"].ToString() + '_' + row["Sa_Cod"].ToString() + '_' + row["Appezza"].ToString() + '_' + row["Id_Reg"].ToString() + '_' + Veg_Cod.ToString() + '_' + Progetto_Cod.ToString();

                        row["chiave_GIAS"] = row["Piva"].ToString() + '-' + row["Sa_Cod"].ToString() + '-' + row["Appezza"].ToString() + '-' + row["Id_Reg"].ToString();
                    }
                    break;
                case (int)Enum_TipoMostra_FiltroRicerca.Esercizi:
                case (int)Enum_TipoMostra_FiltroRicerca.PianoColturale:
                case (int)Enum_TipoMostra_FiltroRicerca.PianoColturaleBudget:
                    foreach (DataRow row in criteriRicerca_OUT.result.Rows)
                    {
                        int Veg_Cod = (int)(row["Veg_Cod"] != DBNull.Value ? row["Veg_Cod"] : 0);
                        row["chiave"] = row["Piva"].ToString() + '_' + row["Sa_Cod"].ToString() + '_' + row["Appezza"].ToString() + '_' + row["Id_Reg"].ToString() + '_' + Veg_Cod.ToString() + '_' + row["Progetto_Cod"].ToString() + (TipoMostra == (int)Enum_TipoMostra_FiltroRicerca.PianoColturaleBudget ? "_" + row["Id_Budget"].ToString() : "");

                        row["chiave_GIAS"] = row["Piva"].ToString() + '-' + row["Sa_Cod"].ToString() + '-' + row["Appezza"].ToString() + '-' + row["Id_Reg"].ToString() + '-' + row["Progetto_Cod"].ToString() + (TipoMostra == (int)Enum_TipoMostra_FiltroRicerca.PianoColturaleBudget ? "_" + row["Id_Budget"].ToString() : "");
                    }
                    break;
                case (int)Enum_TipoMostra_FiltroRicerca.Fabbricati:
                    foreach (DataRow row in criteriRicerca_OUT.result.Rows)
                    {
                        row["chiave"] = row["Piva"].ToString() + '_' + row["Sa_Cod"].ToString() + '_' + row["Fabbricato_Cod"].ToString();

                        row["chiave_GIAS"] = row["Piva"].ToString() + '-' + row["Sa_Cod"].ToString() + '-' + row["Fabbricato_Cod"].ToString();
                    }
                    break;
                case (int)Enum_TipoMostra_FiltroRicerca.Movimenti:
                    foreach (DataRow row in criteriRicerca_OUT.result.Rows)
                    {
                        int Veg_Cod = (int)(row["Veg_Cod"] != DBNull.Value ? row["Veg_Cod"] : 0);
                        row["chiave"] = row["Piva"].ToString() + '_' + row["Sa_Cod"].ToString() + '_' + row["Appezza"].ToString() + '_' + row["Id_Reg"].ToString() + '_' + Veg_Cod.ToString() + '_' + row["Id_Agenda"].ToString();

                        row["chiave_GIAS"] = row["Piva"].ToString() + '-' + row["Sa_Cod"].ToString() + '-' + row["Appezza"].ToString() + '-' + row["Id_Reg"].ToString() + '-' + row["Id_Agenda"].ToString();
                    }
                    break;
            }
        }

        private static void AggiungiColonnaBudget(int Id_Budget, ref CriteriRicerca_OUT criteriRicerca_OUT)
        {
            criteriRicerca_OUT.result.Columns.Add("Id_Budget", typeof(string));

            foreach (DataRow row in criteriRicerca_OUT.result.Rows)
            {
                row["Id_Budget"] = Id_Budget;
            }
        }

        private static void RimuoviImpiantiDoppi(ref CriteriRicerca_OUT criteriRicerca_OUT)
        {

            List<string> chiaviImpianto = new();
            List<DataRow> rowsToRemove = new();

            foreach (DataRow row in criteriRicerca_OUT.result.Rows)
            {
                string chiaveImpianto = row["Piva"].ToString() + row["Sa_Cod"].ToString() + row["Appezza"].ToString() + row["Id_Reg"].ToString();
                if (chiaviImpianto.Contains(chiaveImpianto))
                    rowsToRemove.Add(row);
                else
                    chiaviImpianto.Add(chiaveImpianto);
            }

            foreach (DataRow row in rowsToRemove)
            {
                criteriRicerca_OUT.result.Rows.Remove(row);
            }
        }
        private static void PredisponiDatiMancantiEsercizio(CriteriRicercaExtended criteriRicerca, ref CriteriRicerca_OUT criteriRicerca_OUT)
        {
            criteriRicerca_OUT.result.Columns.Add("Numero_Totale_Piante", typeof(double));
            criteriRicerca_OUT.result.Columns.Add("Resa_Totale_Prevista", typeof(double));

            foreach (DataRow row in criteriRicerca_OUT.result.Rows)
            {
                if (criteriRicerca.FattoreConversione > 1)
                {
                    if (row["Sup_Imp_Acri"] != DBNull.Value)
                    {
                        if (row["P_HA"] != DBNull.Value)
                        {
                            row["Numero_Totale_Piante"] = (double)row["Sup_Imp_Acri"] * (double)row["P_HA"];
                        }
                        if (row["Produzione_Prevista"] != DBNull.Value)
                        {
                            row["Resa_Totale_Prevista"] = (double)row["Sup_Imp_Acri"] * (double)row["Produzione_Prevista"];
                        }
                    }
                }
                else
                {
                    if (row["Sup_Imp"] != DBNull.Value)
                    {
                        if (row["P_HA"] != DBNull.Value)
                        {
                            row["Numero_Totale_Piante"] = (double)row["Sup_Imp"] * (double)row["P_HA"];
                        }
                        if (row["Produzione_Prevista"] != DBNull.Value)
                        {
                            row["Resa_Totale_Prevista"] = (double)row["Sup_Imp"] * (double)row["Produzione_Prevista"];
                        }
                    }
                }
            }
        }
        #endregion

    }
}
