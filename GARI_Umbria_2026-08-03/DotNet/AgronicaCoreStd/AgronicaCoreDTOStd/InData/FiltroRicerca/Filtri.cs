using AgronicaCoreModelsSTD.anagrafiche;
using OutData.Kendo;
using System;
using System.Collections.Generic;
using System.Data;

namespace AgronicaCoreDTOStd.InData.FiltroRicerca
{
    public class CriteriRicerca_IN
    {
        public int TipoMostra { get; set; }
        public bool OnLoad { get; set; }
        public int Id_Budget { get; set; }
        public FiltriAziende FiltriAziende { get; set; }
        public FiltriCentriAziendali FiltriCentriAziendali { get; set; }
        public FiltriCampi FiltriCampi { get; set; }
        public FiltriPianoColturale FiltriPianoColturale { get; set; }
        public FiltriMovimenti FiltriMovimenti { get; set; }
        public FiltriTemporali FiltriTemporali { get; set; }
        public FiltriServizi FiltriServizi { get; set; }
        public FiltriGIS FiltriGIS { get; set; }
        public FiltriCatasto FiltriCatasto { get; set; }
        public CaricaDatiAggiuntivi CaricaDati { get; set; } = new CaricaDatiAggiuntivi();
    }

    public class FiltriAziende
    {
        public List<string> ImpreseReferenti { get; set; }

        public string RagioneSociale { get; set; }
        public string Piva { get; set; }
        public List<int> TipiImpresa { get; set; }
        public string CUAA { get; set; }

        /// <summary>
        /// null = nessun filtro; false = nodi/padri (GerarchiaImprese.Foglia = 0);
        /// true = foglie (GerarchiaImprese.Foglia = 1). Equivale al campo 'gerarchia' della vecchia API VB.
        /// </summary>
        public bool? EstraiSoloFigli { get; set; }

        public List<string> Zone { get; set; }
        /// <summary>
        /// Enum_FiltroOperatoreLogico_FiltroRicerca
        /// </summary>
        public int OperatoreLogicoZone { get; set; }

        public List<string> Stati { get; set; }
        public List<string> Regioni { get; set; }
        public List<string> Province { get; set; }
        public List<string> Comuni { get; set; }
    }

    public class FiltriCentriAziendali
    {
        public string CentroAziendale { get; set; }

        public List<string> Stati { get; set; }
        public List<string> Regioni { get; set; }
        public List<string> Province { get; set; }
        public List<string> Comuni { get; set; }
    }

    public class FiltriCampi
    {
        public List<int> GruppoVegetale { get; set; }
        public List<int> Specie { get; set; }
    }

    public class FiltriPianoColturale
    {
        public List<int> UtilizzoTerreno { get; set; } // == Metodo Produzione (id_cod 1018)

        /// <summary>
        /// Enum_FiltroDestinazioneUso_FiltroRicerca
        /// </summary>
        public int FiltroDestinazioneUso { get; set; }

        public List<int> DestinazioniUso { get; set; }

        public List<int> GruppoVegetale { get; set; }
        public List<int> Specie { get; set; }
        public List<int> TipologiaVarietale { get; set; }
        public List<int> Varieta { get; set; }

        public string Lotto { get; set; }
        public string Progetto { get; set; }

        public List<int> ContributiACA { get; set; }
    }

    public class FiltriMovimenti
    {
        /// <summary>
        /// Enum_FiltroOperazioniSelezionate_FiltroRicerca
        /// </summary>
        public int FiltroOperazioni { get; set; }
        public List<int> GruppoOperazioni { get; set; }
        public List<int> Operazioni { get; set; }
        public IntervalloTemporale DataMovimento { get; set; }
    }

    public class FiltriTemporali
    {
        public List<FiltroTemporale> FiltriData { get; set; }
        /// <summary>
        /// Enum_FiltroOperatoreLogico_FiltroRicerca
        /// </summary>
        public int OperatoreLogicoFiltriTemporali { get; set; }
    }

    public class FiltriServizi
    {
        public int Servizio { get; set; }
        public List<int> StatiPratica { get; set; }
        public DateTime Data { get; set; }
    }

