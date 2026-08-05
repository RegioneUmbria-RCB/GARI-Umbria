import {Component, OnInit, Optional} from '@angular/core';
import { FormGroup, FormGroupDirective } from '@angular/forms';
import { Attivita } from 'app/Model/attivita/Attivita';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import {FORMULATI} from 'app/Model/CostantiPersonalizzate';
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from 'app/Model/siti.enum';
import {enum_LAVCOD} from 'app/Model/TipiEnumerativi';
import { GestioneRichiesteService, ParametriAggiuntivi_QueryString } from 'app/Service/gestione-richieste.service';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { QdCService } from '../../service/qdc.service';
import { QdCFormToAttivitaService } from '../../service/quaderno-di-campagna-form/quaderno-di-campagna-form-to-attivita.service';
import {
    MultiColumnComboboxDose_Etichetta,
    MultiColumnComboboxTrattamento
} from "../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {Gias2010Redirector} from "../../../../menu-agenda/components/grid-qdc/Gias2010Redirector.service";
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {QdCDettagliFormulatiService} from "../../service/prodotti/dettagli-formulati.service";
import {QdCTestataService} from "../../service/testata/testata.service";
import {RilevamentoDiMagazzino} from "../../../../Model/attivita/RilevamentoDiMagazzino";
import {CentroAziendale} from "../../../../Model/anagrafiche/CentroAziendale";
import {cloneDeep} from "lodash";
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
    standalone: false,
    selector: 'app-bottoni-sezione-prodotti',
    templateUrl: './bottoni-sezione-prodotti.component.html',
    styleUrls: ['./bottoni-sezione-prodotti.component.scss']
})
export class BottoniSezioneProdottiComponent implements OnInit {

    Operazione: Lavorazione;

    Categoria_Magazzino = 0;

    ProdottiForm: FormGroup;

    objParametriAgenda: ObjParametriAgenda;

    constructor(
        private qdcservice: QdCService,
        public parent: FormGroupDirective,
        private objParametriAgendaService: ObjParametriAgendaService,
        private gestioneRichiesteService: GestioneRichiesteService,
        private qdCFormToAttivitaService: QdCFormToAttivitaService,
        private masterService: MasterService,
        private gias2010redirectorservice:Gias2010Redirector,
        private testataservice: QdCTestataService,
        @Optional() private qdcdettagliformulatiservice: QdCDettagliFormulatiService
    ) { }

    ngOnInit(): void {
        this.ProdottiForm = <FormGroup> this.parent.form;

        this.Operazione = this.ProdottiForm.get("Operazione").value;

        this.Categoria_Magazzino = this.ProdottiForm.get("Categoria_Magazzino").value;
    }

    MostraBtnCaricoMagazzino(){

        let mostra = false;

        if(this.qdcservice.Scrittura_Carico_Magazzino &&
            this.qdcservice.getGestioneMagazzinoAbilitata(this.Categoria_Magazzino))
                mostra = true;

        return mostra;
    }

    MostraBtnGestioneMagazzini(){

        let mostra = false;

        if(this.qdcservice.getGestioneMagazzinoAbilitata(this.Categoria_Magazzino))
            mostra = true;

        return mostra;
    }

    MostraBtnProfitosan(){

        let mostra = false;

        //Bottone Profitosan visibile solo per i formulati

        if(this.Categoria_Magazzino === FORMULATI)
            mostra = true;

        return mostra;

    }

    // Redirect al profitosan
    Info() {

        if(this.Categoria_Magazzino === FORMULATI){
            let Formulato: MultiColumnComboboxTrattamento = this.ProdottiForm.get("Prodotto").value;

            if(Formulato && Formulato.prodotto.codice > 0){

                let pro_cod = Formulato.prodotto.codice;

                this.gestioneRichiesteService.getLinkProfitosan().then((TargetUrl:string)=>{
                    window.open(TargetUrl + pro_cod, 'profitosan', '');
                });

            }
        }

    }

    MostraBtnDDT_Ricevuto(){

        let mostra = false;

        if(this.qdcservice.Scrittura_DDT_Ricevuto &&
            this.qdcservice.getGestioneMagazzinoAbilitata(this.Categoria_Magazzino))
            mostra = true;

        return mostra;

    }

    // Redirect to GestioneMagazziniBS
    redirectToApriGestioneMagazzini() {

        //TODO Capire se va bene così il redirect

        if(!this.ProdottiForm.get("Prodotto").value || this.ProdottiForm.get("Prodotto").value.prodotto.codice === 0)
            return;

        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        const newobjParametriAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(this.objParametriAgenda));

        newobjParametriAgenda.Id_Agenda = 0;
        newobjParametriAgenda.Pagina_Provenienza = this.masterService.getCurrentPageAsValue();
        newobjParametriAgenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;

        let array_dosi: Array<MultiColumnComboboxDose_Etichetta> = [];

        if(this.ProdottiForm.get("Categoria_Magazzino").value === FORMULATI)
            array_dosi = this.qdcdettagliformulatiservice.Array_Dosi;

        const attivita: Attivita = cloneDeep(this.qdCFormToAttivitaService.mapQdCFormToListaAttivita(this.ProdottiForm,[], true,array_dosi)
                                                .find(a=> a.job.primaryKey.codice === this.Operazione.primaryKey.codice));

        newobjParametriAgenda.GenericObj_string = JSON.stringify(attivita);

