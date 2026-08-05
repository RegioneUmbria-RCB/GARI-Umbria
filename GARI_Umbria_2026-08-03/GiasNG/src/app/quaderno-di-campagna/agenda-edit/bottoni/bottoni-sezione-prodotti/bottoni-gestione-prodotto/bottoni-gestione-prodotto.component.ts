import {Component, OnInit, Optional} from '@angular/core';
import {FormArray, FormGroup, FormGroupDirective} from "@angular/forms";
import {QdCService} from "../../../service/qdc.service";
import {GridDosiProdottiService} from "../../../service/grid-dosi-prodotti/grid-dosi-prodotti.service";
import {
    GridDosiProdottiControlliService
} from "../../../service/grid-dosi-prodotti/grid-dosi-prodotti-controlli.service";
import {enum_TipoOperazioneDB} from "../../../../../Model/TipiEnumerativi";
import {Lavorazione} from "../../../../../Model/attivita/Lavorazione";
import {FERTILIZZANTI, FORMULATI, INSETTI, SEMENTI} from '../../../../../Model/CostantiPersonalizzate';
import {Epoca} from "../../../../../Model/metaschema/Epoca";
import {
    MultiColumnComboboxDose_Etichetta,
    Parametri_Aggiuntivi_Attivita
} from "../../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {Disciplinare} from "../../../../../Model/metaschema/Disciplinari";
import {BaseCodeDescr} from "../../../../../Model/baseClass/baseCodeDescr";
import {QdCDettagliFormulatiService} from "../../../service/prodotti/dettagli-formulati.service";
import {QdCFormulatiService} from "../../../service/prodotti/formulati.service";
import {QdCProdottiService} from "../../../service/prodotti.service";
import {QdCDettagliFertilizzantiService} from "../../../service/prodotti/dettagli-fertilizzanti.service";
import {QdCDettagliSementiService} from "../../../service/prodotti/dettagli-sementi.service";
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity} from "../../../../../Service/master.service";
import {TranslocoService} from "@jsverse/transloco";
import {Blocco, Tipo_Blocco} from "../../../../../Model/attivita/Blocco";

@Component({
  standalone: false,
  selector: 'app-bottoni-gestione-prodotto',
  templateUrl: './bottoni-gestione-prodotto.component.html',
  styleUrls: ['./bottoni-gestione-prodotto.component.css']
})
export class BottoniGestioneProdottoComponent implements OnInit {

    ProdottiForm: FormGroup;

    Nuova_Riga: boolean = true;

    OldProdottiFormValue = null;

    Operazione: Lavorazione = null;

    Categoria_Magazzino: number = 0;

  constructor(public parent: FormGroupDirective,
              public qdcservice: QdCService,
              private gridDosiProdottiService: GridDosiProdottiService,
              private gridcontrolliservice: GridDosiProdottiControlliService,
              @Optional() private qdcdettagliformulatiservice: QdCDettagliFormulatiService,
              @Optional() private qdcformulatiservice: QdCFormulatiService,
              @Optional() private qdcdettaglifertilizzantiservice: QdCDettagliFertilizzantiService,
              @Optional() private qdcdettaglisementiservice: QdCDettagliSementiService,
              private prodottiservice: QdCProdottiService,
              private translocoService: TranslocoService) { }

  ngOnInit(): void {
      this.ProdottiForm = <FormGroup>this.parent.form;

      this.Operazione = this.ProdottiForm.get("Operazione").value;

      this.Categoria_Magazzino = this.ProdottiForm.get("Categoria_Magazzino").value;

      this.OldProdottiFormValue = this.ProdottiForm.getRawValue();

      //Se il form (Riga Prodotto) è stato salvato correttamente allora lo disabilito
      if(this.ProdottiForm.get("Riga_Salvata").value)
          this.ProdottiForm.disable({emitEvent: false});

  }

     Mostra_Btn_Salva_DoseProdotto(){
        let mostra = false;

        if(!this.qdcservice.Sola_Lettura_QdCForm() &&
            this.ProdottiForm  && !this.ProdottiForm.getRawValue().Riga_Salvata){
            mostra = true;
        }

        return mostra;
    }

