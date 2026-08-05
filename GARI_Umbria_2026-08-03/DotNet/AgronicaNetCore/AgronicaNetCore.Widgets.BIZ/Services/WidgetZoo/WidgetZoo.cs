using System.Data;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using AgronicaCoreDTOStd.InData.Zoo;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using AgronicaNetCore.Widgets.DAL.DataLayer.WidgetsZoo;
using AgronicaNetCore.Widgets.BIZ.Resources;
using AgronicaNetCore.Zoo.BIZ.Services.Raggruppamenti;
using AgronicaNetCore.OperazioniZoo.BIZ.Services.OperazioniZoo;
using AgronicaNetCore.Zoo.DAL.DataLayer.Prescrizioni;
using AgronicaNetCore.Base.Constants;
using System.Text.RegularExpressions;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiProfili;
using AgronicaNetCore.Magazzino.DAL.DataLayer;
using AgronicaNetCore.Magazzino.BIZ.Services;

namespace AgronicaNetCore.Widgets.BIZ.Services.WidgetZoo
{
    public class WidgetZoo : BaseServiceWidgetsBIZ, IWidgetZoo
    {
        private const int GIORNI_VALIDITA_CAPO = 5;

        private readonly IWidgetsZoo _widgetsDal;
        private readonly IMagazzino _magazziniDal;
        private readonly IPrescrizioni _prescrizioniDal;
        private readonly IOperazioniZooService _opZooBiz;
        private readonly IMagazzinoService _magazziniBiz;
        private readonly IUtentiProfili _utentiProfili;
        private readonly IUtentiImpostazioni _utentiImpostazioni;
        private readonly IRaggruppamentiService _raggruppamentiBiz;
        private readonly IUtentiVisibilitaAppoggio _utentiVisibilitaAppoggio;

        public WidgetZoo(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _widgetsDal = _serviceProvider.GetRequiredService<IWidgetsZoo>();
            _magazziniDal = _serviceProvider.GetRequiredService<IMagazzino>();
            _prescrizioniDal = _serviceProvider.GetRequiredService<IPrescrizioni>();
            _magazziniBiz = _serviceProvider.GetRequiredService<IMagazzinoService>();
            _opZooBiz = _serviceProvider.GetRequiredService<IOperazioniZooService>();
            _utentiProfili = _serviceProvider.GetRequiredService<IUtentiProfili>();
            _utentiImpostazioni = _serviceProvider.GetRequiredService<IUtentiImpostazioni>();
            _raggruppamentiBiz = _serviceProvider.GetRequiredService<IRaggruppamentiService>();
            _utentiVisibilitaAppoggio = _serviceProvider.GetRequiredService<IUtentiVisibilitaAppoggio>();
        }

        private DataTable GetAndCleanResultScadenzaFarmaci(DataTable dtGiacenze)
        {
            var result = dtGiacenze.Clone();
            result.Clear();

            result.Columns.Remove("Tabella");
            result.Columns.Remove("NomeComune");
            result.Columns.Remove("Tabella_Cod");
            result.Columns.Remove("Tabella_Des");
            result.Columns.Remove("Tabella_Tipo");
            result.Columns.Remove("Elem_Cod");
            result.Columns.Remove("Mat_Cod");
            result.Columns.Remove("Cod_Progetto");
            result.Columns.Remove("Fase_Cod");
            result.Columns.Remove("Cal_Cod");
            result.Columns.Remove("LegatoALinea");
            result.Columns.Remove("Veg_Cod");
            result.Columns.Remove("Cul_Cod");
            result.Columns.Remove("Regolamento");
            result.Columns.Remove("sem_cod");
            result.Columns.Remove("GRVA_COD_VEG");
            result.Columns.Remove("cat_cod");
            result.Columns.Remove("Veg_Des");
            result.Columns.Remove("Cul_Des");
            result.Columns.Remove("Otabella_Cod_Base");
            result.Columns.Remove("Codice_Esterno");
            result.Columns.Remove("Cod_TecnologiaSementi");
            result.Columns.Remove("Germinabilita");
            result.Columns.Remove("Mat_Cod_OMNI");

            result.Columns.Add(new DataColumn("Data_Scadenza", typeof(DateTime)));

            return result;
        }

