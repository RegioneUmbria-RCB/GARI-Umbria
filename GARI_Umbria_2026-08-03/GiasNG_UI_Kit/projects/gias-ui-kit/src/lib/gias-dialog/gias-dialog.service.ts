import { Injectable, TemplateRef } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { DialogAction, DialogCloseResult, DialogRef, DialogResult, DialogService, DialogSettings } from '@progress/kendo-angular-dialog';
import { lastValueFrom, map, Observable } from 'rxjs';
import { GiasDialogComponent } from './gias-dialog.component';

export class Actions extends DialogAction {
  returnObj: Object;
  // [AngularUpdateV14]
  primary?: boolean;

  constructor(text: string, retObject: Object, isPrimary: boolean = false) {
    super();
    this.text = text;
    this.primary = isPrimary;
    this.returnObj = retObject;
  }
}

export class DialogBooleanResult extends DialogCloseResult {
  returnObj: boolean;
}

export enum Dialog_Type {
  error = 1,
  warning = 2,
  info = 3,
  success = 4
}

@Injectable({
  providedIn: 'root',
})
export class GiasDialogService {

  private get onlyOk() {
    return [{ text: this.translocoService.translate('giasgrid.Ok'), returnObj: true }];
  }
  private get confirmAndCancel() {
    return [
      { text: this.translocoService.translate('giasgrid.Conferma'), primary: true, returnObj: true },
      { text: this.translocoService.translate('giasgrid.Annulla'), returnObj: false }
    ];
  }

  constructor(
    private dialogService: DialogService,
    private translocoService: TranslocoService
  ) { }

  /**
   *
   * @param title
   * @param content
   * @param actions i pulsanti che definiscono le opzioni possibili. Se non specificate verranno visualizzati i pulsanti 'Conferma' e 'Annulla'.
   * @param width
   * @param height
   * @param preventAction
   * @param dialog_type
   * @param maxHeight
   * @param maxWidth
   */
  public dialogMessageObs_Result(
    title: string,
    content: string | TemplateRef<any> | Function,
    actions?: Array<Actions>,
    width?: number | string,
    height?: number | string,
    preventAction?: (p: DialogBooleanResult) => boolean,
    dialog_type?: number,
    maxHeight: string | number = 'auto',
    maxWidth: string | number = 'auto'
  ): Observable<DialogResult> {

    let dialog = this.dialogMessageRef(title, content, actions, width, height, preventAction, dialog_type, maxHeight, maxWidth);
    return dialog.result;
  }

  public dialogMessageRef(
    title: string,
    content: string | TemplateRef<any> | Function,
    actions?: Array<Actions>,
    width?: number | string,
    height?: number | string,
    preventAction?: (p: DialogBooleanResult | DialogAction, i?: DialogRef) => boolean,
    dialog_type?: number,
    maxHeight: string | number = 'auto',
    maxWidth: string | number = 'auto'
  ): DialogRef {

    if (actions == undefined) {
      actions = [
        { text: this.translocoService.translate('giasgrid.Conferma'), primary: true, returnObj: true },
        { text: this.translocoService.translate('giasgrid.Annulla'), returnObj: false }
      ];
    }

    let dialogSettings: DialogSettings = {
      title: title,
      content: content,
      actions: actions,
      width: width ? width : 500,
      height: height ? height : 450,
      // return false to close the dialog window, return true to not
      preventAction: preventAction,
      maxHeight: maxHeight,
      maxWidth: maxWidth
    };

    if (typeof content == 'string') {
      dialogSettings.content = GiasDialogComponent;
    }

    if (dialog_type && dialog_type > 0) {

      //TODO verranno aggiunti le classi css da Xonne per cambiare lo stile del messaggio

      //valorizzare la proprieta cssClass di dialogSettings
      switch (dialog_type) {
        case Dialog_Type.error:
          dialogSettings.cssClass = "error-dialog"
          break;
        case Dialog_Type.warning:
          dialogSettings.cssClass = "warning-dialog"
          break;
        case Dialog_Type.info:
          dialogSettings.cssClass = "info-dialog"
          break;
        case Dialog_Type.success:
          dialogSettings.cssClass = "success-dialog"
          break;
        default:
          dialogSettings.cssClass = "info-dialog"
          break;
      }
    } else {
      dialogSettings.cssClass = "info-dialog"
    }

    const dialog: DialogRef = this.dialogService.open(dialogSettings);
    if (typeof content == 'string') {
      const dialogContentInstance = dialog.content.instance as GiasDialogComponent;
      dialogContentInstance.content = content;
      dialogContentInstance.title = title;
      dialogContentInstance.dialogType = dialogSettings.cssClass;
      dialogContentInstance.height = dialogSettings.height;
    }
    return dialog;
  }

