import { Directive, HostListener } from '@angular/core';
import { NavigationService } from './Service/navigation.service';
import { ObjParametriAgendaService } from './Service/obj-parametri-agenda.service';

@Directive({ standalone:false,
  selector: '[homeButton]'
})
export class HomeButtonDirective {
  constructor(
    private navigation: NavigationService,
    private objParametriAgendaService: ObjParametriAgendaService,
  ) { }

  @HostListener('click')
  onClick(): void {
    const params = this.objParametriAgendaService.getObjParamValue();
    params.IdSezione = 0;
    this.objParametriAgendaService.changeObjParametriAgenda(params);

    this.navigation.home();
  }
}
