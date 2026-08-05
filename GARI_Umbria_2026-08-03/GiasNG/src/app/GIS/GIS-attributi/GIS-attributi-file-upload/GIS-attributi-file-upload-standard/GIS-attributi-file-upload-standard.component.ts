import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { SelectEvent } from '@progress/kendo-angular-upload';
import { GISAttributiFileUploadService, OrigineDatiModel, SistemaRiferimentoItem, SistemaRiferimentoModel } from 'app/GIS/GIS-attributi/GIS-attributi-file-upload/GIS-attributi-file-upload.service';
import { enum_OrigineChiamataLoadGeoJson } from 'app/GIS/GIS-enum/GIS-origine-chiamata';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { FileParameter, GisClient, RispostaStandard } from 'app/Service/api.service';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS, FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { DropdownListItem } from 'gias-kendo-grid';
import { TreeGisFiltersService } from 'app/Utility/Template/kendo-tree/filters/gis-tree-filters.service';
import { BehaviorSubject, combineLatest, filter, map, Observable, of, Subject, switchMap, tap, withLatestFrom } from 'rxjs';

@Component({
  standalone: false,
  selector: 'gis-attributi-file-upload-standard',
  templateUrl: './GIS-attributi-file-upload-standard.component.html',
  styleUrls: ['./GIS-attributi-file-upload-standard.component.css']
})
export class GISAttributiFileUploadStandardComponent implements OnChanges {
  @Input() origineDati: OrigineDatiModel;

  private origineDatiSubject = new BehaviorSubject<OrigineDatiModel>(null);
  private centroAziendaleSubject = new BehaviorSubject<DropdownListItem>(null);
  private sistemaRiferimentoSubject = new BehaviorSubject<SistemaRiferimentoItem>(null);
  private filesSubject = new BehaviorSubject<FileParameter[]>([]);
  private validityStartSubject = new BehaviorSubject<Date>(null);
  private validityEndSubject = new BehaviorSubject<Date>(null);
  private submitSubject = new Subject<void>();

  data$ = combineLatest({
    files: this.filesSubject.asObservable(),
    origine: this.origineDatiSubject.asObservable(),
    centro: this.centroAziendaleSubject.asObservable(),
    sistema: this.sistemaRiferimentoSubject.asObservable(),
    validityStart: this.validityStartSubject.asObservable(),
    validityEnd: this.validityEndSubject.asObservable(),
    loading: this.gisBottomWindowService.agendaLoading$
  });
  submit$: Observable<any>;
  centroAziendale$ = this.treeGisFiltersService.asObservable();
  sistemaRiferimento$ = this.origineDatiSubject.asObservable()
    .pipe(
      tap(() => this.changeSistemaRiferimento(null)),
      filter(x => x != null && x.codice != 0),
      switchMap(x => this.gisClient.gisLeggiSistemiRiferimento({ codice_tipologia: x.codice })),
      map(res => JSON.parse(res.RispostaStringa) as SistemaRiferimentoModel),
      tap(res => this.changeSistemaRiferimento(res.ListaSistemiRiferimento.find(x => x.Value == res.default))),
      map(res => res.ListaSistemiRiferimento)
    );

  filterSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;

  constructor(
    private gisClient: GisClient,
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
        tap(([_, data]) => this.doSubmit(data.files, data.origine, data.centro, data.sistema, data.validityStart, data.validityEnd))
      );
  }

  ngOnChanges(): void {
    this.origineDatiSubject.next(this.origineDati);
  }

  changeCentroAziendale($event: DropdownListItem): void {
    this.centroAziendaleSubject.next($event);
  }

  changeSistemaRiferimento($event: SistemaRiferimentoItem): void {
    this.sistemaRiferimentoSubject.next($event);
  }

  onFileSelected(event: SelectEvent): void {
    // There should be only one file
    const files = event.files.map(file => ({ fileName: file.name, data: file.rawFile }));
    this.filesSubject.next(files);
  }

  onFileRemoved(): void {
    this.filesSubject.next([]);
  }

  changeValidityStart($event: Date): void {
    this.validityStartSubject.next($event);
  }

  changeValidityEnd($event: Date): void {
    this.validityEndSubject.next($event);
  }

  submit(): void {
    this.submitSubject.next();
  }

  private doSubmit(files: FileParameter[], origine: OrigineDatiModel, centro: DropdownListItem, sistema: SistemaRiferimentoItem, validityStart: Date | null, validityEnd: Date | null): Observable<RispostaStandard> {
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

    if (sistema == null) {
      this.giasDialogService.baseError("gis.CaricamentoDati", "gis.AttributiWindowSistmaNonValido");
      return of(null);
    }

    const layer = this.layerService.layerItemSelected[0];

    this.giasDialogService.baseSuccess('', 'gis.AttributiWindowCaricamentoDati');
    this.gisBottomWindowService
      .caricaFileShape(file, origine, centro, sistema, validityStart, validityEnd, this.objParametriAgendaService.getObjParamValue().Piva, +layer.id)
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
