using AgronicaCoreModelsSTD.utente;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData
{
    public class AgronicaCoreParametri_NG
    {
        public string PivaSuperUser;
        public string SuperUserUsername;
        public string UsernameOperazione;
        public string UtenteUsername;
        public string UtenteCodFiscale;
        public DateTime FinestraTemporaleInizio;
        public DateTime FinestraTemporaleFine;
        public int Lingua_Cod;
    }

    public class Utente_Permessi
    {
        public string Username;
        public string Nome;
        public string Cognome;
        public string Rag_Soc;
        public string Cod_Fisc;
        public string UsernameCommerciale;
        public List<Utente_Permesso> Permessi;
        public List<Utenti_Impostazioni> Impostazioni;
        public Messaggio_Utente_Permessi ValiditaUtentePermessiLicenza;
    }

    public class Utenti_Impostazioni
    {
        public int Impostazione_Cod;
        public string Valore;
    }

    public class Utente_Permesso
    {
        public int Permesso_ID;
        public int Permesso_Tipo;

        public Utente_Permesso(int idPermesso, int tipoPermesso) 
        {
            Permesso_ID = idPermesso;
            Permesso_Tipo = tipoPermesso;
        }
    }

    public class AgronicaLink_NG
    {
        public string linkGiasBase;
        public string linkAgronicaCoreAPI;
    }

    public class VariabiliInSessione_NG
    {
        public string collegamento_dpi;
        public string collegamento_fito;
        public string id_servizio;
        public string pathfileini;
        public string cn_server;
        public string cn_utenti;
        public string cn_logaccessi;
        public string stringa_cn_server;
        public string stringa_cn_utenti;
        public string progressivo_gias;
        public string utente_usr;
        public string utente_pwd;
        public string utente_codfiscale;
        public string utente_usr_crypt;
        public string utente_pwd_crypt;
        public string superuser_usr;
        public string superuser_pwd;
        public string superuser_piva;
        public string superuser_usr_crypt;
        public string superuser_pwd_crypt;
        public string finestratemporale_inizio;
        public string finestratemporale_fine;
        public string agronicacore_flag_cancellazionelogica;
        public string agronicacore_flag_visibilita;
        public string agronicacore_directorylog;
        public string agronicacore_nomefilelog;
    }

    public class Imprese_Impostazioni
    {
        public string Piva;
        public int Sa_Cod;
        public int Impostazione_Cod;
        public string Valore;
    }

    public class ImpiantiAgendaNG
    {
        public string Piva;
        public int Sa_Cod;
        public int Appezza;
        public int Id_Reg;
        public int Progetto_Cod;
        public int Veg_Cod;
        public int Id_Cod;
        public decimal Sup_Imp;
    }

}

