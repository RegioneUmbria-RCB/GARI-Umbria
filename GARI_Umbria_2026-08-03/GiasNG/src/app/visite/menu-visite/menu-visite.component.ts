import { Component, EventEmitter, Inject, Input, OnDestroy, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { generateGridProviders, GiasMessageService } from 'gias-kendo-grid';
import { GridVisiteHttpService } from '../service/grid-visite-http.service';
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { BussinessMenuAgendaService } from 'app/menu-agenda/shared_services/bussiness-logic.service';
import { VisiteService } from 'app/Service/Visite/visite.service';
import { Subscription, skip } from 'rxjs';
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { Attivita } from 'app/Model/attivita/Attivita';
import { Risorsa } from 'app/Model/attivita/risorse/Risorsa';
import { RisorsaUmanaVisita } from 'app/Model/anagrafiche/RisorsaUmanaVisita';
import { AgendaService } from 'app/Service/Agenda/Agenda.service';
import { FormCopiaVisitaConfig, InitializeFormCopiaVisita } from '../utils';
import { DropdownListSpecieAnimali } from 'app/quaderno-di-campagna/agenda-edit/service/testata/specie-animali-service';
import { RisorsaSpecie } from 'app/Model/attivita/risorse/RisorsaSpecie';
import { RisorsaDestinazioneUso } from 'app/Model/attivita/risorse/RisorsaDestinazioneUso';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { RisorsaZootecnica } from 'app/Model/attivita/risorse/RisorsaZootecnica';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { CentriAziendaliService } from 'app/Service/Anagrafica/centri.service';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { TranslocoService } from '@jsverse/transloco';
import { GridPublicService } from 'gias-kendo-grid';
import { Operatore } from 'app/Model/metaschema/utilizzi/Operatore';
import { Varieta } from 'app/Model/metaschema/utilizzi/Varieta';
import { RispostaStandard } from 'app/Service/master.service';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { CaricaDatiApp } from 'app/Service/api.service';

@Component({
  standalone: false,
  selector: 'app-menu-visite',
  templateUrl: './menu-visite.component.html',
  styleUrls: ['./menu-visite.component.scss'],
  providers: [
              ...generateGridProviders(GridVisiteHttpService,
                                       MenuVisiteComponent),
              BussinessMenuAgendaService,
              GiasDropDownTemplateService,
              GiasMultiSelectTemplateService,
              ReactiveFormsModule
  ],
  encapsulation: ViewEncapsulation.None
})
export class MenuVisiteComponent implements OnInit, OnDestroy {

  visitaForm = CreateFormGroup(this.fb, InitializeFormCopiaVisita());

  showCopiaVisita: boolean = false;

  showGrid: boolean = true;

  private ddlSub: Subscription = new Subscription();

  private visitaDaCopiare: Attivita;

  private operatoreCopiaVisita: Operatore;

  private subCopiaVisita: Subscription;

  @Input() tabVisita;

  @Output() updateDisabled: EventEmitter<boolean> = new EventEmitter<boolean>();

  toggleTabStrip(param: boolean) {
    this.updateDisabled.emit(param); // Invia il nuovo valore allo stato del padre
  }

  //@ViewChild('tabVisita') tabVisita;

  constructor(@Inject(GRID_HTTP_TOKEN) private gridVisiteHttpService: GridVisiteHttpService,
                                       private gridpublicService: GridPublicService,
                                       private fb: FormBuilder,
                                       private giasMessageService: GiasMessageService,
                                       private visiteService: VisiteService,
                                       private agendaservice: AgendaService,
                                       private dialogService: GiasDialogService,
                                       private centriservice: CentriAziendaliService,
                                       private translocoService: TranslocoService,
                                       private ddlService: GiasDropDownTemplateService) {}

  get OperatoriCopiaVisita() { return this.visiteService.operatoriCopiaVisite }

  get SpecieCopiaVisita() { return this.visiteService.specieCopiaVisite }

  get AziendeCopiaVisita() { return this.visiteService.aziendeCopiaVisite }

  get SpecieAnimaleCopiaVisita() { return this.visiteService.specieAnimaliCopiaVisite }
  
  ngOnInit(): void {

    this.visiteService.caricaDDLSpecieAnimaleCopiaVisita();
    //carico le DDL per la copia
    this.visiteService.caricaDDLSpecieCopiaVisita();
    this.visiteService.caricaDDLOperatoriCopiaVisita();

    this.subCopiaVisita = this.visiteService.copiaVisitaSub.pipe(skip(1)).subscribe(res => {

      this.showCopiaVisita = true;

      this.toggleTabStrip(true);

      //mi salvo in una proprietà il valore dell'attività
      this.visitaDaCopiare = res;
      this.fillFormCopiaVisita(res);

      this.ddlSub.add(this.ddlService.currentDropDownValueObject
        .subscribe(async ddlElem => {
          switch (ddlElem.FormControlName) {
            case 'OperatoreCopiaVisita':
              this.clearDDLAgenzieAziendeCopiaVisita();
              this.visiteService.caricaDDLAgenzieAziendeCopiaVisita(this.operatoreCopiaVisita.username, ddlElem.Value.username, res.centroAziendale.primaryKey.partitaIva);
            break;
          }
        })
      );

    });

  }

  fillFormCopiaVisita(attivita: Attivita) {

    this.visitaForm.get('Data_Copia_Visita').patchValue(attivita.inizio);
    this.visitaForm.get('Ora_Inizio_Copia_Visita').patchValue(attivita.oraInizio);
    this.visitaForm.get('Ora_Fine_Copia_Visita').patchValue(attivita.oraFine);
    this.visitaForm.get('DescrizioneCopiaVisita').patchValue(attivita.descrizione);

    this.clearDDLSpecieCopiaVisita();
    this.clearDDLSpecieAnimaleCopiaVisita();
    this.clearDDLAgenzieAziendeCopiaVisita();

    let specieVisita = attivita.risorse.filter((risorsa: Risorsa)=>{ return (risorsa.classType === "RisorsaSpecie" || risorsa.classType === "RisorsaDestinazioneUso" || risorsa.classType === "RisorsaZootecnica") });

    if (specieVisita && specieVisita.length > 0) {
        specieVisita.forEach((risorsa: Risorsa)=>{

          switch (risorsa.classType) {
            case "RisorsaSpecie":
                const risSpecie = risorsa as RisorsaSpecie;
                this.visitaForm.get('SpecieCopiaVisita').patchValue({codice: risSpecie.specie.codice, descrizione: risSpecie.specie.descrizione});
            break;

            case "RisorsaDestinazioneUso":
                const risDestUso = risorsa as RisorsaDestinazioneUso;
                this.visitaForm.get('SpecieCopiaVisita').patchValue({codice: risDestUso.destinazioneUso.codice, descrizione: risDestUso.destinazioneUso.descrizione, classType:"DestinazioneUso"});
            break;

            case "RisorsaZootecnica":
                const defaultSpecieAnimale = this.visiteService.setDefaultDDLSpecieAnimale(risorsa as DropdownListSpecieAnimali);
                this.visitaForm.get('SpecieAnimaleCopiaVisita').patchValue(defaultSpecieAnimale);
            break;
          }

        });
    }

    //se non ho valorizzato i due campi precedentemente, poniamo gli altri di default
    if (!this.visitaForm.get('SpecieCopiaVisita').value) {
      if (attivita.utilizzoTerreno) {
        let varieta = attivita.utilizzoTerreno as Varieta;
        if (varieta.specie)
          this.visitaForm.get('SpecieCopiaVisita').patchValue(varieta.specie);
        else
          this.visitaForm.get('SpecieCopiaVisita').patchValue({codice: -1, descrizione: ""});
      } else {
        this.visitaForm.get('SpecieCopiaVisita').patchValue({codice: -1, descrizione: ""});
      }
    }

    if (!this.visitaForm.get('SpecieAnimaleCopiaVisita').value) {
        this.visitaForm.get('SpecieAnimaleCopiaVisita').patchValue(this.SpecieAnimaleCopiaVisita[0]);
    }

    //per default, metto lo stesso Operatore nella DDL
    let operatoreVisita = attivita.risorse.filter((risorsa: Risorsa)=>{ return risorsa.classType === "RisorsaAssegnatarioVisita" });

    if (operatoreVisita && operatoreVisita.length > 0) {
        operatoreVisita.forEach((risorsa: Risorsa)=>{
            let risorsaPersona = risorsa as RisorsaUmanaVisita;
            let OperatoreSelected = this.OperatoriCopiaVisita.find(item => item.Cod_RisUm === risorsaPersona.risorsaUmana.codice);
            this.operatoreCopiaVisita = OperatoreSelected;
            this.visitaForm.get('OperatoreCopiaVisita').patchValue(OperatoreSelected);
            //carico la DDL delle aziende
            this.visiteService.caricaDDLAgenzieAziendeCopiaVisita(OperatoreSelected.username, OperatoreSelected.username, this.visitaDaCopiare.centroAziendale.primaryKey.partitaIva);
        });
    }

  }

  redirectToNewOrEditVisita(editingMode: boolean) {
    this.gridVisiteHttpService.routingToNewOrEditVisita(editingMode);
  }

  hideNewVisitaButton() {
    return this.gridVisiteHttpService.getPermessoScritturaVisita();
  }

  showCaricaDatiAPPbutton() {
    return this.gridVisiteHttpService.getPermessoCaricaDatiAPP();
  }

  public async closeCopiaVisita(param: string) {

    if (param === 'create') {
      // occorre inizializzare correttamente il modello attivita

      let arrCopiaVisita = [];
      let arrPromiseScrittura = [];
      let arrAziendeScelte = this.visitaForm.get('AziendeCopiaVisita').value;

      if (!arrAziendeScelte || (arrAziendeScelte && arrAziendeScelte.length === 0)) {
        const errMsg = this.translocoService.translate("visite.ScegliereAlmenoUnAzienda");
        this.dialogService.baseError('', errMsg, false);
        return;
      }

      let dialogResponse = await this.visiteService.createDialogWindow('', this.translocoService.translate("visite.ConfermaCopiaVisitePerAziende"));

      if (dialogResponse['returnObj']) {

        this.visitaDaCopiare.inizio = this.visitaForm.get('Data_Copia_Visita').value;
        this.visitaDaCopiare.oraInizio = this.visitaForm.get('Ora_Inizio_Copia_Visita').value;
        this.visitaDaCopiare.oraFine = this.visitaForm.get('Ora_Fine_Copia_Visita').value;
        this.visitaDaCopiare.descrizione = this.visitaForm.get('DescrizioneCopiaVisita').value;

        this.visitaDaCopiare.codice = '';
        this.visitaDaCopiare.raccoglitore = 0;

        //sbiancare la posizione -- DA FARE
        this.visitaDaCopiare.latitude = 0;
        this.visitaDaCopiare.longitude = 0;

        //cancello l'array Risorse nel modello Attività per riempirlo con i nuovi elementi
        this.visitaDaCopiare.risorse.splice(0, this.visitaDaCopiare.risorse.length);

        //cancello gli impianti
        if (this.visitaDaCopiare.centriDiCosto)
          this.visitaDaCopiare.centriDiCosto.splice(0, this.visitaDaCopiare.centriDiCosto?.length);

        //cancello i rilievi
        if (this.visitaDaCopiare.attivitaCollegate)
          this.visitaDaCopiare.attivitaCollegate.splice(0, this.visitaDaCopiare.attivitaCollegate?.length);

        //assegno il nuovo operatore
        let risorsaUmanaVisita = new RisorsaUmanaVisita();
        let operatoreVisita = this.visitaForm.get("OperatoreCopiaVisita").value;
        risorsaUmanaVisita.risorsaUmana.codice = operatoreVisita.Cod_RisUm;

        this.visitaDaCopiare.risorse.push(risorsaUmanaVisita);

        //assegno la specie
        let risorsaSpecie;
        let risorsaSpecieScelta=this.visitaForm.get("SpecieCopiaVisita").value;

        if (risorsaSpecieScelta.hasOwnProperty("classType")) {
            risorsaSpecie = new RisorsaDestinazioneUso();
            risorsaSpecie.destinazioneUso.codice=this.visitaForm.get("SpecieCopiaVisita").value.codice;
            risorsaSpecie.destinazioneUso.descrizione=this.visitaForm.get("SpecieCopiaVisita").value.descrizione;
        } else {
            risorsaSpecie = new RisorsaSpecie();
            risorsaSpecie.specie = new Specie(this.visitaForm.get("SpecieCopiaVisita").value.codice);
            risorsaSpecie.specie.descrizione=this.visitaForm.get("SpecieCopiaVisita").value.descrizione;
        }

        this.visitaDaCopiare.risorse.push(risorsaSpecie);

        let risorsaZootecnica = new RisorsaZootecnica();
        risorsaZootecnica = this.visitaForm.get("SpecieAnimaleCopiaVisita").value;

        this.visitaDaCopiare.risorse.push(risorsaZootecnica);

        for (const item of arrAziendeScelte) {

          arrCopiaVisita = [];

          let selectedAzienda = new Impresa();

          selectedAzienda.partitaIva = item.partitaIva;
          selectedAzienda.ragioneSociale = item.ragioneSociale;

          let LeggiCentriAziendali;

          LeggiCentriAziendali = {
              impresa:  selectedAzienda,
              data: new Date(),
              utilizzoTerreno: risorsaSpecieScelta
          };

          let arrCentriAziendali = await this.centriservice.leggiCentriAziendaliModelloQdC(LeggiCentriAziendali,false);

          // Crea una copia separata di this.visitaDaCopiare
          let copiaVisita = JSON.parse(JSON.stringify(this.visitaDaCopiare));

          copiaVisita.centroAziendale= <CentroAziendale> arrCentriAziendali[0];

          arrCopiaVisita.push(copiaVisita);

          const ScriviListaAttivita = {
            attivita_list: arrCopiaVisita,
            parametri_aggiuntivi_list: []
          };

          let promiseScrittura = this.agendaservice.ScriviListaAttivita(ScriviListaAttivita);

          arrPromiseScrittura.push(promiseScrittura);

        }

        this.gridVisiteHttpService.startLoadGrid();

        Promise.all(arrPromiseScrittura).then(result => {
            let rispostaStandard = result as unknown as RispostaStandard[];
            const indexError = rispostaStandard.findIndex(val => val.RispostaOK === false);

            this.gridVisiteHttpService.stopLoadGrid();

            if(indexError !== -1) {
                this.dialogService.baseWarning('', rispostaStandard[indexError].Errore, false);
            } else {
              this.dialogService.baseSuccess('', this.translocoService.translate("visite.CopiaVisiteOK"), false, false);
              this.gridpublicService.refresh(true);
            }
        });

      }

    }

    this.showCopiaVisita = false;

    this.toggleTabStrip(false);
  }


  private clearDDLSpecieCopiaVisita() {
    this.visitaForm.get('SpecieCopiaVisita').reset();
  }


  private clearDDLSpecieAnimaleCopiaVisita() {
    this.visitaForm.get('SpecieAnimaleCopiaVisita').reset();
  }


  private clearDDLAgenzieAziendeCopiaVisita() {
    this.visitaForm.get('AziendeCopiaVisita').reset();
  }

  onClickCaricaDatiAPP() {
    // throw new Error('Method not implemented.');

    let param = {
      tipo: '30',
      piva: ""
    } as CaricaDatiApp;

    this.visiteService.caricaDatiApp(param).subscribe((res: any) => {
      // console.log(res);
      if (res.RispostaOK) {
        if (res.RispostaStringa !== '') {
          this.gridpublicService.refresh(true);
        } else {
          this.giasMessageService.infoMessagge('visite.NessunDatoDaCaricare', false, true);
        }
      } else {
        this.giasMessageService.errorMessage(res.Errore, false, true);
      }

    });
  }


  ngOnDestroy(): void {
    this.ddlSub.unsubscribe();
    this.subCopiaVisita.unsubscribe();
  }

}

function CreateFormGroup(fb: FormBuilder, mask: FormCopiaVisitaConfig) {
  return fb.group({
      Data_Copia_Visita: new FormControl(mask.Data_Copia_Visita),
      Ora_Inizio_Copia_Visita: new FormControl(mask.Ora_Inizio_Copia_Visita),
      Ora_Fine_Copia_Visita: new FormControl(mask.Ora_Fine_Copia_Visita),
      SpecieCopiaVisita: new FormControl(mask.SpecieCopiaVisita),
      SpecieAnimaleCopiaVisita: new FormControl(mask.SpecieAnimaleCopiaVisita),
      OperatoreCopiaVisita: new FormControl(mask.OperatoreCopiaVisita),
      AziendeCopiaVisita: new FormControl(mask.AziendeCopiaVisita),
      DescrizioneCopiaVisita: new FormControl(mask.DescrizioneCopiaVisita)
  });
}
