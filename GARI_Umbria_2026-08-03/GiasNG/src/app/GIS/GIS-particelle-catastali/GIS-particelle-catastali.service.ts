import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map, of, tap } from 'rxjs';
import { JsonKendoResult } from 'gias-kendo-grid';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { CatastoService } from 'app/Service/Anagrafica/catasto.service';

@Injectable()
export class GISParticelleCatastaliService {
  private cache = new Map<string, GISParticelleCatastaliModel[]>();
  private pivaSubject = new BehaviorSubject<string>(null);
  private existingSubject = new BehaviorSubject<string[]>([]);

  constructor(private catastoService: CatastoService) { }

  public get piva$(): Observable<string> {
    return this.pivaSubject.asObservable();
  }

  public nextPiva(value: string): void {
    this.pivaSubject.next(value);
  }

  public get existing$(): Observable<string[]> {
    return this.existingSubject.asObservable();
  }

  public nextExisting(value: string[]): void {
    this.existingSubject.next(value);
  }

  public read(piva: string, existingIds: string[]): Observable<GISParticelleCatastaliModel[]> {
    const result = this.cache.get(piva);
    if (result != null && result.length > 0) {
      return of(result.filter(x => !existingIds.includes(x.rowId)));
    }

    const body = { Piva: piva, Data: new Date(new Date('1900-01-01').toDateString()) } as ObjParametriAgenda;
    return this.catastoService.LeggiCatastoAzienda(body)
      .pipe(
        map(res => GISParticelleCatastaliService.parseResult(res)),
        map(items => [...new Map(items.map(item => [item.rowId, item])).values()]),
        tap(items => this.cache.set(piva, items)),
        map(items => items.filter(x => !existingIds.includes(x.rowId)))
      );
  }

  private static parseResult(jsonKendoResult: JsonKendoResult): GISParticelleCatastaliModel[] {
    return jsonKendoResult.kendo_rows.map((row: any) => ({
      rowId: `${row.Prov}-${row.com}-${row.SEZIONE}-${row.FOGLIO}-${row.NUMERO}-${row.SUBALTERNO}`,
      Prov: row.Prov,
      Com: row.Com,
      PROVINCIA: row.PROVINCIA,
      COMUNE: row.COMUNE,
      SEZIONE: row.SEZIONE,
      FOGLIO: row.FOGLIO,
      NUMERO: row.NUMERO,
      SUBALTERNO: row.SUBALTERNO,
      Titolo_possesso_cod: row.Titolo_Possesso_Cod,
      Titolo_possesso: row.Titolo_possesso,
      Sup_Condotta: row.Sup_Condotta,
      Validita_Inizio: row.Validita_Inizio,
      Validita_Fine: row.Validita_Fine,
    } as GISParticelleCatastaliModel));
  }
}

export interface GISParticelleCatastaliModel {
  rowId: string;
  Prov: string;
  Com: string;
  PROVINCIA: string;
  COMUNE: string;
  SEZIONE: string;
  FOGLIO: number;
  NUMERO: number;
  SUBALTERNO: string;
  Titolo_possesso_cod: number;
  Titolo_possesso: string;
  Sup_Condotta: number;
  Validita_Inizio: string; // Date
  Validita_Fine: string; // Date
}