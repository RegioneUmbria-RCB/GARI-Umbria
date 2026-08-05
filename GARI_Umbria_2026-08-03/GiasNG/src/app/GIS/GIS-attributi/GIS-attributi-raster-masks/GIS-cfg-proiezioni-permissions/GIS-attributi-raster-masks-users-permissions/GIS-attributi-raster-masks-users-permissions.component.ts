import { Component, Input, OnChanges } from '@angular/core';
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { GisClient, MascheraLayerRaster, ModifichePermessiMaschera, ProvisioningClient } from 'app/Service/api.service';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from 'app/Service/FunzioniComuni.service';
import { BehaviorSubject, combineLatest, finalize, map, Subject, tap } from 'rxjs';
import { GISAttributiRasterMasksPermissionsGrid } from '../GIS-attributi-raster-masks-permissions-grid';
import { IPermissionSubmit, ProjectionPermissionOperation, PrejectionPermissionUser, PermessoConfigurazioneResult } from '../GIS-attributi-raster-masks-permissions.component';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { GisAttributiRasterMasksPermissionsService } from '../GIS-attributi-raster-masks-permissions.service';

@Component({
  standalone: false,
  selector: 'gis-attributi-raster-masks-users-permissions',
  templateUrl: './GIS-attributi-raster-masks-users-permissions.component.html',
  styleUrls: ['./GIS-attributi-raster-masks-users-permissions.component.css'],
})
export class GISAttributiRasterMasksUsersPermissionsComponent extends GISAttributiRasterMasksPermissionsGrid implements IPermissionSubmit, OnChanges {
  @Input() mask: MascheraLayerRaster;

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
    gisAttributiRasterMasksPermissionsService: GisAttributiRasterMasksPermissionsService,
    giasMessageService: GiasMessageService,
  ) {
    super(giasMessageService, gisAttributiRasterMasksPermissionsService);

    this.provisioningClient
      .provisioningListaUtentiDatiBase()
      .pipe(
        map(data => data.RispostaStringa?.ListaDatiBaseUtente?.map(x => PrejectionPermissionUser.fromOriginal(x))),
        tap(users => this.usersSubject.next(users))
      ).subscribe();
  }

  ngOnChanges() {
    this.loadData(this.mask);
  }

  override add(): void {
    if (this.newUser == null) {
      return;
    }

    const newPermission = new PermessoConfigurazioneResult(this.newUser.usernameText, 0, '', true, true, true, true, true, true, ProjectionPermissionOperation.ADDED);
    this.gisAttributiRasterMasksPermissionsService.addRemainingAuths(1);
    this.permissionsSubject.next([...this.permissionsSubject.value, newPermission]);

    this.newUser = null;
    this.isModalOpen = false;
  }

  override loadData(mask: MascheraLayerRaster): void {
    this.loading = true;
    this.gisClient
      .gisLeggiPermessiUtenteMaschera(mask.maschera_cod)
      .pipe(
        map(data => (data.RispostaStringa?.elencoPermessiMaschera ?? []).map(x => PermessoConfigurazioneResult.fromOriginal(x))),
        tap(permissions => this.permissionsSubject.next(permissions)),
        tap(permissions => this.addTotAuths(permissions)),
        finalize(() => this.loading = false)
      )
      .subscribe();
  }

  private getPayload(data: PermessoConfigurazioneResult[]): ModifichePermessiMaschera | null {
    const payload = {
      Maschera_Cod: this.mask.maschera_cod,
      InsertPermessi: data.filter(x => x.operation == ProjectionPermissionOperation.ADDED).map(x => ({ ...x.toOriginal(), Gruppi_Utente_cod: 0 })),
      UpdatePermessi: data.filter(x => x.operation == ProjectionPermissionOperation.UPDATED).map(x => ({ ...x.toOriginal(), Gruppi_Utente_cod: 0 })),
      DeletePermessi: data.filter(x => x.operation == ProjectionPermissionOperation.DELETED).map(x => ({ ...x.toOriginal(), Gruppi_Utente_cod: 0 }))
    } as ModifichePermessiMaschera;

    return payload;
  }
}
