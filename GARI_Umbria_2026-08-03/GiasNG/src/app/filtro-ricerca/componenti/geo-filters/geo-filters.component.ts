import { Component, Input, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { RemoveTagEvent } from "@progress/kendo-angular-dropdowns";
import { GroupResult, groupBy } from "@progress/kendo-data-query";
import { LeggiComuni_IN, LeggiProvince_IN, LeggiRegioni_IN, MetaschemaClient } from "app/Service/net-core6-api.service";
import { GiiasMultiselectTemplateSComponent } from 'gias-ui-kit';

@Component({
    standalone: false,
    selector: 'app-geo-filters',
    templateUrl: './geo-filters.component.html',
    styleUrls: ['./geo-filters.component.scss']
})
export class GeoFiltersComponent {

    @Input() formGroupInput: FormGroup;

    ListaStatiSelezionati = new Array();
    ListaRegioniSelezionate = new Array();
    ListaProvinceSelezionate = new Array();

    constructor(
                private formFieldsService: MetaschemaClient
                ) {}


    openMultiSelectStati(multiEl: GiiasMultiselectTemplateSComponent) {

        multiEl.loading = true;

        if (!multiEl.listItems)
            this.formFieldsService.metaschemaGetStati("").subscribe(r => {
                multiEl.listItems = JSON.parse(r.RispostaStringa);
                multiEl.loading = false;
            });
        else
            multiEl.loading = false;

    }

    onRemoveItemStati(removedItem) {

        let arrRegioni = (this.formGroupInput.get('Regioni')) ? this.formGroupInput.get('Regioni').value : new Array();

        if (arrRegioni) {
            this.formGroupInput.get('Regioni').patchValue(arrRegioni.filter(item => item.Stato_Country !== removedItem.dataItem.Codice));

            let regioniToRemove = arrRegioni.filter(item => item.Stato_Country == removedItem.dataItem.Codice);
            regioniToRemove.forEach(item => {
                let param = new RemoveTagEvent(item);
                this.onRemoveItemRegioni(param);
            });
        }
    }

    onRemoveItemRegioni(removedItem) {

        let arrProvince = (this.formGroupInput.get('Province')) ? this.formGroupInput.get('Province').value : new Array();

        if (arrProvince) {
            this.formGroupInput.get('Province').patchValue(arrProvince.filter(item => item.Regione_Des !== removedItem.dataItem.Regione_Des));

            let provinceToRemove = arrProvince.filter(item => item.Regione_Des == removedItem.dataItem.Regione_Des);

            provinceToRemove.forEach(item => {
                let param = new RemoveTagEvent(item);
                this.onRemoveItemProvince(param);
            });
        }

    }

    onRemoveItemProvince(removedItem) {

        let arrComuni = (this.formGroupInput.get('Comuni')) ? this.formGroupInput.get('Comuni').value : new Array();

        if (arrComuni) {
            this.formGroupInput.get('Comuni').patchValue(arrComuni.filter(item => item.PROV !== removedItem.dataItem.PROV));
        }

    }

    onRemoveAll(values, multiToClear: string[]) {
        if (values.length == 0) {
            multiToClear.forEach(item => {
                this.formGroupInput.get(item).patchValue([]);
            });
        }
    }

    openMultiSelectRegioni(multiEl: GiiasMultiselectTemplateSComponent) {

        let arrStati = (this.formGroupInput.get('Stati').value) ? this.formGroupInput.get('Stati').value.map(item => item['Codice']) : new Array();

        if (arrStati.length == 0) {
            multiEl.listItems = arrStati;
        } else {
            
            if (JSON.stringify(arrStati) !== JSON.stringify(this.ListaStatiSelezionati)) {

                multiEl.loading = true;

                let params = {
                    Stati_List: arrStati,
                    Regione: ""
                } as LeggiRegioni_IN;

                this.formFieldsService.metaschemaGetRegioni(params).subscribe(r => {
                    let listaRegioni = JSON.parse(r.RispostaStringa);
                    let groupedData: GroupResult[] = groupBy(listaRegioni, [
                        { field: "Stato_Country" },
                    ]);
                    multiEl.listItems = groupedData;
                    multiEl.listItemsNoFiltered = multiEl.listItems;
                    multiEl.loading = false;
                });

            }
        }

        this.ListaStatiSelezionati = arrStati;

    }

    openMultiSelectProvince(multiEl: GiiasMultiselectTemplateSComponent) {

        let arrRegioni = (this.formGroupInput.get('Regioni').value) ? this.formGroupInput.get('Regioni').value.map(item => item['REG']) : new Array();

        if (arrRegioni.length == 0) {
            multiEl.listItems = arrRegioni;
        } else {
            
            if (JSON.stringify(arrRegioni) !== JSON.stringify(this.ListaRegioniSelezionate)) {

                multiEl.loading = true;

                let params = {
                    Stato_Country: "",
                    Reg_List: arrRegioni
                } as LeggiProvince_IN;

                this.formFieldsService.metaschemaGetProvince(params).subscribe(r => {
                    let listaProvince = JSON.parse(r.RispostaStringa);
                    let groupedData: GroupResult[] = groupBy(listaProvince, [
                        { field: "Regione_Des" },
                    ]);
                    multiEl.listItems = groupedData;
                    multiEl.listItemsNoFiltered = multiEl.listItems;
                    multiEl.loading = false;
                });

            }
        }

        this.ListaRegioniSelezionate = arrRegioni;

    }

    openMultiSelectComuni(multiEl: GiiasMultiselectTemplateSComponent) {

        let arrProvince = (this.formGroupInput.get('Province').value) ? this.formGroupInput.get('Province').value : new Array();

        if (arrProvince.length == 0) {
            multiEl.listItems = arrProvince;
        } else {
            
            if (JSON.stringify(arrProvince) !== JSON.stringify(this.ListaProvinceSelezionate)) {

                multiEl.loading = true;

                let params = {
                    PROV_List: arrProvince.map(item => item['PROV']),
                    COM_LOCALITA: "",
                    COM_PROVINCIA: "",
                    SIGLA_PROVINCIA: "",
                    CAP: "",
                    REG: "",
                    Filtro_StrProvincia: "",
                    Filtro_StrComune: ""
                } as LeggiComuni_IN;

                this.formFieldsService.metaschemaGetComuni(params).subscribe(r => {
                    let listaComuni = JSON.parse(r.RispostaStringa);
                    listaComuni = listaComuni.map(item => { return {...item, CHIAVE: item.COM_PROVINCIA + "_" + item.COM_LOCALITA} });
                    let groupedData: GroupResult[] = groupBy(listaComuni, [
                        { field: "PROVINCIA" },
                    ]);
                    multiEl.listItems = groupedData;
                    multiEl.listItemsNoFiltered = multiEl.listItems;
                    multiEl.loading = false;
                });

            }
        }

        this.ListaProvinceSelezionate = arrProvince;
    }

}
