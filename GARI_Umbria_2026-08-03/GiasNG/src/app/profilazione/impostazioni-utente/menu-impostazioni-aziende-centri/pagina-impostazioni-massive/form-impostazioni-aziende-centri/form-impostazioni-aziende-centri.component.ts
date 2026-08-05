import { Component, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ImpostazioniAziendeCentriService } from '../../../../services/impostazioni/impostazioni-aziende-centri.service';
import { ImpostazioniFormService } from '../../../../services/impostazioni/impostazioni-form.service';
import {
  FormImpostazioniComponent
} from 'app/profilazione/impostazioni-utente/form-impostazioni/form-impostazioni.component';
import { GiasDialogService } from "../../../../../Service/gias-dialog.service";
import { faCopy } from "@fortawesome/free-solid-svg-icons";
import { filter, of, Subject, switchMap, takeUntil } from "rxjs";
import { TranslocoService } from "@jsverse/transloco";
import { FormControl, FormGroup } from "@angular/forms";
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { ProfilazioneDataShareService } from "../../../../services/profilazione-data-share.service";


@Component({
  standalone: false,
  selector: 'app-form-impostazioni-aziende-centri',
  templateUrl: './form-impostazioni-aziende-centri.component.html',
  styleUrls: ['./form-impostazioni-aziende-centri.component.css'],
  providers: [GiasDropDownTemplateService]
})
export class FormImpostazioniAziendeCentriComponent implements OnInit, OnDestroy {
  @ViewChild('Settings') Settings: FormImpostazioniComponent;
  @ViewChild('copySettingsRef') public copySettingsRef: TemplateRef<any>;

  public isOnlyBusiness = true;
  public readonly faCopy = faCopy;
  protected form = new FormGroup({ impresa: new FormControl('') });
  protected disableForm = true;
  private onDestroy$ = new Subject<boolean>();

  constructor(
    protected transloco: TranslocoService,
    protected datashare: ProfilazioneDataShareService,
    private dialogService: GiasDialogService,
    private settingsService: ImpostazioniFormService,
    private ACService: ImpostazioniAziendeCentriService,
  ) {
    this.disableForm = !this.datashare.userPermissions.canEditBusinessSettings;
  }

  get hasSelection(): boolean {
    return this.ACService.impreseSelezionate.length > 0;
  }

  ngOnInit(): void {
    this.isOnlyBusiness = this.ACService.impreseSelezionate.every(i => i.Sa_Cod === 0);
    this.handleSelectionChange();
    this.handleModeChange();
  }

  ngOnDestroy(): void {
    this.onDestroy$.next(true);
    this.onDestroy$.complete();
  }

  public onOverwriteAll() {
    this.preventIfSelectionNullOrNoPermission(() => this.dialogService.warningThen(
      'prof.ApplicaTutteImpostazioni', 'prof.WarningSalvaTutto', true,
      () => this.settingsService.salvaTutteImpostazioni(this.Settings.settingsForm)
    ));
  }

  public onOverwriteTouched() {
    this.preventIfSelectionNullOrNoPermission(() => this.dialogService.warningThen(
      'prof.ApplicaSoloModificate', 'prof.WarningSalvaModificate', true,
      () => this.settingsService.salvaImpostazioniModificate(this.Settings.settingsForm)
    ));
  }

  private preventIfSelectionNullOrNoPermission(action: Function): void {
    if (!this.hasSelection) {
      this.dialogService.baseError('Errore', 'prof.WarningSelezioneNulla');
    } else if (this.datashare.userPermissions.canEditBusinessSettings) {
      action.call(this);
    } else {
      this.dialogService.baseError('Errore', 'prof.errNoPermissions');
    }
  }

  private handleSelectionChange() {
    let prevLength = 0;
    this.ACService.selectionChange$.pipe(takeUntil(this.onDestroy$))
      .subscribe(selected => {
        if (selected.length === 1 || (prevLength === 1)) {
          try {
            this.Settings.loadSettings();
          } catch (e) {
            console.error(e);
          }
        }
        prevLength = selected.length;
      });
  }

  private handleModeChange() {
    this.ACService.modeChangeEvent.pipe(
      takeUntil(this.onDestroy$),
      filter(next => next.type === 'centersToggled')
    ).subscribe(next => this.isOnlyBusiness = !next.value);
  }

}