        public async Task<DataTable?> GetInvalidAnimals(string piva, int saCod, int staNum, DateTime timeStart, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            DataTable? res = null;

            try
            {
                if (_widgetsDal == null || _utentiVisibilitaAppoggio == null) 
                    throw new Exception("Riferimento mancante per DAL Widget: impossibile proseguire.");

                //DataTable? dtUVisibility = await _utentiVisibilitaAppoggio.ReadAsync((int)Enum_TipoEntita.Fabbricato, objP_Server);
                //bool userVisibility = dtUVisibility != null && dtUVisibility.Rows.Count > 0;

                //DataTable? dtUSettings = await _utentiImpostazioni.ReadAsync((int)Enum_Impostazioni_Utenti.SUPERUSER_Documentale_GestioneWorkFlow, 2, objP_Utenti, objP_Server);
                //bool useWorkflow = dtUSettings != null && dtUSettings.Rows.Count > 0;

                var raggrCod = (string.IsNullOrWhiteSpace(piva) || saCod == 0 || staNum == 0) ? 0 : await _raggruppamentiBiz.ReadRaggruppamentoBDNFromStallaAsync(piva, saCod, staNum, objP_Server);

                var dt = await _opZooBiz.LeggiGiacenzeZooAsync(
                    new LeggiGiacenzeZooDto { 
                        Piva = piva, 
                        CodCentro = saCod, 
                        CodStalla = staNum, 
                        CodRaggruppamento = raggrCod, 
                        DataGiacenza = timeStart
                    }, 
                    objP_Server, objP_Utenti);
                dt.Columns.Add(new DataColumn("alertGG", typeof(bool)));

                var invalidRows = dt.AsEnumerable()
                    .Where(row => row.Field<string>("Validato") == "No")
                    .ToList();

                if (invalidRows == null || invalidRows.Count == 0) return dt.Clone();

                invalidRows.ForEach(row => {
                    DateTime dataValida = row.Field<DateTime>("Validita_Inizio").AddDays(GIORNI_VALIDITA_CAPO).Date;
                    row["alertGG"] = (dataValida.Date < DateTime.Now.Date);
                });
                res = invalidRows.CopyToDataTable();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP_Server, ex);
                throw;
            }
            return res;
        }

        public async Task<DataTable?> GetTreatmentsToDo(string piva, int saCod, int staNum, DateTime timeStart, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            DataTable? res = null;
            try
            {
                if (_widgetsDal == null || _utentiVisibilitaAppoggio == null) throw new Exception("Riferimento mancante per DAL Widget: impossibile proseguire.");

                //DataTable? dtUVisibility = await _utentiVisibilitaAppoggio.ReadAsync((int)Enum_TipoEntita.Fabbricato, objP_Server);
                //bool userVisibility = dtUVisibility != null && dtUVisibility.Rows.Count > 0;

                //DataTable? dtUSettings = await _utentiImpostazioni.ReadAsync((int)Enum_Impostazioni_Utenti.SUPERUSER_Documentale_GestioneWorkFlow, 2, objP_Utenti, objP_Server);
                //bool useWorkflow = dtUSettings != null && dtUSettings.Rows.Count > 0;

                res = await _prescrizioniDal.ReadProtocolliInCorso_Grid(piva, saCod, staNum, timeStart, objP_Server, objP_Utenti);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP_Server, ex);
                throw;
            }

