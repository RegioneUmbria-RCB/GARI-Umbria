using AgronicaCoreAPI.models;
using AgronicaCoreEntityFrameworkSTD_POCO.Models;
using AgronicaCoreModelloSTD;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.avversita;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static AgronicaCoreDataProviderSTD.CostantiPersonalizzate;
using static AgronicaCoreDataProviderSTD.TipiEnumerativi;
using static AgronicaCoreModelsSTD.attivita.Attivita;
using static AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;
using Attivita = AgronicaCoreModelsSTD.attivita.Attivita;
using Fabbricato = AgronicaCoreModelsSTD.anagrafiche.Fabbricato;
using MagazzinoEsterno = AgronicaCoreAPI.models.MagazzinoEsterno;
using Prodotto = AgronicaCoreModelsSTD.attivita.risorse.Prodotto;
using RilevamentoDiMagazzino = AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino;

namespace AgronicaCoreAPI.adapters
{
    public class AttivitaAdapter
    {

        private CoreWSContext db;

        public AttivitaAdapter() {
            this.db = new CoreWSContext(initDati());
        }

        public RicettePerScarico initDati()
        {
            RicettePerScarico dati = new RicettePerScarico();
            dati.Ricette = new List<APP_Ricette>();
            dati.RicetteOperazioni = new List<APP_Ricette_Operazioni>();
            dati.RicetteDettagli = new List<APP_Ricette_Dettagli>();
            dati.RicetteDettaglioTecnico = new List<APP_Ricette_Dettaglio_Tecnico>();
            dati.RicetteDestinazioni = new List<APP_Ricette_Destinazioni>();
            dati.RicetteXNote = new List<APP_RicettexNote>();
            dati.Attivita = new List<APP_CDG_Generale>();
            dati.AttivitaMovimenti = new List<APP_CDG_Movimenti>();
            dati.AttivitaOperazioni = new List<APP_Riferimenti_Interventi_Cdg>();
            return dati;
        }

        // leggo i dati da db interno
        public RicettePerScarico leggiDati()
        {
            return db.leggiDbContext();
        }

        // scrivo i dati su db interno
        public void scriviDati(RicettePerScarico dati, string username)
        {
            db.scriviDati(dati, username);
        }

        public void scriviDatiComuni(RicettePerScarico dati, string username)
        {
            foreach (var ricetteDaScrivere in dati.Ricette)
            {
                CoreWSContext.scriviDatiComuni(ricetteDaScrivere, username);
            }

            foreach (var operazioneDaScrivere in dati.RicetteOperazioni)
            {
                CoreWSContext.scriviDatiComuni(operazioneDaScrivere, username);
            }

            foreach (var dettaglioDaScrivere in dati.RicetteDettagli)
            {
                CoreWSContext.scriviDatiComuni(dettaglioDaScrivere, username);
            }

            foreach (var dettaglioTecnicoDaScrivere in dati.RicetteDettaglioTecnico)
            {
                CoreWSContext.scriviDatiComuni(dettaglioTecnicoDaScrivere, username);
            }

            foreach (var destinazioneDaScrivere in dati.RicetteDestinazioni)
            {
                CoreWSContext.scriviDatiComuni(destinazioneDaScrivere, username);
            }

            foreach (var attivitaDaScrivere in dati.Attivita)
            {
                CoreWSContext.scriviDatiComuni(attivitaDaScrivere, username);
            }

            foreach (var attivitaMovimentoDaScrivere in dati.AttivitaMovimenti)
            {
                CoreWSContext.scriviDatiComuni(attivitaMovimentoDaScrivere, username);
            }

            foreach (var attivitaOperazioneDaScrivere in dati.AttivitaOperazioni)
            {
                CoreWSContext.scriviDatiComuni(attivitaOperazioneDaScrivere, username);
            }

        }

        public bool isZero(double value, double epsilon = 0.000001)
        {
            return Math.Abs(value) < epsilon;
        }

        public List<Attivita> leggiAttivitaMiste(RicettePerScarico dati)
        {
            Attivita attivita = null;
            Attivita attivitaCDG = null;
            this.db = new CoreWSContext(dati);
            List<Attivita> attivitaMiste = new List<Attivita>();
            
            foreach (var lavorazione in dati.RicetteOperazioni)
            {
                attivita = leggiAttivita(lavorazione.Ricetta_Cod, lavorazione.Ricetta_Operazione_Cod);
                attivitaCDG = leggiAttivitaCDG(lavorazione.Ricetta_Cod, lavorazione.Ricetta_Operazione_Cod);

                if (attivitaCDG != null)
                {
                    attivita.job = new JobComposito((Lavorazione)attivita.job, (AttivitaCDG)attivitaCDG.job);
                    attivita.risorse.AddRange(attivitaCDG.risorse);
                }
                
                // posizione
                if (!string.IsNullOrEmpty(dati.posizione))
                {
                    var posizione = dati.posizione.Split("|");
                    attivita.latitude = double.Parse(posizione[0].Replace(".", ","));
                    attivita.longitude = double.Parse(posizione[1].Replace(".", ","));
                }

                attivitaMiste.Add(attivita);

            }

            return attivitaMiste;
        }

        public Attivita leggiAttivita(RicettePerScarico dati)
        {
            Attivita attivita = null;
            this.db = new CoreWSContext(dati);
            var lavorazione = dati.RicetteOperazioni.FirstOrDefault();
            var attivitaCDG = dati.Attivita.FirstOrDefault();

            if (lavorazione != null) {
                attivita = leggiAttivita(lavorazione.Ricetta_Cod, lavorazione.Ricetta_Operazione_Cod);
                if (attivitaCDG != null) {
                    var cdg = leggiAttivitaCDG(attivitaCDG.Id_Cdg_Generale);
                    if (attivita.job is Lavorazione) {
                        attivita.job = new JobComposito((Lavorazione)attivita.job, (AttivitaCDG)cdg.job);
                    }
                    attivita.risorse.AddRange(cdg.risorse);
                }
            } else if (attivitaCDG != null) {
                attivita = leggiAttivitaCDG(attivitaCDG.Id_Cdg_Generale);
            }

            // posizione
            if (attivita!=null && !string.IsNullOrEmpty(dati.posizione)) {
                var posizione = dati.posizione.Split("|");
                attivita.latitude = double.Parse(posizione[0].Replace(".", ","));
                attivita.longitude = double.Parse(posizione[1].Replace(".", ","));
            }

            return attivita;
        }

        public List<Attivita> leggiAttivita(string dati)
        {
            List<Attivita> attivita = new List<Attivita>();
            var ricette = JsonConvert.DeserializeObject<RicettePerScarico>(dati);
            this.db = new CoreWSContext(ricette);

            foreach (var ricetta in ricette.RicetteOperazioni) {
                attivita.Add(leggiAttivita(ricetta.Ricetta_Cod, ricetta.Ricetta_Operazione_Cod));
            }

            return attivita;
        }
        public Attivita leggiAttivitaCDG(int ricetta_cod, int ricetta_operazione_cod)
        {
            APP_Riferimenti_Interventi_Cdg attivitaOperazione = db.leggiAttivitaOperazioni(ricetta_cod, ricetta_operazione_cod).FirstOrDefault();
            if (attivitaOperazione != null) return leggiAttivitaCDG(attivitaOperazione.Id_Cdg_Generale_Rif);
            return null;
        }
            
        public Attivita leggiAttivitaCDG(int id_cdg)
        {
            var attivita = new Attivita();

            APP_CDG_Generale APP_CDG_Generale = db.leggiAttivita(id_cdg).FirstOrDefault();

            if (APP_CDG_Generale == null) return attivita;

            List<APP_CDG_Movimenti> APP_CDG_Movimenti = db.leggiAttivitaMovimenti(id_cdg);
            attivita.inizio = APP_CDG_Generale.Data_Inserimento;
            // attivita.fine = APP_CDG_Generale.Data_Inserimento;
            // attivitaLetta.codice = app_operazioneCorrente.Ricetta_Operazione_Cod.ToString();
            // attivitaLetta.descrizione = app_operazioneCorrente.Ricetta_Operazione_Des;
            attivita.centroAziendale = AnagraficaAdapter.leggiCentroAziendale(APP_CDG_Generale.Piva, 0);
            attivita.tipo = Tipo_Attivita.Ricetta;
            attivita.tipoRicetta = Tipo_Ricetta.Standard_Destinazioni;
            attivita.stato = Stati.Eseguita;
            attivita.note = APP_CDG_Generale.Note;

            List<int> progetti = new();
            List<int> impianti = new();
            List<int> persone = new();
            List<int> macchine = new();

            foreach (var APP_CDG_Movimento in APP_CDG_Movimenti)
            {
                if (attivita.job == null) attivita.job = new AttivitaCDG(APP_CDG_Movimento.Id_Attivita, "");

                if (attivita.centroAziendale != null) {
                    attivita.centroAziendale.primaryKey.codice = APP_CDG_Movimento.sa_cod;
                }

                if (APP_CDG_Movimento.Id_Imputazione != 0)
                {
                    var cdc = new Progetto();
                    cdc.partitaIva = APP_CDG_Movimento.Piva;
                    cdc.codice = new CentroDiCosto.CodeType(APP_CDG_Movimento.Id_Imputazione);
                    if (!progetti.Contains(APP_CDG_Movimento.Id_Imputazione)) {
                        progetti.Add(APP_CDG_Movimento.Id_Imputazione);
                        attivita.centriDiCosto.Add(cdc);
                    }
                }
                else if (APP_CDG_Movimento.progetto_cod != 0)
                {
                    var cdc = new EsercizioCDC();
                    var esercizio = AnagraficaAdapter.leggiEsercizio(APP_CDG_Movimento.Piva, APP_CDG_Movimento.sa_cod, APP_CDG_Movimento.appezza, APP_CDG_Movimento.id_reg);
                    esercizio.codice = APP_CDG_Movimento.progetto_cod;
                    cdc.esercizio = esercizio;
                    if (!impianti.Contains(APP_CDG_Movimento.progetto_cod)) {
                        impianti.Add(APP_CDG_Movimento.progetto_cod);
                        attivita.centriDiCosto.Add(cdc);
                    }
                }

                if (APP_CDG_Movimento.Cod_RisUm != 0)
                {
                    var risorsa = new RisorsaPersona();
                    risorsa.inizio = APP_CDG_Movimento.Data_Ora_Inizio;
                    risorsa.fine = APP_CDG_Movimento.Data_Ora_Fine == AGRODATAFINE ? null : APP_CDG_Movimento.Data_Ora_Fine;
                    risorsa.risorsaUmana = new RisorseUmane();
                    risorsa.risorsaUmana.codice = APP_CDG_Movimento.Cod_RisUm;
                    risorsa.risorsaUmana.contatto = new Contatto();
                    risorsa.risorsaUmana.contatto.badge = APP_CDG_Movimento.NrBadge;
                    if (!persone.Contains(APP_CDG_Movimento.Cod_RisUm))
                    {
                        persone.Add(APP_CDG_Movimento.Cod_RisUm);
                        attivita.risorse.Add(risorsa);
                    }                    
                }
                else if (APP_CDG_Movimento.Mac_Cod != 0)
                {
                    var risorsa = new RisorsaMacchina();
                    risorsa.inizio = APP_CDG_Movimento.Data_Ora_Inizio;
                    risorsa.fine = APP_CDG_Movimento.Data_Ora_Fine == AGRODATAFINE ? null : APP_CDG_Movimento.Data_Ora_Fine;
                    risorsa.macchina = new ParcoMacchine();
                    risorsa.macchina.codice = APP_CDG_Movimento.Mac_Cod;
                    // risorsa.macchina.centroPK = attivita.centroAziendale.primaryKey;
                    if (!macchine.Contains(APP_CDG_Movimento.Mac_Cod))
                    {
                        macchine.Add(APP_CDG_Movimento.Mac_Cod);
                        attivita.risorse.Add(risorsa);
                    }                    
                }
            }
            
            return attivita;
        }

