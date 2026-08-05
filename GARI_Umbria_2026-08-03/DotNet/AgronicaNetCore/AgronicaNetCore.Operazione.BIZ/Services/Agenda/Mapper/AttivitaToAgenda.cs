using AgronicaCoreDTOStd.InData.Agenda.OperazioneAgenda_Temp;
using AgronicaCoreDTOStd.InData.Budget;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.Info;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.costanti;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impianti;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Lavorazione;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Operazione.BIZ.Resources;
using AgronicaNetCore.Operazione.BIZ.Services.Movimenti.Factory;
using AgronicaNetCore.Operazione.BIZ.Services.Movimenti_Dettagli.Factory;
using AgronicaNetCore.Operazione.DAL.Resources.UtilityAgenda;
using InData.Anagrafica;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;

namespace AgronicaNetCore.Operazione.BIZ.Services.Agenda.Mapper
{
    public class AttivitaToAgenda : BaseServiceOperazioneBIZ, IAttivitaToAgenda
    {
        private Attivita _attivita;
        private OperazioneAgenda _agenda;
        private InfoOperazione _info;
        private AgronicaCoreParametriServer _server;

        private readonly IUtilityAgendaClassInitializer _utility;
        private readonly ILavorazione _lavorazione;
        private readonly IImpianti _impianti;
        private readonly IMovimentiFactory _movFactory;
        private readonly IMovDettagliFactory _movDettagliFactory;

        public AttivitaToAgenda(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _utility = provider.GetRequiredService<IUtilityAgendaClassInitializer>();
            _movFactory = provider.GetRequiredService<IMovimentiFactory>();
            _movDettagliFactory = provider.GetRequiredService<IMovDettagliFactory>();
            _lavorazione = provider.GetRequiredService<ILavorazione>();
            _impianti = provider.GetRequiredService<IImpianti>();
        }

        private OperazioneAgenda InitializeAgenda()
        {
            OperazioneAgenda agenda = new();
            agenda.Data = _attivita.inizio;
            agenda.Piva = _attivita.centroAziendale.primaryKey.partitaIva;
            agenda.Sa_Cod = _attivita.centroAziendale.primaryKey.codice;
            agenda.Lav_Cod = _attivita.job.getCodice();
            agenda.Raccoglitore_Cod = _attivita.raccoglitore;
            agenda.Invia_App = _attivita.inviaRicetta ? 1 : 0;
            agenda.Stato_Cod = (int)_attivita.statoWorkflow;
            agenda.DaRemoto = _attivita.daRemoto ? 1 : 0;
            agenda.Origine = _attivita.origine;
            agenda.Blocco_Flag = _attivita.blocco != null ? (int)_attivita.blocco.tipo : 0;
            agenda.Blocco_Username = _attivita.blocco != null ? _attivita.blocco.utente : "";
            agenda.Blocco_Data = _attivita.blocco != null ? _attivita.blocco.data : CostantiPersonalizzate.AGRODATAINIZIO_DATE;
            if (!String.IsNullOrWhiteSpace(_attivita.codice) && _attivita.codice != "0")
            {
                agenda.Id_Attivita = int.Parse(_attivita.codice);
                agenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica;
            }
            _info = _utility.GetInfoOperazione(agenda.Lav_Cod, _attivita.tipo);
            agenda.BaseCode = _info.BaseCode;
            agenda.TopCode = _info.TopCode;
            return agenda;
        }

        private void SetActivityHarvestOptions()
        {
            bool isRicettaBrogliaccio = _attivita.tipo != Attivita.Tipo_Attivita.QuadernoDiCampagna;
            if (_info.IsRaccolta && isRicettaBrogliaccio)
            {
                _attivita.risorse.FindAll(c => c.classType == ClassType.DettaglioRaccolta)
                    .ConvertAll(obj => (DettaglioRaccolta)obj)
                    .ForEach(dett =>
                    {
                        dett.Opzioni_Raccolta.Ripartizione = Opzioni_Raccolta.enum_Ripartizione_Raccolta.AUTO_SUPERFICIE;
                        dett.Opzioni_Raccolta.GenerazioneLotto = Opzioni_Raccolta.enum_Generazione_Lotto_Raccolta.MANUALE;
                        dett.Opzioni_Raccolta.Modalita = Opzioni_Raccolta.enum_Modalita_Raccolta.MECCANICA;
                    });
            }
        }

