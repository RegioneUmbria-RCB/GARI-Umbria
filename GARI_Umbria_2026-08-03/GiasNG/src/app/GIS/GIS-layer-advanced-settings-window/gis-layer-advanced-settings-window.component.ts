import { Component, OnDestroy, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { CancelEvent, EditEvent, GridComponent, RemoveEvent, SaveEvent } from '@progress/kendo-angular-grid';
import { TextBoxComponent } from '@progress/kendo-angular-inputs';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { AggiornaElementoGraficoPerTipoOggetto_In, AttivaAttributoLayer_In, AttributoLayer, AttributoLayer_In, AttributoLayer_In_OperazioneAttributo, Gis_Traduzione, GisClient, ImpostaCampoChiaveLayer_In, ImpostaVisualizzazioneEtichetta_In, LeggiImpostazioniAvanzateLayer, RispostaStandard_1OfLeggiImpostazioniAvanzateLayer, TipologiaLayer } from 'app/Service/api.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { catchError, finalize, Observable, of, Subscription, switchMap, tap } from 'rxjs';
import { DdlFeatureTypeElement } from '../GIS-layer-visibility-configuration-window/GIS-layer-visibility-configuration-window.component';
import { GISLayerAdvancedSettingsWindowService } from './GIS-layer-advanced-settings-window.service';
import { GoogleMapGeoJsonService } from '../google-map/google-map-geojson.service';
import { FeatureType } from '../../Model/GIS/GisDataReadRval_New';
import { GisClient as NetCoreApiGisClient, TraduzioniLayerLabel_In } from 'app/Service/net-core6-api.service';
import { LayerService } from '../services/layer.service';

@Component({
  standalone: false,
  selector: 'gis-layer-advanced-settings-window',
  templateUrl: './gis-layer-advanced-settings-window.component.html',
  styleUrls: ['./gis-layer-advanced-settings-window.component.css']
})
export class GISLayerAdvancedSettingsWindowComponent implements OnDestroy {
  @ViewChild('textbox') textbox: TextBoxComponent;
  @ViewChild('grid') grid: GridComponent;

  formGroup: FormGroup;
  windowArgs: WindowArgs;
  loading: boolean = false;
  attributesData: AttributoLayer[] = [];
  featureTypeElements: DdlFeatureTypeElement[] = [];
  featureTypeSelectedId: number;
  saveVisibile: boolean = false;

  attributeTranslations: Gis_Traduzione[] | null = null;

  private editedRowIndex: number;
  private data: LeggiImpostazioniAvanzateLayer;
  private newAttributeName: string;
  private subscriptions: Subscription[] = [];
  traduzioniPayload: TraduzioniLayerLabel_In = null;

  constructor(
    private kendoWindowsService: KendoWindowsService,
    private gisLayerAdvancedSettingsWindowService: GISLayerAdvancedSettingsWindowService,
    private gisClient: GisClient,
    private giasMessageService: GiasMessageService,
    private transloco: TranslocoService,
    private giasDialogService: GiasDialogService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private netCoreApiGisClient: NetCoreApiGisClient,
    private layerService: LayerService
  ) {
    this.popolaDdlFeatureTypeElements();

    this.subscriptions.push(
      this.kendoWindowsService
        .windowToggle$
        .subscribe(([windowTypes, args]) => {
          if (windowTypes === WindowTypes.LayerAdvancedSettingsWindow) {
            this.windowArgs = args;
            if (this.grid) {
              this.closeEditor(this.grid);
            }
          }
        })
    );

    this.subscribeToNewData();
  }

  private popolaDdlFeatureTypeElements(): void {
    let featureTypes: DdlFeatureTypeElement[] = [
      new DdlFeatureTypeElement(FeatureType.Polygon, this.transloco.translate('Poligono')),
      new DdlFeatureTypeElement(FeatureType.Point, this.transloco.translate('Punto')),
      new DdlFeatureTypeElement(FeatureType.LineString, this.transloco.translate('Linea'))
    ];
    this.featureTypeElements.push(...featureTypes);
  }

  ngOnDestroy(): void {
    for (const sub of this.subscriptions) {
      sub.unsubscribe();
    }

    this.gisLayerAdvancedSettingsWindowService.selectLayer(null);
  }

  cambioChiaveAttributo(item: AttributoLayer): void {
    item.CampoChiave = item.CampoChiave === '0' ? '1' : '0';
    const payload: ImpostaCampoChiaveLayer_In = {
      Impostazione: item.CampoChiave === '1',
      ProgressivoDataStruct: item.ProgressivoDataStruct
    };

    this.gisClient
      .gisImpostaCampoChiaveLayer(payload)
      .subscribe(() => {
        this.giasMessageService.successMessage('gis.AttributoSalvaOk', false, true)
        this.layerService.setLayerAttributesById(+this.gisLayerAdvancedSettingsWindowService.currentLayerSelected.id, this.attributesData);
      });
  }

  cambioAttivitaAttributo(item: AttributoLayer): void {
    item.TemaAttivo = item.TemaAttivo === '0' ? '1' : '0';
    const payload: AttivaAttributoLayer_In = {
      IdLayer: this.gisLayerAdvancedSettingsWindowService.currentLayerSelected.id,
      ProgressivoDataStruct: item.ProgressivoDataStruct,
      Attivazione: item.TemaAttivo === '1'
    };

    this.gisClient
      .gisImpostaAttivazioneAttributoLayer(payload)
      .pipe(
        catchError((a, c) => {
          this.giasMessageService.errorMessage(a.message, false, true);
          return of(JSON.parse(a.response));
        })
      ).subscribe((r) => {
        if (r.RispostaOK) {
          this.giasMessageService.successMessage('gis.AttributoSalvaOk', false, true);
          this.layerService.setLayerAttributesById(+this.gisLayerAdvancedSettingsWindowService.currentLayerSelected.id, this.attributesData);
        }
      });
  }

  cambioVisibilitaEtichettaAttributo(item: AttributoLayer): void {
    item.EtichettaVisibile = !item.EtichettaVisibile;
    const payload: ImpostaVisualizzazioneEtichetta_In = {
      Impostazione: item.EtichettaVisibile,
      ProgressivoDataStruct: item.ProgressivoDataStruct,
    };

    this.gisClient
      .gisImpostaVisualizzazioneEtichettaLayer(payload)
      .pipe(
        catchError((a, c) => {
          this.giasMessageService.errorMessage(a.message, false, true);
          return of(JSON.parse(a.response));
        })
      ).subscribe((r) => {
        if (r.RispostaOK) {
          this.giasMessageService.successMessage('gis.AttributoSalvaOk', false, true);

          this.layerService.setLayerAttributesById(+this.gisLayerAdvancedSettingsWindowService.currentLayerSelected.id, this.attributesData);
          // ricarico le feature per ottenere una descrizione aggiornata delle stesse (AppIdRate)
          this.googleMapGeoJsonService.loadGeoJsonForzato(false);
        }
      });
  }

  cambioTipoOggetto(): void {
    const payload: AggiornaElementoGraficoPerTipoOggetto_In = {
      GIS_TipoOggetto_Cod: this.featureTypeSelectedId,
      GIS_TipoOggetto_Cod_Prev: Number.parseInt(this.data.TipoGIS),
      LayerElementiGrafici_Cod: Number.parseInt(this.gisLayerAdvancedSettingsWindowService.currentLayerSelected.id)
    };

    this.gisClient
      .gisAggiornaElementoGraficoPerTipoOggetto(payload)
      .subscribe(() => {
        this.giasMessageService.successMessage('gis.AttributoSalvaOk', false, true);
        this.subscribeToNewData();
      });
  }

  change(event: any): void {
    if (event === '') {
      this.saveVisibile = false;
      return;
    }

    this.newAttributeName = event;
    this.saveVisibile = true;
  }

  newAttributeToggle(): void {
    if (!this.saveVisibile) {
      this.textbox.focus();
      return;
    }

    if (this.saveVisibile && this.newAttributeName !== '') {
      this.gisClient
        .gisAttributoLayer({
          IdLayer: this.gisLayerAdvancedSettingsWindowService.currentLayerSelected.id,
          IdOperazione: AttributoLayer_In_OperazioneAttributo.INSERT,
          Nome: this.newAttributeName,
          TipoDato: 'string'
        } as AttributoLayer_In)
        .subscribe({
          next: _ => {
            this.giasDialogService.baseSuccess('gis.SalvataggioAttributo', 'gis.SalvataggioNuovoAttributoSuccesso');
            this.textbox.value = '';
            this.saveVisibile = false;
            this.subscribeToNewData();
          },
          error: _ => {
            this.giasDialogService.baseError('gis.SalvataggioAttributo', 'gis.ErroreSalvataggioAttributo');
          }
        });
    }
  }

  protected editAttributo(args: EditEvent): void {
    this.closeEditor(args.sender);

    this.formGroup = new FormGroup({
      NomeAttributo: new FormControl(args.dataItem.NomeAttributo)
    });

    this.editedRowIndex = args.rowIndex;
    args.sender.editRow(args.rowIndex, this.formGroup);
  }

  protected updateAttributo(args: SaveEvent): void {
    let newName = this.formGroup.value.NomeAttributo;

    this.gisClient
      .gisAttributoLayer({
        IdLayer: this.gisLayerAdvancedSettingsWindowService.currentLayerSelected.id,
        IdOperazione: AttributoLayer_In_OperazioneAttributo.UPDATE,
        ProgressivoDataStruct: args.dataItem.ProgressivoDataStruct,
        Nome: newName
      } as AttributoLayer_In)
      .subscribe({
        next: _ => {
          this.giasMessageService.successMessage('gis.AttributoSalvaOk', false, true)
          this.subscribeToNewData();
        }
      });

    this.closeEditor(args.sender);
  }

  protected cancelEditAttributo(args: CancelEvent): void {
    this.closeEditor(args.sender);
  }

  private closeEditor(grid: GridComponent): void {
    grid.closeRow(this.editedRowIndex);
    this.editedRowIndex = undefined;
    this.formGroup = undefined;
  }

  protected deleteAttributo(args: RemoveEvent): void {
    this.gisClient
      .gisAttributoLayer({
        IdLayer: this.gisLayerAdvancedSettingsWindowService.currentLayerSelected.id,
        IdOperazione: AttributoLayer_In_OperazioneAttributo.DELETE,
        ProgressivoDataStruct: args.dataItem.ProgressivoDataStruct
      } as AttributoLayer_In)
      .subscribe({
        next: _ => {
          this.giasMessageService.successMessage('gis.AttributoSalvaOk', false, true)
          this.subscribeToNewData();
        }
      });
  }

  isBloccato(): boolean {
    return this.data != undefined && this.data.Bloccato != '0';
  }

  manageTranslations(attributo: AttributoLayer): void {
    this.traduzioniPayload = {
      TipologiaLayer_struct_Cod: attributo.ProgressivoDataStruct,
      Layer_Cod: this.gisLayerAdvancedSettingsWindowService.currentLayerSelected.id,
      TipologiaLayer_Cod: '1',
      Traduzioni: attributo.Traduzioni
    };
  }

  attributeTranslationsUpdated(translations: Gis_Traduzione[]): void {
    const body = translations.reduce((input, translation) => {
      input.Traduzioni.push({
        Lingua_Cod: translation.Lingua_Cod,
        Traduzione: translation.Traduzione
      });

      return input;
    }, {
      Layer_Cod: this.traduzioniPayload.Layer_Cod,
      TipologiaLayer_Cod: this.traduzioniPayload.TipologiaLayer_Cod,
      TipologiaLayer_struct_Cod: this.traduzioniPayload.TipologiaLayer_struct_Cod,
      Traduzioni: []
    } as TraduzioniLayerLabel_In);

    this.netCoreApiGisClient
      .gisUpdateLayerLabelTranslations(body)
      .pipe(catchError(() => of(false))
      ).subscribe(res => res//this.layerService.refreshLayers(this.layerService.LayerSelected.value));
        ? this.giasMessageService.successMessage(this.transloco.translate('gis.TraduzioniSalvateCorrettamente'))
        : this.giasMessageService.errorMessage(this.transloco.translate('gis.ErroreSalvataggioTraduzioni')));
  }

  private subscribeToNewData(): void {
    this.subscriptions.push(
      this.gisLayerAdvancedSettingsWindowService
        .layerSelected$
        .pipe(
          switchMap(() => this.loadData()),
          tap(data => this.assignData(data.RispostaStringa))
        )
        .subscribe()
    );
  }

  private assignData(data: LeggiImpostazioniAvanzateLayer): void {
    this.data = data;
    this.attributesData = this.data.ListaAttributiLayer;
    this.featureTypeSelectedId = Number.parseInt(this.data.TipoGIS);
  }

  private loadData(): Observable<RispostaStandard_1OfLeggiImpostazioniAvanzateLayer> {
    const layer: TipologiaLayer = this.gisLayerAdvancedSettingsWindowService.currentLayerSelected;
    if (layer == null) {
      return of();
    }

    this.loading = true;

    return this.gisClient
      .gisLeggiImpostazioniAvanzateLayer(+layer.id)
      .pipe(
        tap(res => this.layerService.setLayerAttributesById(+this.gisLayerAdvancedSettingsWindowService.currentLayerSelected.id, res.RispostaStringa.ListaAttributiLayer)),
        finalize(() => this.loading = false)
      );
  }
}