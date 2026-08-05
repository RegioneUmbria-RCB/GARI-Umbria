import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { GridImpiantiService } from 'app/quaderno-di-campagna/agenda-edit/service/grid-impianti/grid-impianti.service';
import {pairwise, skip, Subscription} from 'rxjs';
import { QdCService } from '../../service/qdc.service';
import {enum_LAVCOD, enum_SEMINA_TIPO} from "../../../../Model/TipiEnumerativi";
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {ObjParametriAgendaService} from "../../../../Service/obj-parametri-agenda.service";
import {MisceleService} from "../../service/miscele.service";
import {Lavorazione} from "../../../../Model/attivita/Lavorazione";
import {ErroreGias} from "../../../../Service/master.service";
import {Sezione_Prodotto} from "../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";

@Component({
    standalone: false,
    selector: 'app-superfici-impianti',
    templateUrl: './superfici-impianti.component.html',
    styleUrls: ['./superfici-impianti.component.css']
})
export class SuperficiImpiantiComponent implements OnInit,OnDestroy {

    public autocorrect=true;

    Subs: Subscription = new Subscription();

    constructor(public qdcservice: QdCService,
        private gridimpiantiservice: GridImpiantiService,
        private changeDetector: ChangeDetectorRef,
        private objParametriAgendaService: ObjParametriAgendaService,
        private misceleservice: MisceleService
    ) {
    }

    ngOnInit(): void {

        // Tolgo e rimetto la propietà autocorrect per aggiornare correttamente il valore della numeric textbox
        this.Subs.add(this.gridimpiantiservice.AutoCorrectNumeric.pipe(skip(1)).subscribe(item =>{
            this.autocorrect = item;
            this.changeDetector.detectChanges();
        }));

        this.Subs.add(this.qdcservice.Sezioni_ProdottoFormArray.valueChanges.subscribe((sezioni: Array<any>)=>{

            //Disabilito la textbox della Superficie Trattata se sono in scrittura e col
            //frazionamento

            let Opzione_Semina = null;

            if(sezioni){

                let Operazione_con_Sementi = sezioni.find(s=> {
                    if(!s.Operazione) return false;
                    if(this.qdcservice.Elenco_Operazioni_Sementi.includes(+s.Operazione.primaryKey.codice))
                        return s;
                });


                if(Operazione_con_Sementi){
                    Opzione_Semina = this.qdcservice.getOpzione_Semina_Model(Operazione_con_Sementi.Operazione);

                    //Recupero il valore di default dell' impostazione utente
                    if(!Opzione_Semina)
                        Opzione_Semina = this.qdcservice.get_Default_Opzione_Semina([Operazione_con_Sementi.Operazione]);

                }

            }

            if (this.qdcservice.Sola_Lettura_QdCForm() ||
                (Opzione_Semina &&
                Opzione_Semina.codice === enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default &&
                this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB === Enum_DBTypeOperation.Write)) {

                this.qdcservice.SuperficiForm.get("Sup_Trattata").disable({ emitEvent: false });

            } else {

                this.qdcservice.SuperficiForm.get("Sup_Trattata").enable({emitEvent: false});

            }
        }));
    }

    ngOnDestroy(): void{
        this.Subs.unsubscribe();
    }

    /* --------------------------------------------------------------------------------------------------------
        La funzione sottostante è stata creata al fine di impedire la ripartizione della superficie trattata
        quando essa viene modificata dalla griglia impianti
    -------------------------------------------------------------------------------------------------------- */
    supTrattataValueChange () {
        if(this.qdcservice.SuperficiForm.get('Sup_Trattata').valid) {

            this.Controlli_SuperficieTrattata();

            this.misceleservice.calcoloMiscele('Sup_Trattata',null,0, this.qdcservice.SuperficiForm.get("Sup_Trattata").value);

            const newkendoserver=this.gridimpiantiservice.RipartizionaSupTrattata(this.qdcservice.GridImpiantiPublicService.getValue());

            this.qdcservice.GridImpiantiPublicService.refresh(false,newkendoserver.data);

            this.qdcservice.AggiornaFormArrayImpiantiSelezionati();

            this.misceleservice.updateGridIrrigationData(this.qdcservice.QdCForm);

            if(this.qdcservice.Sezioni_ProdottoFormArray &&
              this.qdcservice.Sezioni_ProdottoFormArray.getRawValue().findIndex((s: Sezione_Prodotto)=>this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(+ s.Operazione.primaryKey.codice)) >-1)
              this.qdcservice.RicaricaTutteGridProdottixImpianti();
        }

    }

    //Dopo che viene cambiata la superficie trattata controlla se rispetta la percentuale di superficie trattabile dell'abbattimento
    //se non viene rispettata gli rimetto il valore massimo di superficie trattabile e rifaccio i calcoli
    Controlli_SuperficieTrattata(){

        let list_ErroriGias: ErroreGias[]=[];

        let index = this.qdcservice?.TestataForm?.get("Operazioni")?.getRawValue()?.findIndex((o:Lavorazione)=>+ o.primaryKey.codice === enum_LAVCOD.DISERBO ||
            + o.primaryKey.codice === enum_LAVCOD.DISSECCAMENTO);

        let grid = JSON.parse(JSON.stringify(this.qdcservice.GridImpiantiPublicService.getValue()));

        if(index >-1 && grid){
            grid =this.gridimpiantiservice.RipartizionaSupTrattata(grid);

            for(let r of grid.data.rows){
                let ErroriGias = this.qdcservice.ControllaSuperficieTrattabileXPercentualeAbbattimento(r);

                if(ErroriGias.length === 1 && list_ErroriGias.length === 0)
                    list_ErroriGias.push(ErroriGias[0]);
            }
        }

        if(list_ErroriGias.length > 0){

            this.gridimpiantiservice.RicalcolaSuperficieCoinvolta(grid.data.rows,false);

            this.qdcservice.gestisci_ErroriGias(list_ErroriGias,true,true).then();
        }
    }

}
