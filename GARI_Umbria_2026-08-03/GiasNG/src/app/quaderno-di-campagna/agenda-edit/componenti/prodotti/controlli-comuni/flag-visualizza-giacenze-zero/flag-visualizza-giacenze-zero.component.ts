import { Component, OnInit } from "@angular/core";
import { FormGroup, FormGroupDirective } from "@angular/forms";
import { QdCProdottiService } from "app/quaderno-di-campagna/agenda-edit/service/prodotti.service";

@Component({
    standalone: false,
    selector: "app-flag-visualizza-giacenze-zero",
    templateUrl: "./flag-visualizza-giacenze-zero.component.html",
    styleUrls: ["./flag-visualizza-giacenze-zero.component.css"],
})
export class FlagVisualizzaGiacenzeZeroComponent implements OnInit {
    ProdottiForm: FormGroup;

    constructor(
        public parent: FormGroupDirective,
        public prodottiservice: QdCProdottiService
    ) {}

    ngOnInit(): void {
        this.ProdottiForm = <FormGroup>this.parent.form;
    }
}
