import { AfterViewInit, Component, DestroyRef, DoCheck, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { enum_TipoPermesso } from '../../models/profilazione.model';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridPermessiEditService } from './grid-permessi-edit.service';
import { faArrowsRotate, faBan, faBookOpen, faLock, faLockOpen, faPen, faPenSquare } from '@fortawesome/free-solid-svg-icons';
import { TipologieUtentiService } from 'app/profilazione/services/profili-permessi/tipologie-utenti.service';
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { TipologiaUtente } from '../../models/profili-permessi/tipologia-utente.model';
import { GiasDialogService } from "../../../Service/gias-dialog.service";
import { TranslocoService } from "@jsverse/transloco";
import { Utente_Permesso } from "../../../Model/utente/utente_permesso";
import { PermessiUtenteService } from "../../../Service/permessi-utente.service";
import { enum_Security_Attivita } from "../../../Model/TipiEnumerativi";
import { filter, map, Observable, of, switchMap, take, tap } from "rxjs";
import { cloneDeep } from "lodash";
import { MasterService } from "../../../Service/master.service";
import { ClientePermesso, ProfilazioneUtentiService } from "../../services/profilazione-utenti.service";

@Component({
  standalone: false,
  selector: 'app-grid-permessi-edit',
  templateUrl: './grid-permessi-edit.component.html',
  styleUrls: ['./grid-permessi-edit.component.css'],
  providers: [
    ...generateGridProviders(GridPermessiEditService, GridPermessiEditComponent)
  ]
})
export class GridPermessiEditComponent implements OnInit, DoCheck {
  @ViewChild('permessiGrid') permessiGrid: GiasKendoGridComponent;
  @ViewChild('unlockPermissionsEditBtn') lockBtn: ElementRef;

  public permessoGestImpostazioni: boolean;
  public selectedProfile: TipologiaUtente;
  public toAssign: enum_TipoPermesso;
  public readonly permission = {
    Read: enum_TipoPermesso.LETTURA,
    Write: enum_TipoPermesso.LETTURA_SCRITTURA,
    Disabled: enum_TipoPermesso.DISABILITATO
  };
  protected readonly faBook = faBookOpen;
  protected readonly faBan = faBan;
  protected readonly faPen = faPen;
  private isLockBtnInit = false;
  private isHoveringLockBtn = false;
  private scrollPosition = 0;
  private _afterAssign = false;

  constructor(
    private dialog: GiasDialogService,
    private master: MasterService,
    private transloco: TranslocoService,
    private profileService: TipologieUtentiService,
    private permessiService: PermessiUtenteService,
    private utentiService: ProfilazioneUtentiService
  ) {
    this.permessoGestImpostazioni = this.permessiService.getPermesso(enum_Security_Attivita.Gest_UtentiImpostazioni, enum_TipoPermesso.LETTURA_SCRITTURA);
  }

  protected get lockIcon() {
    return this.isHoveringLockBtn ? faLock : faLockOpen;
  }

  ngOnInit(): void {
    this.profileService.selectedProfile$.GiasSubscribe(prof => this.selectedProfile = prof);
  }

  ngDoCheck(): void {
    this.resizeWindow();
    this.handleScroll();
    if (this.lockBtn && !this.isLockBtnInit) {
      this.isLockBtnInit = true;
      this.lockBtn.nativeElement.addEventListener('mouseover', () => this.isHoveringLockBtn = true);
      this.lockBtn.nativeElement.addEventListener('mouseout', () => this.isHoveringLockBtn = false);
    }
  }

  /**
   * @history
   * (09/05/2024): Da riunione con Valerio si è deciso di rimuovere il pulsante di sincronizzazione in favore di
   * una sincronizzazione automatica ogni volta che si agisce sui permessi.
   */
  onSyncConnectedUsers() {
    this.profileService.updateConnectedUsers(this.selectedProfile);
  }

