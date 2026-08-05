import { Operatore } from "app/Model/metaschema/utilizzi/Operatore";
import { Impresa } from "app/Model/anagrafiche/Impresa";
import { Specie } from "app/Model/metaschema/utilizzi/Specie";
import { UtilizzoTerreno } from "app/Model/metaschema/utilizzi/UtilizzoTerreno";
import { DropdownListSpecieAnimali } from "app/quaderno-di-campagna/agenda-edit/service/testata/specie-animali-service";

export enum enum_visiteGridCommands {
    RICERCA_DOCUMENTI,
    COPIA_VISITA,
    VAI_A_RILIEVO,
    CANCELLA_RILIEVO_ASSOCIATO
}

export class FormCopiaVisitaConfig {
    Data_Copia_Visita: Date;
    Ora_Inizio_Copia_Visita: Date;
    Ora_Fine_Copia_Visita: Date;
    OperatoreCopiaVisita: Operatore;
    AziendeCopiaVisita: Impresa[];
    SpecieCopiaVisita: UtilizzoTerreno | Specie;
    SpecieAnimaleCopiaVisita: DropdownListSpecieAnimali;
    DescrizioneCopiaVisita: string;
}

export function InitializeFormCopiaVisita(vals?: Required<FormCopiaVisitaConfig>): FormCopiaVisitaConfig {
    return {
        Data_Copia_Visita: vals?.Data_Copia_Visita ?? null,
        Ora_Inizio_Copia_Visita: vals?.Ora_Inizio_Copia_Visita ?? null,
        Ora_Fine_Copia_Visita: vals?.Ora_Fine_Copia_Visita ?? null,
        OperatoreCopiaVisita: vals?.OperatoreCopiaVisita ?? null,
        AziendeCopiaVisita: vals?.AziendeCopiaVisita ?? [],
        SpecieCopiaVisita: vals?.SpecieCopiaVisita ?? null,
        SpecieAnimaleCopiaVisita: vals?.SpecieAnimaleCopiaVisita ?? null,
        DescrizioneCopiaVisita: vals?.DescrizioneCopiaVisita ?? null
    };
}