    Mostra_Btn_Edit_DoseProdotto(){
        let mostra = false;

        if(!this.qdcservice.Sola_Lettura_QdCForm() &&
            this.ProdottiForm  && this.ProdottiForm.getRawValue().Riga_Salvata){
            mostra = true;
        }

        return mostra;
    }

    /*
    * @description
    * Mostro il bottone di cancellazione del Prodotto se sto facendo un Reinnesco Trappole
    * */
    Mostra_Btn_Elimina_DoseProdotto(){
        let mostra = false;

        let blocco: Blocco = this.qdcservice.TestataForm?.get("Blocco_Attivita")?.getRawValue();

        if(this.qdcservice.objParametriAgenda.TipoOperazioneDB !== Enum_DBTypeOperation.Read &&
           (!blocco || blocco.tipo === Tipo_Blocco.Nessuno) &&
            this.ProdottiForm  && this.ProdottiForm.getRawValue().Riga_Salvata &&
            !this.qdcservice.Is_Ribaltamento_Ricetta_Da_PUA()){
            mostra = true;
        }

        return mostra;
    }

    SalvaDoseProdotto(){

        switch(this.Categoria_Magazzino){
            case FORMULATI:
            case INSETTI:
                this.qdcdettagliformulatiservice.SalvaDoseFormulato(this.Nuova_Riga).then(obj=>{
            //Modalita_Applicazione,

                  if(obj.listerroriGias.length === 0){
                    this.ProdottiForm.patchValue({
                      Riga_Salvata: true
                    });

                    this.ProdottiForm.disable({emitEvent: false});
                  }

                });
                break;
            case FERTILIZZANTI:
                this.qdcdettaglifertilizzantiservice.SalvaDoseFertilizzante(this.Nuova_Riga);
                break;
            case SEMENTI:
                this.qdcdettaglisementiservice.SalvaDoseSemente(this.Nuova_Riga);
                break;
        }
    }

     EliminaDoseProdotto(mostraDialogConferma: boolean){

      return new Promise<boolean>(async (resolve, reject) => {
          let eliminaProdotto: boolean = true;

          if(mostraDialogConferma){

              let listerroriGias = [];

              listerroriGias.push(<ErroreGias>{
                  severity: ErroreGias_Severity.Warning,
                  tipo:  enum_ErroreGias_Tipo.Generico,
                  messaggio: this.translocoService.translate("qdc.SeiSicurodiEliminareilProdotto",{Prodotto: this.ProdottiForm.getRawValue().Prodotto.Descrizione_Concatenata})
              });

              let res = await this.qdcservice.gestisci_ErroriGias(listerroriGias,false,false,"");

              if(res.result.returnObj === false)
                  eliminaProdotto = false;
          }

          if(eliminaProdotto){

              let index_Sezione_Prodotto = this.qdcservice.Sezioni_ProdottoFormArray.getRawValue().findIndex(s=>s.Operazione.primaryKey.codice === this.ProdottiForm.getRawValue().Operazione.primaryKey.codice);

              if(index_Sezione_Prodotto > -1){
                  let index = this.qdcservice.DosiProdottiFormArray(null,this.Operazione).getRawValue().findIndex(s=>s.DosiProdottiGridrowId === this.ProdottiForm.getRawValue().DosiProdottiGridrowId);

                  this.qdcservice.AggiornaFormArrayGridDosiProdotti(this.ProdottiForm.getRawValue(),index,this.Operazione,enum_TipoOperazioneDB.Cancellazione,null);
              }

              if(this.Categoria_Magazzino === FORMULATI)
                  this.qdcformulatiservice.AbilitaDisabilitaEpocaDPI(this.ProdottiForm);


              this.prodottiservice.EventiPostRemoveGridDosiProdotti();
          }

          resolve(eliminaProdotto);
      });

    }

