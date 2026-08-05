import {AfterViewInit, Component, ViewChild} from "@angular/core";
import {Lavorazione} from "app/Model/attivita/Lavorazione";
import {FERTILIZZANTI, FORMULATI, INSETTI, SEMENTI, TRASFORMATI_VEGETALI} from "app/Model/CostantiPersonalizzate";
import {enum_LAVCOD} from "app/Model/TipiEnumerativi";
import {
    enum_Problema_DettaglioProdotto, Key_Parametri_Aggiuntivi,
    Sezione_Prodotto_Fertilizzanti,
    Sezione_Prodotto_Formulati,
    Sezione_Prodotto_Raccolta,
    Sezione_Prodotto_Sementi
} from "../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {QdCService} from "../../service/qdc.service";
import {GiasPanelBar} from 'gias-ui-kit';
import {enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity} from "../../../../Service/master.service";
import {Dialog_Type} from "../../../../Service/gias-dialog.service";
import {RibaltamentoTypes} from "../../../../menu-agenda/components/utils";
import {DettaglioTrattamento} from "../../../../Model/attivita/dettagli/DettaglioTrattamento";
import { Tipo } from "app/Model/attivita/centri_di_costo/CentroDiCosto";
import {DettaglioFertilizzazione} from "../../../../Model/attivita/dettagli/DettaglioFertilizzazione";

@Component({
    standalone: false,
    selector: "app-prodotti",
    templateUrl: "./prodotti.component.html",
    styleUrls: ["./prodotti.component.css"]
})
export class ProdottiComponent implements AfterViewInit {

    public Formulati = FORMULATI;
    public Fertilizzanti = FERTILIZZANTI;
    public Sementi = SEMENTI;
    public Trasformati_Vegetali = TRASFORMATI_VEGETALI;
    public Raccolta = enum_LAVCOD.RACCOLTA;
    public Insetti = INSETTI;

    @ViewChild("Sezione_Prodotti_PanelBar")Sezione_Prodotti_PanelBar: GiasPanelBar;

    constructor(
        public qdcservice: QdCService
    ) {}
    ngAfterViewInit() {
        this.Mostra_Msg_Conferma_Prodotti();
    }