        private string GetWkt()
        {
            Func<double, string> format = x => x.ToString().Replace(",", ".").Trim();
            if (_attivita.latitude != 0 && _attivita.longitude != 0)
            {
                return $"POINT({format(_attivita.longitude)} {format(_attivita.latitude)})";
            }
            return "";
        }

        private Specie GetSpecieFromUtilizzoTerreno(UtilizzoTerreno utilizzo)
        {
            if (utilizzo != null && utilizzo.classType == ClassType.Varieta)
                return ((Varieta)utilizzo).specie;
            else
                return new(0);
        }

        private decimal GetTotalTreatedArea()
        {
            if (_attivita.centriDiCosto == null)
                return 0;

            if (_info.TipoCentroDiCosto == Tipo.ProdottoDaTrattare)
            {
                return _attivita.centriDiCosto.Where(cdc => cdc is ProdottoDaTrattareCDC)
                    .Cast<ProdottoDaTrattareCDC>()
                    .Select(p => p.qtaTrattata)
                    .Aggregate(decimal.Add);
            }
            else
            {
                return _attivita.centriDiCosto.Where(cdc => cdc is EsercizioCDC)
                    .Cast<EsercizioCDC>()
                    .Select(cdc => cdc.superficieTrattata)
                    .Aggregate(decimal.Add);
            }
        }

        private bool isRilievoSenzaImpianti()
        {
            if (_attivita == null || _attivita.job == null)
                return false;

            int[] keyList = _attivita.job.primaryKey.codice.Split('|')
                                .Where(p => int.TryParse(p, out _))
                                .Select(int.Parse)
                                .ToArray(); 

            bool isRilievoMaturitaODanni = keyList.Any(x => x == LAV_COD.LAVCOD_RILIEVO_INDICI_MATURITA || x == LAV_COD.LAVCOD_DANNI_RACCOLTA);

            if (keyList == null || keyList.Length == 0 || !isRilievoMaturitaODanni)
                return false;

            Specie specie = GetSpecieFromUtilizzoTerreno(_attivita.utilizzoTerreno);
            if (specie != null && specie.codice != CostantiPersonalizzate.NessunaSpecieQdC)
                return false;

            List<EsercizioCDC> cdcs = _attivita.centriDiCosto.FindAll(cdc => cdc.classType == ClassType.EsercizioCDC)
                .ConvertAll(obj => (EsercizioCDC)obj);
            if (cdcs != null && cdcs.Count() != 0)
                return false;

            List<DettaglioRilievo> dets = _attivita.risorse.FindAll(det => det.classType == ClassType.DettaglioRilievo)
                .ConvertAll(obj => (DettaglioRilievo)obj);
            if (dets == null || dets.Count() == 0)
                return false;

            IEnumerable<DettaglioRilievo> detsNoPlants = dets.Where(d => 
                d.esercizioCDC == null || (
                    d.esercizioCDC.esercizio.codice == 0 &&
                    d.esercizioCDC.esercizio.impiantoPK.codice == 0 &&
                    d.esercizioCDC.esercizio.impiantoPK.appezza == 0
                ));
            return dets.Count() == detsNoPlants.Count();
        }

        private decimal CheckTotalTreatedArea()
        {
            decimal totalArea = GetTotalTreatedArea();
            if (totalArea <= 0 && !_info.isNonUtilizzo && !_info.IsVisita && !isRilievoSenzaImpianti())
            {
                if (_info.TipoCentroDiCosto == Tipo.ProdottoDaTrattare)
                    throw new InvalidOperationException(_localizer.GetString("QtaProdottoTrattataMaggioreZero").Value);
                else
                    throw new InvalidOperationException(_localizer.GetString("SuperficieTrattataMaggioreZero").Value);
            }
            return totalArea;
        }

