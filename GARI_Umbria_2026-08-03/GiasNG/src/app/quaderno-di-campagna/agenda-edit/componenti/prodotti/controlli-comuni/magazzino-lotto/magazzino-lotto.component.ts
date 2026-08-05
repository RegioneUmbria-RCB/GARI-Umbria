import { Component, Input, OnInit } from "@angular/core";
import { FormGroup, FormGroupDirective } from "@angular/forms";
import { QdCProdottiService } from "app/quaderno-di-campagna/agenda-edit/service/prodotti.service";
import { GiasDropDownTemplateSComponent, Tipo_Attivita } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { UtilityFunctions } from "app/Utility/UtilityFunctions";
import { enum_LAVCOD, enum_PagineGiasNG } from "../../../../../../Model/TipiEnumerativi";
import { ObjParametriAgenda } from 'gias-ui-kit';
import { Attivita } from "../../../../../../Model/attivita/Attivita";
import {
    GestioneRichiesteService,
    ParametriAggiuntivi_QueryString
} from "../../../../../../Service/gestione-richieste.service";
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from "../../../../../../Model/siti.enum";
import { Lavorazione } from "../../../../../../Model/attivita/Lavorazione";
import { QdCService } from "../../../../service/qdc.service";
import {
    QdCFormToAttivitaService
} from "../../../../service/quaderno-di-campagna-form/quaderno-di-campagna-form-to-attivita.service";
import { DropdownListMagazzino } from "../../../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import { enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity } from "../../../../../../Service/master.service";
import { TranslocoService } from "@jsverse/transloco";
import { cloneDeep } from "lodash";

@Component({
    standalone: false,
    selector: "app-magazzino-lotto",
    templateUrl: "./magazzino-lotto.component.html",
    styleUrls: ["./magazzino-lotto.component.scss"],
    providers: [GiasDropDownTemplateService]
})
export class MagazzinoLottoComponent implements OnInit {

    ProdottiForm: FormGroup;

    Operazione: Lavorazione;

    @Input() public MagazzinoFormControlName: string = "Magazzino_del_Prodotto_Selezionato";
    @Input() public LottoFormControlName: string = "Lotto";
    @Input() public flagInnesco: boolean = false;

    constructor(public parent: FormGroupDirective,
        public prodottiservice: QdCProdottiService,
        private qdcservice: QdCService,
        private qdCFormToAttivitaService: QdCFormToAttivitaService,
        private gestioneRichiesteService: GestioneRichiesteService,
        private translocoService: TranslocoService) { }

    ngOnInit(): void {
        this.ProdottiForm = <FormGroup>this.parent.form;

        this.Operazione = this.ProdottiForm.get("Operazione").value;
    }

    // Carico la ddl solo quando scatta l'evento di open
    async openControlMagazzino(
        ddlEl: GiasDropDownTemplateSComponent,
        formName: string
    ) {
        let fn: any;

        switch (formName) {
            case 'Magazzino_del_Prodotto_Selezionato':
            case 'Magazzino_Innesco':
                fn = await this.prodottiservice.getArray_Magazzini();
                UtilityFunctions.loadDropDownItems(ddlEl, fn);
                break;
        }
    }

    mostraBtnCaricoEffluenti(): boolean {

        let mostra = false;

        if (+ this.Operazione.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI &&
            this.prodottiservice.controlli_Giacenze_Lotti.ddl_Magazzino.visibile &&
            this.qdcservice.TestataForm.get("Codici_Attivita").getRawValue().find(c => c.Operazione.primaryKey.codice === this.Operazione.primaryKey.codice)?.pua?.codice > 0 && 
            this.qdcservice.TestataForm.get("Tipo").value === Tipo_Attivita.Ricetta)
            mostra = true;

        return mostra;

    }

    // Redirect to RilievoEffluenti
    redirectToRilievoEffluenti() {

        let magazzino: DropdownListMagazzino = this.ProdottiForm.get("Magazzino_del_Prodotto_Selezionato").value;

        if (!magazzino || magazzino?.Descrizione_Concatenata === "") {

            let listErroriGias = [];

            listErroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.Bloccante,
                tipo: enum_ErroreGias_Tipo.Generico,
                messaggio: this.translocoService.translate("SelezionareUnMagazzino")
            });

            this.qdcservice.gestisci_ErroriGias(listErroriGias, true, true).then();

            return;
        }

        let Operazioni = this.qdcservice.TestataForm.get('Operazioni').getRawValue();

        if (Operazioni && Operazioni.length > 0) {

            const newobjParametriAgenda: ObjParametriAgenda = cloneDeep(this.qdcservice.objParametriAgenda);

            newobjParametriAgenda.Pagina_Provenienza = this.qdcservice.masterService.getCurrentPageAsValue();

            newobjParametriAgenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;

            const attivita: Attivita = cloneDeep(this.qdCFormToAttivitaService.mapQdCFormToListaAttivita(this.ProdottiForm, [], true, []).find(a => +a.job.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI));

            newobjParametriAgenda.GenericObj_string = JSON.stringify(attivita);

            const parametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = [];

            this.gestioneRichiesteService.gestionePassaggioAltroSito_Aperto_in_Iframe(
                Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                enum_PagineAgenda_2010.Pagina_RilievoEffluenti,
                parametriAggiuntivi,
                newobjParametriAgenda,
                false,
                -1,
                'qdc.CaricoEffluenti.Text').then();
        }
    }

    getMagazzinoDescrizione(): string {
      let descrizione: string = "";

      if(this.flagInnesco){
        descrizione = this.translocoService.translate('MagazzinoInnesco');
      }else{
        descrizione = this.translocoService.translate('Magazzino');
      }

      return descrizione;
    }

    getLottoDescrizione(): string {
      let descrizione: string = "";

      if(this.flagInnesco){
        descrizione = this.translocoService.translate('LottoInnesco');
      }else{
        descrizione = this.translocoService.translate('Lotto2');
      }

      return descrizione;
    }

    public mostraDDLMagazzino(): boolean{
      if(this.flagInnesco){
        return this.prodottiservice.controlli_Giacenze_Lotti_Innesco.ddl_Magazzino.visibile;
      }else{
        return this.prodottiservice.controlli_Giacenze_Lotti.ddl_Magazzino.visibile;
      }
    }

   public mostraLotto(): boolean{
      if(this.flagInnesco){
        return this.prodottiservice.controlli_Giacenze_Lotti_Innesco.txt_Lotto.visibile;
      }else{
        return this.prodottiservice.controlli_Giacenze_Lotti.txt_Lotto.visibile;
      }
    }
}
