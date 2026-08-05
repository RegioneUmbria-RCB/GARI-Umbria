import { Injectable } from "@angular/core";
import { FormArray, FormGroup } from "@angular/forms";
import { FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import { QdCService } from "./qdc.service";
import {MultiColumnComboboxTrattamento} from "../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {FORMULATI} from "../../../Model/CostantiPersonalizzate";
import {CellCloseEvent} from "@progress/kendo-angular-grid";

export class SupTrattata_SupRiduzioneBuffer {
    Sup_Trattata: number;
    SupRiduzioneBuffer: number;
    BufferMin: number;
}

@Injectable()
export class CalcoloSuperficiService {

    constructor (private qdCService: QdCService,
        private funzionicomuniservice: FunzioniComuniService) {  }

    /**
     * @description
     * Richiamato per aggiornare le colonne delle buffer zone e la superficie trattata
     */
    public ricalcoloSuperfici(dataItem:any):boolean {

        let ricalcoliEffettuati = false;

        let distBZ_complessiva = dataItem.DistBZ_Allevamenti + dataItem.DistBZ_AreeResPub + dataItem.DistBZ_CorpiIdrici + dataItem.DistBZ_VegNatNonColt;

        //Se la buffer impostata nell'anagrafica degli impianti è 0 allora non faccio nessun ricalcolo delle superfici
         if(distBZ_complessiva <= 0)
            return ricalcoliEffettuati;


        let index = this.qdCService.ImpiantiSelezionatiFormArray.getRawValue().findIndex(i => i.APPEZZA == dataItem.APPEZZA &&
            i.PIVA == dataItem.PIVA &&
            i.ID_REG == dataItem.ID_REG &&
            i.SA_COD == dataItem.SA_COD &&
            i.Progetto_Cod == dataItem.Progetto_Cod
        );

        let superfici_calcolate = this.calcolo(dataItem.Sup_Imp, dataItem.Perc_Riduzione_Deriva, dataItem.SupBZ_Riduzione, dataItem.Sup_Riduzione_BufferZone,distBZ_complessiva);

        //Se ho impostato una bufferzone più alta di quella calcolata, allora ricalcolo la supTrattata secondo la BZ più elevata
        if(superfici_calcolate.SupRiduzioneBuffer < dataItem.Sup_Riduzione_BufferZone) {
            superfici_calcolate.SupRiduzioneBuffer = dataItem.Sup_Riduzione_BufferZone;
            superfici_calcolate.Sup_Trattata = this.funzionicomuniservice.roundNumber((dataItem.Sup_Imp-dataItem.Sup_Riduzione_BufferZone),this.qdCService.get4DecimalNumericSettings().decimals);
        }

        //Se la superficie di trattata è minore di quella calcolata mantengo la superficie trattata già presente
        if(superfici_calcolate.Sup_Trattata > dataItem.Sup_Imp_help){
            superfici_calcolate.Sup_Trattata = dataItem.Sup_Imp_help;
        }

        //Aggiorno il QdCForm
        var val = this.qdCService.ImpiantiSelezionatiFormArray.getRawValue()[index];

        val.Sup_Imp_help = superfici_calcolate.Sup_Trattata;

        val.Sup_Riduzione_BufferZone = superfici_calcolate.SupRiduzioneBuffer;

        val.Perc_Riduzione_Deriva = dataItem.Perc_Riduzione_Deriva;

        if(dataItem.Sup_Imp_help !== val.Sup_Imp_help || dataItem.Sup_Riduzione_BufferZone !== val.Sup_Riduzione_BufferZone){
            ricalcoliEffettuati = true;
        }

        dataItem.Sup_Imp_help = val.Sup_Imp_help;

        dataItem.Sup_Riduzione_BufferZone = val.Sup_Riduzione_BufferZone;

        this.qdCService.ImpiantiSelezionatiFormArray.at(index).patchValue(val);

        return ricalcoliEffettuati;

    }

    /**
     * @description
     * Richiamato durante il cellclose della griglia
     * Replica il onEditKendoSup_Coinvolta della OperazioneBootstrap.master
     */
    public ricalcoloSuperfici_Edit_Grid_Impianti(dataItem:any, event: CellCloseEvent) {

        let colonna_editata: string = event.column.field;

        //Se c'è la percentuale di abbattimento vince lei sulla riduzione della Superficie Trattata rispetto alla bufferzone rispetto alla

        if(event){

            let superfici_calcolate = <SupTrattata_SupRiduzioneBuffer>{
                SupRiduzioneBuffer: 0,
                Sup_Trattata: dataItem.Sup_Imp,
                BufferMin: 0
            };

            let distBZ_complessiva = dataItem.DistBZ_Allevamenti + dataItem.DistBZ_AreeResPub + dataItem.DistBZ_CorpiIdrici + dataItem.DistBZ_VegNatNonColt;

            //se c'è una buffer calcolo la sup massima trattabile e riducibile
            if(distBZ_complessiva > 0){

                //Nel caso in cui l'utente ha editato di sua spontanea volontà la perc_riduzione_deriva perchè sta indicando la percentuale di quanto deve essere ridotta la Sup_Riduzione_BufferZone
                //passo la Sup_Riduzione_BufferZone a 0 per sovrascrivere il valore presente nella colonna

                let Sup_Riduzione_BufferZone: number = 0;

                if(colonna_editata !== "Perc_Riduzione_Deriva"){
                    Sup_Riduzione_BufferZone = dataItem.Sup_Riduzione_BufferZone;
                }

                superfici_calcolate = this.calcolo(dataItem.Sup_Imp, dataItem.Perc_Riduzione_Deriva, dataItem.SupBZ_Riduzione,Sup_Riduzione_BufferZone, distBZ_complessiva);
            }

            let percAbb:number = this.qdCService.OttieniPercentualeAbbattimentoDiserboDisseccamento(null,null);

            switch (colonna_editata){
                case "Sup_Imp_help":

                    //Prendo la sup trattata che rispetta la buffer
                    if(superfici_calcolate.Sup_Trattata < dataItem.Sup_Imp_help || dataItem.Sup_Imp_help === undefined || dataItem.Sup_Imp_help === null){
                        dataItem.Sup_Imp_help = superfici_calcolate.Sup_Trattata;
                    }

                    //cambio la sup riduzione
                    if(percAbb === 0 && superfici_calcolate.SupRiduzioneBuffer > 0){
                        dataItem.Sup_Riduzione_BufferZone = superfici_calcolate.SupRiduzioneBuffer;
                    }
                    break;
                case "Sup_Riduzione_BufferZone":

                    //Prendo la buffer più alta
                    if(superfici_calcolate.SupRiduzioneBuffer > dataItem.Sup_Riduzione_BufferZone || dataItem.Sup_Riduzione_BufferZone === undefined || dataItem.Sup_Riduzione_BufferZone === null){
                        dataItem.Sup_Riduzione_BufferZone = superfici_calcolate.SupRiduzioneBuffer;
                    }

                    //se ho la buffer verifico che stia nel range la sup ridotta

                    //cambio la sup trattata
                    if(percAbb === 0 && superfici_calcolate.SupRiduzioneBuffer > 0){
                        dataItem.Sup_Imp_help = superfici_calcolate.Sup_Trattata;
                    }
                    break;
                case "Perc_Riduzione_Deriva":
                    //sto editando la percentuale di riduzione deriva
                    //cambio la sup riduzione
                    dataItem.Sup_Riduzione_BufferZone = this.funzionicomuniservice.roundNumber((dataItem.Sup_Imp-superfici_calcolate.Sup_Trattata),this.qdCService.get4DecimalNumericSettings().decimals);

                    //cambio la sup trattata
                    if(percAbb === 0 && superfici_calcolate.SupRiduzioneBuffer > 0){
                        dataItem.Sup_Imp_help = superfici_calcolate.Sup_Trattata;
                    }

                    if(dataItem.Perc_Riduzione_Deriva === undefined || dataItem.Perc_Riduzione_Deriva === null){
                        dataItem.Perc_Riduzione_Deriva = 0;
                    }
                    break;
            }

            let index = this.qdCService.ImpiantiSelezionatiFormArray.getRawValue().findIndex(i => i.APPEZZA == dataItem.APPEZZA &&
                i.PIVA == dataItem.PIVA &&
                i.ID_REG == dataItem.ID_REG &&
                i.SA_COD == dataItem.SA_COD &&
                i.Progetto_Cod == dataItem.Progetto_Cod
            );

            let listerroriGias = this.qdCService.ControllaSuperficieTrattabileXPercentualeAbbattimento(dataItem);

            this.qdCService.gestisci_ErroriGias(listerroriGias,true,true).then();

            //Aggiorno il QdCForm
            var val = this.qdCService.ImpiantiSelezionatiFormArray.getRawValue()[index];

            val.Sup_Riduzione_BufferZone = dataItem.Sup_Riduzione_BufferZone;

            val.Sup_Imp_help = dataItem.Sup_Imp_help;

            val.Perc_Riduzione_Deriva = dataItem.Perc_Riduzione_Deriva;

            this.qdCService.ImpiantiSelezionatiFormArray.at(index).patchValue(val);

        }
    }

    private calcolo(Sup_Imp: number, perc_riduzione_deriva: number, SupBZ_Riduzione: number,SupBZ_Riduzione_QdC: number, lunghezza: number): SupTrattata_SupRiduzioneBuffer {

        let ret = new SupTrattata_SupRiduzioneBuffer();
        let sup_rid = 0;

        let buffer_min:number = this.Calcola_Buffer_Minima_Piu_Alta_DosiProdotti();

        if (buffer_min != 0) {
            sup_rid = this.calcolaSupRid(buffer_min, perc_riduzione_deriva, SupBZ_Riduzione, lunghezza);

            //Prendo La BufferZone maggiore tra quella calcolata in anagrafica e quella che mi arriva dalla colonna editabile del QdC
            if(SupBZ_Riduzione_QdC > sup_rid){
                sup_rid = SupBZ_Riduzione_QdC;
            }

        }

        ret.SupRiduzioneBuffer = sup_rid;
        ret.BufferMin = buffer_min;

        //Se la nuova sup trattata è negativa (perchè la bufferzone che richiede il prodotto è maggiore della superficie impianto) la porto a 0
        let sup_trattata: number =  this.funzionicomuniservice.roundNumber((Sup_Imp-sup_rid),this.qdCService.get4DecimalNumericSettings().decimals);

        if(sup_trattata > 0){
          ret.Sup_Trattata = sup_trattata;
        }else{
          ret.Sup_Trattata = 0;
        }

        return ret;
    }

    public Calcola_Buffer_Minima_Piu_Alta_DosiProdotti(){

        let buffer_min = 0;

        (this.qdCService.Sezioni_ProdottoFormArray.controls.forEach( (sp_fa: FormArray) => {
            (<FormArray> sp_fa.get('DosiProdotti')).controls.forEach(dp => {

                let Categoria_Magazzino =  dp.getRawValue().Categoria_Magazzino;

                if(Categoria_Magazzino === FORMULATI){
                    let Formulato: MultiColumnComboboxTrattamento = dp.getRawValue().Prodotto;

                    if(Formulato && Formulato.bufferzone &&  Formulato.bufferzone.minimo > buffer_min){
                        buffer_min = Formulato.bufferzone.minimo;
                    }
                }
            });
        }));

        return buffer_min;
    }

    private calcolaSupRid(buffer_min: number, perc_riduzione_deriva: number, SupBZ_Riduzione: number, lunghezza: number) {
        let sup_rid = ((buffer_min-SupBZ_Riduzione) - ((buffer_min-SupBZ_Riduzione) * (perc_riduzione_deriva/100))) * lunghezza;
        sup_rid = this.funzionicomuniservice.roundNumber((sup_rid/10000),this.qdCService.get4DecimalNumericSettings().decimals);
        return sup_rid;
    }

    public ricalcoloSuperfici_daDosi() {
        this.qdCService.ImpiantiSelezionatiFormArray.controls.forEach(c => {

            let distBZ_complessiva = c.value['DistBZ_Allevamenti'] + c.value['DistBZ_AreeResPub'] + c.value['DistBZ_CorpiIdrici'] + c.value['DistBZ_VegNatNonColt'];
            let superfici_calcolate = this.calcolo(c.value['Sup_Imp'], c.value['Perc_Riduzione_Deriva'], c.value['SupBZ_Riduzione'],c.value['Sup_Riduzione_BufferZone'], distBZ_complessiva);

            var val = c.value;

            if(val.Sup_Imp_help > superfici_calcolate.Sup_Trattata)
                val.Sup_Imp_help = superfici_calcolate.Sup_Trattata;

            if(val.Sup_Riduzione_BufferZone < superfici_calcolate.SupRiduzioneBuffer)
                val.Sup_Riduzione_BufferZone = superfici_calcolate.SupRiduzioneBuffer;

            val.Sup_Imp_help = this.calcoloPercentualeAbbatimento(c.value['Sup_Imp'],val.Sup_Imp_help);

            c.value['Sup_Imp_help'] = val.Sup_Imp_help;
            c.value['Sup_Riduzione_BufferZone'] = val.Sup_Riduzione_BufferZone;
        });

        this.qdCService.AggiornaImpiantiSelezionatiGrid();
    }

    calcoloPercentualeAbbatimento(Sup_Imp: number,Sup_Imp_help:number){

        // ******************************************************************************************************
        // CALCOLI PER PERCENTUALE ABBATTIMENTO
        // ******************************************************************************************************

        let percAbb:number = this.qdCService.OttieniPercentualeAbbattimentoDiserboDisseccamento(null,null);

        if (percAbb > 0 && percAbb < 100)
        {
            let supImpianto = Sup_Imp;
            let supTrattata = Sup_Imp_help;
            let supTrattabile = this.funzionicomuniservice.roundNumber(supImpianto * percAbb / 100 , this.qdCService.get4DecimalNumericSettings().decimals);

            if (supTrattabile < supTrattata){
                Sup_Imp_help = supTrattabile;
            }
        }

        return Sup_Imp_help;
    }
}
