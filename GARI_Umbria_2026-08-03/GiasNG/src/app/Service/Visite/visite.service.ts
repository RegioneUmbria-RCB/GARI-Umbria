import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, lastValueFrom, map, of, throwError } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { Link_ElimOpMultipla } from 'app/menu-agenda/components/utils';
import { UtilizzoTerreno } from 'app/Model/metaschema/utilizzi/UtilizzoTerreno';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { elimina_operazione_multipla } from 'app/menu-agenda/components/grid-qdc/qdc-config.service';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { enum_ID_Area_Alert, enum_statiWorkflowQdC } from 'app/Model/TipiEnumerativi';
import { Utente } from 'app/Model/utente/utente';
import { ObjParametriAgendaService } from '../obj-parametri-agenda.service';
import {GiasIFrameWindowService, ObjParametriAgenda, rispostaStandard} from 'gias-ui-kit';
import { AgendaService } from '../Agenda/Agenda.service';
import { Attivita } from 'app/Model/attivita/Attivita';
import { Operatore } from 'app/Model/metaschema/utilizzi/Operatore';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { DestinazioneUso } from 'app/Model/metaschema/utilizzi/DestinazioneUso';
import { SpecieVegetaliService } from '../Metaschema/specie-vegetali.service';
import { OperatoreDDLService } from 'app/quaderno-di-campagna/agenda-edit/service/testata/operatore-ddl-service';
import { AziendaDDLService, LeggiAziende } from 'app/quaderno-di-campagna/agenda-edit/service/testata/azienda-ddl-service';
import { DialogBooleanResult, GiasDialogService, Dialog_Type } from '../gias-dialog.service';
import { TranslocoService } from '@jsverse/transloco';
import { DialogResult} from '@progress/kendo-angular-dialog';
import { AttivitaPersonalizzata } from 'app/Model/attivita/AttivitaPersonalizzata';
import { RisorsaZootecnica } from 'app/Model/attivita/risorse/RisorsaZootecnica';
import { DropdownListSpecieAnimali, SpecieAnimaliService } from 'app/quaderno-di-campagna/agenda-edit/service/testata/specie-animali-service';
import { CaricaDatiApp } from 'app/Service/api.service';
import { GestioneRichiesteService, KeyValuePair, ParametriAggiuntivi_QueryString } from '../gestione-richieste.service';
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from 'app/Model/siti.enum';

export class LeggiVisite {

    operatore: Utente;
    azienda: Impresa;
    specie: UtilizzoTerreno;
    centro_aziendale: CentroAziendale;
    data_da: Date;
    data_a:  Date;
    impianti: String[];
    operazioni: AttivitaPersonalizzata[];
    tipoVisita: enum_statiWorkflowQdC;
    risorsaZootecnica: RisorsaZootecnica;
    withDettaglioRilievo: boolean;

    constructor() {
        this.operatore = new Utente();
        this.azienda = new Impresa();
        this.specie = null;
        this.centro_aziendale = null;
        this.data_da = new Date();
        this.data_a = new Date();
        this.impianti = [];
        this.operazioni = [];
        this.tipoVisita = enum_statiWorkflowQdC.Non_Definito;
        this.risorsaZootecnica = null;
        this.withDettaglioRilievo = false;
    }
}


export class Elimina_Rilievi_Visita
{

    Piva: string;
    idAgendaVisita: number;
    idAgendaRilievo: number;

}


@Injectable({providedIn: 'root'})
export class VisiteService {

    copiaVisitaSub: BehaviorSubject<Attivita> = new BehaviorSubject(null);
    deleteRilievo: BehaviorSubject<Attivita> = new BehaviorSubject(null);

    objParametriAgenda: ObjParametriAgenda;

    public operatoriCopiaVisite: Operatore[] = [];
    public aziendeCopiaVisite: Impresa[] = [];
    public specieCopiaVisite: UtilizzoTerreno[] | Specie[] = [];
    public specieAnimaliCopiaVisite: DropdownListSpecieAnimali[] = [];

    constructor(private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
                private objParametriAgendaService: ObjParametriAgendaService,
                private agendaService: AgendaService,
                private specieService: SpecieVegetaliService,
                private operatoreService: OperatoreDDLService,
                private dialogService: GiasDialogService,
                private translocoService: TranslocoService,
                private gestioneRichieste: GestioneRichiesteService,
                private aziendaDDLService: AziendaDDLService,
                private windowService: GiasIFrameWindowService,
                private specieAnimaliService: SpecieAnimaliService) {

                this.objParametriAgenda=this.objParametriAgendaService.getObjParamValue();
    }

    public leggiVisite(p: LeggiVisite): Observable<string>{

        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiVisite, string>('Visite/MenuVisite/LeggiVisite',p).pipe(map(r =>{
            return r.RispostaStringa;
        }));

        return Obs;
    }

    public caricaDatiApp(p: CaricaDatiApp): Observable<rispostaStandard<string>>{

        const Obs = this.ajaxAgronicaAPIService.ajaxAPIPost<CaricaDatiApp, string>('GiasApp/CaricaDatiApp', p).pipe(map(r =>{
            return r;
        }));

        return Obs;
    }

    public deleteItem(params) {

        return this.ajaxAgronicaAPIService.ajaxAPIPost<elimina_operazione_multipla, string>(Link_ElimOpMultipla, params);
        // let ris = new RispostaStandard();
        // ris.RispostaOK = true;
        // return of(new RispostaStandard());
    }

    public copiaVisita(param: any) {
        (this.agendaService.LeggiListaAttivitaWS(param.Piva,
                                                param.Id_Agenda)).then(listaAttivita => {
                                                    this.copiaVisitaSub.next(listaAttivita[0]);
                                                });
    }

