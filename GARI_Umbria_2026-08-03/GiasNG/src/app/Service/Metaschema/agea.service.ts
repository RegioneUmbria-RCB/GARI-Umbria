import { Impresa } from "../../Model/anagrafiche/Impresa";
import { Injectable } from "@angular/core";
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";

export class ExportQdCtoAgea {
  impresa: Impresa;
  anno: number;
}
export class ExportQdCtoAgeaPaginated extends ExportQdCtoAgea {
  skip: number;
  top: number;
}

@Injectable({
  providedIn: 'root'
})

export class AgeaService {
  constructor(private ajaxAgronicaAPIService: AjaxAgronicaAPIService) { }

  public exportQdCtoAgea(p: ExportQdCtoAgea) {
    return this.ajaxAgronicaAPIService
      .ajaxAPIPost<ExportQdCtoAgea, string>('MetaschemaNG/ExportQdCToAgea', p, true);
  }

  public getAllBundleState(p: ExportQdCtoAgeaPaginated) {
    return this.ajaxAgronicaAPIService
      .ajaxAPIPost<ExportQdCtoAgea, HubAgeaResult[]>('MetaschemaNG/GetAllBundleState', p, false);
  }

  public countBundleState(p: ExportQdCtoAgea) {
    return this.ajaxAgronicaAPIService
      .ajaxAPIPost<ExportQdCtoAgea, number>('MetaschemaNG/CountBundleState', p, false);
  }

  public readJson(bundleId: number) {
    return this.ajaxAgronicaAPIService
      .ajaxAPIGet<any, string>(`MetaschemaNG/GetBundleData/${bundleId}`, undefined, true);
  }
  public readLog(instanceId: string) {
    return this.ajaxAgronicaAPIService
      .ajaxAPIGet<any, string>(`MetaschemaNG/ReadBundleLog/${instanceId}`, undefined, true);
  }
}


export interface HubAgeaResult {
  bundleId: number;
  ageaToken: string;
  creationDate: Date;
  creationUser: string;
  submissionState: SubmissionState;
  loadingStageState: LoadingStageState;
}

export interface SubmissionState {
  state: string;
  user: string;
  date: Date;
  message: string;
  instanceId?: string;
}

export interface LoadingStageState {
  state: string;
  user: string;
  date: Date;
  message: string;
  instanceId?: string;
}