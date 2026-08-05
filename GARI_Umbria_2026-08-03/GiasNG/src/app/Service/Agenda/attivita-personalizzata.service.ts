import { Injectable } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { AttivitaPersonalizzata } from "app/Model/attivita/AttivitaPersonalizzata";
import { CoreWS_Generic } from "app/Model/CoreWS/CoreWS_Generic";
import { AjaxAgronicaService } from "../ajax-agronica.service";
import { MasterService } from "../master.service";
import { Lavorazione } from "app/Model/attivita/Lavorazione";
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import { map } from "rxjs";

export class LeggiAttivitaPersonalizzata{
   
   public Operazioni_scelte: Array<Lavorazione>;

   constructor() {
    this.Operazioni_scelte = [];
   }
}

@Injectable({
    providedIn: 'root'
})
export class AttivitaPersonalizzataService{

    constructor(private masterService: MasterService,
                private ajaxAgronicaService: AjaxAgronicaService,
                private translocoService: TranslocoService,
                private ajaxAgronicaAPIService: AjaxAgronicaAPIService){}

    /*Leggi_AttivitaPersonalizzata_Old(flagPrimaRiga: boolean, codiceRigaVuota: number,descrizioneRigaVuota: string){
        return new Promise<AttivitaPersonalizzata[]>(async (resolve, reject) => {

            let List =[];

            const parametri: CoreWS_Generic<LeggiAttivitaPersonalizzata> = new CoreWS_Generic
            (
                this.masterService.getCoreWSGenericObjP(),
                {}
            );

            const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<AttivitaPersonalizzata[], LeggiAttivitaPersonalizzata>(this.masterService.link_CoreWS + '/Contab/Attivita.asmx/Leggi_AttivitaPersonalizzata_Modello', parametri);

            List = <AttivitaPersonalizzata[]>R.RispostaStringa;

            if(flagPrimaRiga){

                const AttivitaPersonalizzataPrimaRiga=new AttivitaPersonalizzata(codiceRigaVuota);

                AttivitaPersonalizzataPrimaRiga.descrizione = (descrizioneRigaVuota !== "") ? this.translocoService.translate(descrizioneRigaVuota) : "";

                List.unshift(AttivitaPersonalizzataPrimaRiga);
            }

            resolve(List);
        });
    }*/

    Leggi_AttivitaPersonalizzata(flagPrimaRiga: boolean, codiceRigaVuota: number,descrizioneRigaVuota: string, arrayOperazioni: Array<Lavorazione>){
        return new Promise<AttivitaPersonalizzata[]>(async (resolve, reject) => {

            let List =[];

            let param: LeggiAttivitaPersonalizzata = new LeggiAttivitaPersonalizzata();

            param.Operazioni_scelte = arrayOperazioni;

            this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiAttivitaPersonalizzata, AttivitaPersonalizzata[]>('Contab/Leggi_AttivitaPersonalizzata_Modello', param).pipe(map(R => {
                List = <AttivitaPersonalizzata[]>R.RispostaStringa;
                if(flagPrimaRiga){
    
                    const AttivitaPersonalizzataPrimaRiga=new AttivitaPersonalizzata(codiceRigaVuota);
    
                    AttivitaPersonalizzataPrimaRiga.descrizione = (descrizioneRigaVuota !== "") ? this.translocoService.translate(descrizioneRigaVuota) : "";
    
                    List.unshift(AttivitaPersonalizzataPrimaRiga);
                }
    
                resolve(List);
            })).subscribe();
        });
    }

    Leggi_AttivitaPersonalizzataVisita() {
        return this.ajaxAgronicaAPIService.ajaxAPIGet<string, Array<AttivitaPersonalizzata>>('Visite/LeggiVisiteAttivita', "").pipe(map(r =>{
            return r.RispostaStringa;
        }));
    }

}