    /*
    * @description:
    * Messaggio di riepilogo per confermare i prodotti non salvati (perchè ci sono degli errori o
    * durante il ribaltamento di una ricetta da APP/DEMETRA)
    * */
    Mostra_Msg_Conferma_Prodotti(){

        if(this.qdcservice.CheckDosiProdottiNonSalvate()){

            let Sezioni_Prodotto: Array<Sezione_Prodotto_Formulati | Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Sementi> = [];

            if(this.qdcservice.Sezioni_ProdottoFormArray){
              Sezioni_Prodotto = this.qdcservice.Sezioni_ProdottoFormArray.getRawValue();
            }

            if(Sezioni_Prodotto && Sezioni_Prodotto.length > 0){

                let Dosi_Prodotti_Non_Salvate: Array<string> = [];
                let Dosi_Prodotti_Con_Prodotti_Non_Corretti: Array<string> = [];
                let Dosi_Prodotti_Con_Prodotti_Ambigui: Array<string> = [];
                let Dosi_Prodotti_Con_Avversita_Non_Corrette: Array<string> = [];
                let Dosi_Prodotti_Con_Avversita_Ambigue: Array<string> = [];
                let Dosi_Prodotti_Con_Avversita_Non_Valorizzate: Array<string> = [];

              Sezioni_Prodotto.forEach(s=>{
                    s.DosiProdotti.forEach(d=>{

                        if(!d.Tutti_Problemi_DettaglioProdotto || d.Tutti_Problemi_DettaglioProdotto.length === 0){
                          if(d.Prodotto && d.Prodotto.prodotto && d.Prodotto.prodotto.descrizione !== ""){
                            let str: string = "</br>- "+ d.Prodotto.prodotto.descrizione;

                            if(!Dosi_Prodotti_Non_Salvate.includes(str) && d.Riga_Salvata === false){
                              Dosi_Prodotti_Non_Salvate.push(str);
                            }
                          }
                        }else if(d.Tutti_Problemi_DettaglioProdotto && d.Tutti_Problemi_DettaglioProdotto.length > 0){

                            d.Tutti_Problemi_DettaglioProdotto.forEach(Problema_DettaglioProdotto=>{

                                let listaAttivita = this.qdcservice.getListaAttivita();

                                let str = "";

                                switch(Problema_DettaglioProdotto){
                                    case enum_Problema_DettaglioProdotto.Formulato_Non_Corretto:
                                    case enum_Problema_DettaglioProdotto.Fertilizzante_Non_Corretto:

                                        if(listaAttivita){
                                            listaAttivita.forEach(a=>{
                                                if(a.parametri_aggiuntivi && a.parametri_aggiuntivi.length > 0){

                                                    let parametri_aggiuntivi_con_prodotti_non_trovati = a.parametri_aggiuntivi.filter(x=>x.key === Key_Parametri_Aggiuntivi.Formulati_Non_Corretti ||
                                                                                                                                                                                        x.key === Key_Parametri_Aggiuntivi.Fertilizzanti_Non_Corretti);

                                                    if(parametri_aggiuntivi_con_prodotti_non_trovati && parametri_aggiuntivi_con_prodotti_non_trovati.length > 0){

                                                        parametri_aggiuntivi_con_prodotti_non_trovati.forEach(p=>{
                                                            if(p.value && p.value !== ""){
                                                                let Prodotti_Non_Corretti: Array<DettaglioTrattamento | DettaglioFertilizzazione> = JSON.parse(p.value);

                                                                Prodotti_Non_Corretti.forEach(p=>{

                                                                    str = p.prodotto.descrizione;

                                                                    if(!Dosi_Prodotti_Con_Prodotti_Non_Corretti.includes(str) && d.Riga_Salvata === false){
                                                                        Dosi_Prodotti_Con_Prodotti_Non_Corretti.push(str);
                                                                    }
                                                                });

                                                            }
                                                        })


                                                    }
                                                }
                                            })
                                        }

                                        break;

                                    case enum_Problema_DettaglioProdotto.Formulato_Ambiguo:
                                    case enum_Problema_DettaglioProdotto.Fertilizzante_Ambiguo:
                                        str = d.Prodotto.prodotto.descrizione;

                                        if(!Dosi_Prodotti_Con_Prodotti_Ambigui.includes(str) && d.Riga_Salvata === false){
                                            Dosi_Prodotti_Con_Prodotti_Ambigui.push(str);
                                        }

                                        break;
                                    case enum_Problema_DettaglioProdotto.Avversita_Non_Corretta:

                                        if(listaAttivita){
                                            listaAttivita.forEach(a=>{
                                                if(a.parametri_aggiuntivi && a.parametri_aggiuntivi.length > 0){
                                                    let parametri_aggiuntivi_con_prodotti_non_corretti = a.parametri_aggiuntivi.filter(x=>x.key === Key_Parametri_Aggiuntivi.Avversita_Non_Corrette);

                                                    if(parametri_aggiuntivi_con_prodotti_non_corretti && parametri_aggiuntivi_con_prodotti_non_corretti.length > 0){

                                                        parametri_aggiuntivi_con_prodotti_non_corretti.forEach(p=>{
                                                            if(p.value && p.value !== ""){
                                                                let Prodotti_Non_Corretti: Array<DettaglioTrattamento> = JSON.parse(p.value);

                                                                Prodotti_Non_Corretti.forEach(p=>{

                                                                    str = p.avversitaGruppo.descrizione;

                                                                    if(!Dosi_Prodotti_Con_Avversita_Non_Corrette.includes(str) && d.Riga_Salvata === false){
                                                                        Dosi_Prodotti_Con_Avversita_Non_Corrette.push(str);
                                                                    }
                                                                });


                                                            }
                                                        })


                                                    }
                                                }
                                            })
                                        }


                                        break;
                                    case enum_Problema_DettaglioProdotto.Avversita_Ambigua:

                                        str = d.Avversita.descrizione;

                                        if(!Dosi_Prodotti_Con_Avversita_Ambigue.includes(str) && d.Riga_Salvata === false){
                                            Dosi_Prodotti_Con_Avversita_Ambigue.push(str);
                                        }
                                        break;
                                    case enum_Problema_DettaglioProdotto.Avversita_Non_Valorizzata:
                                        if(listaAttivita){
                                            listaAttivita.forEach(a=>{
                                                if(a.parametri_aggiuntivi && a.parametri_aggiuntivi.length > 0){
                                                    let parametri_aggiuntivi_con_avversita_non_valorizzata = a.parametri_aggiuntivi.filter(x=>x.key === Key_Parametri_Aggiuntivi.Avversita_Non_Valorizzate);

                                                    if(parametri_aggiuntivi_con_avversita_non_valorizzata && parametri_aggiuntivi_con_avversita_non_valorizzata.length > 0){

                                                        parametri_aggiuntivi_con_avversita_non_valorizzata.forEach(p=>{
                                                            if(p.value && p.value !== ""){
                                                                let Prodotti_Non_Corretti: Array<DettaglioTrattamento> = JSON.parse(p.value);

                                                                Prodotti_Non_Corretti.forEach(p=>{

                                                                    str = p.prodotto.descrizione;

                                                                    if(!Dosi_Prodotti_Con_Avversita_Non_Valorizzate.includes(str) && d.Riga_Salvata === false){
                                                                        Dosi_Prodotti_Con_Avversita_Non_Valorizzate.push(str);
                                                                    }
                                                                });


                                                            }
                                                        })


                                                    }
                                                }
                                            })
                                        }
                                        break;
                                }
                            });


                        }

                    });
                });

                let origine: string = this.qdcservice.TestataForm.get("Origine").getRawValue();

                let msg:string = "";

                if(Dosi_Prodotti_Non_Salvate && Dosi_Prodotti_Non_Salvate.length > 0 && origine !== ""){
                    if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_ad_Agenda){
                        msg = this.qdcservice.translocoService.translate("qdc.MsgRibaltareRicettadaAPPInQdC",{Origine: origine, ProdottiList: Dosi_Prodotti_Non_Salvate.join(" ")});
                    }else if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda){
                        msg = this.qdcservice.translocoService.translate("qdc.MsgRibaltareAttivitadaAPPInQdC",{Origine: origine, ProdottiList: Dosi_Prodotti_Non_Salvate.join(" ")});
                    }
                }

                if(Dosi_Prodotti_Con_Prodotti_Non_Corretti && Dosi_Prodotti_Con_Prodotti_Non_Corretti.length > 0){

                    if(msg !== "")
                        msg += "<br>";

                    if(Dosi_Prodotti_Con_Prodotti_Non_Corretti.length === 1){

                      if(Dosi_Prodotti_Con_Prodotti_Non_Corretti[0] && Dosi_Prodotti_Con_Prodotti_Non_Corretti[0] !== ""){
                        if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_ad_Agenda && origine !== ""){
                          msg += this.qdcservice.translocoService.translate("qdc.ProdottoNonCompatibiledaAPPRicetta",{Origine: origine, Prodotto: Dosi_Prodotti_Con_Prodotti_Non_Corretti.join(",")});
                        }else if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda && origine !== ""){
                          msg += this.qdcservice.translocoService.translate("qdc.ProdottoNonCompatibiledaAPPAttivita",{Origine: origine, Prodotto: Dosi_Prodotti_Con_Prodotti_Non_Corretti.join(",")});
                        }else{
                          msg += this.qdcservice.translocoService.translate("qdc.ProdottoNonCompatibile",{Prodotto: Dosi_Prodotti_Con_Prodotti_Non_Corretti.join(",")});
                        }
                      }else{
                        if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_ad_Agenda && origine !== ""){
                          msg += this.qdcservice.translocoService.translate("ProdottoNonTrovatodaAPPRicetta",{Origine: origine});
                        }else if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda && origine !== ""){
                          msg += this.qdcservice.translocoService.translate("ProdottoNonTrovatodaAPPAttivita",{Origine: origine});
                        }else{
                          msg += this.qdcservice.translocoService.translate("ProdottoNonTrovatoAttivita");
                        }
                      }


                    }else{

                      if(Dosi_Prodotti_Con_Prodotti_Non_Corretti.filter(d=>d && d !== "").length === Dosi_Prodotti_Con_Prodotti_Non_Corretti.length){
                        if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_ad_Agenda && origine !== ""){
                          msg += this.qdcservice.translocoService.translate("qdc.ProdottiNonCompatibilidaAPPRicetta",{Origine: origine, ProdottiList: Dosi_Prodotti_Con_Prodotti_Non_Corretti.join(",")});
                        }else if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda && origine !== ""){
                          msg += this.qdcservice.translocoService.translate("qdc.ProdottiNonCompatibilidaAPPAttivita",{Origine: origine, ProdottiList: Dosi_Prodotti_Con_Prodotti_Non_Corretti.join(",")});
                        }else{
                          msg += this.qdcservice.translocoService.translate("qdc.ProdottiNonCompatibili",{Prodotto: Dosi_Prodotti_Con_Prodotti_Non_Corretti.join(",")});
                        }
                      }else{
                        if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_ad_Agenda && origine !== ""){
                          msg += this.qdcservice.translocoService.translate("ProdottiNonTrovatidaAPPRicetta",{Origine: origine});
                        }else if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda && origine !== ""){
                          msg += this.qdcservice.translocoService.translate("ProdottiNonTrovatidaAPPAttivita",{Origine: origine});
                        }else{
                          msg += this.qdcservice.translocoService.translate("ProdottiNonTrovatiAttivita");
                        }
                      }


                    }

                }

                if(Dosi_Prodotti_Con_Prodotti_Ambigui && Dosi_Prodotti_Con_Prodotti_Ambigui.length > 0){

                    if(msg !== "")
                        msg += "<br>";

                    if(Dosi_Prodotti_Con_Prodotti_Ambigui.length === 1){

                      if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_ad_Agenda && origine !== ""){
                        msg += this.qdcservice.translocoService.translate("qdc.ProdottoAmbiguodaAPPRicetta",{Origine: origine,Prodotto: Dosi_Prodotti_Con_Prodotti_Ambigui.join(",")});
                      }else if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda && origine !== ""){
                        msg += this.qdcservice.translocoService.translate("qdc.ProdottoAmbiguodaAPPAttivita",{Origine: origine, Prodotto: Dosi_Prodotti_Con_Prodotti_Ambigui.join(",")});
                      }else{
                        msg += this.qdcservice.translocoService.translate("qdc.ProdottoAmbiguo",{Prodotto: Dosi_Prodotti_Con_Prodotti_Ambigui.join(",")});
                      }
                    }else{

                      if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_ad_Agenda && origine !== ""){
                        msg += this.qdcservice.translocoService.translate("qdc.ProdottiAmbiguidaAPPRicetta",{Origine: origine, ProdottiList: Dosi_Prodotti_Con_Prodotti_Ambigui.join(",")});
                      }else if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda && origine !== ""){
                        msg += this.qdcservice.translocoService.translate("qdc.ProdottiAmbiguidaAPPAttivita",{Origine: origine, ProdottiList: Dosi_Prodotti_Con_Prodotti_Ambigui.join(",")});
                      }else{
                        msg += this.qdcservice.translocoService.translate("qdc.ProdottiAmbigui",{ProdottiList: Dosi_Prodotti_Con_Prodotti_Ambigui.join(",")});
                      }
                    }

                }

                if(Dosi_Prodotti_Con_Avversita_Non_Corrette && Dosi_Prodotti_Con_Avversita_Non_Corrette.length > 0){

                    if(msg !== "")
                        msg += "<br>";


                    if(Dosi_Prodotti_Con_Avversita_Non_Corrette.length === 1){

                      if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_ad_Agenda && origine !== ""){
                        msg += this.qdcservice.translocoService.translate("qdc.AvversitaNonCompatibiledaAPPRicetta",{Origine: origine, Avversita: Dosi_Prodotti_Con_Avversita_Non_Corrette.join(",")});
                      }else if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda && origine !== ""){
                        msg += this.qdcservice.translocoService.translate("qdc.AvversitaNonCompatibiledaAPPAttivita",{Origine: origine, Avversita: Dosi_Prodotti_Con_Avversita_Non_Corrette.join(",")});
                      }else{
                        msg += this.qdcservice.translocoService.translate("qdc.AvversitaNonCompatibile",{Avversita: Dosi_Prodotti_Con_Avversita_Non_Corrette.join(",")});
                      }

                    }else{

                      if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_ad_Agenda && origine !== ""){
                        msg += this.qdcservice.translocoService.translate("qdc.AvversitaNonCompatibilidaAPPRicetta",{Origine: origine, AvversitaList: Dosi_Prodotti_Con_Avversita_Non_Corrette.join(",")});
                      }else if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda && origine !== ""){
                        msg += this.qdcservice.translocoService.translate("qdc.AvversitaNonCompatibilidaAPPAttivita",{Origine: origine, AvversitaList: Dosi_Prodotti_Con_Avversita_Non_Corrette.join(",")});
                      }else{
                        msg += this.qdcservice.translocoService.translate("qdc.AvversitaNonCompatibili",{AvversitaList: Dosi_Prodotti_Con_Avversita_Non_Corrette.join(",")});
                      }
                    }

                }

                if(Dosi_Prodotti_Con_Avversita_Ambigue && Dosi_Prodotti_Con_Avversita_Ambigue.length > 0){
                    if(msg !== "")
                        msg += "<br>";

                    if(Dosi_Prodotti_Con_Avversita_Ambigue.length === 1){

                      if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_ad_Agenda && origine !== ""){
                        msg += this.qdcservice.translocoService.translate("qdc.AvversitaAmbiguadaAPPRicetta",{Origine: origine,Avversita: Dosi_Prodotti_Con_Avversita_Ambigue.join(",")});
                      }else if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda && origine !== ""){
                        msg += this.qdcservice.translocoService.translate("qdc.AvversitaAmbiguadaAPPAttivita",{Origine: origine, Avversita: Dosi_Prodotti_Con_Avversita_Ambigue.join(",")});
                      }else{
                        msg += this.qdcservice.translocoService.translate("qdc.AvversitaAmbigua",{Avversita: Dosi_Prodotti_Con_Avversita_Ambigue.join(",")});
                      }
                    }else{

                      if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Ricetta_ad_Agenda && origine !== ""){
                        msg += this.qdcservice.translocoService.translate("qdc.AvversitaAmbiguedaAPPRicetta",{Origine: origine,AvversitaList: Dosi_Prodotti_Con_Avversita_Ambigue.join(",")});
                      }else if(this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda && origine !== ""){
                        msg += this.qdcservice.translocoService.translate("qdc.AvversitaAmbiguedaAPPAttivita",{Origine: origine, AvversitaList: Dosi_Prodotti_Con_Avversita_Ambigue.join(",")});
                      }else{
                        msg += this.qdcservice.translocoService.translate("qdc.AvversitaAmbigue",{AvversitaList: Dosi_Prodotti_Con_Avversita_Non_Corrette.join(",")});
                      }
                    }
                }

                if(Dosi_Prodotti_Con_Avversita_Non_Valorizzate && Dosi_Prodotti_Con_Avversita_Non_Valorizzate.length > 0){
                    if(msg !== "")
                        msg += "<br>";

                    if(Dosi_Prodotti_Con_Avversita_Non_Valorizzate.length === 1){
                        msg += this.qdcservice.translocoService.translate("qdc.PerilSeguenteProdottoNonèStataIndicataAvversita",{Prodotto: Dosi_Prodotti_Con_Avversita_Non_Valorizzate.join(",")});
                    }else{
                        msg += this.qdcservice.translocoService.translate("qdc.PeriSeguentiProdottiNonèStataIndicataAvversita",{ProdottiList: Dosi_Prodotti_Con_Avversita_Non_Valorizzate.join(",")});
                    }

                }

                if(msg !== ""){
                    let listerroriGias: ErroreGias[]= [];

                    listerroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.WarningBloccante,
                        tipo:  enum_ErroreGias_Tipo.Generico,
                        messaggio: msg
                    });

                    this.qdcservice.gestisci_ErroriGias(listerroriGias,false,false,"",Dialog_Type.info).then();
                }

            }
        }

    }

    mostraProdotti() {
        let mostra: boolean = false;

        //Mostro la sezione dei Prodotti solo se l'Operazione è un Trattamento,
        //Fertilizzazione, Semina ,Raccolta e ci sono degli impianti selezionati

        if(this.qdcservice.QdCForm) {
            let Operazioni: Lavorazione[] = this.qdcservice.TestataForm.get("Operazioni").value;

            if(Operazioni && Operazioni.findIndex(o=>
                this.qdcservice.getCategoria_Magazzino(+ o.primaryKey.codice) === FORMULATI ||
                this.qdcservice.getCategoria_Magazzino(+ o.primaryKey.codice) === FERTILIZZANTI ||
                this.qdcservice.getCategoria_Magazzino(+ o.primaryKey.codice) === SEMENTI ||
                this.qdcservice.getCategoria_Magazzino(+ o.primaryKey.codice) === INSETTI ||
                (this.qdcservice.getCategoria_Magazzino(+ o.primaryKey.codice) === TRASFORMATI_VEGETALI) &&
                + o.primaryKey.codice === enum_LAVCOD.RACCOLTA
            ) > -1) {

                mostra = true;
            }
        }

        return mostra;
    }

    public Componi_Descrizione_Sezione(
        sezione: Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Formulati | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta
    ): string {
        let descr: string = "";
        descr = sezione.Operazione.descrizione;
        return descr;
    }

    public Disabilita_Sezione_Prodotti(): boolean{

        //Se sono in post raccolta e ci sono prodotti da trattare selezionati (o se è già aperto) abilito il panel bar
        if ((this.qdcservice.getCentroDiCostoTipo() == Tipo.ProdottoDaTrattare && this.qdcservice.ProdottiDaTrattareSelezionatiFormArray?.length > 0) ||
        this.Sezione_Prodotti_PanelBar?.IsExpanded.getValue()){
            return false;
        }

        //Se non ci sono impianti selezionati disabilito il panel bar se per caso il caso il panel bar invece è già aperto non lo disabilito
        if (this.qdcservice.getCentroDiCostoTipo() != Tipo.ProdottoDaTrattare && ((this.qdcservice.ImpiantiSelezionatiFormArray && this.qdcservice.ImpiantiSelezionatiFormArray.length > 0) ||
            this.Sezione_Prodotti_PanelBar?.IsExpanded.getValue())){
            return false;
        }

        return true;
    }

    public Espandi_Sezione_Prodotti(): boolean{
        let espandi = false;

        if(!this.Disabilita_Sezione_Prodotti()) {
            espandi = true;
        }

        return espandi;
    }
}
