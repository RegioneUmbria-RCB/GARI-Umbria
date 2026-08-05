using AgronicaCoreEntityFrameworkSTD_POCO.Models;
using AgronicaCoreModelloSTD;
using System;
using System.Collections.Generic;
using System.Linq;
using static AgronicaCoreDataProviderSTD.TipiEnumerativi;
using static AgronicaCoreDataProviderSTD.CostantiPersonalizzate;
using AgronicaCoreContabStdBIZ;
using AgronicaCoreAnagrafeStdDAL;
using AgronicaCoreAPI.models;

namespace AgronicaCoreAPI.adapters
{
    public class RicetteAdapter
    {
        private CoreWSContext db;

        public RicetteAdapter(RicettePerScarico ricette)
        {
            this.db = new CoreWSContext(ricette);
        }

        public AgronicaCoreModelloSTD.Ricette leggiRicetta(int ricetta_cod, int ricetta_operazione_cod)
        {
            var ricettaLetta = new AgronicaCoreModelloSTD.Ricette();
            APP_Ricette app_RicettaLetta = db.leggiRicette(ricetta_cod).FirstOrDefault();

            if (app_RicettaLetta == null) return ricettaLetta;

            List<APP_Ricette_Operazioni> app_listaOperazioni = db.leggiOperazioni(ricetta_cod, ricetta_operazione_cod);
            List<APP_Ricette_Dettagli> app_listaDettagli;
            List<APP_Ricette_Dettaglio_Tecnico> app_listaDettaglioTecnico;
            List<APP_Ricette_Destinazioni> app_listaDestinazioni;

            ricettaLetta.ricetta_cod = app_RicettaLetta.ricetta_cod;
            ricettaLetta.ricetta_des = app_RicettaLetta.ricetta_des;
            ricettaLetta.ricetta_numero = app_RicettaLetta.Ricetta_Numero;
            ricettaLetta.note = app_RicettaLetta.note;
            ricettaLetta.Tipo_Ricetta = (enum_TipoRicetta_DB)app_RicettaLetta.Tipo_Ricetta;

            string pivaPerLetturaProdotti = "";
            if (!string.IsNullOrEmpty(app_RicettaLetta.piva))
            {
                pivaPerLetturaProdotti = app_RicettaLetta.piva;
            }

            ricettaLetta.RicetteOperazioni = new List<AgronicaCoreModelloSTD.Ricette_Operazioni>();

            foreach (var app_operazioneCorrente in app_listaOperazioni)
            {

                var Operazione = new AgronicaCoreModelloSTD.Ricette_Operazioni();
                LeggiOperazione(Operazione, app_operazioneCorrente);
                app_listaDettagli = db.leggiDettagli(ricetta_cod, app_operazioneCorrente.Ricetta_Operazione_Cod);

                Operazione.Dettagli = new List<AgronicaCoreModelloSTD.Ricette_Dettagli>();
                AgronicaCoreModelloSTD.Ricette_Dettagli testTipo = AgronicaCoreContabStdBIZ.Ricette.OggettoDaLavCod(app_operazioneCorrente.Lav_Cod).OggettoDettaglioPerTipo;

                if (testTipo is Ricette_Dettagli_Fertilizzazione || testTipo is Ricette_Dettagli_Trattamento)
                {

                    APP_Ricette_Dettaglio_Tecnico app_dettaglioTecnicoAcqua = db.leggiDettaglioTecnico(ricetta_cod, app_operazioneCorrente.Ricetta_Operazione_Cod, 0).FirstOrDefault();

                    if (app_dettaglioTecnicoAcqua != null)
                    {
                        if (app_dettaglioTecnicoAcqua.Qta_Ril > 0)
                        {
                            Operazione.Acqua = app_dettaglioTecnicoAcqua.Qta_Ril;
                        }
                        else
                        {
                            Operazione.Acqua = -app_dettaglioTecnicoAcqua.Qta_Ril;
                            Operazione.Acqua_Totale_o_Ha = 1;
                        }
                    }

                }

                var causali = new List<string>() {
                    CAU_IMPUTAZIONE_PARCOMACCHINE,
                    CAU_IMPUTAZIONE_MANODOPERA,
                    CAU_IMPUTAZIONE_TECNICO_RESPONSABILE,
                    CAU_IMPUTAZIONE_TERZISTI,
                    CAU_SCARICO,
                    CAU_CARICO };

                List<APP_Ricette_Dettagli> app_listaDettagliDistribuzioni = (
                    from d in app_listaDettagli where !causali.Contains(d.Cau_Mov.ToString()) select d).ToList();

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

                        if (testTipo is AgronicaCoreModelloSTD.Ricette_Dettagli_Rilievi)
                        {
                            app_listaDettaglioTecnico =
                                db.leggiDettaglioTecnico(ricetta_cod, app_operazioneCorrente.Ricetta_Operazione_Cod, impiantoDestinazione.Ricetta_Dettaglio_Cod);

                            APP_Ricette_Dettagli app_dettaglioCorrente = (
                                from d in app_listaDettagliDistribuzioni
                                where d.Ricetta_Dettaglio_Cod == impiantoDestinazione.Ricetta_Dettaglio_Cod
                                select d
                            ).FirstOrDefault();

                            APP_Ricette_Dettaglio_Tecnico app_tecnico = app_listaDettaglioTecnico.FirstOrDefault();

                            var dettaglioCorrente = new AgronicaCoreModelloSTD.Ricette_Dettagli_Rilievi();
                            LeggiDettagliRilievi(dettaglioCorrente, app_dettaglioCorrente, app_tecnico, impiantoDestinazione);

                            var DistribuzioneDaPopolare = new Ricette_DistribuzioniSuImpianti();
                            Ricette_Destinazioni.LeggiDestinazioni(DistribuzioneDaPopolare, impiantoDestinazione);

                            if (dettaglioCorrente.ImpiantiInteressati == null)
                            {
                                dettaglioCorrente.ImpiantiInteressati = new List<AgronicaCoreModelloSTD.Ricette_DistribuzioniSuImpianti>();
                            }

                            dettaglioCorrente.ImpiantiInteressati.Add(DistribuzioneDaPopolare);

                            LeggiCentroSpecie(Operazione, DistribuzioneDaPopolare);

                            dettaglioCorrente.Operazione = Operazione;
                            Operazione.Dettagli.Add(dettaglioCorrente);
                        }

                    }
                }

