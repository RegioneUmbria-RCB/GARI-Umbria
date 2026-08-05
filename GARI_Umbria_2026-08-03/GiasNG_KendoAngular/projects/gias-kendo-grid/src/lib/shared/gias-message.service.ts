import { Injectable } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { NotificationRef, NotificationService } from "@progress/kendo-angular-notification";
import { CustomMessageComponent, CustomMessageService } from './custom-message.service';
import { Dialog_Type } from 'gias-ui-kit';

@Injectable({
  providedIn: 'root',
})
export class GiasMessageService {
  private hideAfter: 5000;
  private errorOpen: boolean = false;

  private errorsVisible: NotificationRef[] = [];

  customMessageComponent: typeof CustomMessageComponent;

  constructor(
    private notificationService: NotificationService,
    private transloco: TranslocoService,
    private customMessageService: CustomMessageService
  ) { }

  successMessage(content: string, closable: boolean = false, useTransloco: boolean = false, translocoParams: string[] = [], hideAfter?: number): NotificationRef {
    return this.showMessage('button-notification', 'success', content, closable, useTransloco, translocoParams, hideAfter);
  }

  infoMessagge(content: string, closable: boolean = false, useTransloco: boolean = false, translocoParams: string[] = [], hideAfter?: number): NotificationRef {
    return this.showMessage('button-notification', 'info', content, closable, useTransloco, translocoParams, hideAfter);
  }

  errorMessage(content: string, closable: boolean = false, useTransloco: boolean = false, translocoParams: string[] = [], hideAfter?: number): NotificationRef {
    return this.showMessage('button-notification', 'error', content, closable, useTransloco, translocoParams, hideAfter);
  }

  warningMessage(content: string, closable: boolean = false, useTransloco: boolean = false, translocoParams: string[] = [], hideAfter?: number): NotificationRef {
    return this.showMessage('button-notification', 'warning', content, closable, useTransloco, translocoParams, hideAfter);
  }

  noneMessage(content: string, closable: boolean = false, useTransloco: boolean = false, translocoParams: string[] = [], hideAfter?: number): NotificationRef {
    return this.showMessage('button-notification', 'none', content, closable, useTransloco, translocoParams, hideAfter);
  }

  setErrorOpen(status: boolean) {
    this.errorOpen = status;

    if (!status) {
      this.errorsVisible.forEach(e => e.hide());
    }
  }

  message(messageType: Dialog_Type, content: string, closable: boolean = false, useTransloco: boolean = false, translocoParams: string[] = [], hideAfter?: number): NotificationRef {
    switch (messageType) {
      case Dialog_Type.error:
        return this.errorMessage(content, closable, useTransloco, translocoParams, hideAfter);
      case Dialog_Type.warning:
        return this.warningMessage(content, closable, useTransloco, translocoParams, hideAfter);
      case Dialog_Type.info:
        return this.infoMessagge(content, closable, useTransloco, translocoParams, hideAfter);
      case Dialog_Type.success:
        return this.successMessage(content, closable, useTransloco, translocoParams, hideAfter);
      default:
        return this.noneMessage(content, closable, useTransloco, translocoParams);
    }
  }

  customMessage(contentString: string, actionString: string, color: string, customFunction, closable: boolean = true, hideAfter?: number): NotificationRef {
    this.customMessageService.createCustomMessage(contentString, actionString, color, customFunction);
    this.customMessageComponent = this.customMessageService.getCustomMessageComponent();

    const toolbarWidth = document.getElementById('gis-toolbar')?.clientWidth;

    return this.notificationService.show({
      content: this.customMessageComponent,
      closable: true,
      cssClass: closable ? "button-notification-custom" : "button-notification-custom-no-close",
      position: { horizontal: 'center', vertical: 'top' },
      type: { style: "info", icon: false },
      width: toolbarWidth
    });
  }

  private showMessage(
    cssClass: string,
    style: 'none' | 'success' | 'warning' | 'error' | 'info',
    content: string,
    closable: boolean = false,
    useTransloco: boolean = false,
    translocoParams: string[] = [],
    hideAfter?: number
  ): NotificationRef {
    if (useTransloco)
      content = this.transloco.translate(content, translocoParams);

    return this.notificationService.show({
      content: content,
      cssClass: cssClass,
      animation: { type: 'slide', duration: 400 },
      position: { horizontal: 'right', vertical: 'bottom' },
      type: { style: style },
      hideAfter: (!!hideAfter && hideAfter !== 0) ? hideAfter : this.hideAfter,
      closable: closable,
    });
  }
}
