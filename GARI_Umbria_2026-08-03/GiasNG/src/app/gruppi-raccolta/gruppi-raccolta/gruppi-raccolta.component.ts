import { Component } from '@angular/core';
import {generateGridProviders} from 'gias-kendo-grid';
import {GruppiRaccoltaGridConfigurationService} from './gruppi-raccolta-grid-configuration.service';
import {ObjParametriAgendaService} from '../../Service/obj-parametri-agenda.service';
import {GruppiRaccoltaGridEventsService} from './gruppi-raccolta-grid-events.service';

@Component({
    standalone: false,
    selector: 'app-gruppi-raccolta',
    templateUrl: './gruppi-raccolta.component.html',
    styleUrls: ['./gruppi-raccolta.component.css'],
    providers: [
        ...generateGridProviders(GruppiRaccoltaGridConfigurationService, GruppiRaccoltaComponent), GruppiRaccoltaGridEventsService
    ]
})
export class GruppiRaccoltaComponent {

    constructor(
        private objParametriAgendaService: ObjParametriAgendaService
    ) { }

    public impresaSelezionata(): boolean {
        return this.objParametriAgendaService.getObjParamValue().Piva != null && this.objParametriAgendaService.getObjParamValue().Piva !== '';
    }
}