  alertMessage(
    content: string | TemplateRef<any>,
    preventAction?: (p: DialogResult) => boolean,
    dialog_type?: number | Dialog_Type,
    maxHeight: string | number = 'auto',
    maxWidth: string | number = 'auto') {

    let dialogSettings: DialogSettings = {
      content: content,
      actions: [{ text: this.translocoService.translate('giasgrid.Ok'), returnObj: true }],
      preventAction: preventAction,
      maxHeight: maxHeight,
      maxWidth: maxWidth
    }

    if (dialog_type && dialog_type > 0) {

      //TODO verranno aggiunti le classi css da Xonne per cambiare lo stile del messaggio

      //valorizzare la proprieta cssClass di dialogSettings
      switch (dialog_type) {
        case Dialog_Type.error:
          dialogSettings.cssClass = "error-dialog"
          break;
        case Dialog_Type.warning:
          dialogSettings.cssClass = "warning-dialog"
          break;
        case Dialog_Type.info:
          dialogSettings.cssClass = "info-dialog"
          break;
        case Dialog_Type.success:
          dialogSettings.cssClass = "success-dialog"
          break;
        default:
          dialogSettings.cssClass = "info-dialog"
          break;
      }
    } else {
      dialogSettings.cssClass = "info-dialog"
    }

    const dialog: DialogRef = this.dialogService.open(dialogSettings);

    return dialog.result;
  }

  /**
   * @param title titolo finestra di dialogo
   * @param content messaggio di errore
   * @param transloco usare transloco? (Default: true)
   */
  public baseError(title: string, content: string | TemplateRef<any>, transloco = true) {
    void this.errorPromise(title, content, transloco);
  }

  /**
   * @param title titolo finestra di dialogo
   * @param content messaggio di errore
   * @param transloco usare transloco? (Default: true)
   * @return promessa che si adempie alla chiusura del dialog
   */
  public errorPromise(
    title: string,
    content: string | TemplateRef<any>,
    transloco = true
  ): Promise<DialogResult> {

    let dialog = this.dialogMessageRef(
      transloco ? this.translocoService.translate(title) : title,
      this.extractContent(content, transloco),
      this.onlyOk, 'auto', 'auto',
      (p) => p instanceof DialogCloseResult,
      Dialog_Type.error
    );
    if (typeof content == 'string')
      dialog.content.location.nativeElement.style.whiteSpace = 'pre-line';
    return lastValueFrom(dialog.result);
  }

  /**
   * Mostra un messaggio di avviso con pulsanti di scelta Annulla/Conferma.
   * @param title titolo finestra di dialogo
   * @param content richiesta interazione utente
   * @param transloco usare transloco? (Default: true)
   * @param preventAction a predicate that verifies if the pressed dialog action should be prevented
   * @return promessa contenente l'azione selezionata
   * @usageNotes Per controllare che l'utente ha cliccato su conferma controllare il campo <code>returnObj</code> dell'oggetto restituito.
   */
  public baseWarning(
    title: string,
    content: string | TemplateRef<any>,
    transloco = true,
    preventAction: (p: DialogBooleanResult) => boolean = () => {
      return false;
    }
  ): Promise<DialogResult> {
    return lastValueFrom(this.warningObs(title, content, transloco, preventAction, false));
  }

