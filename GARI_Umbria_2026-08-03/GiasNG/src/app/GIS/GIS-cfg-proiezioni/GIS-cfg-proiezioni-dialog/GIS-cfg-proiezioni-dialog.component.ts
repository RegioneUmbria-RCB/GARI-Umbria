import {Component, EventEmitter, Input, OnChanges, Output} from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { AlgoritmoProiezione, ConfigurazioneProiezione, GisClient, ParametriAlgoritmoProiezione, ParametriProiezioneLayer, ProiezioneLayer } from 'app/Service/api.service';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS, FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { BehaviorSubject, combineLatest, map, Observable, of, switchMap, tap, withLatestFrom } from 'rxjs';
import {FormBuilder, FormGroup} from '@angular/forms';
import { LayerService } from 'app/GIS/services/layer.service';

export const DEFAULT_CONFIGURATION_LAYER_ID = "1";

@Component({
  standalone: false,
  selector: 'gis-cfg-proiezioni-dialog',
  templateUrl: './GIS-cfg-proiezioni-dialog.component.html',
  styleUrls: ['./GIS-cfg-proiezioni-dialog.component.css'],
  providers: []
})
export class GISCfgProiezioniDialogComponent implements OnChanges {
  @Input() inputConfiguration: ConfigurazioneProiezione | null;
  @Input() algorithms: AlgoritmoProiezione[] = [];

  @Output() close = new EventEmitter<void>();

  configurationSubject = new BehaviorSubject<ConfigurazioneProiezione>(null);
  algorithmSubject = new BehaviorSubject<AlgoritmoProiezione>(null);
  descriptionSubject = new BehaviorSubject<string>('');
  layer1Subject = new BehaviorSubject<ProiezioneLayer>({ TipologiaLayer_cod: null, Params: [] });
  layer2Subject = new BehaviorSubject<ProiezioneLayer>({ TipologiaLayer_cod: null, Params: [] });
  layerResSubject = new BehaviorSubject<ProiezioneLayer>({ TipologiaLayer_cod: null, Params: [] });

  layers$ = this.layerService
    .readLayersFromBackend({ Layer_Selezionato: DEFAULT_CONFIGURATION_LAYER_ID, leggiLayerNonVisibili: true })
    .pipe(map(res => res.RispostaStringa.ListaTipologieLayer));

  algorithmSelected$ = this.algorithmSubject
    .asObservable()
    .pipe(
      withLatestFrom(this.configurationSubject.asObservable(), this.layer1Subject.asObservable(), this.layer2Subject.asObservable(), this.layerResSubject.asObservable()),
      tap(([algorithm, conf, layer1, layer2, layerRes]) => {
        this.layer1Subject.next(this.mapLayerProjection(algorithm, layer1, conf?.Layer1, 1));
        this.layer2Subject.next(this.mapLayerProjection(algorithm, layer2, conf?.Layer2, 2));
        this.layerResSubject.next(this.mapLayerProjection(algorithm, layerRes, conf?.LayerRisultato, 3));
      }),
      map(([algorithm, _]) => algorithm)
    );

  data$ = combineLatest({
    configuration: this.configurationSubject.asObservable(),
    algorithm: this.algorithmSelected$,
    description: this.descriptionSubject.asObservable(),
    layer1: this.layer1Subject.asObservable(),
    layer2: this.layer2Subject.asObservable(),
    layerRes: this.layerResSubject.asObservable(),
  });

  configuration$: Observable<any>;
  filterSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;
  width = window.innerWidth * 0.95;

  activateForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private gisClient: GisClient,
    private giasDialogService: GiasDialogService,
    private translocoService: TranslocoService,
    private layerService: LayerService
  ) {
    this.activateForm = this.fb.group({
      canActivate: false
    });

    this.configuration$ = this.configurationSubject.asObservable().pipe(
      tap((config) => {
        this.selectAlgorithmFromConfig(config, this.algorithms);
        this.activateForm.controls['canActivate'].setValue(config.canActivate); // inizializzo il form coi dati della configurazione
      })
    );
  }

  ngOnChanges(): void {
    this.configurationSubject.next(this.inputConfiguration);
  }

  submit(): void {
    of(true)
      .pipe(
        withLatestFrom(this.data$),
        map(([_, data]) => ({
          AlgoritmoProiezione_Cod: data.algorithm.Algoritmo_Cod,
          ConfigurazioneProiezione_Cod: data.configuration?.ConfigurazioneProiezione_Cod ?? 0,
          ConfigurazioneProiezione_Des: data.description,
          ConfigurazioneProiezione_GUID: data.configuration?.ConfigurazioneProiezione_GUID,
          canManage: data.configuration?.canManage,
          AttivoTuttiLayer: true,
          Layer1: data.layer1,
          Layer2: data.layer2,
          LayerRisultato: data.layerRes
        } as ConfigurazioneProiezione)),
        switchMap(body => this.gisClient.gisSalvaConfigurazioneProiezione(body))
      )
      .subscribe({
        next: () => {
          this.close.emit()
          this.giasDialogService.baseSuccess('', 'gis.ConfigurazioneSalvataCorrettamente');
        },
        error: error => {
          this.close.emit();
          this.giasDialogService.baseError('', FunzioniComuniService.getResponseError(error, this.translocoService, 'SiÈVerificatoUnErroreDuranteLaFaseDiSalvat'));
        }
      });
  }

  onLayerChange(layer: ProiezioneLayer, layerSubject: BehaviorSubject<ProiezioneLayer>): void {
    layerSubject.next(layer);
  }

  closeDialog(): void {
    this.close.emit();
  }

  private selectAlgorithmFromConfig(config: ConfigurazioneProiezione | null, algorithms: AlgoritmoProiezione[]): void {
    const algorithm = algorithms?.find(x => x.Algoritmo_Cod == config?.AlgoritmoProiezione_Cod);
    this.algorithmSubject.next(algorithm);
    this.descriptionSubject.next(config?.ConfigurazioneProiezione_Des ?? '');
  }

  private mapLayerProjection(algorithm: AlgoritmoProiezione, currentLayer: ProiezioneLayer | null, configLayer: ProiezioneLayer | null, layerNum: number): ProiezioneLayer {
    const result = {
      LayerElementiGrafici_Cod: currentLayer?.LayerElementiGrafici_Cod ?? configLayer?.LayerElementiGrafici_Cod,
      TipologiaLayer_cod: currentLayer?.TipologiaLayer_cod ?? configLayer?.TipologiaLayer_cod ?? DEFAULT_CONFIGURATION_LAYER_ID,
      LayerElementiGrafici_GUID: currentLayer?.LayerElementiGrafici_GUID ?? configLayer?.LayerElementiGrafici_GUID
    } as ProiezioneLayer;

    const params: ParametriProiezioneLayerComplete[] = [];
    for (const parameter of algorithm?.Parametri?.filter(x => x.tipo_Parametro == layerNum) ?? []) {
      const layerAttributeId = currentLayer?.Params?.find(x => x.Parametro_Cod == parameter.Parametro_Cod)?.TipologiaLayer_struct_cod
        ?? configLayer?.Params?.find(x => x.Parametro_Cod == parameter.Parametro_Cod)?.TipologiaLayer_struct_cod
        ?? 0;

      const guid: string = currentLayer?.Params?.find(x => x.Parametro_Cod == parameter.Parametro_Cod)?.TipologiaLayer_struct_GUID
        ?? configLayer?.Params?.find(x => x.Parametro_Cod == parameter.Parametro_Cod)?.TipologiaLayer_struct_GUID;

      params.push({ Parametro_Cod: parameter.Parametro_Cod, TipologiaLayer_struct_cod: layerAttributeId, parameter: parameter, TipologiaLayer_struct_GUID: guid });
    }

    return { ...result, Params: params };
  }

  updateConfigurationCanActivateProperty(e): void {
    let newValue: ConfigurazioneProiezione = this.configurationSubject.getValue();
    newValue.canActivate = e;
    this.configurationSubject.next(newValue);
  }
}

export interface ParametriProiezioneLayerComplete extends ParametriProiezioneLayer {
  parameter: ParametriAlgoritmoProiezione;
}
