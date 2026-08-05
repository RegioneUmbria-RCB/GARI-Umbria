import { Injectable } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup } from '@angular/forms';
import { enum_zoomVisualizzazioneTotale } from 'app/GIS/GIS-enum/GIS-zoom';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { CodiciAnagrafeValori, CodiciAnagrafeValoriChiave } from 'app/Model/anagrafiche/CodiciAnagrafeValori';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { enum_PosizioniNodo, enum_TipoNodo } from 'app/Model/TipiEnumerativi';
import { TreeNode } from 'app/Utility/Template/kendo-tree/model';
import { ChiaveAlbero, ConfigurazioneGisUtente, ConfigurazioneGisUtente_enum_TipoRender_ServerSide } from './api.service';
import { separatoreChiaveAlbero } from './utils';
import { TranslocoService } from '@jsverse/transloco';
import { Tile, Mercator } from 'app/GIS/utils/mercator.utils';
import { Observable, of } from 'rxjs';
import { enum_FeatureProperty } from 'app/GIS/GIS-enum/GIS-feature';
import { GiasDialogService } from './gias-dialog.service';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { PolygonInterceptionUtils } from 'app/GIS/services/polygon-interception-utils';
import { GoogleMapUtils } from 'app/GIS/utils/google-map.utils';
import { enum_ClusteringLevel } from 'app/GIS/GIS-enum/GIS-clustering-level';

class CodiciAnagrafe_Occorrenze {
  codice: number;
  occorrenze: number;
}

@Injectable({
  providedIn: 'root'
})
export class FunzioniComuniService {
  private readonly tipiNodoImpianti = [
    enum_TipoNodo.ImpiantoArborea,
    enum_TipoNodo.ImpiantoErbacea,
    enum_TipoNodo.ImpiantoNudo,
    enum_TipoNodo.ImpiantoOrticola,
    enum_TipoNodo.Impianto_Generico
  ];

  constructor(private giasDialogService: GiasDialogService) { }

