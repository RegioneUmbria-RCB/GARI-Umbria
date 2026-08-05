using AgronicaCoreAPI.models;
using static AgronicaCoreDataProviderSTD.CostantiPersonalizzate;
using AgronicaCoreEntityFrameworkSTD_POCO.Models;
using AgronicaCoreModelloSTD;
using System.Collections.Generic;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using System.Linq;
using System;
using AgronicaCoreModelsSTD.attivita;
using static AgronicaCoreModelsSTD.attivita.Attivita;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System.Data;
using AgronicaCoreUtilityStd;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Newtonsoft.Json;
using Attivita = AgronicaCoreModelsSTD.attivita.Attivita;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreDTOStd.InData;
using AgronicaCoreModelsSTD.metaschema;

namespace AgronicaCoreAPI.adapters
{
    public class VisiteAdapter    {

        public VisitePerScarico initDati()
        {
            VisitePerScarico dati = new VisitePerScarico();
            dati.Visite = new List<APP_Visite>();
            dati.VisiteDettagli = new List<APP_Visite_Dettagli>();
            dati.VisiteDestinazioni = new List<APP_Visite_Destinazioni>();
            return dati;
        }

        public bool isZero(double value, double epsilon = 0.000001)
        {
            return Math.Abs(value) < epsilon;
        }

        public List<Attivita> convertiAttivita(List<Attivita> attivita)
        {
            // var json = JsonConvert.SerializeObject(attivita);
            List<Attivita> visite = new List<Attivita>();

            foreach (var x in attivita) {

                Attivita agenda = x;

                /* foreach (var risorsa in x.risorse)
                {
                    if (risorsa is DettaglioVisita)
                    {
                        agenda = ((DettaglioVisita)risorsa).AttivitaCollegate.FirstOrDefault();
                    }
                } 
                */
                 
                var visita = new Attivita();
                visita.codice = x.codice;
                visita.descrizione = x.descrizione;
                visita.centroAziendale = agenda.centroAziendale;
                visita.job = agenda.job;
                visita.inizio = x.oraInizio;
                visita.fine = x.oraFine;
                visita.stato = Stati.Eseguita;
                visita.statoWorkflow = StatiWorkflowQdC.Da_Eseguire;
                visita.tipo = Tipo_Attivita.Ricetta;
                if (agenda.attivitaPersonalizzata != null)
                {
                    var l = new Lavorazione(LAVCOD_VISITA, "");
                    var a = new AttivitaCDG(agenda.attivitaPersonalizzata.codice, "");
                    visita.job = new JobComposito(l, a);
                }
                visita.latitude = x.latitude;
                visita.longitude = x.longitude;
                visita.note = x.note;

                if (agenda.utilizzoTerreno != null)
                {
                    if (agenda.utilizzoTerreno is DestinazioneUso)
                    {
                        visita.risorse.Add(new RisorsaDestinazioneUso() { destinazioneUso = (DestinazioneUso)agenda.utilizzoTerreno });
                    }
                    else if (agenda.utilizzoTerreno is Varieta)
                    {
                        visita.risorse.Add(new RisorsaSpecie() { specie = ((Varieta)agenda.utilizzoTerreno).specie });
                    }
                }

                foreach (var r in agenda.risorse) { 
                    if (r is DettaglioRilievo) ((DettaglioRilievo)r).DataOraRilievo = agenda.inizio;
                    if (r is not RisorsaAssegnatarioVisita) visita.risorse.Add(r);
                }

                foreach (var cdc in agenda.centriDiCosto) visita.centriDiCosto.Add(cdc);

                visite.Add(visita);
            }

            return visite;
        }

