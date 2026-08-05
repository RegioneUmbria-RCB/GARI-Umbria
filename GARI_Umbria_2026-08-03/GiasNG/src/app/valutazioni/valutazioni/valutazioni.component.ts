import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { Subject, Subscription } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { generateGridProviders } from 'gias-kendo-grid';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { GridPublicService } from 'gias-kendo-grid';
import { ValutazioniHttpService } from './valutazioni-grid-config.service';
import { EditEvent } from '@progress/kendo-angular-grid';
import { ValutazioniService } from './service/valutazioni.service';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-valutazioni',
  templateUrl: './valutazioni.component.html',
  styleUrls: ['./valutazioni.component.css'],
  providers: [
    ...generateGridProviders(ValutazioniHttpService, ValutazioniComponent)
  ]
})
export class ValutazioniComponent implements OnInit, OnDestroy {
  objParametriAgenda: ObjParametriAgenda;
  public permessoEdit: boolean;
  editSubscription: Subscription;

  public signal$: Subject<void> = new Subject();

  constructor(private objParametriAgendaService: ObjParametriAgendaService,
    private router: Router,
    private kendoGridService: GridPublicService,
    private route: ActivatedRoute,
    private permessiUtenteService: PermessiUtenteService,
    private objParametriService: ObjParametriAgendaService,
    private valutazioniService: ValutazioniService
  ) {

  }

  async ngOnInit() {
    this.objParametriAgendaService.currentObjParametriAgenda.pipe(takeUntil(this.signal$)).subscribe((val) => {
      this.objParametriAgenda = val;
    });

    this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Valutazioni_Rischio, 2);

    this.editSubscription = this.kendoGridService.changeDetected.GiasSubscribe((event: any) => {
      if (event?.action === 'remove') {
        this.eliminaValutazioni(event);
      }

      if (event?.action === 'info') {
        this.infoValutazioni(event);
      }
    });
  }

  eliminaValutazioni(event: EditEvent) {
    const datiRiga = event.dataItem;
    this.objParametriAgenda.Piva = datiRiga.piva;
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Delete;
  }

  onNuovo() {
    this.objParametriAgenda = this.objParametriService.getObjParamValue();
    const piva = this.objParametriAgenda.Piva;
    this.objParametriAgenda.Piva = piva;
    this.objParametriAgenda.Sa_Cod = 0;
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
    this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);

    let leggi_Testata = {
      piva: piva,
      idTestata: 0,
      includiAnno: true
    };
    this.valutazioniService.changeLeggiTestata(leggi_Testata);
    this.router.navigate(['Valutazioni-Edit'], { relativeTo: this.route });
  }

  infoValutazioni(dataItem: any) {
    console.log(dataItem);
    console.log("read");
    this.objParametriAgenda = this.objParametriService.getObjParamValue();
    const piva = dataItem.dataItem.Piva;
    const ragSocial = dataItem.dataItem.Rag_Soc;
    this.objParametriAgenda.Piva = piva;
    this.objParametriAgenda.RagSoc = ragSocial;
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
    this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);

    let leggi_Testata = {
      piva: dataItem.dataItem.Piva,
      idTestata: dataItem.dataItem.Id_Testata,
      includiAnno: true
    };
    this.valutazioniService.changeLeggiTestata(leggi_Testata);
    this.router.navigate(['Valutazioni-Edit'], { relativeTo: this.route });
  }

  ngOnDestroy() {
    this.signal$.next();
    this.signal$.complete();
  }
}
