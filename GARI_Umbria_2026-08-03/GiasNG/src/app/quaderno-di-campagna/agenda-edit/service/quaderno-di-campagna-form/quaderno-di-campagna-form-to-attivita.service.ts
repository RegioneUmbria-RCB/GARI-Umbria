/* eslint-disable */
import {Inject, Injectable, LOCALE_ID} from '@angular/core';
import {FormGroup} from '@angular/forms';
import {CentroAziendale} from 'app/Model/anagrafiche/CentroAziendale';
import {Contatto, PK} from 'app/Model/anagrafiche/Contatto';
import {Fabbricato} from 'app/Model/anagrafiche/Fabbricato';
import {ParcoMacchine} from 'app/Model/anagrafiche/ParcoMacchine';
import {RapportoContabile} from 'app/Model/anagrafiche/RapportoContabile';
import {RisorseUmane} from '../../../../Model/anagrafiche/RisorseUmane';
import {Attivita, Tipo_Raccolta} from 'app/Model/attivita/Attivita';
import {AttivitaPersonalizzata} from 'app/Model/attivita/AttivitaPersonalizzata';
import {EsercizioCDC} from 'app/Model/attivita/centri_di_costo/EsercizioCDC';
import {DettaglioFertilizzazione} from 'app/Model/attivita/dettagli/DettaglioFertilizzazione';
import {ConsiglioIrrigazione, DettaglioIrrigazione} from 'app/Model/attivita/dettagli/DettaglioIrrigazione';
import {DettaglioRaccolta} from 'app/Model/attivita/dettagli/DettaglioRaccolta';
import {DettaglioSemina} from 'app/Model/attivita/dettagli/DettaglioSemina';
import {DettaglioTrattamento} from 'app/Model/attivita/dettagli/DettaglioTrattamento';
import {QuantitaSuImpianto} from 'app/Model/attivita/dettagli/QuantitaSuImpianto';
import {Lavorazione} from 'app/Model/attivita/Lavorazione';
import {NoteIntervento} from 'app/Model/attivita/note_intervento/NoteIntervento';
import {NoteInterventoGruppi} from 'app/Model/attivita/note_intervento/NoteInterventoGruppi';
import {RilevamentoDiMagazzino} from 'app/Model/attivita/RilevamentoDiMagazzino';
import {Prodotto} from 'app/Model/attivita/risorse/Prodotto';
import {DoseAcqua, RisorsaAcqua} from 'app/Model/attivita/risorse/RisorsaAcqua';
import {RisorsaMacchina} from 'app/Model/attivita/risorse/RisorsaMacchina';
import {RisorsaPersona} from 'app/Model/attivita/risorse/RisorsaPersona';
import {RisorsaProdotto} from 'app/Model/attivita/risorse/RisorsaProdotto';
import {
  AGRODATAINIZIO,
  FERTILIZZANTI,
  FORMULATI,
  INNESCHI,
  INSETTI,
  LAVCOD_ABBATTIMENTOIMPIANTI,
  NessunDpiNessunaEtichetta,
  SEMENTI,
  TRASFORMATI_VEGETALI
} from 'app/Model/CostantiPersonalizzate';
import {AvversitaGruppo} from 'app/Model/metaschema/avversita/AvversitaGruppo';
import {BufferZone} from 'app/Model/metaschema/BufferZone';
import {Disciplinare} from 'app/Model/metaschema/Disciplinari';
import {DoseEtichetta} from 'app/Model/metaschema/DoseEtichetta';
import {Effluente} from 'app/Model/metaschema/Effluente';
import {PrincipioAttivo} from 'app/Model/metaschema/PrincipioAttivo';
import {Soglia} from 'app/Model/metaschema/Soglia';
import {TipoFertilizzante} from 'app/Model/metaschema/TipoFertilizzante';
import {TipologiaFertilizzante} from 'app/Model/metaschema/TipologiaFertilizzante';
import {UnitaDiMisura} from 'app/Model/metaschema/UnitaDiMisura';
import {DestinazioneUso} from 'app/Model/metaschema/utilizzi/DestinazioneUso';
import {Specie} from 'app/Model/metaschema/utilizzi/Specie';
import {Varieta} from 'app/Model/metaschema/utilizzi/Varieta';
import {
  enum_LAVCOD,
  enum_SEMINA_TIPO,
  enum_statiWorkflowQdC,
  enum_UnitaMisura,
  SIMBOLO_M3_HA,
  SIMBOLO_MM
} from 'app/Model/TipiEnumerativi';
import {FabbricatiService} from 'app/Service/Anagrafica/fabbricati.service';
import {cloneDeep} from 'lodash';
import {NotaInterventoDdlItem} from '../../componenti/grid-note/note.model';
import {
  enum_Ripartizione_Raccolta,
  normalizeOpzioniRaccolta,
  OpzioniRaccolta
} from '../../componenti/prodotti/sezioni/raccolta/opzioni-raccolta/opzioni-raccolta.model';
import {
  Acqua,
  Codici_Attivita_x_CentriAziendali,
  CodiciXOperazione,
  DataCarenzaRaccolta_x_Impianto,
  Dettaglio_Formulato,
  GridImpiantoSelezionatoModel,
  GridMacchinaModel,
  GridOperatoreModel,
  Key_Parametri_Aggiuntivi,
  Key_Parametri_Aggiuntivi_Attivita,
  MultiColumnComboboxDose_Etichetta,
  MultiColumnComboboxSemina,
  Parametri_Aggiuntivi_Attivita,
  Sezione,
  Sezione_Prodotto_Fertilizzanti,
  Sezione_Prodotto_Formulati,
  Sezione_Prodotto_Raccolta,
  Sezione_Prodotto_Sementi,
  Sezione_Rilievi,
  SupTrattata_x_DettaglioSemina
} from '../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import {GridDosiProdottiService} from '../grid-dosi-prodotti/grid-dosi-prodotti.service';
import {QdCService} from '../qdc.service';
import {Documento} from "../../../../Model/documenti/Documento";
import {DatePipe} from "@angular/common";
import {TestataRicetta} from "../../../../Model/attivita/TestataRicetta";
import {DettaglioRilievo} from '../../../../Model/attivita/dettagli/DettaglioRilievo';
import {Esercizio} from '../../../../Model/anagrafiche/Esercizio';
import {Enum_DBTypeOperation, enum_TipoControllo, Tipo_Attivita} from 'gias-ui-kit';
import {RisorsaUmanaVisita} from 'app/Model/anagrafiche/RisorsaUmanaVisita';
import {RisorsaZootecnica} from 'app/Model/attivita/risorse/RisorsaZootecnica';
import {RisorsaSpecie} from 'app/Model/attivita/risorse/RisorsaSpecie';
import {RisorsaDestinazioneUso} from 'app/Model/attivita/risorse/RisorsaDestinazioneUso';
import {RibaltamentoTypes} from "../../../../menu-agenda/components/utils";
import {RisorsaRegistrazione} from "../../../../Model/attivita/risorse/RisorsaRegistrazione";
import {ProdottoDaTrattareCDC} from 'app/Model/attivita/centri_di_costo/ProdottoDaTrattareCDC';
import {Tipo} from 'app/Model/attivita/centri_di_costo/CentroDiCosto';
import {RisorsaCausale} from 'app/Model/attivita/risorse/RisorsaCausale';
import {Blocco, Tipo_Blocco} from "../../../../Model/attivita/Blocco";

@Injectable()
export class QdCFormToAttivitaService{


    constructor(private qdcservice: QdCService,
                private gridDosiProdottiService: GridDosiProdottiService,
                private fabbricatiService: FabbricatiService,
                private datePipe: DatePipe,
                @Inject(LOCALE_ID) private locale_id: string){
    }


