import { Observable } from 'rxjs';
import { enum_InputType, ErrorMsg, LoadingObject } from './models';

export const GIAS_MASTER_SERVICE_TOKEN = 'GIAS_MASTER_SERVICE_TOKEN'; // TODO add it in kendo

export interface IGiasMasterService {

  ObjParametri_Super_Server: any;
  ObjParametri_Server: any;
  ObjParametri_Utenti: any;

  currentInputType: Observable<enum_InputType>;
  serverTimeZoneOffset: string;
  link_API: string;

  changeErrorMsgType(upd_errorMsg: ErrorMsg): void;

  set_isLoading(loadingObject: LoadingObject): void;

  get_isLoading(): LoadingObject;

  changeShowBackground(upd_ShowBackground: boolean);
}

