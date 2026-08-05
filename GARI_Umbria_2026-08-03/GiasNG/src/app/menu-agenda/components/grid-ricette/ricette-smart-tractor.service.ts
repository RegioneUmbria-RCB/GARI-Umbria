import { Injectable } from '@angular/core';
import {AjaxAgronicaNetCoreDataExchangeApiService} from '../../../Service/ajax-agronica-net-core-data-exchange-api.service';
import {rispostaStandard} from '../../../Service/master.service';

@Injectable()
export class RicetteSmartTractorService {

  constructor(
    private apiDataExchangeService: AjaxAgronicaNetCoreDataExchangeApiService
  ) { }

  public sendPrescriptionToSTEngine(ricettaOperazioneCod: number) {
    return this.apiDataExchangeService.ajaxAPIPost<number, SendPrescriptionResponse[]>(
      'SmartTractor/SendPrescription', ricettaOperazioneCod
    );
  }
}

export class SendPrescriptionResponse {
  readonly RicettaOperazioneCod: number;
  readonly ProviderResponseId: string;
  readonly MacCod: number;
  readonly Success: boolean;
  readonly PayloadSizeBytes: number;
  readonly SentAt: string;
  readonly Error: string;
}
