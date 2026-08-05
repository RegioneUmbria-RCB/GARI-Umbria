import { Injectable } from '@angular/core';
import {CellCloseEvent, ColumnComponent} from '@progress/kendo-angular-grid';
import { TooltipDirective } from '@progress/kendo-angular-tooltip';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { BehaviorSubject, Subject } from 'rxjs';
import { CalcoloSuperficiService } from '../calcolo-superfici.service';
import { QdCService } from '../qdc.service';
import { QdCTestataService } from '../testata/testata.service';
import {Lavorazione} from "../../../../Model/attivita/Lavorazione";
import {
  Dettaglio_Formulato,
  GridImpiantoSelezionatoModel,
  Sezione_Prodotto
} from "../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {MisceleService} from "../miscele.service";
import { enum_LAVCOD, SIMBOLO_M3_HA, SIMBOLO_MM, enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { ErroreGias_Severity, ErroreGias } from "app/Service/master.service";
import {QuantitaSuImpianto} from "../../../../Model/attivita/dettagli/QuantitaSuImpianto";
import {EsercizioCDC} from "../../../../Model/attivita/centri_di_costo/EsercizioCDC";
import {CodiceDescrizione} from 'app/Utility/UtilityFunctions';
import {IrrigazioneAcquaService} from '../irrigazione-acqua.service';
import {TranslocoService} from "@jsverse/transloco";


@Injectable()

export class GridImpiantiService {

    public tooltip: TooltipDirective;

    // Observable per cambiare la proprietà AutoCorrect della numeric textbox
    private AutoCorrectNumericSource = new BehaviorSubject<boolean>(true);
    // Observable per cambiare la proprietà AutoCorrect della numeric textbox
    AutoCorrectNumeric = this.AutoCorrectNumericSource.asObservable();

    constructor(private qdcservice: QdCService,
                private funzionicomuniservice: FunzioniComuniService,
                private calcoloSuperfici: CalcoloSuperficiService,
                public testataservice: QdCTestataService,
                private misceleservice: MisceleService,
                private giasMessageService: GiasMessageService,
                private translocoService: TranslocoService,
                private irrigazioneAcquaService: IrrigazioneAcquaService) {
    }
    settooltip(tooltip: TooltipDirective){
        this.tooltip=tooltip;
    }

    // Ripartizione della superficie trattata
    RipartizionaSupTrattata(grid) {

        const SupSel = this.qdcservice.SuperficiForm.get('Sup_Selezionata').value;
        const SupTratt = this.qdcservice.SuperficiForm.get('Sup_Trattata').value ?? 0;

        let attr = this.funzionicomuniservice.roundNumber(SupTratt / SupSel,this.qdcservice.get4DecimalNumericSettings().decimals);

        const rows = grid.data.rows;
        const filteredData = this.kGetElementiSelezionati(rows);
        const nSel = filteredData.length;
        let somma: any = 0;
        const that=this;

        $.each(filteredData, function (idx, dataItem) {
            let daInserire;
            const app = parseFloat(dataItem.Sup_Imp);
            if (nSel - 1 !== idx) {
                daInserire = that.funzionicomuniservice.roundNumber((app * attr), that.qdcservice.get4DecimalNumericSettings().decimals);
                somma += daInserire;
            } else {
                daInserire = (SupTratt.toFixed(that.qdcservice.get4DecimalNumericSettings().decimals) - somma.toFixed(that.qdcservice.get4DecimalNumericSettings().decimals)).toFixed(that.qdcservice.get4DecimalNumericSettings().decimals);
            }

            dataItem.Sup_Imp_help =  that.funzionicomuniservice.roundNumber(+ daInserire, that.qdcservice.get4DecimalNumericSettings().decimals);

        });


        return grid;

    }

    kGetElementiSelezionati(rows): Array<any>{

        if(!rows || rows.length == 0){
            return [];
        }

        return rows.filter(r=>r.Selected !== undefined && r.Selected !== null && r.Selected === true);
    }

    kCiSonoImpiantiSelezionati(rows): boolean{
        return this.kGetElementiSelezionati(rows).length > 0;
    }

    // Funzione per l'aggiornamento della superficie Totale
    RicalcolaSuperficieTotale(rows){
        let SupTot = 0;
        // se la checkbox è chekkata

        const filteredData = this.kGetElementiSelezionati(rows);

        $.each(filteredData, function (idx, dataItem) {

            SupTot += parseFloat(dataItem.Sup_Imp);
        });

        this.qdcservice.SuperficiForm.patchValue({
            Sup_Selezionata: this.funzionicomuniservice.roundNumber(SupTot,this.qdcservice.get4DecimalNumericSettings().decimals)
        });

        this.qdcservice.SuperficiForm.markAsTouched();

    }

    //@description
    //Aggiornamento della superficie Coinvolta
    // Se il flag_calcolo_miscele è false non riscattano tutti i ricalcoli dovuti al cambio della Superficie Trattata
    RicalcolaSuperficieCoinvolta(rows:any,flag_calcolo_miscele: boolean = true) {
        let SupTot = 0;
        // sel la checkbox è chekkata

        const filteredData = this.kGetElementiSelezionati(rows);

        $.each(filteredData, function (idx, dataItem) {
            SupTot += parseFloat(dataItem.Sup_Imp_help);
        });

        this.AutoCorrectNumericSource.next(false);

        this.qdcservice.SuperficiForm.patchValue({
            Sup_Trattata:this.funzionicomuniservice.roundNumber(SupTot,this.qdcservice.get4DecimalNumericSettings().decimals)
        });

        this.AutoCorrectNumericSource.next(true);

        this.qdcservice.SuperficiForm.markAsTouched();

        if(flag_calcolo_miscele){
            this.misceleservice.calcoloMiscele('Sup_Trattata',null,0, this.qdcservice.SuperficiForm.get("Sup_Trattata").value);
        }
    }

    private udmDoseSubject = new Subject<string>();
    udmDose$ = this.udmDoseSubject.asObservable();
    //aggiorna il valore della ddl sopra la griglia
    aggiornaUdmDoseIrrigazioneGridImpiantiComponent(udmDose: string){
        this.udmDoseSubject.next(udmDose);
    }
    aggiornaUdmDoseIrrigazione(udmDose: CodiceDescrizione){
        this.misceleservice.aggiornaUdmDoseIrrigazione(udmDose);
    }

    GridImpiantionCellClose(event: CellCloseEvent, inputElementRef: any, ConsigliClickedCell: any[], IrrigazioniUtilizzabiliClickedCell: any[]){

        // Validate that DataFineIrrigazione is not earlier than DataInizioIrrigazione
        if (event.column && (event.column.field === "DataFineIrrigazione" || event.column.field === "DataInizioIrrigazione")) {
            const start = event.dataItem?.DataInizioIrrigazione;
            const end = event.dataItem?.DataFineIrrigazione;
            const startDate = start instanceof Date ? start : (typeof start === 'string' && start ? new Date(start) : null);
            const endDate = end instanceof Date ? end : (typeof end === 'string' && end ? new Date(end) : null);

            if (startDate instanceof Date && !isNaN(startDate.getTime()) && endDate instanceof Date && !isNaN(endDate.getTime())) {
                if (endDate < startDate) {
                    // Block the cell close and show an error
                    try { event.preventDefault(); } catch (e) { /* ignore */ }

                    const msg = this.translocoService.translate('DataInizioNonDeveEssereMaggioreDiDataFine');

                    try {
                        if (this.giasMessageService && this.giasMessageService.errorMessage) {
                            this.giasMessageService.errorMessage(msg);
                        } else {
                            console.error(msg);
                        }
                    } catch (e) { console.error(e); }

                    return;
                }
            }
        }

        this.calcoloSuperfici.ricalcoloSuperfici_Edit_Grid_Impianti(event.dataItem,event);

        if (event.column.field === "FrequenzaIrrigazioneMedia"
            && (event.dataItem.FrequenzaIrrigazioneMedia == undefined || event.dataItem.FrequenzaIrrigazioneMedia == null)) {

            event.dataItem.FrequenzaIrrigazioneMedia = 1; //valore di default
        } else if (event.column.field === "Efficienza"
            && (event.dataItem.Efficienza == undefined || event.dataItem.Efficienza == null)) {

            event.dataItem.Efficienza = 100; //valore di default

        } else if (event.column.field === "DoseAcquaGiornaliera"
            && (event.dataItem.DoseAcquaGiornaliera == undefined || event.dataItem.DoseAcquaGiornaliera == null)) {

            event.dataItem.DoseAcquaGiornaliera = 0;

        } else if (event.column.field === "Portata"
            && (event.dataItem.Portata == undefined || event.dataItem.Portata == null)) {

            event.dataItem.Portata = 0;

        } else if (event.column.field === "OreIrrigazione"
            && (event.dataItem.OreIrrigazione == undefined || event.dataItem.OreIrrigazione == null)) {

            event.dataItem.OreIrrigazione = 0;
        }

        if(!event.formGroup.valid &&
            event.formGroup.get('Sup_Imp_help')) {
            this.tooltip.show(inputElementRef.numericInput.nativeElement);
        } else {
            this.tooltip.hide();
        }

        let rows = this.qdcservice.GridImpiantiPublicService.getValue().data.rows;

        this.RicalcolaSuperficieCoinvolta(rows);

        if(event.column.field == "Sup_Riduzione_BufferZone" || event.column.field == "Perc_Riduzione_Deriva" || event.column.field === "Sup_Imp_help"
            || event.column.field === "DataInizioIrrigazione" || event.column.field === "DataFineIrrigazione" || event.column.field === "FrequenzaIrrigazioneMedia"
            || event.column.field === "Efficienza" || event.column.field === "DoseAcquaGiornaliera" || event.column.field === "Portata" || event.column.field === "OreIrrigazione"){

            this.qdcservice.AggiornaFormArrayImpiantiSelezionati();
            this.misceleservice.updateGridIrrigationData(this.qdcservice.QdCForm, event.column.field);
        }

        this.AssociaValoriDDLAllaGrigliaImpianti(event,ConsigliClickedCell,IrrigazioniUtilizzabiliClickedCell);
    }

    EventiPostselectionChangeGridImpianti(rows: Array<any>,ImpostaDisciplinare: boolean = true){

        //Funzione richiamata sia run-time quando seleziono/deseleziono una riga sia quando
        //ottengo gli ImpiantiSelezionati dal formarray

        this.Abilita_Disabilita_Resto(rows);
        this.RicalcolaSuperficieTotale(rows);
        this.RicalcolaSuperficieCoinvolta(rows);

        let Operazioni: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").getRawValue();

        //Imposto la ddl del Disciplinare solamente se è visibile
        if(ImpostaDisciplinare &&
            Operazioni &&
            Operazioni.findIndex(o=>this.qdcservice.MostraDisciplinare(o,this.qdcservice.Sezioni_ProdottoFormArray)) > -1){

            if(!this.testataservice.Array_Disciplinari){
                this.testataservice.getArray_Disciplinari().then(d=>{
                    this.qdcservice.ImpostaDisciplinare_Dagli_ImpiantiSelezionati(this.testataservice.Array_Disciplinari).then(r=>{
                        if(r){
                            this.qdcservice.GestisciColoreRigheGridImpianti();
                            this.qdcservice.AvvisoImpianti(rows);
                            this.qdcservice.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti_Tutti();
                        }

                    });
                });
            }else{
                this.qdcservice.ImpostaDisciplinare_Dagli_ImpiantiSelezionati(this.testataservice.Array_Disciplinari).then(r=>{
                    if(r){
                        this.qdcservice.GestisciColoreRigheGridImpianti();
                        this.qdcservice.AvvisoImpianti(rows);
                        this.qdcservice.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti_Tutti();
                    }

                });
            }
        }

        this.misceleservice.updateGridIrrigationData(this.qdcservice.QdCForm);

        this.GestisciQuantitaSuImpianti_InstallazioneTrappoleCatturaDiMassa(Operazioni);
    }



    Abilita_Disabilita_Resto(rows){

        let abilita = true;

        if(this.kCiSonoImpiantiSelezionati(rows) || this.qdcservice.flag_Reinnesco)
          abilita = false;

        this.Abilita_Disabilita_DDL('Specie', abilita);

        this.Abilita_Disabilita_DDL('Centro_Aziendale', abilita);

        this.Abilita_Disabilita_DDL('Campo', abilita);

        this.Abilita_Disabilita_DDL('Operatore_Visita', abilita);

        this.Abilita_Disabilita_DDL('Azienda_Visita', abilita);

        this.Abilita_Disabilita_DDL('Visualizza_Specie', abilita);
    }

    Abilita_Disabilita_DDL(nomeddl: string,abilita: boolean){

        if(this.qdcservice.TestataForm.get(nomeddl)){
            if (!abilita) {
                this.qdcservice.TestataForm.get(nomeddl).disable({emitEvent: false});
            } else {
                this.qdcservice.TestataForm.get(nomeddl).enable({emitEvent: false});
            }
        }

        if(this.qdcservice.TestataVisitaForm.get(nomeddl)){
            if (!abilita) {
                this.qdcservice.TestataVisitaForm.get(nomeddl).disable({emitEvent: false});
            } else {
                this.qdcservice.TestataVisitaForm.get(nomeddl).enable({emitEvent: false});
            }
        }

    }

    //replica una parte dell' inizializzazioneKendo_BufferZone della Trattamenti_2
    AggiornaGridImpiantiDopoCambioPercentualeAbbatimento(){

        let percAbb:number = this.qdcservice.OttieniPercentualeAbbattimentoDiserboDisseccamento(null,null);

        let datiGriglia = this.qdcservice.GridImpiantiPublicService.getValue().data.rows;

        let ricalcoliEffettuati = false;

        // ******************************************************************************************************
        // CALCOLI PER PERCENTUALE ABBATTIMENTO
        // ******************************************************************************************************

        if (percAbb > 0 && percAbb < 100)
        {

            for (var i = 0; i < datiGriglia.length; i++) {

                if(datiGriglia[i].Selected){
                    datiGriglia[i].Sup_Imp_help = this.calcoloSuperfici.calcoloPercentualeAbbatimento(datiGriglia[i].Sup_Imp, datiGriglia[i].Sup_Imp_help);
                }

            }

            ricalcoliEffettuati = true;
        }


        if(ricalcoliEffettuati){
            this.qdcservice.AggiornaFormArrayImpiantiSelezionati();

            this.RicalcolaSuperficieCoinvolta(datiGriglia);
        }

    }

    //Richiamo questa funzione al cambio del Prodotto per poter ricalcolare correttamente le buffer in base alla nuova
    //buffer minima del prodotto (se la buffer minima è minore di quella già presente nella grid considero quello più alta)
    AggiornaGridImpiantiXBufferzone(){

        let ricalcoliEffettuati: boolean = false;

        let ImpiantiSelezionati: Array<GridImpiantoSelezionatoModel> = JSON.parse(JSON.stringify(this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue()));

        if(ImpiantiSelezionati && ImpiantiSelezionati.length > 0){
            for(let i of ImpiantiSelezionati){
                if(this.calcoloSuperfici.ricalcoloSuperfici(i)){
                    ricalcoliEffettuati = true;
                }
            }
        }


        if(ricalcoliEffettuati){

            this.qdcservice.RicaricaGridImpianti();

            let rows = this.qdcservice.GridImpiantiPublicService.getValue().data.rows;

            this.RicalcolaSuperficieCoinvolta(rows);
        }

    }

    AssociaValoriDDLAllaGrigliaImpianti(event: CellCloseEvent, ConsigliClickedCell: any[],IrrigazioniUtilizzabiliClickedCell: any[]) {

      let index_Consiglio: number = -1;

      let index_irrigazione_utilizzata: number = -1;

      if (event) {

        let row: any = event.dataItem;

        let column: ColumnComponent = event.column;

        if (column.field === "Consiglio_Codice" && ConsigliClickedCell && ConsigliClickedCell.length > 0) {

          index_Consiglio = ConsigliClickedCell.findIndex(c => c.Consiglio_Codice === row.Consiglio_Codice);

          if (index_Consiglio > -1) {
            row.Consiglio_Descrizione = ConsigliClickedCell[index_Consiglio].Consiglio_Descrizione;
            row.DataConsiglio = ConsigliClickedCell[index_Consiglio].DataConsiglio;
            row.DoseAcquaConsiglio = ConsigliClickedCell[index_Consiglio].DoseAcquaConsiglio;
            row.UdmConsiglio = ConsigliClickedCell[index_Consiglio].UdmConsiglio;
            row.minDataTurno = ConsigliClickedCell[index_Consiglio].minDataTurno;
            row.maxDataTurno = ConsigliClickedCell[index_Consiglio].maxDataTurno;

            if (row.Consiglio_Codice === 0) { //nessun consiglio
              let data: Date = this.qdcservice.TestataForm.get("Data").value;
              row.DataInizioIrrigazione = data;
              row.DataFineIrrigazione = data;
              row.FrequenzaIrrigazioneMedia = 1;
              row.DoseAcquaGiornaliera = 0;
            } else { //consiglio diverso da 'nessun consiglio'
              if (ConsigliClickedCell[index_Consiglio].minDataTurno && ConsigliClickedCell[index_Consiglio].maxDataTurno) { //consiglio con i turni
                row.FrequenzaIrrigazioneMedia = 1;
                row.DataInizioIrrigazione = ConsigliClickedCell[index_Consiglio].minDataTurno;
                row.DataFineIrrigazione = ConsigliClickedCell[index_Consiglio].maxDataTurno;
                const daysDiff = this.irrigazioneAcquaService.getDaysDifference(row.DataInizioIrrigazione, row.DataFineIrrigazione);
                const daysDiffWithFreq = Math.max(Math.floor(daysDiff / row.FrequenzaIrrigazioneMedia), 1);

                if (row.UdmDose === row.UdmConsiglio.simbolo) {
                  row.DoseAcquaGiornaliera = row.DoseAcquaConsiglio / daysDiffWithFreq;
                        } else if (row.UdmDose === SIMBOLO_M3_HA && row.UdmConsiglio.simbolo === SIMBOLO_MM){
                  row.DoseAcquaGiornaliera = row.DoseAcquaConsiglio / daysDiffWithFreq * 10;
                        } else if (row.UdmDose === SIMBOLO_MM && row.UdmConsiglio.simbolo === SIMBOLO_M3_HA){
                  row.DoseAcquaGiornaliera = row.DoseAcquaConsiglio / daysDiffWithFreq / 10;
                }
              } else { //consiglio senza i turni
                if (row.UdmDose === row.UdmConsiglio.simbolo) {
                  row.DoseAcquaGiornaliera = row.DoseAcquaConsiglio;
                        } else if (row.UdmDose === SIMBOLO_M3_HA && row.UdmConsiglio.simbolo === SIMBOLO_MM){
                  row.DoseAcquaGiornaliera = row.DoseAcquaConsiglio * 10;
                        } else if (row.UdmDose === SIMBOLO_MM && row.UdmConsiglio.simbolo === SIMBOLO_M3_HA){
                  row.DoseAcquaGiornaliera = row.DoseAcquaConsiglio / 10;
                }
              }
            }
          }

        } else if (column.field === "IrrigazioneUtilizzata_Codice" && IrrigazioniUtilizzabiliClickedCell && IrrigazioniUtilizzabiliClickedCell.length > 0) {

          index_irrigazione_utilizzata = IrrigazioniUtilizzabiliClickedCell.findIndex(c => c.IrrigazioneUtilizzata_Codice === row.IrrigazioneUtilizzata_Codice);

          if (index_irrigazione_utilizzata > -1) {

            let stessaMacchinaIrrigazione: boolean = false;
            if (!row.macchinaIrrigazione && !IrrigazioniUtilizzabiliClickedCell[index_irrigazione_utilizzata].macchinaIrrigazione) { //se entrambe sono null
              stessaMacchinaIrrigazione = true;
            } else if (!row.macchinaIrrigazione || !IrrigazioniUtilizzabiliClickedCell[index_irrigazione_utilizzata].macchinaIrrigazione) {
              stessaMacchinaIrrigazione = false;
            } else {
              stessaMacchinaIrrigazione = row.macchinaIrrigazione.codice === IrrigazioniUtilizzabiliClickedCell[index_irrigazione_utilizzata].macchinaIrrigazione.codice;
            }

            let stessoTipoIrrigazione: boolean = false;
            if (!row.tipoIrrigazione && !IrrigazioniUtilizzabiliClickedCell[index_irrigazione_utilizzata].tipoIrrigazione) { //se entrambe sono null
              stessoTipoIrrigazione = true;
            } else if (!row.tipoIrrigazione || !IrrigazioniUtilizzabiliClickedCell[index_irrigazione_utilizzata].tipoIrrigazione) {
              stessoTipoIrrigazione = false;
            } else {
              stessoTipoIrrigazione = row.tipoIrrigazione.codice === IrrigazioniUtilizzabiliClickedCell[index_irrigazione_utilizzata].tipoIrrigazione.codice;
            }

            if (!stessoTipoIrrigazione || !stessaMacchinaIrrigazione) { //aggiorno solo se è cambiato il tipo di irrigazione o la macchina di irrigazione
              row.IrrigazioneUtilizzata_Descrizione = IrrigazioniUtilizzabiliClickedCell[index_irrigazione_utilizzata].IrrigazioneUtilizzata_Descrizione;
              row.tipoIrrigazione = IrrigazioniUtilizzabiliClickedCell[index_irrigazione_utilizzata].tipoIrrigazione;
              row.macchinaIrrigazione = IrrigazioniUtilizzabiliClickedCell[index_irrigazione_utilizzata].macchinaIrrigazione;
              row.Efficienza = IrrigazioniUtilizzabiliClickedCell[index_irrigazione_utilizzata].Efficienza;
              row.Portata = IrrigazioniUtilizzabiliClickedCell[index_irrigazione_utilizzata].Portata;

              let operazioni: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").getRawValue();
              const fertirrigazione = operazioni && operazioni.length > 0 && operazioni.findIndex(o => +o.primaryKey.codice === enum_LAVCOD.FERTIRRIGAZIONE) > -1;
              const irrigazione = operazioni && operazioni.length > 0 && operazioni.findIndex(o => +o.primaryKey.codice === enum_LAVCOD.IRRIGAZIONE) > -1;

              if (fertirrigazione) {
                this.irrigazioneAcquaService.calcolaOreFertirrigazione(row);
                this.irrigazioneAcquaService.calcolaValoriAssorbitiIrrigazione(row, row.Efficienza);
              } else if (irrigazione) {
                this.irrigazioneAcquaService.gestisciCambioPortataIrrigazione(row, row.Sup_Imp_help, row.DataInizioIrrigazione, row.DataFineIrrigazione, row.Efficienza, row.Portata);
              }

              if (row.OreIrrigazione > 24) {
                let listErroriGias = [{
                  severity: ErroreGias_Severity.Bloccante,
                  messaggio: this.translocoService.translate("qdc.OreIrrigazioneMaggioriDi24"),
                } as ErroreGias];

                let obj = this.qdcservice.gestisci_ErroriGias(listErroriGias, true, true).then();
              }
            }
          }

        }
      }

      if (index_Consiglio > -1 || index_irrigazione_utilizzata > -1) {
        this.qdcservice.AggiornaFormArrayImpiantiSelezionati();
      }
    }

    /*
    * @description Aggiorno il valore di Quantità su Impianti per l'operazione di Installazione Trappole Cattura di Massa
    * Ogni volta che modifico gli impianti selezionati
    * */
    private GestisciQuantitaSuImpianti_InstallazioneTrappoleCatturaDiMassa(Operazioni: Array<Lavorazione>): void{

      //Aggiungo tante righe quanti sono gli impianti selezionati
      if(Operazioni &&
        Operazioni.findIndex((o: Lavorazione)=>this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(+ o.primaryKey.codice)) > -1){

        let Sezioni_Prodotto: Array<Sezione_Prodotto> = this.qdcservice.Sezioni_ProdottoFormArray.getRawValue();

        //Nel caso del Reinnesco evito che scatti l'emitEvent
        let emitEvent: boolean = !this.qdcservice.flag_Reinnesco;

        if(Sezioni_Prodotto){

          let index = Sezioni_Prodotto.findIndex(f=> this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(+ f.Operazione.primaryKey.codice));

          if(index > -1){

            if(Sezioni_Prodotto[index].DosiProdotti && Sezioni_Prodotto[index].DosiProdotti.length > 0){

              let impianti_selezionati: Array<GridImpiantoSelezionatoModel> = this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue();

              Sezioni_Prodotto[index].DosiProdotti.forEach((d: Dettaglio_Formulato,index_d: number)=>{

                if(impianti_selezionati && impianti_selezionati.length > 0){

                  if(d.QuantitaSuImpianti && d.QuantitaSuImpianti.length > 0){

                    let list_index_qt_da_scartare: number[] = [];

                    d.QuantitaSuImpianti.forEach((qt: QuantitaSuImpianto,index_qt: number)=>{
                      if(qt && qt.esercizioCDC && qt.esercizioCDC.esercizio){

                        let esercizio =  qt.esercizioCDC.esercizio;

                        if(impianti_selezionati.findIndex(imp=>imp.PIVA === esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva &&
                          imp.SA_COD === esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice &&
                          imp.APPEZZA === esercizio.impiantoPK.appezzamentoPK.codice &&
                          imp.ID_REG === esercizio.impiantoPK.codice &&
                          imp.Progetto_Cod === esercizio.codice) === -1){

                          list_index_qt_da_scartare.push(index_qt);

                        }
                      }
                    });

                    if(list_index_qt_da_scartare && list_index_qt_da_scartare.length > 0){
                      let new_quantitaSuImpianti = d.QuantitaSuImpianti.filter((q,i)=> !list_index_qt_da_scartare.includes(i));

                      d.QuantitaSuImpianti = new_quantitaSuImpianti;

                      this.qdcservice.AggiornaFormArrayGridDosiProdotti(d,index_d,Sezioni_Prodotto[index].Operazione,enum_TipoOperazioneDB.Modifica,this.qdcservice.DosiProdottiFormArray(null,Sezioni_Prodotto[index].Operazione),emitEvent);

                      this.qdcservice.RicaricaTutteGridProdottixImpianti();
                    }

                    //Aggiungo gli impianti selezionati mancanti alla griglia
                    if(d.QuantitaSuImpianti.length < impianti_selezionati.length){
                      let impianti_da_aggiungere = impianti_selezionati.filter(gridimpianto=>d.QuantitaSuImpianti.findIndex(imp=>imp.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva === gridimpianto.PIVA &&
                        imp.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice === gridimpianto.SA_COD &&
                        imp.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice === gridimpianto.APPEZZA &&
                        imp.esercizioCDC.esercizio.impiantoPK.codice === gridimpianto.ID_REG &&
                        imp.esercizioCDC.esercizio.codice === gridimpianto.Progetto_Cod) === -1);

                      if(impianti_da_aggiungere && impianti_da_aggiungere.length > 0){

                        impianti_da_aggiungere.forEach(imp=>{
                          d.QuantitaSuImpianti.push({
                            esercizioCDC: this.qdcservice.GetEsercizioCDCModel(imp),
                            Qta: 0,
                            Magazzino: null,
                            Lotto: ""
                          });
                        });

                        this.qdcservice.AggiornaFormArrayGridDosiProdotti(d,index_d,Sezioni_Prodotto[index].Operazione,enum_TipoOperazioneDB.Modifica,this.qdcservice.DosiProdottiFormArray(null,Sezioni_Prodotto[index].Operazione),emitEvent);

                        this.qdcservice.RicaricaTutteGridProdottixImpianti();
                      }

                    }
                  }else{
                    d.QuantitaSuImpianti = [];

                    impianti_selezionati.forEach((q,i)=> {

                      let qt = new QuantitaSuImpianto();

                      qt.esercizioCDC = this.qdcservice.GetEsercizioCDCModel(q);

                      d.QuantitaSuImpianti.push(qt);
                    });

                    this.qdcservice.AggiornaFormArrayGridDosiProdotti(d,index_d,Sezioni_Prodotto[index].Operazione,enum_TipoOperazioneDB.Modifica,this.qdcservice.DosiProdottiFormArray(null,Sezioni_Prodotto[index].Operazione),emitEvent);

                    this.qdcservice.RicaricaTutteGridProdottixImpianti();
                  }

                }else{

                  //Non ho alcun impianto selezionato imposto array vuoto la QuantitaSuImpianto

                  d.QuantitaSuImpianti = [];

                  this.qdcservice.AggiornaFormArrayGridDosiProdotti(d,index_d,Sezioni_Prodotto[index].Operazione,enum_TipoOperazioneDB.Modifica,this.qdcservice.DosiProdottiFormArray(null,Sezioni_Prodotto[index].Operazione),emitEvent);

                  this.qdcservice.RicaricaTutteGridProdottixImpianti();

                }
              });
            }
          }
        }
      }
    }
}
