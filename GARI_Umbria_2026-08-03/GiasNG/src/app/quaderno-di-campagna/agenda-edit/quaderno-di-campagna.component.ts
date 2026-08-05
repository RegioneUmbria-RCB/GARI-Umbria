import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {TranslocoService} from '@jsverse/transloco';
import {MenuContestualeService} from 'app/Master/menu-contestuale/menu-contestuale.service';
import {Lavorazione} from 'app/Model/attivita/Lavorazione';
import {GestioneRichiesteService} from "app/Service/gestione-richieste.service";
import { ObjParametriAgendaService} from 'app/Service/obj-parametri-agenda.service';
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {Subscription, take} from 'rxjs';
import {CalcoloSuperficiService} from './service/calcolo-superfici.service';
import {GridDosiProdottiService} from './service/grid-dosi-prodotti/grid-dosi-prodotti.service';
import {GridImpiantiService} from './service/grid-impianti/grid-impianti.service';
import {NoteService} from './service/grid-note/note.service';
import {MisceleService} from './service/miscele.service';
import {QdCService} from './service/qdc.service';
import {
  QdCFormToAttivitaService
} from './service/quaderno-di-campagna-form/quaderno-di-campagna-form-to-attivita.service';
import {QdCFormService} from './service/quaderno-di-campagna-form/quaderno-di-campagna-form.service';
import {QdCTestataService} from './service/testata/testata.service';
import {QdCVisibilitaControlliTestataService} from './service/testata/visibilita-controlli-testata-service';
import {QdCControlliSalvataggioService} from "./service/qdc-controlli-salvataggio.service";
import {enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity, MasterService} from 'app/Service/master.service';
import {ActivatedRoute, Params} from '@angular/router';
import {Blocco, Tipo_Blocco} from "../../Model/attivita/Blocco";
import {Dialog_Type} from "../../Service/gias-dialog.service";
import { Tipo } from 'app/Model/attivita/centri_di_costo/CentroDiCosto';
import { ObjParametriAgenda, Stati, Tipo_Attivita } from 'gias-ui-kit';

//Provide In nel componente padre non nel modulo perchè causava dei problemi
@Component({
    standalone: false,
    selector: 'app-quaderno-di-campagna',
    templateUrl: './quaderno-di-campagna.component.html',
    styleUrls: ['./quaderno-di-campagna.component.scss'],
    providers:[
        QdCFormService,
        QdCFormToAttivitaService,
        QdCService,
        QdCTestataService,
        GridImpiantiService,
        GridDosiProdottiService,
        QdCVisibilitaControlliTestataService,
        NoteService,
        MisceleService,
        CalcoloSuperficiService,
        QdCControlliSalvataggioService
    ]
})
export class QuadernoDiCampagnaComponent implements OnInit, OnDestroy {

    Subs: Subscription = new Subscription();

    objParametriAgenda: ObjParametriAgenda;
    @Input() inWindow: boolean = false;

    constructor(private objParametriAgendaService: ObjParametriAgendaService,
        private qdcformservice: QdCFormService,
        public qdcservice: QdCService,
        private qdcformtoattivitaservice: QdCFormToAttivitaService,
        private menuContestualeService: MenuContestualeService,
        private translocoService: TranslocoService,
        private masterService: MasterService,
        private gestioneRichiesteService: GestioneRichiesteService,
        public qdcvisibilitacontrollitestata: QdCVisibilitaControlliTestataService,
        private route: ActivatedRoute) {
    }