    mapAttivita(o: Lavorazione, Sezione_ProdottoDaMappare: FormGroup, listParametri_Aggiuntivi_Attivita: Parametri_Aggiuntivi_Attivita[], fromDdl: boolean = false, array_dosi_for_fromDdl: MultiColumnComboboxDose_Etichetta[] = [], flagVisita: boolean) {
        const attivita = new Attivita();

        //Così sto passando i codici della prima attivita che trovo anche se in realta sono in un multicentro
        let ca:CodiciXOperazione = (this.qdcservice.TestataForm.get("Codici_Attivita").value as Array<CodiciXOperazione>).find(c=>c.Operazione.primaryKey.codice === o.primaryKey.codice);

        attivita.tipo=this.qdcservice.TestataForm.get("Tipo").value;
        attivita.tipoRicetta=this.qdcservice.TestataForm.get("TipoRicetta").value;
        attivita.stato=this.qdcservice.TestataForm.get("Stato").value;
        attivita.latitude=this.qdcservice.TestataForm.get("Latitude").value;
        attivita.longitude=this.qdcservice.TestataForm.get("Longitude").value;

        if(flagVisita)
          //passiamo oraInizio e oraFine Visita
          attivita.oraInizio = this.qdcservice.TestataVisitaForm.get("Ora_Inizio_Visita").value;
        else
          attivita.oraInizio = this.qdcservice.TestataForm.get("Ora").value;

        if (this.qdcservice.TestataVisitaForm.get("impOrarioFine_Visita").value === 1) {
            attivita.oraFine = this.qdcservice.TestataVisitaForm.get("Ora_Fine_Visita").value;
        } else {
            attivita.oraFine = new Date(attivita.oraInizio);
            attivita.oraFine.setHours(this.qdcservice.TestataVisitaForm.get("Ora_Inizio_Visita").value.getHours() + (+this.qdcservice.TestataVisitaForm.get("NrOreTotali_Visita").value));
            attivita.oraFine.setMinutes(this.qdcservice.TestataVisitaForm.get("Ora_Inizio_Visita").value.getMinutes() + (+this.qdcservice.TestataVisitaForm.get("NrOreTotali_Visita").value*60)%60);
        }

        if (flagVisita)
            attivita.daRemoto = this.qdcservice.TestataVisitaForm.get("Da_Remoto_Visita").value;
        else
            attivita.daRemoto = false;

        if (flagVisita)
            attivita.statoWorkflow = this.qdcservice.TestataVisitaForm.get("statoWorkflow_Visita").value ? enum_statiWorkflowQdC.Eseguito : enum_statiWorkflowQdC.Da_Eseguire;
        else
            attivita.statoWorkflow = enum_statiWorkflowQdC.Non_Definito;

        switch(attivita.tipo){
            case Tipo_Attivita.QuadernoDiCampagna:
                attivita.codice=ca.CodiceAttivita;
                break;
            case Tipo_Attivita.Ricetta:
                attivita.codice=ca.CodiceOperazioneRicetta;
                break;
        }

        attivita.associazionePK=ca.Associazione_PK;

        attivita.inviaRicetta = this.qdcservice.TestataForm.get("InviaRicetta").value;

        attivita.origine = this.qdcservice.TestataForm.get("Origine").getRawValue();

        attivita.testataRicetta=this.setTestataRicetta(ca);

        attivita.descrizione=this.getDescrizioneAttivita(ca);

        attivita.raccoglitore=this.qdcservice.TestataForm.get("Raccoglitore").value;

        attivita.inizio=this.qdcservice.TestataForm.get("Data").value;

        if (flagVisita) {                        // se siamo in Visita salvo il job Visita
            attivita.job = new Lavorazione(enum_LAVCOD.VISITA.toString());
            attivita.job.descrizione = "Visita";
            attivita.attivitaPersonalizzata=this.qdcservice.TestataForm.get("Attivita_Personalizzata").value as AttivitaPersonalizzata;
        } else {
            attivita.job=<Lavorazione>ca.Operazione;
            attivita.attivitaPersonalizzata=this.qdcservice.TestataForm.get("Attivita_Personalizzata").value as AttivitaPersonalizzata;
        }

        attivita.attivitaCollegate = this.getAttivitaCollegate(o);

        attivita.centroAziendale=<CentroAziendale> this.qdcservice.TestataForm.get("Centro_Aziendale").value;
        attivita.disciplinare=this.getDisciplinareAttivita(ca);
        attivita.blocco = this.getBlocco();
        attivita.centriDiCosto=[];
        attivita.risorse=[];


        if (this.qdcservice.getCentroDiCostoTipo() == Tipo.ProdottoDaTrattare) {
            let prodottiDaTrattareSelezionati = this.qdcservice.ProdottiDaTrattareSelezionatiFormArray.value;
            if (prodottiDaTrattareSelezionati?.length > 0) {
                prodottiDaTrattareSelezionati.forEach((prodottoDaTrattareCDC: ProdottoDaTrattareCDC) => {
                    attivita.centriDiCosto.push(prodottoDaTrattareCDC);
                });
            }
        } else {
            let EserciziCDCSelezionatiModel = this.qdcservice.GetEserciziCDCSelezionatiModel();
            if(EserciziCDCSelezionatiModel && EserciziCDCSelezionatiModel.length > 0){

                EserciziCDCSelezionatiModel.forEach((esercizioCDC:EsercizioCDC)=>{
                    attivita.centriDiCosto.push(esercizioCDC);
                });
            }

        }

        this.mapRisorseMacchina(attivita,flagVisita);

        let GridOperatori: Array<GridOperatoreModel> = this.qdcservice.OperatoriFormArray.getRawValue() as Array<GridOperatoreModel>;

        //TODO sistemare le descrizioni mancanti

        if(GridOperatori && GridOperatori.length > 0 && !flagVisita){

            GridOperatori.forEach((o: GridOperatoreModel)=>{
                let newRisorsaPersona = new RisorsaPersona();

                let newRisorseUmane = new RisorseUmane();

                let newContatto = new Contatto();

                let newDocumento = new Documento();

                newDocumento.Data_Scadenza = o.Data_Scadenza_Patentino;

                let newRapportoContabile = new RapportoContabile(o.Cod_Rapporto);

                newRapportoContabile.descrizione = o.Rapporto_Des;

                newRisorseUmane.codice = o.Cod_RisUm;

                newContatto.primaryKey = new PK(o.Piva, o.Cod_Contatto);

                newContatto.nome = o.Nome;

                newContatto.cognome = o.Cognome;

                newContatto.documenti = [newDocumento];

                newRisorseUmane.contatto = newContatto;

                newRisorseUmane.rapportoContabile = newRapportoContabile;

                newRisorsaPersona.risorsaUmana = newRisorseUmane;

                attivita.risorse.push(newRisorsaPersona);
            });
        }

        if (!flagVisita) {
            attivita.utilizzoTerreno = this.qdcservice.getUtilizzoTerrenoModel();
            let utilizzoTerrenoClassType=this.qdcservice.TestataForm.get("Specie").value?.classType;

            if (utilizzoTerrenoClassType === "DestinazioneUso") {
                attivita.utilizzoTerreno=new DestinazioneUso();
                attivita.utilizzoTerreno.codice=this.qdcservice.TestataForm.get("Specie").value.codice;
                attivita.utilizzoTerreno.descrizione=this.qdcservice.TestataForm.get("Specie").value.descrizione;
            } else {
                attivita.utilizzoTerreno=new Varieta();
                (<Varieta>attivita.utilizzoTerreno).specie=new Specie(this.qdcservice.TestataForm.get("Specie").value.codice);
                (<Varieta>attivita.utilizzoTerreno).specie.descrizione=this.qdcservice.TestataForm.get("Specie").value.descrizione;
            }
        }

        let risorsaAcqua = this.getRisorsaAcqua(ca.Operazione);

        if(risorsaAcqua && !flagVisita)
            attivita.risorse.push(risorsaAcqua);

        this.setNoteIntervento(attivita, flagVisita)

        if (!flagVisita) {
            if(!Sezione_ProdottoDaMappare){
                const Sezioni_ProdottoFormArray = this.qdcservice.Sezioni_ProdottoFormArray;

                if(Sezioni_ProdottoFormArray){

                    let Sezione_ProdottoFormArrayValue: Sezione_Prodotto_Formulati | Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta = Sezioni_ProdottoFormArray.getRawValue().find(
                        (s: Sezione_Prodotto_Formulati | Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta)=>{
                            return s.Operazione.primaryKey.codice === ca.Operazione.primaryKey.codice;
                        }
                    );

                    if(Sezione_ProdottoFormArrayValue)
                        this.mapSezione_Prodotto(Sezione_ProdottoFormArrayValue,attivita,listParametri_Aggiuntivi_Attivita, fromDdl,array_dosi_for_fromDdl);
                }
            }else{
                this.mapSezione_Prodotto(Sezione_ProdottoDaMappare.getRawValue(),attivita,listParametri_Aggiuntivi_Attivita, fromDdl,array_dosi_for_fromDdl);
            }

            if (+o.primaryKey.codice === enum_LAVCOD.FERTIRRIGAZIONE || +o.primaryKey.codice === enum_LAVCOD.IRRIGAZIONE) {
                let dettagliIrrigazione: DettaglioIrrigazione[] = this.mapIrrigazione(o, attivita, listParametri_Aggiuntivi_Attivita);
                if (dettagliIrrigazione && dettagliIrrigazione.length > 0) {
                    dettagliIrrigazione.forEach(dettaglio => {
                        attivita.risorse.push(dettaglio);
                    });
                }
            }

            // Mappo le sezioni senza prodotto
            const Sezioni_Senza_ProdottoFormArray = this.qdcservice.Sezioni_Senza_ProdottoFormArray;
            if(Sezioni_Senza_ProdottoFormArray){

                let Sezione_Senza_ProdottoFormArrayValue: Sezione_Rilievi = Sezioni_Senza_ProdottoFormArray.getRawValue()
                    .find((s: Sezione_Rilievi) =>
                        s.Operazione.primaryKey.codice === ca.Operazione.primaryKey.codice
                    );

                if(Sezione_Senza_ProdottoFormArrayValue)
                    this.mapSezione_Senza_Prodotto(
                        Sezione_Senza_ProdottoFormArrayValue, attivita,
                        listParametri_Aggiuntivi_Attivita
                    );
            }
        }

        //gestione delle causali
        let causaliRilievo = this.qdcservice.QdCForm.get('Trattamento').get('Causali').getRawValue();

        if (causaliRilievo && causaliRilievo.length > 0) {
            causaliRilievo.forEach(item => {
                let risorsaCausale = new RisorsaCausale();
                risorsaCausale.id = item.Id;
                risorsaCausale.causale = item.Causale;
                attivita.risorse.push(risorsaCausale);
            });
        }

        return attivita;
    }
    mapIrrigazione(lavorazione: Lavorazione, attivita: Attivita, listParametri_Aggiuntivi_Attivita: Parametri_Aggiuntivi_Attivita[]): DettaglioIrrigazione[] {

        let dettagliIrrigazione: DettaglioIrrigazione[] = [];

        // Get selected plants/impianti data
        const impiantiSelezionati: Array<GridImpiantoSelezionatoModel> = this.qdcservice.ImpiantiSelezionatiFormArray?.getRawValue() || [];

        impiantiSelezionati.forEach(impianto => {
            let dettaglioIrrigazione = {} as DettaglioIrrigazione;

            if (impianto.UdmDose === SIMBOLO_M3_HA){
                dettaglioIrrigazione.unitaDiMisura = new UnitaDiMisura(enum_UnitaMisura.METRI3__HA, SIMBOLO_M3_HA, SIMBOLO_M3_HA);
            } else if (impianto.UdmDose === SIMBOLO_MM){
                dettaglioIrrigazione.unitaDiMisura = new UnitaDiMisura(enum_UnitaMisura.Millimetri, SIMBOLO_MM, SIMBOLO_MM);
            }

            // Hours and flow rate
            dettaglioIrrigazione.Ore = impianto.OreIrrigazione;
            dettaglioIrrigazione.Portata = impianto.Portata;
            dettaglioIrrigazione.Efficienza = impianto.Efficienza;

            // Date range
            dettaglioIrrigazione.DataInizio = impianto.DataInizioIrrigazione;
            dettaglioIrrigazione.DataFine = impianto.DataFineIrrigazione;

            // Frequency
            dettaglioIrrigazione.Frequenza = impianto.FrequenzaIrrigazioneMedia;

            if (impianto.UdmDose === SIMBOLO_M3_HA){
                dettaglioIrrigazione.QtaTotale = impianto.DoseAcquaGiornaliera * impianto.Sup_Imp_help; //m3; qta totale acqua giornaliera
            } else if (impianto.UdmDose === SIMBOLO_MM){
                dettaglioIrrigazione.QtaTotale = impianto.DoseAcquaGiornaliera * impianto.Sup_Imp_help * 10; //m3; qta totale acqua giornaliera; moltiplico per 10 per convertire da mm a m3/Ha
            }

            dettaglioIrrigazione.QtaRilevata = impianto.DoseAcquaGiornaliera;

            dettaglioIrrigazione.tipoIrrigazione = null;

            dettaglioIrrigazione.macchina = null;

            dettaglioIrrigazione.consiglioIrrigazione = new ConsiglioIrrigazione(0);

            dettaglioIrrigazione.consiglioIrrigazione.dataConsiglio = impianto.DataConsiglio;

            dettaglioIrrigazione.consiglioIrrigazione.qtaAcqua = impianto.DoseAcquaConsiglio;

            // Irrigation advice
            if (impianto.Consiglio_Codice) {

                if(typeof impianto.Consiglio_Codice === 'object' && impianto.Consiglio_Codice.id !== 0){

                dettaglioIrrigazione.consiglioIrrigazione.codice = impianto.Consiglio_Codice.id;

                }else if(typeof impianto.Consiglio_Codice === 'number' && impianto.Consiglio_Codice !== 0){

                dettaglioIrrigazione.consiglioIrrigazione.codice = impianto.Consiglio_Codice;

                }

                dettaglioIrrigazione.consiglioIrrigazione.descrizione = impianto.Consiglio_Descrizione;
                dettaglioIrrigazione.consiglioIrrigazione.unitaDiMisura = impianto.UdmConsiglio;
                dettaglioIrrigazione.consiglioIrrigazione.minDataTurno = impianto.minDataTurno;
                dettaglioIrrigazione.consiglioIrrigazione.maxDataTurno = impianto.maxDataTurno;

            }

            //Irrigation Type
            if(impianto.IrrigazioneUtilizzata_Codice){
                if(impianto.macchinaIrrigazione && impianto.macchinaIrrigazione.codice > 0){
                    dettaglioIrrigazione.macchina = impianto.macchinaIrrigazione;
                }

                if(impianto.tipoIrrigazione && impianto.tipoIrrigazione.codice > 0){
                    dettaglioIrrigazione.tipoIrrigazione = impianto.tipoIrrigazione;
                }
            }

            // Set up exercise/cost center data from current selected plant
            dettaglioIrrigazione.esercizioCDC = this.qdcservice.GetEsercizioCDC(impianto);

            // Set resource base properties
            dettaglioIrrigazione.classType = "DettaglioIrrigazione";

            // Add to array
            dettagliIrrigazione.push(dettaglioIrrigazione);
        });

        listParametri_Aggiuntivi_Attivita.push({
            operazione: lavorazione,
            key: 'Verifica_Compatibilita_Microirrigazione',
            value: this.qdcservice.verificaCompatibilitaMicroirrigazione.toString()
        });

        return dettagliIrrigazione;
    }

    /**
     * @description
     * Converte il FormGroup in una Attivita.
     * Se Sezione_ProdottoDaMappare è valorizzato allora creo l'attività solo della Sezione che mi serve
     * ATTENZIONE: Se sono in modifica di un multicentro per un lav_cod posso avere più id_agenda ma in questo caso nel map
     * prenderò solo il primo dei due.
     */
    mapQdCFormToListaAttivita(Sezione_ProdottoDaMappare: FormGroup, listParametri_Aggiuntivi_Attivita:Parametri_Aggiuntivi_Attivita[], fromDdl: boolean = false,array_dosi_for_fromDdl: MultiColumnComboboxDose_Etichetta[] = [],list_lav_cod_da_non_mappare: number[]=[]): Attivita[] {

        let listaAttivita: Attivita[] = new Array();

        let Operazioni = this.qdcservice.TestataForm.get("Operazioni").getRawValue() as Array<Lavorazione>;

        let flagVisita = this.qdcservice.TestataForm.get("flagVisita").value;

        //Se devo mappare solamente una sezione allora prendo il suo codice attivita
        if(Sezione_ProdottoDaMappare){

            let Operazione_Sezione:Lavorazione = Sezione_ProdottoDaMappare.get("Operazione").value;

            Operazioni = Operazioni.filter(o=>Operazione_Sezione.primaryKey.codice === o.primaryKey.codice);

        }

        if(Operazioni && Operazioni.length > 0){

            //Escludo i lav_cod da non mappare
            if(list_lav_cod_da_non_mappare && list_lav_cod_da_non_mappare.length > 0){
              Operazioni = Operazioni.filter(o=>!list_lav_cod_da_non_mappare.includes(+ o.primaryKey.codice));
            }

            Operazioni.forEach((o:Lavorazione)=>{
                let mainAttivita = this.mapAttivita(o, Sezione_ProdottoDaMappare, listParametri_Aggiuntivi_Attivita, fromDdl, array_dosi_for_fromDdl, flagVisita)

                if(flagVisita) {                                        //devo aggiungere la risorsa con operatore, specie e riossazootecnica

                    let risorsaUmanaVisita = new RisorsaUmanaVisita();
                    let operatoreVisita = this.qdcservice.TestataVisitaForm.get("Operatore_Visita").value;
                    risorsaUmanaVisita.risorsaUmana.codice = operatoreVisita.Cod_RisUm;
                    mainAttivita.risorse.push(risorsaUmanaVisita);

                    // let dettVisita = new DettaglioVisita();
                    // dettVisita.AttivitaCollegate.push(this.mapAttivita(o, Sezione_ProdottoDaMappare, listParametri_Aggiuntivi_Attivita, fromDdl, array_dosi_for_fromDdl, !flagVisita));
                    // mainAttivita.risorse.push(dettVisita);

                    let risorsaSpecie;
                    let utilizzoTerrenoClassType=this.qdcservice.TestataForm.get("Specie").value?.classType;

                    if (utilizzoTerrenoClassType === "DestinazioneUso") {
                        risorsaSpecie = new RisorsaDestinazioneUso();
                        risorsaSpecie.destinazioneUso.codice=this.qdcservice.TestataForm.get("Specie").value.codice;
                        risorsaSpecie.destinazioneUso.descrizione=this.qdcservice.TestataForm.get("Specie").value.descrizione;
                    } else {
                        risorsaSpecie = new RisorsaSpecie();
                        risorsaSpecie.specie = new Specie(this.qdcservice.TestataForm.get("Specie").value.codice);
                        risorsaSpecie.specie.descrizione=this.qdcservice.TestataForm.get("Specie").value.descrizione;
                    }

                    mainAttivita.risorse.push(risorsaSpecie);

                    if (this.qdcservice.TestataVisitaForm.get("SpecieAnimali_Visita").value) {
                        let risorsaZootecnica = new RisorsaZootecnica();
                        risorsaZootecnica = this.qdcservice.TestataVisitaForm.get("SpecieAnimali_Visita").value;
                        mainAttivita.risorse.push(risorsaZootecnica);
                    }
                }

                listaAttivita.push(mainAttivita);
            });

        }

        //Una lista di warning in comune con tutte le operazioni
        this.set_lista_MostraWarning(listParametri_Aggiuntivi_Attivita);

        //Aggiungi i parametri Aggiuntivi Comuni a tutte le Operazioni
        this.setParametriAggiuntivi_Attivita_Generali(listParametri_Aggiuntivi_Attivita);

        return listaAttivita;
    }


