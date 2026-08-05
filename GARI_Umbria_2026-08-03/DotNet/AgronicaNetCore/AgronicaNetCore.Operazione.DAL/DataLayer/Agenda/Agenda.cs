using System.Text;
using InData.Agenda;
using System.Dynamic;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Constants;
using static AgronicaNetCore.Base.Models.AgronicaCoreParametri;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Operazione.DAL.Resources;
using AgronicaDataProvider6.Extensions;
using AgronicaCoreDTOStd.InData.Budget;
using Newtonsoft.Json;
using System.Transactions;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using AgronicaNetCore.Operazione.DAL.DataLayer.RicettexAgenda;
using AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti;
using AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti_Dettagli;
using AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Destinazioni;
using AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti_Zoo;
using AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Dettaglio_Tecnico;
using AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Dettaglio_Tecnico_Extra;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.Info;
using AgronicaCoreModelsSTD.costanti;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreDTOStd.InData.Anagrafica;
using InData.Anagrafica;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.attivita.note_intervento;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using AgronicaCoreModelsSTD.attivita.dettagli;
using static AgronicaCoreModelsSTD.attivita.dettagli.Opzioni_Raccolta;
using AgronicaCoreModelsSTD.metaschema.avversita;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;
using AgronicaCoreModelsSTD.meteo;
using System.Globalization;
using AgronicaCoreDTOStd.OutData.NewAgri;
using AgronicaNetCore.Base.Utility;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Agenda
{
    public class Agenda : BaseDALOperazione, IAgenda
    {
        private readonly IRicettexAgenda _ricettexAgDal;
        private readonly IMovimenti _movimentiDal;
        private readonly IMovimenti_Zoo _movZooDal;
        private readonly IMovimenti_Dettagli _movDettagliDal;
        private readonly IMov_Destinazioni _movDestinazioniDal;
        private readonly IMov_Dettaglio_Tecnico _movDettTecDal;
        private readonly IMov_Dettaglio_Tecnico_Extra _movDettTecExtraDal;

        private readonly IAgro_Sequence _sequenceDal;

    public Agenda(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider,localizer, securityBypass)
        {
            _ricettexAgDal = provider.GetRequiredService<IRicettexAgenda>();
            _movimentiDal = provider.GetRequiredService<IMovimenti>();
            _movZooDal = provider.GetRequiredService<IMovimenti_Zoo>();
            _movDettagliDal = provider.GetRequiredService<IMovimenti_Dettagli>();
            _movDestinazioniDal = provider.GetRequiredService<IMov_Destinazioni>();
            _movDettTecDal = provider.GetRequiredService<IMov_Dettaglio_Tecnico>();
            _movDettTecExtraDal = provider.GetRequiredService<IMov_Dettaglio_Tecnico_Extra>();

            _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
        }   

        private async Task<WriteAgenda> Valorizza(WriteAgenda dtoAgenda)
        {
            DateTime adInizio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
            DateTime adFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);

            dtoAgenda.Sa_Cod ??= 0;
            dtoAgenda.Sta_Num ??= 0;
            dtoAgenda.Linea_Cod ??= 0;
            dtoAgenda.Preparazione_Cod ??= 0;
            dtoAgenda.Id_Trasformazione ??= 0;
            dtoAgenda.Tipo_Accettazione ??= 0;
            dtoAgenda.Audit_Cod ??= 0;
            dtoAgenda.Stato_Export ??= 0;
            dtoAgenda.Stato_Export2 ??= 0;
            dtoAgenda.Tipo_Visibilita ??= 0;
            dtoAgenda.ChkCoge_Manuale ??= 0;
            dtoAgenda.Id_Attivita ??= 0;
            dtoAgenda.Modulo ??= 0;
            dtoAgenda.Raccoglitore_Cod ??= 0;
            dtoAgenda.Split ??= 0;
            dtoAgenda.Pratica_Cod ??= 0;
            dtoAgenda.Origine ??= "";
            dtoAgenda.Stato_Cod ??= 0;
            dtoAgenda.DaRemoto ??= 0;
            
            dtoAgenda.Blocco_Flag ??= 0;
            dtoAgenda.Blocco_Data ??= adInizio;
            dtoAgenda.Blocco_Username ??= "";
            dtoAgenda.Inviato ??= 0;

            if(!dtoAgenda.Validita_Inizio.IsInRange(adInizio, adFine))
                dtoAgenda.Validita_Inizio = adInizio;
            if(!dtoAgenda.Validita_Fine.IsInRange(adInizio, adFine))
                dtoAgenda.Validita_Fine = adFine;

            return dtoAgenda;
        }

        public async Task<RisultatoTabelleAgenda> LeggiTutteLeTabelleDiAgenda(List<int> idAgendas, OpzioniLetturaAgenda opzioniLetturaAgenda, AgronicaCoreParametriTriple parametriTriple)
        {
            var sb = new StringBuilder();
            var dataProvider = GetDataProvider(parametriTriple.ObjParametriServer);
            var parametersSql = new Dictionary<string, object>();

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Id_Agenda, Piva, Sa_Cod, Lav_Cod, Des_Lib, Validita_Inizio, Stato_Cod, DaRemoto, ");
            sb.AppendLine("     Raccoglitore_Cod, Origine, Blocco_Flag, Blocco_Data, Blocco_Username, Id_Attivita ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     Agenda ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     Id_Agenda IN (@idAgendas)");
            sb.AppendLine(" ;");

            sb.AppendLine(" ;");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("   Id_Agenda, Ricetta_Cod, Ricetta_Operazione_Cod ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("   RicettexAgenda WITH(NOLOCK) ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("   Id_Agenda IN (@idAgendas) ");

            sb.AppendLine(" ;");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("    Id_Agenda, Id_Mov, Cau_Mov, Mov_Desc, Data_Movimento, ");
            sb.AppendLine("    Mezzo, Ora, Extra_Int, Modalita, OraFine, Modalita_Applicazione ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("    Movimenti WITH(NOLOCK) ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("    Id_Agenda IN (@idAgendas) ");

            sb.AppendLine(" ;");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("    Id_Agenda, Id_Mov, Id_Mov_Det, Qta_Ril, Av_Cod, Av_Gru, N, P, K, Mg, ");
            sb.AppendLine("    Efficienza, Cu, Ditta_cod, Dose, Freatimetro, Inn1_data, Inn2_data, ");
            sb.AppendLine("    Sigla_AV, Extra_Str, Extra_Int, Extra_Date, dett_cod, Soglia_Cod, ");
            sb.AppendLine("    Soglia_Quantita, Parziale, Nitrati ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("    Mov_Dettaglio_Tecnico WITH(NOLOCK) ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("    Id_Agenda IN (@idAgendas) ");

            sb.AppendLine(" ;");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("    Piva, Id_Agenda, Id_Mov, Id_Mov_Det, Elem_Cod, Pro_Cod, ");
            sb.AppendLine("    Mat_Cod, Qta, Udm_Cod, Extra_Int, Extra_Str, Lotto, Cod_Progetto, ");
            sb.AppendLine("    Cal_Cod, Udm_Cod_Extra, Qta_Extra, TempoCarenza, DoseEtichetta, ");
            sb.AppendLine("    DoseEtichetta_Value, PrincipiAttivi, PrincipiAttiviPesi, Buffer, ");
            sb.AppendLine("    Qta_Extra_Totale, Mezzo_Det, PrincipiAttiviPercAbb, Polverulento ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("    Movimenti_Dettagli WITH(NOLOCK) ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("    Id_Agenda IN (@idAgendas) ");

            sb.AppendLine(" ;");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("      md.Piva, md.Sa_Cod, md.Id_Agenda, md.Id_Mov, ");
            sb.AppendLine("      md.Id_Mov_Det, md.Appezza, md.Id_Destinazione, ");
            sb.AppendLine("      md.Tipo_Destinazione, md.Qta, md.Qta2, md.Sup_Riduzione_BufferZone, ");
            sb.AppendLine("      md.Perc_Riduzione_Deriva, ISNULL(ip.Progetto_Cod,0) as Progetto_Cod ");
            sb.AppendLine(" FROM  ");
            sb.AppendLine("      Mov_Destinazioni md ");
            sb.AppendLine(" JOIN ");
            sb.AppendLine("      Agenda a ");
            sb.AppendLine("      ON a.Piva = md.Piva");
            sb.AppendLine("      AND a.Id_Agenda = md.Id_Agenda ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("      Imprese_Progetti ip");
            sb.AppendLine("      ON ip.Piva = md.PIVA ");
            sb.AppendLine("      And ip.Sa_Cod = md.SA_COD");
            sb.AppendLine("      And ip.Appezza = md.APPEZZA");
            sb.AppendLine("      And ip.Id_Reg = md.Id_Destinazione");
            sb.AppendLine("      And ip.Validita_Inizio <= a.Validita_Inizio ");
            sb.AppendLine("      And ip.Validita_Fine >= a.Validita_Inizio ");
            sb.AppendLine("      AND md.Tipo_Destinazione = 0 ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("      md.Id_Agenda IN (@idAgendas) ");

            sb.AppendLine(" ;");

            if (opzioniLetturaAgenda.LeggiNote)
            {
                sb.AppendLine(" SELECT ");
                sb.AppendLine("     Nota_Cod, Id_Agenda ");
                sb.AppendLine(" FROM  ");
                sb.AppendLine("     AgendaxNote WITH(NOLOCK) ");
                sb.AppendLine(" WHERE ");
                sb.AppendLine("     Id_Agenda IN (@idAgendas)");

                sb.AppendLine(" ;");

                sb.AppendLine(" SELECT ");
                sb.AppendLine("   ni.[Nota_Cod], ");
                sb.AppendLine("   ni.[NotaGruppo_Cod] ");
                sb.AppendLine(" FROM ");
                sb.AppendLine("   Note_Intervento ni WITH(NOLOCK) ");
                sb.AppendLine(" JOIN ");
                sb.AppendLine("   Note_Intervento_Gruppi nig WITH(NOLOCK) ");
                sb.AppendLine("   ON ni.PivaSuperUser = nig.PivaSuperUser  ");
                sb.AppendLine("   And ni.NotaGruppo_Cod = nig.NotaGruppo_Cod ");
                sb.AppendLine(" JOIN ");
                sb.AppendLine("   Note_Intervento_UtilizzoxGruppi niu WITH(NOLOCK)");
                sb.AppendLine("   ON nig.PivaSuperUser = niu.PivaSuperUser  ");
                sb.AppendLine("   AND nig.NotaGruppo_Cod = niu.NotaGruppo_Cod ");
                sb.AppendLine(" WHERE ");
                sb.AppendLine("   ni.PivaSuperUser = @pivaSuperUser ");
                sb.AppendLine("   AND NotaUtilizzo_Cod = @notaUtilizzoCod");

                sb.AppendLine(" ;");

                parametersSql.Add("@pivaSuperUser", parametriTriple.ObjParametriServer.PivaSuperUser);
                parametersSql.Add("@notaUtilizzoCod", (int)Enum_Note_Intervento_Utilizzo.QuadernoCampagna);

            }

            Dictionary<string, Dictionary<Type, List<object>>> parametersIn = new()
            {
                { "@idAgendas", FormatClauseIn(idAgendas) },
            };

            DataSet risultatoLettura = null;
            try
            {
                risultatoLettura = await dataProvider.ExecuteMultipleReadAsync(sb.ToString(),"resultTabelleAgenda", parametersSql, parametersIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriTriple.ObjParametriServer, ex);
                throw;
            }

            DataTable dtAgenda = risultatoLettura.Tables[0];
            var agendaRows = dtAgenda.AsEnumerable().ToList();
            var lavCods = agendaRows.Select(dr => dr.Field<int>("Lav_Cod")).Distinct().ToList();

            var sbOperazioni = new StringBuilder();
            DataTable dtOperazioni;

            sbOperazioni.AppendLine(" SELECT ");
            sbOperazioni.AppendLine("   grOP.Tipo, OP.Lav_Cod ");
            sbOperazioni.AppendLine(" FROM  ");
            sbOperazioni.AppendLine("   Operazioni OP WITH(NOLOCK) ");
            sbOperazioni.AppendLine(" JOIN ");
            sbOperazioni.AppendLine("   GruppoOperazioni grOP WITH(NOLOCK) ");
            sbOperazioni.AppendLine("   ON OP.GRU_OP = grOP.GRU_COD ");
            sbOperazioni.AppendLine(" WHERE ");
            sbOperazioni.AppendLine("   Lav_Cod IN (@lavCods) ");

            var parametersOperazioni = new Dictionary<string, object>();

            Dictionary<string, Dictionary<Type, List<object>>> parametersInOperazioni = new()
            {
                { "@lavCods", FormatClauseIn(lavCods) },
            };

            try
            {
                dtOperazioni = await dataProvider.ExecuteReadAsync(sbOperazioni.ToString(), parametersOperazioni, parametersInOperazioni);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriTriple.ObjParametriServer, ex);
                throw;
            }

            DataTable dtRicetteXAgenda = risultatoLettura.Tables[1];
            DataTable dtMovimenti = risultatoLettura.Tables[2];
            DataTable dtMovDettaglioTecnico = risultatoLettura.Tables[3];
            DataTable dtMovimentiDettagli = risultatoLettura.Tables[4];
            DataTable dtMovDestinazioni = risultatoLettura.Tables[5];

            DataTable? dtNote = null;
            DataTable? dtNoteIntervento = null;
            
            if (opzioniLetturaAgenda.LeggiNote)
            {
                dtNote = risultatoLettura.Tables[6];
                dtNoteIntervento = risultatoLettura.Tables[7];
            }

            var result = new RisultatoTabelleAgenda(
                agendaRows,
                dtOperazioni?.AsEnumerable().ToList() ?? new(),
                dtRicetteXAgenda?.AsEnumerable().ToList() ?? new(),
                dtNote?.AsEnumerable().ToList() ?? new(),
                dtNoteIntervento?.AsEnumerable().ToList() ?? new(),
                dtMovimenti?.AsEnumerable().ToList() ?? new(),
                dtMovDettaglioTecnico?.AsEnumerable().ToList() ?? new(),
                dtMovimentiDettagli?.AsEnumerable().ToList() ?? new(),
                dtMovDestinazioni?.AsEnumerable().ToList() ?? new());

            return result;
        }

        public async Task<(DataTable dtAttivita, DataTable dtAttivitaCancellate, DataTable dtBrogliacci, DataTable dtBrogliacciCancellati)> LeggiAttivitaBrogliacciPerAppAsync(string piva,
            DateTime dataRiferimento, DateTime dataUltimaSincro, AgronicaCoreParametriServer parametriServer)
        {
            var sb = new StringBuilder();

            AggiungiQueryPerAttivitaApp(sb);
            AggiungiQueryPerBrogliacciApp(sb);

            var parameters = new Dictionary<string, object>
            {
                { "@piva", piva },
                { "@dataLavorazioneMin", dataRiferimento },
                { "@dataUltimaSincro", dataUltimaSincro },
                { "@contabilizzatoValorePianificate", -1 },
                { "@cancellazioni", 3 },
                { "@tipo", "Ricette_Operazioni" },
                { "@zero", 0 },
            };

            var tipiAppDatiPerAttivita = new List<string>()
            {
                ((int)DatiApp.Attivita).ToString(),
                ((int)DatiApp.AttivitaDemetra).ToString(),
            };

            var tipiDemetra = new List<string>()
            {
                ((int)DatiApp.AttivitaDemetra).ToString(),
                ((int)DatiApp.RicetteDemetra).ToString(),
            };

            var tipiApp = new List<string>()
            {
                ((int)DatiApp.Attivita).ToString(),
                ((int)DatiApp.Ricette).ToString(),
            };

            var listOfStati = new List<int>()
            {
                (int)Attivita.Stati.Eseguita,
                (int)Attivita.Stati.Da_Eseguire,
            };

            Dictionary<string, Dictionary<Type, List<object>>> parametersIn = new()
            {
                { "@lavCods", FormatClauseIn(CostantiPersonalizzate.OPERAZIONI_GESTITE_APP_DEMETRA_LIST) },
                { "@tipiAttivita", FormatClauseIn(tipiAppDatiPerAttivita) },
                { "@listOfStati", FormatClauseIn(listOfStati) },
                { "@tipiDemetra", FormatClauseIn(tipiDemetra) },
                { "@tipiApp", FormatClauseIn(tipiApp) },
            };

            DataSet result;
            try
            {
                result = await GetDataProvider(parametriServer).ExecuteMultipleReadAsync(sb.ToString(), "resultAgendePerApp", parameters, parametersIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriServer, ex);
                throw;
            }

            return (result.Tables[0], result.Tables[1], result.Tables[2], result.Tables[3]);
        }

        public async Task<(DataTable dtAttivita, DataTable dtAttivitaCancellate)> LeggiAttivitaPerAppAsync(string piva, DateTime dataRiferimento, DateTime dataUltimaSincro, AgronicaCoreParametriServer parametriServer)
        {
            var sb = new StringBuilder();

            AggiungiQueryPerAttivitaApp(sb);

            var parameters = new Dictionary<string, object>
            {
                { "@piva", piva },
                { "@dataLavorazioneMin", dataRiferimento },
                { "@dataUltimaSincro", dataUltimaSincro },
                { "@contabilizzatoValorePianificate", -1 },
                { "@cancellazioni", 3 },
            };

            var tipiAppDatiPerAttivita = new List<string>()
            {
                ((int)DatiApp.Attivita).ToString(),
                ((int)DatiApp.AttivitaDemetra).ToString(),
            };

            Dictionary<string, Dictionary<Type, List<object>>> parametersIn = new()
            {
                { "@lavCods", FormatClauseIn(CostantiPersonalizzate.OPERAZIONI_GESTITE_APP_DEMETRA_LIST) },
                { "@tipiAttivita", FormatClauseIn(tipiAppDatiPerAttivita) },
            };

            DataSet result;
            try
            {
                result = await GetDataProvider(parametriServer).ExecuteMultipleReadAsync(sb.ToString(),"resultAgendePerApp", parameters, parametersIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriServer, ex);
                throw;
            }

            return (result.Tables[0], result.Tables[1]);
        }

        public async Task<(DataTable dtBrogliacci, DataTable dtBrogliacciCancellati)> LeggiBrogliacciPerAppAsync(string piva, DateTime dataRiferimento, 
            DateTime dataUltimaSincro, AgronicaCoreParametriServer parametriServer)
        {
            var sb = new StringBuilder();

            AggiungiQueryPerBrogliacciApp(sb);

            var parameters = new Dictionary<string, object>
            {
                { "@piva", piva },
                { "@dataLavorazioneMin", dataRiferimento },
                { "@dataUltimaSincro", dataUltimaSincro },
                { "@cancellazioni", 3 },
                { "@tipo", "Ricette_Operazioni" },
                { "@zero", 0 },
            };

            var tipiDemetra = new List<string>()
            {
                ((int)DatiApp.AttivitaDemetra).ToString(),
                ((int)DatiApp.RicetteDemetra).ToString(),
            };

            var tipiApp = new List<string>()
            {
                ((int)DatiApp.Attivita).ToString(),
                ((int)DatiApp.Ricette).ToString(),
            };

            var listOfStati = new List<int>()
            {
                (int)Attivita.Stati.Eseguita,
                (int)Attivita.Stati.Da_Eseguire,
            };

            Dictionary<string, Dictionary<Type, List<object>>> parametersIn = new()
            {
                { "@lavCods", FormatClauseIn(CostantiPersonalizzate.OPERAZIONI_GESTITE_APP_DEMETRA_LIST) },
                { "@listOfStati", FormatClauseIn(listOfStati) },
                { "@tipiDemetra", FormatClauseIn(tipiDemetra) },
                { "@tipiApp", FormatClauseIn(tipiApp) },
            };

            try
            {
                var resultDataSet = await GetDataProvider(parametriServer).ExecuteMultipleReadAsync(sb.ToString(), 
                    "resultBrogliacciPerApp",parameters, parametersIn);

                return (resultDataSet.Tables[0], resultDataSet.Tables[1]);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriServer, ex);
                throw;
            }
        }

        private void AggiungiQueryPerAttivitaApp(StringBuilder sb)
        {
            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Piva, TRY_CONVERT(INT, SUBSTRING(riferimento,0,CHARINDEX('|',riferimento,0))) ID_Agenda, ID as GuidRicetta, Versione, Tipo, ");
            sb.AppendLine("     TRY_CONVERT(INT, SUBSTRING(riferimento, CHARINDEX('|', riferimento)+1,  LEN(riferimento) - CHARINDEX('|', riferimento))) CodiceRicetta, ");
            sb.AppendLine("     Riferimento_Pianificata ");
            sb.AppendLine(" INTO ");
            sb.AppendLine("     #cteAgendeDemetraEApp ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     APP_Dati (NOLOCK) ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     tipo IN(@tipiAttivita) ");
            sb.AppendLine("     AND SUBSTRING(riferimento, 0, CHARINDEX('|',riferimento,0)) > 0 ");

            sb.AppendLine("; ");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("	 a.Piva, a.Id_Agenda, a.Lav_Cod, a.Raccoglitore_Cod, ISNULL(ada.GuidRicetta, '') as GuidRicetta, ISNULL(ada.Versione, '') as Versione, ISNULL(ada.Tipo, '') as Tipo, ");
            sb.AppendLine("    ISNULL(ada.CodiceRicetta, 0) as CodiceRicetta, ISNULL(ada.Riferimento_Pianificata, '') as CodiceGiasPianificata, ISNULL(ra.Ricetta_Operazione_Cod, 0) as Ricetta_Operazione_Cod");
            sb.AppendLine(" FROM ");
            sb.AppendLine("	 Agenda a (NOLOCK) ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("    RicetteXAgenda ra ");
            sb.AppendLine("    ON ra.Id_Agenda = a.Id_Agenda ");
            sb.AppendLine(" LEFT JOIN");
            sb.AppendLine("	 #cteAgendeDemetraEApp ada");
            sb.AppendLine("	 ON a.Id_Agenda = ada.ID_Agenda");
            sb.AppendLine("	 AND a.PIVA = ada.Piva");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("	 a.Piva = @piva");
            sb.AppendLine("    AND a.Validita_Inizio >= @dataLavorazioneMin ");
            sb.AppendLine("    AND a.Lav_Cod IN (@lavCods) ");
            sb.AppendLine("    AND NOT EXISTS ");
            sb.AppendLine("    ( SELECT ");
            sb.AppendLine("       1 ");
            sb.AppendLine("     FROM ");
            sb.AppendLine("       Movimenti_Dettagli md (NOLOCK)");
            sb.AppendLine("     WHERE ");
            sb.AppendLine("       a.Id_Agenda = md.Id_Agenda ");
            sb.AppendLine("       AND md.Contabilizzato = @contabilizzatoValorePianificate )"); //rimuove le attività pianificate
            sb.AppendLine("       AND (a.Raccoglitore_Cod = 0 ");
            sb.AppendLine("           OR NOT EXISTS (SELECT 1 "); //rimuove un'attività con raccoglitore cod diverso da 0, se le altre attività con lo stesso raccoglitore cod non fanno parte delle attività gestite dall'app
            sb.AppendLine("                          FROM Agenda ");
            sb.AppendLine("                          WHERE Agenda.Raccoglitore_Cod = a.Raccoglitore_Cod ");
            sb.AppendLine("                          AND Agenda.Lav_Cod NOT IN (@lavCods)");
            sb.AppendLine("                          )");
            sb.AppendLine("        )");
            sb.AppendLine("    AND EXISTS (SELECT 1 ");
            sb.AppendLine("                FROM Agronica_Log_Agenda ala ");
            sb.AppendLine("                WHERE ala.Id_Agenda = a.Id_Agenda AND ala.Data_Ora_RegistrazioneLog >= @dataUltimaSincro )");

            sb.AppendLine(" ;");

            LogAgendaHelper.AggiungiCreazioneLogAgendaUltimaOperazione(sb);

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Id_Agenda ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     #ultimoLogAgenda");
            sb.AppendLine(" WHERE ");
            sb.AppendLine(" 	  UltimaOperazione = @cancellazioni ");

            sb.AppendLine(" ;");

            sb.AppendLine(" DROP TABLE #cteLogAgenda; ");
            sb.AppendLine(" DROP TABLE #ultimoLogAgenda; ");
            sb.AppendLine(" DROP TABLE #cteAgendeDemetraEApp; ");
        }

        private void AggiungiQueryPerBrogliacciApp(StringBuilder sb)
        {
            sb.AppendLine(" SELECT ");
            sb.AppendLine("	  r.Ricetta_SuperUser, r.Piva, r.Ricetta_Cod,");
            sb.AppendLine("	  ro.Ricetta_Operazione_Cod, ro.Raccoglitore_Cod, ro.Lav_Cod,");
            sb.AppendLine("     ro.W_Anagrafica_Stati_Cod as StatoRicetta,");
            sb.AppendLine(" 	  CASE");
            sb.AppendLine(" 	  	  WHEN CHARINDEX('|', ro.APP_Ricetta_Operazione_ID) > 0");
            sb.AppendLine(" 	  	  THEN LEFT(ro.APP_Ricetta_Operazione_ID, CHARINDEX('|', ro.APP_Ricetta_Operazione_ID) - 1)");
            sb.AppendLine(" 	  	  ELSE ro.APP_Ricetta_Operazione_ID");
            sb.AppendLine(" 	  END as GuidRicetta ");
            sb.AppendLine(" INTO ");
            sb.AppendLine("     #cteBrogliacciFiltrati ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("	  Ricette_Operazioni ro ");
            sb.AppendLine(" JOIN ");
            sb.AppendLine("	  Ricette r");
            sb.AppendLine("	  on r.Ricetta_SuperUser = ro.Ricetta_SuperUser");
            sb.AppendLine("	  and r.Ricetta_Cod = ro.Ricetta_Cod");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     r.Piva = @piva ");
            sb.AppendLine("     AND ro.W_Anagrafica_Stati_Cod IN (@listOfStati) ");
            sb.AppendLine("     AND ro.Lav_Cod IN (@lavCods) ");
            sb.AppendLine("     AND ro.Validita_Inizio >= @dataLavorazioneMin ");
            sb.AppendLine("     AND NOT EXISTS (SELECT 1  "); //solo brogliacci non ribaltati in agenda
            sb.AppendLine("                     FROM RicettexAgenda ");
            sb.AppendLine("                     WHERE Ricetta_Operazione_Cod = ro.Ricetta_Operazione_Cod)");
            sb.AppendLine("     AND ro.APP_Ricetta_Operazione_ID IS NOT NULL ");
            sb.AppendLine("     AND ro.APP_Ricetta_Operazione_ID <> '' "); //solo le operazioni nate su app/demetra avranno id diverso da stringa vuota
            sb.AppendLine("     AND (ro.Raccoglitore_Cod = 0 ");
            sb.AppendLine("          OR NOT EXISTS (SELECT 1 "); //rimuove un'attività con raccoglitore cod diverso da 0, se le altre attività con lo stesso raccoglitore cod non fanno parte delle attività gestite dall'app
            sb.AppendLine("                        FROM Ricette_Operazioni roLavCod ");
            sb.AppendLine("                        WHERE roLavCod.Raccoglitore_Cod = ro.Raccoglitore_Cod ");
            sb.AppendLine("                        AND roLavCod.Lav_Cod NOT IN (@lavCods)");
            sb.AppendLine("                        )");
            sb.AppendLine("          )");
            sb.AppendLine("     AND EXISTS (SELECT 1 ");
            sb.AppendLine(" 	 			 FROM Agronica_Log_Ricette alr");
            sb.AppendLine(" 	 			 WHERE alr.SuperUser = ro.Ricetta_SuperUser ");
            sb.AppendLine(" 	 			 AND alr.Chiave = ro.Ricetta_Operazione_Cod ");
            sb.AppendLine(" 	 			 AND alr.Tipo = @tipo");
            sb.AppendLine(" 	 			 AND alr.Data_Ora_RegistrazioneLog >=  @dataUltimaSincro )");

            sb.AppendLine(";");

            sb.AppendLine(" SELECT  ");
            sb.AppendLine(" 	bf.*, ISNULL(ad.Riferimento_Pianificata, '') as CodiceGiasPianificata, ad.Versione, ad.Tipo ");
            sb.AppendLine(" FROM  ");
            sb.AppendLine(" 	#cteBrogliacciFiltrati bf");
            sb.AppendLine(" JOIN");
            sb.AppendLine("	APP_Dati ad");
            sb.AppendLine("	ON ad.ID = bf.GuidRicetta");
            sb.AppendLine(" WHERE");
            sb.AppendLine("   CHARINDEX('|', ad.riferimento) > @zero ");
            sb.AppendLine("	AND SUBSTRING(ad.riferimento,0,CHARINDEX('|',riferimento,0)) = @zero"); //solo brogliacci non ribaltati in agenda (ulteriore controllo)
            sb.AppendLine("	AND ad.cancellato = @zero "); //solo brogliacci non cancellati 
            sb.AppendLine(" ORDER BY");
            sb.AppendLine("     Ricetta_Cod, Ricetta_Operazione_Cod;");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Id, Tipo, SUBSTRING(riferimento, CHARINDEX('|', riferimento)+1,  LEN(riferimento) - CHARINDEX('|', riferimento)  ) Ricetta_Cod ");
            sb.AppendLine(" INTO ");
            sb.AppendLine("     #cteAppDatiDemetra ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     APP_Dati ad ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     Tipo IN (@tipiDemetra) ");
            sb.AppendLine("     AND CHARINDEX('|', riferimento) > 0 ");
            sb.AppendLine("     AND Piva = @piva ");

            sb.AppendLine(" ; ");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Id, Tipo, SUBSTRING(riferimento, CHARINDEX('|', riferimento)+1,  LEN(riferimento) - CHARINDEX('|', riferimento)  ) Ricetta_Operazione_Cod ");
            sb.AppendLine(" INTO ");
            sb.AppendLine("     #cteAppDatiApp ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     APP_Dati ad ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     Tipo IN (@tipiApp) ");
            sb.AppendLine("     AND CHARINDEX('|', riferimento) > @zero ");
            sb.AppendLine("     AND Piva = @piva ");

            sb.AppendLine(" ; ");

            LogRicetteHelper.AggiungiCreazioneLogRicetteUltimaOperazione(sb);

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     chiave as Ricetta_Operazione_Cod, param1 as Ricetta_Cod ");
            sb.AppendLine(" INTO ");
            sb.AppendLine("     #logRicette ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     #ultimoLogRicette ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     UltimaOperazione = @cancellazioni "); //cancellazioni

            sb.AppendLine(" ; ");

            sb.AppendLine(" select ");
            sb.AppendLine(" 	DISTINCT COALESCE(a.Id, d.Id, '') as GuidRicetta, COALESCE(a.Tipo, d.Tipo, '') as Tipo ");
            sb.AppendLine(" from ");
            sb.AppendLine(" 	#logRicette lr ");
            sb.AppendLine(" left join ");
            sb.AppendLine(" 	#cteAppDatiApp a ");
            sb.AppendLine(" 	on a.Ricetta_Operazione_Cod = lr.Ricetta_Operazione_Cod ");
            sb.AppendLine(" left join ");
            sb.AppendLine(" 	#cteAppDatiDemetra d ");
            sb.AppendLine(" 	on d.Ricetta_Cod = lr.Ricetta_Cod ");
            sb.AppendLine(" where ");
            sb.AppendLine(" 	COALESCE(a.Id, d.Id, '') <> '' ");

            sb.AppendLine(" ; ");

            sb.AppendLine(" DROP TABLE #cteBrogliacciFiltrati; ");
            sb.AppendLine(" DROP TABLE #cteAppDatiDemetra; ");
            sb.AppendLine(" DROP TABLE #cteAppDatiApp; ");
            sb.AppendLine(" DROP TABLE #cteLogRicette; ");
            sb.AppendLine(" DROP TABLE #ultimoLogRicette; ");
            sb.AppendLine(" DROP TABLE #logRicette; ");
        }

        public async Task<(DataTable dtOperatori, DataTable dtMacchine)> LeggiOreOperatorePerListaAttivitaAsync(List<int> listaDiRicettaOperazioneCod, AgronicaCoreParametriServer parametriServer)
        {
            var sb = new StringBuilder();
            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Qta, Mat_Cod, Ricetta_Operazione_Cod ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     Ricette_Dettagli ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     Ricetta_Operazione_Cod IN (@listaDiRicettaOperazioneCod)");
            sb.AppendLine("     AND Elem_Cod = @elemCodManoDopera");
            sb.AppendLine("     AND Cau_Mov IN (@cauImputazioneManodopera, @cauImputazioneTecnico, @cauImputazioneTerzisti)");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Qta, Mat_Cod, Ricetta_Operazione_Cod ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     Ricette_Dettagli ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     Ricetta_Operazione_Cod IN (@listaDiRicettaOperazioneCod)");
            sb.AppendLine("     AND Elem_Cod = @elemCodMacchine");
            sb.AppendLine("     AND Cau_Mov = @cauImputazioneMacchine");

            var parametriSql = new Dictionary<string, object>
            {
                { "@elemCodManoDopera", ELEM_COD.ELEMCOD_MANODOPERA },
                { "@cauImputazioneManodopera", CAU_MOV.CAU_IMPUTAZIONE_MANODOPERA },
                { "@cauImputazioneTecnico", CAU_MOV.CAU_IMPUTAZIONE_TECNICO_RESPONSABILE },
                { "@cauImputazioneTerzisti", CAU_MOV.CAU_IMPUTAZIONE_TERZISTI },
                { "@elemCodMacchine", ELEM_COD.MACCHINE },
                { "@cauImputazioneMacchine", CAU_MOV.CAU_IMPUTAZIONE_PARCOMACCHINE },
            };

            Dictionary<string, Dictionary<Type, List<object>>> parametriIn = new()
            {
                { "@listaDiRicettaOperazioneCod", FormatClauseIn(listaDiRicettaOperazioneCod) },
            };

            DataSet result;
            try
            {
                result = await GetDataProvider(parametriServer).ExecuteMultipleReadAsync(sb.ToString(), "resultOreOperatoreMacchine", parametriSql, parametriIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriServer, ex);
                throw;
            }

            return (result.Tables[0], result.Tables[1]);
        }

        public async Task<DataTable> ReadAsync(string Piva, int Id_Agenda, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            stbQuery.AppendLine("SELECT * FROM Agenda ")
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
            sqlParams.TryAdd("@inizio", objParametriServer.FinestraTemporaleFine);
            sqlParams.TryAdd("@fine", objParametriServer.FinestraTemporaleInizio);
            stbQuery.AppendLine("    AND Validita_Inizio <= @inizio ")
                .AppendLine("    AND Validita_Fine >= @fine ");

            if (xFiltroAggiuntivo != null)
                stbQuery.AppendLine(FormatFiltroAggiuntivo(xFiltroAggiuntivo, ref sqlParams));

            stbQuery.AppendLine("ORDER BY Id_Agenda DESC ");

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

        public async Task<bool> ExistAsync(int Id_Agenda, AgronicaCoreParametriServer objParametriServer)
        {
            if (Id_Agenda == 0)
                throw new Exception("Id_Agenda non valorizzato.");

            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@idAgenda", Id_Agenda);

            stbQuery.AppendLine("SELECT TOP(1) * FROM Agenda ")
                .AppendLine("WHERE Id_Agenda = @idAgenda ");

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

        public async Task<bool> CreateAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer)
        {
            dtoAgenda = await Valorizza(dtoAgenda);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO Agenda (PIVA, Sa_Cod, Sta_Num, Id_Agenda, Lav_Cod, des_lib, ")
                .AppendLine("    Linea_Cod, Preparazione_Cod, Id_Trasformazione, Tipo_Accettazione, Audit_Cod, Stato_Export, Stato_Export_2, Tipo_Visibilita, ChkCoge_Manuale, Id_Attivita, Modulo, ")
                .AppendLine("    Raccoglitore_Cod, Split, Pratica_Cod, Origine, Stato_Cod, DaRemoto, ")
                .AppendLine("    Blocco_Flag, Blocco_Data, Blocco_Username, inviato, ");
            if (dtoAgenda.Data_Invio != null)
            {
                stbQuery.AppendLine("    DataInvio, ");
            }
            stbQuery.AppendLine("    Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ");
            stbQuery.AppendLine("VALUES (@piva, @saCod, @staNum, @idAgenda, @lavCod, @desLib,  ")
                .AppendLine("    @lineaCod, @prepCod, @idTrasf, @tipoAcc, @auditCod, @statoExp, @statoExp2, @tipoVisib, @chkCogMan, @idAtt, @modulo, @raccCod, @split, @pratCod, @orig, @statoCod, @daRemoto, ")
                .AppendLine("    @bloccoFl, @bloccoDt, @bloccoUsr, @inviato, ");
            if (dtoAgenda.Data_Invio != null)
            {
                stbQuery.AppendLine("    @dtInvio, ");
            }
            stbQuery.AppendLine("    GETDATE(), GETDATE(), @userOp, @userOp, @inizio, @fine) ");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", dtoAgenda.Piva);
            expandoObj.TryAdd("@saCod", dtoAgenda.Sa_Cod);
            expandoObj.TryAdd("@staNum", dtoAgenda.Sta_Num);
            expandoObj.TryAdd("@idAgenda", dtoAgenda.Id_Agenda);
            expandoObj.TryAdd("@lavCod", dtoAgenda.Lav_Cod);
            expandoObj.TryAdd("@desLib", dtoAgenda.Des_Lib);

            expandoObj.TryAdd("@lineaCod", dtoAgenda.Linea_Cod);
            expandoObj.TryAdd("@prepCod", dtoAgenda.Preparazione_Cod);
            expandoObj.TryAdd("@idTrasf", dtoAgenda.Id_Trasformazione);
            expandoObj.TryAdd("@tipoAcc", dtoAgenda.Tipo_Accettazione);
            expandoObj.TryAdd("@auditCod", dtoAgenda.Audit_Cod);
            expandoObj.TryAdd("@statoExp", dtoAgenda.Stato_Export);
            expandoObj.TryAdd("@statoExp2", dtoAgenda.Stato_Export2);
            expandoObj.TryAdd("@tipoVisib", dtoAgenda.Tipo_Visibilita);
            expandoObj.TryAdd("@chkCogMan", dtoAgenda.ChkCoge_Manuale);
            expandoObj.TryAdd("@idAtt", dtoAgenda.Id_Attivita);
            expandoObj.TryAdd("@modulo", dtoAgenda.Modulo);
            expandoObj.TryAdd("@raccCod", dtoAgenda.Raccoglitore_Cod);
            expandoObj.TryAdd("@split", dtoAgenda.Split);
            expandoObj.TryAdd("@pratCod", dtoAgenda.Pratica_Cod);
            expandoObj.TryAdd("@orig", dtoAgenda.Origine);
            expandoObj.TryAdd("@statoCod", dtoAgenda.Stato_Cod);
            expandoObj.TryAdd("@daRemoto", dtoAgenda.DaRemoto);
            
            expandoObj.TryAdd("@bloccoFl", dtoAgenda.Blocco_Flag);
            expandoObj.TryAdd("@bloccoDt", dtoAgenda.Blocco_Data);
            expandoObj.TryAdd("@bloccoUsr", dtoAgenda.Blocco_Username);
            expandoObj.TryAdd("@inviato", dtoAgenda.Inviato);
            if (dtoAgenda.Data_Invio != null)
            {
                expandoObj.TryAdd("@dtInvio", dtoAgenda.Data_Invio);
            }
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            expandoObj.TryAdd("@inizio", dtoAgenda.Validita_Inizio);
            expandoObj.TryAdd("@fine", dtoAgenda.Validita_Fine);

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

        public async Task<bool> UpdateAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer)
        {
            if (dtoAgenda.Id_Agenda == 0)
                throw new Exception("Id_Agenda non valorizzato.");

            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", dtoAgenda.Piva);
            expandoObj.TryAdd("@idAgenda", dtoAgenda.Id_Agenda);
            expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);

            var excludedProperties = new HashSet<string>
            {
                "Piva",
                "Sa_Cod",
                "Sta_Num",
                "Lav_Cod",
                "Id_Agenda",
                "inviato",
                "DataInvio",
                "Blocco_Flag",
                "Blocco_Data",
                "Blocco_Username",
                "Username_Creazione",
                "Username_Modifica",
                "Data_Creazione",
                "Data_Modifica"
            };

            var setClauses = new List<string>();
            foreach (var property in typeof(WriteAgenda).GetProperties())
            {
                if (excludedProperties.Contains(property.Name)) continue;

                var value = property.GetValue(dtoAgenda);
                if (value != null)
                {
                    string paramName = "@" + property.Name;
                    setClauses.Add($"{property.Name} = {paramName}");
                    expandoObj.TryAdd(paramName, value);
                }
            }
            if (setClauses.Count == 0) return false;

            string updateQuery = 
                $@"UPDATE Agenda SET
                    {string.Join(", ", setClauses)}
                    , Username_Modifica = @userOp
                    , Data_Modifica = GETDATE()
                WHERE Piva = @piva AND Id_Agenda = @idAgenda";

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

        public async Task<bool> DeleteAsync(string Piva, int Id_Agenda, int Sa_Cod, AgronicaCoreParametriServer objParametriServer)
        {
            if (Id_Agenda == 0)
                throw new Exception("Id_Agenda non valorizzato.");

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@piva", Piva);
            expandoObj.TryAdd("@idAgenda", Id_Agenda);

            if (objParametriServer.FlagCancellazioneLogica == enumCancellazioneLogica.CancellazioneLogica)
            {
                stbQuery.AppendLine("UPDATE Agenda SET ")
                    .AppendLine("    Inviato = -1, ")
                    .AppendLine("    Username_Modifica = @userOp ")
                    .AppendLine("WHERE Inviato >= 0 ");
                expandoObj.TryAdd("@userOp", objParametriServer.UsernameOperazione);
            } else
            {
                stbQuery.AppendLine("DELETE FROM Agenda ")
                    .AppendLine("WHERE 1=1 ");
            }

            stbQuery.AppendLine("    AND Piva = @piva ")
                .AppendLine("    AND Id_Agenda = @idAgenda ");

            if (Sa_Cod != 0)
            {
                stbQuery.AppendLine("    AND Sa_Cod = @saCod ");
                expandoObj.TryAdd("@saCod", Sa_Cod);
            }

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

        public async Task<int> ScriviModificaAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer, string jobj = "")
        {
            try
            {
                bool isNew = false;

                if (dtoAgenda.Id_Agenda == 0)
                {
                    dtoAgenda.Id_Agenda = await _sequenceDal.NuovoId_TabellaAsync("agenda", 0, 2000000000, objParametriServer);
                    isNew = true;
                }
                else
                    isNew = !await ExistAsync(dtoAgenda.Id_Agenda, objParametriServer);

                if (isNew)
                {
                    await CreateAsync(dtoAgenda, objParametriServer);
                }
                else
                {
                    await UpdateAsync(dtoAgenda, objParametriServer);
                }

                return dtoAgenda.Id_Agenda;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> BloccaAttivitaAsync(string Piva, int Sa_Cod, int Id_Agenda, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                WriteAgenda dtoAgenda = new(Piva, Sa_Cod, Id_Agenda)
                {
                    Blocco_Flag = 1,
                    Blocco_Data = DateTime.Now,
                    Blocco_Username = objParametriServer.UsernameOperazione
                };
                return await UpdateAsync(dtoAgenda, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> SbloccaAttivitaAsync(string Piva, int Sa_Cod, int Id_Agenda, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                WriteAgenda dtoAgenda = new(Piva, Sa_Cod, Id_Agenda)
                {
                    Blocco_Flag = 0,
                    Blocco_Data = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                    Blocco_Username = objParametriServer.UsernameOperazione
                };
                return await UpdateAsync(dtoAgenda, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> BloccaAttivitaAsync(string Piva, int Sa_Cod, List<int> Operazioni, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(objParametriServer.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (int Id_Agenda in Operazioni)
                    {
                        WriteAgenda dtoAgenda = new(Piva, Sa_Cod, Id_Agenda)
                        {
                            Blocco_Flag = 1,
                            Blocco_Data = DateTime.Now,
                            Blocco_Username = objParametriServer.UsernameOperazione
                        };
                        await UpdateAsync(dtoAgenda, objParametriServer);
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

        public async Task<bool> SbloccaAttivitaAsync(string Piva, int Sa_Cod, List<int> Operazioni, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(objParametriServer.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (int Id_Agenda in Operazioni)
                    {
                        WriteAgenda dtoAgenda = new(Piva, Sa_Cod, Id_Agenda)
                        {
                            Blocco_Flag = 0,
                            Blocco_Data = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                            Blocco_Username = objParametriServer.UsernameOperazione
                        };
                        await UpdateAsync(dtoAgenda, objParametriServer);
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

        public async Task<bool> EliminaAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await DeleteAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, dtoAgenda.Sa_Cod ?? 0, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<bool> EliminaAssociateAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(objParametriServer.objTransazione != null ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    /*-- MOV DETTAGLIO TECNICO EXTRA --*/
                    DataTable dtMovDetTecEx = await _movDettTecExtraDal.ReadAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, 0, 0, 0, objParametriServer);
                    if (dtMovDetTecEx.Rows.Count > 0)
                    {
                        var dtosMovDettTec = dtMovDetTecEx.ToDictionaryList().Select(row => new WriteMovDettTecnicoExtra()
                        {
                            Piva = Convert.ToString(row["Piva"]),
                            Id_Agenda = (int)row["Id_Agenda"],
                            Id_Mov = (int)row["Id_Mov"],
                            Id_Mov_Det = (int)row["Id_Mov_Det"],
                            Id_Reg_Det = (int)row["Id_Reg_Det"]
                        }).ToList();
                        await _movDettTecExtraDal.EliminaAsync(dtosMovDettTec, objParametriServer);
                    }

                    /*-- MOV DETTAGLIO TECNICO --*/
                    DataTable dtMovDetTec = await _movDettTecDal.ReadAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, 0, 0, 0, objParametriServer);
                    if (dtMovDetTec.Rows.Count > 0)
                    {
                        var dtosMovDettTec = dtMovDetTec.ToDictionaryList().Select(row =>
                            new WriteMovDettTecnico(Convert.ToString(row["Piva"]), (int)row["Id_Agenda"], (int)row["Id_Mov"], (int)row["Id_Mov_Det"], (int)row["Id_Reg_Dettaglio"])).ToList(); 
                        await _movDettTecDal.EliminaAsync(dtosMovDettTec, objParametriServer);
                    }

                    /*-- MOV DESTINAZIONI --*/
                    DataTable dtMovDest = await _movDestinazioniDal.ReadAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, 0, 0, 0, objParametriServer);
                    if (dtMovDest.Rows.Count > 0)
                    {
                        var dtosMovDest = dtMovDest.ToDictionaryList()
                            .Select(row => new WriteMovDestinazioni(Convert.ToString(row["Piva"]), (int)row["Id_Agenda"], (int)row["Id_Mov"], (int)row["Id_Mov_Det"], (int)row["Id_Destinazione"]))
                            .ToList();
                        await _movDestinazioniDal.EliminaAsync(dtosMovDest, objParametriServer);
                    }

                    /*-- MOVIMENTI DETTAGLI --*/
                    DataTable dtMovDett = await _movDettagliDal.ReadAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, 0, 0, objParametriServer);
                    if (dtMovDett.Rows.Count > 0)
                    {
                        var dtosMovDett = dtMovDett.ToDictionaryList()
                            .Select(row => new WriteMovDettagli(Convert.ToString(row["PIVA"]), (int)row["Id_Agenda"], (int)row["Id_Mov"], (int)row["Id_Mov_Det"]))
                            .ToList();
                        await _movDettagliDal.EliminaAsync(dtosMovDett, objParametriServer);
                    }

                    /*-- MOVIMENTI ZOO --*/
                    DataTable dtMovZoo = await _movZooDal.ReadAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, 0, objParametriServer);
                    if (dtMovZoo.Rows.Count > 0)
                    {
                        var dtosMovZoo = dtMovZoo.ToDictionaryList()
                            .Select(row => new WriteMovimentiZoo(Convert.ToString(row["Piva"]), (int)row["Id_Agenda"], (int)row["Id_Mov"]))
                            .ToList();
                        await _movZooDal.EliminaAsync(dtosMovZoo, objParametriServer);
                    }

                    /*-- MOVIMENTI --*/
                    DataTable dtMov = await _movimentiDal.ReadAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, 0, objParametriServer);
                    if (dtMov.Rows.Count > 0)
                    {
                        var dtosMov = dtMov.ToDictionaryList()
                            .Select(row => new WriteMovimenti(Convert.ToString(row["PIVA"]), (int)row["Id_Agenda"], (int)row["Id_Mov"]))
                            .ToList();
                        await _movimentiDal.EliminaAsync(dtosMov, objParametriServer);
                    }

                    /*-- RICETTExAGENDA --*/
                    DataTable dtRxA = await _ricettexAgDal.ReadAsync(0, dtoAgenda.Id_Agenda, objParametriServer);
                    if (dtRxA.Rows.Count > 0)
                    {
                        var dtosRxA = dtRxA.ToDictionaryList().Select(row => new WriteRicettexAgenda()
                        {
                            Ricetta_Cod = (int)row["Ricetta_Cod"],
                            Id_Agenda = (int)row["Id_Agenda"]
                        }).ToList();
                        await _ricettexAgDal.EliminaAsync(dtosRxA, objParametriServer);
                    }

                    /*-- AGENDA --*/
                    await DeleteAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, dtoAgenda.Sa_Cod ?? 0, objParametriServer);

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

        public async Task<DataTable> ReadUtentiOperazioniAsync(string nomeDbUtenti, string dataInizio, string dataFine, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("@dataValiditaInizio", dataInizio);
            sqlParams.TryAdd("@dataValiditaFine", dataFine);
            DataTable result;


            stbQuery.AppendLine("SELECT DISTINCT tot.*")
                .AppendLine("FROM (")
            // Operazioni di Campagna
                .AppendLine("   SELECT DISTINCT Agenda.Piva,")
                .AppendLine("      Agenda.Username_Creazione,")
                .AppendLine("      ud.UserName,")
                .AppendLine("      ud.CodFisc,")
                .AppendLine("      ud.Nome + ' ' + ud.Cognome AS Utente")
                .AppendLine("   FROM Agenda (NOLOCK)")
                .AppendLine("   INNER JOIN Movimenti (NOLOCK)")
                .AppendLine("   ON Agenda.piva = Movimenti.piva")
                .AppendLine("   AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
                .AppendLine($"  INNER JOIN {nomeDbUtenti}.dbo.Utenti_Dettagli ud (NOLOCK)")
                .AppendLine("   ON Agenda.Username_Creazione = ud.CodFisc")
                .AppendLine("   WHERE Movimenti.cau_mov IN ('2050','2100','2200','2300')")
                .AppendLine("   AND Agenda.Validita_inizio <= @dataValiditaFine")
                .AppendLine("   AND Agenda.Validita_Fine >= @dataValiditaInizio")
                .AppendLine("   UNION")
            // Operazioni di Magazzino
                .AppendLine("   SELECT DISTINCT Agenda.Piva,")
                .AppendLine("      Agenda.Username_Creazione,")
                .AppendLine("      ud.UserName,")
                .AppendLine("      ud.CodFisc,")
                .AppendLine("      ud.Nome + ' ' + ud.Cognome AS Utente")
                .AppendLine("   FROM Agenda (NOLOCK)")
                .AppendLine("   INNER JOIN Movimenti (NOLOCK)")
                .AppendLine("   ON Agenda.piva = Movimenti.piva")
                .AppendLine("   AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
                .AppendLine("   INNER JOIN Mov_Destinazioni (NOLOCK)")
                .AppendLine("   ON Movimenti.piva = Mov_Destinazioni.piva")
                .AppendLine("   AND Movimenti.id_agenda = Mov_Destinazioni.id_agenda")
                .AppendLine("   AND Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov")
                .AppendLine("   AND Mov_Destinazioni.Tipo_Destinazione = 20")
                .AppendLine($"  INNER JOIN {nomeDbUtenti}.dbo.Utenti_Dettagli ud (NOLOCK)")
                .AppendLine("   ON Agenda.Username_Creazione = ud.CodFisc")
                .AppendLine("   WHERE Movimenti.cau_mov IN ('7300')")
                .AppendLine("   AND Agenda.Validita_inizio <= @dataValiditaFine")
                .AppendLine("   AND Agenda.Validita_Fine >= @dataValiditaInizio")
                .AppendLine("   UNION")
            // PUA
                .AppendLine("   SELECT DISTINCT PUA_Testata.piva,")
                .AppendLine("      PUA_Testata.Username_Creazione,")
                .AppendLine("      ud.UserName,")
                .AppendLine("      ud.CodFisc,")
                .AppendLine("      ud.Nome + ' ' + ud.Cognome AS Utente")
                .AppendLine("   FROM PUA_Testata (NOLOCK)")
                .AppendLine($"  INNER JOIN {nomeDbUtenti}.dbo.Utenti_Dettagli ud (NOLOCK)")
                .AppendLine("   ON PUA_Testata.Username_Creazione = ud.CodFisc")
                .AppendLine("   AND PUA_Testata.Validita_inizio <= @dataValiditaFine")
                .AppendLine("   AND PUA_Testata.Validita_Fine >= @dataValiditaInizio")
                .AppendLine("   UNION")
            // Piani Concimazione
                .AppendLine("   SELECT DISTINCT PianoConcimazione_Dettagli.PC_Dettagli_PIVA as piva,")
                .AppendLine("      PianoConcimazione_Dettagli.Username_Creazione,")
                .AppendLine("      ud.UserName,")
                .AppendLine("      ud.CodFisc,")
                .AppendLine("      ud.Nome + ' ' + ud.Cognome AS Utente")
                .AppendLine("   FROM PianoConcimazione_Dettagli (NOLOCK)")
                .AppendLine($"  INNER JOIN {nomeDbUtenti}.dbo.Utenti_Dettagli ud (NOLOCK)")
                .AppendLine("   ON PianoConcimazione_Dettagli.Username_Creazione = ud.CodFisc")
                .AppendLine("   AND PianoConcimazione_Dettagli.Validita_inizio <= @dataValiditaFine")
                .AppendLine("   AND PianoConcimazione_Dettagli.Validita_Fine >= @dataValiditaInizio")
                .AppendLine(") as tot")
                .AppendLine("ORDER BY tot.piva");

            try
            {
                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }
    }

    public class OpzioniLetturaAgenda
    {
        public bool LeggiNote { get; set; }

        public OpzioniLetturaAgenda()
        {
            LeggiNote = true;
        }
    }

    public class RisultatoTabelleAgenda
    {
        public List<DataRow> RowsAgenda { get; set; }
        public List<DataRow> RowsOperazioni { get; set; }
        public List<DataRow> RowsRicetteXAgenda { get; set; }
        public List<DataRow> RowsNote { get; set; }
        public List<DataRow> RowsNoteIntervento { get; set; }
        public List<DataRow> RowsMovimenti { get; set; }
        public List<DataRow> RowsMovDettaglioTecnico { get; set; }
        public List<DataRow> RowsMovimentiDettagli { get; set; }
        public List<DataRow> RowsMovDestinazioni { get; set; }
        public RisultatoTabelleAgenda(List<DataRow> rowsAgenda, List<DataRow> rowsOperazioni, List<DataRow> rowsRicetteXAgenda, List<DataRow> rowsNote, List<DataRow> rowsNoteIntervento,
            List<DataRow> rowsMovimenti, List<DataRow> rowsMovDettaglioTecnico, List<DataRow> rowsMovimentiDettagli, List<DataRow> rowsMovDestinazioni)
        {
            RowsAgenda = rowsAgenda;
            RowsOperazioni = rowsOperazioni;
            RowsRicetteXAgenda = rowsRicetteXAgenda;
            RowsNote = rowsNote;
            RowsNoteIntervento = rowsNoteIntervento;
            RowsMovimenti = rowsMovimenti;
            RowsMovDettaglioTecnico = rowsMovDettaglioTecnico;
            RowsMovimentiDettagli = rowsMovimentiDettagli;
            RowsMovDestinazioni = rowsMovDestinazioni;
        }
    }
}
