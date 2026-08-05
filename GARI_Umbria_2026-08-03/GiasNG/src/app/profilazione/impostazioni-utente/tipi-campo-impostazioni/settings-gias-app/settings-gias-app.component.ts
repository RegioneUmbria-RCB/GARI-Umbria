import {AfterViewInit, Component, DestroyRef, inject, Input, OnInit} from '@angular/core';
import {FormArray, FormControl, FormGroup, ValidatorFn, Validators} from "@angular/forms";
import {enum_Impostazioni_Utenti} from "../../../../Model/Impostazioni_Utenti.enum";
import {customAppSettings, hashString, ImpostazioniApp} from "../../../models/Impostazioni/ImpostazioniApp.model";
import {TranslocoService} from "@jsverse/transloco";
import { enum_TipoControllo, ImpostazioniFormItem } from 'gias-ui-kit';
import {ProfilazioneDataShareService} from "../../../services/profilazione-data-share.service";
import {cloneDeep} from "lodash";
import {map, takeUntil, filter} from "rxjs";
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  standalone: false,
  selector: 'app-settings-gias-app',
  templateUrl: './settings-gias-app.component.html',
  styleUrls: ['./settings-gias-app.component.css']
})
export class SettingsGiasAppComponent implements OnInit, AfterViewInit {
  @Input('formGroup') form: FormGroup;

  private destroy = inject(DestroyRef);
  private appSettingsObj = new ImpostazioniApp();
  private appForm = new FormGroup({
    settings: new FormArray([])
  });

  private readonly _maxDocumentSizeLimit = 20;

  constructor(private datashare: ProfilazioneDataShareService,
              private transloco: TranslocoService) { }

  public get getSettings(){
    return (this.appForm.get('settings') as FormArray).controls;
  }

  ngOnInit(): void {
    this.datashare.formsMap.set(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI, this.form);
    this.parseSettings();
    this.initAppForm();
    this.handleChange();
  }

  ngAfterViewInit(): void {
    const importazioniFrom = this.datashare.formsMap.get(
      enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI + hashString("Importazioni") * 1000
    );
    if (importazioniFrom != undefined) {
      importazioniFrom.valueChanges
        .pipe(takeUntilDestroyed(this.destroy))
        .subscribe((v: ImpostazioniFormItem) => this.applyChange({settings: [v]}));
    }
  }

  private parseSettings() {
    let valueStr = this.form.value.valori.length == 0 ? "" : this.form.value.valori[0].valore;
    this.form.get('valoreCorrente')?.patchValue(valueStr);
    this.appSettingsObj.parse(valueStr);
  }

  private initAppForm() {
    console.debug('initAppForm', this.appSettingsObj);
    for (let key in this.appSettingsObj) {
      const setting = this.appSettingsObj.getAdditionalData(key);
      setting.valori.forEach(v => v.descrizione = this.transloco.translate(v.descrizione));
      const fg = new FormGroup({
          guida: new FormControl(setting.guida),
          valoreCorrente: new FormControl(setting.valoreCorrente, this.getFieldValidator(key)),
          valori: new FormControl(setting.valori)
        });
      this.handleStatusChange(fg, key);
      (this.appForm.get('settings') as FormArray).push(fg);
    }
  }

  private getFieldValidator(key: string): ValidatorFn[] {
    if (key == 'DocumentSizeLimit') {
      return [Validators.max(this._maxDocumentSizeLimit)];
    }
    return [];
  }

  private handleStatusChange(fg: FormGroup, key: string) {
    if (key == 'DocumentSizeLimit') {
      fg.statusChanges.pipe(takeUntilDestroyed(this.destroy), filter(x => x == 'INVALID'))
      .subscribe(() => setTimeout(() => {
        fg.get('valoreCorrente').setValue(this._maxDocumentSizeLimit);
        fg.markAsTouched();
      }, 2000));
    }
  }

  private handleChange() {
    this.appForm.valueChanges.pipe(
        takeUntil(this.datashare.exitProfilazione),
        map((ch: Partial<{settings:ImpostazioniFormItem[]}>) => {
          if (!this.datashare.settingsLoaded) {
           this.reapplyDefaults(ch);
          }
          return ch;
        })
      ).subscribe((ch: Partial<{settings:ImpostazioniFormItem[]}>) => this.applyChange(ch));
      this.appForm.get('settings').valueChanges.pipe(takeUntil(this.datashare.exitProfilazione)).subscribe(v => { console.debug('settings change', v); } );
  }

