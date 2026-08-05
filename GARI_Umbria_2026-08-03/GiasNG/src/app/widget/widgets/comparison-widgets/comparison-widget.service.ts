import { Injectable } from '@angular/core';
import { WidgetIndiciProduttivitaGlobal, WidgetRequestIndiciProduttivita, WidgetsClient } from 'app/Service/api.service';
import { catchError, map, Observable, of, ReplaySubject } from 'rxjs';
import { ComparisonWidgetData } from './comparison-widget.component';
import { TranslocoService } from '@jsverse/transloco';
import { WidgetIndiciClient } from 'app/Service/net-core6-api.service';

export const PIVA_VALORE_MEDIO_IN_PORTAFOGLIO = -30;
export const PIVA_VALORE_REGIONALE_DI_RIFERIMENTO = '-3';

const MULTI_YEAR_PALETTE = ['#002D5A', '#005987', '#00879A', '#00B393', '#82DB7C', '#F9F871'];

@Injectable()
export class ComparisonWidgetService {
  private static namePiva: string;
  private static nameTot: string;
  private static nameAvarage: string;

  private dataSource = new Map<string, ReplaySubject<WidgetIndiciProduttivitaGlobal>>();
  private validYearsSource = new Map<string, ReplaySubject<number[]>>();

  constructor(
    private widgetsClient: WidgetIndiciClient,
    private translocoService: TranslocoService
  ) {
    ComparisonWidgetService.namePiva = this.translocoService.translate('AziendaSelezionata');
    ComparisonWidgetService.nameTot = this.translocoService.translate('ValoreMedioInPortafoglio');
    ComparisonWidgetService.nameAvarage = this.translocoService.translate('MediaRegionale');
  }

  public getData$(piva: string, years: number[]): Observable<WidgetIndiciProduttivitaGlobal> {
    const sortedYears = years.sort((a, b) => (a - b));
    const key = piva + '-' + sortedYears.join('-');
    if (this.dataSource.has(key)) {
      return this.dataSource.get(key).asObservable();
    }

    const source = new ReplaySubject<WidgetIndiciProduttivitaGlobal>(1);
    this.dataSource.set(key, source);
    this.nextData(piva, sortedYears);
    return source.asObservable();
  }

  private nextData(piva: string, years: number[]): void {
    const key = piva + '-' + years.join('-');

    // TODO VANNI: Richiesta una sola azienda e una sola specie
    this.widgetsClient
      .widgetIndiciGetWidgetProduttivita({
        requestXImpresa: [{
          piva: piva,
          specieXYear: years.map(year => ({
            year: year,
            specieVegetale: []
          }))
        }],
      } as WidgetRequestIndiciProduttivita)
      .pipe(
        map(res => JSON.parse(res.RispostaStringa) as WidgetIndiciProduttivitaGlobal),
        catchError(() => of(null))
      )
      .subscribe(res => this.dataSource.get(key).next(res));
  }

  public getYears$(piva: string) {
    if (this.validYearsSource.has(piva)) {
      return this.validYearsSource.get(piva).asObservable();
    }

    const source = new ReplaySubject<number[]>(1);
    this.validYearsSource.set(piva, source);
    this.widgetsClient
      .widgetIndiciGetAvailableYearsIndiciProduttivita(piva)
      .pipe(
        map(res => JSON.parse(res.RispostaStringa) as number[]),
        catchError(() => of([]))
      )
      .subscribe(res => this.validYearsSource.get(piva).next(res));

    return source.asObservable();
  }

  public static getIndexData(input: WidgetIndiciProduttivitaGlobal, index: string): ComparisonWidgetData[][] {
    const result: ComparisonWidgetData[][] = [];
    const data = input?.indiciXImpresa[0]?.indiciXAnno;
    if (data == null) {
      return result;
    }

    for (let i = 0; i < data.length; i++) {
      const valuePiva = data[i]?.indiciProduttivita?.find(x => x.indiceProduttivitaAICod >= 0)?.indici?.find(x => x.des == index)?.value;
      const valueTot = data[i]?.indiciProduttivita?.find(x => x.indiceProduttivitaAICod == PIVA_VALORE_MEDIO_IN_PORTAFOGLIO)?.indici?.find(x => x.des == index)?.value;
      const valueAvarage = data[i]?.indiciProduttivita?.find(x => x.indiceProduttivitaAICod < 0 && x.indiceProduttivitaAICod != PIVA_VALORE_MEDIO_IN_PORTAFOGLIO)?.indici?.find(x => x.des == index)?.value;

      const local = [
        { index: valuePiva, name: ComparisonWidgetService.namePiva, year: data[i].year.toString(), color: MULTI_YEAR_PALETTE[0] },
        { index: valueTot, name: ComparisonWidgetService.nameTot, year: data[i].year.toString(), color: MULTI_YEAR_PALETTE[1] },
      ];

      if (index != 'Produttivita') {
        local.push({ index: valueAvarage, name: ComparisonWidgetService.nameAvarage, year: data[i].year.toString(), color: MULTI_YEAR_PALETTE[2] });
      }

      result.push(local);
    }

    if (result.length > 1) {
      // Use multi year palette
      for (let i = 0; i < data.length; i++) {
        for (const element of result[i]) {
          element.color = MULTI_YEAR_PALETTE[i];
        }
      }
    }

    return result;
  }
}
