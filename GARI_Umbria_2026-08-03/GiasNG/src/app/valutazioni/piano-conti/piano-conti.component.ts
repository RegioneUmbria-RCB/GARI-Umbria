import { Component, OnInit } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { PianoContiHttpService } from './piano-conti-grid-config.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Subscription, Subject, takeUntil } from 'rxjs';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { Router, ActivatedRoute } from '@angular/router';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { GridPublicService } from 'gias-kendo-grid';
import { ValutazioniService } from '../valutazioni/service/valutazioni.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { PianoContiService } from './service/pianoconti.service';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-piano-conti',
  templateUrl: './piano-conti.component.html',
  styleUrls: ['./piano-conti.component.css'],
  providers: [
    ...generateGridProviders(PianoContiHttpService, PianoContiComponent)
  ]
})
export class PianoContiComponent implements OnInit {

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
    private valutazioniService: ValutazioniService,
    private pianocontiService: PianoContiService) { }

  ngOnInit(): void {
    this.objParametriAgendaService.currentObjParametriAgenda.pipe(takeUntil(this.signal$)).subscribe((val) => {
      this.objParametriAgenda = val;
    });

    this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Valutazioni_Rischio, 2);

    this.editSubscription = this.kendoGridService.changeDetected.GiasSubscribe((event: any) => {
      if (event?.action === 'info') {
        this.infoValutazioni(event);
      }
    });
  }

  onNuovo() {
    this.objParametriAgenda = this.objParametriService.getObjParamValue();
    const piva = this.objParametriAgenda.Piva;
    this.objParametriAgenda.Piva = piva;
    this.objParametriAgenda.Sa_Cod = 0;
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
    this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);

    let leggi_Piano = {
      piva: "",
      pianoCod: 0
    };
    this.pianocontiService.changePianoConti(leggi_Piano);
    this.pianocontiService.changeValAssociato(0);
    this.router.navigate(['PianoConti-Edit'], { relativeTo: this.route });
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

    let leggi_Piano = {
      piva: dataItem.dataItem.Piva,
      pianoCod: dataItem.dataItem.Valutazione_Piano_Cod
    };
    this.pianocontiService.changePianoConti(leggi_Piano);
    this.pianocontiService.changeValAssociato(dataItem.Num_Valutazioni_Associate);

    this.router.navigate(['PianoConti-Edit'], { relativeTo: this.route });
  }

  ngOnDestroy() {
    this.signal$.next();
    this.signal$.complete();
  }

}
