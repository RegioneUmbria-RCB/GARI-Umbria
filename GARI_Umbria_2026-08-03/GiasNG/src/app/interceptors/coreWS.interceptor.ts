import { Inject, Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpHeaders } from '@angular/common/http';
import {Observable, of, tap} from 'rxjs';
import { API_BASE_URL } from 'app/Service/api.service';
import {MasterService} from "../Service/master.service";
import {environment} from "../../environments/environment";
import {DialogService} from "@progress/kendo-angular-dialog";
import {GiasDialogService} from "../Service/gias-dialog.service";
import {ConfigurazioneSitiService} from "../Service/configurazione-siti.service";
import {switchMap, take} from "rxjs/operators";

@Injectable()
export class CoreWSInterceptor implements HttpInterceptor {

    constructor(private masterService: MasterService,
                private dialogService: GiasDialogService,
                private configurazioneSitiService: ConfigurazioneSitiService) {
    }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (this.masterService._link_CoreWS != null && this.masterService._link_CoreWS != "" && request.url.indexOf(this.masterService._link_CoreWS) != -1) {
            if (environment.production == false) {
                this.dialogService.alertMessage("Chiamata ai CoreWS: " + request.url);
                //console.log(request.url);
                return null;
                //return next.handle(request);
            } else {
                 return this.configurazioneSitiService.leggiChiave("BloccaCoreWS_NG").pipe(
                     switchMap((val) => {
                         if (val?.Valore == 'true'){
                             this.dialogService.alertMessage("Chiamata ai CoreWS: " + request.url);
                             //console.log(request.url);
                             return of(null);
                         } else {
                             return next.handle(request);
                         }
                     })
                 )
            }
        } else {
            return next.handle(request);
        }
    }
}

