import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ConfigurazioneProiezione, GisClient} from '../../../Service/api.service';
import {GISCfgProiezioniConfigService} from './gis-cfg-proiezioni-config.service';
import {generateGridProviders} from 'gias-kendo-grid';
import {GISCfgProiezioniConfigDataService} from './gis-cfg-proiezioni-config-data.service';
import {FunzioniComuniService} from '../../../Service/FunzioniComuni.service';
import {TranslocoService} from '@jsverse/transloco';
import {GiasDialogService} from '../../../Service/gias-dialog.service';

@Component({
  standalone: false,
  selector: 'gis-cfg-proiezioni-config',
  templateUrl: './gis-cfg-proiezioni-config.component.html',
  styleUrls: ['./gis-cfg-proiezioni-config.component.css'],
  providers: [
    ...generateGridProviders(GISCfgProiezioniConfigService, GISCfgProiezioniConfigComponent), GISCfgProiezioniConfigDataService
  ]
})
export class GISCfgProiezioniConfigComponent implements OnInit {
  @Input() inputConfiguration: ConfigurazioneProiezione | null;
  @Output() close: EventEmitter<void> = new EventEmitter<void>();

  width: number = window.innerWidth * 0.95;
  constructor(
    private gisClient: GisClient,
    private translocoService: TranslocoService,
    private giasDialogService: GiasDialogService,
    private gisCfgProiezioniConfigDataService: GISCfgProiezioniConfigDataService
  ) {  }

  ngOnInit(): void {
    this.gisCfgProiezioniConfigDataService.cfg = this.inputConfiguration?.cfg ?? '';
  }

  public closeDialog(): void {
    this.close.emit();
  }

  public submit(): void {
    let conf: ConfigurazioneProiezione = JSON.parse(JSON.stringify(this.inputConfiguration)); // copy of the configuraztion
    conf.cfg = this.gisCfgProiezioniConfigDataService.cfg;
    this.gisClient.gisSaveAlgorithmConfigurationCfg(conf).subscribe({
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
}
