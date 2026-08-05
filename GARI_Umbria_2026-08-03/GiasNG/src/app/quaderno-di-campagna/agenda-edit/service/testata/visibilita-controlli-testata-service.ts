import { Injectable } from '@angular/core';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { enum_LAVCOD } from 'app/Model/TipiEnumerativi';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import {QdCService} from "../qdc.service";
import {
    DpiBio,
    FERTILIZZANTI,
    FORMULATI, NessunDpi,
    NessunDpiNessunaEtichetta,
    TRASFORMATI_VEGETALI
} from "../../../../Model/CostantiPersonalizzate";
import { Tipo } from 'app/Model/attivita/centri_di_costo/CentroDiCosto';
import { Tipo_Attivita } from 'gias-ui-kit';

@Injectable()

export class QdCVisibilitaControlliTestataService{


    constructor(private objParametriAgendaService: ObjParametriAgendaService,
                private qdcservice: QdCService){


    }

    MostraddlAttivitaeDescrizione(l: Lavorazione){

        let mostra = false;

        if(+ l.primaryKey.codice === enum_LAVCOD.ALTRE_OPERAZIONI)
            mostra = true;

        return mostra;

    }

    MostraFiltroVisualizza_Solo_Operazioni_Preferite(){

        //In modifica non mostrare il filtro Visualizza_Solo_Operazioni_Preferite
        let mostra = false;

        if(this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB !== Enum_DBTypeOperation.Update &&
            this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB !== Enum_DBTypeOperation.Read)
            mostra = true;

        return mostra;

    }

    MostrabtnSalvaOperazioniPreferite(Operazioni_Scelte: Array<Lavorazione>){

        //In modifica non mostrare il bottone SalvaOperazioniPreferite
        let mostra = false;

        if(this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB !== Enum_DBTypeOperation.Update &&
            !this.DisabilitabtnSalvaOperazioniPreferite(Operazioni_Scelte))
            mostra = true;

        return mostra;
    }

    DisabilitabtnSalvaOperazioniPreferite(Operazioni_Scelte: Array<Lavorazione>){

        //Se non c'è nessuna operazione selezionata nascondo il pulsante di salvataggio preferiti altrimenti lo abilito

        let disabilita = true;

        if(Operazioni_Scelte && Operazioni_Scelte.length > 0)
            disabilita = false;

        return disabilita;

    }

    MostraBtn_VerificaDoseConsigliata(Operazioni: Array<Lavorazione>){

        let mostra = false;

        let index = -1;

        //Non visibile nelle Ricette e nel Brogliaccio
        if(this.qdcservice.TestataForm &&
            this.qdcservice.TestataForm.get("Tipo").value === Tipo_Attivita.QuadernoDiCampagna &&
            this.qdcservice.getCentroDiCostoTipo() != Tipo.ProdottoDaTrattare){

            if(Operazioni && Operazioni.length > 0){
                index = Operazioni.findIndex(o=> (this.qdcservice.Elenco_Operazioni_Formulati.includes(+ o.primaryKey.codice) ||
                                                            this.qdcservice.Elenco_Operazioni_Fertilizzanti.includes(+ o.primaryKey.codice)) &&
                                                            !this.qdcservice.Elenco_Operazioni_Con_Gestione_TrappoleFormulati.includes(+ o.primaryKey.codice));
            }

        }



        if(index > -1)
            mostra = true;

        return mostra;
    }

    MostraBtn_VerificaDoseConsigliataBS(Operazioni: Array<Lavorazione>) {
        let mostra = false;

        let index = -1;

        //Bottone di Verifica Dose Consigliata Disciplinare visibile solo per il diserbo e se è stato scelto un disciplinare

        //Non visibile nelle Ricette e Brogliaccio
        if(this.qdcservice.TestataForm &&
            this.qdcservice.TestataForm.get("Tipo").value === Tipo_Attivita.QuadernoDiCampagna){

            if(Operazioni && Operazioni.length > 0){
                index = Operazioni.findIndex(o=>{
                    if(+ o.primaryKey.codice === enum_LAVCOD.DISERBO){
                        let disciplinare = this.qdcservice.getDisciplinareModelValue(enum_LAVCOD.DISERBO);

                        if(disciplinare && disciplinare.codice !== NessunDpiNessunaEtichetta && disciplinare.codice !== NessunDpi && disciplinare.codice !== DpiBio){
                            return true;
                        }
                    }

                    return false;
                });
            }

        }


        if(index > -1)
            mostra = true;

        return mostra;
    }

    MostraBtn_VerificaConformita(Operazioni: Array<Lavorazione>) {
        let mostra = false;

        let operazione = null;

        // attivare la verifica conformità su trattamenti, fertilizzazioni e raccolte, tranne quando il disciplinare è nessuno/nessuno
        // le raccolte sempre invece


        //Non visibile nelle Ricette e Brogliaccio
        if(this.qdcservice.TestataForm &&
            this.qdcservice.TestataForm.get("Tipo").value === Tipo_Attivita.QuadernoDiCampagna &&
            this.qdcservice.getCentroDiCostoTipo() != Tipo.ProdottoDaTrattare){

            if(Operazioni && Operazioni.length > 0){
                operazione = Operazioni.find((o:Lavorazione)=>{

                    let lav_cod = + o.primaryKey.codice;

                    let elem_cod = this.qdcservice.getCategoria_Magazzino(lav_cod);

                    let disciplinare = this.qdcservice.getDisciplinareModelValue(lav_cod);

                    if((elem_cod === FORMULATI || elem_cod === FERTILIZZANTI) && (disciplinare && disciplinare.codice !== NessunDpiNessunaEtichetta)){
                        return o;
                    }else if (elem_cod === TRASFORMATI_VEGETALI && lav_cod === enum_LAVCOD.RACCOLTA){
                        return o;
                    }
                });
            }

        }


        if(operazione && + operazione.primaryKey.codice > 0)
            mostra = true;

        return mostra;
    }

    MostraBtnTestata(Operazioni: Array<Lavorazione>){
        let mostra = false;

        if(this.MostraBtn_VerificaDoseConsigliataBS(Operazioni) || this.MostraBtn_VerificaDoseConsigliata(Operazioni) || this.MostraBtn_VerificaConformita(Operazioni)){
            mostra = true;
        }

        return mostra;
    }

}