    async ModificaDoseProdotto(){

        this.OldProdottiFormValue = this.ProdottiForm.getRawValue();

        let lav_cod: number = + (this.OldProdottiFormValue.Operazione as Lavorazione).primaryKey.codice;

        if(!this.qdcservice.Is_Ribaltamento_Ricetta_Da_PUA()){
          this.ProdottiForm.enable({emitEvent: false});
        }

        this.ProdottiForm.patchValue({
            Riga_Salvata: false
        });

        //Abilito/Disabilito i controlli in base all'impostazioni utente
        this.prodottiservice.Gestisci_Controlli_Lotti_Giacenze();

        this.prodottiservice.Gestisci_Controlli_Lotti_Giacenze_Innesco();

        switch(this.Categoria_Magazzino){
            case FORMULATI:
                await this.qdcdettagliformulatiservice.MostraMessaggiBottoniEditProttoFormulatiForm();

                this.qdcdettagliformulatiservice.AbilitaDisabilitaDoseHaDoseTot();
                break;
            case FERTILIZZANTI:
                this.qdcservice.Gestisci_Controlli_Fertilizzanti(this.ProdottiForm,lav_cod);

                //Messaggio di Dose Consigliata da mostrare solo se sono in modifica
                await this.qdcdettaglifertilizzantiservice.MostraMessaggiBottoniEditProttoFertilizzantiForm();
                break;
            case SEMENTI:
                break;
        }

        this.Nuova_Riga = false;


    }

    Mostra_Btn_Annulla_DoseProdotto(){
        let mostra = false;

        if(!this.qdcservice.Sola_Lettura_QdCForm() &&
            this.ProdottiForm  && !this.ProdottiForm.getRawValue().Riga_Salvata){
            mostra = true;
        }

        return mostra;
    }

    async AnnullaDoseProdotto(){

        if(!this.ProdottiForm.get("Prodotto").value){
            await this.EliminaDoseProdotto(false);
        }else{

            //Se la Riga che è stata aperta in edit non era ancora stata salvata
            // (es se sono in ribaltamento in agenda mostro in edit tutti i prodotti) chiedo messaggio di conferma per eliminarla
            if(!this.OldProdottiFormValue.Riga_Salvata){

                 await this.EliminaDoseProdotto(true);

                 return;
            }

            this.ProdottiForm.patchValue(
                this.OldProdottiFormValue, {emitEvent: false}
            );

            switch(this.Categoria_Magazzino){
                case FORMULATI:
                case INSETTI:

                    //Nel caso in cui sono nel filtro Avversita-> Prodotto e non è stata scelta l'avversita non faccio il setArrayDDLSezioneProdottiFormulatiForm
                    //per le avversita perchè altrimenti verrebbe nascosta la ddl delle avversita
                    let setArray_Avversita = true;

                    if(this.qdcservice.Mostra_Avversita_Prima_Dei_Prodotti && !this.ProdottiForm.get("Avversita").value)
                        setArray_Avversita = false;

                    //Imposto il valore nelle ddl per poterle mostrare correttamente se non ho l'inserimento in griglia
                    this.qdcdettagliformulatiservice.setArrayDDLSezioneProdottiFormulatiForm(this.ProdottiForm.getRawValue(),setArray_Avversita);

                    await this.qdcdettagliformulatiservice.Carica_DDL_Formulati(false);

                    //Gestisce i messaggi e bottoni che sono da mostrare sempre anche quando è già stato salvato il prodotto
                    await this.qdcdettagliformulatiservice.MostraMessaggiBottoniSenzaGridDosiProdottiFormulatiForm();
                    break;
                case FERTILIZZANTI:
                    //Imposto il valore nelle ddl per poterle mostrare correttamente se non ho l'inserimento in griglia
                    this.qdcdettaglifertilizzantiservice.setArrayDDLSezioneProdottiFertilizzantiForm(this.ProdottiForm.getRawValue());
                    break;
                case SEMENTI:
                    //Imposto il valore nelle ddl per poterle mostrare correttamente se non ho l'inserimento in griglia
                    this.qdcdettaglisementiservice.setArrayDDLSezioneProdottiSementiForm(this.ProdottiForm.getRawValue());
                    break;
            }


            if(this.ProdottiForm.get("Riga_Salvata").value){
                this.ProdottiForm.disable({emitEvent: false});
            }else{
                this.ProdottiForm.enable({emitEvent: false});
            }
        }

        this.qdcservice.Calcola_Percentuale_N_Per_Piano_Nutrizionale_Fertilizzanti(this.ProdottiForm.get("DosiProdottiGridrowId").value);
    }

}
