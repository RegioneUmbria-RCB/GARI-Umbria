using System;

namespace AgronicaCoreModelsSTD.Gis
{
    /// <summary>
    /// Classe parametri configurazione Gis per utente
    /// </summary>
    public class ConfigurazioneGisUtente
    {

        /// <summary>
        /// Tipo di lettura dei dati SQL Spatial, se completa non si applicano filtri basati su Bounding Box, se parziale sì
        /// </summary>
        /// <example>0</example>
        public enum enum_TipoRender_ServerSide
        {
            Completa = 0,
            Parziale = 1
        }

        /// <summary>
        /// Visualizza l'anteprima del menu agenda in apposito riquadro
        /// </summary>
        /// <example>false</example>
        public bool ckMostraOperazioniAgenda { get; set; } = false;

        /// <summary>
        /// Visuaizza i dati di planning (Nell'albero anagrafica o altrove)
        /// </summary>
        /// <example>false</example>
        public bool ckMostraPlanning { get; set; } = false;

        /// <summary>
        /// Visualizza di dati delle ricette (Nell'albero anagrafica o altrove)
        /// </summary>
        /// <example>false</example>
        public bool ckMostraRicette { get; set; } = false;

        /// <summary>
        /// Visualizza di dati dei fabbricati (nell'albero anagrafica o altrove)
        /// </summary>
        /// <example>false</example>
        public bool ckMostraFabbricati { get; set; } = false;

        /// <summary>
        /// Visualizza i dati delle analisi (Nell'albero anagrafica o altrove)
        /// </summary>
        /// <example>false</example>
        public bool chkMostraAnalisi { get; set; } = false;

        /// <summary>
        /// Visualizza i dati di anagrafica (Nell'albero anagrafica o altrove)
        /// </summary>
        /// <example>true</example>
        public bool chkMostraAnagrafica { get; set; } = false;

        /// <summary>
        /// Visualizza i dati catastali (Nell'albero anagrafica o altrove)
        /// </summary>
        /// <example>true</example>
        public bool chkMostraCatasto { get; set; } = false;

        /// <summary>
        /// Visualizza i dati catastali dettagliati per ciascun appezzamento (Nell'albero anagrafica o altrove)
        /// </summary>
        /// <example>false</example>
        public bool chkMostraCatastoAppezzamento { get; set; } = false;
        
        /// <summary>
        /// Mostra una griglia con i tiles di goole, utiili per lo sviluppo
        /// </summary>
        /// <example>false</example>
        public bool ckGrigliaTiles_Sviluppo { get; set; } = false;

        /// <summary>
        /// Visualizza le finestre in maniera modale, in alternativa esegue una redirezione alla pagina abbandonando il GIS
        /// </summary>
        /// <example>true</example>
        public bool ckViewModal { get; set; } = false;

        /// <summary>
        /// Tipo di lettura dei dati SQL Spatial, se completa non si applicano filtri basati su Bounding Box, se parziale sì
        /// </summary>
        /// <example>0</example>
        public enum_TipoRender_ServerSide srvGisTipoRender_ServerSide { get; set; } = enum_TipoRender_ServerSide.Completa;
        
        /// <summary>
        /// Sistema di riferimento predefinito per popolare le caselle a discesa
        /// </summary>
        /// <example>1</example>
        public int SistemaDiRiferimentoPredefinito { get; set; } = 0;
        
        /// <summary>
        /// Gruppo Operazione colturale (per filtro)
        /// </summary>
        /// <example>0</example>
        public string GruppoOperazioneColturale { get; set; }

        /// <summary>
        /// Tipo Operazione Colturale (per filtro)
        /// </summary>
        /// <example>0</example>
        public string TipoOperazioneColturale { get; set; }

        /// <summary>
        /// Livello auto zoom per visualizzazione totale.
        /// Valori ammessi:
        /// -1 = auto zoom disabilitato,
        /// 8-16 = specifico livello di auto zoom.
        /// </summary>
        /// <example>15</example>
        public int iAutoZoomSuVisualizzazioneTotale { get; set; }
        
        /// <summary>
        /// Flag per indicare l'utilizzo di un punto interno al poligono in assenza di rilievo puntuale
        /// </summary>
        /// <example>true</example>
        public bool ckAvversitaUsaPuntoInterno { get; set; } = true;

        /// <summary>
        /// Flag per consentire l'utilizzo di un punto interno al posto dell'intero poligono
        /// </summary>
        /// <example>true</example>
        public bool ckAvversitaPuntoPoligono { get; set; } = true;

        /// <summary>
        /// Flag per consentire l'utilizzo di un punto interno al posto dell'intero poligono
        /// </summary>
        /// <example>true</example>
        public bool ckGestioneAnalisiMappeLegacy { get; set; } = false;

        /// <summary>
        /// Livello definito per inizio funzione di clusterizzazione poligoni
        /// </summary>
        /// <example>15</example>
        public int LivelloClusterizzazione { get; set; } = 15;

        /// <summary>
        /// Flag per visualizzare i cluster sulla mappa in versione heatmap
        /// </summary>
        /// <example>false</example>
        public bool chkMostraHeatmap { get; set; } = false;
    }

    /// <summary>
    /// Configurazione di inizializzazione
    /// </summary>
    public class ConfigurazioniGisGenerali
    {
        /// <summary>
        /// Inizio Annata Agraria
        /// </summary>
        /// <example>2022-01-01T00:00:00.000</example>
        public DateTime AnnataAgrariaInizio { get; set; }

        /// <summary>
        /// Fine Annata Agraria
        /// </summary>
        /// <example>2022-12-31T00:00:00.000</example>
        public DateTime AnnataAgrariaFine { get; set; }
        