  /** Gestisce l'update del valore el controllo tramite l'evento di output del component.
   * 
   * @param newValue Il nuovo valore selezionato
   * @param ctrlIdx L'indice del controllo nel form array
   * @history
   * 02/10/2025: dovuta aggiungere questa funzione perché a quanto pare {@link handleChange()}
   * non sta funzionando nel caso in cui l'utente non abbia già un valore salvato per l'impostazione app.
   */
  public onValueChange(newValue: any, ctrlIdx: number) {
    this.getSettings[ctrlIdx].get('valoreCorrente').patchValue(newValue);
    this.getSettings[ctrlIdx].get('valoreCorrente').markAsTouched({onlySelf: false});
    this.applyChange(this.appForm.value);
  }

  private applyChange(ch: Partial<{settings:ImpostazioniFormItem[]}>) {
    const orig = cloneDeep(this.appSettingsObj);
    let changed = false;
    this.applyValue(ch);
    this.form.get('valoreCorrente').patchValue(JSON.stringify(this.appSettingsObj));
    if (this.datashare.settingsLoaded) {
      for (let field in orig) {
        if (this.appSettingsObj[field] !== orig[field]) {
          changed = true;
          console.log('changed: ', field, changed, ': ', orig[field], '-->', this.appSettingsObj[field]);
          this.form.markAsTouched();
        }
      }
    }
  }

  private applyValue(ch: Partial<{settings:ImpostazioniFormItem[]}>) {
    for (let newValue of ch.settings) {
      let key = newValue.guida.Impostazione_Des;
      if (+newValue.guida.Tipo_Campo === enum_TipoControllo.CASELLA_SPUNTA) {
        this.appSettingsObj[key] = !!(newValue.valoreCorrente);
      } else if (+newValue.guida.Tipo_Campo === enum_TipoControllo.NUMERO_INTERO) {
        this.appSettingsObj[key] = Math.round(+(newValue.valoreCorrente));
      } else if (+newValue.guida.Tipo_Campo === enum_TipoControllo.NUMERO_DECIMALE) {
        this.appSettingsObj[key] = +(newValue.valoreCorrente);
      } else if (+newValue.guida.Tipo_Campo === enum_TipoControllo.CASELLA_TESTO) {
        this.appSettingsObj[key] = newValue.valoreCorrente;
      } else if (+newValue.guida.Tipo_Campo === enum_TipoControllo.MENU_DISCESA) {
        this.appSettingsObj[key] = newValue.valori.find(x => x.codice == newValue.valoreCorrente)?.codice
          ?? newValue.valoreCorrente;
      } else if (+newValue.guida.Tipo_Campo === enum_TipoControllo.MULTISELECT) {
        this.appSettingsObj[key] = newValue.valoreCorrente.filter((x: string) => x != "")
          .reduce((x1: string, x2: string) => x1 + "|" + x2);
        console.debug(key, this.appSettingsObj[key], newValue.valoreCorrente)
      } else if (+newValue.guida.Tipo_Campo === enum_TipoControllo.UNDEFINED && customAppSettings.includes(key)) {
        this.appSettingsObj[key] = newValue.valoreCorrente;
      } else if (!customAppSettings.includes(key)) {
        console.error('app settings: control not handled for setting ', newValue);
      }
    }
  }

  private reapplyDefaults(ch: Partial<{settings:ImpostazioniFormItem[]}>) {
    let read = JSON.parse(this.form.value.valori[0].valore);
    let presetKeys: string[] = [];
    for (let k in read) {
      presetKeys.push(k);
    }
    ch.settings.map((s,i) => ({ctrlValue: s, index:i }))
      .filter(s => presetKeys.includes(s.ctrlValue.guida.Impostazione_Des))
      .forEach(s => {
        let key = s.ctrlValue.guida.Impostazione_Des;
        if (s.ctrlValue.valoreCorrente != read[key] && !!read[key]) {
          s.ctrlValue.valoreCorrente = read[key];
          (this.appForm.get('settings') as FormArray).controls[s.index].patchValue(s.ctrlValue);
        }
      });
  }

}
