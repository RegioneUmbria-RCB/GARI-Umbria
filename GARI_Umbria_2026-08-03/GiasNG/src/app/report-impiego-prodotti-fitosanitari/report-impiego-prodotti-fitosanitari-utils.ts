export interface ImpresaModel {
    Piva: string,
    Rag_Soc: string,
}

export interface SostanzaAttiva{
    Pa_Cod: number,
    Pa_Des: string
  }

export class FormReportImpiegoProdottiFitosanitariConfig {
    Province: any[];
    Comuni: any[];
    Data_Da: Date;
    Data_A: Date;
    ZVN: boolean;
    SostanzeAttiveDropdown: SostanzaAttiva;
    VisualizzaProdotti: boolean;
    IncludiFertilizzanti: boolean;
    Imprese: ImpresaModel[];
}

export function InitializeFormReportImpiegoProdottiFitosanitari(vals?: Required<FormReportImpiegoProdottiFitosanitariConfig>) : FormReportImpiegoProdottiFitosanitariConfig {
    return {
        Province: vals?.Province ?? new Array(),
        Comuni: vals?.Comuni ?? new Array(),
        Data_Da: vals?.Data_Da ?? null,
        Data_A: vals?.Data_A ?? null,
        ZVN: vals?.ZVN ?? false,
        SostanzeAttiveDropdown: vals?.SostanzeAttiveDropdown ?? {Pa_Cod: 0, Pa_Des: ''},
        VisualizzaProdotti: vals?.VisualizzaProdotti ?? false,
        IncludiFertilizzanti: vals?.IncludiFertilizzanti ?? false,
        Imprese: vals?.Imprese ?? new Array()
    };
}
