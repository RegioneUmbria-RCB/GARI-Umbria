import {Component, OnInit} from "@angular/core";
import { FormGroup, FormGroupDirective } from "@angular/forms";
import { Lavorazione } from "app/Model/attivita/Lavorazione";
import {QdCService} from "../../../../service/qdc.service";
import {Subscription} from "rxjs";
import {QdCSementiService} from "../../../../service/prodotti/sementi.service";

@Component({
    standalone: false,
    selector: "app-sementi",
    templateUrl: "./sementi.component.html",
    styleUrls: ["./sementi.component.scss"],
    providers:[QdCSementiService]
})
export class SementiComponent implements OnInit {
    SementiForm: FormGroup;

    Operazione: Lavorazione;

    Subs: Subscription = new Subscription();

    constructor(public parent: FormGroupDirective,
                public qdcsementiservice: QdCSementiService,
                public qdcservice: QdCService
    ) {  }

    ngOnInit(): void {

        this.SementiForm = <FormGroup>this.parent.form;

        this.Operazione = this.SementiForm.get("Operazione").value;

        //Triggero il validator del formgroup
        this.SementiForm.markAllAsTouched();
    }

}