        // legge l'attivita partendo dai dati restituiti dal server (core ws)
        public Attivita leggiAttivita(int ricetta_cod, int ricetta_operazione_cod)
        {
            var attivitaLetta = new Attivita();

            APP_Ricette app_RicettaLetta = db.leggiRicette(ricetta_cod).FirstOrDefault();

            if (app_RicettaLetta == null) return attivitaLetta;

            List<APP_Ricette_Operazioni> app_listaOperazioni = db.leggiOperazioni(ricetta_cod, ricetta_operazione_cod);
            List<APP_Ricette_Dettagli> app_listaDettagli;
            List<APP_Ricette_Dettaglio_Tecnico> app_listaDettaglioTecnico;
            List<APP_Ricette_Destinazioni> app_listaDestinazioni;

            attivitaLetta.tipo = Tipo_Attivita.Ricetta;
            attivitaLetta.tipoRicetta = (Tipo_Ricetta)app_RicettaLetta.Tipo_Ricetta;
            attivitaLetta.note = app_RicettaLetta.note;
            attivitaLetta.centroAziendale = AnagraficaAdapter.leggiCentroAziendale(app_RicettaLetta.piva, app_RicettaLetta.sa_cod);

            string pivaPerLetturaProdotti = app_RicettaLetta.piva;
            
            foreach (var app_operazioneCorrente in app_listaOperazioni)
            {

                attivitaLetta.codice = app_operazioneCorrente.Ricetta_Operazione_Cod.ToString();
                attivitaLetta.descrizione = app_operazioneCorrente.Ricetta_Operazione_Des;
                attivitaLetta.inizio = app_operazioneCorrente.Validita_Inizio;
                attivitaLetta.fine = app_operazioneCorrente.Validita_Fine;
                attivitaLetta.stato = (Stati)app_operazioneCorrente.W_Anagrafica_Stati_Cod;
                attivitaLetta.raccoglitore = app_operazioneCorrente.Raccoglitore_Cod;

                Job job = new Lavorazione(app_operazioneCorrente.Lav_Cod);

                // forzo il job per altre lavorazioni
                if (app_operazioneCorrente.Lav_Cod == LAVCOD_ALTRE_OPERAZIONI && app_operazioneCorrente.Extra_Int != 0) {
                    job = new JobComposito(new Lavorazione(LAVCOD_ALTRE_OPERAZIONI, ""), new AttivitaCDG(app_operazioneCorrente.Extra_Int, ""));
                }

                app_listaDettagli = db.leggiDettagli(ricetta_cod, app_operazioneCorrente.Ricetta_Operazione_Cod);

                Ricette_Dettagli testTipo = AgronicaCoreContabStdBIZ.Ricette.OggettoDaLavCod(app_operazioneCorrente.Lav_Cod).OggettoDettaglioPerTipo;

                // dettaglio tecnico legato ad operazione per rilettura Acqua
                if (testTipo is Ricette_Dettagli_Fertilizzazione || testTipo is Ricette_Dettagli_Trattamento)
                {
                    APP_Ricette_Dettaglio_Tecnico app_dettaglioTecnicoAcqua = db.leggiDettaglioTecnico(ricetta_cod, app_operazioneCorrente.Ricetta_Operazione_Cod, 0).FirstOrDefault();
                    if (app_dettaglioTecnicoAcqua != null)
                    {
                        var risorsaAcqua = new RisorsaAcqua();
                        if (app_dettaglioTecnicoAcqua.Qta_Ril > 0)
                        {
                            risorsaAcqua.acqua = app_dettaglioTecnicoAcqua.Qta_Ril;
                            risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.TOTALE;
                        }
                        else
                        {
                            risorsaAcqua.acqua = -app_dettaglioTecnicoAcqua.Qta_Ril;
                            risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.HA;
                        }
                        attivitaLetta.risorse.Add(risorsaAcqua);
                    }
                }

                // causali da escludere dai dettagli distribuzione
                var causali = new List<string>() {
                    CAU_IMPUTAZIONE_PARCOMACCHINE,
                    CAU_IMPUTAZIONE_MANODOPERA,
                    CAU_IMPUTAZIONE_TECNICO_RESPONSABILE,
                    CAU_IMPUTAZIONE_TERZISTI,
                    CAU_SCARICO,
                    CAU_CARICO };

                List<APP_Ricette_Dettagli> app_listaDettagliDistribuzioni = (from d in app_listaDettagli where !causali.Contains(d.Cau_Mov.ToString()) select d).ToList();

                app_listaDestinazioni = db.leggiDestinazioni(ricetta_cod, app_operazioneCorrente.Ricetta_Operazione_Cod, 0);

                if (string.IsNullOrEmpty(pivaPerLetturaProdotti) && app_listaDestinazioni != null && app_listaDestinazioni.Count > 0)
                {
                    pivaPerLetturaProdotti = app_listaDestinazioni.First().Piva;
                }

                decimal SuperficeTotale = 0;
                var listaImpianti = new List<string>();

                if (app_listaDestinazioni != null && app_listaDestinazioni.Count > 0)
                {
                    foreach (var impiantoDestinazione in app_listaDestinazioni)
                    {

                        string chiave = impiantoDestinazione.Piva + "_" + impiantoDestinazione.Sa_Cod + "_" + impiantoDestinazione.Appezza + "_" + impiantoDestinazione.Id_Reg;

                        if (impiantoDestinazione.Tipo_Destinazione == 0 && !listaImpianti.Contains(chiave))
                        {
                            SuperficeTotale += impiantoDestinazione.Qta2;
                            listaImpianti.Add(chiave);
                        }

                        // la lettura dei dettagli nei rilievi viene fatta diversamente, partendo dalle destinazioni
                        if (testTipo is Ricette_Dettagli_Rilievi)
                        {

                            app_listaDettaglioTecnico = db.leggiDettaglioTecnico(ricetta_cod, app_operazioneCorrente.Ricetta_Operazione_Cod, impiantoDestinazione.Ricetta_Dettaglio_Cod);

                            APP_Ricette_Dettagli app_dettaglioCorrente = (
                                from d in app_listaDettagliDistribuzioni
                                where d.Ricetta_Dettaglio_Cod == impiantoDestinazione.Ricetta_Dettaglio_Cod
                                select d
                            ).FirstOrDefault();

                            APP_Ricette_Dettaglio_Tecnico app_tecnico = app_listaDettaglioTecnico.FirstOrDefault();

                            var cdc = new EsercizioRilievoCDC();
                            leggiEsercizioRilievoCDC(cdc, impiantoDestinazione);
                            // attivitaLetta.centriDiCosto.Add(cdc);

                            var dettaglioCorrente = new DettaglioRilievo();
                            dettaglioCorrente.esercizioCDC = cdc;
                            leggiDettaglioRilievo(dettaglioCorrente, app_dettaglioCorrente, app_tecnico, impiantoDestinazione);
                            if (app_operazioneCorrente.Lav_Cod != LAVCOD_FASI_FENOLOGICHE)
                            {
                                dettaglioCorrente.DataOraRilievo = app_operazioneCorrente.Validita_Inizio;
                            }
                            attivitaLetta.risorse.Add(dettaglioCorrente);
                        }

                    }
                }
                               
                // se non si tratta di un rilievo
                if (!(testTipo is Ricette_Dettagli_Rilievi))
                {

                    foreach (var app_dettaglioCorrente in app_listaDettagliDistribuzioni)
                    {
                        app_listaDettaglioTecnico = db.leggiDettaglioTecnico(ricetta_cod, app_operazioneCorrente.Ricetta_Operazione_Cod, app_dettaglioCorrente.Ricetta_Dettaglio_Cod);

                        APP_Ricette_Dettagli app_dettaglioMagazzino = (
                            from d in app_listaDettagli
                            where d.Cau_Mov.ToString() == CAU_SCARICO
                                    && d.Elem_Cod == app_dettaglioCorrente.Elem_Cod
                                    && d.Pro_Cod == app_dettaglioCorrente.Pro_Cod
                                    && d.Mat_Cod == app_dettaglioCorrente.Mat_Cod
                            select d
                        ).FirstOrDefault();

                        APP_Ricette_Dettagli app_dettaglioMagazzino_Carico = (
                            from d in app_listaDettagli
                            where d.Cau_Mov.ToString() == CAU_CARICO
                                    && d.Elem_Cod == app_dettaglioCorrente.Elem_Cod
                                    && d.Pro_Cod == app_dettaglioCorrente.Pro_Cod
                                    && d.Mat_Cod == app_dettaglioCorrente.Mat_Cod
                            select d
                        ).FirstOrDefault();

                        var app_listaDestinazioniMagazzino = new List<APP_Ricette_Destinazioni>();
                        if (app_dettaglioMagazzino != null)
                        {
                            app_listaDestinazioniMagazzino = (
                                from dd in app_listaDestinazioni
                                where dd.Ricetta_Dettaglio_Cod == app_dettaglioMagazzino.Ricetta_Dettaglio_Cod
                                    && dd.Tipo_Destinazione == 20
                                select dd
                             ).ToList();
                        }
                        else if (app_dettaglioMagazzino_Carico != null)
                        {
                            app_listaDestinazioniMagazzino = (
                                from dd in app_listaDestinazioni
                                where dd.Ricetta_Dettaglio_Cod == app_dettaglioMagazzino_Carico.Ricetta_Dettaglio_Cod
                                    && dd.Tipo_Destinazione == 20
                                select dd
                             ).ToList();
                        }

                        if (testTipo is Ricette_Dettagli_Lavorazioni && app_dettaglioMagazzino_Carico != null && app_operazioneCorrente.Lav_Cod == LAVCOD_RACCOLTA) //gestione raccolte
                        {
                            var dettaglioCorrente = new DettaglioTrattamento();

                            leggiRisorsaProdotto(pivaPerLetturaProdotti, dettaglioCorrente, app_dettaglioCorrente, app_dettaglioMagazzino_Carico, app_listaDestinazioniMagazzino);

                            attivitaLetta.risorse.Add(dettaglioCorrente);
                        }

                        if (testTipo is Ricette_Dettagli_Trattamento)
                        {

                            var dettaglioCorrente = new DettaglioTrattamento();

                            leggiDettaglioTrattamento(
                                pivaPerLetturaProdotti,
                                dettaglioCorrente,
                                app_dettaglioCorrente,
                                app_listaDettaglioTecnico.FirstOrDefault(),
                                app_dettaglioMagazzino,
                                app_listaDestinazioniMagazzino
                             );

                            // forzo qta per semine/trapianti
                            if (app_dettaglioCorrente.Cau_Mov.ToString() == CAU_LAVORAZIONE && SuperficeTotale > 0 && app_dettaglioCorrente.Qta_Extra_Totale == 0)
                            {
                                //dettaglioCorrente.doseHaReale = app_dettaglioCorrente.Qta / SuperficeTotale;
                                //dettaglioCorrente.quantitaTotaleReale = app_dettaglioCorrente.Qta / SuperficeTotale;
                            }

                            attivitaLetta.risorse.Add(dettaglioCorrente);
                        }

                        if (testTipo is Ricette_Dettagli_Fertilizzazione)
                        {

                            var dettaglioCorrente = new DettaglioFertilizzazione();

                            leggiDettaglioFertilizzazione(
                                pivaPerLetturaProdotti,
                                (DettaglioFertilizzazione)dettaglioCorrente,
                                app_dettaglioCorrente,
                                app_listaDettaglioTecnico.FirstOrDefault(),
                                app_dettaglioMagazzino,
                                app_listaDestinazioniMagazzino
                             );

                            attivitaLetta.risorse.Add(dettaglioCorrente);
                        }

                        // filtro per i soli impianti interessati (tipo = 0 )
                        var listaImpiantiInteressati = (from imp in app_listaDestinazioni where imp.Ricetta_Dettaglio_Cod == app_dettaglioCorrente.Ricetta_Dettaglio_Cod && imp.Tipo_Destinazione == 0 select imp).ToList();

                        if (attivitaLetta.centriDiCosto.Count == 0)
                        {
                            foreach (var app_destinazione_corrente in listaImpiantiInteressati)
                            {
                                var cdc = new EsercizioCDC();
                                // cdc.codice = new CentroDiCosto.CodeType(app_destinazione_corrente.Ricetta_Destinazione_Cod);
                                leggiEsercizioCDC(cdc, app_destinazione_corrente);
                                attivitaLetta.centriDiCosto.Add(cdc);
                            }
                        }
                    }
                }

                List<APP_Ricette_Dettagli> app_listaDettagliMacchine = (from d in app_listaDettagli where d.Cau_Mov.ToString() == CAU_IMPUTAZIONE_PARCOMACCHINE select d).ToList();
                foreach (var dettaglio in app_listaDettagliMacchine)
                {
                    var risorsa = new RisorsaMacchina();
                    risorsa.macchina = new ParcoMacchine { codice = dettaglio.Mat_Cod };
                    risorsa.inizio = app_operazioneCorrente.Validita_Inizio;
                    risorsa.fine = app_operazioneCorrente.Validita_Fine;
                    attivitaLetta.risorse.Add(risorsa);
                }

                var causali_operatori = new List<string>() {CAU_IMPUTAZIONE_MANODOPERA, CAU_IMPUTAZIONE_TECNICO_RESPONSABILE, CAU_IMPUTAZIONE_TERZISTI };
                List<APP_Ricette_Dettagli> app_listaDettagliOperatore = (from d in app_listaDettagli where causali_operatori.Contains(d.Cau_Mov.ToString()) select d).ToList();
                foreach (var dettaglio in app_listaDettagliOperatore)
                {
                    var risorsa = new RisorsaPersona();
                    risorsa.risorsaUmana = new RisorseUmane { codice = dettaglio.Mat_Cod };
                    risorsa.inizio = app_operazioneCorrente.Validita_Inizio;
                    risorsa.fine = app_operazioneCorrente.Validita_Fine;
                    attivitaLetta.risorse.Add(risorsa);
                }

                attivitaLetta.job = job;
            }

            return attivitaLetta;

        }