    public class FiltriGIS
    {
        /// <summary>
        /// Enum_FiltroPoligoni_FiltroRicerca
        /// </summary>
        public int FiltroPoligoni { get; set; }
        public List<string> Anomalie { get; set; }
    }

    public class FiltriCatasto
    {
        /// <summary>
        /// Enum_FiltroRipartoCatasto_FiltroRicerca
        /// </summary>
        public int FiltroRipartoCatastale { get; set; }
    }

    public class FiltroData : IntervalloTemporale
    {
        /// <summary>
        /// 0 = Data Successiva A ...
        /// 1 = Data Precendete A ...
        /// </summary>
        public int TipoFiltroInizio { get; set; }

        /// <summary>
        /// 0 = Data Successiva A ...
        /// 1 = Data Precendete A ...
        /// </summary>
        public int TipoFiltroFine { get; set; }
    }

    public class FiltroTemporale
    {
        public int Entita { get; set; }
        public int ColonnaData { get; set; }
        public int TipoConfronto { get; set; }
        public int ModalitaFiltroData { get; set; }
        public IntervalloTemporale Date { get; set; }

    }

    public class CaricaDatiAggiuntivi
    {
        public bool ImpreseReferenti { get; set; }
        public DatiIscrizioneLibroSoci DatiIscrizioneLibroSoci { get; set; }
        public bool LegaleRappresentante { get; set; }

        public bool IndirizzoAzienda { get; set; }
        public bool IndirizzoCentroAziendale { get; set; }
        public bool IndirizziPianoColturale { get; set; }

        public bool CatastoCentroAziendale { get; set; }
        public bool CatastoAppezzamento { get; set; }
        public bool CatastoCampo { get; set; }

        public bool GISImpianto { get; set; }

        public bool ContributiACA { get; set; }

        public bool Servizi { get; set; }

        /// <summary>
        /// Quando true, aggiunge alle aziende le colonne Foglia, Livello e Padre dalla tabella GerarchiaImprese.
        /// Necessario per l'endpoint LeggiImprese_APP.
        /// </summary>
        public bool DatiGerarchia { get; set; }

        public List<CodiceAnagrafeBase> CodiciAzienda { get; set; } = new List<CodiceAnagrafeBase>();
        public List<CodiceAnagrafeBase> CodiciCentroAziendale { get; set; } = new List<CodiceAnagrafeBase>();
        public List<CodiceAnagrafeBase> CodiciCampo { get; set; } = new List<CodiceAnagrafeBase>();
        public List<CodiceAnagrafeBase> CodiciAppezzamento { get; set; } = new List<CodiceAnagrafeBase>();
        public List<CodiceAnagrafeBase> CodiciImpianto { get; set; } = new List<CodiceAnagrafeBase>();
        public List<CodiceAnagrafeBase> CodiciEsercizio { get; set; } = new List<CodiceAnagrafeBase>();
        public List<CodiceAnagrafeBase> CodiciFabbricato { get; set; } = new List<CodiceAnagrafeBase>();
    }

    public class CriteriRicercaExtended : CriteriRicerca_IN
    {
        public bool CheckVisibilitaAziende { get; set; }
        public bool CheckVisibilitaCentriAziendali { get; set; }
        public decimal FattoreConversione { get; set; } = 1;

        /// <summary>
        /// Verifica se il livello del database è >= 130 (usato per string_split, introdotto con sql2016)
        /// </summary>
        public bool SQLCompatibility { get; set; }
        public bool USE_FORCE_LEGACY_CARDINALITY_ESTIMATION { get; set; }

        /// <summary>
        /// Chiave NumeroMassimoRigheEstraibiliFiltroRicerca, letto a scalare Server (default chiave non dichiarata) > SuperServer (default 100K) 
        /// </summary>
        public int selectTopRows { get; set; }

    }

    public class DatiIscrizioneLibroSoci
    {
        public bool NumeroIscrizione { get; set; }
        public bool DataIscrizione { get; set; }
    }
}