                if (testTipo is not AgronicaCoreModelloSTD.Ricette_Dettagli_Rilievi)
                {

                    foreach (var app_dettaglioCorrente in app_listaDettagliDistribuzioni)
                    {
                        app_listaDettaglioTecnico =
                            db.leggiDettaglioTecnico(ricetta_cod, app_operazioneCorrente.Ricetta_Operazione_Cod, app_dettaglioCorrente.Ricetta_Dettaglio_Cod);

                        AgronicaCoreModelloSTD.Ricette_Dettagli dettaglioCorrente = null;

                        APP_Ricette_Dettagli app_dettaglioMagazzino = (
                            from d in app_listaDettagli
                            where d.Cau_Mov.ToString() == CAU_SCARICO
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

                        if (testTipo is Ricette_Dettagli_Trattamento)
                        {
                            dettaglioCorrente = new Ricette_Dettagli_Trattamento();
                            LeggiDettagliTrattamenti(
                                pivaPerLetturaProdotti,
                                (Ricette_Dettagli_Trattamento)dettaglioCorrente,
                                app_dettaglioCorrente,
                                app_listaDettaglioTecnico.FirstOrDefault(),
                                app_dettaglioMagazzino,
                                app_listaDestinazioniMagazzino
                             );

                            // forzo qta per semine/trapianti
                            if (app_dettaglioCorrente.Cau_Mov.ToString() == CAU_LAVORAZIONE && SuperficeTotale > 0 && app_dettaglioCorrente.Qta_Extra_Totale == 0)
                            {
                                ((Ricette_Dettagli_Trattamento)dettaglioCorrente).Dose_Ha_Reale = app_dettaglioCorrente.Qta / SuperficeTotale;
                                ((Ricette_Dettagli_Trattamento)dettaglioCorrente).Dose_Totale_Reale = app_dettaglioCorrente.Qta;
                            }
                        }

                        if (testTipo is Ricette_Dettagli_Fertilizzazione)
                        {
                            dettaglioCorrente = new Ricette_Dettagli_Fertilizzazione();
                            LeggiDettagliFertilizzazioni(
                                pivaPerLetturaProdotti,
                                (Ricette_Dettagli_Fertilizzazione)dettaglioCorrente,
                                app_dettaglioCorrente,
                                app_listaDettaglioTecnico.FirstOrDefault(),
                                app_dettaglioMagazzino,
                                app_listaDestinazioniMagazzino
                            );
                        }

                        if (testTipo is AgronicaCoreModelloSTD.Ricette_Dettagli_Lavorazioni)
                        {
                            dettaglioCorrente = new AgronicaCoreModelloSTD.Ricette_Dettagli_Lavorazioni();
                            AgronicaCoreContabStdBIZ.Ricette_Dettagli_Lavorazioni.LeggiDettagli((AgronicaCoreModelloSTD.Ricette_Dettagli_Lavorazioni)dettaglioCorrente, app_dettaglioCorrente);
                        }

                        if (dettaglioCorrente == null) throw (new Exception("Lav_cod non riconosciuto..."));

                        dettaglioCorrente.ImpiantiInteressati = new List<Ricette_DistribuzioniSuImpianti>();

                        // filtro per i soli impianti interessati (tipo = 0 )
                        var listaImpiantiInteressati = (from imp in app_listaDestinazioni where imp.Ricetta_Dettaglio_Cod == dettaglioCorrente.Ricetta_Dettaglio_Cod && imp.Tipo_Destinazione == 0 select imp).ToList();
                        foreach (var app_destinazione_corrente in listaImpiantiInteressati)
                        {
                            var DistribuzioneDaScrivere = new Ricette_DistribuzioniSuImpianti();
                            Ricette_Destinazioni.LeggiDestinazioni(DistribuzioneDaScrivere, app_destinazione_corrente);
                            dettaglioCorrente.ImpiantiInteressati.Add(DistribuzioneDaScrivere);
                            LeggiCentroSpecie(Operazione, DistribuzioneDaScrivere);
                        }

                        dettaglioCorrente.Operazione = Operazione;

                        Operazione.Dettagli.Add(dettaglioCorrente);
                    }
                }

                ricettaLetta.RicetteOperazioni.Add(Operazione);
            }

