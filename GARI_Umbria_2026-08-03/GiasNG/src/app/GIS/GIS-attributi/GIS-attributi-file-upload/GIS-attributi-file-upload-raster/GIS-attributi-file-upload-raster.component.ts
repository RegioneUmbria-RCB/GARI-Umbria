import { Component, Input, OnChanges } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { SelectEvent } from '@progress/kendo-angular-upload';
import { GISAttributiFileUploadService, OrigineDatiModel } from 'app/GIS/GIS-attributi/GIS-attributi-file-upload/GIS-attributi-file-upload.service';
import { enum_OrigineChiamataLoadGeoJson } from 'app/GIS/GIS-enum/GIS-origine-chiamata';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { FileParameter, RispostaStandard } from 'app/Service/api.service';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS, FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { DropdownListItem } from 'gias-kendo-grid';
import { TreeGisFiltersService } from 'app/Utility/Template/kendo-tree/filters/gis-tree-filters.service';
import { BehaviorSubject, combineLatest, Observable, of, Subject, tap, withLatestFrom } from 'rxjs';

@Component({
  standalone: false,
  selector: 'gis-attributi-file-upload-raster',
  templateUrl: './GIS-attributi-file-upload-raster.component.html',
  styleUrls: ['./GIS-attributi-file-upload-raster.component.css']
})
export class GISAttributiFileUploadRasterComponent implements OnChanges {
  @Input() origineDati: OrigineDatiModel;

  private origineDatiSubject = new BehaviorSubject<OrigineDatiModel>(null);
  private centroAziendaleSubject = new BehaviorSubject<DropdownListItem>(null);
  private filesSubject = new BehaviorSubject<FileParameter[]>([]);
  private descriptionSubject = new BehaviorSubject<string>(null);
  private validityStartSubject = new BehaviorSubject<Date>(null);
  private validityEndSubject = new BehaviorSubject<Date>(null);
  private pixelSizeSubject = new BehaviorSubject<number>(null);
  private submitSubject = new Subject<void>();

  data$ = combineLatest({
    files: this.filesSubject.asObservable(),
    origine: this.origineDatiSubject.asObservable(),
    centro: this.centroAziendaleSubject.asObservable(),
    loading: this.gisBottomWindowService.agendaLoading$,
    description: this.descriptionSubject.asObservable(),
    validityStart: this.validityStartSubject.asObservable(),
    validityEnd: this.validityEndSubject.asObservable(),
    pixelSize: this.pixelSizeSubject.asObservable(),
  });
  submit$: Observable<any>;
  centroAziendale$ = this.treeGisFiltersService.asObservable();
  filterSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;

  constructor(
    private treeGisFiltersService: TreeGisFiltersService,
    private giasDialogService: GiasDialogService,
    private layerService: LayerService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private gisBottomWindowService: GISAttributiFileUploadService,
    private sharedDataService: SharedDataService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private translocoService: TranslocoService
  ) {
    this.submit$ = this.submitSubject.asObservable()
      .pipe(
        withLatestFrom(this.data$),
        tap(([_, data]) => this.doSubmit(data.files, data.origine, data.centro, data.description, data.validityStart, data.validityEnd, data.pixelSize))
      );
  }

  ngOnChanges(): void {
    this.origineDatiSubject.next(this.origineDati);
  }

  changeCentroAziendale($event: DropdownListItem): void {
    this.centroAziendaleSubject.next($event);
  }

  onFileSelected(event: SelectEvent): void {
    // There should be only one file
    const files = event.files.map(file => ({ fileName: file.name, data: file.rawFile }));
    this.filesSubject.next(files);
  }

  onFileRemoved(): void {
    this.filesSubject.next([]);
  }

  changeDescription($event: string): void {
    this.descriptionSubject.next($event);
  }

  changeValidityStart($event: Date): void {
    this.validityStartSubject.next($event);
  }

  changeValidityEnd($event: Date): void {
    this.validityEndSubject.next($event);
  }

  changePixelSize($event: number): void {
    this.pixelSizeSubject.next($event);
  }

  submit(): void {
    this.submitSubject.next();
  }

  private doSubmit(files: FileParameter[], origine: OrigineDatiModel, centro: DropdownListItem, description: string, validityStart: Date | null, validityEnd: Date | null, pixelSize: number | null): Observable<RispostaStandard> {
    if (origine == null) {
      this.giasDialogService.baseError("gis.CaricamentoDati", "gis.AttributiWindowOrigineNonValida");
      return of(null);
    }

    if (centro == null) {
      this.giasDialogService.baseError("gis.CaricamentoDati", "gis.AttributiWindowCentroNonValido");
      return of(null);
    }

    const file = files[0];
    if (file?.data == null || !origine.restrictions.allowedExtensions.some(ext => file?.fileName.endsWith(ext))) {
      const title = this.translocoService.translate('gis.CaricamentoDati');
      const translate = this.translocoService.translate('gis.SelezionaFileValidoConEstensioneX', { extensions: origine.restrictions.allowedExtensions.join(', ') });
      this.giasDialogService.baseError(title, translate, false);
      return of(null);
    }

    if (description == null) {
      this.giasDialogService.baseError("gis.CaricamentoDati", "gis.AttributiWindowDescrizioneNonValida");
      return of(null);
    }

    const layer = this.layerService.layerItemSelected[0];

    this.giasDialogService.baseSuccess('', 'gis.AttributiWindowCaricamentoDati');
    this.gisBottomWindowService
      .caricaShapeFileRaster(file, origine, centro, description, validityStart, validityEnd, pixelSize ?? 0, this.objParametriAgendaService.getObjParamValue().Piva, +layer.id)
      .subscribe({
        next: () => this.giasDialogService.baseSuccess('', 'gis.AttributiWindowCaricamentoOk', true, false).then(() => this.reloadData()),
        error: error => this.giasDialogService.baseError('', FunzioniComuniService.getResponseError(error, this.translocoService, 'SiÈVerificatoUnErroreDuranteLaFaseDiSalvat'), false)
      });
  }

  private reloadData(): void {
    this.sharedDataService.setOrigineChiamataLoadGeoJson(enum_OrigineChiamataLoadGeoJson.CentraSuAzienda);
    this.googleMapGeoJsonService.loadGeoJsonForzato(true);
  }
}
