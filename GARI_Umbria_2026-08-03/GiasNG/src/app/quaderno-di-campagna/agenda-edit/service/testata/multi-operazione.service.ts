import {Injectable, OnDestroy} from '@angular/core';
import {TranslocoService} from '@jsverse/transloco';
import {MultiSelectComponent, RemoveTagEvent} from '@progress/kendo-angular-dropdowns';
import {CentroAziendale} from 'app/Model/anagrafiche/CentroAziendale';
import {Lavorazione} from 'app/Model/attivita/Lavorazione';
import {LinkQdCAngular, LinkVisiteEditAngular} from 'app/Model/CostantiPersonalizzate';
import {enum_LAVCOD, enum_SEMINA_TIPO, enum_TipoOperazioneDB} from 'app/Model/TipiEnumerativi';
import {AgendaService, LeggiLink_Operazione} from 'app/Service/Agenda/Agenda.service';
import {enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity} from 'app/Service/master.service';
import {LeggiOperazioni} from 'app/Service/Metaschema/operazioni.service';
import {ObjParametriAgendaService} from 'app/Service/obj-parametri-agenda.service';
import {Enum_DBTypeOperation, GiiasMultiselectTemplateSComponent, MultiSelectFormItem} from 'gias-ui-kit';
import {BehaviorSubject, Subscription} from 'rxjs';
import {
  CodiciXOperazione,
  DropdownListDisciplinare,
  GridMacchinaModel,
  Obj_Errore_Gias_QdC
} from '../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import {QdCService} from '../qdc.service';
import {QdCTestataService} from './testata.service';
import {Disciplinare} from "../../../../Model/metaschema/Disciplinari";
import {Gias2010Redirector} from 'app/menu-agenda/components/grid-qdc/Gias2010Redirector.service';
import {GridDosiProdottiService} from "../grid-dosi-prodotti/grid-dosi-prodotti.service";
import {GridDataWithFilter} from 'gias-kendo-grid';
import {Pua} from "../../../../Model/metaschema/Pua";
import {UtilityFunctions} from "../../../../Utility/UtilityFunctions";

@Injectable()


export class QdCMultiOperazioneService implements OnDestroy{

    MultiSelectOperazioni: MultiSelectComponent;

    //Escludo la creazione del Reinnesco dal bottone 'Nuova Operazione' si può fare solo col bottone Reinnesco
    //dalla griglia Gestione Trappole
    Elenco_Operazioni_da_Escludere: Array<number> = [enum_LAVCOD.REINNESCO_TRAPPOLE];

    Subs: Subscription= new Subscription();

    constructor(private qdcservice: QdCService,
                private objParametriAgendaService: ObjParametriAgendaService,
                private gias2010Redirector: Gias2010Redirector,
                private agendaservice: AgendaService,
                public testataservice: QdCTestataService,
                private translocoService: TranslocoService,
                private griddosiprodottiservice: GridDosiProdottiService){

    }