            return res;
        }

        public async Task<DataTable?> GetTreatmentsToSend(string piva, int saCod, int staNum, DateTime timeStart, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            DataTable? res = null;
            try
            {
                if (_widgetsDal == null || _utentiVisibilitaAppoggio == null) throw new Exception("Riferimento mancante per DAL Widget: impossibile proseguire.");

                //DataTable? dtUVisibility = await _utentiVisibilitaAppoggio.ReadAsync((int)Enum_TipoEntita.Fabbricato, objP_Server);
                //bool userVisibility = dtUVisibility != null && dtUVisibility.Rows.Count > 0;

                //DataTable? dtUSettings = await _utentiImpostazioni.ReadAsync((int)Enum_Impostazioni_Utenti.SUPERUSER_Documentale_GestioneWorkFlow, 2, objP_Utenti, objP_Server);
                //bool useWorkflow = dtUSettings != null && dtUSettings.Rows.Count > 0;

                bool userVisibility = await _utentiProfili.VisibilitaTotaleGiasOnline(objP_Utenti, objP_Server);

                //timeStart = timeStart.Date;
                res = await _widgetsDal.GetTrattamentiToSend(piva, saCod, staNum, null, null, objP_Server, 0, "", "", filtroVisibilitaUtente: userVisibility);                
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP_Server, ex);
                throw;
            }
            return res;
        }

        public async Task<DataTable?> GetExpiringDrugs(string piva, DateTime timeStart, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            DataTable? res = null;
            try
            {
                if (_widgetsDal == null || _utentiVisibilitaAppoggio == null) throw new Exception("Riferimento mancante per DAL Widget: impossibile proseguire.");

                //DataTable? dtUVisibility = await _utentiVisibilitaAppoggio.ReadAsync((int)Enum_TipoEntita.Fabbricato, objP_Server);
                //bool userVisibility = dtUVisibility != null && dtUVisibility.Rows.Count > 0;

                //DataTable? dtUSettings = await _utentiImpostazioni.ReadAsync((int)Enum_Impostazioni_Utenti.SUPERUSER_Documentale_GestioneWorkFlow, 2, objP_Utenti, objP_Server);
                //bool useWorkflow = dtUSettings != null && dtUSettings.Rows.Count > 0;

                var dtGiacenze = await _magazziniDal.SchedaGiacenzeMagazzino(
                    timeStart.Date, piva, Sa_Cod: 0, 0, 
                    ELEM_COD.FARMACI, 0, 0, 0, 0, 0, 0, 
                    COSTANTI_GENERALI.LOTTO_NONDEFINITO, 
                    Flag_QtaNoZero: true, 
                    null, null, null, null, null, null, null, null, null, null, null, null, "", 
                    objP_Server, objP_Utenti, 
                    Flag_QtaMaggioreZero: true
                );

                var timeEnd = timeStart.AddDays(30).Date;
                var dtResult = GetAndCleanResultScadenzaFarmaci(dtGiacenze);

                foreach (var row in dtGiacenze.AsEnumerable())
                {
                    string lotto = row.Field<string>("Lotto") ?? string.Empty;

                    if (String.IsNullOrEmpty(lotto)) continue;

                    Match match = Regex.Match(lotto, @"\d{4}-\d{2}-\d{2}", RegexOptions.None, TimeSpan.FromSeconds(3));

                    if (match.Success)
                    {
                        if (DateTime.TryParse(match.Value, out DateTime expDate))
                        {
                            if (expDate.Date > timeStart && expDate.Date <= timeEnd.Date)
                            {
                                var newRow = dtResult.NewRow();
                                foreach (DataColumn col in dtResult.Columns)
                                {
                                    if (col.ColumnName == "Data_Scadenza")
                                        newRow["Data_Scadenza"] = expDate.Date;
                                    else if (dtGiacenze.Columns.Contains(col.ColumnName))
                                        newRow[col.ColumnName] = row[col.ColumnName];
                                }
                                dtResult.Rows.Add(newRow);
                            }
                        }
                    }
                }

                res = dtResult;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP_Server, ex);
                throw;
            }
            return res;
        }
    }
}