        private void leggiRisorsaProdotto(string piva, RisorsaProdotto DettaglioDaLeggere, APP_Ricette_Dettagli App_Dettagli, APP_Ricette_Dettagli APP_DettaglioMagazzino, List<APP_Ricette_Destinazioni> App_Destinazioni)
        {

            int ProdottoCod = App_Dettagli.Mat_Cod == 0 ? App_Dettagli.Pro_Cod : -App_Dettagli.Mat_Cod;

            DettaglioDaLeggere.prodotto = new Prodotto(ProdottoCod)
            {
                elemCod = App_Dettagli.Elem_Cod
                // tipo = new TipoRisorsa( App_Dettagli.Elem_Cod)
            };

            DettaglioDaLeggere.doseHaReale = App_Dettagli.Qta;
            DettaglioDaLeggere.doseHlReale = App_Dettagli.Qta_Extra;
            DettaglioDaLeggere.quantitaTotaleReale = App_Dettagli.Qta_Extra_Totale;
            
            DettaglioDaLeggere.flagTipoDose = App_Dettagli.Udm_Cod_Extra == 10 ? -1 : App_Dettagli.Mezzo_Det;
            DettaglioDaLeggere.flagDoseQuantitaTotale = App_Dettagli.Udm_Cod_Extra;
            
            // unità di misura
            DettaglioDaLeggere.unitaDiMisura = new AgronicaCoreModelsSTD.metaschema.UnitaDiMisura(App_Dettagli.Udm_Cod);
            DettaglioDaLeggere.unitaDiMisuraIndicata = new AgronicaCoreModelsSTD.metaschema.UnitaDiMisura(App_Dettagli.Extra_Int);

            // parte di carico/scarico di magazzino
            if (App_Destinazioni.Count > 0 && APP_DettaglioMagazzino != null)
            {

                var xDestinazione = App_Destinazioni.FirstOrDefault();

                if (xDestinazione != null && xDestinazione.Id_Reg != 0) {

                    var rilevamentoMagazzino = new RilevamentoDiMagazzino()
                    {
                        Lotto = APP_DettaglioMagazzino.Lotto,
                        Magazzino = AnagraficaAdapter.leggiFabbricato(xDestinazione.Piva, xDestinazione.Sa_Cod, xDestinazione.Id_Reg, ""),
                        udm = new AgronicaCoreModelsSTD.metaschema.UnitaDiMisura(DettaglioDaLeggere.unitaDiMisura.codice)
                        {
                            descrizione = DettaglioDaLeggere.unitaDiMisura.descrizione
                        },
                        Qta = APP_DettaglioMagazzino.Qta,
                        Prodotto = new Prodotto(DettaglioDaLeggere.prodotto.codice)
                        {
                            elemCod = APP_DettaglioMagazzino.Elem_Cod,
                            // tipo = new TipoRisorsa(APP_DettaglioMagazzino.Elem_Cod),
                            descrizione = DettaglioDaLeggere.prodotto.descrizione,
                            unitaDiMisura = new AgronicaCoreModelsSTD.metaschema.UnitaDiMisura(DettaglioDaLeggere.unitaDiMisura.codice)
                            {
                                descrizione = DettaglioDaLeggere.unitaDiMisura.descrizione
                            }
                        },
                        TipoRilevamento = RilevamentoMagazzinoTipo.giacenza,
                        Descrizione = DettaglioDaLeggere.prodotto.descrizione + "[" + xDestinazione.Id_Reg + "]"
                    };

                    // salvo il magazzino agenzia/esterno se presente (1 = Agenzia, 2 = Uso Terzi)
                    if (!string.IsNullOrEmpty(xDestinazione.MagazzinoEsterno_Cod)) {
                        var codici = xDestinazione.MagazzinoEsterno_Cod.Split("-");
                        if (codici.Length < 4 || int.Parse(codici[3]) == (int)enum_MagazzinoEsterno_Tipo.Agenzia) {
                            // magazzino agenzia
                            var magazzinoEsterno = new MagazzinoEsterno() {
                                codice = xDestinazione.MagazzinoEsterno_Cod,
                                descrizione = xDestinazione.MagazzinoEsterno_Des,
                                dettagli = xDestinazione.MagazzinoEsterno_Dettagli
                            };
                            rilevamentoMagazzino.Agenzia = new Fabbricato() {
                                descrizione = JsonConvert.SerializeObject(magazzinoEsterno)
                            };
                        } else {
                            // magazzino uso terzi
                            var descrizione = xDestinazione.MagazzinoEsterno_Des;
                            rilevamentoMagazzino.Agenzia = AnagraficaAdapter.leggiFabbricato(codici[0], int.Parse(codici[1]), int.Parse(codici[2]), descrizione);
                        }
                    }

                    DettaglioDaLeggere.MagazziniMovimentazioni = new List<RilevamentoDiMagazzino>() { rilevamentoMagazzino };

                }
            }
        }

