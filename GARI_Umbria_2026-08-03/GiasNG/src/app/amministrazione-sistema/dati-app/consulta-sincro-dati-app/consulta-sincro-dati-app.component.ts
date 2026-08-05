import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { Subject, Subscription, takeUntil, map } from 'rxjs';
import { AGRODATAFINE, AGRODATAINIZIO } from '../../../Model/CostantiPersonalizzate';
import { MasterService } from '../../../Service/master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { TranslocoService } from '@jsverse/transloco';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { generateGridProviders } from 'gias-kendo-grid';
import { ConsultaSincroService, enum_Dati_App, enum_Filtro_Importati } from './consulta-sincro.service';
import { ConsultaSincroLogGridService } from './consulta-sincro-log-grid.service';
import { GridPublicService } from 'gias-kendo-grid';
import { faMagnifyingGlass } from '@fortawesome/free-solid-svg-icons';
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { enum_Esportazioni_Sistema_Cod } from 'app/Model/TipiEnumerativi';
import { pick } from 'lodash';

@Component({
  standalone: false,
  selector: 'app-consulta-sincro-dati-app',
  templateUrl: './consulta-sincro-dati-app.component.html',
  styleUrls: ['./consulta-sincro-dati-app.component.css'],
  providers: [
    ...generateGridProviders(ConsultaSincroLogGridService, ConsultaSincroDatiAppComponent)]
})
export class ConsultaSincroDatiAppComponent implements OnInit, OnDestroy {

  datiCaricati: boolean = false;
  isDemetra: boolean = false;
  objParametriAgenda: ObjParametriAgenda;
  formEnable: boolean;

  binaryButtonEnable: boolean;
  binaryButtonValue: boolean;
  binaryButtonName: string;

  faSearch = faMagnifyingGlass;

  public contestoCaricato: boolean = false;

  private inFrame: boolean;

  formCaricaDati: Subscription;
  valueChangesSub: Subscription;

  public signal$: Subject<void> = new Subject();

  consultaSincroDatiAppForm: FormGroup = new FormGroup({
    datiAggiuntivi: new FormControl(false),
    tipo: new FormControl([]),
    pivaCUAA: new FormControl(''),
    validita: this.fb.group({
      inizio: [AGRODATAINIZIO],
      fine: [AGRODATAFINE]
    }),
    filtroImportati: new FormControl(0)
  });

  constructor(

    private masterService: MasterService,

    private fb: FormBuilder,
    private router: Router,
    private location: Location,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private translocoService: TranslocoService,
    private gridPublicService: GridPublicService,
    private route: ActivatedRoute,
    private consultaSincroService: ConsultaSincroService,
        @Inject(GRID_HTTP_TOKEN) protected consultaSincroLogGridService: ConsultaSincroLogGridService
  ) { }

  async ngOnInit() {
    this.handleQueryParams();
    let dataInizio = new Date();
    dataInizio.setDate(dataInizio.getDate() - 1)
    this.consultaSincroDatiAppForm.controls['validita'].patchValue({
      inizio: dataInizio,
      fine: AGRODATAFINE
    })
    this.consultaSincroDatiAppForm.controls['filtroImportati'].patchValue({ codice: 0, descrizione: this.translocoService.translate("Importati") })
    this.consultaSincroService.controllaModalitaDemetra().pipe(map(isDemetra => {
      this.contestoCaricato = true;
      this.isDemetra = isDemetra;
      this.consultaSincroLogGridService.isDemetra = isDemetra;
      if (this.isDemetra) {
        this.consultaSincroDatiAppForm.controls['datiAggiuntivi'].patchValue(true);
        this.consultaSincroDatiAppForm.controls['datiAggiuntivi'].disable();
        this.consultaSincroService.setDettagliAggiuntivi(this.consultaSincroDatiAppForm.controls['datiAggiuntivi'].value);
        this.consultaSincroDatiAppForm.controls['filtroImportati'].patchValue({ codice: enum_Filtro_Importati.Non_Importati, descrizione: this.translocoService.translate("NonImportati") });
      }
    })).subscribe();
  }

  applyFilters() {
    let form = this.consultaSincroDatiAppForm.controls;
    form['validita'].patchValue({
      inizio: form.validita.value.inizio == null ? AGRODATAINIZIO : form.validita.value.inizio,
      fine: form.validita.value.fine == null ? AGRODATAFINE : form.validita.value.fine
    });
    this.consultaSincroService.setFiltroRicerca({
      tipiDato: form.tipo.value,
      pivaCUAA: form.pivaCUAA.value,
      dataFiltro_inizio: form.validita.value.inizio,
      dataFiltro_fine: form.validita.value.fine,
      datiAggiuntivi: form.datiAggiuntivi.value,
      filtroImportati: form.filtroImportati.value.codice
    });
    this.consultaSincroService.setDettagliAggiuntivi(form.datiAggiuntivi.value);
    this.gridPublicService.refresh(true);
  }

  /*private setPrimaryKey() {
      this.sincroDatiForm.value.primaryKey = {codice: 0,
          impresaPK: { codice: 0,
              partitaIva: this.objParametriAgenda.Piva
          }
      };
  }*/