            return ricettaLetta;

        }

        public static void LeggiDettagliProdotti(string piva, AgronicaCoreModelloSTD.Ricette_Dettagli_Prodotti DettaglioDaLeggere, APP_Ricette_Dettagli App_Dettagli, APP_Ricette_Dettagli APP_DettaglioMagazzino, List<APP_Ricette_Destinazioni> App_Destinazioni)
        {

            // dati
            int ProdottoCod = 0;

            if (App_Dettagli.Mat_Cod == 0)
                ProdottoCod = App_Dettagli.Pro_Cod;
            else
                ProdottoCod = -App_Dettagli.Mat_Cod;

            // lettura con descrizione
            if (DettaglioDaLeggere.Prodotto == null)
            {
                // Prodotti_R letturaProdotto = new Prodotti_R();
                // DettaglioDaLeggere.Prodotto = letturaProdotto.Leggi(dbContext, piva, App_Dettagli.Elem_Cod, ProdottoCod);
                DettaglioDaLeggere.Prodotto = new AgronicaCoreModelloSTD.Prodotto() { Elem_Cod = App_Dettagli.Elem_Cod, Prodotto_Cod = ProdottoCod };
            }

            DettaglioDaLeggere.Dose_Ha_Reale = App_Dettagli.Qta;
            DettaglioDaLeggere.Dose_Hl_Reale = App_Dettagli.Qta_Extra;
            DettaglioDaLeggere.Dose_Totale_Reale = App_Dettagli.Qta_Extra_Totale;

            DettaglioDaLeggere.Ha_Hl = App_Dettagli.Mezzo_Det;
            DettaglioDaLeggere.Dose_QtaTotale = App_Dettagli.Udm_Cod_Extra;


            // lettura unità di misura
            DettaglioDaLeggere.Udm = new UnitaMisura() { udm_cod = App_Dettagli.Udm_Cod };
            DettaglioDaLeggere.Udm_Indicata = new UnitaMisura() { udm_cod = App_Dettagli.Extra_Int };

            // parte di carico/scarico di magazzino
            if (App_Destinazioni.Count > 0 && APP_DettaglioMagazzino != null)
            {
                var xDestinazione = App_Destinazioni.First();
               
                //Magazzini_R leggiXMagazzini = new Magazzini_R();
                //APP_Magazzini magazzino = null; // leggiXMagazzini.LeggiMagazzino(dbContext, xDestinazione.Piva, xDestinazione.Sa_Cod, 20, xDestinazione.Id_Reg);

                DettaglioDaLeggere.MagazziniMovimentazioni = new AgronicaCoreModelloSTD.RilevamentoDiMagazzino()
                {
                    Lotto = APP_DettaglioMagazzino.Lotto,
                    Magazzino = new AgronicaCoreModelloSTD.Fabbricato()
                    {
                        Piva = xDestinazione.Piva,
                        Sa_Cod = xDestinazione.Sa_Cod,
                        Tipo_Destinazione = xDestinazione.Tipo_Destinazione,
                        Fabbricato_Cod = xDestinazione.Id_Reg,
                        Fabbricato_Des = "" //DT 08/04/2024: per refactoring SonarQube abbiamo tolto = magazzino.Ubic_Des, perchè magazzino è stato messo a null da tempo (vedi riga 280 commentata)
                    },
                    udm = new AgronicaCoreModelloSTD.UnitaMisura() { udm_cod = DettaglioDaLeggere.Udm.udm_cod, udm_des = DettaglioDaLeggere.Udm.udm_des },
                    Qta = APP_DettaglioMagazzino.Qta,
                    Prodotto = new AgronicaCoreModelloSTD.Prodotto()
                    {
                        Elem_Cod = APP_DettaglioMagazzino.Elem_Cod,
                        Prodotto_Cod = DettaglioDaLeggere.Prodotto.Prodotto_Cod,
                        Prodotto_Des = DettaglioDaLeggere.Prodotto.Prodotto_Des,
                        Udm = new AgronicaCoreModelloSTD.UnitaMisura() { udm_cod = DettaglioDaLeggere.Udm.udm_cod, udm_des = DettaglioDaLeggere.Udm.udm_des }
                    },
                    TipoRilevamento = AgronicaCoreDataProviderSTD.TipiEnumerativi.enum_RilevamentoMagazzinoTipo.giacenza,
                    Descrizione = DettaglioDaLeggere.Prodotto.Prodotto_Des //DT 08/04/2024: per refactoring SonarQube abbiamo tolto + "[" + magazzino.Ubic_Des + "]", perchè magazzino è stato messo a null da tempo (vedi riga 280 commentata) 
                };                
            }
        }


