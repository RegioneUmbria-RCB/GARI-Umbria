import { Component, DoCheck, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { faBan, faBookOpen, faLock, faLockOpen, faPen } from '@fortawesome/free-solid-svg-icons';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { BaseCodeDescr } from 'app/Service/api.service';
import { MasterService } from 'app/Service/master.service';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { TipologieUtentiService } from 'app/profilazione/services/profili-permessi/tipologie-utenti.service';
import { take, map } from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';
import { enum_TipoPermesso } from 'app/profilazione/models/profilazione.model';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { ClientePermesso, ProfilazioneUtentiService } from 'app/profilazione/services/profilazione-utenti.service';
import { ClientePermessiGridService } from './cliente-permessi-grid.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
export const PERMS_CANNOT_BE_DISABLED = [
  enum_Security_Attivita.Gest_Menu, enum_Security_Attivita.Profilazione_NG
];

@Component({
  standalone: false,
  selector: 'app-cliente-permessi-grid',
  templateUrl: './cliente-permessi-grid.component.html',
  styleUrls: ['./cliente-permessi-grid.component.css'],
  providers: [
    ...generateGridProviders(ClientePermessiGridService, ClientePermessiGridComponent)
  ]
})
export class ClientePermessiGridComponent implements OnInit, DoCheck, OnDestroy {
  @ViewChild('unlockPermissionsEditBtn') lockBtn: ElementRef;
  @ViewChild('permessiAmministratoreGrid') permessiAmministratoreGrid: GiasKendoGridComponent;
  profiliForm: FormGroup;
  profiloSelezionato: BaseCodeDescr = undefined;
  public permessoEdit = true;
  public isEditingPermessi = false;
  public bloccaSbloccaTooltip: string;
  public readonly permission = {
    Read: enum_TipoPermesso.LETTURA,
    Write: enum_TipoPermesso.LETTURA_SCRITTURA,
    Disabled: enum_TipoPermesso.DISABILITATO
  };
  protected readonly faBook = faBookOpen;
  protected readonly faBan = faBan;
  protected readonly faPen = faPen;
  private isHoveringLockBtn = false;
  private isLockInit = false;

  constructor(
    private fb: FormBuilder,
    private masterService: MasterService,
    private profileService: TipologieUtentiService,
    private profilazioneUtentiService: ProfilazioneUtentiService,
    private translocoService: TranslocoService,
    private giasDialogService: GiasDialogService
  ) {
  }

  protected get lockIcon() {
    return this.isHoveringLockBtn ? faLockOpen : faLock;
  }

  private get selected(): ClientePermesso[] {
    return this.permessiAmministratoreGrid.rows
      .filter(r => r['Selected'] && !PERMS_CANNOT_BE_DISABLED.includes(r['codice']))
      .map(r => new ClientePermesso(r['codice'], r['id_operazione']));
  }

  ngDoCheck(): void {
    if (this.lockBtn && !this.isLockInit) {
      this.isLockInit = true;
      this.lockBtn.nativeElement.addEventListener('mouseover', () => this.isHoveringLockBtn = this.isEditingPermessi ? false : true);
      this.lockBtn.nativeElement.addEventListener('mouseout', () => this.isHoveringLockBtn = this.isEditingPermessi ? true : false);
    }
  }

  ngOnInit(): void {
    this.profiliForm = this.fb.group({
      profili: new FormControl({codice: 0, descrizione: this.translocoService.translate("Profili")})
    });
    this.profiliForm.controls['profili'].valueChanges.subscribe(r => {
      this.profiloSelezionato = r;
    });
    this.profileService.isEditing.subscribe(value => {
      this.isEditingPermessi = value;
      this.bloccaSbloccaTooltip = this.translocoService.translate(this.isEditingPermessi ? 'BloccaModifica' : 'SbloccaModifica');
    });
  }

  ngOnDestroy(): void {
    this.profileService.isEditing.next(false);
  }

  // public applicaProfilo(event: any): void {
  //   this.profilazioneUtentiService.readGerarchiaPermessi(this.profiloSelezionato.codice).pipe(take(1), map(r => {
  //     this.profilazioneUtentiService.$profiloApplicato.next(r);
  //   })).subscribe();
  // }

  onEditPermissions() {
    this.isLockInit = false;
    this.profileService.isEditing.next(!this.isEditingPermessi);
  }

  async openDdl(ddlEl: GiasDropDownTemplateSComponent) {
    switch (ddlEl.giasFormControlName) {
      case 'profili':
        if (ddlEl.listItems == undefined) {
          this.masterService.set_isLoading({isLoading: true, message: ''});
          this.profileService.readTipologie().pipe(take(1), map(result => {
            this.masterService.set_isLoading({isLoading: false, message: ''});
            ddlEl.listItems = result.map(r => ({codice: r.codice, descrizione: r.descrizione}));
          })).subscribe();
        }
        break;
    }
  }

  onSetPermissions(toAssign: enum_TipoPermesso) {
    if (this.hasErrors(toAssign)) {
      return;
    }
    //let permessi = this.getCurrentRows(toAssign);
    const permessi = this.selected;
    permessi.forEach(p => p.Id_Operazione = toAssign);
    this.masterService.set_isLoading({isLoading: true, message: ''});
    this.profilazioneUtentiService.aggiornaClientePermessi({
      UserName: this.masterService.objP_utenti.UtenteUsername,
      permessi: permessi
    }).subscribe((r) => {
      this.masterService.set_isLoading({isLoading: false, message: ''});
      this.permessiAmministratoreGrid.forceReload();
      if (r.RispostaOK)
        void this.giasDialogService.baseSuccess('Successo', 'Permessi aggiornati con successo.');
    });
  }

  private hasErrors(toAssign: enum_TipoPermesso): boolean {
    const selected = this.selected;
    if (selected.length === 0) {
      this.giasDialogService.baseError('Errore', 'NessunPermessoSelezionato');
      return true;
    }
    if (toAssign === enum_TipoPermesso.DISABILITATO && selected.some(x => x.Id_Attivita === enum_Security_Attivita.Gest_Menu)) {
      this.giasDialogService.baseError('ImpossibileDisabilitarePermesso', 'ErroreDisabilitazionePermessoAccessoSuperUser');
      return true;
    }
    return false;
  }

  private getCurrentRows(toAssign: enum_TipoPermesso): ClientePermesso[] {
    let permessiAttivi = this.permessiAmministratoreGrid.rows;
    permessiAttivi.map(riga => {
      if (riga['Selected'])
        riga['id_operazione'] = toAssign;
    });
    permessiAttivi = permessiAttivi.filter(row => row['id_operazione'] != 1);
    return permessiAttivi.map(permesso => ({
      Id_Servizio: 5,
      Id_Attivita: permesso['codice'],
      Id_Operazione: permesso['id_operazione']
    }));
  }
}
