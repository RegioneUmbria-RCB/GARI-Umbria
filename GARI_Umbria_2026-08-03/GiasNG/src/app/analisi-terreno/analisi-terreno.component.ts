import { Component, OnDestroy, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { AnalisiTerrenoService } from "./griglia-analisi-terreno/service/analisi-terreno.service";
import { faFlask } from "@fortawesome/free-solid-svg-icons";
import { Subject, debounceTime, takeUntil } from "rxjs";

@Component({
    standalone: false,
    selector: 'app-analisi-terreno',
    templateUrl: './analisi-terreno.component.html',
    styleUrls: ['./analisi-terreno.component.scss'],
    providers: []
})
export class AnalisiTerrenoComponent implements OnInit, OnDestroy {

    public filtroForm: FormGroup = this.fb.group({
        Piva: '',
        Data: [AGRODATAINIZIO]
    });

    public signal$: Subject<void> = new Subject();
    faFlask = faFlask;

    constructor(private fb: FormBuilder,
                private analisiService: AnalisiTerrenoService
    ) {
        this.setOggi();
    }

    ngOnDestroy(): void {
        this.signal$.next();
        this.signal$.complete();
    }

    public AGRODATA_INIZIO = AGRODATAINIZIO;

    setOggi() {
        this.filtroForm.get("Data").patchValue(new Date());
    }

    ngOnInit(): void {
        this.filtroForm.valueChanges.pipe(
            debounceTime(1000),
            takeUntil(this.signal$)
        ).subscribe((val) => {
            this.analisiService.filterData.next(val);
        });
    }

}
