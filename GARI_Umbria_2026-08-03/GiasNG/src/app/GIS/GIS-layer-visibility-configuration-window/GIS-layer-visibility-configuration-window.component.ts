import { Component, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { TextBoxComponent } from '@progress/kendo-angular-inputs';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { AggiornaElencoTipologie_In, FlagVisibilitaLayer, Gis_Traduzione, GisClient, ObjOptionHTML_Out, SalvaFlagVisibilitaLayer_In, TipologiaLayer } from 'app/Service/api.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import {Subject, tap, debounceTime, takeUntil, catchError, of} from 'rxjs';
import { GISWindowComponent } from '../GIS-window/GIS-window.component';
import { LayerService } from '../services/layer.service';
import { PermessiUtenteService } from '../../Service/permessi-utente.service';
import { enum_Security_Attivita } from '../../Model/TipiEnumerativi';
import { SharedDataService } from '../services/shared-data.service';
import { MasterService } from 'app/Service/master.service';
import { GiasDialogService } from '../../Service/gias-dialog.service';
import { FeatureType } from 'app/Model/GIS/GisDataReadRval_New';
import { GISLayerPermissionsWindowService } from '../GIS-layer-permissions-window/GIS-layer-permissions-window.service';
import { DEFAULT_TOP_POSITION } from '../GIS-toolbar/gis-toolbar.service';
import { GISLayerAdvancedSettingsWindowService } from '../GIS-layer-advanced-settings-window/GIS-layer-advanced-settings-window.service';
import { enum_TipologiaLayer } from '../GIS-enum/GIS-tipologia-layer';
import {faEllipsis} from '@fortawesome/free-solid-svg-icons';
import {take} from 'rxjs/operators';
import {enum_OrigineChiamataLoadGeoJson} from '../GIS-enum/GIS-origine-chiamata';
import {GoogleMapGeoJsonService} from '../google-map/google-map-geojson.service';
import {GiasDropDownButtonActionItem} from 'gias-ui-kit';
import {FeatureInformationService} from '../services/feature-information.service';
import {DialogRef} from '@progress/kendo-angular-dialog';
import { TraduzioniLayer_In } from 'app/Service/net-core6-api.service';
import { GisClient as NetCoreApiGisClient } from 'app/Service/net-core6-api.service';

export class DdlFeatureTypeElement {
  codice: number;
  descrizione: string;

  constructor(codice: number, descrizione: string) {
    this.codice = codice;
    this.descrizione = descrizione;
  }
}

@Component({
  standalone: false,
  selector: 'gis-layer-visibility-configuration-window',
  templateUrl: './GIS-layer-visibility-configuration-window.component.html',
  styleUrls: ['./GIS-layer-visibility-configuration-window.component.css']
})
export class GISLayerVisibilityConfigurationWindowComponent implements OnDestroy {
  @ViewChild('window') window: GISWindowComponent;
  @ViewChild('textbox') textbox: TextBoxComponent;
  @ViewChild('load') divLoad: ElementRef;

  windowArgs: WindowArgs;
  layersData: TipologiaLayer[] = [];
  isLoading: boolean = false;
  filterValue: string = '';
  featureTypeElements: DdlFeatureTypeElement[] = [];
  featureTypeSelectedId: number;
  enableNewLayerBtn: boolean = false;
  saveVisibile: boolean = false;
  traduzioniPayload: TraduzioniLayer_In = null;

  protected readonly faDots = faEllipsis;

  private selectedLayer: ObjOptionHTML_Out | null = {
    Option_Value: '',
    Option_DesValue: ''
  };

  private totalLayersData: TipologiaLayer[] = [];
  private openWindowEvent: boolean = false;
  private layersDataVisibleOpen: TipologiaLayer[] = [];
  private newLayerName: string = '';
  private saveNotifier: Subject<void> = new Subject<void>();
  private signal: Subject<void> = new Subject<void>();

  constructor(
    private kendoWindowsService: KendoWindowsService,
    private layerService: LayerService,
    private gisClient: GisClient,
    private netCoreApiGisClient: NetCoreApiGisClient,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private transloco: TranslocoService,
    private permessiUtenteService: PermessiUtenteService,
    private sharedDataService: SharedDataService,
    private masterService: MasterService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private gisLayerPermissionsWindowService: GISLayerPermissionsWindowService,
    private gisLayerAdvancedSettingsWindowService: GISLayerAdvancedSettingsWindowService,
    private featureInformationService: FeatureInformationService
  ) {
    this.popolaDdlFeatureTypeElements();

    this.kendoWindowsService.windowToggle$
      .pipe(takeUntil(this.signal)).subscribe(([windowTypes, args]) => {
      if (windowTypes === WindowTypes.LayerVisibilityConfigurationWindow) {
        this.openWindowEvent = true;
        this.windowArgs = args;
        this.featureTypeSelectedId = this.featureTypeElements[0].codice;
        const ricaricatiLayers = this.seRicaricaLayers();

        if (this.windowArgs.openState) {
          if (ricaricatiLayers === false) {
            this.loadLayersDataVisibleOpen();
          }
          this.openWindowEvent = false;
        }
      }
    });

    this.layerService.LayerSelected
      .pipe(takeUntil(this.signal)).subscribe(type => this.seRicaricaLayers(type));

    this.saveNotifier
      .pipe(debounceTime(1000))
      .subscribe(() => this.submit());

    this.enableNewLayerBtn = this.permessiUtenteService.getPermesso(enum_Security_Attivita.GIS_GestioneLayerPersonalizzati, 2);
  }

  ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
    this.saveNotifier.complete();
  }

  toggleVisible(item: TipologiaLayer): void {
    let newFlagVisibile: boolean = item.flagvisibile !== '1';
    this.impostaFlagVisibile(newFlagVisibile, item);
    this.saveNotifier.next(null);
  }

  toggleActive(item: TipologiaLayer): void {
    item.flagattivo = item.flagattivo === '0' ? '1' : '0';
    let newFlagVisibile: boolean = item.flagattivo === '1';
    this.impostaFlagVisibile(newFlagVisibile, item);
    this.saveNotifier.next(null);
  }

  newLayerToggle(): void {
    if (!this.saveVisibile) {
      this.textbox.focus();
      return;
    }

    if (this.saveVisibile && this.newLayerName !== '') {
      this.gisClient.gisScriviNuovoLayerPersonalizzato(
        {
          NomeLayer: this.newLayerName,
          MostraDescrizioneAssociata: '0',
          FeatureTypeId: this.featureTypeSelectedId
        }
      ).subscribe({
        next: _ => {
          this.giasDialogService.baseSuccess('Salvataggio layer', 'gis.SalvataggioNuovoLayerSuccesso');
          this.getActiveLayers(this.selectedLayer.Option_Value);
          this.layerService.refreshLayers(this.selectedLayer);
          this.textbox.value = '';
          this.saveVisibile = false;
        },
        error: _ => {
          this.giasDialogService.baseError('Salvataggio layer', 'gis.ErroreSalvataggioLayer');
        }
      });
    }
  }

  onFilterChange(): void {
    this.layersData = this.getFilteredLayers();
  }

  loadAllLayers(): void {
    this.getAllLayers(this.selectedLayer.Option_Value);
    this.filterValue = '';
  }

  loadActiveLayers(): void {
    this.getActiveLayers(this.selectedLayer.Option_Value);
    this.filterValue = '';
  }

  change(event: any): void {
    if (event === '') {
      this.saveVisibile = false;
      return;
    }

    this.newLayerName = event;
    this.saveVisibile = true;
  }

  managePermission(layer: TipologiaLayer): void {
    const defaultArgs: WindowArgs = new WindowArgs(
      WindowTypes.LayerPermissionWindow,
      true,
      'Permessi tipologia layer',
      undefined,
      1000,
      250,
      undefined,
      DEFAULT_TOP_POSITION,
      true,
      true,
      true,
      false,
      false,
      false,
      true
    );
    const args: WindowArgs = this.kendoWindowsService.getWindowArgs(WindowTypes.LayerPermissionWindow) ?? defaultArgs;
    args.title = layer.nome;

    this.gisLayerPermissionsWindowService.selectLayer(layer);
    this.kendoWindowsService.open(WindowTypes.LayerPermissionWindow, args, false);
  }

  openAdvancedSettingsWindow(layer: TipologiaLayer): void {
    const defaultArgs = new WindowArgs(
      WindowTypes.LayerAdvancedSettingsWindow,
      true,
      'Impostazioni avanzate layer',
      undefined,
      800,
      500,
      undefined,
      DEFAULT_TOP_POSITION,
      true,
      true,
      true,
      false,
      false,
      false,
      true
    );

    const args: WindowArgs = this.kendoWindowsService.getWindowArgs(WindowTypes.LayerAdvancedSettingsWindow) ?? defaultArgs;
    args.title = layer.nome;

    this.gisLayerAdvancedSettingsWindowService.selectLayer(layer);
    this.kendoWindowsService.open(WindowTypes.LayerAdvancedSettingsWindow, args, false);
  }

  calculateBodyHeight(): number {
    return this.windowArgs ? this.windowArgs.height * 0.64 : 0;
  }

  manageTranslations(layer: TipologiaLayer): void {
    this.traduzioniPayload = {
      Layer_Cod: layer.id,
      TipologiaLayer_Cod: '1', // TODO M
      Traduzioni: layer.Traduzioni
    };
  }

  translationsUpdated(translations: Gis_Traduzione[]): void {
    const body = translations.reduce((input, translation) => {
      input.Traduzioni.push({
        Lingua_Cod: translation.Lingua_Cod,
        Traduzione: translation.Traduzione
      });

      return input;
    }, { Layer_Cod: this.traduzioniPayload.Layer_Cod, TipologiaLayer_Cod: this.traduzioniPayload.TipologiaLayer_Cod, Traduzioni: [] } as TraduzioniLayer_In);

    this.netCoreApiGisClient
      .gisUpdateLayerTranslations(body)
      .pipe(
        catchError(() => of(false)),
        tap(res => res
        ? this.giasMessageService.successMessage(this.transloco.translate('gis.TraduzioniSalvateCorrettamente')) 
        : this.giasMessageService.errorMessage(this.transloco.translate('gis.ErroreSalvataggioTraduzioni'))),
      ).subscribe(() => this.layerService.refreshLayers(this.layerService.LayerSelected.value));
  }
  
  getActionsList(item: any): GiasDropDownButtonActionItem[] {
    return [
      {
        text: this.transloco.translate('ImpostazioniAvanzate'),
        icon: 'xi-option-edit',
        disabled: item.FlagAmministrazione != '1',
        click: () => this.openAdvancedSettingsWindow(item)
      },
      {
        text: this.transloco.translate('EliminaLayer'),
        icon: 'xi-option-delete',
        disabled: item.FlagAmministrazione != '1',
        click: () => this.tryDeleteCustomLayer(item.id)
      }
    ];
  }

  private submit(): void {
    const payload: SalvaFlagVisibilitaLayer_In = {
      TipologiaLayer_Cod: enum_TipologiaLayer.Entita, // "1"
      DatiVisibilitaLayer: this.layersData.map(x => <FlagVisibilitaLayer>{
        ID: x.id,
        Flag_Visibile: +x.flagvisibile,
        Flag_Attivo: +x.flagattivo
      }),
    };

    this.gisClient
      .gisSalvaFlagVisibilitaLayer(payload)
      .pipe(tap(() => this.layerService.LayerSelectedTrigger(this.selectedLayer)))
      .subscribe({
        next: _ => {
          this.giasMessageService.successMessage(this.transloco.translate('gis.SalvataggioConfigurazioneSuccesso'));
        },
        error: _ => {
          this.giasDialogService.baseError('Visibilità layer', 'gis.ErroreSalvataggioConfigurazione');
        },
        complete: () => console.log('gisSalvaVisibilitaLayer complete')
      });
  }

  private getActiveLayers(layerSelezionato: string): void {
    this.isLoading = true;
    this.masterService.set_isLoading({isLoading: true, component: this.divLoad});

    const payload: AggiornaElencoTipologie_In = {
      Layer_Selezionato: layerSelezionato,
      leggiLayerNonVisibili: true
    };

    this.layerService
      .readLayersFromBackend(payload)
      .subscribe(result => {
        this.layersData = result.RispostaStringa.ListaTipologieLayer;
        this.totalLayersData = this.layersData;
        this.masterService.set_isLoading({isLoading: false, component: this.divLoad});
        this.isLoading = false;
        if (this.openWindowEvent === true) {
          this.loadLayersDataVisibleOpen();
        }
      });
  }

  private getAllLayers(layerSelezionato: string): void {
    this.isLoading = true;
    this.masterService.set_isLoading({isLoading: true, component: this.divLoad});

    const payload: AggiornaElencoTipologie_In = {
      Layer_Selezionato: layerSelezionato,
      leggiLayerNonAttivi: true,
      leggiLayerNonVisibili: true
    };

    this.layerService
      .readLayersFromBackend(payload)
      .subscribe(result => {
        this.layersData = result.RispostaStringa.ListaTipologieLayer;
        this.totalLayersData = this.layersData;
        this.masterService.set_isLoading({isLoading: false, component: this.divLoad});
        this.isLoading = false;
      });
  }

  private getFilteredLayers(): TipologiaLayer[] {
    return this.filterValue !== '' ? this.totalLayersData
      .filter(x => x.nome.toLowerCase().includes(this.filterValue.toLowerCase()))
      .sort((a, b) => +a?.zindex - +b?.zindex) : this.totalLayersData;
  }

  private popolaDdlFeatureTypeElements(): void {
    let featureTypes: DdlFeatureTypeElement[] = [
      new DdlFeatureTypeElement(FeatureType.Polygon, this.transloco.translate('Poligono')),
      new DdlFeatureTypeElement(FeatureType.Point, this.transloco.translate('Punto')),
      new DdlFeatureTypeElement(FeatureType.LineString, this.transloco.translate('Linea')),
      new DdlFeatureTypeElement(FeatureType.Raster, this.transloco.translate('Raster'))
    ];
    this.featureTypeElements.push(...featureTypes);
  }

  private seRicaricaLayers(layerSelezionato?: ObjOptionHTML_Out): boolean {
    let ricaricatiLayers: boolean = false;

    if (this.windowArgs?.openState) {
      if (layerSelezionato == undefined) {
        layerSelezionato = {
          Option_Value: this.sharedDataService.getTipoLayerSelezionato(),
          Option_DesValue: ''
        };
      }

      if (this.selectedLayer.Option_Value !== layerSelezionato.Option_Value) {
        this.layersData = [];
        this.selectedLayer.Option_Value = layerSelezionato.Option_Value;
        this.selectedLayer.Option_DesValue = layerSelezionato.Option_DesValue;
        this.getActiveLayers(this.selectedLayer.Option_Value);
        ricaricatiLayers = true;
      }
    }

    return ricaricatiLayers;
  }

  private loadLayersDataVisibleOpen(): void {
    this.layersDataVisibleOpen = this.totalLayersData.filter(layer => layer.flagvisibile === '1');
  }

  private seRicaricaFeature(): void {
    let nuoviLayerVisibili: boolean = false;
    let layersDataVisibleClose: TipologiaLayer[] = this.totalLayersData.filter(layer => layer.flagvisibile === '1');

    if (layersDataVisibleClose.length > this.layersDataVisibleOpen.length) {
      nuoviLayerVisibili = true;
    } else {
      for (let i = 0; i < layersDataVisibleClose.length; i++) {
        let layerIndex: number = this.layersDataVisibleOpen.findIndex(layer => layer.id === layersDataVisibleClose[i].id);

        if (layerIndex === -1) {
          nuoviLayerVisibili = true;
          break;
        }
      }
    }

    if (nuoviLayerVisibili) {
      this.sharedDataService.loadGeoJsonForzato.next([true, true]);
    }
  }

  private impostaFlagVisibile(flagvisibile: boolean, item: TipologiaLayer): void {
    if (!flagvisibile) {
      this.layerService.toggleLayerItemVisible(item, flagvisibile);
    } else {
      item.flagvisibile = '1';
    }
  }

  private tryDeleteCustomLayer(id: string): void {
    if (this.featureInformationService.getByLayer(id).length > 0) {
      const dialog: DialogRef = this.giasDialogService.dialogMessageRef(
        this.transloco.translate('EliminaLayer'),
        this.transloco.translate('ContinueCustomLayerItemsAssociated')
      );

      dialog.result.pipe().subscribe(resp => {
        if (resp['returnObj']) {
          this.deleteCustomLayer(id);
        }
      });
    } else {
      this.deleteCustomLayer(id);
    }
  }

  private deleteCustomLayer(id: string) {
    this.masterService.set_isLoading({isLoading: true});
    this.gisClient.gisDeleteCustomLayer(Number.parseInt(id)).pipe(
      take(1),
      catchError(() => {
        this.masterService.set_isLoading({isLoading: false});
        this.giasMessageService.errorMessage('gis.LayerRemoveError', false, true);
        return of(null);
      })
    ).subscribe({
      next: r => {
        this.masterService.set_isLoading({isLoading: false});

        if (r?.RispostaStringa && r?.RispostaOK) {
          this.giasMessageService.successMessage('gis.LayerRemoveSuccess', false, true);
          this.getActiveLayers(this.selectedLayer.Option_Value);
          this.layerService.refreshLayers(this.selectedLayer);
          this.layerService.layerDeleted.next({id: id});
          this.sharedDataService.setOrigineChiamataLoadGeoJson(enum_OrigineChiamataLoadGeoJson.Indefinito);
          this.googleMapGeoJsonService.loadGeoJsonForzato(true);
        } else if (r?.RispostaOK) {
          this.giasMessageService.errorMessage(`${this.transloco.translate('gis.LayerRemoveNotAllowed')}: ${r.Errore}`, false, true);
        } else {
          this.giasMessageService.errorMessage('gis.LayerRemoveError', false, true);
        }
      },
      error: () => {
        this.masterService.set_isLoading({isLoading: false});
        this.giasMessageService.errorMessage('gis.LayerRemoveError', false, true);
      }
    });
  }
}