        public void LeggiDettagliTrattamenti(string pivaPerLetturaProdotti, Ricette_Dettagli_Trattamento DettaglioDaLeggere, APP_Ricette_Dettagli App_Dettagli, APP_Ricette_Dettaglio_Tecnico app_Tecnico, APP_Ricette_Dettagli APP_Dettaglio_Magazzino, List<APP_Ricette_Destinazioni> APP_Destinazioni)
        {
            // parte comune:
            DettaglioDaLeggere.Ricetta_Dettaglio_Cod = App_Dettagli.Ricetta_Dettaglio_Cod;

            // parte prodotti:
            LeggiDettagliProdotti(pivaPerLetturaProdotti, DettaglioDaLeggere, App_Dettagli, APP_Dettaglio_Magazzino, APP_Destinazioni);

            // parte trattamenti:        
            // Avversita_R letturaAvversita = new Avversita_R();
            // GruppoAvversita_R letturaGruppi = new GruppoAvversita_R();

            if (app_Tecnico != null)
            {
                //APP_Avversita appAvv = letturaAvversita.Leggi(dbContext, app_Tecnico.Av_Cod);
                //APP_GruppoAvversita appGru = letturaGruppi.Leggi(dbContext, app_Tecnico.Av_Gru);

                if (app_Tecnico.Av_Cod != 0)
                    DettaglioDaLeggere.AvversitaGruppo = new AvversitaConGruppi() { Cod = "0|" + app_Tecnico.Av_Cod, DescrizioneAvversitaGruppo = "" };
                else if (app_Tecnico.Av_Gru != 0)
                    DettaglioDaLeggere.AvversitaGruppo = new AvversitaConGruppi() { Cod = app_Tecnico.Av_Gru + "|0", DescrizioneAvversitaGruppo = "" };
            }
        }

