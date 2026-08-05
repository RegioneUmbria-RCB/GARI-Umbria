import { Component, Input, OnInit } from '@angular/core';
import { Macchina, ProfilazioneImpreseService } from 'app/profilazione-imprese/services/profilazione-imprese.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { BehaviorSubject, catchError, combineLatest, debounceTime, Observable, of, switchMap, tap } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-macchine',
  templateUrl: './profilazione-imprese-macchine.component.html',
  styleUrls: ['./profilazione-imprese-macchine.component.scss', '../../profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseMacchineComponent implements OnInit {

  @Input() globalSubject: BehaviorSubject<boolean> | null = null;
  @Input() macchineSelectedSubject: BehaviorSubject<number[]> | null = null;

  macchineLoading: boolean = false;
  macchine$: Observable<Macchina[]> | null = null;

  constructor(
    private service: ProfilazioneImpreseService,
    private objParametriAgendaService: ObjParametriAgendaService
  ) { }

  ngOnInit(): void {
    this.macchine$ = combineLatest([this.objParametriAgendaService.currentObjParametriAgenda, this.globalSubject])
      .pipe(
        debounceTime(100),
        tap(() => this.macchineLoading = true),
        switchMap(([obj, global]) => this.service.leggiParcoMacchine(global ? undefined : obj.Piva)),
        tap(x => x.forEach(y => y.Mac_Des = [y.Mac_Des, y.Ditta_Des, y.Modello, y.Referente].join(' - '))),
        catchError(() => of([])),
        tap(() => this.macchineLoading = false)
      );
  }

  selectMacchina(selected: boolean, macchina: Macchina): void {
    const current = this.macchineSelectedSubject.value;
    if (selected) {
      current.push(macchina.Mac_Cod);
    } else {
      const index = current.indexOf(macchina.Mac_Cod);
      if (index > -1) {
        current.splice(index, 1);
      }
    }

    this.macchineSelectedSubject.next(current);
  }

  isMacchinaSelected(macchina: Macchina): boolean {
    const result = this.macchineSelectedSubject.value.find(x => macchina.Mac_Cod == x) != null;
    return result;
  }
}