        public void leggiDettaglioTrattamento(string pivaPerLetturaProdotti, DettaglioTrattamento DettaglioDaLeggere, APP_Ricette_Dettagli App_Dettagli, APP_Ricette_Dettaglio_Tecnico app_Tecnico, APP_Ricette_Dettagli APP_Dettaglio_Magazzino, List<APP_Ricette_Destinazioni> APP_Destinazioni)
        {
            leggiRisorsaProdotto(pivaPerLetturaProdotti, DettaglioDaLeggere, App_Dettagli, APP_Dettaglio_Magazzino, APP_Destinazioni);

            if (app_Tecnico != null)
            {
                if (app_Tecnico.Av_Cod != 0)
                {
                    DettaglioDaLeggere.avversitaGruppo = new AgronicaCoreModelsSTD.metaschema.avversita.Avversita(app_Tecnico.Av_Cod);
                }
                else if (app_Tecnico.Av_Gru != 0)
                {
                    DettaglioDaLeggere.avversitaGruppo = new AgronicaCoreModelsSTD.metaschema.avversita.GruppoAvversita(app_Tecnico.Av_Gru);
                }
            }
        }

        private void leggiDettaglioFertilizzazione(string pivaPerLetturaProdotti, DettaglioFertilizzazione DettaglioDaLeggere, APP_Ricette_Dettagli App_Dettagli, APP_Ricette_Dettaglio_Tecnico app_Tecnico, APP_Ricette_Dettagli APP_DettaglioMagazzino, List<APP_Ricette_Destinazioni> App_Destinazioni)
        {
            leggiRisorsaProdotto(pivaPerLetturaProdotti, DettaglioDaLeggere, App_Dettagli, APP_DettaglioMagazzino, App_Destinazioni);

            // parte fertilizzazioni
            if (app_Tecnico != null)
            {
                DettaglioDaLeggere.N = app_Tecnico.N;
                DettaglioDaLeggere.P = app_Tecnico.P;
                DettaglioDaLeggere.K = app_Tecnico.K;
                DettaglioDaLeggere.Cu = app_Tecnico.CU;
            }
        }

        private void leggiDettaglioRilievo(DettaglioRilievo DettaglioDaLeggere, APP_Ricette_Dettagli app_Dettagli, APP_Ricette_Dettaglio_Tecnico app_Tecnico, APP_Ricette_Destinazioni app_Destinazione)
        {
            // Avversità
            if (app_Tecnico.Av_Cod != 0 || app_Tecnico.Av_Gru != 0)
            {
                DettaglioDaLeggere.avversitaGruppo = getAvversitaGruppo(app_Tecnico.Av_Cod, app_Tecnico.Av_Gru);
                DettaglioDaLeggere.unitaDiMisura = new UnitaDiMisura(app_Tecnico.Dett_Cod);
                DettaglioDaLeggere.QtaRilevata = app_Destinazione.Qta;
                DettaglioDaLeggere.QtaRilevataString = app_Destinazione.Qta.ToString();
            }

            // Fase Fenologica (la data è stata specificata)
            if (app_Tecnico.FF_Classe != 0 && app_Destinazione.Validita_Inizio != AGRODATAINIZIO)
            {
                DettaglioDaLeggere.faseFenologica = new FaseFenologica(app_Tecnico.FF_Classe);
                DettaglioDaLeggere.DataOraRilievo = app_Destinazione.Validita_Inizio;
                DettaglioDaLeggere.QtaRilevataString = app_Destinazione.Validita_Inizio.ToShortDateString();
            }

            // Indici Maturità
            if (app_Tecnico.FF_Classe != 0 && app_Destinazione.Validita_Inizio == AGRODATAINIZIO)
            {
                DettaglioDaLeggere.IndiceMaturita = app_Tecnico.FF_Classe;
                DettaglioDaLeggere.unitaDiMisura = new UnitaDiMisura(app_Tecnico.Dett_Cod);
                DettaglioDaLeggere.QtaRilevata = app_Destinazione.Qta;
                DettaglioDaLeggere.QtaRilevataString = app_Destinazione.Qta.ToString();
            }
        }

        private void leggiEsercizioCDC(EsercizioCDC esercizioCDC, APP_Ricette_Destinazioni impiantoDestinazione)
        {
            var impianto = new Reg_Impianti
            {
                piva = impiantoDestinazione.Piva,
                sa_cod = impiantoDestinazione.Sa_Cod,
                appezza = impiantoDestinazione.Appezza,
                id_reg = impiantoDestinazione.Id_Reg
            };
            esercizioCDC.quantita = impiantoDestinazione.Qta;
            esercizioCDC.superficieTrattata = impiantoDestinazione.Qta2;
            esercizioCDC.esercizio = AnagraficaAdapter.leggiEsercizio(impianto);
        }

        private void leggiEsercizioRilievoCDC(EsercizioRilievoCDC cdc, APP_Ricette_Destinazioni impiantoDestinazione)
        {
            leggiEsercizioCDC(cdc, impiantoDestinazione);
            cdc.dataRiferimento = impiantoDestinazione.Validita_Inizio;
        }

        public void scriviAttivita(RicettePerScarico dati, AgronicaCoreModelsSTD.attivita.Attivita attivita, Job job, string user)
        {
            // forzo descrizione attività uguale a quella del job
            if (string.IsNullOrEmpty(attivita.descrizione)) attivita.descrizione = job.descrizione;

            // scrive lavorazione + attività cdg
            if (job.getTipo() == TipiJob.JOB_COMPOSITE)
            {
                var job2 = (JobComposito)job;
                scriviAttivitaCampagna(dati, attivita, job2.lavorazione.getCodice(), user);
                if (attivita.risorse.Count > 0 && dati.AttivitaOperazioni.Count==0)
                {
                    scriviAttivitaCDG(dati, attivita, job2.attivitaCDG.getCodice());
                }

            }
            // scrive attività di campagna
            else if (job.getTipo() == TipiJob.LAVORAZIONE)
            {
                scriviAttivitaCampagna(dati, attivita, job.getCodice(), user);
            }
            // scrive attività cdg
            else if (job.getTipo() == TipiJob.ATTIVITACDG)
            {
                scriviAttivitaCDG(dati, attivita, job.getCodice());
            }

            //Allegati dell'attività
            if (attivita.documenti is not null)
            {
                foreach (var d in attivita.documenti)
                {
                    ScriviAllegati(attivita.centroAziendale.primaryKey.partitaIva, dati, d, dati.RicetteOperazioni != null && dati.RicetteOperazioni.Count > 0 ?  dati.RicetteOperazioni.First().Ricetta_Operazione_Cod : 0, 0);
                }
            }

        }

        public void ScriviAllegati(String piva, RicettePerScarico dati, AgronicaCoreModelsSTD.documenti.Documento documento, int Ricetta_Operazione_Cod, int Ricetta_Destinazione_Cod)
        {
            DocumentoPerScarico documentoPerScarico = new DocumentoPerScarico();
            documentoPerScarico.Documento_Cod = -db.nuovoId("documento");
            documentoPerScarico.Piva = piva;
            documentoPerScarico.Descrizione = documento.Descrizione;
            documentoPerScarico.Note = documento.Note;
            documentoPerScarico.Data_Scadenza = documento.Data_Scadenza;
            if (documentoPerScarico.Data_Scadenza < AGRODATAINIZIO) {
                documentoPerScarico.Data_Scadenza = AGRODATAFINE;
            }
            documentoPerScarico.ID_Tipologia = documento.ID_Tipologia;
            documentoPerScarico.Allegati = new List<DocumentoAllegato>();
            documentoPerScarico.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod;
            documentoPerScarico.Ricetta_Destinazione_Cod = Ricetta_Destinazione_Cod;
            foreach (var allegato in documento.Allegati)
            {
                documentoPerScarico.Allegati.Add(new DocumentoAllegato() { FileName = allegato.FileName, FileByte = allegato.FileByte });
            }
            if (dati.Documenti is null)
            {
                dati.Documenti = new List<DocumentoPerScarico>();
            }
            dati.Documenti.Add(documentoPerScarico);
        }