        public static void LeggiDettagliFertilizzazioni(string pivaPerLetturaProdotti, AgronicaCoreModelloSTD.Ricette_Dettagli_Fertilizzazione DettaglioDaLeggere, APP_Ricette_Dettagli App_Dettagli, APP_Ricette_Dettaglio_Tecnico app_Tecnico, APP_Ricette_Dettagli APP_DettaglioMagazzino, List<APP_Ricette_Destinazioni> App_Destinazioni)
        {

            // parte comune:
            DettaglioDaLeggere.Ricetta_Dettaglio_Cod = App_Dettagli.Ricetta_Dettaglio_Cod;

            // parte prodotti
            LeggiDettagliProdotti(pivaPerLetturaProdotti, DettaglioDaLeggere, App_Dettagli, APP_DettaglioMagazzino, App_Destinazioni);

            // parte fertilizzazioni
            DettaglioDaLeggere.N = app_Tecnico.N;
            DettaglioDaLeggere.P = app_Tecnico.P;
            DettaglioDaLeggere.K = app_Tecnico.K;
            DettaglioDaLeggere.CU = app_Tecnico.CU;
        }

        public static void LeggiDettagliRilievi(AgronicaCoreModelloSTD.Ricette_Dettagli_Rilievi DettaglioDaLeggere, APP_Ricette_Dettagli app_Dettagli, APP_Ricette_Dettaglio_Tecnico app_Tecnico, APP_Ricette_Destinazioni app_Destinazione)
        {

            // parte comune:
            DettaglioDaLeggere.Ricetta_Dettaglio_Cod = app_Dettagli.Ricetta_Dettaglio_Cod;

            // parte di rilievi (comune)
            // Reg_Impianti_R xImpiantoLettura = new Reg_Impianti_R();
            // APP_Reg_Impianti impiantoDes = xImpiantoLettura.LeggiImpianto(dbContext, app_Destinazione.Piva, app_Destinazione.Sa_Cod, app_Destinazione.Appezza, app_Destinazione.Id_Reg);

            // CentriAziendali_R xSaLettura = new CentriAziendali_R();
            // Centri_Aziendali sa = xSaLettura.EstraiListaCentriAziendali(dbContext, app_Destinazione.Piva).Where(g => g.Sa_Cod == app_Destinazione.Sa_Cod).FirstOrDefault;

            DettaglioDaLeggere.Impianto = ""; // SalvaRilievoComponiDescrizione(sa, impiantoDes);

            // Avversità
            if (app_Tecnico.Av_Cod != 0 || app_Tecnico.Av_Gru != 0)
            {
                //MisuraXAvversita_R xLetturaAV = new MisuraXAvversita_R();
                //MisuraXAvversita ffDes = xLetturaAV.Leggi(dbContext, 0, app_Tecnico.Av_Cod, app_Tecnico.Av_Gru, app_Tecnico.Dett_Cod).FirstOrDefault;

                // if (ffDes != null) DettaglioDaLeggere.Descrizione = ffDes.MisuraAvversitaDescrizione;
                DettaglioDaLeggere.Avversita = app_Tecnico.Av_Cod;
                DettaglioDaLeggere.GruppoAvversita = app_Tecnico.Av_Gru;
                DettaglioDaLeggere.UnitaDiMisuraCod = app_Tecnico.Dett_Cod;
                DettaglioDaLeggere.QtaRilevata = (double)app_Destinazione.Qta;
                DettaglioDaLeggere.QtaRilevataString = app_Destinazione.Qta.ToString();
            }

            // Fase Fenologica (la data è stata specificata)
            if (app_Tecnico.FF_Classe != 0 && app_Destinazione.Validita_Inizio != AGRODATAINIZIO)
            {
                // SpecieVegetaliXStadiCrescita_R xLetturaFF = new SpecieVegetaliXStadiCrescita_R();
                // SpecieVegetaliXStadiCrescita ffDes = xLetturaFF.EstraiListaSpecieVegetaliXStadiCrescitaFiltrataPerSpecieBBCH(dbContext, 0, app_Tecnico.FF_Classe).FirstOrDefault;

                DettaglioDaLeggere.FaseFenologica = app_Tecnico.FF_Classe;
                DettaglioDaLeggere.DataOraRilievo = app_Destinazione.Validita_Inizio;
                // DettaglioDaLeggere.Descrizione = ffDes.Descrizione;
                DettaglioDaLeggere.QtaRilevataString = app_Destinazione.Validita_Inizio.ToShortDateString();
            }
        }

