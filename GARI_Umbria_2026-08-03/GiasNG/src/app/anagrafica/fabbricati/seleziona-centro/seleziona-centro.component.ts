import { Component, OnInit } from '@angular/core';
import {BaseCodeDescr} from "../../../Model/baseClass/baseCodeDescr";
import {Lavorazione} from "../../../Model/attivita/Lavorazione";
import {FormBuilder, FormControl, FormGroup} from "@angular/forms";
import {PermessiUtenteService} from "../../../Service/permessi-utente.service";
import {NuovaOperazioneService} from "../../esercizi/nuova-operazione/nuova-operazione.service";
import {from, map} from "rxjs";
import {CentroAziendale} from "../../../Model/anagrafiche/CentroAziendale";
import {CentriService} from "../../centri/services/centri.service";
import {CentriAziendaliService, LeggiCentriAziendali} from "../../../Service/Anagrafica/centri.service";
import {Impresa} from "../../../Model/anagrafiche/Impresa";
import {ObjParametriAgendaService} from "../../../Service/obj-parametri-agenda.service";
import {SelezionaCentroService} from "./seleziona-centro.service";

@Component({
    standalone: false,
    selector: 'app-seleziona-centro',
    templateUrl: './seleziona-centro.component.html'
})
export class SelezionaCentroComponent implements OnInit {
    public ListCentri: BaseCodeDescr[];
    public OperazionePredefinita: Lavorazione;
    public mostraForm: boolean = false;
    public formSelezionaCentro: FormGroup = this.fb.group({
        Centro: new FormControl(new BaseCodeDescr(0)),
    });

    constructor(private centriService: CentriAziendaliService,
                private objParametriAgendaService: ObjParametriAgendaService,
                private selezionaCentroService: SelezionaCentroService,
                private fb: FormBuilder) {
    }

    ngOnInit(): void {
        this.formSelezionaCentro.valueChanges.subscribe((formVal) => {
            this.selezionaCentroService.centroSelezionatoSubject.next(formVal.Centro);
        });
        var impresa = new Impresa();
        impresa.partitaIva = this.objParametriAgendaService.getObjParamValue().Piva;
        from(this.centriService.leggiCentriAziendaliModelloQdC(<LeggiCentriAziendali>{impresa: impresa, data: new Date()}, false)).pipe(
            map((vals) => {
                return vals.map((el) => {
                    return { codice: el.primaryKey.codice, descrizione: el.nome }
                })
            })
        ).subscribe((vals) => {
            this.mostraForm = true
            this.ListCentri = vals;
            this.formSelezionaCentro.controls['Centro'].setValue(this.ListCentri[0]);
        })
    }
}
