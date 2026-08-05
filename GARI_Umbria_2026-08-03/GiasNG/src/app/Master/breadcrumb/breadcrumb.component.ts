import { Component, Input } from '@angular/core';
import { BreadcrumbsInfo } from 'app/Service/api.service';
import { BreadcrumbsTxtService } from './breadcrumbs-txt.service';
import { ExternalNavigationService } from 'app/Service/external-navigation.service';
import {MasterService} from "../../Service/master.service";

@Component({
  standalone: false,
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.css']
})
export class BreadcrumbComponent {
  @Input() isDashboard: boolean = false;

  data: BreadcrumbsInfo | null = null;

  constructor(
    private breadcrumbsService: BreadcrumbsTxtService,
    private externalNavigationService: ExternalNavigationService,
    private masterService: MasterService
  ) {
    this.breadcrumbsService
      .breadcrumbsInfo$()
      .subscribe(data => this.data = data);
  }

  get isExternalLoad(): boolean {
    return false; // i breadcrumbs vengono sempre mostrati per rendere la navigazione più chiara all'utente
    //return this.externalNavigationService.externalLoad != null;
  }

  selectClicked(event: MouseEvent): void {
    event.stopPropagation();
  }

  public ShowBackButton(){
    if(this.masterService.getHeader() && this.masterService.getHeader().visible && this.masterService.getHeader().BackButton){
      return true;
    }else{
      return false;
    }
  }
}