    changeOperazioni(multiElem: MultiSelectFormItem){
        multiElem.newValue = multiElem.newValue as Lavorazione;
        this.qdcservice.isNoteInitialized = false;
        if(multiElem.newValue){
            if(multiElem.Values.length > 1){
                //Se sono nel caso con più di due Operazioni scelte allora non devo fare redirect alle altre pagine
                //delle operazioni che non sono gestite
                this.changeOperazioni_QdC(multiElem);
            }else{
                //this.qdcservice.Mostra_QdC = false;
                let leggiLink_Operazione = new LeggiLink_Operazione;
                let Codice_Attivita: CodiciXOperazione = this.qdcservice.TestataForm.get("Codici_Attivita").value.find(function(el:CodiciXOperazione) {
                    return el.Operazione && el.Operazione.primaryKey.codice === multiElem.newValue.primaryKey.codice;
                });

                if(Codice_Attivita){
                    leggiLink_Operazione.id_agenda = Codice_Attivita.CodiceAttivita;
                    leggiLink_Operazione.codiceOperazioneRicetta = Codice_Attivita.CodiceOperazioneRicetta;
                    leggiLink_Operazione.codiceRicetta = Codice_Attivita.CodiceRicetta;
                }else{
                    leggiLink_Operazione.id_agenda = "0";
                    leggiLink_Operazione.codiceOperazioneRicetta = "0";
                    leggiLink_Operazione.codiceRicetta = "0";
                }

                leggiLink_Operazione.tipo_operazione = this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB;
                leggiLink_Operazione.impresa = this.qdcservice.getImpresa_Model();
                leggiLink_Operazione.centroaziendale = new CentroAziendale({codice: 0,partitaIva: leggiLink_Operazione.impresa.partitaIva})
                leggiLink_Operazione.specie = this.qdcservice.GetSpeciefromUtilizzoTerreno();
                leggiLink_Operazione.data = this.qdcservice.TestataForm.get("Data").value;
                leggiLink_Operazione.lavorazione = <Lavorazione> multiElem.newValue;
                leggiLink_Operazione.tipo = this.qdcservice.TestataForm.get("Tipo").value;
                leggiLink_Operazione.tiporicetta = this.qdcservice.TestataForm.get("TipoRicetta").value;
                leggiLink_Operazione.stato = this.qdcservice.TestataForm.get("Stato").value;

                this.agendaservice.Redirect_In_Base_Al_Lav_Cod(leggiLink_Operazione).then(link=>{
                    switch(link) {
                      case LinkQdCAngular:
                      case LinkVisiteEditAngular:
                          this.changeOperazioni_QdC(multiElem);
                        break;
                      default:
                          this.gias2010Redirector.reindirizza(link, this.gias2010Redirector.getAgenda(leggiLink_Operazione,this.qdcservice.masterService.getCurrentPageAsValue(),this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB));
                        break;
                    }
                });
            }
        }else{
            this.changeOperazioni_QdC(multiElem);
        }
    }

