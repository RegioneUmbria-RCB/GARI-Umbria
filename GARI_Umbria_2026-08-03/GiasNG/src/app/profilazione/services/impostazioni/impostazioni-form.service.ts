import { EventEmitter, Injectable } from '@angular/core';
import { FormArray, FormGroup } from '@angular/forms';
import { AjaxAgronicaService } from 'app/Service/ajax-agronica.service';
import { MasterService } from 'app/Service/master.service';
import { map, Observable, of, Subject, tap } from 'rxjs';
import { ImpostazioniAziendeCentriService } from './impostazioni-aziende-centri.service';
import { ImpostazioniUtentiService } from './impostazioni-utenti.service';
import { enum_PaginaImpostazioni } from '../../impostazioni-utente/impostazioni.model';
import { AjaxAgronicaAPIService } from '../../../Service/ajax-agronica.api.service';
import { GuidaValoreImpostazione, Impostazione } from 'gias-ui-kit';
import { ProfilazioneDataShareService } from '../profilazione-data-share.service';
import { ImpostazioniFormItem } from 'gias-ui-kit';
import { Utente_Impostazioni } from '../../../Model/utente/utente_impostazioni';
import { Lavorazione } from "../../../Model/attivita/Lavorazione";
import { cloneDeep, isDate } from "lodash";
import { enum_PagineProfilazione } from "../../models/PaginaProfilazione.model";
import { enum_TipoControllo } from 'gias-ui-kit';


const linkAreeGIAS = "Metaschema/ImpostazioniUtente.asmx/Carica_AreeGIAS";


@Injectable()
export class ImpostazioniFormService {
  /** Flag interno per gestire le impostazioni */
  public readonly MASTER_EDIT = false;

  /** Indica l'utente da usare come riferiemnto per il caricamento delle impostazioni.*/
  public utentiSelezionati: string[] = [];
  public USAGE_AREA: enum_PaginaImpostazioni = enum_PaginaImpostazioni.UNDEFINED;
  public settingsForm: FormGroup;
  public addOperationsInFilter$: Subject<Lavorazione[]> = new Subject();

  constructor(
    public datashare: ProfilazioneDataShareService,
    public masterService: MasterService,
    private userSettings: ImpostazioniUtentiService,
    private ACSettings: ImpostazioniAziendeCentriService,
    private ajaxAgronicaService: AjaxAgronicaService,
    private APIService: AjaxAgronicaAPIService,
  ) {
    this.utentiSelezionati = [this.masterService.objP_utenti.UtenteUsername];
  }

  public get currentPage(): enum_PagineProfilazione {
    return this.datashare.currentPage.value;
  }


  /*********** FILTRO SQL ***********/

  filterChanged: EventEmitter<any> = new EventEmitter();
  private _SQLFilter: any[] = [];

  public set SQL_Filter(filter: any[]) {
    this._SQLFilter = filter;

    this.filterChanged.emit(this._SQLFilter);
  }

  public get SQL_Filter(): any[] {
    return this._SQLFilter;
  }

  // #region Lettura dati

  leggiSezioniImpostazioni(usage?: enum_PaginaImpostazioni): Observable<any> {
    const forPage = usage || this.USAGE_AREA;
    if (forPage === enum_PaginaImpostazioni.AZIENDE_CENTRI) {
      return this.ACSettings.leggiSezioniImpostazioniAziendeCentri();
    } else if (forPage === enum_PaginaImpostazioni.UTENTI) {
      return this.userSettings.leggiSezioniImpostazioniUtente(0, this.utentiSelezionati);
    }
    return this.userSettings.leggiSezioniImpostazioniUtente();
  }

  /**
   * Legge i valori delle impostazioni contenute nella pagina.
   * In base alla pagina di utilizzo specificata in this.USAGE_AREA carica i
   * valori selezionati dall'utente o dall'impresa/e selezionate.
   *
   * @param codiciImpostazioni codici delle impostazioni da caricare
   * @param selected username dell'utente o piva dell'impresa/e
   * @returns Observable contenente la lista di valori di ogni impostazione
   */
  leggiDatiImpostazioni(codiciImpostazioni: number[], selected?: string[], setLoading = true)
    : Observable<{ Impostazione_Cod: number; data: GuidaValoreImpostazione[] }[]> {
    if (!selected) {
      selected = this.utentiSelezionati;
    }
    const alreadyLoading = this.masterService.get_isLoading().isLoading;
    if (setLoading && !alreadyLoading) {
      this.masterService.set_isLoading({ isLoading: true, zIndex: 20000 });
    }
    if (this.USAGE_AREA === enum_PaginaImpostazioni.AZIENDE_CENTRI) {
      return this.ACSettings.leggiDatiImpostazioniAziendeCentri(codiciImpostazioni)
        .pipe(tap(() => {
          if (setLoading && !alreadyLoading) this.masterService.set_isLoading({ isLoading: false });
        }));
    } else {
      return this.userSettings.leggiControlliImpostazioniUtente(codiciImpostazioni, selected)
        .pipe(tap(() => {
          if (setLoading && !alreadyLoading) this.masterService.set_isLoading({ isLoading: false });
        }));
    }
  }

