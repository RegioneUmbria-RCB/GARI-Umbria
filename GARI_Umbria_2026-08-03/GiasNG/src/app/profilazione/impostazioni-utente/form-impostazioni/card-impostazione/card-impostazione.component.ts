import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {FormArray, FormControl, FormGroup} from '@angular/forms';
import {ImpostazioniAziendeCentriService} from '../../../services/impostazioni/impostazioni-aziende-centri.service';
import {AziendaCentro} from 'app/profilazione/impostazioni-utente/impostazioni.model';
import {ProfilazioneDataShareService} from '../../../services/profilazione-data-share.service';
import {ImpostazioniFormService} from '../../../services/impostazioni/impostazioni-form.service';
import {Utente_Impostazioni} from '../../../../Model/utente/utente_impostazioni';
import { GeneralFieldComponent } from '../../tipi-campo-impostazioni/settings-general-field/settings-general-field.component';
import {map, Observable, take, tap, takeUntil, Subject} from 'rxjs';
import { GiasGeneralInputTemplateComponent, Impostazione } from 'gias-ui-kit';
import { enum_TipoControllo } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-card-impostazione',
  templateUrl: './card-impostazione.component.html',
  styleUrls: ['./card-impostazione.component.css']
})
export class CardImpostazioneComponent implements OnInit, OnDestroy {
  @ViewChild('generalInput') generalInput: GiasGeneralInputTemplateComponent;
  @ViewChild('generalSetting') generalSetting: GeneralFieldComponent;

  public impresa: AziendaCentro;
  /** FormGroup rappresentante il controller dell'impostazione visualizzata */
  public imp: FormGroup;
  private signal = new Subject();

  constructor(
    private ImpService: ImpostazioniFormService,
    public dataShare: ProfilazioneDataShareService,
    private ACSettings: ImpostazioniAziendeCentriService,
  ) { }

  ngOnInit(): void {
    this.dataShare.cardInfo.pipe(takeUntil(this.signal))
      .subscribe(value => this.resetCardData(value));
  }
  ngOnDestroy(): void {
    this.signal.next(true);
    this.signal.complete();
  }

  public onSave() {
    this.ACSettings.salvaImpostazioni(
      [this.impresa],
      [new Utente_Impostazioni(this.imp.value.guida.Impostazione_Cod, this.imp.value.valoreCorrente)]
    );
  }

  private resetCardData(value: Impostazione) {
    this.ACSettings.masterService.set_isLoading({isLoading: true});
    // Quando mostro una card impostazione faccio sempre riferimento a una sola azienda
    this.impresa = this.ACSettings.impreseSelezionate[0];
    this.getImpostazioneFormControl(value)
      .pipe(take(1))
      .subscribe(form => {
        this.imp = form;
        this.applyFormValue();
        this.ACSettings.masterService.set_isLoading({isLoading: false});
      });
  }

  private applyFormValue() {
    switch (+this.imp.value.guida.Tipo_Campo) {
      case enum_TipoControllo.UNDEFINED:
        if (this.generalSetting?.form) {
          this.generalSetting.form = this.imp;
          this.generalSetting.reloadComponent(this.imp.value.valoreCorrente);
        }
        break;
      case enum_TipoControllo.CASELLA_SPUNTA:
        if (this.generalInput?.form) {
          this.generalInput.form = this.imp;
          this.generalInput.applyCurrentValue();
        }
        break;
      default:
        if (this.generalInput?.form) {
          this.generalInput.form = this.imp;
        }
    }
  }

  private getImpostazioneFormControl(impostazione: Impostazione): Observable<FormGroup> {
    let form = new FormGroup({
      guida: new FormControl(impostazione),
      // guida: new FormControl(this.ImpService.findGuidaImpostazione(impostazione.Impostazione_Cod)),
      valoreCorrente: new FormControl(impostazione.value),
      valori: new FormArray([])
    });
    return this.ImpService.leggiDatiImpostazioni([impostazione.Impostazione_Cod])
      .pipe(tap(valori => {
          for (let valore of valori[0].data)
            (form.get('valori') as FormArray).push(new FormControl(valore));
        }),
        map(() => form)
      );
  }

}
