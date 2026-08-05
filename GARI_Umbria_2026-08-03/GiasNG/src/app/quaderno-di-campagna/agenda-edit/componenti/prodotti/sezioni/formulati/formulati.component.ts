import {AfterViewInit, Component, OnDestroy, OnInit} from "@angular/core";
import {AbstractControl, FormArray, FormGroup, FormGroupDirective} from "@angular/forms";
import { Lavorazione } from "app/Model/attivita/Lavorazione";
import {enum_Stato_Innesco, QdCService} from "app/quaderno-di-campagna/agenda-edit/service/qdc.service";
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { UtilityFunctions } from "app/Utility/UtilityFunctions";
import { Subscription } from "rxjs";
import { QdCFormulatiService } from "../../../../service/prodotti/formulati.service";
import {
  Dettaglio_Formulato, GridImpiantoSelezionatoModel,
  MultiColumnComboboxTrattamento
} from "../../../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity} from "../../../../../../Service/master.service";
import {Dialog_Type} from "../../../../../../Service/gias-dialog.service";
import {KendoGridRow} from "gias-kendo-grid";
import {Attivita_Con_Parametri_Aggiuntivi} from "../../../../../../Service/Agenda/Agenda.service";
import {enum_LAVCOD} from "../../../../../../Model/TipiEnumerativi";
import {QdCControlliSalvataggioService} from "../../../../service/qdc-controlli-salvataggio.service";

@Component({
    standalone: false,
    selector: "app-formulati",
    templateUrl: "./formulati.component.html",
    styleUrls: ["./formulati.component.scss"],
    providers: [GiasDropDownTemplateService, QdCFormulatiService]
})
export class FormulatiComponent implements OnInit, OnDestroy, AfterViewInit {

    FormulatiForm: FormGroup;
    Operazione: Lavorazione;
    Subs: Subscription = new Subscription();

    constructor(public parent: FormGroupDirective,
        public qdcservice: QdCService,
        public qdcformulatiservice: QdCFormulatiService,
        private qdccontrollisalvataggioservice: QdCControlliSalvataggioService
    ) { }

    async ngOnInit() {

        this.FormulatiForm = <FormGroup>this.parent.form;

        this.Operazione = <Lavorazione>(
            this.FormulatiForm.get("Operazione").value
        );

        //Triggero il validator del formgroup
        this.FormulatiForm.markAllAsTouched();

        //Aggiungo un elemento all'Array delle epocheDPI per poter visualizzare la dropdown
        if (this.FormulatiForm.get("EpocaDPI").value) {

            this.qdcformulatiservice.Array_EpocheDPI.push(this.FormulatiForm.get("EpocaDPI").value);

        } else {

            this.FormulatiForm.patchValue({
                EpocaDPI: null,
            }, { emitEvent: false });

            await this.qdcformulatiservice.getArray_EpocheDPI(this.FormulatiForm);
        }

        this.Subs.add(
            this.qdcservice.TestataForm.get("Disciplinare").valueChanges.subscribe(async d => {

                await this.qdcformulatiservice.getArray_EpocheDPI(this.FormulatiForm);

            })
        );

        //Aggiorno il valore di EpocaDPI in tutti i DosiProdotti
        this.Subs.add(this.FormulatiForm.get("EpocaDPI").valueChanges.subscribe(value => {

            let DosiProdotti = this.qdcservice.DosiProdottiFormArray(this.FormulatiForm, null);

            if (DosiProdotti && DosiProdotti.controls.length > 0) {
                for (let i = 0; i < DosiProdotti.controls.length; i++) {
                    DosiProdotti.controls[i].patchValue({
                        EpocaDPI: value
                    });
                }
            }
        }));

        //Aggiorno il valore di Ripartizione_Trappole in tutti i DosiProdotti
        this.Subs.add(this.FormulatiForm.get("Ripartizione_Trappole")?.valueChanges?.subscribe(value=>{

          let DosiProdotti = this.qdcservice.DosiProdottiFormArray(this.FormulatiForm,null);

          if(DosiProdotti && DosiProdotti.controls.length > 0){
            for(let i = 0; i< DosiProdotti.controls.length;i++){
              DosiProdotti.controls[i].patchValue({
                Ripartizione_Trappole: value
              });
            }
          }
        }));

        this.qdcformulatiservice.AbilitaDisabilitaEpocaDPI(this.FormulatiForm);

        await this.qdcformulatiservice.Carica_Controlli_Ribaltamento_in_Agenda_Di_Ricetta_da_APP_Formulati(this.FormulatiForm);
    }


