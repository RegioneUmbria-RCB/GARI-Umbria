import {AfterViewInit, Component, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormGroupDirective} from '@angular/forms';
import {TranslocoService} from '@jsverse/transloco';
import {Lavorazione} from 'app/Model/attivita/Lavorazione';
import {enum_Gestione_Giacenze, enum_LAVCOD} from 'app/Model/TipiEnumerativi';
import {enum_Stato_Innesco, QdCService} from 'app/quaderno-di-campagna/agenda-edit/service/qdc.service';
import {
  GiasDropDownTemplateSComponent,
  GiasDropDownTemplateService,
  GiasMultiColumnComboboxTemplateComponent,
  MultiColumnComboboxService
} from 'gias-ui-kit';
import {UtilityFunctions} from 'app/Utility/UtilityFunctions';
import {skip, Subscription} from 'rxjs';
import {QdCDettagliFormulatiService} from '../../../../service/prodotti/dettagli-formulati.service';
import {
  DropdownListAvversita,
  enum_Problema_DettaglioProdotto
} from '../../../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import {LAVCOD_DISTRIBUZIONE_INSETTI} from '../../../../../../Model/CostantiPersonalizzate';
import {QdCProdottiService} from "../../../../service/prodotti.service";
import {QdCFormulatiService} from "../../../../service/prodotti/formulati.service";

@Component({
    standalone: false,
    selector: 'app-avversita',
    templateUrl: './avversita.component.html',
    styleUrls: ['./avversita.component.css'],
    providers: [GiasDropDownTemplateService]
})
export class AvversitaComponent implements OnInit, OnDestroy, AfterViewInit {

    @Input() hideRileva = false;
    @Input() showComandi = false;

    @ViewChild('AvversitaMultiColumnCombobox') AvversitaMultiColumnCombobox: GiasMultiColumnComboboxTemplateComponent;

    @ViewChild("MultiColumnComboboxAvversitaInneschi") MultiColumnComboboxAvversitaInneschi: GiasMultiColumnComboboxTemplateComponent;

    FormulatiForm: FormGroup;
    Operazione: Lavorazione;
    Subs: Subscription = new Subscription();

    constructor(private ddlService: GiasDropDownTemplateService,
                public qdcservice: QdCService,
                private translocoService: TranslocoService,
                public parent: FormGroupDirective,
                public qdcdettagliformulatiservice: QdCDettagliFormulatiService,
                private multiColumnComboboxService: MultiColumnComboboxService,
                public qdcprodottiservice: QdCProdottiService,
                private qdcformulatiservice: QdCFormulatiService
    ) { }

    async ngOnInit() {

        this.FormulatiForm = <FormGroup>this.parent.form;
        this.Operazione = <Lavorazione>(this.FormulatiForm.get('Operazione').value);

        //Carico subito le Avversità se sono nel caso avversita->prodotto
        if (this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti) {
            let avversita: DropdownListAvversita = this.FormulatiForm.get('Avversita').value;

            //TODO Da verificare in modifica di un coadiuvante con filtro avversita - > prodotto
            if ((!avversita || avversita.codice <= 0) && avversita?.codice != -1) {
                this.FormulatiForm.patchValue({
                    Avversita: null,
                }, { emitEvent: false });

                await this.qdcdettagliformulatiservice.getArray_Avversita();
            }
        }

        this.Subs.add(
            this.ddlService.currentDropDownValueObject
                .pipe(skip(1))
                .subscribe(async (ddlElem) => {
                    switch (ddlElem.FormControlName) {
                        case 'Avversita':
                            switch (+this.Operazione.primaryKey.codice) {
                                case LAVCOD_DISTRIBUZIONE_INSETTI:
                                    await this.qdcdettagliformulatiservice.changeAvversitaInsetti(ddlElem.Value);
                                    break;
                                default:
                                    await this.qdcdettagliformulatiservice.changeAvversita(ddlElem.Value);
                                    break;
                            }
                            break;
                    }
                })
        );

        this.Subs.add(
          this.multiColumnComboboxService.currentMultiColumnComboboxValueObject
            .pipe(skip(1))
            .subscribe(async (ddlElem) => {
              switch (ddlElem.FormControlName) {
                case 'Avversita':
                  this.qdcdettagliformulatiservice.changeAvversita(ddlElem.Value);
                  break;
              }
            })
        );

        this.Subs.add(this.FormulatiForm.get("Innesco_Incluso").valueChanges.subscribe((v)=>{
          this.qdcdettagliformulatiservice.changeInnesco_Incluso();
        }));

        this.Subs.add(this.FormulatiForm.get("Visualizza_Solo_Inneschi_in_Giacenza").valueChanges.subscribe((v)=>{
          this.qdcdettagliformulatiservice.changeVisualizza_Solo_Inneschi_in_Giacenza();
        }));

        this.qdcprodottiservice.Gestisci_Controlli_Lotti_Giacenze_Innesco();

        this.qdcdettagliformulatiservice.setColumnComboboxAvversitaInneschi();

    }

