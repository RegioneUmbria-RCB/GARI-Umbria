import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormControl, FormGroup } from "@angular/forms";
import { Lavorazione } from "app/Model/attivita/Lavorazione";
import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import { MasterService } from "app/Service/master.service";
import { OperazioniService } from "app/Service/Metaschema/operazioni.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { map } from "rxjs";
import { NuovaOperazioneService } from "./nuova-operazione.service";

@Component({
    standalone: false,
    selector: 'app-esercizi-nuova-operazione',
    templateUrl: './nuova-operazione.component.html',
    providers: []
})
export class NuovaOperazioneComponent implements OnInit {
    ListOperazioni: BaseCodeDescr[];
    OperazionePredefinita: Lavorazione;
    mostraForm: boolean = false;
    formOperazione: FormGroup = this.fb.group({
        Operazione: new FormControl(new BaseCodeDescr(0)),
    });

    constructor(private OperazioniService: OperazioniService,
        private permessiUtenteService: PermessiUtenteService,
        private nuovaOperazioneService: NuovaOperazioneService,
        private fb: FormBuilder) {
    }

    ngOnInit(): void {
        this.formOperazione.valueChanges.subscribe((formVal) => {
            this.nuovaOperazioneService.operazioneSelezionataSubject.next(formVal.Operazione);
        });
        this.OperazioniService.CaricaComboLavorazioni(['C', 'E']).pipe(
           map((vals) => { return vals.map((el) => { return { codice: parseInt(el.primaryKey.codice), descrizione: el.descrizione } }) })
        ).subscribe((vals) => {
            this.mostraForm = true
            this.ListOperazioni = vals;
            let impostazioneUtente = this.permessiUtenteService.getImpostazione_Utente(856)
            if (impostazioneUtente != undefined && impostazioneUtente.Valore != "") {
                const operazionePredefinita = this.ListOperazioni.find((val) => val.codice == parseInt(impostazioneUtente.Valore));
                if (operazionePredefinita != undefined) {
                    this.formOperazione.controls['Operazione'].setValue(operazionePredefinita);
                }
            } else {
                this.formOperazione.controls['Operazione'].setValue(this.ListOperazioni[0]);
            }
        })
    }
}
