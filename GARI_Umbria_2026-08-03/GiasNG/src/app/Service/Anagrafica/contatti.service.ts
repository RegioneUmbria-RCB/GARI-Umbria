import { Injectable } from '@angular/core';
import { Contatto } from 'app/Model/anagrafiche/Contatto';
import { Fabbricato } from 'app/Model/anagrafiche/Fabbricato';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { FiltroImpresa } from 'app/Model/filtri/FiltroImpresa';
import { BehaviorSubject, map, Observable, of } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, rispostaStandard, RispostaStandard } from '../master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { ContattiFactoryService } from '../ServiceFactory/contatti.factory.service';
import { UtenteFlatModel } from 'app/profilazione/models/utente.model';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { Campo } from '../../Model/anagrafiche/Campo';
import { AnagraficaNGClient, LeggiRapportoSpecifico } from '../api.service';
import { Leggi_Impostazioni } from 'app/profilazione/services/impostazioni/impostazioni-utenti.service';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';
import { Indirizzo } from 'app/Model/anagrafiche/addresses/Indirizzo';



@Injectable({
    providedIn: 'root'
})
export class ContattiService extends ContattiFactoryService {
    
    utenteDaAssociare: UtenteFlatModel = new UtenteFlatModel();

    utenteDaAssociareSource = new BehaviorSubject(this.utenteDaAssociare);

    windowAssociaUtenteRef: WindowRef = new WindowRef();

    windowAssociaUtenteRefSource = new BehaviorSubject(this.windowAssociaUtenteRef);

    constructor(
        protected ajaxAgronicaService: AjaxAgronicaService,
        protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        protected masterService: MasterService,
        protected anagraficaNGClient: AnagraficaNGClient
    ) {
        super(ajaxAgronicaService,ajaxAgronicaAPIService, masterService, anagraficaNGClient);
        
    }

    /*public LeggiOrganismiReferenti_Old(impresa: Impresa): Promise<Contatto[]> {
        return new Promise<Contatto[]>(async (resolve, reject) => {
            if (impresa.partitaIva == "") {
                resolve(new Array<Contatto>());
            }
            if (this.organismoReferentexImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }) == undefined) {

                let parametri: CoreWS_Generic<FiltroImpresa> = new CoreWS_Generic(
                    this.masterService.getCoreWSGenericObjP(),
                    { impresa: impresa }
                );

                let R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Contatto[], FiltroImpresa>(
                    this.masterService.link_CoreWS + "/Anagrafica/Contatti.asmx/CaricaComboCmb_OrganismoReferente_Modello",
                    parametri,
                    false);

                //this.organismoReferentexImpresa.push({ impresa: impresa, organismoReferente: R.RispostaStringa });
                resolve(R.RispostaStringa);
                //resolve(this.organismoReferentexImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }).organismoReferente);

            } else {

                resolve(this.organismoReferentexImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }).organismoReferente);

            }
        })
    }*/

