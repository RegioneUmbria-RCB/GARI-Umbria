import { Injectable } from '@angular/core';
import { Contatto } from 'app/Model/anagrafiche/Contatto';
import { Fabbricato } from 'app/Model/anagrafiche/Fabbricato';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { FiltroImpresa } from 'app/Model/filtri/FiltroImpresa';
import { map, Observable, of } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService } from '../master.service';
import { ContattiFactoryService } from '../ServiceFactory/contatti.factory.service';
import { AnagraficaNGClient } from '../api.service';

@Injectable({
    providedIn: 'root'
})
export class ContattiBudgetService extends ContattiFactoryService {
    constructor(protected ajaxAgronicaService: AjaxAgronicaService,
        protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        protected masterService: MasterService,
        protected  anagraficaNGClient: AnagraficaNGClient) {
        super(ajaxAgronicaService, ajaxAgronicaAPIService, masterService, anagraficaNGClient);
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


    public leggi(): Observable<any> {
        return of({ kendo_rows: [] });
    }

    public leggiNG(): Observable<any> {
            return of({ kendo_rows: [] });
    }

}