        private async Task<string> GetDescrizioneSpecieVarieta()
        {
            if (_attivita.utilizzoTerreno == null)
                return "";

            HashSet<string> cultivars = new();
            string specie = "";
            var cdcs = _attivita.centriDiCosto.Where(cdc => cdc.classType == ClassType.EsercizioCDC)
                .Cast<EsercizioCDC>();
            foreach (EsercizioCDC cdc in cdcs)
            {
                DataTable? infoSpecie = await _impianti.LeggiInfoVarietaAsync(
                    cdc.esercizio.impiantoPK.piva, cdc.esercizio.impiantoPK.saCod,
                    cdc.esercizio.impiantoPK.appezza, cdc.esercizio.impiantoPK.codice,
                    _server
                );
                if (infoSpecie != null && infoSpecie.Rows.Count > 0)
                {
                    specie = infoSpecie.Rows[0]["Veg_Des"].ToString() ?? "";
                    cultivars.Add(infoSpecie.Rows[0]["Cul_Des"].ToString() ?? "");
                }
            }
            string cultivarConcat = string.Join(", ", cultivars);

            if (_attivita.utilizzoTerreno.classType == ClassType.Varieta)
            {
                string start = specie != "" ? " (" + specie : "";
                string mid = cultivarConcat != "" ? " [" + cultivarConcat + "]" : "";
                string end = specie != "" ? ")" : "";
                return start + mid + end;
            }
            else if (_attivita.utilizzoTerreno.classType == ClassType.DestinazioneUso)
            {
                return specie != "" ? " (" + specie + ")" : "";
            }
            return "";
        }

        private async Task<string> GetDescrizioneAgenda()
        {
            if (!string.IsNullOrEmpty(_attivita.descrizione))
                return _attivita.descrizione;

            string desc = "";
            if (_attivita.job != null)
            {
                desc = await _lavorazione.GetLavDes(_attivita.job.getCodice(), _server);
            }
            desc += await GetDescrizioneSpecieVarieta();
            return desc;
        }

        private int CreaAttivitaPersonalizzata()
        {
            if (_attivita.attivitaPersonalizzata != null && _attivita.attivitaPersonalizzata.codice > 0 &&
                (_agenda.Lav_Cod == LAV_COD.LAVCOD_ALTRE_OPERAZIONI || _agenda.Lav_Cod == LAV_COD.LAVCOD_VISITA))
            {
                return _attivita.attivitaPersonalizzata.codice;
            }
            return 0;
        }

