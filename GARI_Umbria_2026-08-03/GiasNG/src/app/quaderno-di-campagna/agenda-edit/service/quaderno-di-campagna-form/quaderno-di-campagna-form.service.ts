import {DatePipe} from '@angular/common';
import {Inject, Injectable, LOCALE_ID} from '@angular/core';
import {FormArray, FormBuilder, FormControl, FormGroup} from '@angular/forms';
import {TranslocoService} from '@jsverse/transloco';
import {PKAppezzamento} from 'app/Model/anagrafiche/Appezzamento';
import {Campo, PKCampo} from 'app/Model/anagrafiche/Campo';
import {CentroAziendale, PKCentroAziendale} from 'app/Model/anagrafiche/CentroAziendale';
import {Impianto, PKImpianto} from 'app/Model/anagrafiche/Impianto';
import {Attivita, Tipo_Raccolta, Tipo_Ricetta} from 'app/Model/attivita/Attivita';
import {EsercizioCDC} from 'app/Model/attivita/centri_di_costo/EsercizioCDC';
import {DettaglioFertilizzazione} from 'app/Model/attivita/dettagli/DettaglioFertilizzazione';
import {DettaglioRaccolta} from 'app/Model/attivita/dettagli/DettaglioRaccolta';
import {DettaglioSemina} from 'app/Model/attivita/dettagli/DettaglioSemina';
import {DettaglioTrattamento, enum_Ripartizione_Trappole} from 'app/Model/attivita/dettagli/DettaglioTrattamento';
import {QuantitaSuImpianto} from 'app/Model/attivita/dettagli/QuantitaSuImpianto';
import {Lavorazione} from 'app/Model/attivita/Lavorazione';
import {NoteIntervento} from 'app/Model/attivita/note_intervento/NoteIntervento';
import {RilevamentoDiMagazzino} from 'app/Model/attivita/RilevamentoDiMagazzino';
import {Risorsa} from 'app/Model/attivita/risorse/Risorsa';
import {DoseAcqua, RisorsaAcqua} from 'app/Model/attivita/risorse/RisorsaAcqua';
import {RisorsaMacchina} from 'app/Model/attivita/risorse/RisorsaMacchina';
import {RisorsaPersona} from 'app/Model/attivita/risorse/RisorsaPersona';
import {
  AGRODATAFINE,
  AGRODATAINIZIO,
  ELEMCOD_MANODOPERA,
  FERTILIZZANTI,
  FORMULATI,
  INSETTI,
  MACCHINE, NessunaSpecieQdC,
  NessunDpi,
  NessunDpiNessunaEtichetta,
  SEMENTI,
  TRASFORMATI_VEGETALI
} from 'app/Model/CostantiPersonalizzate';
import {Disciplinare} from 'app/Model/metaschema/Disciplinari';
import {Epoca} from 'app/Model/metaschema/Epoca';
import {DestinazioneUso} from 'app/Model/metaschema/utilizzi/DestinazioneUso';
import {Varieta} from 'app/Model/metaschema/utilizzi/Varieta';
import {
  enum_doseQuantitaTotale,
  enum_LAVCOD,
  enum_OrigineApp,
  enum_PagineGiasNG,
  enum_PUARegolamenti_Tipo,
  enum_SEMINA_TIPO,
  enum_statiWorkflowQdC,
  enum_TipoMezzo,
  enum_TipoOperazioneDB
} from 'app/Model/TipiEnumerativi';
import {AgendaService, Attivita_Con_Parametri_Aggiuntivi} from 'app/Service/Agenda/Agenda.service';
import {FunzioniComuniService} from 'app/Service/FunzioniComuni.service';
import {GestioneRichiesteService} from 'app/Service/gestione-richieste.service';
import { ObjParametriAgendaService} from 'app/Service/obj-parametri-agenda.service';
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {from, Observable, of} from 'rxjs';
import {NotaInterventoDdlItem} from '../../componenti/grid-note/note.model';
import {
  enum_Generazione_Lotto_Raccolta,
  enum_Ripartizione_Raccolta,
  normalizeOpzioniRaccolta,
  OpzioniRaccolta
} from '../../componenti/prodotti/sezioni/raccolta/opzioni-raccolta/opzioni-raccolta.model';
import {
  Acqua,
  Causale,
  CodiciXOperazione,
  CostiAccessori,
  DropdownListAttivitaPersonalizzata,
  DropdownListCampo,
  DropdownListDisciplinare,
  enum_Problema_DettaglioProdotto,
  GridImpiantoSelezionatoModel,
  GridMacchinaModel,
  GridOperatoreModel,
  Key_Parametri_Aggiuntivi, MultiColumnComboboxAvversitaInnesco,
  MultiColumnComboboxFertilizzazione,
  MultiColumnComboboxSemina,
  MultiColumnComboboxTrattamento,
  QdCFormModel,
  Sezione_Prodotto_Fertilizzanti,
  Sezione_Prodotto_Formulati,
  Sezione_Prodotto_Raccolta,
  Sezione_Prodotto_Sementi,
  Sezione_Rilievi,
  Superfici,
  Testata_Visita
} from '../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import {GridDosiProdottiService} from '../grid-dosi-prodotti/grid-dosi-prodotti.service';
import {QdCService, Redirect_To_GiasNG_Page} from '../qdc.service';
import {Specie} from "../../../../Model/metaschema/utilizzi/Specie";
import {QdCTestataService} from "../testata/testata.service";
import {DettaglioRilievo} from '../../../../Model/attivita/dettagli/DettaglioRilievo';
import {Elenco_Opzioni_Semina} from "../../componenti/prodotti/controlli-comuni/opzioni-semina/elenco-opzioni-semina";
import {RisorsaProdotto} from "../../../../Model/attivita/risorse/RisorsaProdotto";
import {MisceleService} from "../miscele.service";
import {cloneDeep} from "lodash";
import {Fabbricato} from '../../../../Model/anagrafiche/Fabbricato';
import {ActivatedRoute} from '@angular/router';
import {PermessiUtenteService} from 'app/Service/permessi-utente.service';
import {RisorsaUmanaVisita} from 'app/Model/anagrafiche/RisorsaUmanaVisita';
import {Operatore} from 'app/Model/metaschema/utilizzi/Operatore';
import {Impresa} from 'app/Model/anagrafiche/Impresa';
import {RisorsaZootecnica} from 'app/Model/attivita/risorse/RisorsaZootecnica';
import {RisorsaSpecie} from 'app/Model/attivita/risorse/RisorsaSpecie';
import {RisorsaDestinazioneUso} from 'app/Model/attivita/risorse/RisorsaDestinazioneUso';
import {ConversionService} from "../../../../Service/conversion.service";
import {Pua} from "../../../../Model/metaschema/Pua";
import {Enum_SiteRedirector} from "../../../../Model/siti.enum";
import {DropdownListSpecieAnimali, SpecieAnimaliService} from '../testata/specie-animali-service';
import {enum_Impostazioni_Utenti} from 'app/Model/Impostazioni_Utenti.enum';
import {RibaltamentoTypes} from "../../../../menu-agenda/components/utils";
import {Tipo_Blocco} from "../../../../Model/attivita/Blocco";
import { ProdottoDaTrattareCDC } from 'app/Model/attivita/centri_di_costo/ProdottoDaTrattareCDC';
import { RisorsaCausale } from 'app/Model/attivita/risorse/RisorsaCausale';
import { ObjParametriAgenda, Stati, Tipo_Attivita } from 'gias-ui-kit';
import {BaseCodeDescr} from "../../../../Model/baseClass/baseCodeDescr";
import {AvversitaGruppo} from "../../../../Model/metaschema/avversita/AvversitaGruppo";
import {DettaglioIrrigazione, TipoIrrigazione} from 'app/Model/attivita/dettagli/DettaglioIrrigazione';
import {ParcoMacchine} from "../../../../Model/anagrafiche/ParcoMacchine";
import {UnitaDiMisura} from "../../../../Model/metaschema/UnitaDiMisura";

@Injectable()

export class QdCFormService {

    QdCForm: FormGroup;

    objParametriAgenda: ObjParametriAgenda;

    constructor(private objParametriAgendaService: ObjParametriAgendaService,
                private qdcservice: QdCService,
                private agendaService: AgendaService,
                private griddosiprodottiservice: GridDosiProdottiService,
                private funzionicomuniservice: FunzioniComuniService,
                private translocoService: TranslocoService,
                private datePipe: DatePipe,
                private fb: FormBuilder,
                @Inject(LOCALE_ID) private locale_id: string,
                private gestionerichiesteservice: GestioneRichiesteService,
                private testataservice: QdCTestataService,
                private misceleservice: MisceleService,
                private route: ActivatedRoute,
                private permessiUtenteService: PermessiUtenteService,
                private conversionService: ConversionService,
                private specieAnimaliService: SpecieAnimaliService)
    {  }


    getQdCFormValue(): Observable<QdCFormModel> {

        this.objParametriAgenda=this.objParametriAgendaService.getObjParamValue();

        let ObsQdCFormModel: Observable<QdCFormModel>;

        if(this.objParametriAgenda.TipoOperazioneAgenda === Tipo_Attivita.QuadernoDiCampagna){

            switch(this.objParametriAgenda.TipoOperazioneDB){
                case Enum_DBTypeOperation.Write:

                    //Se Ricetta_Operazione_Cod è valorizzato ma il TipoOperazioneAgenda è Quaderno di Campagna
                    //e TipoOperazioneDB è scrittura allora sto cercando di salvare una agenda partendo da un brogliaccio/ricetta
                    if((this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda ||
                        this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_ad_Agenda) &&
                        this.objParametriAgenda.Ricetta_Operazione_Cod !== 0){

                        ObsQdCFormModel = from(this.agendaService.LeggiListaAttivitaDaRicettaOperazionePerAgendaWS(this.objParametriAgenda.Ricetta_Operazione_Cod).then((listaAttivita_con_parametri_aggiuntivi: Attivita_Con_Parametri_Aggiuntivi[]) => {
                                this.qdcservice.setListaAttivita_Con_Parametri_Aggiuntivi(listaAttivita_con_parametri_aggiuntivi);
                                return this.mapListaAttivitaToQdCFormModel(listaAttivita_con_parametri_aggiuntivi);
                            })
                        );

                    }else if(this.qdcservice.flag_Reinnesco){

                      ObsQdCFormModel = from(this.agendaService.LeggiListaAttivitaWS(this.objParametriAgenda.Piva,
                          this.objParametriAgenda.Id_Agenda).then(listaAttivita => {

                          let listaAttivita_con_parametri_aggiuntivi = listaAttivita as Attivita_Con_Parametri_Aggiuntivi[];

                          this.qdcservice.setListaAttivita_Con_Parametri_Aggiuntivi(listaAttivita_con_parametri_aggiuntivi);

                          return this.mapListaAttivitaToQdCFormModel(listaAttivita_con_parametri_aggiuntivi);
                        })
                      );

                    }else{
                        let qdcformModel: QdCFormModel = this.setQdCFormModelfromGenericObj();

                        if(!qdcformModel){
                            ObsQdCFormModel = from(this.setNewQdCFormModel());
                        }else{
                            ObsQdCFormModel = of(qdcformModel);
                        }
                    }

                    break;
                case Enum_DBTypeOperation.Update:
                case Enum_DBTypeOperation.Read:

                    //Leggo i dati su db dell'id_agenda scelta
                    ObsQdCFormModel = from(this.agendaService.LeggiListaAttivitaWS(this.objParametriAgenda.Piva,
                            this.objParametriAgenda.Id_Agenda).then(listaAttivita => {

                            let listaAttivita_con_parametri_aggiuntivi = listaAttivita as Attivita_Con_Parametri_Aggiuntivi[];

                            this.qdcservice.setListaAttivita_Con_Parametri_Aggiuntivi(listaAttivita_con_parametri_aggiuntivi);

                            return this.mapListaAttivitaToQdCFormModel(listaAttivita_con_parametri_aggiuntivi);
                        })
                    );
                    break;
            }

        }else if(this.objParametriAgenda.TipoOperazioneAgenda === Tipo_Attivita.Ricetta){

            switch(this.objParametriAgenda.TipoOperazioneDB){
                case Enum_DBTypeOperation.Write:

                    //Se Ricetta_Operazione_Cod è valorizzato ma il TipoOperazioneAgenda è Ricetta
                    //e TipoOperazioneDB è scrittura allora sto cercando di salvare una brogliaccio partendo da una ricetta
                    if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_a_Brogliaccio &&
                        this.objParametriAgenda.Ricetta_Operazione_Cod !== 0){

                        ObsQdCFormModel = from(this.agendaService.LeggiListaAttivitaDaRicettaOperazionePerBrogliaccioWS(this.objParametriAgenda.Ricetta_Operazione_Cod).then(listaAttivita => {
                                let listaAttivita_con_parametri_aggiuntivi = listaAttivita as Attivita_Con_Parametri_Aggiuntivi[];

                                this.qdcservice.setListaAttivita_Con_Parametri_Aggiuntivi(listaAttivita_con_parametri_aggiuntivi);
                                return this.mapListaAttivitaToQdCFormModel(listaAttivita_con_parametri_aggiuntivi);
                            })
                        );
                    }else{
                        let qdcformModel: QdCFormModel = this.setQdCFormModelfromGenericObj();

                        if(!qdcformModel){
                            ObsQdCFormModel = from(this.setNewQdCFormModel());
                        }else{
                            ObsQdCFormModel = of(qdcformModel);
                        }
                    }

                    break;
                case Enum_DBTypeOperation.Update:
                case Enum_DBTypeOperation.Read:
                    //Leggo i dati su db della ricetta_operazione_cod scelta
                    ObsQdCFormModel = from(this.agendaService.LeggiListaAttivitaDaRicettaOperazionePerRicettaWS(this.objParametriAgenda.Ricetta_Operazione_Cod).then(listaAttivita => {

                            let listaAttivita_con_parametri_aggiuntivi = listaAttivita as Attivita_Con_Parametri_Aggiuntivi[];

                            this.qdcservice.setListaAttivita_Con_Parametri_Aggiuntivi(listaAttivita_con_parametri_aggiuntivi);
                            return this.mapListaAttivitaToQdCFormModel(listaAttivita_con_parametri_aggiuntivi);
                        })
                    );
                    break;
            }

        }

