import { Component, OnDestroy, OnInit } from "@angular/core";
import { enum_TipoOperatoreVisita } from "app/Model/TipiEnumerativi";
import { Operatore } from "app/Model/metaschema/utilizzi/Operatore";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { GiasDropDownTemplateSComponent, ObjParametriAgenda } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { QdCService } from "app/quaderno-di-campagna/agenda-edit/service/qdc.service";
import { OperatoreDDLService } from "app/quaderno-di-campagna/agenda-edit/service/testata/operatore-ddl-service";
import { QdCTestataService } from "app/quaderno-di-campagna/agenda-edit/service/testata/testata.service";
import { Subscription, lastValueFrom, skip } from "rxjs";


@Component({
    standalone: false,
    selector: 'app-operatore',
    templateUrl: './operatore.component.html',
    styleUrls: ['./operatore.component.css'],
      providers: [GiasDropDownTemplateService]
  })
export class OperatoreComponent implements OnInit, OnDestroy {

    private ddlSub: Subscription = new Subscription();

    private typeAccount: enum_TipoOperatoreVisita;

    public arrayOperatori: Operatore[] = [];

    objParametriAgenda: ObjParametriAgenda;

    constructor(public qdcservice: QdCService,
                public objParametriAgendaService: ObjParametriAgendaService,
                public permessiUtenteS: PermessiUtenteService,
                private ddlService: GiasDropDownTemplateService,
                private operatoreDDLService: OperatoreDDLService,
                private testataService: QdCTestataService) {
                }


    ngOnInit(): void {

        this.objParametriAgenda= this.objParametriAgendaService.getObjParamValue();

        this.ddlSub.add(this.ddlService.currentDropDownValueObject
            .pipe(skip(1))
            .subscribe(async ddlElem => {
                switch (ddlElem.FormControlName) {
                    case 'Operatore_Visita':
                        this.qdcservice.TestataVisitaForm.patchValue({
                            Operatore_Visita:  ddlElem.Value
                        });
                        this.testataService.getListaAziende(ddlElem.Value.username, true, true);
                    break;
                }

            })
        );

        this.getTypeOperatore();
        this.getListaTecnici();

    }

    async getTypeOperatore() {
        let R = await lastValueFrom(this.operatoreDDLService.leggiTecnicoOCapo());
        console.log(R);
        if (R ==="2")
            this.typeAccount = enum_TipoOperatoreVisita.CapoTecnico;
        else if (R ==="1")
            this.typeAccount = enum_TipoOperatoreVisita.Tecnico;
        else if (R ==="0")
            this.typeAccount = enum_TipoOperatoreVisita.Capo;
    }


    public valueChange(value: any): void {
        console.log("valueChange " + value);
    }


    async getListaTecnici() {
        let result = await lastValueFrom(this.operatoreDDLService.leggiListaTecnici());
        this.arrayOperatori = JSON.parse(JSON.stringify(result));

        let username = '';
        let fromSwitchAziende: boolean = true;

        if (this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write) {
            if (this.arrayOperatori.length > 0) {
                this.qdcservice.TestataVisitaForm.controls['Operatore_Visita'].patchValue(this.arrayOperatori[0]);
                username = this.arrayOperatori[0].username;
            }
        } else {                                                                                            //se sono in update, chiamo comunque la funzione che riempie la ddl
            if (this.arrayOperatori.length > 0) {
                let operatoreUpdate = this.qdcservice.TestataVisitaForm.get("Operatore_Visita").value;               //delle aziende
                operatoreUpdate = this.arrayOperatori.filter((o: Operatore) => {return o.Cod_RisUm === operatoreUpdate.Cod_RisUm});
                this.qdcservice.TestataVisitaForm.controls['Operatore_Visita'].patchValue(operatoreUpdate[0]);            //faccio questo perché manca lo username nella risorsa umana
                this.qdcservice.TestataVisitaForm.get("Operatore_Visita").disable({emitEvent: false});
                username = operatoreUpdate[0].username;
            }
            fromSwitchAziende = false;
        }

        this.testataService.getListaAziende(username, fromSwitchAziende, true);
    }


    async openDdlOperatore(ddlEl: GiasDropDownTemplateSComponent, formName: string) {

        let fn: any;

        switch (formName) {
            case 'Operatore':
                ddlEl.listItems = this.arrayOperatori;
                break;
        }
    }



    ngOnDestroy(): void {
        this.ddlSub.unsubscribe();
    }

    getArray_Operatori() {
        return this.arrayOperatori;
    }


}
