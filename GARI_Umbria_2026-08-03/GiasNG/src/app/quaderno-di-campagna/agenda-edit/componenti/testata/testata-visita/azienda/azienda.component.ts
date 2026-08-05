import { Component, OnDestroy, OnInit } from "@angular/core";
import { GISGeometrySelectionService } from "app/GIS/services/gis-geometry-selection.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { GiasDropDownTemplateSComponent, ObjParametriAgenda } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { QdCService } from "app/quaderno-di-campagna/agenda-edit/service/qdc.service";
import { AziendaDDLService } from "app/quaderno-di-campagna/agenda-edit/service/testata/azienda-ddl-service";
import { QdCTestataService } from "app/quaderno-di-campagna/agenda-edit/service/testata/testata.service";
import { Subscription, skip, tap } from "rxjs";

@Component({
    standalone: false,
    selector: 'app-azienda',
    templateUrl: './azienda.component.html',
    styleUrls: ['./azienda.component.css'],
    providers: [GiasDropDownTemplateService]
})
export class AziendaComponent implements OnInit, OnDestroy {

    // public array_Aziende: Impresa[] = [];

    private ddlSub: Subscription = new Subscription();

    objParametriAgenda: ObjParametriAgenda;

    constructor(public qdcservice: QdCService,
        public objParametriAgendaService: ObjParametriAgendaService,
        public aziendaDDLService: AziendaDDLService,
        public testataService: QdCTestataService,
        private gisGeometrySelectionService: GISGeometrySelectionService,
        private ddlService: GiasDropDownTemplateService) { }


    async openDdlAzienda(ddlEl: GiasDropDownTemplateSComponent, formName: string) {

        let fn: any;

        switch (formName) {
            case 'Azienda':
                ddlEl.listItems = this.testataService.Array_Aziende;
                break;
        }
    }

    ngOnInit() {

        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

        this.ddlSub.add(this.ddlService.currentDropDownValueObject
            .pipe(
                skip(1),
                tap(() => this.gisGeometrySelectionService.resetFromOutside())
            )
            .subscribe(async ddlElem => {
                switch (ddlElem.FormControlName) {
                    case 'Azienda_Visita':
                        await this.testataService.getArray_CentriAziendale();
                        await this.testataService.getArray_UtilizziTerreno();
                        await this.testataService.CambioCentroAziendale();
                        this.qdcservice.RicaricaGridImpianti();
                        this.qdcservice.RicaricaProdottiDaMagazzino();
                        this.qdcservice.TrovaPoligoniGIS();
                        break;
                }
            }));
    }

    ngOnDestroy(): void {
        this.ddlSub.unsubscribe();
    }

}
