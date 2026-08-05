using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreDTOStd.InData.Zoo;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazioni;
using AgronicaNetCore.OperazioniZoo.BIZ.Resources;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.OperazioniZoo;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiProfili;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.UtilityDB;
using InData.Zoo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using static AgronicaNetCore.Base.Constants.CostantiPersonalizzate;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.OperazioniZoo
{
    public class OperazioniZooService : BaseServiceOperazioniZooBIZ, IOperazioniZooService
    {
        private const int COD_ANTIBIOTICO = 2; // Farmaci_Categorie_Semplificate.ID per Antibiotico
        private const int COD_ANTINFIAMMATORIO = 6; // Farmaci_Categorie_Semplificate.ID per Antinfiammatorio

        private readonly IOperazioniZoo _operazioniZoo;
        private readonly IOperazioni _operazioni;
        private readonly IUtentiImpostazioni _utentiImpostazioni;
        private readonly IUtentiProfili _utentiProfili;
        private readonly IUtentiVisibilitaAppoggio _utentiVisibilitaAppoggio;
        private readonly IUtilityDB _utilityDB;
        private readonly IAgro_Sequence _agroSequence;


        readonly List<int> ListaOperazioniZoo = new() { 3000, 3001, 3002, 3003, 3004, 3020, 3023, 3028, 3030, 3033, 3034, 3035, 3036, 3037 };
        public OperazioniZooService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _operazioniZoo = provider.GetRequiredService<IOperazioniZoo>();
            _operazioni = provider.GetRequiredService<IOperazioni>();
            _utentiImpostazioni = provider.GetRequiredService<IUtentiImpostazioni>();
            _utentiProfili = provider.GetRequiredService<IUtentiProfili>();
            _utentiVisibilitaAppoggio = provider.GetRequiredService<IUtentiVisibilitaAppoggio>();
            _utilityDB = provider.GetRequiredService<IUtilityDB>();
            _agroSequence = provider.GetRequiredService<IAgro_Sequence>();
        }

        public async Task<DataTable> LeggiCentriAziendaliZooAsync(string piva, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            DataTable dt;
            try
            {
                bool visibilita_totale = await _utentiProfili.VisibilitaTotaleGiasOnline(objParametriUtenti, objParametriServer);
                dt = await _operazioniZoo.LeggiCentriZooAsync(piva, objParametriServer, visibilita_totale);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return dt;
        }

        public async Task<DataTable> LeggiStalleZooAsync(string piva, int centro, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            DataTable dt;
            try
            {
                bool visibilita_totale = await _utentiProfili.VisibilitaTotaleGiasOnline(objParametriUtenti, objParametriServer);
                dt = await _operazioniZoo.LeggiStalleZooAsync(piva, centro, objParametriServer, visibilita_totale);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return dt;
        }

        public async Task<DataTable> LeggiStalleRaggruppamentiZooAsync(string piva, int centro, int stanum, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable dt;
            try
            {
                dt = await _operazioniZoo.LeggiStalleRaggruppamentiZooAsync(piva, centro, stanum, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return dt;
        }

        public async Task<List<Zootecnia>> LeggiOperazioniZooAsync(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            try
            {
                var utentiImpostazioniDt = await _utentiImpostazioni.Read_JoinWithFiltroMonoAsync(TipiEnumerativi.Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_OPERAZIONI, 1, objParametriUtenti, objParametriServer);
                var dtOperazioni = (await _operazioni.Operazioni_GestioneFiltroUtente_LeggiAsync(new LeggiOperazioni_IN { Operazioni = ListaOperazioniZoo }, utentiImpostazioniDt, objParametriServer, objParametriUtenti)).DataTable;

                var sortedRows = dtOperazioni.Select("", "Lav_Des");

                var listaOperazioni = new List<Zootecnia>();

                foreach (var item in sortedRows)
                {
                    listaOperazioni.Add(new Zootecnia((int)item["LAV_COD"], item["LAV_DES"].ToString()));
                }

                return listaOperazioni;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        public async Task<List<Zootecnia>> LeggiOperazioniZooPreferiteAsync(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            try
            {
                //Prelevo i preferiti
                var dtPreferiti = await _utentiImpostazioni.Read_User_Then_SuperUserAsync((int)Enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_ZOO_PREFERITE, objParametriUtenti, objParametriServer);

                string val = "";

                var listaPreferiti = new List<Zootecnia>();

                if (dtPreferiti != null && dtPreferiti.Rows.Count > 0)
                {
                    val = dtPreferiti.Rows[0]["Impostazione_Valore_1"].ToString()!;

                    foreach (var item in val.Split("|"))
                    {
                        listaPreferiti.Add(new Zootecnia(int.Parse(item), ""));
                    }
                }

                return listaPreferiti;

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

        }

        public async Task<DataTable> LeggiOperazioniAgendaZooAsync(string piva, int saCod, int staNum, DateTime inizio, DateTime fine, AgronicaCoreParametriServer objP_Server)
        {
            var startTime = DateTime.Now;
            var nomeRoutine = "LeggiOperazioniAgendaZooAsync";

            try
            {
                var requestObject = new
                {
                    Piva = piva,
                    SaCod = saCod,
                    StaNum = staNum,
                    Inizio = inizio,
                    Fine = fine
                };

                LogInformation("Request [" + nomeRoutine + "]:" + JsonConvert.SerializeObject(requestObject), objP_Server);
                List<int> filtroCentri = new();

                if (saCod == 0)
                {
                    // check se ci sono filtri sui centri aziendali
                    var dtCentriVisibili = await _utentiVisibilitaAppoggio.ReadVisibilitaCentriAsync(piva, objP_Server);
                    if (dtCentriVisibili != null && dtCentriVisibili.Rows.Count > 0)
                    {
                        filtroCentri = dtCentriVisibili
                            .AsEnumerable()
                            .Where(r => r["sa_cod"] != null)
                            .Select(r => r.Field<int>("sa_cod")!)
                            .ToList();
                    }
                }
                var valFineGiacenze = (fine < AGRODATAFINE_DATE) ? fine.AddDays(1) : fine;
                var compLvl = await _utilityDB.Read_SQL_Compatibility_LevelAsync(objP_Server);

                var dtOperazioni = await _operazioniZoo.CaricaAgendaZooAsync(piva, saCod, staNum, inizio, fine, valFineGiacenze, filtroCentri, compLvl, objP_Server);

                var elapsedSeconds = (DateTime.Now - startTime).TotalSeconds;
                int rowNumber = dtOperazioni?.Rows.Count ?? 0;
                LogInformation($"Response [{nomeRoutine}]: Rows={rowNumber}, ExecutionTime={elapsedSeconds:F2}s", objP_Server);

                return dtOperazioni;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP_Server, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiOperazioniAgendaZooNewAsync(string piva, int saCod, int staNum, DateTime inizio, DateTime fine, AgronicaCoreParametriServer objP_Server)
        {
            var startTime = DateTime.Now;
            var nomeRoutine = "LeggiOperazioniAgendaZooNewAsync";

            try
            {
                var requestObject = new
                {
                    Piva = piva,
                    SaCod = saCod,
                    StaNum = staNum,
                    Inizio = inizio,
                    Fine = fine
                };

                LogInformation("Request [" + nomeRoutine + "]:" + JsonConvert.SerializeObject(requestObject), objP_Server);
                List<int> filtroCentri = new();

                if (saCod == 0)
                {
                    // check se ci sono filtri sui centri aziendali
                    var dtCentriVisibili = await _utentiVisibilitaAppoggio.ReadVisibilitaCentriAsync(piva, objP_Server);
                    if (dtCentriVisibili != null && dtCentriVisibili.Rows.Count > 0)
                    {
                        filtroCentri = dtCentriVisibili
                            .AsEnumerable()
                            .Where(r => r["sa_cod"] != null)
                            .Select(r => r.Field<int>("sa_cod")!)
                            .ToList();
                    }
                }
                var valFineGiacenze = (fine < AGRODATAFINE_DATE) ? fine.AddDays(1) : fine;
                var compLvl = await _utilityDB.Read_SQL_Compatibility_LevelAsync(objP_Server);

                var dtOperazioni = await _operazioniZoo.CaricaAgendaZooNewAsync(piva, saCod, staNum, inizio, fine, valFineGiacenze, filtroCentri, compLvl, objP_Server);

                var elapsedSeconds = (DateTime.Now - startTime).TotalSeconds;
                int rowNumber = dtOperazioni?.Rows.Count ?? 0;
                LogInformation($"Response [{nomeRoutine}]: Rows={rowNumber}, ExecutionTime={elapsedSeconds:F2}s", objP_Server);

                return dtOperazioni;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP_Server, ex);
                throw;
            }
        }

        public async Task<DataTable> LeggiGiacenzeZooAsync(
            LeggiGiacenzeZooDto paramsLeggiGiacenze,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti,
            string? superUserUsername = null)
        {
            DataTable result;
            try
            {
                var filtroVisibilitaUtente = !await _utentiProfili.VisibilitaTotale(
                    objParametriUtenti.UtenteUsername, objParametriUtenti, objParametriServer, (int)Enum_Id_Servizio.GiasOnline);

                // Parameter checks
                var staNum = (paramsLeggiGiacenze.CodCentro != 0 && paramsLeggiGiacenze.CodStalla != 0) ? paramsLeggiGiacenze.CodStalla : 0;
                var raggrCod = (staNum != 0 && paramsLeggiGiacenze.CodRaggruppamento != 0) ? paramsLeggiGiacenze.CodRaggruppamento : 0;         
                bool hasListaAnimali = paramsLeggiGiacenze.CodAnimale == 0 && paramsLeggiGiacenze.ListaCodAnimali != null && paramsLeggiGiacenze.ListaCodAnimali.Any();
                var listaCodAnimali = hasListaAnimali ? paramsLeggiGiacenze.ListaCodAnimali! : new List<int>();

                result = await _operazioniZoo.Leggi_GiacenzeAsync(
                    paramsLeggiGiacenze.Piva,
                    paramsLeggiGiacenze.CodCentro,
                    staNum,
                    raggrCod,
                    paramsLeggiGiacenze.CodAnimale,
                    paramsLeggiGiacenze.DataGiacenza,
                    objParametriServer,
                    paramsLeggiGiacenze.Istantanea,
                    filtroVisibilitaUtente,
                    listaCodAnimali,
                    filtraGiacenze1: true,
                    filtraFornitori: true,
                    paramsLeggiGiacenze.MostraPesate,
                    true,
                    paramsLeggiGiacenze.Matricola ?? string.Empty,
                    paramsLeggiGiacenze.MostraGGPrimoCaricamento,
                    paramsLeggiGiacenze.CFproprietario ?? string.Empty);
            }
            catch (Exception ex)
            {
                LogError(ex.Message,objParametriServer, ex);
                throw;
            }

            return result;
        }

        public async Task<DataTable> LeggiTrattamentiZooAsync(
            GetTrattamentiZooDto filter,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer)
        {
            string? nomeRoutine = MethodBase.GetCurrentMethod()?.Name;
            DataTable result;
            try
            {
                var filtroVisibilitaUtente = !await _utentiProfili.VisibilitaTotale(
                    objParametriUtenti.UtenteUsername, objParametriUtenti, objParametriServer, (int)Enum_Id_Servizio.GiasOnline);

                result = await _operazioniZoo.LeggiTrattamentiAsync(
                     piva: filter.Piva,
                     saCod: filter.CodCentro,
                     staNum: filter.CodStalla,
                     raggruppamentoCod: filter.CodRaggruppamento,
                     codAnimale: filter.CodAnimale,
                     matricola: filter.Matricola,
                     dataInizio: filter.DataInizio,
                     dataFine: filter.DataFine,
                     filtroVisibilitaUtente: filtroVisibilitaUtente,
                     listCodAnimali: filter.ListaCodAnimali,
                     farmCatList: filter.FarmCatList?.ToArray(),
                     farmCatSemplList: filter.FarmCatSemplList?.ToArray(),
                    objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw new Exception("[" + nomeRoutine + "] : " + ex.Message);
            }
            return result;
        }

        public async Task<DataTable> LeggiCapiSenzaTrattamentiAsync(
            GetSenzaTrattamentiZooDto filtro,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer)
        {
            string? nomeRoutine = MethodBase.GetCurrentMethod()?.Name;
            DataTable result;
            try
            {
                var filtroVisibilitaUtente = !await _utentiProfili.VisibilitaTotale(
                    objParametriUtenti.UtenteUsername, objParametriUtenti, objParametriServer, (int)Enum_Id_Servizio.GiasOnline);

                var compatibilityLevel = await _utilityDB.Read_SQL_Compatibility_LevelAsync(objParametriServer);
                result = await _operazioniZoo.LeggiCapiSenzaTrattamentiAsync
                    (
                        filtro.Piva,
                        filtro.CodCentro,
                        filtro.CodStalla,
                        filtro.CodRaggruppamento,
                        filtro.CodAnimale,
                        filtro.Data,
                        filtro.GiorniSenzaTrattamento,
                        filtroVisibilitaUtente: filtroVisibilitaUtente,
                        null,
                        (filtro.FarmCatList ?? new List<int>()).ToArray(),
                        (filtro.FarmCatSemplList ?? new List<int>()).ToArray(),
                        objParametriServer,
                        compatibilityLevel,
                        filtro.MostraGGInizioTotali,
                        filtro.MostraAnomalie
                    );
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw new Exception("[" + nomeRoutine + "] : " + ex.Message);
            }
            return result;
        }

        public async Task<DataTable> LeggiStazionamentoZooAsync(
            GetStazionamentoZooDto filter,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer)
        {
            string? nomeRoutine = MethodBase.GetCurrentMethod()?.Name;
            DataTable result;
            try
            {
                var filtroVisibilitaUtente = !await _utentiProfili.VisibilitaTotale(
                    objParametriUtenti.UtenteUsername, objParametriUtenti, objParametriServer, (int)Enum_Id_Servizio.GiasOnline);

                var compatibilityLevel = await _utilityDB.Read_SQL_Compatibility_LevelAsync(objParametriServer);

                result = await _operazioniZoo.LeggiStazionamentoZooAsync(
                    filter.Piva,
                    filter.CodCentro,
                    filter.CodStalla,
                    filter.CodRaggruppamento,
                    filter.CodAnimale,
                    filter.Data,
                    filter.GiorniStazionamento,
                    filtroVisibilitaUtente: filtroVisibilitaUtente,
                    filter.ListaCodAnimali ?? new List<int>(),
                    objParametriServer,
                    compatibilityLevel
                    );
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw new Exception("[" + nomeRoutine + "] : " + ex.Message);
            }
            return result;
        }

        public async Task<DataTable> LeggiGiacenzeZooDaAAsync(
            string piva,
            int saCod,
            int staNum,
            int codRaggruppamento,
            int codAnimale,
            DateTime periodoInizio,
            DateTime periodoFine,
            bool mostraPesate,
            bool mostraGgInizioTot,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer)
        {
            var nomeRoutine = MethodBase.GetCurrentMethod()?.Name;

            try
            {
                var filtroVisibilitaUtente = !await _utentiProfili.VisibilitaTotale(
                    objParametriUtenti.UtenteUsername,
                    objParametriUtenti,
                    objParametriServer,
                    (int)Enum_Id_Servizio.GiasOnline);

                var periodoGiacenza = periodoInizio.Date.AddDays(1).AddSeconds(-1);

                var dtGiacenze = await _operazioniZoo.Leggi_GiacenzeAsync(
                    piva,
                    saCod,
                    staNum,
                    codRaggruppamento,
                    codAnimale,
                    periodoGiacenza,
                    objParametriServer,
                    bAll: false,
                    Filtro_Visibilita_Utente: filtroVisibilitaUtente,
                    listCod_Animali: null,
                    filtraGiacenze1: true,
                    filtraFornitori: true,
                    mostraPesate: mostraPesate,
                    mostraAnomalie: true,
                    Matricola: string.Empty,
                    MostraGGPrimoCaricamento: mostraGgInizioTot,
                    CFproprietario: string.Empty,
                    listMatricola_Animali: null,
                    leggiUltimaPesata: true);

                var dtCarico = await LeggiCapiCaricoAsync(
                    piva,
                    saCod,
                    staNum,
                    codRaggruppamento,
                    codAnimale,
                    periodoInizio,
                    periodoFine,
                    lavCod: 0,
                    mostraPesate,
                    filtroVisibilitaUtente,
                    objParametriServer);

                var dtScarico = await LeggiCapiScaricoAsync(
                    piva,
                    saCod,
                    staNum,
                    codRaggruppamento,
                    codAnimale,
                    listCodAnimali: null,
                    matricola: string.Empty,
                    periodoInizio,
                    periodoFine,
                    lavCod: 0,
                    mostraPesate,
                    mostraGgInizioTot,
                    filtroVisibilitaUtente,
                    objParametriServer);

                EnsureColumn(dtGiacenze, "Tipo", typeof(string));
                EnsureColumn(dtGiacenze, "Presenza_Nel_Periodo", typeof(int));

                foreach (DataRow row in dtGiacenze.Rows)
                {
                    row["Tipo"] = "Giacenza";
                }

                var matricoleGiacenze = new HashSet<string>(
                    dtGiacenze.AsEnumerable().Select(r => GetString(r, "Matricola")),
                    StringComparer.OrdinalIgnoreCase);

                foreach (DataRow rowCarico in dtCarico.Rows)
                {
                    var matricola = GetString(rowCarico, "Matricola");
                    if (!matricoleGiacenze.Contains(matricola))
                    {
                        var newRow = dtGiacenze.NewRow();
                        CopySharedColumns(rowCarico, newRow);
                        newRow["Tipo"] = "Carico";
                        dtGiacenze.Rows.Add(newRow);
                        matricoleGiacenze.Add(matricola);
                    }
                }

                foreach (DataRow rowScarico in dtScarico.Rows)
                {
                    var matricola = GetString(rowScarico, "Matricola");
                    if (!matricoleGiacenze.Contains(matricola))
                    {
                        var newRow = dtGiacenze.NewRow();
                        CopySharedColumns(rowScarico, newRow);
                        newRow["Tipo"] = "Scarico";
                        dtGiacenze.Rows.Add(newRow);
                        matricoleGiacenze.Add(matricola);
                    }
                }

                var matricoleScarico = new HashSet<string>(
                    dtScarico.AsEnumerable().Select(r => GetString(r, "Matricola")),
                    StringComparer.OrdinalIgnoreCase);

                foreach (DataRow row in dtGiacenze.Rows)
                {
                    var tipo = GetString(row, "Tipo");
                    var matricola = GetString(row, "Matricola");

                    if (string.Equals(tipo, "Giacenza", StringComparison.OrdinalIgnoreCase)
                        && matricoleScarico.Contains(matricola))
                    {
                        tipo = "Scarico";
                        row["Tipo"] = tipo;
                    }

                    var inizio = GetDate(row, "Validita_Inizio");
                    var fine = GetDate(row, "Validita_Fine");
                    var dataNascita = GetDate(row, "Dat_Nascita");

                    if (inizio == DateTime.MinValue || dataNascita == DateTime.MinValue)
                        continue;

                    switch (tipo?.ToUpperInvariant())
                    {
                        case "CARICO":
                            SetIfColumnExists(row, "giorni_in_stalla", DaysDiff(inizio.Date, periodoFine.Date) + 1);
                            SetIfColumnExists(row, "Presenza_Nel_Periodo", DaysDiff(inizio.Date, periodoFine.Date) + 1);
                            SetIfColumnExists(row, "Eta_Giorni_TOTALI", DaysDiff(dataNascita.Date, periodoFine.Date));

                            if (mostraGgInizioTot && HasColumn(row, "giorni_stalla_primo_caricamento") && row["giorni_stalla_primo_caricamento"] != DBNull.Value)
                            {
                                row["giorni_stalla_primo_caricamento"] = Convert.ToInt32(row["giorni_stalla_primo_caricamento"]) + DaysDiff(periodoInizio.Date, periodoFine.Date);
                            }

                            if (mostraPesate
                                && HasColumn(row, "Qta_Ultima_Pesata")
                                && HasColumn(row, "Incremento_Teorico")
                                && HasColumn(row, "Data_Ultima_Pesata")
                                && row["Qta_Ultima_Pesata"] != DBNull.Value
                                && row["Incremento_Teorico"] != DBNull.Value
                                && row["Data_Ultima_Pesata"] != DBNull.Value)
                            {
                                var qtaUltimaPesata = Convert.ToDouble(row["Qta_Ultima_Pesata"]);
                                var incrementoTeorico = Convert.ToDouble(row["Incremento_Teorico"]);
                                var dataUltimaPesata = Convert.ToDateTime(row["Data_Ultima_Pesata"]).Date;
                                var giorniIncremento = DaysDiff(dataUltimaPesata, periodoFine.Date);

                                SetIfColumnExists(row, "Incremento_Teorico_Calcolato", qtaUltimaPesata + (incrementoTeorico * giorniIncremento));
                            }

                            var (mesiCarico, giorniCarico) = CalcoloEtaMesiGiorni(dataNascita.Date, periodoFine.Date);
                            SetIfColumnExists(row, "eta_mesi", mesiCarico);
                            SetIfColumnExists(row, "eta_giorni", giorniCarico);
                            break;

                        case "SCARICO":
                            SetIfColumnExists(row, "giorni_in_stalla", DaysDiff(inizio.Date, fine.Date) + 1);
                            SetIfColumnExists(row, "Presenza_Nel_Periodo", DaysDiff(periodoInizio.Date, fine.Date) + 1);
                            SetIfColumnExists(row, "Eta_Giorni_TOTALI", DaysDiff(dataNascita.Date, fine.Date));

                            if (mostraGgInizioTot && HasColumn(row, "giorni_stalla_primo_caricamento") && row["giorni_stalla_primo_caricamento"] != DBNull.Value)
                            {
                                row["giorni_stalla_primo_caricamento"] = Convert.ToInt32(row["giorni_stalla_primo_caricamento"]) + DaysDiff(periodoInizio.Date, fine.Date);
                            }

                            if (mostraPesate
                                && HasColumn(row, "Qta_Ultima_Pesata")
                                && HasColumn(row, "Incremento_Teorico")
                                && HasColumn(row, "Data_Ultima_Pesata")
                                && row["Qta_Ultima_Pesata"] != DBNull.Value
                                && row["Incremento_Teorico"] != DBNull.Value
                                && row["Data_Ultima_Pesata"] != DBNull.Value)
                            {
                                var qtaUltimaPesata = Convert.ToDouble(row["Qta_Ultima_Pesata"]);
                                var incrementoTeorico = Convert.ToDouble(row["Incremento_Teorico"]);
                                var dataUltimaPesata = Convert.ToDateTime(row["Data_Ultima_Pesata"]).Date;
                                var giorniIncremento = DaysDiff(dataUltimaPesata, fine.Date);

                                SetIfColumnExists(row, "Incremento_Teorico_Calcolato", qtaUltimaPesata + (incrementoTeorico * giorniIncremento));
                            }

                            var (mesiScarico, giorniScarico) = CalcoloEtaMesiGiorni(dataNascita.Date, fine.Date);
                            SetIfColumnExists(row, "eta_mesi", mesiScarico);
                            SetIfColumnExists(row, "eta_giorni", giorniScarico);
                            break;

                        case "GIACENZA":
                        default:
                            SetIfColumnExists(row, "giorni_in_stalla", DaysDiff(inizio.Date, periodoFine.Date) + 1);
                            SetIfColumnExists(row, "Presenza_Nel_Periodo", DaysDiff(periodoInizio.Date, periodoFine.Date) + 1);
                            SetIfColumnExists(row, "Eta_Giorni_TOTALI", DaysDiff(dataNascita.Date, periodoFine.Date));

                            if (mostraGgInizioTot && HasColumn(row, "giorni_stalla_primo_caricamento") && row["giorni_stalla_primo_caricamento"] != DBNull.Value)
                            {
                                row["giorni_stalla_primo_caricamento"] = Convert.ToInt32(row["giorni_stalla_primo_caricamento"]) + DaysDiff(periodoInizio.Date, periodoFine.Date);
                            }

                            if (mostraPesate
                                && HasColumn(row, "Qta_Ultima_Pesata")
                                && HasColumn(row, "Incremento_Teorico")
                                && HasColumn(row, "Data_Ultima_Pesata")
                                && row["Qta_Ultima_Pesata"] != DBNull.Value
                                && row["Incremento_Teorico"] != DBNull.Value
                                && row["Data_Ultima_Pesata"] != DBNull.Value)
                            {
                                var qtaUltimaPesata = Convert.ToDouble(row["Qta_Ultima_Pesata"]);
                                var incrementoTeorico = Convert.ToDouble(row["Incremento_Teorico"]);
                                var dataUltimaPesata = Convert.ToDateTime(row["Data_Ultima_Pesata"]).Date;
                                var giorniIncremento = DaysDiff(dataUltimaPesata, periodoFine.Date);

                                SetIfColumnExists(row, "Incremento_Teorico_Calcolato", qtaUltimaPesata + (incrementoTeorico * giorniIncremento));
                            }

                            var (mesiGiacenza, giorniGiacenza) = CalcoloEtaMesiGiorni(dataNascita.Date, periodoFine.Date);
                            SetIfColumnExists(row, "eta_mesi", mesiGiacenza);
                            SetIfColumnExists(row, "eta_giorni", giorniGiacenza);
                            break;
                    }
                }

                return dtGiacenze;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw new Exception("[" + nomeRoutine + "] : " + ex.Message);
            }
        }

        private async Task<DataTable> LeggiCapiCaricoAsync(
            string piva,
            int saCod,
            int staNum,
            int raggruppamentoCod,
            int codAnimale,
            DateTime dataInizio,
            DateTime dataFine,
            int lavCod,
            bool mostraPesate,
            bool filtroVisibilitaUtente,
            AgronicaCoreParametriServer objParametriServer)
        {
            var compatibilityLevel = await _utilityDB.Read_SQL_Compatibility_LevelAsync(objParametriServer);

            var dt = await _operazioniZoo.LeggiCaricoCapiAsync(
                piva,
                saCod,
                staNum,
                raggruppamentoCod,
                codAnimale,
                listCodAnimali: null,
                matricola: string.Empty,
                dataInizio,
                dataFine,
                lavCod,
                flagFornitore: true,
                flagAziendaUscita: true,
                mostraPesate,
                filtroVisibilitaUtente,
                objParametriServer,
                compatibilityLevel);

            if (!mostraPesate || dt.Rows.Count == 0)
                return dt;

            var listCodsProgetto = dt.AsEnumerable()
                .Where(r => HasColumn(r, "Cod_Progetto") && r["Cod_Progetto"] != DBNull.Value)
                .Select(r => Convert.ToInt32(r["Cod_Progetto"]))
                .Distinct()
                .ToList();

            if (!listCodsProgetto.Any())
                return dt;

            var dtGiacenze = await _operazioniZoo.Leggi_GiacenzeAsync(
                piva,
                saCod,
                staNum,
                raggruppamentoCod,
                0,
                dataFine,
                objParametriServer,
                false,
                filtroVisibilitaUtente,
                listCodsProgetto,
                false,
                false,
                true,
                false,
                string.Empty,
                false,
                string.Empty,
                new List<string>(),
                true);

            EnsureColumn(dt, "Data_Prima_Pesata", typeof(DateTime));
            EnsureColumn(dt, "Qta_Prima_Pesata", typeof(double));
            EnsureColumn(dt, "Data_Ultima_Pesata", typeof(DateTime));
            EnsureColumn(dt, "Qta_Ultima_Pesata", typeof(double));

            foreach (DataRow row in dt.Rows)
            {
                var pivaRiga = GetString(row, "Piva");
                var matricola = GetString(row, "Matricola");

                var giacenza = dtGiacenze.AsEnumerable().FirstOrDefault(r =>
                    string.Equals(GetString(r, "Piva"), pivaRiga, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(GetString(r, "Matricola"), matricola, StringComparison.OrdinalIgnoreCase));

                if (giacenza is null)
                    continue;

                CopyColumnIfExists(giacenza, row, "Data_Prima_Pesata");
                CopyColumnIfExists(giacenza, row, "Qta_Prima_Pesata");
                CopyColumnIfExists(giacenza, row, "Data_Ultima_Pesata");
                CopyColumnIfExists(giacenza, row, "Qta_Ultima_Pesata");
            }

            EnsureColumn(dt, "Tipo", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                row["Tipo"] = "Carico";
            }

            return dt;
        }

        private async Task<DataTable> LeggiCapiScaricoAsync(
            string piva,
            int saCod,
            int staNum,
            int raggruppamentoCod,
            int codAnimale,
            List<int>? listCodAnimali,
            string matricola,
            DateTime dataInizio,
            DateTime dataFine,
            int lavCod,
            bool mostraPesate,
            bool mostraGgInizioTot,
            bool filtroVisibilitaUtente,
            AgronicaCoreParametriServer objParametriServer)
        {
            var compatibilityLevel = await _utilityDB.Read_SQL_Compatibility_LevelAsync(objParametriServer);

            var dtScarico = await _operazioniZoo.LeggiScaricoCapiAsync(
                piva,
                saCod,
                staNum,
                raggruppamentoCod,
                codAnimale,
                listCodAnimali,
                matricola,
                dataInizio,
                dataFine,
                lavCod,
                flagFornitore: true,
                flagAziendaUscita: true,
                mostraPesate,
                filtroVisibilitaUtente,
                objParametriServer,
                compatibilityLevel);

            if ((!mostraPesate && !mostraGgInizioTot) || dtScarico.Rows.Count == 0)
                return dtScarico;

            var listCodsProgetto = dtScarico.AsEnumerable()
                .Where(r => HasColumn(r, "Cod_Progetto") && r["Cod_Progetto"] != DBNull.Value)
                .Select(r => Convert.ToInt32(r["Cod_Progetto"]))
                .Distinct()
                .ToList();

            if (!listCodsProgetto.Any())
                return dtScarico;

            var dtGiacenze = await _operazioniZoo.Leggi_GiacenzeAsync(
                piva,
                saCod,
                staNum,
                raggruppamentoCod,
                0,
                dataFine,
                objParametriServer,
                false,
                filtroVisibilitaUtente,
                listCodsProgetto,
                false,
                false,
                mostraPesate,
                false,
                string.Empty,
                mostraGgInizioTot,
                string.Empty,
                new List<string>(),
                true);

            if (mostraPesate)
            {
                EnsureColumn(dtScarico, "Data_Prima_Pesata", typeof(DateTime));
                EnsureColumn(dtScarico, "Qta_Prima_Pesata", typeof(double));
                EnsureColumn(dtScarico, "Data_Ultima_Pesata", typeof(DateTime));
                EnsureColumn(dtScarico, "Qta_Ultima_Pesata", typeof(double));
            }

            if (mostraGgInizioTot)
            {
                EnsureColumn(dtScarico, "giorni_stalla_primo_caricamento", typeof(int));
            }

            foreach (DataRow row in dtScarico.Rows)
            {
                var pivaRiga = GetString(row, "Piva");
                var matricolaRiga = GetString(row, "Matricola");

                var giacenza = dtGiacenze.AsEnumerable().FirstOrDefault(r =>
                    string.Equals(GetString(r, "Piva"), pivaRiga, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(GetString(r, "Matricola"), matricolaRiga, StringComparison.OrdinalIgnoreCase));

                if (giacenza is null)
                    continue;

                if (mostraPesate)
                {
                    CopyColumnIfExists(giacenza, row, "Data_Prima_Pesata");
                    CopyColumnIfExists(giacenza, row, "Qta_Prima_Pesata");
                    CopyColumnIfExists(giacenza, row, "Data_Ultima_Pesata");
                    CopyColumnIfExists(giacenza, row, "Qta_Ultima_Pesata");
                }

                if (mostraGgInizioTot
                    && HasColumn(giacenza, "data_primo_caricamento")
                    && giacenza["data_primo_caricamento"] != DBNull.Value
                    && HasColumn(row, "Validita_Fine")
                    && row["Validita_Fine"] != DBNull.Value)
                {
                    var dataPrimoCaricamento = Convert.ToDateTime(giacenza["data_primo_caricamento"]).Date;
                    var dataMorte = Convert.ToDateTime(row["Validita_Fine"]).Date;
                    row["giorni_stalla_primo_caricamento"] = DaysDiff(dataPrimoCaricamento, dataMorte);
                }
            }

            EnsureColumn(dtScarico, "Tipo", typeof(string));
            foreach (DataRow row in dtScarico.Rows)
            {
                row["Tipo"] = "Scarico";
            }

            return dtScarico;
        }

        public async Task<DataTable> LeggiScaricoCapiAsync(
            string piva,
            int saCod,
            int staNum,
            int raggruppamentoCod,
            int codAnimale,
            List<int>? listCodAnimali,
            string matricola,
            DateTime dataInizio,
            DateTime dataFine,
            int lavCod,
            bool mostraPesate,
            bool mostraGgInizioTot,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer)
        {
            var filtroVisibilitaUtente = !await _utentiProfili.VisibilitaTotale(
                objParametriUtenti.UtenteUsername,
                objParametriUtenti,
                objParametriServer,
                (int)Enum_Id_Servizio.GiasOnline);

            return await LeggiCapiScaricoAsync(
                piva,
                saCod,
                staNum,
                raggruppamentoCod,
                codAnimale,
                listCodAnimali,
                matricola,
                dataInizio,
                dataFine,
                lavCod,
                mostraPesate,
                mostraGgInizioTot,
                filtroVisibilitaUtente,
                objParametriServer);
        }

        private static void EnsureColumn(DataTable table, string columnName, Type type)
        {
            if (!table.Columns.Contains(columnName))
            {
                table.Columns.Add(new DataColumn(columnName, type));
            }
        }

        private static bool HasColumn(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName);
        }

        private static string GetString(DataRow row, string columnName)
        {
            if (!HasColumn(row, columnName) || row[columnName] == DBNull.Value)
                return string.Empty;

            return row[columnName]?.ToString() ?? string.Empty;
        }

        private static DateTime GetDate(DataRow row, string columnName)
        {
            if (!HasColumn(row, columnName) || row[columnName] == DBNull.Value)
                return DateTime.MinValue;

            return Convert.ToDateTime(row[columnName]);
        }

        private static int DaysDiff(DateTime start, DateTime end)
        {
            return (end.Date - start.Date).Days;
        }

        private static void SetIfColumnExists(DataRow row, string columnName, object value)
        {
            if (HasColumn(row, columnName))
            {
                row[columnName] = value;
            }
        }

        private static void CopyColumnIfExists(DataRow source, DataRow target, string columnName)
        {
            if (HasColumn(source, columnName) && HasColumn(target, columnName) && source[columnName] != DBNull.Value)
            {
                target[columnName] = source[columnName];
            }
        }

        private static void CopySharedColumns(DataRow source, DataRow target)
        {
            foreach (DataColumn targetColumn in target.Table.Columns)
            {
                if (source.Table.Columns.Contains(targetColumn.ColumnName) && source[targetColumn.ColumnName] != DBNull.Value)
                {
                    target[targetColumn.ColumnName] = source[targetColumn.ColumnName];
                }
            }
        }

        private static (int mesi, int giorni) CalcoloEtaMesiGiorni(DateTime dataNascita, DateTime dataRiferimento)
        {
            var d1 = dataNascita.Date;
            var d2 = dataRiferimento.Date;
            var sign = 1;

            if (d2 < d1)
            {
                sign = -1;
                var tmp = d1;
                d1 = d2;
                d2 = tmp;
            }

            var years = d2.Year - d1.Year;
            var months = d2.Month - d1.Month;
            var days = d2.Day - d1.Day;

            if (days < 0)
            {
                months -= 1;
                var prev = d2.AddMonths(-1);
                days += DateTime.DaysInMonth(prev.Year, prev.Month);
            }

            if (months < 0)
            {
                years -= 1;
                months += 12;
            }

            var totalMonths = (years * 12 + months) * sign;
            var totalDays = days * sign;

            return (totalMonths, totalDays);
        }

        public async Task<DataTable> LeggiGiacenzeZooFirstSommAsync(LeggiGiacenzeZooFirstSommDto dtoParams, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            DataTable result;

            try
            {
                result = await LeggiGiacenzeZooAsync(dtoParams, objP_Server, objP_Utenti);  

                if (result != null && result.Rows.Count > 0)
                {
                    result.Columns.Add("OnAntibiotico", typeof(bool));
                    result.Columns.Add("OnAntinfiammatorio", typeof(bool));

                    var dtTrattInCorso = await _operazioniZoo.LeggiTrattamentiCorrenti(dtoParams.Piva, dtoParams.CodCentro, dtoParams.CodStalla, dtoParams.DataGiacenza, objP_Server);
                    if (dtTrattInCorso != null && dtTrattInCorso.Rows.Count > 0)
                    {
                        foreach (DataRow rowGiacenza in result.Rows)
                        {
                            int codAnimale = rowGiacenza.Field<int>("Cod_Animale");
                            rowGiacenza["OnAntibiotico"] = false;
                            rowGiacenza["OnAntinfiammatorio"] = false;

                            var rowsTrattInCorso = dtTrattInCorso.AsEnumerable().Where(r => r.Field<int>("Cod_Animale") == codAnimale).ToList();
                            if (rowsTrattInCorso != null && rowsTrattInCorso.Count > 0)
                            {
                                rowGiacenza["OnAntibiotico"] = rowsTrattInCorso.Any(r => (int)r["Antibiotico"] == 1);
                                rowGiacenza["OnAntinfiammatorio"] = rowsTrattInCorso.Any(r => (int)r["Antinfiammatorio"] == 1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex: ex);
                throw;
            }


            return result ?? new DataTable();
        }

    }
}