        /// <summary>
        /// Attiva modalità demo (obsoleto)
        /// </summary>
        /// <example>false</example>
        public bool DemoGisAttivo { get; set; } = false;

        /// <summary>
        /// attiva modalita prosecco (obsoleto)
        /// </summary>
        /// <example>false</example>
        public bool ProseccoGisAttivo { get; set; } = false;

        /// <summary>
        /// l'utente ha i permessi per la visualizzazione e gestione dei layer WMS
        /// </summary>
        /// <example>true</example>
        public bool WmsAttivo { get; set; } = false;

        /// <summary>
        /// base path del servizio di ritaglio
        /// </summary>
        /// <example>https://agrosat.netagronica.it/WS_Mappe_2013/</example>
        public string Gis_Global_ws_mappe_2013_basePath { get; set; }

        /// <summary>
        /// Richiede avvio della modalità di visualizzazione dei percorsi
        /// </summary>
        /// <example>false</example>
        public bool RichiediPercorsiInizializza { get; set; } = false;

        /// <summary>
        /// Parametri cartografici per inizializzazione
        /// </summary>
        public srvGisParametriCartograficiInizializzazione srvGisParamCartograficiInizializza { get; set; } = new srvGisParametriCartograficiInizializzazione();
        
        /// <summary>
        /// viene impostato un filtro iniziale
        /// </summary>
        /// <example>false</example>
        public bool filtroneImpostato { get; set; } = false;

        /// <summary>
        /// partita iva impresa selezionata
        /// </summary>
        /// <example>01704430519</example>
        public string Piva { get; set; }

        /// <summary>
        /// ID del filtro impostato (viene passato come parametro)
        /// </summary>
        /// <example>0</example>
        public int IDTestataTemp { get; set; }

        /// <summary>
        /// Indica il valore predefinito per la procedura di memorizzazione dei poligoni su layer impianti ed appezzamenti (
        /// </summary>
        public string GIS_cmbElementoGrafico_GenerazionePoligoni_DefaultValue { get; set; }
        
        //InizializzaProprietajs

        /// <summary>
        /// Indica se ci sono dati provenienti dal GIAS PALM non ancora importati (obsoleto)
        /// </summary>
        /// <example>false</example>
        public bool CiSonoVecchiDatiNonImportati { get; set; } = false;

        /// <summary>
        /// Indica se l'utente dispone dei permessi di gestione della precision farming
        /// </summary>
        /// <example>true</example>
        public bool AbilitaPF { get; set; } = false;

        /// <summary>
        /// Codice fiscale tecnico, utilizzato in ambito progetto sementi
        /// </summary>
        /// <example>-1</example>
        public string Codice_Fiscale_Tecnico { get; set; }

        /// <summary>
        /// Data inizio predefinita per il filtro temporale
        /// </summary>
        /// <example>2022-01-01T00:00:00.000</example>
        public string sFinestraTemporale_GIS_Inizio { get; set; }

        /// <summary>
        /// Data fine predefinita per il filtro temporale
        /// </summary>
        /// <example>2022-01-01T00:00:00.000</example>
        public string sFinestraTemporale_GIS_Fine { get; set; }

        //ImpostaVisibilitaPulsanti

        /// <summary>
        /// Nuovo disegno
        /// </summary>
        /// <example>true</example>
        public bool MostraNuovoDisegno { get; set; } = false;

        /// <summary>
        /// Modifica disegno
        /// </summary>
        /// <example>true</example>
        public bool MostraPulsanteSalva { get; set; } = false;

        /// <summary>
        /// Elimina disegno
        /// </summary>
        /// <example>true</example>
        public bool MostraElimina { get; set; } = false;

        /// <summary>
        /// Esporta
        /// </summary>
        /// <example>true</example>
        public bool MostraEsporta { get; set; } = false;

        /// <summary>
        /// Importa
        /// </summary>
        /// <example>true</example>
        public bool MostraImporta { get; set; } = false;

        /// <summary>
        /// Strumento di disegno linee guida
        /// </summary>
        /// <example>true</example>
        public bool MostraAB { get; set; } = false;

        /// <summary>
        /// l'utente dispone dei permessi di analisi dati meteo
        /// </summary>
        /// <example>true</example>
        public bool permessoAnalisiMeteo { get; set; } = false;

        /// <summary>
        /// l'utente dispone dei permessi di gestion visite
        /// </summary>
        /// <example>true</example>
        public bool permessoVisite { get; set; } = false;


        //MostraNascondiPulsanti


        /// <summary>
        /// l'utente dispone dei permessi di gestione catasto
        /// </summary>
        /// <example>true</example>
        public bool permessoCatasto { get; set; } = false;


        /// <summary>
        /// l'utente dispone dei permessi di gestione strumenti precision farming
        /// </summary>
        /// <example>true</example>
        public bool permessoPrecisionFarming { get; set; } = false;


        /// <summary>
        /// l'utente dispone dei permessi di esportazione dati
        /// </summary>
        /// <example>true</example>
        public bool permessoEsportaDati { get; set; } = false;


        /// <summary>
        /// l'utente dispone dei permessi di gestione delle buffer zone
        /// </summary>
        /// <example>true</example>
        public bool permessoBufferZone { get; set; } = false;
    }

    /// <summary>
    /// Parametri di inizializzazione cartografici
    /// </summary>
    public class srvGisParametriCartograficiInizializzazione
    {
        /// <summary>
        /// Stringa che rappresenta un bouding box
        /// </summary>
        /// <example></example>
        public string BBox { get; set; }

        /// <summary>
        /// coordinate NE e SW del bounding box
        /// </summary>
        /// <example></example>
        public string BBox_NE_SW_LatLng { get; set; }

        /// <summary>
        /// coordinate centroide del bouindg box
        /// </summary>
        /// <example></example>
        public string Center { get; set; }
    }

}
