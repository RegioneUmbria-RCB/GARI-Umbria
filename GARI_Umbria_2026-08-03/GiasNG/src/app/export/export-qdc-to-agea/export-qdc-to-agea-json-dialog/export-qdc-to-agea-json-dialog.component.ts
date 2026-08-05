import { Component, EventEmitter, Input, Output, ViewChild, ViewContainerRef } from '@angular/core';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Clipboard } from '@angular/cdk/clipboard';
import { faCopy, faDownload } from '@fortawesome/free-solid-svg-icons';

@Component({
  standalone: false,
  selector: 'app-export-qdc-to-agea-json-dialog',
  templateUrl: './export-qdc-to-agea-json-dialog.component.html',
  styleUrls: ['./export-qdc-to-agea-json-dialog.component.css']
})
export class ExportQdcToAgeaJsonDialogComponent {
  @ViewChild("jsonContent", { read: ViewContainerRef, static: false }) jsonContent!: ViewContainerRef;
  @ViewChild("notificationContainer", { read: ViewContainerRef, static: false }) notificationContainer!: ViewContainerRef;

  @Input() title?: string;
  @Input() data?: string;
  @Input() isLoading: boolean = false;
  @Output() onClose = new EventEmitter();

  faCopy = faCopy;
  faDownload = faDownload;

  constructor(
    private clipboard: Clipboard,
    private notificationService: NotificationService
  ) { }

  copy(): void {
    const value = this.jsonContent.element.nativeElement.innerHTML;
    const copy = this.clipboard.beginCopy(value);
    if (copy.copy()) {
      this.notificationService.show({
        content: 'Copiato!',
        appendTo: this.notificationContainer,
        type: { style: 'success', icon: true },
        hideAfter: 1000
      });
      copy.destroy();
    }
  }

  close(): void {
    this.data = undefined;
    this.onClose.emit();
  }
}
