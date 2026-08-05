import { Component, Input, OnInit } from '@angular/core';
import { ProfilazioneImpreseService, Varieta } from 'app/profilazione-imprese/services/profilazione-imprese.service';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from 'app/Service/FunzioniComuni.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { combineLatest, map, Observable, Subject, switchMap, tap } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-varieta',
  templateUrl: './profilazione-imprese-varieta.component.html',
  styleUrls: ['./profilazione-imprese-varieta.component.scss', '../../profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseVarietaComponent implements OnInit {
  dropdownSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;

  // @Input() globalSubject: BehaviorSubject<boolean> | null = null;
  @Input() additionalElement: Varieta | null = null;
  @Input() specieSubject: Subject<number> | null = null;
  @Input() tutteVarietaSubject: Subject<boolean> | null = null;
  @Input() varietaSubject: Subject<number> | null = null;

  varieta$: Observable<Varieta[]> | null = null;

  constructor(
    private service: ProfilazioneImpreseService,
    private objParametriAgendaService: ObjParametriAgendaService
  ) { }

  ngOnInit(): void {
    if (this.specieSubject == null) {
      throw new Error("Specie subject cannot be null");
    }

    this.varieta$ = combineLatest([this.objParametriAgendaService.currentObjParametriAgenda, this.tutteVarietaSubject, this.specieSubject])
      .pipe(
        switchMap(([obj, all, specie]) => all ? this.service.getAllVarieta(specie) : this.service.getFilteredVarieta(obj.Piva, specie)),
        map(varieta => this.additionalElement == null ? varieta : [this.additionalElement, ...varieta]),
        tap(varieta => {
          if (varieta.length > 0) {
            this.varietaSubject.next(varieta[0].Cul_Cod);
          }
        })
      );
  }
}