        // legge l'attivita visita dai dati restituiti dal server
        public AgronicaCoreModelsSTD.attivita.Attivita leggiAttivita(VisitePerScarico dati)
        {
            if (dati.Visite.Count == 0) return null;

            var attivita = new AgronicaCoreModelsSTD.attivita.Attivita();

            APP_Visite APP_Visita = dati.Visite.First();
            APP_Visite_Dettagli APP_Visita_Dettaglio = dati.VisiteDettagli.First();

            attivita.centroAziendale = new AgronicaCoreModelsSTD.anagrafiche.CentroAziendale();
            attivita.centroAziendale.primaryKey = new AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK();
            attivita.centroAziendale.primaryKey.partitaIva = APP_Visita.Piva;
            attivita.centroAziendale.primaryKey.codice = APP_Visita.Sa_Cod;
            attivita.inizio = APP_Visita.Data_Visita;
            attivita.fine = APP_Visita.Validita_Fine;
            attivita.stato = Stati.Eseguita;
            attivita.statoWorkflow = StatiWorkflowQdC.Eseguito;
            attivita.tipo = Tipo_Attivita.Ricetta;
            var l = new Lavorazione(LAVCOD_VISITA, "");
            var a = new AttivitaCDG(APP_Visita_Dettaglio.Id_Attivita, "");
            attivita.job = new JobComposito(l, a);
            attivita.descrizione = APP_Visita.Note;
            attivita.note = APP_Visita_Dettaglio.Descrizione;

            if (!string.IsNullOrEmpty(APP_Visita.Posizione))
            {
                var posizione = APP_Visita.Posizione.Split("|");
                attivita.latitude = double.Parse(posizione[0].Replace(".",","));
                attivita.longitude = double.Parse(posizione[1].Replace(".", ","));
            }
            
            if (APP_Visita_Dettaglio.Dettaglio_VegCod != 0)
            {
                attivita.risorse.Add(new RisorsaSpecie() { specie = new Specie(APP_Visita_Dettaglio.Dettaglio_VegCod) });
            } else if (APP_Visita_Dettaglio.Dettaglio_IdCod != 0)
            {
                attivita.risorse.Add(new RisorsaDestinazioneUso() { destinazioneUso = new DestinazioneUso(APP_Visita_Dettaglio.Dettaglio_VegCod) });
            }

            if (APP_Visita_Dettaglio.Dettaglio_SpeCod != 0)
            {
                attivita.risorse.Add(
                    new RisorsaZootecnica(
                        APP_Visita_Dettaglio.Dettaglio_GenCod,
                        APP_Visita_Dettaglio.Dettaglio_SpeCod,
                        APP_Visita_Dettaglio.Dettaglio_IProCod));
            }

            if (APP_Visita_Dettaglio.Dettaglio_SpeCod != 0)
            {
                attivita.risorse.Add(
                    new RisorsaZootecnica(
                        APP_Visita_Dettaglio.Dettaglio_GenCod,
                        APP_Visita_Dettaglio.Dettaglio_SpeCod,
                        APP_Visita_Dettaglio.Dettaglio_IProCod));
            }

            foreach (var destinazione in dati.VisiteDestinazioni) {
                
                if (destinazione.Id_Reg == 0 && destinazione.Appezza != 0)
                {
                    if (destinazione.Appezza > 0)
                    {
                        attivita.risorse.Add(new RisorsaSpecie() { specie = new Specie(destinazione.Appezza) });
                    } 
                    else
                    {
                        attivita.risorse.Add(new RisorsaDestinazioneUso() { destinazioneUso = new DestinazioneUso(-destinazione.Appezza) });
                    }
                } 
                else 
                {
                    var centroAziendalePK = new AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(destinazione.Sa_Cod, destinazione.Piva);
                    var appezzamentoPK = new AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(destinazione.Appezza, centroAziendalePK);
                    var impiantoPK = new AgronicaCoreModelsSTD.anagrafiche.Impianto.PK(destinazione.Id_Reg, appezzamentoPK);

                    EsercizioCDC cdc = new EsercizioCDC();
                    cdc.esercizio = new AgronicaCoreModelsSTD.anagrafiche.Esercizio();
                    cdc.esercizio.impiantoPK = impiantoPK;
                    cdc.superficieTrattata = destinazione.Qta2;
                    attivita.centriDiCosto.Add(cdc);
                }                
            }

            return attivita;
        }