    private getDisciplinareAttivita(ca: CodiciXOperazione): Disciplinare{

        let disciplinare = null;

        if(this.qdcservice.Operazioni_Con_Disciplinare(ca.Operazione)){

            disciplinare = this.qdcservice.getDisciplinareModelValue(+ ca.Operazione.primaryKey.codice);

            if(this.qdcservice.Elenco_Operazioni_Fertilizzanti.includes(+ ca.Operazione.primaryKey.codice) && disciplinare){
                if(disciplinare.codice === NessunDpiNessunaEtichetta){
                    disciplinare = this.qdcservice.Obj_NessunDpi;
                }
            }

        }else{
            //Per la nuova operazione CONFUSIONE_DISORIENTAMENTO_SESSUALE e per TRATTAMENTO_POST_RACCOLTA anche se non è visibile il disciplinare lo imposto
            //comunque a Solo Etichetta perchè lato server verrà gestito in questo modo
            if(this.qdcservice.Elenco_Operazioni_Con_Default_DPI.findIndex(x=>x=== + ca.Operazione.primaryKey.codice) > -1){

                disciplinare = this.qdcservice.Obj_NessunDpi;
            }
        }

        return disciplinare;

    }

    private setNoteIntervento(attivita: Attivita, flagVisita: boolean) {
      let Nota_Testuale = this.qdcservice.Nota_TestualeFormControl.getRawValue();
      if(Nota_Testuale && !flagVisita) {
          attivita.note = Nota_Testuale;
      } else {
          attivita.note = "";
      }

      attivita.noteIntervento = [];
      let GridNote =  this.qdcservice.NoteFormArray.getRawValue() as Array<NotaInterventoDdlItem>;
      if(GridNote && GridNote.length > 0 && !flagVisita) {
          GridNote.forEach((n: NotaInterventoDdlItem)=>{
              let newNoteIntervento = new NoteIntervento(n.id);
              let newnoteInterventoGruppi = new NoteInterventoGruppi(n.NotaGruppo_Cod);
              newNoteIntervento.descrizione = n.descrizione;
              newNoteIntervento.noteInterventoGruppi = newnoteInterventoGruppi;
              attivita.noteIntervento.push(newNoteIntervento);
          });
      }
      if (!this.qdcservice.isNoteInitialized && attivita.noteIntervento.findIndex(n=>n.codice === 0) === -1) {
          attivita.noteIntervento.push(new NoteIntervento(0));
      }
    }

    getRisorsaAcqua(Operazione: Lavorazione):RisorsaAcqua{

        let risorsaAcqua=null;

        let lav_cod = + Operazione.primaryKey.codice;

        if(this.qdcservice.AcquaForm && this.qdcservice.Elenco_Operazioni_con_Acqua.includes(lav_cod)){

            let acqua=<Acqua>this.qdcservice.AcquaForm.getRawValue();
            risorsaAcqua=new RisorsaAcqua();
            risorsaAcqua.doseAcqua=acqua.Dose_Acqua;
            switch (acqua.Dose_Acqua) {
                case DoseAcqua.HA:
                    risorsaAcqua.acqua=acqua.Acqua_Ha;
                    break;
                case DoseAcqua.TOTALE:
                    risorsaAcqua.acqua=acqua.Acqua_Tot;
                    break;
            }
        }

        return risorsaAcqua;

    }

    private mapSezione_Prodotto(
        s,
        attivita,
        listParametri_Aggiuntivi_Attivita: Parametri_Aggiuntivi_Attivita[],
        fromDdl: boolean,
        array_dosi_for_fromDdl: MultiColumnComboboxDose_Etichetta[]
    ): void {

        //L'UdM nel caso fromDdl potrebbe ancora non essere stata scelta dall'utente
        let UdM: UnitaDiMisura = null;

        if (fromDdl) {
            if(!s.UdM) {
                UdM = new UnitaDiMisura(0,"","");
            } else {
                UdM = new UnitaDiMisura(s.UdM.codice,s.UdM.descrizione,s.UdM.simbolo);
            }
        }

        switch(s.Categoria_Magazzino) {
            case INSETTI:
            case FORMULATI:
                if (fromDdl) {
                    this.getDettaglioTrattamentoFromDdl(s as Sezione_Prodotto_Formulati, attivita,UdM,array_dosi_for_fromDdl)
                } else {
                    this.getDettagliTrattamento(s as Sezione_Prodotto_Formulati, attivita);
                }
                break;
            case FERTILIZZANTI:
                if (fromDdl) {
                    this.getDettagliFertilizzazioneFromDdl(s as Sezione_Prodotto_Fertilizzanti, attivita,UdM)
                } else {
                    this.getDettagliFertilizzazione(s as Sezione_Prodotto_Fertilizzanti, attivita);
                }
                break;
            case SEMENTI:
                if (fromDdl) {
                    this.getDettagliSeminaFromDdl(s as Sezione_Prodotto_Sementi, attivita,UdM)
                } else {
                    this.getDettagliSemina(s as Sezione_Prodotto_Sementi, attivita);
                }
                break;
            case TRASFORMATI_VEGETALI:
                if (s.Operazione.primaryKey.codice === enum_LAVCOD.RACCOLTA.toString()){
                    this.getDettagliRaccolta(s as Sezione_Prodotto_Raccolta, attivita)
                }
                break;
        }

        this.setParametriAggiuntivi_Attivita_x_Sezioni(s, listParametri_Aggiuntivi_Attivita);

    }

  /** Mappa le sezioni senza prodotto.
   * @private
   * @history
   * (11/04/2023): Creazione funzione -- mappa sezioni rilievi
   *
   * (20/08/2024): Aggiunge gestione parametro aggiornamento anagrafica per abbattimento impianti
   */
    private mapSezione_Senza_Prodotto(
        s: Sezione_Rilievi | Sezione, attivita: Attivita,
        listParametri_Aggiuntivi_Attivita: Parametri_Aggiuntivi_Attivita[] ,
    ){
        if (this.isAbbattimento(s.Operazione)) {
            listParametri_Aggiuntivi_Attivita.push({
              operazione: s.Operazione,
              key: "Opzione_Abbattimento_Aggiornamento_Anagrafica",
              value: this.qdcservice.Opzione_Raccolta.toString()
            });
        }
        if (s['DettagliRilievi']?.length) {
            this.getDettagliRilievi(s as Sezione_Rilievi, attivita)
        }
    }

    //Compone il des_lib dell'operazione
    private getDescrizioneAttivita(co: CodiciXOperazione): string{

        let descrizione: string = "";
        let Veg_Des = this.qdcservice.TestataForm.get("Specie").getRawValue().descrizione;
        let StrVarieta: string = "";
        let Lav_Des: string = co.Operazione.descrizione;
        let gridImpiantiSelezionati: Array<GridImpiantoSelezionatoModel>=this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue();

        if(gridImpiantiSelezionati){

            let ListVarieta = gridImpiantiSelezionati.map(item => item.Cul_Des).filter((value, index, self) => self.indexOf(value) === index);

            if(ListVarieta){

                let StrSpecie:string = "("+ Veg_Des+ "  ";

                StrVarieta = "[" +ListVarieta.join(", ")+ "])";

                let StrSpecieVarieta:string = StrSpecie + StrVarieta;

                if(+ co.Operazione.primaryKey.codice === enum_LAVCOD.ALTRE_OPERAZIONI || (this.qdcservice.TestataForm.get("flagVisita").value && + co.Operazione.primaryKey.codice === enum_LAVCOD.VISITA)){

                    //Per ora nel nuovo QdC il des_lib per le Altre Operazioni è solo la descrzione della textbox niente concatenazione con gli impianti
                    let Descrizione_Altre_Lavorazioni: string = this.qdcservice.TestataForm.get("Descrizione_Altre_Lavorazioni").getRawValue();

                    descrizione = Descrizione_Altre_Lavorazioni;

                    //Nella Trattamenti_2 se siamo in altre operazioni concateno la decsrzione della textbox Descrizione_Altre_Lavorazioni con la
                    //specie e varietà degli impianti selezionati
                    /* if(Descrizione_Altre_Lavorazioni && Descrizione_Altre_Lavorazioni !== ""){

                        if(Descrizione_Altre_Lavorazioni.includes(StrSpecieVarieta)){
                            descrizione = Descrizione_Altre_Lavorazioni;
                        }else{

                            if((co.CodiceAttivita !== "" && co.CodiceAttivita !== "0") ||
                                (co.CodiceOperazioneRicetta !== "" && co.CodiceOperazioneRicetta !== "0") ||
                                (co.CodiceRicetta !== "" && co.CodiceRicetta !== "0")){

                                    let attivita: Attivita = this.attivitaService.getListaAttivita().find(a=>{
                                        a.job.primaryKey.codice === co.Operazione.primaryKey.codice &&
                                        a.codice === co.CodiceAttivita ||
                                        a.codiceOperazioneRicetta === co.CodiceOperazioneRicetta ||
                                        a.codiceRicetta === co.CodiceRicetta
                                    });

                                    if(attivita){

                                    }

                            }
                            descrizione = Descrizione_Altre_Lavorazioni + " "+ StrSpecieVarieta;
                        }
                    } */
                } else {
                    descrizione = Lav_Des + " "+ StrSpecieVarieta;
                }

            }

        }

        return descrizione;

    }

    //Restituisce i Dettagli Trattamento all'attivita
    private getDettagliTrattamento(Sezione_Formulato: Sezione_Prodotto_Formulati, attivita: Attivita){

        if(Sezione_Formulato) {

            attivita.epoca = Sezione_Formulato.EpocaDPI;

            attivita.modalitaApplicazione = Sezione_Formulato.Modalita_Applicazione;

            const DosiFormulati: Array<any> = Sezione_Formulato.DosiProdotti;

            if(DosiFormulati) {
                DosiFormulati.forEach((f: Dettaglio_Formulato) => {

                    let newDettaglioTrattamento = new DettaglioTrattamento();

                    if(f.Prodotto && f.Operazione) {
                        let index: number = attivita.risorse.findIndex((r:any) =>
                            r.classType === "DettaglioTrattamento" &&
                            attivita.job.primaryKey.codice === f.Operazione.primaryKey.codice &&
                            r.prodotto.codice === f.Prodotto.prodotto
                        );

                        if(index === -1) {

                            let newRisorsaProdotto: RisorsaProdotto = this.getRisorsaProdotto(f);

                            newDettaglioTrattamento.avversitaGruppo = this.getAvversita(f);
                            newDettaglioTrattamento.principiAttivi = <PrincipioAttivo[]> f.Prodotto.principiAttivi;
                            newDettaglioTrattamento.tempoCarenza = f.Carenza;
                            newDettaglioTrattamento.bufferzone = <BufferZone> f.BufferZone;
                            newDettaglioTrattamento.dettaglioProdotto = f.Prodotto.dettaglioProdotto;
                            newDettaglioTrattamento.dosiEtichetta =  <DoseEtichetta[]> f.Dosi_Etichetta;
                            newDettaglioTrattamento.soglia = <Soglia> f.Soglia_Avversita;
                            newDettaglioTrattamento.descrizionePrecedente = f.Prodotto.descrizionePrecedente;
                            newDettaglioTrattamento.dataSmaltimentoScorte = f.Prodotto.dataSmaltimentoScorte;
                            newDettaglioTrattamento.classificazioni = f.Prodotto.classificazioni;
                            newDettaglioTrattamento.inRevisione = f.Prodotto.inRevisione;
                            newDettaglioTrattamento.dataAttoNormativo = f.Prodotto.dataAttoNormativo;
                            newDettaglioTrattamento.formulatiXAllegatiNormative_IDRiga = f.Prodotto.formulatiXAllegatiNormative_IDRiga;
                            newDettaglioTrattamento.tipoFormulato = f.Prodotto.tipoFormulato;
                            newDettaglioTrattamento.epocheBlocchi = f.Prodotto.epocheBlocchi;
                            newDettaglioTrattamento.dettagliDose = f.Prodotto.dettagliDose;
                            newDettaglioTrattamento.protezione = f.Prodotto.protezione;
                            newDettaglioTrattamento.modalitaImpiego = f.Prodotto.modalitaImpiego;
                            newDettaglioTrattamento.prodotto = newRisorsaProdotto.prodotto;
                            newDettaglioTrattamento.MagazziniMovimentazioni = newRisorsaProdotto.MagazziniMovimentazioni;
                            newDettaglioTrattamento.flagDoseQuantitaTotale = newRisorsaProdotto.flagDoseQuantitaTotale;
                            newDettaglioTrattamento.flagTipoDose = newRisorsaProdotto.flagTipoDose;
                            newDettaglioTrattamento.doseHaReale = newRisorsaProdotto.doseHaReale;
                            newDettaglioTrattamento.doseHlReale = newRisorsaProdotto.doseHlReale;
                            newDettaglioTrattamento.quantitaTotaleReale = newRisorsaProdotto.quantitaTotaleReale;
                            newDettaglioTrattamento.unitaDiMisura = newRisorsaProdotto.unitaDiMisura;
                            newDettaglioTrattamento.unitaDiMisuraIndicata = newRisorsaProdotto.unitaDiMisuraIndicata;
                            newDettaglioTrattamento.polverulento =  f.Prodotto.polverulento;
                            newDettaglioTrattamento.isImpollinatore = f.Prodotto.isImpollinatore;
                            newDettaglioTrattamento.durataFeromone = f.Prodotto.durataFeromone;
                            newDettaglioTrattamento.scadenzaFeromone = f.Prodotto.scadenzaFeromone;
                            newDettaglioTrattamento.quantitaSuImpianti = f.QuantitaSuImpianti;
                            newDettaglioTrattamento.ripartizioneTrappole = f.Ripartizione_Trappole ? f.Ripartizione_Trappole.codice : 0;

                            attivita.risorse.push(newDettaglioTrattamento);

                        } else {

                            //In questo caso è stato inserito lo stesso prodotto in due magazzini differenti quindi nel modello dovrò
                            //aggiungere un elemento nell'array  MagazziniMovimentazioni nello stesso prodotto

                            newDettaglioTrattamento = attivita.risorse[index] as DettaglioTrattamento;

                            if(!newDettaglioTrattamento.MagazziniMovimentazioni)
                                newDettaglioTrattamento.MagazziniMovimentazioni = [];

                            let newRilevamentoDiMagazzino: RilevamentoDiMagazzino = this.getRilevamentoDiMagazzino(f);

                            newDettaglioTrattamento.MagazziniMovimentazioni.push(newRilevamentoDiMagazzino);

                        }
                    }

                });
            }

        }

    }

