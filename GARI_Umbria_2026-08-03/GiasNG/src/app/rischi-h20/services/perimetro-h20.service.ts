import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AjaxAgronicaNetCore6ApiService } from 'app/Service/ajax-agronica-net-core6-api.service';
import { rispostaStandard } from 'app/Service/master.service';
import {
  CalcoloH20Request,
  CalcoloH20Response,
  ColturaH20Item,
  ColtureH20Response,
  EsercizioH20Payload,
  FiliereH20Item,
  FiliereH20Response,
  ModalitaCalcolo,
  PerimetroH20Payload,
  RiepilogoPerimetroRow,
  RiepilogoRaccoltiRequest,
  RiepilogoRaccoltiResponse,
  RigaRiepilogoRaccolti
} from '../models/rischi-h20.model';
import { RispostaStandard } from 'gias-ui-kit';

@Injectable({ providedIn: 'root' })
export class PerimetroH20Service {

  constructor(private ajaxService: AjaxAgronicaNetCore6ApiService) {}

  loadFiliere(): Observable<FiliereH20Item[]> {
    return this.ajaxService.ajaxAPIGet<object, FiliereH20Response>(
      '/v1/sostenibilita/filiere-aziende',
      {}
    ).pipe(map(r => r.RispostaStringa.Filiere));
  }

  loadColtureByFiliera(pivaFiliera: string): Observable<ColturaH20Item[]> {
    return this.ajaxService.ajaxAPIGet<{ pivaFiliera: string }, ColtureH20Response>(
      '/v1/sostenibilita/colture-aziende',
      { pivaFiliera }
    ).pipe(map(r => r.RispostaStringa.Colture));
  }

  getRiepilogoRaccolti(
    pivaFiliera: string,
    anno: number
  ): Observable<RiepilogoRaccoltiResponse> {
    const body: RiepilogoRaccoltiRequest = {
      PivaFiliera: pivaFiliera,
      VegCod: 0,
      Anno: anno
    };
    return this.ajaxService.ajaxAPIPost<RiepilogoRaccoltiRequest, RiepilogoRaccoltiResponse>(
      '/v1/sostenibilita/riepilogo-raccolti',
      body,
      true
    ).pipe(map(r => r.RispostaStringa));
  }

  mapToRiepilogoRows(
    righe: RigaRiepilogoRaccolti[],
    modalita: ModalitaCalcolo,
    coltureSelezionate: string[] | null
  ): RiepilogoPerimetroRow[] {
    const tutteLeColture = !coltureSelezionate || coltureSelezionate.length === 0;
    return righe
      .filter(r => {
        if (modalita === 'Aziendale' || tutteLeColture) {
          return true;
        }
        return coltureSelezionate.includes(r.SpecieColturale);
      })
      .map(r => ({
        rowKey: `${r.Azienda}|${r.Appezzamento}|${r.Esercizio}`,
        azienda: r.Azienda,
        piva: r.PartitaIva,
        appezzamento: r.Appezzamento,
        esercizio: r.Esercizio,
        id_esercizio: parseInt(r.Esercizio, 10),
        nazione: r.Nazione,
        regione: r.Regione,
        istat_reg: r.ISTAT_reg,
        provincia: r.Provincia,
        superficie_ha: r.Superficie,
        specie_colturale: r.SpecieColturale,
        prodotti_raccolti: r.ProdottiRaccolti,
        codici_lotti: r.CodiceLotti,
        data_ultima_raccolta: r.DataUltimaRaccolta,
        totale_raccolto_kg: r.TotaleRaccoltaKg,
        selezionabile: modalita !== 'Aziendale'
      }));
  }

  avviaCalcoloH20(payload: CalcoloH20Request): Observable<rispostaStandard<CalcoloH20Response>> {
    return this.ajaxService.ajaxAPIPost<CalcoloH20Request, CalcoloH20Response>(
      '/v1/sostenibilita-h2o/calcolo-sostenibilita-h2o',
      payload,
      false,  // setLoading — gestito da isCalcoloInProgress nel componente
      true,   // compressione
      true,   // showErroriGestiti
      false   // showErroriNonGestiti — gestito dal componente
    );
  }
}
