import { AfterViewInit, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormGroupDirective } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { QdCProdottiService } from 'app/quaderno-di-campagna/agenda-edit/service/prodotti.service';
import { QdCService } from 'app/quaderno-di-campagna/agenda-edit/service/qdc.service';
import { MasterService } from 'app/Service/master.service';
import { MultiColumnComboboxService } from 'gias-ui-kit';
import { from, skip } from 'rxjs';
import { Subscription } from 'rxjs/internal/Subscription';
import { GiasMultiColumnComboboxTemplateComponent } from 'gias-ui-kit';
import { UtilityFunctions } from '../../../../../../../Utility/UtilityFunctions';
import { QdCDettagliFormulatiService } from '../../../../../service/prodotti/dettagli-formulati.service';
import { LAVCOD_DISTRIBUZIONE_INSETTI } from '../../../../../../../Model/CostantiPersonalizzate';
import { enum_Problema_DettaglioProdotto } from '../../../../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';

@Component({
    standalone: false,
    selector: 'app-ricerca-formulati',
    templateUrl: './ricerca-formulati.component.html',
    styleUrls: ['./ricerca-formulati.component.scss'],
    providers: [MultiColumnComboboxService]
})
export class RicercaFormulatiComponent implements OnInit, OnDestroy, AfterViewInit {

    @Input() showComandi = false;
    Subs: Subscription = new Subscription();

    public Categoria_Magazzino: number;

    public Lav_Cod: number;

    public Operazione: Lavorazione;

    ProdottiForm: FormGroup;

    public ServerFiltering = true;

    @ViewChild('MultiColumnCombobox') MultiColumnCombobox: GiasMultiColumnComboboxTemplateComponent;

    constructor(
        public parent: FormGroupDirective,
        public prodottiservice: QdCProdottiService,
        public qdcservice: QdCService,
        private multicolumncomboboxservice: MultiColumnComboboxService,
        private translocoService: TranslocoService,
        private masterService: MasterService,
        public qdcdettagliformulatiservice: QdCDettagliFormulatiService
    ) { }

    ngOnInit(): void {
        this.ProdottiForm = <FormGroup>this.parent.form;
        this.Operazione = this.ProdottiForm.get('Operazione').value;
        this.Categoria_Magazzino = this.ProdottiForm.get('Categoria_Magazzino').value;
        this.Lav_Cod = this.ProdottiForm.get('Operazione').value.primaryKey.codice;

        this.Subs.add(this.multicolumncomboboxservice.currentMultiColumnComboboxValueObject.pipe(skip(1)).subscribe(async ddlElem => {
            switch (ddlElem.FormControlName) {
                case 'Prodotto':
                    switch (+this.Lav_Cod) {
                        case LAVCOD_DISTRIBUZIONE_INSETTI:
                            await this.qdcdettagliformulatiservice.changeInsetto(ddlElem.Value);
                            break;
                        default:
                            await this.qdcdettagliformulatiservice.changeFormulato(ddlElem.Value);
                            break;
                    }

            }
        }));

        this.ServerFiltering = this.prodottiservice.Abilita3CaratteriServerFiltering();
        this.qdcdettagliformulatiservice.setColumnComboboxFormulati();
    }

    ngAfterViewInit() {
        this.prodottiservice.MultiColumnComboboxProdotti = this.MultiColumnCombobox;

        if (this.ProdottiForm.get('Problema_DettaglioProdotto_Da_Risolvere').getRawValue() === enum_Problema_DettaglioProdotto.Formulato_Ambiguo) {
            this.qdcdettagliformulatiservice.RicercaFormulati(false, false);
        }
    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    loadFunctionddlServerFilteringProdotti(filter: string) {
        return from(this.qdcdettagliformulatiservice.getArray_Formulati(filter));
    }

    async openddl(ddlEl: GiasMultiColumnComboboxTemplateComponent) {

        if (!this.ServerFiltering && !this.qdcdettagliformulatiservice.Prima_Apertura_MultiColumn_Trattamento) {
            UtilityFunctions.loadDropDownItems(ddlEl, this.qdcdettagliformulatiservice.getArray_Formulati(''));
            this.qdcdettagliformulatiservice.Prima_Apertura_MultiColumn_Trattamento = true;
        }

    }

    MostraRiga_NoteProdotto() {

        let mostra = false;

        //Da mostrare solamente se la riga non è salvata
        if (!this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi() && this.ProdottiForm.get('Riga_Salvata').value)
            return mostra;

        if (this.qdcdettagliformulatiservice.Riga_NoteProdotto !== '' && this.ProdottiForm.get('Prodotto').value) {
            mostra = true;
        }

        return mostra;
    }

    public Numero_Formulati() {

        let number = this.prodottiservice?.MultiColumnComboboxProdotti?.GetNumberOfRows();

        let descrizione = "(" + this.translocoService.translate('Trovati') + ": " + number + ")"

        return descrizione;
    }

    public filterChangeFormulati(filter: string) {
        let a = 0; //Commento per funzione vuota SonarQube
    }

}
