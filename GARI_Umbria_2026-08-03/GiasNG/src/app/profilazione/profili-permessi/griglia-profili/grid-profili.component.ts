import { Component, TemplateRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridProfiliMenuActions, GridProfiliService } from './grid-profili.service';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { TipologiaUtente } from '../../models/profili-permessi/tipologia-utente.model';
import { TipologieUtentiService } from 'app/profilazione/services/profili-permessi/tipologie-utenti.service';
import { GiasDialogService, Dialog_Type, GiasDialogAction } from '../../../Service/gias-dialog.service';
import { TranslocoService } from '@jsverse/transloco';
import { filter, map, switchMap, tap, of, Observable } from 'rxjs';
import { enum_PaginaImpostazioni } from '../../impostazioni-utente/impostazioni.model';
import { ImpostazioniFormService } from '../../services/impostazioni/impostazioni-form.service';
import { GiasWindowsService } from 'gias-ui-kit';
import { DialogAction, DialogCloseResult, DialogResult } from '@progress/kendo-angular-dialog';
import { MasterService } from 'app/Service/master.service';

@Component({
  standalone: false,
  selector: 'app-grid-profili',
  templateUrl: './grid-profili.component.html',
  styleUrls: ['./grid-profili.component.css'],
  providers: [
    ...generateGridProviders(GridProfiliService, GridProfiliComponent)
  ]
})
export class GridProfiliComponent {

  @ViewChild('profileGrid') profileGrid: any;
  @ViewChild('copyFormTemplate') copyFormTemplate: TemplateRef<any>;
  @ViewChild('impostazioniRef') impostazioniRef;

  public selected: BaseCodeDescr;
  public copyProfileForm: FormGroup = new FormGroup({
    Description: new FormControl('', Validators.required),
    copySettings: new FormControl(false)
  });

  constructor(
    private profileService: TipologieUtentiService,
    private impostazioniService: ImpostazioniFormService,
    private dialog: GiasDialogService,
    private windowService: GiasWindowsService,
    private transloco: TranslocoService,
    private master: MasterService,
    private router: Router,
  ) {
    this.showCopyProfileForm();
    this.showSettingsForm();
  }

  public selectProfile(event: any) {
    if (event.selectedRows?.length) {
      let profile = event.selectedRows.at(0).dataItem;
      this.selected = profile;
      this.profileService.selectedProfile$.next(profile);
    } else {
      this.profileService.selectedProfile$.next(null);
    }
  }

  /**
   * Apre il form per la modifica delle impostazioni collegate al profilo.
   * La modalità di caricamento delle impostazioni per le tipologie è la stessa che per gli utenti.
   * (Impostazioni sempre salvate nella tabella Utenti_Impostazioni)
   */
  public showSettingsForm() {
    this.profileService.profileGridEvent$.pipe(
      filter(menuAction => menuAction.event === GridProfiliMenuActions.SET_SETTINGS),
      map(menuAction => menuAction.item),
      tap(selected => this.selected = selected),
    ).subscribe(selected => {
      this.impostazioniService.USAGE_AREA = enum_PaginaImpostazioni.UTENTI;
      this.impostazioniService.utentiSelezionati = [selected.codice.toString()];
      this.windowService.open({
        title: this.transloco.translate('prof.ImpostazioniUtenteDelProfilo') + ' "' + selected.descrizione + '"',
        content: this.impostazioniRef,
        height: window.innerHeight * 0.9,
        width: window.innerWidth * 0.9,
      });
    });
  }

  public saveSettings() {
    const touched = this.impostazioniService.getTouchedSettings(this.impostazioniService.settingsForm);
    const profile: TipologiaUtente = this.selected as TipologiaUtente;
    this.dialog.warningObs('prof.SalvataggioImpostazioni', 'prof.WarningSalvaModificate')
      .pipe(
        filter((response: boolean) => response == true),
        switchMap(() => this.impostazioniService.salvaImpostazioniModificateObs()),
        filter((response: boolean) => response == true),
        switchMap(() => this.profileService.readConnectedUsers(profile)),
        switchMap((connected) => {
          if (connected.length == 0) return of(new DialogCloseResult());
          else {
            //return this.dialog.warningObs('prof.SalvataggioImpostazioni', 'prof.ApplicaImpostazioniNUtentiCollegatiProfilo');
            const title = this.transloco.translate('prof.SalvataggioImpostazioni');
            const content = this.transloco.translate('prof.ApplicaImpostazioniNUtentiCollegatiProfilo', { 0: connected.length });
            const actions = [
              new GiasDialogAction(this.transloco.translate('ApplicaModifiche'), 0, true),
              new GiasDialogAction(this.transloco.translate('ApplicaTutto'), 1, false),
              new GiasDialogAction(this.transloco.translate('NonApplicare'), -1, false),
            ];
            return this.dialog.dialogMessageObs_Result(title, content, actions, 'auto', 'auto', () => false, Dialog_Type.warning);
          }
        }),
        map((dialogResult: DialogResult) => dialogResult instanceof DialogCloseResult ? -1 : dialogResult['returnObj']),
        switchMap((result: number) => {
          if (result < 0) return of(false);
          else if (result > 0) return this.profileService.propagateSettings(profile, []);
          else return this.profileService.propagateSettings(profile, touched);
        }),
      ).subscribe();

    // this.dialog.warningThen('prof.SalvataggioImpostazioni', content, true,
    //   () => {        
    //     if (onlyTouched)
    //       this.impostazioniService.salvaImpostazioniModificate();
    //     else
    //       this.impostazioniService.salvaTutteImpostazioni();
    //   });
  }

  public showCopyProfileForm() {
    this.profileService.profileGridEvent$.pipe(
      filter(menuAction => menuAction.event === GridProfiliMenuActions.COPY),
      map(menuAction => menuAction.item),
      tap(selected => this.selected = selected),
      switchMap(selected => {
        const title = this.transloco.translate('prof.CopiaProfilo') + ' ' + selected.descrizione;
        this.copyProfileForm.controls['Description']
          .patchValue(selected.descrizione + ' - ' + this.transloco.translate('Copia'));
        return this.dialog.dialogMessageObs_Result(
          title, this.copyFormTemplate, undefined, undefined, undefined,
          (p) => this.copyProfileForm.invalid
        );
      }),
      filter(res => res['returnObj'] === true),
      switchMap(() => this.copyProfile())
    ).subscribe(() => this.reloadComponent());
  }

  private copyProfile(): Observable<any> {
    this.master.set_isLoading({ isLoading: true });
    let original: TipologiaUtente = new TipologiaUtente(this.selected.codice, this.selected.descrizione);
    let copy = new TipologiaUtente(
      0, this.copyProfileForm.controls['Description'].value
    );
    return this.profileService.copyTipologie(copy, original, this.copyProfileForm.controls['copySettings'].value)
      .pipe(tap(() => this.master.set_isLoading({ isLoading: false })));
  }

  private reloadComponent() {
    this.profileGrid.publicService.refresh(true);
  }

}
