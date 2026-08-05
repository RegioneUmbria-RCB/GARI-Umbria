import {Component, DoCheck, ElementRef, OnInit, ViewChild} from '@angular/core';
import {faEdit, faLock, faLockOpen,} from '@fortawesome/free-solid-svg-icons';
import {ProfilazioneUtentiService} from 'app/profilazione/services/profilazione-utenti.service';
import {TipologieUtentiService} from 'app/profilazione/services/profili-permessi/tipologie-utenti.service';
import {generateGridProviders} from 'gias-kendo-grid';
import {GridPermessiService} from './grid-permessi.service';
import {PermessiUtenteService} from "../../../Service/permessi-utente.service";
import {enum_Security_Attivita} from "../../../Model/TipiEnumerativi";
import {enum_TipoPermesso} from "../../models/profilazione.model";
import {TipologiaUtente} from "../../models/profili-permessi/tipologia-utente.model";

@Component({
  standalone: false,
  selector: 'app-grid-permessi',
  templateUrl: './grid-permessi.component.html',
  styleUrls: ['./grid-permessi.component.css'],
  providers: [
    ...generateGridProviders(GridPermessiService, GridPermessiComponent)
  ]
})
export class GridPermessiComponent implements OnInit, DoCheck {
  @ViewChild('PermessiGrid') permessiGrid: any;
  @ViewChild('unlockPermissionsEditBtn') lockBtn: ElementRef;

  public permessoEdit: boolean;
  public permessoGestImpostazioni: boolean;
  public selectedProfile: TipologiaUtente;
  public users = [];

  private isLockBtnInit = false;
  private isHoveringLockBtn = false;

  constructor(
    private permessiService: PermessiUtenteService,
    private profileService: TipologieUtentiService,
    private utentiService: ProfilazioneUtentiService,
  ) {
    this.permessoGestImpostazioni = this.permessiService.getPermesso(enum_Security_Attivita.Gest_UtentiImpostazioni, enum_TipoPermesso.LETTURA_SCRITTURA);
  }

  protected get lockIcon() {
    return this.isHoveringLockBtn ? faLockOpen : faLock;
  }

  ngDoCheck(): void {
    this.resizeWindow();
    if (this.lockBtn && !this.isLockBtnInit) {
      this.isLockBtnInit = true;
      this.lockBtn.nativeElement.addEventListener('mouseover', () => this.isHoveringLockBtn = true );
      this.lockBtn.nativeElement.addEventListener('mouseout', () => this.isHoveringLockBtn = false );
    }
  }

  ngOnInit(): void {
    this.permessoGestImpostazioni = this.permessiService.getPermesso(enum_Security_Attivita.Gest_UtentiImpostazioni, enum_TipoPermesso.LETTURA_SCRITTURA);
    this.permessoEdit = this.permessiService.getPermesso(enum_Security_Attivita.Gest_UtentiProfili, enum_TipoPermesso.LETTURA_SCRITTURA);
    this.profileService.selectedProfile$.GiasSubscribe(profile => this.selectedProfile = profile);
  }

  /**
   * @history
   * (09/05/2024): Da riunione con Valerio si è deciso di rimuovere il pulsante di sincronizzazione in favore di
   * una sincronizzazione automatica ogni volta che si agisce sui permessi.
   */
  onSyncConnectedUsers() {
    this.profileService.updateConnectedUsers(this.selectedProfile);
  }

  onEditPermissions() {
    this.profileService.isEditing.next(true);
  }

  selectPermission(event: any) {
    let selected = event.selectedRows.at(0).dataItem;
    this.utentiService.selectedPermission = [selected];
  }

  private resizeWindow(): void {
    let winRefs = document.getElementsByClassName('k-window ng-star-inserted');
    for (let i = 0; i < winRefs.length; i++) {
      if (!winRefs[i]?.classList.contains('resized') || (winRefs[i]['style']['zIndex'] ?? '') !== '10000') {
        winRefs[i]?.setAttribute('style', 'z-index:10000 !important; top: 5%; left: 5%; width: 90%; height: 90%;');
        winRefs[i]?.classList.add('resized');
      }
    }
    winRefs = document.getElementsByClassName('k-content k-window-content ng-star-inserted');
    for (let i = 0; i < winRefs.length; i++)
      winRefs[i].setAttribute('style', 'overflow: auto !important');
  }

}
