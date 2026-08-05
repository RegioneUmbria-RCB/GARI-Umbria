import {Observable, of} from 'rxjs';
import {
  ConfigurazioneAlbero_enum_Contesto, ConfigurazioneGisUtente_enum_TipoRender_ServerSide,
  Enum_TipoDebug,
  RispostaStandard_1OfCfgAlbero_CfgGisUtente,
  RispostaStandard_1OfConfigurazioniGisGenerali,
  RispostaStandard_1OfElencoTipologieLayer
} from '../../api.service';

export class GISUtility {
  public static defaultGisAggiornaElencoTipologie(): Observable<RispostaStandard_1OfElencoTipologieLayer> {

    let response: RispostaStandard_1OfElencoTipologieLayer = {
      Compressa: false,
      Errore: "",
      ErroriGias: [],
      ParametroDue: false,
      RispostaCompressa: null,
      RispostaConferma: false,
      RispostaOK: true,
      RispostaStringa: {
        ListaTipologieLayer: [
          {
            FeatureTypeId: "5",
            FlagAmministrazione: "0",
            FlagCancellazione: "1",
            FlagInformazioni: "0",
            FlagInserimento: "1",
            FlagModifica: "1",
            MostraDescrizioneAssociata: "1",
            RaggruppaDescrizioneAssociata: "1",
            TipoNodoAlberoAnagrafe: "0",
            colore_1: "5d54bd",
            colore_2: "",
            flagattivo: "1",
            flagvisibile: "1",
            icona16: "Impianto16.png",
            icona32: "Impianto32.png",
            id: "19",
            nome: "IMPIANTI",
            tiles: [],
            trasparenza: "0,8",
            varianza: "1",
            zindex: "2"
          }
        ]
      },
      Sessione: true,
      TipoDebug: Enum_TipoDebug.Off,
      opzioniWatable: {PrefissoNomeFileExport: '', nomeVarDtInSession: ''}
    };

    return  of(response);
  }

  public static defaultGisCfgGISLeggiGenerali(): Observable<RispostaStandard_1OfConfigurazioniGisGenerali> {
    let response: RispostaStandard_1OfConfigurazioniGisGenerali = {
      RispostaCompressa: null,
      ErroriGias: [],
      TipoDebug: Enum_TipoDebug.Off,
      Compressa: false,
      Sessione: true,
      RispostaOK: true,
      RispostaConferma: false,
      RispostaStringa: {
        AnnataAgrariaInizio: new Date(new Date().getFullYear(), 0, 1),
        AnnataAgrariaFine: new Date(new Date().getFullYear(), 11, 31),
        DemoGisAttivo: true,
        ProseccoGisAttivo: false,
        WmsAttivo: true,
        Gis_Global_ws_mappe_2013_basePath: "https://agrosat.agronica.it/ws_mappe_2013/",
        RichiediPercorsiInizializza: false,
        srvGisParamCartograficiInizializza: {BBox: null, BBox_NE_SW_LatLng: null, Center: null},
        filtroneImpostato: false,
        Piva: null,
        IDTestataTemp: 0,
        GIS_cmbElementoGrafico_GenerazionePoligoni_DefaultValue: "3",
        CiSonoVecchiDatiNonImportati: false,
        AbilitaPF: true,
        Codice_Fiscale_Tecnico: "",
        sFinestraTemporale_GIS_Inizio: "",
        sFinestraTemporale_GIS_Fine: "",
        MostraNuovoDisegno: true,
        MostraPulsanteSalva: true,
        MostraElimina: true,
        MostraEsporta: true,
        MostraImporta: true,
        MostraAB: true,
        permessoAnalisiMeteo: false,
        permessoVisite: true,
        permessoCatasto: true,
        permessoPrecisionFarming: true,
        permessoEsportaDati: true,
        permessoBufferZone: true,
      },
      ParametroDue: false,
      Errore: "",
      opzioniWatable: {PrefissoNomeFileExport: "", nomeVarDtInSession: ""}
    }

    return of(response);
  }