    ngAfterViewInit() {

        this.qdcdettagliformulatiservice.MultiColumnComboboxAvversitaInneschi = this.MultiColumnComboboxAvversitaInneschi;

        if (this.FormulatiForm.get('Problema_DettaglioProdotto_Da_Risolvere').getRawValue() === enum_Problema_DettaglioProdotto.Avversita_Non_Corretta ||
            this.FormulatiForm.get('Problema_DettaglioProdotto_Da_Risolvere').getRawValue() === enum_Problema_DettaglioProdotto.Avversita_Ambigua) {

            this.qdcdettagliformulatiservice.getArray_Avversita(false).then();
        }
    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    getDescrizioneAvversita(): string {
        let descrizione: string = "";

        switch(+this.Operazione.primaryKey.codice){
          case enum_LAVCOD.DISERBO:
            descrizione = this.translocoService.translate('Infestanti') + '/' + this.translocoService.translate('GruppiInfestanti');
            break;
          default:
            if(this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(+this.Operazione.primaryKey.codice)){
              descrizione = this.translocoService.translate('AvversitaGruppiAvversitaInnesco');
            }else{
              descrizione = this.translocoService.translate('qdc.lbl_avversitaResource1.Text');
            }
            break;
        }

        descrizione += " *";

        return descrizione;
    }

    //Carico la ddl solo quando scatta l'evento di open
    async openControlAvversita(ddlEl: GiasDropDownTemplateSComponent) {
        let fn: any = await this.qdcdettagliformulatiservice.getArray_Avversita();
        UtilityFunctions.loadDropDownItems(ddlEl, fn);
    }

    mostraAvversita() {
        let mostra: boolean = false;

        if (this.qdcdettagliformulatiservice.MostraNascondiDDLSezioneFormulati('Avversita') &&
            this.qdcdettagliformulatiservice.Array_Avversita &&
            this.qdcdettagliformulatiservice.Array_Avversita.length > 0) {
            mostra = true;
        }

        return mostra;
    }

    mostraAvversitaDDL(): boolean{
      let mostra: boolean = false;
      let lav_cod:number = +this.Operazione.primaryKey.codice;

      if (this.mostraAvversita() && !this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(lav_cod)) {
        mostra = true;
      }

      return mostra;
    }

    /*
    * @description Mostra la combobox multi colonna per la selezione dell'avversità solo se l'operazione è "Installazione trappole catture di massa" anche se non ci sono
    * avversità caricate
    * */
    mostraAvversitaMultiColumnCombobox(): boolean{
      let mostra: boolean = false;
      let lav_cod:number = +this.Operazione.primaryKey.codice;

      if (this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(lav_cod)) {
        mostra = true;
      }

      return mostra;
    }

    public ClassInnesco(): string{
      let cssClass = "";
      let lav_cod:number = +this.Operazione.primaryKey.codice;

      if (this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(lav_cod)){

        if(this.FormulatiForm.get("Riga_Salvata").getRawValue()){
          cssClass = "border-innesco";
        }else{
          cssClass = "border-innesco-non-salvato";
        }

        let statoInnesco: enum_Stato_Innesco = this.qdcformulatiservice.Controlla_Stato_Innesco(this.FormulatiForm.get("Prodotto").getRawValue());

        switch(statoInnesco){
          case enum_Stato_Innesco.Scaduto:
            cssClass = "innesco-scaduto";
            break;
          case enum_Stato_Innesco.In_Scadenza:
            cssClass = "innesco-in-scadenza";
            break;
        }
      }


      return cssClass;
    }

    public ClassHeader(){
      let cssClass = "gias-section-title-with-icon avversita-title-container";
      let lav_cod:number = +this.Operazione.primaryKey.codice;

      if (this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(lav_cod))
        cssClass += " margin-innesco-header";

      return cssClass;
    }

    public ClassBody(){
      let cssClass = "row form-row";
      let lav_cod:number = +this.Operazione.primaryKey.codice;

      if (this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(lav_cod))
        cssClass += " margin-innesco";

      return cssClass;
    }

    public mostraSwitchInnesco_Incluso(): boolean{
      return this.qdcservice.GestioneMagazzino_Abilitata_Inneschi;
    }

    public setInfoLabelInneschi(){
      let descr: string = "";

      if(this.qdcservice.GestioneMagazzino_Abilitata_Inneschi){
        if(this.qdcservice.Utente_Cod_Blocca_se_Supera_Giacenze){
          descr = this.translocoService.translate("InBaseAlleImpostazioniSoloInneschiaMagazzino");
        }else{
          switch(this.qdcservice.GestioneGiacenze_Inneschi){
            case enum_Gestione_Giacenze.SoloMovimentati:
              descr = this.translocoService.translate("InBaseAlleImpostazioniSoloInneschiMovimentati");
              break;
            case enum_Gestione_Giacenze.SoloPresenti:
              descr = this.translocoService.translate("InBaseAlleImpostazioniSoloInneschiaMagazzino");
              break;
            case enum_Gestione_Giacenze.TuttiProdotti:
              descr = this.translocoService.translate("InBaseAlleImpostazioniImpiegareInneschiaMagazzieMaiMovimenati");
              break;
          }
        }
      }else{
        descr = this.translocoService.translate("InBaseAlleImpostazioniNonScaricareProdotti");
      }

      return descr;
    }

}