        // scrive l'attivita visita nei dati da passare al server
        public void scriviAttivita(VisitePerScarico dati, AgronicaCoreModelsSTD.attivita.Attivita attivita, int id_attivita, string user)
        {
            // numerazione negativa per dati app
            // int Visita_Cod = int.Parse(attivita.codice);
            int Visita_Cod = int.Parse(attivita.codice)>0?int.Parse(attivita.codice):-1;
            int Visita_Dettaglio_Cod = -1;
            int Visita_Destinazione_Cod = -1;
            int Visita_Documento_Cod = -1;

            APP_Visite APP_Visita = new();
            APP_Visita.Visita_Cod = Visita_Cod;
            APP_Visita.Tipo_Visita = 0;
            APP_Visita.Piva = attivita.centroAziendale.primaryKey.partitaIva;
            APP_Visita.Sa_Cod = attivita.centroAziendale.primaryKey.codice;
            APP_Visita.Data_Visita = attivita.inizio;
            APP_Visita.Validita_Inizio = attivita.inizio;
            APP_Visita.Validita_Fine = attivita.fine;
            APP_Visita.Operatore = user;
            if (!isZero(attivita.latitude) && !isZero(attivita.longitude)) {
                APP_Visita.Posizione = (attivita.latitude + "|" + attivita.longitude).Replace(",", ".");
            }
            APP_Visita.Lav_Cod = LAVCOD_VISITA;
            APP_Visita.Note = attivita.descrizione;
            dati.Visite.Add(APP_Visita);

            APP_Visite_Dettagli APP_Visita_Dettaglio = new();
            APP_Visita_Dettaglio.Visita_Cod = APP_Visita.Visita_Cod;
            APP_Visita_Dettaglio.Visita_Dettaglio_Cod = Visita_Dettaglio_Cod;
            APP_Visita_Dettaglio.Id_Attivita = id_attivita;
            APP_Visita_Dettaglio.Descrizione = attivita.note;
            dati.VisiteDettagli.Add(APP_Visita_Dettaglio);
            
            foreach (var r in attivita.risorse)
            {
                /* if (r is RisorsaSpecie || r is RisorsaDestinazioneUso)
                {
                    int specie = 0;                    
                    if (r is RisorsaSpecie) specie = ((RisorsaSpecie)r).specie.codice;
                    else specie = -((RisorsaDestinazioneUso)r).destinazioneUso.codice;
                
                    APP_Visite_Destinazioni APP_Visita_Destinazione = new()
                    {
                        Visita_Cod = APP_Visita.Visita_Cod,
                        Visita_Destinazione_Cod = Visita_Destinazione_Cod,
                        Piva = APP_Visita.Piva,
                        Sa_Cod = APP_Visita.Sa_Cod,
                        Appezza = specie,
                        Id_Reg = 0
                    };

                    dati.VisiteDestinazioni.Add(APP_Visita_Destinazione);
                    Visita_Destinazione_Cod--;
                } */

                if (r is RisorsaSpecie) {
                    APP_Visita_Dettaglio.Dettaglio_VegCod = ((RisorsaSpecie)r).specie.codice;
                } 
                else if (r is RisorsaDestinazioneUso)
                {
                    APP_Visita_Dettaglio.Dettaglio_IdCod = ((RisorsaDestinazioneUso)r).destinazioneUso.codice;
                }
                else if (r is RisorsaZootecnica)
                {
                    APP_Visita_Dettaglio.Dettaglio_GenCod = ((RisorsaZootecnica)r).genere.codice;
                    APP_Visita_Dettaglio.Dettaglio_SpeCod = ((RisorsaZootecnica)r).specie.codice;
                    APP_Visita_Dettaglio.Dettaglio_IProCod = ((RisorsaZootecnica)r).indirizzoProd.codice;
                }
            }                

            foreach (var cdc in attivita.centriDiCosto)
            {

                if (cdc is EsercizioCDC)
                {
                    EsercizioCDC esercizioCDC = (EsercizioCDC)cdc;
                    var impianto = esercizioCDC.esercizio.impiantoPK;
                    var centro = impianto.appezzamentoPK.centroAziendalePK;

                    APP_Visite_Destinazioni APP_Visita_Destinazione = new()
                    {
                        Visita_Cod = APP_Visita.Visita_Cod,
                        Visita_Destinazione_Cod = Visita_Destinazione_Cod,
                        Piva = centro.partitaIva,
                        Sa_Cod = centro.codice,
                        Appezza = impianto.appezzamentoPK.codice,
                        Id_Reg = impianto.codice,
                        Qta2 = esercizioCDC.superficieTrattata
                    };

                    dati.VisiteDestinazioni.Add(APP_Visita_Destinazione);
                    Visita_Destinazione_Cod--;
                }
            }

            //Allegati visita
            if (attivita.documenti is not null)
            {
                foreach (var d in attivita.documenti)
                {
                    ScriviAllegati(APP_Visita.Piva, dati, d, Visita_Cod, Visita_Documento_Cod);
                    Visita_Documento_Cod--;
                }
            }
        }

        public void ScriviAllegati(string piva, VisitePerScarico dati, AgronicaCoreModelsSTD.documenti.Documento documento, int Visita_Cod, int Visita_Documento_Cod)
        {
            DocumentoPerScarico documentoPerScarico = new DocumentoPerScarico();
            documentoPerScarico.Documento_Cod = Visita_Documento_Cod;
            documentoPerScarico.Piva = piva;
            documentoPerScarico.Descrizione = documento.Descrizione;
            documentoPerScarico.Note = documento.Note;
            documentoPerScarico.Data_Scadenza = documento.Data_Scadenza;
            if (documentoPerScarico.Data_Scadenza < AGRODATAINIZIO)
            {
                documentoPerScarico.Data_Scadenza = AGRODATAFINE;
            }
            documentoPerScarico.ID_Tipologia = documento.ID_Tipologia;
            documentoPerScarico.Allegati = new List<DocumentoAllegato>();
            documentoPerScarico.Visita_Cod = Visita_Cod;
            documentoPerScarico.Ricetta_Operazione_Cod = Visita_Cod;
            documentoPerScarico.Ricetta_Destinazione_Cod = 0;
            foreach (var allegato in documento.Allegati)
            {
                documentoPerScarico.Allegati.Add(new DocumentoAllegato() { FileName = allegato.FileName, FileByte = allegato.FileByte });
            }
            if (dati.VisiteDocumenti is null)
            {
                dati.VisiteDocumenti = new List<DocumentoPerScarico>();
            }
            dati.VisiteDocumenti.Add(documentoPerScarico);
        }

        public void scriviDatiComuni(VisitePerScarico dati, string username)
        {
            foreach (var ricetteDaScrivere in dati.Visite)
            {
                CoreWSContext.scriviDatiComuni(ricetteDaScrivere, username);
            }

            foreach (var dettaglioDaScrivere in dati.VisiteDettagli)
            {
                CoreWSContext.scriviDatiComuni(dettaglioDaScrivere, username);
            }

            foreach (var destinazioneDaScrivere in dati.VisiteDestinazioni)
            {
                CoreWSContext.scriviDatiComuni(destinazioneDaScrivere, username);
            }
        }
    }
}
