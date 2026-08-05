import { Injectable } from '@angular/core';
import {ConfigurazioneProiezione, GisClient} from '../../Service/api.service';
import {GiasMessageService} from '../../Service/gias-message.service';

@Injectable()
export class GISCfgProiezioniService {

  constructor(
    private gisClient: GisClient,
    private giasMessageService: GiasMessageService,
  ) {  }

  public activateAlgorithm(configuration: ConfigurazioneProiezione): void {
    this.gisClient.gisActivateAlgorithm(configuration).subscribe(
      {
        next: r => {
          console.log(r);
          switch (r.RispostaStringa) {
            case 'OK':
              this.giasMessageService.successMessage('ConfigurazioneAttivata', false, true);
              break;
            case 'CONF_INUSO':
              this.giasMessageService.infoMessagge("ConfigurazioneInUso", false, true);
              break;
            case 'CONF_INESISTENTE':
              this.giasMessageService.errorMessage('ConfigurazioneInesistente', false, true);
              break;
            default :
              this.giasMessageService.errorMessage(r.Errore);
          }
        },
        error: r => {
          console.error(r);
          this.giasMessageService.errorMessage(r.message);
        }
      });
  }

}