  ngOnDestroy(): void {
    if (this.formCaricaDati != undefined) {
      this.formCaricaDati.unsubscribe();
    }
    if (this.valueChangesSub != undefined) {
      this.valueChangesSub.unsubscribe();
    }

    this.signal$.next();
    this.signal$.complete();
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

          this.inFrame = true;
        }
      });
  }

  /*private handleRedirect(campo: Campo): void{
      if (this.inFrame) {
          let objPostMessage: PostMessageStrutturata<Campo> = new PostMessageStrutturata<Campo>(
              messaggioPostMessage.chiudiWindowGiasNG,
              contestoPostMessage.SalvaCampo,
              campo
          );
          window.parent.postMessage(objPostMessage);
          // console.log('postMessage effettuato',objPostMessage);
      } else {
          this.location.back();
      }
  }*/

  async openDdl(ddlEl: GiasDropDownTemplateSComponent) {
    switch (ddlEl.giasFormControlName) {
      case 'tipo':
        if (this.isDemetra) {
          ddlEl.listItems = [
            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Import_Analisi, descrizione: this.translocoService.translate("Demetra_Import_Analisi") },
            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Export_Analisi, descrizione: this.translocoService.translate("Demetra_Export_Analisi") },

            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Import_Attivita, descrizione: this.translocoService.translate("Demetra_Import_Attivita") },
            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Export_Attivita, descrizione: this.translocoService.translate("Demetra_Export_Attivita") },

            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Import_Fabbricati, descrizione: this.translocoService.translate("Demetra_Import_Fabbricati") },
            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Export_Fabbricati, descrizione: this.translocoService.translate("Demetra_Export_Fabbricati") },

            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Import_LavoratoriQDC, descrizione: this.translocoService.translate("Demetra_Import_LavoratoriQDC") },
            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Export_LavoratoriQDC, descrizione: this.translocoService.translate("Demetra_Export_LavoratoriQDC") },

            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Import_Fornitori, descrizione: this.translocoService.translate("Demetra_Import_Fornitori") },
            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Export_Fornitori, descrizione: this.translocoService.translate("Demetra_Export_Fornitori") },

            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Import_Macchine, descrizione: this.translocoService.translate("Demetra_Import_Macchine") },
            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Export_Macchine, descrizione: this.translocoService.translate("Demetra_Export_Macchine") },

            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Import_MovimentiMag, descrizione: this.translocoService.translate("Demetra_Import_MovimentiMag") },
            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Export_MovimentiMag, descrizione: this.translocoService.translate("Demetra_Export_MovimentiMag") },

            { codice: enum_Esportazioni_Sistema_Cod.Demetra_Import_DataPublish, descrizione: this.translocoService.translate("Demetra_Import_DataPublish") }

          ].sort((a, b) => a.descrizione.localeCompare(b.descrizione),);
        } else {
          ddlEl.listItems = [{ codice: enum_Dati_App.Acquisti, descrizione: this.translocoService.translate("Acquisti") },
          { codice: enum_Dati_App.OperazioniDiCampagna, descrizione: this.translocoService.translate("OperazioniDiCampagna") },
          { codice: enum_Dati_App.OreMacchineCdG, descrizione: this.translocoService.translate("OreMacchineCdG") },
          { codice: enum_Dati_App.DocumentiFileFotoFilmatiAudio, descrizione: this.translocoService.translate("DocumentiFileFotoFilmatiAudio") },
          { codice: enum_Dati_App.ManutenzioniMacchine, descrizione: this.translocoService.translate("ManutenzioniMacchine") },
          { codice: enum_Dati_App.MovimentiDiMagazzino, descrizione: this.translocoService.translate("MovimentiDiMagazzino") },
          { codice: enum_Dati_App.Ricette, descrizione: this.translocoService.translate("Ricette") },
          { codice: enum_Dati_App.RilieviEVisiteConRilievi, descrizione: this.translocoService.translate("Rilievi") },
          { codice: enum_Dati_App.Visite, descrizione: this.translocoService.translate("Visite") },
          { codice: enum_Dati_App.Imprese, descrizione: this.translocoService.translate("Imprese") },
          { codice: enum_Dati_App.Appezzamenti, descrizione: this.translocoService.translate("Appezzamenti") },
          { codice: enum_Dati_App.AttivitaZoo, descrizione: this.translocoService.translate("AttivitaZoo") },
          { codice: enum_Dati_App.PianoCampionamento, descrizione: this.translocoService.translate("PianoCampionamento") },
          { codice: enum_Dati_App.AttivitaDemetra, descrizione: this.translocoService.translate("AttivitaDemetra") },
          { codice: enum_Dati_App.RicetteDemetra, descrizione: this.translocoService.translate("RicetteDemetra") }].sort((a, b) => a.descrizione.localeCompare(b.descrizione),);
        }
        break;
      case 'filtroImportati':
        ddlEl.listItems = [{ codice: enum_Filtro_Importati.Importati, descrizione: this.translocoService.translate("Importati") },
        { codice: enum_Filtro_Importati.Non_Importati, descrizione: this.translocoService.translate("NonImportati") },
        { codice: enum_Filtro_Importati.Tutti, descrizione: this.translocoService.translate("Tutti") }];
        break;
    }
  }

  async rowsLoaded() {
    this.gridPublicService.value.data.columns.find(t => t.field == 'Descrizione').hidden = !this.consultaSincroService.getDettagliAggiuntivi()
  }
}
