import { Observable } from 'rxjs';
import { HttpAction } from 'gias-kendo-grid';
import { KendoGridRow } from 'gias-kendo-grid';

import { CodiciAnagrafeValoriChiave } from 'app/Model/anagrafiche/CodiciAnagrafeValori';
import { CodiceAnagrafe } from 'app/Model/anagrafiche/CodiceAnagrafe';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { GridPublicService } from 'gias-kendo-grid';

export interface ICodiciTemplateService {

    gridPublicService: GridPublicService
    gridId: string;
    updateTable(action: HttpAction, row: CodiciAnagrafeValoriChiave);
    leggiDatiTabella(): Observable<CodiciAnagrafeValoriChiave[]>;
    leggiDropdowns(): Observable<CodiceAnagrafe[]> | Observable<BaseCodeDescr[]>;
    postForm(): void;
    //-----------------------------
    disableCodesGrid?: () => void;
}
