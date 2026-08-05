import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormGroupDirective } from '@angular/forms';
import { QdCProdottiService } from 'app/quaderno-di-campagna/agenda-edit/service/prodotti.service';
import { QdCDettagliFertilizzantiService } from 'app/quaderno-di-campagna/agenda-edit/service/prodotti/dettagli-fertilizzanti.service';
import { QdCService } from 'app/quaderno-di-campagna/agenda-edit/service/qdc.service';
import { MultiColumnComboboxService } from 'gias-ui-kit';
import { TranslocoService } from '@jsverse/transloco';
import { from, skip, Subscription } from 'rxjs';
import { GiasMultiColumnComboboxTemplateComponent } from 'gias-ui-kit';
import { UtilityFunctions } from '../../../../../../../Utility/UtilityFunctions';
import { QdCFertilizzantiService } from '../../../../../service/prodotti/fertilizzanti.service';
import { enum_Problema_DettaglioProdotto } from 'app/quaderno-di-campagna/agenda-edit/quaderno-di-campagna-form/quaderno-di-campagna-form.model';

@Component({
    standalone: false,
    selector: 'app-ricerca-fertilizzanti',
    templateUrl: './ricerca-fertilizzanti.component.html',
    styleUrls: ['./ricerca-fertilizzanti.component.scss'],
    providers: [MultiColumnComboboxService]
})
export class RicercaFertilizzantiComponent implements OnInit, OnDestroy, AfterViewInit {

    Subs: Subscription = new Subscription();

    public Categoria_Magazzino: number;

    public Lav_Cod: number;

    ProdottiForm: FormGroup;

    public ServerFiltering = true;

    @ViewChild('MultiColumnCombobox') MultiColumnCombobox: GiasMultiColumnComboboxTemplateComponent;

    constructor(public parent: FormGroupDirective,
        public prodottiservice: QdCProdottiService,
        public qdcservice: QdCService,
        private multicolumncomboboxservice: MultiColumnComboboxService,
        public qdcdettaglifertilizzantiservice: QdCDettagliFertilizzantiService,
        private translocoService: TranslocoService,
        public qdcfertilizzantiservice: QdCFertilizzantiService) { }

    ngOnInit(): void {

        this.ProdottiForm = <FormGroup>this.parent.form;

        this.Categoria_Magazzino = this.ProdottiForm.get('Categoria_Magazzino').value;

        this.Lav_Cod = this.ProdottiForm.get('Operazione').value.primaryKey.codice;

        this.Subs.add(this.multicolumncomboboxservice.currentMultiColumnComboboxValueObject.pipe(skip(1)).subscribe(async ddlElem => {
            switch (ddlElem.FormControlName) {
                case this.translocoService.translate('Prodotto'):
                    await this.qdcdettaglifertilizzantiservice.changeFertilizzante(ddlElem.Value);
                    break;
            }
        }));

        this.qdcdettaglifertilizzantiservice.setColumnComboboxFertilizzanti();

        this.ServerFiltering = this.prodottiservice.Abilita3CaratteriServerFiltering();

    }

    ngAfterViewInit() {
        this.prodottiservice.MultiColumnComboboxProdotti = this.MultiColumnCombobox;

        if(this.ProdottiForm.get("Problema_DettaglioProdotto_Da_Risolvere").getRawValue() === enum_Problema_DettaglioProdotto.Fertilizzante_Ambiguo){
          this.qdcdettaglifertilizzantiservice.RicercaFertilizzanti(false,false);
        }
    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    loadFunctionddlServerFilteringProdotti(filter: string) {
        return from(this.qdcdettaglifertilizzantiservice.getArray_Fertilizzanti(filter));
    }

    async openddl(ddlEl: GiasMultiColumnComboboxTemplateComponent) {

        if (!this.ServerFiltering && !this.qdcdettaglifertilizzantiservice.Prima_Apertura_MultiColumn_Fertilizzante) {
            UtilityFunctions.loadDropDownItems(ddlEl, this.qdcdettaglifertilizzantiservice.getArray_Fertilizzanti(''));
            this.qdcdettaglifertilizzantiservice.Prima_Apertura_MultiColumn_Fertilizzante = true;
        }

    }

    public Numero_Fertilizzanti() {

      let number = this.prodottiservice?.MultiColumnComboboxProdotti?.GetNumberOfRows();

      let descrizione = "("+this.translocoService.translate('Trovati') + ": "+ number +")"

      return descrizione;
    }

    public filterChangeFertilizzanti(filter: string) {
        let a = 0; //Commento per funzione vuota SonarQube
    }


}
