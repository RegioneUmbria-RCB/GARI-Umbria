import { Injectable } from '@angular/core';
import {AbstractAjaxAgronicaService} from './ajax-agronica-base.service';
import {HttpClient} from '@angular/common/http';
import {MasterService} from './master.service';
import {ConversionService} from './conversion.service';
import {GiasDialogService} from './gias-dialog.service';

@Injectable({
  providedIn: 'root'
})
export class AjaxAgronicaNetCoreDataExchangeApiService extends AbstractAjaxAgronicaService {

  constructor(
    http: HttpClient,
    masterService: MasterService,
    conversionService: ConversionService,
    giasDialogService: GiasDialogService,
  ) {
    super(http, masterService, conversionService, giasDialogService);
  }

  protected get baseUrl(): string {
    return this.masterService.linkNetCoreDataExchange;
  }
}
