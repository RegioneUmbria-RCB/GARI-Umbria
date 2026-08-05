import { Component, Input, OnInit } from '@angular/core';
import { LayerService } from 'app/GIS/services/layer.service';
import { MappePrescrizioneRasterService } from 'app/GIS/services/mappe-prescrizione-raster.service';
import { ObjOptionHTML_Out, TipologiaLayer } from 'app/Service/api.service';
import { Observable } from 'rxjs';

@Component({
  standalone: false,
  selector: 'gis-layer-list-item-mappe-prescrizione',
  templateUrl: './GIS-layer-list-item-mappe-prescrizione.component.html',
  styleUrls: ['./GIS-layer-list-item-mappe-prescrizione.component.css']
})
export class GISLayerListItemMappePrescrizioneComponent implements OnInit {
  @Input() public dataItem: TipologiaLayer = null;
  @Input() public type: ObjOptionHTML_Out;

  visible$: Observable<boolean>;

  constructor(
    private mappePrescrizioneRasterService: MappePrescrizioneRasterService,
    private layerService: LayerService
  ) { }

  ngOnInit(): void {
    this.visible$ = this.mappePrescrizioneRasterService.visibility$;
    this.layerService.toggleLayerItemVisible(this.dataItem, false);
  }

  toggleVisible($event: Event, visible: boolean): void {
    $event.stopPropagation();
    if (visible) {
      this.mappePrescrizioneRasterService.clearOverlay();
    }
    // When turning on, the user must use the attachment picker (datiAgricolturaPrecisone operation)
  }

  openMappePrescrizioneConfig($event: Event, item: TipologiaLayer | null): void {
    $event.stopPropagation();

    // TODO
  }
}
