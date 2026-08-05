import { Injectable, signal } from '@angular/core';
import { Observable, of } from 'rxjs';
import { finalize, map, tap } from 'rxjs/operators';
import { AjaxAgronicaNetCore6ApiService } from 'app/Service/ajax-agronica-net-core6-api.service';
import {
  AssemblyPayloadM4Request,
  CarburanteM4,
  EnergiaM4,
  EsercizioM4,
  PerimetroM4,
  ValidazioneSostenibilitaRequest,
  ValidazioneSostenibilitaResult
} from '../models/sostenibilita-co2.model';

@Injectable({ providedIn: 'root' })
export class M4SubmissionService {

  private readonly _payload = signal<AssemblyPayloadM4Request | null>(null);

  constructor(private ajaxService: AjaxAgronicaNetCore6ApiService) {}

  payloadReady(): boolean {
    return this._payload() !== null;
  }

  validaEComponi(
    carburanti: CarburanteM4[],
    energia: EnergiaM4[],
    perimetro: PerimetroM4,
    esercizi: EsercizioM4[]
  ): Observable<ValidazioneSostenibilitaResult> {
    const request: ValidazioneSostenibilitaRequest = {
      PerimetroAziende: perimetro.Aziende,
      Carburanti: carburanti,
      Energia: energia
    };

    return this.ajaxService.ajaxAPIPost<
      ValidazioneSostenibilitaRequest,
      ValidazioneSostenibilitaResult
    >(
      '/v1/sostenibilita/validazione-sostenibilita',
      request,
      true
    ).pipe(
      map(r => r.RispostaStringa),
      tap(result => {
        if (result.ValidazioneEsito) {
          this._payload.set({
            Perimetro: perimetro,
            Carburanti: result.ConsumiValidati.Carburanti,
            Energia: result.ConsumiValidati.Energia,
            Esercizi: esercizi
          });
        }
      })
    );
  }

  inviaDatiAM4(): Observable<boolean> {
    const payload = this._payload();
    if (!payload) {
      return of(false);
    }

    return this.ajaxService.ajaxAPIPost<AssemblyPayloadM4Request, unknown>(
      '/v1/sostenibilita/calcolo-sostenibilita-co2',
      payload,
      true
    ).pipe(
      map(() => true),
      finalize(() => this._payload.set(null))
    );
  }

  reset(): void {
    this._payload.set(null);
  }
}
