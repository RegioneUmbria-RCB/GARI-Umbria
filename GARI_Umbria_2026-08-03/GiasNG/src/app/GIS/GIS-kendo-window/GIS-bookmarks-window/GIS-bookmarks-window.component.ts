import { Component, HostListener, ViewChild } from '@angular/core';
import { KendoWindowsService, WindowTypes } from 'app/Service';
import { filter, map, take, tap } from 'rxjs';
import { GISBookmarkWindowGridConfigService } from './GIS-bookmarks-window-grid-config.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { GoogleMapMovementControlService } from 'app/GIS/services/google-map-movement-control.service';
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { GISBookmarksWindowService } from './GIS-bookmarks-window.service';
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { Bookmark } from 'app/Service/api.service';
import { NavigationStart, Router } from '@angular/router';

const LOCAL_STORAGE_KEY = 'GIS-last-position-visited';

@Component({
  standalone: false,
  selector: 'gis-bookmarks-window',
  templateUrl: './GIS-bookmarks-window.component.html',
  styleUrls: ['./GIS-bookmarks-window.component.css'],
  providers: [...generateGridProviders(GISBookmarkWindowGridConfigService, GISBookmarksWindowComponent)]
})
export class GISBookmarksWindowComponent {
  @ViewChild('kendoGrid') kendoGrid: GiasKendoGridComponent;

  windowArgs$ = this.kendoWindowsService
    .windowToggle$
    .pipe(
      filter(([windowTypes, _]) => windowTypes === WindowTypes.BookmarksWindow),
      map(([_, args]) => args),
    );

  firstLoad$ = this.windowArgs$
    .pipe(
      filter(args => args.openState),
      take(1)
    );

  lastPositionSaver$ = this.router.events
    .pipe(
      filter(event => event instanceof NavigationStart && event.url != '/GIS'),
      tap(() => this.saveLastPosition())
    );

  constructor(
    private kendoWindowsService: KendoWindowsService,
    private googleMapService: GoogleMapService,
    private googleMapMovementControlService: GoogleMapMovementControlService,
    private gisBookmarksWindowService: GISBookmarksWindowService,
    private router: Router
  ) {
    const lastPosition = localStorage.getItem(LOCAL_STORAGE_KEY);
    if (lastPosition != null) {
      const entry = JSON.parse(lastPosition);
      this.gisBookmarksWindowService.updateLastPosition(entry.lat, entry.lng, entry.zoom).subscribe(() => localStorage.removeItem(LOCAL_STORAGE_KEY));
    }
  }

  @HostListener("window:beforeunload", ["$event"])
  saveLastPosition(): void {
    const map = this.googleMapService.googleMapWrapper.googleMap;
    const center = map.getCenter();
    const entry = JSON.stringify({ lat: center.lat(), lng: center.lng(), zoom: map.getZoom() });
    localStorage.setItem(LOCAL_STORAGE_KEY, entry);
  }

  viewOnMap(dataItem: Bookmark): void {
    const map = this.googleMapService.googleMapWrapper.googleMap;
    const center = new google.maps.LatLng({ lat: dataItem.Center_Lat, lng: dataItem.Center_Lng });
    this.googleMapMovementControlService.goToPosition(map, center, dataItem.Zoom);
  }
}
