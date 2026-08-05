import { Component, ElementRef, Injector, NgZone, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faHome, faIndustry } from '@fortawesome/free-solid-svg-icons';
import { TranslocoService, TranslocoPipe } from '@jsverse/transloco';
import { MenuContestualeService } from 'app/Master/menu-contestuale/menu-contestuale.service';
import { AGRODATAINIZIO, SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { BudgetService } from 'app/Service/Budget/budget.service';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { TreeContainerService } from 'app/Utility/Template/kendo-tree/services/tree-container.service';
import { AnagraficaService } from 'app/anagrafica/anagrafica.service';
import { SessionStorageService } from 'ngx-webstorage';
import { Subject } from 'rxjs';
import { RouterModule } from '@angular/router';

@Component({
  standalone: false,
  selector: 'app-valutazioni-main-component',
  templateUrl: './valutazioni-main-component.component.html',
  styleUrls: ['./valutazioni-main-component.component.css']
})
export class ValutazioniMainComponentComponent implements OnInit {

  @ViewChild('anchor', { static: false })
  public anchor: ElementRef<HTMLElement>;
  public AGRODATA_INIZIO = AGRODATAINIZIO;
  public signal$: Subject<void> = new Subject();

  @ViewChild('anagTopTar2', { static: false })
  public anagTopTar2: ElementRef<HTMLElement>;
  @ViewChild('topTabsContainer', { static: false })
  public topTabsContainer: ElementRef<HTMLElement>;
  @ViewChild('topTabsRightArrow', { static: false })
  public topTabsRightArrow: ElementRef<HTMLElement>;
  @ViewChild('topTabsLeftArrow', { static: false })
  public topTabsLeftArrow: ElementRef<HTMLElement>;

  public windowHtml = window;
  SMARTPHONE_WIDTH = SMARTPHONE_WIDTH;
  public Valutazioni_Rischio = enum_Security_Attivita.Valutazioni_Rischio;

  faIndustry = faIndustry;
  faHome = faHome;

  constructor(
    private router: Router,
    private permessiUtenteService: PermessiUtenteService,
    private zone: NgZone,
    private fb: FormBuilder,
    public drawerService: TreeContainerService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private injector: Injector,
    private gestioneRichiesteService: GestioneRichiesteService,
    private anagraficaService: AnagraficaService,
    private menuContestualeService: MenuContestualeService,
    private transloco: TranslocoService,
    private translocopipe: TranslocoPipe,
    private masterService: MasterService,
    private giasMessageService: GiasMessageService,
    private budgetService: BudgetService,
    private sessionSt: SessionStorageService,
    private route: ActivatedRoute,
  ) { }

  public moveToLeft() {
    // versione di codice con
    //const tabs = document.getElementById("anag-top-bar-2");
    this.anagTopTar2.nativeElement.scrollLeft -= 120;
  }

  public moveToRight() {
    // const tabs = document.getElementById("anag-top-bar-2");
    // tabs.scrollLeft += 100;
    this.anagTopTar2.nativeElement.scrollLeft += 120;
  }

  public mouseMoved(event: WheelEvent) {
    const direction = event.deltaY;
    if (direction <= 0) {
      this.moveToRight();
    } else {
      this.moveToLeft();
    }
  }

  public getPermesso(attivita: number, operazione: number): boolean {
    return this.permessiUtenteService.getPermesso(attivita, operazione);
  }

  ngOnInit(): void {
    //this.router.navigate(['Valutazioni-Main/Valutazioni']);
  }

}