    ngAfterViewInit() {
      this.Mostra_Msg_Trappole_da_Reinnescare();
    }

  ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    //Carico la ddl solo quando scatta l'evento di open
    async openControlFormulati(
        ddlEl: GiasDropDownTemplateSComponent,
        formName: string
    ) {
        let fn: any;

        switch (formName) {
            case "EpocaDPI":
                fn = await this.qdcformulatiservice.getArray_EpocheDPI(this.FormulatiForm);
                UtilityFunctions.loadDropDownItems(ddlEl, fn);
                break;
        }
    }


    public ClassDettagliFormulati(Dettagli_Formulati: FormGroup): string{

        let Riga_Salvata: boolean = Dettagli_Formulati.get('Riga_Salvata').getRawValue();

        let Formulato: MultiColumnComboboxTrattamento = Dettagli_Formulati.get('Prodotto').getRawValue();

        let classCss: string = "";

        if(Riga_Salvata){

          classCss = "salvata";

          if(this.qdcservice.flag_Reinnesco){
            let statoInnesco: enum_Stato_Innesco = this.qdcformulatiservice.Controlla_Stato_Innesco(Formulato);

            switch(statoInnesco){
              case enum_Stato_Innesco.Scaduto:
                classCss = "innesco-scaduto";
                break;
              case enum_Stato_Innesco.In_Scadenza:
                classCss = "innesco-in-scadenza";
                break;
            }
          }

        }else{
          classCss = "non-salvata";
        }

        return classCss;
    }

    /*
    * @description
    * Indico a video i Prodotti che sono da Reinnescare perchè sono Scaduti o in Scadenza
    * */
    private Mostra_Msg_Trappole_da_Reinnescare(){

      if(this.qdcservice.flag_Reinnesco) {

        let str_elenco: string = "<br> - ";

        let msg: string = this.qdccontrollisalvataggioservice.GetMsgImpiantiNonValidiReinnesco();

        let Trappole_Con_Inneschi_Scaduti: Array<string> = [];

        let Trappole_Con_Inneschi_In_Scadenza: Array<string> = [];

        let DosiProdotti: Array<Dettaglio_Formulato> = this.qdcservice.DosiProdottiFormArray(this.FormulatiForm, null).getRawValue();

        if (DosiProdotti && DosiProdotti.length > 0) {

          DosiProdotti.forEach(d => {
            let statoInnesco = this.qdcformulatiservice.Controlla_Stato_Innesco(d.Prodotto);

            switch (statoInnesco) {
              case enum_Stato_Innesco.Scaduto:
                if (!Trappole_Con_Inneschi_Scaduti.includes(d.Prodotto.Descrizione_Concatenata))
                  Trappole_Con_Inneschi_Scaduti.push(d.Prodotto.Descrizione_Concatenata);
                break;
              case enum_Stato_Innesco.In_Scadenza:
                if (!Trappole_Con_Inneschi_In_Scadenza.includes(d.Prodotto.Descrizione_Concatenata))
                  Trappole_Con_Inneschi_In_Scadenza.push(d.Prodotto.Descrizione_Concatenata);
                break;
            }

          });
        }

        if (Trappole_Con_Inneschi_Scaduti && Trappole_Con_Inneschi_Scaduti.length > 0) {

          if(msg !== "")
            msg += "<br>";

          let str: string = str_elenco + Trappole_Con_Inneschi_Scaduti.join(str_elenco);

          msg += this.qdcservice.translocoService.translate("ProdottiConInneschiScaduti",{Prodotti: str});
        }

        if (Trappole_Con_Inneschi_In_Scadenza && Trappole_Con_Inneschi_In_Scadenza.length > 0) {

          if(msg !== "")
            msg += "<br>";

          let str: string = str_elenco + Trappole_Con_Inneschi_In_Scadenza.join(str_elenco);

          msg += this.qdcservice.translocoService.translate("ProdottiConInneschiInScadenza",{Prodotti: str});
        }

        if (msg !== "") {
          let listerroriGias: ErroreGias[] = [];

          listerroriGias.push(<ErroreGias>{
            severity: ErroreGias_Severity.WarningBloccante,
            tipo: enum_ErroreGias_Tipo.Generico,
            messaggio: msg
          });

          this.qdcservice.gestisci_ErroriGias(listerroriGias, false, false, "", Dialog_Type.info).then();

        }
      }
    }
}
