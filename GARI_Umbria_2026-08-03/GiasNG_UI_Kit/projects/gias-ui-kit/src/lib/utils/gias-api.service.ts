import { Observable } from "rxjs";
import { rispostaStandard } from "./models";

export const GIAS_API_SERVICE_TOKEN = 'GIAS_API_SERVICE_TOKEN';

export interface IGiasApiService { // TODO use it in kendgo grid
  ajaxAPIPost<InType, OutType>(
    url: string,
    parametri: InType,
    setLoading?: boolean,
    compressione?: boolean,
    showErroriGestiti?: boolean,
    showErroriNonGestiti?: boolean): Observable<rispostaStandard<OutType>>;

  ajaxAPIGet<InType, OutType>(
    url: string,
    parametri: InType,
    setLoading?: boolean,
    compressione?: boolean,
    showErroriGestiti?: boolean,
    showErroriNonGestiti?: boolean): Observable<rispostaStandard<OutType>>;
}
