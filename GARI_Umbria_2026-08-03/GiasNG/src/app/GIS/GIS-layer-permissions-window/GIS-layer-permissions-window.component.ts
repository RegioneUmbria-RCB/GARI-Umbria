import { Component, OnDestroy } from '@angular/core';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { PermessiXGruppiUtente, PermessiXUtente, TipologiaLayer } from 'app/Service/api.service';
import { Subscription } from 'rxjs';
import { GISLayerPermissionsWindowService } from './GIS-layer-permissions-window.service';

@Component({
  standalone: false,
  selector: 'gis-layer-permissions-window',
  templateUrl: './GIS-layer-permissions-window.component.html',
  styleUrls: ['./GIS-layer-permissions-window.component.css']
})
export class GISLayerPermissionsWindowComponent implements OnDestroy {
  windowArgs: WindowArgs;
  layer: TipologiaLayer;
  usersPermissions: PermessiXUtente[] = [];
  groupsPermission: PermessiXGruppiUtente[] = [];

  private subscriptions: Subscription[] = [];

  constructor(
    private kendoWindowsService: KendoWindowsService,
    private gisLayerPermissionsWindowService: GISLayerPermissionsWindowService
  ) {

    this.subscriptions.push(
      this.kendoWindowsService
        .windowToggle$
        .subscribe(([windowTypes, args]) => {
          if (windowTypes === WindowTypes.LayerPermissionWindow) {
            this.windowArgs = args;
          }
        })
    );
  }

  ngOnDestroy(): void {
    for (const sub of this.subscriptions) {
      sub.unsubscribe();
    }

    this.gisLayerPermissionsWindowService.selectLayer(null);
  }
}
