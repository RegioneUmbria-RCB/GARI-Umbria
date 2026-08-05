import { Component, ElementRef, EventEmitter, Input, OnChanges, OnDestroy, Output } from '@angular/core';
import { Enum_SiteRedirector } from 'app/Model/siti.enum';
import { LinkMenu } from 'app/Service/api.service';
import { ExternalNavigationService } from 'app/Service/external-navigation.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import { Subscription } from 'rxjs';

export interface CustomLinkMenu extends LinkMenu {
  open: boolean;
  hidden: boolean;
}

@Component({
  standalone: false,
  selector: 'gias-sidemenu-services',
  templateUrl: './sidemenu-services.component.html',
  styleUrls: ['./sidemenu-services.component.scss']
})
export class SideMenuServicesComponent implements OnChanges, OnDestroy {

  @Input() menus: LinkMenu[] = [];
  @Input() filterText = "";

  @Output() childClicked = new EventEmitter<LinkMenu>();

  viewMenus: CustomLinkMenu[] = [];
  currentObjParametriAgendaService: ObjParametriAgenda;

  private sub: Subscription;

  constructor(
    private objParametriAgendaService: ObjParametriAgendaService,
    private externalNavigationService: ExternalNavigationService
  ) {
    this.currentObjParametriAgendaService = this.objParametriAgendaService.getObjParamValue();
    this.sub = this.objParametriAgendaService.currentObjParametriAgenda.subscribe(x => this.currentObjParametriAgendaService = x);
  }

  get isExternalLoad(): boolean {
    return this.externalNavigationService.externalLoad;
  }

  ngOnChanges(): void {
    if (this.menus == null) {
      return;
    }

    this.viewMenus = this.menus.map((x) => ({ ...x, open: false, hidden: false } as CustomLinkMenu));
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  isClickable(menu: CustomLinkMenu): boolean {
    return this.hasValidUrl(menu) || menu.Figli.length > 0;
  }

  redirectMenu(menu: CustomLinkMenu, submenuContainer: ElementRef | null = null): void {
    if (this.hasValidUrl(menu)) {
      if (menu.sitoRichiesto == Enum_SiteRedirector.GiasNG && menu.idSezione != null) {
        this.currentObjParametriAgendaService.IdSezione = menu.idSezione;
        this.objParametriAgendaService.changeObjParametriAgenda(this.currentObjParametriAgendaService);
      }

      this.childClicked.emit(menu);
      return;
    }

    this.openMenu(menu, submenuContainer);
  }

  openMenu(menu: CustomLinkMenu, submenuContainer: any | null = null): void {
    const open = !menu.open;
    for (const element of this.viewMenus) {
      element.open = false;
    }
    menu.open = open;

    if (open && submenuContainer) {
      setTimeout(() => {
        const submenuRect = submenuContainer.getBoundingClientRect();
        const isSubmenuNotVisible = submenuRect.top < 0 || submenuRect.bottom > window.innerHeight;

        if (isSubmenuNotVisible) {
          submenuContainer.scrollIntoView({ behavior: 'smooth', block: 'end' });
        }
      }, 300);
    }
  }

  isOpen(menu: CustomLinkMenu): boolean {
    if (menu.open) {
      return true;
    }

    if (this.isLastMenuFiltered(menu)) {
      this.openMenu(menu);
    }

    return false;
  }

  isActive(menu: CustomLinkMenu): boolean {
    const idSezione = this.currentObjParametriAgendaService.IdSezione;
    if (this.isDashboard()) {
      return false;
    }

    return menu.idSezione == idSezione || menu.Figli.find(x => x.idSezione == idSezione) != null;
  }

  isDashboard(): boolean {
    const idSezione = this.currentObjParametriAgendaService.IdSezione;
    return idSezione == null || idSezione <= 0;
  }

  private isLastMenuFiltered(menu: CustomLinkMenu): boolean {
    return this.filterText != "" && !menu.hidden && this.viewMenus.filter(m => !m.hidden).length == 1
  }

  private hasValidUrl(menu: CustomLinkMenu): boolean {
    if (menu.redirectUrl != null && menu.redirectUrl !== '' && menu.sitoRichiesto == 0) {
      menu.sitoRichiesto = Enum_SiteRedirector.Sito_AgronicaAgenda_2010;
      return true;
    }

    if (menu.sitoRichiesto > 0) {
      return true;
    }

    return menu.redirectUrl != null && menu.redirectUrl !== '';
  }
}
