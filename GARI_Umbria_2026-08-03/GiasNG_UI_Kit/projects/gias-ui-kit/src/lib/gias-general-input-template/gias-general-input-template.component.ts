import { Component, ElementRef, EventEmitter, Inject, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { Observable, Subject, take, takeUntil } from 'rxjs';
import { FormGroup, FormGroupDirective } from '@angular/forms';
import { GiasMultiSelectTemplateService } from '../gias-multiselect-template-s/gias-multiselect-template-s.service';
import { GiasDropDownTemplateService } from '../gias-drop-down-template-s/gias-drop-down-template-s.service';
import { GiiasMultiselectTemplateSComponent } from '../gias-multiselect-template-s/gias-multiselect-template-s.component';
import { BaseCodeDescr, AGRODATAFINE, BaseCodeDescrStr, enum_TipoControllo, GuidaValoreImpostazione } from '../utils/models';
import { GiasDropDownTemplateComponent } from '../gias-drop-down-template/gias-drop-down-template.component';
import { GIAS_PARAMETRI_AGENDA_TOKEN, IObjParametriAgendaService } from '../utils/obj-parametri-agenda.service';
import { GIAS_API_SERVICE_TOKEN, IGiasApiService } from '../utils/gias-api.service';

class ClientExtendedDdlParameters {
  url: string;
  httpCallType: 'POST' | 'GET';
  useObjParams = false;
  objParamField: string;
}

@Component({
  standalone: false,
  selector: 'gias-general-input-template',
  templateUrl: './gias-general-input-template.component.html',
  styleUrls: ['./gias-general-input-template.component.css'],
  providers: [GiasMultiSelectTemplateService, GiasDropDownTemplateService]
})
export class GiasGeneralInputTemplateComponent implements OnInit, OnDestroy {
  @ViewChild('radioList') radioList: ElementRef;
  @ViewChild('multi') multiselect: GiiasMultiselectTemplateSComponent; // GiiasMultiselectTemplateSComponent;
  @ViewChild('ddl') dropdown: GiasDropDownTemplateComponent; // DropDownTemplateComponent

  @Input() id = '';
  @Input() name: string;
  @Input() label: string;
  @Input() value: any;
  /** Array contentente le diverse opzioni riferite al controllo. */
  @Input() listItems: Array<BaseCodeDescr | BaseCodeDescrStr>;
  @Input() controlType: enum_TipoControllo | number = 0;
  @Input() giasFormControlName: string;
  @Input() tooltip: string;
  @Input() defaultItem: any;
  @Input() obligatory = false;
  @Input() valuePrimitive = true;
  @Input() clearButton = true;
  @Input() isDisabled = false;
  @Output() valueChange = new EventEmitter<any>();
  // Usati per i campi numerici
  @Input() maxValue: number = Number.MAX_SAFE_INTEGER;
  @Input() minValue: number = Number.MIN_SAFE_INTEGER;

  public form: FormGroup;
  public readonly enum_TipoControllo = {
    UNDEFINED: enum_TipoControllo.UNDEFINED,
    CASELLA_TESTO: enum_TipoControllo.CASELLA_TESTO,
    AREA_TESTO: enum_TipoControllo.AREA_TESTO,
    MENU_DISCESA: enum_TipoControllo.MENU_DISCESA,
    CASELLA_SPUNTA: enum_TipoControllo.CASELLA_SPUNTA,
    CALENDARIO: enum_TipoControllo.CALENDARIO,
    ALLEGATO: enum_TipoControllo.ALLEGATO, // Non ancora gestiti
    LINK: enum_TipoControllo.LINK, // Non ancora gestiti
    PASSWORD: enum_TipoControllo.PASSWORD, // Non ancora gestiti
    MULTISELECT_ESTESA_SERVER: enum_TipoControllo.MULTISELECT_ESTESA_SERVER,
    PULSANTE_SCELTA: enum_TipoControllo.PULSANTE_SCELTA,
    IMMAGINE: enum_TipoControllo.IMMAGINE, // Non ancora gestiti
    DDL_ESTESA_CLIENT: enum_TipoControllo.DDL_ESTESA_CLIENT,
    GIS_VIEWER: enum_TipoControllo.GIS_VIEWER, // Non ancora gestiti
    DDL_ESTESA_SERVER_LIGHT: enum_TipoControllo.DDL_ESTESA_SERVER_LIGHT,
    NUMERO_INTERO: enum_TipoControllo.NUMERO_INTERO,
    NUMERO_DECIMALE: enum_TipoControllo.NUMERO_DECIMALE,
    MULTISELECT: enum_TipoControllo.MULTISELECT
  };
  protected readonly AGRODATAFINE = AGRODATAFINE;

  // Original value of the form control
  private orig: any;
  private signal$: Subject<void> = new Subject();

  constructor(private rootFormGroup: FormGroupDirective,
    @Inject(GIAS_API_SERVICE_TOKEN) private apiService: IGiasApiService,
    @Inject(GIAS_PARAMETRI_AGENDA_TOKEN) private objAgenda: IObjParametriAgendaService // usato per in una chiamata ddl estesa client, NON RIMUOVERE
  ) { }

  private get formControlValue() {
    return this.form?.controls[this.giasFormControlName]?.value;
  }

  private get isFormControlDisabled(): boolean {
    return this.isDisabled || this.form.get(this.giasFormControlName).disabled;
  }

  ngOnInit(): void {
    this.form = this.rootFormGroup?.form;
    this.applyCurrentValue();
    this.setOnChange();
    if (this.controlType === enum_TipoControllo.DDL_ESTESA_SERVER_LIGHT
      || this.controlType === enum_TipoControllo.MULTISELECT_ESTESA_SERVER) {
      this.loadListItems();
    }
    if (this.controlType === enum_TipoControllo.DDL_ESTESA_CLIENT) {
      this.parseDdlEstesaClient();
    }
    if (this.controlType === enum_TipoControllo.MULTISELECT
      || this.controlType === enum_TipoControllo.MULTISELECT_ESTESA_SERVER) {
      this.parseMultiselectValue();
    }
    if (this.controlType === enum_TipoControllo.CALENDARIO) {
      this.parseDate();
    }
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }

  public onChange(val: any) {
    if (val !== this.orig && !this.isFormControlDisabled) {
      this.orig = val;
      switch (this.controlType) {
        case enum_TipoControllo.CASELLA_SPUNTA:
          this.onCheckboxChange(val);
          break;
        case enum_TipoControllo.PULSANTE_SCELTA:
          this.onRadioChange(val);
          break;
        case enum_TipoControllo.MULTISELECT:
          this.onMultiSelectChange(val);
          break;
        default:
      }
      this.valueChange.emit(this.formControlValue);
    }
  }

  public applyCurrentValue() {
    const valori: GuidaValoreImpostazione[] = this.form?.value?.valori;
    const valoreCorrente = valori?.at(0)?.valore || this.value;
    this.form?.controls[this.giasFormControlName]?.patchValue(valoreCorrente);

    switch (this.controlType) {
      case enum_TipoControllo.CASELLA_SPUNTA:
        const checked = +(this.form?.controls[this.giasFormControlName]?.value);
        this.form?.controls[this.giasFormControlName]?.patchValue(checked);
        break;
      case enum_TipoControllo.DDL_ESTESA_CLIENT:
        if (!this.listItems) this.listItems = [];
        const currValue = this.form.get(this.giasFormControlName).value;
        const item = this.listItems.find(i => i.codice == currValue);
        if (item && this.dropdown) {
          // this.dropdown.value = this.dropdown.valuePrimitive ? item.codice : item;
          this.dropdown.dropdownlist.value = this.dropdown.dropdownlist.valuePrimitive ? item.codice : item;
        }
        break;
      default:
    }
  }

  private setOnChange() {
    this.orig = this.form?.controls[this.giasFormControlName]?.value;
    this.form?.controls[this.giasFormControlName]?.valueChanges?.pipe(takeUntil(this.signal$))
      .subscribe(val => this.onChange(val));
  }

  private onCheckboxChange(checked: boolean) {
    if (this.listItems.length > 1) {
      const selected = this.listItems.filter(item => item['value'])
        .map(item => item.codice.toString());
      let toPatch = selected.length > 0
        ? selected.reduce((c1, c2) => c1 + '|' + c2)
        : '';
      this.form?.controls[this.giasFormControlName]?.patchValue(toPatch);
      this.form?.controls[this.giasFormControlName]?.markAsTouched();
    } else {
      this.form?.controls[this.giasFormControlName]?.patchValue(checked ? 1 : 0);
      this.form?.controls[this.giasFormControlName]?.markAsTouched();
    }
  }

  private onRadioChange(radioValue: any) {
    for (let radioItem of this.radioList.nativeElement.children) {
      // eslint-disable-next-line eqeqeq
      radioItem.firstChild.checked = (radioItem.firstChild.value == radioValue);
    }
    this.form?.controls[this.giasFormControlName]?.patchValue(radioValue);
    this.form?.controls[this.giasFormControlName]?.markAsTouched();
  }

  private onMultiSelectChange(values: any[]) {
    this.form?.controls[this.giasFormControlName]?.patchValue(values);
    this.form?.controls[this.giasFormControlName]?.markAsTouched();
  }

  private loadListItems() {
    if (!this.listItems) this.listItems = [];
    const indexInfoLoad = this.listItems.findIndex(i => i.codice === "url");
    if (indexInfoLoad >= 0 && this.listItems.at(indexInfoLoad).descrizione !== "") {
      // Estraggo il record contenente l'url da cui caricare gli altri valori
      const infoLoad = this.listItems.at(indexInfoLoad);
      this.listItems.splice(indexInfoLoad, 1);
      this.apiService.ajaxAPIPost<any, Array<BaseCodeDescr>>(infoLoad.descrizione, '')
        .pipe(take(1))
        .subscribe(data => {
          if (data.RispostaStringa) {
            let cod2str = data.RispostaStringa.map(x => new BaseCodeDescrStr(x.codice.toString(), x.descrizione));
            this.listItems = this.listItems.concat(cod2str);
          } else {
            console.error("Load additional settings' data failed.", data);
          }
        });
    }
  }

  private parseMultiselectValue() {
    let control = this.form?.get(this.giasFormControlName);
    if (control && control.value && !Array.isArray(control.value)) {
      let selected = control.value.split('|');
      control.patchValue(selected);
    }
  }

  private parseDate() {
    let control = this.form?.get(this.giasFormControlName);
    if (control && control.value && typeof (control.value) === 'string') {
      const yyyy = +(control.value as string).substring(0, 4);
      const mm = +(control.value as string).substring(4, 2) - 1;
      const dd = +(control.value as string).substring(6);
      control.patchValue(new Date(yyyy, mm, dd));
    }
  }

  private parseDdlEstesaClient() {
    const validIndex = (i: number) => (i >= 0 && this.listItems.at(i).descrizione !== "");
    const getInfoOrElse = (infoName: string, orElse: string) => {
      const idx = this.listItems.findIndex(i => i.codice === infoName);
      if (validIndex(idx)) {
        const info = this.listItems.at(idx);
        this.listItems.splice(idx, 1);
        return info.descrizione;
      }
      return orElse;
    };
    const paramsStr = getInfoOrElse('loadParams', null);
    if (paramsStr) {
      try {
        const params: ClientExtendedDdlParameters = JSON.parse(paramsStr);
        let obs: Observable<any>;
        let callInParams;
        if (params.useObjParams) {
          callInParams = params.objParamField ? this.objAgenda.getObjParamValue()[params.objParamField] : this.objAgenda.getObjParamValue();
        } else {
          callInParams = params.objParamField;
        }
        if (params.httpCallType === 'POST') {
          obs = this.apiService.ajaxAPIPost<any, any[]>(params.url, callInParams).pipe(take(1));
        } else {
          obs = this.apiService.ajaxAPIGet<any, any[]>(params.url, callInParams).pipe(take(1));
        }
        obs.subscribe(data => {
          if (data.RispostaStringa) {
            let l = data.RispostaStringa as BaseCodeDescrStr[];
            let distinctItems = Object.fromEntries(l.map(x => [x['codice'], x]));
            for (let item in distinctItems) {
              this.listItems.push(distinctItems[item]);
            }
            this.applyCurrentValue();
          }
        });
      } catch (ex) {
        console.error(ex);
      }
    }
  }

}