        /// <remarks>
        /// HAS TODO TO RESOLVE
        /// </remarks>
        private List<Nota> GetNotes()
        {
            List<Nota> notes = new();
            if (_attivita.noteIntervento == null)
                return notes;

            if (_attivita.noteIntervento.Count() != 1 || _attivita.noteIntervento.First().codice != 0)
            {
                return _attivita.noteIntervento.Select(n =>
                {
                    Nota note = new();
                    note.Id_Agenda = _agenda.Id_Agenda;
                    note.Nota_Cod = n.codice;
                    return note;
                }).ToList();
            }
            if (_attivita.utilizzoTerreno != null)
            {
                _attivita.noteIntervento = new();
                Specie specie;
                if (_attivita.utilizzoTerreno.classType == ClassType.Varieta)
                {
                    specie = ((Varieta)_attivita.utilizzoTerreno).specie;
                }
                // TODO: return leggi note da specie lavorazione
                throw new NotImplementedException();
            }
            return notes;
        }
        /// <remarks>
        /// HAS TODO TO RESOLVE
        /// </remarks>
        private void AddCostiAccessori()
        {

            return;

            if (_attivita.risorse == null || _attivita.risorse.Count() == 0)
                return;

            //foreach(Risorsa risorsa in _attivita.risorse)
            //{
            //    switch (risorsa.classType)
            //    {
            //        case ClassType.RisorsaMacchina:
            //        case ClassType.RisorsaPersona:

            //    }
            //}

            IEnumerable<Movimento> movimentiRisorse = new List<Movimento>();
            
            _agenda.Movimenti.AddRange(movimentiRisorse);
        }
        /// <remarks>
        /// HAS TODO TO RESOLVE
        /// </remarks>
        private void SetAssegnatariVisita()
        {
            if (_info.IsVisita)
            {
                throw new NotImplementedException();
            }
        }
        /// <remarks>
        /// HAS TODO TO RESOLVE
        /// </remarks>
        private Movimento AddMovimentoCampagna()
        {
            Movimento movCampagna = _movFactory.GetMovimentoCampagna(_attivita, _agenda, _info);
            movCampagna.Movimenti_Dettagli = new List<Movimento_Dettaglio>();
            movCampagna.Movimenti_Dettagli_Tecnici = new List<Movimento_Dettaglio_Tecnico>();
            _agenda.Movimenti.Add(movCampagna);
            return movCampagna;
        }
        /// <remarks>
        /// HAS TODO TO RESOLVE
        /// </remarks>
        private void AddDettaglioTecnicoAcqua()
        {
            // TODO
        }
        /// <remarks>
        /// HAS TODO TO RESOLVE
        /// </remarks>
        private void AddDettaglioTecnicoCausale()
        {
            // TODO
        }

        /// <remarks>
        /// HAS TODO TO RESOLVE
        /// </remarks>
        private void AddMovimentiDettagli(Movimento movCampagna, decimal area)
        {
            List<Movimento_Dettaglio> movimenti = _movDettagliFactory.GetMovDettagliList(_attivita, _agenda, _info, area);
            movCampagna.Movimenti_Dettagli.AddRange(movimenti);

            if (_info.IsRaccolta)
            {
                DettaglioRaccolta? detail = (DettaglioRaccolta?)_attivita.risorse.Find(r => r.classType == ClassType.DettaglioRaccolta);
                if (detail != null)
                {
                    movCampagna.Mezzo = (int)detail.Opzioni_Raccolta.Ripartizione;
                    movCampagna.Modalita = (int)detail.Opzioni_Raccolta.Modalita;
                }
            }
            //if (!_info.IsRaccolta && !_info.IsSemina && !_info.IsIrrigazione)
            //{
            //    throw new NotImplementedException();
            //}
        }
        /// <remarks>
        /// HAS TODO TO RESOLVE
        /// </remarks>
        private void AddMovimentoScarico()
        {
            // TODO
        }
        /// <remarks>
        /// HAS TODO TO RESOLVE
        /// </remarks>
        private void AddMovimentoCarico()
        {
            // TODO
        }

        private void SetGisParameters()
        {
            string wkt = GetWkt();
            if (string.IsNullOrEmpty(wkt)) return;
            
            throw new NotImplementedException();
        }

        public async Task<OperazioneAgenda> MapAttivitaToAgenda(Attivita attivita, AgronicaCoreParametriServer objParametriServer)
        {
            _attivita = attivita;
            _server = objParametriServer;
            _agenda = InitializeAgenda();
            SetActivityHarvestOptions();
            decimal treatedArea = CheckTotalTreatedArea();
            _agenda.Des_Lib = await GetDescrizioneAgenda();
            _agenda.Id_Agenda = CreaAttivitaPersonalizzata();
            _agenda.Note = GetNotes();
            _agenda.Movimenti = new List<Movimento>();
            AddCostiAccessori();
            SetAssegnatariVisita();
            Movimento movCampagna = AddMovimentoCampagna();
            AddDettaglioTecnicoAcqua();
            AddDettaglioTecnicoCausale();
            AddMovimentiDettagli(movCampagna, treatedArea);
            SetGisParameters();
            AddMovimentoScarico();
            AddMovimentoCarico();
            return _agenda;
        }
    }
}
