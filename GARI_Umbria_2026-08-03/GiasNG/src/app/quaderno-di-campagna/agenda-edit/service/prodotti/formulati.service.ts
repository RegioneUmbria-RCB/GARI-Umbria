import {Injectable, OnDestroy} from "@angular/core";
import {FormGroup} from "@angular/forms";
import {Lavorazione} from "app/Model/attivita/Lavorazione";
import {Subscription} from "rxjs";
import {enum_LAVCOD} from "../../../../Model/TipiEnumerativi";
import {
  AGRODATAFINE,
  AGRODATAINIZIO,
  DpiBio,
  NessunDpi,
  NessunDpiNessunaEtichetta
} from "../../../../Model/CostantiPersonalizzate";
import {TranslocoService} from "@jsverse/transloco";
import {enum_Stato_Innesco, QdCService} from "../qdc.service";
import {EpocheService, LeggiEpoche} from "../../../../Service/Metaschema/epoche.service";
import {ObjParametriAgendaService} from "../../../../Service/obj-parametri-agenda.service";
import {Epoca} from "app/Model/metaschema/Epoca";
import {enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity} from "../../../../Service/master.service";
import {MultiColumnComboboxTrattamento} from "../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";

@Injectable()

export class QdCFormulatiService implements OnDestroy {

    public Array_EpocheDPI: Array<Epoca> = [];

    Subs: Subscription = new Subscription();

    constructor(private translocoService: TranslocoService,
                private qdcservice: QdCService,
                private epocheservice: EpocheService,
                private objParametriAgendaService: ObjParametriAgendaService){

    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    getDescrizioneEpocaDPI(Form: FormGroup): string {

        let descr = '';

        let lav_cod = + <Lavorazione>(Form.get("Operazione").value).primaryKey.codice;

        switch (lav_cod) {
            case enum_LAVCOD.DISERBO:
                descr = this.translocoService.translate('Epoca');
                break;
            case enum_LAVCOD.TRATTAMENTO_ANTIPARASSITARIO:
                descr = this.translocoService.translate('Modulo');
                break;
        }

        return descr;
    }

    async getArray_EpocheDPI(Form: FormGroup) {

        let lav_cod = + <Lavorazione>(Form.get("Operazione").value).primaryKey.codice;

        let Disciplinare = this.qdcservice.getDisciplinareModelValue(lav_cod);

        //Nascondo le EpocheDPI per il disseccamento e fitoregolatore (vedi mail 'R: Spollonante – patata- carfentrazole-etile – Spotlight plus 13466 --> dubbio caricamento epoche disciplinare nel disseccamento/spollonatura')

        if(lav_cod !== enum_LAVCOD.DISSECCAMENTO &&
            lav_cod !== enum_LAVCOD.TRATTAMENTO_FITOREGOLATORE &&
            Disciplinare &&
            Disciplinare.codice !== NessunDpiNessunaEtichetta &&
            Disciplinare.codice !== NessunDpi &&
            Disciplinare.codice !== DpiBio){

            const LeggiEpoche = <LeggiEpoche>{
                lavorazione: <Lavorazione>(Form.get("Operazione").value),
                specie: this.qdcservice.GetSpeciefromUtilizzoTerreno(),
                disciplinare: this.qdcservice.getDisciplinareModelValue(lav_cod),
            };

            this.Array_EpocheDPI = await this.epocheservice.Leggi_EpocheDPI_QdC(
                LeggiEpoche
            );

            // Se c'è solo una Epoca DPI lo imposto come default
            //Non impostare emitevent a false perchè devono scattare le subscribe nei componenti figli (dettagli-formulati)
            if (this.Array_EpocheDPI.length === 1) {
                Form.patchValue({
                    EpocaDPI: this.Array_EpocheDPI[0],
                });
            }

        }else{
            this.Array_EpocheDPI = [];
        }



        return this.Array_EpocheDPI;
    }

    public mostraEpocaDPI(Form: FormGroup) {
        let mostra = false;

        //Nascondo le EpocheDPI per il disseccamento e fitoregolatore (vedi mail 'R: Spollonante – patata- carfentrazole-etile – Spotlight plus 13466 --> dubbio caricamento epoche disciplinare nel disseccamento/spollonatura')

        let lav_cod = + <Lavorazione>(Form.get("Operazione").value).primaryKey.codice;

        if(lav_cod !== enum_LAVCOD.DISSECCAMENTO &&
            lav_cod !== enum_LAVCOD.TRATTAMENTO_FITOREGOLATORE){

            let Disciplinare = this.qdcservice.getDisciplinareModelValue(lav_cod);

            //Deve essere scelto anche un disciplinare
            if (Disciplinare &&
                Disciplinare.codice !== NessunDpiNessunaEtichetta &&
                Disciplinare.codice !== NessunDpi &&
                Disciplinare.codice !== DpiBio &&
                this.Array_EpocheDPI &&
                this.Array_EpocheDPI.length > 0
            ) {
                mostra = true;
            }
        }


        return mostra;
    }

    AbilitaDisabilitaEpocaDPI(Form: FormGroup){

        //Disabilito la ddl delle Epoche DPI se ci sono delle righe nella griglia dei dosaggi
        if(this.mostraEpocaDPI(Form)){

            if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()){

                if(this.qdcservice.Sola_Lettura_QdCForm()  ||
                    Array<any>(Form.get("DosiProdotti").value)?.length > 0){

                    Form.get("EpocaDPI").disable({ emitEvent: false });

                }else{

                    Form.get("EpocaDPI").enable({ emitEvent: false });

                }

            }else{

                let lav_cod = + <Lavorazione>(Form.get("Operazione").value).primaryKey.codice;

                let index = this.qdcservice.Sezioni_ProdottoFormArray.getRawValue().findIndex(s=>+ s.Operazione.primaryKey.codice === lav_cod);

                if(this.qdcservice.Sola_Lettura_QdCForm() ||
                    (this.qdcservice.DosiProdotticonRigheSalvate(Form,null).length > 0)){

                    this.qdcservice.Sezioni_ProdottoFormArray.controls[index].get("EpocaDPI").disable({ emitEvent: false });

                }else{
                    this.qdcservice.Sezioni_ProdottoFormArray.controls[index].get("EpocaDPI").enable({ emitEvent: false });
                }

            }

        }

    }

