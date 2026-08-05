import { Component, OnDestroy } from '@angular/core';
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { GisClient, PermessiXUtente, ProvisioningClient, SalvaPermessiLayerUtenti_In } from 'app/Service/api.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { debounceTime, finalize, map, Observable, of, Subject, Subscription, switchMap, tap } from 'rxjs';
import { GISLayerPermissionsWindowService } from '../GIS-layer-permissions-window.service';
import {MasterService} from '../../../Service/master.service';
import {ObjParametriAgendaService} from '../../../Service/obj-parametri-agenda.service';

@Component({
  standalone: false,
  selector: 'gis-layer-users-permissions',
  templateUrl: './GIS-layer-users-permissions.component.html',
  styleUrls: ['./GIS-layer-users-permissions.component.css']
})
export class GISLayerUsersPermissionsComponent implements OnDestroy {
  data: PermessiXUtenteResult[] = [];
  users: User[] = [];
  faDelete = faTrashAlt;
  pageSize = 5;
  isModalOpen = false;
  loading = false;
  newUser: User | null = null;
  filterSettings: DropDownFilterSettings = { caseSensitive: false, operator: "contains" };

  private allUsers: User[] = [];
  private save$ = new Subject<void>();
  private subscriptions: Subscription[] = [];

  constructor(
    private gisClient: GisClient,
    private provisioningClient: ProvisioningClient,
    private gisLayerPermissionsWindowService: GISLayerPermissionsWindowService,
    private giasMessageService: GiasMessageService,
    private masterService: MasterService,
    private objParametriService: ObjParametriAgendaService
  ) {
    this.subscriptions.push(
      this.gisLayerPermissionsWindowService
        .layerSelected$
        .pipe(
          switchMap(() => this.loadData()),
          tap(data => this.data = data),
          tap(() => this.computeTotAuths()),
          tap(() => this.updateUsers())
        )
        .subscribe()
    );

    this.subscriptions.push(
      this.save$
        .pipe(debounceTime(500))
        .subscribe(() => this.doSave())
    );

    this.provisioningClient
      .provisioningListaUtentiDatiBase()
      .pipe(tap(data => {
        this.allUsers = data.RispostaStringa?.ListaDatiBaseUtente?.map(x => ({
          usernameValue: x.UserName,
          usernameDescrValue: `${x.Nome} ${x.Cognome}`,
          usernameText: x.Nome == '' ? x.UserName : `${x.UserName} (${x.Nome} ${x.Cognome})`
        })) ?? [];
      }))
      .subscribe(() => this.updateUsers());
  }

  ngOnDestroy(): void {
    for (const sub of this.subscriptions) {
      sub.unsubscribe();
    }
  }

  addPermesso(): void {
    if (this.newUser == null) {
      return;
    }

    this.data = [...this.data, PermessiXUtenteResult.fromOriginal({
      Username: this.newUser.usernameValue,
      UsernameDescr: this.newUser.usernameDescrValue,
      Flag_Amministrazione: 1,
      Flag_Cancellazione: 1,
      Flag_Informazioni: 1,
      Flag_Inserimento: 1,
      Flag_Modifica: 1,
      Flag_Rimozione: 1
    })];

    this.updateUsers();
    this.newUser = null;
    this.isModalOpen = false;
    this.gisLayerPermissionsWindowService.addRemainingAuths(1);
    this.save$.next();
  }

  updatePermesso(permesso: PermessiXUtenteResult, rowIndex: number, flagName: string): void {
    if (!this.masterService.isSuperuserUsername(permesso.Username) || this.masterService.isSuperuserUsername(this.masterService.getCurrentUserUsername())) {
      this.save$.next();
    } else {
      // following two rows are necessary to prevent the flag to be displayed FALSE instead of TRUE
      document.querySelectorAll(`#${flagName}`)[rowIndex]['checked'] = true;
      permesso[flagName] = true;

      this.giasMessageService.warningMessage('SuperUserPermissionsEditNotAllowed', false, true);
      return;
    }
  }

