import { Component, Input, OnChanges } from '@angular/core';
import { enum_FeatureProperty } from 'app/GIS/GIS-enum/GIS-feature';
import { AttivazioneConfigurazioneAlgoritmiCartografici, ConfigurazioneSuEntitaAttiva, ConfigurazioneSuLayer, EntitaAlgoritmoCartografico, GeoJson_Feature_New_1OfGeoJSONAgroGisProp, GisClient, ObjOptionHTML_Out, TipologiaLayer } from 'app/Service/api.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { AlgorithmConfigurationWindowService } from '../GIS-algorithm-configuration-window.service';

@Component({
  standalone: false,
  selector: 'gis-algorithm-configuration',
  templateUrl: './GIS-algorithm-configuration.component.html',
  styleUrls: ['./GIS-algorithm-configuration.component.css']
})
export class GisAlgorithmConfigurationComponent implements OnChanges {
  @Input() configurations: ConfigurazioneSuLayer[];
  @Input() layer: TipologiaLayer;
  @Input() tipoLayer: ObjOptionHTML_Out;
  @Input() feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp;

  description: string | null = null;
  activeConfigurations = new Map<number, ConfigurazioneSuEntitaAttiva>();

  constructor(
    private gisClient: GisClient,
    private giasMessageService: GiasMessageService,
    private algorithmConfigurationWindowService: AlgorithmConfigurationWindowService
  ) { }

  ngOnChanges(): void {
    this.description = this.feature == null ? null : `GIS: ${this.getFeatureCode()} - ${this.feature.properties.etichetta}`;
    this.activeConfigurations = new Map<number, ConfigurazioneSuEntitaAttiva>();

    for (const conf of this.configurations) {
      this.activeConfigurations.set(conf.LayerAnalysisConfig_Algorithm_Cod, conf.EntitaAttive.find(entity => entity.Entita_Cod == this.getFeatureCode()));
    }
  }

  getActiveConfiguration(configuration: ConfigurazioneSuLayer): ConfigurazioneSuEntitaAttiva | null {
    return this.activeConfigurations.get(configuration.LayerAnalysisConfig_Algorithm_Cod);
  }

  isLayerChecked(configuration: ConfigurazioneSuLayer): boolean {
    return configuration?.AttivaSuTuttiLayer;
  }

  isChecked(configuration: ConfigurazioneSuLayer): boolean {
    return this.isLayerChecked(configuration) || this.getActiveConfiguration(configuration) != null;
  }

  changeActive(checked: boolean, configuration: ConfigurazioneSuLayer): void {
    const body = {
      configurazioneProiezione_Cod: configuration.LayerAnalysisConfig_Cod,
      layer_cod: 0,
      isAttivo: checked,
      tipologia_layer_cod: +this.tipoLayer.Option_Value,
      listaEntita: [{ entita_cod_1: this.getFeatureCode(), entita_cod_2: 0, entita_cod_risultato: 0 } as EntitaAlgoritmoCartografico]
    } as AttivazioneConfigurazioneAlgoritmiCartografici;

    this.gisClient
      .gisAttivaDisattivaConfigurazione(body)
      .subscribe({
        next: () => {
          this.giasMessageService.successMessage('OperazioneRiuscita', false, true)
          this.algorithmConfigurationWindowService.forceReload();
        },
        error: () => this.giasMessageService.errorMessage('ErroreModifica', false, true)
      });
  }

  private getFeatureCode(): number {
    return +this.feature?.properties.Entita_Cod;
  }
}
