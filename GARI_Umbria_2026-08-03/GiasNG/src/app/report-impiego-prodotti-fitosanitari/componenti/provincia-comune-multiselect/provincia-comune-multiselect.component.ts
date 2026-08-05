import { Component, Input, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { GroupResult, groupBy } from "@progress/kendo-data-query";
import { LeggiComuni_IN, LeggiProvince_IN, MetaschemaClient } from "app/Service/net-core6-api.service";
import { GiiasMultiselectTemplateSComponent } from 'gias-ui-kit';
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';


@Component({
  standalone: false,
  selector: 'app-provincia-comune-multiselect',
  templateUrl: './provincia-comune-multiselect.component.html',
  styleUrls: ['./provincia-comune-multiselect.component.css'],
  providers: [GiasMultiSelectTemplateService]
})
export class ProvinciaComuneMultiselectComponent implements OnInit {

  @Input() formGroupInput: FormGroup;

  ListaProvinceSelezionate = new Array();

  constructor(private formFieldsService: MetaschemaClient) { }

  ngOnInit(): void {
  }

  onRemoveItemProvince(removedItem) {

    let arrComuni = (this.formGroupInput.get('Comuni')) ? this.formGroupInput.get('Comuni').value : new Array();

    if (arrComuni) {
      this.formGroupInput.get('Comuni').patchValue(arrComuni.filter(item => item.PROV !== removedItem.dataItem.PROV));
    }

  }

  onRemoveAll(values, multiToClear: string[]) {
    if (values.length === 0) {
      multiToClear.forEach(item => {
        this.formGroupInput.get(item).patchValue([]);
      });
    }
  }

  openMultiSelectProvince(multiEl: GiiasMultiselectTemplateSComponent) {

    multiEl.loading = true;

    let params = {
      Stato_Country: "IT",
      Reg_List: []
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

  openMultiSelectComuni(multiEl: GiiasMultiselectTemplateSComponent) {

    let arrProvince = (this.formGroupInput.get('Province').value) ? this.formGroupInput.get('Province').value : new Array();

    if (arrProvince.length === 0) {
      multiEl.listItems = arrProvince;
    } else {

      if (arrProvince.toString() !== this.ListaProvinceSelezionate.toString()) {

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
          listaComuni = listaComuni.map(item => { return { ...item, CHIAVE: item.COM_PROVINCIA + "_" + item.COM_LOCALITA } });
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
