import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AjaxAgronicaNetCore6ApiService } from 'app/Service/ajax-agronica-net-core6-api.service';
import {
  ColturaCO2Item,
  ColtureCO2Response,
  FiliereCO2Item,
  FiliereCO2Response,
  ModalitaCalcolo,
  RiepilogoPerimetroRow,
  RiepilogoRaccoltiRequest,
  RiepilogoRaccoltiResponse,
  RigaRiepilogoRaccolti,
  TokenGenerabileApiRow,
  TokenGenerabileRow,
  TokenGenerabiliResponse
} from '../models/sostenibilita-co2.model';

@Injectable({ providedIn: 'root' })
export class PerimetroCO2Service {

  constructor(private ajaxService: AjaxAgronicaNetCore6ApiService) {}

  loadFiliere(): Observable<FiliereCO2Item[]> {
    return this.ajaxService.ajaxAPIGet<object, FiliereCO2Response>(
      '/v1/sostenibilita/filiere-aziende',
      {}
    ).pipe(map(r => r.RispostaStringa.Filiere));
  }

  loadColtureByFiliera(pivaFiliera: string): Observable<ColturaCO2Item[]> {
    return this.ajaxService.ajaxAPIGet<{ pivaFiliera: string }, ColtureCO2Response>(
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

  // ── FS2.07.1 — Lookup endpoints ─────────────────────────────────────────────

  getAnniLookup(pivaFiliera: string): Observable<number[]> {
    return this.ajaxService.ajaxAPIGet<{ filiera: string }, number[]>(
      '/v1/sostenibilita/token-generabili/anni',
      { filiera: pivaFiliera }
    ).pipe(map(r => {
      const data = r.RispostaStringa as any;
      return (Array.isArray(data) ? data : data?.Anni ?? []) as number[];
    }));
  }

  getTokenGenerabili(pivaFiliera: string, anno: number): Observable<TokenGenerabileRow[]> {
    return this.ajaxService.ajaxAPIGet<{ filiera: string; anno: number }, TokenGenerabiliResponse>(
      '/v1/sostenibilita/token-generabili',
      { filiera: pivaFiliera, anno }
    ).pipe(
      map(r => {
        const data = r.RispostaStringa as any;
        const righe: TokenGenerabileApiRow[] = Array.isArray(data)
          ? data
          : (data?.Righe ?? []);
        return this.mapToTokenGenerabiliRows(righe);
      })
    );
  }

  private mapToTokenGenerabiliRows(righe: TokenGenerabiliResponse['Righe']): TokenGenerabileRow[] {
    // Pre-compute the greatest DataInvocazione per azienda to determine the selectable row
    const latestByAzienda = new Map<string, string>();
    for (const r of righe) {
      const current = latestByAzienda.get(r.PivaAzienda);
      if (!current || r.DataInvocazione > current) {
        latestByAzienda.set(r.PivaAzienda, r.DataInvocazione);
      }
    }
    return righe.map(r => {
      const isMostRecent = r.DataInvocazione === latestByAzienda.get(r.PivaAzienda);

      // Derive values from JsonRisposta when the backend doesn't compute them directly
      // Coerce to number: the backend may serialise these as strings
      let variazSocBiogenico: number = parseFloat(r.VarSocSoilBiogenicCarbon as any);
      if (isNaN(variazSocBiogenico)) {
        variazSocBiogenico = null;
      }
      let nAppezzamenti: number = parseInt(r.NumeroAppezzamenti as any, 10);
      if (isNaN(nAppezzamenti)) {
        nAppezzamenti = null;
      }

      // Fall back to JsonRisposta when the backend fields are missing
      if (r.JsonRisposta && (variazSocBiogenico == null || nAppezzamenti == null)) {
        try {
          const payload = JSON.parse(r.JsonRisposta);
          const aziende: any[] = payload?.aziende ?? [];
          if (variazSocBiogenico == null) {
            const totali = aziende[0]?.totali?.[0];
            if (totali != null) {
              variazSocBiogenico = parseFloat(totali.CO2eq_tot_var_SOC ?? '0');
            }
          }
          if (nAppezzamenti == null) {
            nAppezzamenti = aziende.reduce((sum: number, a: any) => sum + (a.appezzamenti?.length ?? 0), 0);
          }
        } catch { /* malformed JSON — keep server values */ }
      }

      return {
        rowKey: r.IdInvocazione,
        idInvocazione: r.IdInvocazione,
        dataInvocazione: r.DataInvocazione,
        azienda: r.PivaAzienda,
        aziendaPiva: r.PivaAzienda,
        aziendaLabel: r.RagSocAzienda ? `${r.RagSocAzienda} (${r.PivaAzienda})` : r.PivaAzienda,

        jsonRisposta: r.JsonRisposta ?? '',
        variazSocBiogenico,
        nAppezzamenti,
        isMostRecent
      };
    });
  }
}
