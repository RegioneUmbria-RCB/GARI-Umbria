import { Component, Input, OnChanges } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { AttributoLayer, GisClient, ParametriProiezioneLayer, ProiezioneLayer, TipologiaLayer } from "app/Service/api.service";
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from "app/Service/FunzioniComuni.service";
import { DEFAULT_CONFIGURATION_LAYER_ID } from "../GIS-cfg-proiezioni-dialog.component";

const DEFAULT_ATTRIBUTE_ID = '0';

@Component({
  standalone: false,
  selector: 'gis-cfg-proiezioni-layer',
  templateUrl: './GIS-cfg-proiezioni-layer.component.html',
  styleUrls: ['./GIS-cfg-proiezioni-layer.component.css']
})
export class GISCfgProziezioniLayerComponent implements OnChanges {
  @Input() title: string;
  @Input() layers: TipologiaLayer[];
  @Input() inputLayer: ProiezioneLayer;

  filterSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;

  private noneString = this.translocoService.translate('Nessuno');

  constructor(
    private gisClient: GisClient,
    private translocoService: TranslocoService
  ) { }

  ngOnChanges(): void {
    if (this.inputLayer?.LayerElementiGrafici_Cod != null) {
      this.onLayerSelected(this.inputLayer.LayerElementiGrafici_Cod);
    }
  }

  onLayerSelected(layerId: number): void {
    this.gisClient
      .gisLeggiImpostazioniAvanzateLayer(layerId)
      .subscribe(res => {
        const layerAttributes = this.getStandardAttributes(res.RispostaStringa.ListaAttributiLayer);
        const params = [];
        for (const param of this.inputLayer.Params) {
          const value = this.inputLayer.LayerElementiGrafici_Cod == layerId ? this.getExistingAttribute(param, layerAttributes) : this.getDefaultAttribute(param, layerAttributes);
          params.push(value);
        }

        this.inputLayer.Params = params;
        this.inputLayer.LayerElementiGrafici_Cod = layerId;
        this.inputLayer.TipologiaLayer_cod = +DEFAULT_CONFIGURATION_LAYER_ID;
      });
  }

  onAttributeSelected(data: ProiezioneLayer, layerAttribute: AttributoLayer, paramCode: number): void {
    const params = data.Params;
    const param = params.find(x => x.Parametro_Cod == paramCode) as ParametriAlgoritmoProiezioneLayer | null;
    if (param == null) {
      return;
    }

    param.TipologiaLayer_struct_cod = +layerAttribute.ProgressivoDataStruct;
    param.selectedAttribute = param.layerAttributes.find(x => +x.ProgressivoDataStruct == param.TipologiaLayer_struct_cod);
    this.inputLayer.Params = params;
  }

  getSelectedLayer(layerId: number): TipologiaLayer | null {
    return this.layers.find(x => +x.id == layerId);
  }

  private getStandardAttributes(attributes: AttributoLayer[]): AttributoLayer[] {
    return [{ NomeAttributo: this.noneString, ProgressivoDataStruct: DEFAULT_ATTRIBUTE_ID }, ...attributes];
  }

  private getExistingAttribute(param: ParametriProiezioneLayer, layerAttributes: AttributoLayer[]): ParametriAlgoritmoProiezioneLayer {
    const result = layerAttributes.find(x => +x.ProgressivoDataStruct == param.TipologiaLayer_struct_cod);
    if (result == null) {
      return this.getDefaultAttribute(param, layerAttributes);
    }

    return { ...param, layerAttributes: layerAttributes, TipologiaLayer_struct_cod: param.TipologiaLayer_struct_cod, selectedAttribute: result };
  }

  private getDefaultAttribute(param: ParametriProiezioneLayer, layerAttributes: AttributoLayer[]): ParametriAlgoritmoProiezioneLayer {
    return { ...param, layerAttributes: layerAttributes, TipologiaLayer_struct_cod: 0, selectedAttribute: layerAttributes[0] };
  }
}

interface ParametriAlgoritmoProiezioneLayer extends ParametriProiezioneLayer {
  selectedAttribute: AttributoLayer;
  layerAttributes: AttributoLayer[];
}