  updatePermessoAmministra($event: Event, rowIndex: number, permesso: PermessiXUtenteResult): void {
    if (!this.masterService.isSuperuserUsername(permesso.Username) || this.masterService.isSuperuserUsername(this.masterService.getCurrentUserUsername())) {
      const remainingAuths: number = this.gisLayerPermissionsWindowService.currentRemainingAuths;

      // Permesso amministra aggiunto
      if (($event.currentTarget as HTMLInputElement).checked) {
        this.gisLayerPermissionsWindowService.addRemainingAuths(1);
        this.save$.next();
        return;
      }

      // Permesso amministra rimosso, ma non ho altre auth
      if (remainingAuths <= 1) {
        $event.preventDefault();

        // following two rows are necessary to prevent the flag to be displayed FALSE instead of TRUE
        document.querySelectorAll('#Flag_Amministrazione')[rowIndex]['checked'] = true;
        permesso.Flag_Amministrazione = true;

        this.giasMessageService.errorMessage('gis.PermessoUltimoUtente', false, true);
        return;
      }

      // Permesso amministra rimosso correttamente
      this.gisLayerPermissionsWindowService.addRemainingAuths(-1);
      this.save$.next();
    } else {
      // following two rows are necessary to prevent the flag to be displayed FALSE instead of TRUE
      document.querySelectorAll('#Flag_Amministrazione')[rowIndex]['checked'] = true;
      permesso.Flag_Amministrazione = true;
      this.giasMessageService.warningMessage('SuperUserPermissionsEditNotAllowed', false, true);
      return;
    }
  }

  deletePermesso(permesso: PermessiXUtenteResult): void {
    if (!this.masterService.isSuperuserUsername(permesso.Username) || this.masterService.isSuperuserUsername(this.masterService.getCurrentUserUsername())) {
      const remainingAuths = this.gisLayerPermissionsWindowService.currentRemainingAuths;
      if (remainingAuths <= 1) {
        this.giasMessageService.errorMessage('gis.PermessoUltimoUtente', false, true);
        return;
      }

      this.data = this.data.filter(x => x != permesso);
      this.updateUsers();
      this.gisLayerPermissionsWindowService.addRemainingAuths(-1);
      this.save$.next();
    } else {
      this.giasMessageService.warningMessage('SuperUserPermissionsDeletionNotAllowed', false, true);
      return;
    }
  }

  private loadData(): Observable<PermessiXUtenteResult[]> {
    const layer = this.gisLayerPermissionsWindowService.currentLayerSelected;
    if (layer == null) {
      return of([]);
    }

    this.loading = true;

    return this.gisClient
      .gisLeggiElencoPermessiLayerUtenti({ Layer_Cod: +layer.id })
      .pipe(
        map(data => data.RispostaStringa.utenti_permessi.map(x => PermessiXUtenteResult.fromOriginal(x))),
        tap(() => this.updateUsers()),
        finalize(() => this.loading = false)
      );
  }

  private computeTotAuths(): void {
    const currentAuths = this.data.filter(x => x.Flag_Amministrazione).length;
    this.gisLayerPermissionsWindowService.addRemainingAuths(currentAuths);
  }

  private updateUsers(): void {
    this.users = this.allUsers.filter(user => this.data.find(permesso => permesso.Username == user.usernameValue) == null);
  }

  private doSave(): void {
    const payload = {
      Layer_Cod: +this.gisLayerPermissionsWindowService.currentLayerSelected.id,
      utenti_permessi: this.data.map(x => x.toOriginal())
    } as SalvaPermessiLayerUtenti_In;

    this.gisClient
      .gisSalvaPermessiLayerUtenti(payload)
      .subscribe(() => this.giasMessageService.successMessage('gis.PermessiSalvaOk', false, true));
  }
}

class PermessiXUtenteResult {
  constructor(
    public Username: string,
    public UsernameDescr: string,
    public UsernameDisplay: string,
    public Flag_Inserimento: boolean,
    public Flag_Modifica: boolean,
    public Flag_Cancellazione: boolean,
    public Flag_Informazioni: boolean,
    public Flag_Amministrazione: boolean,
    public Flag_Rimozione: boolean
  ) { }

  toOriginal(): PermessiXUtente {
    return {
      Username: this.Username,
      UsernameDescr: this.UsernameDescr,
      Flag_Inserimento: +this.Flag_Inserimento,
      Flag_Modifica: +this.Flag_Modifica,
      Flag_Cancellazione: +this.Flag_Cancellazione,
      Flag_Informazioni: +this.Flag_Informazioni,
      Flag_Amministrazione: +this.Flag_Amministrazione,
      Flag_Rimozione: +this.Flag_Rimozione
    } as PermessiXUtente;
  }

  static fromOriginal(permesso: PermessiXUtente): PermessiXUtenteResult {
    return new PermessiXUtenteResult(
      permesso.Username,
      permesso.UsernameDescr,
      permesso.UsernameDescr.trim() == '' ? permesso.Username : `${permesso.Username} (${permesso.UsernameDescr})`,
      permesso.Flag_Inserimento == 1,
      permesso.Flag_Modifica == 1,
      permesso.Flag_Cancellazione == 1,
      permesso.Flag_Informazioni == 1,
      permesso.Flag_Amministrazione == 1,
      permesso.Flag_Rimozione == 1
    );
  }
}

interface User {
  usernameText: string;
  usernameDescrValue: string;
  usernameValue: string;
}