    private getDettaglioTrattamentoFromDdl(
        Sezione_Formulato: Sezione_Prodotto_Formulati,
        attivita: Attivita,
        UdM:UnitaDiMisura,
        array_dosi: MultiColumnComboboxDose_Etichetta[]
    ): void {
        if(Sezione_Formulato) {

            attivita.epoca = Sezione_Formulato.EpocaDPI;

            attivita.modalitaApplicazione = Sezione_Formulato.Modalita_Applicazione;

            let newDettaglioTrattamento = new DettaglioTrattamento();

            let newRisorsaProdotto: RisorsaProdotto = this.getRisorsaProdottoFromDdl(Sezione_Formulato,UdM);

            newDettaglioTrattamento.avversitaGruppo = this.getAvversitafromDDL(Sezione_Formulato);
            newDettaglioTrattamento.principiAttivi = <PrincipioAttivo[]> Sezione_Formulato.Prodotto.principiAttivi;
            newDettaglioTrattamento.tempoCarenza = Sezione_Formulato.Prodotto.tempoCarenza;
            newDettaglioTrattamento.bufferzone = <BufferZone> Sezione_Formulato.Prodotto.bufferzone;
            newDettaglioTrattamento.dettaglioProdotto = Sezione_Formulato.Prodotto.dettaglioProdotto;
            newDettaglioTrattamento.dosiEtichetta =  this.gridDosiProdottiService.getDosi_Etichetta(array_dosi,Sezione_Formulato.Dose_Etichetta);
            newDettaglioTrattamento.soglia = <Soglia> Sezione_Formulato.Soglia_Avversita;
            newDettaglioTrattamento.descrizionePrecedente = Sezione_Formulato.Prodotto.descrizionePrecedente;
            newDettaglioTrattamento.dataSmaltimentoScorte = Sezione_Formulato.Prodotto.dataSmaltimentoScorte;
            newDettaglioTrattamento.classificazioni = Sezione_Formulato.Prodotto.classificazioni;
            newDettaglioTrattamento.inRevisione = Sezione_Formulato.Prodotto.inRevisione;
            newDettaglioTrattamento.dataAttoNormativo = Sezione_Formulato.Prodotto.dataAttoNormativo;
            newDettaglioTrattamento.formulatiXAllegatiNormative_IDRiga = Sezione_Formulato.Prodotto.formulatiXAllegatiNormative_IDRiga;
            newDettaglioTrattamento.tipoFormulato = Sezione_Formulato.Prodotto.tipoFormulato;
            newDettaglioTrattamento.epocheBlocchi = Sezione_Formulato.Prodotto.epocheBlocchi;
            newDettaglioTrattamento.dettagliDose = Sezione_Formulato.Prodotto.dettagliDose;
            newDettaglioTrattamento.protezione = Sezione_Formulato.Prodotto.protezione;
            newDettaglioTrattamento.modalitaImpiego = Sezione_Formulato.Prodotto.modalitaImpiego;
            newDettaglioTrattamento.soglia = Sezione_Formulato.Soglia_Avversita;
            newDettaglioTrattamento.prodotto = newRisorsaProdotto.prodotto;
            newDettaglioTrattamento.MagazziniMovimentazioni = newRisorsaProdotto.MagazziniMovimentazioni;
            newDettaglioTrattamento.flagDoseQuantitaTotale = newRisorsaProdotto.flagDoseQuantitaTotale;
            newDettaglioTrattamento.flagTipoDose = newRisorsaProdotto.flagTipoDose;
            newDettaglioTrattamento.doseHaReale = newRisorsaProdotto.doseHaReale;
            newDettaglioTrattamento.doseHlReale = newRisorsaProdotto.doseHlReale;
            newDettaglioTrattamento.quantitaTotaleReale = newRisorsaProdotto.quantitaTotaleReale;

            newDettaglioTrattamento.unitaDiMisuraIndicata = newRisorsaProdotto.unitaDiMisuraIndicata;
            newDettaglioTrattamento.unitaDiMisura = newRisorsaProdotto.unitaDiMisura;
            newDettaglioTrattamento.polverulento =  Sezione_Formulato.Prodotto.polverulento;

            newDettaglioTrattamento.quantitaSuImpianti = Sezione_Formulato.QuantitaSuImpianti;

            newDettaglioTrattamento.ripartizioneTrappole = Sezione_Formulato.Ripartizione_Trappole ? Sezione_Formulato.Ripartizione_Trappole.codice : 0;

            attivita.risorse.push(newDettaglioTrattamento);
        }
    }

    private getDettagliSemina(Sezione_Sementi: Sezione_Prodotto_Sementi, attivita: Attivita){

        if(Sezione_Sementi){

            const DosiSementi: Array<any> = Sezione_Sementi.DosiProdotti;

            if(DosiSementi){
                DosiSementi.forEach(s=>{

                    let newDettaglioSemina = new DettaglioSemina();

                    if(s.Prodotto && s.Operazione){
                        let index: number = attivita.risorse.findIndex((r:any) =>
                            r.classType === "DettaglioSemina" &&
                            attivita.job.primaryKey.codice === s.Operazione.primaryKey.codice &&
                            r.prodotto.codice === s.Prodotto.prodotto
                        );

                        if(index === -1){

                            let newRisorsaProdotto: RisorsaProdotto = this.getRisorsaProdotto(s);

                            newDettaglioSemina.prodotto = newRisorsaProdotto.prodotto;
                            newDettaglioSemina.MagazziniMovimentazioni = newRisorsaProdotto.MagazziniMovimentazioni;
                            newDettaglioSemina.flagDoseQuantitaTotale = newRisorsaProdotto.flagDoseQuantitaTotale;
                            newDettaglioSemina.flagTipoDose = newRisorsaProdotto.flagTipoDose;
                            newDettaglioSemina.doseHaReale = newRisorsaProdotto.doseHaReale;
                            newDettaglioSemina.doseHlReale = newRisorsaProdotto.doseHlReale;
                            newDettaglioSemina.quantitaTotaleReale = newRisorsaProdotto.quantitaTotaleReale;
                            newDettaglioSemina.unitaDiMisura = newRisorsaProdotto.unitaDiMisura;
                            newDettaglioSemina.unitaDiMisuraIndicata = newRisorsaProdotto.unitaDiMisuraIndicata;
                            newDettaglioSemina.codArticolo = s.Prodotto.codArticolo;
                            newDettaglioSemina.varieta = s.Prodotto.varieta;
                            newDettaglioSemina.regolamento = s.Prodotto.regolamento;

                            attivita.risorse.push(newDettaglioSemina);

                        }else{

                            //In questo caso è stato inserito lo stesso prodotto in due magazzini differenti quindi nel modello dovrò
                            //aggiungere un elemento nell'array  MagazziniMovimentazioni nello stesso prodotto


                            newDettaglioSemina = attivita.risorse[index] as DettaglioSemina;

                            if(!newDettaglioSemina.MagazziniMovimentazioni)
                                newDettaglioSemina.MagazziniMovimentazioni = [];

                            let newRilevamentoDiMagazzino: RilevamentoDiMagazzino = this.getRilevamentoDiMagazzino(s);

                            newDettaglioSemina.MagazziniMovimentazioni.push(newRilevamentoDiMagazzino);

                        }
                    }



                });
            }

        }

    }

    private getDettagliSeminaFromDdl(Sezione_Sementi: Sezione_Prodotto_Sementi, attivita: Attivita,UdM:UnitaDiMisura){

        if(Sezione_Sementi){

            let newDettaglioSemina = new DettaglioSemina();

            let newRisorsaProdotto: RisorsaProdotto = this.getRisorsaProdottoFromDdl(Sezione_Sementi,UdM);

            newDettaglioSemina.prodotto = newRisorsaProdotto.prodotto;
            newDettaglioSemina.MagazziniMovimentazioni = newRisorsaProdotto.MagazziniMovimentazioni;
            newDettaglioSemina.flagDoseQuantitaTotale = newRisorsaProdotto.flagDoseQuantitaTotale;
            newDettaglioSemina.flagTipoDose = newRisorsaProdotto.flagTipoDose;
            newDettaglioSemina.doseHaReale = newRisorsaProdotto.doseHaReale;
            newDettaglioSemina.doseHlReale = newRisorsaProdotto.doseHlReale;
            newDettaglioSemina.quantitaTotaleReale = newRisorsaProdotto.quantitaTotaleReale;
            newDettaglioSemina.unitaDiMisura = newRisorsaProdotto.unitaDiMisura;
            newDettaglioSemina.unitaDiMisuraIndicata = newRisorsaProdotto.unitaDiMisuraIndicata;
            newDettaglioSemina.codArticolo = Sezione_Sementi.Prodotto.codArticolo;
            newDettaglioSemina.varieta = Sezione_Sementi.Prodotto.varieta;
            newDettaglioSemina.regolamento = Sezione_Sementi.Prodotto.regolamento;

            attivita.risorse.push(newDettaglioSemina);

        }

    }

