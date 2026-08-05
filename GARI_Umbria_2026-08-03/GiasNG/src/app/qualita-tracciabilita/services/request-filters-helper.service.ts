import { Injectable } from '@angular/core';
import { TranslocoService } from "@jsverse/transloco";
import { Observable, BehaviorSubject, tap, map } from 'rxjs';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { VisibilitaService } from 'app/profilazione/services/visibilita.service';
import { CentriAziendaliService } from 'app/Service/Anagrafica/centri.service';
import { ImpiantiService } from 'app/Service/Anagrafica/impianti.service';
import { SpecieVegetaliService } from 'app/Service/Metaschema/specie-vegetali.service';
import { DisciplinariService } from 'app/Service/Metaschema/disciplinari.service';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { BaseCodeDescrStr } from 'app/Model/baseClass/baseCodeDescrStr';
import { OperationTypes } from "app/qualita-tracciabilita/models/operation-types.enum";
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';

@Injectable({
  providedIn: 'root'
})
export class RequestFiltersHelperService {

  public readonly companies$: BehaviorSubject<BaseCodeDescrStr[]> = new BehaviorSubject<BaseCodeDescrStr[]>([]);
  public readonly businessCenters$: BehaviorSubject<BaseCodeDescr[]> = new BehaviorSubject<BaseCodeDescr[]>([]);
  public readonly species$: BehaviorSubject<BaseCodeDescr[]> = new BehaviorSubject<BaseCodeDescr[]>([]);
  public readonly plots$: BehaviorSubject<BaseCodeDescrStr[]> = new BehaviorSubject<BaseCodeDescrStr[]>([]);
  public readonly protocols$: BehaviorSubject<BaseCodeDescrStr[]> = new BehaviorSubject<BaseCodeDescrStr[]>([]);
  public readonly operations: BaseCodeDescr[] = [
    new BaseCodeDescr(OperationTypes.All, this.transloco.translate('TutteLeOperazioni')),
    new BaseCodeDescr(OperationTypes.Treatment, this.transloco.translate('Trattamenti')),
    new BaseCodeDescr(OperationTypes.Fertilization, this.transloco.translate('Fertilizzazioni')),
    new BaseCodeDescr(OperationTypes.Harvest, this.transloco.translate('Raccolte')),
  ];

  public readonly defaultBusinessCenter = new BaseCodeDescr(0, this.transloco.translate('TuttiICentriAziendali'));
  public readonly defaultProtocol = new BaseCodeDescrStr('', this.transloco.translate('IndicatoInOperazione'));
  public readonly defaultSpecie = new BaseCodeDescr(0, this.transloco.translate('TutteLeSpecie'));
  public readonly defaultPlot = new BaseCodeDescrStr('', this.transloco.translate('TuttiGliImpianti'));
  public readonly defaultOperation = this.operations[0];

  constructor(
    private transloco: TranslocoService,
    private master: MasterService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private visibilitaService: VisibilitaService,
    private centriService: CentriAziendaliService,
    private specieService: SpecieVegetaliService,
    private impiantiService: ImpiantiService,
    private disciplinariService: DisciplinariService,
  ) { }

  // #region Load Data
  public loadCompanies(): Observable<BaseCodeDescrStr[]> {
    return this.visibilitaService.leggiVisibilitaUtente(this.master.getCurrentUserUsername(), false, false)
      .pipe(
        tap(vis => this.companies$.next(vis.map(x => new BaseCodeDescrStr(x.piva, x.rag_soc)))),
        map(() => this.companies$.value)
      );
  }

  public loadCompanyCenters(piva: string): Observable<BaseCodeDescr[]> {
    const p = this.objParametriAgendaService.getObjParamValue();
    p.piva = piva;
    return this.centriService.leggiCentri(p)
      .pipe(
        tap(centri => {
          centri.unshift(this.defaultBusinessCenter);
          this.businessCenters$.next(centri.map(c => new BaseCodeDescr(c.sa_cod, c.sa_nome)))
        }),
        map(() => this.businessCenters$.value)
      );
  }

  public loadSpecies(piva: string, sacod: number = 0): Observable<BaseCodeDescr[]> {
    return this.specieService.leggiPerCentroAziendale(piva, sacod)
      .pipe(
        tap(species => {
          if (species.length > 1) {
            species.unshift(this.defaultSpecie);
          }
          this.species$.next(species);
        }),
        map(() => this.species$.value)
      );
  }

  public loadPlots(piva: string, sacod: number = 0, vegcod: number = 0, period?: IntervalloTemporale): Observable<BaseCodeDescrStr[]> {
    period = period ?? new IntervalloTemporale();
    return this.impiantiService.leggiPerCentroSpecie(piva, sacod, vegcod, period.inizio, period.fine)
      .pipe(
        tap(plots => {
          if (plots.length > 1) {
            plots.unshift(this.defaultPlot);
          }
          this.plots$.next(plots);
        }),
        map(() => this.plots$.value)
      );
  }

  public loadProtocols(vegcod: number = 0): Observable<BaseCodeDescrStr[]> {
    return this.disciplinariService.leggi({
      lavorazione: null,
      specie: new BaseCodeDescr(vegcod),
      data: new Date(),
      privato: false,
      regolamento: null,
      // validita: new IntervalloTemporale(this.startDate, this.endDate),
    }).pipe(
      tap(protocols => this.protocols$.next(protocols as BaseCodeDescrStr[])),
      map(() => this.protocols$.value)
    );
  }
  // #endregion
}
