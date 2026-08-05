import {Component, OnInit, ViewEncapsulation} from '@angular/core';
import {FormGroup, FormGroupDirective} from "@angular/forms";
import {
  GridImpiantoSelezionatoModel,
  Sezione_Prodotto_Fertilizzanti, Sezione_Prodotto_Formulati,
  Sezione_Prodotto_Sementi
} from "../../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {Epoca} from "../../../../../Model/metaschema/Epoca";
import {Disciplinare} from "../../../../../Model/metaschema/Disciplinari";
import {BaseCodeDescr} from "../../../../../Model/baseClass/baseCodeDescr";
import {FERTILIZZANTI, FORMULATI, INSETTI, SEMENTI} from '../../../../../Model/CostantiPersonalizzate';
import {enum_LAVCOD, enum_TipoOperazioneDB} from "../../../../../Model/TipiEnumerativi";
import {Lavorazione} from "../../../../../Model/attivita/Lavorazione";
import {QdCService} from "../../../service/qdc.service";
import {GridDosiProdottiService} from "../../../service/grid-dosi-prodotti/grid-dosi-prodotti.service";
import {QuantitaSuImpianto} from "../../../../../Model/attivita/dettagli/QuantitaSuImpianto";

@Component({
  standalone: false,
  selector: 'app-bottone-aggiunta-prodotto',
  templateUrl: './bottone-aggiunta-prodotto.component.html',
  styleUrls: ['./bottone-aggiunta-prodotto.component.css']
})
export class BottoneAggiuntaProdottoComponent implements OnInit {

    ProdottiForm: FormGroup;

  constructor(private qdcservice: QdCService,
              private gridDosiProdottiService: GridDosiProdottiService,
              public parent: FormGroupDirective) { }

  ngOnInit(): void {
      this.ProdottiForm = <FormGroup>this.parent.form;
  }

    public AggiungiDoseProdotto(sezione_form: FormGroup){

        let sezione_prodotto: Sezione_Prodotto_Sementi | Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Formulati = sezione_form.getRawValue();

        let elem_cod: number = sezione_prodotto.Categoria_Magazzino;

        let lav_cod: number = + (sezione_prodotto.Operazione.primaryKey.codice);

        let EpocaDPI: Epoca = null;

        let EpocaFertilizzazione: Epoca = null;

        let Utilizza_Direttiva_Nitrati: boolean = false;

        let Direttiva_Nitrati: Disciplinare = null;

        let Opzioni_Semina: BaseCodeDescr = null;

        let Modalita_Applicazione: BaseCodeDescr = null;

        let Ripartizione_Trappole: BaseCodeDescr = null;

        let QuantitaSuImpianti: QuantitaSuImpianto[] = null;

        switch(elem_cod){
            case FORMULATI:
            case INSETTI:
                sezione_prodotto = sezione_prodotto as Sezione_Prodotto_Formulati;
                EpocaDPI = sezione_prodotto.EpocaDPI;
                Modalita_Applicazione = sezione_prodotto.Modalita_Applicazione;
                Ripartizione_Trappole = sezione_prodotto.Ripartizione_Trappole;

                if(this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(lav_cod)){
                  let GridImpianti: Array<GridImpiantoSelezionatoModel> = this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue();

                  if(GridImpianti && GridImpianti.length > 0){

                    QuantitaSuImpianti = [];

                    GridImpianti.forEach(GridImpianto => {
                      QuantitaSuImpianti.push({ esercizioCDC: this.qdcservice.GetEsercizioCDCModel(GridImpianto), Qta: 0 } as QuantitaSuImpianto);
                    });
                  }
                }
                break;
            case FERTILIZZANTI:
                sezione_prodotto = sezione_prodotto as Sezione_Prodotto_Fertilizzanti;
                EpocaFertilizzazione = sezione_prodotto.EpocaFertilizzazione;
                Utilizza_Direttiva_Nitrati = sezione_prodotto.Utilizza_Direttiva_Nitrati;
                Direttiva_Nitrati = sezione_prodotto.Direttiva_Nitrati;
                Modalita_Applicazione = sezione_prodotto.Modalita_Applicazione;
                break;
            case SEMENTI:
                sezione_prodotto = sezione_prodotto as Sezione_Prodotto_Sementi;
                Opzioni_Semina = sezione_prodotto.Opzioni_Semina;
                break;
        }


        let row: FormGroup = this.gridDosiProdottiService.getRigaGridDosiProdotti(sezione_prodotto.DosiProdotti,-1,null,null,null,
            sezione_prodotto.Operazione,null,null,
            null,null,[],EpocaDPI,EpocaFertilizzazione,Utilizza_Direttiva_Nitrati,Direttiva_Nitrati,Opzioni_Semina,Modalita_Applicazione,QuantitaSuImpianti,Ripartizione_Trappole,true,false,true);


        if(row){
            this.qdcservice.AggiornaFormArrayGridDosiProdotti(row.getRawValue(),0,sezione_prodotto.Operazione,enum_TipoOperazioneDB.Scrittura,null,false);
        }


    }

    //Disabilito il pulsante di AggiungiDoseProdotto se ci sono delle Dosi Prodotto non salvate
    public Disabilita_Btn_AggiungiDoseProdotto(Form: FormGroup){

        let disabilita = false;

        let Operazione: Lavorazione = Form.get("Operazione").getRawValue();

        for(let s of this.qdcservice.Sezioni_ProdottoFormArray.getRawValue()){
            if(s.DosiProdotti && s.DosiProdotti.findIndex(d=>!d.Riga_Salvata && + d.Operazione.primaryKey.codice === + Operazione.primaryKey.codice) > -1){
                disabilita = true;

                break;
            }
        }

        return disabilita;

    }


}
