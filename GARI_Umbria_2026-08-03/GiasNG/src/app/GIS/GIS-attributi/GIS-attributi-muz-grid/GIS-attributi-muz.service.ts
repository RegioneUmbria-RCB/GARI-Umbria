import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { Plot } from './GIS-attributi-muz-grid.component';
import { DatiMUZVisibili_Out, GruppoAreaOmogenea, MUZ } from 'app/Service/api.service';

export const DEFAULT_PLOT = { Appezza: -1, Desc: 'Nessuno' } as Plot;

@Injectable()
export class GISAttributiMuzService {
  private selectedPlotSubject = new BehaviorSubject<Plot>(DEFAULT_PLOT);
  private groupsSubject = new BehaviorSubject<GruppoAreaOmogenea[] | null>(null);
  private muzsSubject = new BehaviorSubject<DatiMUZVisibili_Out[] | null>(null);
  private gridLoadRequestSubject = new Subject<void>();
  private muzToEditSubject = new BehaviorSubject<MUZ | null>(null);

  public get selectedPlot$(): Observable<Plot> {
    return this.selectedPlotSubject.asObservable();
  }

  public nextSelectedPlot(selectedPlot: Plot): void {
    this.selectedPlotSubject.next(selectedPlot);
  }

  public get groups$(): Observable<GruppoAreaOmogenea[] | null> {
    return this.groupsSubject.asObservable();
  }

  public nextGroups(groups: GruppoAreaOmogenea[]): void {
    this.groupsSubject.next(groups);
  }

  public get muzs$(): Observable<DatiMUZVisibili_Out[] | null> {
    return this.muzsSubject.asObservable();
  }

  public nextMuzs(muzs: DatiMUZVisibili_Out[] | null): void {
    this.muzsSubject.next(muzs);
  }

  public get gridLoadRequest$(): Observable<void> {
    return this.gridLoadRequestSubject.asObservable();
  }

  public nextGridLoadRequest(): void {
    this.gridLoadRequestSubject.next();
  }

  public get muzToEdit$(): Observable<MUZ | null> {
    return this.muzToEditSubject.asObservable();
  }

  public nextMuzToEdit(muz: MUZ | null): void {
    this.muzToEditSubject.next(muz);
  }

  public isDefaultPlot(): boolean {
    return this.selectedPlotSubject.value.Appezza == DEFAULT_PLOT.Appezza || this.selectedPlotSubject.value.GroupCod == null;
  }

  public static plotInMuz(muz: DatiMUZVisibili_Out, plot: Plot): boolean {
    return muz.MUZ_Appezzamenti.find(x => x.Piva == plot.Piva && x.Sa_Cod == plot.Sa_Cod && x.Appezza == plot.Appezza) != null;
  }
}