  private checkSettingsValues(impostazioni: Utente_Impostazioni[]) {
    let setting = impostazioni.find(z => z.Impostazione_Cod == 860);
    if (setting != undefined) {
      let value = JSON.parse(setting.Valore);
      value['DocumentSizeLimit'] = value['DocumentSizeLimit'] <= 20 ? value['DocumentSizeLimit'] : 20;
      setting.Valore = JSON.stringify(value);
    }
  }
  // #endregion

  // #region Salvataggio

  salvaTutteImpostazioni(settingsForm?: FormGroup) {
    let form = settingsForm || this.settingsForm;
    this.salvaImpostazioni(this.dividiImpostazioni(form));
  }

  salvaImpostazioniModificate(settingsForm?: FormGroup) {
    let impostazioni = this.getTouchedSettings(settingsForm || this.settingsForm);
    this.salvaImpostazioni(impostazioni);
  }

  salvaImpostazioniModificateObs(settingsForm?: FormGroup): Observable<boolean> {
    let impostazioni = this.getTouchedSettings(settingsForm || this.settingsForm);
    this.checkSettingsValues(impostazioni);
    return this.salvaImpostazioniObs(impostazioni);
  }

  private salvaImpostazioni(impostazioni: Utente_Impostazioni[]) {
    if (this.USAGE_AREA === enum_PaginaImpostazioni.UTENTI || this.USAGE_AREA === enum_PaginaImpostazioni.SUPERUSER) {
      this.userSettings.salvaImpostazioniUtente(impostazioni, this.utentiSelezionati);
    } else if (this.USAGE_AREA === enum_PaginaImpostazioni.AZIENDE_CENTRI) {
      this.ACSettings.salvaImpostazioniAziendeCentri(impostazioni);
    }
  }

  private salvaImpostazioniObs(impostazioni: Utente_Impostazioni[]): Observable<boolean> {
    if (this.USAGE_AREA === enum_PaginaImpostazioni.UTENTI || this.USAGE_AREA === enum_PaginaImpostazioni.SUPERUSER) {
      return this.userSettings.salvaImpostazioniUtenteObs(impostazioni, this.utentiSelezionati);
    } else if (this.USAGE_AREA === enum_PaginaImpostazioni.AZIENDE_CENTRI) {
      return this.ACSettings.salvaImpostazioniAziendeCentriObs(impostazioni);
    }
  }

  public copiaImpostazioni(base: string[], template: string): Observable<boolean> {
    if (this.USAGE_AREA === enum_PaginaImpostazioni.UTENTI) {
      return this.userSettings.copiaImpostazioni(base, template);
    } else if (this.USAGE_AREA === enum_PaginaImpostazioni.AZIENDE_CENTRI) {
      let impostazioni = this.dividiImpostazioni(this.settingsForm);
      return this.ACSettings.copiaImpostazioni(base, template, impostazioni);
    }
    return of(false);
  }

  /**
   * Reimposta il valore delle impostazioni specificate al valore registrato dal superuser o quello di default.
   * @param selected gli username degli utenti cui resettare le impostazioni
   * @param codes i codici delle impostazioni su cui eseguire l'operazione
   */
  public resetImpostazioniToDefault(selected: string[], codes: number[]) {
    if (!selected)
      selected = this.utentiSelezionati;
    if (this.USAGE_AREA === enum_PaginaImpostazioni.UTENTI) {
      return this.userSettings.resetImpostazioniToDefault(codes, selected);
    }
  }
  // #endregion

  // #region Ricerca & Utils

  /**
   * Se il form impostazioni è stato caricato, ritorna l'impostazione specificata.
   * @param codice codice dell'impostazione da ricercare
   * @return return una copia del valore del controller dell'impostazione o <code>undefined</code> se l'impostazione non è stata trovata.
   */
  public findGuidaImpostazione(codice: number): Impostazione {
    for (let sezione of this.settingsForm?.value?.sezioni) {
      for (let sub of sezione.children) {
        for (let livello of sub.children) {
          const impostazione = livello.impostazioni.find(i => i.guida.Impostazione_Cod === codice);
          if (impostazione !== undefined) {
            impostazione.guida.value = impostazione.valoreCorrente;
            return cloneDeep(impostazione.guida);
          }
        }
      }
    }
    return undefined;
  }

  public findImpostazioneForm(codice: number): FormGroup {
    for (let sezione of (this.settingsForm?.get("sezioni") as FormArray).controls) {
      for (let sub of (sezione.get("children") as FormArray).controls) {
        for (let livello of (sub.get("children") as FormArray).controls) {
          const impostazione = (livello.get("impostazioni") as FormArray).controls
            .find(ctrl => ctrl.value.guida.Impostazione_Cod === codice);
          if (impostazione !== undefined) {
            return impostazione as FormGroup;
          }
        }
      }
    }
    return undefined;
  }

