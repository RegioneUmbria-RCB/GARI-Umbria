import { Injectable } from '@angular/core';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { CodiceAnagrafe } from 'app/Model/anagrafiche/CodiceAnagrafe';
import { CodiciAnagrafeValoriChiave } from 'app/Model/anagrafiche/CodiciAnagrafeValori';
import { CentriAziendaliService } from 'app/Service/Anagrafica/centri.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ICodiciTemplateService } from 'app/Utility/Template/codici-template/services/codici-template.service';
import { HttpAction } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { BehaviorSubject, Observable } from 'rxjs';




@Injectable({ providedIn: 'root' })
export class CentroEditService implements ICodiciTemplateService {
    gridId = "CentriCodici";
    public gridPublicService: GridPublicService;
    public centro: CentroAziendale;
    public codici: CodiciAnagrafeValoriChiave[];

    constructor(
        private centriService: CentriAziendaliService,
        private parametriAgenda: ObjParametriAgendaService) {
    }


    updateTable(action: HttpAction, row: CodiciAnagrafeValoriChiave) {
        throw new Error('Method not implemented.');
    }
    leggiDatiTabella(): Observable<CodiciAnagrafeValoriChiave[]> {
        throw new Error('Method not implemented.');
    }
    leggiDropdowns(): Observable<CodiceAnagrafe[]> {
        throw new Error('Method not implemented.');
    }
    postForm(): void {
        throw new Error('Method not implemented.');
    }
}