    //Funzione di change per l'Operazioni che fanno parte del QdC
    //Controllare quando si aggiungono nuove funzionalità se sono da aggiungere anche per il quaderno-di-campagna.component
    //quando viene valorizzato il ObjPageToMemorizeper l'apertura della pagina nel Salvae Nuovo oppure da Anagrafica
    changeOperazioni_QdC(multiElem: MultiSelectFormItem){

        this.Controlla_Compatibilita_Dati_Con_Nuova_Operazione(multiElem).then(async (nuova_operazione_compatibile : boolean) =>{

            if(!nuova_operazione_compatibile){
                //this.qdcservice.Mostra_QdC = true;
                return;
            }

            let codici_attivita: Array<CodiciXOperazione> = this.qdcservice.TestataForm.get("Codici_Attivita").value;

            let Operazioni_Scelte: Array<Lavorazione> = multiElem.Values;

            this.qdcservice.GestisciValidatorImpiantiSuperficie(Operazioni_Scelte);

            Operazioni_Scelte.forEach((o:Lavorazione)=>{

                //Aggiungo un nuovo Codice Attività
                if(codici_attivita.findIndex((c) => c.Operazione === o) === -1){

                    let index_codice_attivita_no_operazione = codici_attivita.findIndex((c) => !c.Operazione);

                    if(index_codice_attivita_no_operazione > -1){
                        codici_attivita[index_codice_attivita_no_operazione].Operazione = o;
                    }else{

                        //Il codice Ricetta e il Pua_Cod deve essere uguale per ogni attivita (utilizzata solo se sto facendo una ricetta)
                        let codiciricetta = codici_attivita.map((item => item.CodiceRicetta)).filter((value, index, self) => self.indexOf(value) === index);

                        let pua_cod = codici_attivita.map((item => item?.pua?.codice)).filter((value, index, self) => self.indexOf(value) === index);

                        let codice_attivita_visita = codici_attivita.map((item => item?.CodiceAttivitaVisita)).filter((value, index, self) => self.indexOf(value) === index);

                        if(codiciricetta && codiciricetta.length === 1 && pua_cod && pua_cod.length === 1 && codice_attivita_visita && codice_attivita_visita.length === 1){
                            codici_attivita.push(new CodiciXOperazione("0",codiciricetta[0],"0", o,null,null,"", codice_attivita_visita[0],new Pua(pua_cod[0])));
                        }

                    }

                    this.qdcservice.TestataForm.patchValue({
                        Codici_Attivita: codici_attivita
                    });

                }
            });

            if(Operazioni_Scelte){
                if (Operazioni_Scelte.length == 1) {
                    if (this.qdcservice.isProdottoDaTrattare(+Operazioni_Scelte[0].primaryKey.codice)) {
                        this.qdcservice.ImpiantiSelezionatiFormArray.clear();
                    } else {
                        this.qdcservice.ProdottiDaTrattareSelezionatiFormArray.clear()
                    }

                    this.qdcservice.eventiPostSelectionChangeGridImpianti.next([]);
                }

                //Quando seleziono una nuova Operazione rileggo la funzione Inizializza_QdC
                this.qdcservice.Inizializza_QdC().then(async r=>{
                    if(r){

                        this.qdcservice.setImpostazione_Avversita_Prima_Dei_Prodotti(Operazioni_Scelte);

                        await Promise.all(this.RicaricaControlli());

                        let disciplinare_testata: DropdownListDisciplinare = this.qdcservice.TestataForm.getRawValue().Disciplinare;

                        this.qdcservice.Gestisci_Sezioni(this.qdcservice.TestataForm.get("Operazioni").getRawValue());

                        this.griddosiprodottiservice.AggiungiRigaVuotaGridDosiProdotti();

                        this.qdcservice.setVariabiliGlobali();

                        this.qdcservice.TestataForm.patchValue(
                            this.qdcservice.ImpostaValoriDefaultXOperazione(this.qdcservice.TestataForm.getRawValue())
                        );

                        //Richiamo la funzione ImpostaDisciplinare dagli Impianti Selezionati se la operazione ha dei Disciplinari
                        //(N.B. Nel caso  di multi-operazione i disciplinari vengono ricaricati prima controllando anche che il dpi impostato nella combo sia compatibile con le Operazioni scelte)
                        if((Operazioni_Scelte.length === 1 || !disciplinare_testata) &&
                            Operazioni_Scelte.findIndex((o: Lavorazione) => this.qdcservice.MostraDisciplinare(o, this.qdcservice.Sezioni_ProdottoFormArray)) > -1){

                            await this.testataservice.getArray_Disciplinari();

                            await this.qdcservice.ImpostaDisciplinare_Dagli_ImpiantiSelezionati(this.testataservice.Array_Disciplinari);

                        }

                        this.qdcservice.RicaricaGridImpianti();

                        this.qdcservice.RicaricaProdottiDaMagazzino();

                        this.qdcservice.TrovaPoligoniGIS();

                        this.qdcservice.Avviso_dichiarazione_non_utilizzo((this.qdcservice?.GridImpiantiPublicService as BehaviorSubject<GridDataWithFilter>)?.getValue()?.data?.rows);

                        //this.qdcservice.Mostra_QdC = true;

                        //Per la nuova operazione CONFUSIONE_DISORIENTAMENTO_SESSUALE e per TRATTAMENTO_POST_RACCOLTA anche se non è visibile il disciplinare lo imposto
                        //comunque a Solo Etichetta perchè lato server verrà gestito in questo modo
                        if(Operazioni_Scelte.length === 1 && Operazioni_Scelte.findIndex(o=>this.qdcservice.Elenco_Operazioni_Con_Default_DPI.includes(+ o.primaryKey.codice)) > -1){

                            this.qdcservice.TestataForm.patchValue({
                                Disciplinare: this.qdcservice.setCodDescrDdlDisciplinare(this.qdcservice.Obj_NessunDpi as DropdownListDisciplinare)
                            },{emitEvent: false});
                        }

                    }
                });
            }

            this.DisabilitabtnGroupVisualizza_Solo_Operazioni_Preferite();


        });

    }

    RicaricaControlli():Promise<any>[]{

        let arrayPromise:Promise<any>[] = [
            this.testataservice.getArray_UtilizziTerreno(),
            this.qdcservice.RicaricaGridMacchine(),
            this.qdcservice.RicaricaGridOperatori()
        ];

        if(this.testataservice.Leggi_Tutti_Centri_Senza_Filtro_X_Specie())
          arrayPromise.push(this.testataservice.getArray_CentriAziendale(true));

        return arrayPromise;
    }

