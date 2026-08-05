import { Component, Input, OnInit } from '@angular/core';
import {FormArray, FormControl, FormGroup, Validators} from "@angular/forms";
import {ImpostazioniGIS} from "../../../models/Impostazioni/ImpostazioneSetupGis.model";
import {ProfilazioneDataShareService} from "../../../services/profilazione-data-share.service";
import {TranslocoService} from "@jsverse/transloco";
import {enum_Impostazioni_Utenti} from "../../../../Model/Impostazioni_Utenti.enum";
import { enum_TipoControllo } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-settings-gis',
  templateUrl: './settings-gis.component.html',
  styleUrls: ['./settings-gis.component.css']
})
export class SettingsGisComponent implements OnInit {
  @Input() formGroup: FormGroup;

  protected maxValue = 20;
  protected minValue = 0;

  private setup = new ImpostazioniGIS();
  private gisForm = new FormGroup({
    settings: new FormArray([])
  });

  constructor(private datashare: ProfilazioneDataShareService,
              private transloco: TranslocoService) { }

  public get getSettings(){
    return (this.gisForm.get('settings') as FormArray).controls;
  }
  ngOnInit(): void {
    this.datashare.formsMap.set(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI, this.formGroup);
    this.parseSettings();
    this.initGisForm();
    this.handleChange();
  }

  private parseSettings() {
    let valueStr = this.formGroup.value.valori[0].valore;
    this.formGroup.get('valoreCorrente')?.patchValue(valueStr);
    this.setup.parse(valueStr);
  }

  private initGisForm() {
    for (let key of this.setup.keys.filter(k => !this.setup.isReadOnly(k))) {
      let setting = this.setup.getAdditionalData(key);
      setting.valori.forEach(v => v.descrizione = this.transloco.translate(v.descrizione));
      (this.gisForm.get('settings') as FormArray).push(new FormGroup({
        guida: new FormControl(setting.guida),
        valoreCorrente: new FormControl(setting.valoreCorrente),
        valori: new FormControl(setting.valori)
      }));
    }
  }


  private handleChange() {
    this.gisForm.valueChanges.subscribe(ch => {
      for (let newValue of ch.settings) {
        let key = newValue.guida.Impostazione_Des;
        if (+newValue.guida.Tipo_Campo === enum_TipoControllo.CASELLA_SPUNTA) {
          this.setup[key] = (newValue.valoreCorrente) ? true : false;
        } else if (+newValue.guida.Tipo_Campo === enum_TipoControllo.NUMERO_INTERO
          || +newValue.guida.Tipo_Campo === enum_TipoControllo.MENU_DISCESA) {
          this.setup[key] = +(newValue.valoreCorrente);
        } else if (+newValue.guida.Tipo_Campo === enum_TipoControllo.CASELLA_TESTO) {
          this.setup[key] = newValue.valoreCorrente;
        } else {
          console.log('app settings: control not handled for setting ', newValue);
        }
      }
      this.formGroup.get('valoreCorrente').patchValue(this.setup.toString());
    });
  }

}
