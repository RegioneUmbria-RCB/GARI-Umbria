import { Component, Input, OnChanges } from '@angular/core';
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { GisClient, ModifichePermessiConfigurazione, ProvisioningClient } from 'app/Service/api.service';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from 'app/Service/FunzioniComuni.service';
import { BehaviorSubject, combineLatest, finalize, map, Subject, tap } from 'rxjs';
import { GISCfgProiezioniPermissionsGrid } from '../GIS-cfg-proiezioni-permissions-grid';
import { IPermissionSubmit, ProjectionPermissionOperation, PrejectionPermissionUser, PermessoConfigurazioneResult } from '../GIS-cfg-proiezioni-permissions.component';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { GisCfgProiezioniPermissionsService } from '../GIS-cfg-proiezioni-permissions.service';

@Component({
  standalone: false,
  selector: 'gis-cfg-proiezioni-users-permissions',
  templateUrl: './GIS-cfg-proiezioni-users-permissions.component.html',
  styleUrls: ['./GIS-cfg-proiezioni-users-permissions.component.css'],
})
export class GISCfgProiezioniUsersPermissionsComponent extends GISCfgProiezioniPermissionsGrid implements IPermissionSubmit, OnChanges {
  @Input() configurationCode: number;

  saveSubject = new Subject<void>();
  usersSubject = new BehaviorSubject<PrejectionPermissionUser[]>([]);

  permissions$ = this.permissionsSubject.asObservable();
  filteredPermissions$ = this.permissions$.pipe(map(operations => operations.filter(x => x.Username != '' && x.operation != ProjectionPermissionOperation.DELETED)));
  users$ = combineLatest([this.usersSubject.asObservable(), this.permissions$])
    .pipe(map(([users, permissions]) => users.filter(user => permissions.find(permission => permission.Username == user.usernameValue) == null)));
  savePayload$ = this.permissions$.pipe(map((permissions) => this.getPayload(permissions)));

  faDelete = faTrashAlt;
  pageSize = 5;
  isModalOpen = false;
  loading = false;
  newUser: PrejectionPermissionUser | null = null;
  filterSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;

  constructor(
    private provisioningClient: ProvisioningClient,
    private gisClient: GisClient,
    gisCfgProiezioniPermissionsService: GisCfgProiezioniPermissionsService,
    giasMessageService: GiasMessageService,
  ) {
    super(giasMessageService, gisCfgProiezioniPermissionsService);

    this.provisioningClient
      .provisioningListaUtentiDatiBase()
      .pipe(
        map(data => data.RispostaStringa?.ListaDatiBaseUtente?.map(x => PrejectionPermissionUser.fromOriginal(x))),
        tap(users => this.usersSubject.next(users))
      ).subscribe();
  }

  ngOnChanges() {
    this.loadData(this.configurationCode);
  }

  override add(): void {
    if (this.newUser == null) {
      return;
    }

    const newPermission = new PermessoConfigurazioneResult(this.newUser.usernameText, 0, '', true, true, true, true, true, true, ProjectionPermissionOperation.ADDED);
    this.gisCfgProiezioniPermissionsService.addRemainingAuths(1);
    this.permissionsSubject.next([...this.permissionsSubject.value, newPermission]);

    this.newUser = null;
    this.isModalOpen = false;
  }

  override loadData(configurationCode: number): void {
    this.loading = true;
    this.gisClient
      .gisLeggiPermessiUtenteConfigurazioni(configurationCode)
      .pipe(
        map(data => data.RispostaStringa.elencoPermessiConfigurazione.map(x => PermessoConfigurazioneResult.fromOriginal(x))),
        tap(permissions => this.permissionsSubject.next(permissions)),
        tap(permissions => this.addTotAuths(permissions)),
        finalize(() => this.loading = false)
      )
      .subscribe();
  }

  private getPayload(data: PermessoConfigurazioneResult[]): ModifichePermessiConfigurazione | null {
    const payload = {
      LayerAnalysisConfig_Cod: this.configurationCode,
      InsertPermessi: data.filter(x => x.operation == ProjectionPermissionOperation.ADDED).map(x => ({ ...x.toOriginal(), Gruppi_Utente_cod: 0 })),
      UpdatePermessi: data.filter(x => x.operation == ProjectionPermissionOperation.UPDATED).map(x => ({ ...x.toOriginal(), Gruppi_Utente_cod: 0 })),
      DeletePermessi: data.filter(x => x.operation == ProjectionPermissionOperation.DELETED).map(x => ({ ...x.toOriginal(), Gruppi_Utente_cod: 0 }))
    } as ModifichePermessiConfigurazione;

    return payload;
  }
}
