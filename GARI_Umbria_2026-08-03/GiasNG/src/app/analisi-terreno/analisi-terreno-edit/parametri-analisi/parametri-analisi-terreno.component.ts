import { Component, Inject, ViewEncapsulation, forwardRef } from "@angular/core";
import { generateGridProviders } from 'gias-kendo-grid';
import { GridParametriAnalisiTerrenoHttpService } from "./service/grid-parametri-analisi-terreno.service";
import { ControlContainer, FormGroupDirective, NG_VALUE_ACCESSOR } from "@angular/forms";
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';

@Component({
    standalone: false,
    selector: 'app-parametri-analisi-terreno',
    templateUrl: './parametri-analisi-terreno.component.html',
    styleUrls: ['./parametri-analisi-terreno.component.scss'],
    providers: [...generateGridProviders(GridParametriAnalisiTerrenoHttpService, ParametriAnalisiTerrenoComponent)],
    viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }],
    encapsulation: ViewEncapsulation.None
})
export class ParametriAnalisiTerrenoComponent { }
