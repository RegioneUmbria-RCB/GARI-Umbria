import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {GisClient, RispostaStandard_1OfSalvaEntitaConAttributi_Out, SalvaEntitaConAttributi_In} from '../../Service/api.service';
import {rispostaStandard} from '../../Service/master.service';
import {AjaxAgronicaAPIService} from '../../Service/ajax-agronica.api.service';
import { GisDataReadRval_New_1OfGeoJSONAgroGisProp } from 'app/Service/net-core6-api.service';

export class ReadTecniciParams {
  DataInizio: string;
  DataFine: string;
  Utente_corrente: boolean;

  constructor(
    dataInizio: Date,
    dataFine: Date,
    utente_corrente: boolean
  ) {
    this.DataInizio = dataInizio.toDateString();
    this.DataFine = dataFine.toDateString();
    this.Utente_corrente = utente_corrente;
  }
}

@Injectable()
export class GisAttributiService {

  constructor(
    protected ajaxApiService: AjaxAgronicaAPIService,
    private gisClientService: GisClient
  ) {  }

  public leggiElencoStrutturaAttributiLayer(codiceLayer: string): Observable<rispostaStandard<any>> {
    const LayerElementiGraficiCod = codiceLayer;

    return this.ajaxApiService.ajaxAPIPost<string, rispostaStandard<any>>(
      'Gis/LeggiElencoStrutturaAttributiLayer',
      LayerElementiGraficiCod,
      false,
      true
    );
  }

  public leggiTecnici(params: ReadTecniciParams): Observable<rispostaStandard<GisDataReadRval_New_1OfGeoJSONAgroGisProp>> {
    const parametri = params;

    return this.ajaxApiService.ajaxAPIPost<ReadTecniciParams, GisDataReadRval_New_1OfGeoJSONAgroGisProp>(
      'Gis/UltimaPosizione',
      parametri,
      false,
      true
    );
  }

  public gisSalvaEntitaConAttributi(body?: SalvaEntitaConAttributi_In | undefined): Observable<RispostaStandard_1OfSalvaEntitaConAttributi_Out> {
    return this.gisClientService.gisSalvaEntitaConAttributi(body);
  }

}
