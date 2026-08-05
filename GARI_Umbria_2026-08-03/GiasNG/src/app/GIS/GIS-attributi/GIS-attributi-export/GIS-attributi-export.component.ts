import { Component, ViewChild } from '@angular/core';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS, FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { Subject, catchError, combineLatest, finalize, forkJoin, map, of, startWith, tap, withLatestFrom } from 'rxjs';
import { GISAttributiExportGridConfig } from './GIS-attributi-export.component-grid-config.service';
import { EsportaShapeEntita_In, GisClient, ObjOptionHTML_Out, TipologiaLayer } from 'app/Service/api.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { enum_FeatureProperty } from 'app/GIS/GIS-enum/GIS-feature';
import { enum_LayerElementiGraficiStd } from 'app/GIS/GIS-enum/GIS-layer-elementi-grafici';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { TranslocoService } from '@jsverse/transloco';
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { FormBuilder } from '@angular/forms';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { FeatureInformationService } from 'app/GIS/services/feature-information.service';

const DEFAULT_EXPORT_DATA = [{ id: 1, name: 'Shapefile Esri' } as ExportDataType];

@Component({
  standalone: false,
  selector: 'gis-attributi-export',
  templateUrl: './GIS-attributi-export.component.html',
  styleUrls: ['./GIS-attributi-export.component.css'],
  providers: [...generateGridProviders(GISAttributiExportGridConfig, GISAttributiExportComponent)]
})
export class GISAttributiExportComponent {
  @ViewChild('kendoGrid') kendoGrid: GiasKendoGridComponent;

  filterSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;
  enum_LayerElementiGraficiStd = enum_LayerElementiGraficiStd;
  exportDataTypes = DEFAULT_EXPORT_DATA;
  loading = false;
  gridLoading = false;
  submitSubject = new Subject<void>();

  layer$ = this.layerService.layerItemSelected$.pipe(map(x => x[1] ? x[0] : null));
  bounds$ = this.googleMapService.idle$
    .pipe(
      map(() => this.googleMapService.googleMapWrapper.getBounds()),
      startWith(this.googleMapService.googleMapWrapper.getBounds()));
  featureUpdater$ = combineLatest([this.bounds$, this.layer$]).pipe(
    tap(([bounds, layer]) => this.countFeatures(bounds, layer)),
    map(() => true)
  );
  submit$ = this.submitSubject
    .asObservable()
    .pipe(
      withLatestFrom(this.bounds$),
      tap(([_, bounds]) => this.doSubmit(this.form.getRawValue(), bounds)),
      map(() => true)
    );

  form = this.formBuilder.group<AttributiExportData>({
    layer: this.layerService.layerItemSelected[0],
    layerType: this.layerService.LayerSelected.value,
    exportDataType: DEFAULT_EXPORT_DATA[0],
    description: null,
    readCartographicData: false,
    exportOnlyVisibleElementsMapArea: true,
    totalFeatures: 0
  });

  constructor(
    private layerService: LayerService,
    private googleMapService: GoogleMapService,
    private gisClient: GisClient,
    private giasDialogService: GiasDialogService,
    private translocoService: TranslocoService,
    private permessiUtenteService: PermessiUtenteService,
    private formBuilder: FormBuilder,
    private featureInformationService: FeatureInformationService
  ) {
    this.form.controls.description.markAsDirty();
  }

  get hasTotalExportPermissions(): boolean {
    return this.permessiUtenteService.getPermesso(enum_Security_Attivita.GisBulkExportSuLayer, 0);
  }

  isImpiantiLayer(layer: TipologiaLayer | null): boolean {
    return layer?.id == enum_LayerElementiGraficiStd.IMPIANTI;
  }

