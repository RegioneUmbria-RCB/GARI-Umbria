import { ICodiceTemplateResult as ICodiceTemplateResult } from 'app/Utility/Template/codici-template/models/codici.model';
import { CodiceAnagrafe } from './anagrafiche/CodiceAnagrafe';
import { CodiciAnagrafeValoriChiave } from './anagrafiche/CodiciAnagrafeValori';

export class Codice {
    public chiave: number;
    public Descrizione: string;
    public Id_Cod: string;
    public Val_Cod: string;
    public Validita_Inizio: Date;
    public Validita_Fine: Date;
}

export class ImpresaCodiciTemplateResult implements ICodiceTemplateResult {
    public textField: string;
    public valueField: string;
    public dateControl: boolean;
    public rows: CodiciAnagrafeValoriChiave[];
    public codiciDropdown: CodiceAnagrafe[];
}
