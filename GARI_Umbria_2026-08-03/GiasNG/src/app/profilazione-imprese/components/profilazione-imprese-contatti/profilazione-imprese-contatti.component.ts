import { Component, Input, OnInit } from '@angular/core';
import { Contatto, ProfilazioneImpreseService } from 'app/profilazione-imprese/services/profilazione-imprese.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { BehaviorSubject, catchError, combineLatest, Observable, of, Subject, switchMap, tap } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-contatti',
  templateUrl: './profilazione-imprese-contatti.component.html',
  styleUrls: ['./profilazione-imprese-contatti.component.scss', '../../profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseContattiComponent implements OnInit {

  @Input() globalSubject: Subject<boolean> | null = null;
  @Input() contattiSelectedSubject: BehaviorSubject<number[]> | null = null;

  contattiLoading: boolean = false;
  contatti$: Observable<Contatto[]> | null = null;

  constructor(
    private service: ProfilazioneImpreseService,
    private objParametriAgendaService: ObjParametriAgendaService
  ) { }

  ngOnInit(): void {
    this.contatti$ = combineLatest([this.objParametriAgendaService.currentObjParametriAgenda, this.globalSubject])
      .pipe(
        tap(() => this.contattiLoading = true),
        switchMap(([obj, global]) => this.service.leggiContatti(global ? undefined : obj.Piva)),
        tap(x => x.forEach(y => y.description = `(${y.Cognome} ${y.Nome}) - ${y.Rapporto_Des}`)),
        catchError(() => of([])),
        tap(() => this.contattiLoading = false)
      );
  }

  selectContatto(selected: boolean, contatto: Contatto): void {
    const current = this.contattiSelectedSubject.value;
    if (selected) {
      current.push(contatto.Cod_RisUm);
    } else {
      const index = current.indexOf(contatto.Cod_RisUm);
      if (index > -1) {
        current.splice(index, 1);
      }
    }

    this.contattiSelectedSubject.next(current);
  }

  isContattoSelected(contatto: Contatto): boolean {
    const result = this.contattiSelectedSubject.value.find(x => contatto.Cod_RisUm == x) != null;
    return result;
  }

}
