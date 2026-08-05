import { Component } from '@angular/core';
import { ProfilazioneImpreseService, Specie, Varieta } from '../../services/profilazione-imprese.service';
import { BehaviorSubject, Subject } from 'rxjs';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from 'app/Service/FunzioniComuni.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { map } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-default-piani-colturali',
  templateUrl: './profilazione-imprese-default-piani-colturali.component.html',
  styleUrls: ['./profilazione-imprese-default-piani-colturali.component.scss', '../../profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseDefaultPianiColturaliComponent {

  additionalSpecie = { Veg_Cod: 0, Veg_Des: 'Tutte le Varietà' } as Specie;
  additionalVarieta = { Cul_Cod: 0, Cul_Des: 'Tutte le Varietà' } as Varieta;
  dropdownSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;

  globalSubject = this.service.globalSubject;
  specieSubject = this.service.defaultPianiColturaliSpecieSubject;
  tutteSpecieSubject = new BehaviorSubject<boolean>(false);
  varietaSubject = new Subject<number>();
  piva$ = this.objParametriAgendaService.currentObjParametriAgenda.pipe(map(x => x.Piva));

  constructor(
    private service: ProfilazioneImpreseService,
    private objParametriAgendaService: ObjParametriAgendaService
  ) { }

  addDefault(vegCod: number, culCod: number, global: boolean, piva: string): void {
    this.service
      .scriviParametriGenerali({ Piva: global ? '' : piva, VegCod: vegCod, CulCod: culCod })
      .subscribe(() => this.service.reloadData.next(null));
  }
}
