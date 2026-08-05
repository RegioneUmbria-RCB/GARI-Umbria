import { FormBuilder, FormControl } from "@angular/forms";
import { CentroAziendale } from "app/Model/anagrafiche/CentroAziendale";
import { Impresa } from "app/Model/anagrafiche/Impresa";
import { Operatore } from "app/Model/metaschema/utilizzi/Operatore";
import { Specie } from "app/Model/metaschema/utilizzi/Specie";
import { ImpiantoItem } from "app/menu-agenda/components/utils";
import { TipoVisita } from "./filters-visite.service";
import { UtilizzoTerreno } from "app/Model/metaschema/utilizzi/UtilizzoTerreno";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { AttivitaPersonalizzata } from "app/Model/attivita/AttivitaPersonalizzata";
import { DropdownListSpecieAnimali } from "app/quaderno-di-campagna/agenda-edit/service/testata/specie-animali-service";


export class FiltersConfig {
    OperatoreVisita: Operatore;
    Aziende_Agenzie: boolean;
    AziendeVisita: Impresa;
    CentroAziendaleVisita: CentroAziendale;
    Visualizza_Specie: boolean;
    SpecieVisita: UtilizzoTerreno | Specie;
    TipoOperazione: AttivitaPersonalizzata[];
    Impianti: ImpiantoItem[];
    Da: Date;
    A: Date;
    TipoVisita: TipoVisita;            //aggiunta per filtrare Visite per eseguite, non eseguite
    SpecieAnimaliVisita: DropdownListSpecieAnimali;
    WithDettaglioRilievo: boolean;
}

const DA = AGRODATAINIZIO //new Date(new Date().getUTCFullYear(), 2, 5);
const A = AGRODATAFINE; //new Date(new Date().getUTCFullYear(), 2, 10);

export function InitializeFilters(vals?: Required<FiltersConfig>): FiltersConfig {
    return {
        OperatoreVisita: vals?.OperatoreVisita ?? null,
        Aziende_Agenzie: vals?.Aziende_Agenzie ?? null,
        AziendeVisita: vals?.AziendeVisita ?? null,
        CentroAziendaleVisita: vals?.CentroAziendaleVisita ?? null,
        Visualizza_Specie: vals?.Visualizza_Specie ?? null,
        SpecieVisita: vals?.SpecieVisita ?? null,
        TipoOperazione: vals?.TipoOperazione ?? [],
        Impianti: vals?.Impianti ?? [],
        Da: vals?.Da ?? DA,
        A: vals?.A ?? A,
        TipoVisita: vals?.TipoVisita ?? null,
        SpecieAnimaliVisita: vals?.SpecieAnimaliVisita ?? null,
        WithDettaglioRilievo: vals?.WithDettaglioRilievo ?? false
    };
}

export function CreateFormGroup(fb: FormBuilder, mask: FiltersConfig) {
    return fb.group({
        OperatoreVisita: new FormControl(mask.OperatoreVisita),
        Aziende_Agenzie: new FormControl(mask.Aziende_Agenzie),
        AziendeVisita: new FormControl(mask.AziendeVisita),
        CentroAziendaleVisita: new FormControl(mask.CentroAziendaleVisita),
        Visualizza_Specie: new FormControl(mask.Visualizza_Specie),
        SpecieVisita: new FormControl(mask.SpecieVisita),
        TipoOperazione: new FormControl(mask.TipoOperazione),
        Impianti: new FormControl(mask.Impianti),
        Da: mask.Da,
        A: mask.A,
        TipoVisita: new FormControl(mask.TipoVisita),
        SpecieAnimaliVisita: new FormControl(mask.SpecieAnimaliVisita),
        WithDettaglioRilievo: new FormControl(mask.WithDettaglioRilievo)
    });
}