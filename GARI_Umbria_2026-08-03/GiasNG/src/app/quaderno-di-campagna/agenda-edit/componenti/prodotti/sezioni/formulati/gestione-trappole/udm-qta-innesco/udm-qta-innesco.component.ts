import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormGroup, FormGroupDirective} from "@angular/forms";
import {QdCService} from "../../../../../../service/qdc.service";
import {skip, Subscription} from "rxjs";
import {UtilityFunctions} from "../../../../../../../../Utility/UtilityFunctions";
import {UnitaDiMisura} from "../../../../../../../../Model/metaschema/UnitaDiMisura";
import {GiasDropDownTemplateSComponent, GiasDropDownTemplateService} from "gias-ui-kit";
import { QdCDettagliFormulatiService } from 'app/quaderno-di-campagna/agenda-edit/service/prodotti/dettagli-formulati.service';
import {QdCProdottiService} from "../../../../../../service/prodotti.service";

@Component({
  standalone: false,
  selector: 'app-udm-qta-innesco',
  templateUrl: './udm-qta-innesco.component.html',
  styleUrl: './udm-qta-innesco.component.css',
  providers: [GiasDropDownTemplateService]
})
export class UdmQtaInnescoComponent implements OnInit, OnDestroy {

  Subs: Subscription = new Subscription();
  ProdottiForm: FormGroup;

  constructor(public parent: FormGroupDirective,
              private ddlService: GiasDropDownTemplateService,
              public qdcservice: QdCService,
              public qdcdettagliformulatiService: QdCDettagliFormulatiService,
              public prodottiservice: QdCProdottiService) {
  }

  ngOnInit() {
    this.ProdottiForm = <FormGroup>this.parent.form;

    this.Subs.add(
      this.ddlService.currentDropDownValueObject
        .pipe(skip(1))
        .subscribe(async (ddlElem) => {
          switch (ddlElem.FormControlName) {
            case "UdM_Innesco":
              this.qdcdettagliformulatiService.changeUdM_Innesco(ddlElem.Value);
              break;
          }
        })
    );
  }

  //Carico la ddl solo quando scatta l'evento di open
  async openControlUdM(ddlEl: GiasDropDownTemplateSComponent, formName: string): Promise<void> {
    let fn: any;

    switch (formName) {
      case "UdM_Innesco":
        fn = await this.qdcdettagliformulatiService.getArray_UdM_Innesco(this.ProdottiForm.getRawValue());
        UtilityFunctions.loadDropDownItems(ddlEl,fn);
        if((fn instanceof Array) && (<Array<UnitaDiMisura>>fn).length == 1) {
          this.qdcdettagliformulatiService.Aggiorna_UdM_Innesco((<Array<UnitaDiMisura>>fn)[0]);
        }
        break;
    }
  }

  ngOnDestroy() {
    this.Subs.unsubscribe();
  }

}
