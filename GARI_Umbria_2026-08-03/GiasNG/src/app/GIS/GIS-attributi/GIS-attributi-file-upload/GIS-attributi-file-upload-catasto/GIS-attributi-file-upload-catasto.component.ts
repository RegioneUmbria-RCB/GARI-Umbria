import { Component, Input, OnChanges } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { SelectEvent } from '@progress/kendo-angular-upload';
import { DatiCatastoInput, FormatoDatiModel, GISAttributiFileUploadService, OrigineDatiModel, SistemaRiferimentoItem, SistemaRiferimentoModel } from 'app/GIS/GIS-attributi/GIS-attributi-file-upload/GIS-attributi-file-upload.service';
import { enum_OrigineChiamataLoadGeoJson } from 'app/GIS/GIS-enum/GIS-origine-chiamata';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { FileParameter, GisClient, RispostaStandard, TipoFileCatasto } from 'app/Service/api.service';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS, FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { BehaviorSubject, combineLatest, filter, map, Observable, of, Subject, switchMap, tap, withLatestFrom } from 'rxjs';

const DEFAULT_FORMATO_DATI = [
  { codice: TipoFileCatasto.CatastoDXF, descrizione: 'DXF in Formato Agenzia Entrate' },
  { codice: TipoFileCatasto.CatastoSHP, descrizione: 'Shapefile' },
] as FormatoDatiModel[];

const DEFAULT_LAYERS = [
  "FABBRICATI",
  "PARTICELLE",
  "STRADA",
  "ACQUE",
  "CONFINE",
  "LINEEVARIE",
  "SIMBOLI",
  "TESTI",
  "FIDUCIALI",
] as string[];

@Component({
  standalone: false,
  selector: 'gis-attributi-file-upload-catasto',
  templateUrl: './GIS-attributi-file-upload-catasto.component.html',
  styleUrls: ['./GIS-attributi-file-upload-catasto.component.css']
})
export class GISAttributiFileUploadCatastoComponent implements OnChanges {
  @Input() origineDati: OrigineDatiModel;

  private origineDatiSubject = new BehaviorSubject<OrigineDatiModel>(null);
  formatoDatiSubject = new BehaviorSubject<FormatoDatiModel>(DEFAULT_FORMATO_DATI[0]);
  filesSubject = new BehaviorSubject<FileParameter[]>([]);
  filtroParticelleCatastaliSubject = new BehaviorSubject<string>(null);
  listaLayersSubject = new BehaviorSubject<string[]>([DEFAULT_LAYERS[1], DEFAULT_LAYERS[3]]);
  listaLayersEditables = new BehaviorSubject<string[]>([DEFAULT_LAYERS[1], DEFAULT_LAYERS[3]]);
  sovrascriviSubject = new BehaviorSubject<number>(2);
  sistemaRiferimentoSubject = new BehaviorSubject<SistemaRiferimentoItem>(null);

  codiceBelfioreSubject = new BehaviorSubject<string>("Cod_comune");
  provinciaSubject = new BehaviorSubject<string>(null);
  comuneSubject = new BehaviorSubject<string>(null);
  sezioneCatastaleSubject = new BehaviorSubject<string>(null);
  foglioSubject = new BehaviorSubject<string>("foglio");
  particellaSubject = new BehaviorSubject<string>("part");
  subalternoSubject = new BehaviorSubject<string>(null);
  identificativoEsternoSubject = new BehaviorSubject<string>(null);

  submitSubject = new Subject<void>();

  data$ = combineLatest({
    origine: this.origineDatiSubject.asObservable(),
    sistema: this.sistemaRiferimentoSubject.asObservable(),
    fileCatasto: this.formatoDatiSubject.asObservable().pipe(map(x => x.codice)),
    files: this.filesSubject.asObservable(),
    loading: this.gisBottomWindowService.agendaLoading$,
    filtroParticelleCatastali: this.filtroParticelleCatastaliSubject.asObservable(),
    listaLayersDXFAgenziaEntrate: this.listaLayersSubject.asObservable().pipe(map(x => x.join(","))),
    creaLayerTestuale: of(false), // Sempre false perché nascosto

    codBelfiore: this.codiceBelfioreSubject.asObservable(),
    provincia: this.provinciaSubject.asObservable(),
    comune: this.comuneSubject.asObservable(),
    sezione: this.sezioneCatastaleSubject.asObservable(),
    foglio: this.foglioSubject.asObservable(),
    particella: this.particellaSubject.asObservable(),
    subalterno: this.subalternoSubject.asObservable(),
    identificativoEsterno: this.identificativoEsternoSubject.asObservable(),

    comportamentoImportazione: this.sovrascriviSubject.asObservable().pipe(map(x => +x)),
  }) as Observable<DatiCatasto>;
  formatoDati$ = of(DEFAULT_FORMATO_DATI);
  layers$ = of(DEFAULT_LAYERS);
  sistemaRiferimento$ = this.origineDatiSubject.asObservable()
    .pipe(
      tap(() => this.sistemaRiferimentoSubject.next(null)),
      filter(x => x != null && x.codice != 0),
      switchMap(x => this.gisClient.gisLeggiSistemiRiferimento({ codice_tipologia: x.codice })),
      map(res => JSON.parse(res.RispostaStringa) as SistemaRiferimentoModel),
      tap(res => this.sistemaRiferimentoSubject.next(res.ListaSistemiRiferimento.find(x => x.Value == res.default))),
      map(res => res.ListaSistemiRiferimento)
    );
  submit$: Observable<any>;

