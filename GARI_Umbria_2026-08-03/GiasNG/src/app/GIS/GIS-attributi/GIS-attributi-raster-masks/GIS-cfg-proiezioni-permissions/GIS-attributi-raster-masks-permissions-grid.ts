import { BehaviorSubject } from 'rxjs';
import { PermessoConfigurazioneResult, ProjectionPermissionOperation } from './GIS-attributi-raster-masks-permissions.component';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { GisAttributiRasterMasksPermissionsService } from './GIS-attributi-raster-masks-permissions.service';
import { MascheraLayerRaster } from 'app/Service/api.service';

export abstract class GISAttributiRasterMasksPermissionsGrid {
  permissionsSubject = new BehaviorSubject<PermessoConfigurazioneResult[]>([]);

  constructor(
    protected giasMessageService: GiasMessageService,
    protected gisAttributiRasterMasksPermissionsService: GisAttributiRasterMasksPermissionsService
  ) { }

  abstract add(): void;
  abstract loadData(mask: MascheraLayerRaster): void;

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
    const remainingAuths = this.gisAttributiRasterMasksPermissionsService.currentRemainingAuths;

    // Permesso amministra aggiunto
    if (($event.currentTarget as HTMLInputElement).checked) {
      this.gisAttributiRasterMasksPermissionsService.addRemainingAuths(1);
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
    this.gisAttributiRasterMasksPermissionsService.addRemainingAuths(-1);
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
      const remainingAuths = this.gisAttributiRasterMasksPermissionsService.currentRemainingAuths;
      // If last administrator don't remove it
      if (remainingAuths <= 1) {
        this.giasMessageService.errorMessage('gis.PermessoUltimoUtente', false, true);
        return;
      }

      // Update admin counter
      this.gisAttributiRasterMasksPermissionsService.addRemainingAuths(-1);
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
    this.gisAttributiRasterMasksPermissionsService.addRemainingAuths(currentAuths);
  }
}
