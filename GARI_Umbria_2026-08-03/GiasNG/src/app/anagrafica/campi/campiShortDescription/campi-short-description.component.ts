import { Component, Input, OnInit, Type } from "@angular/core";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { CustomComponent } from 'gias-kendo-grid';

@Component({
    standalone: false,
    selector: 'campi-short-description',
    templateUrl: './campi-short-description.component.html',
    styleUrls: ['./campi-short-description.component.css'],
    providers: []
})
export class CampiShortDescriptionComponent implements OnInit, CustomComponent {
    @Input() input: any;
    @Input() edit: boolean;
    @Input() field: string;
    public imagePath: SafeResourceUrl;

    show: boolean
    constructor(private _sanitizer: DomSanitizer) {
    }

    campo_des: string;
    validita: string;

    ngOnInit(): void {
        this.campo_des = this.input.Campo;
        if (this.input.rif_alfanumerico != '') {
            this.campo_des += ' (' + this.input.rif_alfanumerico + ')';
        }

        let validita_inizio = ''
        if (this.input.Validita_Inizio != AGRODATAINIZIO) {
            validita_inizio = (this.input.Validita_Inizio as Date).toLocaleDateString();
        }

        let validita_fine = ''
        if (this.input.Validita_Fine != AGRODATAFINE) {
            validita_fine = (this.input.Validita_Fine as Date).toLocaleDateString();
        }

        this.validita = validita_inizio + ' - ' + validita_fine;
    }

}