    private getDettagliRaccolta(Sezione_Raccolta: Sezione_Prodotto_Raccolta, attivita: Attivita){
        if (!Sezione_Raccolta) return;

        const ProdottiRaccolti: Array<DettaglioRaccolta> = Sezione_Raccolta.ProdottiRaccolti;

        if (!ProdottiRaccolti) return;

        this.adjustRaccoltaCDCs(attivita, ProdottiRaccolti);

        /** Se è una raccolta con carichi di magazzino, questi
        * dovrebbero essere già stati valorizzati, in alternativa vado
        * a creare un movimento di magazzino fittizio preso in
        * considerazione lato server a seconda del tipo di raccolta.
        */
        const valorizeMovimentazioni = (R: DettaglioRaccolta) => {
            if (!R.MagazziniMovimentazioni)
                R.MagazziniMovimentazioni = []

            if (!Sezione_Raccolta.Opzioni_Raccolta?.CarichiMagazzinoAttivi)
                return R;

            if (!R.MagazziniMovimentazioni.length) {
                R.MagazziniMovimentazioni.push(this.getRilevamentoNoMagazzino(R, attivita));
            }

            R.MagazziniMovimentazioni.forEach(m => {
                m.Qta = m.Qta ? m.Qta : 0;
                m.QtaTot = m.QtaTot ? m.QtaTot : 0;
            });

            /* Rimpiazzo le movimentazioni con il magazzino a null o undefined
             * con movimentazioni a magazzin fittizi.
             */
            let noMag = R.MagazziniMovimentazioni.filter(mov => !mov.Magazzino
                || mov.Magazzino.primaryKey.codice === null
                || mov.Magazzino.primaryKey.centroAziendalePK.codice === null);

            let index = R.MagazziniMovimentazioni.findIndex(m => noMag.includes(m));
            while (index > -1) {
                R.MagazziniMovimentazioni.splice(index, 1);
                R.MagazziniMovimentazioni.push(this.getRilevamentoNoMagazzino(R, attivita));

                index = R.MagazziniMovimentazioni.findIndex(m => noMag.includes(m));
            }
        }

        /** Aggiorna il campo QuantitaSuImpianti di un DettaglioRaccolta,
         * recuperando i valori per MAgazzino e Lotto dalle movimentazioni.
         */
        const updateQsIFromMovimentazioni = (R: DettaglioRaccolta) => {
            if (!R.MagazziniMovimentazioni || !R.MagazziniMovimentazioni.length)
                return R;

            let QsI = R.QuantitaSuImpianti;
            let mov: RilevamentoDiMagazzino = R.MagazziniMovimentazioni.at(0);

            QsI.forEach((q: QuantitaSuImpianto) => {
                q.Magazzino = mov.Magazzino;
            });
        }

        if(!ProdottiRaccolti.length && Sezione_Raccolta.FastHarvestTemplate) {
            let fast = Sezione_Raccolta.FastHarvestTemplate;
            fast.QuantitaSuImpianti = [];
            attivita.centriDiCosto.forEach(cdc => {
                let suImpianto = new QuantitaSuImpianto();
                suImpianto.Qta = 0;
                suImpianto.esercizioCDC = cdc as EsercizioCDC;
                suImpianto.Lotto = '';
                fast.QuantitaSuImpianti.push(suImpianto);
            })
            ProdottiRaccolti.push(fast);
        } else if (!ProdottiRaccolti.length) {
            let fast = new DettaglioRaccolta(Tipo_Raccolta.Fast);
            this.qdcservice.GetEserciziCDCSelezionatiModel().forEach(e => {
                let qsi = new QuantitaSuImpianto();
                qsi.esercizioCDC = e;
                qsi.Qta = 0;
                qsi.Lotto = '';
                qsi.Magazzino = new Fabbricato({
                    codice: 0, centroAziendalePK: {codice: 0, partitaIva: ''}
                })
                fast.QuantitaSuImpianti.push(qsi);
            })
            ProdottiRaccolti.push(fast);
        }
        if (!ProdottiRaccolti[0])
            console.error("Could not load FastHarvestTemplate from 'Sezione_Raccolta'\n"
            + "See what happened at line 750 of quaderno-di-campagna-form-to-attivita.service.ts.");

        let impiantiFromGrid: GridImpiantoSelezionatoModel[] = this.qdcservice.GridImpiantiPublicService.gridComp.data.data;

        for (let r of ProdottiRaccolti) {
            r.quantitaTotaleReale = r.quantitaTotaleReale ?
                    parseFloat(r.quantitaTotaleReale.toString()) : 0;
            r.dataIngresso = r.dataIngresso < AGRODATAINIZIO ?
                    AGRODATAINIZIO : r.dataIngresso;

            r.QuantitaSuImpianti.forEach(q => {
                let impianto = impiantiFromGrid.find(i => i.Progetto_Cod === q.esercizioCDC.esercizio.codice
                        && i.ID_REG === q.esercizioCDC.esercizio.impiantoPK.codice
                        && i.APPEZZA === q.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice
                        && i.SA_COD === q.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                        && i.PIVA=== q.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva);
                if (!impianto) return;

                if (!q.esercizioCDC.esercizio.piante_Impianto) {
                    q.esercizioCDC.esercizio.piante_Impianto = impianto['P_Impianto'];
                }
                if (!q.esercizioCDC.esercizio.descrizione) {
                    q.esercizioCDC.esercizio.descrizione = impianto['Progetto'] || impianto['APP_NOME'];
                }
            })

            let newDettaglioRaccolta: DettaglioRaccolta = cloneDeep(r);

            // Per essere raggruppati nello stesso dettaglio i prodotti raccolti
            // devono avere stesso codice, magazzino e lotto
            let index: number = attivita.risorse.findIndex((res:any) =>
                res.classType === "DettaglioRaccolta" &&
                attivita.job.primaryKey.codice === Sezione_Raccolta.Operazione.primaryKey.codice &&
                this.compareDettaglioRaccolta(res,r)
            );

            if (index === -1) {
                valorizeMovimentazioni(newDettaglioRaccolta);
                updateQsIFromMovimentazioni(newDettaglioRaccolta);

                // TODO(Anny): aggiorna con misura indicata
                // es. um -> kg, umIndicata -> g
                newDettaglioRaccolta.unitaDiMisuraIndicata = newDettaglioRaccolta.unitaDiMisura;

                newDettaglioRaccolta.Opzioni_Raccolta = Sezione_Raccolta.Opzioni_Raccolta;
                normalizeOpzioniRaccolta( newDettaglioRaccolta.Opzioni_Raccolta);

                attivita.risorse.push(newDettaglioRaccolta);
            } else {
                // In questo caso è stato inserito lo stesso prodotto con stesso
                // magazzino e lotto. Devo aggiornare le quantità nelle viarie strutture

                newDettaglioRaccolta = attivita.risorse[index] as DettaglioRaccolta;
                valorizeMovimentazioni(r);
                newDettaglioRaccolta.quantitaTotaleReale += r.quantitaTotaleReale;

                // Aggiorno quantità movimentata (usata lato server per
                // salvataggio in tabella Movimenti_Dettagli)
                if (r.MagazziniMovimentazioni) {
                    for (let magazzino of r.MagazziniMovimentazioni) {
                        let same = newDettaglioRaccolta.MagazziniMovimentazioni.find( mov =>
                            mov.Lotto === magazzino.Lotto && mov.Magazzino.primaryKey.codice === magazzino.Magazzino.primaryKey.codice &&
                            mov.Magazzino.primaryKey.centroAziendalePK.codice === magazzino.Magazzino.primaryKey.centroAziendalePK.codice &&
                            mov.Magazzino.primaryKey.centroAziendalePK.codice === magazzino.Magazzino.primaryKey.centroAziendalePK.codice &&
                            mov.Magazzino.primaryKey.centroAziendalePK.partitaIva === magazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva
                        );
                        if (same) {
                            same.Qta = +magazzino.Qta + +same.Qta;
                            same.QtaTot = +magazzino.QtaTot + +same.QtaTot;
                            continue;
                        }
                        newDettaglioRaccolta.MagazziniMovimentazioni.push(magazzino);
                    }
                }

                r.QuantitaSuImpianti.forEach(qsi => {
                    let Q = newDettaglioRaccolta.QuantitaSuImpianti.find(e =>
                        e.esercizioCDC.esercizio.codice === qsi.esercizioCDC.esercizio.codice
                    );

                    if (!Q) {
                        newDettaglioRaccolta.QuantitaSuImpianti.push(qsi);
                    } else {
                        Q.Qta = +Q.Qta + +qsi.Qta;
                    }
                })
            }
        }
    }

    private getDettagliRilievi(sezioneRilievi: Sezione_Rilievi, attivita: Attivita) {
        if (!sezioneRilievi) return;

        const dettagliRilievi: Array<DettaglioRilievo> = sezioneRilievi.DettagliRilievi;
        if (!dettagliRilievi) return;

        for (let rilievo of dettagliRilievi) {
            rilievo.qtaRilevata = rilievo.qtaRilevata || 0;
            if (rilievo.presets && rilievo.presets.length > 0) {
                rilievo.qtaRilevata = +rilievo.presets.find(p =>
                    p.CodiceAnagrafica === rilievo.qtaRilevata
                ).valoreAnagrafica;
                rilievo.qtaRilevataString = rilievo.qtaRilevata.toString();
            }

            if (rilievo.avversitaGruppo) {
              
              rilievo.avversitaGruppo.dataSmaltimentoScorte = new Date();

              if(rilievo.avversitaGruppo.gruppo){
                rilievo.avversitaGruppo.gruppo.dataSmaltimentoScorte = new Date();
              }

            }
            if (rilievo.erbaInfestante) {
                rilievo.erbaInfestante.dataSmaltimentoScorte = new Date();
            }

            if(rilievo.esercizioCDC){
              if (rilievo.esercizioCDC.classType !== "EsercizioRilievoCDC")
                rilievo.esercizioCDC.classType = "EsercizioRilievoCDC"
              rilievo.esercizioCDC.dataRiferimento = rilievo.dataOraRilievo instanceof Date?
                rilievo.dataOraRilievo : new Date(rilievo.dataOraRilievo);
            }

            if(rilievo.unitaDiMisura?.tipoControllo){
              switch(rilievo.unitaDiMisura?.tipoControllo.codice){
                case enum_TipoControllo.AREA_TESTO:
                case enum_TipoControllo.CASELLA_TESTO:
                  rilievo.note = rilievo.qtaRilevataString;
                  break;
              }
            }

            attivita.risorse.push(rilievo);
        }
    }

    private getDettagliFertilizzazione(Sezione_Fertilizzante: Sezione_Prodotto_Fertilizzanti, attivita: Attivita){

        if(Sezione_Fertilizzante) {

            attivita.epoca = Sezione_Fertilizzante.EpocaFertilizzazione;

            attivita.modalitaApplicazione = Sezione_Fertilizzante.Modalita_Applicazione;

            const DosiFertilizzanti: Array<any> = Sezione_Fertilizzante.DosiProdotti;

            if(DosiFertilizzanti){
                DosiFertilizzanti.forEach(f=>{

                    let newDettaglioFertilizzazione = new DettaglioFertilizzazione();

                    if(f.Prodotto && f.Operazione){
                        let index: number = attivita.risorse.findIndex((r:any) =>
                            r.classType === "DettaglioFertilizzazione" &&
                            attivita.job.primaryKey.codice === f.Operazione.primaryKey.codice &&
                            r.prodotto.codice === f.Prodotto.prodotto
                        );

                        if(index === -1){

                            let newRisorsaProdotto: RisorsaProdotto = this.getRisorsaProdotto(f);

                            newDettaglioFertilizzazione.prodotto = newRisorsaProdotto.prodotto;

                            newDettaglioFertilizzazione.MagazziniMovimentazioni = newRisorsaProdotto.MagazziniMovimentazioni;

                            newDettaglioFertilizzazione.flagDoseQuantitaTotale = newRisorsaProdotto.flagDoseQuantitaTotale;

                            newDettaglioFertilizzazione.flagTipoDose = newRisorsaProdotto.flagTipoDose;

                            newDettaglioFertilizzazione.doseHaReale = newRisorsaProdotto.doseHaReale;

                            newDettaglioFertilizzazione.doseHlReale = newRisorsaProdotto.doseHlReale;

                            newDettaglioFertilizzazione.quantitaTotaleReale = newRisorsaProdotto.quantitaTotaleReale;

                            newDettaglioFertilizzazione.unitaDiMisura = newRisorsaProdotto.unitaDiMisura;

                            newDettaglioFertilizzazione.unitaDiMisuraIndicata = newRisorsaProdotto.unitaDiMisuraIndicata;

                            newDettaglioFertilizzazione.Cu = f.Cu;

                            newDettaglioFertilizzazione.K = f.K;

                            newDettaglioFertilizzazione.Mg = f.Prodotto.Mg;

                            newDettaglioFertilizzazione.N = f.N;

                            newDettaglioFertilizzazione.P = f.P;

                            newDettaglioFertilizzazione.efficienza = f.Efficienza;

                            newDettaglioFertilizzazione.effluente = <Effluente> f.Prodotto.effluente;

                            newDettaglioFertilizzazione.tipoFertilizzante = <TipoFertilizzante> f.Prodotto.tipoFertilizzante;

                            newDettaglioFertilizzazione.tipologieFertilizzante = new Array<TipologiaFertilizzante>();

                            if (f.Prodotto.tipologieFertilizzante != undefined) {
                                (f.Prodotto.tipologieFertilizzante as Array<any>).forEach((element: TipologiaFertilizzante) => {
                                    newDettaglioFertilizzazione.tipologieFertilizzante.push(element);
                                });
                            }

                            attivita.risorse.push(newDettaglioFertilizzazione);

                        }else{

                            //In questo caso è stato inserito lo stesso prodotto in due magazzini differenti quindi nel modello dovrò
                            //aggiungere un elemento nell'array  MagazziniMovimentazioni nello stesso prodotto

                            newDettaglioFertilizzazione = attivita.risorse[index] as DettaglioFertilizzazione;

                            if(!newDettaglioFertilizzazione.MagazziniMovimentazioni)
                                newDettaglioFertilizzazione.MagazziniMovimentazioni = [];

                            let newRilevamentoDiMagazzino: RilevamentoDiMagazzino = this.getRilevamentoDiMagazzino(f);

                            newDettaglioFertilizzazione.MagazziniMovimentazioni.push(newRilevamentoDiMagazzino);

                        }
                    }

                });
            }

        }

    }

