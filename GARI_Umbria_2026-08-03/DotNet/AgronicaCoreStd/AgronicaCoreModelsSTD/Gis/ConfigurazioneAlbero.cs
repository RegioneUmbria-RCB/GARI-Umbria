using System;

namespace AgronicaCoreModelsSTD.Gis
{
    /// <summary>
    /// Classe parametri configurazione albero
    /// </summary>
    public class ConfigurazioneAlbero
    {
        public enum enum_Contesto
        {
            Generico = 0,
            GIS = 1
        }
        public bool LetturaViaSQLJson { get; set; } = false;
        public bool Flag_Esplodi_Tutto { get; set; } = false;
        public bool Flag_CheckBox { get; set; } = false;
        public bool Flag_Planning { get; set; } = false;
        public bool Flag_Anagrafica { get; set; } = false;
        public bool Flag_Contatti { get; set; } = false;
        public bool Flag_Analisi { get; set; } = false;
        public bool Flag_PianoConcimazione { get; set; } = false;
        public bool Flag_Esercizio { get; set; } = false;
        public bool Flag_ParcoMacchine { get; set; } = false;
        public bool Flag_CatastoAziendale { get; set; } = false;
        public bool Flag_CatastoAppezzamento { get; set; } = false;
        public bool Flag_Fabbricati { get; set; } = false;
        public bool Flag_PortafoglioProdotti { get; set; } = false;
        public bool Flag_Singola_Selezione { get; set; } = true;
        public bool Flag_Appezzamenti_Filtra_Tecnico { get; set; } = true;
        public bool Flag_Agenda { get; set; } = false;
        public int FiltroImpiantiIdTestataTemp { get; set; } = 0;
        public bool ordinaDataUltimoImpianto { get; set; } = false;
        public bool visualizzaRiferimentoAlfanumericoImpianto { get; set; } = false;
        public string TipoOperazioneColturale { get; set; }
        public string Piva { get; set; }
        public string Sa_Cod { get; set; }
        public int Veg_Cod { get; set; } = 0;
        public int Cul_Cod { get; set; }
        public bool Flag_Carica_Primo_Giro { get; set; }
        public DateTime dataInizio { get; set; }
        public DateTime dataFine { get; set; }
        public CheckBoxFlags CheckBoxes { get; set; } = new CheckBoxFlags();
        public string DatiSportelloSementieri { get; set; }
        public DateTime ParametriAgendaData { get; set; }
        public string Elenco_Icone_SpecieVegetali { get; set; }
        public string PivaPadre { get; set; } = "";
        public string Valore_Albero { get; set; }
        public bool FlagModalitaSementieri { get; set; } = false;
        public string GruppoOperazioneColturale { get; set; }
        public bool Flag_Ricette { get; set; } = false;
        public int TipologiaLayer_Cod { get; set; } = 1;
        public enum_Contesto Contesto { get; set; } = enum_Contesto.Generico;
    }

    /// <summary>
    /// Classe parametri configurazione albero e configurazione Gis utente
    /// </summary>
    public class CfgAlbero_CfgGisUtente
    {
        public ConfigurazioneAlbero CfgAlbero { get; set; } = new ConfigurazioneAlbero();
        public ConfigurazioneGisUtente CfgGisUtente { get; set; } = new ConfigurazioneGisUtente();
    }

    /// <summary>
    /// Classe indicativi checkbox per parametri configurazione albero
    /// </summary>
    public class CheckBoxFlags
    {
        public bool Flag_CheckBoxUtente { get; set; } = false;
        public bool Flag_CheckBoxImpresa { get; set; } = false;
        public bool Flag_CheckBoxContatti { get; set; } = false;
        public bool Flag_CheckBoxParcoMacchine { get; set; } = false;
        public bool Flag_CheckBoxCentro { get; set; } = false;
        public bool Flag_CheckBoxCatasto { get; set; } = false;
        public bool Flag_CheckBoxParticella { get; set; } = false;
        public bool Flag_CheckBoxProdotti { get; set; } = false;
        public bool Flag_CheckBoxFabbricati { get; set; } = false;
        public bool Flag_CheckBoxMagazzino { get; set; } = false;
        public bool Flag_CheckBoxGiacenze { get; set; } = false;
        public bool Flag_CheckBoxMovimenti { get; set; } = false;
        public bool Flag_CheckBoxAppezzamento { get; set; } = false;
        public bool Flag_CheckBoxImpianto { get; set; } = false;
        public bool Flag_CheckBoxCampo { get; set; } = false;
        public bool Flag_CheckBoxSerra { get; set; } = false;
        public bool Flag_CheckBoxAnalisi { get; set; } = false;
        public bool Flag_CheckBoxCampioni { get; set; } = false;
        public bool Flag_CheckBoxRicette { get; set; } = false;
    }

}