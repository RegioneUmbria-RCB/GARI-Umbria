import { ChangeDetectorRef, Component, Inject, OnDestroy, OnInit, Optional, Renderer2, ViewChild } from '@angular/core';
import { FormArray, FormGroup, FormGroupDirective } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { FERTILIZZANTI, FORMULATI, SEMENTI } from 'app/Model/CostantiPersonalizzate';
import { enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import {GRID_HTTP_TOKEN, KendoGridRow} from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { generateGridProviders } from 'gias-kendo-grid';
import { Subscription } from 'rxjs';
import { CalcoloSuperficiService } from '../../service/calcolo-superfici.service';
import { GridDosiProdottiControlliService } from '../../service/grid-dosi-prodotti/grid-dosi-prodotti-controlli.service';
import { GridDosiProdottiHttpService } from '../../service/grid-dosi-prodotti/grid-dosi-prodotti-http.service';
import { GridDosiProdottiService } from '../../service/grid-dosi-prodotti/grid-dosi-prodotti.service';
import { QdCProdottiService } from '../../service/prodotti.service';
import { QdCDettagliFertilizzantiService } from '../../service/prodotti/dettagli-fertilizzanti.service';
// able-next-line max-len
import { QdCService } from '../../service/qdc.service';
import {QdCDettagliFormulatiService} from "../../service/prodotti/dettagli-formulati.service";
import {QdCDettagliSementiService} from "../../service/prodotti/dettagli-sementi.service";
import {QdCFormulatiService} from "../../service/prodotti/formulati.service";
import {QdCFertilizzantiService} from "../../service/prodotti/fertilizzanti.service";

@Component({
    standalone: false,
    selector: 'app-grid-dosi-prodotti',
    templateUrl: './grid-dosi-prodotti.component.html',
    styleUrls: ['./grid-dosi-prodotti.component.scss'],
    providers: [
        ...generateGridProviders(GridDosiProdottiHttpService, GridDosiProdottiComponent)
    ]
})
export class GridDosiProdottiComponent implements OnInit,OnDestroy {

    //Utilizzo questa variabile per nascondere/mostrare la griglia perchè in alcuni casi vengono nascoste alcune colonne
    public mostraGridDosiProdotti: boolean = true;

    @ViewChild('GridDosiProdotti') GridDosiProdottiElRef: GiasKendoGridComponent;

    Subs: Subscription= new Subscription();

    ProdottiForm: FormGroup;

    public Categoria_Magazzino: number;

    public Lav_Cod: number;

    Operazione: Lavorazione;

    constructor(
        public parent: FormGroupDirective,
        private qdcservice: QdCService,
        private gridpublicService: GridPublicService,
        private translocoService: TranslocoService,
        private changeDetector: ChangeDetectorRef,
        private griddosiprodottiservice: GridDosiProdottiService,
        private prodottiservice: QdCProdottiService,
        private calcoloSuperfici: CalcoloSuperficiService,
        private renderer: Renderer2,
        @Optional() private qdcformulatiservice: QdCFormulatiService,
        @Optional() private qdcfertilizzantiservice: QdCFertilizzantiService,
        @Optional() private qdcdettagliformulatiservice: QdCDettagliFormulatiService,
        @Optional() private qdcdettaglifertilizzantiservice: QdCDettagliFertilizzantiService,
        @Optional() private qdcdettaglisementiservice: QdCDettagliSementiService,
        @Inject(GRID_HTTP_TOKEN) private gridhttpService: GridDosiProdottiHttpService,
        private gridcontrolliservice: GridDosiProdottiControlliService
    ) {  }

    ngOnInit(): void {

        this.prodottiservice.GridDosiProdottiHttpService = this.gridhttpService;
        this.prodottiservice.GridDosiProdottiPublicService = this.gridpublicService;

        this.ProdottiForm = <FormGroup> this.parent.form;

        this.Operazione = this.ProdottiForm.get('Operazione').value;

        this.Lav_Cod = + this.Operazione.primaryKey.codice;

        this.Categoria_Magazzino = this.ProdottiForm.get('Categoria_Magazzino').value;

        if(this.Categoria_Magazzino === FORMULATI)
            this.qdcformulatiservice.AbilitaDisabilitaEpocaDPI(this.ProdottiForm);

        this.Subs.add(this.gridpublicService.changeDetected.subscribe((event: any) => {
            if(this.Categoria_Magazzino === FORMULATI)
                this.qdcformulatiservice.AbilitaDisabilitaEpocaDPI(this.ProdottiForm);
        }));

        this.Subs.add(this.gridhttpService.EditCompleto.subscribe(value => {
            if(value)
                this.onEditCompletoGridDosiProdotti(value.Dataitem,value.RowIndex);
        }));

        this.Subs.add(this.prodottiservice.InserisciDoseProdotto.subscribe(value => {
            if(value)
                this.InserisciDoseProdotto();
        }));

        // Necessario per aggiornare i parametri della griglia dopo il ricalcolo delle dosi
        this.Subs.add(this.prodottiservice.ProdottiForm.get('DosiProdotti').valueChanges.subscribe((v) => {
            this.gridpublicService.refresh(true);
        }));

    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    /*mostraGridDosiProdotti(){

        let mostra=false;

        let DosiProdotti = (this.ProdottiForm.get("DosiProdotti") as FormArray).getRawValue();

        if(DosiProdotti &&
            DosiProdotti.length > 0 ){
            mostra = true;
        }

        return mostra;

    }*/

    async InserisciDoseProdotto(){

        let DosiProdotti = (this.ProdottiForm.get("DosiProdotti") as FormArray).getRawValue();
        let gridDosiProdottiRows = [];
        let dosi_etichetta = null;
        let epocadpi = null;
        let epocafertilizzazione = null;
        let utilizza_direttiva_nitrati = false;
        let direttiva_nitrati = null;
        let opzioni_semina = null;
        let modalita_applicazione = null;
        let ripartizione_trappole = null;


        if(this.gridpublicService.getValue()?.data)
            gridDosiProdottiRows = this.gridpublicService.getValue().data.rows;

        let Tipo_Operazione_DB = enum_TipoOperazioneDB.Scrittura;

        if(this.gridhttpService.ID_RigaDoseProdottoDaModificare > -1)
            Tipo_Operazione_DB = enum_TipoOperazioneDB.Modifica;

        //Rileggo le dosi per avere i dati aggiornati
        if(this.Categoria_Magazzino === FORMULATI){
            dosi_etichetta = await this.qdcdettagliformulatiservice.getArray_Dosi(false)
            epocadpi = this.ProdottiForm.get("EpocaDPI").value;
            modalita_applicazione = this.ProdottiForm.get("Modalita_Applicazione").value;
            ripartizione_trappole = this.ProdottiForm.get("Ripartizione_Trappole").value;
        }

        if(this.Categoria_Magazzino === FERTILIZZANTI){
            epocafertilizzazione = this.ProdottiForm.get("EpocaFertilizzazione").value;
            utilizza_direttiva_nitrati = this.ProdottiForm.get('Utilizza_Direttiva_Nitrati').value;
            direttiva_nitrati = this.ProdottiForm.get('Direttiva_Nitrati').value;
            modalita_applicazione = this.ProdottiForm.get("Modalita_Applicazione").value;
        }

        if(this.Categoria_Magazzino === SEMENTI){
            opzioni_semina = this.ProdottiForm.get('Opzioni_Semina').value;
        }


        let row: FormGroup = this.griddosiprodottiservice.getRigaGridDosiProdotti(
            DosiProdotti,
            this.gridhttpService.ID_RigaDoseProdottoDaModificare,
            this.ProdottiForm,
            null,
            null,
            this.Operazione,
            this.qdcservice.TestataForm.get("Data").value,
            this.ProdottiForm.get('flagTipoDose').value,
            this.ProdottiForm.get('flagDoseQuantitaTotale').value,
            this.qdcservice.AcquaForm.get('Dose_Acqua').value,
            dosi_etichetta,
            epocadpi,
            epocafertilizzazione,
            utilizza_direttiva_nitrati,
            direttiva_nitrati,
            opzioni_semina,
            modalita_applicazione,
            null,
            ripartizione_trappole,
            false,
            false,
            false
        );

        this.gridcontrolliservice.ControllaSeInserireDoseProdotto(
            gridDosiProdottiRows,
            row.value,
            this.ProdottiForm,
            Tipo_Operazione_DB,
            this.gridhttpService.ID_RigaDoseProdottoDaModificare,
            this.qdcdettagliformulatiservice?.doseConsentitaDiserbo,
            this.qdcdettagliformulatiservice?.Array_Soglie_Avversita,
            this.qdcdettagliformulatiservice?.Array_Dosi
        ).then(listerroriGias => {

            if(listerroriGias.length === 0){
                this.Continua_InserimentoDoseProdotto(row);
            }

        });

    }

    Continua_InserimentoDoseProdotto(row: FormGroup){

        this.mostraGridDosiProdotti = false;
        this.changeDetector.detectChanges();

        row.patchValue({
            Riga_Salvata: true
        },{emitEvent: false});

        if(this.gridhttpService.ID_RigaDoseProdottoDaModificare === -1) {
            this.qdcservice.AggiornaFormArrayGridDosiProdotti(row.value,-1,this.Operazione,enum_TipoOperazioneDB.Scrittura,null);
            this.gridpublicService.refresh(true);
            this.calcoloSuperfici.ricalcoloSuperfici_daDosi();
        } else {
            this.ModificaRigaGridDosiProdotti(row.value);
        }

        this.mostraGridDosiProdotti = true;

        //Reimposto il nome del bottone
        this.prodottiservice.btn_Inserisci_Modifica_Prodotto_Text = this.translocoService.translate('qdc.InserisciProdottoInMiscela');

        //Resetta tutti i controlli tranne la ddl di Ricerca dei Prodotti
        switch(this.Categoria_Magazzino){
            case FERTILIZZANTI:
                this.qdcdettaglifertilizzantiservice.clearSezioneProdottiFertilizzantiForm(false,true,false);
                break;
            case FORMULATI:
                this.qdcdettagliformulatiservice.clearSezioneProdottiFormulatiForm(false,true,false,true,true);
                break;
            case SEMENTI:
                this.qdcdettaglisementiservice.clearSezioneProdottiSementiForm(false,true);
                break;
        }

        if(this.Categoria_Magazzino === FORMULATI)
            this.qdcformulatiservice.AbilitaDisabilitaEpocaDPI(this.ProdottiForm);

        //Gestisco i controlli in base all'impostazione utente delle giacenze e dei lotti
        this.prodottiservice.Gestisci_Controlli_Lotti_Giacenze();

        this.prodottiservice.Gestisci_Controlli_Lotti_Giacenze_Innesco();

        this.changeDetector.detectChanges();

        // Per fare lo scroll della pagina in basso verso la grid delle dosi dei Prodotti
        //Concateno anche il codice della operazione per rendere i vari id univoci
        const element = document.querySelector('#divGridDosiProdotti_'+this.Lav_Cod);

        if (element)
            element.scrollIntoView();

        if(this.gridhttpService.ID_RigaDoseProdottoDaModificare > -1) {
            let DosiProdotti = (this.ProdottiForm.get("DosiProdotti") as FormArray).getRawValue();
            let index = DosiProdotti.findIndex((r: any) => r.DosiProdottiGridrowId === this.gridhttpService.ID_RigaDoseProdottoDaModificare);

            this.GestisciColoreRighe(index,false);
        }

        this.gridhttpService.EditCompleto.next(null);

    }

    ModificaRigaGridDosiProdotti(rowDaModificare){
        let DosiProdotti = (this.ProdottiForm.get("DosiProdotti") as FormArray).getRawValue();

        let index = DosiProdotti.findIndex((r: any) => r.DosiProdottiGridrowId === this.gridhttpService.ID_RigaDoseProdottoDaModificare);

        this.qdcservice.AggiornaFormArrayGridDosiProdotti(rowDaModificare,index,this.Operazione,enum_TipoOperazioneDB.Modifica,null);

        this.gridpublicService.refresh(true);

        this.gridhttpService.ID_RigaDoseProdottoDaModificare = -1;
    }

    //Edit Completo della riga
    onEditCompletoGridDosiProdotti(dataItem: any, rowIndex: number){

        this.GestisciColoreRighe(rowIndex,true);

        this.prodottiservice.btn_Inserisci_Modifica_Prodotto_Text = this.translocoService.translate('qdc.ModificaProdottoInMiscela');

        this.gridhttpService.ID_RigaDoseProdottoDaModificare = dataItem.DosiProdottiGridrowId;

        let index = (<FormArray> this.ProdottiForm.get("DosiProdotti")).getRawValue().findIndex(d=>d.DosiProdottiGridrowId === this.gridhttpService.ID_RigaDoseProdottoDaModificare);

        (<FormArray> this.ProdottiForm.get("DosiProdotti")).controls[index].patchValue({
                Riga_Salvata: false
        },{emitEvent: false});

        switch(this.Categoria_Magazzino){
            case FORMULATI:
                this.qdcdettagliformulatiservice.setArrayDDLSezioneProdottiFormulatiForm(dataItem,true);
                break;
            case FERTILIZZANTI:
                this.qdcdettaglifertilizzantiservice.setArrayDDLSezioneProdottiFertilizzantiForm(dataItem);
                break;
            case SEMENTI:
                this.qdcdettaglisementiservice.setArrayDDLSezioneProdottiSementiForm(dataItem);
                break;
        }

        this.ProdottiForm.patchValue({
            Prodotto: dataItem.Prodotto,
            Magazzino_del_Prodotto_Selezionato: dataItem.Magazzino_del_Prodotto_Selezionato,
            Lotto: dataItem.Lotto,
            Dose_Ha: dataItem.Dose_Ha,
            Dose_Hl: dataItem.Dose_Hl,
            DoseTot_Ha: dataItem.DoseTot_Ha,
            flagTipoDose: dataItem.flagTipoDose,
            flagDoseQuantitaTotale: dataItem.flagDoseQuantitaTotale
        });

        this.prodottiservice.Aggiorna_UdM(dataItem.UdM);

        switch(this.Categoria_Magazzino){
            case FORMULATI:
                this.ProdottiForm.patchValue({
                    Avversita: dataItem.Avversita,
                    Soglia_Avversita: dataItem.Soglia_Avversita,
                    Dose_Etichetta: dataItem.Dose_Etichetta
                });
                break;
            case FERTILIZZANTI:
                this.ProdottiForm.patchValue({
                    Efficienza: dataItem.Efficienza,
                    N: dataItem.N,
                    N_Utile: dataItem.N_Utile,
                    P: dataItem.P,
                    K: dataItem.K,
                    Cu: dataItem.Cu
                });
                break;
            case SEMENTI:
                break;
        }

        //Gestisco i controlli in base all'impostazione utente delle giacenze e dei lotti
        this.prodottiservice.Gestisci_Controlli_Lotti_Giacenze();

        this.prodottiservice.Gestisci_Controlli_Lotti_Giacenze_Innesco();

        // Per fare lo scroll della pagina in alto verso la ddl del Prodotto
        //Concateno anche il codice della operazione per rendere i vari id univoci
        const element = document.querySelector('#RicercaProdotto_'+this.Lav_Cod);

        if (element)
            element.scrollIntoView();
    }

    //Permette di colorare di giallo la riga della griglia in cui sono in edit
    GestisciColoreRighe(rowIndex: number, coloraRiga: boolean){

        let elementRef: Array<any> = this.GridDosiProdottiElRef.gridElRef.nativeElement.querySelectorAll('tbody tr');

        elementRef.forEach(e=>{
            this.renderer.removeClass(e, 'Modifica_Dose_Prodotti');
        });

        if(coloraRiga)
            this.renderer.addClass(elementRef[rowIndex], 'Modifica_Dose_Prodotti');

    }

    public MostraBtn_Dose_Consigliata(){
        let mostra = false;

        if(this.Categoria_Magazzino === FERTILIZZANTI)
            mostra = true;

        return mostra;
    }

    public Disabilita_Btn_Dose_Consigliata(row: KendoGridRow,inLineModificaIsDisabled: boolean){

        let disabilita = false;

        if(this.gridhttpService.DisabilitaBottoni(null) || inLineModificaIsDisabled === true)
            disabilita = true;

        return disabilita;
    }

}