    public LeggiOrganismiReferenti(impresa: Impresa): Promise<Contatto[]> {
        return new Promise<Contatto[]>(async (resolve, reject) => {
            if (impresa.partitaIva == "") {
                resolve(new Array<Contatto>());
            }
            if (this.organismoReferentexImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }) == undefined) {

                this.ajaxAgronicaAPIService.ajaxAPIPost<FiltroImpresa, Contatto[]>(
                    "AnagraficaNG/CaricaComboCmbOrganismoReferenteModello",
                    { impresa: impresa },
                    false).pipe(map(R => {
                        //this.organismoReferentexImpresa.push({ impresa: impresa, organismoReferente: R.RispostaStringa });
                        resolve(R.RispostaStringa);
                        //resolve(this.organismoReferentexImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }).organismoReferente);
                    })).subscribe();

            } else {

                resolve(this.organismoReferentexImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }).organismoReferente);

            }
        })
    }

    /*public LeggiRiferimento_Trasferimento_Dati_Old(impresa: Impresa): Promise<Contatto[]> {
        return new Promise<Contatto[]>(async (resolve, reject) => {

            if (this.riferimentoTrasferimentoDatiImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }) == undefined) {

                let parametri: CoreWS_Generic<FiltroImpresa> = new CoreWS_Generic(
                    this.masterService.getCoreWSGenericObjP(),
                    { impresa: impresa }
                );

                let R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Contatto[], FiltroImpresa>(
                    this.masterService.link_CoreWS + "/Anagrafica/Contatti.asmx/CaricaComboCmb_Riferimento_Trasferimento_Dati_Modello",
                    parametri,
                    false);

                this.riferimentoTrasferimentoDatiImpresa.push({ impresa: impresa, riferimentoTrasferimentoDati: R.RispostaStringa });
                resolve(this.riferimentoTrasferimentoDatiImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }).riferimentoTrasferimentoDati);

            } else {

                resolve(this.riferimentoTrasferimentoDatiImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }).riferimentoTrasferimentoDati);

            }
        })
    }*/

    public LeggiRiferimento_Trasferimento_Dati(impresa: Impresa): Promise<Contatto[]> {
        return new Promise<Contatto[]>(async (resolve, reject) => {

            if (this.riferimentoTrasferimentoDatiImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }) == undefined) {

                this.ajaxAgronicaAPIService.ajaxAPIPost<FiltroImpresa, Contatto[]>(
                    "AnagraficaNG/CaricaComboCmbRiferimentoTrasferimentoDati_Modello",
                    { impresa: impresa },
                    false).pipe(map(R => {
                        this.riferimentoTrasferimentoDatiImpresa.push({ impresa: impresa, riferimentoTrasferimentoDati: R.RispostaStringa });
                        resolve(this.riferimentoTrasferimentoDatiImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }).riferimentoTrasferimentoDati);
                    })).subscribe();

            } else {

                resolve(this.riferimentoTrasferimentoDatiImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }).riferimentoTrasferimentoDati);

            }
        })
    }

    /*public LeggiMagazzinoConferimento_Old(impresa: Impresa): Promise<Fabbricato[]> {
        return new Promise<Fabbricato[]>(async (resolve, reject) => {

            if (this.magazzinoConferimentoImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }) == undefined) {

                let parametri: CoreWS_Generic<FiltroImpresa> = new CoreWS_Generic(
                    this.masterService.getCoreWSGenericObjP(),
                    { impresa: impresa }
                );

                let R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Fabbricato[], FiltroImpresa>(
                    this.masterService.link_CoreWS + "/Anagrafica/Contatti.asmx/CaricaComboCmb_MagazzinoConferimento_Modello",
                    parametri,
                    false);

                this.magazzinoConferimentoImpresa.push({ impresa: impresa, magazzinoConferimento: R.RispostaStringa });
                resolve(this.magazzinoConferimentoImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }).magazzinoConferimento);

            } else {

                resolve(this.magazzinoConferimentoImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }).magazzinoConferimento);

            }
        })
    }*/

    public LeggiMagazzinoConferimento(impresa: Impresa): Promise<Fabbricato[]> {
        return new Promise<Fabbricato[]>(async (resolve, reject) => {

            if (this.magazzinoConferimentoImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }) == undefined) {

                this.ajaxAgronicaAPIService.ajaxAPIPost<FiltroImpresa, Fabbricato[]>(
                    "AnagraficaNG/CaricaComboCmbMagazzinoConferimentoModello",
                    { impresa: impresa },
                    false).pipe(map(R => {
                        this.magazzinoConferimentoImpresa.push({ impresa: impresa, magazzinoConferimento: R.RispostaStringa });
                        resolve(this.magazzinoConferimentoImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }).magazzinoConferimento);
                    })).subscribe();

            } else {

                resolve(this.magazzinoConferimentoImpresa.find((el) => { if (el.impresa.partitaIva == impresa.partitaIva) return el }).magazzinoConferimento);

            }
        })
    }

    /*public leggi_Old(objAgenda: ObjParametriAgenda): Observable<any> {
        const parametri: CoreWS_Generic<ObjParametriAgenda> = {
            objP: this.masterService.getCoreWSGenericObjP(),
            InData: objAgenda
        };

        return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<any[], ObjParametriAgenda>(
            this.masterService.link_CoreWS + '/Anagrafica/Contatti.asmx/Leggi_Contatti_Anagrafica1',
            parametri).pipe(
            map((r) => r.RispostaStringa)
        );
    }*/

    public leggi(objAgenda: ObjParametriAgenda): Observable<any> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, any[]>(
            'AnagraficaNG/LeggiContattiAnagrafica1',
            objAgenda,
            false).pipe(
                map((r) => r.RispostaStringa)
            );
    }

    public leggiNG(objAgenda: ObjParametriAgenda): Observable<any> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, any[]>(
            'AnagraficaNG/LeggiContattiAnagrafica',
            objAgenda,
            false).pipe(
                map((r) => r.RispostaStringa)
            );
    }

    public associaUtente(utente): Observable<any> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost('AnagraficaNG/ContattoAssociaUtente', utente);
    }

    public getUtenteDaAssociare() {
        return this.utenteDaAssociareSource.getValue()
    }

    public setUtenteDaAssociare(utente: any) {
        this.utenteDaAssociareSource.next(utente);
    }

    public getWindowAssociaUtenteRef() {
        return this.windowAssociaUtenteRefSource.getValue();
    }

    public setWindowAssociaUtenteRef(windowRef: WindowRef) {
        this.windowAssociaUtenteRefSource.next(windowRef);
    }

    public leggiContattiMacchina(leggiContattiMacchine: LeggiContattiMacchine): Observable<any[]> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiContattiMacchine, any[]>(
            'AnagraficaNG/LeggiContattiMacchina',
            leggiContattiMacchine).pipe(
                map((r) =>
                    r.RispostaStringa.map(row => {
                        return { ...row, primaryKey: row.primaryKey.codice }
                    }))
            );
    }

    public leggiContattiStazioniMeteo(piva: string): Observable<any[]> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<string, any[]>(
            'AnagraficaNG/LeggiContattiStazioniMeteo',
            piva).pipe(
                map((r) =>
                    r.RispostaStringa.map(row => {
                        return { ...row, primaryKey: row.primaryKey.codice }
                    }))
            );
    }

    public leggiUtentiDaAssociare() {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>('AnagraficaNG/LeggiUtentiDaAssociare', "").pipe(map(r => {
            return r.RispostaStringa
        }))
    }

    public leggiUtenteAssociato(piva: string, contattoCod: string) {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>('AnagraficaNG/LeggiUtenteAssociato', ({ piva: piva, contatto_cod: contattoCod })).pipe(map(r => {
            return r.RispostaStringa[0]
        }))
    }

    public leggiRapportoSpecifico(params: LeggiRapportoSpecifico): Observable<rispostaStandard<any>> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiRapportoSpecifico, RispostaStandard>('AnagraficaNG/LeggiRapportoSpecifico',
            params,
            false
        ).pipe(map(data => {
            return data;
        }))
    }

    public leggiImpostazioniTemplate(p: Leggi_Impostazioni): Observable<ImpostazioneSemplice[]> {
        const url_ = "/MetaschemaNG/Carica_ImpostazioniTemplate";

        return this.ajaxAgronicaAPIService.ajaxAPIPost<Leggi_Impostazioni, any>(url_, p)
            .pipe(
                map(response => {
                    if (Array.isArray(response.RispostaStringa)) {
                        return response.RispostaStringa as ImpostazioneSemplice[];
                    }
                })
            );
    }
    leggiIndirizzo(): Observable<Indirizzo> {
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>('AnagraficaNG/LeggiIndirizzoAziendaSuperUser', {})
            .pipe(map(res => (res.RispostaStringa as Indirizzo)));
    }
    LeggiImpostazioniProprietaContatti(): Observable<ImpostazioneSemplice[]> {
        return this.leggiImpostazioniTemplate(new Leggi_Impostazioni([], [enum_Impostazioni_Utenti.SUPERUSER_NuovaAzienda_ProprietaContatti]))
    }
}

export class LeggiContattiMacchine {
    objNG: ObjParametriAgenda;
    Flag_Pubblico_Privato: boolean;
    Flag_Visibilita_Centri: boolean;
    Cod_Contatto: string;
}
export interface ImpostazioneCampo {
    Valore: string;
    TipoCampo: string;
    Note: string;
    codice: string;
    descrizione: string;
}
export interface ImpostazioneSemplice {
    Impostazione_Cod: number;
    data: ImpostazioneCampo[];
}
