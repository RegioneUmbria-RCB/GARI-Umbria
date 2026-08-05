import { Injectable } from "@angular/core";
import { CodiceAnagrafe } from "app/Model/anagrafiche/CodiceAnagrafe";
import { CodiciAnagrafeValoriChiave } from "app/Model/anagrafiche/CodiciAnagrafeValori";
import { IntervalloTemporale } from "app/Model/anagrafiche/IntervalloTemporale";
import { CoreWS_Generic } from "app/Model/CoreWS/CoreWS_Generic";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { AjaxAgronicaAPIService } from "app/Service/ajax-agronica.api.service";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import { MasterService } from "app/Service/master.service";
import { ICodiciTemplateService } from "app/Utility/Template/codici-template/services/codici-template.service";
import { HttpAction } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { BehaviorSubject, map, Observable, of, take } from "rxjs";

const itemIndex = (item: CodiciAnagrafeValoriChiave, data: CodiciAnagrafeValoriChiave[]): number => {
    for (let idx = 0; idx < data.length; idx++) {
        if (data[idx].chiave === item.chiave) {
            return idx;
        }
    }

    return -1;
};

@Injectable()
export class AppezzamentoEditCodici implements ICodiciTemplateService {
    gridId = "AppezzamentoCodici";

    public gridPublicService: GridPublicService;
        private codici: BehaviorSubject<CodiciAnagrafeValoriChiave[]> = new BehaviorSubject<CodiciAnagrafeValoriChiave[]>([]);
    currentodici: Observable<CodiciAnagrafeValoriChiave[]> = this.codici.asObservable();

    private codiciList: CodiceAnagrafe[];

    constructor(private ajaxAgronicaService: AjaxAgronicaService,
                private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
                private masterService: MasterService
    ) {
        let a = 0;
    }

    getCodici(): CodiciAnagrafeValoriChiave[] {
        return this.codici.getValue();
    }

    setCodici(vals: CodiciAnagrafeValoriChiave[])  {
        this.codici.next(vals);
    }

    public updateTable(action: HttpAction, row: CodiciAnagrafeValoriChiave) {
        let newCods = this.codici.getValue();
        switch(action) {
            case HttpAction.CREATE:
                const chiave = Math.max.apply(Math, this.codici.getValue().map(function (o) {
                    return o.chiave;
                }));
                if (chiave > 0) {
                    row.chiave = chiave + 1;
                } else {
                    row.chiave = 0;
                }
                newCods.push(row);
                break;
            case HttpAction.UPDATE:
                const index = itemIndex(row, this.codici.getValue());
                newCods.splice(index, 1, row);
                break;
            case HttpAction.REMOVE:
                const _index = itemIndex(row, this.codici.getValue());
                newCods.splice(_index, 1);
                break;
        }
        this.codici.next(newCods);
        this.gridPublicService.refresh(true);
    }

    private setRowsUniqueId(rows: CodiciAnagrafeValoriChiave[]) {
        let i = 0;
        rows.forEach((row) => {
            row.chiave = i++;
        });
    }

    public async postForm() {
      let a = 0; //Commento per funzione vuota SonarQube
    }

    /*leggiDropdowns_Old(): Observable<CodiceAnagrafe[]> {
        if (this.codiciList == null) {
            let parametri:CoreWS_Generic<object> = {
                objP: this.masterService.getCoreWSGenericObjP(),
                InData: null
            }
            const obs: Observable<CodiceAnagrafe[]> =
                this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<CodiceAnagrafe[], object>(
                    this.masterService.link_CoreWS + "/Anagrafica/Appezzamento.asmx/Leggi_DropDown_Appezzamenti_Codici",
                    parametri).pipe(take(1), map(el => {
                        this.codiciList = el.RispostaStringa;
                    return el.RispostaStringa;
                }))
            return obs;
        } else {
            return of(this.codiciList);
        }
    }*/

    leggiDropdowns(): Observable<CodiceAnagrafe[]> {
        if (this.codiciList == null) {
            let parametri:object = {
                InData: null
            }
            const obs: Observable<CodiceAnagrafe[]> =
                this.ajaxAgronicaAPIService.ajaxAPIGet<object, CodiceAnagrafe[]>("AnagraficaNG/LeggiDropDownAppezzamentiCodici",
                    parametri).pipe(take(1), map(el => {
                        this.codiciList = el.RispostaStringa;
                    return el.RispostaStringa;
                }))
            return obs;
        } else {
            return of(this.codiciList);
        }
    }

    leggiDatiTabella(): Observable<CodiciAnagrafeValoriChiave[]> {
        return of(this.codici.getValue());
    }

}