  /**
   * Mostra un messaggio di avviso con pulsanti di scelta Annulla/Conferma.
   * @param title titolo finestra di dialogo
   * @param content richiesta interazione utente
   * @param transloco usare transloco? (Default: true)
   * @param preventAction a predicate that verifies if the pressed dialog action should be prevented
   * @param mapToBoolean se impostato a true, ritorna `true` in caso l'utente abbia cliccato su Conferma, `false` altrimenti.
   * @return observable contenente l'azione selezionata
   */
  public warningObs(
    title: string,
    content: string | TemplateRef<any>,
    transloco = true,
    preventAction: (p: DialogBooleanResult) => boolean = () => {
      return false;
    },
    mapToBoolean = true
  ): Observable<DialogResult | boolean> {
    let dialog = this.dialogMessageRef(
      transloco ? this.translocoService.translate(title) : title,
      this.extractContent(content, transloco),
      this.confirmAndCancel, 'auto', 'auto',
      preventAction,
      Dialog_Type.warning
    );
    if (typeof content == 'string') {
      dialog.content.location.nativeElement.style.whiteSpace = 'pre-line';
    }
    return dialog.result.pipe(map(dialogResult => mapToBoolean ? dialogResult['returnObj'] : dialogResult));
  }

  /**
   * Mostra un messaggio di avviso con pulsanti di scelta Annulla/Conferma.
   * Esegue le azioni specificate in base alla scelta dell'utente.
   * @param title titolo finestra di dialogo
   * @param content richiesta interazione utente
   * @param transloco usare transloco? (Default: true)
   * @param ifConfirm funzione da eseguire in caso di scelta tasto 'Conferma'
   * @param ifCancel funzione da eseguire in caso di scelta tasto 'Annulla'
   * @param preventAction a predicate that verifies if the pressed dialog action should be prevented
   */
  public warningThen(
    title: string,
    content: string | TemplateRef<any>,
    transloco = true,
    ifConfirm = () => {
      let a = 0; //Commento per funzione vuota SonarQube
    },
    ifCancel = () => {
      let a = 0; //Commento per funzione vuota SonarQube
    },
    preventAction: (p: DialogBooleanResult) => boolean = () => {
      return false;
    }
  ): void {

    this.baseWarning(title, content, transloco, preventAction)
      .then(resp => {
        if (resp['returnObj']) {
          ifConfirm();
        } else {
          ifCancel();
        }
      })
  }

  /**
   * @param title titolo finestra di dialogo
   * @param content messaggio di errore
   * @param transloco usare transloco? (Default: true)
   */
  public baseInfo(
    title: string,
    content: string | TemplateRef<any>,
    transloco = true
  ): DialogRef {

    let dialog = this.dialogMessageRef(
      transloco ? this.translocoService.translate(title) : title,
      this.extractContent(content, transloco),
      this.onlyOk, 'auto', 'auto',
      (p) => p instanceof DialogCloseResult,
      Dialog_Type.info
    );
    if (typeof content == 'string')
      dialog.content.location.nativeElement.style.whiteSpace = 'pre-line';

    return dialog;
  }

  /**
   * @param title titolo finestra di dialogo
   * @param content messaggio di errore
   * @param transloco usare transloco? (Default: true)
   */
  public async baseSuccess(
    title: string,
    content: string | TemplateRef<any>,
    transloco = true,
    timeout: boolean = true
  ): Promise<DialogResult> {

    return new Promise((resolve, reject) => {
      let dialog = this.dialogMessageRef(
        transloco ? this.translocoService.translate(title) : title,
        this.extractContent(content, transloco),
        this.onlyOk, 'auto', 'auto',
        () => false,
        Dialog_Type.success
      );

      if (typeof content == 'string')
        dialog.content.location.nativeElement.style.whiteSpace = 'pre-line';

      dialog.result.subscribe(res => resolve(res))

      if (timeout)
        setTimeout(() => {
          dialog.close();
          resolve(null);
        }, 5000);

    });
  }

  public salvataggioOk(extraText: string = '') {
    return this.baseSuccess('SalvataggioAvvenutoConSuccesso', extraText);
  }

  private extractContent(content: string | TemplateRef<any>, isTransloco: boolean): string | TemplateRef<any> {
    if (typeof content == 'string')
      return isTransloco ? this.translocoService.translate(content) : content;
    else
      return content;
  }

}