  roundNumber(num, dec) {
    const result = Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec);
    return result;
  }

  public codiciValidi(codici: CodiciAnagrafeValoriChiave[]): CodiciAnagrafeValoriChiave[] {
    var invalidi = new Array<CodiciAnagrafeValoriChiave>();
    const id_codN = new Array<CodiciAnagrafe_Occorrenze>();
    codici.forEach((el, i, arr) => {
      const occ = id_codN.findIndex((occ_ell) => occ_ell.codice == el.codiceAnagrafe.codice);
      if (occ == -1) {
        id_codN.push({ codice: el.codiceAnagrafe.codice, occorrenze: 1 });
      } else {
        const occObj = id_codN[occ];
        occObj.occorrenze += 1;
        id_codN[occ] = occObj;
      }
    });

    const multipleOcc = id_codN.filter((el) => el.occorrenze > 1);

    multipleOcc.forEach((el) => {
      var inval = invalidi.concat(this.codiciValidiMultipleOcc(codici.filter((el1) => el.codice == el1.codiceAnagrafe.codice)));
    });

    return invalidi;
  }

  public codiciValidiNoDate(codici: CodiciAnagrafeValoriChiave[]): CodiciAnagrafeValoriChiave[] {
    let invalidi = new Array<CodiciAnagrafeValoriChiave>();
    let id_codN = new Array<CodiciAnagrafe_Occorrenze>();
    codici.forEach((el, i, arr) => {
      const occ = id_codN.findIndex((occ_ell) => occ_ell.codice == el.codiceAnagrafe.codice);
      if (occ == -1) {
        id_codN.push({ codice: el.codiceAnagrafe.codice, occorrenze: 1 });
      } else {
        const occObj = id_codN[occ];
        occObj.occorrenze += 1;
        id_codN[occ] = occObj;
      }
    });

    const multipleOcc = id_codN.filter((el) => el.occorrenze > 1);

    multipleOcc.forEach((el) => {
      let codice = codici.find((el1) => el.codice == el1.codiceAnagrafe.codice)
      invalidi = invalidi.concat(codice);
    });

    if (invalidi.length > 0) {
      let messaggio = "I codici ";
      messaggio += invalidi.map((el) => el.codiceAnagrafe.descrizione).join(',');
      messaggio += " sono doppi";
      this.giasDialogService.baseError("Attenzione", messaggio);
    }

    return invalidi;
  }

  private codiciValidiMultipleOcc(codici: CodiciAnagrafeValoriChiave[]): CodiciAnagrafeValoriChiave[] {
    const invalidi = new Array<CodiciAnagrafeValoriChiave>();
    for (let i = 0; i < codici.length; i++) {
      const codiceAttuale = codici[i];
      for (let j = 0; j < codici.length; j++) {
        if (i != j) {
          let valido = true;
          const codiceConfronto = codici[j];
          if (codiceAttuale.validita.inizio >= codiceConfronto.validita.inizio && codiceAttuale.validita.inizio <= codiceConfronto.validita.fine) {
            valido = false;
          }
          if (codiceAttuale.validita.fine >= codiceConfronto.validita.inizio && codiceAttuale.validita.fine <= codiceConfronto.validita.fine) {
            valido = false;
          }
          if (!valido) {
            invalidi.push(codiceAttuale);
            break;
          }
        }
      }
    }
    return invalidi;
  }

  codiciSovrapposti(codici: CodiciAnagrafeValoriChiave[] | CodiciAnagrafeValori[]): boolean {

    for (let i = 0; i < codici.length; i++) {
      let altriCodici = codici.slice();
      altriCodici.splice(i, 1);
      for (let j = 0; j < altriCodici.length; j++) {
        if (codici[i].codiceAnagrafe.codice == altriCodici[j].codiceAnagrafe.codice && this.checkSovrapposizioneIntervalli(codici[i].validita, altriCodici[j].validita)) {
          this.giasDialogService.baseError("Attenzione", "I codici " + codici[i].codiceAnagrafe.descrizione + " (Valore: " + codici[i].valore + ")" + " e " + altriCodici[j].codiceAnagrafe.descrizione + " (Valore: " + altriCodici[j].valore + ")" + " hanno una sovrapposizione nelle loro durate di validità.")
          return true;
        }
      }
    }
    return false;
  }

  checkSovrapposizioneIntervalli(intervallo: IntervalloTemporale, intervalloConfronto: IntervalloTemporale): boolean {
    intervallo?.inizio?.setHours(0, 0, 0, 0);
    intervallo?.fine?.setHours(0, 0, 0, 0);
    intervalloConfronto?.inizio?.setHours(0, 0, 0, 0);
    intervalloConfronto?.fine?.setHours(0, 0, 0, 0);
    return ((intervallo.fine ?? AGRODATAFINE) >= (intervalloConfronto.inizio ?? AGRODATAINIZIO) &&
      (intervallo.inizio ?? AGRODATAINIZIO) <= (intervalloConfronto.fine ?? AGRODATAFINE))
  }

  cloneAbstractControl<T extends AbstractControl>(control: T): T {
    let newControl: T;


    if (control instanceof FormGroup) {
      const formGroup = new FormGroup({}, control.validator, control.asyncValidator);
      const controls = control.controls;


      Object.keys(controls).forEach(key => {
        formGroup.addControl(key, this.cloneAbstractControl(controls[key]), { emitEvent: false });
      });


      newControl = formGroup as any;

    } else if (control instanceof FormArray) {

      const formArray = new FormArray([], control.validator, control.asyncValidator);
      control.controls.forEach(formControl => formArray.push(this.cloneAbstractControl(formControl), { emitEvent: false }));

      newControl = formArray as any;
    } else if (control instanceof FormControl) {
      newControl = new FormControl(control.value, control.validator, control.asyncValidator) as any;
    } else {
      throw new Error('Error: unexpected control value');
    }

    if (control.disabled) {
      newControl.disable({ emitEvent: false });
    }
    return newControl;

  }

  private STRICT_EQUALITY_BROKEN = (a, b) => a === b;

  private STRICT_EQUALITY_NO_NAN = (a, b) => {

    if (typeof a == 'number' && typeof b == 'number' && '' + a == 'NaN' && '' + b == 'NaN')
    // isNaN does not do what you think; see +/-Infinity
    {
      return true;
    } else {
      return a === b;
    }
  };

  deepEquals(a, b, areEqual = this.STRICT_EQUALITY_NO_NAN, setElementsAreEqual = this.STRICT_EQUALITY_NO_NAN) {
    /* compares objects hierarchically using the provided
    notion of equality (defaulting to ===);
    supports Arrays, Objects, Maps, ArrayBuffers */
    if (a instanceof Array && b instanceof Array) {
      return this.arraysEqual(a, b, areEqual);
    }
    if (Object.getPrototypeOf(a) === Object.prototype && Object.getPrototypeOf(b) === Object.prototype) {
      return this.objectsEqual(a, b, areEqual);
    }
    if (a instanceof Map && b instanceof Map) {
      return this.mapsEqual(a, b, areEqual);
    }
    if (a instanceof Set && b instanceof Set) {
      if (setElementsAreEqual === this.STRICT_EQUALITY_NO_NAN) {
        return this.setsEqual(a, b);
      } else {
        throw new Error('Error: set equality by hashing not implemented because cannot guarantee custom notion of equality is transitive without programmer intervention.');
      }
    }
    if ((a instanceof ArrayBuffer || ArrayBuffer.isView(a)) && (b instanceof ArrayBuffer || ArrayBuffer.isView(b))) {
      return this.typedArraysEqual(a, b);
    }
    return areEqual(a, b);  // see note[1] -- IMPORTANT
  }

  private arraysEqual(a, b, areEqual) {
    if (a.length != b.length) {
      return false;
    }
    for (let i = 0; i < a.length; i++) {
      if (!this.deepEquals(a[i], b[i], areEqual)) {
        return false;
      }
    }
    return true;
  }

  private objectsEqual(a, b, areEqual) {
    const aKeys = Object.getOwnPropertyNames(a);
    const bKeys = Object.getOwnPropertyNames(b);
    if (aKeys.length != bKeys.length) {
      return false;
    }
    aKeys.sort();
    bKeys.sort();
    for (let i = 0; i < aKeys.length; i++) {
      if (!areEqual(aKeys[i], bKeys[i])) // keys must be strings
      {
        return false;
      }
    }
    return this.deepEquals(aKeys.map(k => a[k]), aKeys.map(k => b[k]), areEqual);
  }

  private mapsEqual(a, b, areEqual) { // assumes Map's keys use the '===' notion of equality, which is also the assumption of .has and .get methods in the spec; however, Map's values use our notion of the areEqual parameter
    if (a.size != b.size) {
      return false;
    }
    return [...a.keys()].every(k =>
      b.has(k) && this.deepEquals(a.get(k), b.get(k), areEqual)
    );
  }
  private setsEqual(a, b) {
    // see discussion in below rest of StackOverflow answer
    return a.size == b.size && [...a.keys()].every(k =>
      b.has(k)
    );
  }

  private typedArraysEqual(a, b) {
    // we use the obvious notion of equality for binary data
    a = new Uint8Array(a);
    b = new Uint8Array(b);
    if (a.length != b.length) {
      return false;
    }
    for (let i = 0; i < a.length; i++) {
      if (a[i] != b[i]) {
        return false;
      }
    }
    return true;
  }

  public parseURL(url): {
    domain: string;
    host: string;
    path: string;
    protocol: string;
    subdomain: string;
    tld: string;
  } {
    const parsed_url = {} as any;


    if (url == null || url.length == 0) {
      return parsed_url;
    }

    const protocol_i = url.indexOf('://');
    parsed_url.protocol = url.substring(0, protocol_i);

    const remaining_url = url.substring(protocol_i + 3, url.length);
    let domain_i = remaining_url.indexOf('/');
    domain_i = domain_i == -1 ? remaining_url.length - 1 : domain_i;
    parsed_url.domain = remaining_url.substring(0, domain_i);
    parsed_url.path = domain_i == -1 || domain_i + 1 == remaining_url.length ? null : remaining_url.substr(domain_i + 1, remaining_url.length);

    const domain_parts = parsed_url.domain.split('.');
    switch (domain_parts.length) {
      case 2:
        parsed_url.subdomain = null;
        parsed_url.host = domain_parts[0];
        parsed_url.tld = domain_parts[1];
        break;
      case 3:
        parsed_url.subdomain = domain_parts[0];
        parsed_url.host = domain_parts[1];
        parsed_url.tld = domain_parts[2];
        break;
      case 4:
        parsed_url.subdomain = domain_parts[0];
        parsed_url.host = domain_parts[1];
        parsed_url.tld = domain_parts[2] + '.' + domain_parts[3];
        break;
    }

    parsed_url.parent_domain = parsed_url.host + '.' + parsed_url.tld;

    return parsed_url;
  }

  public static convertStringDdMmYyyyToDate(dateIT: string): Date {
    let ddmmyyyy: number[] = dateIT.split(' ')[0].split('/').map(s => Number.parseInt(s));
    return new Date(ddmmyyyy[2], ddmmyyyy[1] - 1, ddmmyyyy[0]);
  }

  //--------------------------------------------------------------------------------
  // Utilità albero
  //--------------------------------------------------------------------------------

  /**
   *
   * @param chiaveAlbero the *complete* tree key to be decomposed.
   * @returns an object contaning the properties of the tree key.
   *
   * @UsageNotes
   * Partendo da una feature:
   * ```
   *  import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
   *  // ...
   *  const partialTreeKey: string = feature.properties.chiavealbero;
   *  const completeTreeKey: string = FunzioniComuniService.chiaveAlberoRidottaToBig(partialTreeKey);
   *  const objChiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(completeTreeKey);
   * ```
   */
  public static scomponiChiaveAlbero(chiaveAlbero: string): ChiaveAlbero {
    let ca: ChiaveAlbero = {};
    let cods: string[] = chiaveAlbero.split(separatoreChiaveAlbero);
    ca.TipoNodo = Number.parseInt(cods[0]);
    ca.Piva = cods[1];
    ca.Sa_Cod = Number.parseInt(cods[2]);
    ca.Campo_Cod = Number.parseInt(cods[3]);
    ca.Appezza = Number.parseInt(cods[4]);
    ca.Id_Imp = Number.parseInt(cods[5]);
    ca.p_Part_Cod = cods[6];
    ca.p_Provincia_Cod = cods[7];
    ca.p_Comune_Cod = cods[8];
    ca.p_Sezione = cods[9];
    ca.p_Foglio = cods[10];
    ca.p_Numero = cods[11];
    ca.p_Subalterno = cods[12];
    ca.Cod_Fiscale = cods[13];
    ca.Fabbricato_Cod = cods[14];
    ca.Prodotto_Cod = cods[15];
    ca.Data_Lavorazione = cods[16];
    ca.Analisi_Certificato_Cod = cods[17];
    ca.Analisi_Testata_Cod = cods[18];
    ca.Analisi_Dettaglio_Cod = cods[19];
    ca.Analisi_Campione_Cod = cods[20];
    ca.PianoConcimazioneTestata_Cod = cods[21];
    ca.Progetto_Cod = cods[22];
    ca.Programmazione_Cod = cods[23];
    ca.Programmazione_Entita_Cod = cods[24];
    ca.Id_Agenda = cods[25];
    ca.PivaPadre = cods[26];
    ca.Ricetta_Cod = cods[27];
    ca.RicettaOperazione_Cod = cods[28];
    return ca;
  }

  public ricomponiChiaveAlbero(objChiaveAlbero: ChiaveAlbero): string {
    let chiaveAlbero = ""
    chiaveAlbero = Object.keys(objChiaveAlbero)
      .map(function (k) { return objChiaveAlbero[k] })
      .join(separatoreChiaveAlbero);
    return chiaveAlbero;
  }

  public isTipoNodoImpianto(tipoNodo: enum_TipoNodo) {
    return this.tipiNodoImpianti.includes(tipoNodo)
  }

  public getNewTreeNode(): TreeNode {
    let nodo = new TreeNode({
      id: '',
      text: '',
      type: '',
      imageUrl: '',
      style: '',
      items: [],
      expanded: false,
      selected: false,
      startDate: AGRODATAINIZIO,
      endDate: AGRODATAFINE
    });
    return nodo;
  };

  public static chiaveAlberoRidottaToBig(chiaveAlberoRidotta: string): string {
    const Utility = require('app/GiasJSLibraries/GIS-js-libraries/Utility');
    return Utility.utility.chiaveAlbero_ridotta_to_big(chiaveAlberoRidotta);
  }

  public static chiaveAlberoBigToRidotta(chiaveAlberoBig: string): string {

    // Funzione originale: ChiaveAlbero_Codifica_x_json_solo_valorizzati

    let chiaveAlberoArray: string[] = chiaveAlberoBig.split(separatoreChiaveAlbero);
    const objChiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(chiaveAlberoBig);
    let chiaveAlberoRidotta: string = "";

    chiaveAlberoArray.forEach((elementoChiaveAlbero, posizione) => {
      switch (true) {

        // Il tipo nodo va sempre aggiunto
        case posizione === enum_PosizioniNodo.TipoNodo:
          chiaveAlberoRidotta += elementoChiaveAlbero.toString();
          break;

        // Il campo va sempre aggiunto se è un appezzamento
        case posizione === enum_PosizioniNodo.Campo_Cod &&
          objChiaveAlbero.TipoNodo === enum_TipoNodo.Appezzamento:
          chiaveAlberoRidotta += FunzioniComuniService.stringaElementoChiaveAlbero(elementoChiaveAlbero);
          break;

        // La piva padre va sempre aggiunta
        case posizione === enum_PosizioniNodo.PivaPadre:
          chiaveAlberoRidotta += FunzioniComuniService.stringaElementoChiaveAlbero(elementoChiaveAlbero);
          break;

        // In tutti gli altri casi aggiungo l'elemento solo se valorizzato
        default:
          if (elementoChiaveAlbero !== '0') {
            chiaveAlberoRidotta += FunzioniComuniService.stringaElementoChiaveAlbero(elementoChiaveAlbero);
          }
          break;
      }
    });

    return chiaveAlberoRidotta;

  }

  public getIdFeature(pivaSuperUser: string, entitaCod: string, tipologiaLayer: string, layer: string) {
    return `${pivaSuperUser}|${entitaCod}|${tipologiaLayer}|${layer}`;
  }

  private static stringaElementoChiaveAlbero(elementoChiaveAlbero: any): string {
    return separatoreChiaveAlbero + elementoChiaveAlbero.toString();
  }

  public static getStandardConfigurazioneGisUtente(config: ConfigurazioneGisUtente | null): ConfigurazioneGisUtente {
    return {
      ckMostraOperazioniAgenda: config?.ckMostraOperazioniAgenda ?? false,
      ckMostraPlanning: config?.ckMostraPlanning ?? false,
      ckMostraRicette: config?.ckMostraRicette ?? false,
      ckMostraFabbricati: config?.ckMostraFabbricati ?? false,
      chkMostraAnalisi: config?.chkMostraAnalisi ?? false,
      chkMostraAnagrafica: config?.chkMostraAnagrafica ?? false,
      chkMostraCatasto: config?.chkMostraCatasto ?? false,
      chkMostraCatastoAppezzamento: config?.chkMostraCatastoAppezzamento ?? false,
      chkMostraHeatmap: config?.chkMostraHeatmap ?? false,
      ckGrigliaTiles_Sviluppo: config?.ckGrigliaTiles_Sviluppo ?? false,
      ckViewModal: config?.ckViewModal ?? false,
      SistemaDiRiferimentoPredefinito: config?.SistemaDiRiferimentoPredefinito ?? 0,
      GruppoOperazioneColturale: config?.GruppoOperazioneColturale ?? "",
      srvGisTipoRender_ServerSide: config?.srvGisTipoRender_ServerSide ?? ConfigurazioneGisUtente_enum_TipoRender_ServerSide.Completa,
      ckGestioneAnalisiMappeLegacy: config?.ckGestioneAnalisiMappeLegacy ?? false,
      TipoOperazioneColturale: config?.TipoOperazioneColturale ?? "",
      ckAvversitaUsaPuntoInterno: config?.ckAvversitaUsaPuntoInterno ?? false,
      ckAvversitaPuntoPoligono: config?.ckAvversitaPuntoPoligono ?? false,
      iAutoZoomSuVisualizzazioneTotale: config?.iAutoZoomSuVisualizzazioneTotale ?? enum_zoomVisualizzazioneTotale.no,
      LivelloClusterizzazione: config?.LivelloClusterizzazione ?? enum_ClusteringLevel.default,
    } as ConfigurazioneGisUtente;
  }

  public static getResponseError(error: any, translocoService: TranslocoService, defaultMessage: string = 'ErroreSalvataggio'): string {
    let result = null;
    try {
      result = JSON.parse(error?.response)?.Errore;
    } catch {
      result = translocoService.translate(defaultMessage);
    }

    if (result == null || result == '') {
      result = translocoService.translate(defaultMessage);
    }

    return result;
  }

  public static computeCurrentlyVisibleTiles(gMap: google.maps.Map): Tile[] {
    const zoom = gMap.getZoom();
    const ne = gMap.getBounds().getNorthEast();
    const sw = gMap.getBounds().getSouthWest();

    const neTile = Mercator.getTileAtLatLng(ne, zoom);
    const swTile = Mercator.getTileAtLatLng(sw, zoom);

    const result: Tile[] = [];
    for (let x = swTile.x; x <= neTile.x; x++) {
      for (let y = neTile.y; y <= swTile.y; y++) {
        result.push({ x, y, zoom } as Tile);
      }
    }

    return result;
  }

  public static getCurrentlyVisibleTiles(gMap: google.maps.Map): Observable<Tile[]> {
    return of(FunzioniComuniService.computeCurrentlyVisibleTiles(gMap));
  }

  public static getVisibleFeaturesInLayers(gData: google.maps.Data, validLayers: number[], gMap: google.maps.Map): google.maps.Data.Polygon[] {
    const result: google.maps.Data.Polygon[] = [];

    gData.forEach(feature => {
      const isValidLayer = validLayers.includes(+feature.getProperty(enum_FeatureProperty.layer));
      if (!isValidLayer) {
        return;
      }

      const isVisible = GoogleMapUtils.isPolygonInViewport(gMap, feature.getGeometry() as google.maps.Data.Polygon);
      if (!isVisible) {
        return;
      }

      result.push(feature.getGeometry() as google.maps.Data.Polygon);
    });

    return result;
  }

  public static isCustomLayer(id: number): boolean {
    return id >= 1_000_000;
  }

  public static getTilesWithFeatures(geometries: google.maps.Data.Geometry[], gMap: google.maps.Map): Tile[] {
    const result: Tile[] = [];

    const polygons: google.maps.Data.Polygon[] = [];
    geometries.forEach(geometry => {
      const polygon = geometry as google.maps.Data.Polygon;
      if (GoogleMapUtils.isPolygonInViewport(gMap, polygon)) {
        polygons.push(polygon);
      }
    });

    const tiles = FunzioniComuniService.computeCurrentlyVisibleTiles(gMap);
    for (const tile of tiles) {
      for (const polygon of polygons) {
        if (PolygonInterceptionUtils.getPointPolygonIntersection(polygon, new google.maps.Point(tile.x, tile.y), tile.zoom).length > 0) {
          result.push(tile);
          break;
        }
      }
    }

    return result;
  }

  public static getPointsDistance(pos1: google.maps.Point, pos2: google.maps.Point): number {
    return Math.sqrt((Math.pow((pos1.x - pos2.x), 2) + Math.pow((pos1.y - pos2.y), 2)));
  }

  public static areDatesEqual(date1: Date, date2: Date): boolean {
    return date1.getFullYear() == date2.getFullYear() && date1.getMonth() == date2.getMonth() && date1.getDate() == date2.getDate();
  }

  public getOrigins(): string {
    if (window.location.origin.indexOf("localhost") > -1) {
      return '*'
    } else {
      return window.location.origin
    }
  }


}

export const DEFAULT_DROPDOWN_FILTER_SETTINGS: DropDownFilterSettings = {
  caseSensitive: false,
  operator: "contains",
};
