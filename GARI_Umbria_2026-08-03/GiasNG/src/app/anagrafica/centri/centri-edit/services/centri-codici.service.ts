import { Injectable } from '@angular/core';
import { CodiceAnagrafe } from 'app/Model/anagrafiche/CodiceAnagrafe';
import { CodiciAnagrafeValoriChiave } from 'app/Model/anagrafiche/CodiciAnagrafeValori';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { ICodiciTemplateService } from 'app/Utility/Template/codici-template/services/codici-template.service';
import { HttpAction } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { Observable } from 'rxjs';

@Injectable({providedIn: 'root'})
export class CentriCodici implements ICodiciTemplateService {
    gridId = "CentriCodici";
    private codiciList: CodiceAnagrafe[];
    public gridPublicService: GridPublicService;

    leggiDropdowns(): Observable<CodiceAnagrafe[]> | Observable<BaseCodeDescr[]> {
        // if (this.codiciList == null) {
        //     return this.store.ddls.pipe(map(s => {
        //         return this.store.codici;
        //     }));
        // } else {
        //     return of(this.store.codici);
        // }
        return null;
    }

    updateTable(action: HttpAction, row: CodiciAnagrafeValoriChiave) {
        throw new Error('Method not implemented.');
    }
    leggiDatiTabella(): Observable<CodiciAnagrafeValoriChiave[]> {
        throw new Error('Method not implemented.');
    }

    postForm(): void {
        throw new Error('Method not implemented.');
    }

}