    ngOnInit(): void {

        try {

            if(this.gestioneRichiesteService.ObjPageToMemorize &&
                this.gestioneRichiesteService.ObjPageToMemorize.QdCFormValue && this.gestioneRichiesteService.ObjPageToMemorize.objParametriAgenda){

                this.objParametriAgendaService.changeObjParametriAgenda(this.gestioneRichiesteService.ObjPageToMemorize.objParametriAgenda);

            }


            this.Subs.add(this.route.queryParams.pipe(take(1)).subscribe((R: Params)=>{
                let seFrame = R.seFrame;
                if (R.ParametriAggiuntivi) {
                    let parameriAggiuntivi = JSON.parse(R.ParametriAggiuntivi);
                    if (parameriAggiuntivi.find(p => p.key === 'seFrame')) {
                        seFrame = +parameriAggiuntivi.find(p => p.key === 'seFrame')?.value;
                    }
                    if (parameriAggiuntivi.find(p => p.key === 'ObjPageToMemorize')) {
                        this.gestioneRichiesteService.ObjPageToMemorize = JSON.parse(
                            parameriAggiuntivi.find(p => p.key === 'ObjPageToMemorize').value
                        )
                        this.objParametriAgendaService.changeObjParametriAgenda(
                            this.gestioneRichiesteService.ObjPageToMemorize.objParametriAgenda
                        );
                    }
                }
                if (seFrame == 1){

                    this.qdcservice.inFrame = true;

                    let header = this.masterService.getHeader();
                    header.visible = true;
                    header.BackButton = false;
                    header.ColumnLeft = false;
                    header.ColumnRight = false;
                    this.masterService.changeHeader(header);

                    let footer = this.masterService.getFooter();
                    footer.visible = false;
                    this.masterService.changeFooter(footer);

                    let menuContestuale = this.menuContestualeService.getMenuContestualeSettings();
                    menuContestuale.show = false;
                    this.menuContestualeService.changeMenuContestualeSettings(menuContestuale);

                }

                if(R.t_r && +R.t_r > 0) {
                  this.qdcservice.TipoRibaltamento = +R.t_r;
                }

                if(R.f_r && + R.f_r === 1){
                  this.qdcservice.flag_Reinnesco = true;
                }

                this.qdcservice.inWindow = this.inWindow;

                this.Subs.add(this.qdcformservice.getQdCFormValue().pipe(take(1)).subscribe(qdCFormModel => {

                    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

                    this.qdcservice.QdCForm.patchValue(qdCFormModel);

                    this.qdcservice.QdCForm.markAllAsTouched();

                    this.disabilitaQdCForm();

                    this.qdcservice.Inizializza_QdC().then(async (r)=>{
                        if(r){
                            this.qdcservice.getUtente(this.qdcservice.TestataForm.get("Operazioni").getRawValue());

                            this.qdcservice.setImpostazione_Avversita_Prima_Dei_Prodotti(this.qdcservice.TestataForm.get("Operazioni").getRawValue());

                            this.qdcservice.GestisciValidatorImpiantiSuperficie(this.qdcservice.TestataForm.get("Operazioni").getRawValue());

                            //Carico le Macchine e gli operatori con i default della Profilazione quando ho i dati impostati in seguito
                            //al Salva e Nuovo oppure se arrivo dall'Anagrafica
                            if(this.gestioneRichiesteService.ObjPageToMemorize &&
                                this.gestioneRichiesteService.ObjPageToMemorize.objParametriAgenda &&
                                this.gestioneRichiesteService.ObjPageToMemorize.QdCFormValue){

                                await Promise.all([
                                    this.qdcservice.RicaricaGridMacchine(),
                                    this.qdcservice.RicaricaGridOperatori()
                                ]);

                            }

                            this.gestioneRichiesteService.ObjPageToMemorize = null;

                            this.qdcservice.Mostra_QdC = true;
                            if (this.qdcservice.getCentroDiCostoTipo() == Tipo.ProdottoDaTrattare) {
                                this.qdcservice.Mostra_Sezioni_Prodotto = true;
                            }
                        }
                    });

                    this.ImpostaMenuContestuale(this.qdcservice.TestataForm?.get("Operazioni").getRawValue());

                    this.MostraMsgOperazioneBloccata();
                }));
            }));

            this.Subs.add(this.qdcservice.TestataForm?.get("Operazioni").valueChanges.subscribe((op: Lavorazione[])=>{
                this.ImpostaMenuContestuale(op);
            }));

        } catch (e) {
            console.log('Error:', e);
        }
    }

    ngOnDestroy(): void {

        this.Subs.unsubscribe();

        //Imposto a null il QdCForm nel qdcservice
        this.qdcservice.QdCForm = null;

        //Resetto la listaAttivita
        this.qdcservice.setListaAttivita_Con_Parametri_Aggiuntivi([]);

    }


    ImpostaMenuContestuale(Operazioni: Lavorazione[]){

        let titolo = this.translocoService.translate('OperazioniColturali') + " (";

        switch(this.objParametriAgenda?.TipoOperazioneAgenda){
            case Tipo_Attivita.QuadernoDiCampagna:
                break;
            case Tipo_Attivita.Ricetta:
                if(this.objParametriAgenda.Stato === Stati.Da_Eseguire){
                    titolo += this.translocoService.translate('RicettaOdL') + " - ";
                }else if(this.objParametriAgenda.Stato === Stati.Eseguita){
                    titolo += this.translocoService.translate('Brogliaccio') + " - ";
                }
                break;
        }

        switch(this.objParametriAgenda?.TipoOperazioneDB){
            case Enum_DBTypeOperation.Read:
                titolo += this.translocoService.translate('Info');
                break;
            case Enum_DBTypeOperation.Update:
                titolo += this.translocoService.translate('Modifica');
                break;
            case Enum_DBTypeOperation.Write:
                titolo += this.translocoService.translate('Nuovo');
                break;
        }

        if(Operazioni && Operazioni.length > 0){
            let strOperazioni = Operazioni.map(o=> o.descrizione).join(",");

            if(strOperazioni && strOperazioni !== ""){
                titolo += " "+ strOperazioni;
            }
        }

        titolo+=")";

        this.menuContestualeService.changeMenuContestualeSettings({
            show: true,
            background_color: "#92D050",
            color: 'white',
            title: titolo,
            search: false,
            bookmarks: true,
            contextualMenu: true,
            IDTipoSezione: 2,
            IDSezionePadre: 14
        });
    }


    disabilitaQdCForm(){

        if(this.qdcservice.Sola_Lettura_QdCForm()){

            this.qdcservice.QdCForm.disable({emitEvent: false});

            this.qdcservice.abilitaGrid = false;
        }else{
            this.qdcservice.abilitaGrid = true;
        }
    }

    MostraQdC(){

        let mostra = false;

        //let Operazioni = this.qdcservice.TestataForm?.get("Operazioni")?.value;
        // && Operazioni && Operazioni.length > 0
        if(this.qdcservice.Mostra_QdC )
            mostra = true;

        return mostra;
    }

    MostraMsgOperazioneBloccata(){

      let msg:string = "";

      let blocco: Blocco = this.qdcservice.TestataForm.get("Blocco_Attivita").getRawValue();

      if(this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Update &&
        blocco && blocco.tipo === Tipo_Blocco.QuadernoDiCampagna){

        let Utente: string = blocco.utente;

        let Data: string = this.qdcservice.datepipe.transform(blocco.data,'dd/MM/yyyy',undefined,this.qdcservice.locale_id);

        msg = this.qdcservice.translocoService.translate("qdc.MsgOperazioneBloccata",{Utente: Utente, Data: Data});
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

    isPostRaccolta(): boolean {
        return this.qdcservice.getCentroDiCostoTipo() == Tipo.ProdottoDaTrattare;
    }
}