    /*
    * @description: Forzo il Caricamento delle EpocheDPI durante il ribaltamento di una ricetta da APP perchè potrebbe non arrivarmi da APP
    * ma il prodotto potrebbe richiederlo
    */
    async Carica_Controlli_Ribaltamento_in_Agenda_Di_Ricetta_da_APP_Formulati(Form: FormGroup){
        if(this.qdcservice.Is_Ribaltamento_Ricetta_Da_Origine_Diversa()){
            let sezione_formulato: any = Form.getRawValue();

            if(sezione_formulato){

                let epocaDPI: Epoca = sezione_formulato.EpocaDPI;

                if(!epocaDPI || epocaDPI?.codice === 0){
                    await this.getArray_EpocheDPI(Form);
                }
            }
        }
    }

    public mostraRipartizioneTrappole(Form: FormGroup): boolean{
      let mostra = false;

      let lav_cod = + <Lavorazione>(Form.get("Operazione").value).primaryKey.codice;

      if(this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(lav_cod))
        mostra = true;

      return mostra;
    }

    public ObbligatoryEpocaDPI(): boolean{
        let obbligatory = false;

        if(this.Array_EpocheDPI && this.Array_EpocheDPI.length > 0)
            obbligatory = true;

        return obbligatory;
    }

      /*
    * @description Epoca DPI è obbligatoria per i trattamenti antiparassitari e diserbo se ci sono delle Epoche caricate nella combo
    * (Per i trattamenti si chiamano Moduli ma dovrebbero essere attivi solo per i DPI più vecchi pre 2010)
    * */
    public Controlla_EpocaDPI(EpocaDPI: Epoca,Operazione: Lavorazione,listerroriGias: ErroreGias[]){

        if(Operazione && this.ObbligatoryEpocaDPI()) {
          if (!EpocaDPI ||
            EpocaDPI.codice === 0) {

            if (+Operazione.primaryKey.codice === enum_LAVCOD.TRATTAMENTO_ANTIPARASSITARIO) {
              listerroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.Bloccante,
                tipo: enum_ErroreGias_Tipo.Generico,
                messaggio: this.translocoService.translate("SelezionareUnModulo")
              });
            } else if (+Operazione.primaryKey.codice === enum_LAVCOD.DISERBO) {
              listerroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.Bloccante,
                tipo: enum_ErroreGias_Tipo.Generico,
                messaggio: this.translocoService.translate("qdc.SelezionareUnEpoca")
              });
            }
          }
        }

    }

    /*
    * @description
    * In base al Prodotto passato indico se il suo Innesco è Valido, Scaduto o in Scadenza
    * */
    public Controlla_Stato_Innesco(Formulato: MultiColumnComboboxTrattamento): enum_Stato_Innesco{

      let stato_innesco: enum_Stato_Innesco = enum_Stato_Innesco.Valido;

      if(this.qdcservice.flag_Reinnesco &&
        Formulato && Formulato.durataFeromone > 0 &&
        Formulato.scadenzaFeromone !== AGRODATAINIZIO && Formulato.scadenzaFeromone !== AGRODATAFINE){

        let Data = (new Date()).setHours(0, 0, 0, 0);

        let Data_Scadenza_Feromone = (Formulato.scadenzaFeromone as Date).setHours(0, 0, 0, 0);

        if(Data_Scadenza_Feromone < Data){
          stato_innesco = enum_Stato_Innesco.Scaduto;
        }else{
          let difference = Data_Scadenza_Feromone - Data;

          let daysBetween = Math.ceil(difference / (1000 * 3600 * 24));

          if(daysBetween >= 0 &&  daysBetween < 7)
            stato_innesco = enum_Stato_Innesco.In_Scadenza;
        }

      }

      return stato_innesco;

    }

}
