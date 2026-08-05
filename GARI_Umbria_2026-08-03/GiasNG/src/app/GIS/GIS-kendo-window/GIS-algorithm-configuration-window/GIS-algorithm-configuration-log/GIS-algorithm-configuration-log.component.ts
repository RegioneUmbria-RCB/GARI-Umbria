import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ConfigurazioneSuLayer, GeoJson_Feature_New_1OfGeoJSONAgroGisProp, GisClient, LogEsecuzioniConfigurazioniProiezione, ObjOptionHTML_Out, TipologiaLayer } from 'app/Service/api.service';
import { Observable, map, of } from 'rxjs';

@Component({
  standalone: false,
  selector: 'gis-algorithm-configuration-log',
  templateUrl: './GIS-algorithm-configuration-log.component.html',
  styleUrls: ['./GIS-algorithm-configuration-log.component.css']
})
export class GisAlgorithmConfigurationLogComponent implements OnChanges {
  @Input() configurations: ConfigurazioneSuLayer[];
  @Input() layer: TipologiaLayer;
  @Input() tipoLayer: ObjOptionHTML_Out;
  @Input() feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp;

  logs$: Observable<Log[]> = of([]);
  description: string | null = null;
  guid: string | null = null;

  constructor(private gisClient: GisClient) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['feature']?.currentValue === changes['feature']?.previousValue) {
      return;
    }

    if (this.feature == null) {
      this.logs$ = of([]);
      return;
    }

    this.description = this.feature == null ? null : `GIS: ${this.feature.properties.Entita_Cod} - ${this.feature.properties.etichetta}`;
    this.guid = this.feature == null || this.feature.properties.Entita_GUID?.trim() == '' ? null : this.feature.properties.Entita_GUID;

    this.logs$ = this.gisClient
      .gisLeggiLogEsecuzioniConfigurazione(+this.feature.properties.Entita_Cod)
      .pipe(
        map(res => res.RispostaStringa.elencoLogEsecuzioniConfigurazione ?? []),
        map(logs => this.mapLogs(logs))
      );
  }

  mapLogs(logs: LogEsecuzioniConfigurazioniProiezione[]): Log[] {
    const result: Map<number, Log> = new Map<number, Log>();
    for (const log of logs) {
      let current = result.get(log.LayerAnalysisConfig_Cod);
      if (current == null) {
        current = { code: log.LayerAnalysisConfig_Cod, description: log.LayerAnalysisConfig_Des, messages: [] };
        result.set(log.LayerAnalysisConfig_Cod, current);
      }

      current.messages.push({ date: log.Data_Creazione, messages: log.Messaggi });
    }

    return [...result.values()];
  }
}

interface Log {
  code: number;
  description: string;
  messages: Message[];
}

interface Message {
  date: Date;
  messages: string[];
}
