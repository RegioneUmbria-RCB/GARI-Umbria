using AgronicaCoreAPI.adapters;
using AgronicaCoreAPI.models;
using AgronicaCoreEntityFrameworkSTD_POCO.Models;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.risorse;
using System;
using System.Collections.Generic;
using static AgronicaCoreDataProviderSTD.CostantiPersonalizzate;
using static AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto;

namespace AgronicaCoreAPI.test
{
    public class AttivitaTest
    {
        public static CentroAziendale getCentroAziendale()
        {
            Impresa impresa = new Impresa();
            impresa.partitaIva = "00031571201";
            impresa.ragioneSociale = "Impresa Test";
            impresa.CUAA = "";

            CentroAziendale centro = new CentroAziendale(new CentroAziendale.PK(72482821, impresa.partitaIva));
            centro.nome = "Centro Test";

            return centro;
        }

        public static Impianto getImpianto()
        {

            CentroAziendale centroAziendale = getCentroAziendale();
            CentroAziendale.PK centroAziendalePK = centroAziendale.primaryKey;

            Appezzamento.PK appezzamentoPK = new Appezzamento.PK(72483840, centroAziendalePK);

            Appezzamento appezzamento = new Appezzamento(appezzamentoPK);                                    
            appezzamento.descrizione = "App. Test";

            Impianto.PK impiantoPrimaryKey = new Impianto.PK(72482817, appezzamentoPK);

            Impianto impianto = new Impianto(impiantoPrimaryKey);                                    
            impianto.descrizione = "Impianto Test";

            return impianto;
        }

        public static EsercizioCDC getEsercizioCDC(int codice, Impianto impianto)
        {
            Esercizio esercizio = new Esercizio(codice, "Esercizio " + codice);
            esercizio.validita = new IntervalloTemporale(AGRODATAINIZIO, AGRODATAFINE);
            esercizio.impiantoPK = getImpianto().primaryKey;
            // impianto.esercizi.Add(esercizio);

            EsercizioCDC cdc = new EsercizioCDC();
            // cdc.codice = new CodeType(123);
            cdc.esercizio = esercizio;

            return cdc;
        }

        public static Attivita getAttivita()
        {
            Job lavorazione = new Lavorazione(8, "Aratura");
            Attivita attivita = new Attivita();
            attivita.inizio = DateTime.Now;
            attivita.fine = AGRODATAFINE;
            attivita.centroAziendale = getCentroAziendale();
            attivita.centriDiCosto.Add(getEsercizioCDC(123,getImpianto()));
            attivita.job = lavorazione;
            attivita.note = "test";

            return attivita;
        }

        public static Attivita getAttivitaCDG()
        {
            Job attivitaCDG = new AttivitaCDG(58, "Aratura");
            Attivita attivita = new Attivita();
            attivita.inizio = DateTime.Now;
            attivita.fine = AGRODATAFINE;
            attivita.centroAziendale = getCentroAziendale();
            attivita.centriDiCosto.Add(getEsercizioCDC(123, getImpianto()));
            attivita.centriDiCosto.Add(getEsercizioCDC(456, getImpianto()));
            // attivita.centriDiCosto.Add(new Progetto() { codice = new CodeType() { intValue = 9 } });
            setRisorseCDG(attivita.risorse);
            attivita.job = attivitaCDG;
            attivita.note = "test attivita cdg";

            return attivita;
        }

        public static void setRisorseCDG(List<Risorsa> risorse)
        {

            risorse.Add(new RisorsaPersona()
            {
                risorsaUmana = new RisorseUmane(348) { contatto = new Contatto { badge = "" } },
                inizio = new DateTime(2021, 9, 14, 9, 0, 0),
                fine = new DateTime(2021, 9, 14, 13, 0, 0)
            });

            risorse.Add(new RisorsaPersona()
            {
                risorsaUmana = new RisorseUmane(349) { contatto = new Contatto { badge = "" } },
                inizio = new DateTime(2021, 9, 14, 14, 0, 0),
                fine = new DateTime(2021, 9, 14, 18, 0, 0)
            });

            risorse.Add(new RisorsaMacchina()
            {
                macchina = new ParcoMacchine { codice = 2 },
                inizio = new DateTime(2021, 9, 14, 9, 0, 0),
                fine = new DateTime(2021, 9, 14, 18, 0, 0)
            });

            risorse.Add(new RisorsaMacchina()
            {
                macchina = new ParcoMacchine { codice = 133 },
                inizio = new DateTime(2021, 9, 14, 9, 0, 0),
                fine = new DateTime(2021, 9, 14, 18, 0, 0)
            });
        }

        public static void saveAttivita(Attivita attivita)
        {
            using (var db = new CoreAPIContext())
            {
                // Console.WriteLine($"Database path: {db.DbPath}.");

                // Console.WriteLine("Crea attivita");
                db.Add(attivita);
                db.SaveChanges();

                //Console.WriteLine("Legge attivita");
                //attivita = db.Attivita.First();

                //Console.WriteLine("Aggiorna attivita");
                //attivita.note = "test";
                //db.SaveChanges();

                //Console.WriteLine("Delete the blog");
                //db.Remove(attivita);
                //db.SaveChanges();
            }
        }
    }
}