    private getDettagliFertilizzazioneFromDdl(Sezione_Fertilizzante: Sezione_Prodotto_Fertilizzanti, attivita: Attivita,UdM :UnitaDiMisura){

        if(Sezione_Fertilizzante){

            attivita.epoca = Sezione_Fertilizzante.EpocaFertilizzazione;

            attivita.modalitaApplicazione = Sezione_Fertilizzante.Modalita_Applicazione;

            let newDettaglioFertilizzazione = new DettaglioFertilizzazione();

            let newRisorsaProdotto: RisorsaProdotto = this.getRisorsaProdottoFromDdl(Sezione_Fertilizzante,UdM);

            newDettaglioFertilizzazione.prodotto = newRisorsaProdotto.prodotto;
            newDettaglioFertilizzazione.MagazziniMovimentazioni = newRisorsaProdotto.MagazziniMovimentazioni;
            newDettaglioFertilizzazione.flagDoseQuantitaTotale = newRisorsaProdotto.flagDoseQuantitaTotale;
            newDettaglioFertilizzazione.flagTipoDose = newRisorsaProdotto.flagTipoDose;
            newDettaglioFertilizzazione.doseHaReale = newRisorsaProdotto.doseHaReale;
            newDettaglioFertilizzazione.doseHlReale = newRisorsaProdotto.doseHlReale;
            newDettaglioFertilizzazione.quantitaTotaleReale = newRisorsaProdotto.quantitaTotaleReale;
            newDettaglioFertilizzazione.unitaDiMisuraIndicata = newRisorsaProdotto.unitaDiMisuraIndicata;
            newDettaglioFertilizzazione.unitaDiMisura = newRisorsaProdotto.unitaDiMisura;
            newDettaglioFertilizzazione.Cu = Sezione_Fertilizzante.Cu;
            newDettaglioFertilizzazione.K = Sezione_Fertilizzante.K;
            newDettaglioFertilizzazione.Mg = Sezione_Fertilizzante.Prodotto.Mg;
            newDettaglioFertilizzazione.N = Sezione_Fertilizzante.N;
            newDettaglioFertilizzazione.P = Sezione_Fertilizzante.P;
            newDettaglioFertilizzazione.efficienza = Sezione_Fertilizzante.Efficienza;

            newDettaglioFertilizzazione.effluente = <Effluente> Sezione_Fertilizzante.Prodotto.effluente;
            newDettaglioFertilizzazione.tipoFertilizzante = <TipoFertilizzante> Sezione_Fertilizzante.Prodotto.tipoFertilizzante;
            newDettaglioFertilizzazione.tipologieFertilizzante = [];

            if (Sezione_Fertilizzante.Prodotto.tipologieFertilizzante && Sezione_Fertilizzante.Prodotto.tipologieFertilizzante.length > 0) {
                (Sezione_Fertilizzante.Prodotto.tipologieFertilizzante as Array<any>).forEach((element: TipologiaFertilizzante) => {
                    newDettaglioFertilizzazione.tipologieFertilizzante.push(element);
                });
            }

            attivita.risorse.push(newDettaglioFertilizzazione);

        }
    }

    private  getRisorsaProdotto(gridrowDosiProdotti: any){

        let newRisorsaProdotto = new RisorsaProdotto();

        newRisorsaProdotto.prodotto = <Prodotto> gridrowDosiProdotti.Prodotto.prodotto;
        newRisorsaProdotto.MagazziniMovimentazioni = [];

        if(gridrowDosiProdotti.Magazzino_del_Prodotto_Selezionato?.primaryKey &&
            gridrowDosiProdotti.Magazzino_del_Prodotto_Selezionato?.primaryKey.codice !== 0){

            let newRilevamentoDiMagazzino: RilevamentoDiMagazzino = this.getRilevamentoDiMagazzino(gridrowDosiProdotti);

            newRisorsaProdotto.MagazziniMovimentazioni.push(newRilevamentoDiMagazzino);

            //Sarebbero le somme dei valori salvati nei magazzini ma non c'è bisogno che faccio questi calcoli
            //perchè vengono rifatti lato server

            newRisorsaProdotto.doseHaReale = gridrowDosiProdotti.Dose_Ha;
            newRisorsaProdotto.doseHlReale = gridrowDosiProdotti.Dose_Hl;
            newRisorsaProdotto.quantitaTotaleReale = gridrowDosiProdotti.DoseTot_Ha;

        }else{

            newRisorsaProdotto.doseHaReale = gridrowDosiProdotti.Dose_Ha;
            newRisorsaProdotto.doseHlReale = gridrowDosiProdotti.Dose_Hl;
            newRisorsaProdotto.quantitaTotaleReale = gridrowDosiProdotti.DoseTot_Ha;

        }

        newRisorsaProdotto.flagDoseQuantitaTotale = gridrowDosiProdotti.flagDoseQuantitaTotale;
        newRisorsaProdotto.flagTipoDose = gridrowDosiProdotti.flagTipoDose;
        newRisorsaProdotto.unitaDiMisura = <UnitaDiMisura> gridrowDosiProdotti.UdM_Magazzino;
        newRisorsaProdotto.unitaDiMisuraIndicata = <UnitaDiMisura> gridrowDosiProdotti.UdM_Indicata;

        return newRisorsaProdotto;

    }

    private getRisorsaProdottoFromDdl(Sezione_Prodotto:
        Sezione_Prodotto_Formulati | Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Sementi,UdM: UnitaDiMisura) {

        let newRisorsaProdotto = new RisorsaProdotto();
        newRisorsaProdotto.prodotto = <Prodotto> Sezione_Prodotto.Prodotto.prodotto;
        newRisorsaProdotto.MagazziniMovimentazioni = [];

        if(Sezione_Prodotto.Magazzino_del_Prodotto_Selezionato?.primaryKey &&
            Sezione_Prodotto.Magazzino_del_Prodotto_Selezionato?.primaryKey.codice !== 0) {

            let newRilevamentoDiMagazzino: RilevamentoDiMagazzino = this.getRilevamentoDiMagazzinoFromDdl(Sezione_Prodotto,UdM);
            newRisorsaProdotto.MagazziniMovimentazioni.push(newRilevamentoDiMagazzino);

            //Sarebbero le somme dei valori salvati nei magazzini ma non c'è bisogno che faccio questi calcoli
            //perchè vengono rifatti lato server

            newRisorsaProdotto.doseHaReale = Sezione_Prodotto.Dose_Ha;
            newRisorsaProdotto.doseHlReale = Sezione_Prodotto.Dose_Hl;
            newRisorsaProdotto.quantitaTotaleReale = Sezione_Prodotto.DoseTot_Ha;

        } else {
            newRisorsaProdotto.doseHaReale = Sezione_Prodotto.Dose_Ha;
            newRisorsaProdotto.doseHlReale = Sezione_Prodotto.Dose_Hl;
            newRisorsaProdotto.quantitaTotaleReale = Sezione_Prodotto.DoseTot_Ha;
        }

        newRisorsaProdotto.flagDoseQuantitaTotale = Sezione_Prodotto.flagDoseQuantitaTotale;
        newRisorsaProdotto.flagTipoDose = Sezione_Prodotto.flagTipoDose;

        newRisorsaProdotto.unitaDiMisuraIndicata = UdM;
        newRisorsaProdotto.unitaDiMisura = this.gridDosiProdottiService.getUnitaDiMisura(UdM);

        newRisorsaProdotto.unitaDiMisuraIndicata.simbolo = this.gridDosiProdottiService.getSimboloUnitaDiMisuraIndicata(newRisorsaProdotto.unitaDiMisuraIndicata, Sezione_Prodotto.flagTipoDose)

        return newRisorsaProdotto;
    }

    private getRilevamentoDiMagazzino(gridrowDosiProdotti: any,Innesco: boolean = false): RilevamentoDiMagazzino{

        let rilevamentodimagazzino = new RilevamentoDiMagazzino();

        if(Innesco){
          rilevamentodimagazzino.Prodotto = new Prodotto(gridrowDosiProdotti.Av_Cod, gridrowDosiProdotti.Av_Gru);
          rilevamentodimagazzino.Prodotto.elemCod = INNESCHI;
          rilevamentodimagazzino.Magazzino = <Fabbricato> gridrowDosiProdotti.Magazzino_Innesco;
          rilevamentodimagazzino.Agenzia = <Fabbricato> gridrowDosiProdotti.Magazzino_Agenzia_Innesco;
          rilevamentodimagazzino.Qta = this.getDoseTrasformata(gridrowDosiProdotti.DoseTot_Ha_Innesco, gridrowDosiProdotti.UdM_Innesco?.codice);
          rilevamentodimagazzino.Lotto = gridrowDosiProdotti.Lotto_Innesco;
          rilevamentodimagazzino.udm = <UnitaDiMisura> gridrowDosiProdotti.UdM_Innesco;
          rilevamentodimagazzino.registrazioniCollegate = [];
        }else{
          rilevamentodimagazzino.Prodotto = <Prodotto> gridrowDosiProdotti.Prodotto.prodotto;
          rilevamentodimagazzino.Magazzino = <Fabbricato> gridrowDosiProdotti.Magazzino_del_Prodotto_Selezionato;
          rilevamentodimagazzino.Agenzia = <Fabbricato> gridrowDosiProdotti.Magazzino_Agenzia;
          rilevamentodimagazzino.Qta = this.getDoseTrasformata(gridrowDosiProdotti.DoseTot_Ha, gridrowDosiProdotti.UdM_Indicata?.codice);
          rilevamentodimagazzino.Lotto = gridrowDosiProdotti.Lotto;
          rilevamentodimagazzino.udm = <UnitaDiMisura> gridrowDosiProdotti.UdM_Magazzino;
          rilevamentodimagazzino.doseHaIndicata = gridrowDosiProdotti.Dose_Ha;
          rilevamentodimagazzino.doseHlIndicata = gridrowDosiProdotti.Dose_Hl;
          rilevamentodimagazzino.registrazioniCollegate = [];

          if(gridrowDosiProdotti.Magazzino_Esterno && (gridrowDosiProdotti.Magazzino_Esterno as Fabbricato).primaryKey.codice > 0){

            const rilevamentodimagazzinoCopia = cloneDeep(rilevamentodimagazzino);

            const rilevamentodimagazzinoEsterno = cloneDeep(rilevamentodimagazzino);

            rilevamentodimagazzinoEsterno.Magazzino = gridrowDosiProdotti.Magazzino_Esterno;

            const attivitaCarico = this.getAttivita_Scarico_Carico(rilevamentodimagazzinoCopia,enum_LAVCOD.CARICO,gridrowDosiProdotti.DoseTot_Ha,gridrowDosiProdotti.UdM_Indicata);

            rilevamentodimagazzino.registrazioniCollegate.push(attivitaCarico);

            const attivitaScarico = this.getAttivita_Scarico_Carico(rilevamentodimagazzinoEsterno,enum_LAVCOD.SCARICO,gridrowDosiProdotti.DoseTot_Ha,gridrowDosiProdotti.UdM_Indicata);

            rilevamentodimagazzino.registrazioniCollegate.push(attivitaScarico);
          }
        }

        if(gridrowDosiProdotti.Categoria_Magazzino === FERTILIZZANTI){

          rilevamentodimagazzino.N = gridrowDosiProdotti.N;
          rilevamentodimagazzino.P2O5 = gridrowDosiProdotti.P;
          rilevamentodimagazzino.K2O = gridrowDosiProdotti.K;
          rilevamentodimagazzino.Cu = gridrowDosiProdotti.Cu;

        }


        return rilevamentodimagazzino;
    }

    private getRilevamentoDiMagazzinoFromDdl(Sezione_Formulato: any,UdM:UnitaDiMisura): RilevamentoDiMagazzino{

        let rilevamentodimagazzino = new RilevamentoDiMagazzino();

        rilevamentodimagazzino.Prodotto = <Prodotto> Sezione_Formulato.Prodotto.prodotto;

        rilevamentodimagazzino.Magazzino = <Fabbricato> Sezione_Formulato.Magazzino_del_Prodotto_Selezionato;

        rilevamentodimagazzino.Agenzia = this.gridDosiProdottiService.getMagazzino_Agenzia(Sezione_Formulato.Prodotto.MagazziniMovimentazioni);

        rilevamentodimagazzino.Qta = this.getDoseTrasformata(Sezione_Formulato.DoseTot_Ha, UdM.codice);

        rilevamentodimagazzino.Lotto = Sezione_Formulato.Lotto;

        rilevamentodimagazzino.udm = <UnitaDiMisura> this.gridDosiProdottiService.getUnitaDiMisura(UdM);

        rilevamentodimagazzino.doseHaIndicata = Sezione_Formulato.Dose_Ha;

        rilevamentodimagazzino.doseHlIndicata = Sezione_Formulato.Dose_Hl;

        rilevamentodimagazzino.registrazioniCollegate = [];

        if(Sezione_Formulato.Magazzino_Esterno && (Sezione_Formulato.Magazzino_Esterno as Fabbricato).primaryKey.codice > 0){

          const rilevamentodimagazzinoCopia = cloneDeep(rilevamentodimagazzino);

          const rilevamentodimagazzinoEsterno = cloneDeep(rilevamentodimagazzino);

          rilevamentodimagazzinoEsterno.Magazzino = Sezione_Formulato.Magazzino_Esterno;

          const attivitaCarico = this.getAttivita_Scarico_Carico(rilevamentodimagazzinoCopia,enum_LAVCOD.CARICO,Sezione_Formulato.DoseTot_Ha,Sezione_Formulato.Magazzino_del_Prodotto_Selezionato);

          rilevamentodimagazzino.registrazioniCollegate.push(attivitaCarico);

          const attivitaScarico = this.getAttivita_Scarico_Carico(rilevamentodimagazzinoEsterno,enum_LAVCOD.SCARICO,Sezione_Formulato.DoseTot_Ha,Sezione_Formulato.Magazzino_del_Prodotto_Selezionato);

          rilevamentodimagazzino.registrazioniCollegate.push(attivitaScarico);
        }

        if(Sezione_Formulato.Categoria_Magazzino === FERTILIZZANTI){

          rilevamentodimagazzino.N = Sezione_Formulato.N;
          rilevamentodimagazzino.P2O5 = Sezione_Formulato.P;
          rilevamentodimagazzino.K2O = Sezione_Formulato.K;
          rilevamentodimagazzino.Cu = Sezione_Formulato.Cu;

        }


        return rilevamentodimagazzino;
    }

