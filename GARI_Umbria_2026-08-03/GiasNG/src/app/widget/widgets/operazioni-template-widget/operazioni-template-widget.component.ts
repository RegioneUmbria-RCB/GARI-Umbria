import {Component, EventEmitter, Input, Output, SecurityContext} from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { faInfo } from '@fortawesome/free-solid-svg-icons';
import { TranslocoService } from '@jsverse/transloco';
import { Widget_Operazione } from 'app/Service/api.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-operazioni-template-widget',
  templateUrl: './operazioni-template-widget.component.html',
  styleUrls: ['./operazioni-template-widget.component.css']
})
export class OperazioniTemplateWidgetComponent {
  @Input() operations: Widget_Operazione[] = [];

  @Output() onNavigate = new EventEmitter<[Widget_Operazione, Enum_DBTypeOperation]>();

  faInfo = faInfo;

  private language: string;

  constructor(
    private translocoService: TranslocoService,
    private sanitizer: DomSanitizer
  ) {
    this.language = this.translocoService.getActiveLang();
  }

  getDayName(operation: Widget_Operazione): string {
    return this.getAsDate(operation.Data_Operazione).toLocaleDateString(this.language, { weekday: 'short' });
  }

  getDayValue(operation: Widget_Operazione): string {
    return this.getAsDate(operation.Data_Operazione).getDate().toString();
  }

  getMonthName(operation: Widget_Operazione): string {
    return this.getAsDate(operation.Data_Operazione).toLocaleDateString(this.language, { month: 'long' });
  }

  listToString(list: string[]): string {
    return list?.join(', ') ?? '';
  }

  getImage(operationImage: string): SafeResourceUrl {
    //return this.sanitizer.bypassSecurityTrustResourceUrl('data:image/jpg;base64,' + operationImage);
    return this.sanitizer.sanitize(
      SecurityContext.URL,
      'data:image/jpg;base64,' + operationImage
    )
  }

  navigateToAllProducts(): void {
    let a = 0; //Commento per funzione vuota SonarQube
  }

  navigateToModifica(attivita: Widget_Operazione): void {
    this.onNavigate.emit([attivita, Enum_DBTypeOperation.Update]);
  }

  navigateToDettagli(attivita: Widget_Operazione): void {
    this.onNavigate.emit([attivita, Enum_DBTypeOperation.Read]);
  }

  private getAsDate(date: string | Date): Date {
    return date instanceof Date ? date : new Date(date);
  }
}
