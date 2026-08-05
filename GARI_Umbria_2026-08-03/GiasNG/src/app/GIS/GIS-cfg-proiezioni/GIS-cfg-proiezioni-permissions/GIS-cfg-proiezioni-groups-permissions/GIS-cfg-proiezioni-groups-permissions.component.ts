import { Component, Input } from '@angular/core';
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { ProvisioningClient, ModifichePermessiConfigurazione, GisClient } from 'app/Service/api.service';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from 'app/Service/FunzioniComuni.service';
import { BehaviorSubject, combineLatest, finalize, map, tap } from 'rxjs';
import { GISCfgProiezioniPermissionsGrid } from '../GIS-cfg-proiezioni-permissions-grid';
import { IPermissionSubmit, PermessoConfigurazioneResult, PrejectionPermissionGroup, ProjectionPermissionOperation } from '../GIS-cfg-proiezioni-permissions.component';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { GisCfgProiezioniPermissionsService } from '../GIS-cfg-proiezioni-permissions.service';

@Component({
  standalone: false,
  selector: 'gis-cfg-proiezioni-groups-permissions',
  templateUrl: './GIS-cfg-proiezioni-groups-permissions.component.html',
  styleUrls: ['./GIS-cfg-proiezioni-groups-permissions.component.css'],
})
export class GISCfgProiezioniGroupsPermissionsComponent extends GISCfgProiezioniPermissionsGrid implements IPermissionSubmit {
  @Input() configurationCode: number;

  groupsSubject = new BehaviorSubject<PrejectionPermissionGroup[]>([]);

  permissions$ = this.permissionsSubject.asObservable();
  filteredPermissions$ = this.permissions$.pipe(map(operations => operations.filter(x => x.Gruppi_Utente_cod != 0 && x.operation != ProjectionPermissionOperation.DELETED)));
  groups$ = combineLatest([this.groupsSubject.asObservable(), this.permissions$])
    .pipe(map(([groups, permissions]) => groups.filter(group => permissions.find(permission => permission.Gruppi_Utente_cod == group.groupId) == null)));
  savePayload$ = this.permissions$.pipe(map((permissions) => this.getPayload(permissions)));

  faDelete = faTrashAlt;
  pageSize = 5;
  isModalOpen = false;
  loading = false;
  newGroup: PrejectionPermissionGroup | null = null;
  filterSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;

  constructor(
    private provisioningClient: ProvisioningClient,
    private gisClient: GisClient,
    gisCfgProiezioniPermissionsService: GisCfgProiezioniPermissionsService,
    giasMessageService: GiasMessageService,
  ) {
    super(giasMessageService, gisCfgProiezioniPermissionsService);

    this.provisioningClient
      .provisioningListaGruppiUtente()
      .pipe(
        map(data => data.RispostaStringa?.ListaDatiGruppoUtente?.map(x => PrejectionPermissionGroup.fromOriginal(x))),
        tap(groups => this.groupsSubject.next(groups)),
      ).subscribe();
  }

  ngOnChanges() {
    this.loadData(this.configurationCode);
  }

  override add(): void {
    if (this.newGroup == null) {
      return;
    }

    const newPermission = new PermessoConfigurazioneResult('', this.newGroup.groupId, this.newGroup.groupName, true, true, true, true, true, true, ProjectionPermissionOperation.ADDED);
    this.gisCfgProiezioniPermissionsService.addRemainingAuths(1);
    this.permissionsSubject.next([...this.permissionsSubject.value, newPermission]);

    this.newGroup = null;
    this.isModalOpen = false;
  }

  override loadData(configurationCode: number): void {
    this.loading = true;
    this.gisClient
      .gisLeggiPermessiGruppiUtenteConfigurazioni(configurationCode)
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
      InsertPermessi: data.filter(x => x.operation == ProjectionPermissionOperation.ADDED).map(x => ({ ...x.toOriginal(), UserName: '' })),
      UpdatePermessi: data.filter(x => x.operation == ProjectionPermissionOperation.UPDATED).map(x => ({ ...x.toOriginal(), UserName: '' })),
      DeletePermessi: data.filter(x => x.operation == ProjectionPermissionOperation.DELETED).map(x => ({ ...x.toOriginal(), UserName: '' }))
    } as ModifichePermessiConfigurazione;

    return payload;
  }
}
