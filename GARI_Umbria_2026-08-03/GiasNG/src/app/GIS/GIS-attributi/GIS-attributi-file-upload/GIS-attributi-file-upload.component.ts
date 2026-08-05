import { Component } from '@angular/core';
import { OrigineDatiModel } from 'app/GIS/GIS-attributi/GIS-attributi-file-upload/GIS-attributi-file-upload.service';
import { enum_LayerElementiGraficiStd } from 'app/GIS/GIS-enum/GIS-layer-elementi-grafici';
import { LayerService } from 'app/GIS/services/layer.service';
import { FeatureType } from 'app/Model/GIS/GisDataReadRval_New';
import { TipologiaLayer } from 'app/Service/api.service';
import { BehaviorSubject, map, tap } from 'rxjs';

const CATASTO_ORIGINE_DATI = [
  { codice: 4, descrizione: 'ZIP di particelle Catastali (formato DXF, SHP)', restrictions: { allowedExtensions: ['.zip'] } },
] as OrigineDatiModel[];

const RASTER_ORIGINE_DATI = [
  { codice: 100, descrizione: 'Raster', restrictions: { allowedExtensions: ['.tif', '.tiff', '.ecw', '.jp2', '.zip'] } },
] as OrigineDatiModel[];

const DEFAULT_ORIGINE_DATI = [
  { codice: 5, descrizione: 'ZIP di ShapeFile generici', restrictions: { allowedExtensions: ['.zip'] } },
  { codice: 7, descrizione: 'Kml / Kmz', restrictions: { allowedExtensions: ['.kml', '.kmz'] } },
] as OrigineDatiModel[];

@Component({
  standalone: false,
  selector: 'gis-attributi-file-upload',
  templateUrl: './GIS-attributi-file-upload.component.html',
  styleUrls: ['./GIS-attributi-file-upload.component.css']
})
export class GISAttributiFileUploadComponent {
  private origineDatiSubject = new BehaviorSubject<OrigineDatiModel>(null);

  origine$ = this.origineDatiSubject.asObservable();
  origineDati$ = this.layerService
    .layerItemSelected$
    .pipe(
      map(layer => this.filterByLayer(layer[0])),
      tap(origini => this.changeOrigine(origini[0]))
    );

  constructor(private layerService: LayerService) { }

  changeOrigine($event: OrigineDatiModel): void {
    this.origineDatiSubject.next($event);
  }

  private filterByLayer(layer: TipologiaLayer): OrigineDatiModel[] {
    if (layer?.id == enum_LayerElementiGraficiStd.CATASTO) {
      return CATASTO_ORIGINE_DATI;
    }

    if (+layer?.FeatureTypeId == FeatureType.Raster) {
      return RASTER_ORIGINE_DATI;
    }

    return DEFAULT_ORIGINE_DATI;
  }
}
