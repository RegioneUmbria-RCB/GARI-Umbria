import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormGroup } from '@angular/forms';
import { Subject, takeUntil, tap } from 'rxjs';
import { ImpostazioniFormService } from '../../../services/impostazioni/impostazioni-form.service';
import { ProfilazioneDataShareService } from "../../../services/profilazione-data-share.service";
import { GuidaValoreImpostazione } from 'gias-ui-kit';
import { TranslocoService } from '@jsverse/transloco';
import { MasterService } from 'app/Service/master.service';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';

export enum OrigineImpostazione {
  Utente,
  Tipologia,
  Default
}

@Component({
  standalone: false,
  selector: 'app-sezione-impostazioni',
  templateUrl: './sezione-impostazioni.component.html',
  styleUrls: ['./sezione-impostazioni.component.css']
})
export class SezioneImpostazioniComponent implements OnInit, OnDestroy {
  @Input('sezioneForm') sezioneForm: FormGroup;
  private _loaded = false;
  private signal: Subject<void> = new Subject();
  private readonly AgendaBlocchiSubsectionCode = 5;

  constructor(
    private impostazioniService: ImpostazioniFormService,
    private datashare: ProfilazioneDataShareService,
    private transloco: TranslocoService,
    private masterService: MasterService
  ) { }

  public get loaded(): boolean {
    return this._loaded;
  }

  private get cannotEditAdvancedSettings(): boolean {
    return !this.datashare.userPermissions.canEditAgendaBlocksUserSettings
      || !this.datashare.userPermissions.canEditAdvancedUserSettings;
  }

  ngOnInit(): void {
    this.datashare.settingsLoaded = false;
    this.impostazioniService.leggiDatiImpostazioni(this.codiciImpostazioni)
      .pipe(
        takeUntil(this.signal),
        tap(R => {
          this.valorizaSettingsOptions(R);
          if (this.cannotEditAdvancedSettings) {
            this.disableAgendaBlocks();
          }
          // 30/03/2026: impostazione modalità Demetra è stata segnata come readonly
          this.findSetting(enum_Impostazioni_Utenti.SUPERUSER_ModalitaDemetra)?.disable();
        })
      ).subscribe(R => setTimeout(() => this.datashare.settingsLoaded = true, 500));
  }

  ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
  }

  /** Ritorna un array con i codici delle impostazioni contenute nella sezione. */
  public get codiciImpostazioni(): number[] {
    const cods: number[] = [];
    for (let sub of this.sezioneForm.value.children) {
      for (let lvl of sub.children) {
        for (let imp of lvl.impostazioni)
          cods.push(imp.guida.Impostazione_Cod);
      }
    }
    return cods;
  }

  public getChildren(sezione: FormGroup): AbstractControl[] {
    return (sezione.get('children') as FormArray).controls;
  }

  public getSettings(sezione: FormGroup) {
    return (sezione.get('impostazioni') as FormArray).controls;
  }

  public getOriginTooltip(imp): string {
    if (this.datashare.settingsLoaded && imp.value.valori.length > 0) {
      return imp.value.valori[0].Username;
    }
    return "";
  }

  protected isAgendaBlocchiAndCannotBeEdited(subsection: FormGroup) {
    return subsection.get('codice').value === this.AgendaBlocchiSubsectionCode && this.cannotEditAdvancedSettings;
  }

  private valorizaSettingsOptions(valoriImpostazioni: Array<{
    Impostazione_Cod: number;
    data: GuidaValoreImpostazione[]
  }>) {
    valoriImpostazioni.forEach(setting => {
      let imp: FormGroup = this.findSetting(setting.Impostazione_Cod);
      imp.get('valori').patchValue(setting.data);
    });
    this._loaded = true;
  }

  private findSetting(cod: number): FormGroup {
    for (let sb of (this.sezioneForm.get('children') as FormArray).controls) {
      for (let lv of (sb.get('children') as FormArray).controls) {
        for (let i of (lv.get('impostazioni') as FormArray).controls) {
          if (i.value.guida.Impostazione_Cod === cod)
            return i as FormGroup;
        }
      }
    }
  }

  private disableAgendaBlocks() {
    for (let subSection of (this.sezioneForm.get('children') as FormArray).controls) {
      if (subSection.get('codice').value === this.AgendaBlocchiSubsectionCode) {
        subSection.disable();
        return;
      }
    }
  }

}
