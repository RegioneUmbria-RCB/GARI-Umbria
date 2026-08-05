import { Component, Input, OnInit, Type } from "@angular/core";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { CustomComponent } from 'gias-kendo-grid';

@Component({
    standalone: false,
    selector: 'macchina-short-description',
    templateUrl: './macchina-short-description.component.html',
    styleUrls: ['./macchina-short-description.component.css'],
    providers: []
})
export class MacchinaShortDescriptionComponent implements OnInit, CustomComponent {
    @Input() input: any;
    @Input() edit: boolean;
    @Input() field: string;
    public imagePath: SafeResourceUrl;

    tipo: string;
    marca: number;
    macchina: string;

    show: boolean
    constructor(private _sanitizer: DomSanitizer) {
    }

    ngOnInit(): void {
        this.tipo = this.input.tipologia;
        this.marca = this.input.Ditta_Des;
        let macchina_des = "";
        this.macchina = this.input.Modello
        if (this.input.Macchina != '') {
            macchina_des += ' - ' + this.input.Macchina;
        }
        if (this.input.Targa != '') {
            macchina_des += ' (' + this.input.Targa + ')';
        }
        this.macchina = macchina_des;
    }

}
