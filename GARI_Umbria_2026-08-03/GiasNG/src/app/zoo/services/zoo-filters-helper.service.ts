import {Injectable} from '@angular/core';
import {map, Observable, of, ReplaySubject, filter, tap} from "rxjs";
import {BaseCodeDescr as IBaseCodeDescr } from "../../Service/api.service";
import {OperazioniZooClient} from "../../Service/net-core6-api.service";
import {FormControl, FormGroup} from "@angular/forms";
import {Lavorazione} from "../../Model/attivita/Lavorazione";
import {BaseCodeDescrStr} from "../../Model/baseClass/baseCodeDescrStr";
import { DateUtility } from 'gias-ui-kit';
import {BaseCodeDescr} from "../../Model/baseClass/baseCodeDescr";
import {ZooOperationsFiltersForm} from "../models/zoo-operations-filters.model";
import {TranslocoService} from '@jsverse/transloco';

export class StableItems extends BaseCodeDescr {
  key: string;
  constructor(codice: number, key: string, descrizione?: string) {
    super(codice, descrizione);
    this.key = key;
  }
}

@Injectable({
  providedIn: 'root'
})
export class ZooFiltersHelperService {

  private lavorazioni: BaseCodeDescrStr[] = null;
  private lavorazioniPreferite: BaseCodeDescrStr[] = null;

  private center: { piva:string, centri:IBaseCodeDescr[] }[] = [];
  private stable: { piva:string, centro:number, stalle:StableItems[] }[] = [];

  public resetFilters$ = new ReplaySubject<boolean>(null);
  constructor(
    public transloco: TranslocoService,
    private client: OperazioniZooClient
  ) {
    this.resetFilters$.pipe(filter(value => !!value))
    .subscribe(() => {
      this.lavorazioni = null;
      this.lavorazioniPreferite = null;
      this.center = [];
      this.stable = [];
    });
  }

  private get yearStart(): Date {
    return new Date(DateUtility.getCurrentYear(), DateUtility.months.JANUARY, 1);
  }

  private get yearEnd(): Date {
    return new Date(DateUtility.getCurrentYear(), DateUtility.months.DECEMBER, 31);
  }

  public get monthStart(): Date {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth(), 1);
  }

  public get monthEnd(): Date {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth() + 1, 0);
  }

  /** Creates the zoo filters' FormGroup template. */
  public getFiltersFormGroup(): FormGroup<ZooOperationsFiltersForm> {
    const allCenters = 0;
    return new FormGroup<ZooOperationsFiltersForm>({
      center: new FormControl(allCenters),
      stable: new FormControl('0_0'),
      from: new FormControl(this.monthStart),
      to: new FormControl(this.monthEnd),
      operations: new FormControl([]) 
    });
  }

  /**
   * Loads the business centers for a specified business.
   * @param piva string containing the piva of the business of reference
   */
  public loadBusinessCenters(piva: string): Observable<IBaseCodeDescr[]> {
    if (this.center.find((el) => el.piva == piva) != null){
      return of(this.center.find((el) => el.piva = piva).centri);
    }
    return this.client.operazioniZooGetCentriAziendali(piva).pipe(
      map(r => JSON.parse(r.RispostaStringa)),
      map(centers => centers.map(x => {
        let data = new BaseCodeDescr(x['sa_cod'], x['sa_nome']);
        data['piva'] = piva;
        return data;
      })),
      map(centers => {
        const found = centers.find(x => x.codice === 0);
        if (!found) {
          centers = [new BaseCodeDescr(0, this.transloco.translate('TuttiICentriAziendali'))].concat(centers);
        }
        return centers;
      }),
      tap((centers) => this.center.push({piva: piva, centri: centers}))
    );
  }

  /**
   * Loads the stables available in the specified center.
   * @param piva string containing the piva of the business of reference
   * @param saCod string containing the center code of the center of reference
   */
  public loadStables(piva: string, saCod: number): Observable<StableItems[]> {
    if (this.stable.find((el) =>  el.piva == piva && el.centro == saCod) != null){
      return of(this.stable.find((el) =>  el.piva == piva && el.centro == saCod).stalle);
    }
    return this.client.operazioniZooGetStalle(piva, saCod)
      .pipe(
        map(r => JSON.parse(r.RispostaStringa)),
        map(stables => stables.map(x => new StableItems(x['STA_NUM'], x['sa_cod'] + '_' + x['STA_NUM'], x['STA_DES']))),
        tap(stables => stables.unshift(new StableItems(0, '0_0', this.transloco.translate('TutteLeStalle')))),
        tap((stables) => this.stable.push({piva: piva, centro: saCod, stalle: stables}))
      );
  }

  /**
   * Loads all animal races registered for the given piva, regardless of current date filters.
   */
  public loadRazzeZoo(): Observable<{ text: string; value: string }[]> {
    return this.client.operazioniZooGetRazzeZoo().pipe(
      map(r => JSON.parse(r.RispostaStringa) as { razza_key: string; razza_des: string }[]),
      map(rows => rows.map(r => ({ text: r.razza_des, value: r.razza_key })))
    );
  }

  /**
   * Loads the zoological operations available.
   */
  public loadZooOperations(): Observable<BaseCodeDescrStr[]> {
    if (this.lavorazioni != null){
      return of(this.lavorazioni);
    }
    return this.client.operazioniZooGetOperazioni().pipe(
      map(r => JSON.parse(r.RispostaStringa) as Lavorazione[]),
      map((ops: Lavorazione[]) => ops.map(x => new BaseCodeDescrStr(x.primaryKey.codice, x.descrizione))), 
      tap((lav) => this.lavorazioni = lav)
    );
  }

  /**
   * Loads the user's favorite zoological operations.
   */
  public loadFavorites(): Observable<BaseCodeDescrStr[]> {
    if (this.lavorazioniPreferite != null){
      return of(this.lavorazioniPreferite);
    }
    return this.client.operazioniZooGetOperazioniPreferite().pipe(
      map(r => JSON.parse(r.RispostaStringa) as Lavorazione[]),
      map((ops: Lavorazione[]) => ops.map(x => new BaseCodeDescrStr(x.primaryKey.codice, x.descrizione))),
      tap((lav) => this.lavorazioniPreferite = lav)
    );
  }

}
