import { Injectable } from '@angular/core';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { Observable, take, map, tap, ReplaySubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MappaturaIsolamentiService {

  public speciesClasses: ReplaySubject<any[] | null> = new ReplaySubject<any[] | null>(null);

  constructor(
    private apiService: AjaxAgronicaAPIService
  ) { }

  public loadClassiSpecieSementieri(): Observable<any[]> {
    return this.apiService.ajaxAPIGet("MetaschemaNG/CaricaClassiSpecieSementieri", "")
      .pipe(
        take(1),
        map(res => res.RispostaOK ? res.RispostaStringa as any[] : []),
        tap(classes => this.speciesClasses.next(classes))
      );
  }

  public loadUsersClasses(): Observable<any[]> {
    return this.apiService.ajaxAPIGet("MetaschemaNG/CaricaClassiSpecieXUtentiSementieri", "")
      .pipe(
        take(1),
        map(res => res.RispostaOK ? res.RispostaStringa as any[] : [])
      );
  }

  public saveUserClassSpecies(dataItem: any): Observable<boolean> {
    return this.apiService.ajaxAPIPost<any, any>("MetaschemaNG/SalvaClassiSpecieXUtentiSementieri", dataItem)
      .pipe(take(1), map(res => res.RispostaOK));
  }
}
