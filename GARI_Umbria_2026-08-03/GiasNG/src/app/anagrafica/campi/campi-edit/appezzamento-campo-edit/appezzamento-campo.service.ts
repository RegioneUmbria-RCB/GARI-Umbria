import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { AppezzamentoCampo } from './appezzamento-campi-edit';

export class AppezzamentoCampoId extends AppezzamentoCampo{
    id: number;
}

@Injectable()
export class AppezzamentoCampoService{
    appezzamentoCampo: AppezzamentoCampoId[] = new Array();

    appezzamentoCampoSource = new BehaviorSubject(this.appezzamentoCampo);

    public setAppezzamentoCampo(appezzamentoCampo: AppezzamentoCampoId[]) {
        for (const appCampo of appezzamentoCampo) {
            if (appCampo.id == undefined || appCampo.id < 0) {
                appCampo.id = this.getNextId(appezzamentoCampo);
            }
        }
        this.appezzamentoCampoSource.next(appezzamentoCampo);
    }

    private getNextId(appezzamentoCampo: AppezzamentoCampoId[]): number {
        let newId = 0;
        const chiave = Math.max.apply(Math, appezzamentoCampo.map(function (o) {
            if (o.id != undefined) {
                return o.id;
            } else {
                return -1;
            }
        }));
        if (chiave >= 0) {
            newId = chiave + 1;
        }
        return newId;
    }

    public getAppezzamentoCampo(): AppezzamentoCampoId[] {
        return this.appezzamentoCampoSource.getValue();
    }

    ControllaSeSelezionareLaRiga(dataItem, catasto: AppezzamentoCampoId[]){

        let index=-1;

        if(catasto && catasto.length>0){

            //dataItem.chiave;

            const Piva = dataItem.chiave.split('_')[0];
            const Sa_Cod = parseInt(dataItem.chiave.split('_')[1]);
            const Appezza = parseInt(dataItem.chiave.split('_')[2]);
            const Id_Reg = parseInt(dataItem.chiave.split('_')[3]);
            const Campo_Cod = parseInt(dataItem.chiave.split('_')[4]);

            for(let x=0; x<catasto.length; x++){
                if(catasto[x].piva === Piva &&
                catasto[x].sa_cod === Sa_Cod &&
                catasto[x].appezza === Appezza &&
                catasto[x].id_reg ===  Id_Reg &&
                catasto[x].campo_cod ===  Campo_Cod) {

                    index=x;
                    return index;

                }
            }
        }

        return index;

    }

}
