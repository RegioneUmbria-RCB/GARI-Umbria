using System;
using System.Collections.Generic;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.anagrafiche;

namespace AgronicaCoreModelsSTD.attivita
{
    public class Parametri_Aggiuntivi_Attivita
    {
        public Lavorazione operazione { get; set; }

        public string key { get; set; }
        public string value { get; set; }

    }


    //CLASSE USATA NELLA SEMINA CON FRAZIONAMENTO PER MEMORIZZARE LA SUPERFICIE TRATTATA PER SINGOLO PRODOTTO USATO
    public class SupTrattata_x_DettaglioSemina
    {
        public int Mat_Cod { get; set; }
        public decimal Sup_Trattata { get; set; }

    }


    //CLASSE USATA NELLA RACCOLTA PER MEMORIZZARE LA DATA CARENZA DI OGNI IMPIANTO 
    public class DataCarenzaRaccolta_x_Impianto
    {
        public string KeyImpianto { get; set; }  //PIVA | SA_COD | APPEZZA | ID_REG
        public string App_Nome { get; set; }
        public string DataCarenza { get; set; }
        public string CarenzaStr { get; set; }

    }

    public class ModificaMultipla_Attivita
    {
        public int Tipo_Modifica { get; set; } //enum enum_ModificaMultiplaOperazioni
        public List<Attivita_xModificaMultipla> Attivita_list { get; set; }
        public List<Risorsa> Risorsa_list { get; set; } //Valorizzate solo le chiavi: per la macchina --> mac_cod (macchina.codice); per le persona --> Cod_Rapp (risorsaUmana.rapportoContabile.codice ) e Cod_Risum (risorsaUmana.codice)
        public Fabbricato Magazzino { get; set; } //Valorizzata solo la PK
        public bool? Elimina_Precedenti { get; set; } //Presente solo in Aggiungi Macchina/Operatore
        public bool? Solo_Aziendali { get; set; } //Presente solo in Aggiungi Macchina/Operatore
    }

    public class Attivita_xModificaMultipla
    {
        public int ID_Agenda { get; set; }
        public int Raccoglitore_Cod { get; set; }
        public string Lav_Des { get; set; }
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Lav_Cod { get; set; }
        public DateTime Data { get; set; }

    }

    public class CodiciAttivita_x_CentriAziendali
    {
        public int ID_Agenda { get; set; }
        public int Sa_Cod { get; set; }
        public int Lav_Cod { get; set; }
        public int Ricetta_Cod { get; set; }
        public int Ricetta_Operazione_Cod { get; set; }

    }

    public class Key_Parametri_Aggiuntivi_Attivita
    {
        public const String list_DataCarenzaRaccolta_x_Impianto = "list_DataCarenzaRaccolta_x_Impianto";
        public const String Opzione_Raccolta_Aggiornamento_Anagrafica = "Opzione_Raccolta_Aggiornamento_Anagrafica";
        public const String Opzione_Abbattimento_Aggiornamento_Anagrafica = "Opzione_Abbattimento_Aggiornamento_Anagrafica";

        public const String lista_SupTrattata_x_DettaglioSemina = "lista_SupTrattata_x_DettaglioSemina";
        public const String Opzione_Semina = "Opzione_Semina";

        public const String lista_Codici_Attivita_x_CentriAziendali = "lista_Codici_Attivita_x_CentriAziendali";
        public const String CaricoMagazzinoAutomatico = "CaricoMagazzinoAutomatico";
        public const String lista_MostraWarning = "lista_MostraWarning";
        public const String mostraWarning_CheckListaAttivita = "mostraWarning_CheckListaAttivita";
        public const String mostraWarning_CheckMagazzino = "mostraWarning_CheckMagazzino";
        public const String mostraWarning_ScriviAttivitaToAgenda = "mostraWarning_ScriviAttivitaToAgenda";
        public const String mostraWarning_CheckDPI = "mostraWarning_CheckDPI";

        public const String sincro_modello4 = "sincro_modello4";

        public const String Formulati_Non_Corretti = "Formulati_Non_Corretti";
        public const String Formulati_Ambigui = "Formulati_Ambigui";
        public const String Avversita_Non_Corrette = "Avversita_Non_Corrette";
        public const String Avversita_Ambigue = "Avversita_Ambigue";
        public const String Avversita_Non_Valorizzate = "Avversita_Non_Valorizzate";

        public const String Id_Visita_Collegata = "Id_Visita_Collegata";

        public const String Fertilizzanti_Non_Corretti = "Fertilizzanti_Non_Corretti";
        public const String Fertilizzanti_Ambigui = "Fertilizzanti_Ambigui";
        public const String Verifica_Compatibilita_Microirrigazione = "Verifica_Compatibilita_Microirrigazione";
        public const string IdTestataVerificaConformita = "IdTestataVerificaConformita";
    }
}
