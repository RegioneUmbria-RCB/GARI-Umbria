import { Component, OnInit } from '@angular/core';
import {AjaxAgronicaService} from '../../../Service/ajax-agronica.service';
import {MasterService} from '../../../Service/master.service';
import {ImpostazioniAziendeCentriService} from '../../services/impostazioni/impostazioni-aziende-centri.service';
import {ImpostazioniFormService} from '../../services/impostazioni/impostazioni-form.service';
import { Impostazione } from 'gias-ui-kit';
import {FormControl, FormGroup} from '@angular/forms';

@Component({
  standalone: false,
  selector: 'app-settings-manager',
  templateUrl: './settings-manager.component.html',
  styleUrls: ['./settings-manager.component.css']
})
export class SettingsManagerComponent implements OnInit {
    public formGuidaImp: FormGroup;

    constructor(private ajax: AjaxAgronicaService,
                private master: MasterService,
                private settingsService: ImpostazioniFormService,
                private imp: ImpostazioniAziendeCentriService) { }

    ngOnInit(): void {
        this.initForm();
    }

    aggiungiGuidaImpostazione() {
        console.log(this.formGuidaImp);
        const s = new Impostazione(this.formGuidaImp.value.codice, this.formGuidaImp.value.descrizione);
        s.setVisibility({isSuperUser:this.formGuidaImp.value.isSuperUser, isAzienda:this.formGuidaImp.value.isAziendaCentro, isUtente:0});
        s.setSection(0, 0, 0, 0);
        this.settingsService.aggiungiGuidaImpostazione([s]);
    }

    private initForm() {
        this.formGuidaImp = new FormGroup<any>({
            codice: new FormControl(0),
            descrizione: new FormControl(''),
            isSuperUser: new FormControl(false),
            isAziendaCentro: new FormControl(false),
            sezione: new FormControl(0),
            sottoSezione: new FormControl(0),
            livello: new FormControl(0)
        });
    }

}
