import { Component, Input, OnInit } from '@angular/core';
import { ProfilazioneImpreseService, Specie } from 'app/profilazione-imprese/services/profilazione-imprese.service';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from 'app/Service/FunzioniComuni.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { BehaviorSubject, combineLatest, map, Observable, Subject, switchMap } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-specie',
  templateUrl: './profilazione-imprese-specie.component.html',
  styleUrls: ['./profilazione-imprese-specie.component.scss', '../../profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseSpecieComponent implements OnInit {
  dropdownSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;

  @Input() globalSubject: BehaviorSubject<boolean> | null = null;
  @Input() additionalElement: Specie | null = null;
  @Input() specieSubject: Subject<string> | null = null;

  specie$: Observable<Specie[]> | null = null;

  constructor(
    private service: ProfilazioneImpreseService,
    private objParametriAgendaService: ObjParametriAgendaService
  ) { }

  ngOnInit(): void {
    this.specie$ = combineLatest([this.objParametriAgendaService.currentObjParametriAgenda, this.globalSubject]).pipe(
      switchMap(([obj, global]) => global ? this.service.getSpecieGlobali() : this.service.getSpecieAziendali(obj.Piva)),
      map(specie => this.additionalElement == null ? specie : [this.additionalElement, ...specie])
    )
  }
}
