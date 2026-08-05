import { GuidaValoreImpostazione } from 'gias-ui-kit';
import { ImpostazioniFormItem } from 'gias-ui-kit';
import { Impostazione } from 'gias-ui-kit';
import { enum_TipoControllo } from 'gias-ui-kit';

export class ImpostazioniGIS {
  public ckMostraOperazioniAgenda: boolean = true;
  public ckMostraPlanning: boolean = true;
  public ckMostraRicette: boolean = true;
  public chkMostraAnalisi: boolean = true;
  public chkMostraAnagrafica: boolean = true;
  public chkMostraCatasto: boolean = true;
  public chkMostraCatastoAppezzamento: boolean = true;
  public chkMostraHeatmap: boolean = false;
  public ckGrigliaTiles_Sviluppo: boolean = false;
  public ckViewModal: boolean = false;
  public ckAvversitaUsaPuntoInterno: boolean = false;
  public ckAvversitaPuntoPoligono: boolean = false;
  public ckGestioneAnalisiMappeLegacy: boolean = false;
  /** Valori da 0 a 20 */
  public iAutoZoomSuVisualizzazioneTotale: number = 8;
  public PrecisionFarming: boolean = true;
  /** Valori da 0 a 20 */
  public LivelloClusterizzazione: number = 15;

  constructor(
    private _srvGisTipoRender_ServerSide: number = 0,
    private _SistemaDiRiferimentoPredefinito: number = 0,
    private _GruppoOperazioneColturale: string = "",
    private _TipoOperazioneColturale: string = ""
  ) {  }

  // Nessuna gestione da interfaccia per le seguenti proprietà:
  public get srvGisTipoRender_ServerSide(): number { return this._srvGisTipoRender_ServerSide };
  public get SistemaDiRiferimentoPredefinito(): number { return this._SistemaDiRiferimentoPredefinito };
  public get GruppoOperazioneColturale(): string { return this._GruppoOperazioneColturale };
  public get TipoOperazioneColturale(): string { return this._TipoOperazioneColturale };

  public get keys(): string[] {
    const keys = [];
    for (let key in this) {
      keys.push(key.startsWith("_") ? key.substring(1) : key);
    }
    return keys;
  }

  public toString(): string {
    let str = "{";
    str += this.keys.map(k => '"' + k + '":' + this[k])
      .reduce((acc, x) => acc + ',' + x);
    return  str + "}"
  }

  public parse(json: string) {
    let parsed;
    try {
      parsed = JSON.parse(json);
    } catch (e) {
      console.error("ImpostazioniApp: Error while parsing the string");
    }
    this.copyValue(parsed);
  }

  /**
   * Copia il valore delle impostazioni da un altro oggetto.
   * @param source oggetto da cui copiare i valori delle impostazioni
   */
  public copyValue(source: any) {
    for (let key in source) {
      if (this.isReadOnly(key)) {
        this['_' + key] = source[key] ?? this[key]
      } else {
        this[key] = source[key] ?? this[key];
      }
    }
  }

  public isReadOnly(key: string): boolean {
    const readonlyFields = [
      "_srvGisTipoRender_ServerSide",
      "_SistemaDiRiferimentoPredefinito",
      "_GruppoOperazioneColturale",
      "_TipoOperazioneColturale",

      "srvGisTipoRender_ServerSide",
      "SistemaDiRiferimentoPredefinito",
      "GruppoOperazioneColturale",
      "TipoOperazioneColturale",
    ];
    return readonlyFields.includes(key);
  }

  /**
   * Restituisce l'impostazione sotto forma di {@link ImpostazioniFormItem}.
   * @param field il campo che codifica l'impostazione d'interesse
   */
  public getAdditionalData(field: string): ImpostazioniFormItem {
    let data = new ImpostazioniFormItem();
    data.guida = new Impostazione(0, field);
    if ( data.guida.Impostazione_Des === "iAutoZoomSuVisualizzazioneTotale") {
      data.guida.Note = "prof.SetupGisLabeliAutoZoomSuVisualizzazioneTotaleTooltip"
    }
    data.valori = [];
    data.valoreCorrente = this[field]?.toString() || "";
    this.setControlType(data);
    data.valori.push(new GuidaValoreImpostazione(0, 'prof.SetupGisLabel' + data.guida.Impostazione_Des));
    data.valori.forEach(v => v.valore = data.valoreCorrente);
    return data;
  }

  private setControlType(data: ImpostazioniFormItem) {
    // Imposta default
    let type = typeof(this[data.guida.Impostazione_Des]);
    switch (type) {
      case "boolean":
        data.guida.Tipo_Campo = enum_TipoControllo.CASELLA_SPUNTA.toString(); break;
      case "number":
        data.guida.Tipo_Campo = enum_TipoControllo.NUMERO_INTERO.toString(); break;
      case "string":
        data.guida.Tipo_Campo = enum_TipoControllo.CASELLA_TESTO.toString(); break;
      default:
        console.log(type);
        data.guida.Tipo_Campo = enum_TipoControllo.UNDEFINED.toString(); break;
    }
  }

}
