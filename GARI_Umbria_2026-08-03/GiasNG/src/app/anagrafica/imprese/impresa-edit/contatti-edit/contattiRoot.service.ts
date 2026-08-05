import { Injectable } from "@angular/core";
import { ContattoAzienda } from "app/Service/api.service";
import { KendoGridRow } from 'gias-kendo-grid';
import { BehaviorSubject } from "rxjs";

@Injectable({providedIn:'root'})
export class ContattiRootService {

    private salvaInPadre: boolean = false;
    private salvaInPadreSource = new BehaviorSubject(this.salvaInPadre);
    padre: number = 0;

    private creaContattiPubblici: boolean = true;

    private creaContattiPubbliciSource = new BehaviorSubject(this.creaContattiPubblici);

    gridRowsContatti: KendoGridRow[] = new Array<KendoGridRow>();

    gridRowsContattiSource = new BehaviorSubject(this.gridRowsContatti);

    private singoloPadre: boolean = true;

    private objcontattoAzienda: ContattoAzienda;

    public singoloPadreSource = new BehaviorSubject(this.singoloPadre);

    public setSalvaInPadre(val: boolean){
        console.log("setSalvaInPadre " + val);
        this.creaContattiPubbliciSource.next(val);
    }

    public getSalvaInPadre(): boolean {
        return this.creaContattiPubbliciSource.getValue();
    }

    public setCreaContattiPubblici(val: boolean){
        console.log("setCreaContattiPubblici " + val);
        this.creaContattiPubbliciSource.next(val);
    }

    public getCreaContattiPubblici(): boolean {
        return this.creaContattiPubbliciSource.getValue();
    }

    public setSingoloPadre(val: boolean){
        this.singoloPadreSource.next(val);
    }

    public getSingoloPadre(): boolean {
        return this.singoloPadreSource.getValue();
    }

    public getRowsContatti(): KendoGridRow[] {
        return this.gridRowsContattiSource.getValue();
    }

    public setRowsContatti(rows: Array<KendoGridRow>) {
        this.gridRowsContattiSource.next(rows);
    }

    public setContattoAzienda(contattoAzienda: ContattoAzienda){
        this.objcontattoAzienda = contattoAzienda;
        this.objcontattoAzienda.contattoPubblico = this.getCreaContattiPubblici();
        console.log(this.objcontattoAzienda)
    }

    public getContattoAzienda(): ContattoAzienda {
        return this.objcontattoAzienda;
    }
    
}