    public deleteRilievoVisita(param: Elimina_Rilievi_Visita) {
        //console.log(param);
        return this.ajaxAgronicaAPIService.ajaxAPIPost<Elimina_Rilievi_Visita, string>('Visite/EliminaRilieviVisite', param);
    }

    public async caricaDDLSpecieCopiaVisita() {
        this.specieCopiaVisite = await lastValueFrom(this.specieService.leggiTutteLeSpecieAPI());

        let destUso = new DestinazioneUso();
        destUso.codice = -2;
        destUso.descrizione = "Utilizzo Terreno";
        this.specieCopiaVisite.push(destUso);
    }

    public async caricaDDLSpecieAnimaleCopiaVisita() {
        this.specieAnimaliCopiaVisite = await lastValueFrom(this.specieAnimaliService.leggiListaSpecieAnimaliDDL());
        let risorsaZoodefault = new RisorsaZootecnica();
        this.specieAnimaliCopiaVisite.unshift(this.specieAnimaliService.setDefaultDDLSpecieAnimale(risorsaZoodefault as DropdownListSpecieAnimali, "TutteLeSpecie"));
    }

    public setDefaultDDLSpecieAnimale(risorsa: RisorsaZootecnica) {
        return this.specieAnimaliService.setCodDescrDDLSpecieAnimale(risorsa as DropdownListSpecieAnimali);
    }

    public async caricaDDLOperatoriCopiaVisita() {
        let result = await lastValueFrom(this.operatoreService.leggiListaTecnici());
        this.operatoriCopiaVisite = JSON.parse(JSON.stringify(result));
    }


    public async caricaDDLAgenzieAziendeCopiaVisita(usernameOperatoreCopia: string, username: string, piva: string) {

        //capisco se la piva associata alla visita è relativa a un'azienda o a un'agenzia
        let param: LeggiAziende = new LeggiAziende(usernameOperatoreCopia);

        let resArrAziende = await lastValueFrom(this.aziendaDDLService.leggiListaAziende(param));

        let arrAziende: Impresa[];

        arrAziende = JSON.parse(JSON.stringify(resArrAziende));

        if (arrAziende.length !== 0 && arrAziende.find(item => item.partitaIva === piva) !== undefined) {  //se l'azienda della vecchia è presente nel primo array

            if (usernameOperatoreCopia !== username) {
                let newParam: LeggiAziende = new LeggiAziende(username);
                let resNewArrAziende = await lastValueFrom(this.aziendaDDLService.leggiListaAziende(newParam));
                arrAziende = JSON.parse(JSON.stringify(resNewArrAziende));
            }
            // let aziendaCopia = arrAziende.find(item => item.partitaIva === piva);
            // let index = arrAziende.indexOf(aziendaCopia);
            // arrAziende.splice(index, 1);
            this.aziendeCopiaVisite = arrAziende;

        } else {

            let resArrAgenzie = await lastValueFrom(this.aziendaDDLService.leggiListaAgenzie(param));

            let arrAgenzie: Impresa[];

            arrAgenzie = JSON.parse(JSON.stringify(resArrAgenzie));

            if (arrAgenzie.length !== 0 && arrAgenzie.find(item => item.partitaIva === piva) !== undefined) {

                if (usernameOperatoreCopia !== username) {
                    let newParam: LeggiAziende = new LeggiAziende(username);
                    let resNewArrAgenzie = await lastValueFrom(this.aziendaDDLService.leggiListaAgenzie(newParam));
                    arrAgenzie = JSON.parse(JSON.stringify(resNewArrAgenzie));
                }
                // let agenziaCopia = arrAgenzie.find(item => item.partitaIva === piva);
                // let index = arrAgenzie.indexOf(agenziaCopia);
                // arrAgenzie.splice(index, 1);
                this.aziendeCopiaVisite = arrAgenzie;
            }
        }

    }

    createDialogWindow(title: string, content: string, preventAction: (p: DialogBooleanResult) => boolean = ()=>{return false}): Promise<DialogResult> {
        return new Promise((resolve, reject) => {
            let dialog = this.dialogService.dialogMessageRef(
                title,
                content,
                [
                    { text: this.translocoService.translate('Conferma'), primary: true, returnObj: true },
                    { text: this.translocoService.translate('Annulla'), returnObj: false }
                ], 'auto', 'auto',
                preventAction,
                Dialog_Type.info
            );

            if(typeof content == 'string')
                dialog.content.location.nativeElement.style.whiteSpace = 'pre-line';
            dialog.result.subscribe(res => resolve(res))
        })
    }

    ApriKendoWindowRicercaDocumenti(idAgenda: number, piva: string) {

        const parametri: ParametriAggiuntivi_QueryString[] = [
                                                                KeyValuePair.Create("type", "doc"),
                                                                // KeyValuePair.Create("area_provenienza", Id_Area.toString()),
                                                                KeyValuePair.Create("p", piva),
                                                                KeyValuePair.Create("area_provenienza", (enum_ID_Area_Alert.Operazioni_Campagna_QDC).toString()),
                                                                KeyValuePair.Create("id_agenda", idAgenda.toString())
                                                            ];

        this.gestioneRichieste.gestionePassaggioAltroSito(
                                                            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                            enum_PagineAgenda_2010.Pagina_Scadenzario_Lista,
                                                            parametri)
                              .then(link => {
                                            this.windowService.open({
                                                title: this.translocoService.translate("RicercaDocumenti"),
                                                content: link,
                                                height: window.innerHeight * 0.9,
                                                width: window.innerWidth * 0.9
                                            });
        });
    }

}
