using AgronicaCoreDTOStd.InData.FiltroRicerca;
using AgronicaNetCore.Base.Models;
using Microsoft.VisualBasic;
using System.Data;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;
using static AgronicaNetCore.Base.Constants.CostantiPersonalizzate;
using static AgronicaNetCore.Base.Constants.ELEM_COD;
using AgronicaNetCore.Base.Constants;
using AgronicaCoreModelsSTD.Models.ConfiguratoreFiltroRicerca;
using AgronicaCoreDTOStd.OutData.FiltroRicerca;
using OutData.Kendo;
using AgronicaCoreModelsSTD.anagrafiche;

using AgronicaNetCore.FiltroRicerca.DAL.Utils.Utils;
using static AgronicaNetCore.FiltroRicerca.DAL.Utils.Utils.Utils;
using System.Diagnostics;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.FiltroRicerca.DAL.Resources;

namespace AgronicaNetCore.FiltroRicerca.DAL.DataLayer.FiltroRicerca
{

    public class FiltroRicerca : BaseDALFiltroRicerca, IFiltroRicerca
    {
        #region "util var"
        private bool _onLoad = true;
        private string _modalitaBudget = "";
        private int _IdBudget = 0;
        private int _selectTopRows = 0;
        private decimal _fattoreConversione = 1;
        private bool _SQLCompatibility = false;

        [Obsolete("Con query performanti e strutturate in modo corretto non è necessario l'attributo, anzi peggiora le performance", true)]
        private bool _USE_FORCE_LEGACY_CARDINALITY_ESTIMATION = false;

        private bool _checkVisibilitaAziende = false;
        private bool _checkVisibilitaCentriAziendali = false;

        private int columnOrderIndex = 1;

        private StringBuilder _stbDropTemporaryTable = new();

        private string _utenti_DB_name = "";

        #endregion

        ConfiguratoreFiltroRicerca _configuratore;

        public FiltroRicerca(IServiceProvider serviceProvider, IStringLocalizer<Messages> localizer) : base(serviceProvider, localizer)
        {
        }

        public async Task<CriteriRicerca_OUT> GetResultAsync(CriteriRicercaExtended criteriRicerca, ConfiguratoreFiltroRicerca configuratore, string utenti_DB_name, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {

            //Con query performanti e strutturate in modo corretto non è necessario l'attributo, anzi peggiora le performance
            //_USE_FORCE_LEGACY_CARDINALITY_ESTIMATION = criteriRicerca.USE_FORCE_LEGACY_CARDINALITY_ESTIMATION;
            _SQLCompatibility = criteriRicerca.SQLCompatibility;
            _onLoad = criteriRicerca.OnLoad;
            _fattoreConversione = criteriRicerca.FattoreConversione;

            _checkVisibilitaAziende = criteriRicerca.CheckVisibilitaAziende;
            _checkVisibilitaCentriAziendali = criteriRicerca.CheckVisibilitaCentriAziendali;

            _utenti_DB_name = utenti_DB_name;

            if (criteriRicerca.TipoMostra == (int)Enum_TipoMostra_FiltroRicerca.PianoColturaleBudget)
            {
                _IdBudget = criteriRicerca.Id_Budget;
                _modalitaBudget = "Budget_";

                configuratore.joinBudget = true;

                if (_IdBudget == 0 && !_onLoad)
                    throw new Exception("Selezionare un budget.");
            }

            _selectTopRows = criteriRicerca.selectTopRows;

            StbFiltri stbFiltri = new();

            List<KendoColumn> kendoColumns = new();
            DataTable result = new();
            Dictionary<string, object> parSql = new();
            Dictionary<string, Dictionary<Type, List<object>>> parSqlIn = new();
            _configuratore = configuratore;

            CreaFiltroDati(criteriRicerca, parSql, parSqlIn, stbFiltri);

            result = await BuildQueryAsync(kendoColumns, parSql, parSqlIn, stbFiltri, objParametriServer, objParametriUtenti);

            return new() { result = result, kendoColumns = kendoColumns.ToArray() };
        }


        #region "Build Query"
        private async Task<DataTable> BuildQueryAsync(List<KendoColumn> kendoColumns,
                                                      Dictionary<string, object> parSql,
                                                      Dictionary<string, Dictionary<Type, List<object>>> parSqlIn,
                                                      StbFiltri stbFiltri,
                                                      AgronicaCoreParametriServer objParametriServer,
                                                      AgronicaCoreParametriUtenti objParametriUtenti)

        {

            var stbQuery = new StringBuilder();

            if (Debugger.IsAttached)
                stbQuery.AppendLine(GetTimestamp(onlyDeclare: true).ToString());

            stbQuery.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ");

            #region "BUILD TT"

            stbQuery.AppendLine(GetUtenti_TT(kendoColumns, objParametriUtenti).ToString());

            //Le imprese base vengono sempre estratte
            stbQuery.AppendLine(await GetAziendeBase_TTAsync(kendoColumns, stbFiltri, parSql, objParametriServer, objParametriUtenti));

            if (_configuratore.AziendeDettagli)
            {
                stbQuery.AppendLine(GetAziendeDettagli_TT(ref kendoColumns, stbFiltri, objParametriServer));
            }

            if (_configuratore.joinCentro)
            {
                stbQuery.AppendLine(await GetCentriAziendaliBase_TTAsync(kendoColumns, stbFiltri, parSql, objParametriServer, objParametriUtenti));

                if (_configuratore.CentriAziendaliDettagli)
                {
                    stbQuery.AppendLine(GetCentriAziendaliDettagli_TT(ref kendoColumns, stbFiltri, objParametriServer));
                }
            }

            if (_configuratore.joinCampo)
            {
                stbQuery.AppendLine(await GetCampiBase_TTAsync(kendoColumns, stbFiltri, objParametriServer, objParametriUtenti));

                if (_configuratore.CampiDettagli)
                {
                    stbQuery.AppendLine(GetCampiDettagli_TT(ref kendoColumns, stbFiltri, objParametriServer));
                }
            }

            if (_configuratore.joinAppezzamento)
            {
                stbQuery.AppendLine(await GetAppezzamentiBase_TTAsync(kendoColumns, stbFiltri, objParametriServer, objParametriUtenti));

                if (_configuratore.AppezzamentiDettagli)
                {
                    stbQuery.AppendLine(GetAppezzamentiDettagli_TT(ref kendoColumns, stbFiltri, objParametriServer));
                }
            }

            if (_configuratore.joinImpianto)
            {
                stbQuery.AppendLine(await GetImpiantiBase_TTAsync(kendoColumns, stbFiltri, objParametriServer, objParametriUtenti));

                if (_configuratore.ImpiantiDettagli)
                {
                    stbQuery.AppendLine(GetImpiantiDettagli_TT(ref kendoColumns, stbFiltri, objParametriServer));
                }
            }

            if (_configuratore.joinEsercizio)
            {
                stbQuery.AppendLine(await GetEserciziBase_TTAsync(kendoColumns, stbFiltri, objParametriServer, objParametriUtenti));

                if (_configuratore.EserciziDettagli)
                {
                    stbQuery.AppendLine(GetEserciziDettagli_TT(ref kendoColumns, stbFiltri, objParametriServer));
                }
            }

            if (_configuratore.joinFabbricato)
            {
                stbQuery.AppendLine(await GetFabbricatiBase_TTAsync(kendoColumns, stbFiltri, objParametriServer, objParametriUtenti));

                if (_configuratore.FabbricatiDettagli)
                {
                    stbQuery.AppendLine(GetFabbricatiDettagli_TT(ref kendoColumns, stbFiltri, objParametriServer));
                }
            }

            if (_configuratore.isMostraMovimenti)
            {
                stbQuery.AppendLine(GetMovimentiBase_TT(ref kendoColumns, stbFiltri, objParametriUtenti));
            }

            #endregion

            stbQuery.AppendLine($" SELECT DISTINCT {(_selectTopRows > 0 ? $"TOP {_selectTopRows + 1}" : "")}");
            stbQuery.AppendLine("        AziendeBase_TT.* ");

            #region "BUILD SELECT LIST"
            if (_configuratore.AziendeDettagli)
                stbQuery.AppendLine("      , AziendeDettagli_TT.* ");

            if (_configuratore.needColumnsCentriAziendali)
            {
                stbQuery.AppendLine("      , CentriAziendaliBase_TT.* ");

                if (_configuratore.CentriAziendaliDettagli)
                    stbQuery.AppendLine("      , CentriAziendaliDettagli_TT.* ");
            }

            if (_configuratore.needColumnsCampi)
            {
                stbQuery.AppendLine("      , CampiBase_TT.* ");

                if (_configuratore.CampiDettagli)
                    stbQuery.AppendLine("      , CampiDettagli_TT.* ");
            }

            if (_configuratore.needColumnsAppezzamenti)
            {
                stbQuery.AppendLine("      , AppezzamentiBase_TT.* ");

                if (_configuratore.AppezzamentiDettagli)
                    stbQuery.AppendLine("      , AppezzamentiDettagli_TT.* ");
            }

            if (_configuratore.needColumnsImpianti)
            {
                stbQuery.AppendLine("      , ImpiantiBase_TT.* ");

                if (_configuratore.ImpiantiDettagli)
                    stbQuery.AppendLine("      , ImpiantiDettagli_TT.* ");
            }

            if (_configuratore.needColumnsEsercizi)
            {
                stbQuery.AppendLine("      , EserciziBase_TT.* ");

                if (_configuratore.EserciziDettagli)
                    stbQuery.AppendLine("      , EserciziDettagli_TT.* ");
            }

            if (_configuratore.needColumnsFabbricati)
            {
                stbQuery.AppendLine("      , FabbricatiBase_TT.* ");

                if (_configuratore.FabbricatiDettagli)
                    stbQuery.AppendLine("      , FabbricatiDettagli_TT.* ");
            }

            if (_configuratore.isMostraMovimenti)
            {
                stbQuery.AppendLine("      , MovimentiBase_TT.* ");
            }

            #endregion

            if (_configuratore.isMostraMovimenti)
                //Per questioni di performance, i movimenti hanno un giro di join diverso
                stbQuery.AppendLine(" FROM #MovimentiBase_TT MovimentiBase_TT ");
            else
                stbQuery.AppendLine(" FROM #AziendeBase_TT AziendeBase_TT ");

            #region "BUILD JOIN"
            stbQuery.AppendLine(BuildJoin().ToString());
            #endregion

            #region "BUILD WHERE"
            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
                stbQuery.AppendLine(" WHERE 1 = 1 ");
            #endregion

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("Execute Finale", total: true).ToString());
                stbQuery.AppendLine(_stbDropTemporaryTable.ToString());
            }

