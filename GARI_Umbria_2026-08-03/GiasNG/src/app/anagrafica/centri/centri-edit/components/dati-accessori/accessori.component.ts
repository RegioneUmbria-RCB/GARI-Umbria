import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormGroupDirective } from '@angular/forms';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { ModuliAttivi } from 'app/Service/moduli-attivi.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { EditCentroStore } from '../../services/centri-store.service';
import { CodiciAnagraficiConfigService } from '../../services/grids/codici-anagrafici.service';


@Component({
  standalone: false,
  selector: 'dati-accessori',
  templateUrl: './accessori.component.html',
  styleUrls: ['./accessori.component.scss'],
  providers: [...generateGridProviders(CodiciAnagraficiConfigService, DatiAccessoriComponent)]
})
export class DatiAccessoriComponent implements OnInit {
    @Input() inEditMode: boolean;

    dataInizio: Date = AGRODATAINIZIO;
    dataFine: Date = AGRODATAFINE;

    validita: FormGroup;

    showCentroAziendaleEsternoCollegato: boolean = false;

    constructor(private rootFormGroup: FormGroupDirective,
        public store: EditCentroStore,
        private moduliAttivi: ModuliAttivi,
        private objParametriagendaService: ObjParametriAgendaService)
    {
        let moduli = this.moduliAttivi.getModuli(this.objParametriagendaService.getObjParamValue().Piva);
        if(moduli.Modulo_FreshFood || moduli.Modulo_Zoo) this.showCentroAziendaleEsternoCollegato = true;
    }

    ngOnInit(): void {
        this.validita = this.rootFormGroup.form.get('validita') as FormGroup;
    }

}