  onSetPermissions(toAssign: enum_TipoPermesso) {
    let selected = this.getSelectedRows();
    if (selected.length === 0) {
      this.dialog.baseError('Errore_', 'NessunPermessoSelezionato');
      return;
    }
    this.canAssignWith(toAssign).pipe(
      filter(next => next),
      tap(() => this.master.set_isLoading({ isLoading: true })),
      switchMap(() => this.profileService.readConnectedUsers(this.selectedProfile)),
      tap(() => this.master.set_isLoading({ isLoading: false }))
    ).subscribe((users) => {
      this.toAssign = toAssign;
      const title = this.transloco.translate('Attenzione');
      const content = this.transloco.translate('prof.LeModificheSiRipercuoterannoSuNUtenti', [users.length])
        + ' "' + this.selectedProfile.descrizione + '". '
        + this.transloco.translate('SiDesideraProcedere');
      this.dialog.warningThen(
        title, content, false,
        () => {
          this.setPermissions(selected, toAssign)
          this._afterAssign = true;
          this.scrollPosition = this.permessiGrid.gridElRef.nativeElement
            .querySelector('.k-grid-aria-root .k-grid-container .k-grid-content').scrollTop;
        },
        () => this.toAssign = null
      );
    });
  }

  onGoBack() {
    this.profileService.isEditing.next(false);
  }

  private setPermissions(activities: Utente_Permesso[], permissionLevel: enum_TipoPermesso) {
    let profile = cloneDeep(this.selectedProfile);
    this.profileService.setPermessiTipologia(
      profile, activities, permissionLevel
    ).pipe(take(1))
      .subscribe(() => this.profileService.selectedProfile$.next(this.selectedProfile));
  }

  private getSelectedRows(): Utente_Permesso[] {
    return this.permessiGrid.rows.filter(row => row['Selected'])
      .map(row => new Utente_Permesso(row['Attivita_Cod'], row['Tipo_Permesso']));
  }

  private resizeWindow(): void {
    let winRefs = document.getElementsByClassName('k-window ng-star-inserted');
    for (let i = 0; i < winRefs.length; i++) {
      if (!winRefs[i]?.classList.contains('resized')) {
        winRefs[i]?.setAttribute('style', 'z-index:10000 !important; top: 5%; left: 5%; width: 90%; height: 90%;');
        winRefs[i]?.classList.add('resized');
      }
    }
    winRefs = document.getElementsByClassName('k-content k-window-content ng-star-inserted');
    for (let i = 0; i < winRefs.length; i++)
      winRefs[i].setAttribute('style', 'overflow: auto !important');
  }

  private canAssignWith(permType: enum_TipoPermesso): Observable<boolean> {
    const selected = this.getSelectedRows();
    return this.utentiService.leggiClientePermessi("").pipe(
      take(1),
      map((clientPerms: ClientePermesso[]) => {
        const cannotActivateWithPermLv = [];
        if (clientPerms.length > 0) {
          for (let perm of selected) {
            let ofClient = clientPerms.find(r => r.Id_Attivita === perm.Permesso_ID);
            if (ofClient.Id_Operazione < permType) {
              cannotActivateWithPermLv.push(perm.Permesso_ID);
            }
          }
        }
        return cannotActivateWithPermLv;
      }),
      switchMap((couldntActivate: number[]) => {
        if (couldntActivate.length > 0) {
          return this.dialog.warningObs(
            'prof.PermessiNonSufficienti',
            'prof.errPermessiInsufficientiInInstallazioneText'
          ).pipe(map(res => res as boolean));
        } else {
          return of(true);
        }
      })
    );
  }

  private handleScroll() {
    if (this._afterAssign) {
      const currentScroll = this.permessiGrid.gridElRef.nativeElement
        .querySelector('.k-grid-aria-root .k-grid-container .k-grid-content').scrollTop;
      if (currentScroll === 0) {
        this.permessiGrid.gridElRef.nativeElement
          .querySelector('.k-grid-aria-root .k-grid-container .k-grid-content')
          .scroll(0, this.scrollPosition);
        this._afterAssign = false;
      }
    }
  }

}
