import { Component, Input, Output } from '@angular/core';
import { EventEmitter } from 'events';
import { AGRODATAFINE, AGRODATAINIZIO, Codice } from '../utils/models';

@Component({
    standalone: false,
    selector: 'gias-codice-template',
    templateUrl: './gias-codice-template.component.html',
    styleUrls: ['./gias-codice-template.component.scss']
})
/** CodiceTemplate component*/
export class GiasCodiceTemplateComponent {
    @Input() codice: Codice;
    @Input() lista_codici: Array<any>;
    @Input() text_field: string;
    @Input() value_field: string;
    @Input() dateControl: boolean;
    @Input() formEnable: boolean;
    @Output() changeValue = new EventEmitter();

    AGRODATAINIZIO = AGRODATAINIZIO;
    AGRODATAFINE = AGRODATAFINE;
}