        return ObsQdCFormModel;

    }

    setTestataVisitaModel(listaAttivita: Attivita[], qdCFormModel: QdCFormModel) {

        if (this.objParametriAgenda.Lav_Cod === enum_LAVCOD.VISITA) {   //aggiunto set del flag
            qdCFormModel.Trattamento.Testata.flagVisita = true;

            qdCFormModel.Trattamento.Testata.TestataVisita = new Testata_Visita();

            qdCFormModel.Trattamento.Testata.TestataVisita.impOrarioFine_Visita = (this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE)) ? +this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE).Valore : 1;
            let nrOreVisita = +this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_NR_ORE_VISITA).Valore;
            qdCFormModel.Trattamento.Testata.TestataVisita.NrOreTotali_Visita = nrOreVisita;

            if (this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Update) {
                qdCFormModel.Trattamento.Testata.TestataVisita.Ora_Inizio_Visita = listaAttivita[0].oraInizio as Date;
                qdCFormModel.Trattamento.Testata.TestataVisita.Ora_Fine_Visita = listaAttivita[0].oraFine as Date;

                if (qdCFormModel.Trattamento.Testata.TestataVisita.impOrarioFine_Visita === 0) {
                    const differenzaInMillisecondi = Math.abs(listaAttivita[0].oraFine.getTime() - listaAttivita[0].oraInizio.getTime());
                    qdCFormModel.Trattamento.Testata.TestataVisita.NrOreTotali_Visita = differenzaInMillisecondi / (1000 * 60 * 60);
                }

                qdCFormModel.Trattamento.Testata.TestataVisita.Da_Remoto_Visita = listaAttivita[0].daRemoto;
                qdCFormModel.Trattamento.Testata.TestataVisita.statoWorkflow_Visita = (listaAttivita[0].statoWorkflow === enum_statiWorkflowQdC.Da_Eseguire) ? false : true;
                let aziendaVisita = new Impresa();
                aziendaVisita.partitaIva = listaAttivita[0].centroAziendale.primaryKey.partitaIva;
                qdCFormModel.Trattamento.Testata.TestataVisita.Azienda_Visita = aziendaVisita;

            } else {
                qdCFormModel.Trattamento.Testata.TestataVisita.Ora_Inizio_Visita = new Date();
                qdCFormModel.Trattamento.Testata.TestataVisita.Ora_Fine_Visita = new Date();
                qdCFormModel.Trattamento.Testata.TestataVisita.Ora_Fine_Visita.setHours(qdCFormModel.Trattamento.Testata.TestataVisita.Ora_Inizio_Visita.getHours() + nrOreVisita);
                qdCFormModel.Trattamento.Testata.TestataVisita.Ora_Fine_Visita.setMinutes(qdCFormModel.Trattamento.Testata.TestataVisita.Ora_Inizio_Visita.getMinutes() + (nrOreVisita*60)%60);
                qdCFormModel.Trattamento.Testata.Attivita_Collegate = [];
                let impStatoVisita = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_STATO_VISITA);
                qdCFormModel.Trattamento.Testata.TestataVisita.statoWorkflow_Visita = (impStatoVisita) ? ((+impStatoVisita.Valore === 0) ? false : true ) : false;
            }
        }else{
          qdCFormModel.Trattamento.Testata.flagVisita = false;
        }
    }

    completeQdCFormModel(attivita: Attivita_Con_Parametri_Aggiuntivi, qdCFormModel: QdCFormModel, listaAttivita: Attivita_Con_Parametri_Aggiuntivi[]) {
        const lavorazione=new Lavorazione(attivita.job.primaryKey.codice);

        lavorazione.descrizione=attivita.job.descrizione;

        const elem_cod = this.qdcservice.getCategoria_Magazzino(+ lavorazione.primaryKey.codice);

        if (attivita.risorse) {

            let risorse: Risorsa[] = null;

            if (!qdCFormModel.Trattamento.Testata.flagVisita) {     //se arrivo dal menu Visite, non popolo le griglie degli operatori e delle macchine

                risorse = attivita.risorse.filter((risorsa: Risorsa)=>{ return risorsa.classType === "RisorsaMacchina" });

                if(risorse && risorse.length > 0){
                    risorse.forEach((risorsa: Risorsa)=>{
                        let risorsaMacchina = risorsa as RisorsaMacchina;
                        this.setGridMacchineModel(risorsaMacchina,lavorazione, qdCFormModel);
                    });
                }

                risorse = attivita.risorse.filter((risorsa: Risorsa)=>{ return risorsa.classType === "RisorsaPersona" });

                if(risorse && risorse.length > 0){
                    risorse.forEach((risorsa: Risorsa)=>{
                        let risorsaPersona = risorsa as RisorsaPersona;
                        this.setGridOperatoriModel(risorsaPersona, qdCFormModel);
                    });
                }

            }

            risorse = attivita.risorse.filter((risorsa: Risorsa)=>{ return risorsa.classType === "DettaglioRilievo" });

            if(risorse && risorse.length > 0){
                risorse.forEach((risorsa: Risorsa)=>{
                    let risorsaDettaglioRilievo = risorsa as DettaglioRilievo;
                    risorsaDettaglioRilievo.id_agenda = attivita.codice;
                    this.setSezioneSenzaProdottoModel(risorsaDettaglioRilievo,
                        qdCFormModel, lavorazione,
                        attivita
                    );
                });
            }

            risorse = attivita.risorse.filter((risorsa: Risorsa)=>{ return risorsa.classType === "DettaglioTrattamento" });

            if(risorse && risorse.length > 0){
                risorse.forEach((risorsa: Risorsa)=>{
                    let risorsaDettaglioTrattamento = risorsa as DettaglioTrattamento;
                    this.setSezioneProdottoModel(risorsaDettaglioTrattamento,
                        qdCFormModel,
                        lavorazione,
                        attivita.centroAziendale,
                        elem_cod,
                        attivita.epoca,
                        attivita.disciplinare,
                        attivita.modalitaApplicazione,
                        attivita
                    );
                });
            }

            risorse = attivita.risorse.filter((risorsa: Risorsa)=>{ return risorsa.classType === "DettaglioFertilizzazione" });

            if(risorse && risorse.length > 0){
                risorse.forEach((risorsa: Risorsa)=>{
                    let risorsaDettaglioFertilizzazione = risorsa as DettaglioFertilizzazione;
                    this.setSezioneProdottoModel(risorsaDettaglioFertilizzazione,
                        qdCFormModel,
                        lavorazione,
                        attivita.centroAziendale,
                        elem_cod,
                        attivita.epoca,
                        attivita.disciplinare,
                        attivita.modalitaApplicazione,
                        attivita
                    );
                });
            }

            risorse = attivita.risorse.filter((risorsa: Risorsa)=>{ return risorsa.classType === "DettaglioSemina" });

            if(risorse && risorse.length > 0){
                risorse.forEach((risorsa: Risorsa)=>{
                    let risorsaDettaglioSemina = risorsa as DettaglioSemina;
                    this.setSezioneProdottoModel(risorsaDettaglioSemina,
                        qdCFormModel,
                        lavorazione,
                        attivita.centroAziendale,
                        elem_cod,
                        attivita.epoca,
                        attivita.disciplinare,
                        attivita.modalitaApplicazione,
                        attivita
                    );
                });
            }else{
                if(elem_cod === SEMENTI){
                    //Per le Semine l'Operazione si può fare anche senza prodotti quindi mostro la sezione comunque
                    this.setSezioneProdottoModel(null,
                        qdCFormModel,
                        lavorazione,
                        attivita.centroAziendale,
                        elem_cod,
                        attivita.epoca,
                        attivita.disciplinare,
                        attivita.modalitaApplicazione,
                        attivita
                    );
                }

            }

            risorse = attivita.risorse.filter((risorsa: Risorsa)=>{ return risorsa.classType === "DettaglioRaccolta" });

            if(risorse && risorse.length > 0){
                console.log(risorse)
                risorse.forEach((risorsa: Risorsa)=>{
                    let risorsaDettaglioRaccolta = risorsa as DettaglioRaccolta;
                    this.setSezioneProdottoModel(risorsaDettaglioRaccolta,
                        qdCFormModel,
                        lavorazione,
                        attivita.centroAziendale,
                        elem_cod,
                        null,
                        attivita.disciplinare,
                        attivita.modalitaApplicazione,
                        attivita
                    );
                });
            } else {
                this.handleRaccoltaFast(qdCFormModel, lavorazione,elem_cod, attivita,listaAttivita)
            }

            //gestisco le causali
            risorse = attivita.risorse.filter((risorsa: Risorsa)=>{ return risorsa.classType === "RisorsaCausale" });

            if (risorse && risorse.length > 0) {
                risorse.forEach((risorsa: Risorsa)=>{
                    let risorsaCausale= risorsa as RisorsaCausale;
                    this.setCausaliModel(risorsaCausale, qdCFormModel);
                });
            }
        }

        if (attivita.noteIntervento)
            this.setGridNoteModel(attivita.noteIntervento,qdCFormModel);

        //Prendo la nota testuale la prima volta se non è valorizzata
        if(!qdCFormModel.Trattamento.Nota_Testuale || qdCFormModel.Trattamento.Nota_Testuale === "")
            qdCFormModel.Trattamento.Nota_Testuale = attivita.note;
    }




    private mapListaAttivitaToQdCFormModel(listaAttivita_con_parametri_aggiuntivi: Attivita_Con_Parametri_Aggiuntivi[],crea_sezione_prodotti: boolean = false): QdCFormModel {

        //I dati comuni come gli impianti le macchine e gli operatori sono comuni per tutte le attivita

        let qdCFormModel= new QdCFormModel();

        if(this.gestionerichiesteservice.ObjPageToMemorize &&
            this.gestionerichiesteservice.ObjPageToMemorize.QdCFormValue && this.gestionerichiesteservice.ObjPageToMemorize.objParametriAgenda){

            let ObjPageToMemorize: Redirect_To_GiasNG_Page = this.gestionerichiesteservice.ObjPageToMemorize;

        }else{

            this.QdCForm = this.qdcservice.QdCForm;

            if(listaAttivita_con_parametri_aggiuntivi){

                // this.setTestataVisitaModel(listaAttivita_con_parametri_aggiuntivi, qdCFormModel);

                // if (!qdCFormModel.Trattamento.Testata.flagVisita) {

                    this.setTestataModel(listaAttivita_con_parametri_aggiuntivi,null,qdCFormModel,Enum_DBTypeOperation.Update);
                    this.setGridImpiantiSelezionatiModel(listaAttivita_con_parametri_aggiuntivi,qdCFormModel);

                    //TODO Da chiarire bene come gestire il calcolo dell'acqua in una lista di attivita
                    //per ora prendo l'acqua dalla prima attivita
                    this.setAcquaModel(listaAttivita_con_parametri_aggiuntivi, qdCFormModel);
                // }

                listaAttivita_con_parametri_aggiuntivi.forEach((attivita: Attivita_Con_Parametri_Aggiuntivi,index: number) => {

                    // if (!qdCFormModel.Trattamento.Testata.flagVisita) {
                        this.completeQdCFormModel(attivita, qdCFormModel, listaAttivita_con_parametri_aggiuntivi);
                    // } else {

                        if(attivita.risorse && qdCFormModel.Trattamento.Testata.flagVisita) {

                            let operatoreVisita = attivita.risorse.filter((risorsa: Risorsa)=>{ return risorsa.classType === "RisorsaAssegnatarioVisita" });

                            if(operatoreVisita && operatoreVisita.length > 0){
                                operatoreVisita.forEach((risorsa: Risorsa)=>{
                                    let risorsaPersona = risorsa as RisorsaUmanaVisita;
                                    let OperatoreSelected = new Operatore("",
                                        risorsaPersona.risorsaUmana.contatto.nome + " " + risorsaPersona.risorsaUmana.contatto.cognome,
                                        risorsaPersona.risorsaUmana.codice,
                                        risorsaPersona.risorsaUmana.contatto.primaryKey.codice);
                                    qdCFormModel.Trattamento.Testata.TestataVisita.Operatore_Visita = OperatoreSelected;
                                });
                            }

                            let specieVisita = attivita.risorse.filter((risorsa: Risorsa)=>{ return (risorsa.classType === "RisorsaSpecie" || risorsa.classType === "RisorsaDestinazioneUso") });

                            if (specieVisita && specieVisita.length > 0) {
                                specieVisita.forEach((risorsa: Risorsa)=>{
                                    switch (risorsa.classType) {
                                        case "RisorsaSpecie":
                                            const risSpecie = risorsa as RisorsaSpecie;
                                            qdCFormModel.Trattamento.Testata.Specie={codice: risSpecie.specie.codice, descrizione: risSpecie.specie.descrizione};
                                        break;

                                        case "RisorsaDestinazioneUso":
                                            const risDestUso = risorsa as RisorsaDestinazioneUso;
                                            qdCFormModel.Trattamento.Testata.Specie={codice: risDestUso.destinazioneUso.codice, descrizione: risDestUso.destinazioneUso.descrizione, classType:"DestinazioneUso"};
                                        break;
                                    }
                                });
                            }

                            let risorsaZootecnica = attivita.risorse.filter((risorsa: Risorsa)=>{ return risorsa.classType === "RisorsaZootecnica" });

                            if (risorsaZootecnica && risorsaZootecnica.length > 0) {

                                risorsaZootecnica.forEach((risorsa: RisorsaZootecnica) => {
                                    qdCFormModel.Trattamento.Testata.TestataVisita.SpecieAnimali_Visita = this.specieAnimaliService.setCodDescrDDLSpecieAnimale(risorsa as DropdownListSpecieAnimali);
                                });

                            }

                            // let dettagliVisita = (attivita.risorse.filter((val) => val.classType == "DettaglioVisita")) as DettaglioVisita[];
                            // if (dettagliVisita) {
                            //     dettagliVisita.forEach((d) => {
                            //         if(d.AttivitaCollegate) {

                            //             this.setTestataModel(d.AttivitaCollegate,null,qdCFormModel,Enum_DBTypeOperation.Update, attivita.codice);
                            //             this.setGridImpiantiSelezionatiModel(d.AttivitaCollegate,qdCFormModel);
                            //             this.setAcquaModel(d.AttivitaCollegate, qdCFormModel);

                            //             d.AttivitaCollegate.forEach((a) => {
                            //                 this.completeQdCFormModel(a as Attivita_Con_Parametri_Aggiuntivi, qdCFormModel, d.AttivitaCollegate as Attivita_Con_Parametri_Aggiuntivi[]);
                            //             });
                            //         }
                            //     });
                            // }
                        }
                    // }
                });
            }
        }

        this.Gestisci_Qta_Prodotto_x_MultiCentro(qdCFormModel,listaAttivita_con_parametri_aggiuntivi);

        this.setFormArrayinQdCForm(qdCFormModel,crea_sezione_prodotti);

        // Reimposto la QdC Form anche nel service per essere reperibile
        // anche negli altri componenti/servizi.
        this.qdcservice.QdCForm = this.QdCForm;

        return qdCFormModel;
    }


    private handleRaccoltaFast(qdCFormModel: QdCFormModel,
                               lavorazione: Lavorazione,
                               elem_cod: number,
                               attivita: Attivita_Con_Parametri_Aggiuntivi,
                               listaAttivita: Attivita_Con_Parametri_Aggiuntivi[]) {

        if (+ lavorazione.primaryKey.codice !== enum_LAVCOD.RACCOLTA
            || attivita.tipoRaccolta !== Tipo_Raccolta.Fast) return;

        let risorsaDettaglioRaccolta = new DettaglioRaccolta();

        risorsaDettaglioRaccolta.TipoRaccolta = attivita.tipoRaccolta;
        risorsaDettaglioRaccolta.specie = attivita.utilizzoTerreno['specie'];
        risorsaDettaglioRaccolta.quantitaTotaleReale = 0;
        risorsaDettaglioRaccolta.QuantitaSuImpianti = attivita.centriDiCosto
            .map(cdc => {
                let qsi = new QuantitaSuImpianto()
                qsi.Qta = 0;
                qsi.esercizioCDC = cdc as EsercizioCDC;
                return qsi;
            })

        this.setSezioneProdottoModel(risorsaDettaglioRaccolta,
            qdCFormModel, lavorazione,attivita.centroAziendale, elem_cod, null, attivita.disciplinare,null,attivita
        );
    }

    private setNewQdCFormModel(): Promise<QdCFormModel> {

        return new Promise<QdCFormModel>(async (resolve, reject)=>{
            let qdCFormModel=new QdCFormModel();

            if(this.gestionerichiesteservice.ObjPageToMemorize &&
              this.gestionerichiesteservice.ObjPageToMemorize.QdCFormValue && this.gestionerichiesteservice.ObjPageToMemorize.objParametriAgenda){

                let ObjPageToMemorize: Redirect_To_GiasNG_Page = this.gestionerichiesteservice.ObjPageToMemorize;

                if(ObjPageToMemorize.QdCFormValue){

                    this.QdCForm = this.qdcservice.QdCForm;

                    if((this.objParametriAgenda.Sito_Provenienza !== Enum_SiteRedirector.GiasNG) ||
                        (this.objParametriAgenda.Sito_Provenienza === Enum_SiteRedirector.GiasNG && this.objParametriAgenda.Pagina_Provenienza !== enum_PagineGiasNG.Pagina_Menu_Agenda && this.objParametriAgenda.Pagina_Provenienza !== enum_PagineGiasNG.Pagina_Edit_Attivita) ||
                        (this.objParametriAgenda.Sito_Provenienza === Enum_SiteRedirector.GiasNG && this.objParametriAgenda.Pagina_Provenienza !== enum_PagineGiasNG.Pagina_Menu_Visite && this.objParametriAgenda.Pagina_Provenienza !== enum_PagineGiasNG.Pagina_Edit_Visite)){

                        this.qdcservice.Apertura_QdC ={
                            flag_QdC_Aperta_da_Altra_Pagina: true,
                            flag_Selezione_Impianti: true
                        };

                    }

                    qdCFormModel = ObjPageToMemorize.QdCFormValue;

                    //Per la nuova operazione CONFUSIONE_DISORIENTAMENTO_SESSUALE e per TRATTAMENTO_POST_RACCOLTA anche se non è visibile il disciplinare lo imposto
                    //comunque a Solo Etichetta perchè lato server verrà gestito in questo modo
                    if(qdCFormModel?.Trattamento?.Testata?.Operazioni?.length === 1 &&
                        qdCFormModel?.Trattamento?.Testata?.Operazioni?.findIndex(o=>this.qdcservice.Elenco_Operazioni_Con_Default_DPI.includes(+ o.primaryKey.codice) ) > -1){

                        qdCFormModel.Trattamento.Testata.Disciplinare = this.qdcservice.setCodDescrDdlDisciplinare(this.qdcservice.Obj_NessunDpi as DropdownListDisciplinare);
                    }else{
                        //Imposto il disciplinare a Obj_NessunDpi se valorizzata la Specie come faccio nella funzione di CambioSpecie
                        if(qdCFormModel?.Trattamento?.Testata?.Specie &&
                            !qdCFormModel?.Trattamento?.Testata?.Disciplinare &&
                            qdCFormModel?.Trattamento?.Testata?.Operazioni?.findIndex(o=>{ return this.qdcservice.MostraDisciplinare(o,this.qdcservice.Sezioni_ProdottoFormArray); }) > -1){

                            if(qdCFormModel?.Trattamento?.Testata?.Specie instanceof Specie){
                                qdCFormModel.Trattamento.Testata.Disciplinare = this.qdcservice.setCodDescrDdlDisciplinare(this.qdcservice.Obj_NessunDpi as DropdownListDisciplinare);
                            }else{
                                qdCFormModel.Trattamento.Testata.Disciplinare = this.qdcservice.setCodDescrDdlDisciplinare(this.qdcservice.Obj_NessunDpiNessunaEtichetta as DropdownListDisciplinare);
                            }

                        }
                    }


                    this.setFormArrayinQdCForm(qdCFormModel,true);
                }

            }else{

                this.QdCForm = this.qdcservice.QdCForm;

                let lavorazione: Lavorazione = null;

                //Entro dentro la pagina senza aver un'operazione selezionata
                if(this.objParametriAgenda.Lav_Cod !== 0 && this.objParametriAgenda.Lav_Des !== ""){
                    lavorazione = new Lavorazione(this.objParametriAgenda.Lav_Cod.toString());
                    lavorazione.descrizione = this.objParametriAgenda.Lav_Des;
                }

                this.setTestataModel(null,lavorazione,qdCFormModel,Enum_DBTypeOperation.Write);
                this.setFormArrayinQdCForm(qdCFormModel,true);

                // Reimposto la QdC Form anche nel service per essere reperibile
                // anche negli altri componenti/servizi.
                this.qdcservice.QdCForm = this.QdCForm;
            }

            //Imposto la Specie se non è valorizzata da quella memorizzata nei cookie (cioè l'ultima specie utilizzata da quell'utente).
            //Il cookie ha una validità di un giorno di default
            if(!qdCFormModel.Trattamento.Testata.Specie || qdCFormModel.Trattamento.Testata.Specie.codice === -1){
                let defaultSpecie;

                if (!qdCFormModel.Trattamento.Testata.flagVisita)
                    defaultSpecie = this.qdcservice.getDefaultSpecieFromCookie();

                if(defaultSpecie && defaultSpecie.codice !== -1){
                    let index =(await this.testataservice.getArray_UtilizziTerreno()).findIndex(u=>u.codice === defaultSpecie.codice);

                    if(index > -1)
                        qdCFormModel.Trattamento.Testata.Specie = defaultSpecie

                }else{
                    qdCFormModel.Trattamento.Testata.Specie={codice: -1, descrizione:this.translocoService.translate("NessunaSelezione")};
                }
            }

            resolve(qdCFormModel);
        });

    }



    private setTestataModel(
        listaAttivita: Attivita_Con_Parametri_Aggiuntivi[],
        lavorazione: Lavorazione,
        qdCFormModel: QdCFormModel,
        TipoOperazioneDB: Enum_DBTypeOperation
    ): void{

        const Default_Data: Date = new Date(new Date().setHours(0, 0, 0, 0));

        let centro_aziendale: CentroAziendale = this.qdcservice.Obj_TuttiICentriAziendali;

        this.setTestataVisitaModel(listaAttivita, qdCFormModel);        //chiama questa funzione che setta il flag in modo da far comparire la seconda testata
                                                                        //anche quando si apre il qdc per una Nuova Visita

        let CodiceAttivitaVisita = this.get_CodiceVisita_from_Parametri_Aggiuntivi(listaAttivita);

        //Fatto in questo modo perché nel ribaltamento da brogliaccio -> ad agenda è come se siamo in una nuova Operazione di Agenda,
        //ma in realtà il modello Attivita ce lo abbiamo valorizzato dalla lettura delle tabelle delle ricette
        if(listaAttivita) {

            this.resetForReinnesco(listaAttivita,Default_Data);

            //Se mi vengono restituiti più centri aziendali allora sono nel caso di multicentro
            const centri_aziendali_x_attivita =  [...new Map(listaAttivita.filter(item=>item.centroAziendale && item.centroAziendale.primaryKey.codice > 0 && item.centroAziendale.primaryKey.partitaIva !== "")
                                                                            .map(item => [item.centroAziendale.primaryKey.codice, item])).values()];

            if(centri_aziendali_x_attivita && centri_aziendali_x_attivita.length === 1) {
                centro_aziendale = centri_aziendali_x_attivita[0].centroAziendale;
            }

            listaAttivita.forEach((attivita: Attivita)=> {

                const lavorazioneattivita= new Lavorazione(attivita.job.primaryKey.codice);

                lavorazioneattivita.descrizione = attivita.job.descrizione;

                //Se sono in una agenda l'attivita.codice è l'id_agenda se
                //sono in una ricetta l'attivita.codice il Ricetta_Operazione_Cod

                let CodiceAttivita: string = "0";
                let CodiceRicetta: string = "0";
                let CodiceOperazioneRicetta: string = "0";
                let pua: Pua = null;

                switch(attivita.tipo) {
                    case Tipo_Attivita.QuadernoDiCampagna:
                        CodiceAttivita = attivita.codice;
                        CodiceOperazioneRicetta = "0";
                        CodiceRicetta = "0";
                        break;
                    case Tipo_Attivita.Ricetta:
                        CodiceAttivita = "0";
                        CodiceOperazioneRicetta = attivita.codice;
                        CodiceRicetta = attivita.testataRicetta.Ricetta_Cod.toString();

                        if(attivita.tipoRicetta === Tipo_Ricetta.PianoDistribuzionePua &&
                            attivita.stato === Stati.Da_Eseguire){

                            pua = attivita.testataRicetta.pua;
                        }

                        break;
                }

                //Se ho aperto in modifica un'attivita allora valorizzo anche la proprietà centro_Aziendale altrimenti la lascio a null
                // if(centro_aziendale.primaryKey.codice === this.qdcservice.Obj_TuttiICentriAziendali.primaryKey.codice) {
                //
                //     qdCFormModel.Trattamento.Testata.Codici_Attivita.push(
                //         new CodiciXOperazione(
                //             CodiceAttivita,
                //             CodiceRicetta,
                //             CodiceOperazioneRicetta,
                //             lavorazioneattivita,
                //             attivita.associazionePK,
                //             attivita.centroAziendale,
                //             attivita.appRicettaOperazioneID,
                //             CodiceAttivitaVisita,
                //             pua
                //         )
                //     );
                // } else {
                //     qdCFormModel.Trattamento.Testata.Codici_Attivita.push(
                //         new CodiciXOperazione(
                //             CodiceAttivita,
                //             CodiceRicetta,
                //             CodiceOperazioneRicetta,
                //             lavorazioneattivita,
                //             attivita.associazionePK,
                //             attivita.centroAziendale,
                //             attivita.appRicettaOperazioneID,
                //             CodiceAttivitaVisita,
                //             pua
                //         )
                //     );
                // }
              qdCFormModel.Trattamento.Testata.Codici_Attivita.push(
                new CodiciXOperazione(
                  CodiceAttivita,
                  CodiceRicetta,
                  CodiceOperazioneRicetta,
                  lavorazioneattivita,
                  attivita.associazionePK,
                  attivita.centroAziendale,
                  attivita.appRicettaOperazioneID,
                  CodiceAttivitaVisita,
                  pua
                )
              );

                if(qdCFormModel.Trattamento.Testata.Operazioni.findIndex(o=>o.primaryKey.codice === lavorazioneattivita.primaryKey.codice) === -1){
                    qdCFormModel.Trattamento.Testata.Operazioni.push(lavorazioneattivita);
                }

                if(attivita.blocco && attivita.blocco.tipo === Tipo_Blocco.QuadernoDiCampagna){
                  qdCFormModel.Trattamento.Testata.Blocco_Attivita = attivita.blocco;
                }
            })

        } else {
            qdCFormModel.Trattamento.Testata.Codici_Attivita.push(
                new CodiciXOperazione(
                    "0",
                    "0",
                    "0",
                    lavorazione,
                    null,
                    null,
                    "",
                    "",
                    null
                )
            );

            if(lavorazione && + lavorazione.primaryKey.codice > 0) {
                qdCFormModel.Trattamento.Testata.Operazioni.push(lavorazione);
            }
        }

        //Se sono nel caso delle Visita aggiungo nel CodiceAttivita l'operazione vista
        if(qdCFormModel.Trattamento.Testata.flagVisita){

          let OpVisita = new Lavorazione(enum_LAVCOD.VISITA.toString());

          OpVisita.descrizione = "Visita";

          let index = qdCFormModel.Trattamento.Testata.Operazioni.findIndex(o=>+ o.primaryKey.codice === enum_LAVCOD.VISITA);

          if(index === -1){
              qdCFormModel.Trattamento.Testata.Operazioni.push(OpVisita);
          }

          qdCFormModel.Trattamento.Testata.Codici_Attivita.forEach(c=>{
            if(!c.Operazione){
              c.Operazione = OpVisita
            }
          });

        }


        //Me lo faccio passare alla funzione il TipoOperazioneDB e non lo ottengo io dall'objParametriAgenda perché nel ribaltamento da brogliaccio -> ad agenda il
        //objParametriAgenda.TipoOperazioneDB è valorizzato come scriuttura ma il modello attività in realtà ce l'ho valorizzato
        if(TipoOperazioneDB === Enum_DBTypeOperation.Write) {

            qdCFormModel.Trattamento.Testata.Data = Default_Data;
            qdCFormModel.Trattamento.Testata.Origine = "";
            qdCFormModel.Trattamento.Testata.Centro_Aziendale= centro_aziendale;
            qdCFormModel.Trattamento.Testata.MultiCentro = false;
            qdCFormModel.Trattamento.Testata.Raccoglitore=0;
            qdCFormModel.Trattamento.Testata.Tipo=this.objParametriAgenda.TipoOperazioneAgenda;
            qdCFormModel.Trattamento.Testata.TipoRicetta=this.objParametriAgenda.TipoRicetta;
            qdCFormModel.Trattamento.Testata.Stato=this.objParametriAgenda.Stato as Stati;
            qdCFormModel.Trattamento.Testata.Latitude = 0;
            qdCFormModel.Trattamento.Testata.Longitude = 0;
            qdCFormModel.Trattamento.Testata.Specie={codice: -1, descrizione:this.translocoService.translate("NessunaSelezione")};
            qdCFormModel.Trattamento.Testata.InviaRicetta = false;
            qdCFormModel.Trattamento.Testata.Ricetta_Des = "";
            qdCFormModel.Trattamento.Testata.Ricetta_Des_Long = "";
            qdCFormModel.Trattamento.Testata.Ricetta_Numero = "";
            qdCFormModel.Trattamento.Testata.Ricetta_Note = "";
            qdCFormModel.Trattamento.Testata.Ricetta_Data_Da = null;
            qdCFormModel.Trattamento.Testata.Ricetta_Data_A = null;
            qdCFormModel.Trattamento.Testata.Attivita_Collegate = [];

            if(lavorazione && +lavorazione.primaryKey.codice > 0) {
                qdCFormModel.Trattamento.Testata = this.qdcservice.ImpostaValoriDefaultXOperazione(qdCFormModel.Trattamento.Testata);
            }
            qdCFormModel.Trattamento.Testata.Ora = Default_Data;
        } else {

            if(listaAttivita.length > 0) {

                //Per i dati in comune da assegnare al QdCFormModel prendo il primo elemento della listaAttivita

                let data_operazione:Date = cloneDeep(listaAttivita[0].inizio);

                qdCFormModel.Trattamento.Testata.Data = new Date(data_operazione.setHours(0,0,0,0));

                if(listaAttivita[0].origine && listaAttivita[0].origine !== ""){
                  qdCFormModel.Trattamento.Testata.Origine = listaAttivita[0].origine.toUpperCase();
                }else{
                  qdCFormModel.Trattamento.Testata.Origine = "";
                }


                qdCFormModel.Trattamento.Testata.Raccoglitore = listaAttivita[0].raccoglitore;
                qdCFormModel.Trattamento.Testata.Tipo = listaAttivita[0].tipo;
                qdCFormModel.Trattamento.Testata.TipoRicetta = listaAttivita[0].tipoRicetta;
                qdCFormModel.Trattamento.Testata.Stato = listaAttivita[0].stato;
                qdCFormModel.Trattamento.Testata.Latitude = listaAttivita[0].latitude;
                qdCFormModel.Trattamento.Testata.Longitude = listaAttivita[0].longitude;
                qdCFormModel.Trattamento.Testata.Centro_Aziendale = centro_aziendale;

                if(centro_aziendale.primaryKey.codice === this.qdcservice.Obj_TuttiICentriAziendali.primaryKey.codice) {
                    qdCFormModel.Trattamento.Testata.MultiCentro = true;
                } else {
                    qdCFormModel.Trattamento.Testata.MultiCentro = false;
                }

                let attivitaAltre_Operazioni: Attivita = listaAttivita.find(attivita => + attivita.job.primaryKey.codice === enum_LAVCOD.ALTRE_OPERAZIONI || + attivita.job.primaryKey.codice === enum_LAVCOD.VISITA);

                if(attivitaAltre_Operazioni) {
                    if(attivitaAltre_Operazioni.attivitaPersonalizzata){
                        qdCFormModel.Trattamento.Testata.Attivita_Personalizzata = this.qdcservice.setCodDescrDdlAttivitaPersonalizzata(attivitaAltre_Operazioni.attivitaPersonalizzata as DropdownListAttivitaPersonalizzata);
                    }

                    qdCFormModel.Trattamento.Testata.Descrizione_Altre_Lavorazioni = attivitaAltre_Operazioni.descrizione;
                }

                qdCFormModel.Trattamento.Testata.Disciplinare = this.setDisciplinareQdCForm(listaAttivita,qdCFormModel);

                if(listaAttivita[0].utilizzoTerreno) {
                    switch (listaAttivita[0].utilizzoTerreno.classType) {
                        case "Varieta":
                            const varieta = <Varieta>listaAttivita[0].utilizzoTerreno;
                            qdCFormModel.Trattamento.Testata.Specie={codice: varieta.specie.codice, descrizione:varieta.specie.descrizione};
                            break;

                        case "DestinazioneUso":
                            const destinazioneUso = <DestinazioneUso>listaAttivita[0].utilizzoTerreno;
                            qdCFormModel.Trattamento.Testata.Specie={codice: destinazioneUso.codice, descrizione:destinazioneUso.descrizione, classType:"DestinazioneUso"};
                            break;
                    }
                } else {
                    //Per le Operazioni di non utilizzo posso salvare senza selezionare la Specie quindi imposto sulla combo la voce NessunaSelezione
                    //Se non ho selezionato neanche il centro vengano create n agende in base a quanti centri aziendali vengono caricati nella combo
                    if(listaAttivita.filter(a=>this.qdcservice.Elenco_Operazioni_Non_Utilizzo.includes(+ a.job.primaryKey.codice)).length === listaAttivita.length){

                        qdCFormModel.Trattamento.Testata.Specie={codice: 0, descrizione: this.translocoService.translate("NessunaSelezione")};
                    }

                    //Per alcuni Rilievi posso salvare senza selezionare gli impianti quindi imposto sulla combo la voce NessunaSpecie
                    if(listaAttivita.filter(a=>this.qdcservice.RilievoSenzaImpianti(a)).length === listaAttivita.length){

                      qdCFormModel.Trattamento.Testata.Specie=this.qdcservice.Obj_NessunaSpecie;
                    }
                }

                qdCFormModel.Trattamento.Testata.InviaRicetta = listaAttivita[0].inviaRicetta;

                if(listaAttivita[0].testataRicetta) {
                    qdCFormModel.Trattamento.Testata.Ricetta_Des = listaAttivita[0].testataRicetta.Ricetta_Des
                    qdCFormModel.Trattamento.Testata.Ricetta_Des_Long = listaAttivita[0].testataRicetta.Ricetta_Des_Long
                    qdCFormModel.Trattamento.Testata.Ricetta_Numero = listaAttivita[0].testataRicetta.Ricetta_Numero;
                    qdCFormModel.Trattamento.Testata.Ricetta_Note = listaAttivita[0].testataRicetta.Note;
                    qdCFormModel.Trattamento.Testata.Ricetta_Data_Da = listaAttivita[0].testataRicetta.Data_Da;
                    qdCFormModel.Trattamento.Testata.Ricetta_Data_A = listaAttivita[0].testataRicetta.Data_A;
                } else {
                    qdCFormModel.Trattamento.Testata.Ricetta_Des = "";
                    qdCFormModel.Trattamento.Testata.Ricetta_Des_Long = "";
                    qdCFormModel.Trattamento.Testata.Ricetta_Numero = "";
                    qdCFormModel.Trattamento.Testata.Ricetta_Note = "";
                    qdCFormModel.Trattamento.Testata.Ricetta_Data_Da = null;
                    qdCFormModel.Trattamento.Testata.Ricetta_Data_A = null;
                }

                if(listaAttivita[0].attivitaCollegate){
                  qdCFormModel.Trattamento.Testata.Attivita_Collegate = listaAttivita[0].attivitaCollegate;
                }else{
                  qdCFormModel.Trattamento.Testata.Attivita_Collegate = [];
                }

                qdCFormModel.Trattamento.Testata.Ora = listaAttivita[0].oraInizio;

            }
        }
        //TODO da valutare come fare in modifica perché non abbiamo il campo nel modello
        let campo: Campo = new Campo({centroAziendalePK: qdCFormModel.Trattamento.Testata.Centro_Aziendale.primaryKey, codice: 0} as PKCampo);
        campo.descrizione = this.translocoService.translate("Tutti");

        qdCFormModel.Trattamento.Testata.Campo = this.qdcservice.setCodDescrDdlCampo(campo as DropdownListCampo);
    }

    private setDisciplinareQdCForm(listaAttivita: Attivita[],qdCFormModel: QdCFormModel): DropdownListDisciplinare {

        qdCFormModel.Trattamento.Testata.Disciplinare= null;

        let disciplinare:DropdownListDisciplinare = this.qdcservice.setCodDescrDdlDisciplinare(
            this.qdcservice.Obj_NessunDpiNessunaEtichetta as DropdownListDisciplinare
        );

        if(listaAttivita && listaAttivita.length > 0) {

            let listaOperazioni_con_disciplinari: Array<any> = [];

            listaAttivita.forEach(a=> {
                if(a.disciplinare && !listaOperazioni_con_disciplinari.map(o=>JSON.stringify(o.Disciplinare)).includes(JSON.stringify(a.disciplinare))){

                    let flag_direttiva_nitrati = false;

                    if(+ a.job.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI &&
                        a.disciplinare.regolamentoConcimazione.tipo === enum_PUARegolamenti_Tipo.PUA)
                        flag_direttiva_nitrati = true;


                    listaOperazioni_con_disciplinari.push({
                        Disciplinare: a.disciplinare,
                        Operazione: a.job,
                        Flag_Direttiva_Nitrati: flag_direttiva_nitrati
                    });
                }
            });

            if(listaOperazioni_con_disciplinari && listaOperazioni_con_disciplinari.length > 0){

                /* Nel momento in cui si deve battezzare il disciplinare (unico) fra tutte le operazioni,
                se le fertilizzazioni hanno 0 ma i trattamenti hanno -999, mettere -999 perché poi al
                salvataggio la cosa verrà gestita correttamente*/

                if(listaOperazioni_con_disciplinari.findIndex(o=> this.qdcservice.Elenco_Operazioni_Fertilizzanti.includes(+ (o.Operazione as Lavorazione).primaryKey.codice) && (o.Disciplinare as Disciplinare).codice === NessunDpi) > -1 &&
                    listaOperazioni_con_disciplinari.findIndex(o=> !this.qdcservice.Elenco_Operazioni_Fertilizzanti.includes(+ (o.Operazione as Lavorazione).primaryKey.codice) && (o.Disciplinare as Disciplinare).codice === NessunDpiNessunaEtichetta) > -1){

                    disciplinare=this.qdcservice.setCodDescrDdlDisciplinare(this.qdcservice.Obj_NessunDpiNessunaEtichetta as DropdownListDisciplinare);
                } else {

                    if(listaOperazioni_con_disciplinari.length === 1){

                        //Imposto il disciplinare di testata solamente se regolamentoConcimazione.tipo è diverso da 2 (direttiva nitrati)
                        //altrimenti imposto il disciplinare della distribuzione ammendanti

                        if ((listaOperazioni_con_disciplinari[0].Disciplinare as Disciplinare) &&
                            !listaOperazioni_con_disciplinari[0].Flag_Direttiva_Nitrati) {

                            disciplinare=this.qdcservice.setCodDescrDdlDisciplinare((listaOperazioni_con_disciplinari[0].Disciplinare as Disciplinare) as DropdownListDisciplinare);
                        }

                    } else {

                        if(listaOperazioni_con_disciplinari.length > 1) {

                            //L'unico caso in cui nella lista di attivita si possono avere disciplinari diversi è per il caso di Distribuzione ammendanti con
                            //direttiva nitrati (in questo caso si potrebbe avere un disciplinare dei trattamenti e uno diverso per la distribuzione ammendanti) per
                            //gli altri casi i disciplinari devono coincidere tra le attivita che gestiscono i disciplinari

                            let dpi_direttiva_nitrati = listaOperazioni_con_disciplinari.filter(o=> o.Flag_Direttiva_Nitrati);
                            let dpi_no_direttiva_nitrati = listaOperazioni_con_disciplinari.filter(o=>!o.Flag_Direttiva_Nitrati);

                            if(dpi_no_direttiva_nitrati.length === 1 && dpi_direttiva_nitrati.length === 1){
                                disciplinare=this.qdcservice.setCodDescrDdlDisciplinare((dpi_no_direttiva_nitrati[0].Disciplinare as Disciplinare) as DropdownListDisciplinare);
                            }

                        }

                    }

                }
            }else{
                //Per la nuova operazione CONFUSIONE_DISORIENTAMENTO_SESSUALE e per TRATTAMENTO_POST_RACCOLTA anche se non è visibile il disciplinare lo imposto
                //comunque a Solo Etichetta perchè lato server verrà gestito in questo modo
                if(qdCFormModel?.Trattamento?.Testata?.Operazioni?.length === 1 &&
                    qdCFormModel?.Trattamento?.Testata?.Operazioni?.findIndex(o=>this.qdcservice.Elenco_Operazioni_Con_Default_DPI.includes(+ o.primaryKey.codice)) > -1){

                    disciplinare = this.qdcservice.setCodDescrDdlDisciplinare(this.qdcservice.Obj_NessunDpi as DropdownListDisciplinare);
                }
            }
        }

        return disciplinare;

    }

    private setGridImpiantiSelezionatiModel(listaAttivita: Attivita[], qdCFormModel: QdCFormModel){

        let Sup_Trattata = 0;

        listaAttivita.forEach((attivita: Attivita)=>{
            if(attivita.centriDiCosto){
                attivita.centriDiCosto.forEach(esercizioCDC => {
                    if (esercizioCDC.classType === "EsercizioCDC") {
                        const esercizioCdC=<EsercizioCDC> esercizioCDC;
                        const PKesercizio = esercizioCdC.esercizio.codice;
                        const PKimpianto =<PKImpianto> esercizioCdC.esercizio.impiantoPK;
                        const PKappezzamento = <PKAppezzamento>PKimpianto.appezzamentoPK;
                        const PKcentroAziendale = <PKCentroAziendale>PKappezzamento.centroAziendalePK;

                        //Controllo che le chiavi siano valorizzate perchè mi potrebbero arrivare dei Rilievi che non sono
                        //stati registrati su degli impianti ma che valgono per tutta l'azienda
                        if(PKesercizio > 0 && PKimpianto.codice > 0 && PKappezzamento.codice > 0 && PKcentroAziendale.codice > 0 && PKcentroAziendale.partitaIva !== ""){

                            let IrrigazioneUtilizzata_Codice = "0";
                            let IrrigazioneUtilizzata_Descrizione = this.translocoService.translate("Nessuno");
                            let Consiglio_Descrizione = this.translocoService.translate("Nessuno");
                            let Consiglio_Codice = 0;
                            let DataConsiglio = null;
                            let DoseAcquaConsiglio = 0;
                            let UdmConsiglio: UnitaDiMisura = null;
                            let minDataTurno: Date = null;
                            let maxDataTurno: Date = null;
                            let UdmDose = "";
                            let DoseAcquaGiornaliera = 0;
                            let OreIrrigazione = 0;
                            let Portata = 0;
                            let Efficienza = 100;
                            let DataInizioIrrigazione = null;
                            let DataFineIrrigazione = null;
                            let FrequenzaIrrigazioneMedia = 0;
                            let tipoIrrigazione: TipoIrrigazione = null;
                            let macchinaIrrigazione: ParcoMacchine = null;

                            let updateIrrigationValues = false;
                            if(attivita.risorse){
                                const dettagliIrrigazione : DettaglioIrrigazione[] = (attivita.risorse.filter(risorsa => risorsa.classType === "DettaglioIrrigazione") as DettaglioIrrigazione[]);

                                if(dettagliIrrigazione){
                                    let index = dettagliIrrigazione.findIndex( r => r.esercizioCDC && r.esercizioCDC.esercizio.codice == PKesercizio &&
                                        r.esercizioCDC.esercizio.impiantoPK.codice == PKimpianto.codice &&
                                        r.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice == PKappezzamento.codice &&
                                        r.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice == PKcentroAziendale.codice &&
                                        r.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva == PKcentroAziendale.partitaIva);

                                    if(index > -1){

                                        updateIrrigationValues = true;
                                        let flagIsMacchina:boolean = false;

                                        if(dettagliIrrigazione[index].tipoIrrigazione && dettagliIrrigazione[index].tipoIrrigazione.codice >= 0){
                                            IrrigazioneUtilizzata_Codice = dettagliIrrigazione[index].tipoIrrigazione.codice.toString();
                                            tipoIrrigazione = dettagliIrrigazione[index].tipoIrrigazione;
                                        }

                                        if(dettagliIrrigazione[index].macchina){
                                            flagIsMacchina = true;
                                            macchinaIrrigazione = dettagliIrrigazione[index].macchina;
                                            IrrigazioneUtilizzata_Descrizione = this.qdcservice.getIrrigazioneUtilizzata_Descrizione(flagIsMacchina,dettagliIrrigazione[index].macchina.descrizione);
                                            Efficienza = dettagliIrrigazione[index].macchina.efficienza;
                                            Portata = dettagliIrrigazione[index].macchina.portata;
                                            IrrigazioneUtilizzata_Codice += "_" + dettagliIrrigazione[index].macchina.codice;
                                        } else{
                                            IrrigazioneUtilizzata_Descrizione = this.qdcservice.getIrrigazioneUtilizzata_Descrizione(flagIsMacchina,dettagliIrrigazione[index].tipoIrrigazione.descrizione);
                                            Efficienza = 100;
                                            IrrigazioneUtilizzata_Codice += "_0";
                                        }

                                        if (dettagliIrrigazione[index].consiglioIrrigazione){
                                            if(dettagliIrrigazione[index].consiglioIrrigazione.codice > 0){
                                                Consiglio_Codice = dettagliIrrigazione[index].consiglioIrrigazione.codice;
                                            }

                                            if(dettagliIrrigazione[index].consiglioIrrigazione.descrizione !== ""){
                                                Consiglio_Descrizione = dettagliIrrigazione[index].consiglioIrrigazione.descrizione;
                                            }

                                            DataConsiglio = dettagliIrrigazione[index].consiglioIrrigazione.dataConsiglio;
                                            DoseAcquaConsiglio = dettagliIrrigazione[index].consiglioIrrigazione.qtaAcqua;
                                            UdmConsiglio = dettagliIrrigazione[index].consiglioIrrigazione.unitaDiMisura;
                                            minDataTurno = dettagliIrrigazione[index].consiglioIrrigazione.minDataTurno;
                                            maxDataTurno = dettagliIrrigazione[index].consiglioIrrigazione.maxDataTurno;
                                        }

                                        UdmDose = dettagliIrrigazione[index].unitaDiMisura.simbolo;
                                        DoseAcquaGiornaliera = dettagliIrrigazione[index].QtaRilevata;
                                        OreIrrigazione = dettagliIrrigazione[index].Ore;
                                        Portata = dettagliIrrigazione[index].Portata;
                                        Efficienza = dettagliIrrigazione[index].Efficienza;

                                        DataInizioIrrigazione = dettagliIrrigazione[index].DataInizio;
                                        DataFineIrrigazione = dettagliIrrigazione[index].DataFine;
                                        FrequenzaIrrigazioneMedia = dettagliIrrigazione[index].Frequenza;
                                    }
                                }
                            }

                            let impiantoIndex = qdCFormModel.Trattamento.Impianti.ImpiantiSelezionati.findIndex(i=>i.PIVA === PKcentroAziendale.partitaIva &&
                                i.SA_COD === PKcentroAziendale.codice &&
                                i.APPEZZA === PKappezzamento.codice &&
                                i.ID_REG === PKimpianto.codice &&
                                i.Progetto_Cod === PKesercizio)

                            let impiantoIsPresent = impiantoIndex !== -1;
                            if (impiantoIsPresent){
                                if (updateIrrigationValues){
                                    let impianto = qdCFormModel.Trattamento.Impianti.ImpiantiSelezionati[impiantoIndex];
                                    impianto.IrrigazioneUtilizzata_Codice = IrrigazioneUtilizzata_Codice;
                                    impianto.IrrigazioneUtilizzata_Descrizione = IrrigazioneUtilizzata_Descrizione;
                                    impianto.Consiglio_Descrizione = Consiglio_Descrizione;
                                    impianto.Consiglio_Codice = Consiglio_Codice;
                                    impianto.DataConsiglio = DataConsiglio;
                                    impianto.DoseAcquaConsiglio = DoseAcquaConsiglio;
                                    impianto.UdmConsiglio = UdmConsiglio;
                                    impianto.minDataTurno = minDataTurno;
                                    impianto.maxDataTurno = maxDataTurno;
                                    impianto.UdmDose = UdmDose;
                                    impianto.DoseAcquaGiornaliera = DoseAcquaGiornaliera;
                                    impianto.OreIrrigazione = OreIrrigazione;
                                    impianto.Portata = Portata;
                                    impianto.Efficienza = Efficienza;
                                    impianto.DataInizioIrrigazione = DataInizioIrrigazione;
                                    impianto.DataFineIrrigazione = DataFineIrrigazione;
                                    impianto.FrequenzaIrrigazioneMedia = FrequenzaIrrigazioneMedia;
                                    impianto.tipoIrrigazione = tipoIrrigazione;
                                    impianto.macchinaIrrigazione = macchinaIrrigazione;
                                }

                            } else { //Inserisco l'impianto solo se non è già presente

                                Sup_Trattata += esercizioCdC.superficieTrattata;

                                const ImpiantiSelezionati = new GridImpiantoSelezionatoModel(PKcentroAziendale.partitaIva,
                                    PKcentroAziendale.codice,
                                    PKappezzamento.codice,
                                    PKimpianto.codice,
                                    PKesercizio,
                                    0,"",0,"",0,esercizioCdC.superficieTrattata,
                                    esercizioCdC.superficieRiduzioneBufferZone,
                                    esercizioCdC.percentualeRiduzioneDeriva,0,"",[],
                                    AGRODATAINIZIO,AGRODATAFINE,"",null,0,
                                    AGRODATAINIZIO, AGRODATAINIZIO,"",null,0,0,0,0,0,0,"","",
                                    IrrigazioneUtilizzata_Codice,
                                    IrrigazioneUtilizzata_Descrizione,
                                    Consiglio_Descrizione,
                                    Consiglio_Codice,
                                    DataConsiglio,
                                    DoseAcquaConsiglio,
                                    UdmConsiglio,
                                    minDataTurno,
                                    maxDataTurno,
                                    UdmDose,
                                    DoseAcquaGiornaliera,
                                    OreIrrigazione,
                                    Portata,
                                    Efficienza,
                                    DataInizioIrrigazione,
                                    DataFineIrrigazione,
                                    FrequenzaIrrigazioneMedia,
                                    tipoIrrigazione,
                                    macchinaIrrigazione);

                                qdCFormModel.Trattamento.Impianti.ImpiantiSelezionati.push(ImpiantiSelezionati);
                            }
                        }


                    } else if (esercizioCDC.classType === "ProdottoDaTrattareCDC") {
                        const prodottoCdC = esercizioCDC as ProdottoDaTrattareCDC;

                        const pkProdotto = prodottoCdC.giacenzaMagazzino.Prodotto.codice;
                        const lotto = prodottoCdC.giacenzaMagazzino.Lotto;
                        const progettoCod = prodottoCdC.giacenzaMagazzino.codice_progetto;

                        const fabbricatoCod = prodottoCdC.giacenzaMagazzino.Magazzino.primaryKey.codice;
                        const saCod = prodottoCdC.giacenzaMagazzino.Magazzino.primaryKey.centroAziendalePK.codice;
                        const partitaIva = prodottoCdC.giacenzaMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva;

                        //Inserisco l'impianto solo se non è già presente
                        const el = qdCFormModel.Trattamento.ProdottiDaTrattare.ProdottiDaTrattareSelezionati.find(x =>
                            x.giacenzaMagazzino.Prodotto.codice === pkProdotto &&
                            x.giacenzaMagazzino.Lotto === lotto &&
                            x.giacenzaMagazzino.codice_progetto === progettoCod &&
                            x.giacenzaMagazzino.Magazzino.primaryKey.codice === fabbricatoCod &&
                            x.giacenzaMagazzino.Magazzino.primaryKey.centroAziendalePK.codice === saCod &&
                            x.giacenzaMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva === partitaIva)
                        if (el == null){
                            qdCFormModel.Trattamento.ProdottiDaTrattare.ProdottiDaTrattareSelezionati.push(prodottoCdC);
                        }
                    }
                });

            }
        });


        //Aggiorno il model delle Superficie Trattata quella selezionata la ricavo
        //dopo la lettura degli impianti
        if(Sup_Trattata > 0){
            qdCFormModel.Trattamento.Superfici = new Superfici();
            qdCFormModel.Trattamento.Superfici.Sup_Trattata = this.funzionicomuniservice.roundNumber(Sup_Trattata,this.qdcservice.get4DecimalNumericSettings().decimals);
            qdCFormModel.Trattamento.Superfici.Sup_Selezionata = this.funzionicomuniservice.roundNumber(Sup_Trattata,this.qdcservice.get4DecimalNumericSettings().decimals);
        }

    }

    private setGridNoteModel(noteIntervento: Array<NoteIntervento>, qdCFormModel:QdCFormModel){
        noteIntervento.forEach((notaIntervento: NoteIntervento) => {
            let gruppoCod=0;
            if (notaIntervento.noteInterventoGruppi) {
                gruppoCod=notaIntervento.noteInterventoGruppi.codice;
            }

            //Inserisco la nota solo se non è già presente
            if(qdCFormModel.Trattamento.Note.findIndex(i=>i.id === notaIntervento.codice &&
                i.NotaGruppo_Cod === gruppoCod)===-1){
                const nota=new NotaInterventoDdlItem(notaIntervento.codice, notaIntervento.descrizione, gruppoCod);

                qdCFormModel.Trattamento.Note.push(nota);
            }

        });
    }

    private setGridMacchineModel(risorsaMacchina: RisorsaMacchina,lavorazione: Lavorazione, qdCFormModel: QdCFormModel){

        const Macchina = risorsaMacchina.macchina;

        if(Macchina){

            if(!qdCFormModel.Trattamento.CostiAccessori)
                qdCFormModel.Trattamento.CostiAccessori = new CostiAccessori();

            const Macchina_Risorsa_Cod = -1 + "*" + MACCHINE + "*" + 0 + "*"+ Macchina.codice +"*" +0;

            //Inserisco la macchina solo se non è già presente
            if(qdCFormModel.Trattamento.CostiAccessori.Macchine.findIndex(m=>m.Risorsa_Cod === Macchina_Risorsa_Cod) === -1){

                let Macchina_Risorsa_Des = "";

                let Class_Desc = "";

                if(!Macchina.dettaglio_2 || !Macchina.dettaglio_2.descrizione || Macchina.dettaglio_2.descrizione === ""){
                    if(!Macchina.dettaglio_1 || !Macchina.dettaglio_1.descrizione || Macchina.dettaglio_1.descrizione === ""){

                        if(Macchina.tipo && Macchina.tipo.descrizione && Macchina.tipo.descrizione !== "")
                            Class_Desc = Macchina.tipo.descrizione;
                    }else{
                        Class_Desc = Macchina.dettaglio_1.descrizione;
                    }
                }else{
                    Class_Desc = Macchina.dettaglio_2.descrizione;
                }

                Macchina_Risorsa_Des +=  Class_Desc;

                if(Macchina.marca &&
                    Macchina.marca.descrizione &&
                    Macchina.marca.descrizione !== "")
                    Macchina_Risorsa_Des += " - " + Macchina.marca.descrizione;

                if(Macchina.modello &&
                    Macchina.modello !== "")
                    Macchina_Risorsa_Des += " - " + Macchina.modello;

                if(Macchina.descrizione &&
                    Macchina.descrizione !== "")
                    Macchina_Risorsa_Des += " - " + Macchina.descrizione;

                if(Macchina.scadenza_Taratura &&
                    Macchina.scadenza_Taratura.getTime() !== AGRODATAFINE.getTime())
                    Macchina_Risorsa_Des += " " + this.translocoService.translate("DataScadenzaTaraturaAbbr",{Data: this.datePipe.transform(Macchina.scadenza_Taratura,'dd/MM/yyyy',undefined,this.locale_id)});


                const gridMacchinaModel = new GridMacchinaModel(Macchina_Risorsa_Cod,
                    Macchina_Risorsa_Des,
                    risorsaMacchina.macchina.partitaIva,
                    risorsaMacchina.macchina.codice,
                    risorsaMacchina.macchina.taratura_Ugello,
                    risorsaMacchina.macchina.modello,
                    risorsaMacchina.macchina.marca,
                    risorsaMacchina.macchina.scadenza_Taratura,
                    risorsaMacchina.macchina.descrizione,
                    risorsaMacchina.macchina.tipo,
                    risorsaMacchina.macchina.dettaglio_1,
                    risorsaMacchina.macchina.dettaglio_2,
                    risorsaMacchina.macchina.validita);

                qdCFormModel.Trattamento.CostiAccessori.Macchine.push(gridMacchinaModel);

            }

        }

    }

    private setCausaliModel(risorsaCausale: RisorsaCausale, qdCFormModel: QdCFormModel) {
        const causale = new Causale(risorsaCausale.id, risorsaCausale.causale);

        qdCFormModel.Trattamento.Causali.push(causale);
    }

    // @description:
    //Prendo la prima attivita e utilizzo i suo dati dell'acqua,
    // per il caso di multicentro prima ottengo i dati delle attivita che hanno
    //lo stesso lav_cod della prima operazione e poi se la Dose Acqua è totale sommo tutti i quantitavi totali di quel lav_cod
    //e poi calcolo l' Acqua Ha
    private setAcquaModel(listaAttivita: Attivita[],qdCFormModel: QdCFormModel){

        let Dose_Acqua:number = 0;
        let Acqua_Ha:number = 0;
        let Acqua_Tot:number = 0;
        let flag_risorsa_acqua:boolean = false;

        if(!qdCFormModel.Trattamento.Acqua)
            qdCFormModel.Trattamento.Acqua = new Acqua();

        if(qdCFormModel.Trattamento.Testata.Operazioni && qdCFormModel.Trattamento.Testata.Operazioni.length > 0){

            let attivita_con_stesso_lav_cod: Attivita[] = [];

            for(let o of qdCFormModel.Trattamento.Testata.Operazioni){
                if(this.qdcservice.Elenco_Operazioni_con_Acqua.includes(+ o.primaryKey.codice)){
                    attivita_con_stesso_lav_cod = listaAttivita.filter(a=>a.job.primaryKey.codice === o.primaryKey.codice);
                    break;
                }
            }

            if(attivita_con_stesso_lav_cod && attivita_con_stesso_lav_cod.length > 0){
                attivita_con_stesso_lav_cod.forEach((attivita: Attivita)=>{
                    let risorsaAcqua = attivita.risorse.find((risorsa: Risorsa)=>{ return risorsa.classType === "RisorsaAcqua" }) as RisorsaAcqua;

                    if(risorsaAcqua){

                        flag_risorsa_acqua = true;

                        Dose_Acqua = risorsaAcqua.doseAcqua;

                        if(risorsaAcqua.doseAcqua === DoseAcqua.HA){
                            Acqua_Ha =  risorsaAcqua.acqua;
                            Acqua_Tot = 0;
                        }else if(risorsaAcqua.doseAcqua === DoseAcqua.TOTALE){
                            Acqua_Tot += risorsaAcqua.acqua;
                            Acqua_Ha = 0;
                        }
                    }
                });
            }
        }

        if(flag_risorsa_acqua){

            qdCFormModel.Trattamento.Acqua.Dose_Acqua = Dose_Acqua;

            if(Dose_Acqua === DoseAcqua.HA){
                qdCFormModel.Trattamento.Acqua.Acqua_Ha = this.funzionicomuniservice.roundNumber(Acqua_Ha,this.qdcservice.get4DecimalNumericSettings().decimals);
                qdCFormModel.Trattamento.Acqua.Acqua_Tot = this.funzionicomuniservice.roundNumber((qdCFormModel.Trattamento.Superfici.Sup_Trattata * Acqua_Ha),this.qdcservice.get4DecimalNumericSettings().decimals);
            }else if(Dose_Acqua === DoseAcqua.TOTALE){
                qdCFormModel.Trattamento.Acqua.Acqua_Tot = this.funzionicomuniservice.roundNumber(Acqua_Tot,this.qdcservice.get4DecimalNumericSettings().decimals);
                qdCFormModel.Trattamento.Acqua.Acqua_Ha = this.funzionicomuniservice.roundNumber((Acqua_Tot / qdCFormModel.Trattamento.Superfici.Sup_Trattata),this.qdcservice.get4DecimalNumericSettings().decimals);
            }


        }



    }

    private setGridOperatoriModel(risorsaPersona:RisorsaPersona, qdCFormModel: QdCFormModel){

        const RisorsaUmana = risorsaPersona.risorsaUmana;

        if(RisorsaUmana){

            if(!qdCFormModel.Trattamento.CostiAccessori)
                qdCFormModel.Trattamento.CostiAccessori = new CostiAccessori();

            let Cod_RisUm = RisorsaUmana.codice;

            let Cod_Contatto = "";

            let Piva_Contatto = "";

            let Ragione_Sociale_Contatto = "";

            let Documenti_Contatto = [];

            let RapportoContabile_Descrizione = "";

            let RapportoContabile_Codice = 0;

            let Nome = "";

            let Cognome = "";

            let Data_Scadenza_Patentino = AGRODATAFINE;

            if(RisorsaUmana.contatto){

                Cod_Contatto = RisorsaUmana.contatto.primaryKey.codice;

                if(RisorsaUmana.contatto.primaryKey.partitaIva &&
                    RisorsaUmana.contatto.primaryKey.partitaIva !== "")
                    Piva_Contatto = RisorsaUmana.contatto.primaryKey.partitaIva;

                if(RisorsaUmana.contatto.ragione_Sociale &&
                    RisorsaUmana.contatto.ragione_Sociale !== "")
                    Ragione_Sociale_Contatto = RisorsaUmana.contatto.ragione_Sociale;

                if(RisorsaUmana.contatto.documenti)
                    Documenti_Contatto = risorsaPersona.risorsaUmana.contatto.documenti;

                Nome = RisorsaUmana.contatto.nome;

                Cognome = RisorsaUmana.contatto.cognome;
            }

            if(RisorsaUmana.rapportoContabile){

                RapportoContabile_Codice = RisorsaUmana.rapportoContabile.codice;

                if(RisorsaUmana.rapportoContabile.descrizione &&
                    RisorsaUmana.rapportoContabile.descrizione !== ""){
                    RapportoContabile_Descrizione = RisorsaUmana.rapportoContabile.descrizione;
                }

            }


            const Persona_Risorsa_Cod = -2 + "*" + ELEMCOD_MANODOPERA + "*" + 0 + "*"+ Cod_RisUm +"*" +Cod_Contatto+"*" +Piva_Contatto;

            //Inserisco l'operatore solo se non è già presente
            if(qdCFormModel.Trattamento.CostiAccessori.Operatori.findIndex(o=>o.Risorsa_Cod === Persona_Risorsa_Cod) === -1){

                let Persona_Risorsa_Des = "";

                if(RapportoContabile_Descrizione !== "")
                    Persona_Risorsa_Des = RapportoContabile_Descrizione;

                if(Ragione_Sociale_Contatto !== "")
                    Persona_Risorsa_Des += " - " + Ragione_Sociale_Contatto;

                if(Documenti_Contatto.length > 0){
                    let d = Math.max.apply(null, Documenti_Contatto.map(function(d) { return d.Data_Scadenza; }));

                    if(d)
                        Data_Scadenza_Patentino = new Date(d);
                }

                if(Data_Scadenza_Patentino.getTime() !== AGRODATAFINE.getTime())
                    Persona_Risorsa_Des += this.translocoService.translate("DataScadenzaPatentinoAbbr",{Data: this.datePipe.transform(Data_Scadenza_Patentino,'dd/MM/yyyy',undefined,this.locale_id)});


                const gridOperatoreModel = new GridOperatoreModel(Piva_Contatto,
                    Cod_Contatto,
                    Cod_RisUm,
                    Persona_Risorsa_Cod,
                    Persona_Risorsa_Des,
                    RapportoContabile_Codice,
                    RapportoContabile_Descrizione,
                    Nome,
                    Cognome,
                    Data_Scadenza_Patentino);


                qdCFormModel.Trattamento.CostiAccessori.Operatori.push(gridOperatoreModel);
            }


        }


    }

    //N.B: In caso si Semina senza prodotto la variabile Prodotto è null
    private setSezioneProdottoModel(
        Prodotto: DettaglioTrattamento | DettaglioFertilizzazione | DettaglioSemina | DettaglioRaccolta,
        qdCFormModel: QdCFormModel,
        lavorazione: Lavorazione,
        Centro_Aziendale: CentroAziendale,
        elem_cod: number,
        epoca: Epoca,
        disciplinare: Disciplinare,
        modalita_applicazione: BaseCodeDescr,
        attivita_con_parametri_aggiuntivi: Attivita_Con_Parametri_Aggiuntivi
    ): void {

        let lav_cod: number = +lavorazione.primaryKey.codice;
        let flagTipoDose: number = 0;
        let flagDoseQuantitaTotale: number = 0;
        let index: number = -1;
        let EpocaDPI: Epoca = null;
        let EpocaFertilizzazione: Epoca = null;
        let Utilizza_Direttiva_Nitrati: boolean = false;
        let Direttiva_Nitrati: Disciplinare = null;
        let Opzione_Semina = null;
        let Data: Date = qdCFormModel.Trattamento.Testata.Data;
        let Ripartizione_Trappole: BaseCodeDescr = null;

        //Controllo se è già presente una sezione prodotti di quel lav_cod
        //se c'è giàutilizzo quella sezione
        if(
            qdCFormModel?.Trattamento?.Sezioni_Prodotto &&
            qdCFormModel?.Trattamento?.Sezioni_Prodotto.length > 0
        ) {

            index = qdCFormModel.Trattamento.Sezioni_Prodotto.findIndex(
                s => s.Operazione.primaryKey.codice === lavorazione.primaryKey.codice
            );
        }

        let sezione_prodotto: Sezione_Prodotto_Formulati | Sezione_Prodotto_Fertilizzanti
            | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta;

        switch(elem_cod) {
            case FORMULATI:
                if(epoca) {
                    EpocaDPI = epoca;
                }

                if((Prodotto as DettaglioTrattamento).ripartizioneTrappole){
                  Ripartizione_Trappole = this.qdcservice.Opzioni_Ripartizione_Trappole.find(x=>x.codice === (Prodotto as DettaglioTrattamento).ripartizioneTrappole);
                }

              break;
            case FERTILIZZANTI:
                if(epoca){
                    EpocaFertilizzazione = epoca;
                }

                //Se disciplinare.regolamentoConcimazione.tipo = 2 allora siamo nel caso della Direttiva Nitrati
                if(disciplinare) {
                    if(
                        + lavorazione.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI &&
                        disciplinare.regolamentoConcimazione.tipo === enum_PUARegolamenti_Tipo.PUA
                    ) {
                        Utilizza_Direttiva_Nitrati = true;
                        Direttiva_Nitrati = disciplinare;
                    }
                }

                break;
            case SEMENTI:
                Opzione_Semina = Elenco_Opzioni_Semina.find(o => o.codice === enum_SEMINA_TIPO.Solo_Semina_Default);
                break;
            case TRASFORMATI_VEGETALI:
                // TODO M: prodotto da trattare
                break;
        }

        if(index === -1) {

            switch(elem_cod) {
                case FORMULATI:
                    sezione_prodotto = new Sezione_Prodotto_Formulati();

                    if(epoca) {
                        sezione_prodotto.EpocaDPI = EpocaDPI;
                    }

                    if(modalita_applicazione){
                        sezione_prodotto.Modalita_Applicazione = modalita_applicazione;
                    }

                    if(Ripartizione_Trappole){
                      sezione_prodotto.Ripartizione_Trappole = Ripartizione_Trappole;
                    }

                    break;
                case FERTILIZZANTI:
                    sezione_prodotto = new Sezione_Prodotto_Fertilizzanti();

                    //L'epoca non rientra nel butta giù della griglia dei dosaggi dei prodotti
                    if(epoca) {
                        sezione_prodotto.EpocaFertilizzazione = EpocaFertilizzazione;
                    }

                    //Se disciplinare.regolamentoConcimazione.tipo = 2 allora siamo nel caso della Direttiva Nitrati
                    if(disciplinare){
                        if(+ lavorazione.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI &&
                            disciplinare.regolamentoConcimazione.tipo === enum_PUARegolamenti_Tipo.PUA){

                            sezione_prodotto.Utilizza_Direttiva_Nitrati = Utilizza_Direttiva_Nitrati;

                            sezione_prodotto.Direttiva_Nitrati = Direttiva_Nitrati;

                        }
                    }
                    if(modalita_applicazione){
                        sezione_prodotto.Modalita_Applicazione = modalita_applicazione;
                    }
                    break;
                case SEMENTI:
                    sezione_prodotto = new Sezione_Prodotto_Sementi();
                    Opzione_Semina = Elenco_Opzioni_Semina.find(o => o.codice === enum_SEMINA_TIPO.Solo_Semina_Default);
                    sezione_prodotto.Opzioni_Semina = Opzione_Semina;
                    break;
                case TRASFORMATI_VEGETALI:
                    if (lav_cod === enum_LAVCOD.RACCOLTA || lav_cod === enum_LAVCOD.TRATTAMENTO_POST_RACCOLTA)
                        sezione_prodotto = new Sezione_Prodotto_Raccolta();

                    break;

                case INSETTI:
                    sezione_prodotto = new Sezione_Prodotto_Formulati();
                    break;
            }

            sezione_prodotto.Operazione = lavorazione;
            sezione_prodotto.Categoria_Magazzino = elem_cod;
            qdCFormModel.Trattamento.Sezioni_Prodotto.push(sezione_prodotto);

        } else {
            sezione_prodotto = qdCFormModel.Trattamento.Sezioni_Prodotto[index];
        }

        if(lav_cod !== enum_LAVCOD.RACCOLTA && elem_cod !== TRASFORMATI_VEGETALI) {
            flagTipoDose = ! Prodotto?.flagTipoDose ? enum_TipoMezzo.Ettolitro : Prodotto?.flagTipoDose;
            flagDoseQuantitaTotale = ! Prodotto?.flagDoseQuantitaTotale? 0 : Prodotto?.flagDoseQuantitaTotale;
        }

        if (
            elem_cod === TRASFORMATI_VEGETALI &&
            lav_cod === enum_LAVCOD.RACCOLTA
        ) { // Valorizzo raccolta

            let risorsa: DettaglioRaccolta = Prodotto as DettaglioRaccolta;
            if (risorsa.dataIngresso <= AGRODATAINIZIO) {
                risorsa.dataIngresso = this.QdCForm.value.Trattamento?.Testata?.Data;
            }

            sezione_prodotto = sezione_prodotto as Sezione_Prodotto_Raccolta;

            sezione_prodotto.Opzioni_Raccolta = new OpzioniRaccolta();
            sezione_prodotto.Opzioni_Raccolta.Ripartizione = risorsa?.Opzioni_Raccolta?.Ripartizione;
            sezione_prodotto.Opzioni_Raccolta.GenerazioneLotto = risorsa?.Opzioni_Raccolta?.GenerazioneLotto;
            sezione_prodotto.Opzioni_Raccolta.Modalita = risorsa?.Opzioni_Raccolta?.Modalita;
            normalizeOpzioniRaccolta(sezione_prodotto.Opzioni_Raccolta);
            sezione_prodotto.Data_Raccolta = this.QdCForm.value.Trattamento.Testata.Data;

            let prodotti = sezione_prodotto.ProdottiRaccolti;

            if (!prodotti) {
                prodotti = new Array<DettaglioRaccolta>();
                sezione_prodotto.ProdottiRaccolti = prodotti;
            }

            let found = prodotti.find(p => this.Controlla_Se_Stessa_Raccolta(p, risorsa));
            if (found) {
                found.QuantitaSuImpianti = found.QuantitaSuImpianti || [];
                risorsa.QuantitaSuImpianti = risorsa.QuantitaSuImpianti || [];
                for (let cdc of risorsa.QuantitaSuImpianti) {
                    risorsa.Opzioni_Raccolta = risorsa.Opzioni_Raccolta || new OpzioniRaccolta();
                    let impianto = found.QuantitaSuImpianti.find(q => this.ImpiantiStessoCentro(q.esercizioCDC, cdc.esercizioCDC) && this.Controlla_Stesso_LottoMagazzino(q, cdc))
                    if (!impianto || risorsa.Opzioni_Raccolta.Ripartizione === enum_Ripartizione_Raccolta.MANUALE) {
                        found.QuantitaSuImpianti.push(cdc);
                        continue;
                    }
                    impianto.Qta += cdc.Qta;
                }
                found.quantitaTotaleReale += risorsa.quantitaTotaleReale;
                return;
            }

            if (risorsa.MagazziniMovimentazioni && risorsa.MagazziniMovimentazioni[0] && risorsa.MagazziniMovimentazioni[0].Cod_Progetto) {
                sezione_prodotto.Opzioni_Raccolta.GenerazioneLotto = enum_Generazione_Lotto_Raccolta.DA_PROGETTO_COD;
            }

            prodotti.push(risorsa);

            return;
        } else {

            //Faccio il cast a MultiColumnComboboxTrattamento,MultiColumnComboboxFertilizzazione e MultiColumnComboboxSemina per avere le descrizioni concatenate nella grid dosi prodotti

            if(Prodotto){


                let ProdottoDaInserire = null;


                switch(elem_cod) {
                    case INSETTI:
                    case FORMULATI:
                        ProdottoDaInserire = (Prodotto as DettaglioTrattamento) as MultiColumnComboboxTrattamento;
                        break;
                    case FERTILIZZANTI:
                        ProdottoDaInserire = (Prodotto as DettaglioFertilizzazione) as MultiColumnComboboxFertilizzazione;
                        break;
                    case SEMENTI:
                        ProdottoDaInserire = (Prodotto as DettaglioSemina) as MultiColumnComboboxSemina;
                        break;
                }

                ProdottoDaInserire = this.qdcservice.setCodDescrMultiColumnComboboxProdotto(elem_cod,ProdottoDaInserire,Data);

                //Un solo prodotto uguale per ogni sezione prodotto
                let index_prodotto_gia_presente: number = this.Controlla_Se_Inserire_Prodotto(sezione_prodotto,ProdottoDaInserire,lavorazione);

                if(index_prodotto_gia_presente === -1) {

                    let quantitaSuImpianti: QuantitaSuImpianto[] = [];

                    if(this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(lav_cod)){
                      let dettagliTrattamento: DettaglioTrattamento[] = (attivita_con_parametri_aggiuntivi.risorse.filter(r=>r.classType === "DettaglioTrattamento") as DettaglioTrattamento[]);

                      dettagliTrattamento.forEach(dettaglioTrattamento=>{
                        if(this.griddosiprodottiservice.Controlla_Se_Stesso_Formulato(dettaglioTrattamento,ProdottoDaInserire)){
                          for(let x = 0; x < dettaglioTrattamento.quantitaSuImpianti.length; x++){

                            if(this.qdcservice.QuantitaSuImpianto_Presente(quantitaSuImpianti,dettaglioTrattamento.quantitaSuImpianti[x]) === -1){
                              quantitaSuImpianti.push(dettaglioTrattamento.quantitaSuImpianti[x]);
                            }
                          }
                        }
                      });
                    }

                    //No MultiCentro

                    //Se nel prodotto ho più righe di MagazziniMovimentazioni allora faccio una riga della griglia per ogni magazzino del prodotto

                    let acqua: Acqua = qdCFormModel.Trattamento.Acqua;
                    let MagazziniMovimentazioni: Array<RilevamentoDiMagazzino> = ProdottoDaInserire.MagazziniMovimentazioni;

                    //Se c'è una Ricetta/Brogliaccio trattamento creata da APP (oppure anche una Ricetta/Brogliaccio con dei problemi rilevati) che si sta cercando di ribaltare in agenda mostro in edit tutti
                    //i prodotti per fare riscattare tutte le logiche del butta giù
                    let Salva_Prodotto: boolean = true;

                    if((this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write &&
                        (this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_ad_Agenda ||
                        this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda) &&
                        (qdCFormModel.Trattamento.Testata.Origine === enum_OrigineApp.GiasApp ||
                        qdCFormModel.Trattamento.Testata.Origine === enum_OrigineApp.Demetra) &&
                        qdCFormModel.Trattamento.Testata.Codici_Attivita.findIndex(c=>c.APP_RicettaOperazione_ID && c.APP_RicettaOperazione_ID !== "") > -1) ||
                       (this.objParametriAgenda.TipoOperazioneDB !== Enum_DBTypeOperation.Read && this.CheckProblemi_Prodotto(ProdottoDaInserire,attivita_con_parametri_aggiuntivi) !== enum_Problema_DettaglioProdotto.Nessuno)){

                        Salva_Prodotto = false;

                    }

                    if(MagazziniMovimentazioni && MagazziniMovimentazioni.length > 0) {

                        MagazziniMovimentazioni.forEach((m: RilevamentoDiMagazzino) => {

                            let new_ProdottoDaInserire = cloneDeep(ProdottoDaInserire);
                            new_ProdottoDaInserire.MagazziniMovimentazioni = [];
                            new_ProdottoDaInserire.MagazziniMovimentazioni.push(m);
                            new_ProdottoDaInserire = this.qdcservice.setCodDescrMultiColumnComboboxProdotto(elem_cod,new_ProdottoDaInserire,Data);

                            if(!Salva_Prodotto)
                                this.set_ProblemiProdotto(new_ProdottoDaInserire,attivita_con_parametri_aggiuntivi);

                            const newRow = this.griddosiprodottiservice.getRigaGridDosiProdotti(
                                sezione_prodotto.DosiProdotti,
                                -1,
                                null,
                                new_ProdottoDaInserire,
                                Centro_Aziendale,
                                lavorazione,
                                Data,
                                flagTipoDose,
                                flagDoseQuantitaTotale,
                                !acqua ? 0 : qdCFormModel.Trattamento.Acqua.Dose_Acqua,
                                null,
                                EpocaDPI,
                                EpocaFertilizzazione,
                                Utilizza_Direttiva_Nitrati,
                                Direttiva_Nitrati,
                                Opzione_Semina,
                                modalita_applicazione,
                                quantitaSuImpianti,
                                Ripartizione_Trappole,
                                false,
                                Salva_Prodotto,
                                false
                            );

                            //Lasciare questa patchValue all'Original_Row_Value come ultima istruzione
                           //prima del push nell'array delle DosiProdotti
                            newRow.get("Original_Row_Value").patchValue(cloneDeep(newRow.value));

                            sezione_prodotto.DosiProdotti.push(newRow.value);

                        });

                    } else {

                        if(!Salva_Prodotto)
                            this.set_ProblemiProdotto(ProdottoDaInserire as MultiColumnComboboxTrattamento,attivita_con_parametri_aggiuntivi);


                        const newRow = this.griddosiprodottiservice.getRigaGridDosiProdotti(sezione_prodotto.DosiProdotti,
                            -1,
                            null,
                            ProdottoDaInserire,
                            Centro_Aziendale,
                            lavorazione,
                            Data,
                            flagTipoDose,
                            flagDoseQuantitaTotale,
                            !acqua ? 0 : qdCFormModel.Trattamento.Acqua.Dose_Acqua,
                            null,
                            EpocaDPI,
                            EpocaFertilizzazione,
                            Utilizza_Direttiva_Nitrati,
                            Direttiva_Nitrati,
                            Opzione_Semina,
                            modalita_applicazione,
                            quantitaSuImpianti,
                            Ripartizione_Trappole,
                            false,
                            Salva_Prodotto,
                            false
                        );

                        //Lasciare questa patchValue all'Original_Row_Value come ultima istruzione
                        //prima del push nell'array delle DosiProdotti
                        newRow.get("Original_Row_Value").patchValue(cloneDeep(newRow.value));

                        sezione_prodotto.DosiProdotti.push(newRow.value);

                    }
                }else{

                  //Aggiorno la QuantitaSuImpianti nel caso di multicentro
                  if(ProdottoDaInserire?.quantitaSuImpianti &&  ProdottoDaInserire?.quantitaSuImpianti.length > 0){
                    let Qt: QuantitaSuImpianto[] = sezione_prodotto.DosiProdotti[index_prodotto_gia_presente].QuantitaSuImpianti;

                    let Qt_ProdottoDaInserire: QuantitaSuImpianto[] = ProdottoDaInserire.quantitaSuImpianti;

                    if(Qt && Qt.length > 0 && Qt_ProdottoDaInserire && Qt_ProdottoDaInserire.length > 0){
                      Qt_ProdottoDaInserire.forEach(q=>{
                        if(this.qdcservice.QuantitaSuImpianto_Presente(Qt,q) === -1){
                          sezione_prodotto.DosiProdotti[index_prodotto_gia_presente].QuantitaSuImpianti.push(q);
                        }
                      });
                    }
                  }
                }

            }

        }

    }

    private setSezioneSenzaProdottoModel(
        Risorsa: DettaglioRilievo,
        qdCFormModel: QdCFormModel,
        lavorazione: Lavorazione,
        attivita_con_parametri_aggiuntivi: Attivita_Con_Parametri_Aggiuntivi
    ) {
        let lav_cod: number = + lavorazione.primaryKey.codice;

        let sezione: Sezione_Rilievi;

        // Ceco di recuperare la sezione
        if(qdCFormModel?.Trattamento?.Sezioni_Senza_Prodotto?.length > 0 ) {
            sezione = qdCFormModel.Trattamento.Sezioni_Senza_Prodotto.find(
                s => s.Operazione.primaryKey.codice === lavorazione.primaryKey.codice
            );
        }

        if (!sezione) {
            sezione = this.creaSezioneSenzaProdotto(lav_cod, Risorsa);
            sezione.Operazione = lavorazione;
            qdCFormModel.Trattamento.Sezioni_Senza_Prodotto.push(sezione);
        } else {
            sezione.DettagliRilievi.push(Risorsa);
        }
    }

    private creaSezioneSenzaProdotto(
        lav_cod: number | string,
        risorsa: DettaglioRilievo
    ) : Sezione_Rilievi
    {
        let sezione: Sezione_Rilievi;

        switch (lav_cod) {
            case enum_LAVCOD.DANNI_RACCOLTA:
            case enum_LAVCOD.FASI_FENOLOGICHE:
            case enum_LAVCOD.RILIEVO_AVVERSITA_CAMPO:
            case enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA:
            case enum_LAVCOD.RILIEVO_INDICI_MATURITA:
            case enum_LAVCOD.RILIEVO_ERBE_INFESTANTI:
          case enum_LAVCOD.RILIEVO_AVVERSITA_TRAPPOLE:
                sezione  = new Sezione_Rilievi();
                (sezione as Sezione_Rilievi).DettagliRilievi.push(risorsa as DettaglioRilievo);
                break;
            default: break;
        }

        return sezione;
    }

    private setFormArrayinQdCForm(qdCFormModel: QdCFormModel,crea_sezione_prodotti: boolean){

        //Imposto il FormGroup degli ImpiantiSelezionati
        if(qdCFormModel.Trattamento?.Impianti?.ImpiantiSelezionati &&
            qdCFormModel.Trattamento?.Impianti?.ImpiantiSelezionati.length > 0){

            qdCFormModel.Trattamento.Impianti.ImpiantiSelezionati.forEach(I=>{
                this.qdcservice.ImpiantiSelezionatiFormArray.push(new FormControl(I));
            });

        }

        //Imposto il FormGroup degli ProdottiDaTrattareSelezionati
        if (qdCFormModel.Trattamento?.ProdottiDaTrattare?.ProdottiDaTrattareSelezionati?.length > 0){
            qdCFormModel.Trattamento.ProdottiDaTrattare.ProdottiDaTrattareSelezionati.forEach(x => this.qdcservice.ProdottiDaTrattareSelezionatiFormArray.push(new FormControl(x)));
        }

        //Imposto il FormGroup degli Operatori
        if(qdCFormModel.Trattamento?.CostiAccessori?.Operatori &&
            qdCFormModel.Trattamento?.CostiAccessori?.Operatori.length > 0){

            qdCFormModel.Trattamento.CostiAccessori.Operatori.forEach(o=>{
                this.qdcservice.OperatoriFormArray.push(new FormControl(o));
            });

        }

        //Imposto il FormGroup delle Macchine
        if(qdCFormModel.Trattamento?.CostiAccessori?.Macchine &&
            qdCFormModel.Trattamento?.CostiAccessori?.Macchine.length > 0){

            qdCFormModel.Trattamento.CostiAccessori.Macchine.forEach(m=>{
                this.qdcservice.MacchineFormArray.push(new FormControl(m));
            });

        }

        //Imposto il FormGroup delle Note
        if(qdCFormModel.Trattamento?.Note &&
            qdCFormModel.Trattamento?.Note.length >0){

            qdCFormModel.Trattamento.Note.forEach(N=>{
                this.qdcservice.NoteFormArray.push(new FormControl(N));
            });

        }

        //Imposto il FormGroup della Sezioni_Prodotto
        if (qdCFormModel.Trattamento?.Sezioni_Prodotto?.length > 0
            || qdCFormModel.Trattamento?.Sezioni_Senza_Prodotto?.length > 0 ) {

            qdCFormModel.Trattamento.Sezioni_Prodotto.forEach(s=>{

                const FormSezione = this.qdcservice.getSezioneProdottiForm(s.Categoria_Magazzino, s.Operazione);

                //Aggiorno il valore dei controlli che non sono nella griglia dei dosaggi dei prodotti
                switch(s.Categoria_Magazzino){
                    case FORMULATI:
                    case INSETTI:
                        s = s as Sezione_Prodotto_Formulati;

                        if(s.Categoria_Magazzino ===  FORMULATI)
                            FormSezione.patchValue({
                                EpocaDPI: s.EpocaDPI,
                                Ripartizione_Trappole: s.Ripartizione_Trappole
                            });
                        break;
                    case FERTILIZZANTI:
                        s = s as Sezione_Prodotto_Fertilizzanti;
                        break;
                    case SEMENTI:
                        s = s as Sezione_Prodotto_Sementi;

                        FormSezione.patchValue({
                            Opzioni_Semina: s.Opzioni_Semina
                        });
                        break;
                    case TRASFORMATI_VEGETALI:

                        if(+ s.Operazione.primaryKey.codice === enum_LAVCOD.RACCOLTA){
                            s = s as Sezione_Prodotto_Raccolta;

                            FormSezione.patchValue({
                                FastHarvestTemplate: new DettaglioRaccolta(Tipo_Raccolta.Fast)
                            })
                            FormSezione.patchValue({
                                Opzioni_Raccolta: s.Opzioni_Raccolta
                            });
                            if (s.Data_Raccolta < qdCFormModel.Trattamento.Testata.Data)
                                FormSezione.patchValue({Data_Raccolta: qdCFormModel.Trattamento.Testata.Data});
                            else
                                FormSezione.patchValue({Data_Raccolta: s.Data_Raccolta});

                            let prodotti = FormSezione.get('ProdottiRaccolti') as FormArray;

                            s.ProdottiRaccolti.forEach( prodotto => {
                                prodotti.push(new FormControl(prodotto));
                            })
                        }

                        break;
                }

                if(s.DosiProdotti && s.DosiProdotti.length > 0) {

                    let DosiProdotti = FormSezione.get("DosiProdotti") as FormArray;

                    s.DosiProdotti.forEach(p=>{
                        this.qdcservice.AggiornaFormArrayGridDosiProdotti(p,-1,p.Operazione,enum_TipoOperazioneDB.Scrittura,DosiProdotti);
                    });

                }

                this.qdcservice.Sezioni_ProdottoFormArray.push(FormSezione);

                this.qdcservice.Mostra_Sezioni_Prodotto = false;
            });

            qdCFormModel.Trattamento.Sezioni_Senza_Prodotto.forEach((sezRis) => {
                let FormSezione = this.qdcservice.get_Sezione_Senza_Prodotto_Form(sezRis.Operazione);

                if (sezRis.DettagliRilievi?.length) {
                    sezRis.DettagliRilievi.forEach((r) => {
                        (FormSezione.get('DettagliRilievi') as FormArray)
                            .push(new FormGroup({
                                faseFenologica: new FormControl(r['FaseFenologica']),
                                indiceMaturita: new FormControl(r['IndiceMaturita']),
                                indiceResa: new FormControl(r['IndiceResa']),
                                dannoRaccolta: new FormControl(r['DannoRaccolta']),
                                erbaInfestante: new FormControl(r['erbaInfestante']),
                                avversitaGruppo: new FormControl(r['avversitaGruppo']),
                                dataOraRilievo: new FormControl(r['DataOraRilievo']),
                                qtaRilevata: new FormControl(r['QtaRilevata']),
                                impianto: new FormControl(r['Impianto']),
                                esercizioCDC: new FormControl(r['esercizioCDC']),
                                descrizione: new FormControl(r['Descrizione']),
                                qtaRilevataString: new FormControl(r['QtaRilevataString']),
                                classType: new FormControl(r['classType']),
                                id_agenda: new FormControl(r['id_agenda']),
                                codRilievo: new FormControl(r['codRilievo']),
                                presets: new FormControl(r['presets']),
                                note: new FormControl(r['Note']),
                                unitaDiMisura: new FormControl(r['unitaDiMisura']),
                                risorsaProdotto: new FormControl(r['risorsaProdotto'])
                            }))
                    })
                }

                this.qdcservice.Sezioni_Senza_ProdottoFormArray.push(FormSezione);
                this.qdcservice.Mostra_Sezioni_Senza_Prodotto = false;
            });

        }else if(crea_sezione_prodotti){

            this.qdcservice.Gestisci_Sezioni(qdCFormModel.Trattamento?.Testata?.Operazioni);
            this.griddosiprodottiservice.AggiungiRigaVuotaGridDosiProdotti();

        }

    }

    // @description:
    //Controllo se sono nel MultiCentro dove mi arriveranno varie attivita (in base al numero dei Centri) con lo stesso lav_Cod e stessi prodotti
    //ma con Quantita totali differenti. Se invece i valori sono stati calcolati per Dose/ha e Dose/hl sono uguali per tutte le attivita di centri diversi e non li devo sommare.
    // In questo caso dovrà creare solo una riga di prodotti calcolando correttamente
    //tutti i vari dosaggi
    Gestisci_Qta_Prodotto_x_MultiCentro(qdCFormModel: QdCFormModel,
                                        listaAttivita_con_parametri_aggiuntivi: Attivita_Con_Parametri_Aggiuntivi[]){

        if(qdCFormModel.Trattamento.Testata.MultiCentro) {

            if(qdCFormModel.Trattamento.Sezioni_Prodotto && qdCFormModel.Trattamento.Sezioni_Prodotto.length > 0) {
                qdCFormModel.Trattamento.Sezioni_Prodotto.forEach(sezione_prodotto=> {

                    if((sezione_prodotto.Categoria_Magazzino === FORMULATI ||
                            sezione_prodotto.Categoria_Magazzino === FERTILIZZANTI ||
                            sezione_prodotto.Categoria_Magazzino === SEMENTI ||
                            sezione_prodotto.Categoria_Magazzino === INSETTI) &&
                        (sezione_prodotto.DosiProdotti && sezione_prodotto.DosiProdotti.length > 0)){

                        sezione_prodotto.DosiProdotti.forEach((dose_prodotto:any)=>{


                            listaAttivita_con_parametri_aggiuntivi.forEach(Attivita_con_parametri_aggiuntivi=> {

                                if(
                                    Attivita_con_parametri_aggiuntivi.job.primaryKey.codice === dose_prodotto.Operazione.primaryKey.codice &&
                                    (
                                        Attivita_con_parametri_aggiuntivi.centroAziendale.primaryKey.partitaIva !== dose_prodotto.Centro_Aziendale.primaryKey.partitaIva ||
                                        Attivita_con_parametri_aggiuntivi.centroAziendale.primaryKey.codice !== dose_prodotto.Centro_Aziendale.primaryKey.codice
                                    )
                                ) {

                                    let risorsaProdotto: RisorsaProdotto = null;

                                    switch(sezione_prodotto.Categoria_Magazzino) {
                                        case FORMULATI:
                                        case INSETTI:
                                            risorsaProdotto = (Attivita_con_parametri_aggiuntivi.risorse.filter(r=>r.classType === "DettaglioTrattamento") as DettaglioTrattamento[]).find(d=> { return this.griddosiprodottiservice.Controlla_Se_Stesso_Formulato(d,dose_prodotto.Prodotto); }) as RisorsaProdotto;
                                            break;
                                        case FERTILIZZANTI:
                                            risorsaProdotto = (Attivita_con_parametri_aggiuntivi.risorse.filter(r=>r.classType === "DettaglioFertilizzazione") as DettaglioFertilizzazione[]).find(d=>{ return this.Controlla_Se_Stesso_Fertilizzante(d,dose_prodotto.Prodotto); }) as RisorsaProdotto;
                                            break;
                                        case SEMENTI:
                                            risorsaProdotto = (Attivita_con_parametri_aggiuntivi.risorse.filter(r=>r.classType === "DettaglioSemina") as DettaglioSemina[]).find(d=>{ return this.Controlla_Se_Stessa_Semente(d,dose_prodotto.Prodotto); }) as RisorsaProdotto;
                                            break;
                                    }

                                    if(risorsaProdotto) {

                                        if(dose_prodotto.flagDoseQuantitaTotale === enum_doseQuantitaTotale.Qta_Totale) {

                                            let QtaTot: number = 0;

                                            if(risorsaProdotto.MagazziniMovimentazioni && risorsaProdotto.MagazziniMovimentazioni.length > 0){

                                              risorsaProdotto.MagazziniMovimentazioni.forEach(rilevamentoMagazzino => {

                                                if(this.griddosiprodottiservice.Controlla_Se_Stesso_Magazzino(dose_prodotto.Prodotto.MagazziniMovimentazioni,[rilevamentoMagazzino])) {
                                                  QtaTot += this.griddosiprodottiservice.getDosePerMagazzino(rilevamentoMagazzino.Qta, risorsaProdotto.unitaDiMisuraIndicata?.codice);
                                                }

                                              })
                                            }else{
                                              QtaTot = risorsaProdotto.quantitaTotaleReale;
                                            }

                                            dose_prodotto.DoseTot_Ha = QtaTot + dose_prodotto.DoseTot_Ha;

                                        }


                                    }

                                    //Calcolo la DoseTot_Ha_Innesco nel caso di MultiCentro
                                    if(sezione_prodotto.Categoria_Magazzino === FORMULATI &&
                                      dose_prodotto.Magazzino_Innesco &&
                                      dose_prodotto.Avversita){

                                      let avversita: AvversitaGruppo[] = (Attivita_con_parametri_aggiuntivi.risorse.filter(r=>r.classType === "DettaglioTrattamento") as DettaglioTrattamento[]).filter(d=>{ return this.Controlla_Se_Stessa_Avversita(d.avversitaGruppo, dose_prodotto.Avversita); }).map(d=>{return d.avversitaGruppo});

                                      if(avversita && avversita.length > 0){
                                        avversita.forEach(a=>{
                                          if(a.MagazziniMovimentazioni && a.MagazziniMovimentazioni.length > 0){
                                            a.MagazziniMovimentazioni.forEach(m=>{
                                              dose_prodotto.DoseTot_Ha_Innesco += m.Qta;
                                            });
                                          }
                                        });
                                      }

                                    }

                                }

                            });
                        });

                    }

                })

                //Calcolo e Arrotondo tutti i valori delle Qta
                qdCFormModel.Trattamento.Sezioni_Prodotto.forEach(sezione_prodotto=>{

                    if(
                        (
                            sezione_prodotto.Categoria_Magazzino === FORMULATI ||
                            sezione_prodotto.Categoria_Magazzino === FERTILIZZANTI ||
                            sezione_prodotto.Categoria_Magazzino === SEMENTI ||
                            sezione_prodotto.Categoria_Magazzino === INSETTI
                        ) &&
                        (sezione_prodotto.DosiProdotti && sezione_prodotto.DosiProdotti.length > 0)
                    ) {

                        sezione_prodotto.DosiProdotti.forEach(dose_prodotto => {

                            let lav_cod: number = +dose_prodotto.Operazione.primaryKey.codice;
                            let Dose_Ha: number = dose_prodotto.Dose_Ha;
                            let Dose_Hl: number = dose_prodotto.Dose_Hl;
                            let DoseTot_Ha: number = dose_prodotto.DoseTot_Ha
                            let Sup_Trattata: number = qdCFormModel.Trattamento.Superfici.Sup_Trattata;
                            let Acqua_Tot: number = qdCFormModel.Trattamento.Acqua.Acqua_Tot;
                            let flag_acqua: boolean = this.qdcservice.Elenco_Operazioni_con_Acqua.includes(lav_cod);
                            let Operazione: Lavorazione = dose_prodotto.Operazione;
                            let UdM: UnitaDiMisura = dose_prodotto.UdM;

                            if(dose_prodotto.flagDoseQuantitaTotale === enum_doseQuantitaTotale.Qta_Totale) {

                                dose_prodotto.DoseTot_Ha = this.funzionicomuniservice.roundNumber(dose_prodotto.DoseTot_Ha, this.qdcservice.getProductNumericSettings(UdM).decimals);

                                let obj_Dose_ha_hl = this.misceleservice.calcola_dose_ha_hl_da_dose_tot(Dose_Ha, Dose_Hl,
                                    Sup_Trattata, flag_acqua,Acqua_Tot, DoseTot_Ha,Operazione,UdM);

                                dose_prodotto.Dose_Ha = obj_Dose_ha_hl.Dose_Ha;

                                dose_prodotto.Dose_Hl = obj_Dose_ha_hl.Dose_Hl;
                            } else if(dose_prodotto.flagDoseQuantitaTotale === enum_doseQuantitaTotale.Dose) {

                                switch(dose_prodotto.flagTipoDose) {
                                    case enum_TipoMezzo.Ettaro:
                                        let obj_Dose_Hl_Tot = this.misceleservice.calcola_dose_hl_tot_ha_da_dose_ha(Dose_Hl, DoseTot_Ha,
                                            Dose_Ha,Sup_Trattata, flag_acqua, Acqua_Tot, Operazione,UdM);

                                        dose_prodotto.DoseTot_Ha = obj_Dose_Hl_Tot.DoseTot_Ha;

                                        dose_prodotto.Dose_Hl = obj_Dose_Hl_Tot.Dose_Hl;
                                        break;
                                    case enum_TipoMezzo.Ettolitro:
                                        let obj_Dose_Ha_Tot = this.misceleservice.calcola_dose_ha_tot_ha_da_dose_hl(Dose_Ha, DoseTot_Ha,
                                            Dose_Hl,Sup_Trattata, flag_acqua,Acqua_Tot,UdM);

                                        dose_prodotto.Dose_Ha = obj_Dose_Ha_Tot.Dose_Ha;

                                        dose_prodotto.DoseTot_Ha = obj_Dose_Ha_Tot.DoseTot_Ha;
                                        break;
                                }

                            }

                            if(dose_prodotto.DoseTot_Ha_Innesco > 0){
                                dose_prodotto.DoseTot_Ha_Innesco = this.funzionicomuniservice.roundNumber(dose_prodotto.DoseTot_Ha_Innesco, this.qdcservice.getProductNumericSettings(dose_prodotto.UdM_Innesco).decimals);
                            }

                        });

                    }

                });
            }
        }

    }


    Controlla_Se_Inserire_Prodotto(
        sezione_prodotto: Sezione_Prodotto_Raccolta | Sezione_Prodotto_Sementi | Sezione_Prodotto_Formulati | Sezione_Prodotto_Fertilizzanti,
        ProdottoDaInserire: MultiColumnComboboxSemina | MultiColumnComboboxTrattamento | MultiColumnComboboxFertilizzazione,
        Operazione: Lavorazione
    ): number {

        let index: number = -1;

        let Categoria_Magazzino_Prodotto = this.qdcservice.getCategoria_Magazzino(+ Operazione.primaryKey.codice);

        if(sezione_prodotto.Categoria_Magazzino === Categoria_Magazzino_Prodotto){
            switch (Categoria_Magazzino_Prodotto){
                case FORMULATI:
                case INSETTI:

                    let ProdottoDaInserire_Trattamento = ProdottoDaInserire as MultiColumnComboboxTrattamento;

                    index = sezione_prodotto.DosiProdotti.findIndex(d=> d.Operazione.primaryKey.codice === Operazione.primaryKey.codice &&
                      this.griddosiprodottiservice.Controlla_Se_Stesso_Formulato(d.Prodotto,ProdottoDaInserire_Trattamento));
                    break;
                case FERTILIZZANTI:

                    let ProdottoDaInserire_Fertilizzante = ProdottoDaInserire as MultiColumnComboboxFertilizzazione;

                    index = sezione_prodotto.DosiProdotti.findIndex(d=> d.Operazione.primaryKey.codice === Operazione.primaryKey.codice &&
                        this.Controlla_Se_Stesso_Fertilizzante(d.Prodotto,ProdottoDaInserire_Fertilizzante));
                    break;
                case SEMENTI:

                    let ProdottoDaInserire_Semente = ProdottoDaInserire as MultiColumnComboboxSemina;

                    index = sezione_prodotto.DosiProdotti.findIndex(d=> d.Operazione.primaryKey.codice === Operazione.primaryKey.codice &&
                        this.Controlla_Se_Stessa_Semente(d.Prodotto,ProdottoDaInserire_Semente));
                    break;
            }
        }


        return index;
    }

    private Controlla_Se_Stesso_Fertilizzante(fertilizzante_1: DettaglioFertilizzazione | MultiColumnComboboxFertilizzazione, fertilizzante_2: DettaglioFertilizzazione | MultiColumnComboboxFertilizzazione):boolean{

        let stesso_fertilizzante:boolean = false;

        if(fertilizzante_1.prodotto.codice === fertilizzante_2.prodotto.codice &&
          this.griddosiprodottiservice.Controlla_Se_Stesso_Magazzino(fertilizzante_1.MagazziniMovimentazioni,fertilizzante_2.MagazziniMovimentazioni)){

            stesso_fertilizzante = true;

        }

        return stesso_fertilizzante;

    }

    private Controlla_Se_Stessa_Semente(semente_1: DettaglioSemina | MultiColumnComboboxSemina, semente_2: DettaglioSemina | MultiColumnComboboxSemina): boolean{
        let stessa_semente:boolean = false;

        if(semente_1.prodotto.codice === semente_2.prodotto.codice &&
            semente_1.prodotto.elemCod === semente_2.prodotto.elemCod &&
          this.griddosiprodottiservice.Controlla_Se_Stesso_Magazzino(semente_1.MagazziniMovimentazioni,semente_2.MagazziniMovimentazioni)){

            stessa_semente = true;

        }

        return stessa_semente;
    }

    private Controlla_Se_Stessa_Raccolta(raccolta_1: DettaglioRaccolta, raccolta_2: DettaglioRaccolta): boolean{
        const stessoProdotto = raccolta_1?.prodotto?.codice === raccolta_2?.prodotto?.codice
            && raccolta_1?.prodotto?.elemCod === raccolta_2?.prodotto?.elemCod;
        if (!stessoProdotto) {
            return false;
        }

        const stessoLotto = raccolta_1.QuantitaSuImpianti?.at(0)?.Lotto === raccolta_2.QuantitaSuImpianti?.at(0)?.Lotto;

        if (!raccolta_1.QuantitaSuImpianti?.at(0)?.Magazzino && !raccolta_2.QuantitaSuImpianti?.at(0)?.Magazzino) {
            // Nessuna delle due ha carichi di magazzino
            return stessoProdotto && stessoLotto;
        } else if (!raccolta_1.QuantitaSuImpianti?.at(0)?.Magazzino || !raccolta_2.QuantitaSuImpianti?.at(0)?.Magazzino) {
            // Una sola delle due non ha carichi di magazzino
            return false;
        }

        const getPKMagazzinoString = (magazzino: Fabbricato) => magazzino?.primaryKey?.centroAziendalePK?.partitaIva
            + "_" + magazzino?.primaryKey?.centroAziendalePK?.codice
            + "_" + magazzino?.primaryKey?.codice;
        const stessoMagazzino = getPKMagazzinoString(raccolta_1.QuantitaSuImpianti[0].Magazzino)
            === getPKMagazzinoString(raccolta_2.QuantitaSuImpianti[0].Magazzino)

        return stessoProdotto && stessoLotto && stessoMagazzino;
    }

    private Controlla_Stesso_LottoMagazzino(q1: QuantitaSuImpianto, q2: QuantitaSuImpianto) {
        const stessoLotto = q1.Lotto === q2.Lotto;

        if (!q1.Magazzino && !q2.Magazzino) {
            // Nessuna delle due ha carichi di magazzino
            return stessoLotto;
        } else if (!q1.Magazzino || !q2.Magazzino) {
            // Una sola delle due non ha carichi di magazzino
            return false;
        }

        const getPKMagazzinoString = (magazzino: Fabbricato) => magazzino.primaryKey.centroAziendalePK.partitaIva
            + "_" + magazzino.primaryKey.centroAziendalePK.codice
            + "_" + magazzino.primaryKey.codice;
        const stessoMagazzino = getPKMagazzinoString(q1.Magazzino)
            === getPKMagazzinoString(q2.Magazzino)

        return stessoLotto && stessoMagazzino;
    }

    private Controlla_Se_Stessa_Avversita(avversita_1: MultiColumnComboboxAvversitaInnesco | AvversitaGruppo, avversita_2: MultiColumnComboboxAvversitaInnesco | AvversitaGruppo):boolean{

      let stesso_avversita:boolean = false;

      if(avversita_1.codice === avversita_2.codice &&
        this.griddosiprodottiservice.Controlla_Se_Stesso_Magazzino(avversita_1.MagazziniMovimentazioni,avversita_2.MagazziniMovimentazioni)){

        stesso_avversita = true;

      }

      return stesso_avversita;

    }

    /**
     * Controlla che due centri di costo appartengano allo stesso centro.
     * @param q1
     * @param q2
     * @return true se sono dello stesso centro, false altrimenti.
     * @private
     */
    private ImpiantiStessoCentro(q1: EsercizioCDC, q2: EsercizioCDC): boolean {
        return q1.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
            === q2.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
            && q1.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
            === q2.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice;
    }

    private set_ProblemiProdotto(Prodotto: MultiColumnComboboxTrattamento | MultiColumnComboboxFertilizzazione | MultiColumnComboboxSemina,
                                 attivita_con_parametriaggiuntivi: Attivita_Con_Parametri_Aggiuntivi){

        let problema_prodotto = this.CheckProblemi_Prodotto(Prodotto,attivita_con_parametriaggiuntivi);


        if(problema_prodotto !== enum_Problema_DettaglioProdotto.Nessuno){
          Prodotto.Tutti_Problemi_DettaglioProdotto.push(problema_prodotto);
        }

    }

    /*
    * @description
    * Ottengo l'attivita da impostare nella dall' objParametriAgenda.GenericObj_string
    * (Utilizzato per esempio per aprire la pagina con dei dati già preimpostati quando arrivo dal Piano Concimazione PUA)
    * Nell'objParametriAgenda.GenericObj_string salvo anche il Redirect_To_GiasNG_Page se devo aprire la pagina del QdC in iframe da angular
    * */
    private setQdCFormModelfromGenericObj(): QdCFormModel{

        let qdCFormModel = null;

        let objParametriAgenda_Attivita: Attivita = null;

        let genericObj: string = this.objParametriAgenda.GenericObj_string;

        if(genericObj && genericObj !== ""){

            let obj = JSON.parse(genericObj);

            if(obj['objParametriAgenda'] && obj['QdCFormValue']){
              this.gestionerichiesteservice.ObjPageToMemorize = this.conversionService.ConversionDateInObject(<Redirect_To_GiasNG_Page>(obj));
            }else {
              objParametriAgenda_Attivita = this.conversionService.ConversionDateInObject(<Attivita>(obj));
            }
        }


        if(objParametriAgenda_Attivita){

            let attivita: Attivita = objParametriAgenda_Attivita;

            if((this.objParametriAgenda.Sito_Provenienza !== Enum_SiteRedirector.GiasNG) ||
                (this.objParametriAgenda.Sito_Provenienza === Enum_SiteRedirector.GiasNG && this.objParametriAgenda.Pagina_Provenienza !== enum_PagineGiasNG.Pagina_Menu_Agenda && this.objParametriAgenda.Pagina_Provenienza !== enum_PagineGiasNG.Pagina_Edit_Attivita) ||
                (this.objParametriAgenda.Sito_Provenienza === Enum_SiteRedirector.GiasNG && this.objParametriAgenda.Pagina_Provenienza !== enum_PagineGiasNG.Pagina_Menu_Visite && this.objParametriAgenda.Pagina_Provenienza !== enum_PagineGiasNG.Pagina_Edit_Visite)){

                this.qdcservice.Apertura_QdC ={
                    flag_QdC_Aperta_da_Altra_Pagina: true,
                    flag_Selezione_Impianti: true
                };

            }

            qdCFormModel = this.mapListaAttivitaToQdCFormModel([attivita as Attivita_Con_Parametri_Aggiuntivi],true);

            //Imposto il disciplinare direttiva Nitrati
            if(+ attivita.job.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI && attivita.disciplinare &&
                attivita.disciplinare?.regolamentoConcimazione?.tipo === enum_PUARegolamenti_Tipo.PUA){

                if(this.qdcservice.Sezioni_ProdottoFormArray && this.qdcservice.Sezioni_ProdottoFormArray.getRawValue().length > -1){
                    this.qdcservice.Sezioni_ProdottoFormArray.controls.forEach((sezione_form: FormGroup) =>{

                        let Operazione: Lavorazione = sezione_form.getRawValue().Operazione;

                        if(+ Operazione.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI){

                            sezione_form.patchValue({
                                Utilizza_Direttiva_Nitrati: true,
                                Direttiva_Nitrati: attivita.disciplinare
                            },{emitEvent: false});

                            this.qdcservice.DosiProdottiFormArray(sezione_form,null).controls.forEach((d:FormGroup)=>{
                                d.patchValue({
                                    Utilizza_Direttiva_Nitrati: true,
                                    Direttiva_Nitrati: attivita.disciplinare
                                },{emitEvent: false});
                            });
                        }
                    });
                }

            }



        }

        if(genericObj && genericObj !== ""){
          //Ripulisco il GenericObj_string appeno ne ho letto il valore
          this.objParametriAgenda.GenericObj_string = "";

          this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
        }

        return qdCFormModel;
    }

    private get_CodiceVisita_from_Parametri_Aggiuntivi(list_attivita_con_parametriaggiuntivi: Attivita_Con_Parametri_Aggiuntivi[]):string{

      let CodiceVisita: string = "0";

      if(list_attivita_con_parametriaggiuntivi && list_attivita_con_parametriaggiuntivi.length > 0){

        for(let attivita_con_parametriaggiuntivi of list_attivita_con_parametriaggiuntivi){

          if(CodiceVisita && CodiceVisita !== "" && + CodiceVisita !== 0)
            break;

          if (attivita_con_parametriaggiuntivi && attivita_con_parametriaggiuntivi.parametri_aggiuntivi && attivita_con_parametriaggiuntivi.parametri_aggiuntivi.length > 0) {

            for (let p of attivita_con_parametriaggiuntivi.parametri_aggiuntivi) {
              if(p.key === Key_Parametri_Aggiuntivi.Id_Visita_Collegata && p.value && p.value !== "" && + p.value !== 0){
                CodiceVisita = p.value;
              }
            }
          }
        }
      }

      return CodiceVisita;

    }

    private resetForReinnesco(listaAttivita: Attivita_Con_Parametri_Aggiuntivi[],Default_Data: Date){

      if(this.qdcservice.flag_Reinnesco){
        listaAttivita.map((a)=> {
          a.inizio = Default_Data;
          a.codice = "0";
          a.descrizione = "";
          a.raccoglitore = 0;
          a.job = new Lavorazione(this.objParametriAgenda.Lav_Cod.toString());
          a.job.descrizione = this.objParametriAgenda.Lav_Des;
          a.attivitaCollegate = [];

          //Elimino le Movimentazioni di Magazzino del Formulato (Trappola)
          if(a.risorse && a.risorse.findIndex(r=>r.classType === "DettaglioTrattamento") > -1){
            a.risorse.forEach(r=>{
              if(r.classType === "DettaglioTrattamento"){
                (r as DettaglioTrattamento).MagazziniMovimentazioni = [];
              }
            });
          }

        } );
      }

    }

    /*
    * @description Individua se il DettaglioTrattamento passato ha dei dati non valorizzati o non corretti (Prodotti_Non_Corretti, Prodotti_Ambigui, Avversita_Non_Valorizzate
    * Avversita_Non_Corrette, Avversita_Ambigue)
    * */
    private CheckProblemi_Prodotto(Prodotto: MultiColumnComboboxTrattamento | MultiColumnComboboxFertilizzazione | MultiColumnComboboxSemina,attivita_con_parametriaggiuntivi: Attivita_Con_Parametri_Aggiuntivi): enum_Problema_DettaglioProdotto{

      let problema_prodotto: enum_Problema_DettaglioProdotto = enum_Problema_DettaglioProdotto.Nessuno;

      if(Prodotto && Prodotto.prodotto){

        if (attivita_con_parametriaggiuntivi && attivita_con_parametriaggiuntivi.parametri_aggiuntivi && attivita_con_parametriaggiuntivi.parametri_aggiuntivi.length > 0) {

          for (let p of attivita_con_parametriaggiuntivi.parametri_aggiuntivi) {
            if (p.value !== "") {

              let dettagliTrattamento: DettaglioTrattamento[] = [];
              let dettagliFertilizzazione: DettaglioFertilizzazione[] = [];

              if(Prodotto.prodotto.elemCod === FORMULATI || Prodotto.prodotto.elemCod === INSETTI){
                dettagliTrattamento = JSON.parse(p.value);
              }

              if(Prodotto.prodotto.elemCod === FERTILIZZANTI){
                dettagliFertilizzazione = JSON.parse(p.value);
              }

              let index = -1;

              switch (p.key) {
                case Key_Parametri_Aggiuntivi.Formulati_Non_Corretti:
                case Key_Parametri_Aggiuntivi.Formulati_Ambigui:
                case Key_Parametri_Aggiuntivi.Avversita_Non_Valorizzate:
                  index = dettagliTrattamento.findIndex(d => d.prodotto.codice === Prodotto.prodotto.codice &&
                    d.tipoFormulato === (Prodotto as MultiColumnComboboxTrattamento).tipoFormulato &&
                    d.dettaglioProdotto === (Prodotto as MultiColumnComboboxTrattamento).dettaglioProdotto);

                  if (index > -1) {
                    if (p.key === Key_Parametri_Aggiuntivi.Formulati_Non_Corretti) {
                      problema_prodotto = enum_Problema_DettaglioProdotto.Formulato_Non_Corretto;
                    } else if (p.key === Key_Parametri_Aggiuntivi.Formulati_Ambigui) {
                      problema_prodotto = enum_Problema_DettaglioProdotto.Formulato_Ambiguo;
                    } else if (p.key === Key_Parametri_Aggiuntivi.Avversita_Non_Valorizzate) {
                      problema_prodotto = enum_Problema_DettaglioProdotto.Avversita_Non_Valorizzata;
                    }
                  }
                  break;
                case Key_Parametri_Aggiuntivi.Avversita_Non_Corrette:
                case Key_Parametri_Aggiuntivi.Avversita_Ambigue:

                  index = dettagliTrattamento.findIndex(d => d.prodotto.codice === Prodotto.prodotto.codice &&
                    d.tipoFormulato === (Prodotto as MultiColumnComboboxTrattamento).tipoFormulato &&
                    d.dettaglioProdotto === (Prodotto as MultiColumnComboboxTrattamento).dettaglioProdotto &&
                    d.avversitaGruppo.codice === (Prodotto as MultiColumnComboboxTrattamento)?.avversitaGruppo?.codice &&
                    d.avversitaGruppo.classType === (Prodotto as MultiColumnComboboxTrattamento)?.avversitaGruppo?.classType);

                  if (index > -1) {
                    if (p.key === Key_Parametri_Aggiuntivi.Avversita_Non_Corrette) {
                      problema_prodotto = enum_Problema_DettaglioProdotto.Avversita_Non_Corretta;
                    } else if (p.key === Key_Parametri_Aggiuntivi.Avversita_Ambigue) {
                      problema_prodotto = enum_Problema_DettaglioProdotto.Avversita_Ambigua;
                    }
                  }
                  break;

                case Key_Parametri_Aggiuntivi.Fertilizzanti_Non_Corretti:
                case Key_Parametri_Aggiuntivi.Fertilizzanti_Ambigui:
                  index = dettagliFertilizzazione.findIndex(d => d.prodotto.codice === Prodotto.prodotto.codice);

                  if (index > -1) {
                    if (p.key === Key_Parametri_Aggiuntivi.Fertilizzanti_Non_Corretti) {
                      problema_prodotto = enum_Problema_DettaglioProdotto.Fertilizzante_Non_Corretto;
                    } else if (p.key === Key_Parametri_Aggiuntivi.Fertilizzanti_Ambigui) {
                      problema_prodotto = enum_Problema_DettaglioProdotto.Fertilizzante_Ambiguo;
                    }
                  }
                  break;
              }

              if(problema_prodotto !== enum_Problema_DettaglioProdotto.Nessuno)
                break;

            }
          }
        }
      }

      return problema_prodotto;
    }

}