        // scrive l'attivita campagna nei dati da passare al server
        public void scriviAttivitaCampagna(RicettePerScarico dati, Attivita attivita, int lav_cod, string user)
        {

            APP_Ricette APP_Ricetta = new APP_Ricette();
            APP_Ricetta.ricetta_cod = -db.nuovoId("ricetta");
            APP_Ricetta.Tipo_Ricetta = (int)Tipo_Ricetta.Standard_Destinazioni; // (int)attivita.tipoRicetta
            APP_Ricetta.ricetta_des = user + attivita.codice;
            APP_Ricetta.Ricetta_Numero = user + attivita.codice;
            APP_Ricetta.note = attivita.note;
            APP_Ricetta.Veg_Cod = -1;

            APP_Ricetta.piva = attivita.centroAziendale.primaryKey.partitaIva;
            APP_Ricetta.sa_cod = attivita.centroAziendale.primaryKey.codice;

            var classeAttivita = AgronicaCoreContabStdBIZ.Ricette.ClasseAttivitaDaLavCod(lav_cod);

            // TODO: valutare come gestire più operazioni per ricetta
            APP_Ricette_Operazioni APP_Ricetta_Operazioni = new APP_Ricette_Operazioni();
            APP_Ricetta_Operazioni.Ricetta_Cod = APP_Ricetta.ricetta_cod;
            APP_Ricetta_Operazioni.Ricetta_Operazione_Cod = -db.nuovoId("ricette_operazioni");
            APP_Ricetta_Operazioni.Ricetta_Operazione_Des = attivita.descrizione;
            APP_Ricetta_Operazioni.Lav_Cod = lav_cod;

            // gestione ricette collegate
            int ricetta_operazione_cod = int.Parse(attivita.codice);

            // usata nella vecchia gestione attività pianificate 
            if (ricetta_operazione_cod > 0) {
                APP_Ricetta_Operazioni.Ricetta_Operazione_Cod_RIF = ricetta_operazione_cod;
            //  mantiene collegamento con ricetta originale nel brogliaccio
            } else if (!string.IsNullOrEmpty(attivita.codiceOperazioneRicetta)) {
                APP_Ricetta_Operazioni.Ricetta_Operazione_Cod_RIF = int.Parse(attivita.codiceOperazioneRicetta);
            }
            
            // attivita.stato = Stati.Eseguita;
            if (attivita.stato == Stati.Da_Eseguire) {
                APP_Ricetta_Operazioni.Invia_App = attivita.inviaRicetta ? 1 : 0;
            }
            
            APP_Ricetta_Operazioni.W_Anagrafica_Stati_Cod = (int)attivita.stato;
            APP_Ricetta_Operazioni.Bozza = 0; // OperazioneDaScrivere.Bozza;
            APP_Ricetta_Operazioni.Note = attivita.note;
            APP_Ricetta_Operazioni.Extra_Int = 0; // OperazioneDaScrivere.Epoca;

            // forzo a 1 il campo Mezzo su concimazioni trattamenti/diserbi e semine (Mail Grillo del 26/03/2019)
            if (classeAttivita != enum_Classi_Attivita.LavorazioneBase) {
                APP_Ricetta_Operazioni.Mezzo = (int)enum_TipoMezzo.Ettaro;
            }

            // forzo il campo Extra_Int per la raccolta (10 = fast, 30 = carico magazzino)
            if (lav_cod == LAVCOD_RACCOLTA) APP_Ricetta_Operazioni.Extra_Int = attivita.risorse.Count > 0 ? 30 : 10;
            
            // forzo il campo Extra_Int per altre lavorazioni (ID_Attivita)
            if (lav_cod == LAVCOD_ALTRE_OPERAZIONI && attivita.job.getTipo() == TipiJob.JOB_COMPOSITE) {
                APP_Ricetta_Operazioni.Extra_Int = ((JobComposito)attivita.job).attivitaCDG.getCodice();
            }

            APP_Ricetta_Operazioni.Validita_Inizio = attivita.inizio;
            APP_Ricetta_Operazioni.Validita_Fine = attivita.fine;

            decimal superficeTotale = 0;
            var impiantiDestinazioni = new List<EsercizioCDC>();

            // centri di costo (esercizi, ...)
            foreach (var cdc in attivita.centriDiCosto) {
                if (cdc is EsercizioCDC) {
                    EsercizioCDC esercizioCDC = (EsercizioCDC)cdc;
                    superficeTotale += esercizioCDC.superficieTrattata;
                    impiantiDestinazioni.Add(esercizioCDC);
                    if (string.IsNullOrEmpty(APP_Ricetta.piva)) {
                        var impianto = esercizioCDC.esercizio.impiantoPK;
                        var centro = impianto.appezzamentoPK.centroAziendalePK;
                        APP_Ricetta.piva = centro.partitaIva;
                        APP_Ricetta.sa_cod = centro.codice;
                    }                    
                }
            }

            // gestione risorse (acqua, prodotti, ...)
            foreach (var risorsa in attivita.risorse)
            {
                // fertilizzazioni o trattamenti --> Acqua
                if (risorsa is RisorsaAcqua)
                {

                    RisorsaAcqua risorsaAcqua = (RisorsaAcqua)risorsa;

                    APP_Ricette_Dettaglio_Tecnico APP_DettaglioTecnicoH2O = new APP_Ricette_Dettaglio_Tecnico();
                    APP_DettaglioTecnicoH2O.Ricetta_Cod = APP_Ricetta_Operazioni.Ricetta_Cod;
                    APP_DettaglioTecnicoH2O.Ricetta_Operazione_Cod = APP_Ricetta_Operazioni.Ricetta_Operazione_Cod;
                    APP_DettaglioTecnicoH2O.Ricetta_Dettaglio_Cod = 0;
                    APP_DettaglioTecnicoH2O.Ricetta_Tecnico_Cod = -db.nuovoId("ricette_dettaglio_tecnico");

                    // dati, se H2O Totale (vale 0), allora positiva, Se Acqua Per ha, allora negativa 
                    if (risorsaAcqua.doseAcqua == RisorsaAcqua.TipoDoseAcqua.TOTALE)
                    {
                        APP_DettaglioTecnicoH2O.Qta_Ril = risorsaAcqua.acqua;
                    }
                    else
                    {
                        APP_DettaglioTecnicoH2O.Qta_Ril = -risorsaAcqua.acqua;
                    }

                    // valori predefiniti
                    APP_DettaglioTecnicoH2O.Inn1_data = AGRODATAINIZIO;
                    APP_DettaglioTecnicoH2O.Inn2_data = AGRODATAINIZIO;

                    dati.RicetteDettaglioTecnico.Add(APP_DettaglioTecnicoH2O);

                }
                else if (risorsa is RisorsaProdotto || risorsa is DettaglioRilievo)
                {

                    APP_Ricette_Dettagli APP_Dettaglio = new APP_Ricette_Dettagli();
                    APP_Dettaglio.Ricetta_Cod = APP_Ricetta_Operazioni.Ricetta_Cod;
                    APP_Dettaglio.Ricetta_Operazione_Cod = APP_Ricetta_Operazioni.Ricetta_Operazione_Cod;
                    APP_Dettaglio.Ricetta_Dettaglio_Cod = -db.nuovoId("ricette_dettagli");

                    APP_Ricette_Dettaglio_Tecnico APP_Dettaglio_Tecnico = new APP_Ricette_Dettaglio_Tecnico();

                    // imposta il magazzino, se necessario
                    if (risorsa is RisorsaProdotto)
                    {

                        RisorsaProdotto dettaglioProdPerMagazzino = (RisorsaProdotto)risorsa;

                        var movimentiMagazzino = dettaglioProdPerMagazzino.MagazziniMovimentazioni;
                        if (movimentiMagazzino != null && movimentiMagazzino.Count > 0)
                        {

                            var movimentoMagazzino = movimentiMagazzino.FirstOrDefault();
                            if (movimentoMagazzino != null && movimentoMagazzino.Magazzino != null)
                            {

                                // dettaglio per magazzino
                                APP_Ricette_Dettagli App_Dettaglio_Magazzino = new APP_Ricette_Dettagli();
                                App_Dettaglio_Magazzino.Ricetta_Cod = APP_Ricetta_Operazioni.Ricetta_Cod;
                                App_Dettaglio_Magazzino.Ricetta_Operazione_Cod = APP_Ricetta_Operazioni.Ricetta_Operazione_Cod;
                                App_Dettaglio_Magazzino.Ricetta_Dettaglio_Cod = -db.nuovoId("ricette_dettagli");
                                scriviRisorsaProdotto(dettaglioProdPerMagazzino, App_Dettaglio_Magazzino);
                                App_Dettaglio_Magazzino.Qta = dettaglioProdPerMagazzino.quantitaTotaleReale;
                                App_Dettaglio_Magazzino.Qta_Extra_Totale = 0;
                                App_Dettaglio_Magazzino.Mezzo_Det = 0;
                                App_Dettaglio_Magazzino.Extra_Int = 0;
                                App_Dettaglio_Magazzino.Udm_Cod_Extra = 0;
                                App_Dettaglio_Magazzino.Cau_Mov = lav_cod == LAVCOD_RACCOLTA ? int.Parse(CAU_CARICO) : int.Parse(CAU_SCARICO);
                                dati.RicetteDettagli.Add(App_Dettaglio_Magazzino);

                                // destinazione magazzino
                                var magazzinoPK = movimentoMagazzino.Magazzino.primaryKey;
                                APP_Ricette_Destinazioni appDestinazioneMagazzino = new APP_Ricette_Destinazioni()
                                {
                                    Piva = magazzinoPK.centroAziendalePK != null ? magazzinoPK.centroAziendalePK.partitaIva : APP_Ricetta.piva,
                                    Sa_Cod = magazzinoPK.centroAziendalePK != null ? magazzinoPK.centroAziendalePK.codice : APP_Ricetta.sa_cod,
                                    Tipo_Destinazione = 20,
                                    Id_Reg = magazzinoPK.codice,
                                    Qta = dettaglioProdPerMagazzino.quantitaTotaleReale,
                                    Ricetta_Cod = APP_Ricetta_Operazioni.Ricetta_Cod,
                                    Ricetta_Operazione_Cod = APP_Ricetta_Operazioni.Ricetta_Operazione_Cod,
                                    Ricetta_Destinazione_Cod = -db.nuovoId("ricette_destinazioni"),
                                    Ricetta_Dettaglio_Cod = App_Dettaglio_Magazzino.Ricetta_Dettaglio_Cod
                                };

                                // salvo il magazzino agenzia/esterno se presente
                                if (movimentoMagazzino.Agenzia != null) {
                                    try {
                                        var agenziaPK = movimentoMagazzino.Agenzia.primaryKey;
                                        if (agenziaPK == null || agenziaPK.codice == 0) {
                                            // magazzino agenzia
                                            var magazzinoEsterno = JsonConvert.DeserializeObject<MagazzinoEsterno>(movimentoMagazzino.Agenzia.descrizione);
                                            appDestinazioneMagazzino.MagazzinoEsterno_Cod = magazzinoEsterno.codice + "-" + (int)enum_MagazzinoEsterno_Tipo.Agenzia;
                                            appDestinazioneMagazzino.MagazzinoEsterno_Des = magazzinoEsterno.descrizione;
                                            appDestinazioneMagazzino.MagazzinoEsterno_Dettagli = magazzinoEsterno.dettagli;
                                        } else {
                                            // magazzino esterno
                                            var codice = agenziaPK.centroAziendalePK.partitaIva + "-" + agenziaPK.centroAziendalePK.codice + "-" + agenziaPK.codice;
                                            appDestinazioneMagazzino.MagazzinoEsterno_Cod = codice + "-" + (int)enum_MagazzinoEsterno_Tipo.Uso_da_Terzi;
                                            appDestinazioneMagazzino.MagazzinoEsterno_Des = movimentoMagazzino.Agenzia.descrizione;
                                            appDestinazioneMagazzino.MagazzinoEsterno_Dettagli = "" + dettaglioProdPerMagazzino.quantitaTotaleReale;
                                        }
                                    } catch (Exception ex) { }                                    
                                }
                                dati.RicetteDestinazioni.Add(appDestinazioneMagazzino);
                            }
                        }
                    }
                    
                    // imposta destinazioni
                    foreach (var impiantoCorrente in impiantiDestinazioni)
                    {
                        APP_Ricette_Destinazioni APP_Destinazione = new APP_Ricette_Destinazioni();
                        APP_Destinazione.Ricetta_Cod = APP_Dettaglio.Ricetta_Cod;
                        APP_Destinazione.Ricetta_Operazione_Cod = APP_Dettaglio.Ricetta_Operazione_Cod;
                        APP_Destinazione.Ricetta_Dettaglio_Cod = APP_Dettaglio.Ricetta_Dettaglio_Cod;
                        APP_Destinazione.Ricetta_Destinazione_Cod = -db.nuovoId("ricette_destinazioni");
                        // se si tratta di scarico di prodotti calcolo la quantità distribuita
                        if (risorsa is RisorsaProdotto) {
                            impiantoCorrente.quantita = impiantoCorrente.superficieTrattata * ((RisorsaProdotto)risorsa).doseHaReale;
                        }
                        scriviDestinazione(impiantoCorrente, APP_Destinazione, superficeTotale);
                        if (impiantoCorrente.documento is not null)
                        {
                            ScriviAllegati(attivita.centroAziendale.primaryKey.partitaIva, dati, impiantoCorrente.documento, APP_Ricetta_Operazioni.Ricetta_Operazione_Cod, APP_Destinazione.Ricetta_Destinazione_Cod);
                        }
                        dati.RicetteDestinazioni.Add(APP_Destinazione);
                    }

                    // Rilievi
                    if (risorsa is DettaglioRilievo) {

                        DettaglioRilievo risorsaRilievo = (DettaglioRilievo)risorsa;
                        scriviDettaglioRilievo(risorsaRilievo, APP_Dettaglio, APP_Dettaglio_Tecnico);
                        APP_Dettaglio.Cau_Mov = int.Parse(lav_cod == LAVCOD_RILIEVO_INDICI_MATURITA ? CAU_RILIEVO_RACCOLTA : CAU_RILIEVO_CAMPO);
                        APP_Dettaglio_Tecnico.Ricetta_Tecnico_Cod = -db.nuovoId("ricette_dettaglio_tecnico");
                        
                        // aggiungo destinazione relativa al rilievo
                        if (risorsaRilievo.esercizioCDC != null) {
                            APP_Ricette_Destinazioni APP_Destinazione = new APP_Ricette_Destinazioni();
                            APP_Destinazione.Ricetta_Cod = APP_Dettaglio.Ricetta_Cod;
                            APP_Destinazione.Ricetta_Operazione_Cod = APP_Dettaglio.Ricetta_Operazione_Cod;
                            APP_Destinazione.Ricetta_Dettaglio_Cod = APP_Dettaglio.Ricetta_Dettaglio_Cod;
                            APP_Destinazione.Ricetta_Destinazione_Cod = -db.nuovoId("ricette_destinazioni");
                            scriviDestinazione(risorsaRilievo.esercizioCDC, APP_Destinazione, risorsaRilievo.esercizioCDC.superficieTrattata);
                            // fix per data rilievo fasi fenologiche if (lav_cod == LAVCOD_FASI_FENOLOGICHE) 
                            APP_Destinazione.Validita_Inizio = risorsaRilievo.DataOraRilievo;
                            dati.RicetteDestinazioni.Add(APP_Destinazione);
                        }
                    }

                    bool prodottoValorizzato = false;

                    // Trattamenti
                    if (risorsa is DettaglioTrattamento)
                    {

                        // se operazione di semina/raccolta cambio cau_mov e non salvo dettaglio tecnico
                        if (classeAttivita == enum_Classi_Attivita.SeminaTrapianto || classeAttivita == enum_Classi_Attivita.Raccolta)
                        {
                            APP_Dettaglio.Cau_Mov = int.Parse(CAU_LAVORAZIONE);
                            APP_Dettaglio_Tecnico = null;
                        }
                        else
                        {
                            if (APP_Dettaglio_Tecnico.Ricetta_Tecnico_Cod == 0)
                            {
                                APP_Dettaglio_Tecnico.Ricetta_Tecnico_Cod = -db.nuovoId("ricette_dettaglio_tecnico");
                            }
                            APP_Dettaglio.Cau_Mov = int.Parse(CAU_TRATTAMENTO);
                        }

                        scriviDettaglioTrattamento((DettaglioTrattamento)risorsa, APP_Dettaglio, APP_Dettaglio_Tecnico);

                        // forzo qta per semine/trapianti
                        if (APP_Dettaglio.Cau_Mov.ToString() == CAU_LAVORAZIONE)
                        {
                            //APP_Dettaglio.Mezzo_Det = 1;
                            //APP_Dettaglio.Udm_Cod_Extra = 10;
                            //APP_Dettaglio.Qta = APP_Dettaglio.Qta_Extra_Totale;
                            //APP_Dettaglio.Qta_Extra_Totale = 0;
                        }

                        prodottoValorizzato = true;
                    }

                    // Fertilizzazioni
                    if (risorsa is DettaglioFertilizzazione)
                    {
                        if (APP_Dettaglio_Tecnico.Ricetta_Tecnico_Cod == 0)
                        {
                            APP_Dettaglio_Tecnico.Ricetta_Tecnico_Cod = -db.nuovoId("ricette_dettaglio_tecnico");
                        }
                        scriviDettaglioFertilizzazione((DettaglioFertilizzazione)risorsa, APP_Dettaglio, APP_Dettaglio_Tecnico);
                        APP_Dettaglio.Cau_Mov = int.Parse(CAU_LAVORAZIONE);
                        prodottoValorizzato = true;
                    }

                    // Prodotti
                    if (!prodottoValorizzato && risorsa is RisorsaProdotto)
                    {
                        scriviRisorsaProdotto((RisorsaProdotto)risorsa, APP_Dettaglio);
                    }

                    dati.RicetteDettagli.Add(APP_Dettaglio);

                    if (APP_Dettaglio_Tecnico != null && APP_Dettaglio_Tecnico.Ricetta_Tecnico_Cod != 0)
                    {
                        dati.RicetteDettaglioTecnico.Add(APP_Dettaglio_Tecnico);
                    }
                }
                else if (risorsa is DettaglioIrrigazione)
                {
                    DettaglioIrrigazione dettaglioIrrigazione = (DettaglioIrrigazione)risorsa;

                    APP_Ricette_Dettagli APP_Dettaglio = new APP_Ricette_Dettagli();
                    APP_Dettaglio.Ricetta_Cod = APP_Ricetta_Operazioni.Ricetta_Cod;
                    APP_Dettaglio.Ricetta_Operazione_Cod = APP_Ricetta_Operazioni.Ricetta_Operazione_Cod;
                    APP_Dettaglio.Ricetta_Dettaglio_Cod = -db.nuovoId("ricette_dettagli");
                    APP_Dettaglio.Cau_Mov = int.Parse(CAU_LAVORAZIONE);
                    APP_Dettaglio.Mezzo_Det = -1;
                    APP_Dettaglio.Lotto = "";
                    dati.RicetteDettagli.Add(APP_Dettaglio);

                    APP_Ricette_Dettaglio_Tecnico APP_Dettaglio_Tecnico = new APP_Ricette_Dettaglio_Tecnico();
                    APP_Dettaglio_Tecnico.Ricetta_Tecnico_Cod = -db.nuovoId("ricette_dettaglio_tecnico");
                    APP_Dettaglio_Tecnico.Ricetta_Cod = APP_Dettaglio.Ricetta_Cod;
                    APP_Dettaglio_Tecnico.Ricetta_Operazione_Cod = APP_Dettaglio.Ricetta_Operazione_Cod;
                    APP_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod = APP_Dettaglio.Ricetta_Dettaglio_Cod;
                    APP_Dettaglio_Tecnico.Qta_Ril = dettaglioIrrigazione.QtaRilevata;
                    APP_Dettaglio_Tecnico.Dose = dettaglioIrrigazione.Ore;
                    APP_Dettaglio_Tecnico.Parziale = dettaglioIrrigazione.Portata;
                    // APP_Dettaglio_Tecnico.Efficienza = dettaglioIrrigazione.Efficienza;
                    APP_Dettaglio_Tecnico.Nitrati = dettaglioIrrigazione.Frequenza;
                    APP_Dettaglio_Tecnico.Inn1_data = dettaglioIrrigazione.DataInizio;
                    APP_Dettaglio_Tecnico.Inn2_data = dettaglioIrrigazione.DataFine;
                    APP_Dettaglio_Tecnico.Freatimetro = dettaglioIrrigazione.tipoIrrigazione != null ? dettaglioIrrigazione.tipoIrrigazione.codice : 0;
                    APP_Dettaglio_Tecnico.Dett_Cod = dettaglioIrrigazione.unitaDiMisura.codice;
                    // APP_Dettaglio_Tecnico.Ditta_Cod = dettaglioIrrigazione.macchina != null ? dettaglioIrrigazione.macchina.codice : 0;
                    dati.RicetteDettaglioTecnico.Add(APP_Dettaglio_Tecnico);

                    APP_Ricette_Destinazioni APP_Destinazione = new APP_Ricette_Destinazioni();
                    APP_Destinazione.Ricetta_Cod = APP_Dettaglio.Ricetta_Cod;
                    APP_Destinazione.Ricetta_Operazione_Cod = APP_Dettaglio.Ricetta_Operazione_Cod;
                    APP_Destinazione.Ricetta_Dettaglio_Cod = APP_Dettaglio.Ricetta_Dettaglio_Cod;
                    APP_Destinazione.Ricetta_Destinazione_Cod = -db.nuovoId("ricette_destinazioni");
                    scriviDestinazione(dettaglioIrrigazione.esercizioCDC, APP_Destinazione, superficeTotale);
                    APP_Destinazione.Qta = dettaglioIrrigazione.QtaTotale;
                    dati.RicetteDestinazioni.Add(APP_Destinazione);
                }

            }

            // se non ci sono dettagli è ho comunque impianti aggiungo dettaglio per lavorazioni
            if (dati.RicetteDettagli.Count == 0 && impiantiDestinazioni.Count > 0) {

                APP_Ricette_Dettagli APP_Dettaglio = new APP_Ricette_Dettagli();
                APP_Dettaglio.Ricetta_Cod = APP_Ricetta_Operazioni.Ricetta_Cod;
                APP_Dettaglio.Ricetta_Operazione_Cod = APP_Ricetta_Operazioni.Ricetta_Operazione_Cod;
                APP_Dettaglio.Ricetta_Dettaglio_Cod = -db.nuovoId("ricette_dettagli");

                if (lav_cod == LAVCOD_RACCOLTA) {
                    APP_Dettaglio.Cau_Mov = int.Parse(CAU_RILIEVO_RACCOLTA);
                    APP_Dettaglio.Lotto = "Indefinito";
                    APP_Dettaglio.Mezzo_Det = -1;
                    APP_Dettaglio.Elem_Cod = 210;
                } else APP_Dettaglio.Cau_Mov = int.Parse(CAU_LAVORAZIONE);

                // imposta destinazioni
                foreach (var impiantoCorrente in impiantiDestinazioni) {
                    APP_Ricette_Destinazioni APP_Destinazione = new APP_Ricette_Destinazioni();
                    APP_Destinazione.Ricetta_Cod = APP_Dettaglio.Ricetta_Cod;
                    APP_Destinazione.Ricetta_Operazione_Cod = APP_Dettaglio.Ricetta_Operazione_Cod;
                    APP_Destinazione.Ricetta_Dettaglio_Cod = APP_Dettaglio.Ricetta_Dettaglio_Cod;
                    APP_Destinazione.Ricetta_Destinazione_Cod = -db.nuovoId("ricette_destinazioni");
                    if (impiantoCorrente.documento is not null)
                    {
                        ScriviAllegati(attivita.centroAziendale.primaryKey.partitaIva, dati, impiantoCorrente.documento, APP_Ricetta_Operazioni.Ricetta_Operazione_Cod, APP_Destinazione.Ricetta_Destinazione_Cod);
                    }
                    scriviDestinazione(impiantoCorrente, APP_Destinazione, superficeTotale);
                    dati.RicetteDestinazioni.Add(APP_Destinazione);
                }

                dati.RicetteDettagli.Add(APP_Dettaglio);
            }

            // gestione risorse macchine e operatori
            foreach (var risorsa in attivita.risorse)
            {
                // macchina
                if (risorsa is RisorsaMacchina macchina)
                {
                    // dettaglio per macchina
                    APP_Ricette_Dettagli App_DettaglioMacchina = new APP_Ricette_Dettagli();
                    App_DettaglioMacchina.Ricetta_Cod = APP_Ricetta_Operazioni.Ricetta_Cod;
                    App_DettaglioMacchina.Ricetta_Operazione_Cod = APP_Ricetta_Operazioni.Ricetta_Operazione_Cod;
                    App_DettaglioMacchina.Ricetta_Dettaglio_Cod = -db.nuovoId("ricette_dettagli");
                    App_DettaglioMacchina.Elem_Cod = 1;
                    App_DettaglioMacchina.Mat_Cod = macchina.macchina.codice;
                    App_DettaglioMacchina.Cau_Mov = int.Parse(CAU_IMPUTAZIONE_PARCOMACCHINE);
                    
                    if (macchina.inizio is not null && macchina.fine is not null)
                    {
                        TimeSpan difference = macchina.fine.Value - macchina.inizio.Value;
                        App_DettaglioMacchina.Qta = (decimal)difference.TotalHours;
                    }

                    dati.RicetteDettagli.Add(App_DettaglioMacchina);
                }

                // operatore
                else if (risorsa is RisorsaPersona operatore)
                {
                    // dettaglio per operatore
                    APP_Ricette_Dettagli App_DettaglioOperatore = new APP_Ricette_Dettagli();
                    App_DettaglioOperatore.Ricetta_Cod = APP_Ricetta_Operazioni.Ricetta_Cod;
                    App_DettaglioOperatore.Ricetta_Operazione_Cod = APP_Ricetta_Operazioni.Ricetta_Operazione_Cod;
                    App_DettaglioOperatore.Ricetta_Dettaglio_Cod = -db.nuovoId("ricette_dettagli");
                    App_DettaglioOperatore.Elem_Cod = 0;
                    App_DettaglioOperatore.Mat_Cod = operatore.risorsaUmana.codice;
                    string causale = CAU_IMPUTAZIONE_MANODOPERA;
                    if (operatore.risorsaUmana.rapportoContabile != null)
                    {
                        int codRapporto = operatore.risorsaUmana.rapportoContabile.codice;
                        if (codRapporto == (int)Enum_Rapporti_Contabili_Standard.Terzista)
                        {
                            causale = CAU_IMPUTAZIONE_TERZISTI;
                        }
                        else if (codRapporto == (int)Enum_Rapporti_Contabili_Standard.Tecnico_Responsabile)
                        {
                            causale = CAU_IMPUTAZIONE_TECNICO_RESPONSABILE;
                        }
                    }
                    App_DettaglioOperatore.Cau_Mov = int.Parse(causale);

                    if (operatore.inizio is not null && operatore.fine is not null)
                    {
                        TimeSpan difference = operatore.fine.Value - operatore.inizio.Value;
                        App_DettaglioOperatore.Qta = (decimal)difference.TotalHours;
                    }

                    dati.RicetteDettagli.Add(App_DettaglioOperatore);
                }
            }

            dati.RicetteOperazioni.Add(APP_Ricetta_Operazioni);
            dati.Ricette.Add(APP_Ricetta);
        }

