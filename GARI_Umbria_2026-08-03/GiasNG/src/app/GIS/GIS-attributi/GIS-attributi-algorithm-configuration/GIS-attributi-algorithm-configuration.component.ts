import { Component, Input } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { AlgorithmConfigurationWindowService } from 'app/GIS/GIS-kendo-window/GIS-algorithm-configuration-window/GIS-algorithm-configuration-window.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { AttivazioneConfigurazioneAlgoritmiCartografici, ConfigurazioneSuLayer, GisClient, ObjOptionHTML_Out, TipologiaLayer } from 'app/Service/api.service';
import { GiasMessageService } from 'app/Service/gias-message.service';

@Component({
  standalone: false,
  selector: 'gis-attributi-algorithm-configuration',
  templateUrl: './GIS-attributi-algorithm-configuration.component.html',
  styleUrls: ['./GIS-attributi-algorithm-configuration.component.css']
})
export class GISAttributiAlgorithmConfigurationComponent {
  @Input() tipoLayer: ObjOptionHTML_Out;
  @Input() layer: TipologiaLayer;
  @Input() configurations: ConfigurazioneSuLayer[];

  changeLayerConfigurationEnable: [ConfigurazioneSuLayer, boolean] | null = null

  constructor(
    private gisClient: GisClient,
    private giasMessageService: GiasMessageService,
    private algorithmConfigurationWindowService: AlgorithmConfigurationWindowService,
    private translocoService: TranslocoService
  ) { }

  onChangeLayerConfiguration(configuration: ConfigurazioneSuLayer, $event: Event): void {
    $event.stopPropagation();
    $event.preventDefault();
    this.changeLayerConfigurationEnable = [configuration, !configuration.AttivaSuTuttiLayer];
  }

  changeActiveOnAllPolygons(): void {
    const [configuration, checked] = this.changeLayerConfigurationEnable;
    if (checked) {
      configuration.AttivaSuTuttiLayer = true;
      configuration.NumEntitaAttive = configuration.NumEntitaMax;
    } else {
      configuration.AttivaSuTuttiLayer = false;
      configuration.NumEntitaAttive = 0;
    }

    const body = {
      configurazioneProiezione_Cod: configuration.LayerAnalysisConfig_Cod,
      layer_cod: +this.layer.id,
      isAttivo: configuration.AttivaSuTuttiLayer,
      tipologia_layer_cod: +this.tipoLayer.Option_Value,
      listaEntita: []
    } as AttivazioneConfigurazioneAlgoritmiCartografici;

    this.gisClient
      .gisAttivaDisattivaConfigurazione(body)
      .subscribe({
        next: () => {
          this.giasMessageService.successMessage('OperazioneRiuscita');
          this.algorithmConfigurationWindowService.forceReload();
        },
        error: err => {
          const message = err.status == 401
            ? err.message
            : FunzioniComuniService.getResponseError(err, this.translocoService, 'ErroreModifica')

          this.giasMessageService.errorMessage(message);
          this.algorithmConfigurationWindowService.forceReload();
        },
        complete: this.changeLayerConfigurationEnable = null
      });
  }
}
