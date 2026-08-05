import { Component, OnDestroy, OnInit } from '@angular/core';
import { GisService } from 'app/GIS/GIS.service';
import { GISGeometrySelectionService } from 'app/GIS/services/gis-geometry-selection.service';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { combineLatest, Subscription } from 'rxjs';
import { debounceTime, skip, startWith, tap } from 'rxjs/operators';
import { QdCService } from '../../service/qdc.service';
import { QdCTestataService } from '../../service/testata/testata.service';
import { QdCVisibilitaControlliTestataService } from '../../service/testata/visibilita-controlli-testata-service';
import {Lavorazione} from "../../../../Model/attivita/Lavorazione";
import {Attivita} from "../../../../Model/attivita/Attivita";
import { Tipo } from 'app/Model/attivita/centri_di_costo/CentroDiCosto';

@Component({
    standalone: false,
    selector: 'app-testata',
    templateUrl: './testata.component.html',
    styleUrls: ['./testata.component.css'],
    providers: [GiasDropDownTemplateService]
})
export class TestataComponent implements OnInit, OnDestroy {

    ddlSub: Subscription;
    formSub: Subscription;

    constructor(
        public testataservice: QdCTestataService,
        private ddlService: GiasDropDownTemplateService,
        public qdcservice: QdCService,
        public visibilitacontrollitestataservice: QdCVisibilitaControlliTestataService,
        private gisGeometrySelectionService: GISGeometrySelectionService,
        private gisService: GisService) {
    }


    ngOnInit(): void {
        this.gisGeometrySelectionService.resetFromOutside();

        this.ddlSub = this.ddlService.currentDropDownValueObject
            .pipe(
                skip(1),
                tap(() => this.gisGeometrySelectionService.resetFromOutside())
            )
            .subscribe(async ddlElem => {
                switch (ddlElem.FormControlName) {
                    case 'Centro_Aziendale':
                        await this.testataservice.CambioCentroAziendale();
                        this.qdcservice.RicaricaGridImpianti();
                        this.qdcservice.RicaricaProdottiDaMagazzino();
                        this.qdcservice.TrovaPoligoniGIS();
                        break;
                    case 'Campo':
                        this.qdcservice.RicaricaGridImpianti();
                        this.qdcservice.RicaricaProdottiDaMagazzino();
                        this.qdcservice.TrovaPoligoniGIS();
                        break;
                    case 'Attivita_Personalizzata':
                        break;
                }


            });

        this.formSub = combineLatest([
            this.qdcservice.TestataForm.controls.Data.valueChanges.pipe(startWith(null)),
            this.qdcservice.TestataForm.controls.Centro_Aziendale.valueChanges.pipe(startWith(null)),
            this.qdcservice.TestataForm.controls.Campo.valueChanges.pipe(startWith(null)),
            this.qdcservice.TestataForm.controls.Specie.valueChanges.pipe(startWith(null)),
        ])
            .pipe(debounceTime(200))
            .subscribe(() => {
                this.updateGeometrySelection();
            });

        //Carico i centri aziendali senza impostarli nella ddl perchè mi servono per abilitare o meno il GIS
        this.testataservice.getArray_CentriAziendale(false).then();


        //Disabilito la ddl della Attivita Personalizzate se ho aperto una visita con un attivita collegata (per esempio Rilievo)
        if(this.qdcservice.TestataForm.get("flagVisita").getRawValue() &&
            this.qdcservice.TestataForm.get("Attivita_Collegate").getRawValue() &&
            this.qdcservice.TestataForm.get("Attivita_Collegate").getRawValue().findIndex((a: Attivita)=> a.job
              && this.qdcservice.Elenco_Operazioni_Compatibili_Con_Visite.includes(+ a.job.primaryKey.codice)) > -1){

            this.qdcservice.TestataForm.get("Attivita_Personalizzata").disable({emitEvent: false});

        }


    }

    mostraCampo(): boolean {
        if (this.qdcservice.getCentroDiCostoTipo() == Tipo.ProdottoDaTrattare) {
            return false;
        }

        return true;
    }

    private updateGeometrySelection(): void {
        const formValue = this.qdcservice.TestataForm.value;

        const date = formValue.Data ?? new Date();
        const sa_cod = formValue.Centro_Aziendale?.primaryKey?.codice ?? "";
        const campo_cod = formValue.Campo?.primaryKey?.codice ?? "";
        const veg_cod = this.qdcservice.getUtilizzoTerrenoModel();
        if (formValue.flagVisita)
            this.gisService.updateGeoJsonFilterServiceQdc(date, `${sa_cod}`, `${campo_cod}`, veg_cod);
        else
            this.gisService.seUpdateGeoJsonFilterServiceQdc(date, `${sa_cod}`, `${campo_cod}`, veg_cod);
    }

    //Carico la ddl solo quando scatta l'evento di open
    async openDdlTestataForm(ddlEl: GiasDropDownTemplateSComponent, formName: string) {

        //Per le ddl che sono di 'testata' le carico quando scatta l'evento di open
        //solo se listItems è null

        let fn: any;

        switch (formName) {
            case 'Centro_Aziendale':
                fn = async () => { return await this.testataservice.getArray_CentriAziendale() };
                UtilityFunctions.loadDropDownMultiSelectItemsOnlyWhenUndefined(ddlEl, fn);
                break;
            case 'Campo':
                fn = async () => { return await this.testataservice.getArray_Campi() };
                UtilityFunctions.loadDropDownMultiSelectItemsOnlyWhenUndefined(ddlEl, fn);
                break;
            case 'Attivita_Personalizzata':
                fn = async () => { return await this.testataservice.getArray_AttivitaPersonalizzate() };
                UtilityFunctions.loadDropDownMultiSelectItemsOnlyWhenUndefined(ddlEl, fn);
                break;
        }
    }

    MostraddlAttivitaeDescrizione() {

        if (this.qdcservice.TestataForm.get("flagVisita").value)
            return true;

        let mostra = false;

        let Operazioni = this.qdcservice.TestataForm.get("Operazioni").value;

        let index = -1;

        if(Operazioni && Operazioni.length > 0){
            index = Operazioni.findIndex(o => {
                if (this.visibilitacontrollitestataservice.MostraddlAttivitaeDescrizione(o))
                    return o;
            });
        }


        if (index > -1)
            mostra = true;

        return mostra;

    }

    ngOnDestroy(): void {
        this.ddlSub.unsubscribe();
        this.formSub?.unsubscribe();
    }
}
