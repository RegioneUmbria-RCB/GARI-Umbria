import { Injectable, InjectionToken } from "@angular/core";
import { Contatto } from "app/Model/anagrafiche/Contatto";
import { Fabbricato } from "app/Model/anagrafiche/Fabbricato";
import { Impresa } from "app/Model/anagrafiche/Impresa";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import { MasterService } from "app/Service/master.service";
import { Observable } from "rxjs";
import { ObjParametriAgenda } from 'gias-ui-kit';
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import { AnagraficaNGClient } from "../api.service";

export class OrganismoImpresa{
    impresa: Impresa;
    organismoReferente: Contatto[];
}

export class RiferimentoTrasferimentoDatiImpresa{
    impresa: Impresa;
    riferimentoTrasferimentoDati: Contatto[];
}

export class MagazzinoConferimentoImpresa{
    impresa: Impresa;
    magazzinoConferimento: Fabbricato[];
}

export const CONTATTI_SERVICE_TOKEN = new InjectionToken<ContattiFactoryService>('app.contatti.service');

@Injectable()
export abstract class ContattiFactoryService {

    protected organismoReferentexImpresa: OrganismoImpresa[] = new Array<OrganismoImpresa>();
    protected riferimentoTrasferimentoDatiImpresa: RiferimentoTrasferimentoDatiImpresa[] = new Array<RiferimentoTrasferimentoDatiImpresa>();
    protected magazzinoConferimentoImpresa: MagazzinoConferimentoImpresa[] = new Array<MagazzinoConferimentoImpresa>();

    constructor(protected ajaxAgronicaService: AjaxAgronicaService,
                protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
                protected masterService: MasterService,
                protected anagraficaNGClient: AnagraficaNGClient) {
    }

    abstract leggi(oibjAgenda: ObjParametriAgenda): Observable<any>;

    abstract leggiNG(oibjAgenda: ObjParametriAgenda): Observable<any>;

    abstract LeggiOrganismiReferenti(impresa: Impresa): Promise<Contatto[]>;

    abstract LeggiRiferimento_Trasferimento_Dati(impresa: Impresa): Promise<Contatto[]>;

    abstract LeggiMagazzinoConferimento(impresa: Impresa): Promise<Fabbricato[]>;

}