  private static createDefaultGisConfiguraAlbero(): RispostaStandard_1OfCfgAlbero_CfgGisUtente {
    let response: RispostaStandard_1OfCfgAlbero_CfgGisUtente = {
      RispostaCompressa: null,
      ErroriGias: [],
      TipoDebug: Enum_TipoDebug.Off,
      Compressa: false,
      Sessione: true,
      RispostaOK: true,
      RispostaConferma: false,
      RispostaStringa: {
        CfgAlbero: {
          LetturaViaSQLJson: false,
          Flag_Esplodi_Tutto: true,
          Flag_CheckBox: false,
          Flag_Planning: true,
          Flag_Anagrafica: true,
          Flag_Contatti: false,
          Flag_Analisi: true,
          Flag_PianoConcimazione: false,
          Flag_Esercizio: false,
          Flag_ParcoMacchine: false,
          Flag_CatastoAziendale: true,
          Flag_CatastoAppezzamento: false,
          Flag_Fabbricati: false,
          Flag_PortafoglioProdotti: false,
          Flag_Singola_Selezione: true,
          Flag_Appezzamenti_Filtra_Tecnico: false,
          Flag_Agenda: false,
          FiltroImpiantiIdTestataTemp: 0,
          ordinaDataUltimoImpianto: false,
          visualizzaRiferimentoAlfanumericoImpianto: false,
          TipoOperazioneColturale: "",
          Piva: null,
          Sa_Cod: null,
          Veg_Cod: 0,
          Cul_Cod: 0,
          Flag_Carica_Primo_Giro: false,
          dataInizio: new Date(1900, 0, 1),
          dataFine: new Date(2100, 11, 31),
          CheckBoxes: {
            Flag_CheckBoxUtente: false,
            Flag_CheckBoxImpresa: false,
            Flag_CheckBoxContatti: false,
            Flag_CheckBoxParcoMacchine: false,
            Flag_CheckBoxCentro: false,
            Flag_CheckBoxCatasto: false,
            Flag_CheckBoxParticella: false,
            Flag_CheckBoxProdotti: false,
            Flag_CheckBoxFabbricati: false,
            Flag_CheckBoxMagazzino: false,
            Flag_CheckBoxGiacenze: false,
            Flag_CheckBoxMovimenti: false,
            Flag_CheckBoxAppezzamento: false,
            Flag_CheckBoxImpianto: false,
            Flag_CheckBoxCampo: false,
            Flag_CheckBoxSerra: false,
            Flag_CheckBoxAnalisi: false,
            Flag_CheckBoxCampioni: false,
            Flag_CheckBoxRicette: false
          },
          DatiSportelloSementieri: null,
          ParametriAgendaData: new Date(1900, 0, 1),
          Elenco_Icone_SpecieVegetali: null,
          PivaPadre: "",
          Valore_Albero: null,
          FlagModalitaSementieri: false,
          GruppoOperazioneColturale: "",
          Flag_Ricette: false,
          TipologiaLayer_Cod: 1,
          Contesto: ConfigurazioneAlbero_enum_Contesto.GIS
        },
        CfgGisUtente: {
          ckMostraOperazioniAgenda: true,
          ckMostraPlanning: true,
          ckMostraRicette: false,
          ckMostraFabbricati: false,
          chkMostraAnalisi: true,
          chkMostraAnagrafica: true,
          chkMostraCatasto: true,
          chkMostraCatastoAppezzamento: false,
          chkMostraHeatmap: false,
          ckGrigliaTiles_Sviluppo: false,
          ckViewModal: true,
          srvGisTipoRender_ServerSide: ConfigurazioneGisUtente_enum_TipoRender_ServerSide.Completa,
          SistemaDiRiferimentoPredefinito: 0,
          GruppoOperazioneColturale: "",
          TipoOperazioneColturale: "",
          iAutoZoomSuVisualizzazioneTotale: 11,
          LivelloClusterizzazione: 15,
          ckAvversitaUsaPuntoInterno: false,
          ckAvversitaPuntoPoligono: true,
          ckGestioneAnalisiMappeLegacy: false,
        }
      },
      ParametroDue: false,
      Errore: "",
      opzioniWatable: {PrefissoNomeFileExport: "", nomeVarDtInSession: ""}
    }
    return response;
  } 

  public static defaultGisConfiguraAlbero(): Observable<RispostaStandard_1OfCfgAlbero_CfgGisUtente> {
    let response = GISUtility.createDefaultGisConfiguraAlbero();
    return of(response);
  }

  public static gisConfiguraAlberoHeatMapOn(): Observable<RispostaStandard_1OfCfgAlbero_CfgGisUtente> {
    let response = GISUtility.createDefaultGisConfiguraAlbero();
    response.RispostaStringa.CfgGisUtente.chkMostraHeatmap = true;
    return of(response);
  }
}
