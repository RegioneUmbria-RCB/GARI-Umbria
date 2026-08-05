using AgronicaCoreDTOStd.InData.Agenda.OperazioneAgenda_Temp;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.Info;
using AgronicaCoreModelsSTD.attivita.note_intervento;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.costanti;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.avversita;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaCoreModelsSTD.meteo;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.CodiciAnagrafe;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impianti;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.MateriePrime;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogAgenda;
using AgronicaNetCore.Logging.DAL.DataLayer.AgronicaLogRicette;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Contatti;
using AgronicaNetCore.Operazione.BIZ.Resources;
using AgronicaNetCore.Operazione.BIZ.Services.Agenda.Mapper;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.costanti;
using AgronicaNetCore.Operazione.BIZ.Services.Movimenti;
using AgronicaNetCore.Operazione.BIZ.Services.Movimenti.Factory;
using AgronicaNetCore.Operazione.DAL.DataLayer.Agenda;
using AgronicaNetCore.Operazione.DAL.DataLayer.Agronica_Log_Agenda;
using AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Destinazioni;
using AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Dettaglio_Tecnico_Extra;
using AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti;
using AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti_Dettagli;
using AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti_Zoo;
using AgronicaNetCore.Operazione.DAL.DataLayer.Ricette;
using AgronicaNetCore.Operazione.DAL.Resources.UtilityAgenda;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using InData.Agenda;
using InData.Anagrafica;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Transactions;
using static AgronicaCoreModelsSTD.attivita.Attivita;
using static AgronicaCoreModelsSTD.attivita.dettagli.Opzioni_Raccolta;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Operazione.BIZ.Services.Agenda
{
    public class AgendaService : BaseServiceOperazioneBIZ, IAgendaService
    {
        private readonly IAgenda _agendaDal;
        private readonly IRicette _ricetteDal;
        private readonly IAgronica_Log_Agenda _logOpAgenda;
        
        private readonly IMovimenti _movimentiDal;
        private readonly IMovimentiService _movimentiBiz;

        private readonly IMovimenti_Dettagli _movDettagliDal;
        //private readonly IMovDettagliService _movDettagliBiz;

        private readonly IMov_Destinazioni _movDestinazioniDal;
        //private readonly IMovDestinazioniService _movDestinazioniBiz;
        
        private readonly IMovimenti_Zoo _movZooDal;
        //private readonly IMovZooService _movZooBiz;
        
        private readonly IMov_Dettaglio_Tecnico_Extra _movDettTecExtraDal;
        private readonly IAgro_Sequence _sequenceDal;
        private readonly IUtilityAgendaClassInitializer _agendaClassInitializer;

        private readonly IAttivitaToAgenda _agendaMapper;
        private readonly IImpianti _impiantiDal;
        private readonly ICodiciAnagrafe _codiciAnagrafe;
        private readonly IContatti _contattiDal;
        private readonly IMateriePrimeDal _materiePrimeDal;
        private readonly IAgronicaLogAgendaDal _agronicaLogAgendaDal;
        private readonly IAgronicaLogRicetteDal _agronicaLogRicetteDal;

        public AgendaService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _agendaDal = provider.GetRequiredService<IAgenda>();

            _logOpAgenda = provider.GetRequiredService<IAgronica_Log_Agenda>();
            
            _movimentiDal = provider.GetRequiredService<IMovimenti>();
            _movimentiBiz = provider.GetRequiredService<IMovimentiService>();

            _movDettagliDal = provider.GetRequiredService<IMovimenti_Dettagli>();
            //_movDettagliBiz = provider.GetRequiredService<IMovDettagliService>();

            _movDestinazioniDal = provider.GetRequiredService<IMov_Destinazioni>();
            //_movDestinazioniBiz = provider.GetRequiredService<IMovDestinazioniService>();
            
            _movZooDal = provider.GetRequiredService<IMovimenti_Zoo>();
            //_movZooBiz = provider.GetRequiredService<IMovZooService>();

            _movDettTecExtraDal = provider.GetRequiredService<IMov_Dettaglio_Tecnico_Extra>();
            
            _sequenceDal = provider.GetRequiredService<IAgro_Sequence>();
            _agendaClassInitializer = provider.GetRequiredService<IUtilityAgendaClassInitializer>();

            _agendaMapper = provider.GetRequiredService<IAttivitaToAgenda>();
            _impiantiDal = provider.GetRequiredService<IImpianti>();
            _contattiDal = provider.GetRequiredService<IContatti>();
            _materiePrimeDal = provider.GetRequiredService<IMateriePrimeDal>();
            _agronicaLogAgendaDal = provider.GetRequiredService<IAgronicaLogAgendaDal>();
            _agronicaLogRicetteDal = provider.GetRequiredService<IAgronicaLogRicetteDal>();
            _codiciAnagrafe = provider.GetRequiredService<ICodiciAnagrafe>();

            _ricetteDal = provider.GetRequiredService<IRicette>();
        }

        #region Metodi per letture di attività da parte dell'app

        public async Task ValorizzaArchivioAttivitaPerAppAsync(ArchivioAttivitaCampagna archivioAttivitaCampagna, string piva, DateTime dataUltimaSincro, DateTime dataRiferimento, AgronicaCoreParametriTriple objParametriTriple)
        {
            DataTable dtOrePersone = null;
            DataTable dtOreMacchine = null;

            DataTable? dtAttivita = null;
            DataTable? dtAttivitaCancellate = null;
            DataTable? dtBrogliacci = null;
            DataTable? dtBrogliacciCancellati = null;
            
            List<Attivita> listaDiAttivita = new();
            List<Attivita> listaDiBrogliacci = new();
            List<DataRow> tabellaAttivitaDataRows = new();
            List<DataRow> tabellaBrogliacciDataRows = new();

            try
            {
                await OpenConnectionAsync(objParametriTriple.ObjParametriServer, false);

                var esistonoModificheAttivita = await _agronicaLogAgendaDal.EsistonoModificheDopoLaDataPerAttivitaAppAsync(piva, dataRiferimento, dataUltimaSincro, objParametriTriple.ObjParametriServer);

                var esistonoModificheBrogliacci = await _agronicaLogRicetteDal.EsistonoModificheDopoLaDataPerBrogliacciAppAsync(piva, dataRiferimento, dataUltimaSincro, objParametriTriple.ObjParametriServer);

                if (!esistonoModificheAttivita && !esistonoModificheBrogliacci)
                    return;

                if (esistonoModificheAttivita && esistonoModificheBrogliacci)
                {
                    var resultAttivitaBrogliacci = await _agendaDal.LeggiAttivitaBrogliacciPerAppAsync(piva, dataRiferimento, dataUltimaSincro, objParametriTriple.ObjParametriServer);
                    dtAttivita = resultAttivitaBrogliacci.dtAttivita;
                    dtAttivitaCancellate = resultAttivitaBrogliacci.dtAttivitaCancellate;
                    dtBrogliacci = resultAttivitaBrogliacci.dtBrogliacci;
                    dtBrogliacciCancellati = resultAttivitaBrogliacci.dtBrogliacciCancellati;
                }

                List<int> ricettaOperazioneCods = new();
                if (esistonoModificheAttivita)
                {
                    if (dtAttivita is null || dtAttivitaCancellate is null)
                    {
                        var resultAttivitaPerApp = await _agendaDal.LeggiAttivitaPerAppAsync(piva, dataRiferimento, dataUltimaSincro, objParametriTriple.ObjParametriServer);

                        dtAttivita = resultAttivitaPerApp.dtAttivita;
                        dtAttivitaCancellate = resultAttivitaPerApp.dtAttivitaCancellate;
                    }

                    var tabellaAttivitaCancellateDataRows = dtAttivitaCancellate.AsEnumerable().ToList();
                    foreach (var rowAttivitaCancellata in tabellaAttivitaCancellateDataRows)
                    {
                        var idAgendaAttivitaCancellata = rowAttivitaCancellata.Field<int>("Id_Agenda");
                        archivioAttivitaCampagna.AttivitaCancellate.Add(idAgendaAttivitaCancellata);
                    }

                    tabellaAttivitaDataRows = dtAttivita.AsEnumerable().ToList();
                    if (tabellaAttivitaDataRows.Any())
                    {
                        var idAgendas = tabellaAttivitaDataRows.Select(dr => dr.Field<int>("Id_Agenda")).ToList();
                        ricettaOperazioneCods.AddRange(tabellaAttivitaDataRows.Select(dr => dr.Field<int>("Ricetta_Operazione_Cod")).Where(x => x > 0).ToList());

                        var opzioniLetturaAgenda = new OpzioniLetturaAgenda()
                        {
                            LeggiNote = true,
                        };

                        if (idAgendas.Any())
                        {
                            listaDiAttivita = await LeggiListaAttivitaAsync(idAgendas, opzioniLetturaAgenda, objParametriTriple, false);
                        }
                    }
                }

                if (esistonoModificheBrogliacci)
                {
                    if (dtBrogliacci is null || dtBrogliacciCancellati is null)
                    {
                        var resultBrogliacciPerApp = await _agendaDal.LeggiBrogliacciPerAppAsync(piva, dataRiferimento,
                            dataUltimaSincro, objParametriTriple.ObjParametriServer);

                        dtBrogliacci = resultBrogliacciPerApp.dtBrogliacci;
                        dtBrogliacciCancellati = resultBrogliacciPerApp.dtBrogliacciCancellati;
                    }

                    tabellaBrogliacciDataRows = dtBrogliacci.AsEnumerable().ToList();

                    if (tabellaBrogliacciDataRows.Any())
                    {
                        var listaDiRicettaOperazioneCod = tabellaBrogliacciDataRows.Select(dr => dr.Field<int>("Ricetta_Operazione_Cod")).ToList();
                        ricettaOperazioneCods.AddRange(listaDiRicettaOperazioneCod);

                        var listaDiRicettaCod = tabellaBrogliacciDataRows.Select(dr => dr.Field<int>("Ricetta_Cod")).ToList();

                        listaDiBrogliacci = await LeggiListaBrogliacciAsync(listaDiRicettaCod, listaDiRicettaOperazioneCod,
                            objParametriTriple, false);
                    }
                }

                if (ricettaOperazioneCods.Any())
                {
                    var resultLetturaOrePerPersoneMacchine = await _agendaDal.LeggiOreOperatorePerListaAttivitaAsync(ricettaOperazioneCods, objParametriTriple.ObjParametriServer);

                    dtOreMacchine = resultLetturaOrePerPersoneMacchine.dtMacchine;
                    dtOrePersone = resultLetturaOrePerPersoneMacchine.dtOperatori;
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriTriple.ObjParametriServer, ex);
                throw;
            }
            finally
            {
                CloseConnection(objParametriTriple.ObjParametriServer);
            }

            var dictRaccoglitore = new Dictionary<int, IdentificativiAttivitaAppDati>();
            foreach (var row in tabellaAttivitaDataRows)
            {
                var raccoglitoreCod = row.Field<int>("Raccoglitore_Cod");
                if (raccoglitoreCod == 0 || dictRaccoglitore.ContainsKey(raccoglitoreCod))
                    continue;

                dictRaccoglitore.Add(raccoglitoreCod, new IdentificativiAttivitaAppDati());

                var rows = tabellaAttivitaDataRows.Where(r => r.Field<int>("Raccoglitore_Cod") == raccoglitoreCod).ToList();
                var rowConGuidRicetta = rows.FirstOrDefault(r => !string.IsNullOrEmpty(r.Field<string>("GuidRicetta")));

                if (rowConGuidRicetta is null)
                    continue;

                dictRaccoglitore[raccoglitoreCod].Id = rowConGuidRicetta.Field<string>("GuidRicetta");
                dictRaccoglitore[raccoglitoreCod].Tipo = rowConGuidRicetta.Field<string>("Tipo");
                dictRaccoglitore[raccoglitoreCod].Versione = rowConGuidRicetta.Field<string>("Versione");
            }

            foreach (var row in tabellaAttivitaDataRows)
            {
                var raccoglitoreCod = row.Field<int>("Raccoglitore_Cod");
                if (raccoglitoreCod == 0)
                {
                    var idAgenda = row.Field<int>("Id_Agenda");
                    var attivita = listaDiAttivita.FirstOrDefault(a => a.codice == idAgenda.ToString());
                    if (attivita is null)
                        continue;

                    ValorizzaAttivitaConProprietaAppDati(attivita, row);

                    attivita.inizio = new DateTime(attivita.inizio.Year, attivita.inizio.Month, attivita.inizio.Day, 
                        attivita.oraInizio.Hour, attivita.oraInizio.Minute, 0, DateTimeKind.Local);

                    if (attivita.origineAttivita == ((int)DatiApp.Attivita).ToString() || attivita.origineAttivita == ((int)DatiApp.AttivitaDemetra).ToString()) //attività proveniente da app o da demetra
                    {
                        var ricettaOperazioneCod = row.Field<int>("Ricetta_Operazione_Cod");
                        AssegnaOreAMacchineEOperatori(attivita, ricettaOperazioneCod, dtOrePersone, dtOreMacchine);
                    }
                    attivita.stato = Stati.Eseguita;
                    var codiceGiasPianificata = row.Field<string>("CodiceGiasPianificata");
                    if (!string.IsNullOrEmpty(codiceGiasPianificata))
                    {
                        attivita.riferimentoPianificata = codiceGiasPianificata;
                    }
                    
                    var activitiesList = new List<Attivita>() { attivita };
                    archivioAttivitaCampagna.Attivita.Add(activitiesList);
                }
                else
                {
                    if (dictRaccoglitore[raccoglitoreCod].GiaProcessato)
                        continue;

                    dictRaccoglitore[raccoglitoreCod].GiaProcessato = true;
                    var attivitaConRaccoglitoreCod = listaDiAttivita.Where(a => a.raccoglitore == raccoglitoreCod).ToList();
                    foreach (var attivita in attivitaConRaccoglitoreCod)
                    {
                        attivita.stato = Stati.Eseguita;

                        ValorizzaAttivitaConProprietaAppDati(attivita, dictRaccoglitore, raccoglitoreCod);

                        attivita.inizio = new DateTime(attivita.inizio.Year, attivita.inizio.Month, attivita.inizio.Day,
                            attivita.oraInizio.Hour, attivita.oraInizio.Minute, 0, DateTimeKind.Local);

                        if (attivita.origineAttivita == ((int)DatiApp.Attivita).ToString() || attivita.origineAttivita == ((int)DatiApp.AttivitaDemetra).ToString()) //attività proveniente da app o da demetra
                        {
                            var rowAgenda = tabellaAttivitaDataRows.First(r => r.Field<int>("Id_Agenda") == int.Parse(attivita.codice));
                            var ricettaOperazioneCod = rowAgenda.Field<int>("Ricetta_Operazione_Cod");
                            AssegnaOreAMacchineEOperatori(attivita, ricettaOperazioneCod, dtOrePersone, dtOreMacchine);
                        }
                    }

                    archivioAttivitaCampagna.Attivita.Add(attivitaConRaccoglitoreCod);
                }
            }

            var dictRaccoglitoreBrogliacci = new Dictionary<int, IdentificativiAttivitaAppDati>();
            foreach (var row in tabellaBrogliacciDataRows)
            {
                var raccoglitoreCod = row.Field<int>("Raccoglitore_Cod");
                if (raccoglitoreCod == 0 || dictRaccoglitoreBrogliacci.ContainsKey(raccoglitoreCod))
                    continue;

                dictRaccoglitoreBrogliacci.Add(raccoglitoreCod, new IdentificativiAttivitaAppDati());

                dictRaccoglitoreBrogliacci[raccoglitoreCod].Id = row.Field<string>("GuidRicetta");
                dictRaccoglitoreBrogliacci[raccoglitoreCod].Tipo = row.Field<string>("Tipo");
                dictRaccoglitoreBrogliacci[raccoglitoreCod].Versione = row.Field<string>("Versione");
            }

            foreach (var row in tabellaBrogliacciDataRows)
            {
                var raccoglitoreCod = row.Field<int>("Raccoglitore_Cod");
                if (raccoglitoreCod == 0)
                {
                    var ricettaOperazioneCod = row.Field<int>("Ricetta_Operazione_Cod");
                    var brogliaccio = listaDiBrogliacci.FirstOrDefault(a => a.codice == ricettaOperazioneCod.ToString());
                    if (brogliaccio is null)
                        continue;

                    ValorizzaAttivitaConProprietaAppDati(brogliaccio, row);

                    brogliaccio.codice = "-1";
                    brogliaccio.inizio = new DateTime(brogliaccio.inizio.Year, brogliaccio.inizio.Month, brogliaccio.inizio.Day,
                        brogliaccio.oraInizio.Hour, brogliaccio.oraInizio.Minute, 0, DateTimeKind.Local);

                    if (brogliaccio.origineAttivita == ((int)DatiApp.Attivita).ToString() || brogliaccio.origineAttivita == ((int)DatiApp.AttivitaDemetra).ToString() ||
                        brogliaccio.origineAttivita == ((int)DatiApp.Ricette).ToString() || brogliaccio.origineAttivita == ((int)DatiApp.RicetteDemetra).ToString())
                    {
                        AssegnaOreAMacchineEOperatori(brogliaccio, ricettaOperazioneCod, dtOrePersone, dtOreMacchine);
                    }

                    var statoRicetta = row.Field<int>("StatoRicetta");
                    if (statoRicetta == (int)Stati.Da_Eseguire)
                    {
                        brogliaccio.stato = Stati.Da_Eseguire;
                        archivioAttivitaCampagna.AttivitaPianificate.Add(brogliaccio);
                    }
                    else if (statoRicetta == (int)Stati.Eseguita)
                    {
                        brogliaccio.stato = Stati.Eseguita;
                        var codiceGiasPianificata = row.Field<string?>("CodiceGiasPianificata");
                        if (!string.IsNullOrEmpty(codiceGiasPianificata))
                        {
                            brogliaccio.riferimentoPianificata = codiceGiasPianificata;
                        }
                        var listaBrogliacci = new List<Attivita>() { brogliaccio };
                        archivioAttivitaCampagna.Brogliacci.Add(listaBrogliacci);
                    }
                }
                else
                {
                    continue; //fix temporaneo fino alla gestione delle multiattività da parte dell'app
                    
                    if (dictRaccoglitoreBrogliacci[raccoglitoreCod].GiaProcessato)
                        continue;

                    dictRaccoglitoreBrogliacci[raccoglitoreCod].GiaProcessato = true;
                    var brogliacciConRaccoglitoreCod = listaDiBrogliacci.Where(a => a.raccoglitore == raccoglitoreCod).ToList();

                    var listaBrogliacci = new List<Attivita>();
                    foreach (var brogliaccio in brogliacciConRaccoglitoreCod)
                    {
                        ValorizzaAttivitaConProprietaAppDati(brogliaccio, dictRaccoglitoreBrogliacci, raccoglitoreCod);

                        brogliaccio.inizio = new DateTime(brogliaccio.inizio.Year, brogliaccio.inizio.Month, brogliaccio.inizio.Day,
                            brogliaccio.oraInizio.Hour, brogliaccio.oraInizio.Minute, 0, DateTimeKind.Local);

                        var ricettaOperazioneCod = int.Parse(brogliaccio.codice);
                        
                        brogliaccio.codice = "-1";
                        brogliaccio.stato = Stati.Eseguita;

                        if (brogliaccio.origineAttivita == ((int)DatiApp.Attivita).ToString() || brogliaccio.origineAttivita == ((int)DatiApp.AttivitaDemetra).ToString() ||
                            brogliaccio.origineAttivita == ((int)DatiApp.Ricette).ToString() || brogliaccio.origineAttivita == ((int)DatiApp.RicetteDemetra).ToString())
                        {
                            AssegnaOreAMacchineEOperatori(brogliaccio, ricettaOperazioneCod, dtOrePersone, dtOreMacchine);
                        }

                        listaBrogliacci.Add(brogliaccio);
                    }

                    archivioAttivitaCampagna.Brogliacci.Add(listaBrogliacci);
                }
            }

            if (dtBrogliacciCancellati is not null)
            {
                var rowsBrogliacciCancellati = dtBrogliacciCancellati.AsEnumerable().ToList();
                foreach (var row in rowsBrogliacciCancellati)
                {
                    var guidRicetta = row.Field<string>("GuidRicetta");

                    if (string.IsNullOrEmpty(guidRicetta)
                        || archivioAttivitaCampagna.Brogliacci.SelectMany(x => x).Any(a => a.guid == guidRicetta)
                        || archivioAttivitaCampagna.AttivitaPianificate.Any(a => a.guid == guidRicetta))
                        continue;

                    var tipo = row.Field<string>("Tipo");
                    if (tipo == ((int)DatiApp.RicetteDemetra).ToString() || tipo == ((int)DatiApp.Ricette).ToString())
                    {
                        if (guidRicetta.Contains("|"))
                            archivioAttivitaCampagna.AttivitaPianificateCancellate.Add(guidRicetta.Split("|")[0]);
                        else
                            archivioAttivitaCampagna.AttivitaPianificateCancellate.Add(guidRicetta);
                    }
                    else
                    {
                        if (guidRicetta.Contains("|"))
                            archivioAttivitaCampagna.BrogliacciCancellati.Add(guidRicetta.Split("|")[0]);
                        else
                            archivioAttivitaCampagna.BrogliacciCancellati.Add(guidRicetta);
                    }
                }
            }
        }

        private void ValorizzaAttivitaConProprietaAppDati(Attivita attivita, DataRow row)
        {
            var guid = row.Field<string>("GuidRicetta");
            var versione = row.Field<string>("Versione");
            var tipo = row.Field<string>("Tipo");
            attivita.guid = guid;
            attivita.versione = versione;
            attivita.origineAttivita = tipo;
        }

        private void ValorizzaAttivitaConProprietaAppDati(Attivita attivita, Dictionary<int, IdentificativiAttivitaAppDati> dictRaccoglitore, int raccoglitoreCod)
        {
            attivita.guid = dictRaccoglitore[raccoglitoreCod].Id;
            attivita.origineAttivita = dictRaccoglitore[raccoglitoreCod].Tipo;
            attivita.versione = dictRaccoglitore[raccoglitoreCod].Versione;
        }

        class IdentificativiAttivitaAppDati
        {
            public string Id { get; set; } = string.Empty;
            public string Versione { get; set; } = string.Empty;
            public string Tipo { get; set; } = string.Empty;
            public bool GiaProcessato { get; set; }
        }

        private void AssegnaOreAMacchineEOperatori(Attivita attivita, int ricettaOperazioneCod, DataTable dtOrePersone, DataTable dtOreMacchine)
        {
            if (dtOreMacchine is not null && dtOreMacchine.Rows.Count > 0)
            {
                var macchine = attivita.risorse.OfType<RisorsaMacchina>().ToList();
                foreach (var macchina in macchine)
                {
                    var rowsMacchina = dtOreMacchine.AsEnumerable()
                        .Where(r => r.Field<int>("Ricetta_Operazione_Cod") == ricettaOperazioneCod
                            && r.Field<int>("Mat_Cod") == macchina.macchina.codice)
                        .ToList();
                    if (rowsMacchina.Any())
                    {
                        var qta = rowsMacchina.First().Field<double>("Qta");
                        if (qta > 0)
                        {
                            macchina.inizio = new DateTime(attivita.inizio.Year, attivita.inizio.Month, attivita.inizio.Day, 0, 0, 0, DateTimeKind.Local);
                            AssegnaOreLavorateARisorsa(macchina, qta);
                        }
                    }
                }
            }

            if (dtOrePersone is not null && dtOrePersone.Rows.Count > 0)
            {
                var operatori = attivita.risorse.OfType<RisorsaPersona>().ToList();
                foreach (var operatore in operatori)
                {
                    var rowsOperatore = dtOrePersone.AsEnumerable()
                        .Where(r => r.Field<int>("Ricetta_Operazione_Cod") == ricettaOperazioneCod
                            && r.Field<int>("Mat_Cod") == operatore.risorsaUmana.codice)
                        .ToList();

                    if (rowsOperatore.Any())
                    {
                        var qta = rowsOperatore.First().Field<double>("Qta");
                        if (qta > 0)
                        {
                            operatore.inizio = new DateTime(attivita.inizio.Year, attivita.inizio.Month, attivita.inizio.Day, 0, 0, 0, DateTimeKind.Local);
                            AssegnaOreLavorateARisorsa(operatore, qta);
                        }
                    }
                }
            }
        }

        private void AssegnaOreLavorateARisorsa(RisorsaTimeSheet risorsa, double qta)
        {
            int hours = (int)Math.Truncate(qta);
            int minutes = (int)Math.Round((qta - hours) * 60);

            if (minutes == 60)
            {
                hours += 1;
                minutes = 0;
            }

            risorsa.fine = new DateTime(
                    risorsa.inizio.Value.Year,
                    risorsa.inizio.Value.Month,
                    risorsa.inizio.Value.Day,
                    0,
                    0,
                    0,
                    DateTimeKind.Local)
                .AddHours(hours)
                .AddMinutes(minutes);

            risorsa.totaleOre = qta;
        }

        #endregion

        #region Metodi per restituire oggetti di tipo Attivita
        public async Task<Attivita> LeggiAttivitaAsync(int idAgenda, OpzioniLetturaAgenda opzioniLetturaAgenda, AgronicaCoreParametriTriple parametriTriple, bool manageConnectionLifetime = true)
        {
            var idAgendas = new List<int>()
            {
                idAgenda,
            };
            var attivita = await LeggiListaAttivitaAsync(idAgendas, opzioniLetturaAgenda, parametriTriple, manageConnectionLifetime);
            return attivita.FirstOrDefault();
        }

        /// <summary>
        /// Legge lista attività da una lista di agende
        /// IMPORTANTE: non legge i disciplinari, non gestisce i rilievi, non gestisce le visite, i carichi, i scarichi, le registrazioni. Non gestisce verbose. Non legge dalla Mov_Dettagli_Riferimenti, quindi non legge carichi e scarichi da magazzini esterni. Non legge longitudine e latitudine. Non legge pagamenti. 
        /// </summary>
        /// <param name="idAgendas"></param>
        /// <param name="opzioniLetturaAgenda"></param>
        /// <param name="parametriTriple"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<Attivita>> LeggiListaAttivitaAsync(List<int> idAgendas, OpzioniLetturaAgenda opzioniLetturaAgenda, AgronicaCoreParametriTriple parametriTriple, bool manageConnectionLifetime = true)
        {
            var result = new List<Attivita>();
            RisultatoTabelleAgenda risultatoLetturaTabelleAgenda = null;
            try
            {
                if (manageConnectionLifetime)
                {
                    await OpenConnectionAsync(parametriTriple.ObjParametriServer,false);
                }
                risultatoLetturaTabelleAgenda = await _agendaDal.LeggiTutteLeTabelleDiAgenda(idAgendas, opzioniLetturaAgenda, parametriTriple);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriTriple.ObjParametriServer, ex);
                throw;
            }
            finally
            {
                if (manageConnectionLifetime)
                {
                    CloseConnection(parametriTriple.ObjParametriServer);
                }
            }

            var rowsOperazioni = risultatoLetturaTabelleAgenda.RowsOperazioni;
            
            var listaDiAttivitaConInfoBase = new List<AttivitaConInfoBase>();
            foreach (var rowAgenda in risultatoLetturaTabelleAgenda.RowsAgenda)
            {
                try
                {
                    var attivitaConMovimenti = CreaAttivitaConInfoBaseEMovimenti(risultatoLetturaTabelleAgenda, rowsOperazioni, rowAgenda);
                    if (attivitaConMovimenti is not null)
                    {
                        listaDiAttivitaConInfoBase.Add(attivitaConMovimenti);
                    }
                }
                catch (Exception ex)
                {
                    var idAgenda = rowAgenda.Field<int>("Id_Agenda");
                    LogError($"Creazione info base e movmenti per attività con id agenda {idAgenda} fallito, messaggio eccezione: {ex.Message}", parametriTriple.ObjParametriServer, ex);
                }
            }

            var tuttiIMovimenti = listaDiAttivitaConInfoBase.SelectMany(a => a.Movimenti).ToList();
            var causaliOperatori = new List<string>
            {
                CAU_MOV.CAU_IMPUTAZIONE_MANODOPERA,
                CAU_MOV.CAU_IMPUTAZIONE_TECNICO_RESPONSABILE,
                CAU_MOV.CAU_IMPUTAZIONE_TERZISTI
            };

            var listaDiCodRisUm = tuttiIMovimenti
                .Where(m => causaliOperatori.Contains(m.Cau_Mov.ToString()))
                .SelectMany(m => m.Movimenti_Dettagli)
                .Select(md => md.Mat_Cod)
                .ToList();

            DataTable? dtRisorseUmane = null;
            if (listaDiCodRisUm.Any())
            {
                dtRisorseUmane = await _contattiDal.LeggiListaDiContattiAsync(listaDiCodRisUm.Distinct().ToList(), parametriTriple.ObjParametriServer);
            }

            var impiantiPerLetture = listaDiAttivitaConInfoBase
                .Where(a => a.InfoOperazione.TipoCentroDiCosto != Tipo.ProdottoDaTrattare)
                .Select(a => GetMovimentoFromCausale(a.Movimenti, a.InfoOperazione.Cau_Mov)) //movimentoOperazione
                .Select(m => GetPrimoImpianto(m))
                .Where(i => i is not null)
                .Select(i => (i.Piva, i.Sa_Cod, i.Appezza, i.Id_Destinazione))
                .Distinct()
                .ToList();

            DataTable? dtInfoVarieta = null;
            DataTable? dtCodiciDestinazioniUso = null;
            if (impiantiPerLetture.Any())
            {
                dtInfoVarieta = await _impiantiDal.LeggiInfoVarietaAsync(impiantiPerLetture, parametriTriple.ObjParametriServer);
                dtCodiciDestinazioniUso = await _codiciAnagrafe.LeggiCodiciDestinazioniUsoAsync(impiantiPerLetture, parametriTriple.ObjParametriServer);
            }

            foreach (var attivitaConInfoBase in listaDiAttivitaConInfoBase)
            {
                try
                {
                    await PopolaAttivitaAsync(attivitaConInfoBase, parametriTriple, risultatoLetturaTabelleAgenda, 
                        dtRisorseUmane?.AsEnumerable().ToList() ?? new(), dtInfoVarieta?.AsEnumerable().ToList() ?? new(), 
                        dtCodiciDestinazioniUso?.AsEnumerable().ToList() ?? new());

                    if (attivitaConInfoBase.Attivita is not null)
                    {
                        result.Add(attivitaConInfoBase.Attivita);
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Mapping dell'attività con id agenda {attivitaConInfoBase.Attivita.codice} fallito, messaggio eccezione: {ex.Message}", parametriTriple.ObjParametriServer, ex);
                }
            }

            return result;
        }

        public async Task<Attivita> LeggiBrogliaccioAsync(int ricettaCod, int ricettaOperazioneCod, AgronicaCoreParametriTriple parametriTriple, bool manageConnectionLifetime = true)
        {
            var listaDiRicettaCod = new List<int>() { ricettaCod };
            var listaDiRicettaOperazioneCod = new List<int>() { ricettaOperazioneCod };
            var brogliacci = await LeggiListaBrogliacciAsync(listaDiRicettaCod, listaDiRicettaOperazioneCod, parametriTriple, manageConnectionLifetime);
            return brogliacci.FirstOrDefault();
        }

        /// <summary>
        /// Non gestisce LAVCOD_DISTRIBUZIONE_AMMENDANTI. Non legge i disciplinari, non gestisce i rilievi. Non gestisce verbose. Non gestisce magazzini esterni.
        /// </summary>
        /// <param name="listaDiRicettaCod"></param>
        /// <param name="listaDiRicettaOperazioneCod"></param>
        /// <param name="parametriTriple"></param>
        /// <param name="manageConnectionLifetime"></param>
        /// <returns></returns>
        public async Task<List<Attivita>> LeggiListaBrogliacciAsync(List<int> listaDiRicettaCod, List<int> listaDiRicettaOperazioneCod, 
             AgronicaCoreParametriTriple parametriTriple, bool manageConnectionLifetime = true)
        {
            var result = new List<Attivita>();
            RisultatoTabelleRicette risultatoTabelleRicette = null;
            try
            {
                if (manageConnectionLifetime)
                {
                    await OpenConnectionAsync(parametriTriple.ObjParametriServer, false);
                }
                risultatoTabelleRicette = await _ricetteDal.LeggiTutteLeTabellePerLeRicetteAsync(listaDiRicettaCod, listaDiRicettaOperazioneCod, 
                    parametriTriple.ObjParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriTriple.ObjParametriServer, ex);
                throw;
            }
            finally 
            {
                if (manageConnectionLifetime)
                {
                    CloseConnection(parametriTriple.ObjParametriServer);
                }
            }

            var listaDiRicetteConInfo = new List<RicettaConInfoBase>();
            foreach (var rowRicettaOperazione in risultatoTabelleRicette.RowsRicetteOperazioni)
            {
                try
                {
                    var ricettaConInfo = CreaRicettaConInfo(risultatoTabelleRicette, rowRicettaOperazione);

                    if (ricettaConInfo is not null)
                    {
                        listaDiRicetteConInfo.Add(ricettaConInfo);
                    }
                }
                catch (Exception ex)
                {
                    var ricettaOperazioneCod = rowRicettaOperazione.Field<int>("Ricetta_Operazione_Cod");
                    LogError($"Creazione ricetta con info per ricetta operazione con codice {ricettaOperazioneCod} fallita, messaggio eccezione: {ex.Message}", parametriTriple.ObjParametriServer, ex);
                }
            }

            var tuttiIDettagli = listaDiRicetteConInfo.SelectMany(r => r.Dettagli).ToList();
            var causaliOperatori = new List<string>
            {
                CAU_MOV.CAU_IMPUTAZIONE_MANODOPERA,
                CAU_MOV.CAU_IMPUTAZIONE_TECNICO_RESPONSABILE,
                CAU_MOV.CAU_IMPUTAZIONE_TERZISTI
            };
            
            var listaDiCodRisUm = tuttiIDettagli
                .Where(d => causaliOperatori.Contains(d.Cau_Mov.ToString()))
                .Select(md => md.Mat_Cod)
                .ToList();

            DataTable? dtRisorseUmane = null;
            if (listaDiCodRisUm.Any())
            {
                dtRisorseUmane = await _contattiDal.LeggiListaDiContattiAsync(listaDiCodRisUm.Distinct().ToList(), parametriTriple.ObjParametriServer);
            }

            var impiantiPerLetture = listaDiRicetteConInfo
                .Where(r => r.InfoOperazione.TipoCentroDiCosto == Tipo.Esercizio)
                .SelectMany(r => r.Destinazioni)
                .Where(d => d.Tipo_Destinazione == TIPO_DESTINAZIONE.TIPO_DESTINAZIONE_IMPIANTO)
                .Select(d => (d.Piva, d.Sa_Cod, d.Appezza, d.Id_Reg))
                .Distinct()
                .ToList();

            var impiantiEDate = listaDiRicetteConInfo
                .Where(r => r.InfoOperazione.TipoCentroDiCosto == Tipo.Esercizio)
                .Select(r => (r.Attivita.inizio, r.Destinazioni))
                .SelectMany(r => r.Destinazioni
                    .Where(d => d.Tipo_Destinazione == TIPO_DESTINAZIONE.TIPO_DESTINAZIONE_IMPIANTO)
                    .Select(d => (d.Piva, d.Sa_Cod, d.Appezza, d.Id_Reg, r.inizio)))
                .Distinct()
                .ToList();

            DataTable? dtInfoVarieta = null;
            DataTable? dtProgettiCod = null;
            DataTable? dtCodiciDestinazioniUso = null;
            if (impiantiPerLetture.Any())
            {
                dtInfoVarieta = await _impiantiDal.LeggiInfoVarietaAsync(impiantiPerLetture, parametriTriple.ObjParametriServer);
                dtCodiciDestinazioniUso = await _codiciAnagrafe.LeggiCodiciDestinazioniUsoAsync(impiantiPerLetture, parametriTriple.ObjParametriServer);
            }

            if (impiantiEDate.Any())
            {
                dtProgettiCod = await _impiantiDal.LeggiCodiciProgettoAsync(impiantiEDate, parametriTriple.ObjParametriServer);
            }

            foreach (var ricettaConInfo in listaDiRicetteConInfo)
            {
                try
                {
                    await PopolaRicettaAsync(ricettaConInfo, parametriTriple, risultatoTabelleRicette, dtInfoVarieta?.AsEnumerable().ToList() ?? new(),
                        dtCodiciDestinazioniUso?.AsEnumerable().ToList() ?? new(), dtProgettiCod?.AsEnumerable().ToList() ?? new(), 
                        dtRisorseUmane?.AsEnumerable().ToList() ?? new());

                    if (ricettaConInfo.Attivita is not null)
                    {
                        result.Add(ricettaConInfo.Attivita);
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Mapping della ricetta operazione con codice {ricettaConInfo.RicettaOperazione.Ricetta_Operazione_Cod} fallito, messaggio eccezione: {ex.Message}", parametriTriple.ObjParametriServer, ex);
                }
            }

            return result;
        }

        private AttivitaConInfoBase CreaAttivitaConInfoBaseEMovimenti(RisultatoTabelleAgenda risultatoLetturaTabelleAgenda, List<DataRow> rowsOperazioni, DataRow rowAgenda)
        {
            var idAgenda = rowAgenda.Field<int>("Id_Agenda");

            var attivita = new Attivita();
            var lavCod = rowAgenda.Field<int>("Lav_Cod");
            var rowsForLavCod = rowsOperazioni.Where(r => r.Field<int>("Lav_Cod") == lavCod).ToList();
            if (rowsForLavCod.Any())
            {
                var tipo = rowsForLavCod.First().Field<string>("Tipo");
                if (tipo == "C" || tipo == "V")
                    attivita.job = new Lavorazione(lavCod);
                else if (tipo == "E")
                    attivita.job = new Registrazione(lavCod);
            }

            if (attivita.job is null)
            {
                throw new Exception($"Il tipo di lavorazione {lavCod} non è gestito ");
            }

            var infoOperazione = HelperAgenda.GetInfoOperazione(lavCod, Attivita.Tipo_Attivita.QuadernoDiCampagna);
            if (infoOperazione.IsRilievo || infoOperazione.IsVisita || infoOperazione.IsCarico || infoOperazione.IsScarico || infoOperazione.IsRegistrazione)
                throw new Exception($"L'operazione con lav_cod {lavCod} non è ancora stata gestita nel mapping");

            var rowsMovimenti = risultatoLetturaTabelleAgenda.RowsMovimenti
                .Where(dr => dr.Field<int>("Id_Agenda") == idAgenda)
                .OrderBy(dr => dr.Field<int>("Id_Mov"))
                .ToList();

            var rowsMovDettaglioTecnico = risultatoLetturaTabelleAgenda.RowsMovDettaglioTecnico
                .Where(r => r.Field<int>("Id_Agenda") == idAgenda)
                .OrderBy(dr => dr.Field<int>("Id_Mov"))
                .ThenBy(r => r.Field<int>("Id_Mov_Det"))
                .ToList();

            var rowsMovimentiDettagli = risultatoLetturaTabelleAgenda.RowsMovimentiDettagli
                .Where(r => r.Field<int>("Id_Agenda") == idAgenda)
                .OrderBy(dr => dr.Field<int>("Id_Mov"))
                .ThenBy(r => r.Field<int>("Id_Mov_Det"))
                .ToList();

            var rowsMovDestinazioni = risultatoLetturaTabelleAgenda.RowsMovDestinazioni
                .Where(r => r.Field<int>("Id_Agenda") == idAgenda)
                .OrderBy(dr => dr.Field<int>("Id_Mov"))
                .ThenBy(r => r.Field<int>("Id_Mov_Det"))
                .ToList();

            var movimenti = new List<MovimentoAttivita>();
            foreach (var rowMovimento in rowsMovimenti)
            {
                var movimento = MapMovimento(rowMovimento);
                var rowsMovDettaglioTecnicoMovimento = rowsMovDettaglioTecnico.Where(r => r.Field<int>("Id_Mov") == movimento.Id_Mov).ToList();
                AggiungiDettagliTecniciSuMovimento(movimento, rowsMovDettaglioTecnicoMovimento);

                var rowsMovimentiDettagliMovimento = rowsMovimentiDettagli.Where(r => r.Field<int>("Id_Mov") == movimento.Id_Mov).ToList();
                var rowsMovDestinazioniMovimento = rowsMovDestinazioni.Where(r => r.Field<int>("Id_Mov") == movimento.Id_Mov).ToList();
                AggiungiDettagliSuMovimento(movimento, rowsMovimentiDettagliMovimento, rowsMovDestinazioniMovimento, rowsMovDettaglioTecnicoMovimento);

                movimenti.Add(movimento);
            }

            attivita.tipo = Attivita.Tipo_Attivita.QuadernoDiCampagna;
            attivita.codice = idAgenda.ToString();
            attivita.descrizione = rowAgenda.Field<string>("Des_Lib");
            attivita.inizio = rowAgenda.Field<DateTime>("Validita_Inizio");

            var statoCod = rowAgenda.Field<int?>("Stato_Cod");
            if (statoCod.HasValue)
                attivita.statoWorkflow = (Attivita.StatiWorkflowQdC)statoCod;
            else
                attivita.statoWorkflow = Attivita.StatiWorkflowQdC.Non_Definito;

            var daRemoto = rowAgenda.Field<short?>("DaRemoto");
            attivita.daRemoto = daRemoto is null ? false : daRemoto.Value == 1;

            var raccoglitoreCod = rowAgenda.Field<int?>("Raccoglitore_Cod");
            attivita.raccoglitore = raccoglitoreCod is null ? 0 : raccoglitoreCod.Value;

            var origine = rowAgenda.Field<string>("Origine");
            attivita.origine = origine is null ? "" : origine;

            attivita.appRicettaOperazioneID = "";

            var bloccoFlag = (Tipo_Blocco)rowAgenda.Field<short>("Blocco_Flag");
            var bloccoData = rowAgenda.Field<DateTime>("Blocco_Data");
            var bloccoUsername = rowAgenda.Field<string>("Blocco_Username");

            attivita.blocco = new Blocco(bloccoFlag, bloccoUsername, bloccoData);

            var rowsRicettexAgenda = risultatoLetturaTabelleAgenda.RowsRicetteXAgenda.Where(dr => dr.Field<int>("Id_Agenda") == idAgenda).ToList();
            if (rowsRicettexAgenda.Any())
            {
                var associazionePk = new AssociazionePK();
                associazionePk.id_agenda = idAgenda;
                associazionePk.Ricetta_Cod = rowsRicettexAgenda.First().Field<int>("Ricetta_Cod");
                associazionePk.Ricetta_Operazione_Cod = rowsRicettexAgenda.First().Field<int>("Ricetta_Operazione_Cod");
                attivita.associazionePK = associazionePk;
            }

            var saCod = rowAgenda.Field<int>("Sa_Cod");
            var piva = rowAgenda.Field<string>("Piva");
            attivita.centroAziendale = CreaCentroAziendale(piva, saCod);

            var idAttivita = rowAgenda.Field<int?>("Id_Attivita") ?? 0;

            return new AttivitaConInfoBase(attivita, movimenti, infoOperazione, idAgenda, lavCod, piva, saCod, idAttivita);
        }

        private RicettaConInfoBase CreaRicettaConInfo(RisultatoTabelleRicette risultatoTabelleRicette, DataRow rowRicettaOperazione)
        {
            var ricettaOperazioneCod = rowRicettaOperazione.Field<int>("Ricetta_Operazione_Cod");
            var ricettaCod = rowRicettaOperazione.Field<int>("Ricetta_Cod");

            var rowRicetta = risultatoTabelleRicette.RowsRicette.First(r => r.Field<int>("Ricetta_Cod") == ricettaCod);

            var rowsDettagli = risultatoTabelleRicette.RowsRicetteDettagli
                .Where(dr => dr.Field<int>("Ricetta_Operazione_Cod") == ricettaOperazioneCod)
                .OrderBy(dr => dr.Field<int>("Ricetta_Dettaglio_Cod"))
                .ToList();

            var rowsDettagliTecnici = risultatoTabelleRicette.RowsRicetteDettaglioTecnico
                .Where(dr => dr.Field<int>("Ricetta_Operazione_Cod") == ricettaOperazioneCod)
                .OrderBy(dr => dr.Field<int>("Ricetta_Dettaglio_Cod"))
                .ThenBy(dr => dr.Field<int>("Ricetta_Tecnico_Cod"))
                .ToList();

            var rowsDestinazioni = risultatoTabelleRicette.RowsRicetteDestinazioni
                .Where(dr => dr.Field<int>("Ricetta_Operazione_Cod") == ricettaOperazioneCod)
                .OrderBy(dr => dr.Field<int>("Ricetta_Dettaglio_Cod"))
                .ThenBy(dr => dr.Field<int>("Ricetta_Destinazione_Cod"))
                .ToList();

            var ricetta = new RicettaAttivita()
            {
                Ricetta_Cod = ricettaCod,
                Tipo_Ricetta = rowRicetta.Field<short?>("Tipo_Ricetta"),
                Origine = rowRicetta.Field<string>("Origine"),
                Ricetta_Des = rowRicetta.Field<string>("Ricetta_Des"),
                Ricetta_Des_Long = rowRicetta.Field<string>("Ricetta_Des_Long"),
                Ricetta_Numero = rowRicetta.Field<string>("Ricetta_Numero"),
                Note = rowRicetta.Field<string>("Note"),
                Validita_Inizio = rowRicetta.Field<DateTime?>("Validita_Inizio"),
                Validita_Fine = rowRicetta.Field<DateTime?>("Validita_Fine"),
                Programmazione_Cod = rowRicetta.Field<int?>("Programmazione_Cod"),
                Piva = rowRicetta.Field<string>("Piva"),
                Sa_Cod = rowRicetta.Field<int?>("Sa_Cod") ?? 0,
            };

            var ricetta_operazione = new RicettaOperazioneAttivita()
            {
                Ricetta_Cod = ricettaCod,
                Ricetta_Operazione_Cod = ricettaOperazioneCod,
                Ricetta_SuperUser = rowRicettaOperazione.Field<string>("Ricetta_SuperUser"),
                Ricetta_Operazione_Des = rowRicettaOperazione.Field<string>("Ricetta_Operazione_Des"),
                Lav_Cod = rowRicettaOperazione.Field<int>("Lav_Cod"),
                Extra_Int = rowRicettaOperazione.Field<int?>("Extra_Int"),
                Validita_Inizio = rowRicettaOperazione.Field<DateTime?>("Validita_Inizio"),
                Ora = rowRicettaOperazione.Field<DateTime?>("Ora"),
                Validita_Fine = rowRicettaOperazione.Field<DateTime?>("Validita_Fine"),
                Note = rowRicettaOperazione.Field<string>("Note"),
                W_Anagrafica_Stati_Cod = rowRicettaOperazione.Field<int?>("W_Anagrafica_Stati_Cod"),
                Raccoglitore_Cod = rowRicettaOperazione.Field<int?>("Raccoglitore_Cod"),
                Invia_App = rowRicettaOperazione.Field<int?>("Invia_App"),
                APP_Ricetta_Operazione_ID = rowRicettaOperazione.Field<string>("APP_Ricetta_Operazione_ID"),
                Num_Protocollo = rowRicettaOperazione.Field<double?>("Num_Protocollo"),
                Id_Rcdpi = rowRicettaOperazione.Field<int?>("Id_Rcdpi"),
                Mezzo = rowRicettaOperazione.Field<short?>("Mezzo"),
            };

            var dettagli = new List<RicettaDettaglioAttivita>();
            foreach (var row in rowsDettagli)
            {
                var dettaglio = new RicettaDettaglioAttivita()
                {
                    Ricetta_SuperUser = row.Field<string>("Ricetta_SuperUser"),
                    Ricetta_Cod = row.Field<int>("Ricetta_Cod"),
                    Ricetta_Operazione_Cod = row.Field<int>("Ricetta_Operazione_Cod"),
                    Ricetta_Dettaglio_Cod = row.Field<int>("Ricetta_Dettaglio_Cod"),
                    Elem_Cod = row.Field<int?>("Elem_Cod") ?? 0,
                    Pro_Cod = row.Field<int?>("Pro_Cod") ?? 0,
                    Mat_Cod = row.Field<int?>("Mat_Cod") ?? 0,
                    Lotto = row.Field<string>("Lotto"),
                    Cau_Mov = row.Field<string>("Cau_Mov"),
                    ID_Attivita = row.Field<int?>("ID_Attivita"),
                    Qta = row.Field<double?>("Qta"),
                    Qta_Extra = row.Field<double?>("Qta_Extra"),
                    Qta_Extra_Totale = row.Field<double?>("Qta_Extra_Totale"),
                    Mezzo_Det = row.Field<short?>("Mezzo_Det"),
                    Udm_Cod = row.Field<int?>("Udm_Cod"),
                    Udm_Cod_Extra = row.Field<int?>("Udm_Cod_Extra"),
                    Extra_Int = row.Field<int?>("Extra_Int"),
                    DoseEtichetta = row.Field<string>("DoseEtichetta"),
                    DoseEtichetta_Value = row.Field<string>("DoseEtichetta_Value"),
                    PrincipiAttivi = row.Field<string>("PrincipiAttivi"),
                    PrincipiAttiviPesi = row.Field<string>("PrincipiAttiviPesi"),
                    PrincipiAttiviPercAbb = row.Field<string>("PrincipiAttiviPercAbb"),
                    Buffer = row.Field<string>("Buffer"),
                    Extra_Str = row.Field<string>("Extra_Str"),
                    TempoCarenza = row.Field<int?>("TempoCarenza"),
                    Polverulento = row.Field<short?>("Polverulento"),
                    Validita_Inizio = row.Field<DateTime?>("Validita_Inizio"),
                };

                dettagli.Add(dettaglio);
            }

            var dettagliTecnici = new List<RicettaDettaglioTecnicoAttivita>();

            foreach (var row in rowsDettagliTecnici)
            {
                var dettaglioTecnico = new RicettaDettaglioTecnicoAttivita()
                {
                    Ricetta_Operazione_Cod = row.Field<int>("Ricetta_Operazione_Cod"),
                    Ricetta_Dettaglio_Cod = row.Field<int>("Ricetta_Dettaglio_Cod"),
                    Ricetta_Tecnico_Cod = row.Field<int>("Ricetta_Tecnico_Cod"),
                    Qta_Ril = row.Field<double?>("Qta_Ril"),
                    Dose = row.Field<double?>("Dose"),
                    Parziale = row.Field<int?>("Parziale"),
                    Efficienza = row.Field<double?>("Efficienza"),
                    Nitrati = row.Field<short?>("Nitrati"),
                    Inn1_Data = row.Field<DateTime?>("Inn1_Data"),
                    Inn2_Data = row.Field<DateTime?>("Inn2_Data"),
                    Freatimetro = row.Field<double?>("Freatimetro"),
                    Dett_Cod = row.Field<int?>("Dett_Cod"),
                    Ditta_Cod = row.Field<int?>("Ditta_Cod"),
                    Av_Cod = row.Field<int?>("Av_Cod"),
                    Av_Gru = row.Field<int?>("Av_Gru"),
                    Soglia_Cod = row.Field<int?>("Soglia_Cod"),
                    Soglia_Quantita = row.Field<double?>("Soglia_Quantita"),
                    N = row.Field<double?>("N"),
                    P = row.Field<double?>("P"),
                    K = row.Field<double?>("K"),
                    Cu = row.Field<double?>("Cu"),
                    Mg = row.Field<double?>("Mg")
                };
                dettagliTecnici.Add(dettaglioTecnico);
            }

            var destinazioni = new List<RicettaDestinazioneAttivita>();

            foreach (var row in rowsDestinazioni)
            {
                var destinazione = new RicettaDestinazioneAttivita()
                {
                    Ricetta_SuperUser = row.Field<string>("Ricetta_SuperUser"),
                    Ricetta_Cod = row.Field<int>("Ricetta_Cod"),
                    Ricetta_Operazione_Cod = row.Field<int>("Ricetta_Operazione_Cod"),
                    Ricetta_Dettaglio_Cod = row.Field<int>("Ricetta_Dettaglio_Cod"),
                    Ricetta_Destinazione_Cod = row.Field<int>("Ricetta_Destinazione_Cod"),
                    Tipo_Destinazione = row.Field<double?>("Tipo_Destinazione"),
                    Qta = row.Field<double?>("Qta"),
                    Piva = row.Field<string>("Piva"),
                    Sa_Cod = row.Field<int?>("Sa_Cod") ?? 0,
                    Appezza = row.Field<int?>("Appezza") ?? 0,
                    Id_Reg = row.Field<int?>("Id_Reg") ?? 0,
                    Qta2 = row.Field<double>("Qta2"),
                    Sup_Riduzione_BufferZone = row.Field<double?>("Sup_Riduzione_BufferZone"),
                    Perc_Riduzione_Deriva = row.Field<double?>("Perc_Riduzione_Deriva"),
                    MagazzinoEsterno_Cod = row.Field<string>("MagazzinoEsterno_Cod"),
                    MagazzinoEsterno_Des = row.Field<string>("MagazzinoEsterno_Des"),
                    MagazzinoEsterno_Dettagli = row.Field<string>("MagazzinoEsterno_Dettagli")
                };
                destinazioni.Add(destinazione);
            }

            var infoOperazione = HelperAgenda.GetInfoOperazione(ricetta_operazione.Lav_Cod, Tipo_Attivita.Ricetta);
            if (infoOperazione.IsRilievo || infoOperazione.IsVisita || infoOperazione.IsCarico || infoOperazione.IsScarico || infoOperazione.IsRegistrazione)
                throw new Exception($"L'operazione con lav_cod {ricetta_operazione} non è ancora stata gestita nel mapping");

            Epoca epoca = null;
            var codiceAttivitaPersonalizzata = 0;
            Tipo_Raccolta? tipo_Raccolta = null;
            if (ricetta_operazione.Extra_Int is not null && ricetta_operazione.Extra_Int != 0)
            {
                if (infoOperazione.IsRaccolta)
                {
                    tipo_Raccolta = (Tipo_Raccolta)ricetta_operazione.Extra_Int.Value;
                }
                else
                {
                    if (infoOperazione.IsLavorazione)
                        codiceAttivitaPersonalizzata = ricetta_operazione.Extra_Int.Value;
                    else
                        epoca = new Epoca(ricetta_operazione.Extra_Int.Value);
                }
            }

            var attivita = new Attivita()
            {
                tipo = Tipo_Attivita.Ricetta,
                codice = ricetta_operazione.Ricetta_Operazione_Cod.ToString(),
                descrizione = ricetta_operazione.Ricetta_Operazione_Des,
                inizio = ricetta_operazione.Validita_Inizio.Value,
                oraInizio = ricetta_operazione.Ora ?? ricetta_operazione.Validita_Inizio.Value,
                fine = ricetta_operazione.Validita_Fine.Value,
                note = ricetta_operazione.Note,
                stato = (Stati)ricetta_operazione.W_Anagrafica_Stati_Cod,
                epoca = epoca,
                tipoRicetta = ricetta.Tipo_Ricetta is null ? Tipo_Ricetta.Standard : (Tipo_Ricetta)ricetta.Tipo_Ricetta.Value,
                raccoglitore = ricetta_operazione.Raccoglitore_Cod ?? 0,
                inviaRicetta = ricetta_operazione.Invia_App == 1,
                origine = ricetta.Origine,
                appRicettaOperazioneID = ricetta_operazione.APP_Ricetta_Operazione_ID,
            };

            if (tipo_Raccolta is not null)
            {
                attivita.tipoRaccolta = tipo_Raccolta.Value;
            }

            attivita.job = new Lavorazione(ricetta_operazione.Lav_Cod);
            attivita.testataRicetta = new TestataRicetta()
            {
                Ricetta_Cod = ricettaCod,
                Ricetta_Des = ricetta.Ricetta_Des,
                Ricetta_Des_Long = ricetta.Ricetta_Des_Long,
                Ricetta_Numero = ricetta.Ricetta_Numero,
                Note = ricetta.Note,
                Data_Da = ricetta.Validita_Inizio.Value,
                Data_A = ricetta.Validita_Fine.Value,
                pua = null
            };

            var rowsRicettexAgenda = risultatoTabelleRicette.RowsRicetteXAgenda.Where(dr => dr.Field<int>("Ricetta_Operazione_Cod") == ricettaOperazioneCod).ToList();

            if (rowsRicettexAgenda.Any())
            {
                var id_agenda = rowsRicettexAgenda.First().Field<int>("Id_Agenda");
                attivita.associazionePK = new AssociazionePK() 
                { 
                    id_agenda = id_agenda,
                    Ricetta_Cod = ricettaCod,
                    Ricetta_Operazione_Cod = ricettaOperazioneCod,
                };
            }

            return new RicettaConInfoBase()
            {
                Attivita = attivita,
                Ricetta = ricetta,
                RicettaOperazione = ricetta_operazione,
                Dettagli = dettagli,
                DettagliTecnici = dettagliTecnici,
                Destinazioni = destinazioni,
                InfoOperazione = infoOperazione,
                CodiceAttivitaPersonalizzata = codiceAttivitaPersonalizzata,
            };
        }
        private async Task PopolaAttivitaAsync(AttivitaConInfoBase attivitaConInfoBase, AgronicaCoreParametriTriple parametriTriple, RisultatoTabelleAgenda risultatoLetturaTabelleAgenda, 
            List<DataRow> dtRisorseUmaneRows, List<DataRow> dtInfoVarietaRows, List<DataRow> dtCodiciDestinazioniUsoRows)
        {
            
            var movimentoOperazione = GetMovimentoFromCausale(attivitaConInfoBase.Movimenti, attivitaConInfoBase.InfoOperazione.Cau_Mov);
            var movimentoScarico = GetMovimentoFromCausale(attivitaConInfoBase.Movimenti, CAU_MOV.CAU_SCARICO);
            var movimentoCarico = GetMovimentoFromCausale(attivitaConInfoBase.Movimenti, CAU_MOV.CAU_CARICO);

            attivitaConInfoBase.Attivita.note = movimentoOperazione.Mov_Desc;
            attivitaConInfoBase.Attivita.modalita = movimentoOperazione.Modalita;
            attivitaConInfoBase.Attivita.modalitaApplicazione = new BaseCodeDescr(movimentoOperazione.Modalita_Applicazione, "");
            attivitaConInfoBase.Attivita.oraInizio = movimentoOperazione.Ora;
            attivitaConInfoBase.Attivita.oraFine = movimentoOperazione.OraFine;

            var superficieTrattataTotale = 0m;
            var acquaTotale = 0m;

            if (attivitaConInfoBase.InfoOperazione.TipoCentroDiCosto == Tipo.ProdottoDaTrattare)
            {
                var primoProdottoDaTrattare = GetPrimoProdottoDaTrattare(movimentoOperazione, attivitaConInfoBase.InfoOperazione);
                if (primoProdottoDaTrattare is not null)
                {
                    attivitaConInfoBase.Attivita.utilizzoTerreno = await BuildUtilizzoTerrenoFromProdottoDaTrattareAsync(primoProdottoDaTrattare.Piva,
                        primoProdottoDaTrattare.Elem_Cod, primoProdottoDaTrattare.Mat_Cod, parametriTriple.ObjParametriServer);
                }
            }
            else
            {
                var primoImpianto = GetPrimoImpianto(movimentoOperazione);
                if (primoImpianto is not null)
                {
                    attivitaConInfoBase.Attivita.utilizzoTerreno = BuildUtilizzoTerreno(primoImpianto.Piva, primoImpianto.Sa_Cod,
                        primoImpianto.Appezza, primoImpianto.Id_Destinazione,dtInfoVarietaRows, dtCodiciDestinazioniUsoRows,
                        parametriTriple.ObjParametriServer);
                }
            }

            // EPOCA DPI / EPOCA FERTILIZZAZIONE / TIPO RACCOLTA
            CreaExtraInt(attivitaConInfoBase.Attivita, movimentoOperazione, attivitaConInfoBase.InfoOperazione);

            // ATTIVITA PERSONALIZZATA
            CreaAttivitaPersonalizzata(attivitaConInfoBase.Attivita, attivitaConInfoBase.IdAttivita, attivitaConInfoBase.LavCod);

            // NOTE
            var rowsNote = risultatoLetturaTabelleAgenda.RowsNote.Where(r => r.Field<int>("Id_Agenda") == attivitaConInfoBase.IdAgenda).ToList();
            CreaNote(attivitaConInfoBase.Attivita, rowsNote, risultatoLetturaTabelleAgenda.RowsNoteIntervento);

            // COSTI ACCESSORI
            CreaCostiAccessori(attivitaConInfoBase.Attivita, attivitaConInfoBase.Movimenti, dtRisorseUmaneRows);

            if (attivitaConInfoBase.InfoOperazione.TipoCentroDiCosto == Tipo.ProdottoDaTrattare)
                superficieTrattataTotale = CreaProdottoDaTrattareCdC(attivitaConInfoBase.Attivita, attivitaConInfoBase.InfoOperazione, movimentoOperazione);
            else
                superficieTrattataTotale = CreaEserciziCdC(attivitaConInfoBase.Attivita, attivitaConInfoBase.InfoOperazione, movimentoOperazione);

            acquaTotale = CreaRisorsaAcqua(attivitaConInfoBase.Attivita, attivitaConInfoBase.LavCod, movimentoOperazione, superficieTrattataTotale);
            CreaRisorseCausale(attivitaConInfoBase.Attivita, attivitaConInfoBase.LavCod, movimentoOperazione);

            if (movimentoOperazione is not null)
            {
                bool creaDettagliIrrigazioneDefault = attivitaConInfoBase.InfoOperazione.IsFertirrigazione;

                foreach (var movimentoDettaglioOperazione in movimentoOperazione.Movimenti_Dettagli)
                {
                    if (attivitaConInfoBase.InfoOperazione.IsTrattamento)
                    {
                        var dettCorrente = CreaDettaglioTrattamento(attivitaConInfoBase.Piva, attivitaConInfoBase.SaCod, attivitaConInfoBase.IdAgenda, attivitaConInfoBase.LavCod, attivitaConInfoBase.InfoOperazione, attivitaConInfoBase.Attivita, movimentoDettaglioOperazione,
                            movimentoScarico, superficieTrattataTotale, acquaTotale,
                            parametriTriple.ObjParametriSuperServer, parametriTriple.ObjParametriServer, 
                            parametriTriple.ObjParametriUtenti, movimentoOperazione);

                        if (dettCorrente != null)
                            attivitaConInfoBase.Attivita.risorse.Add(dettCorrente);
                    }

                    if (attivitaConInfoBase.InfoOperazione.IsFertilizzazione && !movimentoDettaglioOperazione.IsDettaglioIrrigazione)
                    {
                        var dettCorrente = CreaDettaglioFertilizzazione(attivitaConInfoBase.Piva, attivitaConInfoBase.SaCod, attivitaConInfoBase.IdAgenda, 
                            attivitaConInfoBase.Attivita, attivitaConInfoBase.InfoOperazione, movimentoDettaglioOperazione,
                            movimentoScarico, superficieTrattataTotale, acquaTotale,
                            parametriTriple.ObjParametriSuperServer, parametriTriple.ObjParametriServer, 
                            parametriTriple.ObjParametriUtenti);

                        if (dettCorrente != null)
                            attivitaConInfoBase.Attivita.risorse.Add(dettCorrente);
                    }

                    if (attivitaConInfoBase.InfoOperazione.IsSemina)
                    {
                        var dettCorrente = CreaDettaglioSemina(attivitaConInfoBase.Piva, attivitaConInfoBase.SaCod, attivitaConInfoBase.IdAgenda, attivitaConInfoBase.Attivita, 
                            attivitaConInfoBase.InfoOperazione, movimentoDettaglioOperazione,
                            movimentoScarico, superficieTrattataTotale, acquaTotale,
                            parametriTriple.ObjParametriSuperServer, parametriTriple.ObjParametriServer, 
                            parametriTriple.ObjParametriUtenti);

                        if (dettCorrente != null)
                            attivitaConInfoBase.Attivita.risorse.Add(dettCorrente);
                    }

                    if (attivitaConInfoBase.InfoOperazione.IsRaccolta)
                    {
                        var dettCorrente = CreaDettaglioRaccolta(attivitaConInfoBase.Piva, attivitaConInfoBase.SaCod, attivitaConInfoBase.IdAgenda, attivitaConInfoBase.Attivita, attivitaConInfoBase.InfoOperazione, movimentoDettaglioOperazione,
                            movimentoCarico, superficieTrattataTotale, acquaTotale,
                            parametriTriple.ObjParametriSuperServer, parametriTriple.ObjParametriServer, parametriTriple.ObjParametriUtenti);

                        dettCorrente.Opzioni_Raccolta.Ripartizione = (enum_Ripartizione_Raccolta)movimentoOperazione.Mezzo;
                        dettCorrente.Opzioni_Raccolta.Modalita = (enum_Modalita_Raccolta)movimentoOperazione.Modalita;

                        if (attivitaConInfoBase.Attivita.tipoRaccolta == Attivita.Tipo_Raccolta.Leggera_Con_Dettagli_Magazzino && movimentoCarico != null)
                            dettCorrente.dataIngresso = new DateTime(movimentoCarico.Data.Year, movimentoCarico.Data.Month, movimentoCarico.Data.Day,
                                movimentoCarico.Ora.Hour, movimentoCarico.Ora.Minute, 0);

                        DettaglioRaccolta found;
                        if (dettCorrente.MagazziniMovimentazioni != null)
                        {
                            found = attivitaConInfoBase.Attivita.risorse
                                .Where(r => r.classType == ClassType.DettaglioRaccolta)
                                .Where(r => ((RisorsaProdotto)r).prodotto.codice == dettCorrente.prodotto.codice
                                         && ((RisorsaProdotto)r).MagazziniMovimentazioni.Any())
                                .FirstOrDefault(r => ((RisorsaProdotto)r).MagazziniMovimentazioni.First().Lotto.ToUpper() == dettCorrente.MagazziniMovimentazioni.First().Lotto.ToUpper()
                                                  && ((RisorsaProdotto)r).MagazziniMovimentazioni.First().Magazzino.primaryKey.codice == dettCorrente.MagazziniMovimentazioni.First().Magazzino.primaryKey.codice)
                                as DettaglioRaccolta;

                            if (dettCorrente.MagazziniMovimentazioni.FirstOrDefault()?.Cod_Progetto != 0)
                                dettCorrente.Opzioni_Raccolta.GenerazioneLotto = enum_Generazione_Lotto_Raccolta.DA_ESERCIZO;
                        }
                        else
                        {
                            found = attivitaConInfoBase.Attivita.risorse
                                .Where(r => r.classType == ClassType.DettaglioRaccolta)
                                .FirstOrDefault(r => ((RisorsaProdotto)r).prodotto.codice == dettCorrente.prodotto.codice
                                                  && ((RisorsaProdotto)r).MagazziniMovimentazioni == null)
                                as DettaglioRaccolta;
                        }

                        dettCorrente.quantitaTotaleReale = movimentoDettaglioOperazione?.Qta ?? 0;

                        if (found != null)
                        {
                            var quantitaSuImpianto = new QuantitaSuImpianto
                            {
                                esercizioCDC = dettCorrente.QuantitaSuImpianti.First().esercizioCDC,
                                Qta = dettCorrente.QuantitaSuImpianti.First().Qta
                            };
                            found.QuantitaSuImpianti.Add(quantitaSuImpianto);
                            found.quantitaTotaleReale += dettCorrente.quantitaTotaleReale;
                        }
                        else
                        {
                            // Aggiornamento delle quantità su impianto già eseguito in CreaDettaglioRaccolta
                            if (dettCorrente.quantitaTotaleReale == 0 && dettCorrente.prodotto.codice > 0)
                                continue; // Evito di inserire dettagli creati con quantità nulla
                            attivitaConInfoBase.Attivita.risorse.Add(dettCorrente);
                        }
                    }

                    if (attivitaConInfoBase.InfoOperazione.IsIrrigazione)
                    {
                        var dettCorrenteList = CreaDettagliIrrigazione(attivitaConInfoBase.Attivita, attivitaConInfoBase.InfoOperazione, attivitaConInfoBase.Attivita.inizio, movimentoDettaglioOperazione);
                        if (dettCorrenteList != null)
                            attivitaConInfoBase.Attivita.risorse.AddRange(dettCorrenteList);
                    }

                    if (attivitaConInfoBase.InfoOperazione.IsFertirrigazione && movimentoDettaglioOperazione.IsDettaglioIrrigazione)
                    {
                        var dettCorrenteList = CreaDettagliIrrigazione(attivitaConInfoBase.Attivita, attivitaConInfoBase.InfoOperazione, attivitaConInfoBase.Attivita.inizio, movimentoDettaglioOperazione);
                        if (dettCorrenteList != null)
                        {
                            attivitaConInfoBase.Attivita.risorse.AddRange(dettCorrenteList);
                            creaDettagliIrrigazioneDefault = false;
                        }
                    }
                }

                // creo i dettagli irrigazione default per la nuova fertirrigazione
                if (attivitaConInfoBase.InfoOperazione.IsFertirrigazione && creaDettagliIrrigazioneDefault)
                {
                    decimal doseAcqua = acquaTotale / superficieTrattataTotale / 10;
                    var dettagliIrrigazione = CreaDettagliIrrigazione(attivitaConInfoBase.Attivita, doseAcqua);
                    if (dettagliIrrigazione != null)
                        attivitaConInfoBase.Attivita.risorse.AddRange(dettagliIrrigazione);
                }
            }
        }

        private async Task PopolaRicettaAsync(RicettaConInfoBase ricettaConInfoBase, AgronicaCoreParametriTriple parametriTriple, RisultatoTabelleRicette risultatoLetturaTabelleRicetta,
            List<DataRow> dtInfoVarietaRows, List<DataRow> dtCodiciDestinazioniUsoRows, List<DataRow> dtProgettiCodRows, List<DataRow> dtRisorseUmaneRows)
        {
            var causali = new List<string>()
            {
                CAU_MOV.CAU_IMPUTAZIONE_PARCOMACCHINE,
                CAU_MOV.CAU_IMPUTAZIONE_MANODOPERA,
                CAU_MOV.CAU_IMPUTAZIONE_TECNICO_RESPONSABILE,
                CAU_MOV.CAU_IMPUTAZIONE_TERZISTI,
                CAU_MOV.CAU_SCARICO,
                CAU_MOV.CAU_CARICO
            };

            var listaDettagliCampagna = ricettaConInfoBase.Dettagli
                .Where(d => !causali.Contains(d.Cau_Mov))
                .ToList();

            var pivaRicettaOperazione = ricettaConInfoBase.Ricetta.Piva;
            var saCodRicettaOperazione = ricettaConInfoBase.Ricetta.Sa_Cod;

            if (ricettaConInfoBase.Destinazioni.Any())
            {
                RicettaDestinazioneAttivita? destinazione = null;
                switch (ricettaConInfoBase.InfoOperazione.TipoCentroDiCosto)
                {
                    case Tipo.Esercizio:
                        destinazione = ricettaConInfoBase.Destinazioni
                            .Where(d => d.Tipo_Destinazione == TIPI_DESTINAZIONE.TIPO_DESTINAZIONE_IMPIANTO)
                            .FirstOrDefault();
                        break;
                    case Tipo.ProdottoDaTrattare:
                        destinazione = ricettaConInfoBase.Destinazioni
                            .Where(d => d.Tipo_Destinazione == TIPI_DESTINAZIONE.MAGAZZINO && d.Qta == 0)
                            .FirstOrDefault();
                        break;
                }

                if (destinazione != null)
                {
                    pivaRicettaOperazione = destinazione.Piva;
                    saCodRicettaOperazione = destinazione.Sa_Cod;
                }
            }

            ricettaConInfoBase.Attivita.centroAziendale = new CentroAziendale(new CentroAziendale.PK(saCodRicettaOperazione, pivaRicettaOperazione));

            decimal superficieTrattataTotale = 0;
            var listaImpianti = new Dictionary<string, Tuple<EsercizioCDC, List<RicettaDestinazioneAttivita>>>();
            var listaGiacenzeMagazzino = new Dictionary<string, Tuple<ProdottoDaTrattareCDC, List<RicettaDestinazioneAttivita>>>();
            bool creaDettagliIrrigazioneDefault = ricettaConInfoBase.InfoOperazione.IsFertirrigazione;

            switch (ricettaConInfoBase.InfoOperazione.TipoCentroDiCosto)
            {
                case Tipo.Esercizio:
                    {
                        foreach (var destinazione in ricettaConInfoBase.Destinazioni)
                        {
                            string chiave = destinazione.Piva + "_" + destinazione.Sa_Cod + "_" + destinazione.Appezza + "_" + destinazione.Id_Reg;

                            if (destinazione.Tipo_Destinazione == TIPO_DESTINAZIONE.TIPO_DESTINAZIONE_IMPIANTO && !listaImpianti.ContainsKey(chiave))
                            {
                                var progettoCod = dtProgettiCodRows.FirstOrDefault(p => p.Field<string>("Piva") == destinazione.Piva
                                    && p.Field<int>("Sa_Cod") == destinazione.Sa_Cod && p.Field<int>("Appezza") == destinazione.Appezza
                                    && p.Field<int>("Id_Reg") == destinazione.Id_Reg && p.Field<DateTime>("Validita_Inizio") <= ricettaConInfoBase.Attivita.inizio
                                    && p.Field<DateTime>("Validita_Fine") >= ricettaConInfoBase.Attivita.inizio)?.Field<int>("Progetto_Cod") ?? 0;

                                var esercizioCdc = CreaEsercizioCdC(ricettaConInfoBase.Attivita, ricettaConInfoBase.InfoOperazione,
                                    destinazione.Piva, destinazione.Sa_Cod, destinazione.Appezza, destinazione.Id_Reg, progettoCod,
                                    (decimal)destinazione.Qta2, (decimal)destinazione.Sup_Riduzione_BufferZone, (decimal)destinazione.Perc_Riduzione_Deriva);

                                esercizioCdc.quantita = (decimal)destinazione.Qta;

                                ricettaConInfoBase.Attivita.centriDiCosto.Add(esercizioCdc);
                                superficieTrattataTotale += (decimal)destinazione.Qta2;

                                listaImpianti.Add(chiave,
                                    new Tuple<EsercizioCDC, List<RicettaDestinazioneAttivita>>(esercizioCdc, new List<RicettaDestinazioneAttivita> { destinazione }));

                                ricettaConInfoBase.Attivita.utilizzoTerreno = BuildUtilizzoTerreno(
                                    destinazione.Piva, destinazione.Sa_Cod, destinazione.Appezza, destinazione.Id_Reg, dtInfoVarietaRows, dtCodiciDestinazioniUsoRows, parametriTriple.ObjParametriServer);
                            }
                            else if (listaImpianti.ContainsKey(chiave))
                            {
                                listaImpianti[chiave].Item2.Add(destinazione);
                            }

                            if (ricettaConInfoBase.InfoOperazione.IsIrrigazione)
                            {
                                // DT: le irrigazioni non hanno carichi nè scarichi, pertanto tutte le destinazioni sono impianti
                                var dettaglioCorrente = ricettaConInfoBase.Dettagli.FirstOrDefault(d => d.Ricetta_Dettaglio_Cod == destinazione.Ricetta_Dettaglio_Cod);
                                var dettaglioTecnicoCorrente = ricettaConInfoBase.DettagliTecnici.FirstOrDefault(dt => dt.Ricetta_Dettaglio_Cod == destinazione.Ricetta_Dettaglio_Cod);

                                // DT: si crea un dettaglioIrrigazione per ogni destinazione
                                var dettCorrente = CreaDettaglioIrrigazione(ricettaConInfoBase, dettaglioCorrente, dettaglioTecnicoCorrente,
                                    destinazione, dtProgettiCodRows);
                                ricettaConInfoBase.Attivita.risorse.Add(dettCorrente);
                            }

                            if (ricettaConInfoBase.InfoOperazione.IsFertirrigazione)
                            {
                                var dettaglioCorrente = ricettaConInfoBase.Dettagli.FirstOrDefault(d => d.Ricetta_Dettaglio_Cod == destinazione.Ricetta_Dettaglio_Cod);
                                var dettaglioTecnicoCorrente = ricettaConInfoBase.DettagliTecnici.FirstOrDefault(dt => dt.Ricetta_Dettaglio_Cod == destinazione.Ricetta_Dettaglio_Cod);

                                if (dettaglioCorrente.Elem_Cod == CATEGORIE_MAGAZZINO.ALTRE_MATERIE &&
                                    dettaglioCorrente.Mat_Cod == MAT_COD.MAT_COD_ACQUA_IRRIGAZIONE)
                                {
                                    var dettCorrente = CreaDettaglioIrrigazione(ricettaConInfoBase, dettaglioCorrente, dettaglioTecnicoCorrente,
                                        destinazione, dtProgettiCodRows);
                                    ricettaConInfoBase.Attivita.risorse.Add(dettCorrente);
                                    creaDettagliIrrigazioneDefault = false;
                                }
                            }
                        }
                        break;
                    }

                case Tipo.ProdottoDaTrattare:
                    {
                        foreach (var dettaglio in ricettaConInfoBase.Dettagli)
                        {
                            foreach (var dest in ricettaConInfoBase.Destinazioni)
                            {
                                if (dest.Ricetta_Dettaglio_Cod != dettaglio.Ricetta_Dettaglio_Cod)
                                    continue;

                                string chiaveProdotto = dettaglio.Mat_Cod + "_" + dettaglio.Lotto;

                                if (dest.Tipo_Destinazione == TIPO_DESTINAZIONE.TIPO_DESTINAZIONE_MAGAZZINO &&
                                    dest.Qta == 0 &&
                                    !listaGiacenzeMagazzino.ContainsKey(chiaveProdotto))
                                {
                                    var magazzinoPK = new Fabbricato(dest.Piva, dest.Sa_Cod, dest.Id_Reg, "");
                                    var prodottoDaTrattareCdC = new ProdottoDaTrattareCDC
                                    {
                                        giacenzaMagazzino = new MovimentoDiMagazzino
                                        {
                                            Magazzino = magazzinoPK,
                                            Prodotto = await LeggiProdottoDaTrattareAsync(dest.Piva, dettaglio.Elem_Cod, dettaglio.Mat_Cod, parametriTriple.ObjParametriServer),
                                            codice_progetto = 0, // Nelle ricette raggruppiamo escludendo il progetto_cod
                                            Lotto = dettaglio.Lotto
                                        },
                                        qtaTrattata = (decimal)dettaglio.Qta / 100
                                    };

                                    ricettaConInfoBase.Attivita.centriDiCosto.Add(prodottoDaTrattareCdC);
                                    superficieTrattataTotale += (decimal)dettaglio.Qta / 100;

                                    listaGiacenzeMagazzino.Add(chiaveProdotto,
                                        new Tuple<ProdottoDaTrattareCDC, List<RicettaDestinazioneAttivita>>(
                                            prodottoDaTrattareCdC, new List<RicettaDestinazioneAttivita> { dest }));

                                    ricettaConInfoBase.Attivita.utilizzoTerreno = await BuildUtilizzoTerrenoFromProdottoDaTrattareAsync(
                                        dest.Piva, dettaglio.Elem_Cod, dettaglio.Mat_Cod, parametriTriple.ObjParametriServer);
                                }
                                else if (listaGiacenzeMagazzino.ContainsKey(chiaveProdotto))
                                {
                                    listaGiacenzeMagazzino[chiaveProdotto].Item2.Add(dest);
                                }
                            }
                        }
                        break;
                    }
            }

            var acquaTotale = 0m;
            if (ricettaConInfoBase.InfoOperazione.IsFertilizzazione || ricettaConInfoBase.InfoOperazione.IsTrattamento)
            {
                var dettaglioTecnicoAcqua = ricettaConInfoBase.DettagliTecnici.FirstOrDefault(dt => dt.Ricetta_Dettaglio_Cod == 0);
                if (dettaglioTecnicoAcqua is not null)
                {
                    var risorsaAcqua = new RisorsaAcqua();

                    if (dettaglioTecnicoAcqua.Qta_Ril > 0)
                    {
                        risorsaAcqua.acqua = (decimal)dettaglioTecnicoAcqua.Qta_Ril;
                        risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.TOTALE;
                        acquaTotale = risorsaAcqua.acqua;
                    }
                    else
                    {
                        risorsaAcqua.acqua = (decimal)-dettaglioTecnicoAcqua.Qta_Ril;
                        risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.HA;
                        acquaTotale = risorsaAcqua.acqua * superficieTrattataTotale;
                    }

                    ricettaConInfoBase.Attivita.risorse.Add(risorsaAcqua);
                }
            }

            foreach (var dettaglioCampagna in listaDettagliCampagna)
            {
                var listaDettaglioTecnico = ricettaConInfoBase.DettagliTecnici.Where(dt => dt.Ricetta_Dettaglio_Cod == dettaglioCampagna.Ricetta_Dettaglio_Cod).ToList();
                var dettagliMagazzino = new List<RicettaDettaglioAttivita>();

                RicettaDettaglioAttivita dettaglioMagazzinoInnesco = null;
                RicettaDestinazioneAttivita destinazioneMagazzinoInnesco = null;

                if (ricettaConInfoBase.InfoOperazione.IsSemina || ricettaConInfoBase.InfoOperazione.IsRaccolta)
                {
                    dettagliMagazzino = ricettaConInfoBase.Dettagli
                        .Where(d => (d.Cau_Mov.ToString() == CAU_MOV.CAU_SCARICO || d.Cau_Mov.ToString() == CAU_MOV.CAU_CARICO)
                                 && d.Elem_Cod == dettaglioCampagna.Elem_Cod
                                 && d.Mat_Cod == dettaglioCampagna.Mat_Cod
                                 && d.Lotto.ToUpper() == dettaglioCampagna.Lotto.ToUpper())
                        .ToList();
                }
                else
                {
                    dettagliMagazzino = ricettaConInfoBase.Dettagli
                        .Where(d => (d.Cau_Mov.ToString() == CAU_MOV.CAU_SCARICO || d.Cau_Mov.ToString() == CAU_MOV.CAU_CARICO)
                                 && d.Elem_Cod == dettaglioCampagna.Elem_Cod
                                 && d.Pro_Cod == dettaglioCampagna.Pro_Cod)
                        .ToList();

                    // Controllo se ci sono degli inneschi scaricati da magazzino
                    if (ricettaConInfoBase.InfoOperazione.IsTrattamento)
                    {
                        int avCod = listaDettaglioTecnico.FirstOrDefault().Av_Cod ?? 0;
                        int avGruCod = listaDettaglioTecnico.FirstOrDefault().Av_Gru ?? 0;

                        dettaglioMagazzinoInnesco = ricettaConInfoBase.Dettagli
                            .Where(d => (d.Cau_Mov.ToString() == CAU_MOV.CAU_SCARICO || d.Cau_Mov.ToString() == CAU_MOV.CAU_CARICO)
                                     && d.Elem_Cod == CATEGORIE_MAGAZZINO.INNESCHI
                                     && (d.Pro_Cod == avCod || d.Pro_Cod == avGruCod))
                            .FirstOrDefault();

                        if (dettaglioMagazzinoInnesco != null)
                        {
                            destinazioneMagazzinoInnesco = ricettaConInfoBase.Destinazioni
                                .Where(dd => dd.Ricetta_Dettaglio_Cod == dettaglioMagazzinoInnesco.Ricetta_Dettaglio_Cod
                                          && dd.Tipo_Destinazione == 20)
                                .FirstOrDefault();
                        }
                    }
                }

                // se non ci sono scarichi, deve comunque procedere a creare i dettagli (escamotage per entrare nel foreach seguente)
                if (dettagliMagazzino.Count == 0)
                {
                    dettagliMagazzino.Add(new RicettaDettaglioAttivita());
                }

                foreach (var dettaglioMagazzinoItem in dettagliMagazzino)
                {
                    // mutable local copy: foreach variables are read-only in C#
                    var dettaglioMagazzino = dettaglioMagazzinoItem;
                    var destinazioneMagazzino = new RicettaDestinazioneAttivita();

                    // se il dettaglio dello scarico esiste, aggancio la sua destinazione
                    if (dettaglioMagazzino.Ricetta_Dettaglio_Cod > 0)
                    {
                        destinazioneMagazzino = ricettaConInfoBase.Destinazioni
                            .Where(dd => dd.Ricetta_Dettaglio_Cod == dettaglioMagazzino.Ricetta_Dettaglio_Cod
                                      && dd.Tipo_Destinazione == 20)
                            .FirstOrDefault();
                    }
                    else
                    {
                        dettaglioMagazzino = null; // era il dettaglio creato ad hoc per entrare nel ciclo, ma non esiste, lo rimetto a null
                    }

                    if (ricettaConInfoBase.InfoOperazione.IsLavorazione && dettaglioCampagna.ID_Attivita != 0)
                    {
                        ricettaConInfoBase.CodiceAttivitaPersonalizzata = dettaglioCampagna.ID_Attivita.Value; // DT: se si arriva da Gias, l'id_attivita è qui
                    }

                    if (ricettaConInfoBase.InfoOperazione.IsTrattamento)
                    {
                        if (ricettaConInfoBase.InfoOperazione.TipoCentroDiCosto == Tipo.ProdottoDaTrattare &&
                           (dettaglioCampagna.Elem_Cod == CATEGORIE_MAGAZZINO.TRASFORMATI_VEGETALI || dettaglioCampagna.Elem_Cod == CATEGORIE_MAGAZZINO.SEMENTI))
                        {
                            // Quando il centro di costo è un prodotto da trattare, non devo creare un DettaglioTrattamento dal current listaDettagliCampagna
                        }
                        else
                        {
                            var dettCorrente = CreaDettaglioTrattamento(pivaRicettaOperazione, dettaglioCampagna, dettaglioMagazzinoInnesco,
                                listaDettaglioTecnico.FirstOrDefault(), dettaglioMagazzino, destinazioneMagazzino,
                                destinazioneMagazzinoInnesco, parametriTriple.ObjParametriServer, superficieTrattataTotale, acquaTotale,
                                ricettaConInfoBase.RicettaOperazione.Lav_Cod, ricettaConInfoBase.Attivita, ricettaConInfoBase.RicettaOperazione.Mezzo.Value, ricettaConInfoBase.InfoOperazione, listaImpianti);

                            if (dettaglioCampagna.Cau_Mov.ToString() == CAU_MOV.CAU_LAVORAZIONE && superficieTrattataTotale > 0 && dettaglioCampagna.Qta_Extra_Totale == 0)
                            {
                                dettCorrente.doseHaReale = superficieTrattataTotale != 0 ? (decimal)dettaglioCampagna.Qta.Value / superficieTrattataTotale : 0;
                                dettCorrente.quantitaTotaleReale = superficieTrattataTotale != 0 ? (decimal)dettaglioCampagna.Qta.Value / superficieTrattataTotale : 0;
                            }

                            ricettaConInfoBase.Attivita.risorse.Add(dettCorrente);
                        }
                    }

                    if (ricettaConInfoBase.InfoOperazione.IsFertilizzazione)
                    {
                        if (dettaglioCampagna.Elem_Cod != CATEGORIE_MAGAZZINO.ALTRE_MATERIE || dettaglioCampagna.Mat_Cod != MAT_COD.MAT_COD_ACQUA_IRRIGAZIONE)
                        {
                            var dettCorrente = LeggiDettaglioFertilizzazione(pivaRicettaOperazione, dettaglioCampagna,
                                listaDettaglioTecnico.FirstOrDefault(), dettaglioMagazzino, destinazioneMagazzino,
                                superficieTrattataTotale, acquaTotale, ricettaConInfoBase.Attivita);
                            ricettaConInfoBase.Attivita.risorse.Add(dettCorrente);
                        }
                    }

                    if (ricettaConInfoBase.InfoOperazione.IsSemina)
                    {
                        var dettCorrente = new DettaglioSemina();
                        LeggiDettaglioSemina(pivaRicettaOperazione, dettCorrente, dettaglioCampagna,
                            listaDettaglioTecnico.FirstOrDefault(), dettaglioMagazzino, destinazioneMagazzino,
                            superficieTrattataTotale, acquaTotale, ricettaConInfoBase.Attivita);

                        if (dettCorrente.prodotto != null && dettCorrente.prodotto.codice > 0)
                        {
                            if (dettaglioCampagna.Cau_Mov.ToString() == CAU_MOV.CAU_LAVORAZIONE && superficieTrattataTotale > 0 && dettaglioCampagna.Qta_Extra_Totale == 0)
                            {
                                dettCorrente.doseHaReale = superficieTrattataTotale != 0 ? (decimal)dettaglioCampagna.Qta.Value / superficieTrattataTotale : 0;
                                dettCorrente.quantitaTotaleReale = (decimal)dettaglioCampagna.Qta;
                            }

                            ricettaConInfoBase.Attivita.risorse.Add(dettCorrente);
                        }
                    }

                    if (ricettaConInfoBase.InfoOperazione.IsRaccolta)
                    {
                        // If dettaglioCampagna.Lotto.ToUpper() != dettaglioMagazzino.Lotto.ToUpper() && dettaglioCampagna.Mat_Cod != dettaglioMagazzino.Mat_Cod
                        //     // Il dettaglio di magazzino non è collegato a quest dettaglio prodotto!
                        //     continue;

                        var dettCorrente = creaBaseDettaglioRaccolta(pivaRicettaOperazione,
                            dettaglioCampagna, listaDettaglioTecnico.FirstOrDefault(),
                            dettaglioMagazzino, destinazioneMagazzino,
                            superficieTrattataTotale, acquaTotale, ricettaConInfoBase.Attivita);

                        // listaDestinazioni contiene destinazioni impianti e magazzini per il singolo centro
                        // Prendo la chiave dell'impianto interessato dal dettaglio corrente
                        var keyImpianti = ricettaConInfoBase.Destinazioni
                            .Where(d => d.Ricetta_Dettaglio_Cod == dettaglioCampagna.Ricetta_Dettaglio_Cod && d.Tipo_Destinazione == 0)
                            .Select(d => d.Piva + "_" + d.Sa_Cod + "_" + d.Appezza + "_" + d.Id_Reg)
                            .ToList();
                        var giaAggiunti = new List<string>();

                        if (dettCorrente.MagazziniMovimentazioni.Count > 0)
                        {
                            dettCorrente.quantitaTotaleReale = dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Qta;

                            // trovo la risorsa con stessa combinazione prodotto/lotto/magazzino
                            var found = ricettaConInfoBase.Attivita.risorse
                                .Where(r => r.classType == "DettaglioRaccolta")
                                .Cast<DettaglioRaccolta>()
                                .Where(r => r.prodotto.codice == dettCorrente.prodotto.codice
                                         && r.MagazziniMovimentazioni.FirstOrDefault().Lotto.ToUpper() == dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Lotto.ToUpper()
                                         && r.MagazziniMovimentazioni.FirstOrDefault().Magazzino.primaryKey.codice == dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Magazzino.primaryKey.codice
                                         && r.MagazziniMovimentazioni.FirstOrDefault().Magazzino.primaryKey.centroAziendalePK.codice == dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Magazzino.primaryKey.centroAziendalePK.codice
                                         && r.MagazziniMovimentazioni.FirstOrDefault().Magazzino.primaryKey.centroAziendalePK.partitaIva == dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Magazzino.primaryKey.centroAziendalePK.partitaIva)
                                .FirstOrDefault();

                            if (found != null)
                            {
                                // La risorsa è già stata aggiunta, evitiamo di duplicarla
                                continue;
                            }
                        }

                        foreach (var key in keyImpianti)
                        {
                            if (!giaAggiunti.Contains(key))
                            {
                                aggiungiSuImpianto(dettCorrente, key, listaImpianti);
                                giaAggiunti.Add(key);
                            }
                        }

                        decimal superficieTotaleDettaglio = 0;

                        if (dettCorrente != null && dettCorrente.QuantitaSuImpianti != null && dettCorrente.QuantitaSuImpianti.Count > 0)
                        {
                            superficieTotaleDettaglio = dettCorrente.QuantitaSuImpianti
                                .Select(q => q.esercizioCDC.superficieTrattata)
                                .Aggregate((s1, s2) => s1 + s2);
                        }

                        foreach (var impianto in dettCorrente.QuantitaSuImpianti)
                        {
                            // TODO_DT: verificare se si può migliorare, soprattutto se rifaremo l'import ricetta da app passando dal modello e non da tabelle app_*
                            if (FindValueInDictOrigineAttivitaToImport(ricettaConInfoBase.Attivita.origine))
                            {
                                var dettaglioCampagnaCorrente = listaDettagliCampagna
                                    .Where(d => d.Lotto.ToUpper() == impianto.Lotto.ToUpper())
                                    .FirstOrDefault();
                                var dettaglioDestinazioneCorrente = ricettaConInfoBase.Destinazioni
                                    .Where(d => d.Ricetta_Dettaglio_Cod == dettaglioCampagnaCorrente.Ricetta_Dettaglio_Cod
                                             && d.Tipo_Destinazione == 0
                                             && d.Piva == impianto.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                                             && d.Sa_Cod == impianto.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                                             && d.Appezza == impianto.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice
                                             && d.Id_Reg == impianto.esercizioCDC.esercizio.impiantoPK.codice)
                                    .FirstOrDefault();

                                impianto.Qta = (decimal)dettaglioDestinazioneCorrente.Qta.Value;
                            }
                            else
                            {
                                var quotaSuperficieImpianto = impianto.esercizioCDC.superficieTrattata / superficieTotaleDettaglio;
                                impianto.Qta = Math.Round(dettCorrente.quantitaTotaleReale * quotaSuperficieImpianto, 2);
                            }
                        }

                        ricettaConInfoBase.Attivita.risorse.Add(dettCorrente);
                    }
                }
            }

            // creo i dettagli irrigazione default per la nuova fertirrigazione
            if (ricettaConInfoBase.InfoOperazione.IsFertirrigazione && creaDettagliIrrigazioneDefault)
            {
                decimal doseAcqua = acquaTotale / superficieTrattataTotale / 10;
                var dettagliIrrigazione = CreaDettagliIrrigazione(ricettaConInfoBase.Attivita, doseAcqua);
                if (dettagliIrrigazione != null)
                {
                    ricettaConInfoBase.Attivita.risorse.AddRange(dettagliIrrigazione);
                }
            }

            var rowsNote = risultatoLetturaTabelleRicetta.RowsNote
                .Where(r => r.Field<int>("Ricetta_Operazione_Cod") == ricettaConInfoBase.RicettaOperazione.Ricetta_Operazione_Cod)
                .ToList();

            CreaNote(ricettaConInfoBase.Attivita, rowsNote, risultatoLetturaTabelleRicetta.RowsNoteIntervento);

            CreaCostiAccessori(ricettaConInfoBase.Attivita, ricettaConInfoBase.Dettagli, dtRisorseUmaneRows);

            ricettaConInfoBase.Attivita.attivitaPersonalizzata = ricettaConInfoBase.CodiceAttivitaPersonalizzata != 0 
                ? new AttivitaPersonalizzata(ricettaConInfoBase.CodiceAttivitaPersonalizzata) 
                : null;

            // Split agenzie
            var listaRisorseToAdd = new List<RisorsaProdotto>();

            if (ricettaConInfoBase.Attivita.risorse != null)
            {
                foreach (var risorsa in ricettaConInfoBase.Attivita.risorse)
                {
                    if (risorsa.GetType().BaseType == typeof(RisorsaProdotto))
                    {
                        var risorsaProdotto = (RisorsaProdotto)risorsa;

                        if (risorsaProdotto.MagazziniMovimentazioni != null &&
                            risorsaProdotto.MagazziniMovimentazioni.Count > 1)
                        {
                            var risorsaProdottoOrig = risorsaProdotto.Clona(); // serve per mantenere la lista originale dei rilevamenti

                            // creo una risorsa prodotto per ogni rilevamento di magazzino, ognuno con un rilevamento di magazzino (invece di n come può capitare se ci sono agenzie)
                            for (int i = 0; i <= risorsaProdotto.MagazziniMovimentazioni.Count - 1; i++)
                            {
                                if (i == 0) // preservo la prima risorsaProdotto (per non doverla rimuovere da attivita.risorse) ma lascio solo il primo rilevamento di magazzino
                                {
                                    risorsaProdotto.MagazziniMovimentazioni.Clear();
                                    risorsaProdotto.MagazziniMovimentazioni.Add(risorsaProdottoOrig.MagazziniMovimentazioni[i]);
                                }
                                else
                                {
                                    var risorsaProdottoNew = risorsaProdottoOrig.Clona();
                                    risorsaProdottoNew.MagazziniMovimentazioni.Clear();
                                    risorsaProdottoNew.MagazziniMovimentazioni.Add(risorsaProdottoOrig.MagazziniMovimentazioni[i]);
                                    listaRisorseToAdd.Add(risorsaProdottoNew);
                                }
                            }
                        }
                    }
                }
            }

            ricettaConInfoBase.Attivita.risorse.AddRange(listaRisorseToAdd);
        }

        private async Task<Prodotto> LeggiProdottoDaTrattareAsync(string piva, int elem_Cod, int mat_Cod, AgronicaCoreParametriServer parametriServer)
        {
            int veg_cod = 0;
            string veg_des = "";

            var dtMateriePrime = await _materiePrimeDal.LeggiAsync(piva, elem_Cod, mat_Cod, parametriServer);
            if (dtMateriePrime is not null && dtMateriePrime.Rows.Count > 0)
            {
                var row = dtMateriePrime.Rows[0];
                veg_cod = row.Field<int>("Veg_Cod");
                veg_des = row.Field<string>("Veg_Des");
            }

            return new Prodotto(mat_Cod)
            {
                elemCod = elem_Cod,
                specie = new Specie(veg_cod, veg_des),
            };
        }

        /// <summary>
        /// Dizionario per mappare gli import che utilizzano come tracciato AgronicaCoreStd\AgronicaCoreDTOStd\InData\Demetra\Attivita.cs
        /// </summary>
        public static Dictionary<DatiApp, string> DictOrigineAttivitaToImport = new Dictionary<DatiApp, string>
        {
            { DatiApp.AttivitaDemetra, OrigineApp.Demetra },
            { DatiApp.AttivitaNewAgri, OrigineApp.PUA },
            { DatiApp.RicetteDemetra, OrigineApp.Demetra }
        };

        public static bool FindValueInDictOrigineAttivitaToImport(string val)
        {
            bool find = false;

            if (!string.IsNullOrEmpty(val))
            {
                int index = DictOrigineAttivitaToImport.Values.ToList()
                    .FindIndex(x => x.ToUpper() == val.ToUpper());

                if (index > -1)
                {
                    find = true;
                }
            }

            return find;
        }

        private DettaglioIrrigazione CreaDettaglioIrrigazione(RicettaConInfoBase ricettaConInfoBase, RicettaDettaglioAttivita dettaglio, RicettaDettaglioTecnicoAttivita tecnico, 
            RicettaDestinazioneAttivita destinazione, List<DataRow> dtProgettiCodRows)
        {
            var dettaglioIrrigazione = new DettaglioIrrigazione();

            if (tecnico is not null)
            {
                dettaglioIrrigazione.QtaRilevata = (decimal?)tecnico.Qta_Ril ?? 0;
                dettaglioIrrigazione.Ore = (decimal?)tecnico.Dose ?? 0;
                dettaglioIrrigazione.Portata = tecnico.Parziale ?? 0;

                if (tecnico.Efficienza == 0)
                {
                    dettaglioIrrigazione.Efficienza = 100.0m;

                    if (tecnico.Nitrati == 0)
                    {
                        dettaglioIrrigazione.Frequenza = 1;
                        dettaglioIrrigazione.DataInizio = ricettaConInfoBase.Attivita.inizio;
                        dettaglioIrrigazione.DataFine = ricettaConInfoBase.Attivita.inizio;
                    }
                    else
                    {
                        dettaglioIrrigazione.Frequenza = (short?)tecnico.Nitrati ?? 0;
                        dettaglioIrrigazione.DataInizio = tecnico.Inn1_Data.Value;
                        dettaglioIrrigazione.DataFine = tecnico.Inn2_Data.Value;
                    }
                }
                else 
                {
                    dettaglioIrrigazione.Efficienza = (decimal?)tecnico.Efficienza ?? 0;
                    dettaglioIrrigazione.Frequenza = (short?)tecnico.Nitrati ?? 0;
                    dettaglioIrrigazione.DataInizio = tecnico.Inn1_Data.Value;
                    dettaglioIrrigazione.DataFine = tecnico.Inn2_Data.Value;
                }

                dettaglioIrrigazione.tipoIrrigazione = new TipoIrrigazione((int)tecnico.Freatimetro.Value);
                dettaglioIrrigazione.unitaDiMisura = new UnitaDiMisura(tecnico.Dett_Cod.Value);
                dettaglioIrrigazione.consiglioIrrigazione = new Consiglio_Irrigazione(0, "") 
                { 
                    qtaAcqua = 0, 
                    dataConsiglio = CostantiPersonalizzate.AGRODATAINIZIO_DATE
                };

                if (tecnico.Ditta_Cod.HasValue && tecnico.Ditta_Cod != 0)
                {
                    dettaglioIrrigazione.macchina = new ParcoMacchine { codice = tecnico.Ditta_Cod.Value };
                }
                else
                {
                    dettaglioIrrigazione.macchina = null;
                }
            }

            if (destinazione is not null)
            {
                var progettoCod = dtProgettiCodRows.FirstOrDefault(p => p.Field<string>("Piva") == destinazione.Piva
                    && p.Field<int>("Sa_Cod") == destinazione.Sa_Cod && p.Field<int>("Appezza") == destinazione.Appezza
                    && p.Field<int>("Id_Reg") == destinazione.Id_Reg && p.Field<DateTime>("Validita_Inizio") <= ricettaConInfoBase.Attivita.inizio
                    && p.Field<DateTime>("Validita_Fine") >= ricettaConInfoBase.Attivita.inizio)?.Field<int>("Progetto_Cod") ?? 0;

                dettaglioIrrigazione.QtaTotale = (decimal)destinazione.Qta;
                dettaglioIrrigazione.esercizioCDC = CreaEsercizioCdC(
                    ricettaConInfoBase.Attivita, ricettaConInfoBase.InfoOperazione, destinazione.Piva, destinazione.Sa_Cod, destinazione.Appezza, destinazione.Id_Reg,
                    progettoCod, (decimal)destinazione.Qta2, (decimal)destinazione.Sup_Riduzione_BufferZone, (decimal)destinazione.Perc_Riduzione_Deriva);
                dettaglioIrrigazione.esercizioCDC.quantita = (decimal)destinazione.Qta;
            }

            return dettaglioIrrigazione;
        }

        private List<DettaglioIrrigazione> CreaDettagliIrrigazione(Attivita attivita, decimal doseAcqua)
        {
            var dettagliIrrigazione = new List<DettaglioIrrigazione>();

            foreach (var cdc in attivita.centriDiCosto)
            {
                if (cdc.classType == ClassType.EsercizioCDC)
                {
                    var esercizioCDC = (EsercizioCDC)cdc;
                    var dettCorrente = new DettaglioIrrigazione
                    {
                        QtaRilevata = doseAcqua,
                        Ore = 0,
                        Portata = 0,
                        Efficienza = 100m,
                        Frequenza = 1,
                        DataInizio = attivita.inizio,
                        DataFine = attivita.inizio,
                        tipoIrrigazione = new TipoIrrigazione(0),
                        unitaDiMisura = new UnitaDiMisura((int)Enum_UnitaMisura.METRI3__HA),
                        QtaTotale = doseAcqua * esercizioCDC.superficieTrattata,
                        esercizioCDC = esercizioCDC,
                        consiglioIrrigazione = new Consiglio_Irrigazione(0, "") { qtaAcqua = 0, dataConsiglio = CostantiPersonalizzate.AGRODATAINIZIO_DATE },
                        macchina = null
                    };
                    dettagliIrrigazione.Add(dettCorrente);
                }
            }

            return dettagliIrrigazione;
        }

        private void aggiungiSuImpianto(
            DettaglioRaccolta dettCorrente,
            string key,
            Dictionary<string, Tuple<EsercizioCDC, List<RicettaDestinazioneAttivita>>> listaImpianti)
        {
            var itemImpianto = listaImpianti[key];
            var quantitaSuImpianto = new QuantitaSuImpianto();

            quantitaSuImpianto.Prodotto = dettCorrente.prodotto;
            quantitaSuImpianto.esercizioCDC = itemImpianto.Item1;
            quantitaSuImpianto.Qta = 0;

            if (dettCorrente.MagazziniMovimentazioni.FirstOrDefault() == null)
            {
                quantitaSuImpianto.Lotto = "";
                quantitaSuImpianto.Magazzino = null;
            }
            else
            {
                quantitaSuImpianto.Lotto = dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Lotto;
                quantitaSuImpianto.Magazzino = dettCorrente.MagazziniMovimentazioni.FirstOrDefault().Magazzino;
            }

            dettCorrente.QuantitaSuImpianti.Add(quantitaSuImpianto);
        }

        private List<DettaglioIrrigazione> CreaDettagliIrrigazione(
            Attivita attivita,
            InfoOperazione infoOperazione,
            DateTime data,
            MovDettaglioAttivita movimentoDettaglioOperazione)
        {
            var dettagliIrrigazione = new List<DettaglioIrrigazione>();

            if (movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici != null &&
                movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Any())
            {
                foreach (var movimento_Dettaglio_Tecnico in movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici)
                {
                    if (movimentoDettaglioOperazione.Movimenti_Destinazioni != null &&
                        movimentoDettaglioOperazione.Movimenti_Destinazioni.Any())
                    {
                        foreach (var movimento_Dettaglio_Destinazione in movimentoDettaglioOperazione.Movimenti_Destinazioni)
                        {
                            var dettCorrente = new DettaglioIrrigazione
                            {
                                QtaRilevata = movimento_Dettaglio_Tecnico.Qta_Ril,
                                Ore = movimento_Dettaglio_Tecnico.Dose,
                                Portata = movimento_Dettaglio_Tecnico.Parziale
                            };

                            // irrigazione salvata tramite interfaccia vecchia (IrrigazioneBS.aspx)
                            if (movimento_Dettaglio_Tecnico.Efficienza == 0)
                            {
                                dettCorrente.Efficienza = 100;
                                if (movimento_Dettaglio_Tecnico.Nitrati == 0)
                                {
                                    dettCorrente.Frequenza = 1;
                                    dettCorrente.DataInizio = data;
                                    dettCorrente.DataFine = data;
                                }
                                else
                                {
                                    dettCorrente.Frequenza = movimento_Dettaglio_Tecnico.Nitrati;
                                    dettCorrente.DataInizio = movimento_Dettaglio_Tecnico.Inn1_data;
                                    dettCorrente.DataFine = DateTime.ParseExact(movimento_Dettaglio_Tecnico.Inn2_data, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                }
                            }
                            else // irrigazione salvata tramite interfaccia nuova (Angular)
                            {
                                dettCorrente.Efficienza = movimento_Dettaglio_Tecnico.Efficienza;
                                dettCorrente.Frequenza = movimento_Dettaglio_Tecnico.Nitrati;
                                dettCorrente.DataInizio = movimento_Dettaglio_Tecnico.Inn1_data;
                                dettCorrente.DataFine = DateTime.ParseExact(movimento_Dettaglio_Tecnico.Inn2_data, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            }

                            dettCorrente.tipoIrrigazione = new TipoIrrigazione((int)movimento_Dettaglio_Tecnico.Freatimetro);
                            dettCorrente.unitaDiMisura = new UnitaDiMisura(movimento_Dettaglio_Tecnico.dett_cod);
                            dettCorrente.QtaTotale = movimento_Dettaglio_Destinazione.Qta;
                            dettCorrente.esercizioCDC = CreaEsercizioCdC(
                                attivita, infoOperazione,
                                movimento_Dettaglio_Destinazione.Piva,
                                movimento_Dettaglio_Destinazione.Sa_Cod,
                                movimento_Dettaglio_Destinazione.Appezza,
                                movimento_Dettaglio_Destinazione.Id_Destinazione,
                                movimento_Dettaglio_Destinazione.Progetto_Cod,
                                movimento_Dettaglio_Destinazione.Qta2,
                                movimento_Dettaglio_Destinazione.Sup_Riduzione_BufferZone,
                                movimento_Dettaglio_Destinazione.Perc_Riduzione_Deriva);
                            dettCorrente.consiglioIrrigazione = CreaConsiglio_Irrigazione(movimento_Dettaglio_Tecnico);
                            dettCorrente.macchina = movimento_Dettaglio_Tecnico.Ditta_cod != 0
                                ? new ParcoMacchine { codice = movimento_Dettaglio_Tecnico.Ditta_cod }
                                : null;

                            dettagliIrrigazione.Add(dettCorrente);
                        }
                    }
                }
            }

            return dettagliIrrigazione;
        }


        private Consiglio_Irrigazione CreaConsiglio_Irrigazione(MovimentoDettaglioTecnicoAttivita movimento_Dettaglio_Tecnico)
        {
            var consiglioIrrigazione = new Consiglio_Irrigazione(
                Convert.IsDBNull(movimento_Dettaglio_Tecnico.Extra_Int) ? 0 : movimento_Dettaglio_Tecnico.Extra_Int, "")
            {
                qtaAcqua = (Convert.IsDBNull(movimento_Dettaglio_Tecnico.ExtraStr) || !decimal.TryParse(movimento_Dettaglio_Tecnico.ExtraStr, out decimal qtaAcqua))
                    ? 0
                    : qtaAcqua,
                dataConsiglio = Convert.IsDBNull(movimento_Dettaglio_Tecnico.Extra_Date)
                    ? CostantiPersonalizzate.AGRODATAINIZIO_DATE
                    : movimento_Dettaglio_Tecnico.Extra_Date
            };

            return consiglioIrrigazione;
        }

        private void CreaRisorseCausale(Attivita attivita, int lavCod, MovimentoAttivita movimentoOperazione)
        {
            if (lavCod == LAV_COD.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA)
            {
                attivita.risorse.AddRange(
                    movimentoOperazione.Movimenti_Dettagli_Tecnici
                        .Where(c => c.Id_Mov_Det == 0 && c.dett_cod != 0)
                        .Select(c => new RisorsaCausale { id = c.dett_cod })
                );
            }
        }

        private MovimentoAttivita GetMovimentoFromCausale(List<MovimentoAttivita> movimenti, string causale)
        {
            var movimento = movimenti.FirstOrDefault(m => m.Cau_Mov == causale);
            return movimento;
        }

        private void AggiungiDettagliSuMovimento(MovimentoAttivita movimento, List<DataRow> rowsMovimentiDettagliMovimento, List<DataRow> rowsMovDestinazioniMovimento, List<DataRow> rowsMovDettaglioTecnicoMovimento)
        {
            movimento.Movimenti_Dettagli = new();
            foreach (var row in rowsMovimentiDettagliMovimento)
            {
                var movDet = new MovDettaglioAttivita
                {
                    Piva = row.Field<string>("Piva"),
                    Id_Agenda = row.Field<int>("Id_Agenda"),
                    Id_Mov = row.Field<int>("Id_Mov"),
                    Id_Mov_Det = row.Field<int>("Id_Mov_Det"),
                    Elem_Cod = row.Field<int>("Elem_Cod"),
                    Pro_Cod = row.Field<int>("Pro_Cod"),
                    Mat_Cod = row.Field<int>("Mat_Cod"),
                    Qta = (decimal)row.Field<double>("Qta"),
                    Udm_Cod = row.Field<int>("Udm_Cod"),
                    Extra_Int = row.Field<int>("Extra_Int"),
                    Extra_Str = row.Field<string>("Extra_Str"),
                    Lotto = row.Field<string>("Lotto"),
                    Cod_Progetto = row.Field<int>("Cod_Progetto"),
                    Cal_Cod = row.Field<int>("Cal_Cod"),
                    Udm_Cod_Extra = row.Field<int>("Udm_Cod_Extra"),
                    Qta_Extra = (decimal)row.Field<double>("Qta_Extra"),
                    TempoCarenza = row.Field<int?>("TempoCarenza") ?? 0,
                    DoseEtichetta = row.Field<string>("DoseEtichetta") ?? "",
                    DoseEtichetta_Value = row.Field<string>("DoseEtichetta_Value") ?? "",
                    PrincipiAttivi = row.Field<string>("PrincipiAttivi") ?? "",
                    PrincipiAttiviPesi = row.Field<string>("PrincipiAttiviPesi") ?? "",
                    Buffer = row.Field<string>("Buffer") ?? "",
                    Qta_Extra_Totale = (decimal?)row.Field<double?>("Qta_Extra_Totale") ?? 0m,
                    Mezzo_Det = row.Field<short?>("Mezzo_Det") ?? -1,
                    PrincipiAttiviPercAbb = row.Field<string>("PrincipiAttiviPercAbb") ?? "",
                    Polverulento = row.Field<short?>("Polverulento") ?? 0,
                    Movimenti_Destinazioni = new(),
                };

                var rowsMovDestinazioniXDettaglio = rowsMovDestinazioniMovimento.Where(dr => dr.Field<int>("Id_Mov_Det") == movDet.Id_Mov_Det);
                foreach (var rowDestinazione in rowsMovDestinazioniXDettaglio)
                {
                    var movDest = new MovimentoDestinazioneAttivita
                    {
                        Piva = rowDestinazione.Field<string>("Piva"),
                        Sa_Cod = rowDestinazione.Field<int>("Sa_Cod"),
                        Id_Agenda = rowDestinazione.Field<int>("Id_Agenda"),
                        Id_Mov = rowDestinazione.Field<int>("Id_Mov"),
                        Id_Mov_Det = rowDestinazione.Field<int>("Id_Mov_Det"),
                        Appezza = rowDestinazione.Field<int>("Appezza"),
                        Id_Destinazione = rowDestinazione.Field<int>("Id_Destinazione"),
                        Progetto_Cod = rowDestinazione.Field<int>("Progetto_Cod"),
                        Tipo = rowDestinazione.Field<int>("Tipo_Destinazione"),
                        Qta = (decimal)rowDestinazione.Field<double>("Qta"),
                        Qta2 = (decimal)rowDestinazione.Field<double>("Qta2"),
                        Sup_Riduzione_BufferZone = (decimal?)rowDestinazione.Field<double?>("Sup_Riduzione_BufferZone") ?? 0m,
                        Perc_Riduzione_Deriva = (decimal?)rowDestinazione.Field<double?>("Perc_Riduzione_Deriva") ?? 0m,
                    };

                    movDet.Movimenti_Destinazioni.Add(movDest);
                }

                var rowsDettagliTecniciPerDettaglio = rowsMovDettaglioTecnicoMovimento.Where(dr => dr.Field<int>("Id_Mov_Det") == movDet.Id_Mov_Det);
                foreach (var rowDettaglioTecnico in rowsDettagliTecniciPerDettaglio)
                {
                    var movimentoDettaglioTecnico = MapRowToDettaglioTecnico(rowDettaglioTecnico);
                    movDet.Movimenti_Dettagli_Tecnici.Add(movimentoDettaglioTecnico);
                }

                movimento.Movimenti_Dettagli.Add(movDet);
            }
        }

        private void AggiungiDettagliTecniciSuMovimento(MovimentoAttivita movimento, List<DataRow> rowsMovDettaglioTecnicoMovimento)
        {
            movimento.Movimenti_Dettagli_Tecnici = new();
            foreach (var row in rowsMovDettaglioTecnicoMovimento)
            {
                var movimentoDettaglioTecnico = MapRowToDettaglioTecnico(row);
                movimento.Movimenti_Dettagli_Tecnici.Add(movimentoDettaglioTecnico);
            }
        }

        private MovimentoDettaglioTecnicoAttivita MapRowToDettaglioTecnico(DataRow row)
        {
            var movimentoDettaglioTecnico = new MovimentoDettaglioTecnicoAttivita
            {
                Id_Agenda = row.Field<int>("Id_Agenda"),
                Id_Mov = row.Field<int>("Id_Mov"),
                Id_Mov_Det = row.Field<int>("Id_Mov_Det"),
                Qta_Ril = (decimal)row.Field<double>("Qta_Ril"),
                Av_Cod = row.Field<int>("Av_Cod"),
                Av_Gru = row.Field<int>("Av_Gru"),
                N = (decimal)row.Field<double>("N"),
                P = (decimal)row.Field<double>("P"),
                K = (decimal)row.Field<double>("K"),
                M = (decimal)row.Field<double>("Mg"),
                Efficienza = (decimal?)row.Field<double?>("Efficienza") ?? 0m,
                Cu = (decimal?)row.Field<double?>("Cu") ?? 0m,
                Ditta_cod = row.Field<int>("Ditta_cod"),
                Dose = (decimal)row.Field<double>("Dose"),
                Freatimetro = (decimal)row.Field<double>("Freatimetro"),
                Inn1_data = row.Field<DateTime?>("Inn1_data") ?? DateTime.Now,
                Inn2_data = row.Field<DateTime?>("Inn2_data")?.ToString() ?? "",
                Sigla_av = row.Field<string>("Sigla_AV") ?? "",
                ExtraStr = row.Field<string>("Extra_Str") ?? "",
                Extra_Int = row.Field<int?>("Extra_Int") ?? 0,
                Extra_Date = row.Field<DateTime?>("Extra_Date") ?? new DateTime(),
                dett_cod = row.Field<int?>("dett_cod") ?? 0,
                Soglia_Cod = row.Field<int?>("Soglia_Cod") ?? 0,
                Soglia_Quantita = (decimal?)row.Field<double?>("Soglia_Quantita") ?? 0m,
                Parziale = row.Field<int?>("Parziale") ?? 0,
                Nitrati = row.Field<short?>("Nitrati") ?? 0,
            };

            return movimentoDettaglioTecnico;
        }

        private MovimentoAttivita MapMovimento(DataRow row)
        {
            var movimento = new MovimentoAttivita
            {
                Id_Agenda = row.Field<int>("Id_Agenda"),
                Id_Mov = row.Field<int>("Id_Mov"),
                Cau_Mov = row.Field<string>("Cau_Mov"),
                Mov_Desc = row.Field<string>("Mov_Desc"),
                Data = row.Field<DateTime>("Data_Movimento"),
                Mezzo = row.Field<short>("Mezzo"),
                Ora = row.Field<DateTime>("Ora"),
                Extra_Int = row.Field<int>("Extra_Int"),
                Modalita = row.Field<short>("Modalita"),
                Modalita_Applicazione = row.Field<int?>("Modalita_Applicazione") ?? 0,
                OraFine = row.Field<DateTime?>("OraFine") ?? default,
            };

            return movimento;
        }


        private MovDettaglioAttivita GetPrimoProdottoDaTrattare(MovimentoAttivita movimentoOperazione, InfoOperazione infoOperazione)
        {
            MovDettaglioAttivita primoProdottoDaTrattare = null;

            if (movimentoOperazione != null &&
                movimentoOperazione.Movimenti_Dettagli != null &&
                movimentoOperazione.Movimenti_Dettagli.Any())
            {
                foreach (var movimentoDettaglio in movimentoOperazione.Movimenti_Dettagli)
                {
                    if ((movimentoDettaglio.Elem_Cod == ELEM_COD.SEMENTI || movimentoDettaglio.Elem_Cod == ELEM_COD.TRASFORMATI_VEGETALI) &&
                        movimentoDettaglio.Mat_Cod != 0)
                    {
                        primoProdottoDaTrattare = movimentoDettaglio;
                        break;
                    }
                }
            }

            return primoProdottoDaTrattare;
        }

        private MovimentoDestinazioneAttivita GetPrimoImpianto(MovimentoAttivita movimentoOperazione)
        {
            MovimentoDestinazioneAttivita primoImpianto = null;

            if (movimentoOperazione != null
                && movimentoOperazione.Movimenti_Dettagli != null
                && movimentoOperazione.Movimenti_Dettagli.Any()
                && movimentoOperazione.Movimenti_Dettagli[0].Movimenti_Destinazioni != null
                && movimentoOperazione.Movimenti_Dettagli[0].Movimenti_Destinazioni.Any())
            {
                primoImpianto = movimentoOperazione.Movimenti_Dettagli[0].Movimenti_Destinazioni[0];
            }

            return primoImpianto;
        }

        private void CreaExtraInt(Attivita attivita, MovimentoAttivita movimentoOperazione, InfoOperazione infoOperazione)
        {
            if (movimentoOperazione.Extra_Int != 0)
            {
                if (infoOperazione.IsRaccolta)
                {
                    attivita.tipoRaccolta = (Attivita.Tipo_Raccolta)movimentoOperazione.Extra_Int;
                }
                else
                {
                    var epoca = new Epoca(movimentoOperazione.Extra_Int);
                    attivita.epoca = epoca;
                }
            }
        }

        private void CreaNote(Attivita attivita, List<DataRow> dataRowsNote, List<DataRow> dataRowsNoteIntervento)
        {
            attivita.noteIntervento = new List<NoteIntervento>();

            if (!dataRowsNote.Any())
            {
                return;
            }

            foreach (var drNote in dataRowsNote)
            {
                var notaCod = drNote.Field<int>("Nota_Cod");
                var notaIntervento = new NoteIntervento(notaCod);
                
                if (dataRowsNoteIntervento is not null && dataRowsNoteIntervento.Any())
                {
                    var drNotaIntervento = dataRowsNoteIntervento.FirstOrDefault(dr => dr.Field<int>("Nota_Cod") == notaCod);
                    if (drNotaIntervento is not null)
                    {
                        var notaGruppoCod = drNotaIntervento.Field<int>("NotaGruppo_Cod");
                        notaIntervento.noteInterventoGruppi = new NoteInterventoGruppi(notaGruppoCod);
                    }
                }

                attivita.noteIntervento.Add(notaIntervento);
            }
        }
        private void CreaAttivitaPersonalizzata(Attivita attivita, int idAttivita, int lavCod)
        {
            if ((lavCod == LAV_COD.LAVCOD_ALTRE_OPERAZIONI || lavCod == LAV_COD.LAVCOD_VISITA) && idAttivita > 0)
            {
                attivita.attivitaPersonalizzata = new AttivitaPersonalizzata(idAttivita);
            }
        }

        private void CreaCostiAccessori(Attivita attivita, List<MovimentoAttivita> movimenti, List<DataRow> dtRisorseUmaneRows)
        {
            // udm, qta e prezzo non vengono salvate, si usa il "Salva e vai ai costi" o comunque il CdG
            var causaliCostiAccessori = new List<string>
            {
                CAU_MOV.CAU_IMPUTAZIONE_PARCOMACCHINE,
                CAU_MOV.CAU_IMPUTAZIONE_MANODOPERA,
                CAU_MOV.CAU_IMPUTAZIONE_TECNICO_RESPONSABILE,
                CAU_MOV.CAU_IMPUTAZIONE_TERZISTI
            };

            var movimentiCostiAccessori = movimenti
                .Where(m => causaliCostiAccessori.Contains(m.Cau_Mov.ToString()))
                .ToList();

            foreach (var costoAccessorio in movimentiCostiAccessori)
            {
                foreach (var costoAccessorioDettaglio in costoAccessorio.Movimenti_Dettagli)
                {
                    if (costoAccessorio.Cau_Mov == CAU_MOV.CAU_IMPUTAZIONE_PARCOMACCHINE)
                    {
                        var risorsa = new RisorsaMacchina
                        {
                            macchina = new ParcoMacchine { codice = costoAccessorioDettaglio.Mat_Cod }
                        };
                        attivita.risorse.Add(risorsa);
                    }
                    else if (costoAccessorio.Cau_Mov == CAU_MOV.CAU_IMPUTAZIONE_MANODOPERA ||
                             costoAccessorio.Cau_Mov == CAU_MOV.CAU_IMPUTAZIONE_TECNICO_RESPONSABILE ||
                             costoAccessorio.Cau_Mov == CAU_MOV.CAU_IMPUTAZIONE_TERZISTI)
                    {
                        var risorsa = new RisorsaPersona
                        {
                            risorsaUmana = new RisorseUmane { codice = costoAccessorioDettaglio.Mat_Cod }
                        };

                        risorsa.risorsaUmana = BuildRisorsaUmana(risorsa.risorsaUmana.codice, dtRisorseUmaneRows);
                        attivita.risorse.Add(risorsa);
                    }
                }
            }
        }

        private void CreaCostiAccessori(Attivita attivita, List<RicettaDettaglioAttivita> dettagli, List<DataRow> dtRisorseUmaneRows)
        {
            var causaliCostiAccessori = new List<string>
            {
                CAU_MOV.CAU_IMPUTAZIONE_PARCOMACCHINE,
                CAU_MOV.CAU_IMPUTAZIONE_MANODOPERA,
                CAU_MOV.CAU_IMPUTAZIONE_TECNICO_RESPONSABILE,
                CAU_MOV.CAU_IMPUTAZIONE_TERZISTI
            };

            foreach (var dettaglio in dettagli.Where(d => causaliCostiAccessori.Contains(d.Cau_Mov.ToString())).ToList())
            {
                if (dettaglio.Cau_Mov == CAU_MOV.CAU_IMPUTAZIONE_PARCOMACCHINE)
                {
                    var risorsa = new RisorsaMacchina
                    {
                        macchina = new ParcoMacchine { codice = dettaglio.Mat_Cod }
                    };
                    attivita.risorse.Add(risorsa);
                }
                else if (dettaglio.Cau_Mov == CAU_MOV.CAU_IMPUTAZIONE_MANODOPERA ||
                        dettaglio.Cau_Mov == CAU_MOV.CAU_IMPUTAZIONE_TECNICO_RESPONSABILE ||
                        dettaglio.Cau_Mov == CAU_MOV.CAU_IMPUTAZIONE_TERZISTI)
                {
                    var risorsa = new RisorsaPersona
                    {
                        risorsaUmana = new RisorseUmane { codice = dettaglio.Mat_Cod }
                    };

                    risorsa.risorsaUmana = BuildRisorsaUmana(risorsa.risorsaUmana.codice, dtRisorseUmaneRows);
                    attivita.risorse.Add(risorsa);
                }
            }
        }

        private decimal CreaEserciziCdC(Attivita attivita, InfoOperazione infoOperazione, MovimentoAttivita movimentoOperazione)
        {
            decimal superficieTrattataTotale = 0;
            var impiantiDict = new List<int>();

            foreach (var movimentoDettaglio in movimentoOperazione.Movimenti_Dettagli)
            {
                foreach (var movimentoDestinazione in movimentoDettaglio.Movimenti_Destinazioni)
                {
                    if (!impiantiDict.Contains(movimentoDestinazione.Appezza))
                    {
                        impiantiDict.Add(movimentoDestinazione.Appezza);
                        superficieTrattataTotale += movimentoDestinazione.Qta2;

                        EsercizioCDC esercizioCDC = CreaEsercizioCdC(
                            attivita, infoOperazione,
                            movimentoDestinazione.Piva, movimentoDestinazione.Sa_Cod,
                            movimentoDestinazione.Appezza, movimentoDestinazione.Id_Destinazione,
                            movimentoDestinazione.Progetto_Cod, movimentoDestinazione.Qta2,
                            movimentoDestinazione.Sup_Riduzione_BufferZone, movimentoDestinazione.Perc_Riduzione_Deriva);

                        if (esercizioCDC != null)
                            attivita.centriDiCosto.Add(esercizioCDC);
                    }
                }
            }

            return superficieTrattataTotale;
        }

        private decimal CreaProdottoDaTrattareCdC(Attivita attivita, InfoOperazione infoOperazione, MovimentoAttivita movimentoOperazione)
        {
            decimal quantitaTrattataTotale = 0;
            var prodottiDict = new List<string>();

            foreach (var movimentoDettaglio in movimentoOperazione.Movimenti_Dettagli)
            {
                foreach (var movimentoDestinazione in movimentoDettaglio.Movimenti_Destinazioni)
                {
                    if (movimentoDestinazione.Tipo == TIPO_DESTINAZIONE.TIPO_DESTINAZIONE_MAGAZZINO && movimentoDestinazione.Qta == 0)
                    {
                        string chiaveProdotto = $"{movimentoDestinazione.Piva}_{movimentoDestinazione.Sa_Cod}_{movimentoDestinazione.Id_Destinazione}_" +
                                                $"{movimentoDettaglio.Mat_Cod}_{movimentoDettaglio.Cod_Progetto}_{movimentoDettaglio.Lotto}";

                        if (!prodottiDict.Contains(chiaveProdotto))
                        {
                            prodottiDict.Add(chiaveProdotto);
                            quantitaTrattataTotale += movimentoDettaglio.Qta / 100;

                            ProdottoDaTrattareCDC prodottoDaTrattareCdC = CreaProdottoDaTrattareCdC(
                                attivita, infoOperazione, movimentoDettaglio, movimentoDestinazione);

                            attivita.centriDiCosto.Add(prodottoDaTrattareCdC);
                        }
                    }
                }
            }

            return quantitaTrattataTotale;
        }

        private EsercizioCDC CreaEsercizioCdC(
            Attivita attivita, InfoOperazione infoOperazione,
            string Piva, int Sa_Cod, int Appezza, int Id_Destinazione, int Progetto_Cod,
            decimal Qta2, decimal Sup_Riduzione_BufferZone, decimal Perc_Riduzione_Deriva)
        {
            EsercizioCDC esercizioCDC = null;

            if (Piva != "" && Sa_Cod > 0 && Appezza > 0 && Id_Destinazione > 0)
            {
                var appezzamentoPK = new Appezzamento.PK(Appezza, attivita.centroAziendale.primaryKey);
                var impiantoPK = new Impianto.PK(Id_Destinazione, appezzamentoPK);

                var esercizio = new Esercizio(Progetto_Cod, "")
                {
                    impiantoPK = impiantoPK
                };

                esercizioCDC = new EsercizioCDC
                {
                    superficieTrattata = Qta2,
                    esercizio = esercizio
                };

                if (infoOperazione.IsTrattamento || infoOperazione.IsFertilizzazione)
                {
                    esercizioCDC.superficieRiduzioneBufferZone = Sup_Riduzione_BufferZone;
                    esercizioCDC.percentualeRiduzioneDeriva = Perc_Riduzione_Deriva;
                }
            }

            return esercizioCDC;
        }

        private ProdottoDaTrattareCDC CreaProdottoDaTrattareCdC(
            Attivita attivita, InfoOperazione infoOperazione,
            MovDettaglioAttivita movimentoDettaglio, MovimentoDestinazioneAttivita movimentoDestinazione)
        {
            var magazzinoPK = new Fabbricato(
                movimentoDestinazione.Piva, movimentoDestinazione.Sa_Cod,
                movimentoDestinazione.Id_Destinazione, "");

            var prodottoPK = new Prodotto(movimentoDettaglio.Mat_Cod)
            {
                elemCod = movimentoDettaglio.Elem_Cod
            };

            var prodottoDaTrattareCdC = new ProdottoDaTrattareCDC
            {
                giacenzaMagazzino = new MovimentoDiMagazzino
                {
                    Magazzino = magazzinoPK,
                    Prodotto = prodottoPK,
                    codice_progetto = movimentoDettaglio.Cod_Progetto,
                    Lotto = movimentoDettaglio.Lotto,
                    UdM = new UnitaDiMisura(movimentoDettaglio.Udm_Cod)
                },
                qtaTrattata = movimentoDettaglio.Qta / 100
            };

            return prodottoDaTrattareCdC;
        }

        private decimal CreaRisorsaAcqua(Attivita attivita, int lavCod, MovimentoAttivita movimentoOperazione, decimal superficieTrattataTotale)
        {
            decimal acquaTotale = 0;

            if (lavCod == LAV_COD.LAVCOD_CONCIMAZIONE_FOGLIARE ||
                lavCod == LAV_COD.LAVCOD_FERTIRRIGAZIONE ||
                lavCod == LAV_COD.LAVCOD_TRATTAMENTO_ANTIBUTTERATURA ||
                lavCod == LAV_COD.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO ||
                lavCod == LAV_COD.LAVCOD_DISERBO ||
                lavCod == LAV_COD.LAVCOD_DISSECCAMENTO ||
                lavCod == LAV_COD.LAVCOD_GEODISINFESTAZIONE ||
                lavCod == LAV_COD.LAVCOD_CONCIA_SEME ||
                lavCod == LAV_COD.LAVCOD_TRATTAMENTO_FITOREGOLATORE ||
                lavCod == LAV_COD.LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE ||
                lavCod == LAV_COD.LAVCOD_TRATTAMENTO_POST_RACCOLTA)
            {
                var movimentoDettaglioTecnicoAcqua = movimentoOperazione.Movimenti_Dettagli_Tecnici
                    .FirstOrDefault(c => c.Id_Mov_Det == 0 && c.dett_cod == 0);

                if (movimentoDettaglioTecnicoAcqua != null)
                {
                    var risorsaAcqua = new RisorsaAcqua();

                    if (movimentoDettaglioTecnicoAcqua.Qta_Ril > 0)
                    {
                        risorsaAcqua.acqua = movimentoDettaglioTecnicoAcqua.Qta_Ril;
                        risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.TOTALE;
                        acquaTotale = risorsaAcqua.acqua;
                    }
                    else
                    {
                        risorsaAcqua.acqua = -movimentoDettaglioTecnicoAcqua.Qta_Ril;
                        risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.HA;
                        acquaTotale = risorsaAcqua.acqua * superficieTrattataTotale;
                    }

                    attivita.risorse.Add(risorsaAcqua);
                }
            }

            return acquaTotale;
        }

        private DettaglioTrattamento CreaDettaglioTrattamento(string pivaPerLetturaProdotti,
            RicettaDettaglioAttivita dettaglioCampagna,
            RicettaDettaglioAttivita dettaglioInnesco,
            RicettaDettaglioTecnicoAttivita tecnico,
            RicettaDettaglioAttivita dettaglioMagazzino,
            RicettaDestinazioneAttivita destinazioneMagazzino,
            RicettaDestinazioneAttivita destinazioneMagazzinoInnesco,
            AgronicaCoreParametriServer objParametri_Server,
            decimal superficieTrattataTotale,
            decimal acquaTotale,
            int lav_cod,
            Attivita attivita,
            int mezzo,
            InfoOperazione infoOperazione,
            Dictionary<string, Tuple<EsercizioCDC, List<RicettaDestinazioneAttivita>>> listaImpianti)
        {
            var dettCorrente = new DettaglioTrattamento();

            LeggiRisorsaProdotto(pivaPerLetturaProdotti, dettCorrente, dettaglioCampagna,
                dettaglioMagazzino, destinazioneMagazzino, superficieTrattataTotale, acquaTotale, attivita);

            if (tecnico != null)
            {
                dettCorrente.avversitaGruppo = HelperAgenda.GetAvversitaGruppo(tecnico.Av_Cod.Value, tecnico.Av_Gru.Value);

                if (dettCorrente.avversitaGruppo != null)
                {
                    dettCorrente.avversitaGruppo.MagazziniMovimentazioni = new List<RilevamentoDiMagazzino>();

                    if (dettaglioInnesco != null && destinazioneMagazzinoInnesco != null)
                    {
                        dettCorrente.avversitaGruppo.MagazziniMovimentazioni = LeggiRilevamentiMagazzino(
                            dettaglioInnesco.Pro_Cod, dettaglioInnesco,
                            destinazioneMagazzinoInnesco, superficieTrattataTotale, acquaTotale, attivita);
                    }
                }

                dettCorrente.tipoFormulato = HelperAgenda.GetTipoFormulato(dettCorrente.avversitaGruppo, lav_cod);
                dettCorrente.soglia = HelperAgenda.GetSoglia(tecnico.Soglia_Cod.Value, (decimal)tecnico.Soglia_Quantita.Value);
            }

            // Dati che non arrivano da APP
            dettCorrente.dosiEtichetta = HelperAgenda.DosiEtichetta_from_Stringhe(
                dettaglioCampagna.DoseEtichetta, dettaglioCampagna.DoseEtichetta_Value, objParametri_Server);
            dettCorrente.principiAttivi = HelperAgenda.GetPrincipiAttivi(
                dettaglioCampagna.PrincipiAttivi, dettaglioCampagna.PrincipiAttiviPesi, dettaglioCampagna.PrincipiAttiviPercAbb);
            dettCorrente.bufferzone = HelperAgenda.GetBufferZone(dettaglioCampagna.Buffer);
            dettCorrente.dettaglioProdotto = HelperAgenda.GetDettaglioProdotto(dettaglioCampagna.Extra_Str);
            dettCorrente.tempoCarenza = dettaglioCampagna.TempoCarenza ?? 0;
            dettCorrente.polverulento = dettaglioCampagna.Polverulento is null 
                ? Tipo_Polverulento.NonPolverulento 
                : (Tipo_Polverulento)dettaglioCampagna.Polverulento;

            //DettaglioDaLeggere.ripartizioneTrappole = null;

            if (lav_cod == LAV_COD.LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA || lav_cod == LAV_COD.LAVCOD_REINNESCO_TRAPPOLE)
            {
                dettCorrente.quantitaSuImpianti = GetQuantitaSuImpiantiDettaglioTrattamento(
                    dettCorrente, dettaglioCampagna, listaImpianti);

                dettCorrente.ripartizioneTrappole = (enum_Ripartizione_Trappole)mezzo;
            }

            return dettCorrente;
        }

        private List<QuantitaSuImpianto> GetQuantitaSuImpiantiDettaglioTrattamento(
            DettaglioTrattamento dettaglioTrattamento,
            RicettaDettaglioAttivita dettaglioCampagna,
            Dictionary<string, Tuple<EsercizioCDC, List<RicettaDestinazioneAttivita>>> listaImpianti)
        {
            var quantitaSuImpianti = new List<QuantitaSuImpianto>();

            foreach (var imp in listaImpianti.Values)
            {
                List<RicettaDestinazioneAttivita> ricetta_destinazioni = imp.Item2.FindAll(destinazione =>
                    destinazione.Ricetta_SuperUser == dettaglioCampagna.Ricetta_SuperUser &&
                    destinazione.Ricetta_Cod == dettaglioCampagna.Ricetta_Cod &&
                    destinazione.Ricetta_Operazione_Cod == dettaglioCampagna.Ricetta_Operazione_Cod &&
                    destinazione.Ricetta_Dettaglio_Cod == dettaglioCampagna.Ricetta_Dettaglio_Cod);

                if (ricetta_destinazioni != null && ricetta_destinazioni.Count > 0)
                {
                    foreach (var ricetta_destinazione in ricetta_destinazioni)
                    {
                        var quantitaSuImpianto = new QuantitaSuImpianto
                        {
                            esercizioCDC = imp.Item1,
                            Prodotto = dettaglioTrattamento.prodotto,
                            Qta = (decimal)ricetta_destinazione.Qta.Value
                        };

                        if (dettaglioTrattamento.MagazziniMovimentazioni != null &&
                            dettaglioTrattamento.MagazziniMovimentazioni.Count == 1)
                        {
                            quantitaSuImpianto.Lotto = dettaglioTrattamento.MagazziniMovimentazioni[0].Lotto;
                            quantitaSuImpianto.Magazzino = dettaglioTrattamento.MagazziniMovimentazioni[0].Magazzino;
                        }

                        quantitaSuImpianti.Add(quantitaSuImpianto);
                    }
                }
            }

            return quantitaSuImpianti;
        }

        private void LeggiRisorsaProdotto(
            string piva,
            RisorsaProdotto dettaglioDaLeggere,
            RicettaDettaglioAttivita dettaglio,
            RicettaDettaglioAttivita dettaglioMagazzino,
            RicettaDestinazioneAttivita destinazioneMagazzino,
            decimal superficieTrattataTotale,
            decimal acquaTotale,
            Attivita attivita)
        {
            int ProdottoCod;

            if (dettaglio.Mat_Cod == 0)
            {
                ProdottoCod = dettaglio.Pro_Cod;
            }
            else
            {
                ProdottoCod = Math.Abs((int)(dettaglio.Mat_Cod));
            }

            if (dettaglioDaLeggere.prodotto == null)
            {
                dettaglioDaLeggere.prodotto = new Prodotto(ProdottoCod)
                {
                    elemCod = dettaglio.Elem_Cod
                };
            }

            dettaglioDaLeggere.doseHaReale = (decimal)dettaglio.Qta.Value;
            dettaglioDaLeggere.doseHlReale = (decimal)dettaglio.Qta_Extra.Value;
            dettaglioDaLeggere.quantitaTotaleReale = (decimal)dettaglio.Qta_Extra_Totale.Value;
            dettaglioDaLeggere.flagTipoDose = dettaglio.Mezzo_Det.Value;
            // DT: se non è applicabile, viene valorizzata con 10 (qta totale) (es: semina)
            dettaglioDaLeggere.flagDoseQuantitaTotale = dettaglio.Udm_Cod_Extra.Value != 0 ? dettaglio.Udm_Cod_Extra.Value : 10;
            dettaglioDaLeggere.unitaDiMisura = new UnitaDiMisura(dettaglio.Udm_Cod.Value);
            // DT: se non è applicabile l'udm indicata, viene valorizzata come l'udm (es: semina)
            dettaglioDaLeggere.unitaDiMisuraIndicata = dettaglio.Extra_Int != 0
                ? new UnitaDiMisura(dettaglio.Extra_Int.Value)
                : new UnitaDiMisura(dettaglio.Udm_Cod.Value);

            dettaglioDaLeggere.MagazziniMovimentazioni = LeggiRilevamentiMagazzino(
                ProdottoCod, dettaglioMagazzino, destinazioneMagazzino,
                superficieTrattataTotale, acquaTotale, attivita);
        }

        private DettaglioTrattamento CreaDettaglioTrattamento(
            string piva,
            int saCod,
            int idAgenda,
            int lavCod,
            InfoOperazione infoOperazione,
            Attivita attivita,
            MovDettaglioAttivita movimentoDettaglioOperazione,
            MovimentoAttivita movimentoScarico,
            decimal superficieTrattataTotale,
            decimal acquaTotale,
            AgronicaCoreParametriSuperServer objParametri_Super_Server,
            AgronicaCoreParametriServer objParametri_Server,
            AgronicaCoreParametriUtenti objParametri_Utenti,
            MovimentoAttivita movimentoOperazione)
        {
            var dettCorrente = new DettaglioTrattamento();

            // Quando il centro di costo è un prodotto da trattare, non devo creare un DettaglioTrattamento dal current movimentoDettaglioOperazione
            if (infoOperazione.TipoCentroDiCosto == Tipo.ProdottoDaTrattare &&
                (movimentoDettaglioOperazione.Elem_Cod == CATEGORIE_MAGAZZINO.TRASFORMATI_VEGETALI || movimentoDettaglioOperazione.Elem_Cod == CATEGORIE_MAGAZZINO.SEMENTI))
                return null;

            CreaRisorsaProdotto(piva, saCod, idAgenda, attivita, infoOperazione, dettCorrente, movimentoDettaglioOperazione,
                movimentoScarico, superficieTrattataTotale, acquaTotale,
                objParametri_Super_Server, objParametri_Server, objParametri_Utenti);

            if (dettCorrente != null)
            {
                if (infoOperazione.PregressoConMultiAvversita)
                {
                    if (movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici != null &&
                        movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Count == 0)
                    {
                        // Caso di apertura pregresso con N Avversità collegate al movimento e non al dettaglio:
                        // Prendo solo la prima avversità e la collego a tutti i dettagli presenti
                        var movimentoDettaglioTecnico = movimentoOperazione.Movimenti_Dettagli_Tecnici[0];

                        foreach (var dettaglio in movimentoOperazione.Movimenti_Dettagli)
                        {
                            // Aggiungo la chiave del dettaglio a cui lo collego e inserisco il nuovo dettaglio tecnico ora figlio del dettaglio
                            movimentoDettaglioTecnico.Id_Mov_Det = dettaglio.Id_Mov_Det;
                            dettaglio.Movimenti_Dettagli_Tecnici.Add(movimentoDettaglioTecnico);
                        }
                    }
                    else if (movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici != null &&
                             movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Count > 1)
                    {
                        // Caso di apertura pregresso con N Avversità collegate al dettaglio:
                        // Prendo solo la prima avversità
                        var dummy = movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici[0];
                        movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Clear();
                        movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Add(dummy);
                    }
                }

                if (movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici != null &&
                    movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Any())
                {
                    var movimentoDettaglioAvversita = movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici[0];
                    dettCorrente.avversitaGruppo = HelperAgenda.GetAvversitaGruppo(movimentoDettaglioAvversita.Av_Cod, movimentoDettaglioAvversita.Av_Gru);

                    if (dettCorrente.avversitaGruppo != null)
                    {
                        int proCod = movimentoDettaglioAvversita.Av_Cod != 0 ? movimentoDettaglioAvversita.Av_Cod : movimentoDettaglioAvversita.Av_Gru;

                        CreaRilevamentiMagazzino(null, dettCorrente.avversitaGruppo,
                            infoOperazione, movimentoScarico, superficieTrattataTotale, acquaTotale,
                            piva, saCod, idAgenda, proCod, 0, CATEGORIE_MAGAZZINO.INNESCHI,
                            "", 0,
                            objParametri_Super_Server, objParametri_Server, objParametri_Utenti);
                    }

                    dettCorrente.tipoFormulato = HelperAgenda.GetTipoFormulato(dettCorrente.avversitaGruppo, lavCod);
                    dettCorrente.soglia = HelperAgenda.GetSoglia(movimentoDettaglioAvversita.Soglia_Cod, movimentoDettaglioAvversita.Soglia_Quantita);

                    if (infoOperazione.Elem_Cod == CATEGORIE_MAGAZZINO.INSETTI)
                    {
                        if (movimentoDettaglioAvversita.Sigla_av == "IMPOLL")
                            dettCorrente.isImpollinatore = true;
                    }
                }

                dettCorrente.dosiEtichetta = HelperAgenda.DosiEtichetta_from_Stringhe(movimentoDettaglioOperazione.DoseEtichetta, movimentoDettaglioOperazione.DoseEtichetta_Value, objParametri_Server);
                dettCorrente.principiAttivi = HelperAgenda.GetPrincipiAttivi(movimentoDettaglioOperazione.PrincipiAttivi, movimentoDettaglioOperazione.PrincipiAttiviPesi, movimentoDettaglioOperazione.PrincipiAttiviPercAbb);
                dettCorrente.bufferzone = HelperAgenda.GetBufferZone(movimentoDettaglioOperazione.Buffer);
                dettCorrente.dettaglioProdotto = HelperAgenda.GetDettaglioProdotto(movimentoDettaglioOperazione.Extra_Str);
                dettCorrente.tempoCarenza = movimentoDettaglioOperazione.TempoCarenza;
                dettCorrente.polverulento = (Tipo_Polverulento)movimentoDettaglioOperazione.Polverulento;
                //dettCorrente.ripartizioneTrappole = null;

                if (lavCod == LAV_COD.LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA || lavCod == LAV_COD.LAVCOD_REINNESCO_TRAPPOLE)
                {
                    dettCorrente.quantitaSuImpianti = GetQuantitaSuImpiantiDettaglioTrattamento(
                        attivita, infoOperazione, dettCorrente.prodotto, dettCorrente.unitaDiMisuraIndicata,
                        "", null, movimentoDettaglioOperazione);

                    dettCorrente.ripartizioneTrappole = (enum_Ripartizione_Trappole)movimentoOperazione.Mezzo;
                }
            }

            return dettCorrente;
        }

        private DettaglioFertilizzazione LeggiDettaglioFertilizzazione(
            string pivaPerLetturaProdotti,
            RicettaDettaglioAttivita dettaglio,
            RicettaDettaglioTecnicoAttivita tecnico,
            RicettaDettaglioAttivita dettaglioMagazzino,
            RicettaDestinazioneAttivita destinazioneMagazzino,
            decimal superficieTrattataTotale,
            decimal acquaTotale,
            Attivita attivita)
        {
            var dettCorrente = new DettaglioFertilizzazione();

            LeggiRisorsaProdotto(pivaPerLetturaProdotti, dettCorrente, dettaglio,
                dettaglioMagazzino, destinazioneMagazzino, superficieTrattataTotale, acquaTotale, attivita);

            if (tecnico != null)
            {
                dettCorrente.N = (decimal)tecnico.N.Value;
                dettCorrente.P = (decimal)tecnico.P.Value;
                dettCorrente.K = (decimal)tecnico.K.Value;
                dettCorrente.Cu = (decimal)tecnico.Cu.Value;
                dettCorrente.Mg = (decimal)tecnico.Mg.Value;
                dettCorrente.efficienza = (decimal)tecnico.Efficienza.Value;
            }

            return dettCorrente;
        }

        private void LeggiDettaglioSemina(
            string pivaPerLetturaProdotti,
            DettaglioSemina dettaglioDaLeggere,
            RicettaDettaglioAttivita dettaglio,
            RicettaDettaglioTecnicoAttivita tecnico,
            RicettaDettaglioAttivita dettaglioMagazzino,
            RicettaDestinazioneAttivita destinazioneMagazzino,
            decimal superficieTrattataTotale,
            decimal acquaTotale,
            Attivita attivita)
        {
            LeggiRisorsaProdotto(pivaPerLetturaProdotti, dettaglioDaLeggere, dettaglio,
                dettaglioMagazzino, destinazioneMagazzino, superficieTrattataTotale, acquaTotale, attivita);
        }

        private DettaglioRaccolta creaBaseDettaglioRaccolta(
            string pivaRicettaOperazione,
            RicettaDettaglioAttivita dettaglioCorrente,
            RicettaDettaglioTecnicoAttivita dettaglioTecnico,
            RicettaDettaglioAttivita dettaglioMagazzino,
            RicettaDestinazioneAttivita destinazioneMagazzino,
            decimal superficieTrattataTotale,
            decimal acquaTotale,
            Attivita attivita)
        {
            var dettCorrente = new DettaglioRaccolta();

            LeggiDettaglioRaccolta(pivaRicettaOperazione, dettCorrente,
                dettaglioCorrente, dettaglioTecnico,
                dettaglioMagazzino, destinazioneMagazzino,
                superficieTrattataTotale, acquaTotale, attivita);

            dettCorrente.Opzioni_Raccolta = new Opzioni_Raccolta
            {
                Ripartizione = enum_Ripartizione_Raccolta.AUTO_SUPERFICIE,
                GenerazioneLotto = enum_Generazione_Lotto_Raccolta.MANUALE,
                Modalita = enum_Modalita_Raccolta.MECCANICA
            };

            dettCorrente.dataIngresso = dettaglioCorrente.Validita_Inizio.Value;

            if (dettaglioCorrente.Cau_Mov.ToString() == CAU_MOV.CAU_LAVORAZIONE &&
                superficieTrattataTotale > 0 &&
                dettaglioCorrente.Qta_Extra_Totale == 0)
            {
                dettCorrente.doseHaReale = superficieTrattataTotale != 0 ? (decimal)dettaglioCorrente.Qta.Value / superficieTrattataTotale : 0;
                dettCorrente.quantitaTotaleReale = (decimal)dettaglioCorrente.Qta.Value;
            }

            dettCorrente.QuantitaSuImpianti = new List<QuantitaSuImpianto>();

            return dettCorrente;
        }

        private void LeggiDettaglioRaccolta(
            string pivaPerLetturaProdotti,
            DettaglioRaccolta dettaglioDaLeggere,
            RicettaDettaglioAttivita dettaglio,
            RicettaDettaglioTecnicoAttivita tecnico,
            RicettaDettaglioAttivita dettaglioMagazzino,
            RicettaDestinazioneAttivita destinazioneMagazzino,
            decimal superficieTrattataTotale,
            decimal acquaTotale,
            Attivita attivita)
        {
            LeggiRisorsaProdotto(pivaPerLetturaProdotti, dettaglioDaLeggere, dettaglio,
                dettaglioMagazzino, destinazioneMagazzino, superficieTrattataTotale, acquaTotale, attivita);
        }

        private DettaglioFertilizzazione CreaDettaglioFertilizzazione(
            string piva,
            int saCod,
            int idAgenda,
            Attivita attivita,
            InfoOperazione infoOperazione,
            MovDettaglioAttivita movimentoDettaglioOperazione,
            MovimentoAttivita movimentoScarico,
            decimal superficieTrattataTotale,
            decimal acquaTotale,
            AgronicaCoreParametri objParametri_Super_Server,
            AgronicaCoreParametri objParametri_Server,
            AgronicaCoreParametri objParametri_Utenti)
        {
            var dettCorrente = new DettaglioFertilizzazione();

            CreaRisorsaProdotto(piva, saCod, idAgenda, attivita, infoOperazione, dettCorrente, movimentoDettaglioOperazione,
                movimentoScarico, superficieTrattataTotale, acquaTotale,
                objParametri_Super_Server, objParametri_Server, objParametri_Utenti);

            if (dettCorrente != null)
            {
                if (movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici != null &&
                    movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Any())
                {
                    dettCorrente.N = movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici[0].N;
                    dettCorrente.P = movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici[0].P;
                    dettCorrente.K = movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici[0].K;
                    dettCorrente.Cu = movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici[0].Cu;
                    dettCorrente.Mg = movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici[0].M;
                    dettCorrente.efficienza = movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici[0].Efficienza;
                }
            }

            return dettCorrente;
        }

        private DettaglioSemina CreaDettaglioSemina(
            string piva,
            int saCod,
            int idAgenda,
            Attivita attivita,
            InfoOperazione infoOperazione,
            MovDettaglioAttivita movimentoDettaglioOperazione,
            MovimentoAttivita movimentoScarico,
            decimal superficieTrattataTotale,
            decimal acquaTotale,
            AgronicaCoreParametri objParametri_Super_Server,
            AgronicaCoreParametri objParametri_Server,
            AgronicaCoreParametri objParametri_Utenti)
        {
            var dettCorrente = new DettaglioSemina();

            CreaRisorsaProdotto(piva, saCod, idAgenda, attivita, infoOperazione, dettCorrente, movimentoDettaglioOperazione,
                movimentoScarico, superficieTrattataTotale, acquaTotale,
                objParametri_Super_Server, objParametri_Server, objParametri_Utenti);

            return dettCorrente;
        }

        private DettaglioRaccolta CreaDettaglioRaccolta(
            string piva,
            int saCod,
            int idAgenda,
            Attivita attivita,
            InfoOperazione infoOperazione,
            MovDettaglioAttivita movimentoDettaglioOperazione,
            MovimentoAttivita movimentoCarico,
            decimal superficieTrattataTotale,
            decimal acquaTotale,
            AgronicaCoreParametri objParametri_Super_Server,
            AgronicaCoreParametri objParametri_Server,
            AgronicaCoreParametri objParametri_Utenti)
        {
            var dettCorrente = new DettaglioRaccolta();

            CreaRisorsaProdotto(piva, saCod, idAgenda, attivita, infoOperazione, dettCorrente, movimentoDettaglioOperazione,
                movimentoCarico, superficieTrattataTotale, acquaTotale,
                objParametri_Super_Server, objParametri_Server, objParametri_Utenti);

            if (dettCorrente != null)
            {
                dettCorrente.calCod = movimentoDettaglioOperazione.Cal_Cod;
            }

            return dettCorrente;
        }

        private List<QuantitaSuImpianto> GetQuantitaSuImpiantiDettaglioTrattamento(
            Attivita attivita,
            InfoOperazione infoOperazione,
            Prodotto prodotto,
            UnitaDiMisura unitaDiMisuraIndicata,
            string lotto,
            Fabbricato magazzino,
            MovDettaglioAttivita movimentoDettaglioOperazione)
        {
            var quantitaSuImpianti = new List<QuantitaSuImpianto>();

            if (movimentoDettaglioOperazione != null)
            {
                if (movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici != null &&
                    movimentoDettaglioOperazione.Movimenti_Dettagli_Tecnici.Count > 0)
                {
                    var movimenti_destinazioni = movimentoDettaglioOperazione.Movimenti_Destinazioni
                        .FindAll(mov => mov.Id_Mov_Det == movimentoDettaglioOperazione.Id_Mov_Det);

                    if (movimenti_destinazioni != null && movimenti_destinazioni.Count > 0)
                    {
                        foreach (var movimento_destinazione in movimenti_destinazioni)
                        {
                            // La movimento_destinazione.Qta è salvata sempre in kg/litri oppure le altre udm di base
                            // Per questo motivo devo riconvertire la qta con la GetDosePerMagazzino
                            var quantitaSuImpianto = new QuantitaSuImpianto
                            {
                                esercizioCDC = CreaEsercizioCdC(
                                    attivita, infoOperazione,
                                    movimento_destinazione.Piva,
                                    movimento_destinazione.Sa_Cod,
                                    movimento_destinazione.Appezza,
                                    movimento_destinazione.Id_Destinazione,
                                    movimento_destinazione.Progetto_Cod,
                                    movimento_destinazione.Qta2,
                                    movimento_destinazione.Sup_Riduzione_BufferZone,
                                    movimento_destinazione.Perc_Riduzione_Deriva),
                                Prodotto = prodotto,
                                Lotto = lotto,
                                Magazzino = magazzino,
                                Qta = HelperAgenda.GetDosePerMagazzino(movimento_destinazione.Qta, unitaDiMisuraIndicata.codice)
                            };

                            quantitaSuImpianti.Add(quantitaSuImpianto);
                        }
                    }
                }
            }

            return quantitaSuImpianti;
        }

        private void CreaRisorsaProdotto(
            string piva,
            int saCod,
            int idAgenda,
            Attivita attivita,
            InfoOperazione infoOperazione,
            RisorsaProdotto dettaglio,
            MovDettaglioAttivita movimentoDettaglioOperazione,
            MovimentoAttivita movimentoMagazzino,
            decimal superficieTrattataTotale,
            decimal acquaTotale,
            AgronicaCoreParametri objParametri_Super_Server,
            AgronicaCoreParametri objParametri_Server,
            AgronicaCoreParametri objParametri_Utenti)
        {
            int prodottoCod = HelperAgenda.GetCodiceProdotto(movimentoDettaglioOperazione.Pro_Cod, movimentoDettaglioOperazione.Mat_Cod, infoOperazione);

            dettaglio.prodotto = new Prodotto(prodottoCod, movimentoDettaglioOperazione.Elem_Cod);
            dettaglio.unitaDiMisura = new UnitaDiMisura(movimentoDettaglioOperazione.Udm_Cod);

            if (infoOperazione.IsRaccolta)
            {
                if (movimentoDettaglioOperazione.Movimenti_Destinazioni != null && movimentoDettaglioOperazione.Movimenti_Destinazioni.Any())
                {
                    var quantitaSuImpiantiList = new List<QuantitaSuImpianto>();

                    foreach (var movimentoDettaglioDestinazione in movimentoDettaglioOperazione.Movimenti_Destinazioni)
                    {
                        var quantitaSuImpianto = new QuantitaSuImpianto
                        {
                            Qta = movimentoDettaglioDestinazione.Qta,
                            esercizioCDC = CreaEsercizioCdC(attivita, infoOperazione,
                                movimentoDettaglioDestinazione.Piva, movimentoDettaglioDestinazione.Sa_Cod,
                                movimentoDettaglioDestinazione.Appezza, movimentoDettaglioDestinazione.Id_Destinazione,
                                movimentoDettaglioDestinazione.Progetto_Cod, movimentoDettaglioDestinazione.Qta2,
                                movimentoDettaglioDestinazione.Sup_Riduzione_BufferZone, movimentoDettaglioDestinazione.Perc_Riduzione_Deriva),
                            // I campi Magazzino e Lotto vengono inizializzati assieme ai rilevamenti di magazzino
                            // Intanto metto indefinito come valore default
                            Lotto = ""
                        };

                        quantitaSuImpiantiList.Add(quantitaSuImpianto);
                    }

                    var dettaglioConQtaManuali = (DettaglioRaccolta)dettaglio;
                    dettaglioConQtaManuali.QuantitaSuImpianti = quantitaSuImpiantiList;
                    dettaglioConQtaManuali.calCod = movimentoDettaglioOperazione.Cal_Cod;
                }
            }
            else
            {
                dettaglio.doseHaReale = movimentoDettaglioOperazione.Qta;
                dettaglio.doseHlReale = movimentoDettaglioOperazione.Qta_Extra;
                dettaglio.quantitaTotaleReale = movimentoDettaglioOperazione.Qta_Extra_Totale;
                dettaglio.flagTipoDose = movimentoDettaglioOperazione.Mezzo_Det;
                // DT: se non è applicabile, viene valorizzata con 10 (qta totale) (es: semina)
                dettaglio.flagDoseQuantitaTotale = movimentoDettaglioOperazione.Udm_Cod_Extra != 0
                    ? movimentoDettaglioOperazione.Udm_Cod_Extra
                    : 10;
                // DT: se non è applicabile l'udm indicata, viene valorizzata come l'udm (es: semina)
                dettaglio.unitaDiMisuraIndicata = movimentoDettaglioOperazione.Extra_Int != 0
                    ? new UnitaDiMisura(movimentoDettaglioOperazione.Extra_Int)
                    : new UnitaDiMisura(movimentoDettaglioOperazione.Udm_Cod);

                if (infoOperazione.IsSemina)
                {
                    dettaglio.doseHaReale = superficieTrattataTotale != 0 ? movimentoDettaglioOperazione.Qta / superficieTrattataTotale : 0;
                    dettaglio.quantitaTotaleReale = movimentoDettaglioOperazione.Qta;

                    if (prodottoCod == 0)
                        dettaglio = null;
                }

                if (dettaglio != null &&
                    attivita.job.primaryKey.codice == LAV_COD.LAVCOD_DISTRIBUZIONE_INSETTI.ToString() &&
                    movimentoDettaglioOperazione.Qta_Extra_Totale == 0)
                {
                    dettaglio.quantitaTotaleReale = (int)(movimentoDettaglioOperazione.Qta * superficieTrattataTotale);
                }
            }

            CreaRilevamentiMagazzino(dettaglio, null,
                infoOperazione, movimentoMagazzino, superficieTrattataTotale, acquaTotale,
                piva, saCod, idAgenda,
                movimentoDettaglioOperazione.Pro_Cod, movimentoDettaglioOperazione.Mat_Cod, movimentoDettaglioOperazione.Elem_Cod,
                movimentoDettaglioOperazione.Lotto, movimentoDettaglioOperazione.Qta,
                objParametri_Super_Server, objParametri_Server, objParametri_Utenti);
        }

        /// <summary>
        /// Crea i rilevamenti di magazzino associati ad una operazione di magazzino.
        /// Nel caso dell'operazione INSTALLAZIONE TRAPPOLE CATTURA DI MASSA allora potrei avere dei rilevamenti di magazzino associate all'avversità
        /// </summary>
        private void CreaRilevamentiMagazzino(
            Risorsa dettaglio,
            AvversitaGruppo avversita,
            InfoOperazione infoOperazione,
            MovimentoAttivita movimentoMagazzino,
            decimal superficieTrattataTotale,
            decimal acquaTotale,
            string piva,
            int sa_Cod,
            int id_Agenda,
            int pro_Cod,
            int mat_Cod,
            int elem_Cod,
            string lotto,
            decimal qta,
            AgronicaCoreParametri objParametri_Super_Server,
            AgronicaCoreParametri objParametri_Server,
            AgronicaCoreParametri objParametri_Utenti)
        {
            if (movimentoMagazzino != null && movimentoMagazzino.Movimenti_Dettagli != null && movimentoMagazzino.Movimenti_Dettagli.Any())
            {
                // Tengo conto della quantità totale raccolta per ogni impianto
                var qsI = new List<QuantitaSuImpianto>();

                List<MovDettaglioAttivita> movimentiDettaglioMagazzino;

                if (infoOperazione.IsCarico || infoOperazione.IsScarico)
                {
                    movimentiDettaglioMagazzino = movimentoMagazzino.Movimenti_Dettagli
                        .Where(d => HelperAgenda.Controlla_Se_Stesso_Prodotto(d.Elem_Cod, d.Pro_Cod, d.Mat_Cod, d.Lotto, 0,
                                                                          elem_Cod, pro_Cod, mat_Cod, lotto, 0)
                                 && d.Qta == qta
                                 && d.Lotto == lotto)
                        .ToList();
                }
                else
                {
                    movimentiDettaglioMagazzino = movimentoMagazzino.Movimenti_Dettagli
                        .Where(d => HelperAgenda.Controlla_Se_Stesso_Prodotto(d.Elem_Cod, d.Pro_Cod, d.Mat_Cod, d.Lotto, 0,
                                                                          elem_Cod, pro_Cod, mat_Cod, lotto, 0))
                        .ToList();
                }

                var list_Magazzini_Scarico = new List<Tuple<Fabbricato, decimal, List<Attivita>>>();

                foreach (var movimentoDettaglioMagazzino in movimentiDettaglioMagazzino)
                {
                    if (movimentoDettaglioMagazzino != null && movimentoDettaglioMagazzino.Movimenti_Destinazioni.Any())
                    {
                        var xDestinazione = movimentoDettaglioMagazzino.Movimenti_Destinazioni[0];

                        decimal qtaTrasformata = 0;

                        dynamic dettaglioRisorsa = null;

                        if (dettaglio != null && avversita == null)
                        {
                            if (dettaglio.GetType() == typeof(RisorsaRegistrazione))
                            {
                                dettaglioRisorsa = (RisorsaRegistrazione)dettaglio;
                            }
                            else
                            {
                                dettaglioRisorsa = (RisorsaProdotto)dettaglio;
                                if (infoOperazione.IsRaccolta)
                                {
                                    qtaTrasformata = movimentoDettaglioMagazzino.Qta;
                                    if (qsI.Count == 0)
                                    {
                                        qsI = dettaglioRisorsa.QuantitaSuImpianti;
                                        dettaglioRisorsa.QuantitaSuImpianti = new List<QuantitaSuImpianto>();
                                    }
                                }
                                else
                                {
                                    if (infoOperazione.IsSemina)
                                        qtaTrasformata = movimentoDettaglioMagazzino.Qta;
                                    else
                                        qtaTrasformata = HelperAgenda.GetDosePerMagazzino(movimentoDettaglioMagazzino.Qta, dettaglioRisorsa.unitaDiMisuraIndicata.codice);
                                }
                            }
                        }

                        if (avversita != null && dettaglio == null)
                        {
                            int udm_Cod_Indicata = 0;

                            if (movimentoDettaglioMagazzino.Extra_Int == 0 && movimentoDettaglioMagazzino.Udm_Cod > 0)
                                udm_Cod_Indicata = movimentoDettaglioMagazzino.Udm_Cod;
                            else if (movimentoDettaglioMagazzino.Extra_Int > 0)
                                udm_Cod_Indicata = movimentoDettaglioMagazzino.Extra_Int;

                            qtaTrasformata = HelperAgenda.GetDosePerMagazzino(movimentoDettaglioMagazzino.Qta, udm_Cod_Indicata);
                        }

                        if (xDestinazione.Id_Destinazione != 0)
                        {
                            int prodottoCod = HelperAgenda.GetCodiceProdotto(pro_Cod, mat_Cod, infoOperazione);

                            var rilevamentoMagazzino = new RilevamentoDiMagazzino
                            {
                                Lotto = movimentoDettaglioMagazzino.Lotto,
                                Magazzino = CreaFabbricato(xDestinazione.Piva, xDestinazione.Sa_Cod, xDestinazione.Id_Destinazione),
                                udm = new UnitaDiMisura(movimentoDettaglioMagazzino.Udm_Cod),
                                Qta = movimentoDettaglioMagazzino.Qta,
                                Prodotto = new Prodotto(prodottoCod, movimentoDettaglioMagazzino.Elem_Cod)
                                {
                                    unitaDiMisura = new UnitaDiMisura(movimentoDettaglioMagazzino.Udm_Cod)
                                },
                                TipoRilevamento = RilevamentoDiMagazzino.RilevamentoMagazzinoTipo.giacenza,
                                doseHaIndicata = superficieTrattataTotale != 0 ? qtaTrasformata / superficieTrattataTotale : 0,
                                doseHlIndicata = acquaTotale != 0 ? qtaTrasformata / acquaTotale : 0,
                                Cal_Cod = movimentoDettaglioMagazzino.Cal_Cod,
                                Cod_Progetto = movimentoDettaglioMagazzino.Cod_Progetto
                            };

                            rilevamentoMagazzino.registrazioniCollegate = new List<Attivita>();

                            if (dettaglioRisorsa != null && avversita == null)
                            {
                                if (dettaglioRisorsa.MagazziniMovimentazioni == null)
                                    dettaglioRisorsa.MagazziniMovimentazioni = new List<RilevamentoDiMagazzino>();

                                if (infoOperazione.IsRaccolta)
                                {
                                    foreach (var q in qsI)
                                    {
                                        var quantitaSuImpianto = q.Clone();
                                        quantitaSuImpianto.Magazzino = rilevamentoMagazzino.Magazzino;
                                        quantitaSuImpianto.Lotto = rilevamentoMagazzino.Lotto;

                                        if (movimentoDettaglioMagazzino.Cod_Progetto == 0)
                                        {
                                            // Aggiungo un record di QuantitaSuImpianto con quantità proporzionata a quella nel Carico di magazzino
                                            quantitaSuImpianto.Qta = qta != 0
                                                ? quantitaSuImpianto.Qta * movimentoDettaglioMagazzino.Qta / qta
                                                : 0;
                                        }
                                        else if (movimentoDettaglioMagazzino.Cod_Progetto == q.esercizioCDC.esercizio.codice)
                                        {
                                            // Aggiungo il rilevamento collegato specificatamente a questo esercizio
                                            quantitaSuImpianto.Qta = movimentoDettaglioMagazzino.Qta;
                                        }
                                        else
                                        {
                                            continue;
                                        }

                                        dettaglioRisorsa.QuantitaSuImpianti.Add(quantitaSuImpianto);
                                    }

                                    if (rilevamentoMagazzino.Cod_Progetto != 0)
                                        dettaglioRisorsa.Opzioni_Raccolta.GenerazioneLotto = enum_Generazione_Lotto_Raccolta.DA_ESERCIZO;
                                }

                                dettaglioRisorsa.MagazziniMovimentazioni.Add(rilevamentoMagazzino);
                            }

                            if (avversita != null && dettaglioRisorsa == null)
                            {
                                if (avversita.MagazziniMovimentazioni == null)
                                    avversita.MagazziniMovimentazioni = new List<RilevamentoDiMagazzino>();

                                avversita.MagazziniMovimentazioni.Add(rilevamentoMagazzino);
                            }
                        }
                    }
                }
            }
        }

        private List<RilevamentoDiMagazzino> LeggiRilevamentiMagazzino(
            int codiceProdotto,
            RicettaDettaglioAttivita dettaglioMagazzino,
            RicettaDestinazioneAttivita destinazioneMagazzino,
            decimal superficieTrattataTotale,
            decimal acquaTotale,
            Attivita attivita)
        {
            List<RilevamentoDiMagazzino> magazziniMovimentazioni = null;

            if (dettaglioMagazzino == null)
            {
                magazziniMovimentazioni = new List<RilevamentoDiMagazzino>();
                return magazziniMovimentazioni;
            }

            decimal qtaTrasformata = HelperAgenda.GetDosePerMagazzino((decimal)dettaglioMagazzino.Qta.Value, dettaglioMagazzino.Udm_Cod.Value);

            if (destinazioneMagazzino.Id_Reg != 0)
            {
                magazziniMovimentazioni = new List<RilevamentoDiMagazzino>();

                var rilevamentoMagazzino = new RilevamentoDiMagazzino
                {
                    Lotto = dettaglioMagazzino.Lotto,
                    Magazzino = new Fabbricato()
                    {
                        primaryKey = new Fabbricato.PK()
                        {
                            centroAziendalePK = new CentroAziendale.PK(destinazioneMagazzino.Sa_Cod, destinazioneMagazzino.Piva),
                            codice = destinazioneMagazzino.Id_Reg,
                        },
                        descrizione = "",
                    },
                    udm = new UnitaDiMisura(dettaglioMagazzino.Udm_Cod.Value) 
                    { 
                        descrizione = "" 
                    },
                    Qta = (decimal)dettaglioMagazzino.Qta.Value,
                    Prodotto = new Prodotto(codiceProdotto)
                    {
                        elemCod = dettaglioMagazzino.Elem_Cod,
                        descrizione = "",
                        unitaDiMisura = new UnitaDiMisura(dettaglioMagazzino.Udm_Cod.Value) { descrizione = "" }
                    },
                    TipoRilevamento = RilevamentoDiMagazzino.RilevamentoMagazzinoTipo.giacenza,
                    Descrizione = "[" + destinazioneMagazzino.Id_Reg.ToString() + "]",
                    doseHaIndicata = superficieTrattataTotale != 0 ? qtaTrasformata / superficieTrattataTotale : 0,
                    doseHlIndicata = acquaTotale != 0 ? qtaTrasformata / acquaTotale : 0
                };

                magazziniMovimentazioni.Add(rilevamentoMagazzino);
            }

            return magazziniMovimentazioni;
        }

        public Fabbricato CreaFabbricato(string piva, int sa_Cod, int id_Destinazione)
        {
            var fabbricato = new Fabbricato
            {
                primaryKey = new Fabbricato.PK
                {
                    centroAziendalePK = CreaCentroAziendale(piva, sa_Cod).primaryKey,
                    codice = id_Destinazione
                }
            };

            return fabbricato;
        }

        public CentroAziendale CreaCentroAziendale(string piva, int sa_Cod)
        {
            var centroAziendale = new CentroAziendale(new CentroAziendale.PK(sa_Cod, piva));
            return centroAziendale;
        }

        public UtilizzoTerreno BuildUtilizzoTerreno(string piva, int sa_Cod, int appezza, int id_Reg, List<DataRow> dtInfoVarietaRows, List<DataRow> dtCodiciDestinazioniUsoRows, AgronicaCoreParametriServer objParametriServer)
        {
            UtilizzoTerreno utilizzoTerreno = null;
            if (!string.IsNullOrEmpty(piva) && sa_Cod > 0 && appezza > 0 && id_Reg > 0)
            {
                var impiantoPk = new Impianto.PK(id_Reg, appezza, sa_Cod, piva);

                var rowInfoVarieta = dtInfoVarietaRows.FirstOrDefault(r => r.Field<string>("Piva") == piva 
                    && r.Field<int>("Sa_Cod") == sa_Cod
                    && r.Field<int>("Appezza") == appezza
                    && r.Field<int>("Id_Reg") == id_Reg);
                
                if (rowInfoVarieta.Field<int>("Cul_Cod") != 0)
                {
                    utilizzoTerreno = new Varieta(
                        (int)rowInfoVarieta["Cul_Cod"],
                        rowInfoVarieta["Cul_Des"].ToString(),
                        new Specie((int)rowInfoVarieta["Veg_Cod"], rowInfoVarieta["Veg_Des"].ToString())
                    );
                }
                else
                {
                    var rowsCodici = dtCodiciDestinazioniUsoRows.Where(r => r.Field<string>("Piva") == piva
                        && r.Field<int>("Sa_Cod") == sa_Cod
                        && r.Field<int>("Appezza") == appezza
                        && r.Field<int>("Id_Reg") == id_Reg)
                        .ToList();

                    if (rowsCodici.Any())
                    {
                        foreach (var row in rowsCodici)
                        {
                            var valCod = row.Field<string?>("Val_Cod");
                            if (valCod is not null)
                            {
                                var idCod = row.Field<int>("Id_Cod");
                                var descrizione = row.Field<string>("Descrizione");
                                if (idCod >= 3000 && idCod <= 3999)
                                {
                                    utilizzoTerreno = new DestinazioneUso(idCod)
                                    {
                                        descrizione = descrizione,
                                    };
                                }
                            }
                        }
                    }
                    else
                    {
                        utilizzoTerreno = new DestinazioneUso(0)
                        {
                            descrizione = _localizer.GetString("TerrenoNudo").Value
                        };
                    }
                }

                switch (utilizzoTerreno.classType)
                {
                    case ClassType.Varieta:
                        var varieta = (Varieta)utilizzoTerreno;
                        varieta.descrizione = "";
                        if (varieta.specie != null) varieta.specie.descrizione = "";
                        break;
                    case ClassType.DestinazioneUso:
                        ((DestinazioneUso)utilizzoTerreno).descrizione = "";
                        break;
                }
            }
            return utilizzoTerreno;
        }

        public async Task<UtilizzoTerreno> BuildUtilizzoTerrenoFromProdottoDaTrattareAsync(string piva, int elem_Cod, int mat_Cod, AgronicaCoreParametriServer parametriServer)
        {
            int veg_cod = 0, cul_cod = 0;
            string veg_des = "", cul_des = "";

            var dtMateriePrime = await _materiePrimeDal.LeggiAsync(piva, elem_Cod, mat_Cod, parametriServer);
            if (dtMateriePrime is not null && dtMateriePrime.Rows.Count > 0)
            {
                var row = dtMateriePrime.Rows[0];
                cul_cod = row.Field<int>("Cul_Cod");
                veg_cod = row.Field<int>("Veg_Cod");
                veg_des = row.Field<string>("Veg_Des");
                cul_des = row.Field<string>("Cul_Des");
            }

            UtilizzoTerreno utilizzoTerreno = null;
            if (cul_cod != 0)
            {
                utilizzoTerreno = new Varieta(cul_cod)
                {
                    descrizione = cul_des,
                    specie = new Specie(veg_cod) { descrizione = veg_des }
                };
            }

            if (utilizzoTerreno != null)
            {
                switch (utilizzoTerreno.classType)
                {
                    case ClassType.Varieta:
                        var v = (Varieta)utilizzoTerreno;
                        v.descrizione = "";
                        if (v.specie != null) v.specie.descrizione = "";
                        break;
                    case ClassType.DestinazioneUso:
                        ((DestinazioneUso)utilizzoTerreno).descrizione = "";
                        break;
                }
            }
            return utilizzoTerreno;
        }

        public RisorseUmane? BuildRisorsaUmana(int codiceRisUm, List<DataRow> dtRisorseUmaneRows)
        {
            var row = dtRisorseUmaneRows.FirstOrDefault(r => r.Field<int>("Cod_RisUm") == codiceRisUm);
            var risorsaUmana = new RisorseUmane(codiceRisUm);

            var codRapporto = row.Field<int>("Cod_Rapporto");
            risorsaUmana.rapportoContabile = new RapportoContabile(codRapporto);

            var cliente = row.Field<short>("Cliente");
            var fornitore = row.Field<short>("Fornitore");
            var dipendente = row.Field<short>("Dipendente");
            var terzista = row.Field<short>("Terzista");
            var legale = row.Field<short>("Legale");
            var agente = row.Field<short>("Agente");
            var consulente = row.Field<short>("Consulente");

            risorsaUmana.rapportoContabile.cliente = cliente == 1;
            risorsaUmana.rapportoContabile.fornitore = fornitore == 1;
            risorsaUmana.rapportoContabile.dipendente = dipendente == 1;
            risorsaUmana.rapportoContabile.terzista = terzista == 1;
            risorsaUmana.rapportoContabile.legale = legale == 1;
            risorsaUmana.rapportoContabile.agente = agente == 1;
            risorsaUmana.rapportoContabile.consulente = consulente == 1;

            var piva = row.Field<string>("Piva");
            var codContatto = row.Field<string>("Cod_Contatto");

            risorsaUmana.validita = new IntervalloTemporale(
                row.Field<DateTime>("Validita_Inizio"),
                row.Field<DateTime>("Validita_Fine"));
            risorsaUmana.settore = row.Field<string>("Settore_Des");
            risorsaUmana.attivita = row.Field<string>("Attivita_Des");
            risorsaUmana.rapportoContabile.descrizione = row.Field<string>("Rapporto_Des");

            risorsaUmana.contatto = new Contatto()
            {
                primaryKey = new Contatto.PK(
                    row.Field<string>("Piva"),
                    row.Field<string>("Cod_Contatto")),
                nome = row.Field<string>("Nome"),
                cognome = row.Field<string>("Cognome"),
                codiceFiscale = row.Field<string>("Codice_Fiscale"),
                ragione_Sociale = row.Field<string>("Rag_Soc") == ""
                    ? row.Field<string>("Cognome") + " " + row.Field<string>("Nome")
                    : row.Field<string>("Rag_Soc"),
                risorseUmane = new List<RisorseUmane>
                {
                    new RisorseUmane(row.Field<int>("Cod_RisUm"))
                    {
                        attivita = row.Field<string>("Attivita_Des"),
                        settore = row.Field<string>("Settore_Des"),
                        validita = new IntervalloTemporale(
                            row.Field<DateTime>("Validita_Inizio"),
                            row.Field<DateTime>("Validita_Fine")),
                        rapportoContabile = new RapportoContabile(row.Field<int>("Cod_Rapporto"))
                        {
                            descrizione = row.Field<string>("Rapporto_Des")
                        }
                    }
                },
                fisico_Giuridico = (int)row.Field<short>("Id_CF")
            };

            return risorsaUmana;
        }

        class AttivitaConInfoBase
        {
            public Attivita Attivita { get; set; }
            public List<MovimentoAttivita> Movimenti { get; set; } = new();
            public InfoOperazione InfoOperazione { get; set; }
            public int IdAgenda { get; set; }
            public int LavCod { get; set; }
            public string Piva { get; set; }
            public int SaCod { get; set; }
            public int IdAttivita { get; set; }

            public AttivitaConInfoBase(Attivita attivita, List<MovimentoAttivita> movimenti, InfoOperazione infoOperazione, int idAgenda, int lavCod, string piva, int saCod, int idAttivita)
            {
                Attivita = attivita;
                Movimenti = movimenti;
                InfoOperazione = infoOperazione;
                IdAgenda = idAgenda;
                LavCod = lavCod;
                Piva = piva;
                SaCod = saCod;
                IdAttivita = idAttivita;
            }
        }

        class MovimentoAttivita
        {
            public int Id_Agenda { get; set; }
            public int Id_Mov { get; set; }
            public string Cau_Mov { get; set; }
            public string Mov_Desc { get; set; }
            public DateTime Data { get; set; }
            public short Mezzo { get; set; }
            public DateTime Ora { get; set; }
            public short Modalita { get; set; }
            public int Modalita_Applicazione { get; set; }
            public DateTime OraFine { get; set; }
            public int Extra_Int { get; set; }
            public List<MovimentoDettaglioTecnicoAttivita> Movimenti_Dettagli_Tecnici { get; set; } = new();
            public List<MovDettaglioAttivita> Movimenti_Dettagli { get; set; } = new();
        }

        class MovimentoDettaglioTecnicoAttivita
        {
            public int Id_Agenda { get; set; }
            public int Id_Mov { get; set; }
            public int Id_Mov_Det { get; set; }
            public decimal Qta_Ril { get; set; }
            public int Av_Cod { get; set; }
            public int Av_Gru { get; set; }
            public decimal N { get; set; }
            public decimal P { get; set; }
            public decimal K { get; set; }
            public decimal M { get; set; }
            public decimal Efficienza { get; set; }
            public decimal Cu { get; set; }
            public int Ditta_cod { get; set; }
            public decimal Dose { get; set; }
            public decimal Freatimetro { get; set; }
            public DateTime Inn1_data { get; set; }
            public string Inn2_data { get; set; }
            public string Sigla_av { get; set; }
            public string ExtraStr { get; set; }
            public int Extra_Int { get; set; }
            public DateTime Extra_Date { get; set; }
            public int dett_cod { get; set; }
            public int Soglia_Cod { get; set; }
            public decimal Soglia_Quantita { get; set; }
            public int Parziale { get; set; }
            public short Nitrati { get; set; }
        }

        class MovDettaglioAttivita
        {
            public string Piva { get; set; }

            public int Id_Agenda { get; set; }
            public int Id_Mov { get; set; }
            public int Id_Mov_Det { get; set; }

            public int Elem_Cod { get; set; }
            public int Pro_Cod { get; set; }
            public int Mat_Cod { get; set; }

            public decimal Qta { get; set; }

            public int Udm_Cod { get; set; }
            public int Extra_Int { get; set; }

            public string Extra_Str { get; set; }
            public string Lotto { get; set; }

            public int Cod_Progetto { get; set; }
            public int Cal_Cod { get; set; }
            public int Udm_Cod_Extra { get; set; }

            public decimal Qta_Extra { get; set; }

            public int TempoCarenza { get; set; }

            public string DoseEtichetta { get; set; }
            public string DoseEtichetta_Value { get; set; }
            public string PrincipiAttivi { get; set; }
            public string PrincipiAttiviPesi { get; set; }
            public string Buffer { get; set; }

            public decimal Qta_Extra_Totale { get; set; }

            public short Mezzo_Det { get; set; }

            public string PrincipiAttiviPercAbb { get; set; }

            public bool IsDettaglioIrrigazione
            {
                get
                {
                    //MAT_COD.MAT_COD_ACQUA_IRRIGAZIONE = -1
                    //CATEGORIE_MAGAZZINO.ALTRE_MATERIE = 200
                    return Elem_Cod == 200 && Mat_Cod == -1;
                }
            }
            public short Polverulento { get; set; }
            public List<MovimentoDettaglioTecnicoAttivita> Movimenti_Dettagli_Tecnici { get; set; } = new();
            public List<MovimentoDestinazioneAttivita> Movimenti_Destinazioni { get; set; } = new();
        }

        public class MovimentoDestinazioneAttivita
        {
            public string Piva { get; set; }

            public int Sa_Cod { get; set; }
            public int Id_Agenda { get; set; }
            public int Id_Mov { get; set; }
            public int Id_Mov_Det { get; set; }

            public int Appezza { get; set; }
            public int Id_Destinazione { get; set; }
            public int Progetto_Cod { get; set; }
            public int Tipo { get; set; }

            public decimal Qta { get; set; }
            public decimal Qta2 { get; set; }

            public decimal Sup_Riduzione_BufferZone { get; set; }
            public decimal Perc_Riduzione_Deriva { get; set; }
        }

        public class RicettaConInfoBase
        {
            public Attivita Attivita { get; set; }
            public RicettaAttivita Ricetta { get; set; }
            public RicettaOperazioneAttivita RicettaOperazione { get; set; }
            public List<RicettaDettaglioAttivita> Dettagli { get; set; }
            public List<RicettaDettaglioTecnicoAttivita> DettagliTecnici { get; set; }
            public List<RicettaDestinazioneAttivita> Destinazioni { get; set; }
            public InfoOperazione InfoOperazione { get; set; }

            public int CodiceAttivitaPersonalizzata { get; set; }
        }

        public class RicettaAttivita
        {
            public int Ricetta_Cod { get; set; }
            public short? Tipo_Ricetta { get; set; }
            public string Origine { get; set; }
            public string Ricetta_Des { get; set; }
            public string Ricetta_Des_Long { get; set; }
            public string Ricetta_Numero { get; set; }
            public string Note { get; set; }
            public DateTime? Validita_Inizio { get; set; }
            public DateTime? Validita_Fine { get; set; }
            public int? Programmazione_Cod { get; set; }
            public string Piva { get; set; }
            public int Sa_Cod { get; set; }
        }

        public class RicettaOperazioneAttivita
        {
            public int Ricetta_Operazione_Cod { get; set; }
            public int Ricetta_Cod { get; set; }
            public string Ricetta_SuperUser { get; set; }
            public string Ricetta_Operazione_Des { get; set; }
            public int Lav_Cod { get; set; }
            public int? Extra_Int { get; set; }
            public DateTime? Validita_Inizio { get; set; }
            public DateTime? Ora { get; set; }
            public DateTime? Validita_Fine { get; set; }
            public string Note { get; set; }
            public int? W_Anagrafica_Stati_Cod { get; set; }
            public int? Raccoglitore_Cod { get; set; }
            public int? Invia_App { get; set; }
            public string APP_Ricetta_Operazione_ID { get; set; }
            public double? Num_Protocollo { get; set; }
            public int? Id_Rcdpi { get; set; }
            public short? Mezzo { get; set; }
        }

        public class RicettaDettaglioAttivita
        {
            public string Ricetta_SuperUser { get; set; }
            public int Ricetta_Cod { get; set; }
            public int Ricetta_Operazione_Cod { get; set; }
            public int Ricetta_Dettaglio_Cod { get; set; }
            public int Elem_Cod { get; set; }
            public int Pro_Cod { get; set; }
            public int Mat_Cod { get; set; }
            public string Lotto { get; set; }
            public string Cau_Mov { get; set; }
            public int? ID_Attivita { get; set; }
            public double? Qta { get; set; }
            public double? Qta_Extra { get; set; }
            public double? Qta_Extra_Totale { get; set; }
            public short? Mezzo_Det { get; set; }
            public int? Udm_Cod { get; set; }
            public int? Udm_Cod_Extra { get; set; }
            public int? Extra_Int { get; set; }
            public string DoseEtichetta { get; set; }
            public string DoseEtichetta_Value { get; set; }
            public string PrincipiAttivi { get; set; }
            public string PrincipiAttiviPesi { get; set; }
            public string PrincipiAttiviPercAbb { get; set; }
            public string Buffer { get; set; }
            public string Extra_Str { get; set; }
            public int? TempoCarenza { get; set; }
            public short? Polverulento { get; set; }
            public DateTime? Validita_Inizio { get; set; }
        }

        public class RicettaDettaglioTecnicoAttivita
        {
            public int Ricetta_Operazione_Cod { get; set; }
            public int Ricetta_Dettaglio_Cod { get; set; }
            public int Ricetta_Tecnico_Cod { get; set; }
            public double? Qta_Ril { get; set; }
            public double? Dose { get; set; }
            public int? Parziale { get; set; }
            public double? Efficienza { get; set; }
            public short? Nitrati { get; set; }
            public DateTime? Inn1_Data { get; set; }
            public DateTime? Inn2_Data { get; set; }
            public double? Freatimetro { get; set; }
            public int? Dett_Cod { get; set; }
            public int? Ditta_Cod { get; set; }
            public int? Av_Cod { get; set; }
            public int? Av_Gru { get; set; }
            public int? Soglia_Cod { get; set; }
            public double? Soglia_Quantita { get; set; }
            public double? N { get; set; }
            public double? P { get; set; }
            public double? K { get; set; }
            public double? Cu { get; set; }
            public double? Mg { get; set; }
        }

        public class RicettaDestinazioneAttivita
        {
            public string Ricetta_SuperUser { get; set; }
            public int Ricetta_Cod { get; set; }
            public int Ricetta_Operazione_Cod { get; set; }
            public int Ricetta_Dettaglio_Cod { get; set; }
            public int Ricetta_Destinazione_Cod { get; set; }
            public double? Tipo_Destinazione { get; set; }
            public double? Qta { get; set; }
            public string Piva { get; set; }
            public int Sa_Cod { get; set; }
            public int Appezza { get; set; }
            public int Id_Reg { get; set; }
            public double Qta2 { get; set; }
            public double? Sup_Riduzione_BufferZone { get; set; }
            public double? Perc_Riduzione_Deriva { get; set; }
            public string MagazzinoEsterno_Cod { get; set; }
            public string MagazzinoEsterno_Des { get; set; }
            public string MagazzinoEsterno_Dettagli { get; set; }
        }

        #endregion

        #region Operazioni Agenda QdC
        private async Task<int> Scrivi(OperazioneAgenda agenda, AgronicaCoreParametriServer objParametriServer, int origin = -1)
        {
            int idAgenda = agenda.Id_Agenda;
            if (idAgenda <= 0)
            {
                idAgenda = await _sequenceDal.NuovoId_TabellaAsync("Agenda", agenda.BaseCode, agenda.TopCode, objParametriServer);
            }
            agenda.Id_Agenda = idAgenda;
            await _agendaDal.CreateAsync(agenda.ToWriteAgenda(), objParametriServer);
            await WriteLogAgenda(agenda.ToWriteAgenda(), objParametriServer, origin, agenda);

            DateTime data = agenda.Data;

            // TODO write agenda log
            // TODO write note if agenda.note > 0
            // TODO write movimenti
            if (agenda.Movimenti.Any())
            {
                foreach (Movimento movimento in agenda.Movimenti)
                {
                    movimento.Id_Agenda = idAgenda;
                    await _movimentiBiz.ScriviMovimentoAsync(movimento, objParametriServer);
                }
            }

            return idAgenda;
        }

        private async Task WriteLogAgenda(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer, int origin = -1, OperazioneAgenda? agenda = null)
        {
            string objData = JsonConvert.SerializeObject(agenda, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Local });
            WriteLogAgenda dtoLogAgenda = new()
            {
                Piva = dtoAgenda.Piva,
                Sa_Cod = dtoAgenda.Sa_Cod,
                Id_Agenda = dtoAgenda.Id_Agenda,
                Lav_Cod = dtoAgenda.Lav_Cod,
                Id_Servizio = 5,
                Des_Lib = dtoAgenda.Des_Lib,
                Data_Ora_Lavorazione = dtoAgenda.Validita_Inizio,
                SuperUser = objParametriServer.PivaSuperUser,
                Utente = objParametriServer.UsernameOperazione,
                Tipo_Operazione = (int)enum_Tipo_Operazione_Agenda.QuadernoDiCampagna,
                Data_Ora_RegistrazioneLog = DateTime.Now,
                Object_Data = objData,
                Origine = origin,
                Raccoglitore_Cod = 0
            };
            await _logOpAgenda.CreateAsync(dtoLogAgenda, objParametriServer);
        }

        /// <summary>
        /// Scritta sulla bas di scriviAgenda
        /// </summary>
        /// <summary>
        /// DS02B-BL FASE 2: dopo aver persistito l'Agenda, mappa e persiste
        /// i Movimenti di rilievo (Movimenti_Dettagli + Tecnici + Destinazioni).
        /// </summary>
        private async Task<int> ScriviAttivita(Attivita attivita, OperazioneAgenda agenda, InfoOperazione info, AgronicaCoreParametriServer objParametriServer)
        {
            int idAgenda = await Scrivi(agenda, objParametriServer);

            // TODO if attivita.risorse > 0 && is QdC

            return idAgenda;
        }

        /// <exception cref="NotImplementedException"></exception>
        private async Task<int> ScriviOperazioneAgenda(Attivita attivita, OperazioneAgenda agenda, InfoOperazione info, AgronicaCoreParametriServer objParametriServer)
        {
            int idAgenda = 0;
            List<Movimento_Dettaglio_Tecnico> techDetails = new();

            //if (info.IsRaccolta && attivita.tipo == Attivita.Tipo_Attivita.QuadernoDiCampagna)
            //{
            //    // PostOperazioe. modifica anagrafiche
            //}

            //if esegui solo verifiche conformita then return idAgenda
            if (attivita.tipo == Attivita.Tipo_Attivita.QuadernoDiCampagna)
            {
                idAgenda = await ScriviAttivita(attivita, agenda, info, objParametriServer);
            }
            else
            {
                throw new NotImplementedException();
            }

            return idAgenda;
        }

        private async Task<int> ScriviAttivitaToAgenda(
            Attivita attivita, OperazioneAgenda agenda, AgronicaCoreParametriServer objParametriServer)
        {
            int idAgenda = 0;

            // TODO gestire operazione esistente
            // TODO gestire parametri aggiuntivi

            InfoOperazione info = _agendaClassInitializer.GetInfoOperazione(agenda.Lav_Cod, Attivita.Tipo_Attivita.QuadernoDiCampagna);

            if (info.IsZoo)
            {
                // TODO chiamare funzione inerente
            } else
            {
                return await ScriviOperazioneAgenda(attivita, agenda, info, objParametriServer);
            }

            return idAgenda;
        }
        #endregion

        #region Scrittura
        public async Task<int> ScriviModificaAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer, string jobj = "")
        {
            try
            {
                int tipoOp;
                bool isNew = false;
                
                if (dtoAgenda.Id_Agenda == 0)
                {
                    dtoAgenda.Id_Agenda = await _sequenceDal.NuovoId_TabellaAsync("agenda", 0, 2000000000, objParametriServer);
                    isNew = true;
                }
                else
                    isNew = !await _agendaDal.ExistAsync(dtoAgenda.Id_Agenda, objParametriServer);

                if (isNew)
                {
                    tipoOp = (int)Base.Constants.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura;
                    await _agendaDal.CreateAsync(dtoAgenda, objParametriServer);
                }
                else
                {
                    tipoOp = (int)Base.Constants.TipiEnumerativi.enum_TipoOperazioneDB.Modifica;
                    await _agendaDal.UpdateAsync(dtoAgenda, objParametriServer);
                }

                var jobjAgenda = 
                    JsonConvert.SerializeObject(dtoAgenda, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Local });
                WriteLogAgenda dtoLogAgenda = new()
                {
                    Piva = dtoAgenda.Piva,
                    Sa_Cod = dtoAgenda.Sa_Cod,
                    Id_Agenda = dtoAgenda.Id_Agenda,
                    Lav_Cod = dtoAgenda.Lav_Cod,
                    Id_Servizio = 5,
                    Des_Lib = dtoAgenda.Des_Lib,
                    Data_Ora_Lavorazione = dtoAgenda.Validita_Inizio,
                    SuperUser = objParametriServer.PivaSuperUser,
                    Utente = objParametriServer.UsernameOperazione,
                    Tipo_Operazione = tipoOp,
                    Data_Ora_RegistrazioneLog = DateTime.Now,
                    Object_Data = jobj == "" ? jobjAgenda : jobj,
                    Origine = -1,
                    Raccoglitore_Cod = 0
                };
                await _logOpAgenda.CreateAsync(dtoLogAgenda, objParametriServer);

                return dtoAgenda.Id_Agenda;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        /// <summary>
        /// Scritta sulla base di ScriviListaAttivitaToRaccoglitore
        /// </summary>
        public async Task ScriviListaAttivitaAgendaAsync(List<(Attivita attivita, OperazioneAgenda agenda)> agendaActivityList, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                await OpenConnectionAsync(objParametriServer);

                // TODO eliminare definitivamente le agende che in un salvataggio multicentro
                // non sono state riconfermate (causa deselezione propri impianti/centri)
                // TODO gestione carico/scarico

                Attivita last = agendaActivityList.Last().attivita;

                // DS02B-BL FASE 1: se è un'operazione di rilievo, esegui lo split
                // prima di iterare sulla lista.
                if (agendaActivityList.Count == 1)
                {
                    InfoOperazione infoCheck = _agendaClassInitializer.GetInfoOperazione(
                        agendaActivityList[0].agenda.Lav_Cod,
                        agendaActivityList[0].attivita.tipo);

                    if (infoCheck.IsRilievo)
                    {

                        var codiciPerCentro = agendaActivityList
                            .Select(x => (Sa_Cod: x.agenda.Sa_Cod, Lav_Cod: x.agenda.Lav_Cod, Id_Agenda: x.agenda.Id_Agenda))
                            .ToList();

                        var (attivitaAgendaList, agendeToDelete) = await SplitRilieviAttivitaAsync(
                                                                                                    agendaActivityList[0].attivita,
                                                                                                    codiciPerCentro,
                                                                                                    objParametriServer);

                        agendaActivityList = attivitaAgendaList;

                        // TODO: eliminare le agende non riconfermate (agendeToDelete)

                        last = agendaActivityList.Last().attivita;


                    }
                }

                foreach (var item in agendaActivityList)
                {
                    bool isLast = item.attivita == last;
                    string currActivityDes = item.attivita.job.descrizione;
                    int idAgenda = await ScriviAttivitaToAgenda(item.attivita, item.agenda, objParametriServer);
                }
            }
            catch (Exception ex)
            {
                CloseTransaction(objParametriServer, true);
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            finally
            {
                CloseConnection(objParametriServer);
            }
        }

        public async Task<int> ScriviAttivitaAgendaAsync((Attivita attivita, OperazioneAgenda agenda) agendaActivityList, AgronicaCoreParametriServer objParametriServer, bool useTransaction = true)
        {
            try
            {
                if (useTransaction) await OpenConnectionAsync(objParametriServer);

                // TODO eliminare definitivamente le agende che in un salvataggio multicentro
                // non sono state riconfermate (causa deselezione propri impianti/centri)
                // TODO gestione carico/scarico

                int idAgenda = await ScriviAttivitaToAgenda(agendaActivityList.attivita, agendaActivityList.agenda, objParametriServer);
                return idAgenda;
            }
            catch (Exception ex)
            {
                if (useTransaction) CloseTransaction(objParametriServer, true);
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
            finally
            {
                if (useTransaction) CloseConnection(objParametriServer);
            }
        }

        public async Task ScriviListaAttivitaAsync(List<Attivita> activityList, AgronicaCoreParametriServer objParametriServer)
        {
            List<(Attivita, OperazioneAgenda)> agendaActivityList = new();

            activityList.ForEach(async attivita =>
            {
                OperazioneAgenda agenda = await _agendaMapper.MapAttivitaToAgenda(attivita, objParametriServer);
                agendaActivityList.Add((attivita, agenda));
            });

            await ScriviListaAttivitaAgendaAsync(agendaActivityList, objParametriServer);
        }

        /// <summary>
        /// DS02B-BL FASE 1: suddivide una singola Attivita rilievo in sotto-attività,
        /// una per ogni combinazione (faseFenologica, centroAziendale).
        /// </summary>
        private async Task<(List<(Attivita,OperazioneAgenda)> attivitaAgendaList, List<int> AgendeToDelete)> SplitRilieviAttivitaAsync(
            Attivita attivita,
            List<(int Sa_Cod, int Lav_Cod, int Id_Agenda)> codiciPerCentro,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (attivita is null) throw new ArgumentNullException(nameof(attivita));

            string piva = attivita.centroAziendale.primaryKey.partitaIva;
            bool rilievoSenzaImpianti = IsRilievoSenzaImpianti(attivita);

            var dettagliRilievo = attivita.risorse
                .Where(r => r.classType == ClassType.DettaglioRilievo)
                .Cast<DettaglioRilievo>()
                .ToList();

            // 1. Chiavi di split univoche "faseFenologica-centroAziendale"
            var hashCentri = new HashSet<string>();
            foreach (var det in dettagliRilievo)
            {
                string ff = det.faseFenologica?.codice.ToString() ?? "0";
                string centroKey = rilievoSenzaImpianti
                    ? attivita.centroAziendale.primaryKey.codice.ToString()
                    : det.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice.ToString();
                hashCentri.Add($"{ff}-{centroKey}");
            }

            // 2. Valida raccoglitore
            var raccoglitoriDistinti = attivita.raccoglitore != 0
                ? new List<int> { attivita.raccoglitore }
                : new List<int>();

            int codiceRaccoglitore = 0;
            if (hashCentri.Count > 1)
            {
                codiceRaccoglitore = raccoglitoriDistinti.Count == 0
                    ? await _sequenceDal.NuovoId_TabellaAsync("raccoglitore", 0, 2000000000, objParametriServer)
                    : raccoglitoriDistinti[0];
            }

            // 3. Crea una sotto-attività e una sotto-agenda per ogni chiave
            var attivitaAgendaList = new List<(Attivita attivita, OperazioneAgenda agenda)>();
            foreach (var hashItem in hashCentri)
            {
                var parts = hashItem.Split('-');
                string ff = parts[0];
                int saCod = int.Parse(parts[1]);

                var cloneAttivita = Newtonsoft.Json.JsonConvert.DeserializeObject<Attivita>(
                    Newtonsoft.Json.JsonConvert.SerializeObject(attivita))!;
                cloneAttivita.codice = "0";
                cloneAttivita.raccoglitore = codiceRaccoglitore;
                cloneAttivita.centroAziendale = new AgronicaCoreModelsSTD.anagrafiche.CentroAziendale(
                    new AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(saCod, piva));
                cloneAttivita.risorse = new List<AgronicaCoreModelsSTD.attivita.risorse.Risorsa>();
                cloneAttivita.centriDiCosto = new List<CentroDiCosto>();

                var impiantiHash = new HashSet<string>();
                foreach (var det in dettagliRilievo)
                {
                    int detFF = det.faseFenologica?.codice ?? 0;
                    int detCentro = rilievoSenzaImpianti
                        ? saCod
                        : det.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice;

                    bool ffOk = ff == "0" || detFF == int.Parse(ff);
                    if (!ffOk || detCentro != saCod) continue;

                    cloneAttivita.risorse.Add(det);

                    if (!rilievoSenzaImpianti && det.esercizioCDC != null)
                    {
                        var impiantoPk = $"{det.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice}-{det.esercizioCDC.esercizio.impiantoPK.codice}";
                        if (impiantiHash.Add(impiantoPk))
                            cloneAttivita.centriDiCosto.Add(det.esercizioCDC);
                    }
                }

                var cloneAgenda = await _agendaMapper.MapAttivitaToAgenda(cloneAttivita, objParametriServer);

                attivitaAgendaList.Add((cloneAttivita, cloneAgenda));
            }

            // 4. Riassocia codici pre-esistenti e identifica agende da eliminare
            var confermati = new List<int>();
            foreach (var codice in codiciPerCentro)
            {
                foreach (var (att, agenda) in attivitaAgendaList)
                {
                    if (att.codice != "0") continue;
                    if (att.centroAziendale.primaryKey.codice != codice.Sa_Cod) continue;
                    if (att.job.getCodice() != codice.Lav_Cod) continue;

                    if (att.tipo == Attivita.Tipo_Attivita.QuadernoDiCampagna)
                    {
                        att.codice = codice.Id_Agenda.ToString();
                    }
                    else
                    {
                        throw new NotImplementedException("Split rilievo per tipo non-QdC non supportato.");
                    }

                    confermati.Add(codice.Id_Agenda);
                    break;
                }
            }

            var toDelete = codiciPerCentro
                .Where(c => c.Id_Agenda != 0 && !confermati.Contains(c.Id_Agenda))
                .Select(c => c.Id_Agenda)
                .Distinct()
                .ToList();

            return (attivitaAgendaList, toDelete);
        }

        private static bool IsRilievoSenzaImpianti(Attivita attivita)
        {
            var cdcs = attivita.centriDiCosto.OfType<EsercizioCDC>().ToList();
            if (cdcs.Count != 0) return false;

            var dets = attivita.risorse
                .Where(r => r.classType == ClassType.DettaglioRilievo)
                .Cast<DettaglioRilievo>()
                .ToList();

            if (dets.Count == 0) return false;

            var detsNoPlants = dets.Where(d =>
                d.esercizioCDC == null || (
                    d.esercizioCDC.esercizio.codice == 0 &&
                    d.esercizioCDC.esercizio.impiantoPK.codice == 0 &&
                    d.esercizioCDC.esercizio.impiantoPK.appezza == 0
                )).ToList();

            return dets.Count == detsNoPlants.Count;
        }

        #endregion

        #region Blocco/Sblocco
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
                return await _agendaDal.UpdateAsync(dtoAgenda, objParametriServer);
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
                return await _agendaDal.UpdateAsync(dtoAgenda, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
        
        public async Task<bool> BloccaAttivitaAsync(string Piva, int Sa_Cod, List<int> Operazioni, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0)))
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
                        await _agendaDal.UpdateAsync(dtoAgenda, objParametriServer);
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
                    ts.Dispose();
                }
            }

            return true;
        }

        public async Task<bool> SbloccaAttivitaAsync(string Piva, int Sa_Cod, List<int> Operazioni, AgronicaCoreParametriServer objParametriServer)
        {
            using (TransactionScope ts = new(TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0)))
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
                        await _agendaDal.UpdateAsync(dtoAgenda, objParametriServer);
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
                    ts.Dispose();
                }
            }

            return true;
        }
        #endregion

        #region Eliminazione
        public async Task<bool> EliminaAsync(WriteAgenda dtoAgenda, AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                return await _agendaDal.DeleteAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, dtoAgenda.Sa_Cod ?? 0, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        //    public async Task<bool> EliminaAssociateAsync(WriteAgenda dtoAgenda, AgronicaCoreParametri objP)
        //    {
        //        using (TransactionScope ts = new(TransactionScopeOption.RequiresNew, new TimeSpan(0, 10, 0)))
        //        {
        //            try
        //            {
        //                //mov dett tec extra

        //                //mov dett tec

        //                /*-- MOV DESTINAZIONI --*/
        //                DataTable dtMovDest = await _movDestinazioniDal.ReadAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, 0, 0, 0, objP);
        //                if (dtMovDest.Rows.Count > 0)
        //                {
        //                    var dtosMovDest = dtMovDest.ToDictionaryList().Select(row => new WriteMovDestinazioni()
        //                    {
        //                        Piva = Convert.ToString(row["Piva"]),
        //                        Id_Agenda = (int)row["Id_Agenda"],
        //                        Id_Mov = (int)row["Id_Mov"],
        //                        Id_Mov_Det = (int)row["Id_Mov_Det"],
        //                        Id_Destinazione = (int)row["Id_Destinazione"]
        //                    }).ToList();
        //                    await _movDestinazioniBiz.EliminaAsync(dtosMovDest, objP);
        //                }


        //                /*-- MOVIMENTI DETTAGLI --*/
        //                DataTable dtMovDett = await _movDettagliDal.ReadAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, 0, 0, objP);
        //                if (dtMovDett.Rows.Count > 0)
        //                {
        //                    var dtosMovDett = dtMovDett.ToDictionaryList().Select(row => new WriteMovDettagli()
        //                    {
        //                        Piva = Convert.ToString(row["Piva"]),
        //                        Id_Agenda = (int)row["Id_Agenda"],
        //                        Id_Mov = (int)row["Id_Mov"],
        //                        Id_Mov_Det = (int)row["Id_Mov_Det"]
        //                    }).ToList();
        //                    await _movDettagliBiz.EliminaAsync(dtosMovDett, objP);
        //                }

        //                /*-- MOVIMENTI ZOO --*/
        //                DataTable dtMovZoo = await _movZooDal.ReadAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, 0, objP);
        //                if (dtMovZoo.Rows.Count > 0)
        //                {
        //                    var dtosMovZoo = dtMovZoo.ToDictionaryList().Select(row => new WriteMovimentiZoo()
        //                    {
        //                        Piva = Convert.ToString(row["Piva"]),
        //                        Id_Agenda = (int)row["Id_Agenda"],
        //                        Id_Mov = (int)row["Id_Mov"]
        //                    }).ToList();
        //                    await _movZooBiz.EliminaAsync(dtosMovZoo, objP);
        //                }

        //                /*-- MOVIMENTI --*/
        //                DataTable dtMov = await _movimentiDal.ReadAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, 0, objP);
        //                if(dtMov.Rows.Count > 0)
        //                {
        //                    var dtosMov = dtMov.ToDictionaryList().Select(row => new WriteMovimenti()
        //                    {
        //                        Piva = Convert.ToString(row["Piva"]),
        //                        Id_Agenda = (int)row["Id_Agenda"],
        //                        Id_Mov = (int)row["Id_Mov"]
        //                    }).ToList();
        //                    await _movimentiBiz.EliminaAsync(dtosMov, objP);
        //                }

        //                /*-- AGENDA --*/
        //                await _agendaDal.DeleteAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, dtoAgenda.Sa_Cod ?? 0, objP);

        //                ts.Complete();
        //            }
        //            catch (Exception ex)
        //            {
        //                LogError(ex.Message, objP, ex);
        //                throw;
        //            }
        //            finally
        //            {
        //                ts.Dispose();
        //            }
        //        }

        //    return true;
        //}
        #endregion
    }
}
