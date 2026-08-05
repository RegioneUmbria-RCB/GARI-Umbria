import { Injectable } from '@angular/core';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';
import { GiasDialogService } from 'gias-ui-kit';
import { Observable, take, map, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DocumentaryAuditService {

  constructor(
    private apiService: AjaxAgronicaAPIService,
    private dialog: GiasDialogService
  ) { }

  public GetChecklistMamagementData(): Observable<any> {
    return this.apiService.ajaxAPIPost("Audit/GetCheckListManagementData", "")
      .pipe(
        take(1),
        map(res => res.RispostaOK ? res.RispostaStringa as any[] : [])
      );
  }

  public SaveChecklistManagement(newValue: string): Observable<boolean> {
    return this.apiService.ajaxAPIPost("Audit/SaveCheckListManagement", newValue)
      .pipe(
        take(1),
        map(res => res.RispostaOK)
      );
  }

  public GetWorkflowMamagementData(): Observable<any> {
    return this.apiService.ajaxAPIPost("Audit/GetWorkflowManagementData", "")
      .pipe(
        take(1),
        tap((res) => console.log("WorkflowMamagementData", res)),
        map(res => res.RispostaOK ? res.RispostaStringa as any[] : [])
      );
  }

  public SaveWorkflowManagement(newValue: string): Observable<boolean> {
    return this.apiService.ajaxAPIPost("Audit/SaveWorkflowManagement", newValue)
      .pipe(
        take(1),
        map(res => res.RispostaOK)
      );
  }
}
