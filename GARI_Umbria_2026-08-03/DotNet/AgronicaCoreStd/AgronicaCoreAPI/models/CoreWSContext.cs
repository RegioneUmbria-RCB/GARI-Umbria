using AgronicaCoreEntityFrameworkSTD_POCO.Models;
using AgronicaCoreModelloSTD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgronicaCoreAPI.models
{
    public class CoreWSContext
    {

        RicettePerScarico dbContext;
        Dictionary<string, int> sequenze;

        public CoreWSContext(RicettePerScarico dbContext)
        {
            this.dbContext = dbContext;
            this.sequenze = new Dictionary<string, int>();
        }

        public int nuovoId(string tabella)
        {
            if (sequenze.ContainsKey(tabella))
            {
                sequenze[tabella]++;
            }
            else sequenze[tabella] = 1;
            return sequenze[tabella];
        }

        public RicettePerScarico leggiDbContext()
        {
            return dbContext;
        }

        public List<APP_Ricette> leggiRicette(int ricetta_cod)
        {
            List<APP_Ricette> rval = (from r in dbContext.Ricette where ricetta_cod == 0 || r.ricetta_cod == ricetta_cod select r).ToList();
            return rval;
        }

        public List<APP_Ricette_Operazioni> leggiOperazioni(int ricetta_cod, int ricetta_Operazione_cod)
        {
            List<APP_Ricette_Operazioni> rval = (
                from r in dbContext.RicetteOperazioni
                where (ricetta_cod == 0 || r.Ricetta_Cod == ricetta_cod)
                    && (ricetta_Operazione_cod == 0 || r.Ricetta_Operazione_Cod == ricetta_Operazione_cod)
                select r
            ).ToList();

            return rval;
        }

        public List<APP_Ricette_Dettagli> leggiDettagli(int ricetta_cod, int ricetta_Operazione_cod)
        {
            List<APP_Ricette_Dettagli> rval = (
                from r in dbContext.RicetteDettagli
                where r.Ricetta_Cod == ricetta_cod && r.Ricetta_Operazione_Cod == ricetta_Operazione_cod
                select r
            ).ToList();

            return rval;
        }

        public List<APP_Ricette_Dettaglio_Tecnico> leggiDettaglioTecnico(int ricetta_cod, int ricetta_Operazione_cod, int ricetta_dettaglio_Cod)
        {
            List<APP_Ricette_Dettaglio_Tecnico> rval = (
                from r in dbContext.RicetteDettaglioTecnico
                where r.Ricetta_Cod == ricetta_cod
                    && r.Ricetta_Operazione_Cod == ricetta_Operazione_cod
                    && r.Ricetta_Dettaglio_Cod == ricetta_dettaglio_Cod
                select r
            ).ToList();

            return rval;
        }

        public List<APP_Ricette_Destinazioni> leggiDestinazioni(int ricetta_cod, int ricetta_Operazione_cod, int ricetta_Dettaglio_Cod)
        {
            List<APP_Ricette_Destinazioni> rval = (
                from r in dbContext.RicetteDestinazioni
                where r.Ricetta_Cod == ricetta_cod
                    && r.Ricetta_Operazione_Cod == ricetta_Operazione_cod
                    && ricetta_Dettaglio_Cod == 0 || r.Ricetta_Dettaglio_Cod == ricetta_Dettaglio_Cod
                select r
            ).ToList();

            return rval;
        }

        public List<APP_Riferimenti_Interventi_Cdg> leggiAttivitaOperazioni(int ricetta_cod, int ricetta_Operazione_cod)
        {
            List<APP_Riferimenti_Interventi_Cdg> rval = (
                from r in dbContext.AttivitaOperazioni
                where (ricetta_cod == 0 || r.Ricetta_Cod == ricetta_cod)
                    && (ricetta_Operazione_cod == 0 || r.Ricetta_Operazione_Cod == ricetta_Operazione_cod)
                select r
            ).ToList();

            return rval;
        }

        public List<APP_CDG_Generale> leggiAttivita(int Id_Cdg_Generale)
        {
            List<APP_CDG_Generale> rval = (from r in dbContext.Attivita where Id_Cdg_Generale == 0 || r.Id_Cdg_Generale == Id_Cdg_Generale select r).ToList();
            return rval;
        }

        public List<APP_CDG_Movimenti> leggiAttivitaMovimenti(int Id_Cdg_Generale)
        {
            List<APP_CDG_Movimenti> rval = (from r in dbContext.AttivitaMovimenti where Id_Cdg_Generale == 0 || r.Id_Cdg_Generale == Id_Cdg_Generale select r).ToList();
            return rval;
        }

        public static void scriviDatiComuni(APP_Agronica_Entity oggettoDoveScrivere, string username)
        {
            DateTime dataora = DateTime.Now;

            oggettoDoveScrivere.Data_Creazione = dataora;
            oggettoDoveScrivere.Data_Modifica = dataora;
            oggettoDoveScrivere.Username_Creazione = username;
            oggettoDoveScrivere.Username_Modifica = username;

            // valori non memorizzati danno luogo a data "01/01/0001", quindi allineo su agro-data.inizio/fine
            if (oggettoDoveScrivere.Validita_Inizio < AgronicaCoreDataProviderSTD.CostantiPersonalizzate.AGRODATAINIZIO)
                oggettoDoveScrivere.Validita_Inizio = AgronicaCoreDataProviderSTD.CostantiPersonalizzate.AGRODATAINIZIO;

            if (oggettoDoveScrivere.Validita_Fine < AgronicaCoreDataProviderSTD.CostantiPersonalizzate.AGRODATAINIZIO)
                oggettoDoveScrivere.Validita_Fine = AgronicaCoreDataProviderSTD.CostantiPersonalizzate.AGRODATAFINE;
        }

        public void scriviDati(RicettePerScarico dati, string username)
        {
            foreach (var ricetteDaScrivere in dati.Ricette)
            {
                scriviDatiComuni(ricetteDaScrivere, username);
                dbContext.Ricette.Add(ricetteDaScrivere);
            }

            foreach (var operazioneDaScrivere in dati.RicetteOperazioni)
            {
                scriviDatiComuni(operazioneDaScrivere, username);
                dbContext.RicetteOperazioni.Add(operazioneDaScrivere);
            }

            foreach (var dettaglioDaScrivere in dati.RicetteDettagli)
            {
                scriviDatiComuni(dettaglioDaScrivere, username);
                dbContext.RicetteDettagli.Add(dettaglioDaScrivere);
            }

            foreach (var dettaglioTecnicoDaScrivere in dati.RicetteDettaglioTecnico)
            {
                scriviDatiComuni(dettaglioTecnicoDaScrivere, username);
                dbContext.RicetteDettaglioTecnico.Add(dettaglioTecnicoDaScrivere);
            }

            foreach (var destinazioneDaScrivere in dati.RicetteDestinazioni)
            {
                scriviDatiComuni(destinazioneDaScrivere, username);
                dbContext.RicetteDestinazioni.Add(destinazioneDaScrivere);
            }

            foreach (var attivitaDaScrivere in dati.Attivita)
            {
                scriviDatiComuni(attivitaDaScrivere, username);
                dbContext.Attivita.Add(attivitaDaScrivere);
            }

            foreach (var attivitaMovimentoDaScrivere in dati.AttivitaMovimenti)
            {
                scriviDatiComuni(attivitaMovimentoDaScrivere, username);
                dbContext.AttivitaMovimenti.Add(attivitaMovimentoDaScrivere);
            }

            foreach (var attivitaOperazioneDaScrivere in dati.AttivitaOperazioni)
            {
                scriviDatiComuni(attivitaOperazioneDaScrivere, username);
                dbContext.AttivitaOperazioni.Add(attivitaOperazioneDaScrivere);
            }

        }

    }
}
