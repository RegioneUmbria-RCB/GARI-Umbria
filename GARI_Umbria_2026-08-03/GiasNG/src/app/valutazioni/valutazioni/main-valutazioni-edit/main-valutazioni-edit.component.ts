import { Component, ElementRef, Injector, ViewChild } from '@angular/core';
import { faHome, faIndustry } from '@fortawesome/free-solid-svg-icons';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-main-valutazioni-edit',
  templateUrl: './main-valutazioni-edit.component.html',
  styleUrls: ['./main-valutazioni-edit.component.css']
})
export class MainValutazioniEditComponent {

  @ViewChild('valTopTar', { static: false })
  public valTopTar: ElementRef<HTMLElement>;

  writeMode: boolean = false;

  faHome = faHome;
  faIndustry = faIndustry;

  page: number;

  constructor(injector: Injector,
    private objParametriService: ObjParametriAgendaService,
  ) {

    this.page = 1;
    let objParametriAgenda = this.objParametriService.getObjParamValue();

    if(objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write){
      this.writeMode = true;
    }
  }

  setPage(num : number) : void{
    this.page = num;
  }

}
