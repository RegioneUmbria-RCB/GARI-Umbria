import { Component, EventEmitter, Input, Output, ViewChild } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import { ConfigurazioneProiezione, DatiBaseUtente, DatiGruppoUtente, GisClient, ModifichePermessiConfigurazione, PermessoConfigurazione } from "app/Service/api.service";
import { GiasDialogService } from "app/Service/gias-dialog.service";
import { BehaviorSubject, catchError, Observable, of, switchMap, tap } from "rxjs";

@Component({
  standalone: false,
  selector: 'gis-cfg-proiezioni-permissions',
  templateUrl: './GIS-cfg-proiezioni-permissions.component.html',
  styleUrls: ['./GIS-cfg-proiezioni-permissions.component.css']
})
export class GISCfgProziezioniPermissionsComponent {
  @ViewChild('gisCfgProiezioniUsersPermissions') gisCfgProiezioniUsersPermissions: IPermissionSubmit;
  @ViewChild('gisCfgProiezioniGroupsPermissions') gisCfgProiezioniGroupsPermissions: IPermissionSubmit;

  @Input() inputConfiguration: ConfigurazioneProiezione | null;
  @Output() close = new EventEmitter<void>();

  tabSelectedSubject = new BehaviorSubject<number>(0);
  tabSelected$ = this.tabSelectedSubject.asObservable();
  width = window.innerWidth * 0.8;

  constructor(
    private gisClient: GisClient,
    private giasDialogService: GiasDialogService,
    private translocoService: TranslocoService
  ) { }

  submit(): void {
    let error = "";

    const sub = this.gisCfgProiezioniUsersPermissions.savePayload$
      .pipe(
        switchMap(users => this.gisClient.gisOperazioniPermessiProiezioni(users)),
        catchError(error => {
          error += FunzioniComuniService.getResponseError(error, this.translocoService, 'SiÈVerificatoUnErroreDuranteLaFaseDiSalvat') + '\n';
          return of(null);
        }),
        switchMap(() => this.gisCfgProiezioniGroupsPermissions.savePayload$),
        switchMap(groups => this.gisClient.gisOperazioniPermessiProiezioni(groups)),
        catchError(error => {
          error += FunzioniComuniService.getResponseError(error, this.translocoService, 'SiÈVerificatoUnErroreDuranteLaFaseDiSalvat') + '\n';
          return of(null);
        }),
        tap(() => {
          this.close.next();
          sub.unsubscribe();

          if (error == "") {
            this.giasDialogService.baseSuccess('', 'gis.PermessiSalvaOk')
          } else {
            this.giasDialogService.baseError('', error, false)
          }
        })
      ).subscribe();
  }

  closeDialog(): void {
    this.close.emit();
  }
}

export interface IPermissionSubmit {
  savePayload$: Observable<ModifichePermessiConfigurazione>;
}

export class PrejectionPermissionUser {
  constructor(
    public usernameText: string,
    public usernameDescrValue: string,
    public usernameValue: string,
  ) { }

  static fromOriginal(user: DatiBaseUtente): PrejectionPermissionUser {
    return new PrejectionPermissionUser(
      user.UserName,
      `${user.Nome} ${user.Cognome}`,
      user.Nome == '' ? user.UserName : `${user.UserName} (${user.Nome} ${user.Cognome})`
    );
  }
}

export class PrejectionPermissionGroup {
  constructor(
    public groupName: string,
    public groupId: number
  ) { }

  static fromOriginal(group: DatiGruppoUtente): PrejectionPermissionGroup {
    return new PrejectionPermissionGroup(
      group.Descrizione,
      +group.Codice
    );
  }
}

export class PermessoConfigurazioneResult {
  constructor(
    public Username: string,
    public Gruppi_Utente_cod: number | undefined,
    public groupName: string | undefined,
    public Flag_Inserimento: boolean,
    public Flag_Modifica: boolean,
    public Flag_Cancellazione: boolean,
    public Flag_Informazioni: boolean,
    public Flag_Amministrazione: boolean,
    public Flag_GestioneInteroLayer: boolean,
    public operation: ProjectionPermissionOperation
  ) { }

  toOriginal(): PermessoConfigurazione {
    return {
      UserName: this.Username,
      Gruppi_Utente_cod: this.Gruppi_Utente_cod,
      Gruppi_Utente_des: this.groupName,
      Flag_Inserimento: +this.Flag_Inserimento,
      Flag_Modifica: +this.Flag_Modifica,
      Flag_Cancellazione: +this.Flag_Cancellazione,
      Flag_Informazioni: +this.Flag_Informazioni,
      Flag_Amministrazione: +this.Flag_Amministrazione,
      Flag_GestioneInteroLayer: +this.Flag_GestioneInteroLayer
    } as PermessoConfigurazione;
  }

  static fromOriginal(permesso: PermessoConfigurazione): PermessoConfigurazioneResult {
    return new PermessoConfigurazioneResult(
      permesso.UserName,
      permesso.Gruppi_Utente_cod,
      permesso.Gruppi_Utente_des,
      permesso.Flag_Inserimento == 1,
      permesso.Flag_Modifica == 1,
      permesso.Flag_Cancellazione == 1,
      permesso.Flag_Informazioni == 1,
      permesso.Flag_Amministrazione == 1,
      permesso.Flag_GestioneInteroLayer == 1,
      ProjectionPermissionOperation.READ
    );
  }
}

export enum ProjectionPermissionOperation {
  READ,
  ADDED,
  UPDATED,
  DELETED
}
