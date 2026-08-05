import {Injectable} from "@angular/core";
import {Epoca} from "app/Model/metaschema/Epoca";
import {Disciplinare} from "app/Model/metaschema/Disciplinari";
import {
    enum_LAVCOD,
    enum_PUARegolamenti_Tipo
} from "app/Model/TipiEnumerativi";
import {QdCService} from "../qdc.service";
import {FormGroup} from "@angular/forms";
import {EpocheService, LeggiEpoche} from "app/Service/Metaschema/epoche.service";
import {DpiService, LeggiDisciplinari} from "app/Service/DPI/dpi.service";
import {Lavorazione} from "../../../../Model/attivita/Lavorazione";
import {enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity} from "../../../../Service/master.service";

@Injectable()

export class QdCFertilizzantiService{

    Array_EpocheFertilizzazione: Array<Epoca> = [];

    Array_Direttive_Nitrati: Array<Disciplinare> = [];

    constructor(private qdcservice: QdCService,
                private epocheservice: EpocheService,
                private dpiservice: DpiService){

    }

    //Letture Lato Server

    async getArray_EpocheFertilizzazione(Form: FormGroup) {

        let Disciplinare = this.qdcservice.getDisciplinareModelValue(+ Form.get("Operazione").value.primaryKey.codice);

        const LeggiEpoche = <LeggiEpoche>{
            lavorazione: Form.get("Operazione").value,
            specie: this.qdcservice.GetSpeciefromUtilizzoTerreno(),
            disciplinare: Disciplinare
        };

        this.Array_EpocheFertilizzazione = await this.epocheservice.Leggi_EpocheFertilizzazione_QdC(
            LeggiEpoche
        );

        // Se c'è solo una Epoca di Fertilizzazione lo imposto come default
        if (this.Array_EpocheFertilizzazione.length === 1) {
            Form.patchValue({
                EpocaFertilizzazione: this.Array_EpocheFertilizzazione[0],
            },{emitEvent: false});
        }

        this.AggiungiRigaVuota_EpocaFertilizzazione(Disciplinare,Form);

        return this.Array_EpocheFertilizzazione;
    }


    getArray_Direttiva_Nitrati() {

        return new Promise<Disciplinare[]>(async (resolve,reject)=>{
            const LeggiDisciplinari=<LeggiDisciplinari>{
                lavorazioni: this.qdcservice.TestataForm.get('Operazioni').getRawValue(),
                data: this.qdcservice.TestataForm.get('Data').value
            };

            this.Array_Direttive_Nitrati = await this.dpiservice.Leggi_Disciplinari_Testata_DirettivaNitrati(LeggiDisciplinari);


            //Escludo il disciplinare ER 2018-2020 perchè non deve essere gestito su angular ma viene gestito nelle pagine vecchie
            if(this.Array_Direttive_Nitrati && this.Array_Direttive_Nitrati.length > 0){
                this.Array_Direttive_Nitrati = this.Array_Direttive_Nitrati.filter(d=>d.regolamentoConcimazione.codice !== 55);
            }

            resolve(this.Array_Direttive_Nitrati);
        });


    }

    //Fine Letture Lato Server


    AggiungiRigaVuota_EpocaFertilizzazione(d: Disciplinare,Form: FormGroup){
        //Non aggiungo la Riga vuota solo per DISTRIBUZIONE_AMMENDANTI e se il disciplinare scelto ha una direttiva nitrati
        //(RegolamentoConcimazione.tipo = 2).
        //Negli altri casi la Epoca di Fertlizzazione non è obbligatoria e aggiungo la riga vuota nella ddl.


        let EpocaFertilizzazoine_Selezionata: Epoca = Form.get("EpocaFertilizzazione").value;

        if(+ Form.get("Operazione").value.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI &&
            d &&
            d.regolamentoConcimazione &&
            d.regolamentoConcimazione.tipo === enum_PUARegolamenti_Tipo.PUA){

            //Tolgo la riga vuota perchè la Epoca di Fertilizzazione è obbligatoria
            let index: number = this.Array_EpocheFertilizzazione.findIndex( e => e.codice === 0);

            if(index > -1)
                this.Array_EpocheFertilizzazione.splice(index,1);

            if(EpocaFertilizzazoine_Selezionata &&
                EpocaFertilizzazoine_Selezionata.codice === 0){

                Form.patchValue({
                    EpocaFertilizzazione: null
                });
            }

        }else{

            //Aggiungo la riga vuota se non è obbligatoria la Epoca di Fertilizzazione
            if(this.Array_EpocheFertilizzazione && this.Array_EpocheFertilizzazione.findIndex( e => e.codice === 0) === -1){

                const EpocaVuota = new Epoca(0);

                EpocaVuota.descrizione = "";

                this.Array_EpocheFertilizzazione.unshift(EpocaVuota);

                if(!Form.get("EpocaFertilizzazione").value){

                    Form.patchValue({
                        EpocaFertilizzazione: EpocaVuota
                    });
                }
            }
        }
    }

    mostraEpocaFertilizzazione(){

        let mostra = false;

        if(this.Array_EpocheFertilizzazione){

            //Se c'è la riga vuota non la conto come riga per mostrare/nascondere le epoche
            if(this.Array_EpocheFertilizzazione.findIndex(e=>e.codice === 0)>-1){
                if(this.Array_EpocheFertilizzazione.length > 1){
                    mostra = true;
                }
            }else{
                if(this.Array_EpocheFertilizzazione.length > 0){
                    mostra = true;
                }
            }

        }

        return mostra;
    }

    mostraDivDirettivaNitrati(Form: FormGroup){

        let mostra = false;

        if(+ (<Lavorazione> Form.get("Operazione").value).primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI &&
            this.Array_Direttive_Nitrati && this.Array_Direttive_Nitrati.length > 0){
            mostra = true;
        }

        return mostra;
    }

    mostraddlDirettivaNitrati(Form: FormGroup){

        let mostra = false;

        if(this.mostraDivDirettivaNitrati(Form)&& Form.get("Utilizza_Direttiva_Nitrati").value){
            mostra = true;
        }

        return mostra;
    }

    public ObbligatoryEpocaFertilizzazione(Operazione: Lavorazione): boolean{
      let obbligatory = false;

      if(this.Array_EpocheFertilizzazione && this.Array_EpocheFertilizzazione.length > 0){
        if(Operazione && + Operazione.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI){
          let Disciplinare = this.qdcservice.getDisciplinareModelValue(enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI);

          if(Disciplinare &&
            Disciplinare.regolamentoConcimazione &&
            Disciplinare.regolamentoConcimazione.tipo === enum_PUARegolamenti_Tipo.PUA){

            obbligatory = true;
          }
        }

      }


      return obbligatory;
    }



}
