import { BehaviorSubject } from 'rxjs';
import { PermessoConfigurazioneResult, ProjectionPermissionOperation } from './GIS-cfg-proiezioni-permissions.component';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { GisCfgProiezioniPermissionsService } from './GIS-cfg-proiezioni-permissions.service';

export abstract class GISCfgProiezioniPermissionsGrid {
  permissionsSubject = new BehaviorSubject<PermessoConfigurazioneResult[]>([]);

  constructor(
    protected giasMessageService: GiasMessageService,
    protected gisCfgProiezioniPermissionsService: GisCfgProiezioniPermissionsService
  ) { }

  abstract add(): void;
  abstract loadData(configurationCode: number): void;

  update(permission: PermessoConfigurazioneResult): void {
    const permissions = this.permissionsSubject.value;
    const value = permissions.find(x => x == permission);
    if (value == null) {
      return;
    }

    if (value.operation != ProjectionPermissionOperation.ADDED) {
      value.operation = ProjectionPermissionOperation.UPDATED;
    }

    this.permissionsSubject.next(permissions);
  }

  updatePermessoAmministra($event: Event, permission: PermessoConfigurazioneResult): void {
    const remainingAuths = this.gisCfgProiezioniPermissionsService.currentRemainingAuths;

    // Permesso amministra aggiunto
    if (($event.currentTarget as HTMLInputElement).checked) {
      this.gisCfgProiezioniPermissionsService.addRemainingAuths(1);
      this.update(permission);
      return;
    }

    // Permesso amministra rimosso, ma non ho altre auth
    if (remainingAuths <= 1) {
      $event.preventDefault();
      this.giasMessageService.errorMessage('gis.PermessoUltimoUtente', false, true);
      return;
    }

    // Permesso amministra rimosso correttamente
    this.gisCfgProiezioniPermissionsService.addRemainingAuths(-1);
    this.update(permission);
  }

  delete(permission: PermessoConfigurazioneResult): void {
    const permissions = this.permissionsSubject.value;
    const value = permissions.find(x => x == permission);
    if (value == null) {
      return;
    }

    // Check if it is administrator
    if (value.Flag_Amministrazione) {
      const remainingAuths = this.gisCfgProiezioniPermissionsService.currentRemainingAuths;
      // If last administrator don't remove it
      if (remainingAuths <= 1) {
        this.giasMessageService.errorMessage('gis.PermessoUltimoUtente', false, true);
        return;
      }

      // Update admin counter
      this.gisCfgProiezioniPermissionsService.addRemainingAuths(-1);
    }

    if (value.operation == ProjectionPermissionOperation.ADDED) {
      // If just added remove it
      this.permissionsSubject.next(permissions.filter(x => x != value));
      return;
    }

    // Mark it as deleted for db update
    value.operation = ProjectionPermissionOperation.DELETED;
    this.permissionsSubject.next(permissions);
  }

  protected addTotAuths(permissions: PermessoConfigurazioneResult[]): void {
    const currentAuths = permissions.filter(x => x.Flag_Amministrazione).length;
    this.gisCfgProiezioniPermissionsService.addRemainingAuths(currentAuths);
  }
}
