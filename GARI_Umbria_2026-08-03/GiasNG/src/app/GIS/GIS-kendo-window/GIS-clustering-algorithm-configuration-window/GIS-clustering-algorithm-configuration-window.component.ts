import { Component } from "@angular/core";
import { KendoWindowsService, WindowArgs, WindowTypes } from "app/Service";
import { Observable, filter, map, startWith } from "rxjs";

@Component({
  standalone: false,
  selector: 'gis-clustering-algorithm-configuration-window',
  templateUrl: './GIS-clustering-algorithm-configuration-window.component.html',
  styleUrls: ['./GIS-clustering-algorithm-configuration-window.component.css']
})
export class GisClusteringAlgorithmConfigurationWindowComponent {

  windowArgs$: Observable<WindowArgs> = this.kendoWindowsService
    .windowToggle$
    .pipe(
      filter(([windowTypes, _]) => windowTypes == WindowTypes.ClusteringAlgorithmConfigurationWindow),
      map(([_, args]) => args)
    );

  isOpen$ = this.windowArgs$.pipe(
    map(args => args.openState),
    startWith(false)
  );

  constructor(
    private kendoWindowsService: KendoWindowsService
  ) { }
}