    Controlla_Compatibilita_Dati_Con_Nuova_Operazione(multiElem: MultiSelectFormItem){

        return new Promise<boolean>(async (resolve, reject) => {

            let listerroriGias:ErroreGias[] = [];

            let nuova_operazione_compatibile: boolean = true;

            multiElem.newValue = multiElem.newValue as Lavorazione;

            if(multiElem.newValue){

                //Controllo se nelle precedenti Operazioni era gestito il Disciplinare

                //TODO quando verranno gestite le operazioni che non hanno il terreno nudo (es Irrigazione) dovrò aggiungere
                //il controllo sulla Specie /Destinazione d'uso

                let Operazioni_Precedenti: Array<Lavorazione> = multiElem.Values.filter(o=>o.primaryKey.codice !== multiElem.newValue.primaryKey.codice);

                if(Operazioni_Precedenti.findIndex(o=>{ return this.qdcservice.MostraDisciplinare(o,this.qdcservice.Sezioni_ProdottoFormArray); }) > -1 &&
                    this.qdcservice.MostraDisciplinare(multiElem.newValue,null)){

                    let disciplinare_scelto = this.qdcservice.getDisciplinareModelValue(0);

                    if(disciplinare_scelto){

                        let newArray: Array<DropdownListDisciplinare> = await this.testataservice.getArray_Disciplinari(false);

                        //Ricarico l' array dei disciplinari e se non è compreso il disciplinare scelto in precedenza per l'operazione
                        //annullo la selezione della nuova operazione e lancio un messaggio di errore
                        if(newArray){

                            let disciplinare_compatibile: Disciplinare = newArray.find(d=> d.codice === disciplinare_scelto.codice &&
                                                                                            d.disciplinarePubblicoPrivato === disciplinare_scelto.disciplinarePubblicoPrivato &&
                                                                                            this.qdcservice.Controlla_Flag_Protetto_Disciplinari(disciplinare_scelto,d)  &&
                                                                                            this.qdcservice.Controlla_GruppoFinalita_Disciplinari(disciplinare_scelto,d) &&
                                                                                            d.regolamentoConcimazione?.codice === disciplinare_scelto.regolamentoConcimazione?.codice);

                            if(!disciplinare_compatibile){

                                nuova_operazione_compatibile = false;

                                listerroriGias.push(<ErroreGias>{
                                    severity: ErroreGias_Severity.Bloccante,
                                    tipo:  enum_ErroreGias_Tipo.Generico,
                                    messaggio: this.translocoService.translate("qdc.Errore_Disciplinare_non_compatibile",
                                        { Disciplinare: this.qdcservice.setCodDescrDdlDisciplinare(disciplinare_scelto as DropdownListDisciplinare).Descrizione_Concatenata,
                                            Operazione: multiElem.newValue.descrizione})
                                });

                                await this.qdcservice.gestisci_ErroriGias(listerroriGias,true,true);

                            }else{

                                //Reimposto il disciplinare dagli impianti se ha lo stesso codice selezionato del dpi della combo

                                let dpi_impianti = await this.qdcservice.get_Disciplinare_Dagli_ImpiantiSelezionati(newArray);

                                if(dpi_impianti && dpi_impianti.codice === disciplinare_compatibile.codice){
                                    disciplinare_compatibile = dpi_impianti;
                                }

                                this.qdcservice.TestataForm.patchValue({
                                    Disciplinare: disciplinare_compatibile
                                });

                                this.testataservice.Array_Disciplinari = newArray;
                            }
                        }
                    }
                }

                //Non permetto l'inserimento di una macchina di tipo Irrigatrice nella griglia 'Macchine' se nella lista di operazioni c'è Irrigazione o Fertirrigazione
                //perchè deve essere indicata nella colonna 'Irrigazione Utilizzata' della griglia impianti
                if(listerroriGias.length === 0 &&
                  (+ multiElem.newValue.primaryKey.codice === enum_LAVCOD.FERTIRRIGAZIONE ||
                    + multiElem.newValue.primaryKey.codice === enum_LAVCOD.IRRIGAZIONE)){

                  let GridMacchine: Array<GridMacchinaModel> = this.qdcservice.MacchineFormArray.getRawValue();

                  if(GridMacchine && GridMacchine.length > 0){

                    let Macchine_Non_Compatibili_Desc: Array<string> = GridMacchine.filter((m: GridMacchinaModel)=>m.Tipo && m.Tipo.codice === "05").map(
                                                                                            (m: GridMacchinaModel)=> m.Risorsa_Des);

                    if(Macchine_Non_Compatibili_Desc && Macchine_Non_Compatibili_Desc.length > 0){

                      listerroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.WarningBloccante,
                        tipo:  enum_ErroreGias_Tipo.Generico,
                        messaggio: this.translocoService.translate("ReInserireMacchineInImpianti",
                          { Macchine_Non_Compatibili_Desc: "<br> - "+Macchine_Non_Compatibili_Desc.join("<br> -")})
                      });

                      await this.qdcservice.gestisci_ErroriGias(listerroriGias,true,true);

                      let Macchine_Compatibili: Array<GridMacchinaModel> = GridMacchine.filter((m: GridMacchinaModel)=>m.Tipo && m.Tipo.codice !== "05");

                      this.qdcservice.MacchineFormArray.clear();

                      if(Macchine_Compatibili && Macchine_Compatibili.length > 0)
                        this.qdcservice.AggiornaFormArrayMacchine(Macchine_Compatibili);
                    }
                  }
                }
            }

            if(!nuova_operazione_compatibile)
              this.qdcservice.TestataForm.patchValue({
                Operazioni: multiElem.Values.filter((v: Lavorazione)=> v.primaryKey.codice !== multiElem.newValue.primaryKey.codice)
              });

            resolve(nuova_operazione_compatibile);
        });
    }

    async openMultiOperazione(multiEl:  GiiasMultiselectTemplateSComponent) {

        let Operazioni_Selezionate: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").value;

        if (this.qdcservice.TestataForm.get("flagVisita").value && Operazioni_Selezionate.length > 0) {
            multiEl.listItems = [];
            return;
        }

        multiEl.loading = true;

        let listItems = [];

        let Opzione_Semina = null;

        //Se sto facendo una semina con frazionamento impedisco il multi operazione
        if(Operazioni_Selezionate && Operazioni_Selezionate.length > 0){
            let Operazione_con_Sementi = Operazioni_Selezionate.find(o=>this.qdcservice.Elenco_Operazioni_Sementi.includes(+ o.primaryKey.codice));

            if(Operazione_con_Sementi){
                Opzione_Semina = this.qdcservice.getOpzione_Semina_Model(Operazione_con_Sementi);

                //Recupero il valore di default dell' impostazione utente
                if(!Opzione_Semina)
                    Opzione_Semina = this.qdcservice.get_Default_Opzione_Semina([Operazione_con_Sementi]);
            }

        }

        //Con il Frazionamento o la modifica dell'anagrafica non è possibile il multi operazione
        //Non aggiungo le Operazioni già selezionate dall'utente nell'elenco di operazioni così evito che l'utente
        //le deselezioni senza il removeTagEvent
        if(!Opzione_Semina ||
            (Opzione_Semina.codice !== enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default &&
            Opzione_Semina.codice !== enum_SEMINA_TIPO.Semina_e_Modifica_Appezzamenti_Default)){
            //Alla creazione di un QdC l’utente deve poter scegliere operazioni a cui è abilitato.
            //Se entrasse in modifica invece non ci deve essere questa restrizione, perché se nel frattempo le sue impostazioni fossero diventate più restrittive o comunque diverse,
            //si troverebbe a dover editare un QdC non ammissibile per lui (con conseguenti problemi di salvataggio).

            let filtraimpostazioniutente = false;

            if(this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB === Enum_DBTypeOperation.Write)
                filtraimpostazioniutente = true;

            const params: LeggiOperazioni = {
                gruppiOperazioni: [],
                lista_Lav_Cod: Operazioni_Selezionate.map((operazione) => +operazione.primaryKey.codice),
                FiltraImpostazioniUtente: filtraimpostazioniutente,
                Visualizza_Solo_Operazioni_Preferite: this.qdcservice.TestataForm.get("Visualizza_Solo_Operazioni_Preferite").value,
                tipo_Attivita: this.qdcservice.TestataForm.get("Tipo").value,
                tipo_Ricetta: this.qdcservice.TestataForm.get("TipoRicetta").value,
                stato: this.qdcservice.TestataForm.get("Stato").value
            }

            listItems = await this.testataservice.getArray_Operazioni(params);

            if(listItems){

                //Escludo le Operazioni che non devo gestire
                listItems = listItems.filter((o:Lavorazione) =>{

                    let mostra = false;

                    let Lav_Cod = + o.primaryKey.codice;

                    if(! this.Elenco_Operazioni_da_Escludere.includes(Lav_Cod))
                        mostra = true;

                    return mostra;
                });

            }
        }

        multiEl.listItems = listItems;

        multiEl.loading = false;
    }

    async onRemoveOperazione(event: RemoveTagEvent){
        //Chiedere la conferma della cancellazione solo se è stato scelto un Prodotto

        let elimina_operazione = false;

        let Prodotto_Inserito = null

        let elem_cod:number = this.qdcservice.getCategoria_Magazzino(+(event.dataItem as Lavorazione).primaryKey.codice);

        if(elem_cod > 0){

            if(this.qdcservice.Abilitato_Inserimento_Prodotti_da_Grid_Dosi()){
                Prodotto_Inserito = this.qdcservice.Sezione_ProdottoFormGroup(event.dataItem)?.getRawValue()?.find(p=> p.Prodotto && p.Prodotto.prodotto.codice !== 0);
            }else{

                Prodotto_Inserito = this.qdcservice.DosiProdottiFormArray(null,event.dataItem)?.getRawValue()?.find(p=> p.Prodotto && p.Prodotto.prodotto.codice !== 0);
            }

        }


        if(Prodotto_Inserito){

            event.preventDefault();

            let listErroriGias: ErroreGias[] = [];

            listErroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.Warning,
                tipo:  enum_ErroreGias_Tipo.Generico,
                messaggio: this.translocoService.translate('qdc.ConfermaEliminaOperazione', { Operazione: event.dataItem.descrizione})
            });

            let r: Obj_Errore_Gias_QdC = await this.qdcservice.gestisci_ErroriGias(listErroriGias,false,false,"");

            //Conferma rimozione Operazione
            if (r.result.returnObj === true) {

                //Elimino l'Operazione

                elimina_operazione = true;

                let Operazioni_Restanti = this.qdcservice.TestataForm.get("Operazioni").value.filter(function(el) { return el !== event.dataItem; });

                this.qdcservice.TestataForm.patchValue({
                    Operazioni: Operazioni_Restanti
                });

                this.qdcservice.TestataForm.get("Operazioni").markAsTouched();

                this.qdcservice.RicaricaGridImpianti();

                this.qdcservice.RicaricaProdottiDaMagazzino();

                this.qdcservice.Gestisci_Sezioni(this.qdcservice.TestataForm.get("Operazioni").getRawValue());

                this.DisabilitabtnGroupVisualizza_Solo_Operazioni_Preferite();
            }

        }else{
            elimina_operazione = true;
        }

        //Elimino il Codice Attivita associato all'Operazione
        if(elimina_operazione){

            let Codici_Restanti = this.qdcservice.TestataForm.get("Codici_Attivita").value.filter(function(el:CodiciXOperazione) { return el.Operazione !== event.dataItem; });

            if(Codici_Restanti.length === 0){
                //recupero il Ricetta Cod e il Pua_Cod dell'operazione che sto eliminando

                let Codice_Attivita_Da_Eliminare: CodiciXOperazione = this.qdcservice.TestataForm.get("Codici_Attivita").value.find(function(el:CodiciXOperazione) { return el.Operazione.primaryKey.codice === event.dataItem.primaryKey.codice; });

                Codici_Restanti.push(new CodiciXOperazione("0",Codice_Attivita_Da_Eliminare.CodiceRicetta,"0", null,null,null,"", Codice_Attivita_Da_Eliminare.CodiceAttivitaVisita,Codice_Attivita_Da_Eliminare.pua));
            }

            this.qdcservice.TestataForm.patchValue({
                Codici_Attivita: Codici_Restanti
            });

        }

        this.DisabilitabtnGroupVisualizza_Solo_Operazioni_Preferite();

        this.MultiSelectOperazioni.toggle(false);

    }

    DisabilitabtnGroupVisualizza_Solo_Operazioni_Preferite(){

        //Disabilito il filtro delle Operazioni Preferite appena ne è stata scelta una

        let Operazioni_Scelte: Array<Lavorazione> = this.qdcservice.TestataForm?.get("Operazioni")?.value;

        if(this.qdcservice.Sola_Lettura_QdCForm() || (Operazioni_Scelte && Operazioni_Scelte.length > 0)){
            this.qdcservice.TestataForm?.get("Visualizza_Solo_Operazioni_Preferite")?.disable({ emitEvent: false });
        }else{
            this.qdcservice.TestataForm?.get("Visualizza_Solo_Operazioni_Preferite")?.enable({ emitEvent: false });
        }

    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

}