        // scrive l'attivita cdg nei dati da passare al server
        public void scriviAttivitaCDG(RicettePerScarico dati, AgronicaCoreModelsSTD.attivita.Attivita attivita, int id_attivita)
        {

            APP_CDG_Generale APP_CDG_Generale = new APP_CDG_Generale();
            APP_CDG_Generale.Id_Cdg_Generale = -db.nuovoId("tempirisorse");
            APP_CDG_Generale.Data_Inserimento = attivita.inizio;
            APP_CDG_Generale.Piva = attivita.centroAziendale.primaryKey.partitaIva;
            APP_CDG_Generale.Bozza = 0;
            APP_CDG_Generale.Note = attivita.note;

            var centriDiCosto = new List<CentroDiCosto>();

            if (dati.Ricette.Count > 0) {
                // centro di costo fake se attività collegata a campagna
                centriDiCosto.Add(new CentroDiCosto());
            } else {
                // centri di costo per attività cdg
                foreach (var cdc in attivita.centriDiCosto) {
                    if (cdc is EsercizioCDC || cdc is Progetto) {
                        centriDiCosto.Add(cdc);
                    }
                }
            } 

            foreach (var cdc in centriDiCosto) {

                foreach (var risorsa in attivita.risorse) if (risorsa is RisorsaTimeSheet)
                {
                    APP_CDG_Movimenti APP_CDG_Movimento = new APP_CDG_Movimenti();
                    APP_CDG_Movimento.Id_CDG_Movimenti = -db.nuovoId("tempirisorse_movimenti");
                    APP_CDG_Movimento.Id_Cdg_Generale = APP_CDG_Generale.Id_Cdg_Generale;
                    APP_CDG_Movimento.Piva = APP_CDG_Generale.Piva;
                    APP_CDG_Movimento.Id_Attivita = id_attivita;

                    if (attivita.centroAziendale != null)
                    {
                        APP_CDG_Movimento.sa_cod = attivita.centroAziendale.primaryKey.codice;
                    }

                    if (cdc is Progetto) {
                        APP_CDG_Movimento.Id_Imputazione = cdc.codice.intValue;
                    } else if (cdc is EsercizioCDC) {
                        var esercizio = ((EsercizioCDC)cdc).esercizio;
                        var impianto = AnagraficaAdapter.leggiImpianto(esercizio);
                        APP_CDG_Movimento.sa_cod = impianto.sa_cod;
                        APP_CDG_Movimento.appezza = impianto.appezza;
                        APP_CDG_Movimento.id_reg = impianto.id_reg;
                        APP_CDG_Movimento.progetto_cod = esercizio.codice;
                    }

                    if (risorsa is RisorsaTimeSheet)
                    {
                        RisorsaTimeSheet risorsaTimeSheet = (RisorsaTimeSheet) risorsa;
                        APP_CDG_Movimento.Data_Ora_Inizio = (DateTime)risorsaTimeSheet.inizio;
                        APP_CDG_Movimento.Data_Ora_Fine = risorsaTimeSheet.fine!=null?(DateTime)risorsaTimeSheet.fine:AGRODATAFINE;
                        APP_CDG_Movimento.Qta = 0; // (decimal)((RisorsaTimeSheet)risorsa).getQuantity();
                    }

                    if (risorsa is RisorsaPersona)
                    {
                        var risorsaUmana = ((RisorsaPersona)risorsa).risorsaUmana;
                        APP_CDG_Movimento.Cod_RisUm = risorsaUmana.codice;                        
                        APP_CDG_Movimento.NrBadge = risorsaUmana.contatto.badge;
                    }
                    else if (risorsa is RisorsaMacchina)
                    {
                        var macchina = ((RisorsaMacchina)risorsa).macchina;
                        APP_CDG_Movimento.Mac_Cod = macchina.codice;
                    }

                    dati.AttivitaMovimenti.Add(APP_CDG_Movimento);
                }

            }
            // if (attivita.job.jobConseguenti != null)
            if (dati.Ricette.Count > 0) {
                APP_Riferimenti_Interventi_Cdg APP_CDG_Riferimenti_Interventi = new APP_Riferimenti_Interventi_Cdg();
                APP_CDG_Riferimenti_Interventi.Id_Cdg_Generale_Rif = APP_CDG_Generale.Id_Cdg_Generale;
                APP_CDG_Riferimenti_Interventi.Piva = APP_CDG_Generale.Piva;
                APP_CDG_Riferimenti_Interventi.Ricetta_Cod = dati.Ricette.First().ricetta_cod;
                APP_CDG_Riferimenti_Interventi.Ricetta_Operazione_Cod = dati.RicetteOperazioni.First().Ricetta_Operazione_Cod;
                dati.AttivitaOperazioni.Add(APP_CDG_Riferimenti_Interventi);
            }

            dati.Attivita.Add(APP_CDG_Generale);
        }