        public void LeggiOperazione(AgronicaCoreModelloSTD.Ricette_Operazioni OperazioneDaLeggere, APP_Ricette_Operazioni App_Operazione)
        {

            // chiavi..
            OperazioneDaLeggere.Ricetta_Operazione_Cod = App_Operazione.Ricetta_Operazione_Cod;

            // dati
            OperazioneDaLeggere.Operazione = new Operazione() { lav_cod = App_Operazione.Lav_Cod };
            OperazioneDaLeggere.Ricetta_Operazione_Des = App_Operazione.Ricetta_Operazione_Des;
            OperazioneDaLeggere.W_Anagrafica_Stati_Cod = App_Operazione.W_Anagrafica_Stati_Cod;
            OperazioneDaLeggere.Bozza = App_Operazione.Bozza;
            OperazioneDaLeggere.Note = App_Operazione.Note;

            OperazioneDaLeggere.Epoca = App_Operazione.Extra_Int;

            OperazioneDaLeggere.DataRiferimento = App_Operazione.Validita_Inizio;

            if (App_Operazione.Ricetta_Operazione_Cod_RIF != 0)
                OperazioneDaLeggere.Ricetta_Operazione_Rif = new AgronicaCoreModelloSTD.Ricette_Operazioni()
                {
                    Ricetta_Operazione_Cod = App_Operazione.Ricetta_Operazione_Cod_RIF
                };
        }

        private void LeggiCentroSpecie(AgronicaCoreModelloSTD.Ricette_Operazioni Operazione, Ricette_DistribuzioniSuImpianti DistribuzioneDaScrivere)
        {
            if (Operazione.CentroAziendale == null)
            {
                Operazione.CentroAziendale = new AgronicaCoreModelloSTD.Centri_Aziendali()
                {
                    Piva = DistribuzioneDaScrivere.Impianto.piva,
                    Sa_Cod = DistribuzioneDaScrivere.Impianto.sa_cod
                };

                if (Operazione.Specie == null)
                {
                    //AgronicaCoreAnagrafeStdDAL.SpecieVegetali_R letturaSpecie = new AgronicaCoreAnagrafeStdDAL.SpecieVegetali_R();
                    //SpecieVegetaliDestinazioni SpecieImpianto = letturaSpecie.EstraiSpecieVegetaliImpianto(dbContext, DistribuzioneDaScrivere.Impianto.piva, DistribuzioneDaScrivere.Impianto.sa_cod, DistribuzioneDaScrivere.Impianto.appezza, DistribuzioneDaScrivere.Impianto.id_reg);
                    //Operazione.Specie = SpecieImpianto;
                }
            }
        }

    }
}
