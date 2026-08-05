import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { MasterService, rispostaStandard } from 'app/Service/master.service';
import { CreaTokenBlockchainRequest } from '../models/sostenibilita-co2.model';

@Injectable({ providedIn: 'root' })
export class TokenCo2Service {

  private get baseUrl(): string {
    return this.masterService.link_NetCoreDataExchange;
  }

  private readonly httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true
  };

  constructor(
    private http: HttpClient,
    private masterService: MasterService
  ) {}

  /**
   * Returns the generated token string on success.
   * Throws an Error with the backend's Errore message on business failure (RispostaOK=false).
   * All error display is the caller's responsibility — no framework dialogs are shown.
   */
  creaTokenBlockchain(request: CreaTokenBlockchainRequest): Observable<string> {
    const url = `${this.baseUrl}/v1/sostenibilita-co2/crea-token-blockchain`;
    return this.http.post<rispostaStandard<string>>(url, JSON.stringify(request), this.httpOptions).pipe(
      map(r => {
        if (!r.RispostaOK) {
          throw new Error(r.Errore ?? '');
        }
        return r.RispostaStringa as string;
      })
    );
  }
}