        private void scriviDestinazione(EsercizioCDC impiantoCorrente, APP_Ricette_Destinazioni APP_Destinazione, decimal superficeTotale)
        {
            var impianto = impiantoCorrente.esercizio.impiantoPK;
            var centro = impianto.appezzamentoPK.centroAziendalePK;

            APP_Destinazione.Piva = centro.partitaIva;
            APP_Destinazione.Sa_Cod = centro.codice;
            APP_Destinazione.Appezza = impianto.appezzamentoPK.codice;
            APP_Destinazione.Id_Reg = impianto.codice;
            APP_Destinazione.Qta = impiantoCorrente.quantita;
            APP_Destinazione.Qta2 = impiantoCorrente.superficieTrattata;

            if (impiantoCorrente is EsercizioRilievoCDC) {
                APP_Destinazione.Validita_Inizio = ((EsercizioRilievoCDC)impiantoCorrente).dataRiferimento;
            }

            if (superficeTotale > 0 && impiantoCorrente.superficieTrattata > 0) {
                APP_Destinazione.QuotaDistribuzione = impiantoCorrente.superficieTrattata / superficeTotale;
            }
        }

        private void scriviRisorsaProdotto(RisorsaProdotto DettagliDaScrivere, APP_Ricette_Dettagli App_Dettagli)
        {

            int xProCod = 0;
            int xMatCod = 0;

            if ((DettagliDaScrivere.prodotto.elemCod == (int)enum_CategorieMagazzino.SEMENTI || DettagliDaScrivere.prodotto.elemCod == (int)enum_CategorieMagazzino.TRASFORMATI_VEGETALI) && DettagliDaScrivere.prodotto.codice > 0)
            {
                DettagliDaScrivere.prodotto.codice = -DettagliDaScrivere.prodotto.codice;
            }

            if (DettagliDaScrivere.prodotto.codice > 0)
                xProCod = DettagliDaScrivere.prodotto.codice;
            else
                xMatCod = -DettagliDaScrivere.prodotto.codice;

            App_Dettagli.Elem_Cod = DettagliDaScrivere.prodotto.elemCod;
            // App_Dettagli.Elem_Cod = DettagliDaScrivere.prodotto.tipo.codice;
            App_Dettagli.Pro_Cod = xProCod;
            App_Dettagli.Mat_Cod = xMatCod;

            App_Dettagli.Lotto = "";
            if (DettagliDaScrivere.MagazziniMovimentazioni != null)
            {
                var movimento = DettagliDaScrivere.MagazziniMovimentazioni.FirstOrDefault();
                App_Dettagli.Lotto = movimento != null ? movimento.Lotto : "";
            }

            App_Dettagli.Qta = DettagliDaScrivere.doseHaReale;
            App_Dettagli.Qta_Extra = DettagliDaScrivere.doseHlReale;
            App_Dettagli.Qta_Extra_Totale = DettagliDaScrivere.quantitaTotaleReale;
            App_Dettagli.Mezzo_Det = DettagliDaScrivere.flagDoseQuantitaTotale == 10 ? 1 : DettagliDaScrivere.flagTipoDose;
            App_Dettagli.Udm_Cod_Extra = DettagliDaScrivere.flagDoseQuantitaTotale;

            if (DettagliDaScrivere.unitaDiMisura != null) {
                App_Dettagli.Udm_Cod = DettagliDaScrivere.unitaDiMisura.codice;
            } else if (DettagliDaScrivere.unitaDiMisuraIndicata != null) {
                App_Dettagli.Udm_Cod = DettagliDaScrivere.unitaDiMisuraIndicata.codice;
            }
            if (DettagliDaScrivere.unitaDiMisuraIndicata != null) {
                App_Dettagli.Extra_Int = DettagliDaScrivere.unitaDiMisuraIndicata.codice;
            }
        }