  tipoFileCatasto = TipoFileCatasto;
  filterSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;

  constructor(
    private gisClient: GisClient,
    private giasDialogService: GiasDialogService,
    private layerService: LayerService,
    private gisBottomWindowService: GISAttributiFileUploadService,
    private sharedDataService: SharedDataService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private translocoService: TranslocoService
  ) {
    this.submit$ = this.submitSubject.asObservable()
      .pipe(
        withLatestFrom(this.data$),
        tap(([_, data]: [any, DatiCatasto]) => this.doSubmit(data, data.sistema, data.files, data.origine))
      );
  }

  ngOnChanges(): void {
    this.origineDatiSubject.next(this.origineDati);
  }

  onFileSelected(event: SelectEvent): void {
    // There should be only one file
    const files = event.files.map(file => ({ fileName: file.name, data: file.rawFile }));
    this.filesSubject.next(files);
  }

  onFileRemoved(): void {
    this.filesSubject.next([]);
  }

  changeListaLayersSubject(selected: boolean, layer: string): void {
    const values = this.listaLayersSubject.value;
    const index = values.indexOf(layer);

    if (selected && index == -1) {
      values.push(layer);
    } else if (!selected && index != -1) {
      values.splice(index, 1);
    }

    this.listaLayersSubject.next(values);
  }

  isLayerChecked(layer: string): boolean {
    return this.listaLayersSubject.value.find(x => x == layer) != null;
  }
  isLayerSelectable(layer: string): boolean {
      return this.listaLayersEditables.value.find(x => x == layer) != null;
  }

  submit(): void {
    this.submitSubject.next();
  }

  private doSubmit(data: DatiCatastoInput, sistema: SistemaRiferimentoItem, files: FileParameter[], origine: OrigineDatiModel): Observable<RispostaStandard> {
    if (origine == null) {
      this.giasDialogService.baseError("gis.CaricamentoDati", "gis.AttributiWindowOrigineNonValida");
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

    const parsedData = data.fileCatasto == TipoFileCatasto.CatastoDXF ? this.getDxfData(data) : this.getShpData(data);
    const layer = this.layerService.layerItemSelected[0];

    this.giasDialogService.baseSuccess('', 'gis.AttributiWindowCaricamentoDati');
    this.gisBottomWindowService
      .caricaShapeFileCatasto(parsedData, sistema, file, origine, +layer.id)
      .subscribe({
        next: () => this.giasDialogService.baseSuccess('', 'gis.AttributiWindowCaricamentoOk', true, false).then(() => this.reloadData()),
        error: error => this.giasDialogService.baseError('', FunzioniComuniService.getResponseError(error, this.translocoService, 'SiÈVerificatoUnErroreDuranteLaFaseDiSalvat'), false)
      });
  }

  private getDxfData(data: DatiCatastoInput): DatiCatastoInput {
    return {
      ...data,
      codBelfiore: null,
      provincia: null,
      comune: null,
      sezione: null,
      foglio: null,
      particella: null,
      subalterno: null,
      identificativoEsterno: null
    };
  }

  private getShpData(data: DatiCatastoInput): DatiCatastoInput {
    return {
      ...data,
      filtroParticelleCatastali: null,
      listaLayersDXFAgenziaEntrate: null,
    };
  }

  private reloadData(): void {
    this.sharedDataService.setOrigineChiamataLoadGeoJson(enum_OrigineChiamataLoadGeoJson.CentraSuAzienda);
    this.googleMapGeoJsonService.loadGeoJsonForzato(true);
  }
}

interface DatiCatasto extends DatiCatastoInput {
  files: FileParameter[];
  origine: OrigineDatiModel;
  sistema: SistemaRiferimentoItem;
}
