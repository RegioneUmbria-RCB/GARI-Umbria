using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.profilazione
{

    public class BaseUtente: IUtentePassword
    {
        public string UserName { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;

        public BaseUtente() { }

        public BaseUtente(string username)
        {
            UserName = username;
        }

    }

    // TODO: nei core segna 14 riferimenti, ce ne sono altri in altri progetti?
    //       In caso fossero solo questi, sostituire con Utente2
    public class Utente: IUtentePassword
    {
        public string Attivo { get; set; }
        public string Azienda_Persona { get; set; }
        public string CodFisc { get; set; }
        public string Cognome { get; set; }
        public string Data_Creazione { get; set; }
        public string Data_Modifica { get; set; }
        public string Dettagli { get; set; }
        public string Email { get; set; }
        public string Flag_Azienda_Persona { get; set; }
        public string Flag_Encrypted { get; set; }
        public string GDPR { get; set; }
        public string GruppiCod { get; set; }
        public string GruppiDes { get; set; }
        public string kendoKey { get; set; }
        public string Lingua_Cod { get; set; }
        public string Nome { get; set; }
        public string NumeroAccessi { get; set; }
        public string Password { get; set; }
        public string PIVA { get; set; }
        public string Piva_SuperUser { get; set; }
        public string Rag_Soc { get; set; }
        public string ServiziAttivi { get; set; }
        public string Tel { get; set; }
        public string Tipologia_Cod { get; set; }
        public string Tipologia_Des { get; set; }
        public string UltimoAccesso { get; set; }
        public string UserName { get; set; }
        public string UserNameCommerciale { get; set; }
        public string _Utente { get; set; }
        public string Utente_Profilo { get; set; }
        public DateTime Validita_Fine { get; set; }
        public DateTime Validita_Inizio { get; set; }
        public string Visibilita { get; set; }
        public Boolean Flag_Accesso_SPID { get; set; }
        public string CF_SPID { get; set; }
        public string CF_SPID_Corrente { get; set; }
        public DateTime FinestraTemporaleInizio { get; set; }
        public DateTime FinestraTemporaleFine { get; set; }

    }


    public class UtenteDTO: BaseUtente
    {
        public string Piva_SuperUser { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Rag_Soc { get; set; }
        public string Email { get; set; }
        public string piva { get; set; }
        public string codice_fiscale { get; set; }
        public string username_commerciale { get; set; }
        public int flag_azienda_persona { get; set; }
        public TipologiaUtente Tipologia { get; set; }

    }

    public class Utente_DettagliAWS
    {
        public string UserName { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string UltimoAccesso { get; set; }
        public string NumeroAccessi { get; set; }
    }

    public class Utente_DettagliGDPR
    {
        public string UserName { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string Data_Accettazione { get; set; }
    }

    public class InformazioniAssistenza
    {
        public string email { get; set; }
        public string tel { get; set; }
        public string whatsapp { get; set; }
    }

    public class UtentePermessi: BaseUtente
    {
        public DateTime ValiditaInizioPermessi { get; set; }
        public DateTime ValiditaFinePermessi { get; set; }
        public TipologiaUtente Tipologia { get; set; }
    }

    public class UtenteAccessoSPID : IUtente
    {
        public string UserName { get; set; }
        public string CfLogin { get; set; }
        public Boolean ObbligaLoginSPID { get; set; }
    }

    public class UserCreationConfig: IUserConfig
    {
        public int profile { get; set; }
    }

    public class UtenteFinestraTemp : IUtenteFinestraTemp
    {
        public string UserName { get; set; } = String.Empty;
        public IntervalloTemporale FinestraTemporale { get; set; } = new IntervalloTemporale();

        public UtenteFinestraTemp() { }
        public UtenteFinestraTemp(string username)
        {
            UserName = username;
        }
        public UtenteFinestraTemp(string username, IntervalloTemporale finestra)
        {
            UserName = username;
            FinestraTemporale = finestra;
        }

    }
}
