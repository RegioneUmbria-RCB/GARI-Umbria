import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { CentroItem, ImpiantoItem, OperazioneItem, SpecieItem } from '../utils';
import { DropdownItem, FiltersConfig } from '../models';



const DropdownDefaultItem: DropdownItem = {codice: 0, descrizione: ''};

const CENTRO_DEFAULT: CentroItem = { sa_cod: '0', sa_nome: '' };
const SPECIE_DEFAULT: SpecieItem = { veg_cod: '-1', veg_des: '' };
const OPERAZIONE_DEFAULT: OperazioneItem = { gru_cod: 0,gru_des: '',tipo: ''  };
const IMPIANTO_DEFAULT: ImpiantoItem = { chiave: '0', des: '' };

// const DA2 = new Date(new Date().getFullYear() - 21, 0, 1);
// const A2 = new Date(new Date().getFullYear(), 11, 31);

//const DA = new Date(new Date().getUTCFullYear(), new Date().getUTCMonth(), 1); //INIZO MESE CORRENTE
//const A = new Date(new Date().getUTCFullYear(), new Date().getUTCMonth() + 1, 0); //FINE MESE CORRENTE
const DA = new Date(new Date().getUTCFullYear(), 2, 5);
const A = new Date(new Date().getUTCFullYear(), 2, 10);


export interface Mask {
    CentroAziendale: CentroItem[];
    Specie: SpecieItem[];
    TipoOperazione: OperazioneItem[];
}

export function InitializeFilters(vals?: Required<FiltersConfig>): FiltersConfig {
    return {
        CentroAziendale: vals?.CentroAziendale ?? CENTRO_DEFAULT,
        Specie: vals?.Specie ?? SPECIE_DEFAULT,
        TipoOperazione: vals?.TipoOperazione ?? [],
        Impianti: vals?.Impianti ?? [],
        Da: vals?.Da ?? DA,
        A: vals?.A ?? A,
        MostraDDT: vals?.MostraDDT ?? false,
        TipoVisita: vals?.TipoVisita ?? null
    };
}

export function CreateFormGroup(fb: FormBuilder, mask: FiltersConfig) {
    return fb.group({
        CentroAziendale: new FormControl(mask.CentroAziendale),
        Specie: new FormControl(mask.Specie),
        TipoOperazione: new FormControl(mask.TipoOperazione),
        Impianti: new FormControl(mask.Impianti),
        Da: mask.Da,
        A: mask.A,
        MostraDDT: mask.MostraDDT
    });
}
