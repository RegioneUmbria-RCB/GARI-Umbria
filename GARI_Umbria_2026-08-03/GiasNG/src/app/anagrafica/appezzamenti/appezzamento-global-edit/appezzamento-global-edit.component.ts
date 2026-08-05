import { Component, Inject, OnDestroy, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import { ExpansionPanelComponent } from '@progress/kendo-angular-layout';
import { enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { Appezzamento } from 'app/Model/anagrafiche/Appezzamento';
import { IntlService } from '@progress/kendo-angular-intl';
import { FormBuilder, FormGroup } from '@angular/forms';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { Impianto } from 'app/Model/anagrafiche/Impianto';
import { Esercizio } from 'app/Model/anagrafiche/Esercizio';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { ImpiantiFactoryService, IMPIANTI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/impianti.factory.service';
import { CampiServiceProvider } from 'app/Service/ServiceFactory/campi.factory.provider';
import { ImpiantiServiceProvider } from 'app/Service/ServiceFactory/impianti.factory.provider';
import {MenuContestualeService} from '../../../Master/menu-contestuale/menu-contestuale.service';
import { Subject, takeUntil } from 'rxjs';
import {Utente_Impostazioni} from '../../../Model/utente/utente_impostazioni';
import {enum_Impostazioni_Utenti} from '../../../Model/Impostazioni_Utenti.enum';
import {PermessiUtenteService} from '../../../Service/permessi-utente.service';

@Component({
  standalone: false,
  selector: 'app-appezzamento-global-edit',
  templateUrl: './appezzamento-global-edit.component.html',
  styleUrls: ['./appezzamento-global-edit.component.css'],
  providers: [GiasDropDownTemplateService]
})
export class AppezzamentoGlobalEditComponent implements OnInit, OnDestroy {

  public signal$: Subject<void> = new Subject();

  private piva: string;
  private sa_cod: number;
  private appezza: number;
  private tipoperazioneDB: enum_TipoOperazioneDB;

  @ViewChildren(ExpansionPanelComponent) panels: QueryList<ExpansionPanelComponent>;
  private objParametriAgenda: ObjParametriAgenda;
  private appezzamento: Appezzamento;

  constructor(
    private route: ActivatedRoute,
    private masterService: MasterService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private menuContestualeService: MenuContestualeService,
    @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
    private permessiUtenteService: PermessiUtenteService
  ) { }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }

  async ngOnInit() {
    this.masterService.set_isLoading({ isLoading: true, message: '' });

    try {
      this.appezzamentiService.setAppezzamento(null);
      this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

      if (this.objParametriAgenda.Sa_Cod == 0 || this.objParametriAgenda.Appezza == 0) {
        this.tipoperazioneDB = enum_TipoOperazioneDB.Scrittura;
        const validities = this.getValidityFromUserSettings();

        let appezzamento: Appezzamento = new Appezzamento(
          {
            codice: 0,
            centroAziendalePK: {
              codice: 0,
              partitaIva: this.objParametriAgenda.Piva
            }
          }
        );
        appezzamento.campoPK = { codice: 0, centroAziendalePK: { codice: 0, partitaIva: ''} }
        appezzamento.validita = validities.app;

        let impianto: Impianto = new Impianto({ codice: 0, appezzamentoPK: { codice: 0, centroAziendalePK: { codice: 0, partitaIva: '' }}});
        impianto.validita = validities.imp;

        let esercizio: Esercizio = new Esercizio(0);
        esercizio.validita = validities.ex;

        let esercizi: Esercizio[] = [];
        esercizi.push(esercizio);

        impianto.esercizi = esercizi;

        let impianti: Impianto[] = [];
        impianti.push(impianto);
        appezzamento.impianti = impianti;

        this.appezzamento = appezzamento;
      } else {
        this.tipoperazioneDB = enum_TipoOperazioneDB.Modifica;

        this.appezzamento = await this.appezzamentiService.leggiAppezzamento(this.objParametriAgenda,
          AGRODATAINIZIO,
          false,
          true,
          true,
          true,
          true,
          true,
          true
        );
      }

      this.handleQueryParams();
      this.appezzamentiService.setAppezzamento(this.appezzamento);
    } catch (e) {
      this.masterService.changeErrorMsgType({ show: true, msg: e.message, errorNumber: 0 });
    }

    this.masterService.set_isLoading({isLoading: false, message: ''});
  }

  public onAction(ev, index): void {
    this.panels.forEach((panel, idx) => {
      if (idx !== index && panel.expanded) {
        panel.toggle();
      }
    });
  }

  private handleQueryParams(): void {
    this.route.queryParams.pipe(takeUntil(this.signal$))
      .subscribe(params => {
        if (params.seFrame == 1) {
          let header = this.masterService.getHeader();
          header.visible = false;
          this.masterService.changeHeader(header);

          let footer = this.masterService.getFooter();
          footer.visible = false;
          this.masterService.changeFooter(footer);

          let menuContestuale = this.menuContestualeService.getMenuContestualeSettings();
          menuContestuale.show = false;
          this.menuContestualeService.changeMenuContestualeSettings(menuContestuale);
        }
        if(params.wkt != undefined && params.wkt != '') {
          this.appezzamento.cartografia = params.wkt;
        }
      });
  }

  private getValidityFromUserSettings(): { app: IntervalloTemporale, imp: IntervalloTemporale, ex: IntervalloTemporale } {
    let validitySettingInterpretation: Utente_Impostazioni = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.UTENTE_InterpretazioneInizioFineAnnataAgraria);
    const settings: IntervalloTemporale = this.extractValidityFromSetting();

    let appValidity: IntervalloTemporale;
    let impValidity: IntervalloTemporale;
    let exValidity: IntervalloTemporale = new IntervalloTemporale(settings.inizio, settings.fine);

    switch (validitySettingInterpretation?.Valore ?? '2') {
      case '0':
        appValidity = new IntervalloTemporale(settings.inizio, settings.fine);
        impValidity = new IntervalloTemporale(settings.inizio, settings.fine);
        break;
      case '1':
        appValidity = new IntervalloTemporale(settings.inizio, AGRODATAFINE);
        impValidity = new IntervalloTemporale(settings.inizio, settings.fine);
        break;
      case '2':
        appValidity = new IntervalloTemporale(settings.inizio, AGRODATAFINE);
        impValidity = new IntervalloTemporale(settings.inizio, AGRODATAFINE);
        break;
      default:
        appValidity = new IntervalloTemporale(new Date(new Date().getFullYear(), 0, 1), AGRODATAFINE);
        impValidity = new IntervalloTemporale(new Date(new Date().getFullYear(), 0, 1), AGRODATAFINE);
        exValidity = new IntervalloTemporale(
          new Date(new Date().getFullYear(), 0, 1), new Date(new Date().getFullYear(), 11, 31)
        );
        break;
    }

    return {app: appValidity, imp: impValidity, ex: exValidity};
  }

  private extractValidityFromSetting(): IntervalloTemporale {
    // questo codice è ripetuto nel file esercizi-grid.service, appena si ha tempo correggere
    let validitySetting: Utente_Impostazioni = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.UTENTE_InizioFineAnnataAgraria);

    // default values, if no setting is found
    let endYear: number = new Date().getFullYear();
    let endMonth: number = 11;
    let endDay: number = 31;
    let startYear: number = new Date().getFullYear();
    let startMonth: number = 0;
    let startDay: number = 1;

    if (validitySetting?.Valore != undefined) {
      let startSet: string = validitySetting.Valore.trim().slice(0, 4);
      let endSet: string = validitySetting.Valore.trim().slice(4);

      startDay = Number.parseInt(startSet.slice(0, 2));
      startMonth = Number.parseInt(startSet.slice(2)) - 1;

      endDay = Number.parseInt(endSet.slice(0, 2));
      endMonth = Number.parseInt(endSet.slice(2)) - 1;

      const today: Date = new Date();
      if (startMonth < today.getMonth() || (today.getMonth() === startMonth && startDay < today.getDate())) {
        startYear = today.getFullYear();
      } else {
        startYear = today.getFullYear() - 1;
      }

      // controllo se la differenza è minore di 0, ossia l'annata agrazia inizia e finisce nello stesso anno solare
      if (endMonth > startMonth) {
        endYear = startYear;
      } else {
        endYear = startYear + 1;
      }
    }

    const start: Date = new Date(startYear, startMonth, startDay);
    const end: Date = new Date(endYear, endMonth, endDay);
    return new IntervalloTemporale(start, end);
  }

}