  download(id: number): void {
    this.gisClient
      .gisLeggiAllegatoDocumento(id)
      .pipe(catchError(error => {
        this.giasDialogService.baseError("", FunzioniComuniService.getResponseError(error, this.translocoService, 'SiÈVerificatoUnErroreDuranteLaFaseDiSalvat'), false);
        return of(null);
      }))
      .subscribe(res => {
        if (res == null) {
          return;
        }

        let data = res.RispostaStringa.file_Allegato_DB;
        let filename = res.RispostaStringa.fileName;
        this.saveAndOpenFileByteArray(filename, data);
      });
  }

  reloadData(): void {
    this.gridLoading = true;
    this.kendoGrid.forceReload();
    setTimeout(() => this.gridLoading = false, 1000);
  }

  private countFeatures(bounds: google.maps.LatLngBounds, layer: TipologiaLayer): void {
    if (bounds == null || layer == null) {
      this.form.controls.totalFeatures.setValue(0);
      return;
    }

    let totalCounter: number = this.featureInformationService.getByLayer(layer.id).length;
    this.form.controls.totalFeatures.setValue(totalCounter);
  }

  private doSubmit(data: AttributiExportData, bounds: google.maps.LatLngBounds): void {
    if (data.description == null || data.description == '') {
      this.giasDialogService.baseError('', 'gis.SpecificareUnaDescrizione', true);
      return;
    }

    const features = this.featureInformationService.getByLayer(data.layer.id).map(f => +f.properties.Entita_Cod);
    if (features.length == 0) {
      this.giasDialogService.baseError('', 'gis.NessunaFeatureDaEsportare', true);
      return;
    }

    const payload: EsportaShapeEntita_In = {
      applicaFiltroTabellaUtentiVisibilitaAppoggio: !data.exportOnlyVisibleElementsMapArea,
      fullLayerExport: !data.exportOnlyVisibleElementsMapArea,
      descrizioneEsportazione: data.description,
      layerElementiGrafici_Cod: +data.layer.id,
      tipologiaLayer_Cod: +data.layerType.Option_Value,
      elencoEntita: data.exportOnlyVisibleElementsMapArea ? features : [],
      idEsp: 0,
      inizioValidita: AGRODATAINIZIO,
      fineValidita: AGRODATAFINE
    };

    let dialog: DialogRef | null = null;
    if (!payload.fullLayerExport) {
      // const title = this.translocoService.translate('gis.ExportEntitaIniziatoXNome', { nome: data.description });
      // dialog = this.giasDialogService.baseInfo('', title, false);
    }

    this.loading = true;
    forkJoin([
      this.gisClient.gisEsportaShapeEntita(payload),
      dialog == null ? of(null) : dialog.result
    ])
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: res => {
          dialog?.close();
          this.giasDialogService.baseSuccess('', res[0].RispostaStringa, false, false);
        },
        error: err => {
          dialog?.close();
          this.giasDialogService.baseError('', FunzioniComuniService.getResponseError(err, this.translocoService), false);
        },
        complete: () => this.reloadData()
      })
  }

  private saveAndOpenFileByteArray(filename: string, data: string): void {
    const buffer = require('buffer').Buffer.from(data, 'base64');
    const byteArray = new Uint8Array(buffer);
    const url = window.URL || window.webkitURL;
    const blob = new Blob([byteArray], { type: 'application/zip' });

    const downloadLink = document.createElement('a');
    downloadLink.target = '_blank';
    downloadLink.href = url.createObjectURL(blob);
    downloadLink.download = filename;
    document.body.append(downloadLink);
    downloadLink.click();

    // cleanup: remove element and revoke object URL
    document.body.removeChild(downloadLink);
    url.revokeObjectURL(url.toString());
  };
}

interface ExportDataType {
  id: number;
  name: string;
}

interface AttributiExportData {
  layer: TipologiaLayer;
  layerType: ObjOptionHTML_Out;
  exportDataType: ExportDataType;
  description: string;
  readCartographicData: boolean;
  exportOnlyVisibleElementsMapArea: boolean;
  totalFeatures: number;
}