    private getAttivita_Scarico_Carico(rilevamentodiMagazzino: RilevamentoDiMagazzino,lav_cod: number,DoseTot_Ha: number,UdM: UnitaDiMisura): Attivita{

      const attivitaRegistrazione = new Attivita();

      attivitaRegistrazione.job = new Lavorazione(lav_cod.toString());

      const risorsaRegistrazione = new RisorsaRegistrazione();

      risorsaRegistrazione.prodotto = rilevamentodiMagazzino.Prodotto;

      risorsaRegistrazione.qta = DoseTot_Ha;

      risorsaRegistrazione.unitaDiMisura = UdM;

      risorsaRegistrazione.MagazziniMovimentazioni = [];

      risorsaRegistrazione.MagazziniMovimentazioni.push(rilevamentodiMagazzino);

      attivitaRegistrazione.risorse = [];

      attivitaRegistrazione.risorse.push(risorsaRegistrazione);

      return attivitaRegistrazione;
    }

    /** Crea un rilevamento di magazzino con lotto e unità di misura valorizzati
     * secondo quanto specificato nella RisorsaProdotto. Il  magazzino è un
     * nuovo fabbricato di codice 0.
     */
     public getRilevamentoNoMagazzino(R: RisorsaProdotto, atv: Attivita): RilevamentoDiMagazzino {
        let rilievo = new RilevamentoDiMagazzino();

        rilievo.Prodotto = R.prodotto;
        rilievo.Magazzino = new Fabbricato({
            codice: 0,
            centroAziendalePK: atv.centroAziendale.primaryKey
        });
        rilievo.QtaTot = 0;
        rilievo.Qta = R.quantitaTotaleReale ? R.quantitaTotaleReale : 0;
        rilievo.Lotto = '';
        rilievo.udm = R.unitaDiMisura;

        return rilievo;
    }

    //Corrisponde alla GetDoseTrasformata della Trattamenti_2
    private getDoseTrasformata(dose: number, udm_codice: number): number{
        let doseTrasformata: number = 0

        switch(udm_codice) {
            case enum_UnitaMisura.Grammi:
                doseTrasformata = dose / 1000;
                break;
            case enum_UnitaMisura.Milligrammi:
                doseTrasformata = dose / 1000000;
                break;
            case enum_UnitaMisura.Quintali:
                doseTrasformata = dose * 100;
                break;
            case enum_UnitaMisura.Tonnellate:
                doseTrasformata = dose * 1000;
                break;
            case enum_UnitaMisura.Millilitri:
                doseTrasformata = dose / 1000;
                break;
            case enum_UnitaMisura.CentimetriCubi:
                doseTrasformata = dose / 1000;
                break;
            case enum_UnitaMisura.Metri_Cubi:
                doseTrasformata = dose * 1000;
                break;
            case enum_UnitaMisura.Litri:
            case enum_UnitaMisura.KG:
            case enum_UnitaMisura.Unita_Seme:
            case enum_UnitaMisura.Num_Piante:
            case enum_UnitaMisura.Confezioni:
            case enum_UnitaMisura.Numero:
            case enum_UnitaMisura.Numero_Diffusori:
            case enum_UnitaMisura.UNITA:
            case enum_UnitaMisura.Numero_Trappole:
                doseTrasformata = dose
                break;
        }

        return doseTrasformata;
    }

    /**
     * @description
     * Restituisce una listParametri_Aggiuntivi_Attivita utile in fase di salvataggio per inviare
     * delle informazioni al server che non sono mappate nel modello Attivita legate alla sezione dei prodotti
     */
    setParametriAggiuntivi_Attivita_x_Sezioni(s: Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Formulati | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta,
                                                listParametri_Aggiuntivi_Attivita: Parametri_Aggiuntivi_Attivita[]) {


        if(s) {
            switch(s.Categoria_Magazzino){
                case FORMULATI:
                    break;
                case FERTILIZZANTI:
                    break;
                case SEMENTI:

                    s = s as Sezione_Prodotto_Sementi;

                    if(s.Opzioni_Semina){

                        listParametri_Aggiuntivi_Attivita.push({
                            operazione: s.Operazione,
                            key: Key_Parametri_Aggiuntivi_Attivita.Opzione_Semina,
                            value: !s.Opzioni_Semina ? "0": s.Opzioni_Semina.codice.toString()
                        });


                        if(s.Opzioni_Semina.codice === enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default){

                            let new_sup_trattata_list: Array<SupTrattata_x_DettaglioSemina>=[];

                            s.DosiProdotti.forEach(d=>{

                                let mat_cod:number = (d.Prodotto as MultiColumnComboboxSemina).prodotto.codice;

                                new_sup_trattata_list.push(<SupTrattata_x_DettaglioSemina>{
                                    Mat_Cod: mat_cod,
                                    Sup_Trattata: d.Sup_Calcolata
                                });
                            });

                            listParametri_Aggiuntivi_Attivita.push({
                                operazione: s.Operazione,
                                key: Key_Parametri_Aggiuntivi_Attivita.lista_SupTrattata_x_DettaglioSemina,
                                value: JSON.stringify(new_sup_trattata_list)
                            });
                        }
                    }


                    break;
                case TRASFORMATI_VEGETALI:

                    if (parseInt(s.Operazione.primaryKey.codice) === enum_LAVCOD.RACCOLTA) {
                        s = s as Sezione_Prodotto_Raccolta;

                        if(!s.Opzioni_Raccolta){
                            s.Opzioni_Raccolta = new OpzioniRaccolta();
                        }
                        // Passo opzioni raccolta
                        listParametri_Aggiuntivi_Attivita.push({
                            operazione: s.Operazione,
                            key: "Opzione_Raccolta_Aggiornamento_Anagrafica",
                            value: s.Opzioni_Raccolta.Chiusura.toString()
                        });

                        let newList_DataCarenzaRaccolta = [];

                        let gridImpiantiSelezionati: Array<GridImpiantoSelezionatoModel>=this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue();

                        if(gridImpiantiSelezionati && gridImpiantiSelezionati.length > 0){
                            gridImpiantiSelezionati.forEach(i=>{
                                newList_DataCarenzaRaccolta.push(<DataCarenzaRaccolta_x_Impianto>{
                                    KeyImpianto: i.PIVA + '|' + i.SA_COD + '|' +  i.APPEZZA + '|' +  i.ID_REG,
                                    App_Nome: i.App_Nome,
                                    DataCarenza: this.datePipe.transform(i.DataCarenza,'dd/MM/yyyy',undefined,this.locale_id),
                                    CarenzaStr: i.CarenzaStr
                                });
                            });
                        }

                        listParametri_Aggiuntivi_Attivita.push({
                            operazione: s.Operazione,
                            key: Key_Parametri_Aggiuntivi_Attivita.DataCarenzaRaccolta_x_Impianto,
                            value: JSON.stringify(newList_DataCarenzaRaccolta)
                        });
                    }

                    break;
            }

        }
    }

    /**
     * @description
     * Restituisce una listParametri_Aggiuntivi_Attivita utile in fase di salvataggio per inviare
     * delle informazioni al server che non sono mappate nel modello Attivita
     */
    setParametriAggiuntivi_Attivita_Generali(listParametri_Aggiuntivi_Attivita: Parametri_Aggiuntivi_Attivita[]) {

        let codici_Attivita_x_CentriAziendali = this.setParametriAggiuntivi_X_MultiCentro();

        if(codici_Attivita_x_CentriAziendali && codici_Attivita_x_CentriAziendali.length > 0){
            if(listParametri_Aggiuntivi_Attivita.findIndex(p=>p.key === Key_Parametri_Aggiuntivi.lista_Codici_Attivita_x_CentriAziendali) === -1){
                listParametri_Aggiuntivi_Attivita.push({
                    operazione: null,
                    key: Key_Parametri_Aggiuntivi.lista_Codici_Attivita_x_CentriAziendali,
                    value: JSON.stringify(codici_Attivita_x_CentriAziendali)
                });
            }
        }

        this.setParametriAggiuntivi_Id_Visita_Collegata(listParametri_Aggiuntivi_Attivita);

        if(listParametri_Aggiuntivi_Attivita.findIndex(p=>p.key === Key_Parametri_Aggiuntivi_Attivita.CaricoMagazzinoAutomatico) === -1){
          listParametri_Aggiuntivi_Attivita.push({
            operazione: null,
            key: Key_Parametri_Aggiuntivi_Attivita.CaricoMagazzinoAutomatico,
            value: "false"
          });
        }

    }

    public setParametriAggiuntivi_X_MultiCentro():Array<Codici_Attivita_x_CentriAziendali> {

        let codici_Attivita_x_CentriAziendali: Array<Codici_Attivita_x_CentriAziendali> = [];

        let Codici_Attivita = this.qdcservice.TestataForm.get("Codici_Attivita").getRawValue() as Array<CodiciXOperazione>;

        //Valorizzo la lista Aggiuntiva sempre in modifica oppure se sono
        //in ribaltamento di una ricetta in agenda(in questo caso prendo l'id_agenda,ricetta_operazione_cod e ricetta_cod dall'associazione PK)
        if(this.qdcservice.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Update ||
          this.qdcservice.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Read){

            Codici_Attivita.forEach(co=>{

                let Sa_Cod = 0;

                if(co.Centro_Aziendale){
                    Sa_Cod = + co.Centro_Aziendale.primaryKey.codice;
                }

                codici_Attivita_x_CentriAziendali.push(<Codici_Attivita_x_CentriAziendali>{
                    Id_Agenda: + co.CodiceAttivita,
                    Sa_Cod : Sa_Cod,
                    Lav_Cod: + co.Operazione.primaryKey.codice,
                    Ricetta_Cod: + co.CodiceRicetta,
                    Ricetta_Operazione_Cod: + co.CodiceOperazioneRicetta
                });
            });

        }else if(this.qdcservice.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write &&
                this.qdcservice.TipoRibaltamento !== RibaltamentoTypes.Nessuno){


            Codici_Attivita.forEach(co=>{

                let Sa_Cod = 0;

                if(co.Centro_Aziendale){
                    Sa_Cod = + co.Centro_Aziendale.primaryKey.codice;
                }

                codici_Attivita_x_CentriAziendali.push(<Codici_Attivita_x_CentriAziendali>{
                    Id_Agenda: co.Associazione_PK.id_agenda,
                    Sa_Cod : Sa_Cod,
                    Lav_Cod: + co.Operazione.primaryKey.codice,
                    Ricetta_Cod: co.Associazione_PK.Ricetta_Cod,
                    Ricetta_Operazione_Cod: co.Associazione_PK.Ricetta_Operazione_Cod
                });
            });

        }

        return codici_Attivita_x_CentriAziendali;
    }

    private setParametriAggiuntivi_Id_Visita_Collegata(listParametri_Aggiuntivi_Attivita: Parametri_Aggiuntivi_Attivita[]){
      let Codici_Attivita = this.qdcservice.TestataForm.get("Codici_Attivita").getRawValue() as Array<CodiciXOperazione>;

      if(Codici_Attivita && Codici_Attivita.length > 0){

        let Codice_Attivita_con_Visita = Codici_Attivita.find(c=>c.CodiceAttivitaVisita && c.CodiceAttivitaVisita !== "" && + c.CodiceAttivitaVisita !== 0)

        if(Codice_Attivita_con_Visita && + Codice_Attivita_con_Visita.CodiceAttivitaVisita > 0){
          listParametri_Aggiuntivi_Attivita.push({
            operazione: null,
            key: Key_Parametri_Aggiuntivi.Id_Visita_Collegata,
            value: Codice_Attivita_con_Visita.CodiceAttivitaVisita
          });
        }
      }
    }

    private setTestataRicetta(ca:CodiciXOperazione): TestataRicetta{
        let testataRicetta: TestataRicetta = null;

        if(this.qdcservice.TestataForm.get("Tipo").value === Tipo_Attivita.Ricetta){

            let Data_Operazione: Date = this.qdcservice.TestataForm.get("Data").value;

            //prendo la data inizio editata solo se <= alla data dell'operazione
            let Ricetta_DataInizio: Date = null;

            if(this.qdcservice.TestataForm.get("Ricetta_Data_Da").value && this.qdcservice.TestataForm.get("Ricetta_Data_Da").value.getTime() <=  Data_Operazione.getTime()){
                Ricetta_DataInizio = this.qdcservice.TestataForm.get("Ricetta_Data_Da").value;
            }else{
                Ricetta_DataInizio = Data_Operazione;
            }

            //prendo la data fine editata solo se >= alla data dell'operazione
            let Ricetta_DataFine: Date = null;

            if(this.qdcservice.TestataForm.get("Ricetta_Data_A").value && this.qdcservice.TestataForm.get("Ricetta_Data_A").value.getTime() >= Data_Operazione.getTime()){
                Ricetta_DataFine = this.qdcservice.TestataForm.get("Ricetta_Data_A").value;
            }else{
                Ricetta_DataFine = Data_Operazione;
            }

            testataRicetta = {
                Ricetta_Cod: + ca.CodiceRicetta,
                Ricetta_Des:this.qdcservice.TestataForm.get("Ricetta_Des").value,
                Ricetta_Des_Long:this.qdcservice.TestataForm.get("Ricetta_Des_Long").value,
                Ricetta_Numero:this.qdcservice.TestataForm.get("Ricetta_Numero").value,
                Note:this.qdcservice.TestataForm.get("Ricetta_Note").value,
                Data_Da:Ricetta_DataInizio,
                Data_A:Ricetta_DataFine,
                pua: ca.pua
            };
        }

        return testataRicetta;
    }

