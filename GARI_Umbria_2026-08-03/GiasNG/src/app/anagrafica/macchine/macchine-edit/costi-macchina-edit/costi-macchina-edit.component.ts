import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { Subscription } from 'rxjs';
import { CostiMacchinaDataService } from './costi-macchina-data.service';
import { CostoUnitarioChiave } from 'app/Model/anagrafiche/CostoUnitario';
import { CostiMacchinaParentFormService } from './costi-macchina-parent-form.service';
import { CostiMacchinaEditConfigService } from './costi-macchina-edit-grid-config.service';
import { UnitaDiMisura } from 'app/Model/metaschema/UnitaDiMisura';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
    standalone: false,
    selector: 'app-macchina-edit-costi',
    templateUrl: './costi-macchina-edit.component.html',
    providers:[
        ...generateGridProviders(CostiMacchinaEditConfigService, CostiMacchinaEditComponent)
    ]
})

export class CostiMacchinaEditComponent implements OnInit,OnDestroy {
    @Input('costi') CostiFormArray: FormArray = this.fb.array([]);



    objParametriAgenda: ObjParametriAgenda;
    costiDataSub: Subscription;

    constructor(
        private fb: FormBuilder,
        public intl: IntlService,
        private costiMacchinaDataService: CostiMacchinaDataService,
        private costiMacchinaParentFormService: CostiMacchinaParentFormService,
        private objParametriAgendaService: ObjParametriAgendaService)
    {  }

    async ngOnInit() {
        let costi = new Array<CostoUnitarioChiave>();

        let id = 0;
        this.CostiFormArray.value.forEach((c) => {
            let cost = new CostoUnitarioChiave();
            cost.codice = c.codice;
            cost.chiave = id;
            cost.flag_cancellazione = c.flag_cancellazione;
            cost.prezzo = c.prezzo;
            cost.unitaDiMisura = new UnitaDiMisura(c.unitaDiMisura.codice, c.unitaDiMisura.descrizione);
            cost.validita = new IntervalloTemporale(c.validita.inizio, c.validita.fine);
            costi.push(cost);
            id++;
        });

        this.costiMacchinaDataService.setCostiMacchina(costi);
        this.costiMacchinaParentFormService.setCostiMacchinaParentForm(<FormGroup>this.CostiFormArray.parent);

        this.costiDataSub = this.costiMacchinaDataService.costiMacchinaSource
        .subscribe(i => {
            this.setCostiFormArray(i);
        });
    }

    ngOnDestroy() {
        if (this.costiDataSub)
            this.costiDataSub.unsubscribe();
    }

    private setCostiFormArray(c: CostoUnitarioChiave[]) {
        this.CostiFormArray.controls.splice(0);
        c.forEach(cost => {
            this.CostiFormArray.controls.push(this.fb.group(cost));
        })
    }
}