  /**
   * Suddivide i campi del form impostazioni nei singoli valori da salvare.
   *
   * @param settingsForm il form impostazioni
   * @returns un array contenente gli oggetti da salvare
   */
  private dividiImpostazioni(settingsForm: FormGroup) {
    let impostazioni: Utente_Impostazioni[] = [];
    for (let sezione of settingsForm.get('sezioni').value) {
      for (let sub of sezione.children) {
        for (let livello of sub.children) {
          let toAdd = livello.impostazioni.map(c => {
            if (typeof (c.valoreCorrente) === 'number') {
              c.valoreCorrente = c.valoreCorrente.toString();
            } else if (typeof (c.valoreCorrente) === 'object' && c.valoreCorrente.length) {
              c.valoreCorrente = c.valoreCorrente.reduce((a, b) => a + "|" + b);
            }
            return new Utente_Impostazioni(c.guida.Impostazione_Cod, c.valoreCorrente);
          });
          impostazioni = impostazioni.concat(toAdd);
        }
      }
    }
    return impostazioni;
  }

  /**
   * Suddivide i campi del form impostazioni nei singoli valori da salvare
   * considerando unicamente i campi modificati.
   *
   * @param settingsForm il form impostazioni
   * @returns un array contenente gli oggetti da salvare
   */
  public getTouchedSettings(settingsForm: FormGroup): Utente_Impostazioni[] {
    const touched: Utente_Impostazioni[] = [];
    let touchedSections = (settingsForm.get('sezioni') as FormArray).controls.filter(c => c.touched);
    for (let sezioneCtrl of touchedSections) {

      let touchedSubsections = (sezioneCtrl.get('children') as FormArray).controls.filter(c => c.touched);
      for (let subCtrl of touchedSubsections) {

        let touchedLevels = (subCtrl.get('children') as FormArray).controls.filter(c => c.touched);
        for (let levelCtrl of touchedLevels) {
          (levelCtrl.get('impostazioni') as FormArray).controls
            .filter(c => c.touched)
            .map(absCtrl => absCtrl.value)
            .map(c => {
              this.fixValorization(c);
              return new Utente_Impostazioni(c.guida.Impostazione_Cod, c.valoreCorrente);
            })
            .forEach(s => touched.push(s));
        }
      }
    }
    return touched;
  }

  /**
   * Fix the valorization of the setting based on certain conditions.
   * If setting.valoreCorrente is a date, it converts it to the format YYYYMMDD.
   * If it's an array, it concatenates the elements with a '|' separator.
   * If it matches certain conditions based on setting.guida.Tipo_Campo, it validates setting.valoreCorrente
   * against setting.valori and potentially updates it to an empty string to avoid saving invalid values such as `"undefined"`.
   * @param {ImpostazioniFormItem} setting - the setting to be fixed
   */
  private fixValorization(setting: ImpostazioniFormItem) {
    if (isDate(setting.valoreCorrente)) {
      // Salva la data in formato YYYYMMDD
      setting.valoreCorrente = this.date2yyyymmdd(setting.valoreCorrente as Date);
    } else if (Array.isArray(setting.valoreCorrente)) {
      setting.valoreCorrente = setting.valoreCorrente.length
        ? setting.valoreCorrente.reduce((a, b) => a + '|' + b)
        : "";
    } else if (+setting.guida.Tipo_Campo === enum_TipoControllo.MENU_DISCESA) {
      if (setting.valori.find && !setting.valori.find(value => value.codice === setting.valoreCorrente)) {
        setting.valoreCorrente = "";
      }
    }
  }

  private date2yyyymmdd(date: Date): string {
    const yyyy = date.getFullYear().toString();
    let mm = (date.getMonth() + 1).toString();
    mm = +mm < 10 ? '0' + mm : mm;
    let dd = date.getDate().toString();
    dd = +dd < 10 ? '0' + dd : dd;
    return yyyy + mm + dd;
  }

  //#region Aree Gias

  public leggiAreeGIAS() {
    const parametri = {
      objParametri_Utenti: this.masterService.ObjParametri_Utenti,
      objParametri_Server: this.masterService.ObjParametri_Server,
      data: ''
    };
    this.masterService.set_isLoading({ isLoading: true, message: '' });
    return this.ajaxAgronicaService.legacy_get(
      this.masterService.link_CoreWS + linkAreeGIAS,
      parametri
    ).pipe(map((risposta) => {
      this.masterService.set_isLoading({ isLoading: false, message: '' });
      const listItems = JSON.parse(risposta.RispostaStringa);
      return listItems;
    }));
  }

  /** GUIDA IMPOSTAZIONI **/
  public aggiungiGuidaImpostazione(guida: Array<Impostazione>) {
    return this.APIService.ajaxAPIPost<Impostazione[], any>(
      "Profilazione/AggiungiGuidaImpostazione", guida
    ).subscribe(R => {
      console.log(R);
    });
  }

}
