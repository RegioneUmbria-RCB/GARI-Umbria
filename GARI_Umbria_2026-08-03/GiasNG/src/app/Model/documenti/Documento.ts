import { DocumentoAllegato } from "./DocumentoAllegato";
import { EnteRilascio } from "./EnteRilascio";

export class Documento{
    ID_Tipologia: number;
    Data_Scadenza: Date;
    Descrizione: string;
    Note: string;
    Ente_Rilascio: EnteRilascio;
    Data_Rilascio: Date;
    Numero: string;
    Allegati: DocumentoAllegato[];
}
