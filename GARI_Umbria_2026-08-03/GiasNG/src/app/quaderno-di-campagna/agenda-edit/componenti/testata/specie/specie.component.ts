import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { QdCService } from "../../../service/qdc.service";
import { QdCTestataService } from "../../../service/testata/testata.service";
import { skip, tap } from "rxjs/operators";
import { Subscription, of } from "rxjs";
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { GISGeometrySelectionService } from "../../../../../GIS/services/gis-geometry-selection.service";
import { GisService } from "../../../../../GIS/GIS.service";
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { UtilityFunctions } from "../../../../../Utility/UtilityFunctions";
import { LeggiSpecie } from 'app/Service/Metaschema/specie-vegetali.service';

@Component({
    standalone: false,
    selector: 'app-specie',
    templateUrl: './specie.component.html',
    styleUrls: ['./specie.component.css'],
    providers: [GiasDropDownTemplateService]
})
export class SpecieComponent implements OnInit, OnDestroy  {
    private ddlSub: Subscription = new Subscription();

  @ViewChild('ddl') ddl;

  constructor(public qdcservice: QdCService,
              public testataservice: QdCTestataService,
              private gisGeometrySelectionService: GISGeometrySelectionService,
              private ddlService: GiasDropDownTemplateService) { }

    ngOnInit(): void {

        this.gisGeometrySelectionService.resetFromOutside();

        this.ddlSub.add(this.ddlService.currentDropDownValueObject
            .pipe(
                skip(1),
                tap(() => this.gisGeometrySelectionService.resetFromOutside())
            )
            .subscribe(async ddlElem => {
                switch (ddlElem.FormControlName) {
                    case 'Specie':
                        await this.testataservice.CambioSpecie();
                        this.qdcservice.RicaricaGridImpianti();
                        this.qdcservice.RicaricaProdottiDaMagazzino();
                        this.qdcservice.TrovaPoligoniGIS();
                        break;
                }


            }));
    }

    async changeDominioSpecie(tutteLeSpecie: boolean) {

        await this.testataservice.getArray_UtilizziTerreno();

        if (!tutteLeSpecie && this.qdcservice.TestataForm.get("Specie").value.codice === 0) {
            await this.testataservice.CambioSpecie();
            this.qdcservice.RicaricaGridImpianti();
            this.qdcservice.RicaricaProdottiDaMagazzino();
            this.qdcservice.TrovaPoligoniGIS();
        }
    }

    async openDdlSpecie(ddlEl: GiasDropDownTemplateSComponent, formName: string) {

        //Per le ddl che sono di 'testata' le carico quando scatta l'evento di open
        //solo se listItems è null

        let fn: any;

        switch (formName) {
            case 'Specie':
                fn = async () => {
                    return await this.testataservice.getArray_UtilizziTerreno()
                };
                UtilityFunctions.loadDropDownMultiSelectItemsOnlyWhenUndefined(ddlEl, fn);
                break;
        }
    }

    ngOnDestroy() {
        this.ddlSub.unsubscribe();
    }

}
