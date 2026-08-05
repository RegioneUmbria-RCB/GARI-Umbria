using AgronicaCoreModelsSTD.attivita;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Operazione.BIZ.Resources;
using AgronicaCoreDTOStd.InData.ActivityImport;
using System.Data;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazione;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impianti;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Esercizi;
using AgronicaDataProvider6.Extensions;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaCoreModelsSTD.attivita.Info;
using AgronicaNetCore.Operazione.DAL.Resources.UtilityAgenda;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.attivita.dettagli;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Operazione.BIZ.Services.ActivityImport.Mapper
{
    public abstract class ActivityImportMapperTemplate<T>  : BaseServiceOperazioneBIZ, IActivityImportMapper<T> where T : IActivityImportData
    {
        private const int DEFAULT_RACCOGLITORE = 0;

        protected T _base;
        protected AgronicaCoreParametriServer _server;

        protected string _origin = "";
        /// <see cref="TipiEnumerativi.Enum_Anagrafe_CodiciCliente"/>
        protected int _codiceAnagrafeCliente = 0;
        /// <summary>
        /// Contiene il mapping corrente dell'attività.
        /// Il dato è accessibile durante le varie fasi del mapping.
        /// </summary>
        protected Attivita _activity;
        protected InfoOperazione _infoOperazione;

        protected readonly IOperazione _operazioni;
        protected readonly IImpianti _impianti;
        protected readonly IEsercizi _esercizi;
        protected readonly IUtilityAgendaClassInitializer _utility;

        //T baseObject,
        public ActivityImportMapperTemplate(IServiceProvider provider, IStringLocalizer<Messages> localizer):base (provider, localizer)
        {
            _operazioni = provider.GetRequiredService<IOperazione>();
            _impianti = provider.GetRequiredService<IImpianti>();
            _esercizi = provider.GetRequiredService<IEsercizi>();
            _utility = provider.GetRequiredService<IUtilityAgendaClassInitializer>();
        }

        private async Task CheckOperation()
        {
            DataTable dt = await _operazioni.LeggiAsync(_base.tipo_operazione, "", "", _server);
            const string colName = "Lav_Des";
            if (dt.Rows.Count != 1 || dt.Rows[0][colName] == "")
            {
                throw new InvalidOperationException(_localizer.GetString("TipoOperazioneNonEsiste", _base.tipo_operazione).Value);
            }
        }

        protected virtual async Task SetActivityBaseData(int raccoglitore)
        {
            _activity.tipo = Attivita.Tipo_Attivita.QuadernoDiCampagna;
            _activity.disciplinare = new AgronicaCoreModelsSTD.metaschema.Disciplinare("0");
            _activity.codice = "0"; // D2G-->inifluente, l'aggancio con l'eventuale attività esistente passa attraverso il codice Demetra
            _activity.raccoglitore = raccoglitore;

            _activity.cancellato = _base.flag_cancellazione;
            _activity.inizio = _base.data;
            _activity.note = _base.note;

            await CheckOperation();
            _activity.job = new Lavorazione(_base.tipo_operazione);
            if (_base.tipo_operazione == LAV_COD.LAVCOD_ALTRE_OPERAZIONI)
            {
                _activity.attivitaPersonalizzata = new AttivitaPersonalizzata(1);
                // DT: per ora fisso, valutare se sincronizzare le attività associate al lav_Cod 162
            }

            _activity.origine = _origin;
        }

        private void SetCompanyCenterFromPlants()
        {
            CentroAziendale.PK? pk = _activity.centriDiCosto.Cast<EsercizioCDC>()
                .Select(ex => ex.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK)
                .FirstOrDefault();
            _activity.centroAziendale = new CentroAziendale(pk);
        }

        private async Task<UtilizzoTerreno?> GetUtilizzoTerreno(Impianto.PK plot)
        {
            if (plot.codice <= 0 || plot.appezza <= 0 || plot.saCod <= 0 || plot.piva == "")
                return null;

            DataTable? cultivar = await _impianti.LeggiInfoVarietaAsync(plot.piva, plot.saCod, plot.appezza, plot.codice, _server);
            if (cultivar == null || cultivar.Rows.Count <= 0)
                return null;

            if ((int)cultivar.Rows[0]["Cul_Cod"] != 0)
            {
                return new Varieta(
                    (int)cultivar.Rows[0]["Cul_Cod"],
                    cultivar.Rows[0]["Cul_Des"].ToString(),
                    new Specie((int)cultivar.Rows[0]["Veg_Cod"], cultivar.Rows[0]["Veg_Des"].ToString())
                );
            }
            else
            {
                UtilizzoTerreno? soilUse = null;
                // TODO: leggere da codici??
                // if lettura da codici ritorna qualcosa istnaziarlo come destinazione uso
                // else
                soilUse = new DestinazioneUso(0);
                soilUse.descrizione = _localizer.GetString("TerrenoNudo").Value;
                return soilUse;
            }
        }

        private async Task SetUtilizzoTerrenoFromPlants()
        {
            Impianto.PK? pk = _activity.centriDiCosto.Cast<EsercizioCDC>()
                .Select(ex => ex.esercizio.impiantoPK)
                .FirstOrDefault();
            if (pk != null)
            {
                _activity.utilizzoTerreno = await GetUtilizzoTerreno(pk);
                if (_activity.utilizzoTerreno == null)
                {
                    throw new InvalidOperationException(_localizer.GetString("ImpiantoSenzaSpecie", pk.toString("-")).Value);
                }
            }
        }

        private async Task MapPlants(string piva)
        {
            var dict = new Dictionary<string, (string, EsercizioCDC)>();
            _activity.centriDiCosto = new List<CentroDiCosto>();
            if (_base.impianti != null)
            {
                foreach (IActivityImportPlant impianto in _base.impianti)
                {
                    EsercizioCDC ex = await GetEsercizioCDCForActivity(impianto, piva);
                    _activity.centriDiCosto.Add(ex);

                    if (!dict.TryAdd(impianto.plot_id, (ex.esercizio.impiantoPK.toString("-"), ex)))
                    {
                        throw new DuplicateNameException(_localizer.GetString("ImpiantoDuplicato", impianto.plot_id).Value);
                    }
                }
                _activity.risorse = new List<AgronicaCoreModelsSTD.attivita.risorse.Risorsa>();
            }
        }

        protected virtual async Task<Impianto.PK> CheckPlantValid(IActivityImportPlant plant, string piva)
        {
            DataTable? results = await _impianti.LeggiConCodiciAsync(_server, piva, idCod: _codiceAnagrafeCliente, valCod: plant.plot_id);
            if (results == null || results.Rows.Count == 0)
                throw new InvalidOperationException(_localizer.GetString("ImpiantoNonEsistente", plant.plot_id).Value);
            else if (results.Rows.Count > 1)
                throw new DuplicateNameException(_localizer.GetString("ImpiantoNonUnivoco", plant.plot_id).Value);

            DataRow plotRow = results.Rows[0];
            Impianto.PK key = new Impianto.PK(
                int.Parse(plotRow["Id_Reg"].ToString() ?? "0"),
                int.Parse(plotRow["Appezza"].ToString() ?? "0"),
                int.Parse(plotRow["Sa_Cod"].ToString() ?? "0"),
                plotRow["Piva"].ToString() ?? ""
            );

            DateTime inizio = DateTime.Parse(plotRow["Validita_Inizio"].ToString() ?? CostantiPersonalizzate.AGRODATAINIZIO);
            DateTime fine = DateTime.Parse(plotRow["Validita_Fine"].ToString() ?? CostantiPersonalizzate.AGRODATAFINE);

            if (!_activity.inizio.IsInRange(inizio, fine))
            {
                throw new ArgumentOutOfRangeException(_localizer.GetString(
                    "ImpiantoNonValidoInData", plant.plot_id, key.toString("-"), _activity.inizio.ToString("dd/MM/yyyy")
                ).Value);
            }

            return key;
        }

        private async Task<EsercizioCDC> GetEsercizioCDCForActivity(IActivityImportPlant plant, string piva)
        {
            Impianto.PK plotKey = await CheckPlantValid(plant, piva);
            
            DataTable? esercizi = await _esercizi.LeggiAsync(
                _server, plotKey.piva, plotKey.saCod, plotKey.appezza, plotKey.codice,
                inizio: _activity.inizio, fine: _activity.inizio
            );
            if (esercizi == null || esercizi.Rows.Count == 0)
                throw new InvalidOperationException(_localizer.GetString("EsercizioNonEsistente", plant.plot_id).Value);

            int progCod = (int)esercizi.Rows[0]["Progetto_Cod"];

            EsercizioCDC cdc = new();
            cdc.superficieTrattata = plant.superficie_trattata;
            cdc.esercizio = new Esercizio(progCod, "");
            cdc.esercizio.impiantoPK = plotKey;

            return cdc;
        }

        /// <summary>
        /// Setta la risorsa acqua se l'operazione è di tipo trattamento o fertilizzazione.
        /// </summary>
        protected virtual void SetWater()
        {
            if (_infoOperazione.IsTrattamento || _infoOperazione.IsFertilizzazione)
            {
                RisorsaAcqua water = new();
                water.acqua = 0;
                water.doseAcqua = RisorsaAcqua.TipoDoseAcqua.TOTALE;
                //// TODO case in subclass with _base.acqua != null
                ////
                ////  risorsaAcqua.acqua = attivitaDemetra.acqua.quantita
                ////  If attivitaDemetra.acqua.tipo = TipoAcqua.hl_totale Then
                ////      risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.TOTALE
                ////  Else
                ////      risorsaAcqua.acqua = attivitaDemetra.acqua.quantita
                ////      risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.HA
                ////  End If
                ////
                _activity.risorse.Add(water);
            }
        }

        /// <summary>
        /// Valorizza i dati base della risorsa e ritorna il dettaglio base.
        /// </summary>
        /// <param name="baseDetail">Nuova istanza del dettaglio da aggiungere</param>
        protected RisorsaProdotto ValorizaBaseDetail(RisorsaProdotto baseDetail)
        {
            baseDetail.prodotto = new Prodotto(-1, _infoOperazione.Elem_Cod);
            baseDetail.unitaDiMisuraIndicata = new();
            baseDetail.unitaDiMisuraIndicata.codice = (int)TipiEnumerativi.Enum_UnitaMisura.KG;
            return baseDetail;
        }

        protected virtual void SetProducts()
        {
            // TODO case in subclass with _base.prodotti != null && _base.prodotti.Count() > 0
            if (_infoOperazione.IsTrattamento)
                _activity.risorse.Add(ValorizaBaseDetail(new DettaglioTrattamento()));
            else if (_infoOperazione.IsFertilizzazione)
                _activity.risorse.Add(ValorizaBaseDetail(new DettaglioFertilizzazione()));
        }

        protected abstract void SetHarvests();

        protected abstract void SetIrrigations();

        protected abstract void SetMachines();

        protected abstract void SetOperators();

        protected abstract void SetOrigin();

        public void SetBase(T baseObject)
        {
            _base = baseObject;
            SetOrigin();
        }

        public async Task<Attivita> MapActivity(AgronicaCoreParametriServer objParametriServer)
        {
            return await MapActivity("", DEFAULT_RACCOGLITORE, objParametriServer);
        }

        public async Task<Attivita> MapActivity(string piva, int raccoglitore, AgronicaCoreParametriServer objParametriServer)
        {
            _server = objParametriServer;
            _activity = new Attivita();
            _infoOperazione = _utility.GetInfoOperazione(_base.tipo_operazione, Attivita.Tipo_Attivita.QuadernoDiCampagna);
            await SetActivityBaseData(raccoglitore);
            await MapPlants(piva);
            SetCompanyCenterFromPlants();
            await SetUtilizzoTerrenoFromPlants();

            // Overridable methods -> can be overridden in subclasses
            SetWater();
            SetProducts();
            SetHarvests();
            SetIrrigations();
            SetMachines();
            SetOperators();

            return _activity;
        }

    }
}