            return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql, parSqlIn);
        }

        private StringBuilder BuildJoin()
        {
            StringBuilder stbJoin = new();

            if (_configuratore.isMostraMovimenti)
            {
                //Per questioni di performance, i movimenti hanno un giro di join diverso
                //Andiamo sempre in join con tutti i livelli
                //L'esercizio va in join anche sulla data del movimento

                if (_configuratore.joinEsercizio)
                {
                    stbJoin.AppendLine(" JOIN #EserciziBase_TT EserciziBase_TT ON ");
                    stbJoin.AppendLine("     EserciziBase_TT.[keyPiva] = MovimentiBase_TT.[keyPiva] ");
                    stbJoin.AppendLine(" AND EserciziBase_TT.[keySaCod] = MovimentiBase_TT.[keySaCod] ");
                    stbJoin.AppendLine(" AND EserciziBase_TT.[keyAppezza] = MovimentiBase_TT.[keyAppezza] ");
                    stbJoin.AppendLine(" AND EserciziBase_TT.[keyIdReg] = MovimentiBase_TT.[keyIdReg] ");
                    stbJoin.AppendLine(" AND EserciziBase_TT.Validita_Inizio_Esercizio_JoinMovimenti <= MovimentiBase_TT.Data_Movimento ");
                    stbJoin.AppendLine(" AND EserciziBase_TT.Validita_Fine_Esercizio_JoinMovimenti >= MovimentiBase_TT.Data_Movimento ");

                    if (_configuratore.EserciziDettagli)
                    {
                        stbJoin.AppendLine(" LEFT JOIN #EserciziDettagli_TT EserciziDettagli_TT ON ");
                        stbJoin.AppendLine("     EserciziDettagli_TT.[keyPiva] = EserciziBase_TT.[keyPiva] ");
                        stbJoin.AppendLine(" AND EserciziDettagli_TT.[keySaCod] = EserciziBase_TT.[keySaCod] ");
                        stbJoin.AppendLine(" AND EserciziDettagli_TT.[keyAppezza] = EserciziBase_TT.[keyAppezza] ");
                        stbJoin.AppendLine(" AND EserciziDettagli_TT.[keyIdReg] = EserciziBase_TT.[keyIdReg] ");
                        stbJoin.AppendLine(" AND EserciziDettagli_TT.[keyProgettoCod] = EserciziBase_TT.[keyProgettoCod] ");
                    }
                }

                stbJoin.AppendLine(" JOIN #ImpiantiBase_TT ImpiantiBase_TT ON ");
                stbJoin.AppendLine("     ImpiantiBase_TT.[keyPiva] = MovimentiBase_TT.[keyPiva] ");
                stbJoin.AppendLine(" AND ImpiantiBase_TT.[keySaCod] = MovimentiBase_TT.[keySaCod] ");
                stbJoin.AppendLine(" AND ImpiantiBase_TT.[keyAppezza] = MovimentiBase_TT.[keyAppezza] ");
                stbJoin.AppendLine(" AND ImpiantiBase_TT.[keyIdReg] = MovimentiBase_TT.[keyIdReg] ");

                if (_configuratore.ImpiantiDettagli)
                {
                    stbJoin.AppendLine(" LEFT JOIN #ImpiantiDettagli_TT ImpiantiDettagli_TT ON ");
                    stbJoin.AppendLine("     ImpiantiDettagli_TT.[keyPiva] = ImpiantiBase_TT.[keyPiva] ");
                    stbJoin.AppendLine(" AND ImpiantiDettagli_TT.[keySaCod] = ImpiantiBase_TT.[keySaCod] ");
                    stbJoin.AppendLine(" AND ImpiantiDettagli_TT.[keyAppezza] = ImpiantiBase_TT.[keyAppezza] ");
                    stbJoin.AppendLine(" AND ImpiantiDettagli_TT.[keyIdReg] = ImpiantiBase_TT.[keyIdReg] ");
                }

                stbJoin.AppendLine(" JOIN #AppezzamentiBase_TT AppezzamentiBase_TT ON ");
                stbJoin.AppendLine("     AppezzamentiBase_TT.[keyPiva] = ImpiantiBase_TT.[keyPiva] ");
                stbJoin.AppendLine(" AND AppezzamentiBase_TT.[keySaCod] = ImpiantiBase_TT.[keySaCod] ");
                stbJoin.AppendLine(" AND AppezzamentiBase_TT.[keyAppezza] = ImpiantiBase_TT.[keyAppezza] ");

                if (_configuratore.AppezzamentiDettagli)
                {
                    stbJoin.AppendLine(" LEFT JOIN #AppezzamentiDettagli_TT AppezzamentiDettagli_TT ON ");
                    stbJoin.AppendLine("     AppezzamentiDettagli_TT.[keyPiva] = AppezzamentiBase_TT.[keyPiva] ");
                    stbJoin.AppendLine(" AND AppezzamentiDettagli_TT.[keySaCod] = AppezzamentiBase_TT.[keySaCod] ");
                    stbJoin.AppendLine(" AND AppezzamentiDettagli_TT.[keyAppezza] = AppezzamentiBase_TT.[keyAppezza] ");
                }

                stbJoin.AppendLine(" LEFT JOIN #CampiBase_TT CampiBase_TT ON ");
                stbJoin.AppendLine("     CampiBase_TT.[keyPiva] = AppezzamentiBase_TT.[keyPiva] ");
                stbJoin.AppendLine(" AND CampiBase_TT.[keySaCod] = AppezzamentiBase_TT.[keySaCod] ");
                stbJoin.AppendLine(" AND CampiBase_TT.[keyCampoCod] = AppezzamentiBase_TT.[keyCampoCod] ");

                if (_configuratore.CampiDettagli)
                {
                    stbJoin.AppendLine(" LEFT JOIN #CampiDettagli_TT CampiDettagli_TT ON ");
                    stbJoin.AppendLine("     CampiDettagli_TT.[keyPiva] = CampiBase_TT.[keyPiva] ");
                    stbJoin.AppendLine(" AND CampiDettagli_TT.[keySaCod] = CampiBase_TT.[keySaCod] ");
                    stbJoin.AppendLine(" AND CampiDettagli_TT.[keyCampoCod] = CampiBase_TT.[keyCampoCod] ");
                }

                stbJoin.AppendLine(" JOIN #CentriAziendaliBase_TT CentriAziendaliBase_TT ON ");
                stbJoin.AppendLine("     CentriAziendaliBase_TT.[keyPiva] = AppezzamentiBase_TT.[keyPiva] ");
                stbJoin.AppendLine(" AND CentriAziendaliBase_TT.[keySaCod] = AppezzamentiBase_TT.[keySaCod] ");

                if (_configuratore.CentriAziendaliDettagli)
                {
                    stbJoin.AppendLine(" LEFT JOIN #CentriAziendaliDettagli_TT CentriAziendaliDettagli_TT ON ");
                    stbJoin.AppendLine("     CentriAziendaliBase_TT.[keyPiva] = CentriAziendaliDettagli_TT.[keyPiva] ");
                    stbJoin.AppendLine(" AND CentriAziendaliBase_TT.[keySaCod] = CentriAziendaliDettagli_TT.[keySaCod] ");
                }

                stbJoin.AppendLine(" JOIN #AziendeBase_TT AziendeBase_TT  ON ");
                stbJoin.AppendLine("     AziendeBase_TT.[keyPiva] = CentriAziendaliBase_TT.[keyPiva] ");

                if (_configuratore.AziendeDettagli)
                {
                    stbJoin.AppendLine(" LEFT JOIN #AziendeDettagli_TT AziendeDettagli_TT ON ");
                    stbJoin.AppendLine("     AziendeDettagli_TT.[keyPiva] = AziendeBase_TT.[keyPiva] ");
                }
            }
            else
            {
                if (_configuratore.AziendeDettagli)
                {
                    stbJoin.AppendLine(" LEFT JOIN #AziendeDettagli_TT AziendeDettagli_TT ON ");
                    stbJoin.AppendLine("     AziendeDettagli_TT.[keyPiva] = AziendeBase_TT.[keyPiva] ");
                }

                if (_configuratore.joinCentro)
                {
                    stbJoin.AppendLine(" JOIN #CentriAziendaliBase_TT CentriAziendaliBase_TT ON ");
                    stbJoin.AppendLine("     CentriAziendaliBase_TT.[keyPiva] = AziendeBase_TT.[keyPiva] ");

                    if (_configuratore.CentriAziendaliDettagli)
                    {
                        stbJoin.AppendLine(" LEFT JOIN #CentriAziendaliDettagli_TT CentriAziendaliDettagli_TT ON ");
                        stbJoin.AppendLine("     CentriAziendaliBase_TT.[keyPiva] = CentriAziendaliDettagli_TT.[keyPiva] ");
                        stbJoin.AppendLine(" AND CentriAziendaliBase_TT.[keySaCod] = CentriAziendaliDettagli_TT.[keySaCod] ");
                    }
                }

                if (_configuratore.joinAppezzamento)
                {
                    stbJoin.AppendLine(" JOIN #AppezzamentiBase_TT AppezzamentiBase_TT ON ");
                    stbJoin.AppendLine("     AppezzamentiBase_TT.[keyPiva] = AziendeBase_TT.[keyPiva] ");
                    stbJoin.AppendLine(" AND AppezzamentiBase_TT.[keySaCod] = CentriAziendaliBase_TT.[keySaCod] ");

                    if (_configuratore.AppezzamentiDettagli)
                    {
                        stbJoin.AppendLine(" LEFT JOIN #AppezzamentiDettagli_TT AppezzamentiDettagli_TT ON ");
                        stbJoin.AppendLine("     AppezzamentiDettagli_TT.[keyPiva] = AppezzamentiBase_TT.[keyPiva] ");
                        stbJoin.AppendLine(" AND AppezzamentiDettagli_TT.[keySaCod] = AppezzamentiBase_TT.[keySaCod] ");
                        stbJoin.AppendLine(" AND AppezzamentiDettagli_TT.[keyAppezza] = AppezzamentiBase_TT.[keyAppezza] ");
                    }
                }

                if (_configuratore.joinCampo)
                {
                    stbJoin.AppendLine($" {(_configuratore.isMostraCampi ? "INNER" : "LEFT")} JOIN #CampiBase_TT CampiBase_TT ON ");
                    stbJoin.AppendLine("     CampiBase_TT.[keyPiva] = AziendeBase_TT.[keyPiva] ");
                    stbJoin.AppendLine(" AND CampiBase_TT.[keySaCod] = CentriAziendaliBase_TT.[keySaCod] ");
                    if (_configuratore.joinAppezzamento)
                        stbJoin.AppendLine(" AND CampiBase_TT.[keyCampoCod] = AppezzamentiBase_TT.[keyCampoCod] ");

                    if (_configuratore.CampiDettagli)
                    {
                        stbJoin.AppendLine(" LEFT JOIN #CampiDettagli_TT CampiDettagli_TT ON ");
                        stbJoin.AppendLine("     CampiDettagli_TT.[keyPiva] = CampiBase_TT.[keyPiva] ");
                        stbJoin.AppendLine(" AND CampiDettagli_TT.[keySaCod] = CampiBase_TT.[keySaCod] ");
                        stbJoin.AppendLine(" AND CampiDettagli_TT.[keyCampoCod] = CampiBase_TT.[keyCampoCod] ");
                    }
                }

                if (_configuratore.joinImpianto)
                {
                    stbJoin.AppendLine(" JOIN #ImpiantiBase_TT ImpiantiBase_TT ON ");
                    stbJoin.AppendLine("     ImpiantiBase_TT.[keyPiva] = AziendeBase_TT.[keyPiva] ");
                    stbJoin.AppendLine(" AND ImpiantiBase_TT.[keySaCod] = CentriAziendaliBase_TT.[keySaCod] ");
                    stbJoin.AppendLine(" AND ImpiantiBase_TT.[keyAppezza] = AppezzamentiBase_TT.[keyAppezza] ");

                    if (_configuratore.ImpiantiDettagli)
                    {
                        stbJoin.AppendLine(" LEFT JOIN #ImpiantiDettagli_TT ImpiantiDettagli_TT ON ");
                        stbJoin.AppendLine("     ImpiantiDettagli_TT.[keyPiva] = ImpiantiBase_TT.[keyPiva] ");
                        stbJoin.AppendLine(" AND ImpiantiDettagli_TT.[keySaCod] = ImpiantiBase_TT.[keySaCod] ");
                        stbJoin.AppendLine(" AND ImpiantiDettagli_TT.[keyAppezza] = ImpiantiBase_TT.[keyAppezza] ");
                        stbJoin.AppendLine(" AND ImpiantiDettagli_TT.[keyIdReg] = ImpiantiBase_TT.[keyIdReg] ");
                    }
                }

                if (_configuratore.joinEsercizio)
                {
                    stbJoin.AppendLine(" JOIN #EserciziBase_TT EserciziBase_TT ON ");
                    stbJoin.AppendLine("     EserciziBase_TT.[keyPiva] = AziendeBase_TT.[keyPiva] ");
                    stbJoin.AppendLine(" AND EserciziBase_TT.[keySaCod] = CentriAziendaliBase_TT.[keySaCod] ");
                    stbJoin.AppendLine(" AND EserciziBase_TT.[keyAppezza] = AppezzamentiBase_TT.[keyAppezza] ");
                    stbJoin.AppendLine(" AND EserciziBase_TT.[keyIdReg] = ImpiantiBase_TT.[keyIdReg] ");

                    if (_configuratore.EserciziDettagli)
                    {
                        stbJoin.AppendLine(" LEFT JOIN #EserciziDettagli_TT EserciziDettagli_TT ON ");
                        stbJoin.AppendLine("     EserciziDettagli_TT.[keyPiva] = EserciziBase_TT.[keyPiva] ");
                        stbJoin.AppendLine(" AND EserciziDettagli_TT.[keySaCod] = EserciziBase_TT.[keySaCod] ");
                        stbJoin.AppendLine(" AND EserciziDettagli_TT.[keyAppezza] = EserciziBase_TT.[keyAppezza] ");
                        stbJoin.AppendLine(" AND EserciziDettagli_TT.[keyIdReg] = EserciziBase_TT.[keyIdReg] ");
                        stbJoin.AppendLine(" AND EserciziDettagli_TT.[keyProgettoCod] = EserciziBase_TT.[keyProgettoCod] ");
                    }
                }

                if (_configuratore.joinFabbricato)
                {
                    stbJoin.AppendLine(" JOIN #FabbricatiBase_TT FabbricatiBase_TT ON ");
                    stbJoin.AppendLine("     FabbricatiBase_TT.[keyPiva] = AziendeBase_TT.[keyPiva] ");
                    stbJoin.AppendLine(" AND FabbricatiBase_TT.[keySaCod] = CentriAziendaliBase_TT.[keySaCod] ");

                    if (_configuratore.FabbricatiDettagli)
                    {
                        stbJoin.AppendLine(" LEFT JOIN #FabbricatiDettagli_TT FabbricatiDettagli_TT ON ");
                        stbJoin.AppendLine("     FabbricatiDettagli_TT.[keyPiva] = FabbricatiBase_TT.[keyPiva] ");
                        stbJoin.AppendLine(" AND FabbricatiDettagli_TT.[keySaCod] = FabbricatiBase_TT.[keySaCod] ");
                        stbJoin.AppendLine(" AND FabbricatiDettagli_TT.[keyFabbricatoCod] = FabbricatiBase_TT.[keyFabbricatoCod] ");
                    }
                }

                if (_configuratore.isMostraMovimenti)
                {
                    stbJoin.AppendLine(" JOIN #MovimentiBase_TT MovimentiBase_TT ON ");
                    stbJoin.AppendLine("     MovimentiBase_TT.[keyPiva] = AziendeBase_TT.[keyPiva] ");
                    stbJoin.AppendLine(" AND MovimentiBase_TT.[keySaCod] = CentriAziendaliBase_TT.[keySaCod] ");
                    stbJoin.AppendLine(" AND MovimentiBase_TT.[keyAppezza] = AppezzamentiBase_TT.[keyAppezza] ");
                    stbJoin.AppendLine(" AND MovimentiBase_TT.[keyIdReg] = ImpiantiBase_TT.[keyIdReg] ");
                }
            }
            return stbJoin;
        }
        #endregion

        #region "Get Base & Dettagli"

        #region "Aziende"
        private async Task<string> GetAziendeBase_TTAsync(List<KendoColumn> kendoColumns,
                                                          StbFiltri StbFiltri,
                                                          Dictionary<string, object> parSql,
                                                          AgronicaCoreParametriServer objParametriServer,
                                                          AgronicaCoreParametriUtenti objParametriUtenti)
        {

            #region "KENDO COLUMNS"
            if (_configuratore.CaricaImpreseReferenti)
            {
                kendoColumns.Add(new KendoColumn { Field = "ImpreseReferenti", Title = "ImpreseReferenti", DataType = "string", ColumnOrder = 0 });
                kendoColumns.Add(new KendoColumn { Field = "piva_padri", Title = "PIVAImpreseReferenti", DataType = "string", ColumnOrder = 0, Hidden = true });
                kendoColumns.Add(new KendoColumn { Field = "Foglia", Title = "Foglia", DataType = "string", ColumnOrder = 0, Display = false });
                kendoColumns.Add(new KendoColumn { Field = "Livello", Title = "Livello", DataType = "string", ColumnOrder = 0, Display = false });
            }

            kendoColumns.Add(new KendoColumn { Field = "Piva", Title = "PartitaIvaAbbr", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex), Display = false });
            kendoColumns.Add(new KendoColumn { Field = "PivaReale", Title = "PartitaIvaAbbr", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
            kendoColumns.Add(new KendoColumn { Field = "RagioneSociale", Title = "RagioneSociale", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
            kendoColumns.Add(new KendoColumn { Field = "CUAA", Title = "CodiceUnicoAziendaAgricolaSigla", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
            kendoColumns.Add(new KendoColumn { Field = "GruppoRaccoltaAzienda", Title = (_configuratore.isMostraEsercizi ? "GruppoRaccoltaAzienda" : "GruppoRaccolta"), DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
            kendoColumns.Add(new KendoColumn { Field = "TipoImpresaGerarchia", Title = "TipoImpresa", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
            kendoColumns.Add(new KendoColumn { Field = "TipoImpresaGerarchia_Int", Title = "TipoImpresaInt", DataType = "number", Hidden = true, ColumnOrder = 0 });
            kendoColumns.Add(new KendoColumn { Field = "FormaGiuridica", Title = "FormaGiuridica", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

            if (_configuratore.CaricaLegaleRappresentante)
            {
                kendoColumns.Add(new KendoColumn { Field = "LegaleRappresentante", Title = "LegaleRappresentante", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "LegaleRappresentante_DataNascita", Hidden = true, Title = "LegaleRappresentanteDataNascita", DataType = "date", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "LegaleRappresentante_CodiceFiscale", Hidden = true, Title = "LegaleRappresentanteCodiceFiscale", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "LegaleRappresentante_LuogoNascita", Hidden = true, Title = "LegaleRappresentanteLuogoNascita", DataType = "date", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
            }

            if (_configuratore.CaricaIndirizzoAzienda)
            {
                kendoColumns.Add(new KendoColumn { Field = "Indirizzo_Azienda", Title = (_configuratore.specificaIndirizzo ? "IndirizzoAzienda" : "Indirizzo"), DataType = "string", Hidden = !_configuratore.isMostraAziende, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Frazione_Azienda", Title = (_configuratore.specificaIndirizzo ? "FrazioneAzienda" : "Frazione"), DataType = "string", Hidden = !_configuratore.isMostraAziende, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "CAP_Azienda", Title = (_configuratore.specificaIndirizzo ? "CAPAzienda" : "CAP"), DataType = "string", Hidden = !_configuratore.isMostraAziende, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                kendoColumns.Add(new KendoColumn { Field = "Stato_Azienda", Title = (_configuratore.specificaIndirizzo ? "StatoAzienda" : "Stato"), DataType = "string", Hidden = !_configuratore.isMostraAziende, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Regione_Azienda", Title = (_configuratore.specificaIndirizzo ? "RegioneAzienda" : "Regione"), DataType = "string", Hidden = !_configuratore.isMostraAziende, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Provincia_Azienda", Title = (_configuratore.specificaIndirizzo ? "ProvinciaAzienda" : "Provincia"), DataType = "string", Hidden = !_configuratore.isMostraAziende, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Comune_Azienda", Title = (_configuratore.specificaIndirizzo ? "ComuneAzienda" : "Comune"), DataType = "string", Hidden = !_configuratore.isMostraAziende, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Sigla_Provincia_Azienda", Title = "SiglaProvinciaAzienda", DataType = "string", Hidden = true, ColumnOrder = 0 });
                kendoColumns.Add(new KendoColumn { Field = "REG_Regione_Azienda", Title = "REGRegioneAzienda", DataType = "number", Hidden = true, ColumnOrder = 0 });
                kendoColumns.Add(new KendoColumn { Field = "StatoISO_Azienda", Title = "StatoISOAzienda", DataType = "string", Hidden = true, ColumnOrder = 0 });
            }

            if (_configuratore.joinGerarchiaBase)
            {
                kendoColumns.Add(new KendoColumn { Field = "Foglia", Title = "Foglia", DataType = "number", Hidden = true, ColumnOrder = 0 });
                kendoColumns.Add(new KendoColumn { Field = "Livello", Title = "Livello", DataType = "number", Hidden = true, ColumnOrder = 0 });
                kendoColumns.Add(new KendoColumn { Field = "Padre", Title = "Padre", DataType = "string", Hidden = true, ColumnOrder = 0 });
            }

            if (_configuratore.CaricaDatiServizi)
            {
                kendoColumns.Add(new KendoColumn { Field = "Servizio", Title = "Servizio", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "StatoPratica", Title = "StatoPratica", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "AnnoPratica", Title = "AnnoPratica", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "NumeroPratica", Title = "NumeroPratica", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
            }
            #endregion

            #region "QUERY"  
            var stbQuery = new StringBuilder();
            var stbJoinUtenti = new StringBuilder();
            var stbJoinVisibilitaAppoggio = new StringBuilder();

            if (_checkVisibilitaAziende)
                stbQuery.AppendLine(GetUtentiVisibilitaAppoggio_TT((int)Enum_TipoEntita.Impresa, stbJoinVisibilitaAppoggio, ref parSql, objParametriServer).ToString());

            if (_configuratore.joinImpreseReferenti)
                stbQuery.AppendLine(GetGerarchiaImprese_TT(StbFiltri).ToString());
            if (_configuratore.CaricaLegaleRappresentante)
                stbQuery.AppendLine(GetLegaleRappresentante_TT().ToString());
            if (_configuratore.joinServizio)
                stbQuery.AppendLine(GetServizi_TT(StbFiltri).ToString());
            if (_configuratore.joinOperazionixAzienda)
                stbQuery.AppendLine(GetOperazioniAgenda_TT(StbFiltri).ToString());

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("        Imprese.PIVA AS [keyPiva] ");

            if (_configuratore.CaricaImpreseReferenti)
                stbQuery.AppendLine("       , ImpreseReferenti, piva_padri, Foglia, Livello ");

            stbQuery.AppendLine("       , Imprese.Piva ");
            stbQuery.AppendLine("       , CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.Piva ELSE Imprese.partitaIvaReale END PivaReale ");
            stbQuery.AppendLine("       , Imprese.Rag_Soc AS RagioneSociale ");

            stbQuery.AppendLine("       , ISNULL(CUAA.Val_Cod, '') AS CUAA ");

            stbQuery.AppendLine("       , ISNULL(Gruppi_Raccolta.GruppoRaccolta_Des, '') AS GruppoRaccoltaAzienda ");

            stbQuery.AppendLine("       , CASE Imprese.TipoImpresaGerarchia ");
            stbQuery.AppendLine($"             WHEN {(int)Enum_TipoImpresaGerarchia.Impresa} THEN '{_localizer["Impresa"]}'");
            stbQuery.AppendLine($"             WHEN {(int)Enum_TipoImpresaGerarchia.Cooperativa} THEN '{_localizer["Cooperativa"]}'");
            stbQuery.AppendLine($"             WHEN {(int)Enum_TipoImpresaGerarchia.Consorzio} THEN '{_localizer["Consorzio"]}'");
            stbQuery.AppendLine($"             WHEN {(int)Enum_TipoImpresaGerarchia.OP} THEN '{_localizer["OP"]}'");
            stbQuery.AppendLine("             ELSE ''");
            stbQuery.AppendLine("         END AS TipoImpresaGerarchia ");
            stbQuery.AppendLine("       , Imprese.TipoImpresaGerarchia AS TipoImpresaGerarchia_Int ");

            stbQuery.AppendLine("       , ISNULL(FormeGiuridiche.FG_Des, '') AS FormaGiuridica");

            if (_configuratore.CaricaLegaleRappresentante)
            {
                stbQuery.AppendLine("       , LegaleRappresentante_TT.* ");
            }

            if (_configuratore.CaricaInfoTecnicoImpresa)
                stbQuery.AppendLine("       , InfoTecnico_TT.* ");

            if (_configuratore.CaricaIndirizzoAzienda)
            {
                stbQuery.AppendLine("       , Indirizzi.Ind_Des AS Indirizzo_Azienda ");
                stbQuery.AppendLine("       , Indirizzi.Frz_Des AS Frazione_Azienda ");
                stbQuery.AppendLine("       , Indirizzi.CAP AS CAP_Azienda ");

                stbQuery.AppendLine("       , Lista_Stati.Descrizione AS Stato_Azienda ");
                stbQuery.AppendLine("       , Lista_Regioni.Regione_Des AS Regione_Azienda ");
                stbQuery.AppendLine("       , Lista_Province.Provincia AS Provincia_Azienda ");
                stbQuery.AppendLine("       , ISTAT.Localita AS Comune_Azienda ");
                stbQuery.AppendLine("       , Lista_Province.Sigla AS Sigla_Provincia_Azienda ");
                stbQuery.AppendLine("       , Lista_Regioni.REG AS REG_Regione_Azienda ");
                stbQuery.AppendLine("       , Lista_Regioni.Stato_Country AS StatoISO_Azienda ");
            }

            if (_configuratore.joinGerarchiaBase)
            {
                stbQuery.AppendLine("       , ISNULL(GI_base.Foglia, 1) AS Foglia ");
                stbQuery.AppendLine("       , ISNULL(GI_base.Livello, 1) AS Livello ");
                stbQuery.AppendLine("       , ISNULL(GI_base.Padre, '') AS Padre ");
            }

            if (_configuratore.CaricaDatiServizi)
            {
                stbQuery.AppendLine("       , ISNULL(Servizi_TT.Pratica_Cod, 0) AS Pratica_Cod ");
                stbQuery.AppendLine("       , Servizi_TT.Servizio ");
                stbQuery.AppendLine("       , Servizi_TT.StatoPratica ");
                stbQuery.AppendLine("       , Servizi_TT.AnnoPratica ");
                stbQuery.AppendLine("       , Servizi_TT.NumeroPratica ");
            }

            await GetBaseColumnsAsync(stbQuery, stbJoinUtenti, Enum_Tabelle.Aziende, kendoColumns, !_configuratore.isMostraAziende, objParametriServer);

            if (_configuratore.isMostraAziende)
            {
                stbQuery.AppendLine("       , Creazione.Username AS Username_Creazione ");
                stbQuery.AppendLine("       , Modifica.Username AS Username_Modifica ");
            }

            stbQuery.AppendLine("   INTO #AziendeBase_TT ");
            stbQuery.AppendLine("   FROM Imprese ");

            stbQuery.AppendLine(stbJoinVisibilitaAppoggio.ToString());

            stbQuery.AppendLine($"   LEFT JOIN Imprese_Codici CUAA ON CUAA.Piva = Imprese.Piva AND CUAA.Id_Cod = {(int)Enum_CodiciAnagrafe.CodiceCUAA} ");

            stbQuery.AppendLine("   LEFT JOIN Gruppi_Raccolta ON Gruppi_Raccolta.GruppoRaccolta_Cod = Imprese.GruppoRaccolta_Cod ");

            stbQuery.AppendLine("   LEFT JOIN FormeGiuridiche ON Imprese.Forma_Giuridica = FormeGiuridiche.FG_cod");

            if (_configuratore.joinImpreseReferenti)
                stbQuery.AppendLine($"   {(_configuratore.ApplicaFiltroImpreseReferenti ? "INNER" : "LEFT")} JOIN #GerarchiaImprese_TT GerarchiaImprese_TT ON GerarchiaImprese_TT.[keyGerarchia] = Imprese.Piva ");

            if (_configuratore.joinGerarchiaBase)
                stbQuery.AppendLine("   LEFT JOIN GerarchiaImprese GI_base ON GI_base.Figlio = Imprese.Piva ");

            if (_configuratore.CaricaLegaleRappresentante)
                stbQuery.AppendLine("   LEFT JOIN #LegaleRappresentante_TT LegaleRappresentante_TT ON LegaleRappresentante_TT.[keyPiva7] = Imprese.Piva ");

            if (_configuratore.CaricaInfoTecnicoImpresa)
                stbQuery.AppendLine("   LEFT JOIN #InfoTecnico_TT InfoTecnico_TT ON InfoTecnico_TT.[keyTecnico1] = IC_1088.[keyTecnico] ");

            if (_configuratore.joinIndirizzoAzienda)
            {
                stbQuery.AppendLine($"   JOIN ImpresexIndirizzi ON ImpresexIndirizzi.piva = Imprese.piva AND ImpresexIndirizzi.Tipo_Indirizzo = {(int)Enum_IndirizzoTipo.SedeOperativa} ");
                stbQuery.AppendLine("   JOIN Indirizzi ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo ");
                stbQuery.AppendLine("   JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ");
                stbQuery.AppendLine("   JOIN Lista_Province ON Lista_Province.Sigla = ISTAT.COMUNI_PROV ");
                stbQuery.AppendLine("   JOIN Lista_Regioni ON Lista_Regioni.REG = Lista_Province.REG ");
                stbQuery.AppendLine("   JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 Lista_Stati ON Lista_Stati.Codice = Lista_Regioni.Stato_Country ");
            }

            if (_configuratore.joinServizio)
            {
                if (_configuratore.ApplicaFiltroServizi)
                    stbQuery.AppendLine("   JOIN #Servizi_TT Servizi_TT ON Servizi_TT.Piva = Imprese.Piva ");
                else
                    stbQuery.AppendLine("   LEFT JOIN #Servizi_TT Servizi_TT ON Servizi_TT.Piva = Imprese.Piva ");
            }

            if (_configuratore.joinOperazionixAzienda)
            {
                if (_configuratore.IncludiEscludiOperazioniAgenda == (int)Enum_FiltroOperazioniAgenda_FiltroRicerca.ConOperazioni)
                    stbQuery.AppendLine("   JOIN #OperazioniAgenda_TT OperazioniAgenda_TT ON ");
                else
                    stbQuery.AppendLine("   LEFT JOIN #OperazioniAgenda_TT OperazioniAgenda_TT ON ");

                stbQuery.AppendLine("          OperazioniAgenda_TT.[keyPiva10] = Imprese.PIVA ");
            }

            if (_configuratore.isMostraAziende)
            {
                stbQuery.AppendLine(" LEFT JOIN #UtentiDettagli_TT Creazione ON Creazione.[keyUtente] = Imprese.Username_Creazione ");
                stbQuery.AppendLine(" LEFT JOIN #UtentiDettagli_TT Modifica ON Modifica.[keyUtente] = Imprese.Username_Modifica ");
            }

            stbQuery.AppendLine(stbJoinUtenti.ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine(StbFiltri.FiltroAziendeBase.ToString());

                //Indice per ottimizzazione query su tabella temp
                stbQuery.AppendLine($" CREATE CLUSTERED INDEX IX_AziendeBase_TT_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #AziendeBase_TT (keyPiva);");
            }

            if (Debugger.IsAttached)
                stbQuery.AppendLine(GetTimestamp("#AziendeBase_TT").ToString());

            _stbDropTemporaryTable.AppendLine(" DROP TABLE #AziendeBase_TT; ");
            #endregion

            return stbQuery.ToString();
        }

        private string GetAziendeDettagli_TT(ref List<KendoColumn> kendoColumns,
                                             StbFiltri StbFiltri,
                                             AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var strSqlTTCodici = new StringBuilder();
            var stbSelectCodici = new StringBuilder();
            var stbJoinCodici = new StringBuilder();

            GetCodici(Enum_Tabelle.Aziende, _configuratore.CodiciAzienda, ref strSqlTTCodici, ref stbSelectCodici, ref stbJoinCodici, ref kendoColumns, objParametriServer);

            stbQuery.AppendLine(strSqlTTCodici.ToString());

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("         Imprese.PIVA AS [keyPiva] ");
            stbQuery.AppendLine(stbSelectCodici.ToString());

            stbQuery.AppendLine(" INTO #AziendeDettagli_TT ");
            stbQuery.AppendLine(" FROM Imprese ");

            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.Azienda).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.Azienda).ToString());

            stbQuery.AppendLine(stbJoinCodici.ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine(StbFiltri.FiltroAziendeDettagli.ToString());

                //Indice per ottimizzazione query su tabella temp
                stbQuery.AppendLine($" CREATE CLUSTERED INDEX IX_AziendeDettagli_TT_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #AziendeDettagli_TT (keyPiva);");
            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#AziendeDettagli_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #AziendeDettagli_TT; ");
            }

            return stbQuery.ToString();
        }
        #endregion

        #region "Centri Aziendali"
        private async Task<string> GetCentriAziendaliBase_TTAsync(List<KendoColumn> kendoColumns,
                                                                  StbFiltri StbFiltri,
                                                                  Dictionary<string, object> parSql,
                                                                  AgronicaCoreParametriServer objParametriServer,
                                                                  AgronicaCoreParametriUtenti objParametriUtenti)
        {

            #region "KENDO COLUMNS"
            if (_configuratore.needColumnsCentriAziendali)
            {
                kendoColumns.Add(new KendoColumn { Field = "Sa_Nome", Title = "CentroAziendale", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                if (_configuratore.CaricaIndirizzoCentroAziendale)
                {
                    kendoColumns.Add(new KendoColumn { Field = "Indirizzo_Centro", Title = (_configuratore.specificaIndirizzo ? "IndirizzoCentro" : "Indirizzo"), DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Frazione_Centro", Title = (_configuratore.specificaIndirizzo ? "FrazioneCentro" : "Frazione"), DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "CAP_Centro", Title = (_configuratore.specificaIndirizzo ? "CAPCentro" : "CAP"), DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                    kendoColumns.Add(new KendoColumn { Field = "Stato_Centro", Title = (_configuratore.specificaIndirizzo ? "StatoCentro" : "Stato"), DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Regione_Centro", Title = (_configuratore.specificaIndirizzo ? "RegioneCentro" : "Regione"), DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Provincia_Centro", Title = (_configuratore.specificaIndirizzo ? "ProvinciaCentro" : "Provincia"), DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Comune_Centro", Title = (_configuratore.specificaIndirizzo ? "ComuneCentro" : "Comune"), DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                }

                kendoColumns.Add(new KendoColumn { Field = "Latitudine_Centro", Title = (_configuratore.specificaCoordinate ? "LatitudineCentroAbbr" : "Latitudine"), DataType = "number", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Longitudine_Centro", Title = (_configuratore.specificaCoordinate ? "LongitudineCentroAbbr" : "Longitudine"), DataType = "number", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                kendoColumns.Add(new KendoColumn { Field = "Codice_Centro", Title = "Codice_Centro", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false });

                if (_configuratore.CaricaDatiCatastaliCentroAziendale)
                {
                    kendoColumns.Add(new KendoColumn { Field = "Comune_Particella", Title = "ComuneParticella", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Provincia_Particella", Title = "ProvinciaParticella", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Sezione", Title = "Sezione", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Foglio", Title = "Foglio", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Numero", Title = "Numero", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Subalterno", Title = "Subalterno", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                    kendoColumns.Add(new KendoColumn { Field = "Validita_Inizio_Possesso", Title = "ValiditaInizioPossesso", DataType = "date", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Validita_Fine_Possesso", Title = "ValiditaFinePossesso", DataType = "date", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                    if (_fattoreConversione > 1)
                    {
                        kendoColumns.Add(new KendoColumn { Field = "Sup_Condotta_Acro", Title = "SuperficieCondottaAcroAbbr", DataType = "number", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                        kendoColumns.Add(new KendoColumn { Field = "Sup_Catastale_Acro", Title = "SuperficieCatastaleAcroAbbr", DataType = "number", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    }
                    else
                    {
                        kendoColumns.Add(new KendoColumn { Field = "Sup_Catastale", Title = "SuperficieCatastaleAbbr", DataType = "number", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                        kendoColumns.Add(new KendoColumn { Field = "Sup_Condotta", Title = "SuperficieCondottaAbbr", DataType = "number", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    }
                }
            }
            #endregion

            #region "QUERY"  
            var stbQuery = new StringBuilder();
            var stbJoinUtenti = new StringBuilder();
            var stbJoinVisibilitaAppoggio = new StringBuilder();

            if (_checkVisibilitaCentriAziendali)
                stbQuery.AppendLine(GetUtentiVisibilitaAppoggio_TT((int)Enum_TipoEntita.Centro, stbJoinVisibilitaAppoggio, ref parSql, objParametriServer).ToString());

            if (_configuratore.CaricaDatiCatastaliCentroAziendale)
                stbQuery.AppendLine(GetParticelleCentriAziendali_TT().ToString());
            if (_configuratore.joinOperazionixCentroAziendale)
                stbQuery.AppendLine(GetOperazioniAgenda_TT(StbFiltri, joinSaCod: true).ToString());

            stbQuery.AppendLine("   SELECT ");
            stbQuery.AppendLine("        Centri_Aziendali.PIVA AS [keyPiva] ");
            stbQuery.AppendLine("       , Centri_Aziendali.sa_cod AS [keySaCod] ");

            if (_configuratore.needColumnsCentriAziendali)
            {
                stbQuery.AppendLine("       , Centri_Aziendali.Sa_Cod ");

                stbQuery.AppendLine("       , Centri_Aziendali.Sa_Nome ");

                if (_configuratore.CaricaIndirizzoCentroAziendale)
                {
                    stbQuery.AppendLine("       , Indirizzi.Ind_Des AS Indirizzo_Centro ");
                    stbQuery.AppendLine("       , Indirizzi.Frz_Des AS Frazione_Centro ");
                    stbQuery.AppendLine("       , Indirizzi.CAP AS CAP_Centro ");

                    stbQuery.AppendLine("       , Lista_Stati.Descrizione AS Stato_Centro ");
                    stbQuery.AppendLine("       , Lista_Regioni.Regione_Des AS Regione_Centro ");
                    stbQuery.AppendLine("       , Lista_Province.Provincia AS Provincia_Centro ");
                    stbQuery.AppendLine("       , ISTAT.Localita AS Comune_Centro ");
                }

                stbQuery.AppendLine("       , Centri_Aziendali.lat AS Latitudine_Centro ");
                stbQuery.AppendLine("       , Centri_Aziendali.long AS Longitudine_Centro ");

                stbQuery.AppendLine("       , Centri_Aziendali.Sa_Cod AS Codice_Centro ");

                await GetBaseColumnsAsync(stbQuery, stbJoinUtenti, Enum_Tabelle.CentriAziendali, kendoColumns, !_configuratore.isMostraCentriAziendali, objParametriServer);

                if (_configuratore.CaricaDatiCatastaliCentroAziendale)
                {
                    stbQuery.AppendLine("       , ISNULL(ParticelleCentriAziendali_TT.ID, 0) AS Id_Particella  ");
                    stbQuery.AppendLine("       , ParticelleCentriAziendali_TT.* ");
                }

                if (_configuratore.isMostraCentriAziendali)
                {
                    stbQuery.AppendLine("        , Creazione.Username AS Username_Creazione ");
                    stbQuery.AppendLine("        , Modifica.Username AS Username_Modifica ");
                }
            }

            stbQuery.AppendLine("   INTO #CentriAziendaliBase_TT ");
            stbQuery.AppendLine("   FROM Centri_Aziendali ");

            stbQuery.AppendLine(stbJoinVisibilitaAppoggio.ToString());

            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.CentroAziendale).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.Azienda).ToString());

            if (_configuratore.joinIndirizzoCentroAziendale)
            {
                stbQuery.AppendLine("   JOIN CentrixIndirizzi ON CentrixIndirizzi.piva = Centri_Aziendali.piva AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod AND CentrixIndirizzi.Tipo_Indirizzo = " + (int)Enum_IndirizzoTipo.SedeOperativa + " ");
                stbQuery.AppendLine("   JOIN Indirizzi ON CentrixIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo ");
                stbQuery.AppendLine("   JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ");
                stbQuery.AppendLine("   JOIN Lista_Province ON Lista_Province.Sigla = ISTAT.COMUNI_PROV ");
                stbQuery.AppendLine("   JOIN Lista_Regioni ON Lista_Regioni.REG = Lista_Province.REG ");
                stbQuery.AppendLine("   JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 Lista_Stati ON Lista_Stati.Codice = Lista_Regioni.Stato_Country ");
            }

            if (_configuratore.CaricaDatiCatastaliCentroAziendale)
            {
                stbQuery.AppendLine("   LEFT JOIN #ParticelleCentriAziendali_TT ParticelleCentriAziendali_TT ON ");
                stbQuery.AppendLine("       ParticelleCentriAziendali_TT.[keyPiva1] = Centri_Aziendali.Piva ");
                stbQuery.AppendLine("   AND ParticelleCentriAziendali_TT.[keySaCod1] = Centri_Aziendali.Sa_Cod ");
            }

            if (_configuratore.joinOperazionixCentroAziendale)
            {
                if (_configuratore.IncludiEscludiOperazioniAgenda == (int)Enum_FiltroOperazioniAgenda_FiltroRicerca.ConOperazioni)
                    stbQuery.AppendLine("   JOIN #OperazioniAgenda_TT OperazioniAgenda_TT ON ");
                else
                    stbQuery.AppendLine("   LEFT JOIN #OperazioniAgenda_TT OperazioniAgenda_TT ON ");

                stbQuery.AppendLine("       OperazioniAgenda_TT.[keyPiva10] = Centri_Aziendali.PIVA ");
                stbQuery.AppendLine("   AND OperazioniAgenda_TT.[keySaCod10] = Centri_Aziendali.Sa_Cod ");
            }

            if (_configuratore.isMostraCentriAziendali)
            {
                stbQuery.AppendLine("   LEFT JOIN #UtentiDettagli_TT Creazione ON Creazione.[keyUtente] = Centri_Aziendali.Username_Creazione ");
                stbQuery.AppendLine("   LEFT JOIN #UtentiDettagli_TT Modifica ON Modifica.[keyUtente] = Centri_Aziendali.Username_Modifica ");
            }

            stbQuery.AppendLine(stbJoinUtenti.ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine(StbFiltri.FiltroCentriAziendaliBase.ToString());

                //Indice per ottimizzazione query su tabella temp
                stbQuery.AppendLine($" CREATE CLUSTERED INDEX IX_CentriAziendaliBase_TT_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #CentriAziendaliBase_TT (keyPiva, keySaCod);");
            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#CentriAziendaliBase_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #CentriAziendaliBase_TT ");
            }

            #endregion

            return stbQuery.ToString();
        }

        private string GetCentriAziendaliDettagli_TT(ref List<KendoColumn> kendoColumns,
                                                     StbFiltri StbFiltri,
                                                     AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var strSqlTTCodici = new StringBuilder();
            var stbSelectCodici = new StringBuilder();
            var stbJoinCodici = new StringBuilder();

            GetCodici(Enum_Tabelle.CentriAziendali, _configuratore.CodiciCentri, ref strSqlTTCodici, ref stbSelectCodici, ref stbJoinCodici, ref kendoColumns, objParametriServer);

            stbQuery.AppendLine(strSqlTTCodici.ToString());

            stbQuery.AppendLine("   SELECT ");
            stbQuery.AppendLine("         Centri_Aziendali.PIVA AS [keyPiva] ");
            stbQuery.AppendLine("       , Centri_Aziendali.sa_cod AS [keySaCod] ");
            stbQuery.AppendLine(stbSelectCodici.ToString());

            stbQuery.AppendLine("   INTO #CentriAziendaliDettagli_TT ");
            stbQuery.AppendLine("   FROM Centri_Aziendali ");

            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.CentroAziendale).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.CentroAziendale).ToString());

            stbQuery.AppendLine(stbJoinCodici.ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine(StbFiltri.FiltroCentriAziendaliDettagli.ToString());

                //Indice per ottimizzazione query su tabella temp
                stbQuery.AppendLine($" CREATE CLUSTERED INDEX IX_CentriAziendaliDettagli_TT_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #CentriAziendaliDettagli_TT (keyPiva, keySaCod);");
            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#CentriAziendaliDettagli_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #CentriAziendaliDettagli_TT ");
            }

            return stbQuery.ToString();
        }
        #endregion

        #region "Campi"
        private async Task<string> GetCampiBase_TTAsync(List<KendoColumn> kendoColumns,
                                                        StbFiltri StbFiltri,
                                                        AgronicaCoreParametriServer objParametriServer,
                                                        AgronicaCoreParametriUtenti objParametriUtenti)
        {

            #region "KENDO COLUMNS"
            if (_configuratore.needColumnsCampi)
            {
                kendoColumns.Add(new KendoColumn { Field = "Campo_Des", Title = "Campo", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
            }

            if (_configuratore.isMostraCampi)
            {
                if (!_configuratore.CaricaDatiCatastaliCampo)
                {
                    kendoColumns.Add(new KendoColumn { Field = "SAU_Totale", Title = "SAUTotaleAbbr", DataType = "decimal", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "SAU_Biologico", Title = "SAUBiologico", DataType = "decimal", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "SAU_Convenzionale", Title = "SAUConvenzionaleAbbr", DataType = "decimal", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "SAU_Conversione", Title = "SAUConversioneAbbr", DataType = "decimal", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "SAU_Catastale", Title = "SAUCatastale", DataType = "decimal", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                }
                else
                {
                    kendoColumns.Add(new KendoColumn { Field = "Comune_Particella", Title = "ComuneParticella", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Provincia_Particella", Title = "ProvinciaParticella", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Sezione", Title = "Sezione", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Foglio", Title = "Foglio", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Numero", Title = "Numero", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Subalterno", Title = "Subalterno", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                }

                kendoColumns.Add(new KendoColumn { Field = "Veg_Des", Title = "Specie", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Gru_Des", Title = "GruppoVegetale", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                //Colonne non visibili all'utente
                kendoColumns.Add(new KendoColumn { Field = "Veg_Cod", Title = "Veg_Cod", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false });
                kendoColumns.Add(new KendoColumn { Field = "Gru_Cod", Title = "Gru_Cod", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false });
            }
            #endregion

            #region "QUERY"   
            var stbQuery = new StringBuilder();
            var stbJoinUtenti = new StringBuilder();

            if (_configuratore.isMostraCampi)
            {
                if (!_configuratore.CaricaDatiCatastaliCampo)
                    stbQuery.AppendLine(GetSuperficiCampi_TT().ToString());
                stbQuery.AppendLine(GetParticelleCampi_TT().ToString());
                if (_configuratore.joinOperazionixCampo)
                    stbQuery.AppendLine(GetOperazioniAgenda_TT(StbFiltri, joinSaCod: true, joinCampoCod: true).ToString());
            }

            stbQuery.AppendLine("   SELECT ");
            stbQuery.AppendLine("          Campi.PIVA AS [keyPiva] ");
            stbQuery.AppendLine("        , Campi.sa_cod AS [keySaCod] ");
            stbQuery.AppendLine("        , Campi.campo_cod AS [keyCampoCod] ");

            if (_configuratore.needColumnsCampi)
            {
                stbQuery.AppendLine("       , Campi.Campo_Cod ");

                stbQuery.AppendLine("       , Campi.Campo_Des ");

                await GetBaseColumnsAsync(stbQuery, stbJoinUtenti, $"{_modalitaBudget}{Enum_Tabelle.Campi}", kendoColumns, !_configuratore.isMostraCampi, objParametriServer);

                if (_configuratore.isMostraCampi)
                {
                    stbQuery.AppendLine("       , SpecieVegetali.Veg_Des ");
                    stbQuery.AppendLine("       , SpecieVegetali.Veg_Cod ");
                    stbQuery.AppendLine("       , GruppoVegetale.Gru_Des ");
                    stbQuery.AppendLine("       , GruppoVegetale.Gru_Cod ");


                    stbQuery.AppendLine("       , ParticelleCampi_TT.* ");

                    if (!_configuratore.CaricaDatiCatastaliCampo)
                    {
                        stbQuery.AppendLine("       , CampixSuperfici_TT.* ");
                        stbQuery.AppendLine("       , CampixSuperfici_TT.SAU_Conversione + CampixSuperfici_TT.SAU_Convenzionale + CampixSuperfici_TT.SAU_Biologico AS SAU_Totale ");
                    }

                    stbQuery.AppendLine("        , Creazione.Username AS Username_Creazione ");
                    stbQuery.AppendLine("        , Modifica.Username AS Username_Modifica ");
                }
            }

            stbQuery.AppendLine("   INTO #CampiBase_TT ");
            stbQuery.AppendLine("   FROM " + _modalitaBudget + "Campi AS Campi ");

            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.Campo).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.CentroAziendale).ToString());

            if (_configuratore.isMostraCampi)
            {
                stbQuery.AppendLine("   LEFT JOIN SpecieVegetali ON Campi.VEG_COD = SpecieVegetali.VEG_COD ");
                stbQuery.AppendLine("   LEFT JOIN GruppoVegetale ON Campi.GRU_COD = GruppoVegetale.GRU_COD ");

                if (!_configuratore.CaricaDatiCatastaliCampo)
                {
                    stbQuery.AppendLine("   LEFT JOIN #CampixSuperfici_TT CampixSuperfici_TT ON ");
                    stbQuery.AppendLine("       CampixSuperfici_TT.[keyPiva1] = Campi.Piva ");
                    stbQuery.AppendLine("   AND CampixSuperfici_TT.[keySaCod1] = Campi.Sa_Cod ");
                    stbQuery.AppendLine("   AND CampixSuperfici_TT.[keyCampoCod1] = Campi.Campo_Cod ");
                }

                stbQuery.AppendLine("   LEFT JOIN #ParticelleCampi_TT ParticelleCampi_TT ON ");
                stbQuery.AppendLine("       ParticelleCampi_TT.[keyPiva2] = Campi.Piva ");
                stbQuery.AppendLine("   AND ParticelleCampi_TT.[keySaCod2] = Campi.Sa_Cod ");
                stbQuery.AppendLine("   AND ParticelleCampi_TT.[keyCampoCod2] = Campi.Campo_Cod ");

                if (_configuratore.joinOperazionixCampo)
                {
                    if (_configuratore.IncludiEscludiOperazioniAgenda == (int)Enum_FiltroOperazioniAgenda_FiltroRicerca.ConOperazioni)
                        stbQuery.AppendLine("   JOIN #OperazioniAgenda_TT OperazioniAgenda_TT ON ");
                    else
                        stbQuery.AppendLine("   LEFT JOIN #OperazioniAgenda_TT OperazioniAgenda_TT ON ");

                    stbQuery.AppendLine("       OperazioniAgenda_TT.[keyPiva10] = Campi.PIVA ");
                    stbQuery.AppendLine("   AND OperazioniAgenda_TT.[keySaCod10] = Campi.Sa_Cod ");
                    stbQuery.AppendLine("   AND OperazioniAgenda_TT.[keyCampoCod10] = Campi.Campo_Cod ");
                }

                stbQuery.AppendLine("   LEFT JOIN #UtentiDettagli_TT Creazione ON Creazione.[keyUtente] = Campi.Username_Creazione ");
                stbQuery.AppendLine("   LEFT JOIN #UtentiDettagli_TT Modifica ON Modifica.[keyUtente] = Campi.Username_Modifica ");
            }

            stbQuery.AppendLine(stbJoinUtenti.ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                if (_modalitaBudget != "")
                    stbQuery.AppendLine("   AND Campi.Id_Budget = @IdBudget ");
                stbQuery.AppendLine(StbFiltri.FiltroCampiBase.ToString());

                //Indice per ottimizzazione query su tabella temp
                stbQuery.AppendLine($" CREATE CLUSTERED INDEX IX_CampiBase_TT_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #CampiBase_TT (keyPiva, keySaCod, keyCampoCod);");

            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#CampiBase_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #CampiBase_TT ");
            }

            #endregion

            return stbQuery.ToString();
        }

        private string GetCampiDettagli_TT(ref List<KendoColumn> kendoColumns,
                                           StbFiltri StbFiltri,
                                           AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var strSqlTTCodici = new StringBuilder();
            var stbSelectCodici = new StringBuilder();
            var stbJoinCodici = new StringBuilder();

            GetCodici(Enum_Tabelle.Campi, _configuratore.CodiciCampi, ref strSqlTTCodici, ref stbSelectCodici, ref stbJoinCodici, ref kendoColumns, objParametriServer);

            stbQuery.AppendLine(strSqlTTCodici.ToString());

            stbQuery.AppendLine("   SELECT ");
            stbQuery.AppendLine("          Campi.PIVA AS [keyPiva] ");
            stbQuery.AppendLine("        , Campi.sa_cod AS [keySaCod] ");
            stbQuery.AppendLine("        , Campi.campo_cod AS [keyCampoCod] ");
            stbQuery.AppendLine(stbSelectCodici.ToString());

            stbQuery.AppendLine("   INTO #CampiDettagli_TT ");
            stbQuery.AppendLine("   FROM " + _modalitaBudget + "Campi AS Campi ");

            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.Campo).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.Campo).ToString());

            stbQuery.AppendLine(stbJoinCodici.ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                if (_modalitaBudget != "")
                    stbQuery.AppendLine("   AND Campi.Id_Budget = @IdBudget ");
                stbQuery.AppendLine(StbFiltri.FiltroCampiDettagli.ToString());

                //Indice per ottimizzazione query su tabella temp
                stbQuery.AppendLine($" CREATE CLUSTERED INDEX IX_CampiDettagli_TT_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #CampiDettagli_TT (keyPiva, keySaCod, keyCampoCod);");
            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#CampiDettagli_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #CampiDettagli_TT ");
            }

            return stbQuery.ToString();
        }

        #endregion

        #region "Appezzamenti"
        private async Task<string> GetAppezzamentiBase_TTAsync(List<KendoColumn> kendoColumns,
                                                               StbFiltri StbFiltri,
                                                               AgronicaCoreParametriServer objParametriServer,
                                                               AgronicaCoreParametriUtenti objParametriUtenti)
        {

            #region "KENDO COLUMNS"
            if (_configuratore.needColumnsAppezzamenti)
            {
                kendoColumns.Add(new KendoColumn { Field = "App_Nome", Title = "NomeAppezzamento", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                if (_fattoreConversione > 1)
                    kendoColumns.Add(new KendoColumn { Field = "Sup_App_Acri", Title = "SuperficieAppezzamentoAcriAbbr", DataType = "number", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                else
                    kendoColumns.Add(new KendoColumn { Field = "SUP_APP", Title = "SuperficieAppezzamentoAbbr", DataType = "number", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                kendoColumns.Add(new KendoColumn { Field = "UtilizzoTerreno", Title = "UtilizzoTerreno", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                //kendoColumns.Add(new KendoColumn { Field = "Latitudine_Appezzamento", Title = (_configuratore.specificaCoordinate ? "LatitudineAppezzamentoAbbr" : "Latitudine"), DataType = "number", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                //kendoColumns.Add(new KendoColumn { Field = "Longitudine_Appezzamento", Title = (_configuratore.specificaCoordinate ? "LongitudineAppezzamentoAbbr" : "Longitudine"), DataType = "number", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                kendoColumns.Add(new KendoColumn { Field = "Esposizione", Title = "Esposizione", DataType = "number", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Pendenza", Title = "Pendenza", DataType = "number", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                if (_configuratore.CaricaIndirizziAppezzamento)
                {
                    kendoColumns.Add(new KendoColumn { Field = "Indirizzo_Appezzamento", Title = (_configuratore.specificaIndirizzo ? "IndirizzoAppezzamentoAbbr" : "Indirizzo"), DataType = "string", Hidden = !_configuratore.isMostraAppezzamenti, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Frazione_Appezzamento", Title = (_configuratore.specificaIndirizzo ? "FrazioneAppezzamentoAbbr" : "Frazione"), DataType = "string", Hidden = !_configuratore.isMostraAppezzamenti, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "CAP_Appezzamento", Title = (_configuratore.specificaIndirizzo ? "CAPAppezzamentoAbbr" : "CAP"), DataType = "string", Hidden = !_configuratore.isMostraAppezzamenti, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                    kendoColumns.Add(new KendoColumn { Field = "Stato_Appezzamento", Title = (_configuratore.specificaIndirizzo ? "StatoAppezzamentoAbbr" : "Stato"), DataType = "string", Hidden = !_configuratore.isMostraAppezzamenti, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Regione_Appezzamento", Title = (_configuratore.specificaIndirizzo ? "RegioneApp" : "Regione"), DataType = "string", Hidden = !_configuratore.isMostraAppezzamenti, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Provincia_Appezzamento", Title = (_configuratore.specificaIndirizzo ? "ProvinciaAppezzamentoAbbr" : "Provincia"), DataType = "string", Hidden = !_configuratore.isMostraAppezzamenti, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Comune_Appezzamento", Title = (_configuratore.specificaIndirizzo ? "ComuneAppezzamentoAbbr" : "Comune"), DataType = "string", Hidden = !_configuratore.isMostraAppezzamenti, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                }

                if (_configuratore.isMostraEsercizi)
                {
                    kendoColumns.Add(new KendoColumn { Field = "Validita_Inizio_Appezzamento", Title = "ValiditaInizioAppezzamentoAbbr", DataType = "date", Hidden = !_configuratore.isMostraAppezzamenti, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Validita_Fine_Appezzamento", Title = "ValiditaFineAppezzamentoAbbr", DataType = "date", Hidden = !_configuratore.isMostraAppezzamenti, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                    //kendoColumns.Add(new KendoColumn { Field = "Durata_Appezzamento", Title = "DurataAppezzamentoAbbr", DataType = "string", Hidden = !_configuratore.isMostraAppezzamenti, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                }

                if (_configuratore.CaricaDatiCatastaliAppezzamento)
                {
                    kendoColumns.Add(new KendoColumn { Field = "Comune_Particella", Title = "ComuneParticella", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Provincia_Particella", Title = "ProvinciaParticella", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Sezione", Title = "Sezione", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Foglio", Title = "Foglio", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Numero", Title = "Numero", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Subalterno", Title = "Subalterno", DataType = "string", Hidden = !_configuratore.isMostraCentriAziendali, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "ZVN", Title = "ZVN", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                }

                if (_configuratore.isMostraPianoColturale)
                {
                    kendoColumns.Add(new KendoColumn { Field = "Data_Creazione_Appezzamento", Title = "DataCreazioneAppezzamento", DataType = "date", Hidden = true, ColumnOrder = columnOrder_stdGIAS });
                    kendoColumns.Add(new KendoColumn { Field = "Username_Creazione_Appezzamento", Title = "UtenteCreazioneAppezzamento", DataType = "string", Hidden = true, ColumnOrder = columnOrder_stdGIAS });
                }

                kendoColumns.Add(new KendoColumn { Field = "Blk_Flag", Title = "AppAbbrBloccato", DataType = "string", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Blk_Inizio_Username", Title = "UtenteBloccoAppAbbr", DataType = "string", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Blk_Inizio_Data", Title = "DataBloccoAppAbbr", DataType = "date", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

            }
            #endregion

            #region "QUERY" 
            var stbQuery = new StringBuilder();
            var stbJoinUtenti = new StringBuilder();

            if (_configuratore.joinRiparto)
                stbQuery.AppendLine(GetParticelleAppezzamenti_TT().ToString());
            if (_configuratore.CaricaDatiCatastaliAppezzamento)
                stbQuery.AppendLine(GetZVNAppezzamenti_TT().ToString());
            if (_configuratore.CaricaIndirizziAppezzamento)
                stbQuery.AppendLine(GetIndirizziAppezzamenti_TT().ToString());
            if (_configuratore.joinOperazionixAppezzamento)
                stbQuery.AppendLine(GetOperazioniAgenda_TT(StbFiltri, joinSaCod: true, joinAppezza: true).ToString());

            stbQuery.AppendLine(GetUtilizzoTerreno_TT(StbFiltri).ToString());

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     Appezzamento.PIVA AS [keyPiva] ");
            stbQuery.AppendLine("   , Appezzamento.sa_cod AS [keySaCod] ");
            stbQuery.AppendLine("   , Appezzamento.Appezza AS [keyAppezza] ");
            stbQuery.AppendLine("   , Appezzamento.Campo_Cod AS [keyCampoCod] ");

            if (_configuratore.needColumnsAppezzamenti)
            {
                stbQuery.AppendLine("   , Appezzamento.Appezza ");
                stbQuery.AppendLine("   , Appezzamento.App_Nome ");
                stbQuery.AppendLine("   , CASE UtilizzoTerreno.Val_Cod ");
                stbQuery.AppendLine($"         WHEN '1' THEN '{_localizer["Integrato"]}' ");
                stbQuery.AppendLine($"         WHEN '2' THEN '{_localizer["InConversione"]}' ");
                stbQuery.AppendLine($"         WHEN '3' THEN '{_localizer["Biologico"]}' ");
                stbQuery.AppendLine("         ELSE 'Integrato' ");
                stbQuery.AppendLine("     END AS UtilizzoTerreno ");

                stbQuery.AppendLine("   , Appezzamento.esposiz AS Esposizione ");
                stbQuery.AppendLine("   , Appezzamento.pende AS Pendenza ");

                if (_fattoreConversione > 1)
                    stbQuery.AppendLine("   , ROUND(ISNULL(Appezzamento.SUP_APP,0) * " + _fattoreConversione.ToString().Replace(",", ".") + ", 4) AS Sup_App_Acri ");
                else
                    stbQuery.AppendLine("   , Appezzamento.SUP_APP ");

                if (_configuratore.CaricaIndirizziAppezzamento)
                {
                    stbQuery.AppendLine("   , IndirizziAppezzamenti_TT.* ");
                }

                stbQuery.AppendLine("   , Appezzamento.X AS Latitudine_Appezzamento ");
                stbQuery.AppendLine("   , Appezzamento.Y AS Longitudine_Appezzamento ");

                stbQuery.AppendLine($"   , CASE WHEN ISNULL(Appezzamento.Blk_Flag, 0) = -1 THEN '{_localizer["Si"]}' ELSE '{_localizer["No"]}' END AS Blk_Flag ");
                stbQuery.AppendLine("   , ISNULL(Blk_Inizio_Username.Username, '') AS Blk_Inizio_Username ");
                stbQuery.AppendLine("   , Blk_Inizio_Data AS Blk_Inizio_Data ");

                if (_configuratore.isMostraEsercizi)
                {
                    stbQuery.AppendLine("   , Appezzamento.Validita_Inizio AS Validita_Inizio_Appezzamento ");
                    stbQuery.AppendLine("   , Appezzamento.Validita_Fine AS Validita_Fine_Appezzamento ");

                    //stbQuery.AppendLine("       , CASE WHEN CONVERT(date, Appezzamento.Validita_Inizio, 120) <> '01/01/1900' THEN CONVERT(VARCHAR(100), FORMAT(Appezzamento.Validita_Inizio, 'dd/MM/yyyy')) ELSE '...' END + ' - ' + ");
                    //stbQuery.AppendLine("         CASE WHEN CONVERT(date, Appezzamento.Validita_Fine, 120) <> '31/12/2100' THEN CONVERT(VARCHAR(100), FORMAT(Appezzamento.Validita_Fine, 'dd/MM/yyyy')) ELSE '...' END AS Durata_Appezzamento ");
                }

                if (_configuratore.CaricaDatiCatastaliAppezzamento)
                {
                    stbQuery.AppendLine("   , ParticelleAppezzamenti_TT.* ");
                    stbQuery.AppendLine($"   , CASE WHEN AppezzamentiZVN_TT.ZVN IS NULL THEN '{_localizer["No"]}' ELSE '{_localizer["Si"]}' END AS ZVN ");

                }

                await GetBaseColumnsAsync(stbQuery, stbJoinUtenti, $"{_modalitaBudget}{Enum_Tabelle.Appezzamenti}", kendoColumns, !_configuratore.isMostraAppezzamenti, objParametriServer);

                if (_configuratore.isMostraAppezzamenti || _configuratore.isMostraPianoColturale)
                {
                    stbQuery.AppendLine("   , Creazione.Username AS Username_Creazione" + (_configuratore.isMostraPianoColturale ? "_Appezzamento" : "") + " ");
                    if (_configuratore.isMostraAppezzamenti)
                        stbQuery.AppendLine("   , Modifica.Username AS Username_Modifica ");
                }

                if (_configuratore.isMostraPianoColturale)
                    stbQuery.AppendLine("   , Appezzamento.Data_Creazione AS Data_Creazione_Appezzamento ");
            }

            stbQuery.AppendLine(" INTO #AppezzamentiBase_TT ");
            stbQuery.AppendLine(" FROM " + _modalitaBudget + "Appezzamento AS Appezzamento ");

            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.Appezzamento).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.CentroAziendale).ToString());

            stbQuery.AppendLine(" JOIN #UtilizzoTerreno_TT UtilizzoTerreno ON ");
            stbQuery.AppendLine("     UtilizzoTerreno.[keyPiva11] = Appezzamento.PIVA ");
            stbQuery.AppendLine(" AND UtilizzoTerreno.[keySaCod11] = Appezzamento.SA_COD ");
            stbQuery.AppendLine(" AND UtilizzoTerreno.[keyAppezza11] = Appezzamento.APPEZZA ");

            if (_configuratore.CaricaIndirizziAppezzamento)
            {
                stbQuery.AppendLine(" LEFT JOIN #IndirizziAppezzamenti_TT IndirizziAppezzamenti_TT ON ");
                stbQuery.AppendLine("     IndirizziAppezzamenti_TT.[keyPiva5] = Appezzamento.PIVA ");
                stbQuery.AppendLine(" AND IndirizziAppezzamenti_TT.[keySaCod5] = Appezzamento.SA_COD ");
                stbQuery.AppendLine(" AND IndirizziAppezzamenti_TT.[keyAppezza5] = Appezzamento.APPEZZA ");
            }

            if (_configuratore.joinRiparto)
            {
                if (_configuratore.IncludiEscludiRiparto == (int)Enum_FiltroRipartoCatasto_FiltroRicerca.ConRiparto)
                    stbQuery.AppendLine(" JOIN #ParticelleAppezzamenti_TT ParticelleAppezzamenti_TT ON ");
                else
                    stbQuery.AppendLine(" LEFT JOIN #ParticelleAppezzamenti_TT ParticelleAppezzamenti_TT ON ");

                stbQuery.AppendLine("     ParticelleAppezzamenti_TT.[keyPiva1]  = Appezzamento.Piva ");
                stbQuery.AppendLine(" AND ParticelleAppezzamenti_TT.[keySaCod1] = Appezzamento.Sa_Cod ");
                stbQuery.AppendLine(" AND ParticelleAppezzamenti_TT.[keyAppezza1] = Appezzamento.Appezza ");
            }

            if (_configuratore.CaricaDatiCatastaliAppezzamento)
            {
                stbQuery.AppendLine(" LEFT JOIN #AppezzamentiZVN_TT AppezzamentiZVN_TT ON ");

                stbQuery.AppendLine("     AppezzamentiZVN_TT.[keyPiva12]  = Appezzamento.Piva ");
                stbQuery.AppendLine(" AND AppezzamentiZVN_TT.[keySaCod12] = Appezzamento.Sa_Cod ");
                stbQuery.AppendLine(" AND AppezzamentiZVN_TT.[keyAppezza12] = Appezzamento.Appezza ");
            }

            if (_configuratore.joinOperazionixAppezzamento)
            {
                if (_configuratore.IncludiEscludiOperazioniAgenda == (int)Enum_FiltroOperazioniAgenda_FiltroRicerca.ConOperazioni)
                    stbQuery.AppendLine(" JOIN #OperazioniAgenda_TT OperazioniAgenda_TT ON ");
                else
                    stbQuery.AppendLine(" LEFT JOIN #OperazioniAgenda_TT OperazioniAgenda_TT ON ");

                stbQuery.AppendLine("     OperazioniAgenda_TT.[keyPiva10] = Appezzamento.PIVA ");
                stbQuery.AppendLine(" AND OperazioniAgenda_TT.[keySaCod10] = Appezzamento.Sa_Cod ");
                stbQuery.AppendLine(" AND OperazioniAgenda_TT.[keyAppezza10] = Appezzamento.Appezza ");
            }

            if (_configuratore.isMostraAppezzamenti || _configuratore.isMostraPianoColturale)
            {
                stbQuery.AppendLine(" LEFT JOIN #UtentiDettagli_TT Creazione ON Creazione.[keyUtente] = Appezzamento.Username_Creazione ");
                if (_configuratore.isMostraAppezzamenti)
                    stbQuery.AppendLine(" LEFT JOIN #UtentiDettagli_TT Modifica ON Modifica.[keyUtente] = Appezzamento.Username_Modifica ");
            }
            stbQuery.AppendLine(" LEFT JOIN #UtentiDettagli_TT Blk_Inizio_Username ON Blk_Inizio_Username.[keyUtente] = Appezzamento.Blk_Inizio_Username COLLATE DATABASE_DEFAULT ");

            stbQuery.AppendLine(stbJoinUtenti.ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine(StbFiltri.FiltroAppezzamentiBase.ToString());

                //Indice per ottimizzazione query su tabella temp
                stbQuery.AppendLine($" CREATE CLUSTERED INDEX IX_AppezzamentiBase_TT_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #AppezzamentiBase_TT (keyPiva, keySaCod, keyAppezza);");
                stbQuery.AppendLine($" CREATE NONCLUSTERED INDEX IX_AppezzamentiBaseJoinCampo_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #AppezzamentiBase_TT (keyPiva, keySaCod, keyCampoCod)");
                stbQuery.AppendLine(" INCLUDE (keyAppezza);");

            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#AppezzamentiBase_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #AppezzamentiBase_TT ");
            }

            #endregion

            return stbQuery.ToString();
        }

        private string GetAppezzamentiDettagli_TT(ref List<KendoColumn> kendoColumns,
                                                  StbFiltri StbFiltri,
                                                  AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var strSqlTTCodici = new StringBuilder();
            var stbSelectCodici = new StringBuilder();
            var stbJoinCodici = new StringBuilder();

            GetCodici(Enum_Tabelle.Appezzamenti, _configuratore.CodiciAppezzamenti, ref strSqlTTCodici, ref stbSelectCodici, ref stbJoinCodici, ref kendoColumns, objParametriServer);

            stbQuery.AppendLine(strSqlTTCodici.ToString());

            stbQuery.AppendLine("   SELECT ");
            stbQuery.AppendLine("          Appezzamento.PIVA AS [keyPiva] ");
            stbQuery.AppendLine("        , Appezzamento.sa_cod AS [keySaCod] ");
            stbQuery.AppendLine("        , Appezzamento.Appezza AS [keyAppezza] ");
            stbQuery.AppendLine(stbSelectCodici.ToString());

            stbQuery.AppendLine("   INTO #AppezzamentiDettagli_TT ");
            stbQuery.AppendLine("   FROM " + _modalitaBudget + "Appezzamento AS Appezzamento ");

            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.Appezzamento).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.Appezzamento).ToString());

            stbQuery.AppendLine(stbJoinCodici.ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                if (_modalitaBudget != "")
                    stbQuery.AppendLine("   AND Appezzamento.Id_Budget = @IdBudget ");
                stbQuery.AppendLine(StbFiltri.FiltroAppezzamentiDettagli.ToString());

                //Indice per ottimizzazione query su tabella temp
                stbQuery.AppendLine($" CREATE CLUSTERED INDEX IX_AppezzamentiDettagli_TT_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #AppezzamentiDettagli_TT (keyPiva, keySaCod, keyAppezza);");

            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#AppezzamentiDettagli_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #AppezzamentiDettagli_TT ");
            }

            return stbQuery.ToString();
        }
        #endregion

        #region "Impianti"
        private async Task<string> GetImpiantiBase_TTAsync(List<KendoColumn> kendoColumns,
                                                           StbFiltri StbFiltri,
                                                           AgronicaCoreParametriServer objParametriServer,
                                                           AgronicaCoreParametriUtenti objParametriUtenti)
        {

            #region "KENDO COLUMNS"
            if (_configuratore.needColumnsImpianti)
            {
                if (_fattoreConversione > 1)
                    kendoColumns.Add(new KendoColumn { Field = "Sup_Imp_Acri", Title = "SuperficieImpiantoAcriAbbr", DataType = "number", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                else
                    kendoColumns.Add(new KendoColumn { Field = "Sup_Imp", Title = "SuperficieImpiantoAbbr", DataType = "number", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                kendoColumns.Add(new KendoColumn { Field = "GruppoVegetale", Title = "GruppoVegetale", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "TipologiaVarietale", Title = "TipologiaVarietale", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                kendoColumns.Add(new KendoColumn { Field = "Utilizzo", Title = "Utilizzo", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Varieta", Title = "Varieta", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                kendoColumns.Add(new KendoColumn { Field = "Veg_Cod", Title = _localizer["Veg_Cod"], DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex), Hidden = true, TranslateTitle = false });
                kendoColumns.Add(new KendoColumn { Field = "Cul_Cod", Title = _localizer["Cul_Cod"], DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex), Hidden = true, TranslateTitle = false });
                kendoColumns.Add(new KendoColumn { Field = "Dest_Uso_Cod", Title = _localizer["Dest_Uso_Cod"], DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex), Hidden = true, TranslateTitle = false });
                kendoColumns.Add(new KendoColumn { Field = "Finalita", Title = "Finalità", DataType = "string", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                if (_configuratore.isMostraEsercizi)
                {

                    kendoColumns.Add(new KendoColumn { Field = "Portinnesto", Title = "Portinnesto", DataType = "string", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "FormaAllevamento", Title = "FormaAllevamento", DataType = "string", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Copertura", Title = "Copertura", DataType = "string", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "ImpiantoIrriguo", Title = "ImpiantoIrrigazione", DataType = "string", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                    kendoColumns.Add(new KendoColumn { Field = "Maschi_in_Sesto", Title = "MaschiSesto", DataType = "number", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                    kendoColumns.Add(new KendoColumn { Field = "Data_Inizio_Innesto", Title = "DataInizioInnesto", DataType = "date", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Data_Inizio_Portinnesto", Title = "DataInizioPortinnesto", DataType = "date", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Data_Inizio_Produzione", Title = "DataInizioProduzione", DataType = "date", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Data_Semina_Trapianto", Title = "DataSemina", DataType = "date", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                }

                if (_configuratore.CaricaDatiGISImpianto)
                {
                    kendoColumns.Add(new KendoColumn { Field = "GIS", Title = "GIS", DataType = "string", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Baricentro", Title = "Baricentro", DataType = "string", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                }

                //Colonne non visibili all'utente
                kendoColumns.Add(new KendoColumn { Field = "Grfi_Cod", Title = "Grfi_Cod", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false });
                kendoColumns.Add(new KendoColumn { Field = "Grva_Cod_Veg", Title = "Grva_Cod_Veg", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false });
                kendoColumns.Add(new KendoColumn { Field = "Cop_Cod", Title = "Cop_Cod", DataType = "string", ColumnOrder = columnOrder_stdGIAS, Display = false });

                if (_configuratore.isMostraPianoColturale)
                {
                    kendoColumns.Add(new KendoColumn { Field = "Data_Creazione_Impianto", Title = "DataCreazioneImpianto", DataType = "date", Hidden = true, ColumnOrder = columnOrder_stdGIAS });
                    kendoColumns.Add(new KendoColumn { Field = "Username_Creazione_Impianto", Title = "UtenteCreazioneImpianto", DataType = "string", Hidden = true, ColumnOrder = columnOrder_stdGIAS });
                }

                if (_configuratore.isMostraMovimenti || _configuratore.isMostraEsercizi)
                {
                    kendoColumns.Add(new KendoColumn { Field = "Validita_Inizio_Impianto", Title = "Validita_Inizio_Impianto", DataType = "date", ColumnOrder = SetColumnOrder(ref columnOrderIndex), Hidden = true });
                    kendoColumns.Add(new KendoColumn { Field = "Validita_Fine_Impianto", Title = "Validita_Fine_Impianto", DataType = "date", ColumnOrder = SetColumnOrder(ref columnOrderIndex), Hidden = true });
                }
            }
            #endregion

            #region "QUERY"  
            var stbQuery = new StringBuilder();
            var stbJoinUtenti = new StringBuilder();

            stbQuery.AppendLine(GetDestinazioneUso_TT(StbFiltri).ToString());

            if (_configuratore.isMostraEsercizi)
                stbQuery.AppendLine(GetDataSeminaTrapianto_TT().ToString());
            if (_configuratore.joinGIS)
                stbQuery.AppendLine(GetImpiantiGIS_TT(StbFiltri).ToString());
            if (_configuratore.joinOperazionixImpianto)
                stbQuery.AppendLine(GetOperazioniAgenda_TT(StbFiltri, joinSaCod: true, joinAppezza: true, joinIdReg: true).ToString());

            stbQuery.AppendLine("   SELECT ");
            stbQuery.AppendLine("         Reg_Impianti.PIVA AS [keyPiva] ");
            stbQuery.AppendLine("       , Reg_Impianti.sa_cod AS [keySaCod] ");
            stbQuery.AppendLine("       , Reg_Impianti.Appezza AS [keyAppezza] ");
            stbQuery.AppendLine("       , Reg_Impianti.Id_Reg AS [keyIdReg] ");

            if (_configuratore.needColumnsImpianti)
            {
                stbQuery.AppendLine("       , Reg_Impianti.Id_Reg ");

                if (_configuratore.IncludiEscludiDestinazioniUso == (int)Enum_FiltroDestinazioneUso_FiltroRicerca.SoloDestinazioniUso)
                    stbQuery.AppendLine("       , 0 AS Veg_Cod ");
                else
                    stbQuery.AppendLine("       , ISNULL(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod ");

                stbQuery.AppendLine("       , Reg_Impianti.Cul_Cod ");
                stbQuery.AppendLine("       , Reg_Impianti.Grfi_Cod ");
                stbQuery.AppendLine("       , Reg_Impianti.Grva_Cod_Veg ");
                stbQuery.AppendLine("       , Reg_Impianti.Cop_Cod ");

                if (_fattoreConversione > 1)
                    stbQuery.AppendLine("       , ROUND(ISNULL(Reg_Impianti.Sup_Imp,0) * " + _fattoreConversione.ToString().Replace(",", ".") + ", 4) AS Sup_Imp_Acri ");
                else
                    stbQuery.AppendLine("       , Reg_Impianti.Sup_Imp ");

                switch (_configuratore.IncludiEscludiDestinazioniUso)
                {
                    case (int)Enum_FiltroDestinazioneUso_FiltroRicerca.SoloDestinazioniUso:
                        stbQuery.AppendLine("       , ISNULL(DestinazioniUso_TT.codice, 0) as Dest_Uso_Cod");
                        stbQuery.AppendLine("       , '' AS GruppoVegetale ");
                        stbQuery.AppendLine($"       , ISNULL(DestinazioniUso_TT.Descrizione, '{_localizer["TerrenoNudo"]}')  AS Utilizzo ");
                        stbQuery.AppendLine("       , '' AS TipologiaVarietale ");
                        stbQuery.AppendLine("       , '' AS Varieta ");
                        break;
                    case (int)Enum_FiltroDestinazioneUso_FiltroRicerca.EscludiDestinazioniUso:
                        stbQuery.AppendLine("       , 0 as Dest_Uso_Cod");
                        stbQuery.AppendLine("       , ISNULL(GruppoVegetale.Gru_Des, '') AS GruppoVegetale ");
                        stbQuery.AppendLine("       , ISNULL(SpecieVegetali.Veg_Des, '')  AS Utilizzo ");
                        stbQuery.AppendLine("       , ISNULL(TipologiaVarietale.GRVA_DES, '') AS TipologiaVarietale ");
                        stbQuery.AppendLine("       , ISNULL(Cultivar.Cul_Des, '') AS Varieta ");
                        break;
                    default:
                        stbQuery.AppendLine("       , ISNULL(DestinazioniUso_TT.codice, 0) as Dest_Uso_Cod");
                        stbQuery.AppendLine("       , ISNULL(GruppoVegetale.Gru_Des, '') AS GruppoVegetale ");
                        stbQuery.AppendLine("       , CASE Reg_Impianti.Cul_Cod WHEN 0 ");
                        stbQuery.AppendLine($"           THEN ISNULL(DestinazioniUso_TT.Descrizione, '{_localizer["TerrenoNudo"]}') ");
                        stbQuery.AppendLine("           ELSE SpecieVegetali.Veg_Des END AS Utilizzo ");
                        stbQuery.AppendLine("       , ISNULL(TipologiaVarietale.GRVA_DES, '') AS TipologiaVarietale ");
                        stbQuery.AppendLine("       , ISNULL(Cultivar.Cul_Des, '') AS Varieta ");
                        break;
                }

                stbQuery.AppendLine("       , GruppoFinalita.GRFI_DES AS Finalita ");

                if (_configuratore.isMostraMovimenti || _configuratore.isMostraEsercizi)
                {
                    stbQuery.AppendLine("       , Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto ");
                    stbQuery.AppendLine("       , Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto ");
                }

                if (_configuratore.isMostraEsercizi)
                {
                    stbQuery.AppendLine("       , ISNULL(Port_Des, '') AS Portinnesto ");
                    stbQuery.AppendLine("       , ISNULL(Foral_Des, '') AS FormaAllevamento ");
                    stbQuery.AppendLine("       , ISNULL(Cop_Des, '') AS Copertura ");
                    stbQuery.AppendLine("       , ISNULL(Imp_Des, '') AS ImpiantoIrriguo ");

                    stbQuery.AppendLine("       , CASE WHEN Reg_Impianti.Piante_Maschi_InSesto = 0 ");
                    stbQuery.AppendLine($"                THEN '{_localizer["No"]}' ");
                    stbQuery.AppendLine("                ELSE CASE WHEN Reg_Impianti.Piante_Maschi_InSesto = 1 ");
                    stbQuery.AppendLine($"                               THEN '{_localizer["Si"]}' ");
                    stbQuery.AppendLine("                     END ");
                    stbQuery.AppendLine("         END AS Maschi_in_Sesto ");

                    stbQuery.AppendLine("       , CASE WHEN Reg_Impianti.Data_Inizio_Innesto = CAST('1900-01-01' AS Date) ");
                    stbQuery.AppendLine("                THEN NULL ");
                    stbQuery.AppendLine("                ELSE Reg_Impianti.Data_Inizio_Innesto ");
                    stbQuery.AppendLine("         END AS Data_Inizio_Innesto ");

                    stbQuery.AppendLine("       , CASE WHEN Reg_Impianti.Data_Inizio_Portinnesto = CAST('1900-01-01' AS Date) ");
                    stbQuery.AppendLine("                THEN NULL ");
                    stbQuery.AppendLine("                ELSE Reg_Impianti.Data_Inizio_Portinnesto ");
                    stbQuery.AppendLine("         END AS Data_Inizio_Portinnesto ");

                    stbQuery.AppendLine("       , CASE WHEN Reg_Impianti.Data_Inizio_Produzione = CAST('2100-12-31' AS Date) ");
                    stbQuery.AppendLine("                THEN NULL ");
                    stbQuery.AppendLine("                ELSE Reg_Impianti.Data_Inizio_Produzione ");
                    stbQuery.AppendLine("         END AS Data_Inizio_Produzione ");

                    stbQuery.AppendLine("       , ISNULL(DataSeminaTrapianto_TT.[Data], CAST('1900-01-01' AS Date)) AS Data_Semina_Trapianto ");
                }

                await GetBaseColumnsAsync(stbQuery, stbJoinUtenti, $"{_modalitaBudget}{Enum_Tabelle.Impianti}", kendoColumns, !_configuratore.isMostraImpianti, objParametriServer);

                if (_configuratore.isMostraImpianti || _configuratore.isMostraPianoColturale)
                {
                    stbQuery.AppendLine("        , Creazione.Username AS Username_Creazione" + (_configuratore.isMostraPianoColturale ? "_Impianto" : "") + " ");
                    if (_configuratore.isMostraImpianti)
                        stbQuery.AppendLine("        , Modifica.Username AS Username_Modifica ");
                }

                if (_configuratore.CaricaDatiGISImpianto)
                {
                    stbQuery.AppendLine("       , ISNULL(GIS, '') AS GIS ");
                    stbQuery.AppendLine("       , ISNULL(Baricentro, '') AS Baricentro ");
                }

                if (_configuratore.isMostraPianoColturale)
                    stbQuery.AppendLine("        , Reg_Impianti.Data_Creazione AS Data_Creazione_Impianto ");

            }

            stbQuery.AppendLine("   INTO #ImpiantiBase_TT ");
            stbQuery.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti AS Reg_Impianti ");

            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.Impianto).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.Appezzamento).ToString());

            stbQuery.AppendLine("   LEFT JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.CUL_COD ");

            switch (_configuratore.IncludiEscludiDestinazioniUso)
            {
                case (int)Enum_FiltroDestinazioneUso_FiltroRicerca.SoloDestinazioniUso:
                    stbQuery.AppendLine(" JOIN #DestinazioniUso_TT DestinazioniUso_TT ON ");
                    stbQuery.AppendLine("     DestinazioniUso_TT.[keyPiva1] = Reg_Impianti.PIVA ");
                    stbQuery.AppendLine(" AND DestinazioniUso_TT.[keySaCod1] = Reg_Impianti.Sa_Cod ");
                    stbQuery.AppendLine(" AND DestinazioniUso_TT.[keyAppezza1] = Reg_Impianti.Appezza ");
                    stbQuery.AppendLine(" AND DestinazioniUso_TT.[keyIdReg1] = Reg_Impianti.Id_Reg ");
                    break;
                case (int)Enum_FiltroDestinazioneUso_FiltroRicerca.EscludiDestinazioniUso:
                    stbQuery.AppendLine(" JOIN SpecieVegetali ON Cultivar.VEG_COD = SpecieVegetali.VEG_COD ");
                    stbQuery.AppendLine(" JOIN GruppoVegetale ON GruppoVegetale.GRU_COD = SpecieVegetali.GRU_COD ");
                    stbQuery.AppendLine(" LEFT JOIN GruppoVarietale TipologiaVarietale ON abs(Reg_Impianti.GRVA_Cod_VEG) = TipologiaVarietale.GRVA_COD ");
                    break;
                default:
                    stbQuery.AppendLine(" LEFT JOIN SpecieVegetali ON Cultivar.VEG_COD = SpecieVegetali.VEG_COD ");
                    stbQuery.AppendLine(" LEFT JOIN GruppoVegetale ON GruppoVegetale.GRU_COD = SpecieVegetali.GRU_COD ");
                    stbQuery.AppendLine(" LEFT JOIN GruppoVarietale TipologiaVarietale ON abs(Reg_Impianti.GRVA_Cod_VEG) = TipologiaVarietale.GRVA_COD ");

                    stbQuery.AppendLine(" LEFT JOIN #DestinazioniUso_TT DestinazioniUso_TT ON ");
                    stbQuery.AppendLine("     DestinazioniUso_TT.[keyPiva1] = Reg_Impianti.PIVA ");
                    stbQuery.AppendLine(" AND DestinazioniUso_TT.[keySaCod1] = Reg_Impianti.Sa_Cod ");
                    stbQuery.AppendLine(" AND DestinazioniUso_TT.[keyAppezza1] = Reg_Impianti.Appezza ");
                    stbQuery.AppendLine(" AND DestinazioniUso_TT.[keyIdReg1] = Reg_Impianti.Id_Reg ");
                    break;
            }

            stbQuery.AppendLine(" LEFT JOIN GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod ");

            if (_configuratore.isMostraEsercizi)
            {

                stbQuery.AppendLine(" LEFT JOIN Portinnesti ON Reg_Impianti.Port_Cod = Portinnesti.Port_Cod ");
                stbQuery.AppendLine(" LEFT JOIN FormeAllevamento ON Reg_Impianti.Foral_Cod = FormeAllevamento.Foral_Cod ");
                stbQuery.AppendLine(" LEFT JOIN Copertura ON Reg_Impianti.Cop_Cod = Copertura.Cop_Cod ");
                stbQuery.AppendLine(" LEFT JOIN ImpiantiIrrigazioni ON Reg_Impianti.Imp_Cod = ImpiantiIrrigazioni.Imp_Cod ");

                stbQuery.AppendLine(" LEFT JOIN #DataSeminaTrapianto_TT DataSeminaTrapianto_TT ON ");
                stbQuery.AppendLine("     DataSeminaTrapianto_TT.[keyPiva3] = Reg_Impianti.PIVA ");
                stbQuery.AppendLine(" AND DataSeminaTrapianto_TT.[keySaCod3] = Reg_Impianti.Sa_Cod ");
                stbQuery.AppendLine(" AND DataSeminaTrapianto_TT.[keyAppezza3] = Reg_Impianti.Appezza ");
                stbQuery.AppendLine(" AND DataSeminaTrapianto_TT.[keyIdReg3] = Reg_Impianti.Id_Reg ");
            }

            if (_configuratore.joinGIS)
            {
                if (_configuratore.IncludiEscludiPoligoni == (int)Enum_FiltroPoligoni_FiltroRicerca.ConPoligoni || _configuratore.joinGISAnomalie)
                    stbQuery.AppendLine(" JOIN #ImpiantiGIS_TT ImpiantiGIS_TT ON ");
                else
                    stbQuery.AppendLine(" LEFT JOIN #ImpiantiGIS_TT ImpiantiGIS_TT ON ");

                stbQuery.AppendLine("     ImpiantiGIS_TT.[keyPiva9] = Reg_Impianti.PIVA ");
                stbQuery.AppendLine(" AND ImpiantiGIS_TT.[keySaCod9] = Reg_Impianti.Sa_Cod ");
                stbQuery.AppendLine(" AND ImpiantiGIS_TT.[keyAppezza9] = Reg_Impianti.Appezza ");
                stbQuery.AppendLine(" AND ImpiantiGIS_TT.[keyIdReg9] = Reg_Impianti.Id_Reg ");
            }

            if (_configuratore.joinOperazionixImpianto)
            {
                if (_configuratore.IncludiEscludiOperazioniAgenda == (int)Enum_FiltroOperazioniAgenda_FiltroRicerca.ConOperazioni)
                    stbQuery.AppendLine(" JOIN #OperazioniAgenda_TT OperazioniAgenda_TT ON ");
                else
                    stbQuery.AppendLine(" LEFT JOIN #OperazioniAgenda_TT OperazioniAgenda_TT ON ");

                stbQuery.AppendLine("     OperazioniAgenda_TT.[keyPiva10] = Reg_Impianti.PIVA ");
                stbQuery.AppendLine(" AND OperazioniAgenda_TT.[keySaCod10] = Reg_Impianti.Sa_Cod ");
                stbQuery.AppendLine(" AND OperazioniAgenda_TT.[keyAppezza10] = Reg_Impianti.Appezza ");
                stbQuery.AppendLine(" AND OperazioniAgenda_TT.[keyIdReg10] = Reg_Impianti.Id_Reg ");
            }

            if (_configuratore.isMostraImpianti || _configuratore.isMostraPianoColturale)
            {
                stbQuery.AppendLine(" LEFT JOIN #UtentiDettagli_TT Creazione ON Creazione.[keyUtente] = Reg_Impianti.Username_Creazione ");
                if (_configuratore.isMostraImpianti)
                    stbQuery.AppendLine(" LEFT JOIN #UtentiDettagli_TT Modifica ON Modifica.[keyUtente] = Reg_Impianti.Username_Modifica ");
            }

            stbQuery.AppendLine(stbJoinUtenti.ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                if (_modalitaBudget != "")
                    stbQuery.AppendLine(" AND Reg_Impianti.Id_Budget = @IdBudget ");
                stbQuery.AppendLine(StbFiltri.FiltroImpiantiBase.ToString());

                //Indice per ottimizzazione query su tabella temp
                stbQuery.AppendLine($" CREATE CLUSTERED INDEX IX_ImpiantiBase_TT_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #ImpiantiBase_TT (keyPiva, keySaCod, keyAppezza, keyIdReg);");

            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#ImpiantiBase_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #ImpiantiBase_TT ");
            }

            #endregion

            return stbQuery.ToString();
        }

        private string GetImpiantiDettagli_TT(ref List<KendoColumn> kendoColumns,
                                              StbFiltri StbFiltri,
                                              AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var strSqlTTCodici = new StringBuilder();
            var stbSelectCodici = new StringBuilder();
            var stbJoinCodici = new StringBuilder();

            GetCodici(Enum_Tabelle.Impianti, _configuratore.CodiciImpianti, ref strSqlTTCodici, ref stbSelectCodici, ref stbJoinCodici, ref kendoColumns, objParametriServer);

            stbQuery.AppendLine(strSqlTTCodici.ToString());

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("       Reg_Impianti.PIVA AS [keyPiva] ");
            stbQuery.AppendLine("     , Reg_Impianti.sa_cod AS [keySaCod] ");
            stbQuery.AppendLine("     , Reg_Impianti.Appezza AS [keyAppezza] ");
            stbQuery.AppendLine("     , Reg_Impianti.Id_Reg AS [keyIdReg] ");
            stbQuery.AppendLine(stbSelectCodici.ToString());

            stbQuery.AppendLine(" INTO #ImpiantiDettagli_TT ");
            stbQuery.AppendLine(" FROM " + _modalitaBudget + "Reg_Impianti AS Reg_Impianti ");

            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.Impianto).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.Impianto).ToString());

            stbQuery.AppendLine(stbJoinCodici.ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                if (_modalitaBudget != "")
                    stbQuery.AppendLine(" AND Reg_Impianti.Id_Budget = @IdBudget ");
                stbQuery.AppendLine(StbFiltri.FiltroImpiantiDettagli.ToString());

                //Indice per ottimizzazione query su tabella temp
                stbQuery.AppendLine($" CREATE CLUSTERED INDEX IX_ImpiantiDettagli_TT_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #ImpiantiDettagli_TT (keyPiva, keySaCod, keyAppezza, keyIdReg);");

            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#ImpiantiDettagli_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #ImpiantiDettagli_TT ");
            }

            return stbQuery.ToString();
        }
        #endregion

        #region "Esercizi"
        private async Task<string> GetEserciziBase_TTAsync(List<KendoColumn> kendoColumns,
                                                           StbFiltri StbFiltri,
                                                           AgronicaCoreParametriServer objParametriServer,
                                                           AgronicaCoreParametriUtenti objParametriUtenti)
        {

            #region "KENDO COLUMNS"
            if (_configuratore.needColumnsEsercizi)
            {
                kendoColumns.Add(new KendoColumn { Field = "Vincolo", Title = "Vincolo", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                kendoColumns.Add(new KendoColumn { Field = "EsercizioChiuso", Title = "EsercizioChiuso", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                kendoColumns.Add(new KendoColumn { Field = "Lotto", Title = "Lotto", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Descrizione", Title = "Descrizione", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                if (_configuratore.isMostraEsercizi)
                {

                    kendoColumns.Add(new KendoColumn { Field = "Prodotto", Title = "Prodotto", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "GruppoRaccoltaEsercizio", Title = "GruppoRaccoltaEsercizio", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                    kendoColumns.Add(new KendoColumn { Field = "Stato_Impianto", Title = "StatoImpianto", DataType = "string", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Data_Raccolta_Prevista", Title = "DataRaccoltaPrevista", DataType = "date", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Data_Fioritura_Prevista", Title = "DataFiorituraPrevista", DataType = "date", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Data_Semina_Prevista", Title = "DataSeminaPrevista", DataType = "date", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                    kendoColumns.Add(new KendoColumn { Field = "Validita_Inizio_Esercizio", Title = "ValiditaInizioEsercizio", DataType = "date", ColumnOrder = SetColumnOrder(ref columnOrderIndex), Hidden = true });
                    kendoColumns.Add(new KendoColumn { Field = "Validita_Fine_Esercizio", Title = "ValiditaFineEsercizio", DataType = "date", ColumnOrder = SetColumnOrder(ref columnOrderIndex), Hidden = true });
                    //kendoColumns.Add(new KendoColumn { Field = "Durata_Esercizio", Title = "DurataEsercizio", DataType = "string", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                    kendoColumns.Add(new KendoColumn { Field = "Numero_Totale_Piante", Title = "NumeroTotalePianteAbbr", DataType = "number", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Numero_Piante_Femmina", Title = "NumeroPianteFemminaAbbr", DataType = "number", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Numero_Piante_Maschio", Title = "NumeroPianteMaschioAbbr", DataType = "number", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                    kendoColumns.Add(new KendoColumn { Field = "Resa_Totale_Prevista", Title = "ResaTotalePrevista", DataType = "number", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });

                    //colonne non visibili all'utente
                    kendoColumns.Add(new KendoColumn { Field = "Finanziamento", Title = "Finanziamento", DataType = "number", Display = false, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                    kendoColumns.Add(new KendoColumn { Field = "Regolamento", Title = "Regolamento", DataType = "number", Display = false, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                }

                if (_configuratore.CaricaContributiACA)
                    kendoColumns.Add(new KendoColumn { Field = "ContributiACA", Title = "ContributiACA", DataType = "string", Hidden = false, ColumnOrder = columnOrder_stdGIAS });

                if (_configuratore.isMostraPianoColturale)
                {
                    kendoColumns.Add(new KendoColumn { Field = "Data_Creazione_Esercizio", Title = "DataCreazioneEsercizio", DataType = "date", Hidden = true, ColumnOrder = columnOrder_stdGIAS });
                    kendoColumns.Add(new KendoColumn { Field = "Data_Modifica_Esercizio", Title = "DataModificaEsercizio", DataType = "date", Hidden = true, ColumnOrder = columnOrder_stdGIAS });
                    kendoColumns.Add(new KendoColumn { Field = "Username_Creazione_Esercizio", Title = "UtenteCreazioneEsercizio", DataType = "string", Hidden = true, ColumnOrder = columnOrder_stdGIAS });
                }
            }
            #endregion

            #region "QUERY"  
            var stbQuery = new StringBuilder();
            var stbJoinUtenti = new StringBuilder();

            if (_configuratore.joinOperazionixEsercizio)
                stbQuery.AppendLine(GetOperazioniAgenda_TT(StbFiltri, joinSaCod: true, joinAppezza: true, joinIdReg: true, joinProgettoCod: true).ToString());

            if (_configuratore.joinContributiACA)
                stbQuery.AppendLine(GetContributiACA_TT(StbFiltri).ToString());

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     Imprese_Progetti.PIVA AS [keyPiva] ");
            stbQuery.AppendLine("   , Imprese_Progetti.sa_cod AS [keySaCod] ");
            stbQuery.AppendLine("   , Imprese_Progetti.Appezza AS [keyAppezza] ");
            stbQuery.AppendLine("   , Imprese_Progetti.Id_Reg AS [keyIdReg] ");
            stbQuery.AppendLine("   , Imprese_Progetti.Progetto_Cod AS [keyProgettoCod] ");

            if (_configuratore.isMostraMovimenti)
            {
                //Estraggo validità inizio/fine per la join con i movimenti
                stbQuery.AppendLine("   , Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Esercizio_JoinMovimenti ");
                stbQuery.AppendLine("   , Imprese_Progetti.Validita_Fine AS Validita_Fine_Esercizio_JoinMovimenti ");
            }

            if (_configuratore.needColumnsEsercizi)
            {
                stbQuery.AppendLine("   , Imprese_Progetti.Progetto_Cod ");

                stbQuery.AppendLine("   , ISNULL(Imprese_Progetti.Data_Fine_Prevista, CAST('2100-12-31' AS Date)) AS Data_Raccolta_Prevista ");

                stbQuery.AppendLine($"   , CASE WHEN Imprese_Progetti.Validita_Fine <= GETDATE() THEN '{_localizer["Si"]}' ELSE '{_localizer["No"]}' END AS EsercizioChiuso ");

                stbQuery.AppendLine("   , CASE WHEN DPI_Regolamenti.COD_REGOLAMENTO IS NULL ");
                stbQuery.AppendLine("           THEN CAST(Imprese_Progetti.Regolamento_Cod AS VARCHAR(100)) ");
                stbQuery.AppendLine("          ELSE CAST(DPI_Regolamenti.Flag_Privato_Pubblico AS VARCHAR(10)) + '_' + CAST(DPI_Regolamenti.COD_REGOLAMENTO AS VARCHAR(10)) ");
                stbQuery.AppendLine("     END AS RegolamentoDisciplinare_Cod ");
                stbQuery.AppendLine("   , CASE WHEN DPI_Regolamenti.NomeEsteso IS NULL ");
                stbQuery.AppendLine("             THEN CASE WHEN Regolamenti.Reg_Cod = '" + (int)Enum_Cod_Regolamento.Regolamento_Bio + "' ");
                stbQuery.AppendLine($"                           THEN '{_localizer["Bio"]}' ELSE Regolamenti.Reg_Des ");
                stbQuery.AppendLine("                  END ");
                stbQuery.AppendLine("             ELSE DPI_Regolamenti.NomeEsteso ");
                stbQuery.AppendLine("     END AS Vincolo ");

                stbQuery.AppendLine("   , Imprese_Progetti.Progetto_Nome AS Lotto ");
                stbQuery.AppendLine("   , Imprese_Progetti.Progetto_Des AS Descrizione ");

                if (_configuratore.isMostraEsercizi)
                {
                    await GetBaseColumnsAsync(stbQuery, stbJoinUtenti, $"{_modalitaBudget}{Enum_Tabelle.Esercizi}", kendoColumns, !(_configuratore.isMostraEsercizi && !_configuratore.isMostraPianoColturale), objParametriServer);

                    stbQuery.AppendLine("   , Imprese_Progetti.Regolamento_Cod AS Regolamento ");
                    stbQuery.AppendLine("   , Imprese_Progetti.Disciplinare_Cod AS Finanziamento ");

                    stbQuery.AppendLine("   , ISNULL(Imprese_Progetti.Data_Fioritura_Prevista, CAST('2100-12-31' AS Date)) AS Data_Fioritura_Prevista ");
                    stbQuery.AppendLine("   , ISNULL(Imprese_Progetti.Data_Inizio_Prevista, CAST('1900-01-01' AS Date)) AS Data_Semina_Prevista ");

                    stbQuery.AppendLine("   , Imprese_progetti.P_HA_Femmine AS Numero_Piante_Femmina ");
                    stbQuery.AppendLine("   , CASE WHEN Imprese_Progetti.Stato_Impianto = 0 THEN '' ELSE COALESCE(FasiCicloColturale_Anagrafiche.Fase_Des, CASE WHEN Imprese_Progetti.Stato_Impianto = 102 THEN 'In produzione' ELSE '' END) END AS Stato_Impianto ");
                    stbQuery.AppendLine("   , ISNULL(Materie_Prime.Mat_Des, '') AS Prodotto ");

                    stbQuery.AppendLine("   , ISNULL(Gruppi_Raccolta.GruppoRaccolta_Des, '') AS GruppoRaccoltaEsercizio ");

                    stbQuery.AppendLine("   , Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Esercizio ");
                    stbQuery.AppendLine("   , Imprese_Progetti.Validita_Fine AS Validita_Fine_Esercizio ");

                    //stbQuery.AppendLine("       , CASE WHEN CONVERT(date, Imprese_Progetti.Validita_Inizio, 120) <> '01/01/1900' THEN CONVERT(VARCHAR(100), FORMAT(Imprese_Progetti.Validita_Inizio, 'dd/MM/yyyy')) ELSE '...' END + ' - ' + ");
                    //stbQuery.AppendLine("         CASE WHEN CONVERT(date, Imprese_Progetti.Validita_Fine, 120) <> '31/12/2100' THEN CONVERT(VARCHAR(100), FORMAT(Imprese_Progetti.Validita_Fine, 'dd/MM/yyyy')) ELSE '...' END AS Durata_Esercizio ");

                    stbQuery.AppendLine("   , Creazione.Username AS Username_Creazione" + (_configuratore.isMostraPianoColturale ? "_Esercizio" : "") + " ");
                    stbQuery.AppendLine("   , Modifica.Username AS Username_Modifica ");
                }

                if (_configuratore.CaricaContributiACA)
                    stbQuery.AppendLine("   , ISNULL(ContributiACA_TT.ContributiACA, '') AS ContributiACA ");

                if (_configuratore.isMostraPianoColturale)
                {
                    stbQuery.AppendLine("   , Imprese_Progetti.Data_Creazione AS Data_Creazione_Esercizio ");
                    stbQuery.AppendLine("   , Imprese_Progetti.Data_Modifica AS Data_Modifica_Esercizio ");

                }
            }

            stbQuery.AppendLine(" INTO #EserciziBase_TT ");
            stbQuery.AppendLine(" FROM " + _modalitaBudget + "Imprese_Progetti AS Imprese_Progetti ");

            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.Esercizio).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.Impianto).ToString());

            stbQuery.AppendLine(" LEFT JOIN DPI_Regolamenti ON ");
            stbQuery.AppendLine("     Imprese_Progetti.Disciplinare_Cod = DPI_Regolamenti.COD_REGOLAMENTO ");
            stbQuery.AppendLine(" AND Imprese_Progetti.Disciplinare_PubblicoPrivato = DPI_Regolamenti.Flag_Privato_Pubblico ");

            stbQuery.AppendLine(" LEFT JOIN Regolamenti (NOLOCK) ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod ");

            stbQuery.AppendLine(" LEFT JOIN FasiCicloColturale_Anagrafiche ON Imprese_Progetti.Stato_Impianto = FasiCicloColturale_Anagrafiche.Fase_Cod ");

            if (_configuratore.joinOperazionixEsercizio)
            {
                if (_configuratore.IncludiEscludiOperazioniAgenda == (int)Enum_FiltroOperazioniAgenda_FiltroRicerca.ConOperazioni)
                    stbQuery.AppendLine(" JOIN #OperazioniAgenda_TT OperazioniAgenda_TT ON ");
                else
                    stbQuery.AppendLine(" LEFT JOIN #OperazioniAgenda_TT OperazioniAgenda_TT ON ");

                stbQuery.AppendLine("     OperazioniAgenda_TT.[keyPiva10] = Imprese_Progetti.PIVA ");
                stbQuery.AppendLine(" AND OperazioniAgenda_TT.[keySaCod10] = Imprese_Progetti.Sa_Cod ");
                stbQuery.AppendLine(" AND OperazioniAgenda_TT.[keyAppezza10] = Imprese_Progetti.Appezza ");
                stbQuery.AppendLine(" AND OperazioniAgenda_TT.[keyIdReg10] = Imprese_Progetti.Id_Reg ");
                stbQuery.AppendLine(" AND OperazioniAgenda_TT.[keyProgettoCod10] = Imprese_Progetti.Progetto_Cod ");
            }

            if (_configuratore.joinContributiACA)
                stbQuery.AppendLine($"   {(_configuratore.ApplicaFiltroContributiACA ? "INNER" : "LEFT")} JOIN #ContributiACA_TT ContributiACA_TT ON ContributiACA_TT.[keyProgettoCod11] = Imprese_Progetti.Progetto_Cod ");

            if (_configuratore.isMostraEsercizi)
            {
                stbQuery.AppendLine(" LEFT JOIN Materie_Prime ON Materie_Prime.Mat_Cod = Imprese_Progetti.Mat_Cod AND Materie_Prime.Elem_Cod = " + TRASFORMATI_VEGETALI + " ");
                stbQuery.AppendLine(" LEFT JOIN Gruppi_Raccolta ON Gruppi_Raccolta.GruppoRaccolta_Cod = Imprese_Progetti.GruppoRaccolta_Cod ");

                stbQuery.AppendLine(" LEFT JOIN #UtentiDettagli_TT Creazione ON Creazione.[keyUtente] = Imprese_Progetti.Username_Creazione ");
                stbQuery.AppendLine(" LEFT JOIN #UtentiDettagli_TT Modifica ON Modifica.[keyUtente] = Imprese_Progetti.Username_Modifica ");
            }

            stbQuery.AppendLine(stbJoinUtenti.ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                if (_modalitaBudget != "")
                    stbQuery.AppendLine(" AND Imprese_Progetti.Id_Budget = @IdBudget ");
                stbQuery.AppendLine(StbFiltri.FiltroEserciziBase.ToString());

                //Indice per ottimizzazione query su tabella temp
                stbQuery.AppendLine($" CREATE CLUSTERED INDEX IX_EserciziBase_TT_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #EserciziBase_TT (keyPiva, keySaCod, keyAppezza, keyIdReg, keyProgettoCod);");
            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#EserciziBase_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #EserciziBase_TT ");
            }

            #endregion

            return stbQuery.ToString();
        }

        private string GetEserciziDettagli_TT(ref List<KendoColumn> kendoColumns,
                                              StbFiltri StbFiltri,
                                              AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var strSqlTTCodici = new StringBuilder();
            var stbSelectCodici = new StringBuilder();
            var stbJoinCodici = new StringBuilder();

            GetCodici(Enum_Tabelle.Esercizi, _configuratore.CodiciEsercizi, ref strSqlTTCodici, ref stbSelectCodici, ref stbJoinCodici, ref kendoColumns, objParametriServer);

            stbQuery.AppendLine(strSqlTTCodici.ToString());

            stbQuery.AppendLine("   SELECT ");
            stbQuery.AppendLine("         Imprese_Progetti.PIVA AS [keyPiva] ");
            stbQuery.AppendLine("       , Imprese_Progetti.sa_cod AS [keySaCod] ");
            stbQuery.AppendLine("       , Imprese_Progetti.Appezza AS [keyAppezza] ");
            stbQuery.AppendLine("       , Imprese_Progetti.Id_Reg AS [keyIdReg] ");
            stbQuery.AppendLine("       , Imprese_Progetti.Progetto_Cod AS [keyProgettoCod] ");
            stbQuery.AppendLine(stbSelectCodici.ToString());

            stbQuery.AppendLine("   INTO #EserciziDettagli_TT ");
            stbQuery.AppendLine("   FROM " + _modalitaBudget + "Imprese_Progetti AS Imprese_Progetti ");

            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.Esercizio).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.Esercizio).ToString());

            stbQuery.AppendLine(stbJoinCodici.ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                if (_modalitaBudget != "")
                    stbQuery.AppendLine("   AND Imprese_Progetti.Id_Budget = @IdBudget ");

                stbQuery.AppendLine(StbFiltri.FiltroEserciziDettagli.ToString());

                //Indice per ottimizzazione query su tabella temp
                stbQuery.AppendLine($" CREATE CLUSTERED INDEX IX_EserciziDettagli_TT_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #EserciziDettagli_TT (keyPiva, keySaCod, keyAppezza, keyIdReg, keyProgettoCod);");

            }


            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#EserciziDettagli_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #EserciziDettagli_TT ");
            }

            return stbQuery.ToString();
        }
        #endregion

        #region "Fabbricati"
        private async Task<string> GetFabbricatiBase_TTAsync(List<KendoColumn> kendoColumns,
                                                             StbFiltri StbFiltri,
                                                             AgronicaCoreParametriServer objParametriServer,
                                                             AgronicaCoreParametriUtenti objParametriUtenti)
        {

            #region "KENDO COLUMNS"
            if (_configuratore.needColumnsFabbricati)
            {
                kendoColumns.Add(new KendoColumn { Field = "Denominazione", Title = "Denominazione", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
            }
            #endregion

            #region "QUERY"  
            var stbQuery = new StringBuilder();
            var stbJoinUtenti = new StringBuilder();

            stbQuery.AppendLine("   SELECT ");
            stbQuery.AppendLine("          Fabbricati.PIVA AS [keyPiva] ");
            stbQuery.AppendLine("        , Fabbricati.Sa_Cod AS [keySaCod] ");
            stbQuery.AppendLine("        , Fabbricati.Fabbricato_Cod AS [keyFabbricatoCod] ");

            if (_configuratore.needColumnsFabbricati)
            {
                stbQuery.AppendLine("       , Fabbricati.Fabbricato_Cod ");

                stbQuery.AppendLine("       , Fabbricati.Fabbricato_Des AS Denominazione ");

                await GetBaseColumnsAsync(stbQuery, stbJoinUtenti, Enum_Tabelle.Fabbricati, kendoColumns, !_configuratore.isMostraFabbricati, objParametriServer);

                if (_configuratore.isMostraFabbricati)
                {
                    stbQuery.AppendLine("        , Creazione.Username AS Username_Creazione ");
                    stbQuery.AppendLine("        , Modifica.Username AS Username_Modifica ");
                }
            }

            stbQuery.AppendLine("   INTO #FabbricatiBase_TT ");
            stbQuery.AppendLine("   FROM Fabbricati ");

            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.Fabbricato).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.CentroAziendale).ToString());

            if (_configuratore.isMostraFabbricati)
            {
                stbQuery.AppendLine("   LEFT JOIN #UtentiDettagli_TT Creazione ON Creazione.[keyUtente] = Fabbricati.Username_Creazione ");
                stbQuery.AppendLine("   LEFT JOIN #UtentiDettagli_TT Modifica ON Modifica.[keyUtente] = Fabbricati.Username_Modifica ");
            }

            stbQuery.AppendLine(stbJoinUtenti.ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine(StbFiltri.FiltroFabbricatiBase.ToString());
            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#FabbricatiBase_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #FabbricatiBase_TT ");
            }

            #endregion

            return stbQuery.ToString();
        }

        private string GetFabbricatiDettagli_TT(ref List<KendoColumn> kendoColumns,
                                                StbFiltri StbFiltri,
                                                AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var strSqlTTCodici = new StringBuilder();
            var stbSelectCodici = new StringBuilder();
            var stbJoinCodici = new StringBuilder();

            GetCodici(Enum_Tabelle.Fabbricati, _configuratore.CodiciFabbricati, ref strSqlTTCodici, ref stbSelectCodici, ref stbJoinCodici, ref kendoColumns, objParametriServer);

            stbQuery.AppendLine(strSqlTTCodici.ToString());

            stbQuery.AppendLine("   SELECT ");
            stbQuery.AppendLine("         Fabbricati.PIVA AS [keyPiva] ");
            stbQuery.AppendLine("       , Fabbricati.sa_cod AS [keySaCod] ");
            stbQuery.AppendLine("       , Fabbricati.Fabbricato_Cod AS [keyFabbricatoCod] ");
            stbQuery.AppendLine(stbSelectCodici.ToString());

            stbQuery.AppendLine("   INTO #FabbricatiDettagli_TT ");
            stbQuery.AppendLine("   FROM Fabbricati ");

            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.Fabbricato).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.Fabbricato).ToString());

            stbQuery.AppendLine(stbJoinCodici.ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine(StbFiltri.FiltroFabbricatiDettagli.ToString());
            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#FabbricatiDettagli_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #FabbricatiDettagli_TT ");
            }

            return stbQuery.ToString();
        }

        #endregion

        #region "Movimenti"
        private string GetMovimentiBase_TT(ref List<KendoColumn> kendoColumns,
                                           StbFiltri StbFiltri,
                                           AgronicaCoreParametriUtenti objParametriUtenti)
        {
            #region "KENDO COLUMNS"
            if (_configuratore.isMostraMovimenti)
            {
                kendoColumns.Add(new KendoColumn { Field = "Gru_Des", Title = "GruppoOperazioni", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Lav_Des", Title = "Operazione", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Des_Lib", Title = "Descrizione", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Data_Movimento", Title = "DataMovimento", DataType = "date", ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "OperazioneBloccata", Title = "OperazioneBloccata", DataType = "string", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Id_Agenda", Title = "ChiaveAgenda", DataType = "number", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Username_Creazione_Agenda", Title = "UtenteCreazione", DataType = "string", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
                kendoColumns.Add(new KendoColumn { Field = "Username_Modifica_Agenda", Title = "UtenteModifica", DataType = "string", Hidden = true, ColumnOrder = SetColumnOrder(ref columnOrderIndex) });
            }
            #endregion

            #region "QUERY" 
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine(GetMovimenti_TT(StbFiltri).ToString());

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     Movimenti.[keyPiva] ");
            stbQuery.AppendLine("   , Movimenti.[keySaCod] ");
            stbQuery.AppendLine("   , Movimenti.[keyAppezza] ");
            stbQuery.AppendLine("   , Movimenti.[keyIdReg] ");

            if (_configuratore.isMostraMovimenti)
            {
                stbQuery.AppendLine("   , Agenda.Id_Agenda ");
                stbQuery.AppendLine("   , Agenda.Lav_Cod ");
                stbQuery.AppendLine("   , GruppoOperazioni.Gru_Des ");
                stbQuery.AppendLine("   , Operazioni.Lav_Des ");
                stbQuery.AppendLine("   , Agenda.Des_Lib ");

                stbQuery.AppendLine("   , Movimenti.Data_Movimento ");
                stbQuery.AppendLine("   , Movimenti.Num_Protocollo ");

                stbQuery.AppendLine($"   , CASE WHEN ISNULL(Agenda.Blocco_Flag, 0) = 0 THEN '{_localizer["No"]}' ELSE '{_localizer["Si"]}' END AS OperazioneBloccata ");

                stbQuery.AppendLine("   , CreazioneAgenda.Username AS Username_Creazione_Agenda ");
                stbQuery.AppendLine("   , ModificaAgenda.Username AS Username_Modifica_Agenda ");
            }

            stbQuery.AppendLine(" INTO #MovimentiBase_TT ");
            stbQuery.AppendLine(" FROM #Movimenti_TT Movimenti ");

            stbQuery.AppendLine(" JOIN Agenda ON ");
            stbQuery.AppendLine("     Agenda.Id_Agenda = Movimenti.keyIdAgenda ");

            if (_configuratore.isMostraMovimenti)
            {
                stbQuery.AppendLine(" JOIN Operazioni ON Operazioni.Lav_Cod = Agenda.Lav_Cod ");
                stbQuery.AppendLine(" JOIN GruppoOperazioni ON GruppoOperazioni.GRU_COD = Operazioni.GRU_OP ");

                stbQuery.AppendLine(" LEFT JOIN #UtentiDettagli_TT CreazioneAgenda ON CreazioneAgenda.[keyUtente] = Agenda.Username_Creazione ");
                stbQuery.AppendLine(" LEFT JOIN #UtentiDettagli_TT ModificaAgenda ON ModificaAgenda.[keyUtente] = Agenda.Username_Modifica ");
            }

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine(StbFiltri.FiltroAgenda.ToString());

                //Indice per ottimizzazione query su tabella temp
                stbQuery.AppendLine($" CREATE CLUSTERED INDEX IX_MovimentiBase_TT_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #MovimentiBase_TT (keyPiva, keySaCod, keyAppezza, keyIdReg);");
            }

            #endregion

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#MovimentiBase_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #MovimentiBase_TT ");
            }

            return stbQuery.ToString();
        }

        private string GetMovimenti_TT(StbFiltri stbFiltri)
        {
            StringBuilder stbQuery = new();

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     Mov_Destinazioni.Piva AS [keyPiva] ");
            stbQuery.AppendLine("   , Mov_Destinazioni.Sa_Cod AS [keySaCod] ");
            stbQuery.AppendLine("   , Mov_Destinazioni.Appezza AS [keyAppezza] ");
            stbQuery.AppendLine("   , Mov_Destinazioni.Id_Destinazione AS [keyIdReg] ");
            stbQuery.AppendLine("   , Mov_Destinazioni.Id_Agenda AS [keyIdAgenda] ");
            stbQuery.AppendLine("   , Mov_Destinazioni.Id_Mov AS [keyIdMov] ");
            stbQuery.AppendLine(" INTO #Mov_Destinazioni_TT ");
            stbQuery.AppendLine(" FROM Mov_Destinazioni ");
            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: "Mov_Destinazioni", joinTable: (int)Enum_Entita_FiltroRicerca.Esercizio).ToString());

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine($" AND Mov_Destinazioni.Tipo_Destinazione = {TIPO_DESTINAZIONE.TIPO_DESTINAZIONE_IMPIANTO} ");
            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#Mov_Destinazioni_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #Mov_Destinazioni_TT ");
            }

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     Movimenti.Id_Agenda AS [keyIdAgenda] ");
            stbQuery.AppendLine("   , Movimenti.Id_Mov AS [keyIdMov] ");
            stbQuery.AppendLine("   , Movimenti.Piva AS [keyPiva] ");
            stbQuery.AppendLine("   , Movimenti.Sa_Cod AS [keySaCod] ");
            stbQuery.AppendLine("   , Mov_Destinazioni.[keyAppezza]  ");
            stbQuery.AppendLine("   , Mov_Destinazioni.[keyIdReg] ");
            if (_configuratore.isMostraMovimenti)
            {
                stbQuery.AppendLine("   , Movimenti.Data_Movimento ");
                stbQuery.AppendLine("   , Movimenti.Num_Protocollo ");
            }

            stbQuery.AppendLine(" INTO #Movimenti_TT ");
            stbQuery.AppendLine(" FROM Movimenti ");
            stbQuery.AppendLine(" JOIN #Mov_Destinazioni_TT Mov_Destinazioni ON ");
            stbQuery.AppendLine("     Mov_Destinazioni.[keyPiva] = Movimenti.PIVA ");
            stbQuery.AppendLine(" AND Mov_Destinazioni.[keySaCod] = Movimenti.Sa_Cod ");
            stbQuery.AppendLine(" AND Mov_Destinazioni.keyIdAgenda = Movimenti.Id_Agenda ");
            stbQuery.AppendLine(" AND Mov_Destinazioni.keyIdMov = Movimenti.Id_Mov ");

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine($" AND CAU_MOV IN ('{(int)enum_Agenda_Causali.TRATTAMENTO}', '{(int)enum_Agenda_Causali.RILIEVO_CAMPO}', '{(int)enum_Agenda_Causali.RILIEVO_RACCOLTA}', '{(int)enum_Agenda_Causali.LAVORAZIONE}') ");
                stbQuery.AppendLine(stbFiltri.FiltroMovimenti.ToString());

                //Indice per ottimizzazione query su tabella temp
                stbQuery.AppendLine($" CREATE CLUSTERED INDEX IX_Mov_Destinazioni_TT_{Guid.NewGuid().ToString().Replace("-", "_")}");
                stbQuery.AppendLine(" ON #Mov_Destinazioni_TT (keyPiva, keySaCod, keyAppezza, keyIdReg, keyIdAgenda, keyIdMov);");
            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#Movimenti_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #Movimenti_TT ");
            }

            return stbQuery.ToString();
        }

        #endregion

        #endregion

        #region "Get TT Methods"
        private string GetUtentiVisibilitaAppoggio_TT(int TipoEntita, StringBuilder stbJoin, ref Dictionary<string, object> parSql, AgronicaCoreParametriServer objParametriServer)
        {
            var stbTT = new StringBuilder();

            string nameTT = "";
            string paramID = "";

            switch (TipoEntita)
            {
                case (int)Enum_TipoEntita.Impresa:
                    nameTT = "#UtentiVisibilitaAppoggioImprese_TT";
                    paramID = "@UsernameVisibilitaAppoggio_Impresa";
                    break;
                case (int)Enum_TipoEntita.Centro:
                    nameTT = "#UtentiVisibilitaAppoggioCentriAziendali_TT";
                    paramID = "@UsernameVisibilitaAppoggio_Centro";
                    break;
            }

            stbTT.AppendLine(" SELECT ");

            switch (TipoEntita)
            {
                case (int)Enum_TipoEntita.Impresa:
                    stbTT.AppendLine("        Piva AS [keyPiva] ");
                    stbJoin.AppendLine("   JOIN " + nameTT + " ON " + nameTT + ".[keyPiva] = Imprese.Piva ");
                    break;
                case (int)Enum_TipoEntita.Centro:
                    stbTT.AppendLine("         Piva AS [keyPiva] ");
                    stbTT.AppendLine("       , Sa_Cod AS [keySaCod] ");
                    stbJoin.AppendLine("   JOIN " + nameTT + " ON ");
                    stbJoin.AppendLine("        " + nameTT + ".[keyPiva] = Centri_Aziendali.Piva ");
                    stbJoin.AppendLine("   AND " + nameTT + ".[keySaCod] = Centri_Aziendali.sa_cod ");
                    break;
            }

            stbTT.AppendLine("   INTO " + nameTT + " ");
            stbTT.AppendLine("   FROM Utenti_Visibilita_Appoggio ");
            stbTT.AppendLine("   WHERE Entita_Cod = " + TipoEntita.ToString() + " ");
            stbTT.AppendLine("   AND Username = " + paramID + " ");

            parSql.Add(paramID, objParametriServer.UtenteUsername);

            if (Debugger.IsAttached)
            {
                stbTT.AppendLine(GetTimestamp("#" + nameTT).ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #" + nameTT + " ");
            }

            return stbTT.ToString();
        }

        private void GetCodici(string TableName, List<CodiceAnagrafeBase> CodiciAnagrafe, ref StringBuilder stbSqlTTCodici, ref StringBuilder stbSqlSelectCodici, ref StringBuilder stbSqlJoinCodici, ref List<KendoColumn> kendoColumns, AgronicaCoreParametriServer objParametriServer)
        {
            var prefix = "";
            var TabellaCodici = (TableName.ToString() != Enum_Tabelle.Aziende && TableName.ToString() != Enum_Tabelle.CentriAziendali ? _modalitaBudget : "") + TableName + "_Codici ";
            switch (TableName.ToString())
            {
                case Enum_Tabelle.Aziende:
                    prefix = Enum_PrefixCodici.Aziende;
                    break;
                case Enum_Tabelle.CentriAziendali:
                    prefix = Enum_PrefixCodici.CentriAziendali;
                    break;
                case Enum_Tabelle.Campi:
                    prefix = Enum_PrefixCodici.Campi;
                    break;
                case Enum_Tabelle.Appezzamenti:
                    prefix = Enum_PrefixCodici.Appezzamenti;
                    break;
                case Enum_Tabelle.Impianti:
                    prefix = Enum_PrefixCodici.Impianti;
                    break;
                case Enum_Tabelle.Esercizi:
                    //Per la tabella Imprese_Progetti i codici sono salvati sempre in Reg_Impianti_Codici
                    //con distinzione sulla colonna Progetto_Cod
                    TabellaCodici = _modalitaBudget + Enum_Tabelle.Impianti + "_Codici ";
                    prefix = Enum_PrefixCodici.Esercizi;
                    break;
                case Enum_Tabelle.Fabbricati:
                    prefix = Enum_PrefixCodici.Fabbricati;
                    break;
            }

            try
            {
                //Per mettere le colonne dei codici in fondo 
                int columnOrder = columnOrderIndex + 100;

                foreach (CodiceAnagrafeBase codiceAnagrafe in CodiciAnagrafe)
                {
                    int id_cod = codiceAnagrafe.codice;
                    string codice = codiceAnagrafe.codice.ToString();
                    string descrizione = codiceAnagrafe.descrizione;

                    string dataType = "string";

                    int TipoControllo_Cod = 1;
                    switch (TipoControllo_Cod)
                    {
                        case (int)Enum_TipoControllo.NUMERO_INTERO:
                        case (int)Enum_TipoControllo.NUMERO_DECIMALE:
                            dataType = "number";
                            break;
                        case (int)Enum_TipoControllo.CALENDARIO:
                            dataType = "date";
                            break;
                    }

                    string joinName = prefix + codice;
                    string joinON = "";
                    string ambiguousCodeTag = "";
                    bool isTT = false;
                    bool isBooleanField = false;

                    switch (TableName.ToString())
                    {
                        case Enum_Tabelle.Aziende:
                            joinON = "     Imprese.Piva = " + joinName + ".Piva ";

                            GetCodici_Azienda(id_cod, joinName, ref isTT, ref isBooleanField, ref ambiguousCodeTag, ref stbSqlTTCodici);
                            break;
                        case Enum_Tabelle.CentriAziendali:
                            joinON = "     Centri_Aziendali.Piva = " + joinName + ".Piva " +
                                     " AND Centri_Aziendali.Sa_Cod = " + joinName + ".Sa_Cod ";
                            GetCodici_CentroAziendale(id_cod, joinName, ref isTT, ref isBooleanField, ref ambiguousCodeTag, ref stbSqlTTCodici);
                            break;
                        case Enum_Tabelle.Campi:
                            joinON = "     Campi.Piva = " + joinName + ".Piva " +
                                     " AND Campi.Sa_Cod = " + joinName + ".Sa_Cod " +
                                     " AND Campi.Campo_Cod = " + joinName + ".Campo_Cod ";

                            if (_modalitaBudget != "")
                                joinON += " AND Campi.Id_Budget = " + joinName + ".Id_Budget ";

                            GetCodici_Campo(id_cod, joinName, ref isTT, ref isBooleanField, ref ambiguousCodeTag, ref stbSqlTTCodici);
                            break;
                        case Enum_Tabelle.Appezzamenti:
                            joinON = "     Appezzamento.Piva = " + joinName + ".Piva " +
                                     " AND Appezzamento.Sa_Cod = " + joinName + ".Sa_Cod " +
                                     " AND Appezzamento.Appezza = " + joinName + ".Appezza ";

                            if (_modalitaBudget != "")
                                joinON += " AND Appezzamento.Id_Budget = " + joinName + ".Id_Budget ";

                            GetCodici_Appezzamento(id_cod, joinName, ref isTT, ref isBooleanField, ref ambiguousCodeTag, ref stbSqlTTCodici);
                            break;
                        case Enum_Tabelle.Impianti:
                            joinON = "     Reg_Impianti.Piva = " + joinName + ".Piva " +
                                     " AND Reg_Impianti.Sa_Cod = " + joinName + ".Sa_Cod " +
                                     " AND Reg_Impianti.Appezza = " + joinName + ".Appezza " +
                                     " AND Reg_Impianti.Id_Reg = " + joinName + ".Id_Reg ";

                            if (_modalitaBudget != "")
                                joinON += " AND Reg_Impianti.Id_Budget = " + joinName + ".Id_Budget ";

                            GetCodici_ImpiantoEsercizio(id_cod, joinName, ref isTT, ref isBooleanField, ref ambiguousCodeTag, ref stbSqlTTCodici);
                            break;
                        case Enum_Tabelle.Esercizi:
                            joinON = "     Imprese_Progetti.Piva = " + joinName + ".Piva " +
                                     " AND Imprese_Progetti.Sa_Cod = " + joinName + ".Sa_Cod " +
                                     " AND Imprese_Progetti.Appezza = " + joinName + ".Appezza " +
                                     " AND Imprese_Progetti.Id_Reg = " + joinName + ".Id_Reg " +
                                     " AND Imprese_Progetti.Progetto_Cod = " + joinName + ".Progetto_Cod ";

                            if (_modalitaBudget != "")
                                joinON += " AND Imprese_Progetti.Id_Budget = " + joinName + ".Id_Budget ";

                            GetCodici_ImpiantoEsercizio(id_cod, joinName, ref isTT, ref isBooleanField, ref ambiguousCodeTag, ref stbSqlTTCodici);
                            break;
                        case Enum_Tabelle.Fabbricati:
                            joinON = "     Fabbricati.Piva = " + joinName + ".Piva " +
                                     " AND Fabbricati.Sa_Cod = " + joinName + ".Sa_Cod " +
                                     " AND Fabbricati.Fabbricato_Cod = " + joinName + ".Fabbricato_Cod ";
                            GetCodici_Fabbricato(id_cod, joinName, ref isTT, ref isBooleanField, ref ambiguousCodeTag, ref stbSqlTTCodici);
                            break;
                    }

                    stbSqlJoinCodici.AppendLine("   LEFT JOIN " + (isTT ? "#" + joinName : TabellaCodici) + " " + joinName + " ");
                    stbSqlJoinCodici.AppendLine("       ON " + joinON);
                    if (!isTT)
                        stbSqlJoinCodici.AppendLine("   AND " + joinName + ".Id_Cod = " + codice + " ");

                    stbSqlSelectCodici.AppendLine("        , ISNULL(" + joinName + ".val_cod, '') AS [" + joinName + "] ");


                    kendoColumns.Add(new KendoColumn { Field = joinName, Hidden = false, Title = descrizione + ambiguousCodeTag, DataType = dataType, TranslateTitle = false, ColumnOrder = SetColumnOrder(ref columnOrder) });

                    if (id_cod == (int)Enum_CodiciAnagrafe.Tecnico && _configuratore.CaricaInfoTecnicoImpresa && TableName.ToString() == Enum_Tabelle.Aziende)
                    {
                        //Dati relativi al Tecnico...
                        kendoColumns.Add(new KendoColumn { Field = "Tecnico_Numero", Hidden = true, Title = "TecnicoNumero", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrder) });
                        kendoColumns.Add(new KendoColumn { Field = "Tecnico_Email", Hidden = true, Title = "TecnicoEmail", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrder) });
                        kendoColumns.Add(new KendoColumn { Field = "Tecnico_Sesso", Hidden = true, Title = "TecnicoSesso", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrder) });
                        kendoColumns.Add(new KendoColumn { Field = "Tecnico_DataNascita", Hidden = true, Title = "TecnicoDataNascita", DataType = "date", ColumnOrder = SetColumnOrder(ref columnOrder) });
                    }

                    if (id_cod == (int)Enum_CodiciAnagrafe.Organismo_Referente)
                    {
                        //Quando carici l'Organismo Referente dell'Esercizio aggiungo anche i dati del Libro Soci
                        stbSqlSelectCodici.AppendLine("        , ISNULL(" + joinName + ".LibroSoci_Codice, '') AS LibroSoci_Codice ");
                        stbSqlSelectCodici.AppendLine("        , ISNULL(" + joinName + ".LibroSoci_DataIscrizione, CAST('2100-12-31' AS Date)) AS LibroSoci_DataIscrizione ");

                        kendoColumns.Add(new KendoColumn { Field = "LibroSoci_Codice", Title = "NumeroLibroSoci", DataType = "string", ColumnOrder = SetColumnOrder(ref columnOrder) });
                        kendoColumns.Add(new KendoColumn { Field = "LibroSoci_DataIscrizione", Title = "DataIscrizioneLibroSoci", DataType = "date", ColumnOrder = SetColumnOrder(ref columnOrder) });
                    }

                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
            }
        }

        private void GetCodici_Azienda(int Id_Cod, string joinName, ref bool isTT, ref bool isBooleanField, ref string ambiguousCodeTag, ref StringBuilder stbSqlTTCodici)
        {
            string joinTabellaBase = BuildJoinScalare("tabellaCodice", (int)Enum_Entita_FiltroRicerca.Azienda).ToString();

            isTT = true;
            switch (Id_Cod)
            {
                case (int)Enum_CodiciAnagrafe.Codice_Certificazione:
                    ambiguousCodeTag = " Azienda";
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("             tabellaCodice.Piva ");
                    stbSqlTTCodici.AppendLine("           , STRING_AGG(CertificazioniAziendali.CA_Des, ', ') AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM Imprese_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBase);
                    if (_SQLCompatibility)
                    {
                        stbSqlTTCodici.AppendLine("   CROSS APPLY STRING_SPLIT(val_cod, ', ') ");
                        stbSqlTTCodici.AppendLine("   JOIN CertificazioniAziendali ON value = CertificazioniAziendali.CA_Cod ");
                    }
                    else
                    {
                        stbSqlTTCodici.AppendLine("   CROSS APPLY dbo.fSplit(val_cod, ', ') ");
                        stbSqlTTCodici.AppendLine("   JOIN CertificazioniAziendali ON strName = CertificazioniAziendali.CA_Cod ");
                    }
                    stbSqlTTCodici.AppendLine("   WHERE Id_Cod = " + (int)Enum_CodiciAnagrafe.Codice_Certificazione + " ");
                    stbSqlTTCodici.AppendLine("   GROUP BY tabellaCodice.Piva ");
                    break;
                case (int)Enum_CodiciAnagrafe.Tecnico:
                    ambiguousCodeTag = " Azienda";
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("             tabellaCodice.Piva ");
                    stbSqlTTCodici.AppendLine("           , MAX(Cognome + ' ' + Nome + Rag_Soc) AS Val_Cod ");
                    stbSqlTTCodici.AppendLine("           , Contatti.Cod_Contatto AS [keyTecnico] ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM Imprese_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine("   JOIN Risorse_Umane ON ");
                    stbSqlTTCodici.AppendLine("       Risorse_Umane.Cod_Contatto = Val_Cod ");
                    stbSqlTTCodici.AppendLine("   AND Cod_Rapporto = " + (int)Enum_Rapporti_Contabili_Standard.Tecnico + " ");
                    stbSqlTTCodici.AppendLine("   AND (Risorse_Umane.Piva = tabellaCodice.PIVA OR Risorse_Umane.Sa_Cod = -1) ");
                    stbSqlTTCodici.AppendLine("   JOIN Contatti ON ");
                    stbSqlTTCodici.AppendLine("       Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ");
                    stbSqlTTCodici.AppendLine("   AND Contatti.Piva = Risorse_Umane.PIVA ");
                    stbSqlTTCodici.AppendLine("   AND Contatti.Sa_Cod = Risorse_Umane.Sa_Cod ");
                    stbSqlTTCodici.AppendLine(joinTabellaBase);
                    stbSqlTTCodici.AppendLine("   WHERE Id_Cod = " + (int)Enum_CodiciAnagrafe.Tecnico + " ");
                    stbSqlTTCodici.AppendLine("   GROUP BY tabellaCodice.Piva, Contatti.Cod_Contatto ");
                    stbSqlTTCodici.AppendLine("");

                    _configuratore.CaricaInfoTecnicoImpresa = true;
                    stbSqlTTCodici.AppendLine(GetInfoTecnico_TT().ToString());

                    break;
                case (int)Enum_CodiciAnagrafe.Organismo_di_Controllo:
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("             tabellaCodice.Piva AS Piva ");
                    stbSqlTTCodici.AppendLine("           , Cognome + ' ' + Nome + Rag_Soc AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM Imprese_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine("   JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = Val_Cod ");
                    stbSqlTTCodici.AppendLine("   JOIN Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ");
                    stbSqlTTCodici.AppendLine(joinTabellaBase);
                    stbSqlTTCodici.AppendLine("   WHERE Id_Cod = " + (int)Enum_CodiciAnagrafe.Organismo_di_Controllo + " ");
                    break;
                default:
                    isTT = false;
                    break;
            }

            if (isTT && Debugger.IsAttached)
            {
                stbSqlTTCodici.AppendLine(GetTimestamp("#" + joinName).ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #" + joinName + " ");
            }
        }

        private void GetCodici_CentroAziendale(int Id_Cod, string joinName, ref bool isTT, ref bool isBooleanField, ref string ambiguousCodeTag, ref StringBuilder stbSqlTTCodici)
        {
            string joinTabellaBase = BuildJoinScalare("tabellaCodice", (int)Enum_Entita_FiltroRicerca.CentroAziendale).ToString();

            isTT = true;
            switch (Id_Cod)
            {
                case (int)Enum_CodiciAnagrafe.ORGANISMO_DI_CONTROLLO_BIO:
                    isTT = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("               tabellaCodice.Piva, tabellaCodice.sa_cod ");
                    stbSqlTTCodici.AppendLine("             , BIO_Dati_OrganismiControllo.Codice + ' - ' + BIO_Dati_OrganismiControllo.Organismo_Des + ' (' + BIO_Dati_OrganismiControllo.Organismo_Sigla + ')' AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM Centri_Aziendali_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBase);
                    stbSqlTTCodici.AppendLine("   JOIN BIO_Dati_OrganismiControllo ON BIO_Dati_OrganismiControllo.Organismo_Cod = tabellaCodice.val_cod ");
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.ORGANISMO_DI_CONTROLLO_BIO + " ");
                    break;
                case (int)Enum_CodiciAnagrafe.OTE:
                    isTT = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("               tabellaCodice.Piva, tabellaCodice.sa_cod ");
                    stbSqlTTCodici.AppendLine("             , STRING_AGG(OrientamentoTecnicoEconomico.OTE_Des, ', ') AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM Centri_Aziendali_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBase);
                    if (_SQLCompatibility)
                    {
                        stbSqlTTCodici.AppendLine("   CROSS APPLY STRING_SPLIT(val_cod, '|') ");
                        stbSqlTTCodici.AppendLine("   JOIN OrientamentoTecnicoEconomico ON value = OrientamentoTecnicoEconomico.OTE_cod ");
                    }
                    else
                    {
                        stbSqlTTCodici.AppendLine("   CROSS APPLY dbo.fSplit(val_cod, '|') ");
                        stbSqlTTCodici.AppendLine("   JOIN OrientamentoTecnicoEconomico ON strName COLLATE Latin1_General_CI_AS = OrientamentoTecnicoEconomico.OTE_cod COLLATE Latin1_General_CI_AS ");
                    }
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.OTE + " ");
                    stbSqlTTCodici.AppendLine("   GROUP BY tabellaCodice.Piva, tabellaCodice.sa_cod ");
                    break;
                case (int)Enum_CodiciAnagrafe.TipoAttivita:
                    isTT = true;
                    stbSqlTTCodici.AppendLine("   SELECT   tabellaCodice.PIVA, tabellaCodice.sa_cod, ");
                    stbSqlTTCodici.AppendLine("               CASE tabellaCodice.val_cod");
                    stbSqlTTCodici.AppendLine($"               WHEN 'PV' THEN '{_localizer["ProduzioneVegetale"]}'");
                    stbSqlTTCodici.AppendLine($"               WHEN 'PZ' THEN '{_localizer["ProduzioneZootecnica"]}'");
                    stbSqlTTCodici.AppendLine($"               WHEN 'PVZ' THEN '{_localizer["ProduzioneVegetaleEZootecnica"]}'");
                    stbSqlTTCodici.AppendLine($"               WHEN 'TPV' THEN '{_localizer["PreparazioneVegetale"]}'");
                    stbSqlTTCodici.AppendLine($"               WHEN 'TPZ' THEN '{_localizer["PreparazioneZootecnica"]}'");
                    stbSqlTTCodici.AppendLine($"               WHEN 'TPVZ' THEN '{_localizer["PreparazioneVegetaleEZootecnica"]}'");
                    stbSqlTTCodici.AppendLine($"               WHEN 'I' THEN '{_localizer["Importazione"]}'");
                    stbSqlTTCodici.AppendLine($"               WHEN 'RS' THEN '{_localizer["RaccoltaSpontanea"]}'");
                    stbSqlTTCodici.AppendLine($"               WHEN 'P/TP' THEN '{_localizer["ProduzionePreparazione"]}'");
                    stbSqlTTCodici.AppendLine($"               WHEN 'TP/I' THEN '{_localizer["PreparazioneImportazione"]}'");
                    stbSqlTTCodici.AppendLine($"               WHEN '@' THEN '{_localizer["Altro"]}'");
                    stbSqlTTCodici.AppendLine("               ELSE tabellaCodice.val_cod");
                    stbSqlTTCodici.AppendLine("               END");
                    stbSqlTTCodici.AppendLine("               AS Val_Cod");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM Centri_Aziendali_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBase);
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.TipoAttivita + " ");
                    break;
                default:
                    isTT = false;
                    break;
            }

            if (isTT && Debugger.IsAttached)
            {
                stbSqlTTCodici.AppendLine(GetTimestamp("#" + joinName).ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #" + joinName + " ");
            }
        }
        private void GetCodici_Campo(int Id_Cod, string joinName, ref bool isTT, ref bool isBooleanField, ref string ambiguousCodeTag, ref StringBuilder stbSqlTTCodici)
        {
            string joinTabellaBase = BuildJoinScalare("tabellaCodice", (int)Enum_Entita_FiltroRicerca.Campo).ToString();

            isTT = true;
            switch (Id_Cod)
            {
                default:
                    isTT = false;
                    break;
            }

            if (isTT && Debugger.IsAttached)
            {
                stbSqlTTCodici.AppendLine(GetTimestamp("#" + joinName).ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #" + joinName + " ");
            }
        }

        private void GetCodici_Appezzamento(int Id_Cod, string joinName, ref bool isTT, ref bool isBooleanField, ref string ambiguousCodeTag, ref StringBuilder stbSqlTTCodici)
        {
            string joinTabellaBase = BuildJoinScalare("tabellaCodice", (int)Enum_Entita_FiltroRicerca.Appezzamento).ToString();
            isTT = false;
            switch (Id_Cod)
            {
                case (int)Enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1:
                case (int)Enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2:
                case (int)Enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3:
                case (int)Enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4:
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.sa_cod ");
                    stbSqlTTCodici.AppendLine("        , tabellaCodice.appezza, SpecieVegetali.Veg_Des AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM Appezzamento_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBase);
                    stbSqlTTCodici.AppendLine("   JOIN SpecieVegetali ON ");
                    stbSqlTTCodici.AppendLine("        SpecieVegetali.Veg_Cod = TRY_CAST(SUBSTRING(tabellaCodice.val_Cod, 1, CHARINDEX('|', tabellaCodice.val_Cod + '|') - 1) AS INT) ");
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + Id_Cod + " ");
                    break;
                case (int)Enum_CodiciAnagrafe.Terreno_Inutilizzato:
                case (int)Enum_CodiciAnagrafe.Terreno_Degradato:
                case (int)Enum_CodiciAnagrafe.Low_ILUC:
                    isTT = true;
                    isBooleanField = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.sa_cod, tabellaCodice.appezza ");
                    stbSqlTTCodici.AppendLine("        , CASE tabellaCodice.val_cod ");
                    stbSqlTTCodici.AppendLine($"            WHEN '1' THEN '{_localizer["Si"]}' ");
                    stbSqlTTCodici.AppendLine($"            ELSE '{_localizer["No"]}' ");
                    stbSqlTTCodici.AppendLine("          END AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM Appezzamento_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBase);
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + Id_Cod + " ");
                    break;
            }

            if (isTT && Debugger.IsAttached)
            {
                stbSqlTTCodici.AppendLine(GetTimestamp("#" + joinName).ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #" + joinName + " ");
            }
        }

        private void GetCodici_ImpiantoEsercizio(int Id_Cod, string joinName, ref bool isTT, ref bool isBooleanField, ref string ambiguousCodeTag, ref StringBuilder stbSqlTTCodici)
        {
            string joinTabellaBaseImpianto = BuildJoinScalare("tabellaCodice", (int)Enum_Entita_FiltroRicerca.Impianto).ToString();
            string joinTabellaBaseEsercizio = BuildJoinScalare("tabellaCodice", (int)Enum_Entita_FiltroRicerca.Esercizio).ToString();
            isTT = true;

            switch (Id_Cod)
            {
                case (int)Enum_CodiciAnagrafe.Codice_Certificazione:
                    ambiguousCodeTag = " Esercizio";
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("             tabellaCodice.Piva ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.sa_cod, tabellaCodice.appezza, tabellaCodice.id_reg, tabellaCodice.progetto_cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("           , STRING_AGG(CertificazioniAziendali.CA_Des, ', ') AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    if (_SQLCompatibility)
                    {
                        stbSqlTTCodici.AppendLine("   CROSS APPLY STRING_SPLIT(val_cod, '|') ");
                        stbSqlTTCodici.AppendLine("   JOIN CertificazioniAziendali ON value = CertificazioniAziendali.CA_Cod ");
                    }
                    else
                    {
                        stbSqlTTCodici.AppendLine("   CROSS APPLY dbo.fSplit(val_cod, '|') ");
                        stbSqlTTCodici.AppendLine("   JOIN CertificazioniAziendali ON strName = CertificazioniAziendali.CA_Cod ");
                    }

                    stbSqlTTCodici.AppendLine("   WHERE Id_Cod = " + (int)Enum_CodiciAnagrafe.Codice_Certificazione + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");

                    stbSqlTTCodici.AppendLine("   GROUP BY tabellaCodice.Piva ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.sa_cod, tabellaCodice.appezza, tabellaCodice.id_reg, tabellaCodice.progetto_cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");
                    break;
                case (int)Enum_CodiciAnagrafe.Magazzino_Conferimento:
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("             tabellaCodice.Piva ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.Sa_Cod ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.Appezza ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Reg ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("           , tabellaCodice.Progetto_Cod ");
                    stbSqlTTCodici.AppendLine("           , Fabbricato_Des AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    stbSqlTTCodici.AppendLine("   JOIN " + _modalitaBudget + "Fabbricati Fabbricati ON CAST(Fabbricati.Fabbricato_Cod AS VARCHAR(20)) + '|' + CAST(Fabbricati.SA_COD  AS VARCHAR(20)) + '|' + CAST(Fabbricati.piva AS VARCHAR(20)) = tabellaCodice.Val_Cod ");
                    stbSqlTTCodici.AppendLine("   WHERE Id_Cod = " + (int)Enum_CodiciAnagrafe.Magazzino_Conferimento + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;
                case (int)Enum_CodiciAnagrafe.Tecnico:
                    ambiguousCodeTag = " Esercizio";
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("             tabellaCodice.Piva ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.sa_cod, tabellaCodice.appezza, tabellaCodice.id_reg, tabellaCodice.progetto_cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("           , STRING_AGG(Cognome + ' ' + Nome + Rag_Soc, ', ') AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);

                    if (_SQLCompatibility)
                        stbSqlTTCodici.AppendLine("   CROSS APPLY (SELECT DISTINCT value FROM STRING_SPLIT(val_cod, '|')) split ");
                    else
                        stbSqlTTCodici.AppendLine("   CROSS APPLY (SELECT DISTINCT strName AS value FROM dbo.fSplit(val_cod, '|')) split ");

                    stbSqlTTCodici.AppendLine("   CROSS APPLY (SELECT TOP 1 * FROM Risorse_Umane ");
                    stbSqlTTCodici.AppendLine("                WHERE split.value = Risorse_Umane.Cod_Contatto ");
                    stbSqlTTCodici.AppendLine("                AND (Risorse_Umane.Piva = tabellaCodice.PIVA OR Risorse_Umane.Sa_Cod = -1) ");
                    stbSqlTTCodici.AppendLine("                AND Cod_Rapporto = -6  ");
                    stbSqlTTCodici.AppendLine("               ) Risorse_Umane ");

                    stbSqlTTCodici.AppendLine("   JOIN Contatti ON ");
                    stbSqlTTCodici.AppendLine("       Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ");
                    stbSqlTTCodici.AppendLine("   AND Contatti.piva = Risorse_Umane.piva ");
                    stbSqlTTCodici.AppendLine("   AND Contatti.Sa_Cod = Risorse_Umane.Sa_Cod ");
                    stbSqlTTCodici.AppendLine("   WHERE Id_Cod = " + (int)Enum_CodiciAnagrafe.Tecnico + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");

                    stbSqlTTCodici.AppendLine("   GROUP BY tabellaCodice.Piva ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.sa_cod, tabellaCodice.appezza, tabellaCodice.id_reg, tabellaCodice.progetto_cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");
                    break;
                case (int)Enum_CodiciAnagrafe.Impianto_Ibrido:
                    isBooleanField = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.sa_cod, tabellaCodice.appezza, tabellaCodice.Id_Reg ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("        , CASE tabellaCodice.val_cod ");
                    stbSqlTTCodici.AppendLine($"            WHEN '1' THEN '{_localizer["Si"]}' ");
                    stbSqlTTCodici.AppendLine($"            WHEN '0' THEN '{_localizer["No"]}' ");
                    stbSqlTTCodici.AppendLine("            ELSE '' ");
                    stbSqlTTCodici.AppendLine("          END AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseImpianto);
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.Impianto_Ibrido + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;
                case (int)Enum_CodiciAnagrafe.Distinta_Chiusa:
                    isBooleanField = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.sa_cod, tabellaCodice.appezza, tabellaCodice.id_reg, tabellaCodice.progetto_cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("        , CASE tabellaCodice.val_cod ");
                    stbSqlTTCodici.AppendLine($"            WHEN '1' THEN '{_localizer["Si"]}' ");
                    stbSqlTTCodici.AppendLine($"            WHEN '0' THEN '{_localizer["No"]}' ");
                    stbSqlTTCodici.AppendLine("            ELSE '' ");
                    stbSqlTTCodici.AppendLine("          END AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.Distinta_Chiusa + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;
                case (int)Enum_CodiciAnagrafe.Codice_Residuo:
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("           tabellaCodice.PIVA, tabellaCodice.sa_cod, tabellaCodice.appezza ");
                    stbSqlTTCodici.AppendLine("         , tabellaCodice.Id_Reg, tabellaCodice.Progetto_Cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("         , CAC_Codifica_InfoAggiuntive.InfoAgg_Des AS Val_Cod ");

                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    stbSqlTTCodici.AppendLine("   JOIN CAC_Codifica_InfoAggiuntive ON ");
                    stbSqlTTCodici.AppendLine("       CAC_Codifica_InfoAggiuntive.InfoAgg_Cod = tabellaCodice.val_cod ");
                    stbSqlTTCodici.AppendLine("   AND CAC_Codifica_InfoAggiuntive.Argomento_Cod = " + (int)Enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Residuo + " ");
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.Codice_Residuo + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;
                case (int)Enum_CodiciAnagrafe.Finalita_Concimazione_Impianto:
                    isTT = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.Sa_Cod, tabellaCodice.Appezza, tabellaCodice.Id_Reg, tabellaCodice.Progetto_Cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("        , GRFI_DES AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    stbSqlTTCodici.AppendLine("   LEFT JOIN GruppoFinalita_Rer ON tabellaCodice.val_cod = GruppoFinalita_Rer.GRFI_COD ");
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.Finalita_Concimazione_Impianto + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;
                case (int)Enum_CodiciAnagrafe.Capitolato_Privato:
                    isTT = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.Sa_Cod, tabellaCodice.Appezza, tabellaCodice.Id_Reg, tabellaCodice.Progetto_Cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("        , CAC.InfoAgg_Des AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    stbSqlTTCodici.AppendLine("   JOIN CAC_Codifica_InfoAggiuntive CAC ON CAC.InfoAgg_Cod = tabellaCodice.Val_Cod AND CAC.Argomento_Cod = " + (int)Enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.CapitolatoPrivato + " ");
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.Capitolato_Privato + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;
                case (int)Enum_CodiciAnagrafe.Codice_Certificazione_Prodotto:
                    isTT = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.Sa_Cod, tabellaCodice.Appezza, tabellaCodice.Id_Reg, tabellaCodice.Progetto_Cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("        , CAC.InfoAgg_Des AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    stbSqlTTCodici.AppendLine("   JOIN CAC_Codifica_InfoAggiuntive CAC ON CAC.InfoAgg_Cod = tabellaCodice.Val_Cod AND CAC.Argomento_Cod = " + (int)Enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Certificazione_Prodotto + " ");
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.Codice_Certificazione_Prodotto + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;
                case (int)Enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato:
                    isTT = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.Sa_Cod, tabellaCodice.Appezza, tabellaCodice.Id_Reg ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("        , CAC.InfoAgg_Des AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseImpianto);
                    stbSqlTTCodici.AppendLine("   JOIN CAC_Codifica_InfoAggiuntive CAC ON CAC.InfoAgg_Cod = tabellaCodice.Val_Cod AND CAC.Argomento_Cod = " + (int)Enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.DettaglioSpeciePersonalizzato + " ");
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;
                case (int)Enum_CodiciAnagrafe.Specifica:
                    isTT = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.Sa_Cod, tabellaCodice.Appezza, tabellaCodice.Id_Reg, tabellaCodice.Progetto_Cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("        , CAC.InfoAgg_Des AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    stbSqlTTCodici.AppendLine("   JOIN CAC_Codifica_InfoAggiuntive CAC ON CAC.InfoAgg_Cod = tabellaCodice.Val_Cod AND CAC.Argomento_Cod = " + (int)Enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Specifica + " ");
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.Specifica + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;
                case (int)Enum_CodiciAnagrafe.Lavorazione:
                    isTT = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.Sa_Cod, tabellaCodice.Appezza, tabellaCodice.Id_Reg, tabellaCodice.Progetto_Cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("        , CAC.InfoAgg_Des AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    stbSqlTTCodici.AppendLine("   JOIN CAC_Codifica_InfoAggiuntive CAC ON CAC.InfoAgg_Cod = tabellaCodice.Val_Cod AND CAC.Argomento_Cod = " + (int)Enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Lavorazione + " ");
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.Lavorazione + " ");
                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;
                case (int)Enum_CodiciAnagrafe.Zespri_Fasi_Fase:
                    isTT = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.Sa_Cod, tabellaCodice.Appezza, tabellaCodice.Id_Reg, tabellaCodice.Progetto_Cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("        , OT.Descrizione AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    stbSqlTTCodici.AppendLine("   JOIN OTabelle_Parametri OT ON OT.Tabella_Cod = tabellaCodice.Id_Cod AND OT.Tabella_Par_Cod = tabellaCodice.Val_Cod ");
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.Zespri_Fasi_Fase + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;
                case (int)Enum_CodiciAnagrafe.Organismo_Referente:
                    isTT = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.Sa_Cod, tabellaCodice.Appezza, tabellaCodice.Id_Reg, tabellaCodice.Progetto_Cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("        , Imprese.Rag_Soc AS Val_Cod ");
                    stbSqlTTCodici.AppendLine("        , GerarchiaImprese.LibroSoci_Codice ");
                    stbSqlTTCodici.AppendLine("        , GerarchiaImprese.LibroSoci_DataIscrizione ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    stbSqlTTCodici.AppendLine("   JOIN Imprese ON Imprese.Piva = tabellaCodice.Val_Cod ");
                    stbSqlTTCodici.AppendLine("   LEFT JOIN GerarchiaImprese ON ");
                    stbSqlTTCodici.AppendLine("       GerarchiaImprese.Figlio = tabellaCodice.Piva ");
                    stbSqlTTCodici.AppendLine("   AND GerarchiaImprese.Padre = tabellaCodice.Val_Cod ");

                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.Organismo_Referente + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;

                case (int)Enum_CodiciAnagrafe.Modalita_Liquidazione:
                    isTT = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.Sa_Cod, tabellaCodice.Appezza, tabellaCodice.Id_Reg, tabellaCodice.Progetto_Cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("        , CAC.InfoAgg_Des AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    stbSqlTTCodici.AppendLine("   JOIN CAC_Codifica_InfoAggiuntive CAC ON CAC.InfoAgg_Cod = tabellaCodice.Val_Cod AND CAC.Argomento_Cod = " + (int)Enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.ModalitaLiquidazione + " ");
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.Modalita_Liquidazione + " ");
                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;

                case (int)Enum_CodiciAnagrafe.Origine_Prodotto:
                    isTT = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.Sa_Cod, tabellaCodice.Appezza, tabellaCodice.Id_Reg, tabellaCodice.Progetto_Cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("        , CAC.InfoAgg_Des AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    stbSqlTTCodici.AppendLine("   JOIN CAC_Codifica_InfoAggiuntive CAC ON CAC.InfoAgg_Cod = tabellaCodice.Val_Cod AND CAC.Argomento_Cod = " + (int)Enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.OrigineProdotto + " ");
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.Origine_Prodotto + " ");
                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;

                case (int)Enum_CodiciAnagrafe.CodiceZona:
                    isTT = true;
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.Sa_Cod, tabellaCodice.Appezza, tabellaCodice.Id_Reg, tabellaCodice.Progetto_Cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("        , Materie_Prime.Mat_Des AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    stbSqlTTCodici.AppendLine("   JOIN Materie_Prime ON tabellaCodice.Val_Cod = Materie_Prime.Mat_Cod ");
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.CodiceZona + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");
                    break;
                case (int)Enum_CodiciAnagrafe.Contributi:
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("             tabellaCodice.Piva ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.sa_cod, tabellaCodice.appezza, tabellaCodice.id_reg, tabellaCodice.progetto_cod ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("           , tabellaCodice.Id_Budget ");

                    stbSqlTTCodici.AppendLine("           , STRING_AGG(ContributiColtivazioni.Contributo_Des, ', ') AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Reg_Impianti_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBaseEsercizio);
                    if (_SQLCompatibility)
                    {
                        stbSqlTTCodici.AppendLine("   CROSS APPLY STRING_SPLIT(val_cod, '|') ");
                        stbSqlTTCodici.AppendLine("   JOIN ContributiColtivazioni ON value = ContributiColtivazioni.Contributo_Cod ");
                    }
                    else
                    {
                        stbSqlTTCodici.AppendLine("   CROSS APPLY dbo.fSplit(val_cod, '|') ");
                        stbSqlTTCodici.AppendLine("   JOIN ContributiColtivazioni ON strName = ContributiColtivazioni.Contributo_Cod ");
                    }

                    stbSqlTTCodici.AppendLine("   WHERE Id_Cod = " + (int)Enum_CodiciAnagrafe.Contributi + " ");

                    if (_modalitaBudget != "")
                        stbSqlTTCodici.AppendLine("   AND tabellaCodice.Id_Budget = @IdBudget ");

                    stbSqlTTCodici.AppendLine("   GROUP BY tabellaCodice.Piva ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.sa_cod, tabellaCodice.appezza, tabellaCodice.id_reg, tabellaCodice.progetto_cod ");
                    break;

                default:
                    isTT = false;
                    break;
            }

            if (isTT && Debugger.IsAttached)
            {
                stbSqlTTCodici.AppendLine(GetTimestamp("#" + joinName).ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #" + joinName + " ");
            }
        }

        private void GetCodici_Fabbricato(int Id_Cod, string joinName, ref bool isTT, ref bool isBooleanField, ref string ambiguousCodeTag, ref StringBuilder stbSqlTTCodici)
        {
            string joinTabellaBase = BuildJoinScalare("tabellaCodice", (int)Enum_Entita_FiltroRicerca.Fabbricato).ToString();

            isTT = true;
            switch (Id_Cod)
            {
                case (int)Enum_CodiciAnagrafe.Fabbricato_Forno_Combustibile:
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("             tabellaCodice.Piva AS Piva ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.Sa_Cod AS Sa_Cod ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.Fabbricato_Cod AS Fabbricato_Cod ");
                    stbSqlTTCodici.AppendLine($"           , CASE WHEN tabellaCodice.Val_Cod IN ('Metano ','METANO') THEN '{_localizer["Metano"]}' ");
                    stbSqlTTCodici.AppendLine($"                   WHEN tabellaCodice.Val_Cod IN ('GPL ','gpl') THEN '{_localizer["GPL"]}' ");
                    stbSqlTTCodici.AppendLine($"                  WHEN tabellaCodice.Val_Cod IN ('Gasolio ','GASOLIO') THEN '{_localizer["Gasolio"]}' ");
                    stbSqlTTCodici.AppendLine($"                  WHEN tabellaCodice.Val_Cod IN ('Cippato ','cippato') THEN '{_localizer["Cippato"]}' ");
                    stbSqlTTCodici.AppendLine("             ELSE tabellaCodice.Val_Cod END AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM Fabbricati_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBase);
                    stbSqlTTCodici.AppendLine("   WHERE Id_Cod = " + (int)Enum_CodiciAnagrafe.Fabbricato_Forno_Combustibile + " ");
                    break;
                case (int)Enum_CodiciAnagrafe.Fabbricato_Forno_Fiamma:
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("             tabellaCodice.Piva AS Piva ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.Sa_Cod AS Sa_Cod ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.Fabbricato_Cod AS Fabbricato_Cod ");
                    stbSqlTTCodici.AppendLine($"           , CASE WHEN tabellaCodice.Val_Cod IN ('indir','INDIRE','Indiretta','Indiretto') THEN '{_localizer["Indiretta"]}' ");
                    stbSqlTTCodici.AppendLine("            ELSE tabellaCodice.Val_Cod END AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM Fabbricati_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBase);
                    stbSqlTTCodici.AppendLine("   WHERE Id_Cod = " + (int)Enum_CodiciAnagrafe.Fabbricato_Forno_Fiamma + " ");
                    break;
                case (int)Enum_CodiciAnagrafe.Fabbricato_Forno_Cantiere:
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("             tabellaCodice.Piva AS Piva ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.Sa_Cod AS Sa_Cod ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.Fabbricato_Cod AS Fabbricato_Cod ");
                    stbSqlTTCodici.AppendLine($"           , CASE WHEN tabellaCodice.Val_Cod IN ('cassoni ','Cassoni','CASSONI') THEN '{_localizer["Cassoni"]}' ");
                    stbSqlTTCodici.AppendLine("            ELSE tabellaCodice.Val_Cod END AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM Fabbricati_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBase);
                    stbSqlTTCodici.AppendLine("   WHERE Id_Cod = " + (int)Enum_CodiciAnagrafe.Fabbricato_Forno_Cantiere + " ");
                    break;
                case (int)Enum_CodiciAnagrafe.Fabbricato_Forno_Umidificazione:
                    stbSqlTTCodici.AppendLine(" " + joinName + " AS ( ");
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("             tabellaCodice.Piva AS Piva ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.Sa_Cod AS Sa_Cod ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.Fabbricato_Cod AS Fabbricato_Cod ");
                    stbSqlTTCodici.AppendLine($"           , CASE WHEN tabellaCodice.Val_Cod IN ('Acqua ','acqua','ACQUA') THEN '{_localizer["Acqua"]}' ");
                    stbSqlTTCodici.AppendLine("            ELSE tabellaCodice.Val_Cod END AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM " + _modalitaBudget + "Fabbricati_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBase);
                    stbSqlTTCodici.AppendLine("   WHERE Id_Cod = " + (int)Enum_CodiciAnagrafe.Fabbricato_Forno_Umidificazione + " ");
                    break;
                case (int)Enum_CodiciAnagrafe.Fabbricato_Forno_Tipo:
                    stbSqlTTCodici.AppendLine("   SELECT ");
                    stbSqlTTCodici.AppendLine("             tabellaCodice.Piva AS Piva ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.Sa_Cod AS Sa_Cod ");
                    stbSqlTTCodici.AppendLine("           , tabellaCodice.Fabbricato_Cod AS Fabbricato_Cod ");
                    stbSqlTTCodici.AppendLine($"           , CASE WHEN tabellaCodice.Val_Cod IN ('DE CLOE','DE CLOET') THEN '{_localizer["DeCloet"]}' ");
                    stbSqlTTCodici.AppendLine($"                  WHEN tabellaCodice.Val_Cod IN ('EuropeTob','Europetop') THEN '{_localizer["EuropeTob"]}' ");
                    stbSqlTTCodici.AppendLine($"                 WHEN tabellaCodice.Val_Cod IN ('TAB','Tab') THEN '{_localizer["Tab"]}' ");
                    stbSqlTTCodici.AppendLine($"                 WHEN tabellaCodice.Val_Cod IN ('MENCAGLI','Mencagli') THEN '{_localizer["Mencagli"]}' ");
                    stbSqlTTCodici.AppendLine($"                 WHEN tabellaCodice.Val_Cod IN ('GOME','Gome') THEN '{_localizer["Gome"]}' ");
                    stbSqlTTCodici.AppendLine($"                 WHEN tabellaCodice.Val_Cod IN ('GODIOLI','Godioli') THEN '{_localizer["Godioli"]}' ");
                    stbSqlTTCodici.AppendLine($"                 WHEN tabellaCodice.Val_Cod IN ('CALDERINI','Calderini') THEN '{_localizer["Calderini"]}' ");
                    stbSqlTTCodici.AppendLine("            ELSE tabellaCodice.Val_Cod END AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine("   FROM Fabbricati_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBase);
                    stbSqlTTCodici.AppendLine("   WHERE Id_Cod = " + (int)Enum_CodiciAnagrafe.Fabbricato_Forno_Tipo + " ");
                    break;
                case (int)Enum_CodiciAnagrafe.Visibile_da_App:
                    isTT = true;
                    isBooleanField = true;
                    stbSqlTTCodici.AppendLine(" SELECT ");
                    stbSqlTTCodici.AppendLine("          tabellaCodice.Piva, tabellaCodice.sa_cod, tabellaCodice.Fabbricato_Cod ");
                    stbSqlTTCodici.AppendLine("        , CASE tabellaCodice.val_cod ");
                    stbSqlTTCodici.AppendLine($"            WHEN '1' THEN '{_localizer["Si"]}' ");
                    stbSqlTTCodici.AppendLine($"            WHEN '0' THEN '{_localizer["No"]}' ");
                    stbSqlTTCodici.AppendLine("            ELSE '' ");
                    stbSqlTTCodici.AppendLine("          END AS Val_Cod ");
                    stbSqlTTCodici.AppendLine($" INTO #{joinName} ");
                    stbSqlTTCodici.AppendLine(" FROM Fabbricati_Codici tabellaCodice ");
                    stbSqlTTCodici.AppendLine(joinTabellaBase);
                    stbSqlTTCodici.AppendLine("   WHERE tabellaCodice.Id_Cod = " + (int)Enum_CodiciAnagrafe.Visibile_da_App + " ");
                    break;
                default:
                    isTT = false;
                    break;
            }

            if (isTT && Debugger.IsAttached)
            {
                stbSqlTTCodici.AppendLine(GetTimestamp("#" + joinName).ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #" + joinName + " ");
            }
        }

        private async Task GetBaseColumnsAsync(StringBuilder stbQueryBase, StringBuilder stbJoinUtenti, string TableName, List<KendoColumn> kendoColumns, bool skipColonneStdGias, AgronicaCoreParametriServer objParametriServer)
        {

            var stbQuery = new StringBuilder();

            //FilterOut_Current --> based on the table we are selectng, it will contain a different set of columns
            string[] FilterOut_Current = Array.Empty<string>();
            switch (TableName.Replace("Budget_", "")) //Tolgo il tag "Budget_"
            {
                case Enum_Tabelle.Aziende:
                    FilterOut_Current = Enum_DTColumns.FilterOut_Aziende;
                    break;
                case Enum_Tabelle.CentriAziendali:
                    FilterOut_Current = Enum_DTColumns.FilterOut_CentriAziendali;
                    break;
                case Enum_Tabelle.Campi:
                    FilterOut_Current = Enum_DTColumns.FilterOut_Campi;
                    break;
                case Enum_Tabelle.Appezzamenti:
                    FilterOut_Current = Enum_DTColumns.FilterOut_Appezzamenti;
                    break;
                case Enum_Tabelle.Impianti:
                    FilterOut_Current = Enum_DTColumns.FilterOut_Impianti;
                    break;
                case Enum_Tabelle.Esercizi:
                    FilterOut_Current = Enum_DTColumns.FilterOut_Esercizi;
                    break;
                case Enum_Tabelle.Fabbricati:
                    FilterOut_Current = Enum_DTColumns.FilterOut_Fabbricati;
                    break;
            }

            FilterOut_Current = FilterOut_Current.Select(x => x.ToLowerInvariant()).ToArray();

            var parSql = new Dictionary<string, object>();

            //we get the whole set of columns
            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     sys.columns.name AS colonna ");
            stbQuery.AppendLine("   , CASE WHEN sys.types.name IN ('nvarchar', 'VARCHAR', 'char', 'text', 'nchar', 'ntext') THEN 'string' ");
            stbQuery.AppendLine("          WHEN sys.types.name IN ('int','float', 'smallint', 'decimal', 'real', 'tinyint', 'bigint', 'numeric') THEN 'number' ");
            stbQuery.AppendLine("          WHEN sys.types.name IN ('date', 'datetime') THEN 'date' ");
            stbQuery.AppendLine("          WHEN sys.types.name IN ('bit') THEN 'boolean' ");
            stbQuery.AppendLine("          ELSE 'string' ");
            stbQuery.AppendLine("     END AS dataType ");
            stbQuery.AppendLine(" FROM sys.tables ");
            stbQuery.AppendLine(" JOIN sys.columns ON sys.tables.object_id = sys.columns.object_id ");
            stbQuery.AppendLine(" JOIN sys.types ON sys.types.user_type_id = sys.columns.user_type_id ");
            stbQuery.AppendLine(" WHERE sys.tables.name = @TableName");

            parSql.Add("@TableName", TableName);

            DataTable result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), parSql);

            if (result != null && result.Rows.Count > 0)
            {

                foreach (DataRow row in result.Rows)
                {
                    string column = (row["colonna"].ToString())!;
                    string dataType = (row["dataType"].ToString())!;

                    var setStdColumn = false;

                    //for each column we check if it's amongst those we don't want in the select command
                    if (!Enum_DTColumns.FilterOut.Contains(column.ToLowerInvariant())
                        && !Enum_DTColumns.FilterOut_Keys.Contains(column.ToLowerInvariant())
                        && !FilterOut_Current.Contains(column.ToLowerInvariant())
                        && !(Enum_DTColumns.Skip_or_ApplyFormatDate.Contains(column.ToLowerInvariant()) && skipColonneStdGias)
                        )
                    {
                        string strSelect = "";

                        //Tutte le colonne aggiunte dinamicamente sono nascoste
                        bool hiddenKendo = true; // Enum_DTColumns.Skip_or_ApplyFormatDate.Contains(column.ToLowerInvariant());

                        strSelect += "       , ";


                        if (Enum_DTColumns.Skip_or_ApplyFormatDate.Contains(column.ToLowerInvariant()))
                        {
                            if (column.ToLowerInvariant().Contains("data"))
                            {
                                string columnOriginal = column;
                                if (column.ToLowerInvariant().Contains("creazione") || column.ToLowerInvariant().Contains("modifica"))
                                    //Rimuovo l'underscore per matchare la traduzione sul FE
                                    column = column.Replace("_", "");

                                //Cambio nome colonna
                                strSelect += TableName.Replace("Budget_", "") + "." + columnOriginal + " AS " + column + " ";
                            }
                            else
                            {
                                if (column.ToLowerInvariant().Contains("fine"))
                                    //Per i tipi data faccio un cast
                                    strSelect += " ISNULL(" + TableName.Replace("Budget_", "") + "." + column + ", CAST('2100-12-31' AS Date)) AS " + column + " ";
                                else
                                    strSelect += " ISNULL(" + TableName.Replace("Budget_", "") + "." + column + ", CAST('1900-01-01' AS Date)) AS " + column + " ";
                            }

                            //li metto in fondo alle colonne
                            setStdColumn = true;
                        }
                        else
                        {
                            if (dataType == "boolean")
                            {
                                strSelect += $" CASE WHEN {TableName}.{column} = 1 THEN '{_localizer["Si"]}'";
                                strSelect += $"      WHEN {TableName}.{column} = 0 THEN '{_localizer["No"]}'";
                                strSelect += $"      ELSE '' END AS {column} ";
                                dataType = "string";
                            }
                            //else if (Enum_DTColumns.Estrai_Utente.Contains(column) || column.ToLowerInvariant().Contains("username")                         
                            else if (column.ToLowerInvariant().Contains("username"))
                            {
                                string aliasUtentiDettagli = "UtentiDettagli_" + column;
                                strSelect += $"ISNULL({aliasUtentiDettagli}.Username, '') AS {column} ";
                                stbJoinUtenti.AppendLine($" LEFT JOIN #UtentiDettagli_TT {aliasUtentiDettagli} ON {aliasUtentiDettagli}.[keyUtente] = {TableName}.{column} COLLATE DATABASE_DEFAULT ");
                            }
                            else
                                strSelect += TableName.Replace("Budget_", "") + "." + column + " ";
                        }

                        stbQueryBase.AppendLine(strSelect);

                        kendoColumns.Add(new KendoColumn { Field = column, Title = column, DataType = dataType, TranslateTitle = true, Hidden = hiddenKendo, ColumnOrder = (setStdColumn ? columnOrder_stdGIAS : SetColumnOrder(ref columnOrderIndex)) });

                    }
                }
            }
        }

        private string GetUtenti_TT(List<KendoColumn> kendoColumns, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            if (!_configuratore.isMostraPianoColturale && !_configuratore.isMostraMovimenti)
                kendoColumns.Add(new KendoColumn { Field = "Username_Creazione", Title = "UtenteCreazione", DataType = "string", Hidden = true, ColumnOrder = columnOrder_stdGIAS });

            if (!_configuratore.isMostraMovimenti)
            {
                kendoColumns.Add(new KendoColumn { Field = "Username_Modifica", Title = "UtenteModifica", DataType = "string", Hidden = true, ColumnOrder = columnOrder_stdGIAS });
            }

            StringBuilder stbQuery = new();

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     CodFisc AS [keyUtente] ");
            stbQuery.AppendLine("   , LTRIM(RTRIM(COALESCE(nome, '') + ' ' + COALESCE(Cognome, '') + ' ' + COALESCE(Rag_Soc, ''))) AS Username ");
            stbQuery.AppendLine(" INTO #UtentiDettagli_TT ");
            stbQuery.AppendLine($" FROM {_utenti_DB_name}.dbo.Utenti_Dettagli ");
            stbQuery.AppendLine($" WHERE {_utenti_DB_name}.dbo.Utenti_Dettagli.CodFisc <> '' ");
            stbQuery.AppendLine($" AND {_utenti_DB_name}.dbo.Utenti_Dettagli.CodFisc IS NOT NULL ");

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#UtentiDettagli_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #UtentiDettagli_TT ");
            }

            return stbQuery.ToString();

        }

        private string GetGerarchiaImprese_TT(StbFiltri stbFiltri)
        {

            StringBuilder stbQuery = new();

            if (_configuratore.ApplicaFiltroImpreseReferenti)
            {
                stbQuery.AppendLine(" SELECT DISTINCT ");
                stbQuery.AppendLine("      GerarchiaImprese.Figlio ");
                stbQuery.AppendLine(" INTO #FiltroGerarchiaImprese_TT ");
                stbQuery.AppendLine(" FROM GerarchiaImprese ");

                if (_onLoad)
                    //Al primo caricamento mi servono le colonne, non i risultati
                    stbQuery.AppendLine(" WHERE 1 = 2 ");
                else
                {
                    stbQuery.AppendLine(" WHERE 1 = 1 ");

                    stbQuery.AppendLine(stbFiltri.FiltroGerarchiaImprese.ToString());
                }

                if (Debugger.IsAttached)
                {
                    stbQuery.AppendLine(GetTimestamp("#FiltroGerarchiaImprese_TT").ToString());
                    _stbDropTemporaryTable.AppendLine(" DROP TABLE #FiltroGerarchiaImprese_TT ");
                }
            }

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     GerarchiaImprese.Figlio AS [keyGerarchia] ");
            stbQuery.AppendLine("   , STRING_AGG(Padre, ', ') AS piva_padri ");
            stbQuery.AppendLine("   , MAX(Foglia) AS Foglia, MAX(Livello) AS Livello ");
            if (!_configuratore.CaricaNumeroLibroSociImpresaReferente && !_configuratore.CaricaDataIscrizioneLibroSociImpresaReferente)
                //Solo Imprese Referenti
                stbQuery.AppendLine("   , STRING_AGG(Imprese.rag_soc, ', ') AS ImpreseReferenti ");
            else if (_configuratore.CaricaNumeroLibroSociImpresaReferente && !_configuratore.CaricaDataIscrizioneLibroSociImpresaReferente)
            {
                //Imprese Referenti + Numero Libro Soci
                stbQuery.AppendLine("   , STRING_AGG(Imprese.rag_soc + ");
                stbQuery.AppendLine("                ISNULL(CASE WHEN LibroSoci_Codice <> '' THEN  ");
                stbQuery.AppendLine("                             ' (N. Libro Soci: ' + LibroSoci_Codice + ')' ");
                stbQuery.AppendLine("                        END, ''), ', ')  AS ImpreseReferenti ");
            }
            else if (!_configuratore.CaricaNumeroLibroSociImpresaReferente && _configuratore.CaricaDataIscrizioneLibroSociImpresaReferente)
            {
                //Imprese Referenti + Data Iscrizione Libro Soci
                stbQuery.AppendLine("   , STRING_AGG(Imprese.rag_soc + ");
                stbQuery.AppendLine("                ISNULL(CASE WHEN ISNULL(LibroSoci_DataIscrizione, CAST('1900-01-01' AS Date)) <> '1900-01-01' THEN ");
                stbQuery.AppendLine("                             ' (Data Iscrizione Libro Soci: ' + convert(varchar(50), FORMAT(LibroSoci_DataIscrizione, 'dd/MM/yyyy')) + ')'  ");
                stbQuery.AppendLine("                       END, ''), ', ')  AS ImpreseReferenti ");
            }
            else if (_configuratore.CaricaNumeroLibroSociImpresaReferente && _configuratore.CaricaDataIscrizioneLibroSociImpresaReferente)
            {
                //Imprese Referenti + Numero Libro Soci + Data Iscrizione Libro Soci (se presente)
                stbQuery.AppendLine("   , STRING_AGG(Imprese.rag_soc + ");
                stbQuery.AppendLine("                ISNULL(CASE WHEN LibroSoci_Codice <> '' THEN  ");
                stbQuery.AppendLine("                            CASE WHEN LibroSoci_Codice <> ''  AND ISNULL(LibroSoci_DataIscrizione, CAST('1900-01-01' AS Date)) <> '1900-01-01' THEN ");
                stbQuery.AppendLine("                             ' (N. Libro Soci: ' + LibroSoci_Codice + ', Data Iscrizione: ' + convert(varchar(50), FORMAT(LibroSoci_DataIscrizione, 'dd/MM/yyyy')) + ')'  ");
                stbQuery.AppendLine("                            ELSE ");
                stbQuery.AppendLine("                             ' (N. Libro Soci: ' + LibroSoci_Codice + ')' ");
                stbQuery.AppendLine("                            END ");
                stbQuery.AppendLine("                       END, ''), ', ')  AS ImpreseReferenti ");
            }
            stbQuery.AppendLine(" INTO #GerarchiaImprese_TT ");
            stbQuery.AppendLine(" FROM GerarchiaImprese ");
            stbQuery.AppendLine(" JOIN Imprese ON GerarchiaImprese.Padre = Imprese.PIVA ");

            if (_configuratore.ApplicaFiltroImpreseReferenti)
            {
                stbQuery.AppendLine(" JOIN #FiltroGerarchiaImprese_TT FiltroGerarchiaImprese_TT ON FiltroGerarchiaImprese_TT.Figlio = GerarchiaImprese.Figlio ");
            }

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
                stbQuery.AppendLine(" WHERE 1 = 1 ");

            stbQuery.AppendLine(" GROUP BY GerarchiaImprese.Figlio ");

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#GerarchiaImprese_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #GerarchiaImprese_TT ");
            }

            return stbQuery.ToString();
        }

        private string GetParticelleCentriAziendali_TT()
        {

            StringBuilder stbQuery = new();
            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     ImpresexParticelle.ID ");
            stbQuery.AppendLine("   , ImpresexParticelle.PIVA AS [keyPiva1] ");
            stbQuery.AppendLine("   , ImpresexParticelle.Sa_Cod AS [keySaCod1] ");
            stbQuery.AppendLine("   , Istat_Particelle.Localita AS Comune_Particella ");
            stbQuery.AppendLine("   , Istat_Particelle.COMUNI_Prov AS Provincia_Particella ");
            stbQuery.AppendLine("   , CASE WHEN ImpresexParticelle.Sezione = '0' THEN '' ELSE ImpresexParticelle.Sezione END AS Sezione ");
            stbQuery.AppendLine("   , ISNULL(CAST(ImpresexParticelle.Foglio AS VARCHAR(200)), '') AS Foglio ");
            stbQuery.AppendLine("   , ISNULL(CAST(ImpresexParticelle.Numero AS VARCHAR(200)), '') AS Numero ");
            stbQuery.AppendLine("   , ISNULL(CAST(ImpresexParticelle.Subalterno AS VARCHAR(200)), '') AS Subalterno ");
            stbQuery.AppendLine("   , ImpresexParticelle.Validita_Inizio AS Validita_Inizio_Possesso ");
            stbQuery.AppendLine("   , ImpresexParticelle.Validita_Fine AS Validita_Fine_Possesso ");

            if (_fattoreConversione > 1)
            {
                stbQuery.AppendLine($"   , ROUND(ISNULL(ImpresexParticelle.Sup_Condotta,0) * {_fattoreConversione.ToString().Replace(",", ".")}, 4) AS Sup_Condotta_Acro ");
                stbQuery.AppendLine($"   , ROUND(ISNULL(ParticelleCatastali.Ettari + (CAST(ParticelleCatastali.ARE AS decimal) / 100) + (CAST(ParticelleCatastali.CENTIARE AS decimal) / 10000),0)  * {_fattoreConversione.ToString().Replace(",", ".")}, 4) AS Sup_Catastale_Acro ");
            }
            else
            {
                stbQuery.AppendLine("   , ImpresexParticelle.Sup_Condotta ");
                stbQuery.AppendLine("   , ParticelleCatastali.Ettari + (CAST(ParticelleCatastali.ARE AS decimal) / 100) + (CAST(ParticelleCatastali.CENTIARE AS decimal) / 10000) AS Sup_Catastale ");
            }

            stbQuery.AppendLine(" INTO #ParticelleCentriAziendali_TT ");
            stbQuery.AppendLine(" FROM ImpresexParticelle ");
            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: "ImpresexParticelle", joinTable: (int)Enum_Entita_FiltroRicerca.Azienda).ToString());
            stbQuery.AppendLine(" LEFT JOIN ParticelleCatastali ON ImpresexParticelle.PROV = ParticelleCatastali.PROV AND ImpresexParticelle.COM = ParticelleCatastali.COM AND ImpresexParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpresexParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND ImpresexParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpresexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ");
            stbQuery.AppendLine(" LEFT JOIN ISTAT Istat_Particelle ON ImpresexParticelle.PROV = Istat_Particelle.PROV AND ImpresexParticelle.COM = Istat_Particelle.COM ");

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#ParticelleCentriAziendali_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #ParticelleCentriAziendali_TT ");
            }

            return stbQuery.ToString();
        }

        private string GetParticelleAppezzamenti_TT()
        {
            StringBuilder stbQuery = new();

            if (_configuratore.CaricaDatiCatastaliAppezzamento)
            {
                stbQuery.AppendLine(" SELECT ");
                stbQuery.AppendLine("     AppezzamentiXParticelle.PIVA AS [keyPiva2] ");
                stbQuery.AppendLine("   , AppezzamentiXParticelle.Sa_Cod AS [keySaCod2] ");
                stbQuery.AppendLine("   , AppezzamentiXParticelle.Appezza AS [keyAppezza2] ");
                stbQuery.AppendLine(" INTO #AppezzamentiMonoParticella_TT ");
                stbQuery.AppendLine($" FROM {_modalitaBudget}AppezzamentiXParticelle AS AppezzamentiXParticelle ");
                stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: "AppezzamentiXParticelle", joinTable: (int)Enum_Entita_FiltroRicerca.CentroAziendale).ToString());

                if (_onLoad)
                    //Al primo caricamento mi servono le colonne, non i risultati
                    stbQuery.AppendLine(" WHERE 1 = 2 ");
                else
                {
                    stbQuery.AppendLine(" WHERE 1 = 1 ");
                    if (_modalitaBudget != "")
                        stbQuery.AppendLine(" AND AppezzamentiXParticelle.Id_Budget = @IdBudget ");
                }

                stbQuery.AppendLine(" GROUP BY AppezzamentiXParticelle.Piva, AppezzamentiXParticelle.Sa_Cod, AppezzamentiXParticelle.Appezza");
                if (_modalitaBudget != "")
                    stbQuery.Append(", Id_Budget ");

                stbQuery.AppendLine(" HAVING COUNT(0) = 1 ");

                if (Debugger.IsAttached)
                {
                    stbQuery.AppendLine(GetTimestamp("#AppezzamentiMonoParticella_TT").ToString());
                    _stbDropTemporaryTable.AppendLine(" DROP TABLE #AppezzamentiMonoParticella_TT ");
                }
            }


            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("      AppezzamentiXParticelle.PIVA AS [keyPiva1] ");
            stbQuery.AppendLine("   , AppezzamentiXParticelle.Sa_Cod AS [keySaCod1] ");
            stbQuery.AppendLine("   , AppezzamentiXParticelle.Appezza AS [keyAppezza1] ");

            if (_configuratore.CaricaDatiCatastaliAppezzamento)
            {
                stbQuery.AppendLine("   , Istat_Particelle.Localita AS Comune_Particella ");
                stbQuery.AppendLine("   , Istat_Particelle.COMUNI_Prov AS Provincia_Particella ");
                stbQuery.AppendLine("   , CASE WHEN AppezzamentiXParticelle.Sezione = '0' THEN '' ELSE AppezzamentiXParticelle.Sezione END AS Sezione ");
                stbQuery.AppendLine("   , AppezzamentiXParticelle.Foglio ");
                stbQuery.AppendLine("   , AppezzamentiXParticelle.Numero ");
                stbQuery.AppendLine("   , AppezzamentiXParticelle.Subalterno ");
            }
            stbQuery.AppendLine(" INTO #ParticelleAppezzamenti_TT ");
            stbQuery.AppendLine($" FROM {_modalitaBudget}AppezzamentiXParticelle AS AppezzamentiXParticelle ");
            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: "AppezzamentiXParticelle", joinTable: (int)Enum_Entita_FiltroRicerca.CentroAziendale).ToString());

            if (_configuratore.CaricaDatiCatastaliAppezzamento)
            {
                stbQuery.AppendLine(" JOIN #AppezzamentiMonoParticella_TT AppezzamentiMonoParticella_TT ON ");
                stbQuery.AppendLine("     AppezzamentiMonoParticella_TT.[keyPiva2] = AppezzamentiXParticelle.PIVA ");
                stbQuery.AppendLine(" AND AppezzamentiMonoParticella_TT.[keySaCod2] = AppezzamentiXParticelle.Sa_Cod ");
                stbQuery.AppendLine(" AND AppezzamentiMonoParticella_TT.[keyAppezza2] = AppezzamentiXParticelle.Appezza ");
                stbQuery.AppendLine(" LEFT JOIN ISTAT Istat_Particelle ON AppezzamentiXParticelle.PROV = Istat_Particelle.PROV AND AppezzamentiXParticelle.COM = Istat_Particelle.COM ");
            }

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                if (_modalitaBudget != "")
                    stbQuery.AppendLine(" AND AppezzamentiXParticelle.Id_Budget = @IdBudget ");
            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#ParticelleAppezzamenti_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #ParticelleAppezzamenti_TT ");
            }

            return stbQuery.ToString();
        }

        private string GetZVNAppezzamenti_TT()
        {
            StringBuilder stbQuery = new();

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("      AppezzamentiXParticelle.PIVA AS [keyPiva12] ");
            stbQuery.AppendLine("   , AppezzamentiXParticelle.Sa_Cod AS [keySaCod12] ");
            stbQuery.AppendLine("   , AppezzamentiXParticelle.Appezza AS [keyAppezza12] ");
            stbQuery.AppendLine("   , MAX(ZonexParticelle.Zona_Cod) AS ZVN ");

            stbQuery.AppendLine(" INTO #AppezzamentiZVN_TT ");
            stbQuery.AppendLine(" FROM AppezzamentiXParticelle ");
            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: "AppezzamentiXParticelle", joinTable: (int)Enum_Entita_FiltroRicerca.CentroAziendale).ToString());
            stbQuery.AppendLine(" JOIN ZonexParticelle ON ");
            stbQuery.AppendLine("     AppezzamentiXParticelle.PROV = ZonexParticelle.PROV ");
            stbQuery.AppendLine(" AND AppezzamentiXParticelle.COM = ZonexParticelle.COM ");
            stbQuery.AppendLine(" AND AppezzamentiXParticelle.SEZIONE = ZonexParticelle.SEZIONE ");
            stbQuery.AppendLine(" AND AppezzamentiXParticelle.FOGLIO = ZonexParticelle.FOGLIO ");
            stbQuery.AppendLine(" AND AppezzamentiXParticelle.NUMERO = ZonexParticelle.NUMERO ");
            stbQuery.AppendLine(" AND AppezzamentiXParticelle.SUBALTERNO = ZonexParticelle.SUBALTERNO ");
            stbQuery.AppendLine($" AND ZonexParticelle.Zona_Cod = {(int)Enum_Zone.ZVN} ");

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                if (_modalitaBudget != "")
                    stbQuery.AppendLine(" AND AppezzamentiXParticelle.Id_Budget = @IdBudget ");
            }

            stbQuery.AppendLine(" GROUP BY AppezzamentiXParticelle.PIVA, AppezzamentiXParticelle.SA_COD, AppezzamentiXParticelle.APPEZZA ");

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#AppezzamentiZVN_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #AppezzamentiZVN_TT ");
            }

            return stbQuery.ToString();
        }

        private string GetParticelleCampi_TT()
        {
            StringBuilder stbQuery = new();

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     CampiXParticelle.PIVA AS [keyPiva2] ");
            stbQuery.AppendLine("   , Sa_Cod AS [keySaCod2] ");
            stbQuery.AppendLine("   , Campo_Cod AS [keyCampoCod2] ");

            if (_configuratore.CaricaDatiCatastaliCampo)
            {
                //Se mostro i dati catastali, mostro le singole particelle == n righe per campo
                stbQuery.AppendLine("   , Istat_Particelle.Localita AS Comune_Particella ");
                stbQuery.AppendLine("   , Istat_Particelle.COMUNI_Prov AS Provincia_Particella ");
                stbQuery.AppendLine("   , CASE WHEN CampiXParticelle.Sezione = '0' THEN '' ELSE CampiXParticelle.Sezione END AS Sezione ");
                stbQuery.AppendLine("   , CampiXParticelle.Foglio ");
                stbQuery.AppendLine("   , CampiXParticelle.Numero ");
                stbQuery.AppendLine("   , CampiXParticelle.Subalterno ");
            }
            else
            {
                //Se non mostro i dati catastali, mostro la somma della SUP Catastale == una riga per campo
                stbQuery.AppendLine("   , SUM(ISNULL(AREA, 0)) AS SAU_Catastale ");
            }

            stbQuery.AppendLine(" INTO #ParticelleCampi_TT ");
            stbQuery.AppendLine($" FROM {_modalitaBudget}CampiXParticelle AS CampiXParticelle ");
            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: "CampiXParticelle", joinTable: (int)Enum_Entita_FiltroRicerca.Azienda).ToString());

            if (_configuratore.CaricaDatiCatastaliCampo)
                stbQuery.AppendLine(" LEFT JOIN ISTAT Istat_Particelle ON CampiXParticelle.PROV = Istat_Particelle.PROV AND CampiXParticelle.COM = Istat_Particelle.COM ");

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                if (_modalitaBudget != "")
                    stbQuery.AppendLine(" AND CampiXParticelle.Id_Budget = @IdBudget ");
            }

            if (!_configuratore.CaricaDatiCatastaliCampo)
                stbQuery.AppendLine(" GROUP BY CampiXParticelle.PIVA, Sa_Cod, Campo_Cod");
            if (_modalitaBudget != "")
                stbQuery.Append(", Id_Budget ");

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#ParticelleCampi_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #ParticelleCampi_TT ");
            }

            return stbQuery.ToString();
        }

        private string GetSuperficiCampi_TT()
        {
            StringBuilder stbQuery = new();

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     PIVA AS [keyPiva1] ");
            stbQuery.AppendLine("   , Sa_Cod AS [keySaCod1] ");
            stbQuery.AppendLine("   , Campo_Cod AS [keyCampoCod1] ");
            stbQuery.AppendLine($"   , SUM(ISNULL(CASE WHEN X.MetodoProduzione = {(int)Enum_TipoAgricoltura.Convenzionale} THEN ISNULL(Sup_App, 0) END, 0)) AS SAU_Convenzionale ");
            stbQuery.AppendLine($"   , SUM(ISNULL(CASE WHEN X.MetodoProduzione = {(int)Enum_TipoAgricoltura.InConversione} THEN ISNULL(Sup_App, 0) END, 0)) AS SAU_Conversione ");
            stbQuery.AppendLine($"   , SUM(ISNULL(CASE WHEN X.MetodoProduzione = {(int)Enum_TipoAgricoltura.Biologica} THEN ISNULL(Sup_App, 0) END, 0)) AS SAU_Biologico ");
            stbQuery.AppendLine(" INTO #CampixSuperfici_TT ");
            stbQuery.AppendLine(" FROM ");
            stbQuery.AppendLine(" ( ");
            stbQuery.AppendLine("   SELECT ");
            stbQuery.AppendLine("         Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.Campo_Cod, Appezzamento.APPEZZA ");
            stbQuery.AppendLine("       , SUM(Appezzamento.SUP_APP) AS Sup_App ");
            stbQuery.AppendLine($"       , ISNULL(Appezzamento_Codici.Val_Cod, {(int)Enum_TipoAgricoltura.Convenzionale}) AS MetodoProduzione ");
            stbQuery.AppendLine($"   FROM {_modalitaBudget}Appezzamento AS Appezzamento ");
            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: "Appezzamento", joinTable: (int)Enum_Entita_FiltroRicerca.CentroAziendale).ToString());
            stbQuery.AppendLine($"   LEFT JOIN {_modalitaBudget}Appezzamento_Codici Appezzamento_Codici ON ");
            stbQuery.AppendLine("       Appezzamento.PIVA = Appezzamento_Codici.PIVA ");
            stbQuery.AppendLine("   AND Appezzamento.SA_COD = Appezzamento_Codici.SA_COD ");
            stbQuery.AppendLine("   AND Appezzamento.APPEZZA = Appezzamento_Codici.appezza ");
            stbQuery.AppendLine($"   AND Appezzamento_Codici.Id_Cod = {(int)Enum_CodiciAnagrafe.MetodoDiProduzione} ");

            if (_modalitaBudget != "")
                stbQuery.AppendLine("   AND Appezzamento_Codici.Id_Budget = @IdBudget ");

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine("   AND Appezzamento.Campo_Cod <> 0 ");
                if (_modalitaBudget != "")
                    stbQuery.AppendLine("   AND Appezzamento.Id_Budget = @IdBudget ");
            }

            stbQuery.AppendLine("   GROUP BY Appezzamento.PIVA, Appezzamento.SA_COD, Appezzamento.Campo_Cod, Appezzamento.APPEZZA, ISNULL(Appezzamento_Codici.Val_Cod, 1) ");
            stbQuery.AppendLine(" ) AS X ");
            stbQuery.AppendLine(" GROUP BY PIVA, SA_COD, Campo_Cod ");

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#CampixSuperfici_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #CampixSuperfici_TT ");
            }

            return stbQuery.ToString();
        }

        private string GetDestinazioneUso_TT(StbFiltri stbFiltri)
        {
            StringBuilder stbQuery = new();

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     Reg_Impianti.PIVA AS [keyPiva1] ");
            stbQuery.AppendLine("   , Reg_Impianti.Sa_Cod AS [keySaCod1] ");
            stbQuery.AppendLine("   , Reg_Impianti.Appezza AS [keyAppezza1] ");
            stbQuery.AppendLine("   , Reg_Impianti.Id_Reg AS [keyIdReg1] ");
            stbQuery.AppendLine("   , DestinazioniUso.Descrizione ");
            stbQuery.AppendLine("   , DestinazioniUso.Codice ");
            stbQuery.AppendLine(" INTO #DestinazioniUso_TT ");
            stbQuery.AppendLine($" FROM {_modalitaBudget}Reg_Impianti_Codici AS Reg_Impianti ");
            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.Impianto).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.Appezzamento).ToString());

            stbQuery.AppendLine(" LEFT JOIN Codici_Anagrafe DestinazioniUso ON Reg_Impianti.Id_Cod = DestinazioniUso.codice ");
            stbQuery.AppendLine(" WHERE Id_Cod >= 3000 AND Id_Cod < 4000 ");

            if (_modalitaBudget != "")
                stbQuery.AppendLine(" AND Reg_Impianti.Id_Budget = @IdBudget ");

            stbQuery.AppendLine(stbFiltri.FiltroDestinazioniUso.ToString());

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#DestinazioniUso_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #DestinazioniUso_TT ");
            }

            return stbQuery.ToString();
        }

        private string GetUtilizzoTerreno_TT(StbFiltri stbFiltri)
        {
            StringBuilder stbQuery = new();

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     Appezzamento.PIVA AS [keyPiva11] ");
            stbQuery.AppendLine("   , Appezzamento.Sa_Cod AS [keySaCod11] ");
            stbQuery.AppendLine("   , Appezzamento.Appezza AS [keyAppezza11] ");
            stbQuery.AppendLine("   , Appezzamento.Val_Cod AS [Val_Cod] ");
            stbQuery.AppendLine(" INTO #UtilizzoTerreno_TT ");
            stbQuery.AppendLine($" FROM {_modalitaBudget}Appezzamento_Codici AS Appezzamento ");
            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: ((int)Enum_Entita_FiltroRicerca.Appezzamento).ToString(), joinTable: (int)Enum_Entita_FiltroRicerca.CentroAziendale).ToString());
            stbQuery.AppendLine($" WHERE Id_Cod = {(int)Enum_CodiciAnagrafe.MetodoDiProduzione} ");

            if (_modalitaBudget != "")
                stbQuery.AppendLine(" AND Appezzamento.Id_Budget = @IdBudget ");

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#UtilizzoTerreno_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #UtilizzoTerreno_TT ");
            }

            return stbQuery.ToString();
        }

        private string GetDataSeminaTrapianto_TT()
        {
            StringBuilder stbQuery = new();

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     CONVERT(VARCHAR(10), (MAX(Agenda.Validita_Inizio)), 103) AS [Data] ");
            stbQuery.AppendLine("   , Mov_Destinazioni.Piva AS [keyPiva3] ");
            stbQuery.AppendLine("   , Mov_Destinazioni.Sa_Cod AS [keySaCod3] ");
            stbQuery.AppendLine("   , Mov_Destinazioni.Appezza AS [keyAppezza3] ");
            stbQuery.AppendLine("   , Mov_Destinazioni.Id_Destinazione AS [keyIdReg3] ");
            stbQuery.AppendLine(" INTO #DataSeminaTrapianto_TT ");
            stbQuery.AppendLine(" FROM Agenda ");
            stbQuery.AppendLine(" JOIN Movimenti ON ");
            stbQuery.AppendLine("     Agenda.Piva = Movimenti.Piva ");
            stbQuery.AppendLine(" AND Agenda.Sa_Cod = Movimenti.Sa_Cod ");
            stbQuery.AppendLine(" AND Agenda.Id_Agenda = Movimenti.Id_Agenda ");
            stbQuery.AppendLine(" JOIN Movimenti_dettagli ON ");
            stbQuery.AppendLine("     Movimenti.Piva = Movimenti_dettagli.Piva ");
            stbQuery.AppendLine(" AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod ");
            stbQuery.AppendLine(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda ");
            stbQuery.AppendLine(" AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ");
            stbQuery.AppendLine(" JOIN Mov_Destinazioni ON ");
            stbQuery.AppendLine("     Movimenti_dettagli.Piva = Mov_Destinazioni.Piva ");
            stbQuery.AppendLine(" AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ");
            stbQuery.AppendLine(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ");
            stbQuery.AppendLine(" AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ");
            stbQuery.AppendLine(" AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ");
            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: "Mov_Destinazioni", joinTable: (int)Enum_Entita_FiltroRicerca.Appezzamento).ToString());

            stbQuery.AppendLine($" WHERE (Agenda.Lav_Cod = {LAV_COD.LAVCOD_SEMINA} OR Agenda.Lav_Cod = {LAV_COD.LAVCOD_TRAPIANTO}) ");
            stbQuery.AppendLine($" AND Movimenti.Cau_Mov = '{(int)enum_Agenda_Causali.LAVORAZIONE}' ");
            stbQuery.AppendLine(" GROUP BY Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione ");

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#DataSeminaTrapianto_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #DataSeminaTrapianto_TT ");
            }

            return stbQuery.ToString();
        }

        private string GetLegaleRappresentante_TT()
        {
            StringBuilder stbQuery = new();

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("      PIVA ");
            stbQuery.AppendLine("   , MAX(Sa_Cod) AS Sa_Cod ");
            stbQuery.AppendLine("   , MAX(Cod_Contatto) AS Cod_Contatto ");
            stbQuery.AppendLine(" INTO #Contatti_TT ");
            stbQuery.AppendLine(" FROM Risorse_Umane ");
            stbQuery.AppendLine($" WHERE Risorse_Umane.Cod_Rapporto = {(int)Enum_Rapporti_Contabili_Standard.Legale_Rappresentante} ");
            stbQuery.AppendLine(" GROUP BY Piva ");

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#Contatti_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #Contatti_TT ");
            }

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     Contatti_TT.PIVA AS [keyPiva7] ");
            stbQuery.AppendLine("   , Contatti.Sa_Cod AS [keySaCod7] ");
            stbQuery.AppendLine("   , MAX(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome) AS LegaleRappresentante ");
            stbQuery.AppendLine("   , MAX(Data_Nascita) AS LegaleRappresentante_DataNascita ");
            stbQuery.AppendLine("   , MAX(Codice_Fiscale) AS LegaleRappresentante_CodiceFiscale ");
            stbQuery.AppendLine("   , CASE WHEN MAX(ind_des) = '&nbsp;' THEN '' ELSE MAX(ind_des) END AS LegaleRappresentante_LuogoNascita ");
            stbQuery.AppendLine(" INTO #LegaleRappresentante_TT ");
            stbQuery.AppendLine(" FROM Contatti ");
            stbQuery.AppendLine(" JOIN #Contatti_TT Contatti_TT ON Contatti_TT.Cod_Contatto = Contatti.Cod_Contatto AND Contatti_TT.Sa_Cod = Contatti.Sa_Cod and Contatti_TT.Piva = Contatti.Piva ");
            stbQuery.AppendLine(" JOIN ContattiXIndirizzi ON ContattiXIndirizzi.Cod_Contatto = Contatti.Cod_Contatto and Tipo_Indirizzo = " + (int)Enum_IndirizzoTipo.LuogoNascita + " ");
            stbQuery.AppendLine(" JOIN Indirizzi ON Indirizzi.cod_indirizzo = ContattiXIndirizzi.Cod_Indirizzo ");
            stbQuery.AppendLine(" GROUP BY Contatti_TT.PIVA, Contatti.Sa_Cod ");

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#LegaleRappresentante_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #LegaleRappresentante_TT ");
            }

            return stbQuery.ToString();
        }

        private string GetInfoTecnico_TT()
        {
            StringBuilder stbQuery = new();
            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     Contatti.Cod_Contatto AS [keyTecnico1] ");
            stbQuery.AppendLine("   , CASE WHEN descr IN ('Cellulare', 'personale', 'Telefono','elefono', 'ellulare', 'ax', 'Fax:', 'N. Telefono:') THEN descr ELSE '' END AS Tecnico_Numero ");
            stbQuery.AppendLine("   , CASE WHEN descr IN ('Email:', 'mail', 'Email', 'E-Mail:') THEN descr ELSE '' END AS Tecnico_Email ");
            stbQuery.AppendLine("   , sesso AS Tecnico_Sesso ");
            stbQuery.AppendLine("   , data_nascita AS Tecnico_DataNascita ");
            stbQuery.AppendLine(" INTO #InfoTecnico_TT ");
            stbQuery.AppendLine(" FROM Contatti ");
            stbQuery.AppendLine(" JOIN ContattiXRubrica ON CONTATTI.Cod_Contatto = ContattiXRubrica.Cod_Contatto ");
            stbQuery.AppendLine(" JOIN Rubrica ON ContattiXRubrica.Cod_Rubrica = Rubrica.Cod_Rubrica ");
            stbQuery.AppendLine($" JOIN Risorse_Umane ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto AND Risorse_Umane.Cod_Rapporto = {(int)Enum_Rapporti_Contabili_Standard.Tecnico} ");

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#InfoTecnico_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #InfoTecnico_TT ");
            }

            return stbQuery.ToString();
        }

        private string GetIndirizziAppezzamenti_TT()
        {
            StringBuilder stbQuery = new();

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     AppezzamentixIndirizzi.PIVA ");
            stbQuery.AppendLine("   , AppezzamentixIndirizzi.Sa_Cod ");
            stbQuery.AppendLine("   , AppezzamentixIndirizzi.Appezza ");
            stbQuery.AppendLine("   , MAX(Cod_Indirizzo) AS Cod_Indirizzo ");
            stbQuery.AppendLine(" INTO #AppezzamentiXIndirizzi_TT ");
            stbQuery.AppendLine($" FROM {_modalitaBudget}AppezzamentixIndirizzi AS AppezzamentixIndirizzi ");

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                if (_modalitaBudget != "")
                    stbQuery.AppendLine(" AND AppezzamentixIndirizzi.Id_Budget = @IdBudget ");
            }
            stbQuery.AppendLine(" GROUP BY Piva, Sa_Cod, Appezza ");

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#AppezzamentiXIndirizzi_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #AppezzamentiXIndirizzi_TT ");
            }

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     AppezzamentiXIndirizzi_TT.PIVA AS [keyPiva5] ");
            stbQuery.AppendLine("   , AppezzamentiXIndirizzi_TT.Sa_Cod AS [keySaCod5] ");
            stbQuery.AppendLine("   , AppezzamentiXIndirizzi_TT.Appezza AS [keyAppezza5] ");
            stbQuery.AppendLine("   , Indirizzi.Ind_Des AS Indirizzo_Appezzamento ");
            stbQuery.AppendLine("   , Indirizzi.Frz_Des AS Frazione_Appezzamento ");
            stbQuery.AppendLine("   , Indirizzi.CAP AS CAP_Appezzamento ");

            stbQuery.AppendLine("   , Lista_Stati.Descrizione AS Stato_Appezzamento ");
            stbQuery.AppendLine("   , Lista_Regioni.Regione_Des AS Regione_Appezzamento ");
            stbQuery.AppendLine("   , ISTAT.COMUNI_PROV AS Provincia_Appezzamento ");
            stbQuery.AppendLine("   , ISTAT.Localita AS Comune_Appezzamento ");


            stbQuery.AppendLine(" INTO #IndirizziAppezzamenti_TT  ");
            stbQuery.AppendLine($" FROM {_modalitaBudget}AppezzamentixIndirizzi AS AppezzamentixIndirizzi ");
            stbQuery.AppendLine(" JOIN #AppezzamentiXIndirizzi_TT AppezzamentiXIndirizzi_TT ON ");
            stbQuery.AppendLine(" AppezzamentiXIndirizzi_TT.PIVA = AppezzamentixIndirizzi.PIVA ");
            stbQuery.AppendLine(" AND AppezzamentiXIndirizzi_TT.SA_COD = AppezzamentixIndirizzi.SA_COD ");
            stbQuery.AppendLine(" AND AppezzamentiXIndirizzi_TT.APPEZZA = AppezzamentixIndirizzi.APPEZZA ");
            stbQuery.AppendLine(" AND AppezzamentiXIndirizzi_TT.Cod_Indirizzo = AppezzamentixIndirizzi.Cod_Indirizzo ");

            stbQuery.AppendLine(" LEFT JOIN Indirizzi ON AppezzamentixIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo ");
            stbQuery.AppendLine(" LEFT JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ");
            stbQuery.AppendLine(" LEFT JOIN Lista_Province ON Lista_Province.Sigla = ISTAT.COMUNI_PROV ");
            stbQuery.AppendLine(" LEFT JOIN Lista_Regioni ON Lista_Regioni.REG = Lista_Province.REG AND Lista_Province.REG <> '000' ");
            stbQuery.AppendLine(" LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 Lista_Stati ON Lista_Stati.Codice = Lista_Regioni.Stato_Country ");

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                if (_modalitaBudget != "")
                    stbQuery.AppendLine(" AND AppezzamentixIndirizzi.Id_Budget = @IdBudget ");
            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#IndirizziAppezzamenti_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #IndirizziAppezzamenti_TT ");
            }

            return stbQuery.ToString();
        }

        private string GetServizi_TT(StbFiltri stbFiltri)
        {
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine(" SELECT ");
            stbQuery.AppendLine("     Pratiche.Piva ");
            if (_configuratore.CaricaDatiServizi)
            {
                stbQuery.AppendLine("   , Servizi.Servizio_Des AS Servizio ");
                stbQuery.AppendLine("   , WAnagraficaStati.WAnagraficaStati_Des AS StatoPratica ");
                stbQuery.AppendLine("   , CASE WHEN CONVERT(VARCHAR(4), Pratiche.Anno) = '0' THEN '' ELSE CONVERT(VARCHAR(4),Pratiche.Anno) END AS AnnoPratica ");
                stbQuery.AppendLine("   , Pratiche.Numero AS NumeroPratica ");
                stbQuery.AppendLine("   , Pratiche.Pratica_Cod ");
            }
            stbQuery.AppendLine(" INTO #Servizi_TT ");
            stbQuery.AppendLine(" FROM Pratiche ");
            stbQuery.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Piva_SuperUser = Pratiche.Piva_SuperUser AND Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ");

            if (_configuratore.CaricaDatiServizi)
            {
                stbQuery.AppendLine(" JOIN WAnagraficaStati ON WAnagraficaStati.WAnagraficaStati_Cod = Pratiche_Stati_Attuali.Stato_Cod ");
                stbQuery.AppendLine(" JOIN Servizi ON Servizi.Servizio_Cod = Pratiche.Servizio_Cod ");
            }

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine(stbFiltri.FiltroServizi.ToString());
            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#Servizi_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #Servizi_TT");
            }

            return stbQuery.ToString();
        }

        private string GetImpiantiGIS_TT(StbFiltri stbFiltri)
        {
            var stbQuery = new StringBuilder();

            if (_configuratore.joinGISAnomalie)
            {
                stbQuery.AppendLine(" SELECT DISTINCT ");
                stbQuery.AppendLine("   ChiaveGrafica, Algoritmo ");
                stbQuery.AppendLine(" INTO #AnomaliePoligoni_TT ");
                stbQuery.AppendLine(" FROM GIS_ProcessingAlgorithms_Cleaning_Risultati ");

                if (_onLoad)
                    //Al primo caricamento mi servono le colonne, non i risultati
                    stbQuery.AppendLine(" WHERE 1 = 2 ");
                else
                {
                    stbQuery.AppendLine(" WHERE 1 = 1 ");
                    stbQuery.AppendLine(stbFiltri.FiltroGISAnomalie.ToString());
                }

                if (Debugger.IsAttached)
                {
                    stbQuery.AppendLine(GetTimestamp("#AnomaliePoligoni_TT").ToString());
                    _stbDropTemporaryTable.AppendLine(" DROP TABLE #AnomaliePoligoni_TT ");
                }
            }

            stbQuery.AppendLine(" SELECT DISTINCT ");
            stbQuery.AppendLine("     GIS_Entita.PIVA AS [keyPiva9] ");
            stbQuery.AppendLine("   , GIS_Entita.sa_cod AS [keySaCod9] ");
            stbQuery.AppendLine("   , GIS_Entita.Appezza AS [keyAppezza9] ");
            stbQuery.AppendLine("   , GIS_Entita.Id_Imp AS [keyIdReg9] ");
            if (_configuratore.CaricaDatiGISImpianto)
            {
                stbQuery.AppendLine("   ,  N'🌎' AS GIS ");
                stbQuery.AppendLine("   ,  CAST(ROUND(Poligono_GeoEntity.EnvelopeCenter().Lat, 6) AS VARCHAR(100)) + ' ' + CAST(ROUND(Poligono_GeoEntity.EnvelopeCenter().Long, 6) AS VARCHAR(100)) AS Baricentro ");
            }
            stbQuery.AppendLine(" INTO #ImpiantiGIS_TT ");
            stbQuery.AppendLine(" FROM GIS_Entita ");
            stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: "GIS_Entita", joinTable: (int)Enum_Entita_FiltroRicerca.Azienda).ToString());

            stbQuery.AppendLine(" JOIN GIS_ElementiGrafici ON GIS_ElementiGrafici.Entita_Cod = GIS_Entita.Entita_Cod AND GIS_ElementiGrafici.LayerElementiGrafici_Cod = " + (int)Enum_Gis_LayerElementiGrafici_std.IMPIANTI);

            if (_configuratore.joinGISAnomalie)
            {
                stbQuery.AppendLine(" JOIN #AnomaliePoligoni_TT AnomaliePoligoni_TT ON ");
                stbQuery.AppendLine("     GIS_Entita.PIVA + '-' + ");
                stbQuery.AppendLine("     CAST(GIS_Entita.Sa_Cod AS VARCHAR(100)) + '-' + ");
                stbQuery.AppendLine("     CAST(GIS_Entita.Appezza AS VARCHAR(100)) + '-' + ");
                stbQuery.AppendLine("     CAST(GIS_Entita.Id_Imp AS VARCHAR(100)) = AnomaliePoligoni_TT.ChiaveGrafica ");
            }

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
                stbQuery.AppendLine(" WHERE 1 = 1 ");

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#ImpiantiGIS_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #ImpiantiGIS_TT");
            }

            return stbQuery.ToString();
        }

        private string GetOperazioniAgenda_TT(StbFiltri stbFiltri,
                                              bool joinSaCod = false,
                                              bool joinCampoCod = false,
                                              bool joinAppezza = false,
                                              bool joinIdReg = false,
                                              bool joinProgettoCod = false)
        {
            var stbQuery = new StringBuilder();

            stbQuery.AppendLine(" SELECT DISTINCT ");

            stbQuery.AppendLine("    Agenda.Piva AS [keyPiva10] ");

            if (joinSaCod)
                stbQuery.AppendLine("   , Agenda.Sa_Cod AS [keySaCod10] ");
            if (joinCampoCod)
                stbQuery.AppendLine("   , Appezzamento.Campo_Cod AS [keyCampoCod10] ");
            if (joinAppezza)
                stbQuery.AppendLine("   , Mov_Destinazioni.Appezza AS [keyAppezza10] ");
            if (joinIdReg)
                stbQuery.AppendLine("   , Mov_Destinazioni.Id_Destinazione AS [keyIdReg10] ");
            if (joinProgettoCod)
                stbQuery.AppendLine("   , Imprese_Progetti.Progetto_Cod AS [keyProgettoCod10] ");

            stbQuery.AppendLine("   INTO #OperazioniAgenda_TT ");
            stbQuery.AppendLine("   FROM Agenda ");
            //stbQuery.AppendLine(BuildJoinScalare(currentBaseTable: "Agenda", joinTable: (int)Enum_Entita_FiltroRicerca.Azienda).ToString());

            stbQuery.AppendLine(" JOIN Movimenti ON Movimenti.Piva = Agenda.Piva AND Movimenti.Id_Agenda = Agenda.Id_Agenda ");

            if (joinCampoCod || joinAppezza)
            {
                stbQuery.AppendLine(" JOIN Mov_Destinazioni ON Mov_Destinazioni.Piva = Movimenti.Piva AND Mov_Destinazioni.Id_Agenda = Movimenti.Id_Agenda AND Tipo_Destinazione = 0 ");
                if (joinCampoCod)
                {
                    stbQuery.AppendLine(" JOIN Appezzamento ON ");
                    stbQuery.AppendLine("     Appezzamento.Piva = Mov_Destinazioni.Piva ");
                    stbQuery.AppendLine(" AND Appezzamento.Sa_Cod = Mov_Destinazioni.Sa_Cod ");
                    stbQuery.AppendLine(" AND Appezzamento.Appezza = Mov_Destinazioni.Appezza ");
                }
                if (joinProgettoCod)
                {
                    stbQuery.AppendLine(" JOIN Imprese_Progetti ON ");
                    stbQuery.AppendLine("     Imprese_Progetti.Piva = Mov_Destinazioni.Piva ");
                    stbQuery.AppendLine(" AND Imprese_Progetti.Sa_Cod = Mov_Destinazioni.Sa_Cod ");
                    stbQuery.AppendLine(" AND Imprese_Progetti.Appezza = Mov_Destinazioni.Appezza ");
                    stbQuery.AppendLine(" AND Imprese_Progetti.Id_Reg = Mov_Destinazioni.Id_Destinazione ");
                    stbQuery.AppendLine(" AND Imprese_Progetti.Validita_Inizio <= Movimenti.Data_Movimento ");
                    stbQuery.AppendLine(" AND Imprese_Progetti.Validita_Fine >= Movimenti.Data_Movimento ");
                }
            }

            stbQuery.AppendLine(" JOIN Operazioni ON Operazioni.LAV_COD = Agenda.Lav_Cod ");
            stbQuery.AppendLine(" JOIN GruppoOperazioni ON GruppoOperazioni.GRU_COD = Operazioni.GRU_OP ");

            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
            {
                stbQuery.AppendLine(" WHERE 1 = 1 ");
                stbQuery.AppendLine(stbFiltri.FiltroMovimenti.ToString());
                stbQuery.AppendLine(stbFiltri.FiltroAgenda.ToString());
            }

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#OperazioniAgenda_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #OperazioniAgenda_TT ");
            }

            return stbQuery.ToString();
        }

        private string GetContributiACA_TT(StbFiltri stbFiltri)
        {
            var stbQuery = new StringBuilder();

            if (_configuratore.ApplicaFiltroContributiACA)
            {
                stbQuery.AppendLine(" SELECT ");
                stbQuery.AppendLine("     Imprese_ProgettiXContributi.Piva ");
                stbQuery.AppendLine("   , Imprese_ProgettiXContributi.ProgettoCod ");
                stbQuery.AppendLine(" INTO #FiltroContributi_TT ");
                stbQuery.AppendLine(" FROM Imprese_ProgettiXContributi ");

                if (_onLoad)
                    //Al primo caricamento mi servono le colonne, non i risultati
                    stbQuery.AppendLine(" WHERE 1 = 2 ");
                else
                {
                    stbQuery.AppendLine(" WHERE 1 = 1 ");
                    stbQuery.AppendLine(" AND ContributoTipo = 1 ");
                    stbQuery.AppendLine(stbFiltri.FiltroContributiACA.ToString());
                }

                if (Debugger.IsAttached)
                {
                    stbQuery.AppendLine(GetTimestamp("#FiltroContributi_TT").ToString());
                    _stbDropTemporaryTable.AppendLine(" DROP TABLE #FiltroContributi_TT ");
                }
            }

            stbQuery.AppendLine(" SELECT DISTINCT ");
            stbQuery.AppendLine("     Imprese_ProgettiXContributi.ProgettoCod AS [keyProgettoCod11]  ");
            stbQuery.AppendLine("   , STRING_AGG(ContributoDes, ', ') AS ContributiACA ");
            stbQuery.AppendLine(" INTO #ContributiACA_TT ");
            stbQuery.AppendLine(" FROM Imprese_ProgettiXContributi ");

            stbQuery.AppendLine(" JOIN Contributi ON ");
            stbQuery.AppendLine("     Contributi.ContributoCod = Imprese_ProgettiXContributi.ContributoCod ");
            stbQuery.AppendLine(" AND Contributi.Tipo = Imprese_ProgettiXContributi.ContributoTipo ");
            stbQuery.AppendLine($" AND Contributi.Tipo = {(int)ContributionType.ACA} ");

            if (_configuratore.ApplicaFiltroContributiACA)
            {
                stbQuery.AppendLine(" JOIN #FiltroContributi_TT FiltroContributi_TT ON ");
                stbQuery.AppendLine(" FiltroContributi_TT.Piva = Imprese_ProgettiXContributi.Piva ");
                stbQuery.AppendLine(" AND FiltroContributi_TT.ProgettoCod = Imprese_ProgettiXContributi.ProgettoCod ");
            }
            if (_onLoad)
                //Al primo caricamento mi servono le colonne, non i risultati
                stbQuery.AppendLine(" WHERE 1 = 2 ");
            else
                stbQuery.AppendLine(" WHERE 1 = 1 ");

            stbQuery.AppendLine(" GROUP BY Imprese_ProgettiXContributi.ProgettoCod ");

            if (Debugger.IsAttached)
            {
                stbQuery.AppendLine(GetTimestamp("#ContributiACA_TT").ToString());
                _stbDropTemporaryTable.AppendLine(" DROP TABLE #ContributiACA_TT");
            }

            return stbQuery.ToString();
        }


        #endregion

        #region "Util"
        private static int SetColumnOrder(ref int columnOrder)
        {
            return columnOrder++;
        }

        /// <summary>
        /// Quando un filtro temporale viene propagato da un'entità figlia verso un genitore nella gerarchia
        /// (esercizio → impianto → appezzamento), la colonna da filtrare deve cambiare perché il genitore
        /// non deve soddisfare lo stesso vincolo del figlio, ma deve essere attivo nel momento in cui il figlio
        /// soddisfa il suo filtro:
        /// - ValiditaInizio >= D sul figlio → ValiditaFine &gt;= D sul genitore (il genitore deve essere ancora attivo quando il figlio inizia)
        /// - ValiditaFine <= D sul figlio  → ValiditaInizio &lt;= D sul genitore (il genitore deve essere già iniziato quando il figlio finisce)
        /// - CompresoFra e IntervalloValidita richiederebbero condizioni composite sul genitore: non vengono propagati.
        /// </summary>
        private static bool TryAdattaFiltroPerPropagazione(int colonnaData, int tipoConfronto, out int colonnaDataAdattata, out int tipoConfrontoAdattato)
        {
            colonnaDataAdattata = colonnaData;
            tipoConfrontoAdattato = tipoConfronto;

            // Casi che richiedono condizioni composite sul genitore: non propagabili semplicemente
            if (tipoConfronto == (int)Enum_TipoConfronto_FiltroRicerca.CompresoFra ||
                colonnaData == (int)Enum_ColonnaData_FiltroRicerca.IntervalloValidita ||
                colonnaData == (int)Enum_ColonnaData_FiltroRicerca.DataCreazioneAnagrafica)
                return false;

            bool isMaggiore = tipoConfronto == (int)Enum_TipoConfronto_FiltroRicerca.Maggiore ||
                              tipoConfronto == (int)Enum_TipoConfronto_FiltroRicerca.MaggioreUguale;
            bool isMinore = tipoConfronto == (int)Enum_TipoConfronto_FiltroRicerca.Minore ||
                            tipoConfronto == (int)Enum_TipoConfronto_FiltroRicerca.MinoreUguale;

            // ValiditaInizio >= D sul figlio → il genitore deve essere ancora attivo a quella data: ValiditaFine >= D
            if (colonnaData == (int)Enum_ColonnaData_FiltroRicerca.ValiditaInizio && isMaggiore)
                colonnaDataAdattata = (int)Enum_ColonnaData_FiltroRicerca.ValiditaFine;

            // ValiditaFine <= D sul figlio → il genitore deve essere già iniziato a quella data: ValiditaInizio <= D
            if (colonnaData == (int)Enum_ColonnaData_FiltroRicerca.ValiditaFine && isMinore)
                colonnaDataAdattata = (int)Enum_ColonnaData_FiltroRicerca.ValiditaInizio;

            return true;
        }

        private StringBuilder GetTimestamp(string nameTT = "", bool onlyDeclare = false, bool total = false)
        {
            StringBuilder stbTimestamp = new();

            if (onlyDeclare)
            {
                stbTimestamp.AppendLine(" DECLARE @seconds AS FLOAT ");
                stbTimestamp.AppendLine(" DECLARE @start AS DATETIME ");
                stbTimestamp.AppendLine(" DECLARE @end AS DATETIME ");
                stbTimestamp.AppendLine(" DECLARE @TotalMillisecond AS FLOAT ");
                stbTimestamp.AppendLine(" SET @TotalMillisecond = 0 ");
                stbTimestamp.AppendLine(" SET @start = GETDATE() ");
                stbTimestamp.AppendLine("");
            }
            else
            {
                stbTimestamp.AppendLine(" SET @end = GETDATE()");
                stbTimestamp.AppendLine(" SET @seconds = DateDiff(MILLISECOND, @start, @end) ");
                stbTimestamp.AppendLine(" SET @TotalMillisecond = @TotalMillisecond + @seconds ");
                stbTimestamp.AppendLine(" PRINT('" + nameTT + "' + ': ' + CAST(@seconds/1000 AS NVARCHAR(15)) + 's') ");
                stbTimestamp.AppendLine("");
                stbTimestamp.AppendLine("");
                stbTimestamp.AppendLine(" SET @start = GETDATE() ");
                if (total)
                    stbTimestamp.AppendLine(" PRINT('Total ' + CAST(@TotalMillisecond/1000 AS NVARCHAR(15)) + 's') ");
            }

            return stbTimestamp;
        }
        private StringBuilder BuildJoinScalare(string currentBaseTable, int joinTable)
        {

            StringBuilder stbJoinScalare = new();
            string tabella = "";

            if (int.TryParse(currentBaseTable, out int currentBaseTableID))
            {
                switch (currentBaseTableID)
                {
                    case (int)Enum_Entita_FiltroRicerca.Azienda:
                        tabella = "Imprese";
                        break;
                    case (int)Enum_Entita_FiltroRicerca.CentroAziendale:
                        tabella = "Centri_Aziendali";
                        break;
                    case (int)Enum_Entita_FiltroRicerca.Campo:
                        tabella = "Campi";
                        break;
                    case (int)Enum_Entita_FiltroRicerca.Appezzamento:
                        tabella = "Appezzamento";
                        break;
                    case (int)Enum_Entita_FiltroRicerca.Impianto:
                        tabella = "Reg_Impianti";
                        break;
                    case (int)Enum_Entita_FiltroRicerca.Esercizio:
                        tabella = "Imprese_Progetti";
                        break;
                    case (int)Enum_Entita_FiltroRicerca.Fabbricato:
                        tabella = "Fabbricati";
                        break;
                    case -1:
                        tabella = "tabellaCodice";
                        break;
                }
            }
            else
            {
                tabella = currentBaseTable;
            }

            switch (joinTable)
            {
                case (int)Enum_Entita_FiltroRicerca.Azienda:
                    stbJoinScalare.AppendLine($" JOIN #AziendeBase_TT AziendeBase_TT ON AziendeBase_TT.[keyPiva] = {tabella}.PIVA ");
                    break;
                case (int)Enum_Entita_FiltroRicerca.CentroAziendale:
                    stbJoinScalare.AppendLine(" JOIN #CentriAziendaliBase_TT CentriAziendaliBase_TT ON ");
                    stbJoinScalare.AppendLine($"     CentriAziendaliBase_TT.[keyPiva] = {tabella}.PIVA ");
                    stbJoinScalare.AppendLine($" AND CentriAziendaliBase_TT.[keySaCod] = {tabella}.Sa_Cod ");
                    break;
                case (int)Enum_Entita_FiltroRicerca.Campo:
                    stbJoinScalare.AppendLine(" JOIN #CampiBase_TT CampiBase_TT ON ");
                    stbJoinScalare.AppendLine($"     CampiBase_TT.[keyPiva] = {tabella}.PIVA ");
                    stbJoinScalare.AppendLine($" AND CampiBase_TT.[keySaCod] = {tabella}.Sa_Cod ");
                    stbJoinScalare.AppendLine($" AND CampiBase_TT.[keyAppezza] = {tabella}.Campo_Cod ");
                    break;
                case (int)Enum_Entita_FiltroRicerca.Appezzamento:
                    stbJoinScalare.AppendLine(" JOIN #AppezzamentiBase_TT AppezzamentiBase_TT ON ");
                    stbJoinScalare.AppendLine($"     AppezzamentiBase_TT.[keyPiva] = {tabella}.PIVA ");
                    stbJoinScalare.AppendLine($" AND AppezzamentiBase_TT.[keySaCod] = {tabella}.Sa_Cod ");
                    stbJoinScalare.AppendLine($" AND AppezzamentiBase_TT.[keyAppezza] = {tabella}.Appezza ");
                    break;
                case (int)Enum_Entita_FiltroRicerca.Impianto:
                    stbJoinScalare.AppendLine(" JOIN #ImpiantiBase_TT ImpiantiBase_TT ON ");
                    stbJoinScalare.AppendLine($"    ImpiantiBase_TT.[keyPiva] = {tabella}.PIVA ");
                    stbJoinScalare.AppendLine($" AND ImpiantiBase_TT.[keySaCod] = {tabella}.Sa_Cod ");
                    stbJoinScalare.AppendLine($" AND ImpiantiBase_TT.[keyAppezza] = {tabella}.Appezza ");
                    stbJoinScalare.AppendLine($" AND ImpiantiBase_TT.[keyIdReg] = {tabella}.Id_Reg ");
                    break;
                case (int)Enum_Entita_FiltroRicerca.Esercizio:
                    stbJoinScalare.AppendLine(" JOIN #EserciziBase_TT EserciziBase_TT ON ");
                    stbJoinScalare.AppendLine($"     EserciziBase_TT.[keyPiva] = {tabella}.PIVA ");
                    stbJoinScalare.AppendLine($" AND EserciziBase_TT.[keySaCod] = {tabella}.Sa_Cod ");
                    stbJoinScalare.AppendLine($" AND EserciziBase_TT.[keyAppezza] = {tabella}.Appezza ");
                    if (tabella == "Mov_Destinazioni")
                        stbJoinScalare.AppendLine($" AND EserciziBase_TT.[keyIdReg] = {tabella}.Id_Destinazione ");
                    else
                    {
                        stbJoinScalare.AppendLine($" AND EserciziBase_TT.[keyIdReg] = {tabella}.Id_Reg ");
                        stbJoinScalare.AppendLine($" AND EserciziBase_TT.[keyProgettoCod] = {tabella}.Progetto_Cod ");
                    }
                    break;
                case (int)Enum_Entita_FiltroRicerca.Fabbricato:
                    stbJoinScalare.AppendLine(" JOIN #FabbricatiBase_TT FabbricatiBase_TT ON ");
                    stbJoinScalare.AppendLine($"     FabbricatiBase_TT.[keyPiva] = {tabella}.PIVA ");
                    stbJoinScalare.AppendLine($" AND FabbricatiBase_TT.[keySaCod] = {tabella}.Sa_Cod ");
                    stbJoinScalare.AppendLine($" AND FabbricatiBase_TT.[keyFabbricatoCod] = {tabella}.Fabbricato_Cod ");
                    break;
            }
            return stbJoinScalare;
        }
        #endregion

        #region "Build Filtri"
        private void CreaFiltroDati(CriteriRicercaExtended criteriRicerca,
                                    Dictionary<string, object> parSql,
                                    Dictionary<string, Dictionary<Type, List<object>>> parSqlIn,
                                    StbFiltri stbFiltri)
        {
            if (_modalitaBudget != "")
                parSql.Add("@IdBudget", _IdBudget);

            AggiungiFiltriAziende(criteriRicerca.FiltriAziende, parSql, parSqlIn, stbFiltri);
            AggiungiFiltriCentriAziendali(criteriRicerca.FiltriCentriAziendali, parSql, parSqlIn, stbFiltri);
            AggiungiFiltriPianoColturale(criteriRicerca.FiltriPianoColturale, parSql, parSqlIn, stbFiltri);
            AggiungiFiltriCampi(criteriRicerca.FiltriCampi, parSql, parSqlIn, stbFiltri);
            AggiungiFiltriTemporali(criteriRicerca.FiltriTemporali, criteriRicerca.FiltriMovimenti, parSql, parSqlIn, stbFiltri);
            AggiungiFiltriServizi(criteriRicerca.FiltriServizi, parSql, parSqlIn, stbFiltri);
            AggiungiFiltriMovimenti(criteriRicerca.FiltriMovimenti, criteriRicerca, parSql, parSqlIn, stbFiltri);
            AggiungiFiltriGIS(criteriRicerca.FiltriGIS, parSql, parSqlIn, stbFiltri);
            AggiungiFiltriCatasto(criteriRicerca.FiltriCatasto, parSql, parSqlIn, stbFiltri);
        }
        private void AggiungiFiltriAziende(FiltriAziende filtriAziende, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn, StbFiltri stbFiltri)
        {
            if (filtriAziende == null) return;

            // IMPRESE REFERENTI
            if (filtriAziende.ImpreseReferenti != null && filtriAziende.ImpreseReferenti.Any())
            {
                stbFiltri.FiltroGerarchiaImprese.AppendLine(" AND GerarchiaImprese.Padre IN (@parImpresaReferente) ");

                parSqlIn.Add("@parImpresaReferente", FormatClauseIn(filtriAziende.ImpreseReferenti));

                _configuratore.joinImpreseReferenti = true;
                _configuratore.ApplicaFiltroImpreseReferenti = true;
            }

            // PIVA
            if (filtriAziende.Piva != null && filtriAziende.Piva.Trim() != "")
            {
                //stbFiltri.FiltroAziendeBase.AppendLine(" AND Imprese.Piva LIKE @Piva ");
                stbFiltri.FiltroAziendeBase.AppendLine(" AND CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END LIKE @Piva ");
                parSql.Add("@Piva", $"%{filtriAziende.Piva.Trim()}%");
            }

            //Tipo impresa gerarchia
            if (filtriAziende.TipiImpresa is not null && filtriAziende.TipiImpresa.Any())
            {
                stbFiltri.FiltroAziendeBase.AppendLine(" AND Imprese.TipoImpresaGerarchia IN (@parTipiImpresa) ");
                parSqlIn.Add("@parTipiImpresa", FormatClauseIn(filtriAziende.TipiImpresa));
            }

            // FOGLIA GERARCHIA (null=nessun filtro; false=nodi Foglia=0; true=foglie Foglia=1)
            if (filtriAziende.EstraiSoloFigli.HasValue)
            {
                stbFiltri.FiltroAziendeBase.AppendLine(" AND GI_base.Foglia = @Foglia ");
                parSql.Add("@Foglia", filtriAziende.EstraiSoloFigli.Value ? 1 : 0);
                _configuratore.joinGerarchiaBase = true;
            }

            // RAGIONE SOCIALE
            if (filtriAziende.RagioneSociale != null && filtriAziende.RagioneSociale.Trim() != "")
            {
                stbFiltri.FiltroAziendeBase.AppendLine(" AND Imprese.Rag_Soc LIKE @RagioneSociale ");
                parSql.Add("@RagioneSociale", $"%{filtriAziende.RagioneSociale.Trim()}%");
            }

            // CUAA
            if (filtriAziende.CUAA != null && filtriAziende.CUAA.Trim() != "")
            {
                stbFiltri.FiltroAziendeBase.AppendLine(" AND CUAA.Val_Cod LIKE @CUAA ");
                parSql.Add("@CUAA", $"%{filtriAziende.CUAA.Trim()}%");
            }

            // ZONE
            if (filtriAziende.Zone != null && filtriAziende.Zone.Any())
            {
                int Num_Sel = filtriAziende.Zone.Count;

                stbFiltri.FiltroAziendeBase.AppendLine(" AND Imprese.Piva IN (SELECT DISTINCT IP.PIVA ");
                stbFiltri.FiltroAziendeBase.AppendLine("                      FROM  ImpreseXParticelle IP ");
                stbFiltri.FiltroAziendeBase.AppendLine("                      WHERE EXISTS ");
                stbFiltri.FiltroAziendeBase.AppendLine("                          (SELECT PC.PROV, PC.COM, PC.SEZIONE, PC.FOGLIO, PC.NUMERO, PC.SUBALTERNO ");
                stbFiltri.FiltroAziendeBase.AppendLine("                           FROM ZonexParticelle ");
                stbFiltri.FiltroAziendeBase.AppendLine("                           JOIN ParticelleCatastali PC ON ");
                stbFiltri.FiltroAziendeBase.AppendLine("                               ZonexParticelle.PROV = PC.PROV AND ZonexParticelle.COM = PC.COM ");
                stbFiltri.FiltroAziendeBase.AppendLine("                           AND ZonexParticelle.SEZIONE = PC.SEZIONE AND ZonexParticelle.FOGLIO = PC.FOGLIO AND ZonexParticelle.NUMERO = PC.NUMERO ");
                stbFiltri.FiltroAziendeBase.AppendLine("                           AND ZonexParticelle.SUBALTERNO = PC.SUBALTERNO ");
                stbFiltri.FiltroAziendeBase.AppendLine("                           WHERE ZonexParticelle.Zona_Cod IN (@parZone) AND (IP.PROV = PC.PROV AND IP.COM = PC.COM AND IP.SEZIONE = PC.SEZIONE ");
                stbFiltri.FiltroAziendeBase.AppendLine("                           AND IP.FOGLIO = PC.FOGLIO AND IP.NUMERO = PC.NUMERO AND IP.SUBALTERNO = PC.SUBALTERNO) ");
                stbFiltri.FiltroAziendeBase.AppendLine("                           GROUP BY PC.PROV, PC.COM, PC.SEZIONE, PC.FOGLIO, PC.NUMERO, PC.SUBALTERNO ");

                if (filtriAziende.OperatoreLogicoZone == (int)Enum_FiltroOperatoreLogico_FiltroRicerca.AND_TutteLeCondizioniTrue)
                {
                    stbFiltri.FiltroAziendeBase.AppendLine("                          HAVING COUNT(*) = @countZone ");
                    parSql.Add("@countZone", Num_Sel.ToString());
                }

                stbFiltri.FiltroAziendeBase.AppendLine("                          )");
                stbFiltri.FiltroAziendeBase.AppendLine("                      )");

                parSqlIn.Add("@parZone", FormatClauseIn(filtriAziende.Zone));
            }

            stbFiltri.FiltroAziendeBase.AppendLine(CreaFiltroStatiRegioniProvinceComuni(filtriAziende.Stati, filtriAziende.Regioni, filtriAziende.Province, filtriAziende.Comuni, parSql, parSqlIn, (int)Enum_Entita_FiltroRicerca.Azienda).ToString());

            #region "Controllo rimosso. Save for later?"
            //// CODICE AZIENDA
            //if (filtriAziende.CodiceAzienda != null)
            //{
            //    if (filtriAziende.CodiceAzienda.codice != 0 && filtriAziende.CodiceAzienda.valore.Trim() != "")
            //    {
            //        var idJoin = pAziende + filtriAziende.CodiceAzienda.codice;
            //        stbFiltri.FiltroAziendeDettagli.AppendLine("   AND " + idJoin + ".Val_Cod LIKE @valoreCodiceImpresa ");
            //        parSql.Add("@valoreCodiceImpresa", "%" + filtriAziende.CodiceAzienda.valore.Trim() + "%");

            //        _configuratore.AziendeDettagli = true;
            //        _configuratore.filtroImpreseDettagli = true;
            //    }
            //}
            #endregion
        }
        private void AggiungiFiltriCentriAziendali(FiltriCentriAziendali filtriCentriAziendali, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn, StbFiltri stbFiltri)
        {
            if (filtriCentriAziendali == null) return;

            // NOME CENTRO --OK
            if (filtriCentriAziendali.CentroAziendale != null && filtriCentriAziendali.CentroAziendale.Trim() != "")
            {
                stbFiltri.FiltroCentriAziendaliBase.AppendLine(" AND Centri_Aziendali.Sa_Nome LIKE @Sa_Nome ");
                parSql.Add("@Sa_Nome", $"%{filtriCentriAziendali.CentroAziendale.Trim()}%");

                _configuratore.joinCentro = true;
            }

            stbFiltri.FiltroCentriAziendaliBase.AppendLine(CreaFiltroStatiRegioniProvinceComuni(filtriCentriAziendali.Stati, filtriCentriAziendali.Regioni, filtriCentriAziendali.Province, filtriCentriAziendali.Comuni, parSql, parSqlIn, (int)Enum_Entita_FiltroRicerca.CentroAziendale).ToString());

            #region "Controllo rimosso. Save for later?"
            //// CODICE CENTRO
            //if (filtriCentriAziendali.CodiceCentro != null)
            //{
            //    if (filtriCentriAziendali.CodiceCentro.codice != 0 && filtriCentriAziendali.CodiceCentro.valore.Trim() != "")
            //    {
            //        var idJoin = prefix_CentriAziendaliCodici + filtriCentriAziendali.CodiceCentro.codice;
            //        stbFiltri.FiltroCentriAziendaliDettagli.AppendLine("   AND " + idJoin + ".Val_Cod LIKE @valoreCodiceCentro ");
            //        parSql.Add("@valoreCodiceCentro", "%" + filtriCentriAziendali.CodiceCentro.valore.Trim() + "%");

            //        _configuratore.CentriAziendaliBase = true;
            //        _configuratore.CentriAziendaliDettagli = true;
            //        _configuratore.filtroCentriAziendaliDettagli = true;
            //    }
            //}
            #endregion
        }
        private void AggiungiFiltriPianoColturale(FiltriPianoColturale filtriPianoColturale, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn, StbFiltri stbFiltri)
        {
            if (filtriPianoColturale == null) return;

            // METODO PRODUZIONE                 
            if (filtriPianoColturale.UtilizzoTerreno != null && filtriPianoColturale.UtilizzoTerreno.Any())
            {
                var idJoin = Enum_PrefixCodici.Appezzamenti + (int)Enum_CodiciAnagrafe.MetodoDiProduzione;

                stbFiltri.FiltroAppezzamentiBase.AppendLine(" AND UtilizzoTerreno.Val_Cod IN (@parUtilizzoTerreno) ");

                parSqlIn.Add("@parUtilizzoTerreno", FormatClauseIn(filtriPianoColturale.UtilizzoTerreno));

                _configuratore.joinAppezzamento = true;
            }

            string strFiltroColturale = "";

            // GRUPPO VEGETALE
            if (filtriPianoColturale.GruppoVegetale != null && filtriPianoColturale.GruppoVegetale.Any())
            {
                strFiltroColturale += " GruppoVegetale.Gru_Cod IN (@parGruppoVegetale) AND ";
                parSqlIn.Add("@parGruppoVegetale", FormatClauseIn(filtriPianoColturale.GruppoVegetale));
            }

            // SPECIE
            if (filtriPianoColturale.Specie != null && filtriPianoColturale.Specie.Any())
            {
                strFiltroColturale += " SpecieVegetali.Veg_Cod IN (@parSpecieImpianto) AND ";
                parSqlIn.Add("@parSpecieImpianto", FormatClauseIn(filtriPianoColturale.Specie));
            }

            // TIPOLOGIA VARIETALE
            if (filtriPianoColturale.TipologiaVarietale != null && filtriPianoColturale.TipologiaVarietale.Any())
            {
                strFiltroColturale += " Reg_Impianti.GRVA_Cod_VEG IN (@parTipologiaVarietale) AND ";
                parSqlIn.Add("@parTipologiaVarietale", FormatClauseIn(filtriPianoColturale.TipologiaVarietale));
            }

            // VARIETA'
            if (filtriPianoColturale.Varieta != null && filtriPianoColturale.Varieta.Any())
            {
                strFiltroColturale += " Reg_Impianti.Cul_Cod IN (@parVarieta) AND ";
                parSqlIn.Add("@parVarieta", FormatClauseIn(filtriPianoColturale.Varieta));
            }

            // tolgo l'ultimo and
            if (strFiltroColturale != "")
                strFiltroColturale = Strings.Left(strFiltroColturale, strFiltroColturale.Length - 4);


            _configuratore.IncludiEscludiDestinazioniUso = filtriPianoColturale.FiltroDestinazioneUso;

            switch (filtriPianoColturale.FiltroDestinazioneUso)
            {
                case (int)Enum_FiltroDestinazioneUso_FiltroRicerca.Tutto:
                    if (strFiltroColturale != "" && filtriPianoColturale.DestinazioniUso.Any())
                    {
                        //Sono state selezionate specie/gruppi vegetali + destinazioni
                        strFiltroColturale = $"(({strFiltroColturale}) OR (DestinazioniUso_TT.codice IN (@parDestinazioniUso)))";
                        parSqlIn.Add("@parDestinazioniUso", FormatClauseIn(filtriPianoColturale.DestinazioniUso));
                    }
                    else if (strFiltroColturale == "" && filtriPianoColturale.DestinazioniUso.Any())
                    {
                        //Tutte le specie e le destinazioni selezionate
                        strFiltroColturale = " ((Reg_Impianti.Cul_Cod <> 0) AND " +
                                             $" (GruppoVegetale.Gru_Cod IN ({(int)enum_GruppoVegetale.Arboree}, {(int)enum_GruppoVegetale.Erbacee}, {(int)enum_GruppoVegetale.OrtoFloroVivaismo}))) " +
                                             " OR DestinazioniUso_TT.codice IN (@parDestinazioniUso) ";
                        parSqlIn.Add("@parDestinazioniUso", FormatClauseIn(filtriPianoColturale.DestinazioniUso));
                    }
                    else if (strFiltroColturale != "" && filtriPianoColturale.DestinazioniUso.Count == 0)
                    {
                        //Tutte le destinazioni e le specie/gruppivegetali selezionate
                        strFiltroColturale = " ((Reg_Impianti.Cul_Cod = 0) OR (" + strFiltroColturale + ")) ";
                    }
                    break;
                case (int)Enum_FiltroDestinazioneUso_FiltroRicerca.SoloDestinazioniUso:
                    strFiltroColturale = " Reg_Impianti.Cul_Cod = 0 ";

                    if (filtriPianoColturale.DestinazioniUso != null && filtriPianoColturale.DestinazioniUso.Any())
                    {
                        stbFiltri.FiltroDestinazioniUso.AppendLine("   AND DestinazioniUso.codice IN (@parDestinazioniUso) ");
                        parSqlIn.Add("@parDestinazioniUso", FormatClauseIn(filtriPianoColturale.DestinazioniUso));
                    }
                    break;
                case (int)Enum_FiltroDestinazioneUso_FiltroRicerca.EscludiDestinazioniUso:
                    if (strFiltroColturale == "")
                        strFiltroColturale = " ((Reg_Impianti.Cul_Cod <> 0) AND (GruppoVegetale.Gru_Cod IN (" + (int)enum_GruppoVegetale.Arboree + "," + (int)enum_GruppoVegetale.Erbacee + "," + (int)enum_GruppoVegetale.OrtoFloroVivaismo + "))) ";
                    else
                        strFiltroColturale = " ((Reg_Impianti.Cul_Cod <> 0) AND (" + strFiltroColturale + ")) ";
                    break;
            }

            if (strFiltroColturale != "")
            {
                stbFiltri.FiltroImpiantiBase.AppendLine(" AND " + strFiltroColturale);

                _configuratore.joinImpianto = true;
            }

            // LOTTO
            if (filtriPianoColturale.Lotto != null && filtriPianoColturale.Lotto.Trim() != "")
            {
                stbFiltri.FiltroEserciziBase.AppendLine(" AND Imprese_Progetti.Progetto_Nome LIKE @Lotto");
                parSql.Add("@Lotto", $"%{filtriPianoColturale.Lotto.Trim()}%");

                _configuratore.joinEsercizio = true;
            }

            // PROGETTO
            if (filtriPianoColturale.Progetto != null && filtriPianoColturale.Progetto.Trim() != "")
            {
                stbFiltri.FiltroEserciziBase.AppendLine(" AND Imprese_Progetti.Progetto_Des LIKE @Progetto");
                parSql.Add("@Progetto", $"%{filtriPianoColturale.Progetto.Trim()}%");

                _configuratore.joinEsercizio = true;
            }

            // Contributi ACA
            if (filtriPianoColturale.ContributiACA != null && filtriPianoColturale.ContributiACA.Any())
            {
                stbFiltri.FiltroContributiACA.AppendLine(" AND Imprese_ProgettiXContributi.ContributoCod IN (@parContributiACA) ");

                parSqlIn.Add("@parContributiACA", FormatClauseIn(filtriPianoColturale.ContributiACA));

                _configuratore.joinContributiACA = true;
                _configuratore.ApplicaFiltroContributiACA = true;
            }
        }
        private void AggiungiFiltriCampi(FiltriCampi filtriCampi, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn, StbFiltri stbFiltri)
        {
            if (filtriCampi == null) return;

            // SPECIE
            if (filtriCampi.Specie != null && filtriCampi.Specie.Any())
            {
                stbFiltri.FiltroCampiBase.AppendLine(" AND (Campi.Veg_Cod = 0 OR Campi.Veg_Cod IN (@parSpecieCampo)) ");
                parSqlIn.Add("@parSpecieCampo", FormatClauseIn(filtriCampi.Specie));
            }
        }
        private void AggiungiFiltriTemporali(FiltriTemporali filtriTemporali, FiltriMovimenti filtriMovimenti, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn, StbFiltri stbFiltri)
        {

            bool OuterFilter = false;

            if (filtriTemporali != null && filtriTemporali.FiltriData != null && filtriTemporali.FiltriData.Any())
            {

                //sono stati specificati N filtri in OR, su almeno due entità diverse. Sono costretta a spostare la clausola WHERE fuori da ogni TT
                OuterFilter = filtriTemporali.FiltriData.Select(x => x.Entita).Distinct().ToList().Count > 1 && filtriTemporali.OperatoreLogicoFiltriTemporali == (int)Enum_FiltroOperatoreLogico_FiltroRicerca.OR_AlmenoUnaCondizioneTrue;

                if (!OuterFilter)
                {
                    // 06/12 Ora le singole tabelle base vengono filtrate a scalare con il livello precedente (filtrato a sua volta): ex, Esercizi filtrato da Impianti filtrato da Appezzamenti...
                    // Propago i filtri impostati a scalare per avere set di dati sempre minori
                    // Posso farlo solo in caso di AND_TutteLeCondizioniTrue --> così ogni condizione va nella sua specifica CTE
                    List<FiltroTemporale> propagatore = new();
                    foreach (FiltroTemporale filtro in filtriTemporali.FiltriData)
                    {
                        // TryAdattaFiltroPerPropagazione esclude DataCreazioneAnagrafica, CompresoFra e IntervalloValidita
                        // e adatta la colonna per rispettare la gerarchia di validità (figlio ⊆ genitore)
                        if (!TryAdattaFiltroPerPropagazione(filtro.ColonnaData, filtro.TipoConfronto,
                                out int colonnaDataPropagata, out int tipoConfrontoPropagato))
                            continue;

                        switch (filtro.Entita)
                        {
                            case (int)Enum_Entita_FiltroRicerca.Esercizio:
                                //Propago questi filtri a tutte le entità precedenti all'esercizio
                                FiltroTemporale dummy1 = new()
                                {
                                    Entita = (int)Enum_Entita_FiltroRicerca.Impianto,
                                    ColonnaData = colonnaDataPropagata,
                                    TipoConfronto = tipoConfrontoPropagato,
                                    ModalitaFiltroData = filtro.ModalitaFiltroData,
                                    Date = filtro.Date
                                };
                                propagatore.Add(dummy1);

                                FiltroTemporale dummy2 = new()
                                {
                                    Entita = (int)Enum_Entita_FiltroRicerca.Appezzamento,
                                    ColonnaData = colonnaDataPropagata,
                                    TipoConfronto = tipoConfrontoPropagato,
                                    ModalitaFiltroData = filtro.ModalitaFiltroData,
                                    Date = filtro.Date
                                };
                                propagatore.Add(dummy2);
                                break;
                            case (int)Enum_Entita_FiltroRicerca.Impianto:
                                //Propago questi filtri a tutte le entità precedenti
                                FiltroTemporale dummy3 = new()
                                {
                                    Entita = (int)Enum_Entita_FiltroRicerca.Appezzamento,
                                    ColonnaData = colonnaDataPropagata,
                                    TipoConfronto = tipoConfrontoPropagato,
                                    ModalitaFiltroData = filtro.ModalitaFiltroData,
                                    Date = filtro.Date
                                };
                                propagatore.Add(dummy3);
                                break;
                        }
                    }
                    filtriTemporali.FiltriData.AddRange(propagatore);
                }
            }

            //AF 12/25: Propagazione dei filtri temporali dei movimenti sulle date degli impianti
            //IMPORTANTE: La propagazione viene effettuata solo quando:
            // 1. La ricerca è con operazioni (FiltroOperazioni == ConOperazioni)
            // 2. È stato specificato un intervallo di date per i movimenti (DataMovimento.inizio != AGRODATAINIZIO E DataMovimento.fine != AGRODATAFINE)
            //Scenario NON supportato con la propagazione:
            // - Ricerca di aziende SENZA movimenti dal 2030 in poi
            //   In questo caso, filtrando sulle date degli impianti/esercizi, se non esistono entità valide alla data specificata, 
            //   l'azienda non verrebbe mostrata nel risultato, anche se effettivamente non ha movimenti futuri.
            //   Per evitare questa casistica errata, la propagazione avviene SOLO quando si ricercano entità CON operazioni.
            if (!OuterFilter && filtriMovimenti != null && filtriMovimenti.DataMovimento != null && filtriMovimenti.FiltroOperazioni == (int)Enum_FiltroOperazioniAgenda_FiltroRicerca.ConOperazioni)
            {
                IntervalloTemporale filtroData = filtriMovimenti.DataMovimento;
                if (filtroData.inizio != DateTime.Parse(AGRODATAINIZIO) && filtroData.fine != DateTime.Parse(AGRODATAFINE))
                {
                    List<FiltroTemporale> propagatore = new();

                    //Propago questi filtri a tutte le entità 
                    FiltroTemporale dummy1 = new()
                    {
                        Entita = (int)Enum_Entita_FiltroRicerca.Esercizio,
                        ColonnaData = (int)Enum_ColonnaData_FiltroRicerca.EsercizioFiltroMovimenti,
                        TipoConfronto = (int)Enum_TipoConfronto_FiltroRicerca.EsercizioFiltroMovimenti,
                        Date = filtroData
                    };
                    propagatore.Add(dummy1);

                    FiltroTemporale dummy2 = new()
                    {
                        Entita = (int)Enum_Entita_FiltroRicerca.Impianto,
                        ColonnaData = (int)Enum_ColonnaData_FiltroRicerca.EsercizioFiltroMovimenti,
                        TipoConfronto = (int)Enum_TipoConfronto_FiltroRicerca.EsercizioFiltroMovimenti,
                        Date = filtroData
                    };
                    propagatore.Add(dummy2);

                    FiltroTemporale dummy3 = new()
                    {
                        Entita = (int)Enum_Entita_FiltroRicerca.Appezzamento,
                        ColonnaData = (int)Enum_ColonnaData_FiltroRicerca.EsercizioFiltroMovimenti,
                        TipoConfronto = (int)Enum_TipoConfronto_FiltroRicerca.EsercizioFiltroMovimenti,
                        Date = filtroData
                    };
                    propagatore.Add(dummy3);

                    if (filtriTemporali == null)
                    {
                        filtriTemporali = new FiltriTemporali();
                        filtriTemporali.FiltriData = new List<FiltroTemporale>();
                    }
                    else filtriTemporali.FiltriData ??= new List<FiltroTemporale>();

                    filtriTemporali.FiltriData.AddRange(propagatore);
                }
            }

            if (filtriTemporali != null && filtriTemporali.FiltriData != null)
            {
                List<string> listafiltriTemporaliAppezzamento = new();
                List<string> listafiltriTemporaliImpianto = new();
                List<string> listafiltriTemporaliEsercizio = new();

                List<string> listafiltriTemporaliOuter = new();

                int rowIndex = 1;

                foreach (FiltroTemporale filtro in filtriTemporali.FiltriData)
                {
                    if ((filtro.Entita == -1) || (filtro.TipoConfronto == -1) || (filtro.ColonnaData == -1))
                    {
                        // la riga corrente ha dati mancanti, 
                        // senza entità
                        // senza colonna data
                        // senza tipo controllo
                        continue;
                    }

                    switch (filtro.TipoConfronto)
                    {
                        // la riga corrente ha date non coerenti
                        case (int)Enum_TipoConfronto_FiltroRicerca.Maggiore:
                        case (int)Enum_TipoConfronto_FiltroRicerca.Minore:
                        case (int)Enum_TipoConfronto_FiltroRicerca.MaggioreUguale:
                        case (int)Enum_TipoConfronto_FiltroRicerca.MinoreUguale:
                            if (filtro.Date.inizio == DateTime.Parse(AGRODATAFINE) ||
                                filtro.Date.inizio == DateTime.Parse(AGRODATAINIZIO))
                            {
                                continue;
                            }
                            break;
                        case (int)Enum_TipoConfronto_FiltroRicerca.CompresoFra:
                        case (int)Enum_TipoConfronto_FiltroRicerca.EsercizioFiltroMovimenti:
                            if ((filtro.Date.inizio == DateTime.Parse(AGRODATAINIZIO) && filtro.Date.fine == DateTime.Parse(AGRODATAFINE)) ||
                                (filtro.Date.inizio == DateTime.Parse(AGRODATAFINE) && filtro.Date.fine == DateTime.Parse(AGRODATAINIZIO)))
                            {
                                continue;
                            }
                            break;
                    }

                    string Entita = "";
                    string ColonnaData = "";
                    string Criterio = "";

                    DateTime Data1_Inizio = filtro.Date.inizio;
                    DateTime Data2_Fine = filtro.Date.fine;

                    bool skip = false;

                    string strFiltro = "";

                    string Parametro = "@Row" + rowIndex + "_" + ((Enum_Entita_FiltroRicerca)filtro.Entita).ToString() + "_" +
                                                                 ((Enum_ColonnaData_FiltroRicerca)filtro.ColonnaData).ToString() + "_" +
                                                                 ((Enum_TipoConfronto_FiltroRicerca)filtro.TipoConfronto).ToString();

                    switch (filtro.Entita)
                    {
                        case (int)Enum_Entita_FiltroRicerca.Appezzamento:
                            if (OuterFilter)
                                Entita = "AppezzamentiBase_TT";
                            else
                                Entita = Enum_Tabelle.Appezzamenti;

                            _configuratore.joinAppezzamento = true;
                            break;
                        case (int)Enum_Entita_FiltroRicerca.Impianto:
                            if (OuterFilter)
                                Entita = "ImpiantiBase_TT";
                            else
                                Entita = Enum_Tabelle.Impianti;
                            _configuratore.joinImpianto = true;
                            break;
                        case (int)Enum_Entita_FiltroRicerca.Esercizio:
                            if (OuterFilter)
                                Entita = "EserciziBase_TT";
                            else
                                Entita = Enum_Tabelle.Esercizi;
                            _configuratore.joinEsercizio = true;
                            break;
                    }

                    switch (filtro.ColonnaData)
                    {
                        case (int)Enum_ColonnaData_FiltroRicerca.ValiditaInizio:
                            ColonnaData = Entita + ".Validita_Inizio";
                            break;
                        case (int)Enum_ColonnaData_FiltroRicerca.ValiditaFine:
                            ColonnaData = Entita + ".Validita_Fine";
                            break;
                        case (int)Enum_ColonnaData_FiltroRicerca.DataCreazioneAnagrafica:
                            ColonnaData = "CONVERT(Date," + Entita + ".Data_Creazione, 120)";
                            break;
                        case (int)Enum_ColonnaData_FiltroRicerca.IntervalloValidita:
                            string paramData1 = Parametro + "_Data1_" + rowIndex.ToString();
                            string paramData2 = Parametro + "_Data2_" + rowIndex.ToString();

                            strFiltro = "(" + Entita + ".Validita_Inizio" + Enum_FiltroDate.MaggioreUguale + paramData1 + " AND " +
                                              Entita + ".Validita_Fine" + Enum_FiltroDate.MinoreUguale + paramData2 + ")";

                            //listaFiltri.Add(str);
                            parSql.Add(paramData1, Data1_Inizio);
                            parSql.Add(paramData2, Data2_Fine);

                            skip = true;
                            break;
                        case (int)Enum_ColonnaData_FiltroRicerca.EsercizioFiltroMovimenti:
                            string paramDataFiltroMovimentoDAL = Parametro + "_DataFiltroMovimentoDAL_" + rowIndex.ToString();
                            string paramDataFiltroMovimentoAL = Parametro + "_DataFiltroMovimentoAL_" + rowIndex.ToString();

                            strFiltro = "(" + Entita + ".Validita_Inizio" + Enum_FiltroDate.MinoreUguale + paramDataFiltroMovimentoAL + " AND " +
                                              Entita + ".Validita_Fine" + Enum_FiltroDate.MaggioreUguale + paramDataFiltroMovimentoDAL + ")";

                            //listaFiltri.Add(str);
                            parSql.Add(paramDataFiltroMovimentoDAL, Data1_Inizio);
                            parSql.Add(paramDataFiltroMovimentoAL, Data2_Fine);

                            skip = true;
                            break;
                    }

                    if (!skip)
                    {
                        switch (filtro.TipoConfronto)
                        {
                            case (int)Enum_TipoConfronto_FiltroRicerca.Maggiore:
                                Criterio = Enum_FiltroDate.Maggiore;
                                break;
                            case (int)Enum_TipoConfronto_FiltroRicerca.Minore:
                                Criterio = Enum_FiltroDate.Minore;
                                break;
                            case (int)Enum_TipoConfronto_FiltroRicerca.MaggioreUguale:
                                Criterio = Enum_FiltroDate.MaggioreUguale;
                                break;
                            case (int)Enum_TipoConfronto_FiltroRicerca.MinoreUguale:
                                Criterio = Enum_FiltroDate.MinoreUguale;
                                break;
                            case (int)Enum_TipoConfronto_FiltroRicerca.CompresoFra:
                                string paramData1 = Parametro + "_Data1_" + rowIndex.ToString();
                                string paramData2 = Parametro + "_Data2_" + rowIndex.ToString();

                                strFiltro = "(" + ColonnaData + Enum_FiltroDate.MaggioreUguale + paramData1 + " AND " +
                                                  ColonnaData + Enum_FiltroDate.MinoreUguale + paramData2 + ")";

                                //listaFiltri.Add(str);
                                parSql.Add(paramData1, Data1_Inizio);
                                parSql.Add(paramData2, Data2_Fine);

                                skip = true;
                                break;
                        }

                        if (!skip)
                        {
                            //Parametro += "_" + rowIndex.ToString();

                            strFiltro = ColonnaData + Criterio + Parametro;
                            //listaFiltri.Add(ColonnaData + Criterio + parametro);
                            parSql.Add(Parametro, Data1_Inizio);
                        }
                    }

                    if (!OuterFilter)
                    {
                        switch (filtro.Entita)
                        {
                            case (int)Enum_Entita_FiltroRicerca.Appezzamento:
                                listafiltriTemporaliAppezzamento.Add(strFiltro);
                                break;
                            case (int)Enum_Entita_FiltroRicerca.Impianto:
                                listafiltriTemporaliImpianto.Add(strFiltro);
                                break;
                            case (int)Enum_Entita_FiltroRicerca.Esercizio:
                                listafiltriTemporaliEsercizio.Add(strFiltro);
                                break;
                        }
                    }
                    else
                    {
                        listafiltriTemporaliOuter.Add(strFiltro);
                    }

                    rowIndex++;
                }

                string OperatoreLogico = filtriTemporali.OperatoreLogicoFiltriTemporali == (int)Enum_FiltroOperatoreLogico_FiltroRicerca.AND_TutteLeCondizioniTrue ? " AND " : " OR ";

                if (!OuterFilter)
                {
                    //Di queste clausole solo una è vera per volta
                    if (listafiltriTemporaliAppezzamento.Any())
                        stbFiltri.FiltroAppezzamentiBase.AppendLine($" AND ({string.Join(OperatoreLogico, listafiltriTemporaliAppezzamento)})");
                    if (listafiltriTemporaliImpianto.Any())
                        stbFiltri.FiltroImpiantiBase.AppendLine($" AND ({string.Join(OperatoreLogico, listafiltriTemporaliImpianto)})");
                    if (listafiltriTemporaliEsercizio.Any())
                        stbFiltri.FiltroEserciziBase.AppendLine($" AND ({string.Join(OperatoreLogico, listafiltriTemporaliEsercizio)})");
                }
                else
                {
                    if (listafiltriTemporaliOuter.Any())
                        stbFiltri.FiltroOuter.AppendLine($" AND ({string.Join(OperatoreLogico, listafiltriTemporaliOuter)})");
                }
            }
        }

        private void AggiungiFiltriServizi(FiltriServizi filtriServizi, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn, StbFiltri stbFiltri)
        {
            if (filtriServizi == null) return;
            // SERVIZIO
            if (filtriServizi.Servizio > 0)
            {
                stbFiltri.FiltroServizi.AppendLine(" AND Pratiche.Servizio_Cod = @Servizio ");
                parSql.Add("@Servizio", filtriServizi.Servizio);

                _configuratore.joinServizio = true;
                _configuratore.ApplicaFiltroServizi = true;
            }

            // STATO PRATICA
            if (filtriServizi.StatiPratica != null && filtriServizi.StatiPratica.Any())
            {
                stbFiltri.FiltroServizi.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod IN (@parStatiPratica) ");
                parSqlIn.Add("@parStatiPratica", FormatClauseIn(filtriServizi.StatiPratica));

                _configuratore.joinServizio = true;
                _configuratore.ApplicaFiltroServizi = true;
            }

            // AL
            if (filtriServizi.Data != DateTime.Parse(AGRODATAINIZIO))
            {
                stbFiltri.FiltroServizi.AppendLine($" AND Pratiche_Stati_Attuali.Validita_Inizio {Enum_FiltroDate.MinoreUguale} @StatoServizioAL ");
                stbFiltri.FiltroServizi.AppendLine($" AND Pratiche_Stati_Attuali.Validita_Fine {Enum_FiltroDate.MaggioreUguale} @StatoServizioAL ");

                parSql.Add("@StatoServizioAL", filtriServizi.Data);

                _configuratore.joinServizio = true;
                _configuratore.ApplicaFiltroServizi = true;
            }
        }
        private void AggiungiFiltriMovimenti(FiltriMovimenti filtriMovimenti, CriteriRicercaExtended criteriRicerca, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn, StbFiltri stbFiltri)
        {
            if (filtriMovimenti == null) return;

            bool almenoUnaCondizine = filtriMovimenti.FiltroOperazioni != (int)Enum_FiltroOperazioniAgenda_FiltroRicerca.Tutto;

            //Hardening BE --> se siamo in 'Mostra Movimenti' l'unica modalità possibile è 'Con Operazioni'
            if (_configuratore.isMostraMovimenti) filtriMovimenti.FiltroOperazioni = (int)Enum_FiltroOperazioniAgenda_FiltroRicerca.ConOperazioni;

            if (filtriMovimenti.FiltroOperazioni > 0)
            {
                string FiltroGruppoOperazioni = "";
                string FiltroOperazioni = "";

                if (filtriMovimenti.GruppoOperazioni != null && filtriMovimenti.GruppoOperazioni.Any())
                {
                    FiltroGruppoOperazioni = " (@parGruppoOperazioni) ";
                    parSqlIn.Add("@parGruppoOperazioni", FormatClauseIn(filtriMovimenti.GruppoOperazioni));

                    almenoUnaCondizine = true;
                }

                if (filtriMovimenti.Operazioni != null && filtriMovimenti.Operazioni.Any())
                {
                    FiltroOperazioni = " (@parOperazioni) ";
                    parSqlIn.Add("@parOperazioni", FormatClauseIn(filtriMovimenti.Operazioni));

                    almenoUnaCondizine = true;
                }

                // DATE MOVIMENTI
                if (filtriMovimenti.DataMovimento != null)
                {
                    // MOVIMENTI DAL
                    if (filtriMovimenti.DataMovimento.inizio != DateTime.Parse(AGRODATAINIZIO))
                    {
                        stbFiltri.FiltroMovimenti.AppendLine($" AND Movimenti.Data_Movimento {Enum_FiltroDate.MaggioreUguale} @DataMovimentoDA ");

                        parSql.Add("@DataMovimentoDA", filtriMovimenti.DataMovimento.inizio);

                        almenoUnaCondizine = true;
                    }

                    // MOVIMENTI AL
                    if (filtriMovimenti.DataMovimento.fine != DateTime.Parse(AGRODATAFINE))
                    {
                        stbFiltri.FiltroMovimenti.AppendLine($" AND Movimenti.Data_Movimento {Enum_FiltroDate.MinoreUguale} @DataMovimentoA");

                        parSql.Add("@DataMovimentoA", filtriMovimenti.DataMovimento.fine);

                        almenoUnaCondizine = true;
                    }
                }

                _configuratore.IncludiEscludiOperazioniAgenda = filtriMovimenti.FiltroOperazioni;

                if (filtriMovimenti.FiltroOperazioni == (int)Enum_FiltroOperazioniAgenda_FiltroRicerca.SenzaOperazioni &&
                    criteriRicerca.TipoMostra != (int)Enum_TipoMostra_FiltroRicerca.Movimenti)
                {
                    switch (criteriRicerca.TipoMostra)
                    {
                        case (int)Enum_TipoMostra_FiltroRicerca.Aziende:
                            stbFiltri.FiltroAziendeBase.AppendLine(" AND OperazioniAgenda_TT.[keyPiva10] IS NULL ");
                            break;
                        case (int)Enum_TipoMostra_FiltroRicerca.CentriAziendali:
                            stbFiltri.FiltroCentriAziendaliBase.AppendLine(" AND OperazioniAgenda_TT.[keyPiva10] IS NULL ");
                            stbFiltri.FiltroCentriAziendaliBase.AppendLine(" AND OperazioniAgenda_TT.[keySaCod10] IS NULL ");
                            break;
                        case (int)Enum_TipoMostra_FiltroRicerca.Campi:
                            stbFiltri.FiltroCampiBase.AppendLine(" AND OperazioniAgenda_TT.[keyPiva10] IS NULL ");
                            stbFiltri.FiltroCampiBase.AppendLine(" AND OperazioniAgenda_TT.[keySaCod10] IS NULL ");
                            stbFiltri.FiltroCampiBase.AppendLine(" AND OperazioniAgenda_TT.[keyCampoCod10] IS NULL ");
                            break;
                        case (int)Enum_TipoMostra_FiltroRicerca.Appezzamenti:
                            stbFiltri.FiltroAppezzamentiBase.AppendLine(" AND OperazioniAgenda_TT.[keyPiva10] IS NULL ");
                            stbFiltri.FiltroAppezzamentiBase.AppendLine(" AND OperazioniAgenda_TT.[keySaCod10] IS NULL ");
                            stbFiltri.FiltroAppezzamentiBase.AppendLine(" AND OperazioniAgenda_TT.[keyAppezza10] IS NULL ");
                            break;
                        case (int)Enum_TipoMostra_FiltroRicerca.Impianti:
                            stbFiltri.FiltroImpiantiBase.AppendLine(" AND OperazioniAgenda_TT.[keyPiva10] IS NULL ");
                            stbFiltri.FiltroImpiantiBase.AppendLine(" AND OperazioniAgenda_TT.[keySaCod10] IS NULL ");
                            stbFiltri.FiltroImpiantiBase.AppendLine(" AND OperazioniAgenda_TT.[keyAppezza10] IS NULL ");
                            stbFiltri.FiltroImpiantiBase.AppendLine(" AND OperazioniAgenda_TT.[keyIdReg10] IS NULL ");
                            break;
                        case (int)Enum_TipoMostra_FiltroRicerca.Esercizi:
                        case (int)Enum_TipoMostra_FiltroRicerca.PianoColturale:
                            stbFiltri.FiltroEserciziBase.AppendLine(" AND OperazioniAgenda_TT.[keyPiva10] IS NULL ");
                            stbFiltri.FiltroEserciziBase.AppendLine(" AND OperazioniAgenda_TT.[keySaCod10] IS NULL ");
                            stbFiltri.FiltroEserciziBase.AppendLine(" AND OperazioniAgenda_TT.[keyAppezza10] IS NULL ");
                            stbFiltri.FiltroEserciziBase.AppendLine(" AND OperazioniAgenda_TT.[keyIdReg10] IS NULL ");
                            stbFiltri.FiltroEserciziBase.AppendLine(" AND OperazioniAgenda_TT.[keyProgettoCod10] IS NULL ");
                            break;
                    }
                }

                if (FiltroOperazioni != "")
                    stbFiltri.FiltroAgenda.AppendLine($" AND Agenda.Lav_Cod IN {FiltroOperazioni} ");
                if (FiltroGruppoOperazioni != "")
                    stbFiltri.FiltroAgenda.AppendLine($" AND GruppoOperazioni.Gru_Cod IN {FiltroGruppoOperazioni} ");

                if (almenoUnaCondizine)
                {
                    if (_configuratore.isMostraMovimenti)
                        _configuratore.joinMovimento = _configuratore.isMostraMovimenti;
                    else
                    {
                        _configuratore.joinOperazionixAzienda = _configuratore.isMostraAziende;
                        _configuratore.joinOperazionixCentroAziendale = _configuratore.isMostraCentriAziendali;
                        _configuratore.joinOperazionixCampo = _configuratore.isMostraCampi;
                        _configuratore.joinOperazionixAppezzamento = _configuratore.isMostraAppezzamenti;
                        _configuratore.joinOperazionixImpianto = _configuratore.isMostraImpianti;
                        _configuratore.joinOperazionixEsercizio = _configuratore.isMostraEsercizi || _configuratore.isMostraPianoColturale;
                    }
                }
            }
        }
        private void AggiungiFiltriGIS(FiltriGIS filtriGIS, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn, StbFiltri stbFiltri)
        {
            if (filtriGIS == null) return;

            if (filtriGIS.FiltroPoligoni > 0)
            {
                _configuratore.IncludiEscludiPoligoni = filtriGIS.FiltroPoligoni;
                if (filtriGIS.FiltroPoligoni == (int)Enum_FiltroPoligoni_FiltroRicerca.SenzaPoligoni)
                    stbFiltri.FiltroImpiantiBase.AppendLine(" AND ImpiantiGIS_TT.[keyPiva9] IS NULL ");

                _configuratore.joinGIS = true;
            }

            if (filtriGIS.Anomalie != null && filtriGIS.Anomalie.Any())
            {
                stbFiltri.FiltroGISAnomalie.AppendLine(" AND Algoritmo IN (@parGISAnomalie) ");
                parSqlIn.Add("@parGISAnomalie", FormatClauseIn(filtriGIS.Anomalie));

                _configuratore.joinGISAnomalie = true;
            }
        }
        private void AggiungiFiltriCatasto(FiltriCatasto filtriCatasto, Dictionary<string, object> parSql, Dictionary<string, Dictionary<Type, List<object>>> parSqlIn, StbFiltri stbFiltri)
        {
            if (filtriCatasto == null) return;
            if (filtriCatasto.FiltroRipartoCatastale > 0)
            {
                _configuratore.IncludiEscludiRiparto = filtriCatasto.FiltroRipartoCatastale;
                if (filtriCatasto.FiltroRipartoCatastale == (int)Enum_FiltroRipartoCatasto_FiltroRicerca.SenzaRiparto)
                    stbFiltri.FiltroAppezzamentiBase.AppendLine(" AND ParticelleAppezzamenti_TT.[keyPiva1] IS NULL ");

                _configuratore.joinRiparto = true;
            }
        }
        private StringBuilder CreaFiltroStatiRegioniProvinceComuni(List<string> Stati,
                                                           List<string> Regioni,
                                                           List<string> Province,
                                                           List<string> Comuni,
                                                           Dictionary<string, object> parSql,
                                                           Dictionary<string, Dictionary<Type, List<object>>> parSqlIn,
                                                           int entitaDestinazione)
        {

            StringBuilder stbFiltroStatiRegioniProvinceComuni = new();

            string idParametro = "";
            switch (entitaDestinazione)
            {
                case (int)Enum_Entita_FiltroRicerca.Azienda:
                    idParametro = Enum_Entita_FiltroRicerca.Azienda.ToString();
                    break;
                case (int)Enum_Entita_FiltroRicerca.CentroAziendale:
                    idParametro = Enum_Entita_FiltroRicerca.CentroAziendale.ToString();
                    break;
            }

            if (Comuni != null && Comuni.Any())
            {
                List<string> listProvinceComuni = new();
                List<string> listComuni = new();

                foreach (string comune in Comuni)
                {
                    var dummy = comune.Split("_");
                    listProvinceComuni.Add(dummy[0]);
                    listComuni.Add(dummy[1]);
                }

                // Se la stringa non è vuota l'aggiungo al filtro....
                if (listComuni.Any())
                {
                    string idParamProvinceComuni = "@parProvinceComuni" + idParametro;
                    string idParamComuni = "@parComuni" + idParametro;

                    stbFiltroStatiRegioniProvinceComuni.AppendLine($" AND ISTAT.PROV IN ({idParamProvinceComuni}) ");
                    stbFiltroStatiRegioniProvinceComuni.AppendLine($" AND ISTAT.COM IN ({idParamComuni}) ");

                    parSqlIn.Add(idParamProvinceComuni, FormatClauseIn(listProvinceComuni));
                    parSqlIn.Add(idParamComuni, FormatClauseIn(listComuni));

                }
            }
            else if (Province != null && Province.Any())
            {
                string idParamProvincia = "@parProvince" + idParametro;

                stbFiltroStatiRegioniProvinceComuni.AppendLine($" AND Lista_Province.PROV IN ({idParamProvincia}) ");

                parSqlIn.Add(idParamProvincia, FormatClauseIn(Province));
            }
            else if (Regioni != null && Regioni.Any())
            {
                string idParamRegioni = "@parRegioni" + idParametro;

                stbFiltroStatiRegioniProvinceComuni.AppendLine($" AND Lista_Regioni.Reg IN ({idParamRegioni}) ");

                parSqlIn.Add(idParamRegioni, FormatClauseIn(Regioni));
            }
            else if (Stati != null && Stati.Any())
            {
                if (Stati.Contains("IT"))
                    Stati.Add("ITALIA");

                string idParamStati = "@parStati" + idParametro;

                stbFiltroStatiRegioniProvinceComuni.AppendLine($" AND ISTAT.Stato_Country IN ({idParamStati}) ");

                parSqlIn.Add(idParamStati, FormatClauseIn(Stati));
            }

            if (stbFiltroStatiRegioniProvinceComuni.Length > 0)
            {
                switch (entitaDestinazione)
                {
                    case (int)Enum_Entita_FiltroRicerca.Azienda:
                        _configuratore.joinIndirizzoAzienda = true;
                        break;
                    case (int)Enum_Entita_FiltroRicerca.CentroAziendale:
                        _configuratore.joinIndirizzoCentroAziendale = true;
                        _configuratore.joinCentro = true;
                        break;
                }
            }

            return stbFiltroStatiRegioniProvinceComuni;
        }
        #endregion

    }
    public class StbFiltri
    {
        public StringBuilder FiltroGerarchiaImprese { get; set; } = new();

        public StringBuilder FiltroAziendeBase { get; set; } = new();
        public StringBuilder FiltroAziendeDettagli { get; set; } = new();

        public StringBuilder FiltroCentriAziendaliBase { get; set; } = new();
        public StringBuilder FiltroCentriAziendaliDettagli { get; set; } = new();

        public StringBuilder FiltroCampiBase { get; set; } = new();
        public StringBuilder FiltroCampiDettagli { get; set; } = new();

        public StringBuilder FiltroAppezzamentiBase { get; set; } = new();
        public StringBuilder FiltroAppezzamentiDettagli { get; set; } = new();

        public StringBuilder FiltroImpiantiBase { get; set; } = new();
        public StringBuilder FiltroImpiantiDettagli { get; set; } = new();
        public StringBuilder FiltroDestinazioniUso { get; set; } = new();

        public StringBuilder FiltroEserciziBase { get; set; } = new();
        public StringBuilder FiltroEserciziDettagli { get; set; } = new();
        public StringBuilder FiltroContributiACA { get; set; } = new();

        public StringBuilder FiltroFabbricatiBase { get; set; } = new();
        public StringBuilder FiltroFabbricatiDettagli { get; set; } = new();

        public StringBuilder FiltroMovimenti { get; set; } = new();
        public StringBuilder FiltroAgenda { get; set; } = new();
        public StringBuilder FiltroOperazioni { get; set; } = new();

        public StringBuilder FiltroServizi { get; set; } = new();

        public StringBuilder FiltroGISAnomalie { get; set; } = new();

        public StringBuilder FiltroOuter { get; set; } = new();
    }
}