        private void scriviDettaglioRilievo(DettaglioRilievo DettagliDaScrivere, APP_Ricette_Dettagli App_Dettagli, APP_Ricette_Dettaglio_Tecnico app_Tecnico)
        {
            App_Dettagli.Mezzo_Det = -1;

            if (app_Tecnico != null)
            {

                // chiavi
                app_Tecnico.Ricetta_Cod = App_Dettagli.Ricetta_Cod;
                app_Tecnico.Ricetta_Operazione_Cod = App_Dettagli.Ricetta_Operazione_Cod;
                app_Tecnico.Ricetta_Dettaglio_Cod = App_Dettagli.Ricetta_Dettaglio_Cod;

                // fase fenologica
                if (DettagliDaScrivere.faseFenologica != null && DettagliDaScrivere.faseFenologica.codice != 0)
                    app_Tecnico.FF_Classe = DettagliDaScrivere.faseFenologica.codice;

                // indice maturità
                if (DettagliDaScrivere.IndiceMaturita != 0)
                {
                    app_Tecnico.FF_Classe = DettagliDaScrivere.IndiceMaturita;
                    app_Tecnico.Dett_Cod = DettagliDaScrivere.unitaDiMisura != null ? DettagliDaScrivere.unitaDiMisura.codice: 0;
                }

                // indice resa
                if (DettagliDaScrivere.IndiceResa != 0)
                {
                    app_Tecnico.FF_Classe = DettagliDaScrivere.IndiceResa;
                    app_Tecnico.Dett_Cod = DettagliDaScrivere.unitaDiMisura != null ? DettagliDaScrivere.unitaDiMisura.codice : 0;
                }

                // rilievo avversità
                if (DettagliDaScrivere.IndiceMaturita == 0 && (DettagliDaScrivere.faseFenologica == null || DettagliDaScrivere.faseFenologica.codice == 0))
                {
                    app_Tecnico.Dett_Cod = DettagliDaScrivere.unitaDiMisura != null ? DettagliDaScrivere.unitaDiMisura.codice : 0;

                    switch (DettagliDaScrivere.avversitaGruppo.classType)
                    {
                        case "Avversita":
                            app_Tecnico.Av_Cod = DettagliDaScrivere.avversitaGruppo.codice;
                            //DT: se avversità singola con codice negativo, valorizzare con lo stesso valore il gruppo
                            if (DettagliDaScrivere.avversitaGruppo.codice < 0)
                            {
                                app_Tecnico.Av_Gru = app_Tecnico.Av_Cod;
                            }
                            break;

                        case "GruppoAvversita":
                            app_Tecnico.Av_Cod = 0;
                            app_Tecnico.Av_Gru = DettagliDaScrivere.avversitaGruppo.codice;
                            break;

                    }
                }

                if (app_Tecnico.Inn1_data < AGRODATAINIZIO)
                    app_Tecnico.Inn1_data = AGRODATAINIZIO;

                if (app_Tecnico.Inn2_data < AGRODATAINIZIO)
                    app_Tecnico.Inn2_data = AGRODATAFINE;
            }
        }

        private void scriviDettaglioTrattamento(DettaglioTrattamento DettagliDaScrivere, APP_Ricette_Dettagli App_Dettagli, APP_Ricette_Dettaglio_Tecnico app_Tecnico)
        {
            scriviRisorsaProdotto(DettagliDaScrivere, App_Dettagli);

            if (app_Tecnico != null)
            {

                // chiavi
                app_Tecnico.Ricetta_Cod = App_Dettagli.Ricetta_Cod;
                app_Tecnico.Ricetta_Operazione_Cod = App_Dettagli.Ricetta_Operazione_Cod;
                app_Tecnico.Ricetta_Dettaglio_Cod = App_Dettagli.Ricetta_Dettaglio_Cod;

                // avversità o gruppo.
                if (DettagliDaScrivere.avversitaGruppo != null)                {

                    int codice = DettagliDaScrivere.avversitaGruppo.codice;
                    if (codice < 0)
                    {
                        app_Tecnico.Av_Cod = codice;
                        app_Tecnico.Av_Gru = codice;
                    }
                    else if (DettagliDaScrivere.avversitaGruppo.classType == "Avversita")
                    {
                        app_Tecnico.Av_Cod = codice;
                    }
                    else
                    {
                        app_Tecnico.Av_Gru = codice;
                    }
                }              

                // valori predefiniti..:
                app_Tecnico.Inn1_data = AGRODATAINIZIO;
                app_Tecnico.Inn2_data = AGRODATAINIZIO;
            }
        }
        private void scriviDettaglioFertilizzazione(DettaglioFertilizzazione DettagliDaScrivere, APP_Ricette_Dettagli App_Dettagli, APP_Ricette_Dettaglio_Tecnico app_Tecnico)
        {
            scriviRisorsaProdotto(DettagliDaScrivere, App_Dettagli);

            // chiavi, scrivi         
            app_Tecnico.Ricetta_Cod = App_Dettagli.Ricetta_Cod;
            app_Tecnico.Ricetta_Operazione_Cod = App_Dettagli.Ricetta_Operazione_Cod;
            app_Tecnico.Ricetta_Dettaglio_Cod = App_Dettagli.Ricetta_Dettaglio_Cod;

            // avversità
            app_Tecnico.N = DettagliDaScrivere.N;
            app_Tecnico.P = DettagliDaScrivere.P;
            app_Tecnico.K = DettagliDaScrivere.K;
            app_Tecnico.CU = DettagliDaScrivere.Cu;

            if (app_Tecnico.Inn1_data < AGRODATAINIZIO)
                app_Tecnico.Inn1_data = AGRODATAINIZIO;

            if (app_Tecnico.Inn2_data < AGRODATAINIZIO)
                app_Tecnico.Inn2_data = AGRODATAFINE;
        }

        private AvversitaGruppo getAvversitaGruppo(int avCod, int avGru)
        {
            AvversitaGruppo avversitaGruppo = null;

            if (avCod != 0)
            {
                avversitaGruppo = new Avversita(avCod);
                // DT: se avversità singola con codice negativo, valorizzare con lo stesso valore il gruppo
                var av_gru = 0;
                if (avCod < 0)
                    av_gru = avCod;
                ((Avversita)avversitaGruppo).gruppo = new GruppoAvversita(av_gru);
            }
            else if (avGru != 0)
                avversitaGruppo = new GruppoAvversita(avGru);

            return avversitaGruppo;
        }
    }
}