    private set_lista_MostraWarning(listParametri_Aggiuntivi_Attivita: Parametri_Aggiuntivi_Attivita[]){

        let lista_MostraWarning: Array<Parametri_Aggiuntivi_Attivita> = [];

        lista_MostraWarning.push({
            operazione: null,
            key: Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita,
            value: "true"
        });

        lista_MostraWarning.push({
            operazione: null,
            key: Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckMagazzino,
            value: "true"
        });

        lista_MostraWarning.push({
            operazione: null,
            key: Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda,
            value: "true"
        });

        lista_MostraWarning.push({
            operazione: null,
            key: Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckDPI,
            value: "true"
        });

        listParametri_Aggiuntivi_Attivita.push({
            operazione: null,
            key: Key_Parametri_Aggiuntivi_Attivita.lista_MostraWarning,
            value: JSON.stringify(lista_MostraWarning)
        });

    }

    public compareDettaglioRaccolta(d1: DettaglioRaccolta, d2: DettaglioRaccolta): boolean {
        if (!d1 || !d2) return false;

        if (d1.prodotto.codice !== d2.prodotto.codice) return false;

        let mov1 = d1.MagazziniMovimentazioni?.at(0);
        let mov2 = d2.MagazziniMovimentazioni?.at(0);

        if (!mov1 && !mov2) return true;
        if (!mov1 || !mov2) return false;

        let qsi1 = d1.QuantitaSuImpianti;
        let qsi2 = d2.QuantitaSuImpianti;
        if (qsi1[0].esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
            !== qsi2[0].esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice) {
            // Gli impianti appartengono a due centri diversi!
            return false;
        }

        // In entrambi i casi non ho selezionato il magazzino
        // in entrambi i casi il lotto sarà indefinito
        if (!mov1.Magazzino?.primaryKey?.codice
            && !mov2.Magazzino?.primaryKey?.codice) {
                return true;
        }
        return this.fabbricatiService.Equals(mov1.Magazzino, mov2.Magazzino)
            && qsi1[0].Lotto === qsi2[0].Lotto;
    }

    private adjustRaccoltaCDCs(attivita: Attivita, ProdottiRaccolti: Array<DettaglioRaccolta>): void {
        let cdcs: Esercizio[] = this.getEserciziFromRaccolta(ProdottiRaccolti)
        this.addSelectedCDCToRaccolta(attivita, ProdottiRaccolti, cdcs);
        this.removeUnselectdCDCFromRaccolta(attivita, ProdottiRaccolti, cdcs);
        this.fixSuperficieTrattataRaccolta(attivita, ProdottiRaccolti);
    }

    private getEserciziFromRaccolta(ProdottiRaccolti: Array<DettaglioRaccolta>): Esercizio[] {
        const cdcs: Esercizio[] = [];
        const isManuale = this.qdcservice.QdCForm.value.Trattamento.Sezioni_Prodotto[0]
            .Opzioni_Raccolta?.Ripartizione === enum_Ripartizione_Raccolta.MANUALE;
        for (let prod of ProdottiRaccolti) {
            prod.QuantitaSuImpianti.forEach((q) => {
                if (!(q.Qta === 0 && isManuale) && !cdcs.find(e => getChiaveEsercizio(e) === getChiaveEsercizio(q.esercizioCDC.esercizio)))
                    cdcs.push(q.esercizioCDC.esercizio)
            });
        }
        return cdcs;
    }

    private addSelectedCDCToRaccolta(attivita: Attivita, ProdottiRaccolti: Array<DettaglioRaccolta>, cdcs: Esercizio[]) {
        // Necessario per ricollegare gli impianti di centri diversi
        attivita.centriDiCosto.filter((cdc: EsercizioCDC) =>
            !cdcs.find(e => getChiaveEsercizio(e) === getChiaveEsercizio(cdc.esercizio))
        ).forEach((cdc: EsercizioCDC) => {
            for (let prod of ProdottiRaccolti) {
                let q = new QuantitaSuImpianto();
                if (prod.QuantitaSuImpianti?.at(0)) {
                    q = cloneDeep(prod.QuantitaSuImpianti?.at(0))
                }
                q.Qta = 0;
                q.esercizioCDC = cdc;
                prod.QuantitaSuImpianti.push(q)
            }
        })
        this.addCDCToQuantitaSuImpianti(attivita, ProdottiRaccolti, cdcs);
    }

    private addCDCToQuantitaSuImpianti(attivita: Attivita, ProdottiRaccolti: Array<DettaglioRaccolta>, cdcs: Esercizio[]) {
        for (let dettaglio of ProdottiRaccolti) {
            let notContained = cdcs.filter(esercizio =>
                !dettaglio.QuantitaSuImpianti
                    .map(suImpianto => getChiaveEsercizio(suImpianto.esercizioCDC.esercizio))
                    .find(key => key === getChiaveEsercizio(esercizio)))
            for (let esercizio of notContained) {
                let esercizioCDC = attivita.centriDiCosto.map(cdc => cdc as EsercizioCDC)
                    .find(cdc => getChiaveEsercizio(cdc.esercizio) === getChiaveEsercizio(esercizio));
                let suImpianto = new QuantitaSuImpianto();
                if (dettaglio.QuantitaSuImpianti?.at(0)) {
                    suImpianto = cloneDeep(dettaglio.QuantitaSuImpianti?.at(0))
                }
                suImpianto.Qta = 0;
                suImpianto.esercizioCDC = esercizioCDC;
                dettaglio.QuantitaSuImpianti.push(suImpianto)
            }
        }
    }

    private removeUnselectdCDCFromRaccolta(attivita: Attivita, ProdottiRaccolti: Array<DettaglioRaccolta>, cdcs: Esercizio[]) {
        cdcs.filter(e =>
            !attivita.centriDiCosto.map((cdc: EsercizioCDC) => getChiaveEsercizio(cdc.esercizio))
                .includes(getChiaveEsercizio(e))
        ).forEach(e => {
            for (let prod of ProdottiRaccolti) {
                let i = prod.QuantitaSuImpianti.findIndex(c => getChiaveEsercizio(c.esercizioCDC.esercizio) === getChiaveEsercizio(e))
                if (i < 0) continue;
                prod.QuantitaSuImpianti.splice(i, 1);
            }
        })
    }

    private fixSuperficieTrattataRaccolta(attivita: Attivita, ProdottiRaccolti: Array<DettaglioRaccolta>) {
        let supTrattate = new Map<string, number>();
        attivita.centriDiCosto.forEach((cdc) => {
            let eCDC = cdc as EsercizioCDC;
            supTrattate.set(getChiaveEsercizio(eCDC.esercizio), eCDC.superficieTrattata);
        })
        for (let dettaglio of ProdottiRaccolti) {
            dettaglio.QuantitaSuImpianti.forEach(suImpianto =>
                suImpianto.esercizioCDC.superficieTrattata = supTrattate.get(getChiaveEsercizio(suImpianto.esercizioCDC.esercizio)) || 0
            );
        }
    }

    private isAbbattimento(o: Lavorazione): boolean {
        return +o.primaryKey.codice == LAVCOD_ABBATTIMENTOIMPIANTI;
    }

    private getAvversita(gridrowDosiProdotti: any): AvversitaGruppo{

      let newAvversita: AvversitaGruppo = null;

      if(gridrowDosiProdotti.Avversita){

        newAvversita = cloneDeep(gridrowDosiProdotti.Avversita);

        newAvversita.MagazziniMovimentazioni = [];

        if(gridrowDosiProdotti.Magazzino_Innesco &&
          gridrowDosiProdotti.Magazzino_Innesco?.primaryKey &&
          gridrowDosiProdotti.Magazzino_Innesco?.primaryKey.codice !== 0){

          let newRilevamentoDiMagazzino: RilevamentoDiMagazzino = this.getRilevamentoDiMagazzino(gridrowDosiProdotti,true);

          newAvversita.MagazziniMovimentazioni.push(newRilevamentoDiMagazzino);

        }
      }

      return newAvversita;

    }

    private getAvversitafromDDL(Sezione_Prodotto:Sezione_Prodotto_Formulati): AvversitaGruppo{

      let newAvversita: AvversitaGruppo = cloneDeep(Sezione_Prodotto.Avversita);

      /*if(Sezione_Prodotto.Magazzino_Innesco &&
        Sezione_Prodotto.Magazzino_Innesco?.primaryKey &&
        Sezione_Prodotto.Magazzino_Innesco?.primaryKey.codice !== 0){

        let newRilevamentoDiMagazzino: RilevamentoDiMagazzino = this.getRilevamentoDiMagazzino(Sezione_Prodotto);

        newAvversita.MagazziniMovimentazioni.push(newRilevamentoDiMagazzino);

      }*/

      return newAvversita;
    }

    private mapRisorseMacchina(attivita: Attivita,flagVisita: boolean){

      let RisorseMacchina: Array<RisorsaMacchina> = [];

      let GridMacchine = this.qdcservice.MacchineFormArray.getRawValue() as Array<GridMacchinaModel>;

      if(GridMacchine && GridMacchine.length > 0 && !flagVisita){

        GridMacchine.forEach((m: GridMacchinaModel)=>{
          let newRisorsaMacchina = new RisorsaMacchina();

          let newMacchina = new ParcoMacchine();

          newMacchina.codice = m.Mac_Cod;

          newMacchina.descrizione = m.Mac_Des;

          newMacchina.partitaIva = m.Piva;

          newMacchina.modello = m.Modello;

          newMacchina.marca = m.Marca;

          newMacchina.taratura_Ugello = m.Taratura_Ugello;

          newMacchina.scadenza_Taratura = m.Data_Scadenza_Taratura;

          newMacchina.tipo = m.Tipo;

          newMacchina.dettaglio_1 = m.Dettaglio_1;

          newMacchina.dettaglio_2 = m.Dettaglio_2;

          newMacchina.validita = m.Validita;

          newRisorsaMacchina.macchina = newMacchina;

          RisorseMacchina.push(newRisorsaMacchina)
        });
      }

      //Aggiungo nelle Risorse Macchine le macchine di irrigazione associate agli impianti selezionati
      if(+ attivita.job.primaryKey.codice === enum_LAVCOD.FERTIRRIGAZIONE || + attivita.job.primaryKey.codice === enum_LAVCOD.IRRIGAZIONE){

        let ImpiantiSelezionati = this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue() as Array<GridImpiantoSelezionatoModel>;

        if(ImpiantiSelezionati && ImpiantiSelezionati.length > 0){
          let ImpiantiConMacchineIrrigazione: Array<GridImpiantoSelezionatoModel> = ImpiantiSelezionati.filter(i=> i.macchinaIrrigazione && i.macchinaIrrigazione.codice > 0);

          if(ImpiantiConMacchineIrrigazione && ImpiantiConMacchineIrrigazione.length > 0) {
            ImpiantiConMacchineIrrigazione.forEach(i => {

              if(RisorseMacchina.findIndex(r=>r.macchina.codice === i.macchinaIrrigazione.codice) === -1){
                if (i.macchinaIrrigazione) {

                  let newRisorsaMacchina = new RisorsaMacchina();

                  newRisorsaMacchina.macchina = i.macchinaIrrigazione;

                  RisorseMacchina.push(newRisorsaMacchina);
                }
              }
            });
          }
        }

      }

      if(RisorseMacchina && RisorseMacchina.length > 0)
        attivita.risorse = attivita.risorse.concat(RisorseMacchina);
    }

    private getAttivitaCollegate(o: Lavorazione): Attivita[] {
      let attivitaCollegate: Attivita[] = this.qdcservice.TestataForm.get("Attivita_Collegate").getRawValue();


      if(this.qdcservice.flag_Reinnesco &&
        (!attivitaCollegate || attivitaCollegate.length === 0)){
        attivitaCollegate = this.qdcservice.getListaAttivita();
      }

      return attivitaCollegate;
    }

    private getBlocco(): Blocco{

      let blocco: Blocco = this.qdcservice.TestataForm.get("Blocco_Attivita").getRawValue();

      return blocco;
    }
}

export function getChiaveEsercizio(e: Esercizio) {
    return e.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
        + "_" + e.impiantoPK.appezzamentoPK.centroAziendalePK.codice
        + "_" + e.impiantoPK.appezzamentoPK.codice
        + "_" + e.impiantoPK.codice
        + "_" + e.codice;
}
