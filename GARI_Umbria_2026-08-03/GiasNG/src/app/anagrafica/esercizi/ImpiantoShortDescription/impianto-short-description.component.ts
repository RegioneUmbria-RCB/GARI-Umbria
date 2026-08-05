import { Component, Input, OnInit, Type } from "@angular/core";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { CustomComponent } from 'gias-kendo-grid';

@Component({
    standalone: false,
    selector: 'impianto-short-description',
    templateUrl: './impianto-short-description.component.html',
    styleUrls: ['./impianto-short-description.component.css'],
    providers: []
})
export class ImpiantoShortDescriptionComponent implements OnInit, CustomComponent {
    @Input() input: any;
    @Input() edit: boolean;
    @Input() field: string;
    public imagePath: SafeResourceUrl;

    utilizzo: string;
    superficie: number;
    validita: string;

    show: boolean
    constructor(private _sanitizer: DomSanitizer) {
    }

    ngOnInit(): void {
        if (this.input.cul_cod == 0) {
            this.utilizzo = this.input.Destinazione_Uso_Des;
        } else {
            this.utilizzo = this.input.veg_des + ' - ' + this.input.cul_des;
        }

        this.superficie = this.input.sup_imp;

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