        const parametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = [];

        this.gestioneRichiesteService.gestionePassaggioAltroSito_Aperto_in_Iframe(
            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
            enum_PagineAgenda_2010.Pagina_GestioneMagazziniBS,
            parametriAggiuntivi,
            newobjParametriAgenda,
            false,
            -1,
            'qdc.MovimentiMagazzino.Text',
            false).then();

    }

    ApriCaricoMagazzino(){

        let objParametriAgenda: ObjParametriAgenda = cloneDeep(this.objParametriAgendaService.getObjParamValue());

        objParametriAgenda.Sa_Cod = this.Ottieni_Sa_Cod_X_Redirect_DocContabile();

        objParametriAgenda.SaNome = "";

        objParametriAgenda.Lav_Cod = enum_LAVCOD.CARICO;

        objParametriAgenda.Pagina_Provenienza = this.masterService.getCurrentPageAsValue();

        objParametriAgenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;

        let data: Date = this.qdcservice.TestataForm.get("Data").value;

        let prodotto_cod: number = 0;

        if(this.ProdottiForm.get("Prodotto").value && this.ProdottiForm.get("Prodotto").value.prodotto.codice > 0)
            prodotto_cod = this.ProdottiForm.get("Prodotto").value.prodotto.codice;

        let ParametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = this.gias2010redirectorservice.docContabileParametri(objParametriAgenda.Piva,Enum_DBTypeOperation.Write,objParametriAgenda.Sa_Cod,
                                                                        0,objParametriAgenda.Lav_Cod,0,"","",data,prodotto_cod,1,"true");

        this.gestioneRichiesteService.gestionePassaggioAltroSito_Aperto_in_Iframe(
            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
            enum_PagineAgenda_2010.Pagina_DocContabile,
            ParametriAggiuntivi,
            objParametriAgenda,
            true,
            -1,
            'qdc.CaricoMagazzino.Text',
            false).then();
    }

    ApriNuovoDDTRicevuto(){

        let objParametriAgenda: ObjParametriAgenda = cloneDeep(this.objParametriAgendaService.getObjParamValue());

        objParametriAgenda.Sa_Cod = this.Ottieni_Sa_Cod_X_Redirect_DocContabile();

        objParametriAgenda.SaNome = "";

        objParametriAgenda.Lav_Cod = enum_LAVCOD.BOLLA_RICEVUTA;

        objParametriAgenda.Pagina_Provenienza = this.masterService.getCurrentPageAsValue();

        objParametriAgenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;

        let data: Date = this.qdcservice.TestataForm.get("Data").value;

        let prodotto_cod: number = 0;

        if(this.ProdottiForm.get("Prodotto").value && this.ProdottiForm.get("Prodotto").value.prodotto.codice > 0)
            prodotto_cod = this.ProdottiForm.get("Prodotto").value.prodotto.codice;

        let ParametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = this.gias2010redirectorservice.docContabileParametri(objParametriAgenda.Piva,Enum_DBTypeOperation.Write,objParametriAgenda.Sa_Cod,
            0,objParametriAgenda.Lav_Cod,0,"A","C",data,prodotto_cod,1,"true");

        this.gestioneRichiesteService.gestionePassaggioAltroSito_Aperto_in_Iframe(
            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
            enum_PagineAgenda_2010.Pagina_DocContabile,
            ParametriAggiuntivi,
            objParametriAgenda,
            true,
            -1,
            'qdc.NuovoDDTRicevuto',
            false).then();
    }

    MostraBtn_Sezione_Prodotti(){

        let mostra = false;

        if(this.MostraBtnProfitosan() || this.MostraBtnCaricoMagazzino() ||this.MostraBtnDDT_Ricevuto() || this.MostraBtnGestioneMagazzini()){
            mostra = true;
        }

        return mostra;
    }

    //@description:
    // 1. se scegli un prodotto da riga magazzino mandi il sa_cod di quello;
    // 2. se scegli da anagrafica ed esiste un solo centro nell'azienda mandi il sa_cod di quello;
    // 3. altrimenti mandi sa_cod=0
    Ottieni_Sa_Cod_X_Redirect_DocContabile(){

        let sa_cod: number = 0;

        if(this.ProdottiForm.get("Prodotto").value && this.ProdottiForm.get("Prodotto").value.prodotto.codice > 0){
            let magazziniMovimentazioni = this.ProdottiForm.get("Prodotto").value.MagazziniMovimentazioni as RilevamentoDiMagazzino[];

            if(magazziniMovimentazioni && magazziniMovimentazioni.length === 1){
                if(magazziniMovimentazioni[0].Magazzino && magazziniMovimentazioni[0].Magazzino.primaryKey.codice > 0){
                    sa_cod = magazziniMovimentazioni[0].Magazzino.primaryKey.centroAziendalePK.codice;
                }
            }

            if(sa_cod === 0){
                if(this.testataservice.Array_CentriAziendale){

                    let centri_aziendali_reali:CentroAziendale[] = this.testataservice.Array_CentriAziendale.filter(c=>c.primaryKey.codice !== this.qdcservice.Obj_TuttiICentriAziendali.primaryKey.codice);

                    if(centri_aziendali_reali && centri_aziendali_reali.length === 1){
                        sa_cod = centri_aziendali_reali[0].primaryKey.codice;
                    }
                }
            }
        }

        return sa_cod;
    }

}
